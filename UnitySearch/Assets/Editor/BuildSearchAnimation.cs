using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

public static class BuildSearchAnimation
{
    const string Folder = "Assets/Resources/SearchAnimation";
    [MenuItem("Search Demo/Build character animations")]
    public static void Build()
    {
        if (!AssetDatabase.IsValidFolder(Folder)) AssetDatabase.CreateFolder("Assets/Resources", "SearchAnimation");
        var source = Resources.Load<GameObject>("FurinaRig/FurinaRig");
        if (source == null) throw new InvalidOperationException("Furina rig missing.");
        var instance = UnityEngine.Object.Instantiate(source);
        instance.hideFlags = HideFlags.HideAndDontSave;
        try
        {
            var names = new Dictionary<string, Transform>();
            foreach (var bone in instance.GetComponentsInChildren<Transform>()) names[bone.name] = bone;
            Vector3 face = Vector3.ProjectOnPlane((names["Eye_L_08"].position + names["Eye_R_07"].position) * .5f - names["Head_06"].position, Vector3.up);
            instance.transform.rotation = Quaternion.AngleAxis(Vector3.SignedAngle(face, Vector3.back, Vector3.up), Vector3.up) * instance.transform.rotation;
            var pose = new SearchCharacterPose(instance.transform);
            string path = Folder + "/SearchAnimation.controller";
            var controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(path);
            if (controller == null) controller = AnimatorController.CreateAnimatorControllerAtPath(path);
            controller.parameters = new AnimatorControllerParameter[0];
            controller.AddParameter("MotionState", AnimatorControllerParameterType.Int);
            controller.AddParameter("MotionSpeed", AnimatorControllerParameterType.Float);
            var machine = controller.layers[0].stateMachine;
            foreach (var transition in machine.anyStateTransitions) machine.RemoveAnyStateTransition(transition);
            foreach (var state in machine.states) machine.RemoveState(state.state);
            foreach (SearchCharacterState kind in Enum.GetValues(typeof(SearchCharacterState)))
            {
                string clipPath = Folder + "/" + kind + ".anim";
                var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(clipPath);
                if (clip == null) { clip = new AnimationClip(); AssetDatabase.CreateAsset(clip, clipPath); }
                clip.ClearCurves(); clip.name = kind.ToString(); clip.frameRate = 30;
                float length = kind == SearchCharacterState.Idle ? 8 : 1;
                int samples = Mathf.RoundToInt(length * clip.frameRate);
                var curves = new AnimationCurve[pose.Bones.Length, 4];
                for (int b = 0; b < pose.Bones.Length; b++) for (int c = 0; c < 4; c++) curves[b, c] = new AnimationCurve();
                for (int s = 0; s <= samples; s++)
                {
                    float time = (float)s / samples;
                    pose.Sample(kind, time);
                    if (kind == SearchCharacterState.Idle)
                    {
                        var chest = names["Chest_04"];
                        chest.rotation = Quaternion.AngleAxis(Mathf.Sin(time * Mathf.PI * 4) * .7f, Vector3.right) * chest.rotation;
                        chest.rotation = Quaternion.AngleAxis(Mathf.Sin(time * Mathf.PI * 2) * .8f, Vector3.forward) * chest.rotation;
                    }
                    for (int b = 0; b < pose.Bones.Length; b++)
                    {
                        Quaternion q = pose.Bones[b].localRotation;
                        curves[b, 0].AddKey(time * length, q.x); curves[b, 1].AddKey(time * length, q.y);
                        curves[b, 2].AddKey(time * length, q.z); curves[b, 3].AddKey(time * length, q.w);
                    }
                }
                string[] components = { "x", "y", "z", "w" };
                for (int b = 0; b < pose.Bones.Length; b++)
                {
                    string bonePath = AnimationUtility.CalculateTransformPath(pose.Bones[b], instance.transform);
                    for (int c = 0; c < 4; c++)
                    {
                        var curve = curves[b, c];
                        var keys = curve.keys;
                        bool constant = true;
                        for (int k = 1; k < keys.Length && constant; k++)
                            constant = Mathf.Abs(keys[k].value - keys[0].value) < .000001f;
                        // Keep the rest pose without hundreds of identical keys per unused joint.
                        if (constant) curve = new AnimationCurve(new Keyframe(0, keys[0].value), new Keyframe(length, keys[0].value));
                        clip.SetCurve(bonePath, typeof(Transform), "m_LocalRotation." + components[c], curve);
                    }
                }
                clip.EnsureQuaternionContinuity();
                var clipSettings = AnimationUtility.GetAnimationClipSettings(clip);
                clipSettings.loopTime = kind == SearchCharacterState.Idle;
                AnimationUtility.SetAnimationClipSettings(clip, clipSettings);
                var state = machine.AddState(kind.ToString(), new Vector3(((int)kind % 3) * 230, ((int)kind / 3) * 90));
                state.motion = clip; state.speedParameter = "MotionSpeed"; state.speedParameterActive = true;
                if (kind == SearchCharacterState.Idle) machine.defaultState = state;
                var transition = machine.AddAnyStateTransition(state);
                transition.hasExitTime = false; transition.hasFixedDuration = true; transition.duration = .08f;
                transition.canTransitionToSelf = false;
                transition.AddCondition(AnimatorConditionMode.Equals, (int)kind, "MotionState");
                EditorUtility.SetDirty(clip);
            }
            EditorUtility.SetDirty(controller);
            AssetDatabase.SaveAssets();
            Debug.Log("Built seven rig-specific clips and SearchAnimation controller.");
        }
        finally { UnityEngine.Object.DestroyImmediate(instance); }
    }
}
