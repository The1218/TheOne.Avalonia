# Tag 与 VerificationStep

**简体中文** · [English](../en/tag-verification-step.md)

[返回控件索引](README.md)。加载 OneTheme 后可用，命名空间为 `xmlns:one="using:TheOne.Avalonia.Controls"`。

## Tag 状态标签

```xml
<one:Tag Content="已完成" Status="Success" Variant="Soft">
  <one:Tag.Icon>
    <PathIcon Width="14" Height="14" Data="M1,7 L3,5 L5,8 L11,1 L13,3 L5,12 Z" />
  </one:Tag.Icon>
</one:Tag>
<one:Tag Content="处理中" Status="Processing" Variant="Outline" />
<one:Tag Content="错误" Status="Error" Variant="Solid" IsEnabled="False" />
```

| 属性 | 说明 |
| --- | --- |
| `Content` / `ContentTemplate` | 文字、控件或有类型的数据模板 |
| `Icon` / `IconTemplate` | 可选图标槽，支持 PathIcon、Image 或模型模板；保留控件逻辑树与 DataContext |
| `Status` | `Default`、`Information`、`Success`、`Warning`、`Error`、`Processing` |
| `Variant` | `Soft` 浅底、`Outline` 描边、`Solid` 实心 |
| `Spacing` | 图标与内容间距，默认 6；没有图标或文字时不保留空隙 |
| `IconSize` | 图标缩放边长，默认 13 px，比原 14 px 基准减小一号；三档密度共用，可覆盖，不修改调用方图标尺寸 |
| `IsEnabled` | 禁用时统一使用 DisabledSurface / DisabledText |

Tag 用于显示状态，不承担按钮或切换选择行为。状态文字与图标由应用提供，不能只通过颜色表达含义。沿用 TheOne 语义色，支持 Light / Dark 与继承的 small / normal / large；默认最小高度分别为 24 / 28 / 32，字号为 11 / 12 / 15 px。可覆盖标准 Padding、CornerRadius、Foreground 等属性。

## VerificationStep 带验证的 Step

普通编辑页可使用 [Form / FormItem](form.md)，复用本页的校验器与错误类型。`FormItem` 也可用于步骤的自定义内容，按最近校验宿主显示字段错误。

`VerificationStep` 是带验证的 Step：当前步骤验证通过后才能向前导航。它替代原有 `Step` 控件，C# 类型引用和 XAML 标签需改为 `VerificationStep`。配套的 `StepItem`、`StepField`、`StepValidation` API 名称和行为保持不变。

默认用法只需步骤模型、常见验证属性和 ItemsSource。`StepItem<T>` 自动创建属性验证器，并在没有自定义内容模板时生成表单、字段标签及输入下方的错误提示；无需声明 StepField 或逐步调用 StepValidation.For。T 使用模型的具体类型，公共属性的 NativeAOT 元数据由控件库保留。

```csharp
using System.ComponentModel.DataAnnotations;
using TheOne.Avalonia.Controls;

public sealed class AccountModel
{
    [Display(Name = "姓名", Prompt = "请输入姓名", Order = 0)]
    [Required(ErrorMessage = "请输入姓名。")]
    [StringLength(24, MinimumLength = 2)]
    public string Name { get; set; } = "";

    [Display(Name = "年龄", Order = 1)]
    [Range(18, 120, ErrorMessage = "年龄需在 18–120 之间。")]
    public int Age { get; set; } = 18;
}

// 宿主 VM 暴露此集合以及已有的 ICommand CompletedCommand。
public IReadOnlyList<StepItem> Steps { get; } = new StepItem[]
{
    new StepItem<AccountModel>(new()) { Title = "基本资料", Description = "填写必填信息" },
    new StepItem<AccountModel>(new()) { Title = "联系信息", Description = "填写另一组资料" }
};
```

```xml
<!-- 根视图声明 x:DataType，并使用编译绑定。 -->
<one:VerificationStep ItemsSource="{Binding Steps}"
          CompletedCommand="{Binding CompletedCommand}"
          Height="520" />
```

表单通过编译绑定路径把输入直接写入模型。普通属性可在前进时校验；建议模型实现 INotifyPropertyChanged 或使用现有 MVVM 属性通知基类，以便模型的外部修改更新输入、取消待完成验证并使后续进度失效。各步骤可以使用不同的具体模型类型，集合仍为 StepItem 集合。

### 内置表单与属性规则

| 属性 / 特性 | 默认行为 |
| --- | --- |
| 公共、可读写的非索引实例属性 | 按声明顺序生成字段；不扫描程序集或预创建其他步骤视图 |
| string、数字、char、Guid、DateTime、DateTimeOffset、TimeSpan | TextBox，使用目标属性类型转换；转换失败也阻止前进 |
| bool / bool? | CheckBox；可空布尔支持三态 |
| Display(Name / Prompt / Description / Order) | 标签、占位提示、说明、排序；可省略 |
| Display(AutoGenerateField = false) | 不生成输入；其验证规则仍生效，错误显示在整步区域 |
| DataType.Password / MultilineText | 密码输入／多行输入 |
| Editable(false) | 字段只读；不取消其验证规则 |
| 其他属性类型 | 只读文本；编辑集合、复杂对象、枚举选择等请提供自定义内容模板 |

支持标准 ValidationAttribute，包括 Required、Range、StringLength、MinLength、MaxLength、EmailAddress、RegularExpression、AllowedValues 等。Required 失败时不重复报告同字段的其他规则；错误显示在各自输入下方。自定义 ValidationAttribute 与 IValidatableObject 同样可用；跨字段结果应带 MemberNames，未指定成员名的结果属于整步。

普通 `[Required]` 对 false 不报错；必须勾选的确认框可使用 `[AllowedValues(true)]`。Range 的 int / double 构造函数兼容本项目的 NativeAOT；`Range(Type, string, string)` 自带通用 TypeConverter 的裁剪警告，不能通过忽略警告处理。

输入转换失败时，模型仍可能保留上次合法值。控件会保存无效草稿和转换错误，在字段容器回收后仍阻止完成，并在重新显示时恢复草稿。外部模型的对应属性通知会替换过时草稿。内置表单为有界 ListBox / VirtualizingStackPanel，仅实例化可见字段；字段错误列表也有界并回收。

邮箱、密码等一致性校验可使用本库的 StepCompareAttribute：

```csharp
[Required, EmailAddress]
public string Email { get; set; } = "";

[Required]
[StepCompare(nameof(Email), ErrorMessage = "两次邮箱输入不一致。")]
public string ConfirmEmail { get; set; } = "";
```

StepCompare 通过具体模型的属性访问器比较最新值。标准 CompareAttribute 自身带裁剪警告，NativeAOT 应使用 StepCompare；自定义 attribute 若反射其他类型，仍需满足自身的裁剪契约。

### 自定义内容与高级校验

`VerificationStep.ContentTemplate` 优先，其次使用匹配的 DataTemplate；没有匹配模板才使用内置表单。`StepItem<T>` 在自定义布局下仍自动校验模型属性。自定义布局若要在指定输入下展示错误，可使用 StepField，其 MemberName 对应模型属性；整步、隐藏字段或找不到承载位置的错误保留在顶部有界列表中，避免错误丢失。

```xml
<one:VerificationStep ItemsSource="{Binding Steps}" CompletedCommand="{Binding CompletedCommand}">
  <one:VerificationStep.DataTemplates>
    <DataTemplate DataType="{x:Type vm:AccountModel}" x:DataType="vm:AccountModel">
      <one:StepField MemberName="Name">
        <TextBox Text="{Binding Name, Mode=TwoWay}" />
      </one:StepField>
    </DataTemplate>
  </one:VerificationStep.DataTemplates>
</one:VerificationStep>
```

原有非泛型 StepItem 保留，可承载开发者自有控件或模型。它不猜测 object 的运行时属性类型；需要属性校验时显式设置 `Validator = StepValidation.For(model)`。`StepValidation.For(model, nameof(AccountModel.Name))` 可限制当前步的属性范围；用具体模型类型调用，不以 object / 基类隐藏实际类型。泛型 StepItem 的 Validator 非空时覆盖默认属性验证器，适用于范围限制或自定义远程验证。

实现 IStepValidator 可异步校验，返回 IReadOnlyList&lt;StepValidationError&gt; 并响应 CancellationToken。注册模型应与输入实际绑定的模型一致；更换模型时设置对应 StepItem.Content，泛型步骤会自动更新默认验证器和表单，并拒绝 null 或不匹配的模型类型。

### 完成命令、导航与生命周期

完成处理绑定 VM 的 ICommand CompletedCommand，可通过 CompletedCommandParameter 传业务参数。最后一步也必须验证成功，命令只执行一次；CanExecute=false 禁用完成按钮并阻止直接调用完成，异步校验返回后再次检查。完成状态先于命令执行设置。命令的异步业务提交和异常由应用处理；IsBusy 表示步骤校验，不等待命令内部的异步业务操作。

| API | 行为 |
| --- | --- |
| ItemsSource | IEnumerable&lt;StepItem&gt;；默认使用 StepItem&lt;具体模型类型&gt;；集合变化使用 ObservableCollection，每项保持独立身份 |
| SelectedIndex / CurrentContent / CurrentContentTemplate | 只读当前位置、模型／内容、实际内容模板；不能设置索引绕过校验 |
| ContentTemplate | 可选自定义内容模板 |
| GoNextAsync(token) | 校验当前步，通过后前进或完成，返回 bool |
| GoBack() / GoToAsync(index, token) | 可返回先前步骤；前进仅限相邻下一步并验证，不能跳步 |
| Reset() | 重置进度与选择，不清空业务数据或尚未修正的输入草稿 |
| IsBusy / IsCompleted | 校验中与最终完成状态 |
| ValidationErrors / HasErrors | 全部验证错误 |
| UnmappedErrors / HasUnmappedErrors | 整步或没有对应输入承载位置的错误 |
| ValidationFailed | 失败的冒泡事件；完成使用 CompletedCommand |
| NextText / PreviousText / CompleteText | 默认“下一步”／“上一步”／“完成” |
| HeaderHorizontalAlignment | 默认 Center，与内容同轴居中；Stretch 均分可用宽度；也支持 Left / Right |
| HeaderMaxWidth | 标题视口宽度上限，默认 960，必须为有限正数 |

导航及 UI 模型变更在 UI 线程执行。重复前进不会启动第二次校验；更换集合、Content 或 Validator 会重置进度。后退、重置、模型通知或离开视觉树取消进行中的校验，即使验证器忽略取消，也不会由迟到结果推进步骤或执行完成命令。服务异常显示为整步错误并允许重试。控件离开视觉树释放模型和命令订阅。

### 布局与预览

圆点连线导航区分完成勾号、当前实心编号、未开始浅色编号和错误感叹号，沿用 TheOne 语义色。圆点 small / normal / large 直径为 30 / 36 / 42；标题视口高度 112、最大宽度 960，默认居中；每项默认宽度 200。设置 `HeaderHorizontalAlignment="Stretch"` 时均分视口，每项最小宽度 120；较窄窗口或长列表保持横向滚动，不压缩成不可读的圆点。调整窗口或切换对齐实时重算项宽，仍使用官方 ListBox / VirtualizingStackPanel。成功状态保存在 StepItem 上，不依赖已创建的标题容器。

内置表单支持原生 `Tab` / `Shift+Tab` 在输入项间切换焦点，遵循禁用状态与 `KeyboardNavigation.IsTabStop`，末尾字段可继续移到导航按钮。专用 `One.StepForm` 主题使用 `KeyboardNavigation.TabNavigation="Continue"`，不改变普通 ListBox 的导航模式；自定义内容模板仍保留原生焦点行为。

VerificationStep 默认最大高度 720；默认表单最大宽度 640，使用内容区域的有限高度。步骤条与表单默认共享水平中心；若要二者等宽，可设置 `HeaderHorizontalAlignment="Stretch" HeaderMaxWidth="640"`。预览页可直接切换四种对齐方式。自定义内容的长表单使用 ScrollViewer；集合控件保持自己的有界视口。字段错误列表最大高度 96，顶部整步错误区域最大高度 120。

默认模板保留 PART_Steps、PART_ContentPresenter、PART_PreviousButton、PART_NextButton、PART_ValidationErrors；内置表单使用 PART_FormFields。预览“扩展控件 / Step / VerificationStep”直接演示自动表单、标准属性校验、完成命令、重置及 1,000 步回收。验证命令见[预览专项入口](../../../design/PreviewHost/Program.cs)。

只验证 VerificationStep 时运行 `dotnet run --project design/PreviewHost -- --verify-verification-step`，包含真实 Tab / Shift+Tab 按键检查；渲染入口使用 `--render-controls .preview/verification-step VerificationStep`。发布后的 NativeAotSmoke 原生消费程序使用 `--require-aot --verify-verification-step` 执行相同验证。

<!-- generated-contracts:start -->
## 额外属性、样式与完整 API

本节按源码声明列出本库增加的成员；原生控件继承属性仍使用 Avalonia API。注册默认值与主题生效值不同，显式局部值优先。模型、方法、事件、枚举的完整签名可从下表的类型链接查看。PreviewHost 的“控件能力实验室”（各页右上入口）按样式、属性、API／事件和模型分类演示；复杂模型使用原业务示例。

### IStepValidator

[API / model reference](../../api/zh-cn/TheOne.Avalonia.Controls.IStepValidator.md)

没有在该类型上另外声明 CLR／附加属性；继承成员见其基类。

### StepCompareAttribute

[API / model reference](../../api/zh-cn/TheOne.Avalonia.Controls.StepCompareAttribute.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`OtherProperty`](../../../TheOne.Avalonia/Controls/StepCompareAttribute.cs#L10) | `string` | 只读; CLR / n/a | `accessor-defined / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |

### StepField

[API / model reference](../../api/zh-cn/TheOne.Avalonia.Controls.StepField.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Errors`](../../../TheOne.Avalonia/Controls/StepField.cs#L16) | `System.Collections.Generic.IReadOnlyList<TheOne.Avalonia.Controls.StepValidationError>` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`HasErrors`](../../../TheOne.Avalonia/Controls/StepField.cs#L17) | `bool` | 只读; OneWay | `computed / state` | 全部验证错误 |
| [`MemberName`](../../../TheOne.Avalonia/Controls/StepField.cs#L15) | `string` | 可写; OneWay | `""` | 对应模型成员的准确名称；用于将校验错误映射到字段，不是显示标签。 |

### StepValidation

[API / model reference](../../api/zh-cn/TheOne.Avalonia.Controls.StepValidation.md)

没有在该类型上另外声明 CLR／附加属性；继承成员见其基类。

### StepValidationError

[API / model reference](../../api/zh-cn/TheOne.Avalonia.Controls.StepValidationError.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`MemberName`](../../../TheOne.Avalonia/Controls/StepValidation.cs#L7) | `string` | 可写; CLR / init-only | `required constructor argument` | 继承自 `StepField`，匹配错误中的属性名，如 `Name`；不会作为反射路径执行 |
| [`Message`](../../../TheOne.Avalonia/Controls/StepValidation.cs#L7) | `string` | 可写; CLR / init-only | `required constructor argument` | 可供用户或诊断展示的结果说明；操作结果仍以状态字段为准。 |

### Tag

[API / model reference](../../api/zh-cn/TheOne.Avalonia.Controls.Tag.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Icon`](../../../TheOne.Avalonia/Controls/Tag.cs#L23) | `object?` | 可写; OneWay | `null` | 可选图标槽，支持 PathIcon、Image 或模型模板；保留控件逻辑树与 DataContext |
| [`IconSize`](../../../TheOne.Avalonia/Controls/Tag.cs#L21) | `double` | 可写; OneWay | `13` | 图标缩放边长，默认 13 px，比原 14 px 基准减小一号；三档密度共用，可覆盖，不修改调用方图标尺寸 校验：`value => double.IsFinite(value) && value > 0` |
| [`IconTemplate`](../../../TheOne.Avalonia/Controls/Tag.cs#L24) | `Avalonia.Controls.Templates.IDataTemplate?` | 可写; OneWay | `null` | 可选图标槽，支持 PathIcon、Image 或模型模板；保留控件逻辑树与 DataContext |
| [`Spacing`](../../../TheOne.Avalonia/Controls/Tag.cs#L27) | `double` | 可写; OneWay | `6` | 图标与内容间距，默认 6；没有图标或文字时不保留空隙 校验：`value => double.IsFinite(value) && value >= 0` |
| [`Status`](../../../TheOne.Avalonia/Controls/Tag.cs#L25) | `TheOne.Avalonia.Controls.TagStatus` | 可写; OneWay | `0 / default` | `Default`、`Information`、`Success`、`Warning`、`Error`、`Processing` 校验：`Enum.IsDefined` 可选值：`Default`, `Information`, `Success`, `Warning`, `Error`, `Processing` |
| [`Variant`](../../../TheOne.Avalonia/Controls/Tag.cs#L26) | `TheOne.Avalonia.Controls.TagVariant` | 可写; OneWay | `0 / default` | `Soft` 浅底、`Outline` 描边、`Solid` 实心 校验：`Enum.IsDefined` 可选值：`Soft`, `Outline`, `Solid` |

### VerificationStep

[API / model reference](../../api/zh-cn/TheOne.Avalonia.Controls.VerificationStep.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`CanGoBack`](../../../TheOne.Avalonia/Controls/VerificationStep.cs#L73) | `bool` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`CanGoNext`](../../../TheOne.Avalonia/Controls/VerificationStep.cs#L74) | `bool` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`CompleteText`](../../../TheOne.Avalonia/Controls/VerificationStep.cs#L66) | `string` | 可写; OneWay | `"完成"` | 默认“下一步”／“上一步”／“完成” |
| [`CompletedCommand`](../../../TheOne.Avalonia/Controls/VerificationStep.cs#L78) | `System.Windows.Input.ICommand?` | 可写; OneWay | `null` | 宿主提供的命令入口；具体操作与权限由业务实现，配对的 CommandParameter 作为命令参数。 |
| [`CompletedCommandParameter`](../../../TheOne.Avalonia/Controls/VerificationStep.cs#L79) | `object?` | 可写; OneWay | `null` | 校验通过并完成步骤时传给 CompletedCommand 的业务参数。 |
| [`ContentTemplate`](../../../TheOne.Avalonia/Controls/VerificationStep.cs#L62) | `Avalonia.Controls.Templates.IDataTemplate?` | 可写; OneWay | `null` | 文字、控件或有类型的数据模板 |
| [`CurrentContent`](../../../TheOne.Avalonia/Controls/VerificationStep.cs#L69) | `object?` | 只读; OneWay | `computed / state` | 只读当前位置、模型／内容、实际内容模板；不能设置索引绕过校验 |
| [`CurrentContentTemplate`](../../../TheOne.Avalonia/Controls/VerificationStep.cs#L70) | `Avalonia.Controls.Templates.IDataTemplate?` | 只读; OneWay | `computed / state` | 只读当前位置、模型／内容、实际内容模板；不能设置索引绕过校验 |
| [`ForwardText`](../../../TheOne.Avalonia/Controls/VerificationStep.cs#L75) | `string` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`HasErrors`](../../../TheOne.Avalonia/Controls/VerificationStep.cs#L77) | `bool` | 只读; OneWay | `computed / state` | 全部验证错误 |
| [`HasUnmappedErrors`](../../../TheOne.Avalonia/Controls/VerificationStep.cs#L81) | `bool` | 只读; OneWay | `computed / state` | 整步或没有对应输入承载位置的错误 |
| [`HeaderHorizontalAlignment`](../../../TheOne.Avalonia/Controls/VerificationStep.cs#L64) | `Avalonia.Layout.HorizontalAlignment` | 可写; OneWay | `HorizontalAlignment.Center` | 默认 Center，与内容同轴居中；Stretch 均分可用宽度；也支持 Left / Right 校验：`Enum.IsDefined` 可选值：`Stretch`, `Left`, `Center`, `Right` |
| [`HeaderMaxWidth`](../../../TheOne.Avalonia/Controls/VerificationStep.cs#L63) | `double` | 可写; OneWay | `960` | 标题视口宽度上限，默认 960，必须为有限正数 校验：`value => double.IsFinite(value) && value > 0` |
| [`IsBusy`](../../../TheOne.Avalonia/Controls/VerificationStep.cs#L71) | `bool` | 只读; OneWay | `computed / state` | 校验中与最终完成状态 |
| [`IsCompleted`](../../../TheOne.Avalonia/Controls/VerificationStep.cs#L72) | `bool` | 只读; OneWay | `computed / state` | 校验中与最终完成状态 |
| [`ItemsSource`](../../../TheOne.Avalonia/Controls/VerificationStep.cs#L61) | `System.Collections.Generic.IEnumerable<TheOne.Avalonia.Controls.StepItem>?` | 可写; OneWay | `null` | IEnumerable&lt;StepItem&gt;；默认使用 StepItem&lt;具体模型类型&gt;；集合变化使用 ObservableCollection，每项保持独立身份 |
| [`NextText`](../../../TheOne.Avalonia/Controls/VerificationStep.cs#L65) | `string` | 可写; OneWay | `"下一步"` | 默认“下一步”／“上一步”／“完成” |
| [`PreviousText`](../../../TheOne.Avalonia/Controls/VerificationStep.cs#L67) | `string` | 可写; OneWay | `"上一步"` | 默认“下一步”／“上一步”／“完成” |
| [`SelectedIndex`](../../../TheOne.Avalonia/Controls/VerificationStep.cs#L68) | `int` | 只读; OneWay | `computed / state` | 只读当前位置、模型／内容、实际内容模板；不能设置索引绕过校验 |
| [`UnmappedErrors`](../../../TheOne.Avalonia/Controls/VerificationStep.cs#L80) | `System.Collections.Generic.IReadOnlyList<TheOne.Avalonia.Controls.StepValidationError>` | 只读; OneWay | `computed / state` | 整步或没有对应输入承载位置的错误 |
| [`ValidationErrors`](../../../TheOne.Avalonia/Controls/VerificationStep.cs#L76) | `System.Collections.Generic.IReadOnlyList<TheOne.Avalonia.Controls.StepValidationError>` | 只读; OneWay | `computed / state` | 全部验证错误 |

### 公共附加属性

[ControlSizing](../../api/zh-cn/TheOne.Avalonia.Styling.ControlSizing.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Size`](../../../TheOne.Avalonia/Styling/ControlSizing.cs#L11) | `TheOne.Avalonia.Styling.ControlSize` | 可写; OneWay; 附加属性; 向后代继承 | `ControlSize.Normal` | Small / Normal / Large 共享密度，向后代继承；显式单控件值或尺寸类可覆盖。 可选值：`Small`, `Normal`, `Large` |

[Motion](../../api/zh-cn/TheOne.Avalonia.Styling.Motion.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Enabled`](../../../TheOne.Avalonia/Styling/Motion.cs#L9) | `bool` | 可写; OneWay; 附加属性; 向后代继承 | `true` | 向后代继承的动效开关；关闭时转入最终布局和状态，保留原生输入。 |

### Style 类与自动状态

| 控件 | 类名 | 使用边界 | 生效 Setter／资源 |
| --- | --- | --- | --- |
| Tag | `one-large` | 模板自动类；不手动绑定 | one&#124;Tag.one-small, one&#124;Tag.one-normal, one&#124;Tag.one-large => MinHeight=0; Padding=4,0; ^.one-large => MinHeight=32; FontSize=15; Padding=11,4 |
| Tag | `one-normal` | 模板自动类；不手动绑定 | one&#124;Tag.one-small, one&#124;Tag.one-normal, one&#124;Tag.one-large => MinHeight=0; Padding=4,0 |
| Tag | `one-small` | 模板自动类；不手动绑定 | one&#124;Tag.one-small, one&#124;Tag.one-normal, one&#124;Tag.one-large => MinHeight=0; Padding=4,0; ^.one-small => MinHeight=24; FontSize=11; Padding=7,2 |
| Tag | `step-indicator` | 模板自动类；不手动绑定 | one&#124;Tag.step-indicator => Width=36; Height=36; one&#124;Tag.step-indicator.one-small => Width=30; Height=30; one&#124;Tag.step-indicator.one-large => Width=42; Height=42 |
| VerificationStep | `one-large` | 模板自动类；不手动绑定 | one&#124;Tag.step-indicator.one-large => Width=42; Height=42; ^.one-large => FontSize=16; Padding=24 |
| VerificationStep | `one-small` | 模板自动类；不手动绑定 | one&#124;Tag.step-indicator.one-small => Width=30; Height=30; ^.one-small => FontSize=12; Padding=16 |

`small` / `normal` / `large` 是公开密度入口；`one-small` / `one-normal` / `one-large` 为自动生成的内部类。`:pointerover`、`:pressed`、`:disabled`、`:selected`、`:error` 等是由输入或状态驱动的伪类，不是手工添加的 Classes。未列出额外外观类的控件使用默认主题及其属性。

### API、事件与内置模型

| 类型 | 类别 | 属性／枚举值 | 方法／构造 | 事件 |
| --- | --- | ---: | ---: | ---: |
| [`TheOne.Avalonia.Controls.IStepValidator`](../../api/zh-cn/TheOne.Avalonia.Controls.IStepValidator.md) | Interface | 0 | 1 | 0 |
| [`TheOne.Avalonia.Controls.StepCompareAttribute`](../../api/zh-cn/TheOne.Avalonia.Controls.StepCompareAttribute.md) | Class | 1 | 1 | 0 |
| [`TheOne.Avalonia.Controls.StepField`](../../api/zh-cn/TheOne.Avalonia.Controls.StepField.md) | Class | 3 | 1 | 0 |
| [`TheOne.Avalonia.Controls.StepItem`](../../api/zh-cn/TheOne.Avalonia.Controls.StepItem.md) | Class | 13 | 1 | 1 |
| [`TheOne.Avalonia.Controls.StepItem<T>`](../../api/zh-cn/TheOne.Avalonia.Controls.StepItem_T_.md) | Class | 0 | 1 | 0 |
| [`TheOne.Avalonia.Controls.StepState`](../../api/zh-cn/TheOne.Avalonia.Controls.StepState.md) | Enum | 4 | 1 | 0 |
| [`TheOne.Avalonia.Controls.StepValidation`](../../api/zh-cn/TheOne.Avalonia.Controls.StepValidation.md) | Class | 0 | 1 | 0 |
| [`TheOne.Avalonia.Controls.StepValidationError`](../../api/zh-cn/TheOne.Avalonia.Controls.StepValidationError.md) | Class | 2 | 1 | 0 |
| [`TheOne.Avalonia.Controls.Tag`](../../api/zh-cn/TheOne.Avalonia.Controls.Tag.md) | Class | 6 | 1 | 0 |
| [`TheOne.Avalonia.Controls.TagStatus`](../../api/zh-cn/TheOne.Avalonia.Controls.TagStatus.md) | Enum | 6 | 1 | 0 |
| [`TheOne.Avalonia.Controls.TagVariant`](../../api/zh-cn/TheOne.Avalonia.Controls.TagVariant.md) | Enum | 3 | 1 | 0 |
| [`TheOne.Avalonia.Controls.VerificationStep`](../../api/zh-cn/TheOne.Avalonia.Controls.VerificationStep.md) | Class | 21 | 5 | 1 |

[Complete public API index](../../api/zh-cn/README.md)
<!-- generated-contracts:end -->
