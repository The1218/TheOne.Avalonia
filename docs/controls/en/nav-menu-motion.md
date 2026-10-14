# NavMenu motion

[简体中文](../zh-cn/nav-menu-motion.md) · **English**

See [NavMenu](nav-menu.md) for basic setup.

## Expanding, collapsing, and switching the sidebar

`TransitionDuration` is a styleable TimeSpan that supports compiled binding. The theme defaults to `One.Motion.Expand` (200 ms); collapse takes 70% of that duration. Zero switches immediately. `Motion.Enabled=False` immediately reaches the current target even during animation; re-enabling it does not replay prior actions.

Inline expansion of a thousand children travels across a bounded viewport while reusing the full child layout. It does not animate tens of thousands of list pixels, avoiding abrupt first-screen appearance or delayed completion. The full native scrolling range returns when complete; child hit testing is disabled immediately during collapse. Rapid reversals continue from current progress. Explicit SelectPath / distant keyboard navigation completes ancestor transitions before locating the item. Read-only TransitionExtent reports the menu's bounded viewport height without changing the final list extent.

Sidebar sizing and navigation mode update together. Icons already in view translate from their current rendered position to the new one; labels fade in on expansion. Page Width / Height do not animate per frame, and icons / text do not scale. A single progress animation drives the change; recycled items do not replay entry animations. Size changes, disabling motion, unloading, and recycling clear temporary transforms and opacity while preserving selection and business expansion state.

PreviewHost's dedicated NavMenu page provides actions to expand / collapse a thousand children, expand / collapse three levels, and rapidly reverse the sidebar. The top motion switch tests disabling motion midway. Verification uses real Dispatcher frames and Skia advancement, covering Light / Dark × three sizes, actual thousand-item recycling, intermediate frames, reversal, full extent restoration, focus / selection, and unload cleanup:

See the [preview entry points](../../../design/PreviewHost/Program.cs) for commands and scope.

Targeted rendering includes intermediate collapse, expansion, and sidebar frames, plus built-in search suggestions and the collapsed search icon / popup. The current NavMenu set contains 66 images and does not render unrelated controls.
