using System;
using UnityEngine;
using CpE2_DSA_Olivar_26271Sem;

[Serializable]
public sealed class SearchMotionSettings
{
    [Header("Playback (seconds)")]
    [SerializeField, Min(0.01f)] public float defaultStepSeconds = 5f;
    [SerializeField, Min(0.01f)] public float minimumStepSeconds = .4f;
    [SerializeField, Min(0.01f)] public float maximumStepSeconds = 10f;
    [SerializeField, Min(0.01f)] public float chooseSeconds = .3f;
    [SerializeField, Min(0.01f)] public float comparisonSeconds = .45f;
    [Header("Contact phases: 30 fps reference converted to seconds")]
    [SerializeField, Min(0.01f)] public float anticipationSeconds = .2f;
    [SerializeField, Min(0.01f)] public float takeoffSeconds = .0666667f;
    [SerializeField, Min(0.01f)] public float landingSeconds = .1166667f;
    [SerializeField, Min(0.01f)] public float settleSeconds = .2333333f;
    [Header("Travel")]
    [SerializeField] public bool scaleDurationWithDistance = true;
    [SerializeField, Range(0.01f, 1f)] public float constantTravelFraction = .45f;
    [SerializeField, Min(0.01f)] public float referenceDistance = 2.4f;
    [SerializeField, Min(0.01f)] public float minimumTravelSeconds = .12f;
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
    [Header("Step budget: fractions of the requested duration")]
    [SerializeField, Range(.01f, .3f)] public float chooseFraction = .08f;
    [SerializeField, Range(.01f, .3f)] public float anticipationFraction = .18f;
    [SerializeField, Range(.01f, .2f)] public float takeoffFraction = .04f;
    [SerializeField, Range(.01f, .3f)] public float landingFraction = .1f;
    [SerializeField, Range(.01f, .3f)] public float settleFraction = .2f;
    [SerializeField, Range(.01f, .3f)] public float comparisonFraction = .1f;
    [SerializeField, Range(.1f, .95f)] public float maximumReservedFraction = .8f;
    [SerializeField, Range(.01f, .5f)] public float shortTravelFraction = .18f;
    [SerializeField, Range(.01f, .6f)] public float longTravelFraction = .38f;
    [SerializeField, Min(1)] public float longJumpDistances = 3f;
    [SerializeField, Range(.1f, 1)] public float scanTravelScale = .72f;
    [SerializeField, Min(.1f)] public float resetSeconds = 1.2f;
    [SerializeField, Range(.01f, .5f)] public float resetPreframeFraction = .25f;
    [Header("Verification")]
    [SerializeField] public bool logMotionPhases;
    [SerializeField] public bool drawContactGizmos = true;
    [SerializeField, Min(0.0001f)] public float plantedDriftTolerance = .01f;

    public SearchMotionPlan Probe(float distance, double requestedSeconds)
    {
        return Probe(distance, requestedSeconds, false);
    }
    public SearchMotionPlan Probe(float distance, double requestedSeconds, bool scan)
    {
        if (double.IsNaN(requestedSeconds) || double.IsInfinity(requestedSeconds) || requestedSeconds <= 0)
            throw new ArgumentOutOfRangeException("requestedSeconds");
        bool moving = distance > Mathf.Epsilon;
        double choose = Math.Min(chooseSeconds, requestedSeconds * chooseFraction);
        double anticipation = moving ? Math.Min(anticipationSeconds, requestedSeconds * anticipationFraction) : 0;
        double takeoff = moving ? Math.Min(takeoffSeconds, requestedSeconds * takeoffFraction) : 0;
        double landing = moving ? Math.Min(landingSeconds, requestedSeconds * landingFraction) : 0;
        double settle = moving ? Math.Min(settleSeconds, requestedSeconds * settleFraction) : 0;
        double comparison = Math.Min(comparisonSeconds, requestedSeconds * comparisonFraction);
        // Inspector edits cannot make the mandatory phases overrun the requested beat.
        double reserved = choose + anticipation + takeoff + landing + settle + comparison;
        double budgetScale = Math.Min(1, requestedSeconds * maximumReservedFraction / Math.Max(reserved, double.Epsilon));
        choose *= budgetScale; anticipation *= budgetScale; takeoff *= budgetScale;
        landing *= budgetScale; settle *= budgetScale; comparison *= budgetScale;
        double available = requestedSeconds - choose - anticipation - takeoff - landing - settle - comparison;
        float fraction = scaleDurationWithDistance ? Mathf.Lerp(shortTravelFraction, longTravelFraction,
            Mathf.Clamp01(distance / Math.Max(.01f, referenceDistance * longJumpDistances))) : constantTravelFraction;
        double travel = moving ? Math.Min(available, Mathf.Clamp((float)requestedSeconds * fraction * (scan ? scanTravelScale : 1),
            minimumTravelSeconds, maximumTravelSeconds)) : 0;
        comparison = requestedSeconds - choose - anticipation - takeoff - travel - landing - settle;
        return new SearchMotionPlan(new[] { SearchMotionPhase.Choose, SearchMotionPhase.Anticipate,
            SearchMotionPhase.Takeoff, SearchMotionPhase.Travel, SearchMotionPhase.Land,
            SearchMotionPhase.Settle, SearchMotionPhase.Compare },
            new[] { choose, anticipation, takeoff, travel, landing, settle, comparison });
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
