# 编译绑定与 NativeAOT

类库和 PreviewHost 启用编译绑定及 IsAotCompatible。模板使用 TemplateBinding 或编译绑定；MultiBinding 子项显式 CompiledBindingExtension。视图／数据模板声明对应 x:DataType；主题 DynamicResource 保留。禁止动态解析 XAML、反射创建视图、ReflectionBinding 或关闭编译绑定绕过类型检查。

PreviewHost 与链接其 VM 的 NativeAotSmoke 使用 ReactiveUI.Avalonia 12.1.4；只注册平台服务，视图继续采用明确类型的 Avalonia 编译 XAML 绑定和显式工厂。没有使用程序集扫描注册视图、反射 ViewLocator 或表达式反射绑定。ReactiveObject 负责 VM 属性通知，ReactiveCommand 提供命令；类库保持框架独立。设计器的 object 上下文处理见 [预览绑定规范](preview-host.md#reactiveui-与设计器类型)。依赖迁移按测试策略运行完整原生验证，不能用普通 dotnet run 替代。

```xml
<PropertyGroup>
  <TargetFramework>net10.0</TargetFramework>
  <AvaloniaUseCompiledBindingsByDefault>true</AvaloniaUseCompiledBindingsByDefault>
  <IsAotCompatible>true</IsAotCompatible>
  <!-- 仅可执行应用设置；类库不设置 PublishAot。 -->
  <PublishAot>true</PublishAot>
</PropertyGroup>
```

## DataGrid

基于官方 DataGrid 12.1.2 的[源码扩展](../vendor/Avalonia.Controls.DataGrid/README.md)采用显式元数据注册检查编辑、排序和分组。托管、裁剪与 NativeAOT 使用同一路径；在创建集合视图或表格前，为每个具体行类型调用：

```csharp
_ = DataGridAot.PreserveRowType<Record>();
```

只保留指定类型的公共实例属性、访问器和公共无参构造函数，并缓存元数据，不扫描程序集。没有公共无参构造函数的行仍可显示和编辑，只是不提供 AddNew。推荐显式列及默认编译绑定；List<T>、ObservableCollection<T>、IEnumerable<T> 及 DataGridCollectionView<T> 的列自动推断行类型。非泛型 DataGridCollectionView 需在列上声明 x:DataType；路径排序显式设置 SortMemberPath：

```xml
<DataGrid ItemsSource="{Binding Records}" AutoGenerateColumns="False" MaxColumnWidth="320">
  <DataGrid.Columns>
    <DataGridTextColumn Header="名称" SortMemberPath="Name"
        Binding="{Binding Name, Mode=TwoWay}" />
  </DataGrid.Columns>
</DataGrid>
```

有限 MaxColumnWidth 在创建列前设置。嵌套路径沿途模型均需注册；自定义 IComparable<T> 字段也注册该类型，使用编译期实例化的 Comparer<T>.Default。自动列从注册属性生成编译访问器。基础数字、可空值、日期时间和 Guid 使用明确转换 API；格式错误和溢出报告绑定错误。枚举、自定义 TypeConverter、Parse 或隐式转换需显式列 Converter。注册属性支持单参数 int／string 索引器，不在运行时构造泛型接口。筛选／统计继续复用编译 getter，见 [列筛选](controls/datagrid-extensions.md)。

DataGrid 源码启用 IsAotCompatible，已移除动态 MakeGenericType、未注册类型反射查询、DefaultValueConverter 和自动列反射绑定。本机 NativeAOT 发布中的 DataGrid IL2026 / IL2070 / IL2075 / IL3050 警告已消除，没有 NoWarn、裁剪警告抑制或整程序集保留；外部依赖警告仍需独立审查。

List<T>、ObservableCollection<T>、T[]、DataGridCollectionView<T> 的空源行类型由注册表提供。其他空集合形状调用 `DataGridRowMetadata.RegisterCollection<MyCollection, Record>()`。非空源可由首项识别类型，但不会据此保留属性。未注册的属性路径明确报错，避免裁剪后悄悄成为只读或排序无效。

## 图表与消费示例

14 种具体图表共享 ChartSeries / ChartPoint；系列、点、刻度和图例不通过反射或逐点 Control 创建，无需额外模型元数据保留。SeriesSource／选择／Viewport／仪表值使用编译绑定。动态切换创建具体控件，不修改只读 Kind。数据及显示 API 见 [图表索引](controls/charts.md)。

独立消费项目 `design/PreviewHost/Consumers/NativeAotSmoke` 使用自己的 AuditModel、编译 XAML 与 ThemeManifest，验证主题注册、双向绑定、命令、DataGrid 编辑／排序／分组、集合回收、图表与窗口流程。新增主题同步 OneTheme 和 ThemeManifest，不能以预览项目的成功代替独立消费验证。

DataGrid 的包消费另用 `design/PreviewHost/Consumers/DataGridPackageConsumer`，只引用本地 nupkg，分别验证托管／原生及 Headless／桌面后端；跨 RID 发布不能代替目标宿主运行。双包版本与程序集身份见[发布策略](workflow/releasing.md)，平台边界见[集成验收](workflow/datagrid-acceptance.md)。

## Tag / Step 属性验证

Tag 与 Step 使用已注册主题和有类型的数据模板。StepItem<T> 在控件库内部保留具体模型的公共属性、读取 ValidationAttribute，并以编译绑定路径生成默认表单；调用方无需注册验证器或保留整个程序集。自定义内容和范围限制仍可使用 StepValidation.For<T>；不扫描程序集、不创建反射绑定。跨字段比较使用 StepCompareAttribute。每步属性范围、异步验证与控件生命周期见 [Tag 与 Step](controls/tag-step.md)。

## 原生验证与边界

按[测试策略](workflow/testing.md#nativeaot)选择相关原生专项或全面审查。按宿主 RID 发布真实程序，用 `--require-aot` 拒绝托管运行；macOS 使用 xcrun 提供 clang／SDK，仅影响当前发布进程。

Windows 可在对应宿主执行：

```sh
dotnet publish design/PreviewHost/Consumers/NativeAotSmoke -c Release -r win-x64 -p:PublishAot=true -o .preview/aot-win
.preview/aot-win/NativeAotSmoke.exe --require-aot
```

已实跑平台为 macOS ARM64、.NET 10、Avalonia 12.1.4／DataGrid 12.1.2，渲染使用 Skia Headless。其他 RID、真实 GPU、业务模型与外部依赖需在对应宿主验证。配置依据见 [Avalonia NativeAOT 指南](https://docs.avaloniaui.net/docs/deployment/native-aot)。验证结论以当前构建及原生可执行文件实跑日志为准。

Windows 控件已由用户确认实跑完成；本次反馈没有细分 RID 或 NativeAOT 发布方式，因此不扩写为 Windows 原生专项证据。更新后的小尺寸轴标签和 GUI 启动产物需在 Windows 复核；本机跨 RID 产物检查不等于目标宿主执行。

2026-10-14 源码验证：原生独立消费 1,206 项；DataGrid 编译绑定／排序／虚拟化 1,623 项，包含自动列实际编辑提交和只读列拒绝编辑；筛选／统计／冻结 1,104 项；Tag／Step 631 项。最终双包消费在托管及 NativeAOT 下均通过 Headless 152 项和桌面 140 项，隔离缓存的包哈希与新打包产物一致。运行时动态代码关闭，发布没有 DataGrid 裁剪或动态代码警告。这些结论属于当前源码，不能归于已发布的旧包。
