# ReportPaginator

[English](../en/TheOne.Avalonia.Reporting.ReportPaginator.md) · [简体中文](../zh-cn/TheOne.Avalonia.Reporting.ReportPaginator.md) · [API index](README.md)

`TheOne.Avalonia.Reporting.ReportPaginator` · Class · `object`

Single table pagination: repeated title/header, atomic wrapped rows, last-page totals and page numbers.

[Source / 声明](../../../TheOne.Avalonia/Reporting/ReportPaginator.cs#L30)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

没有在该类型上另外声明 CLR／附加属性；继承成员见其基类。

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`static ReportLayout ReportPaginator.Paginate(ReportDocument document, ReportDataSource? source = null)`](../../../TheOne.Avalonia/Reporting/ReportPaginator.cs#L49) | 校验文档并计算不可变分页布局，供设计预览与打印共享。 |

## 公开字段

| 完整签名／值 | 说明 |
| --- | --- |
| [`const double ReportPaginator.DipsPerMillimetre`](../../../TheOne.Avalonia/Reporting/ReportPaginator.cs#L32) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |

## 相关 API 与内置模型

- [`TheOne.Avalonia.Reporting.ReportDataSource`](TheOne.Avalonia.Reporting.ReportDataSource.md)
- [`TheOne.Avalonia.Reporting.ReportDocument`](TheOne.Avalonia.Reporting.ReportDocument.md)
- [`TheOne.Avalonia.Reporting.ReportLayout`](TheOne.Avalonia.Reporting.ReportLayout.md)
