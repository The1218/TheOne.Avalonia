# NavMenuFilterResult

[English](../en/TheOne.Avalonia.Controls.NavMenuFilterResult.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.NavMenuFilterResult.md) · [API index](README.md)

`TheOne.Avalonia.Controls.NavMenuFilterResult` · Class · `object`

A lightweight suggestion containing an original menu model and its ancestor path.

[Source / 声明](../../../TheOne.Avalonia/Controls/NavMenuFilterBox.cs#L98)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Item`](../../../TheOne.Avalonia/Controls/NavMenuFilterBox.cs#L108) | `object` | 只读; CLR / n/a | `accessor-defined / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`Label`](../../../TheOne.Avalonia/Controls/NavMenuFilterBox.cs#L109) | `string` | 只读; CLR / n/a | `accessor-defined / state` | 显示标签文本；不作为模型身份、字段路径或序列化标识。 |
| [`Path`](../../../TheOne.Avalonia/Controls/NavMenuFilterBox.cs#L110) | `string` | 只读; CLR / n/a | `accessor-defined / state` | 输出字段的稳定路径键；与数据引用中的 FieldPath 一致。 |
| [`SearchPath`](../../../TheOne.Avalonia/Controls/NavMenuFilterBox.cs#L111) | `string` | 只读; CLR / n/a | `accessor-defined / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`override string NavMenuFilterResult.ToString()`](../../../TheOne.Avalonia/Controls/NavMenuFilterBox.cs#L114) | 返回此搜索结果的展示文本，不作为模型身份或序列化格式。 |
