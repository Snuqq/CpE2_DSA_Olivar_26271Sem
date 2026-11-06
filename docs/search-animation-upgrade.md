# Search animation setup and checks

The character uses the existing Generic Furina rig in Unity 6000.3.26f1. The new Animator assets contain rig-specific rotation curves. Procedural layers provide travel, pelvis compression, gaze, secondary motion, and foot contact. Search results and probe order still come from the three WinForms search forms.

Build and measurement results from November 1, 2026 are recorded below. The motion uses generated Generic clips and procedural layers, not imported motion-capture performances.

## Open the scene and create the animation assets

1. Open the repository's `UnitySearch` folder in Unity Hub with Unity **6000.3.26f1**.
2. Open `Assets/Search.unity`. Keep the `SearchStage` component on **Search demonstration**. No model or bone assignments are required in this scene.
3. Select **Search Demo > Build character animations**. This runs `BuildSearchAnimation.Build()` and writes the controller and seven clips to `Assets/Resources/SearchAnimation/`.
4. Confirm that `SearchAnimation.controller`, `Idle.anim`, `Anticipate.anim`, `Jump.anim`, `Land.anim`, `Step.anim`, `Celebrate.anim`, and `Shrug.anim` exist. The builder can recreate these generated assets; keep hand-edited alternatives elsewhere.
   The normal `scripts/BuildSearchApp.ps1` build also generates these assets and runs the rig, camera, and animation checks before building the player.
5. Press **Play**. The stage creates **Blue and gold theatre**, **Fontaine opera architecture**, and **Furina performer**. `FurinaActor` adds `SearchCharacterAnimator`, which loads the controller by its Resources path and binds a manual PlayableGraph to the model's Animator.
6. Select **Search Demo > Play-mode checks > Jump found** to preview the supplied ten-number example. Select **Jump missing** for target 12. Use the WinForms app for the Start, Pause, Next Step, Reset, and speed-control tests below.

Keep `Assets/Resources/FurinaRig/FurinaRig` and its bone names unchanged. The Animator uses **Apply Root Motion = false** and **Culling Mode = Always Animate**. Do not add a second independent Animator or animation timer to the performer.

`SearchStage.Motion` settings are available on the saved scene object before Play. The performer, camera, and scenery components exist during Play. Inspector edits to those runtime components disappear after Play; preserve intentional changes through their serialized field defaults in the scripts. The current scene creates these components in code and does not contain authored component prefabs.

## Exact Animator Controller setup

The builder creates one **Base Layer** with **Idle** as its default state. Each state uses its matching `.anim` clip and enables **Speed > Parameter > MotionSpeed**.

| State | `MotionState` value | Clip length | Loop Time | Runtime use |
|---|---:|---:|---|---|
| Idle | 0 | 8 s | On | Waiting, preframing, and inspection holds |
| Anticipate | 1 | 1 s | Off | Grounded preparation before a block leap |
| Jump | 2 | 1 s | Off | Takeoff, rise, apex, and fall |
| Land | 3 | 1 s | Off | Landing compression and recovery |
| Step | 4 | 1 s | Off | Linear-scan hop, including preparation and recovery |
| Celebrate | 5 | 1 s | Off | Found result presentation |
| Shrug | 6 | 1 s | Off | Not-found reaction |

Parameters:

- **MotionState:** Integer, default **0**.
- **MotionSpeed:** Float, default **0**. Runtime sampling keeps this at zero because the presentation clock supplies the clip position directly. For a standalone controller preview outside the driver, set it to **1**.

Create an **Any State** transition to each of the seven states:

- Condition: **MotionState Equals** the state's integer value above.
- **Has Exit Time:** Off.
- **Fixed Duration:** On.
- **Transition Duration:** **0.08 seconds**.
- **Transition Offset:** **0**.
- **Can Transition To Self:** Off.
- **Interruption Source:** None; leave the other transition settings at their defaults.
- No extra state-to-state or Exit transitions are required.

The scheduler owns phase completion. `SearchCharacterAnimator` selects the state and normalized time with `controller.Play(...)`, then evaluates its manual graph with `Evaluate(0)`. It blends sampled bone rotations from the preceding pose for `min(0.08 seconds, receiving phase duration × 0.20)`. Automatic Animator exit times therefore do not control the live demo. This preserves an exact paused pose and keeps each probe inside the requested step duration. Unity documents manual PlayableGraph updates through [DirectorUpdateMode.Manual](https://docs.unity3d.com/6000.3/Documentation/ScriptReference/Playables.DirectorUpdateMode.Manual.html).

### Curves and phase mapping

The builder samples `SearchCharacterPose.Sample()` at **30 samples per second** into each bone's quaternion `m_LocalRotation.x/y/z/w` curves and applies quaternion continuity. Constant channels keep only their endpoint keys. These are interpolated curves; the sample rate does not limit rendering to 30 FPS. No root translation or root scale curve is authored.

| Curve or mapping | Exact rule |
|---|---|
| Pose easing | `SmoothStep(0, 1, t)`, with `t` clamped to 0–1 |
| Travel position | Quintic smootherstep: `t³ × (t × (6t − 15) + 10)` between source and destination |
| Leap height | `4 × height × t × (1 − t)`; endpoints 0, apex `height` at `t = 0.5` |
| Block-leap height | Clamp `distance × 0.12` to **0.30–0.85 m** |
| Scan-hop height | Minimum of `0.22 + distance × 0.12 × 0.15` and **0.85 m** |
| Jump clip | Takeoff samples normalized **0–0.20**; Travel samples **0.20–1** |
| Jump arm poses | Rise reaches its pose at normalized **0.55**; fall blends during **0.55–1** |
| Land clip | Land samples normalized **0–0.35**; Settle samples **0.35–1** |
| Step clip | Anticipate through Settle samples normalized **0–1** |
| Reaction clips | React plus ReactionSettle samples normalized **0–1**, then holds the last pose |
| Generated Idle chest | Pitch: `sin(4πt) × 0.7°`; roll: `sin(2πt) × 0.8°` across the eight-second clip |

The actor applies world-space two-bone foot IK after pose blending, directional rotation, breathing, and secondary motion. It solves foot contacts by rotating the existing joints. The solver does not stretch the leg bones. The Generic rig does not use Humanoid `OnAnimatorIK` foot goals.

## Inspector defaults

### SearchStage > Motion

These are serialized defaults in `SearchMotionSettings`. The numeric control in Jump Search defaults to **5000 ms**. Its current range is **400–5000 ms**; the Unity protocol accepts the configured stage range.

| Setting | Default |
|---|---:|
| Default / minimum / maximum step seconds | 5 / 0.4 / 10 |
| Choose / comparison seconds, upper bounds | 0.30 / 0.45 |
| Anticipation / takeoff seconds, upper bounds | 0.20 / 0.0666667 |
| Landing / settle seconds, upper bounds | 0.1166667 / 0.2333333 |
| Scale Duration With Distance | On |
| Constant Travel Fraction | 0.45 |
| Reference Distance / Long Jump Distances | 2.4 m / 3 |
| Minimum / maximum travel seconds | 0.12 / 1.8 |
| Minimum / maximum hop height | 0.30 / 0.85 m |
| Height Per Metre | 0.12 |
| Compression Depth / Launch Weight Shift | 0.16 / 0.06 m |
| Choose / anticipation / takeoff fraction | 0.08 / 0.18 / 0.04 |
| Landing / settle / comparison fraction | 0.10 / 0.20 / 0.10 |
| Maximum Reserved Fraction | 0.80 |
| Short / long travel fraction | 0.18 / 0.38 |
| Scan Travel Scale | 0.72 |
| Reset Seconds / Reset Preframe Fraction | 1.20 / 0.25 |
| Chest Lead / Head Lead / Travel Lean | 35° / 55° / 12° |
| Pose Blend Seconds | 0.10 |
| Secondary Recovery Delay / Settle Seconds | 0.1333333 / 0.2333333 |
| Secondary Drag / Oscillation Seconds | 8° / 0.32 |
| Leading / trailing knee lift | 0.30 / 0.45 m |
| Airborne Toe Point / Hip Counterturn | 20° / 9° |
| Look / register / reaction seconds | 0.20 / 0.20 / 0.60 |
| Reaction Settle / Medium Hold / Pullback seconds | 0.2666667 / 0.25 / 1.05 |
| Result Hold Seconds | 7 |
| Reaction Weight Shift / Hip Drop | 0.08 / 0.06 m |
| Log Motion Phases / Draw Contact Gizmos | Off / On |
| Planted Drift Tolerance | 0.01 m |

For a moving probe, each preparation/contact phase uses the smaller of its seconds value and its fraction of the requested duration. If Inspector edits overbook the mandatory phases, the budget scales them down together. Travel uses the distance fraction and configured bounds; the remaining time becomes the Compare hold. A nonterminal probe therefore lasts the requested duration, including its hold. Changing speed affects the next probe. The terminal reaction uses its separate schedule, including the seven-second settled overview hold.

Example defaults for a **7.2 m block leap**:

| Requested duration | Choose | Anticipate | Takeoff | Travel | Land | Settle | Compare |
|---|---:|---:|---:|---:|---:|---:|---:|
| 1.000 s | 0.080 | 0.180 | 0.040 | 0.300 | 0.100 | 0.200 | 0.100 |
| 5.000 s | 0.300 | 0.200 | 0.067 | 1.800 | 0.117 | 0.233 | 2.283 |

The retained floor-walking compatibility settings apply to the actor's direct, unconfigured pose path. The live search driver explicitly chooses block leaps or scan hops. Those retained defaults are: maximum walk distance **2.5 m**, walking seconds **1.65**, stride timing weight **0.70**, support centering **0.60**, floor depth **0.365 m**, platform support radius **0.72 m**, foot lift **0.16 m**, pelvis drop **0.12 m**, walking compression **0.055 m**, knee reach margin **0.06 m**, hip turn **30°**, and sway **4°**. Do not use them to tune the new scan hop.

### Runtime performer and camera

| Component | Field | Default |
|---|---|---:|
| FurinaActor | Scan Hop Height / Scan Knee Scale / Reset Clearance | 0.22 m / 0.45 / 0.25 m |
| SearchCharacterAnimator | Idle Cycle / Breathing Seconds | 8 / 4 |
| SearchCharacterAnimator | Breathing Degrees / Idle Weight Shift | 1.2° / 0.018 m |
| SearchCharacterAnimator | Gaze Degrees / Gaze Smooth Seconds | 35° / 0.18 |
| SearchCharacterAnimator | Idle Blend Seconds / Head Recovery Delay | 0.18 / 0.1333333 |
| SearchCharacterAnimator | Inspection Enter / Hold / Recover Seconds | 0.08 / 0.12 / 0.15 |
| SearchCharacterAnimator | Inspection Recoil / Head Pitch Degrees | 7° / 12° |
| SearchCharacterAnimator | Blend Seconds / Blend Phase Fraction | 0.08 / 0.20 |
| SearchCharacterAnimator | Secondary Spring Seconds / Maximum Degrees | 0.22 / 10° |
| SearchCharacterAnimator | Integration Maximum Delta | 0.05 s |
| SearchTheatre | Camera Follow Seconds / Destination Look Ahead | 0.20 / 0.25 |
| SearchTheatre | Performer Height / Performer Radius / Platform Radius | 2.9 / 0.65 / 0.88 m |
| SearchTheatre | Framing Margin / Minimum Frame Size | 0.08 / 2.15 |
| SearchTheatre | Camera Elevation / Distance | 1.3 / 12 m |
| SearchTheatre | Preframe Widen / Pan Fraction | 0.20 / 0.50 |
| FontaineScenery | Landing Ripple / Found Pulse Seconds | 0.32 / 0.45 |
| FontaineScenery | Ripple Elevation / Start Scale / End Scale | 0.018 / 0.40 / 1.22 |
| FontaineScenery | Found Emission Boost | 1.35 |

The camera widens first, pans second, and tightens last during Choose. It locks zoom before Anticipate and uses `Vector3.SmoothDamp` during travel. Framing includes the full arc, character bounds, and destination platform. Resizing the window can widen the frame to keep those bounds visible. A requested camera-mode change waits until a grounded phase. At the result, the camera waits through the reaction, settle, and medium hold, then centers before zooming out.

Hair and sleeve bones receive delayed rotational recovery and a damped velocity response. This layer has no cloth collision simulation. Its transforms stop advancing during Pause.

When a boundary probe meets or exceeds the target, the comparison hold adds a short recoil and head check. It consumes the existing Compare time and cannot change the trace. Idle breathing, weight shift, and gaze blend in over the configured interval instead of switching on at a phase boundary. Reset discards a queued view toggle so the camera returns to its settled overview.

### Frame pacing and diagnostics

`SearchStage` requests **60 FPS**, enables background execution, and sets the render interval to **1**. It selects VSync only when the screen refresh rate divided by a supported interval is within **0.5 Hz** of 60; otherwise it uses the software cap. Unity's desktop frame pacing depends on [VSync and targetFrameRate](https://docs.unity3d.com/6000.3/Documentation/ScriptReference/Application-targetFrameRate.html), so a requested cap is not proof of delivered performance.

Enable **Log Frame Pacing** on Search demonstration before a performance probe to log target FPS, VSync, render interval, focus, background mode, and frame-time percentiles. **Diagnostic Warmup Seconds** defaults to **1**. Compare standalone and embedded runs at the same player size without recording screenshots.

The retained `RECORD` diagnostic coroutine samples screenshots every **1/30 s**, up to **30 s** by default, and encodes PNGs synchronously. It does not drive character motion. Its capture overhead can lower frame rate; duplicated frames in a converted recording must be checked against the saved capture timestamps and an independent live performance run.

## Generic fallback and optional Humanoid clips

The supplied rig has no suitable imported locomotion clips or Humanoid Avatar. The builder creates seven usable Generic clips for its actual hierarchy. If the generated controller is missing, `SearchCharacterAnimator` samples the same `SearchCharacterPose` recipe directly, then applies the same travel/contact layers. The fallback requires no package or downloaded animation.

For a later authored animation pass, obtain licensed **breathing idle**, **jump anticipation**, **jump rise/apex/fall**, **landing recovery**, **small step or hop**, **asymmetric celebration**, and **shrug/head-shake** clips. Prefer in-place clips: the search presentation must retain control of world movement.

To investigate Humanoid retargeting without modifying the working rig:

1. Import a separate copy of the character into a new folder. Set its Model Importer **Rig > Animation Type = Humanoid**, **Avatar Definition = Create From This Model**, and Apply.
2. Configure the Avatar. Map hips, spine, chest, neck/head, arms, hands, legs, and feet; correct the reference T-pose. Continue only if Unity reports a valid Avatar.
3. Import each source animation as Humanoid and configure that source skeleton's own valid Avatar. Use **Copy From Other Avatar** only for clips exported on the same skeleton as that Avatar.
4. Preview retargeting on the copied character's Animator with the copied character's Avatar. Bake root rotation and root Y/XZ into the pose where appropriate for in-place animation; check feet and knees through takeoff and landing.
5. Keep this experiment separate from the running Generic performer. The current curve bindings and contact setup use exact Furina bone names. Replacing them with Humanoid muscle clips requires adapting the driver, state sampling, and bone lookup, then repeating the contact tests. Changing the importer dropdown alone is not a drop-in upgrade.

Unity describes the Avatar requirement and compatible character setup in its [Humanoid retargeting guide](https://docs.unity3d.com/6000.0/Documentation/Manual/Retargeting.html).

## Play-mode and embedded test checklist

Use sorted values **1, 3, 5, 7, 9, 11, 13, 15, 17, 19**. Run the following at **1000 ms** and **5000 ms**:

- **Found target 13:** Jump Search must probe indexes **2, 5, 8, 6** and return **6**. Block leaps need visible preparation, an arc, and contact recovery. The scan back to index 6 must use a smaller hop and a number-checking pose.
- **Missing target 12:** complete the search without finding a value. Check shoulder drop, weight shift, open palms, then head shake. The reaction must differ from idle without relying on text.
- **Idle:** wait before Start and between single steps. Check breathing, small weight shifts, and target gaze. Neither foot may slide or float.
- **Next Step spam:** click repeatedly during anticipation, flight, and landing. Finish only the active step; no extra probes should queue, and no pose should jump to a later platform.
- **Pause mid-jump:** hold for several seconds. Feet, root, bones, camera, glow, and ripple must stay frozen. Resume through Start or Next Step and verify continuity.
- **Reset mid-jump:** check that the performer returns from its current pose over the configured reset interval. Repeated Reset must begin from the visible pose. Start or Next Step during that return must wait for it to finish. There must be no teleport, stale-session movement, or stuck state.
- **Timing:** enable **Log Motion Phases**. Confirm the scheduled phase durations sum to the requested probe duration. Change speed mid-flight and confirm the current probe keeps its schedule while the next uses the new value.
- **Contact:** enable Scene Gizmos and **Draw Contact Gizmos**. Planted-foot markers must remain green. Read `drift`/`max` in motion logs and require **less than 0.01 m** on planted samples. Inspect the knee bend at takeoff and landing.
- **Camera:** complete zoom before the body moves; keep the character and destination visible through the apex. The reaction must settle before the result pullback. Resize and switch Whole stage/Auto zoom mid-flight to check deferred mode changes and safe framing.
- **Labels/effects:** values remain visible, occupied platforms light gold, found platforms light green, and landing adds one ripple. Reset clears the old highlight and effects.
- **Regression:** run all three searches, found and missing, plus duplicates, one value, and 40 values. Compare probe sequences and final results with the existing search checks.
- **Frame pacing:** compare equivalent standalone and embedded runs, without `RECORD`, with the WinForms control focused and the Unity area focused. Inspect frame-time percentiles and render interval; do not report 60 FPS from the cap setting alone.

From the repository root, run the existing search/form checks and the embedded animation check against the rebuilt player:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File tests/RunSearchChecks.ps1
powershell -NoProfile -ExecutionPolicy Bypass -File tests/RunFormChecks.ps1
powershell -NoProfile -ExecutionPolicy Bypass -File tests/RunEmbeddedChecks.ps1 -AppDirectory bin/Release -Animation
powershell -NoProfile -ExecutionPolicy Bypass -File tests/RunEmbeddedChecks.ps1 -AppDirectory bin/Release -Performance
```

Re-record the same Jump, Exponential, and Fibonacci not-found demonstrations with the original arrays and targets. Preserve the original recordings. Run the animation review prompt on the new recordings for a before/after comparison.

## Pose references

- Use crouch, extension, airborne silhouette, contact, and recovery as distinct readable beats. Jason Martinsen demonstrates body offsets and jump/landing poses in [Animation Mentor's jump tutorial](https://www.animationmentor.com/blog/tutorial-animate-a-jump-and-land/).
- Delay lighter secondary parts after the torso changes direction; let them recover after the main action. Drew Adams explains drag and follow-through in [Animation Mentor's overlap lesson](https://www.animationmentor.com/blog/follow-through-and-overlapping-action-the-12-basic-principles-of-animation/).

These references guide the procedural poses. No third-party animation clips were downloaded or substituted for the protected model.

## Verification results — November 1, 2026

- Unity 6000.3.26f1 Release player and WinForms Debug/Release builds passed.
- Search regressions passed **21,513 searches**, **8,520 plan roundtrips**, and form-owned probe-order checks. The three algorithm methods and shared search/playback code were not changed.
- **3,249** actual-rig samples checked support height, compression, legacy walking, pause, result hold, and all three algorithms with found/missing targets.
- **11,752** stage samples at 30/60/120 Hz checked 1 s/5 s timing, seven Animator states, idle, pause, block/scan arc differences, reset/start races, additive pose continuity, and pending view changes. Maximum measured planted-foot drift was **0.001057 m**, about **1.06 mm**.
- **5,160** camera arc samples across three aspect ratios checked framing, separate zoom/pan, fixed travel zoom, paused effects, landing ripple, and found pulse.
- Array, LinkedList, Stack, Queue, navigation, pasted-number input, and search-form layout checks passed.
- The rebuilt embedded player passed the **1000 ms and 5000 ms** interaction suite: animated idle, pixel-identical Pause captures, rapid Next, airborne Reset, queued-view cancellation, latest-session replay, found/missing outcomes, and process shutdown.
- All **25** protected model/rig and original-recording files retained their hashes.

Live embedded measurements used Intel Iris Xe / Direct3D11, 40 values, render interval 1, target 60, VSync 0, and no screenshot recording. Moving samples lasted 12 seconds each:

| Search | Average update FPS | Median frame | 95th percentile | Frames over 50 ms |
|---|---:|---:|---:|---:|
| Jump | 59.70 | 16.68 ms | 17.40 ms | 0 |
| Exponential | 59.65 | 16.68 ms | 17.59 ms | 0 |
| Fibonacci | 58.91 | 16.68 ms | 19.56 ms | 1 |

Each sampled update requested a render. These are player-loop measurements, not an external display/present trace. They show no 20 FPS host cap in these runs; the Fibonacci run still had one hitch. Screenshot recording is excluded because its synchronous PNG writes can introduce their own judder. Artistic quality still needs judgment from the finished motion, not these numerical checks.
