# CarouselOptions

[English](../en/TheOne.Avalonia.Controls.CarouselOptions.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.CarouselOptions.md) · [API index](README.md)

`TheOne.Avalonia.Controls.CarouselOptions` · Class · `Avalonia.AvaloniaObject`

Enables mouse input on the native carousel swipe recognizer, independently of touch and pen.

[Source / 声明](../../../TheOne.Avalonia/Controls/CarouselOptions.cs#L9)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`IsMouseSwipeEnabled`](../../../TheOne.Avalonia/Controls/CarouselOptions.cs#L11) | `bool` | writable; OneWay; attached | `false` | Allows mouse dragging to swipe the carousel, while touch/native navigation follow Carousel semantics. |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`static bool CarouselOptions.GetIsMouseSwipeEnabled(Carousel carousel)`](../../../TheOne.Avalonia/Controls/CarouselOptions.cs#L27) | Reads the attached property. Allows mouse dragging to swipe the carousel, while touch/native navigation follow Carousel semantics. |
| [`static void CarouselOptions.SetIsMouseSwipeEnabled(Carousel carousel, bool value)`](../../../TheOne.Avalonia/Controls/CarouselOptions.cs#L28) | Sets the attached property. Allows mouse dragging to swipe the carousel, while touch/native navigation follow Carousel semantics. |
| [`CarouselOptions.CarouselOptions()`](../../../TheOne.Avalonia/Controls/CarouselOptions.cs#L9) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |

## Public fields

| Complete signature/value | Contract |
| --- | --- |
| [`static readonly AttachedProperty<bool> CarouselOptions.IsMouseSwipeEnabledProperty`](../../../TheOne.Avalonia/Controls/CarouselOptions.cs#L11) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
