# Control image provenance / 控件配图来源

These PNGs render the actual Avalonia controls and PreviewHost samples through Avalonia 12.1.4 Headless + Skia. They are documentation assets, not conceptual mockups or native desktop interaction evidence. Window images show the client area and custom title bar; OS decorations are excluded.

这些 PNG 由真实控件和 PreviewHost 示例通过 Avalonia 12.1.4 Headless + Skia 渲染，是文档素材，不是概念图，也不代表原生桌面交互验证。窗口配图仅包含客户区与自绘标题栏。

- [English visual index](../en/visual-index.md) / [中文控件图鉴](../zh-cn/visual-index.md)
- [Capture manifest / 渲染清单](capture-manifest.json): actual scene names, theme, pixel dimensions and loaded assembly SHA-256. Filenames ending in Popup, Search or Custom name capture scenarios, not additional public control types.
- [PNG SHA-256 checksums / 图片校验清单](SHA256SUMS.txt).
- Nova, Normal, Light/Dark unless a fixture explicitly overrides an option. Captions describe the fixture; code/contracts determine behavior. Shared images retain the Chinese PreviewHost fixture text, with localized guide captions.
- Lists and the icon gallery show a viewport; chart pictures show fixture data. Static frames do not establish animation smoothness, export/print parity or Windows/Linux execution.
- No upload, deletion or external business request is performed to generate attachment, dialog, report or workflow examples. Progress/status values are fixture data. The landscape image comes from the existing PreviewHost asset.

## Updating images / 更新图片

Use the normal project styles (`OneTheme`, CodeEditorTheme and MaterialIconStyles when a demo uses them). Create an isolated rendering host under `.preview/<task>/`; do not add screenshot arguments or test branches to production Program/App. The current local capture source is `.preview/docs-illustrated-20261021/Capture.csproj` and `Program.cs`. It references PreviewHost, uses actual comparison factories and business views, and adds small fixtures for standalone popup/service states.

For an affected scene, render both themes, use a bounded editor workspace or natural content height, remove trailing empty space, and inspect text, borders, selection and popup bounds. Keep the short caption in both guide languages consistent with the configuration. Do not replace actual controls with a browser approximation or invent a screenshot for a nonvisual model/API.

Current local example, from the repository root:

```sh
dotnet run --project .preview/docs-illustrated-20261021/Capture.csproj -c Release \
  --artifacts-path .preview/docs-illustrated-20261021/artifacts -- \
  --name=MultiSelectBox,MultiSelectBoxPopup --output=docs/controls/images
```

The local capture project is intentionally untracked and may be absent on another machine; this command is a maintainer example, not a prerequisite for consuming the library. The source paths and configuration here allow a new isolated renderer to be created without changing application startup.

图文区使用 `visual-guide` 标记，位于 `generated-contracts` 自动生成区之外。更新 API 文档后检查图文区保留、图片可解码、两种主题成对、相对链接有效、图中配置和说明一致。只重拍受影响场景，不以生成截图代替行为回归。
