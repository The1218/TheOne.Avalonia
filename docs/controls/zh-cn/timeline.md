# Timeline 时间线

垂直展示操作记录、物流事件或审批历史。Timeline 按集合顺序展示事件，不排序、不执行审批、不维护当前步骤；步骤进度使用 [FlowStep](flow-step.md)。注册 OneTheme 后使用，`one` 为 `using:TheOne.Avalonia.Controls`。

```xml
<one:Timeline>
    <one:TimelineItem Header="提交申请" Timestamp="09:00" Content="采购申请已提交。" Status="Success" />
    <one:TimelineItem Header="等待审批" Timestamp="09:30" Content="等待主管处理。" Status="Pending" />
</one:Timeline>
```

## 额外属性与布局

| 属性 | 默认值 | 说明 |
| --- | --- | --- |
| Timeline.Mode | Left | Left：轴在左、内容在右；Right：轴在右、内容在左；Alternate：轴居中，按视觉顺序交错，首项内容在右 |
| Timeline.Reverse | false | 仅反转视觉排列，不修改数据源；交错方向与末项尾线随之更新 |
| Timeline.ItemSpacing | 16 DIP | 继承到 TimelineItem 的下方留白，有限非负，可逐项覆盖；主题三档值为 12/16/20 |
| TimelineItem.Status | Default | Default（强调色）、Success、Warning、Error、Pending；Pending 为静态空心节点，无循环动画 |
| TimelineItem.Timestamp / TimestampTemplate | null | 时间内容及类型模板；null 隐藏。左右模式位于正文上方，交错模式位于轴另一侧 |
| TimelineItem.Dot / DotTemplate | null | 自定义节点内容及类型模板；非 null 替代状态圆点，恢复 null 后显示当前 Status |
| TimelineItem.ItemSpacing | 继承 | 覆盖单项留白；视觉末项无额外尾部留白或尾线 |

```xml
<one:Timeline ItemsSource="{CompiledBinding Events}" Mode="Alternate" Reverse="True" ItemSpacing="20" />
```

默认 TimelinePanel 按自然高度排列，不固定高度、不创建内层滚动条，适合有限事件历史，**不提供虚拟化**。大量事件由宿主分页或按需加载。替换 ItemsPanel 时，宿主面板需自行实现倒序排列。交错布局按宽度换行，不自动改变 Mode，窄页面建议使用 Left。Reverse 不改变内容中原生控件的键盘 Tab 顺序。

无额外公开 Classes、命令或保存 API；公共 Small/Normal/Large、Light/Dark 及配色规则见[统一主题说明](../../theme/zh-cn/README.md#shared-configuration)。

## 内置模型与绑定

```csharp
public ObservableCollection<TimelineItemModel> Events { get; } =
[
    new() { Key = "submit", Header = "申请已提交", Content = "单号 PO-001",
            Timestamp = "2026-10-20 09:00", Status = TimelineStatus.Success },
    new() { Key = "review", Header = "主管审批", Content = "待处理", Status = TimelineStatus.Pending }
];
// UI 线程更新，既有容器通过编译绑定更新，不必重设 ItemsSource。
Events[1].Content = "审核通过";
Events[1].Status = TimelineStatus.Success;
```

TimelineItemModel 是可观察 AvaloniaObject：Key 为宿主业务标识；Header、Content、Timestamp、Dot 为可选内容，默认 null；Status 默认 Default。Timestamp 不隐式格式化，不决定顺序，本地化与时区由宿主处理。支持可观察集合增删、Move、Replace、Reset、切换 ItemsSource。不要在后台线程直接修改 UI 模型或集合。

原生 ItemTemplate 接收原始数据项，只定义正文区域；内置模型仍映射 Header/Timestamp/Status/Dot，未设置 ItemTemplate 时正文为模型 Content。

```xml
<one:Timeline ItemsSource="{CompiledBinding Events}">
    <one:Timeline.ItemTemplate>
        <DataTemplate x:DataType="one:TimelineItemModel">
            <TextBlock Text="{CompiledBinding Content}" TextWrapping="Wrap" />
        </DataTemplate>
    </one:Timeline.ItemTemplate>
</one:Timeline>
```

任意业务类型可用 ItemTemplate 定义正文；如需独立映射标题、时间与状态，可用原生 ItemContainerTheme 的强类型 CompiledBinding Setter。无需反射 getter 或属性名字符串。业务按钮放在 Content 中，保留原生输入、焦点与命令行为。

## 自定义节点与模板

```xml
<one:TimelineItem Header="人工审批" Content="财务已审核。" Status="Success">
    <one:TimelineItem.Dot><one:OneIcon Kind="Approval" IconSize="18" /></one:TimelineItem.Dot>
</one:TimelineItem>
<!-- 需重复使用时，通过模板为每项创建独立控件。 -->
<one:TimelineItem Header="完成" Dot="done">
    <one:TimelineItem.DotTemplate>
        <DataTemplate><one:OneIcon Kind="Success" IconSize="18" /></DataTemplate>
    </one:TimelineItem.DotTemplate>
</one:TimelineItem>
```

轨道宽 24 DIP，自定义节点建议不超过 24 DIP；同一 Control 实例不能跨项共享。HeaderTemplate / ContentTemplate 使用原生机制；TimestampTemplate / DotTemplate 声明对应 x:DataType，普通绑定使用 CompiledBinding。状态圆点为装饰，重要状态同时写进标题或正文，不仅依靠颜色传递信息。

PreviewHost → 数据展示 → Timeline 独立页提供动态事件、三种布局、倒序、五种状态、自定义节点、富内容和编译绑定模板，Description 内提供额外属性及模型短代码。专项范围为 Timeline 及其预览、集合生命周期、模板切换、主题/尺寸/窄布局与内容按钮交互，不做全库回归。

2026-10-20：托管与 macOS ARM64 NativeAOT 专项各 **426 项通过**。覆盖增删、Move/Replace/Reset、切换及清空 ItemsSource、旧模型解绑、倒序/交错/末项连线、模板动态恢复、状态/自定义节点恢复、主题资源解析、明暗/配色/三档尺寸、窄布局、卸载重载及嵌入按钮指针/空格和焦点。真实 macOS 桌面另验证追加事件、倒序、键盘切换布局、滚动及状态按钮。正常 Release 构建通过，保留既有 LoginDemoViewModel 可空警告；没有新增生产依赖或裁剪/AOT 警告。未做全库回归或 Windows/Linux 实跑。

<!-- generated-contracts:start -->
## 专属属性、Classes 用法与业务 API

本节只列本项目的专属属性、公开 Classes 用法、实际业务动作及关联模型，不重复原生继承 API、构造器、属性 Get/Set 包装或公开标识符。注册默认值与主题生效值不同，显式局部值优先。PreviewHost 用实际标签样例展示 Classes，不展示模板选择器／Setter 明细；Description 中的属性与业务动作附短用法代码。ControlSizing、Motion 等通用配置集中在 Home。仅 Button、Input、Select 合并比较，其余控件保留独立完整示例。

### Timeline

[完整源码契约](../../api/zh-cn/TheOne.Avalonia.Controls.Timeline.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`ItemSpacing`](../../../TheOne.Avalonia/Controls/Timeline.cs#L34) | `double` | 可写; OneWay; 向后代继承 | `16` | 16 DIP ; 继承到 TimelineItem 的下方留白，有限非负，可逐项覆盖；主题三档值为 12/16/20 校验：`value => double.IsFinite(value) && value >= 0` |
| [`Mode`](../../../TheOne.Avalonia/Controls/Timeline.cs#L30) | `TheOne.Avalonia.Controls.TimelineMode` | 可写; OneWay | `0 / default` | Left ; Left：轴在左、内容在右；Right：轴在右、内容在左；Alternate：轴居中，按视觉顺序交错，首项内容在右 校验：`Enum.IsDefined` 可选值：`Left`, `Right`, `Alternate` |
| [`Reverse`](../../../TheOne.Avalonia/Controls/Timeline.cs#L32) | `bool` | 可写; OneWay; 向后代继承 | `false` | false ; 仅反转视觉排列，不修改数据源；交错方向与末项尾线随之更新 |

### TimelineItem

[完整源码契约](../../api/zh-cn/TheOne.Avalonia.Controls.TimelineItem.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Dot`](../../../TheOne.Avalonia/Controls/TimelineItem.cs#L30) | `object?` | 可写; OneWay | `null` | null ; 自定义节点内容及类型模板；非 null 替代状态圆点，恢复 null 后显示当前 Status |
| [`DotTemplate`](../../../TheOne.Avalonia/Controls/TimelineItem.cs#L31) | `Avalonia.Controls.Templates.IDataTemplate?` | 可写; OneWay | `null` | null ; 自定义节点内容及类型模板；非 null 替代状态圆点，恢复 null 后显示当前 Status |
| [`ItemSpacing`](../../../TheOne.Avalonia/Controls/TimelineItem.cs#L33) | `double` | 可写; OneWay; 向后代继承 | `16` | 16 DIP ; 继承到 TimelineItem 的下方留白，有限非负，可逐项覆盖；主题三档值为 12/16/20 校验：`value => double.IsFinite(value) && value >= 0` |
| [`Status`](../../../TheOne.Avalonia/Controls/TimelineItem.cs#L32) | `TheOne.Avalonia.Controls.TimelineStatus` | 可写; OneWay | `0 / default` | Default ; Default（强调色）、Success、Warning、Error、Pending；Pending 为静态空心节点，无循环动画 校验：`Enum.IsDefined` 可选值：`Default`, `Success`, `Warning`, `Error`, `Pending` |
| [`Timestamp`](../../../TheOne.Avalonia/Controls/TimelineItem.cs#L28) | `object?` | 可写; OneWay | `null` | null ; 时间内容及类型模板；null 隐藏。左右模式位于正文上方，交错模式位于轴另一侧 |
| [`TimestampTemplate`](../../../TheOne.Avalonia/Controls/TimelineItem.cs#L29) | `Avalonia.Controls.Templates.IDataTemplate?` | 可写; OneWay | `null` | null ; 时间内容及类型模板；null 隐藏。左右模式位于正文上方，交错模式位于轴另一侧 |

### TimelineItemModel

[完整源码契约](../../api/zh-cn/TheOne.Avalonia.Controls.TimelineItemModel.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Content`](../../../TheOne.Avalonia/Controls/TimelineItemModel.cs#L16) | `object?` | 可写; OneWay | `null` | 宿主内容或业务展示对象；具体模板作用域见内容模板契约，不把视图对象当作持久化数据。 |
| [`Dot`](../../../TheOne.Avalonia/Controls/TimelineItemModel.cs#L18) | `object?` | 可写; OneWay | `null` | null ; 自定义节点内容及类型模板；非 null 替代状态圆点，恢复 null 后显示当前 Status |
| [`Header`](../../../TheOne.Avalonia/Controls/TimelineItemModel.cs#L15) | `object?` | 可写; OneWay | `null` | 标题区内容，可为文本或模型；自定义显示由 HeaderTemplate 负责。 |
| [`Key`](../../../TheOne.Avalonia/Controls/TimelineItemModel.cs#L14) | `object?` | 可写; OneWay | `null` | 字段／参数键，用于关联业务值；与显示名称分开维护。 |
| [`Status`](../../../TheOne.Avalonia/Controls/TimelineItemModel.cs#L19) | `TheOne.Avalonia.Controls.TimelineStatus` | 可写; OneWay | `0 / default` | Default ; Default（强调色）、Success、Warning、Error、Pending；Pending 为静态空心节点，无循环动画 校验：`Enum.IsDefined` 可选值：`Default`, `Success`, `Warning`, `Error`, `Pending` |
| [`Timestamp`](../../../TheOne.Avalonia/Controls/TimelineItemModel.cs#L17) | `object?` | 可写; OneWay | `null` | null ; 时间内容及类型模板；null 隐藏。左右模式位于正文上方，交错模式位于轴另一侧 |

### TimelineMode

[完整源码契约](../../api/zh-cn/TheOne.Avalonia.Controls.TimelineMode.md)

没有在该类型上另外声明 CLR／附加属性；继承成员见其基类。

### TimelinePanel

[完整源码契约](../../api/zh-cn/TheOne.Avalonia.Controls.TimelinePanel.md)

没有在该类型上另外声明 CLR／附加属性；继承成员见其基类。

### TimelineStatus

[完整源码契约](../../api/zh-cn/TheOne.Avalonia.Controls.TimelineStatus.md)

没有在该类型上另外声明 CLR／附加属性；继承成员见其基类。

尺寸、动效等通用配置见[统一主题说明](../../theme/zh-cn/README.md#shared-configuration)，PreviewHost 在 Home 集中演示。

### Classes 用法

没有额外公开外观类，使用默认主题及专属属性。

以上是分别使用的公开类名；悬停、按下、禁用和选中等状态由控件维护，不手工添加模板类。实际外观见 PreviewHost 的标签样例。

### 业务 API

没有额外业务方法；通过本节专属属性配置。

### 关联模型

| 模型 | 属性／枚举值 |
| --- | ---: |
| [`TheOne.Avalonia.Controls.TimelineItemModel`](../../api/zh-cn/TheOne.Avalonia.Controls.TimelineItemModel.md) | 6 |
| [`TheOne.Avalonia.Controls.TimelineMode`](../../api/zh-cn/TheOne.Avalonia.Controls.TimelineMode.md) | 3 |
| [`TheOne.Avalonia.Controls.TimelineStatus`](../../api/zh-cn/TheOne.Avalonia.Controls.TimelineStatus.md) | 5 |

[完整源码契约（含构造、属性访问器和标识符，供精确查阅）](../../api/zh-cn/README.md)
<!-- generated-contracts:end -->
