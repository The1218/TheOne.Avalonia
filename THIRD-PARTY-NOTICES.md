# Avalonia template adaptations

ColorPicker, ColorView, ColorSpectrum, ColorSlider and ColorPreviewer use the unmodified official Avalonia.Controls.ColorPicker 12.1.4 package and its Fluent color-editing templates (Avalonia MIT license below). TheOne provides scoped dropdown-button chrome, compact panel layout, a hue-ordered default palette and density styles without changing the color model, drawing algorithms or native interaction code.

PreviewHost additionally uses Material.Icons.Avalonia 3.0.2 and Material.Icons 3.0.2 by SKProCH (MIT), https://github.com/SKProCH/Material.Icons. These are consumer-only demonstration dependencies; TheOne.Avalonia does not require the icon package.

OneWindow retains the native Avalonia 12.1.4 Window template contracts for the transparency fallback, content presenter and VisualLayerManager. TheOne adds customizable titlebar slots, native decoration roles, ThemeButton, automatic native notification hosts and its own dialog presenter/lifecycle. The native Window template is distributed under the Avalonia MIT license below.

LoginWindow retains the same native Window transparency fallback, content presenter and VisualLayerManager contracts with platform window decorations. TheOne supplies semantic appearance and an application-owned login-to-main-window transition; authentication content is provided by the consumer.

NotificationCard is adapted from Avalonia 12.1.4 Fluent resources. Native notification types, close-button routing, layout-transform contract, WindowNotificationManager expiration and callbacks are retained. TheOne supplies semantic surfaces and icons, a local INotification content template, density, and cancellable display/close motion driven by Motion.Enabled.

TransitioningContentControl is adapted from Avalonia 12.1.4 Fluent resources. The two native content presenters and native transition events are retained. TheOne adds a fixed semantic surface, density, and a cancellation-aware adapter around configured IPageTransition instances for Motion.Enabled.

Most base control files in `TheOne.Avalonia/Themes/Controls` are adapted from the Avalonia 12.1.4 Fluent templates. They retain template parts and interaction behaviour; semantic brushes and appearance are customised for TheOne.

RefreshContainer and RefreshVisualizer are adapted from Avalonia 12.1.4 Fluent resources. Native pull gestures, template hosts, compositor animation and refresh deferrals are retained; TheOne provides semantic appearance, density for the container-created visualizer, and a public-compositor adapter for the inherited Motion.Enabled setting.

Carousel is adapted from Avalonia 12.1.4 Fluent resources and retains the native VirtualizingCarouselPanel, selection, transitions and swipe recognizer. TheOne provides semantic appearance, density and a scoped mouse-input option for the native recognizer.

TabStrip and TabStripItem are adapted from Avalonia 12.1.4 Fluent templates, retaining native selection, keyboard and container contracts with a bounded virtualizing viewport and TheOne semantic styles.

FlyoutPresenter is adapted from Avalonia 12.1.4 Fluent resources, retaining the native content presenter and scrolling contract. Popup positioning, focus and dismissal remain native Flyout behavior.

Label and SelectableTextBlock are adapted from Avalonia 12.1.4 Fluent resources. Native access-key, text-layout, selection, clipboard and context-flyout behavior is retained with TheOne typography and semantic colors.

The SplitButton family retains Avalonia 12.1.4 native primary/secondary button contracts; ToggleSplitButton shares the native SplitButton style key. The rounded surface, semantic states and typed theme alias are authored for TheOne.

The DataGrid family is adapted from Avalonia.Controls.DataGrid 12.1.2 (MIT), including cell, column-header, row, row-header and row-group-header themes. Its native virtualization presenters and editing behaviour are retained. The source assembly is now maintained in vendor/Avalonia.Controls.DataGrid for native right-frozen layout, based on commit 1e12211eb0c52a09e314ca57a344147c37d0a26c. Original source headers and upstream MIT license are retained; provenance and original hashes are in vendor/Avalonia.Controls.DataGrid/UPSTREAM.json. The fork uses the distinct package ID TheOne.Avalonia.DataGrid and preserves the upstream assembly identity.

DataGrid source: https://github.com/AvaloniaUI/Avalonia.Controls.DataGrid/tree/12.1.2/src/Avalonia.Controls.DataGrid/Themes

The Calendar family, card-tab templates and interaction helpers are authored for TheOne. The card tabs reference Eremex's documented interaction model; no Eremex source code or binaries are included.

Source: https://github.com/AvaloniaUI/Avalonia/tree/12.1.4/src/Avalonia.Themes.Fluent/Controls

The MIT License (MIT)

Copyright (c) AvaloniaUI OÜ
All Rights Reserved

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
