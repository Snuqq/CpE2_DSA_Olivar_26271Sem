"""Render the attributed Sketchfab GLB into native WinForms sprite sheets.

Requires bpy==4.3.0, numpy<2 and Pillow. Usage:
python scripts/render_furina.py path/to/model.glb [--preview]
The model has no skeleton; motion is whole-body sway, turn and bounce.
"""
import math
import sys
from pathlib import Path
import bpy
from mathutils import Vector
from PIL import Image

source = Path(sys.argv[1]).resolve()
output = Path(__file__).resolve().parents[1] / 'Assets' / 'Furina'
output.mkdir(parents=True, exist_ok=True)
bpy.ops.wm.read_factory_settings(use_empty=True)
bpy.ops.import_scene.gltf(filepath=str(source))
meshes = []
for obj in list(bpy.context.scene.objects):
    if obj.type != 'MESH':
        continue
    if not obj.name.startswith('furi_'):
        bpy.data.objects.remove(obj, do_unlink=True)
        continue
    world = obj.matrix_world.copy()
    obj.parent = None
    obj.matrix_world = world
    meshes.append(obj)
root = bpy.data.objects.new('Furina motion', None)
bpy.context.collection.objects.link(root)
for obj in meshes:
    obj.parent = root

scene = bpy.context.scene
scene.render.engine = 'CYCLES'
scene.cycles.samples = 12
scene.cycles.use_denoising = True
scene.render.resolution_x = 256
scene.render.resolution_y = 320
scene.render.resolution_percentage = 100
scene.render.film_transparent = True
scene.render.image_settings.file_format = 'PNG'
scene.render.image_settings.color_mode = 'RGBA'
scene.view_settings.view_transform = 'Standard'
scene.world = bpy.data.worlds.new('Soft studio')
scene.world.use_nodes = True
scene.world.node_tree.nodes['Background'].inputs[0].default_value = (0.8, 0.85, 1, 1)
scene.world.node_tree.nodes['Background'].inputs[1].default_value = 0.65
for location, power, size in [((15, 40, 50), 17000, 30), ((-30, 15, 25), 9000, 25)]:
    light = bpy.data.lights.new('Softbox', 'AREA')
    light.energy = power
    light.shape = 'DISK'
    light.size = size
    obj = bpy.data.objects.new('Softbox', light)
    scene.collection.objects.link(obj)
    obj.location = location
    obj.rotation_euler = (Vector((0, 0, 16)) - obj.location).to_track_quat('-Z', 'Y').to_euler()
camera = bpy.data.objects.new('Camera', bpy.data.cameras.new('Camera'))
scene.collection.objects.link(camera)
camera.location = (0, 85, 22)
camera.rotation_euler = (Vector((-2, 0, 17)) - camera.location).to_track_quat('-Z', 'Y').to_euler()
camera.data.type = 'ORTHO'
camera.data.ortho_scale = 41
scene.camera = camera
temp = output.parent.parent / 'obj' / 'furina-frame.png'
scene.render.filepath = str(temp)
preview = '--preview' in sys.argv
for action in (['idle'] if preview else ['idle', 'search', 'found']):
    frames = 1 if preview else 24
    sheet = Image.new('RGBA', (256 * 6, 320 * 4))
    for frame in range(frames):
        t = frame / 23.0
        wave = math.sin(t * 2 * math.pi)
        root.rotation_euler = (0, 0, 0)
        root.location = (0, 0, 0)
        root.scale = (1, 1, 1)
        if action == 'idle':
            root.rotation_euler[1] = math.radians(1.5) * wave
            root.location.z = 0.2 * (1 - math.cos(t * 2 * math.pi))
        elif action == 'search':
            root.rotation_euler[2] = math.radians(15) * wave
            root.rotation_euler[1] = math.radians(3) * math.sin(t * math.pi)
        else:
            root.location.z = 2.4 * math.sin(t * math.pi)
            root.rotation_euler[2] = math.radians(12) * wave
            root.scale.z = 1 - 0.04 * math.sin(t * 2 * math.pi)
        bpy.ops.render.render(write_still=True)
        with Image.open(temp) as image:
            sheet.paste(image, ((frame % 6) * 256, (frame // 6) * 320))
            if preview:
                image.save(output.parent.parent / 'obj' / 'furina-preview.png')
        print(action, frame + 1, '/', frames, flush=True)
    if not preview:
        sheet.save(output / (action + '.png'), optimize=True)
