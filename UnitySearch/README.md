# Furina search demonstration (Unity)

Use Unity 6.3 LTS (6000.3.26f1), built-in render pipeline.
Run `powershell -File scripts/BuildSearchApp.ps1` from the repository root. The script builds
Unity, checks that the authored scene was not overwritten, then builds WinForms and copies
the player to `bin/Debug/UnityPlayer`. Distribute the complete output directory. End users
do not need Unity Editor. If Hub was installed as an MSIX, the script uses its package
context for the Editor so the existing license remains visible; MSBuild runs normally.

The existing Jump, Exponential and Fibonacci forms own the inputs and buttons. Each
contains a `UnitySearchHost` that starts one child player with `-parentHWND`, communicates
over a randomly named current-user pipe, resizes the child window, and shuts it down
when the form is disposed. The Designer only displays a placeholder. A missing or
failed player reports an error in the panel; clicking the panel retries.

Each algorithm lives in its corresponding WinForms file: `JumpSearchForm.cs`,
`ExponentialSearchForm.cs`, and `FibonacciSearchForm.cs`. Each form sends
its comparison steps to Unity using `SearchPlan`; Unity only animates those steps.
`SearchPlayback` owns one clock; speed changes apply to the following step. Reset
invalidates older messages by session ID. `SearchConnection` contains background
transport, `SearchStage` coordinates playback, `SearchTheatre` manages layout and camera,
`FontaineScenery` builds the architecture and podiums, `SearchOverlay` draws teaching
information, and `FurinaActor` poses the skinned rig.

Inputs support 1–40 ascending integers, including duplicates and negative values.
Every position has a fixed place on one continuous stage, wrapping into rows of up to
eight positions in left-to-right index order. The camera starts with the
whole array, frames Furina and her destination during movement, and zooms closer as
she arrives. It returns to the overview after the result. **Whole stage** keeps all
positions visible; **Auto zoom** restores camera following, including while paused.
Platform plaques show the value above its index when zoomed in. Compact plaques and
the lower array grid separate the gold index from the value with a rule. Values too
long for a distant plaque or array cell use an ellipsis; hovering over the lower
array cell shows the complete value and index. Rear plaques overlapping Furina are hidden so labels cannot cover
her movement; their values remain in the lower array grid. There is no paging or
manual scrolling.
Gold marks a comparison, green a found result, and dimmed cells eliminated candidates.
Fibonacci offsets and splits, exponential doubling/binary midpoints, and jump block
sizes are shown before the result of each comparison.

The scenery is original procedural geometry inspired by Fontaine's Opera Epiclese:
warm parquet, limestone columns, brass arches, a circular mechanical rose, opera
galleries and velvet drapery. The platforms use bevelled brass octagons with blue
enamel and compass inlays. It is a stylized adaptation for the teaching demo, not an
extracted game environment. Visual references: [HoYoverse's Fontaine introduction](https://blog.playstation.com/2023/08/04/genshin-impact-version-4-0-launches-august-16-first-details/)
and [an in-game interior reference](https://gamershi.com/uploads/image7498trho.jpg).

Open `Assets/Search.unity` to inspect the scene in Unity. The **Create scene** menu is
only for deliberately creating a fresh scene. Build and Preview do not call it.
Model attribution is in `Assets/Resources/FurinaRig/ATTRIBUTION.md` and is copied to
`UnityPlayer/ATTRIBUTION.md` in the app output. The retired sprite renderer and unrigged
model are no longer built or packaged.

Checks: `tests/RunEmbeddedChecks.ps1` runs actual player/window/control tests and saves
rendered frames under `obj/embedded-proof`. `SearchChecks.cs` checks search results and
candidate-range metadata; `PlaybackChecks.cs` checks the clock and bounded pipe framing.
Use `-Motion` for camera/pose continuity, `-Lifecycle` for repeated real shell navigation
and crash cleanup, `-Labels` for dense integer labels and their hover tooltip,
or `-Record` for rendered motion sequences. `-AppDirectory` runs
against a relocated app folder. The `CAPTURE` and `RECORD` protocol commands record
only the player's own render for these checks.

`powershell -File scripts/BuildSearchApp.ps1 -Configuration Release` builds a release.
`powershell -File scripts/BuildSearchApp.ps1 -Preview` renders a scene preview without
changing the authored scene. Both operations verify the scene file hash.

Current verification is incremental: passing algorithm or control tests alone does not
establish animation quality, DPI behavior, or complete lifecycle robustness.
