using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;
using CpE2_DSA_Olivar_26271Sem;

public enum SearchCharacterState { Idle, Anticipate, Jump, Land, Step, Celebrate, Shrug }

// The presentation clock is the only clock. Animator never advances during Pause.
public sealed class SearchCharacterAnimator : MonoBehaviour, IDisposable
{
    [Header("Idle and gaze")]
    [SerializeField, Min(.1f)] float idleCycleSeconds = 8f;
    [SerializeField, Min(.1f)] float breathingSeconds = 4f;
    [SerializeField, Range(0, 5)] float breathingDegrees = 1.2f;
    [SerializeField, Range(0, .1f)] float idleWeightShift = .018f;
    [SerializeField, Range(0, 60)] float gazeDegrees = 35f;
    [SerializeField, Min(.01f)] float gazeSmoothSeconds = .18f;
    [SerializeField, Min(.01f)] float idleBlendSeconds = .18f;
    [SerializeField, Range(.1f, .17f)] float headRecoveryDelay = .1333333f;
    [Header("Overshoot inspection")]
    [SerializeField, Min(.001f)] float inspectionEnterSeconds = .08f;
    [SerializeField, Min(.001f)] float inspectionHoldSeconds = .12f;
    [SerializeField, Min(.001f)] float inspectionRecoverSeconds = .15f;
    [SerializeField, Range(0, 20)] float inspectionRecoilDegrees = 7f;
    [SerializeField, Range(0, 30)] float inspectionHeadPitchDegrees = 12f;
    [Header("Transitions and secondary motion")]
    [SerializeField, Min(.001f)] float blendSeconds = .08f;
    [SerializeField, Range(0, 1)] float blendPhaseFraction = .2f;
    [SerializeField, Min(.01f)] float secondarySpringSeconds = .22f;
    [SerializeField, Range(0, 20)] float secondaryMaximumDegrees = 10f;
    [SerializeField, Min(.001f)] float integrationMaximumDelta = .05f;
    SearchCharacterPose pose;
    Animator animator;
    PlayableGraph graph;
    AnimatorControllerPlayable controller;
    Quaternion[] previous, transition;
    SearchCharacterState lastState;
    float transitionTime, transitionLength, gazeAngle, gazeVelocity, spring, springVelocity;
    float presentationWeight;
    bool hasSample;
    Vector3 previousRoot;
    static readonly int StateParameter = Animator.StringToHash("MotionState");
    static readonly int SpeedParameter = Animator.StringToHash("MotionSpeed");
    public string CurrentState { get { return lastState.ToString(); } }
    public bool HasController { get { return graph.IsValid(); } }

    public void Initialize(Transform root)
    {
        pose = new SearchCharacterPose(root);
        previous = new Quaternion[pose.Bones.Length]; transition = new Quaternion[previous.Length];
        animator = root.GetComponent<Animator>();
        if (animator == null) animator = root.gameObject.AddComponent<Animator>();
        animator.applyRootMotion = false;
        animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
        var asset = Resources.Load<RuntimeAnimatorController>("SearchAnimation/SearchAnimation");
        if (asset != null)
        {
            graph = PlayableGraph.Create("Search character presentation clock");
            graph.SetTimeUpdateMode(DirectorUpdateMode.Manual);
            controller = AnimatorControllerPlayable.Create(graph, asset);
            var output = AnimationPlayableOutput.Create(graph, "Character", animator);
            output.SetSourcePlayable(controller);
            graph.Play();
        }
        previousRoot = transform.position;
    }

    public void ResetClock()
    {
        hasSample = false; transitionTime = 0;
        spring = springVelocity = gazeAngle = gazeVelocity = 0;
        presentationWeight = 0;
        previousRoot = transform.position;
    }

    public void Sample(SearchMotionPlan plan, double elapsed, bool terminal, bool found,
        bool step, bool configured, double idleTime, float deltaTime)
    {
        SearchCharacterState state;
        float normalized, stateSeconds;
        SelectState(plan, elapsed, terminal, found, step, idleTime, out state, out normalized, out stateSeconds);
        pose.Restore();
        if (graph.IsValid())
        {
            controller.SetInteger(StateParameter, (int)state);
            controller.SetFloat(SpeedParameter, 0);
            // Absolute phase sampling makes seeking and repeated paused frames deterministic.
            controller.Play("Base Layer." + state, 0, normalized);
            graph.Evaluate(0);
        }
        else pose.Sample(state, normalized);

        if (configured && hasSample && state != lastState)
        {
            Array.Copy(previous, transition, previous.Length);
            transitionTime = 0;
            transitionLength = Mathf.Min(blendSeconds, stateSeconds * blendPhaseFraction);
        }
        if (configured && hasSample && transitionTime < transitionLength)
        {
            transitionTime += Mathf.Max(0, deltaTime);
            float blend = Mathf.SmoothStep(0, 1, transitionTime / Mathf.Max(.001f, transitionLength));
            for (int i = 0; i < previous.Length; i++)
                pose.Bones[i].localRotation = Quaternion.Slerp(transition[i], pose.Bones[i].localRotation, blend);
        }
        for (int i = 0; i < previous.Length; i++) previous[i] = pose.Bones[i].localRotation;
        lastState = state; hasSample = true;
    }

    void SelectState(SearchMotionPlan plan, double elapsed, bool terminal, bool found, bool step,
        double idleTime, out SearchCharacterState state, out float time, out float seconds)
    {
        state = SearchCharacterState.Idle;
        seconds = idleCycleSeconds; time = (float)(idleTime / idleCycleSeconds % 1);
        if (plan == null) return;
        var phase = plan.PhaseAt(elapsed);
        if (terminal)
        {
            if (elapsed < plan.StartOf(SearchMotionPhase.React)) return;
            state = found ? SearchCharacterState.Celebrate : SearchCharacterState.Shrug;
            seconds = (float)(plan.LengthOf(SearchMotionPhase.React) + plan.LengthOf(SearchMotionPhase.ReactionSettle));
            time = Mathf.Clamp01((float)(elapsed - plan.StartOf(SearchMotionPhase.React)) / Mathf.Max(.001f, seconds));
            return;
        }
        if (phase == SearchMotionPhase.Choose || phase == SearchMotionPhase.Compare) return;
        if (step)
        {
            state = SearchCharacterState.Step;
            seconds = (float)(plan.StartOf(SearchMotionPhase.Compare) - plan.StartOf(SearchMotionPhase.Anticipate));
            time = Mathf.Clamp01((float)(elapsed - plan.StartOf(SearchMotionPhase.Anticipate)) / Mathf.Max(.001f, seconds));
        }
        else if (phase == SearchMotionPhase.Anticipate)
        {
            state = SearchCharacterState.Anticipate;
            seconds = (float)plan.LengthOf(phase); time = plan.ProgressOf(phase, elapsed);
        }
        else if (phase == SearchMotionPhase.Takeoff || phase == SearchMotionPhase.Travel)
        {
            state = SearchCharacterState.Jump;
            seconds = (float)(plan.LengthOf(SearchMotionPhase.Takeoff) + plan.LengthOf(SearchMotionPhase.Travel));
            time = phase == SearchMotionPhase.Takeoff ? .2f * plan.ProgressOf(phase, elapsed)
                : .2f + .8f * plan.ProgressOf(phase, elapsed);
        }
        else
        {
            state = SearchCharacterState.Land;
            seconds = (float)(plan.LengthOf(SearchMotionPhase.Land) + plan.LengthOf(SearchMotionPhase.Settle));
            time = phase == SearchMotionPhase.Land ? .35f * plan.ProgressOf(phase, elapsed)
                : .35f + .65f * plan.ProgressOf(phase, elapsed);
        }
    }

    public float HeadRecovery(SearchMotionPlan plan, double elapsed)
    {
        if (plan == null) return 0;
        // Fit the trailing recovery inside the existing beat, including at fast playback speeds.
        double delay = Math.Min(headRecoveryDelay, plan.LengthOf(SearchMotionPhase.Compare));
        return 1 - Mathf.SmoothStep(0, 1, plan.ProgressOf(SearchMotionPhase.Settle, elapsed - delay));
    }

    public void ApplyInspection(Transform chest, Transform head, SearchMotionPlan plan,
        double elapsed, Vector3 direction)
    {
        if (plan == null) return;
        double available = plan.LengthOf(SearchMotionPhase.Compare);
        if (available <= 0) return;
        double duration = inspectionEnterSeconds + inspectionHoldSeconds + inspectionRecoverSeconds;
        double scale = Math.Min(1, available / duration);
        double enter = inspectionEnterSeconds * scale;
        double hold = inspectionHoldSeconds * scale;
        double recover = inspectionRecoverSeconds * scale;
        double time = elapsed - plan.StartOf(SearchMotionPhase.Compare);
        float enterWeight = Mathf.SmoothStep(0, 1, (float)(time / enter));
        float recoverWeight = Mathf.SmoothStep(0, 1, (float)((time - enter - hold) / recover));
        float weight = enterWeight * (1 - recoverWeight);
        Vector3 recoilAxis = Vector3.Cross(Vector3.up, direction);
        if (recoilAxis.sqrMagnitude < .001f) recoilAxis = Vector3.left;
        chest.rotation = Quaternion.AngleAxis(-inspectionRecoilDegrees * weight, recoilAxis) * chest.rotation;
        head.rotation = Quaternion.AngleAxis(-inspectionHeadPitchDegrees * weight, Vector3.right) * head.rotation;
    }

    public void ApplyPresentation(Transform hips, Transform chest, Transform head, Transform[] secondary,
        float[] weights, Vector3 gazeTarget, double idleTime, float deltaTime, float idleWeight)
    {
        if (deltaTime > 0)
            presentationWeight = Mathf.Lerp(presentationWeight, idleWeight, 1 - Mathf.Exp(-deltaTime / idleBlendSeconds));
        idleWeight = presentationWeight;
        float breath = Mathf.Sin((float)(idleTime % breathingSeconds) / breathingSeconds * Mathf.PI * 2);
        float shift = Mathf.Sin((float)(idleTime % idleCycleSeconds) / idleCycleSeconds * Mathf.PI * 2);
        chest.rotation = Quaternion.AngleAxis(breath * breathingDegrees * idleWeight, Vector3.right) * chest.rotation;
        hips.position += Vector3.right * (shift * idleWeightShift * idleWeight);
        Vector3 look = gazeTarget - head.position;
        float target = look.sqrMagnitude < .001f ? 0
            : Mathf.Clamp(Vector3.SignedAngle(Vector3.back, Vector3.ProjectOnPlane(look, Vector3.up), Vector3.up), -gazeDegrees, gazeDegrees);
        float dt = Mathf.Min(Mathf.Max(0, deltaTime), integrationMaximumDelta);
        if (dt > 0)
        {
            gazeAngle = Mathf.SmoothDampAngle(gazeAngle, target, ref gazeVelocity, gazeSmoothSeconds, Mathf.Infinity, dt);
            Vector3 speed = (transform.position - previousRoot) / Mathf.Max(dt, .001f);
            float drag = Mathf.Clamp(-speed.x * 2, -secondaryMaximumDegrees, secondaryMaximumDegrees);
            spring = Mathf.SmoothDampAngle(spring, drag, ref springVelocity, secondarySpringSeconds, Mathf.Infinity, dt);
            previousRoot = transform.position;
        }
        head.rotation = Quaternion.AngleAxis(gazeAngle * idleWeight, Vector3.up) * head.rotation;
        for (int i = 0; i < secondary.Length; i++)
            secondary[i].localRotation *= Quaternion.Euler(spring * weights[i] * .4f, 0, spring * weights[i]);
    }

    public void Dispose() { if (graph.IsValid()) graph.Destroy(); }
    void OnDestroy() { Dispose(); }
}

// Shared authoring recipe produces real transform curves and the no-asset fallback.
// Only rotations are authored: support height and contact IK remain presentation layers.
public sealed class SearchCharacterPose
{
    public readonly Transform[] Bones;
    readonly Quaternion[] rest;
    readonly Dictionary<string, Transform> named = new Dictionary<string, Transform>();
    public SearchCharacterPose(Transform root)
    {
        var all = root.GetComponentsInChildren<Transform>();
        Bones = new Transform[all.Length - 1]; rest = new Quaternion[Bones.Length];
        for (int i = 1; i < all.Length; i++)
        {
            Bones[i - 1] = all[i]; rest[i - 1] = all[i].localRotation; named[all[i].name] = all[i];
        }
    }
    public void Restore() { for (int i = 0; i < Bones.Length; i++) Bones[i].localRotation = rest[i]; }
    static float Ease(float value) { return Mathf.SmoothStep(0, 1, value); }
    void Rotate(string name, Vector3 axis, float angle)
    {
        Transform bone;
        if (named.TryGetValue(name, out bone)) bone.rotation = Quaternion.AngleAxis(angle, axis) * bone.rotation;
    }
    void Aim(string start, string end, Vector3 direction)
    {
        Transform bone = named[start];
        bone.rotation = Quaternion.FromToRotation(named[end].position - bone.position, direction) * bone.rotation;
    }
    public void Sample(SearchCharacterState state, float time)
    {
        Restore();
        float t = Mathf.Clamp01(time), lean = 0, roll = 0, turn = 0;
        Vector3 left = new Vector3(.22f, -1, -.12f), leftFore = new Vector3(.12f, -.85f, -.35f);
        Vector3 right = new Vector3(-.22f, -1, -.12f), rightFore = new Vector3(-.12f, -.85f, -.35f);
        Vector3 leftBack = new Vector3(.25f, -.8f, .45f), rightBack = new Vector3(-.3f, -.8f, .35f);
        Vector3 leftAir = new Vector3(.65f, -.2f, -.55f), rightAir = new Vector3(-.4f, -.65f, .25f);
        Vector3 leftLand = new Vector3(.55f, -.65f, -.3f), rightLand = new Vector3(-.6f, -.7f, -.25f);
        if (state == SearchCharacterState.Anticipate)
        {
            float a = Ease(t); left = Vector3.Lerp(left, leftBack, a); right = Vector3.Lerp(right, rightBack, a);
            leftFore = Vector3.Lerp(leftFore, new Vector3(.1f, -.8f, .3f), a);
            rightFore = Vector3.Lerp(rightFore, new Vector3(-.1f, -.8f, .4f), a);
            lean = 9 * a; roll = -3 * a;
        }
        else if (state == SearchCharacterState.Jump)
        {
            float rise = Ease(t / .55f), fall = Ease((t - .55f) / .45f);
            left = Vector3.Lerp(Vector3.Lerp(leftBack, leftAir, rise), leftLand, fall);
            right = Vector3.Lerp(Vector3.Lerp(rightBack, rightAir, rise), rightLand, fall);
            leftFore = Vector3.Lerp(Vector3.Lerp(new Vector3(.1f, -.8f, .3f), new Vector3(.2f, .5f, -.6f), rise), new Vector3(.3f, -.6f, -.5f), fall);
            rightFore = Vector3.Lerp(Vector3.Lerp(new Vector3(-.1f, -.8f, .4f), new Vector3(-.3f, -.2f, -.5f), rise), new Vector3(-.35f, -.7f, -.4f), fall);
            lean = Mathf.Lerp(Mathf.Lerp(9, -10, rise), -5, fall); roll = Mathf.Lerp(-3, 5, rise) * (1 - fall);
            turn = 8 * Mathf.Sin(t * Mathf.PI);
            Rotate("Left leg_0120", Vector3.right, -25 * Mathf.Sin(t * Mathf.PI));
            Rotate("Right leg_0124", Vector3.right, 15 * Mathf.Sin(t * Mathf.PI));
        }
        else if (state == SearchCharacterState.Land)
        {
            float recover = Ease((t - .35f) / .65f), impact = Mathf.Sin(Mathf.Clamp01(t / .65f) * Mathf.PI);
            left = Vector3.Lerp(leftLand + Vector3.up * (.15f * impact), left, recover);
            right = Vector3.Lerp(rightLand + Vector3.up * (.1f * impact), right, recover);
            leftFore = Vector3.Lerp(new Vector3(.3f, -.6f, -.5f), leftFore, recover);
            rightFore = Vector3.Lerp(new Vector3(-.35f, -.7f, -.4f), rightFore, recover);
            lean = (-5 - 8 * impact) * (1 - recover);
        }
        else if (state == SearchCharacterState.Step)
        {
            float swing = Mathf.Sin(t * Mathf.PI * 2) * Mathf.Sin(t * Mathf.PI);
            left += Vector3.back * (.35f * swing); right += Vector3.forward * (.35f * swing);
            leftFore += Vector3.back * (.2f * swing); rightFore += Vector3.forward * (.2f * swing);
            lean = -4 * Mathf.Sin(t * Mathf.PI); roll = 3 * swing; turn = -4 * swing;
        }
        else if (state == SearchCharacterState.Celebrate)
        {
            float present = Ease(t / .65f), settle = Ease((t - .65f) / .35f);
            float flourish = Mathf.Sin(t * Mathf.PI) * (1 - .3f * settle);
            right = Vector3.Lerp(right, new Vector3(-1, -.15f, -.35f), present) + Vector3.up * (.2f * flourish);
            rightFore = Vector3.Lerp(rightFore, new Vector3(-.8f, .1f, -.55f), present);
            leftFore = Vector3.Lerp(leftFore, new Vector3(-.2f, .7f, -.4f), present);
            roll = -5 * present; turn = -12 * present;
            Rotate("Head_06", Vector3.forward, 5 * present);
        }
        else if (state == SearchCharacterState.Shrug)
        {
            float register = Ease(t / .25f), shrug = Ease((t - .2f) / .35f), settle = Ease((t - .7f) / .3f);
            float amount = shrug * (1 - .3f * settle);
            left = Vector3.Lerp(left, new Vector3(.55f, -.6f, -.15f), amount);
            right = Vector3.Lerp(right, new Vector3(-.5f, -.7f, -.2f), amount);
            leftFore = Vector3.Lerp(leftFore, new Vector3(.75f, .35f, -.25f), amount);
            rightFore = Vector3.Lerp(rightFore, new Vector3(-.7f, .25f, -.3f), amount);
            lean = -8 * register; roll = 5 * register;
            float shake = Mathf.Sin(Mathf.Clamp01((t - .4f) / .6f) * Mathf.PI * 2) * Mathf.Sin(t * Mathf.PI);
            Rotate("Head_06", Vector3.up, 20 * shake);
        }
        Rotate("Chest_04", Vector3.right, lean);
        Rotate("Chest_04", Vector3.forward, roll);
        Rotate("Chest_04", Vector3.up, turn);
        Aim("Left arm_059", "Left elbow_061", left);
        Aim("Left elbow_061", "Left wrist_063", leftFore);
        Aim("Right arm_085", "Right elbow_087", right);
        Aim("Right elbow_087", "Right wrist_089", rightFore);
    }
}
