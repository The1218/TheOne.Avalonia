using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace TheOne.Avalonia.Workflow;

public sealed partial class WorkflowPropertyEditor
{
    private IEnumerable? _choiceSource;
    private WorkflowChoiceAdapter? _choiceAdapter;
    private INotifyCollectionChanged? _choiceCollection;
    private readonly HashSet<INotifyPropertyChanged> _choiceModels = new(ReferenceEqualityComparer.Instance);
    private Dictionary<INotifyPropertyChanged, string> _choiceModelKeys = new(ReferenceEqualityComparer.Instance);
    private Dictionary<string, WorkflowChoice> _choiceIndex = new(StringComparer.Ordinal);
    private IReadOnlyList<WorkflowChoice> _choices = [];
    private IList _selectedChoices = new ObservableCollection<WorkflowChoice>();
    private bool _syncingChoices, _choicesConfigured;
    public bool IsMultiSelect => Property.Kind == WorkflowValueKind.MultiSelect;
    public bool IsSingleValue => !IsMultiSelect;
    public IReadOnlyList<WorkflowChoice> Choices => _choices;
    public string ChoiceError { get; private set => Set(ref field, value); } = "";
    public bool HasMissingChoices => _selectedChoices.OfType<WorkflowChoice>().Any(c => !c.IsAvailable);

    /// <summary>Two-way item selection. Persisted state belongs to Property.SelectedKeys.</summary>
    public IList SelectedChoices
    {
        get => _selectedChoices;
        set
        {
            if (ReferenceEquals(value, _selectedChoices)) return;
            if (value?.Cast<object>().Any(c => c is not WorkflowChoice) == true)
                throw new ArgumentException("Selection items must be WorkflowChoice models.", nameof(value));
            if (_selectedChoices is INotifyCollectionChanged old) old.CollectionChanged -= ChoicesSelected;
            _selectedChoices = value is { IsReadOnly: false, IsFixedSize: false } ? value :
                new ObservableCollection<WorkflowChoice>(value?.Cast<WorkflowChoice>() ?? []);
            if (_choicesConfigured && _selectedChoices is INotifyCollectionChanged next) next.CollectionChanged += ChoicesSelected;
            Notify();
            ChoicesSelected(this, null);
        }
    }

    /// <summary>Configure host records with typed key/text projections. Collection and label notifications are observed.</summary>
    public void ConfigureChoices(IEnumerable? source, WorkflowChoiceAdapter? adapter)
    {
        if (_choicesConfigured && ReferenceEquals(source, _choiceSource) && ReferenceEquals(adapter, _choiceAdapter)) return;
        if (!_choicesConfigured)
        {
            _choicesConfigured = true;
            Property.PropertyChanged += ChoicePropertyChanged;
            if (_selectedChoices is INotifyCollectionChanged selected) selected.CollectionChanged += ChoicesSelected;
        }
        if (_choiceCollection is not null) _choiceCollection.CollectionChanged -= ChoicesSourceChanged;
        _choiceSource = source;
        _choiceAdapter = adapter;
        _choiceCollection = source as INotifyCollectionChanged;
        if (_choiceCollection is not null) _choiceCollection.CollectionChanged += ChoicesSourceChanged;
        RebuildChoices();
    }

    private void ChoicesSourceChanged(object? sender, NotifyCollectionChangedEventArgs e) => RebuildChoices();
    private void ChoicePropertyChanged(object? sender, PropertyChangedEventArgs e)
    { if (e.PropertyName is nameof(WorkflowProperty.SelectedKeys) or nameof(WorkflowProperty.Reference)) RefreshChoiceSelection(); }
    private void ChoiceModelChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (ChoiceError.Length == 0 && sender is INotifyPropertyChanged model &&
            _choiceModelKeys.TryGetValue(model, out var oldKey))
        {
            try
            {
                var key = _choiceAdapter?.GetKey(model) ?? ((WorkflowChoice)model).Key;
                if (key == oldKey && _choiceIndex.TryGetValue(key, out var choice))
                {
                    choice.Text = _choiceAdapter?.GetText(model) ?? ((WorkflowChoice)model).Text;
                    return;
                }
            }
            catch (ArgumentException) { }
        }
        RebuildChoices();
    }

    private void RebuildChoices()
    {
        var models = new HashSet<INotifyPropertyChanged>(ReferenceEqualityComparer.Instance);
        var modelKeys = new Dictionary<INotifyPropertyChanged, string>(ReferenceEqualityComparer.Instance);
        var index = new Dictionary<string, WorkflowChoice>(StringComparer.Ordinal);
        try
        {
            // Observe every record, including invalid ones, so correcting a key
            // can recover the catalog without replacing the collection.
            var items = new List<object?>();
            foreach (var item in _choiceSource ?? Array.Empty<object>())
            {
                items.Add(item);
                if (item is INotifyPropertyChanged model) models.Add(model);
            }
            foreach (var item in items)
            {
                if (item is null) throw new ArgumentException("选项不能为空。");
                var key = _choiceAdapter?.GetKey(item) ?? (item as WorkflowChoice)?.Key ?? throw new ArgumentException("自定义选项需要 WorkflowChoiceAdapter。");
                var text = _choiceAdapter?.GetText(item) ?? ((WorkflowChoice)item).Text;
                if (string.IsNullOrWhiteSpace(key) || index.ContainsKey(key)) throw new ArgumentException("选项 Key 不能为空或重复。");
                var choice = _choiceIndex.GetValueOrDefault(key) ?? new WorkflowChoice(key, text);
                choice.Text = text;
                choice.IsAvailable = true;
                index.Add(key, choice);
                if (item is INotifyPropertyChanged model) modelKeys[model] = key;
            }
            ChoiceError = "";
        }
        catch (Exception error) when (error is ArgumentException or InvalidOperationException)
        {
            index.Clear();
            ChoiceError = error.Message;
        }
        foreach (var model in _choiceModels.Where(m => !models.Contains(m)).ToArray()) { model.PropertyChanged -= ChoiceModelChanged; _choiceModels.Remove(model); }
        foreach (var model in models) if (_choiceModels.Add(model)) model.PropertyChanged += ChoiceModelChanged;
        _choiceIndex = index;
        _choiceModelKeys = modelKeys;
        _choices = index.Values.ToArray();
        Notify(nameof(Choices));
        RefreshChoiceSelection();
    }

    private void ChoicesSelected(object? sender, NotifyCollectionChangedEventArgs? e)
    {
        if (_syncingChoices) return;
        var choices = _selectedChoices.Cast<object>().ToArray();
        if (choices.Any(c => c is not WorkflowChoice)) { RefreshChoiceSelection(); return; }
        _syncingChoices = true;
        try { _document.SetSelectedKeys(Property, choices.Cast<WorkflowChoice>().Select(c => c.Key)); }
        finally { _syncingChoices = false; }
        RefreshChoiceSelection();
    }

    private void RefreshChoiceSelection()
    {
        if (!_choicesConfigured || _syncingChoices) return;
        var next = Property.SelectedKeys.Select(k => _choiceIndex.GetValueOrDefault(k) ??
            _selectedChoices.OfType<WorkflowChoice>().FirstOrDefault(c => c.Key == k) ?? new WorkflowChoice(k, k)).ToArray();
        foreach (var choice in next) choice.IsAvailable = _choiceIndex.ContainsKey(choice.Key);
        if (!_selectedChoices.Cast<object>().SequenceEqual(next))
        {
            _syncingChoices = true;
            try { _selectedChoices.Clear(); foreach (var choice in next) _selectedChoices.Add(choice); }
            finally { _syncingChoices = false; }
        }
        Notify(nameof(HasMissingChoices));
    }

    public void Dispose()
    {
        if (_choiceCollection is not null) _choiceCollection.CollectionChanged -= ChoicesSourceChanged;
        foreach (var model in _choiceModels) model.PropertyChanged -= ChoiceModelChanged;
        _choiceModels.Clear();
        _choiceModelKeys.Clear();
        Property.PropertyChanged -= ChoicePropertyChanged;
        if (_selectedChoices is INotifyCollectionChanged selected) selected.CollectionChanged -= ChoicesSelected;
        _choiceCollection = null;
        _choicesConfigured = false;
    }
}
