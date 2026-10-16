# Item surface spacing

[简体中文](../zh-cn/item-spacing.md) · **English**

Selectable list and navigation surfaces keep at least 4px clear of their enclosing frame and 4px between neighboring surfaces. Existing larger popup padding is retained. Horizontal strips use 2px on either side of each item to create a 4px gap; their parent provides the edge gutter. Apply spacing to the visible header rather than an entire nested branch when margins would accumulate with depth.

| Family | Implementation |
| --- | --- |
| ListBoxItem / ComboBoxItem | `Margin="4,2"` |
| TreeViewItem / NavMenuItem | Header surface `PART_LayoutRoot` uses `Margin="4,2"`; child indentation stays native |
| AutoCompleteBox / MultiSelectBox results | Inherit ListBoxItem spacing |
| Vertical MenuItem | `Margin="0,2"`; ContextMenu and submenu frames retain 6px padding |
| Menu / horizontal menu flyout | Items use `Margin="2"`; the menu bar adds 2px parent padding, flyouts retain 6px padding |
| MenuFlyoutPresenter | Default 6px padding is forwarded to its border; explicit Padding overrides remain effective |
| Standard TabControl / TabItem | Header scroll area has 2px padding and items have 2px margin, including Top / Bottom / Left / Right placement |
| TabStripItem | `Margin="2"` supports horizontal and vertical panels; TabStrip retains its existing 4px padding |

The menu bar has a minimum height and grows with its items, allowing all three densities to retain their gutters. Explicit item Margin and presenter Padding can override defaults.

The audit also checked these separate layouts:

- Calendar cells already have 2px margins, producing 4px gaps on their two-dimensional grid.
- Pagination buttons have 6px spacing; FlowStep has explicit node spacing. VerificationStep uses indicator connectors rather than adjacent filled selection surfaces.
- Card tabs preserve their separator and selected-tab seam. DataGrid cells preserve continuous grid/data alignment. Gantt's derived row theme explicitly sets zero Margin to avoid inheriting list gaps and shifting the timeline grid. Carousel pages preserve viewport paging. These structures do not receive list margins.
- Generic ItemsControl has caller-owned content and no default selected surface; its template does not add arbitrary item margins. WorkflowEditor uses the shared TabStrip theme and explicit spacing for its node library.

PreviewHost provides independent checks; run only affected families:

```text
--verify-menu-item-spacing [output]
--verify-tab-item-spacing [output]
--verify-tabstrip-item-spacing [output]
--verify-gantt [output]
```

They cover light/dark, Small/Normal/Large, wide/narrow viewports, actual surface/gap pixels, disabled items, and native key/pointer interaction. Menu checks include ContextMenu, vertical/horizontal flyouts, and nested submenus. TabControl checks all four placements; TabStrip checks both panel orientations.
