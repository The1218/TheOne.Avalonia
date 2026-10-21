using Avalonia;
using Avalonia.Controls;
using Avalonia.Styling;
using TheOne.Avalonia.Controls;
using TheOne.Avalonia.Styling;

namespace TheOne.DesignPreview;

/// <summary>A bounded, selectable code example using the standalone editor package.</summary>
public sealed class PreviewCodeBlock : UserControl
{
    private readonly PreviewDeferredContent _content;
    private PreviewCodeEditor? _editor;
    public static readonly StyledProperty<string?> TextProperty =
        AvaloniaProperty.Register<PreviewCodeBlock, string?>(nameof(Text));

    public static readonly StyledProperty<CodeType> CodeTypeProperty =
        AvaloniaProperty.Register<PreviewCodeBlock, CodeType>(nameof(CodeType), CodeType.Axaml);

    public PreviewCodeBlock()
    {
        Content = _content = new PreviewDeferredContent(EnsureEditor, 108, keyboardEntry: true);
        AttachedToVisualTree += (_, _) => Refresh();
        PropertyChanged += (_, args) =>
        {
            if (args.Property == ControlSizing.SizeProperty) Refresh();
        };
        Refresh();
    }

    private PreviewCodeEditor EnsureEditor()
    {
        if (_editor is { } existing) return existing;
        _editor = new PreviewCodeEditor
        {
            IsReadOnly = true, WordWrap = true, ShowCopyButton = true, ShowFormatButton = false,
            ShowStatusBar = false, IsCompletionEnabled = false, IsInlineCompletionEnabled = false,
            IsDiagnosticsEnabled = false, IsSemanticHighlightingEnabled = false,
            IsCurrentLineHighlighted = false, MinHeight = 100
        };
        var resources = new CodeResourceCache(this);
        _editor.ColorResources = resources;
        _editor.Resources.MergedDictionaries.Add(resources);
        ResourcesChanged += (_, args) =>
        {
            resources.Refresh(args);
            // This is the only One.* resource used on the editor itself; child templates still receive native notifications.
            UpdateEditorBackground();
        };
        ActualThemeVariantChanged += (_, _) => UpdateEditorBackground();
        UpdateEditorBackground();
        Refresh();
        return _editor;
    }

    public string? Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public CodeType CodeType
    {
        get => GetValue(CodeTypeProperty);
        set => SetValue(CodeTypeProperty, value);
    }

    public CodeEditor Editor => EnsureEditor();

    internal int EditorPaletteRefreshes => _editor?.PaletteRefreshes ?? 0;

    private void UpdateEditorBackground()
    {
        if (_editor is { } editor && this.TryFindResource("One.Surface", ActualThemeVariant, out var value))
            editor.SetValue(BackgroundProperty, value);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == TextProperty || change.Property == CodeTypeProperty) Refresh();
    }

    private void Refresh()
    {
        var size = ControlSizing.GetSize(this);
        var height = Math.Clamp((Text?.Count(c => c == '\n') + 1 ?? 1) * (size == ControlSize.Large ? 24 : 21) + 62,
            108, 350);
        if (_content is not null) _content.EstimatedHeight = height;
        if (_editor is not { } editor) return;
        editor.CodeType = CodeType;
        editor.Text = Text ?? "";
        editor.Density = size switch
        {
            ControlSize.Small => CodeEditorDensity.Small, ControlSize.Large => CodeEditorDensity.Large,
            _ => CodeEditorDensity.Normal
        };
        editor.Height = height;
    }

    // The package observes each editor color separately and resets text layouts for every notification.
    // Palette-only changes do not alter these syntax colors. Filter only the package's own observers;
    // retain Avalonia's resource propagation to its child controls and all real editor color changes.
    private sealed class PreviewCodeEditor : CodeEditor, IResourceHost
    {
        internal CodeResourceCache? ColorResources;

        public PreviewCodeEditor()
        {
            ResourcesChanged += (_, args) =>
            {
                if (ColorResources is { } cache && cache.Sequence == args.SequenceNumber && !cache.Changed) return;
                PaletteRefreshes++;
                EditorResourcesChanged?.Invoke(this, args);
            };
        }

        internal int PaletteRefreshes { get; private set; }
        protected override Type StyleKeyOverride => typeof(CodeEditor);

        event EventHandler<ResourcesChangedEventArgs>? IResourceHost.ResourcesChanged
        {
            add => EditorResourcesChanged += value;
            remove => EditorResourcesChanged -= value;
        }

        private event EventHandler<ResourcesChangedEventArgs>? EditorResourcesChanged;
    }

    private sealed class CodeResourceCache(PreviewCodeBlock host) : ResourceProvider
    {
        private readonly Dictionary<(object, ThemeVariant?), object?> _values = [];
        private ThemeVariant? _variant;
        public override bool HasResources => true;
        internal int Sequence { get; private set; }
        internal bool Changed { get; private set; } = true;

        internal void Refresh(ResourcesChangedEventArgs args)
        {
            Sequence = args.SequenceNumber;
            Changed = _values.Count == 0 || _variant != host.ActualThemeVariant;
            _variant = host.ActualThemeVariant;
            foreach (var key in _values.Keys.ToArray())
            {
                host.TryFindResource(key.Item1, key.Item2, out var value);
                if (!Equals(_values[key], value)) Changed = true;
                _values[key] = value;
            }
        }

        public override bool TryGetResource(object key, ThemeVariant? theme, out object? value)
        {
            value = null;
            if (key is not string name || !name.StartsWith("CodeEditor.", StringComparison.Ordinal)) return false;
            if (_values.TryGetValue((key, theme), out value)) return true;
            if (!host.TryFindResource(key, theme, out value)) return false;
            _values[(key, theme)] = value;
            return true;
        }
    }
}
