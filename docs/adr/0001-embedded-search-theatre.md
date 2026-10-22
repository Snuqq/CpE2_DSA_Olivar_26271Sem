# Embedded Unity search theatre

Accepted direction: a separate form/button per search algorithm, existing WinForms
controls, and a live Unity player inside the animation panel. Preserve the original
school-themed shell, centered fixed-size layout and black maximized background.
The inner scene uses a blue-and-gold theatre. Teaching takes priority over cinematic
speed. The user permits replacing the unrigged chibi with a rigged Furina model.

Each form owns its algorithm: JumpSearchForm, ExponentialSearchForm and
FibonacciSearchForm compute the comparison steps in their own CreateSteps methods.
This supersedes the earlier shared SearchAlgorithms implementation at the user's request.
Each form validates input and sends its completed plan over a local named
pipe. Unity only plays the supplied steps; it does not recompute the search.
Editor preview samples are exported from the same form methods by RunSearchChecks.ps1.
The forms remain ordinary Designer-compatible Forms and contain their own playback controls.
The player fills the visible space below the controls without scrollbars. No HTML host.

Verification before completion: all three forms embed a real child HWND; pause,
resume, step, reset and speed behave correctly; navigation and crashes leave no orphan
players; 1/24/40 values and integer extremes remain usable; rigged poses and transitions
are visually inspected; Debug/Release packaging works independently of repository paths;
scene hashes stay unchanged during build/preview. Ratings require this evidence and
visual judgment, not just successful compilation.
