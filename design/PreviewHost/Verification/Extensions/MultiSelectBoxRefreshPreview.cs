using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using Avalonia.Controls;
using Avalonia.Input;
using TheOne.Avalonia.Controls;

namespace TheOne.DesignPreview;

internal static partial class MultiSelectBoxPreview
{
    private static void VerifyRefreshIsolation(Action<bool, string> check)
    {
        var choices = Enumerable.Range(0, 10_000)
            .Select(index => new RefreshChoice($"Choice {index:D5}", index != 0)).ToArray();
        var source = new RefreshCollection(choices);
        var selected = new RefreshCollection([choices[0], choices[1], choices[^1]]);
        var textReads = 0;
        var enabledReads = 0;
        var box = new MultiSelectBox
        {
            ItemAdapter = MultiSelectItemAdapter.Create<RefreshChoice>(choice =>
            {
                textReads++;
                return choice.Name;
            }, choice =>
            {
                enabledReads++;
                return choice.Enabled;
            }),
            ItemsSource = source,
            SelectedItems = selected,
            MaxVisibleTags = 2
        };
        var window = Open(box);
        try
        {
            box.OpenDropDown();
            Flush();
            var tags = box.DisplayTags;
            var tagNotifications = 0;
            var entryNotifications = 0;
            var selectionNotifications = 0;
            box.PropertyChanged += (_, args) =>
            {
                if (args.Property == MultiSelectBox.DisplayTagsProperty) tagNotifications++;
            };
            box.SelectionChanged += (_, _) => selectionNotifications++;
            foreach (var entry in box.VisibleItems)
                entry.PropertyChanged += (_, _) => entryNotifications++;
            ResetWorkCounters();

            var queries = new[] { "0", "Choice 000", "00001", "00999", "missing", " Choice ", "CHOICE 00001" };
            foreach (var query in queries)
            {
                box.SearchText = query;
                Flush();
                var expected = choices.Where(choice =>
                    choice.Name.Contains(query.Trim(), StringComparison.CurrentCultureIgnoreCase));
                check(box.VisibleItems.Select(entry => entry.Item).SequenceEqual(expected)
                      && box.HasNoResults == (box.VisibleItems.Count == 0),
                    $"MultiSelectBox/refresh-isolation: cached search matches '{query}' without stale results");
                check(box.SelectedCount == 3 && box.HiddenSelectedCount == 1
                      && ReferenceEquals(box.DisplayTags, tags)
                      && box.DisplayTags.Select(entry => entry.Item).SequenceEqual(new[] { choices[0], choices[1] })
                      && box.VisibleItems.All(entry => entry.IsSelected == selected.Contains(entry.Item)),
                    "MultiSelectBox/refresh-isolation: search preserves hidden selections, tag identity and visible selection state");
            }

            check(source.Enumerations == 0 && selected.Enumerations == 0
                  && textReads == 0 && enabledReads == 0
                  && choices.Sum(choice => choice.SubscriptionAdds) == 0
                  && choices.Sum(choice => choice.SubscriptionRemoves) == 0
                  && tagNotifications == 0 && entryNotifications == 0 && selectionNotifications == 0,
                $"MultiSelectBox/refresh-isolation: 10,000 items / {queries.Length} queries; source reads={source.Enumerations}, selection reads={selected.Enumerations}, text/enabled getters={textReads}/{enabledReads}, model subscriptions +{choices.Sum(choice => choice.SubscriptionAdds)}/-{choices.Sum(choice => choice.SubscriptionRemoves)}, tag/entry/selection notifications={tagNotifications}/{entryNotifications}/{selectionNotifications}");

            box.SearchText = "0000";
            Flush();
            var search = PopupPart<TextBox>(box, "PART_SearchBox");
            var popupHost = TopLevel.GetTopLevel(search)!;
            search.Focus();
            Press(popupHost, Key.Down);
            check(!Results(box).ContainerFromIndex(0)!.IsEffectivelyEnabled
                  && Results(box).ContainerFromIndex(1)!.IsFocused,
                "MultiSelectBox/refresh-isolation: filtered keyboard navigation skips disabled cached rows");
            box.SearchText = "00002";
            Flush();
            search.Focus();
            Press(popupHost, Key.Down);
            Press(popupHost, Key.Space);
            check(box.SelectedCount == 4 && selected.Contains(choices[2]) && selected.Contains(choices[1])
                  && ReferenceEquals(Results(box).SelectedItems!.Cast<MultiSelectBoxItem>().Single().Item, choices[2]),
                "MultiSelectBox/refresh-isolation: query changes reset the active row and toggle only the newly focused model");
            box.SearchText = "missing";
            Flush();
            search.Focus();
            Press(popupHost, Key.Enter);
            box.SearchText = "00000";
            Flush();
            search.Focus();
            Press(popupHost, Key.Down);
            Press(popupHost, Key.Enter);
            check(box.SelectedCount == 4 && selected.Contains(choices[0]) && !box.ToggleItem(choices[0]),
                "MultiSelectBox/refresh-isolation: empty and disabled-only results cannot reuse a stale active selection");

            box.SearchText = "Renamed target";
            Flush();
            ResetWorkCounters();
            choices[1].Rename("Renamed target");
            Flush();
            check(box.VisibleItems.Count == 1 && ReferenceEquals(box.VisibleItems[0].Item, choices[1])
                  && box.VisibleItems[0].IsSelected && box.DisplayTags[1].Text == "Renamed target"
                  && source.Enumerations == 0 && selected.Enumerations == 0
                  && textReads == 1 && enabledReads == 1,
                $"MultiSelectBox/refresh-isolation: one model notification refreshes its cached entry and rematches the query (source/selection reads={source.Enumerations}/{selected.Enumerations}, text/enabled getters={textReads}/{enabledReads})");
            choices[1].SetEnabled(false);
            Flush();
            check(!box.VisibleItems[0].IsEnabled && !box.DisplayTags[1].CanRemove && !box.ToggleItem(choices[1]),
                "MultiSelectBox/refresh-isolation: model availability notifications update matching results and locked tags");

            box.SearchText = "New option";
            Flush();
            ResetWorkCounters();
            var added = new RefreshChoice("New option");
            source.Add(added);
            Flush();
            check(box.VisibleItems.Count == 1 && ReferenceEquals(box.VisibleItems[0].Item, added)
                  && added.ObserverCount == 1 && added.SubscriptionAdds == 1
                  && choices.All(choice => choice.SubscriptionAdds == 0 && choice.SubscriptionRemoves == 0),
                "MultiSelectBox/refresh-isolation: adding an option subscribes only the new model");
            source.Remove(added);
            Flush();
            check(box.HasNoResults && added.ObserverCount == 0 && added.SubscriptionRemoves == 1,
                "MultiSelectBox/refresh-isolation: removing an unselected option releases its model subscription");

            box.SearchText = "00003";
            var replacement = new RefreshChoice("Replacement 00003");
            source[3] = replacement;
            Flush();
            check(box.VisibleItems.Count == 1 && ReferenceEquals(box.VisibleItems[0].Item, replacement)
                  && !box.VisibleItems[0].IsSelected && choices[3].ObserverCount == 0 && replacement.ObserverCount == 1,
                "MultiSelectBox/refresh-isolation: Replace discards the old cached model without transferring its selection");
            ResetWorkCounters();
            choices[3].Rename("Old replacement notification");
            Flush();
            check(textReads == 0 && enabledReads == 0 && ReferenceEquals(box.VisibleItems[0].Item, replacement),
                "MultiSelectBox/refresh-isolation: a replaced model cannot overwrite current results");

            replacement.Rename("Silent update", false);
            box.SearchText = "Silent update";
            Flush();
            check(box.HasNoResults, "MultiSelectBox/refresh-isolation: search uses the cached text until a model notification or explicit refresh");
            box.RefreshItems();
            Flush();
            check(box.VisibleItems.Count == 1 && box.VisibleItems[0].Text == "Silent update",
                "MultiSelectBox/refresh-isolation: explicit RefreshItems rebuilds cached text for non-notifying changes");

            var resetItem = new RefreshChoice("Reset option");
            source.Reset([resetItem]);
            box.SearchText = "";
            Flush();
            check(box.VisibleItems.Count == 1 && ReferenceEquals(box.VisibleItems[0].Item, resetItem)
                  && box.SelectedCount == 4 && replacement.ObserverCount == 0 && resetItem.ObserverCount == 1
                  && choices.All(choice => choice.ObserverCount == (selected.Contains(choice) ? 1 : 0)),
                "MultiSelectBox/refresh-isolation: source Reset replaces results and retains subscriptions only for selected old models");

            var nextItem = new RefreshChoice("Next source");
            var nextSource = new RefreshCollection([nextItem]);
            var nextSelected = new RefreshCollection([nextItem]);
            box.ItemsSource = nextSource;
            box.SelectedItems = nextSelected;
            Flush();
            check(source.ObserverCount == 0 && selected.ObserverCount == 0
                  && nextSource.ObserverCount == 1 && nextSelected.ObserverCount == 1
                  && choices.All(choice => choice.ObserverCount == 0) && resetItem.ObserverCount == 0
                  && nextItem.ObserverCount == 1 && box.SelectedCount == 1,
                "MultiSelectBox/refresh-isolation: switching both sources releases old collection and model subscriptions");
            textReads = enabledReads = nextSource.Enumerations = nextSelected.Enumerations = 0;
            source.Add(new RefreshChoice("Detached source"));
            selected.Add(new RefreshChoice("Detached selection"));
            choices[1].Rename("Detached model");
            Flush();
            check(box.VisibleItems.Count == 1 && ReferenceEquals(box.VisibleItems[0].Item, nextItem)
                  && box.SelectedCount == 1 && box.DisplayTags[0].Text == "Next source"
                  && textReads == 0 && enabledReads == 0
                  && nextSource.Enumerations == 0 && nextSelected.Enumerations == 0,
                "MultiSelectBox/refresh-isolation: notifications from previous sources cannot refresh or contaminate current state");
        }
        finally
        {
            window.Close();
            Flush();
        }

        void ResetWorkCounters()
        {
            source.Enumerations = selected.Enumerations = textReads = enabledReads = 0;
            foreach (var choice in choices) choice.ResetSubscriptionCounters();
        }
    }

    private sealed class RefreshChoice(string name, bool enabled = true) : INotifyPropertyChanged
    {
        private PropertyChangedEventHandler? _changed;

        public string Name { get; private set; } = name;
        public bool Enabled { get; private set; } = enabled;
        public int SubscriptionAdds { get; private set; }
        public int SubscriptionRemoves { get; private set; }
        public int ObserverCount => _changed?.GetInvocationList().Length ?? 0;

        public event PropertyChangedEventHandler? PropertyChanged
        {
            add
            {
                _changed += value;
                SubscriptionAdds++;
            }
            remove
            {
                _changed -= value;
                SubscriptionRemoves++;
            }
        }

        public void Rename(string value, bool notify = true)
        {
            Name = value;
            if (notify) _changed?.Invoke(this, new PropertyChangedEventArgs(nameof(Name)));
        }

        public void SetEnabled(bool value)
        {
            Enabled = value;
            _changed?.Invoke(this, new PropertyChangedEventArgs(nameof(Enabled)));
        }

        public void ResetSubscriptionCounters()
        {
            SubscriptionAdds = SubscriptionRemoves = 0;
        }
    }

    private sealed class RefreshCollection(IEnumerable<RefreshChoice> items) : IList, INotifyCollectionChanged
    {
        private readonly ArrayList _items = new(items.ToArray());
        private NotifyCollectionChangedEventHandler? _changed;

        public int Enumerations { get; set; }
        public int ObserverCount => _changed?.GetInvocationList().Length ?? 0;
        public int Count => _items.Count;
        public bool IsReadOnly => false;
        public bool IsFixedSize => false;
        public bool IsSynchronized => false;
        public object SyncRoot => this;

        public event NotifyCollectionChangedEventHandler? CollectionChanged
        {
            add => _changed += value;
            remove => _changed -= value;
        }

        public object? this[int index]
        {
            get => _items[index];
            set
            {
                var previous = _items[index];
                _items[index] = value;
                _changed?.Invoke(this,
                    new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, value, previous, index));
            }
        }

        public int Add(object? value)
        {
            var index = _items.Add(value);
            _changed?.Invoke(this,
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value, index));
            return index;
        }

        public void Insert(int index, object? value)
        {
            _items.Insert(index, value);
            _changed?.Invoke(this,
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value, index));
        }

        public void Remove(object? value)
        {
            var index = _items.IndexOf(value);
            if (index >= 0) RemoveAt(index);
        }

        public void RemoveAt(int index)
        {
            var removed = _items[index];
            _items.RemoveAt(index);
            _changed?.Invoke(this,
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, removed, index));
        }

        public void Clear()
        {
            _items.Clear();
            _changed?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public void Reset(IEnumerable<RefreshChoice> replacements)
        {
            _items.Clear();
            _items.AddRange(replacements.ToArray());
            _changed?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public bool Contains(object? value) => _items.Contains(value);
        public int IndexOf(object? value) => _items.IndexOf(value);
        public void CopyTo(Array array, int index) => _items.CopyTo(array, index);

        public IEnumerator GetEnumerator()
        {
            Enumerations++;
            return _items.GetEnumerator();
        }
    }
}
