"""Import the licensed rigged GLB without discarding skinning; requires bpy 4.3."""
import sys
from pathlib import Path
import bpy
from mathutils import Vector

root = Path(__file__).resolve().parents[1]
dest = root / 'UnitySearch/Assets/Resources/FurinaRig'
dest.mkdir(parents=True, exist_ok=True)
bpy.ops.wm.read_factory_settings(use_empty=True)
bpy.ops.import_scene.gltf(filepath=str(Path(sys.argv[1]).resolve()))
armatures = [o for o in bpy.context.scene.objects if o.type == 'ARMATURE']
assert len(armatures) == 1, 'Expected one skinned skeleton'
meshes = [o for o in bpy.context.scene.objects if o.type == 'MESH' and any(m.type == 'ARMATURE' for m in o.modifiers)]
assert len(meshes) == 5, 'Expected five skinned material meshes'
for obj in list(bpy.context.scene.objects):
    if obj.type == 'MESH' and obj not in meshes:
        bpy.data.objects.remove(obj, do_unlink=True)
for index, material in enumerate(bpy.data.materials):
    material.name = 'FurinaMaterial_' + str(index)
    if material.use_nodes:
        for node in material.node_tree.nodes:
            if node.type == 'TEX_IMAGE' and node.image:
                image = node.image
                image.filepath_raw = str(dest / (material.name + '.png'))
                image.file_format = 'PNG'
                image.save()
                break
# Preserve original bone axes and bind transforms; Unity scales the instantiated root.
bpy.ops.export_scene.fbx(filepath=str(dest / 'FurinaRig.fbx'),
    object_types={'ARMATURE', 'MESH', 'EMPTY'}, add_leaf_bones=False,
    axis_forward='-Z', axis_up='Y', path_mode='COPY', embed_textures=True,
    bake_anim=False, use_armature_deform_only=False)
print('Exported', len(armatures[0].data.bones), 'bones;', len(meshes), 'skinned meshes')
for name in ['Hips_02', 'Head_06', 'Left arm_059', 'Left elbow_061', 'Left leg_0120', 'Left knee_0121', 'Left ankle_0122']:
    bone = armatures[0].data.bones.get(name)
    if bone:
        print(name, 'head', tuple(round(v, 3) for v in bone.head_local), 'tail', tuple(round(v, 3) for v in bone.tail_local))
