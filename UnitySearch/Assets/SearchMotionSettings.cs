using System;
using UnityEngine;
using CpE2_DSA_Olivar_26271Sem;

[Serializable]
public sealed class SearchMotionSettings
{
    [Header("Playback (seconds)")]
    [SerializeField, Min(0.01f)] public float defaultStepSeconds = 3f;
    [SerializeField, Min(0.01f)] public float minimumStepSeconds = .4f;
    [SerializeField, Min(0.01f)] public float maximumStepSeconds = 10f;
    [SerializeField, Min(0.01f)] public float chooseSeconds = .3f;
    [SerializeField, Min(0.01f)] public float comparisonSeconds = .45f;
    [Header("Contact phases: 30 fps reference converted to seconds")]
    [SerializeField, Min(0.01f)] public float anticipationSeconds = .1666667f;
    [SerializeField, Min(0.01f)] public float takeoffSeconds = .0666667f;
    [SerializeField, Min(0.01f)] public float landingSeconds = .1166667f;
    [SerializeField, Min(0.01f)] public float settleSeconds = .2333333f;
    [Header("Travel")]
    [SerializeField] public bool scaleDurationWithDistance = true;
    [SerializeField, Range(0.01f, 1f)] public float constantTravelFraction = .45f;
    [SerializeField, Min(0.01f)] public float referenceDistance = 2.4f;
    [SerializeField, Min(0.01f)] public float minimumTravelSeconds = .3f;
    [SerializeField, Min(0.01f)] public float maximumTravelSeconds = 1.8f;
    [SerializeField, Min(0)] public float minimumHopHeight = .3f;
    [SerializeField, Min(0)] public float maximumHopHeight = .85f;
    [SerializeField, Min(0)] public float heightPerMetre = .12f;
    [SerializeField, Min(0)] public float compressionDepth = .16f;
    [SerializeField, Min(0)] public float launchWeightShift = .06f;
    [Header("Walking across the floor between neighbouring podiums")]
    [SerializeField, Min(0)] public float maximumWalkDistance = 2.5f;
    [SerializeField, Min(.01f)] public float walkingSeconds = 1.65f;
    [SerializeField, Range(0, 1)] public float walkingStrideTimingWeight = .7f;
    [SerializeField, Range(0, 1)] public float walkingSupportCentering = .6f;
    [SerializeField, Min(0)] public float floorDepth = .365f;
    [SerializeField, Min(0)] public float platformSupportRadius = .72f;
    [SerializeField, Min(0)] public float walkingFootLift = .16f;
    [SerializeField, Min(0)] public float walkingPelvisDrop = .12f;
    [SerializeField, Min(0)] public float walkingCompressionDepth = .055f;
    [SerializeField, Min(0)] public float kneeReachMargin = .06f;
    [SerializeField, Range(0, 60)] public float walkingHipTurnDegrees = 30f;
    [SerializeField, Min(0)] public float leadingKneeLift = .3f;
    [SerializeField, Min(0)] public float trailingKneeLift = .45f;
    [SerializeField, Range(0, 45)] public float airborneToePointDegrees = 20f;
    [SerializeField, Range(0, 20)] public float hipCounterturnDegrees = 9f;
    [SerializeField, Range(0, 20)] public float walkingSwayDegrees = 4f;
    [Header("Directional posing")]
    [SerializeField, Range(0, 90)] public float chestLeadDegrees = 35f;
    [SerializeField, Range(0, 90)] public float headLeadDegrees = 55f;
    [SerializeField, Range(0, 30)] public float travelLeanDegrees = 12f;
    [SerializeField, Min(0.01f)] public float poseBlendSeconds = .1f;
    [SerializeField, Min(0.01f)] public float secondaryRecoveryDelay = .1333333f;
    [SerializeField, Min(0.01f)] public float secondarySettleSeconds = .2333333f;
    [SerializeField, Range(0, 20)] public float secondaryDragDegrees = 8f;
    [SerializeField, Min(.01f)] public float secondaryOscillationSeconds = .32f;
    [Header("Reaction and camera (seconds)")]
    [SerializeField, Min(0.01f)] public float lookSeconds = .2f;
    [SerializeField, Min(0.01f)] public float registerSeconds = .2f;
    [SerializeField, Min(0.01f)] public float reactionSeconds = .6f;
    [SerializeField, Min(0.01f)] public float reactionSettleSeconds = .2666667f;
    [SerializeField, Min(0.01f)] public float mediumHoldSeconds = .25f;
    [SerializeField, Min(0.01f)] public float pullbackSeconds = 1.05f;
    [SerializeField, Range(7, 14)] public float resultHoldSeconds = 7f;
    [SerializeField, Min(0)] public float reactionWeightShift = .08f;
    [SerializeField, Min(0)] public float reactionHipDrop = .06f;
    [Header("Verification")]
    [SerializeField] public bool logMotionPhases;
    [SerializeField] public bool drawContactGizmos = true;
    [SerializeField, Min(0.0001f)] public float plantedDriftTolerance = .01f;

    public SearchMotionPlan Probe(float distance, double requestedSeconds)
    {
        bool moving = distance > Mathf.Epsilon;
        double travel = moving ? requestedSeconds * constantTravelFraction : 0;
        if (moving && scaleDurationWithDistance)
            travel = Mathf.Clamp((float)Math.Max(travel, walkingSeconds) * distance / referenceDistance,
                minimumTravelSeconds, maximumTravelSeconds);
        double anticipation = moving ? anticipationSeconds : 0, takeoff = moving ? takeoffSeconds : 0;
        double landing = moving ? landingSeconds : 0, settle = moving ? settleSeconds : 0;
        double minimum = chooseSeconds + anticipation + takeoff + travel + landing + settle + comparisonSeconds;
        double comparison = comparisonSeconds + Math.Max(0, requestedSeconds - minimum);
        return new SearchMotionPlan(new[] { SearchMotionPhase.Choose, SearchMotionPhase.Anticipate,
            SearchMotionPhase.Takeoff, SearchMotionPhase.Travel, SearchMotionPhase.Land,
            SearchMotionPhase.Settle, SearchMotionPhase.Compare },
            new[] { (double)chooseSeconds, anticipation, takeoff, travel, landing, settle, comparison });
    }
    public SearchMotionPlan Result()
    {
        return new SearchMotionPlan(new[] { SearchMotionPhase.Look, SearchMotionPhase.Register,
            SearchMotionPhase.React, SearchMotionPhase.ReactionSettle, SearchMotionPhase.CameraHold,
            SearchMotionPhase.Pullback, SearchMotionPhase.ResultHold },
            new[] { (double)lookSeconds, registerSeconds, reactionSeconds, reactionSettleSeconds,
                mediumHoldSeconds, pullbackSeconds, resultHoldSeconds });
    }
}
