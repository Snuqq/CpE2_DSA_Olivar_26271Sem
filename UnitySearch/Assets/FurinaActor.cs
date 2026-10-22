using System;
using System.Collections.Generic;
using UnityEngine;
using CpE2_DSA_Olivar_26271Sem;

public sealed class FurinaActor : MonoBehaviour
{
    struct Joint
    {
        public Transform bone;
        public Vector3 position, previousPosition;
        public Quaternion rotation, previousRotation;
        public float secondaryWeight;
    }
    struct Limb
    {
        public Transform upper, lower, end;
        public Limb(Transform upper, Transform lower, Transform end) { this.upper = upper; this.lower = lower; this.end = end; }
    }
    Joint[] joints;
    readonly List<Material> materials = new List<Material>();
    Transform model, hips, chest, head, shoulderL, shoulderR;
    Limb armL, armR, legL, legR;
    SkinnedMeshRenderer[] renderers;
    Vector3 basePosition, leftFoot, rightFoot, hipRest, leftAnchor, rightAnchor;
    Quaternion leftFootRotation, rightFootRotation;
    bool blending;
    float walkingSupportDrop;
    float leftWalkingYaw, rightWalkingYaw;
    float walkingCycle;
    SearchMotionSettings settings;
    // Descend, cross the floor, climb, then gather the trailing foot.
    static readonly float[] WalkContacts = { .35f, .7f, 1f, 1f };
    public bool FeetPlanted { get; private set; }
    public bool LeftPlanted { get; private set; }
    public bool RightPlanted { get; private set; }
    public bool Walking { get; private set; }
    public float PlantedDrift { get; private set; }
    public float MaxPlantedDrift { get; private set; }
    public float PelvisDrop { get; private set; }
    public float SupportHeight { get; private set; }
    public Bounds WorldBounds
    {
        get { var bounds = renderers[0].bounds; for (int i = 1; i < renderers.Length; i++) bounds.Encapsulate(renderers[i].bounds); return bounds; }
    }
    public void ResetPose() { blending = false; MaxPlantedDrift = PlantedDrift = 0; }
    public void BeginMotion()
    {
        blending = true;
        for (int i = 0; i < joints.Length; i++)
        {
            joints[i].previousRotation = joints[i].bone.localRotation;
            joints[i].previousPosition = joints[i].bone.localPosition;
        }
    }
    public void Initialize()
    {
        var source = Resources.Load<GameObject>("FurinaRig/FurinaRig");
        var shader = Resources.Load<Shader>("FurinaRig/FurinaToon");
        if (source == null || shader == null) throw new InvalidOperationException("The rigged Furina asset or shader is missing.");
        model = Instantiate(source, transform).transform;
        renderers = model.GetComponentsInChildren<SkinnedMeshRenderer>();
        if (renderers.Length == 0) throw new InvalidOperationException("Furina must have a skinned mesh.");
        var mappedMaterials = new Dictionary<string, Material>();
        Bounds bounds = renderers[0].bounds;
        foreach (var renderer in renderers)
        {
            bounds.Encapsulate(renderer.bounds);
            var mapped = renderer.sharedMaterials;
            for (int i = 0; i < mapped.Length; i++)
            {
                string name = mapped[i].name.Replace(" (Instance)", "");
                Material material;
                if (!mappedMaterials.TryGetValue(name, out material))
                {
                    var texture = Resources.Load<Texture2D>("FurinaRig/" + name);
                    if (texture == null) throw new InvalidOperationException("Missing character texture: " + name);
                    material = new Material(shader) { mainTexture = texture };
                    materials.Add(material); mappedMaterials.Add(name, material);
                }
                mapped[i] = material;
            }
            renderer.sharedMaterials = mapped;
            renderer.updateWhenOffscreen = true;
        }
        float scale = 2.7f / bounds.size.y;
        model.localScale *= scale;
        model.localPosition -= Vector3.up * bounds.min.y * scale;
        var transforms = model.GetComponentsInChildren<Transform>();
        var bones = new Dictionary<string, Transform>();
        foreach (var bone in transforms) bones.Add(bone.name, bone);
        hips = bones["Hips_02"]; chest = bones["Chest_04"]; head = bones["Head_06"];
        shoulderL = bones["Left shoulder_058"]; shoulderR = bones["Right shoulder_084"];
        armL = new Limb(bones["Left arm_059"], bones["Left elbow_061"], bones["Left wrist_063"]);
        armR = new Limb(bones["Right arm_085"], bones["Right elbow_087"], bones["Right wrist_089"]);
        legL = new Limb(bones["Left leg_0120"], bones["Left knee_0121"], bones["Left ankle_0122"]);
        legR = new Limb(bones["Right leg_0124"], bones["Right knee_0125"], bones["Right ankle_0126"]);
        Vector3 face = Vector3.ProjectOnPlane((bones["Eye_L_08"].position + bones["Eye_R_07"].position) * .5f - head.position, Vector3.up);
        model.rotation = Quaternion.AngleAxis(Vector3.SignedAngle(face, Vector3.back, Vector3.up), Vector3.up) * model.rotation;
        joints = new Joint[transforms.Length];
        for (int i = 0; i < transforms.Length; i++)
        {
            Transform bone = transforms[i];
            float weight = 0;
            if (IsSecondary(bone.name))
            {
                int depth = 1;
                for (var parent = bone.parent; parent != null && IsSecondary(parent.name); parent = parent.parent) depth++;
                weight = 1f / (depth * depth);
            }
            joints[i] = new Joint { bone = bone, rotation = bone.localRotation, position = bone.localPosition, secondaryWeight = weight };
        }
        hipRest = hips.localPosition; basePosition = model.localPosition;
        leftFoot = transform.InverseTransformPoint(legL.end.position); rightFoot = transform.InverseTransformPoint(legR.end.position);
        leftFootRotation = Quaternion.Inverse(transform.rotation) * legL.end.rotation;
        rightFootRotation = Quaternion.Inverse(transform.rotation) * legR.end.rotation;
    }
    static bool IsSecondary(string name)
    {
        return name.StartsWith("HB1_", StringComparison.Ordinal) || name.StartsWith("HB2_", StringComparison.Ordinal)
            || name.StartsWith("HB3_", StringComparison.Ordinal) || name.StartsWith("HB4_", StringComparison.Ordinal) || name.StartsWith("Sleeve_", StringComparison.Ordinal);
    }
    static float Ease(float t) { return Mathf.SmoothStep(0, 1, t); }
    static float Smoother(float t) { t = Mathf.Clamp01(t); return t * t * t * (t * (t * 6 - 15) + 10); }
    static void Aim(Transform bone, Transform end, Vector3 direction)
    {
        bone.rotation = Quaternion.FromToRotation(end.position - bone.position, direction) * bone.rotation;
    }
    static void Arm(Limb arm, Vector3 upper, Vector3 lower)
    {
        Aim(arm.upper, arm.lower, upper); Aim(arm.lower, arm.end, lower);
    }
    static void Rotate(Transform bone, Vector3 axis, float degrees) { bone.rotation = Quaternion.AngleAxis(degrees, axis) * bone.rotation; }
    static void Leg(Limb leg, Vector3 target, Quaternion footRotation, Vector3 forward)
    {
        Vector3 origin = leg.upper.position;
        float a = Vector3.Distance(origin, leg.lower.position), b = Vector3.Distance(leg.lower.position, leg.end.position);
        Vector3 direction = (target - origin).normalized;
        float distance = Mathf.Clamp(Vector3.Distance(origin, target), Mathf.Abs(a - b) + .00001f, a + b - .00001f);
        float along = (a * a + distance * distance - b * b) / (2 * distance);
        Vector3 pole = Vector3.ProjectOnPlane(forward, direction).normalized;
        if (pole.sqrMagnitude < .01f) pole = Vector3.ProjectOnPlane(Vector3.right, direction).normalized;
        Vector3 bend = origin + direction * along + pole * Mathf.Sqrt(Mathf.Max(0, a * a - along * along));
        leg.upper.rotation = Quaternion.FromToRotation(leg.lower.position - origin, bend - origin) * leg.upper.rotation;
        leg.lower.rotation = Quaternion.FromToRotation(leg.end.position - leg.lower.position, target - leg.lower.position) * leg.lower.rotation;
        leg.end.rotation = footRotation;
    }
    void Arms(float presentation)
    {
        Arm(armL, new Vector3(.22f, -1, -.12f), new Vector3(.12f, -1, -.25f));
        Arm(armR, Vector3.Lerp(new Vector3(-.22f, -1, -.12f), new Vector3(-.8f, -.4f, -.35f), presentation),
            Vector3.Lerp(new Vector3(-.12f, -1, -.25f), new Vector3(-.4f, -.3f, -.7f), presentation));
    }
    float Ground(Vector3 foot, Vector3 source, Vector3 destination)
    {
        if (Vector3.ProjectOnPlane(foot - source, Vector3.up).magnitude <= settings.platformSupportRadius) return source.y;
        if (Vector3.ProjectOnPlane(foot - destination, Vector3.up).magnitude <= settings.platformSupportRadius) return destination.y;
        return Mathf.Min(source.y, destination.y) - settings.floorDepth;
    }
    void Walk(float progress, Vector3 source, Vector3 destination, out Vector3 left, out Vector3 right)
    {
        left = source + leftFoot; right = source + rightFoot;
        leftWalkingYaw = rightWalkingYaw = 0;
        float facing = Mathf.Clamp(Vector3.SignedAngle(Vector3.back, destination - source, Vector3.up),
            -settings.walkingHipTurnDegrees, settings.walkingHipTurnDegrees);
        float clock = Mathf.Clamp01(progress), start = 0, length = 0;
        int contact = 0;
        float totalStride = WalkContacts[WalkContacts.Length - 1] + WalkContacts[WalkContacts.Length - 2];
        // Give the longer middle swings more time instead of snapping each foot through an equal beat.
        for (; contact < WalkContacts.Length; contact++)
        {
            float stride = WalkContacts[contact] - (contact < 2 ? 0 : WalkContacts[contact - 2]);
            length = Mathf.Lerp(1f / WalkContacts.Length, stride / totalStride, settings.walkingStrideTimingWeight);
            if (clock <= start + length || contact == WalkContacts.Length - 1) break;
            start += length;
        }
        float swingPhase = Mathf.Clamp01((clock - start) / length);
        walkingCycle = contact + swingPhase;
        float swing = Smoother(swingPhase);
        bool leftLeads = Vector3.Dot(leftFoot - rightFoot, destination - source) >= 0;
        bool leftSwings = (contact % 2 == 0) == leftLeads;
        LeftPlanted = !leftSwings || swing <= 0 || swing >= 1;
        RightPlanted = leftSwings || swing <= 0 || swing >= 1;
        float supportL = source.y, supportR = source.y;
        for (int i = 0; i <= contact; i++)
        {
            bool isLeft = (i % 2 == 0) == leftLeads;
            Vector3 offset = isLeft ? leftFoot : rightFoot;
            Vector3 end = Vector3.Lerp(source, destination, WalkContacts[i]) + offset;
            float ground = Ground(end - Vector3.up * offset.y, source, destination);
            end.y = ground + offset.y;
            float amount = i < contact ? 1 : swing;
            float phase = i < contact ? 1 : swingPhase;
            float landingYaw = WalkContacts[i] < 1 ? facing : 0;
            if (isLeft)
            {
                left = SwingFoot(left, end, amount, phase); supportL = Mathf.Lerp(supportL, ground, amount);
                leftWalkingYaw = Mathf.Lerp(leftWalkingYaw, landingYaw, amount);
            }
            else
            {
                right = SwingFoot(right, end, amount, phase); supportR = Mathf.Lerp(supportR, ground, amount);
                rightWalkingYaw = Mathf.Lerp(rightWalkingYaw, landingYaw, amount);
            }
        }
        leftAnchor = left; rightAnchor = right;
        float lift = Mathf.Sin(swingPhase * Mathf.PI) * settings.walkingFootLift;
        if (!LeftPlanted) left.y += lift;
        if (!RightPlanted) right.y += lift;
        SupportHeight = (supportL + supportR) * .5f;
        walkingSupportDrop = Mathf.Abs(supportL - supportR) * .5f;
        // Share the reach between both legs while preserving smooth source/destination spacing.
        Vector3 root = Vector3.Lerp(source, destination, Smoother(progress));
        Vector3 centered = (left + right - leftFoot - rightFoot) * .5f;
        float centering = Mathf.Sin(progress * Mathf.PI);
        root = Vector3.Lerp(root, centered, settings.walkingSupportCentering * centering * centering);
        root.y = SupportHeight; transform.position = root;
    }
    static Vector3 SwingFoot(Vector3 start, Vector3 end, float progress, float phase)
    {
        var result = Vector3.Lerp(start, end, progress);
        // Clear the raised edge before descending, and rise before reaching the next podium.
        float vertical = end.y < start.y ? Ease((phase - .7f) / .3f)
            : end.y > start.y ? Ease(phase / .3f) : progress;
        result.y = Mathf.Lerp(start.y, end.y, vertical);
        return result;
    }
    public void Pose(SearchMotionPlan plan, double elapsed, Vector3 source, Vector3 destination, bool terminal, bool found, SearchMotionSettings motion)
    {
        settings = motion;
        for (int i = 0; i < joints.Length; i++) { joints[i].bone.localRotation = joints[i].rotation; joints[i].bone.localPosition = joints[i].position; }
        hips.localPosition = hipRest; model.localPosition = basePosition; transform.rotation = Quaternion.identity;
        SearchMotionPhase phase = plan == null ? SearchMotionPhase.Choose : plan.PhaseAt(elapsed);
        float travel = plan == null || terminal ? 0 : plan.ProgressOf(SearchMotionPhase.Travel, elapsed);
        Vector3 direction = Vector3.ProjectOnPlane(destination - source, Vector3.up).normalized;
        float distance = Vector3.Distance(source, destination);
        bool travelling = !terminal && plan != null && phase == SearchMotionPhase.Travel && distance > Mathf.Epsilon;
        bool walkMove = distance > Mathf.Epsilon && distance <= motion.maximumWalkDistance && Mathf.Abs(destination.y - source.y) < .001f;
        Walking = travelling && walkMove;
        bool flight = travelling && !walkMove;
        Vector3 support = terminal ? source : Vector3.Lerp(source, destination, Smoother(travel));
        transform.position = support; SupportHeight = support.y;
        Vector3 origin = terminal || travel < 1 ? source : destination;
        leftAnchor = origin + leftFoot; rightAnchor = origin + rightFoot;
        Vector3 leftTarget = leftAnchor, rightTarget = rightAnchor;
        LeftPlanted = RightPlanted = !flight;
        if (Walking) Walk(travel, source, destination, out leftTarget, out rightTarget);
        if (flight)
        {
            float hop = Mathf.Clamp(distance * motion.heightPerMetre, motion.minimumHopHeight, motion.maximumHopHeight);
            transform.position += Vector3.up * (4 * hop * travel * (1 - travel));
            float tuck = Mathf.Sin(travel * Mathf.PI);
            leftTarget = transform.position + leftFoot + Vector3.up * (motion.leadingKneeLift * tuck) + direction * (.18f * tuck);
            rightTarget = transform.position + rightFoot + Vector3.up * (motion.trailingKneeLift * tuck) - direction * (.24f * tuck);
        }
        float compression = 0;
        if (plan != null && !terminal)
        {
            if (phase == SearchMotionPhase.Anticipate) compression = Ease(plan.ProgressOf(phase, elapsed));
            if (phase == SearchMotionPhase.Takeoff) compression = 1 - Ease(plan.ProgressOf(phase, elapsed));
            if (phase == SearchMotionPhase.Land) compression = Ease(plan.ProgressOf(phase, elapsed));
            if (phase == SearchMotionPhase.Settle) compression = 1 - Ease(plan.ProgressOf(phase, elapsed));
        }
        float envelope = Mathf.Sin(travel * Mathf.PI);
        float stride = Mathf.Sin((Walking ? walkingCycle : travel * WalkContacts.Length) * Mathf.PI);
        PelvisDrop = compression * (walkMove ? motion.walkingCompressionDepth : motion.compressionDepth)
            + (Walking ? motion.walkingPelvisDrop * envelope + walkingSupportDrop : 0);
        hips.position += direction * (motion.launchWeightShift * compression) - Vector3.up * PelvisDrop;
        float recover = plan == null ? 0 : 1 - Ease(plan.ProgressOf(SearchMotionPhase.Settle, elapsed));
        float lead = terminal || plan == null ? 0 : Ease(plan.ProgressOf(SearchMotionPhase.Anticipate, elapsed)) * recover;
        float gaze = terminal || plan == null || distance < Mathf.Epsilon ? 0 : Ease(plan.ProgressOf(SearchMotionPhase.Choose, elapsed)) * recover;
        float inspection = plan == null ? 0 : terminal ? 1 : Ease(plan.ProgressOf(SearchMotionPhase.Compare, elapsed));
        Arms(inspection);
        float yaw = Vector3.SignedAngle(Vector3.back, direction, Vector3.up);
        float side = direction.x >= 0 ? 1 : -1;
        float bodyYaw = walkMove ? Mathf.Clamp(yaw, -motion.walkingHipTurnDegrees, motion.walkingHipTurnDegrees) * lead
            : -side * motion.hipCounterturnDegrees * lead * envelope;
        Rotate(hips, Vector3.up, bodyYaw);
        Rotate(chest, Vector3.up, Mathf.Clamp(yaw, -motion.chestLeadDegrees, motion.chestLeadDegrees) * lead - bodyYaw);
        Rotate(chest, Vector3.Cross(Vector3.up, direction), motion.travelLeanDegrees * lead * (1 - .55f * envelope));
        Rotate(head, Vector3.up, Mathf.Clamp(yaw, -motion.headLeadDegrees, motion.headLeadDegrees) * gaze
            - Mathf.Clamp(yaw, -motion.chestLeadDegrees, motion.chestLeadDegrees) * lead);
        if (lead > 0)
        {
            float swing = walkMove ? stride * envelope : Mathf.Sin(travel * Mathf.PI * 2) * envelope;
            float reach = walkMove ? .28f * swing : .55f * envelope - .25f * compression;
            Arm(armL, Vector3.Lerp(new Vector3(.22f, -1, -.12f), new Vector3(.35f, -.65f, -.12f) + direction * reach, lead),
                Vector3.Lerp(new Vector3(.12f, -1, -.25f), new Vector3(.2f, -.65f + .4f * envelope, -.5f) + direction * (.25f * swing), lead));
            Arm(armR, Vector3.Lerp(new Vector3(-.22f, -1, -.12f), new Vector3(-.4f, -.8f, .1f) - direction * reach, lead),
                Vector3.Lerp(new Vector3(-.12f, -1, -.25f), new Vector3(-.2f, -.7f, -.35f) - direction * (.3f * swing), lead));
            if (Walking) Rotate(chest, Vector3.forward, motion.walkingSwayDegrees * stride * envelope);
        }
        if (terminal && plan != null) Reaction(plan, elapsed, found, motion);
        else Rotate(head, Vector3.right, -8 * inspection);

        double delayed = elapsed - motion.secondaryRecoveryDelay;
        double release = plan == null ? 0 : plan.StartOf(terminal ? SearchMotionPhase.ReactionSettle : SearchMotionPhase.Settle);
        float drag = plan == null ? 0 : Ease(plan.ProgressOf(terminal ? SearchMotionPhase.Register : SearchMotionPhase.Anticipate, delayed));
        float recovery = Ease((float)((delayed - release) / motion.secondarySettleSeconds));
        float flutter = delayed < release ? 0 : Mathf.Sin((float)(delayed - release) * Mathf.PI * 2 / motion.secondaryOscillationSeconds);
        drag *= (1 - recovery) * (1 + .35f * flutter);
        for (int i = 0; i < joints.Length; i++)
        {
            float angle = drag * motion.secondaryDragDegrees * joints[i].secondaryWeight;
            if (angle != 0) joints[i].bone.localRotation *= Quaternion.Euler(angle, 0, -direction.x * angle);
        }
        float blend = Ease((float)elapsed / motion.poseBlendSeconds);
        if (plan != null && blending && blend < 1)
            for (int i = 0; i < joints.Length; i++)
            {
                joints[i].bone.localRotation = Quaternion.Slerp(joints[i].previousRotation, joints[i].bone.localRotation, blend);
                joints[i].bone.localPosition = Vector3.Lerp(joints[i].previousPosition, joints[i].bone.localPosition, blend);
            }
        // Contact IK runs after every other pose operation, including transitions and secondary motion.
        if (walkMove && !terminal)
        {
            float reachDrop = Mathf.Max(ReachDrop(legL, leftTarget, motion.kneeReachMargin * envelope),
                ReachDrop(legR, rightTarget, motion.kneeReachMargin * envelope));
            hips.position -= Vector3.up * reachDrop; PelvisDrop += reachDrop;
        }
        Quaternion leftRotation = leftFootRotation, rightRotation = rightFootRotation;
        if (Walking)
        {
            leftRotation = Quaternion.AngleAxis(leftWalkingYaw, Vector3.up) * leftRotation;
            rightRotation = Quaternion.AngleAxis(rightWalkingYaw, Vector3.up) * rightRotation;
        }
        if (flight)
        {
            leftRotation = Quaternion.AngleAxis(-motion.airborneToePointDegrees * envelope, Vector3.right) * leftRotation;
            rightRotation = Quaternion.AngleAxis(-motion.airborneToePointDegrees * envelope, Vector3.right) * rightRotation;
        }
        Vector3 kneeForward = Quaternion.AngleAxis(bodyYaw, Vector3.up) * Vector3.back;
        Leg(legL, leftTarget, leftRotation, kneeForward); Leg(legR, rightTarget, rightRotation, kneeForward);
        FeetPlanted = LeftPlanted && RightPlanted;
        PlantedDrift = Mathf.Max(LeftPlanted ? Vector3.Distance(legL.end.position, leftAnchor) : 0,
            RightPlanted ? Vector3.Distance(legR.end.position, rightAnchor) : 0);
        MaxPlantedDrift = Mathf.Max(MaxPlantedDrift, PlantedDrift);
    }
    static float ReachDrop(Limb leg, Vector3 target, float margin)
    {
        float reach = Vector3.Distance(leg.upper.position, leg.lower.position)
            + Vector3.Distance(leg.lower.position, leg.end.position) - margin;
        Vector3 delta = leg.upper.position - target;
        float verticalReach = Mathf.Sqrt(Mathf.Max(0, reach * reach - delta.x * delta.x - delta.z * delta.z));
        return Mathf.Max(0, delta.y - verticalReach);
    }
    void Reaction(SearchMotionPlan plan, double elapsed, bool found, SearchMotionSettings motion)
    {
        float look = Ease(plan.ProgressOf(SearchMotionPhase.Look, elapsed));
        float register = Ease(plan.ProgressOf(SearchMotionPhase.Register, elapsed));
        float react = Smoother(plan.ProgressOf(SearchMotionPhase.React, elapsed));
        float settle = Ease(plan.ProgressOf(SearchMotionPhase.ReactionSettle, elapsed));
        float strength = register * (1 - .2f * settle);
        float flourish = Mathf.Sin(react * Mathf.PI);
        PelvisDrop = motion.reactionHipDrop * strength;
        hips.position += Vector3.right * (motion.reactionWeightShift * strength) - Vector3.up * PelvisDrop;
        Arms(found ? 1 : 1 - register);
        if (found)
        {
            Arm(armR, Vector3.Lerp(new Vector3(-.8f, -.4f, -.35f), new Vector3(-1, -.22f, -.3f), react) + Vector3.up * (.35f * flourish),
                Vector3.Lerp(new Vector3(-.4f, -.3f, -.7f), new Vector3(-.85f, -.1f, -.55f), react) + Vector3.back * (.35f * flourish));
            Aim(armL.lower, armL.end, Vector3.Lerp(new Vector3(.12f, -1, -.25f), new Vector3(-.3f, .8f, -.4f), react));
            Rotate(chest, Vector3.up, -14 * strength); Rotate(chest, Vector3.forward, -6 * strength);
            Rotate(head, Vector3.right, -12 * look + 16 * react);
            Rotate(head, Vector3.up, -12 * react); Rotate(head, Vector3.forward, 7 * react);
        }
        else
        {
            Rotate(chest, Vector3.right, -14 * strength); Rotate(chest, Vector3.forward, 8 * strength);
            shoulderL.position -= Vector3.up * (motion.reactionHipDrop * strength);
            shoulderR.position -= Vector3.up * (motion.reactionHipDrop * strength * .7f);
            float shake = Mathf.Sin(react * Mathf.PI * 2) * Mathf.Sin(react * Mathf.PI);
            Rotate(head, Vector3.right, -14 * look); Rotate(head, Vector3.up, 28 * shake);
            Rotate(head, Vector3.forward, -8 * strength);
        }
    }
    void OnDrawGizmos()
    {
        if (settings == null || !settings.drawContactGizmos) return;
        Gizmos.color = PlantedDrift <= settings.plantedDriftTolerance ? Color.green : Color.red;
        if (LeftPlanted) { Gizmos.DrawWireSphere(leftAnchor, settings.plantedDriftTolerance); Gizmos.DrawLine(leftAnchor, legL.end.position); }
        if (RightPlanted) { Gizmos.DrawWireSphere(rightAnchor, settings.plantedDriftTolerance); Gizmos.DrawLine(rightAnchor, legR.end.position); }
    }
    void OnDestroy()
    {
        foreach (var material in materials)
            if (material != null) { if (Application.isPlaying) Destroy(material); else DestroyImmediate(material); }
    }
}
