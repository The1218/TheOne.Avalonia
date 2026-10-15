# The One application mark

The rounded **O** encloses a pale teal **1**. The three solid colors follow the
library's default palette: `One.Accent` (`#006061`), `One.OnAccent` (`#F7F8F8`)
and `One.AccentSoft` (`#C1DCDC`). The transparent outer corners work on light
and dark surfaces. The mark stays fixed to these brand colors when the app
theme changes.

- `theone-app.svg`: editable 512-unit vector source.
- `theone-app.png`: 512-pixel transparent application mark.
- `theone-app.ico`: PNG-compressed 16 / 20 / 24 / 32 / 40 / 48 / 64 / 128 / 256-pixel
  images for the window icon and Windows executable resources.
- `theone-app.icns`: native macOS icon container, ready for a future app bundle.
  The current PreviewHost project does not produce a macOS app bundle.

The sidebar draws `PreviewBrandIcon` directly with Avalonia vector primitives.
Keep that geometry and this SVG in sync when editing the mark. Export raster
assets from the same rendering code after building PreviewHost:

```sh
dotnet <PreviewHost.dll> --export-brand-icon design/PreviewHost/Assets/Brand
```

Rebuild to embed the exported files, then run the focused verification:

```sh
dotnet <PreviewHost.dll> --verify-brand-icon .preview/app-icon/render
```
