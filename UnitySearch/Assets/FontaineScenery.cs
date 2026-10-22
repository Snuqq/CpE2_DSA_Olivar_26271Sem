using System.Collections.Generic;
using UnityEngine;

// Original geometry inspired by the Opera Epiclese's arches, brasswork and warm stage.
public sealed class FontaineScenery : MonoBehaviour
{
    readonly List<Material> materials = new List<Material>();
    readonly List<Mesh> meshes = new List<Mesh>();
    readonly List<Transform> platforms = new List<Transform>();
    readonly List<Renderer> faces = new List<Renderer>(), rims = new List<Renderer>();
    MaterialPropertyBlock tint;
    Transform floor, backdrop, wall, cornice, leftWing, rightWing, stairs;
    Material brass, ivory, navy, wood, enamel, luminous, velvet, glass;
    Texture2D grain;
    Mesh cube, podiumOrnaments;
    Material[] podiumMaterials;
    Shader surfaceShader;
    readonly Dictionary<Vector4, Mesh> discCache = new Dictionary<Vector4, Mesh>();
    readonly Dictionary<Vector2, Mesh> ringCache = new Dictionary<Vector2, Mesh>();
    int previousLeft = int.MinValue, previousRight, previousProbe;
    bool previousFound;

    public void Initialize()
    {
        tint = new MaterialPropertyBlock();
        surfaceShader = Shader.Find("Standard");
        brass = Material("Antique brass", new Color(.72f, .51f, .25f), .45f);
        ivory = Material("Warm limestone", new Color(.83f, .77f, .61f), .08f);
        navy = Material("Midnight blue", new Color(.023f, .05f, .085f), .18f);
        wood = Material("Polished walnut", new Color(.29f, .14f, .065f), .08f);
        wood.SetFloat("_Glossiness", .58f);
        enamel = Material("Hydro enamel", new Color(.025f, .22f, .29f), .28f);
        luminous = Material("Warm window glass", new Color(.96f, .82f, .5f), .05f);
        luminous.SetColor("_EmissionColor", new Color(.56f, .34f, .12f));
        glass = Material("Azure rose glass", new Color(.16f, .39f, .46f), .1f);
        glass.SetColor("_EmissionColor", new Color(.055f, .15f, .19f));
        velvet = Material("Opera velvet", new Color(.19f, .035f, .05f), 0);
        cube = CubeMesh();
        grain = new Texture2D(128, 128, TextureFormat.RGB24, false);
        grain.name = "Walnut parquet"; grain.wrapMode = TextureWrapMode.Repeat;
        var pixels = new Color32[128 * 128];
        for (int y = 0; y < 128; y++) for (int x = 0; x < 128; x++)
        {
            bool seam = y % 16 == 0 || (x + (y / 16 % 2) * 32) % 64 == 0;
            float shade = seam ? .44f : .78f + Mathf.PerlinNoise(x * .045f, y * .7f) * .22f;
            pixels[y * 128 + x] = new Color(shade, shade, shade);
        }
        grain.SetPixels32(pixels); grain.Apply(false, true); wood.mainTexture = grain; wood.mainTextureScale = new Vector2(5, 5);
        floor = Group("Parquet apron", transform);
        Disc("Walnut stage", .5f, .45f, 96, wood, floor, -.59f);
        Disc("Midnight apron fascia", .496f, .18f, 96, navy, floor, -.52f);
        Ring("Outer brass inlay", .486f, .003f, brass, floor, -.354f);
        Ring("Inner brass inlay", .445f, .0014f, brass, floor, -.353f);
        for (int i = 0; i < 16; i++)
        {
            float a = i * Mathf.PI / 8;
            Vector3 p = new Vector3(Mathf.Cos(a) * .466f, -.35f, Mathf.Sin(a) * .466f);
            var inlay = Box("Apron diamond", p, new Vector3(.008f, .002f, .008f), brass, floor);
            inlay.transform.localRotation = Quaternion.Euler(0, 45, 0);
        }
        Combine(floor);
        stairs = Group("Front steps", transform);
        for (int i = 0; i < 3; i++)
        {
            Box("Walnut step", new Vector3(0, -.58f - i * .16f, -.35f - i * .42f), new Vector3(5.6f + i * .65f, .16f, .7f), wood, stairs);
            Box("Step nosing", new Vector3(0, -.493f - i * .16f, -.67f - i * .42f), new Vector3(5.6f + i * .65f, .025f, .055f), brass, stairs);
        }
        Combine(stairs);
        backdrop = Group("Opera Epiclese inspired facade", transform);
        wall = Box("Deep blue rear wall", new Vector3(0, 3.3f, .65f), new Vector3(18, 7, .4f), navy, backdrop).transform;
        cornice = Box("Gilded cornice", new Vector3(0, 6.55f, .15f), new Vector3(18, .18f, .75f), brass, backdrop).transform;
        var centre = Group("Oratrice and central proscenium", backdrop);
        BuildCentre(centre);
        Combine(centre);
        leftWing = BuildWing(-1); rightWing = BuildWing(1);
        for (int i = 0; i < 40; i++) BuildPlatform(i);
        TrimConstructionMeshes();
    }

    Material Material(string name, Color color, float metal)
    {
        var result = new Material(surfaceShader) { name = name, color = color };
        result.SetFloat("_Metallic", metal); result.SetFloat("_Glossiness", .38f);
        result.EnableKeyword("_EMISSION"); result.SetColor("_EmissionColor", color * .035f);
        materials.Add(result); return result;
    }
    Transform Group(string name, Transform parent)
    {
        var item = new GameObject(name).transform; item.SetParent(parent, false); return item;
    }
    Renderer Surface(string name, Mesh mesh, Material material, Transform parent)
    {
        var item = Group(name, parent);
        item.gameObject.AddComponent<MeshFilter>().sharedMesh = mesh;
        var renderer = item.gameObject.AddComponent<MeshRenderer>(); renderer.sharedMaterial = material;
        return renderer;
    }
    Renderer Box(string name, Vector3 position, Vector3 scale, Material material, Transform parent)
    {
        var renderer = Surface(name, cube, material, parent);
        renderer.transform.localPosition = position; renderer.transform.localScale = scale;
        return renderer;
    }
    Mesh CubeMesh()
    {
        var vertices = new List<Vector3>(24); var triangles = new List<int>(36);
        var uv = new List<Vector2>(24);
        Vector3[] normals = { Vector3.forward, Vector3.back, Vector3.right, Vector3.left, Vector3.up, Vector3.down };
        foreach (Vector3 normal in normals)
        {
            Vector3 tangent = Mathf.Abs(normal.y) > .5f ? Vector3.right : Vector3.up;
            Vector3 bitangent = Vector3.Cross(normal, tangent);
            int offset = vertices.Count;
            vertices.Add((normal - tangent - bitangent) * .5f);
            vertices.Add((normal + tangent - bitangent) * .5f);
            vertices.Add((normal + tangent + bitangent) * .5f);
            vertices.Add((normal - tangent + bitangent) * .5f);
            uv.Add(Vector2.zero); uv.Add(Vector2.right); uv.Add(Vector2.one); uv.Add(Vector2.up);
            triangles.AddRange(new[] { offset, offset + 1, offset + 2, offset, offset + 2, offset + 3 });
        }
        return Mesh("Shared bevel-detail block", vertices, triangles, uv);
    }
    Mesh Mesh(string name, List<Vector3> vertices, List<int> triangles, List<Vector2> uv = null)
    {
        var mesh = new Mesh { name = name };
        mesh.SetVertices(vertices); mesh.SetTriangles(triangles, 0);
        if (uv != null) mesh.SetUVs(0, uv);
        mesh.RecalculateNormals(); mesh.RecalculateBounds(); meshes.Add(mesh);
        return mesh;
    }
    // Combine within independently laid-out groups; moving a wing never invalidates its mesh.
    Renderer Combine(Transform root)
    {
        var renderers = root.GetComponentsInChildren<MeshRenderer>();
        var groups = new Dictionary<Material, List<CombineInstance>>();
        foreach (var renderer in renderers)
        {
            var filter = renderer.GetComponent<MeshFilter>();
            var material = renderer.sharedMaterial;
            List<CombineInstance> instances;
            if (!groups.TryGetValue(material, out instances))
            {
                instances = new List<CombineInstance>(); groups.Add(material, instances);
            }
            instances.Add(new CombineInstance { mesh = filter.sharedMesh, transform = root.worldToLocalMatrix * filter.transform.localToWorldMatrix });
        }
        var combined = new List<CombineInstance>(); var combinedMaterials = new List<Material>();
        foreach (var group in groups)
        {
            var submesh = new Mesh { name = root.name + " " + group.Key.name };
            submesh.CombineMeshes(group.Value.ToArray(), true, true);
            combined.Add(new CombineInstance { mesh = submesh, transform = Matrix4x4.identity });
            combinedMaterials.Add(group.Key);
        }
        var result = new Mesh { name = root.name + " combined", indexFormat = UnityEngine.Rendering.IndexFormat.UInt32 };
        result.CombineMeshes(combined.ToArray(), false, false); result.RecalculateBounds(); meshes.Add(result);
        foreach (var temporary in combined) Release(temporary.mesh);
        foreach (var renderer in renderers)
        {
            renderer.enabled = false;
            Release(renderer.gameObject);
        }
        var output = Surface(root.name + " surfaces", result, combinedMaterials[0], root);
        output.sharedMaterials = combinedMaterials.ToArray();
        return output;
    }
    void TrimConstructionMeshes()
    {
        var used = new HashSet<Mesh>();
        foreach (var renderer in GetComponentsInChildren<MeshRenderer>())
            if (renderer.enabled) used.Add(renderer.GetComponent<MeshFilter>().sharedMesh);
        for (int i = meshes.Count - 1; i >= 0; i--)
            if (!used.Contains(meshes[i])) { Release(meshes[i]); meshes.RemoveAt(i); }
        discCache.Clear(); ringCache.Clear();
    }
    Renderer Disc(string name, float radius, float height, int sides, Material material, Transform parent, float y)
    {
        var key = new Vector4(radius, height, sides, 0);
        Mesh cached;
        if (discCache.TryGetValue(key, out cached))
        {
            var reused = Surface(name, cached, material, parent); reused.transform.localPosition = Vector3.up * y; return reused;
        }
        // Bevelled sides catch the light instead of presenting a single flat cylinder.
        var vertices = new List<Vector3>(); var triangles = new List<int>(); var uv = new List<Vector2>();
        float[] levels = { -height * .5f, -height * .5f + Mathf.Min(.04f, height * .25f), height * .5f - Mathf.Min(.04f, height * .25f), height * .5f };
        for (int ring = 0; ring < 4; ring++) for (int i = 0; i < sides; i++)
        {
            float a = (i + .5f) * Mathf.PI * 2 / sides;
            float r = ring == 0 || ring == 3 ? radius * .94f : radius;
            vertices.Add(new Vector3(Mathf.Cos(a) * r, levels[ring], Mathf.Sin(a) * r));
            uv.Add(new Vector2(Mathf.Cos(a) * .5f + .5f, Mathf.Sin(a) * .5f + .5f));
        }
        for (int ring = 0; ring < 3; ring++) for (int i = 0; i < sides; i++)
        {
            int a = ring * sides + i, b = ring * sides + (i + 1) % sides;
            triangles.AddRange(new[] { a, a + sides, b, b, a + sides, b + sides });
        }
        vertices.Add(new Vector3(0, height * .5f, 0)); uv.Add(new Vector2(.5f, .5f));
        for (int i = 0; i < sides; i++) triangles.AddRange(new[] { sides * 4, sides * 3 + (i + 1) % sides, sides * 3 + i });
        var mesh = new Mesh { name = name }; mesh.SetVertices(vertices); mesh.SetTriangles(triangles, 0); mesh.SetUVs(0, uv); mesh.RecalculateNormals(); mesh.RecalculateBounds();
        meshes.Add(mesh); discCache.Add(key, mesh);
        var result = Surface(name, mesh, material, parent); result.transform.localPosition = Vector3.up * y; return result;
    }
    Renderer Ring(string name, float radius, float thickness, Material material, Transform parent, float y)
    {
        var key = new Vector2(radius, thickness);
        Mesh cached;
        if (ringCache.TryGetValue(key, out cached))
        {
            var reused = Surface(name, cached, material, parent); reused.transform.localPosition = Vector3.up * y; return reused;
        }
        const int sides = 96;
        var vertices = new Vector3[sides * 2]; var triangles = new int[sides * 6];
        for (int i = 0; i < sides; i++)
        {
            float a = i * Mathf.PI * 2 / sides;
            Vector3 direction = new Vector3(Mathf.Cos(a), 0, Mathf.Sin(a));
            vertices[i * 2] = direction * (radius - thickness); vertices[i * 2 + 1] = direction * radius;
            int n = (i + 1) % sides * 2, p = i * 2, t = i * 6;
            triangles[t] = p; triangles[t + 1] = n; triangles[t + 2] = p + 1;
            triangles[t + 3] = p + 1; triangles[t + 4] = n; triangles[t + 5] = n + 1;
        }
        var mesh = new Mesh { name = name, vertices = vertices, triangles = triangles }; mesh.RecalculateNormals(); mesh.RecalculateBounds();
        meshes.Add(mesh); ringCache.Add(key, mesh);
        var result = Surface(name, mesh, material, parent); result.transform.localPosition = Vector3.up * y; return result;
    }
    void Beam(string name, Vector3 a, Vector3 b, float width, Material material, Transform parent)
    {
        var beam = Box(name, (a + b) * .5f, new Vector3(width, (b - a).magnitude, width), material, parent);
        beam.transform.localRotation = Quaternion.FromToRotation(Vector3.up, b - a);
    }
    void Arch(float radius, float spring, float z, float width, Material material, Transform parent)
    {
        var vertices = new List<Vector3>(); var triangles = new List<int>();
        const int segments = 48;
        for (int i = 0; i <= segments; i++)
        {
            float angle = i * Mathf.PI / segments;
            for (int depth = 0; depth < 2; depth++)
            for (int edge = 0; edge < 2; edge++)
            {
                float r = radius + (edge - .5f) * width;
                vertices.Add(new Vector3(Mathf.Cos(angle) * r, spring + Mathf.Sin(angle) * r, z + depth * width));
            }
        }
        for (int i = 0; i < segments; i++)
        {
            int a = i * 4, b = a + 4;
            Quad(triangles, a, b, b + 1, a + 1);
            Quad(triangles, a + 3, b + 3, b + 2, a + 2);
            Quad(triangles, a + 1, b + 1, b + 3, a + 3);
            Quad(triangles, a + 2, b + 2, b, a);
        }
        Surface("Continuous arch moulding", Mesh("Continuous arch", vertices, triangles), material, parent);
    }
    static void Quad(List<int> triangles, int a, int b, int c, int d)
    {
        triangles.Add(a); triangles.Add(b); triangles.Add(c);
        triangles.Add(a); triangles.Add(c); triangles.Add(d);
    }
    void ArchedPane(string name, float halfWidth, float bottom, float spring, Material material, Transform parent)
    {
        var vertices = new List<Vector3> { new Vector3(-halfWidth, bottom, 0), new Vector3(halfWidth, bottom, 0) };
        for (int i = 0; i <= 24; i++)
        {
            float angle = i * Mathf.PI / 24;
            vertices.Add(new Vector3(Mathf.Cos(angle) * halfWidth, spring + Mathf.Sin(angle) * halfWidth, 0));
        }
        var triangles = new List<int>();
        for (int i = 1; i < vertices.Count - 1; i++)
        {
            triangles.Add(0); triangles.Add(i + 1); triangles.Add(i);
        }
        Surface(name, Mesh(name, vertices, triangles), material, parent);
    }
    void Window(float x, float halfWidth, float spring, Transform parent)
    {
        var window = Group("Tall luminous arched window", parent);
        window.localPosition = new Vector3(x, 0, .08f);
        ArchedPane("Amber window pane", halfWidth, .4f, spring, luminous, window);
        Arch(halfWidth + .14f, spring, -.08f, .24f, ivory, window);
        Arch(halfWidth, spring, -.12f, .055f, brass, window);
        for (int side = -1; side <= 1; side += 2)
        {
            Box("Window limestone reveal", new Vector3(side * (halfWidth + .14f), spring * .5f, .02f),
                new Vector3(.24f, spring + .08f, .2f), ivory, window);
            Box("Window brass edge", new Vector3(side * halfWidth, spring * .5f, -.1f),
                new Vector3(.055f, spring, .06f), brass, window);
        }
        Box("Window sill", new Vector3(0, .35f, -.05f), new Vector3(halfWidth * 2 + .6f, .16f, .42f), ivory, window);
        Box("Window transom", new Vector3(0, spring, -.1f), new Vector3(halfWidth * 2, .055f, .06f), brass, window);
        for (int bar = -1; bar <= 1; bar++)
        {
            float offset = bar * halfWidth * .5f;
            float top = spring + Mathf.Sqrt(halfWidth * halfWidth - offset * offset);
            Box("Slender window mullion", new Vector3(offset, (top + .4f) * .5f, -.1f),
                new Vector3(.035f, top - .4f, .06f), brass, window);
        }
        for (int band = 0; band < 3; band++)
            Box("Window glazing band", new Vector3(0, 1.1f + band * 1.08f, -.1f),
                new Vector3(halfWidth * 2, .025f, .04f), brass, window);
    }
    void CurvedBand(string name, float radius, float thickness, float height, float angle, Material material, Transform parent, float y)
    {
        var vertices = new List<Vector3>(); var triangles = new List<int>();
        const int segments = 36;
        for (int i = 0; i <= segments; i++)
        {
            float a = Mathf.Lerp(-angle, angle, i / (float)segments) * Mathf.Deg2Rad;
            for (int level = 0; level < 2; level++)
            for (int edge = 0; edge < 2; edge++)
            {
                float r = radius - edge * thickness;
                vertices.Add(new Vector3(Mathf.Sin(a) * r, y + (level - .5f) * height, -Mathf.Cos(a) * r));
            }
        }
        for (int i = 0; i < segments; i++)
        {
            int a = i * 4, b = a + 4;
            Quad(triangles, a + 2, b + 2, b, a);
            Quad(triangles, a + 1, b + 1, b + 3, a + 3);
            Quad(triangles, a + 3, b + 3, b + 2, a + 2);
            Quad(triangles, a, b, b + 1, a + 1);
        }
        Quad(triangles, 1, 3, 2, 0);
        int last = segments * 4;
        Quad(triangles, last + 2, last + 3, last + 1, last);
        Surface(name, Mesh(name, vertices, triangles), material, parent);
    }
    void Column(float x, Transform parent)
    {
        var column = Group("Fluted limestone column", parent); column.localPosition = Vector3.right * x;
        Disc("Column foot", .51f, .28f, 16, brass, column, -.2f);
        Disc("Limestone plinth", .42f, .4f, 16, ivory, column, .1f);
        Disc("Column shaft", .28f, 5.35f, 24, ivory, column, 2.93f);
        for (int i = 0; i < 8; i++)
        {
            float a = i * Mathf.PI / 4;
            Box("Fine brass flute", new Vector3(Mathf.Cos(a) * .28f, 2.93f, Mathf.Sin(a) * .28f), new Vector3(.025f, 5.2f, .025f), brass, column);
        }
        Disc("Capital", .44f, .2f, 16, brass, column, 5.65f);
        Box("Capital crown", new Vector3(0, 5.86f, 0), new Vector3(.95f, .2f, .8f), ivory, column);
    }
    void BuildCentre(Transform centre)
    {
        Column(-3.6f, centre); Column(3.6f, centre);
        Arch(3.6f, 3, -.12f, .22f, brass, centre);
        Arch(3.32f, 3, -.11f, .24f, ivory, centre);
        Arch(3.08f, 3, -.14f, .055f, brass, centre);
        var canopy = Group("Central vaulted recess", centre);
        canopy.localPosition = new Vector3(0, 0, .18f);
        ArchedPane("Deep teal apse", 3.1f, -.35f, 3, enamel, canopy);
        var dial = Group("Oratrice inspired rose mechanism", centre); dial.localPosition = new Vector3(0, 4.1f, -.18f); dial.localRotation = Quaternion.Euler(-90, 0, 0);
        Disc("Azure glass dial", 1.86f, .1f, 64, glass, dial, 0);
        Ring("Outer dial rim", 2.02f, .10f, brass, dial, .07f);
        Ring("Inner dial rim", 1.71f, .035f, ivory, dial, .08f);
        Ring("Mechanical inner wheel", 1.05f, .035f, brass, dial, .09f);
        for (int i = 0; i < 12; i++)
        {
            float a = i * Mathf.PI / 6;
            var tick = Box("Clockwork ray", new Vector3(Mathf.Cos(a) * 1.38f, .09f, Mathf.Sin(a) * 1.38f), new Vector3(.045f, .03f, .65f), brass, dial);
            tick.transform.localRotation = Quaternion.Euler(0, 90 - a * Mathf.Rad2Deg, 0);
            var petal = Group("Rose tracery petal", dial);
            petal.localPosition = new Vector3(Mathf.Cos(a) * 1.26f, .105f, Mathf.Sin(a) * 1.26f);
            petal.localRotation = Quaternion.Euler(0, -a * Mathf.Rad2Deg, 0);
            petal.localScale = new Vector3(1.55f, 1, 1);
            Ring("Petal brass outline", .28f, .018f, brass, petal, 0);
        }
        Ring("Central seal", .67f, .045f, brass, dial, .11f);
        var diamond = Box("Hydro lozenge", new Vector3(0, .12f, 0), new Vector3(.66f, .04f, .66f), luminous, dial);
        diamond.transform.localRotation = Quaternion.Euler(0, 45, 0);
        for (int side = -1; side <= 1; side += 2)
        {
            Window(side * 4.7f, .59f, 4.55f, centre);
            for (int i = 0; i < 5; i++)
            {
                float height = 2.6f + i * .25f;
                var pipe = Disc("Brass organ pipe", .055f, height, 12, brass, centre, .45f + height * .5f);
                pipe.transform.localPosition += new Vector3(side * (2.12f + i * .18f), 0, -.13f);
            }
        }
        BuildScales(centre);
        Box("Rear dais", new Vector3(0, -.1f, -.35f), new Vector3(6.2f, .45f, 1.3f), navy, centre);
        Box("Dais brass rail", new Vector3(0, .16f, -.99f), new Vector3(6.2f, .05f, .07f), brass, centre);
        Box("Burgundy dais runner", new Vector3(0, .135f, -.65f), new Vector3(1.5f, .025f, 1.5f), velvet, centre);
    }
    void BuildScales(Transform parent)
    {
        var scales = Group("Ceremonial balance ornament", parent);
        scales.localPosition = new Vector3(0, 0, -.58f);
        Disc("Scales plinth", .48f, .23f, 12, brass, scales, .28f);
        Disc("Scales stem", .06f, 3.35f, 12, brass, scales, 2.04f);
        Beam("Balance beam left", new Vector3(-1.1f, 3.08f, 0), new Vector3(0, 3.3f, 0), .055f, brass, scales);
        Beam("Balance beam right", new Vector3(0, 3.3f, 0), new Vector3(1.1f, 3.08f, 0), .055f, brass, scales);
        for (int side = -1; side <= 1; side += 2)
        {
            for (int chain = -1; chain <= 1; chain += 2)
                Beam("Balance suspension", new Vector3(side * 1.08f, 3.08f, 0),
                    new Vector3(side * 1.08f + chain * .31f, 2.38f, -.02f), .018f, brass, scales);
            var pan = Disc("Balance pan", .36f, .09f, 24, brass, scales, 2.34f);
            pan.transform.localPosition += Vector3.right * side * 1.08f;
            var inset = Disc("Balance azure inset", .29f, .015f, 24, glass, scales, 2.39f);
            inset.transform.localPosition += Vector3.right * side * 1.08f;
        }
        var crest = Box("Balance finial", new Vector3(0, 3.85f, 0), new Vector3(.2f, .2f, .06f), luminous, scales);
        crest.transform.localRotation = Quaternion.Euler(0, 0, 45);
    }
    Transform BuildWing(int side)
    {
        var wing = Group(side < 0 ? "Left opera gallery" : "Right opera gallery", backdrop);
        Column(0, wing);
        Window(side * 1.28f, .86f, 4.55f, wing);
        for (int i = 0; i < 6; i++)
        {
            float y = 4.95f - Mathf.Sin(i / 5f * Mathf.PI) * .18f;
            var fold = Disc("Velvet curtain swag", .18f, .95f, 16, velvet, wing, y);
            fold.transform.localPosition += new Vector3(side * (.47f + i * .29f), 0, -.16f);
        }
        for (int level = 0; level < 2; level++)
        {
            float y = 1.6f + level * 2.15f;
            var balcony = Group("Curved opera box", wing);
            balcony.localPosition = new Vector3(side * 1.1f, 0, .65f);
            CurvedBand("Ivory balcony fascia", 1.7f, .16f, .72f, 64, ivory, balcony, y);
            CurvedBand("Azure inset balcony band", 1.72f, .025f, .43f, 64, navy, balcony, y);
            CurvedBand("Gilded gallery crown", 1.77f, .2f, .08f, 64, brass, balcony, y + .39f);
            CurvedBand("Gilded gallery sill", 1.76f, .18f, .08f, 64, brass, balcony, y - .39f);
            CurvedBand("Lower moulding", 1.7f, .17f, .06f, 64, brass, balcony, y - .5f);
            for (int bar = 0; bar < 9; bar++)
            {
                float a = Mathf.Lerp(-58, 58, bar / 8f) * Mathf.Deg2Rad;
                Vector3 position = new Vector3(Mathf.Sin(a) * 1.75f, y, -Mathf.Cos(a) * 1.75f);
                var jewel = Box("Balcony gilded lozenge", position, new Vector3(.13f, .25f, .035f), brass, balcony);
                jewel.transform.localRotation = Quaternion.Euler(0, -a * Mathf.Rad2Deg, 45);
                Box("Balcony brass stile", position, new Vector3(.025f, .64f, .04f), brass, balcony);
            }
        }
        Combine(wing);
        return wing;
    }
    void BuildPlatform(int index)
    {
        var root = Group("Index " + index + " enamel podium", transform); platforms.Add(root);
        if (podiumOrnaments == null)
        {
            var detail = Group("Podium fixed ornament", root);
            Disc("Brass octagonal foot", .87f, .10f, 8, brass, detail, -.28f);
            Disc("Navy pedestal", .82f, .19f, 8, navy, detail, -.17f);
            Disc("Ivory foot bevel", .835f, .035f, 8, ivory, detail, -.225f);
            Ring("Fine inner inlay", .53f, .012f, brass, detail, .002f);
            Ring("Fine outer inlay", .66f, .006f, brass, detail, .003f);
            for (int i = 0; i < 8; i++)
            {
                float a = i * Mathf.PI * .25f;
                var jewel = Box("Compass inlay", new Vector3(Mathf.Cos(a) * .6f, .004f, Mathf.Sin(a) * .6f),
                    new Vector3(.06f, .006f, .06f), brass, detail);
                jewel.transform.localRotation = Quaternion.Euler(0, 45, 0);
                var rib = Box("Pedestal gilded flute", new Vector3(Mathf.Cos(a) * .795f, -.17f, Mathf.Sin(a) * .795f),
                    new Vector3(.03f, .12f, .03f), brass, detail);
                rib.transform.localRotation = Quaternion.Euler(0, -a * Mathf.Rad2Deg, 0);
            }
            var ornament = Combine(detail);
            podiumOrnaments = ornament.GetComponent<MeshFilter>().sharedMesh;
            podiumMaterials = ornament.sharedMaterials;
        }
        else
        {
            var ornament = Surface("Podium fixed ornament", podiumOrnaments, podiumMaterials[0], root);
            ornament.sharedMaterials = podiumMaterials;
        }
        rims.Add(Disc("Bevelled brass rim", .82f, .08f, 8, brass, root, -.055f));
        faces.Add(Disc("Enamel surface", .72f, .025f, 8, enamel, root, -.0125f));
    }
    public void Layout(int columns, int rows, int count)
    {
        float width = Mathf.Max(16, (columns - 1) * 2.4f + 8), depth = (rows - 1) * 3.2f + 8;
        floor.localScale = new Vector3(width, 1, depth);
        stairs.localPosition = new Vector3(0, 0, -depth * .48f);
        backdrop.localPosition = new Vector3(0, 0, (rows - 1) * 1.6f + 3.5f);
        wall.localScale = new Vector3(width + 3, 7, .4f); cornice.localScale = new Vector3(width + 3, .18f, .75f);
        leftWing.localPosition = new Vector3(-width * .5f + 1, 0, -.15f);
        rightWing.localPosition = new Vector3(width * .5f - 1, 0, -.15f);
        for (int i = 0; i < platforms.Count; i++)
        {
            platforms[i].gameObject.SetActive(i < count);
            platforms[i].localPosition = new Vector3((i % columns - (columns - 1) * .5f) * 2.4f, 0, ((rows - 1) * .5f - i / columns) * 3.2f);
        }
    }
    public Vector3 Position(int index) { return platforms[index].position; }
    void Colorize(Renderer renderer, Color color, float emission)
    {
        tint.Clear(); tint.SetColor("_Color", color); tint.SetColor("_EmissionColor", color * emission); renderer.SetPropertyBlock(tint);
    }
    public void Highlight(int left, int right, int probe, bool found)
    {
        if (left == previousLeft && right == previousRight && probe == previousProbe && found == previousFound) return;
        previousLeft = left; previousRight = right; previousProbe = probe; previousFound = found;
        for (int i = 0; i < faces.Count; i++)
        {
            bool selected = i == probe, candidate = i >= left && i <= right;
            Color face = selected ? (found ? new Color(.08f, .60f, .42f) : new Color(.66f, .38f, .08f))
                : candidate ? new Color(.035f, .24f, .33f) : new Color(.065f, .09f, .12f);
            Color rim = selected ? (found ? new Color(.36f, .96f, .72f) : new Color(1, .79f, .33f)) : new Color(.58f, .39f, .17f);
            Colorize(faces[i], face, selected ? .22f : .025f); Colorize(rims[i], rim, selected ? .38f : .04f);
        }
    }
    void Release(Object item) { if (Application.isPlaying) Destroy(item); else DestroyImmediate(item); }
    void OnDestroy()
    {
        foreach (var material in materials) if (material != null) Release(material);
        foreach (var mesh in meshes) if (mesh != null) Release(mesh);
        if (grain != null) Release(grain);
    }
}
