namespace TheOne.DesignPreview.Reference;

/// <summary>Small, explicit usage snippets; metadata is never executed or reflected over.</summary>
internal static class ControlUsageExamples
{
    internal static string Property(TypeContract owner, PropertyContract property)
    {
        if (owner.Name == "WorkflowEditor" && property.Name == "ChoiceItemsSource")
            return "<one:WorkflowEditor ChoiceItemsSource=\"{CompiledBinding ApproverItems, Mode=TwoWay}\" ChoiceItemAdapter=\"{CompiledBinding ApproverAdapter}\" />";
        if (owner.Name == "WorkflowEditor" && property.Name == "ChoiceItemAdapter")
            return "// ApprovalTarget 是消费端模型；getter 在编译期校验。\nvar adapter = WorkflowChoiceAdapter.Create<ApprovalTarget>(item => item.Key, item => item.DisplayName);\neditor.ChoiceItemAdapter = adapter;";
        if (owner.Name == "QRCode")
            return property.Name switch
            {
                "Text" => "<one:QRCode Text=\"{CompiledBinding QrText}\" Width=\"192\" Height=\"192\" />",
                "ErrorCorrection" => "<one:QRCode Text=\"https://example.com\" ErrorCorrection=\"High\" />",
                "QuietZone" => "<one:QRCode Text=\"https://example.com\" QuietZone=\"6\" />",
                "ValidationError" => "// 只读状态，可在视图中绑定显示。\nvar error = qrCode.ValidationError;",
                "Foreground" => "<one:QRCode Text=\"https://example.com\" Foreground=\"Black\" />",
                "Background" => "<one:QRCode Text=\"https://example.com\" Background=\"White\" />",
                _ => ""
            };
        var variable = property.Static ? owner.Name : Variable(owner.Name);
        if (!property.Writable)
        {
            if (property.Attached)
                return
                    $"var {Camel(property.Name)} = {owner.Name}.Get{property.Name}({TargetVariable(property.Target)});";
            return
                $"// {variable} 是当前 {owner.Name} {(property.Static ? "类型" : "实例")}。\nvar {Camel(property.Name)} = {variable}.{property.Name};";
        }

        if (!owner.IsControl && !property.Attached && owner.Name is not "OneTheme")
            return ModelProperty(owner, property);

        if (!property.Attached && string.IsNullOrEmpty(property.Identifier))
            return $"{variable}.{property.Name} = viewModel.{property.Name};";

        var prefix = Prefix(owner.Namespace);
        var target = property.Attached ? AttachedTarget(owner, property) : Element(owner);
        var attribute = property.Attached ? prefix + owner.Name + "." + property.Name : property.Name;
        var mode = property.Binding == "TwoWay" ? ", Mode=TwoWay" : "";
        var namespaceHint = NamespaceHint(owner.Namespace, prefix);
        if (target.StartsWith("notifications:", StringComparison.Ordinal))
            namespaceHint += "<!-- xmlns:notifications=\"using:Avalonia.Controls.Notifications\" -->\n";
        var binding = "{Binding " + property.Name + mode + "}";
        return namespaceHint + $"<{target} {attribute}=\"{binding}\" />";
    }

    internal static string Method(TypeContract owner, MemberContract member)
    {
        var name = MethodName(member.Signature);
        if (name.Length == 0 || name == owner.Name || name == "Render" ||
            owner.Properties.Any(property => property.Attached &&
                                             (name == "Get" + property.Name || name == "Set" + property.Name)))
            return "";

        var key = owner.Name + "." + name;
        var example = key switch
        {
            "ReportDesign.SaveTemplate" =>
                "string json = designer.SaveTemplate();\nawait File.WriteAllTextAsync(\"template.reportdesign.json\", json);\ndesigner.MarkSaved(); // 文件成功保存后再标记",
            "ReportDesign.MarkSaved" =>
                "await File.WriteAllTextAsync(\"template.reportdesign.json\", designer.SaveTemplate());\ndesigner.MarkSaved();",
            "ReportDesign.LoadTemplate" =>
                "string json = await File.ReadAllTextAsync(\"template.reportdesign.json\");\ndesigner.LoadTemplate(json);",
            "ReportDesign.LoadTemplateAsync" =>
                "string json = await File.ReadAllTextAsync(\"template.reportdesign.json\");\nawait designer.LoadTemplateAsync(json); // 等待关联资源加载",
            "ReportDesign.ExportPdfAsync" =>
                "await using var pdf = File.Create(\"report.pdf\");\nawait designer.ExportPdfAsync(pdf); // UI 线程调用；包含全部分页",
            "ReportDesign.RefreshReport" => "// 修改现有数据源后重新计算布局。\ndesigner.RefreshReport();",
            "ReportDesign.ShowPreview" => "designer.ShowPreview(); // 当前文档切换到分页预览",
            "ReportDesign.ShowDesign" => "designer.ShowDesign(); // 返回同一文档继续编辑",
            "ReportDesign.GoToPage" => "designer.ShowPreview();\ndesigner.GoToPage(2); // 页码从 1 开始",
            "ReportDesign.AddElement" =>
                "var title = designer.AddElement(ReportElementKind.Text, 24, 24);\ntitle.Text = \"销售明细\";",
            "ReportDesign.SetImage" =>
                "var image = designer.AddElement(ReportElementKind.Image, 24, 60);\nbyte[] bytes = await File.ReadAllBytesAsync(\"logo.png\");\ndesigner.SetImage(image, bytes, \"公司标志\");",
            "ReportDesign.DropField" =>
                "// fieldKey 必须存在于当前 DataSource 的字段中。\ndesigner.DropField(\"CustomerName\", 24, 80);",
            "ReportDesign.BindField" => "// 先选择待绑定的文本或表格列，再绑定数据源字段。\ndesigner.BindField(\"CustomerName\");",
            "ReportDesign.DropDataNodeAsync" =>
                "// dataNode 是数据树中的 ReportDataNode。\nawait designer.DropDataNodeAsync(dataNode, 24, 80);",
            "ReportDesign.FetchApiDataAsync" when
                member.Signature.Contains("ReportApiRequest request", StringComparison.Ordinal) =>
                "var request = new ReportApiRequest(\"https://example.com/api/orders\",\n    Method: \"POST\", Parameters: \"{\\\"status\\\":\\\"open\\\"}\");\nawait designer.FetchApiDataAsync(request);",
            "ReportDesign.FetchApiDataAsync" => "await designer.FetchApiDataAsync(\"https://example.com/api/orders\");",
            "ReportPreview.LoadAsync" =>
                "string json = await File.ReadAllTextAsync(\"template.reportdesign.json\");\nawait preview.LoadAsync(json);",
            "ReportPreview.GoToPage" => "preview.GoToPage(2); // 加载完成后跳至第 2 页",
            "ReportPreview.ExportPdfAsync" =>
                "await using var pdf = File.Create(\"report.pdf\");\nawait preview.ExportPdfAsync(pdf);",
            "ReportPreview.PrintAsync" =>
                "// 先由宿主配置 preview.PrintHandler，打开打印工具的设置与预览。\nvar result = await preview.PrintAsync(new ReportPrintOptions(Copies: 1));",
            "WorkflowSerializer.Save" =>
                "// WorkflowEditor.Document 是业务流程模型。\nstring json = WorkflowSerializer.Save(editor.Document!);\nawait File.WriteAllTextAsync(\"workflow.json\", json);",
            "WorkflowSerializer.Load" =>
                "string json = await File.ReadAllTextAsync(\"workflow.json\");\neditor.Document = WorkflowSerializer.Load(json);\neditor.FitToView();",
            "WorkflowDocument.Undo" => "editor.Document?.Undo(); // 撤销当前流程最近一次编辑",
            "WorkflowDocument.Redo" => "editor.Document?.Redo(); // 重做已撤销的流程编辑",
            "WorkflowDocument.ClearHistory" => "document.ClearHistory(); // 建立新的编辑历史起点",
            "WorkflowDocument.Validate" =>
                "var issues = document.Validate(allowCycles: false);\nforeach (var issue in issues)\n    Console.WriteLine(issue.Message);",
            "WorkflowDocument.AddNode" =>
                "var node = new WorkflowNode { Title = \"审批\", X = 120, Y = 80 };\ndocument.AddNode(node); // 进入撤销历史",
            "WorkflowDocument.Remove" => "document.Remove(editor.SelectedNodes.ToArray(), editor.SelectedConnection);",
            "WorkflowDocument.MoveNodes" => "document.MoveNodes(new[] { (Node: node, X: 160d, Y: 120d) });",
            "WorkflowDocument.SetProperty" =>
                "// property 是当前节点的 WorkflowProperty。\ndocument.SetProperty(property, \"approved\", reference: null);",
            "WorkflowDocument.SetConnectionProperty" =>
                "// connection 属于当前 document，property 属于该连线的 Properties。\ndocument.SetConnectionProperty(connection, property, \"approved\", reference: null);",
            "WorkflowDocument.SetSelectedKeys" =>
                "// property.Kind 为 WorkflowValueKind.MultiSelect；按稳定 Key 保存，一次编辑可撤销。\ndocument.SetSelectedKeys(property, [\"user:42\", \"role:finance\"]);",
            "WorkflowDocument.Edit" =>
                "double oldX = node.X;\ndocument.Edit(\"移动节点\", () => node.X = 200, () => node.X = oldX);",
            "WorkflowDocument.CheckConnection" =>
                "string? error = document.CheckConnection(\"start\", \"out\", \"approve\", \"in\");\n// null 表示这些已有节点与端口允许连接。",
            "WorkflowDocument.TryConnect" =>
                "bool connected = document.TryConnect(\"start\", \"out\", \"approve\", \"in\", out string? error);",
            "WorkflowDocument.IsReachable" => "bool reachesApproval = document.IsReachable(\"start\", \"approve\");",
            "WorkflowDocument.GetReferenceOptions" =>
                "var options = document.GetReferenceOptions(targetNode, property);\n// 作为该字段的上游数据引用候选项。",
            "WorkflowEditor.AddNode" =>
                "// definition 来自已配置的 NodeDefinitions。\nvar node = editor.AddNode(definition, new Point(120, 80));",
            "WorkflowEditor.SelectNode" => "editor.SelectNode(node, additive: false); // 替换当前选择",
            "WorkflowEditor.Connect" =>
                "bool connected = editor.Connect(\"start\", \"out\", \"approve\", \"in\");\n// 节点 ID 和端口 ID 必须来自当前 Document。",
            "WorkflowEditor.DeleteSelection" => "editor.DeleteSelection(); // 删除所选节点或连线，可撤销",
            "WorkflowEditor.FitToView" => "editor.FitToView(); // 将当前流程完整放入编辑视口",
            "WorkflowEditor.Locate" => "editor.Locate(node.Id); // 定位当前文档中的节点",
            "WorkflowEditor.ArrangeNodes" => "editor.ArrangeNodes(); // 自动布局当前流程节点",
            "WorkflowEditor.CopySelection" => "editor.CopySelection(); // 复制当前节点选择，供 PasteSelection 使用",
            "WorkflowEditor.PasteSelection" => "editor.PasteSelection(); // 粘贴此前复制的节点，生成新 ID",
            "Form.ValidateAsync" => "bool valid = await form.ValidateAsync();\nif (!valid) form.FocusFirstError();",
            "Form.SubmitAsync" => "bool submitted = await form.SubmitAsync();\n// 校验成功后由 Form 执行已配置的 SubmitCommand。",
            "Form.ClearValidation" => "form.ClearValidation(); // 清除校验提示，保留已输入的数据",
            "Form.FocusFirstError" => "if (form.HasErrors)\n    form.FocusFirstError();",
            "StepValidation.For" =>
                "var validator = StepValidation.For(model, nameof(model.Name), nameof(model.Email));\nform.Validator = validator;",
            "IStepValidator.ValidateAsync" =>
                "var errors = await validator.ValidateAsync(CancellationToken.None);\nforeach (var error in errors)\n    Console.WriteLine(error.Message);",
            "VerificationStep.Reset" => "steps.Reset(); // 清除步骤状态并回到起点",
            "VerificationStep.GoBack" => "bool moved = steps.GoBack(); // 返回上一步",
            "VerificationStep.GoToAsync" => "bool moved = await steps.GoToAsync(1); // 步骤索引从 0 开始",
            "VerificationStep.GoNextAsync" => "bool moved = await steps.GoNextAsync(); // 当前步骤校验通过后继续",
            "MultiSelectBox.OpenDropDown" => "bool opened = multiSelect.OpenDropDown(); // 请求打开搜索列表",
            "MultiSelectBox.CloseDropDown" => "multiSelect.CloseDropDown(); // 关闭列表，保留当前选择",
            "MultiSelectBox.RefreshItems" => "// 模型显示文字或禁用状态改变后重新读取适配器。\nmultiSelect.RefreshItems();",
            "MultiSelectBox.ToggleItem" =>
                "// option 是 ItemsSource 中的同一个模型实例。\nbool changed = multiSelect.ToggleItem(option);",
            "MultiSelectBox.ClearSelection" => "bool changed = multiSelect.ClearSelection();\n// 已选禁用项保留，允许编辑的选中项被移除。",
            "MultiSelectItemAdapter.Create" =>
                "multiSelect.ItemAdapter = MultiSelectItemAdapter.Create<Option>(\n    option => option.Name, option => option.IsEnabled);\n// Option 是应用模型，公开 Name 和 IsEnabled。",
            "MultiSelectItemAdapter.GetText" => "string text = adapter.GetText(option); // option 必须匹配适配器的模型类型",
            "MultiSelectItemAdapter.IsEnabled" => "bool canSelect = adapter.IsEnabled(option);",
            "GanttChart.RefreshData" => "// 重新读取现有任务集合并计算汇总任务。\ngantt.RefreshData();",
            "GanttChart.ScrollToToday" => "gantt.ScrollToToday(); // 时间轴滚动到今天",
            "GanttChart.FitToProject" => "gantt.FitToProject(); // 时间轴覆盖当前项目的完整日期范围",
            "GanttChart.BringTaskIntoView" => "bool found = gantt.BringTaskIntoView(task.Id); // task 来自当前 ItemsSource",
            "NavMenu.OpenSubmenu" => "menu.IsCollapsed = true;\nbool opened = menu.OpenSubmenu(0); // 第一个一级菜单",
            "NavMenu.CloseSubmenu" => "menu.CloseSubmenu(); // 关闭折叠菜单的浮层",
            "NavMenu.OpenFilter" => "menu.ShowFilter = true;\nbool opened = menu.OpenFilter(); // 已显示窗口中的导航搜索入口",
            "NavMenu.CloseFilter" => "menu.CloseFilter(); // 关闭导航搜索浮层",
            "NavMenu.SelectPath" => "bool selected = menu.SelectPath(0, 1);\n// 选择第一个根节点下的第二项；每层索引从 0 开始。",
            "ImagePreview.Previous" => "imagePreview.Previous(); // 上一张图片",
            "ImagePreview.Next" => "imagePreview.Next(); // 下一张图片",
            "ImagePreview.Reload" => "imagePreview.Reload(); // 重新加载当前图片资源",
            "ImagePreview.FitToWindow" => "imagePreview.FitToWindow(); // 按当前视口完整显示图片",
            "ImagePreview.ActualSize" => "imagePreview.ActualSize(); // 显示图片原始尺寸",
            "ImagePreview.ZoomIn" => "imagePreview.ZoomIn(); // 按控件步进放大",
            "ImagePreview.ZoomOut" => "imagePreview.ZoomOut(); // 按控件步进缩小",
            "ImagePreview.ToggleFullScreen" => "imagePreview.ToggleFullScreen(); // 切换图片全屏预览",
            "Avatar.Reload" => "avatar.Reload(); // 重试加载当前头像资源",
            "RelationGraph.RefreshData" => "graph.RefreshData(); // 重新读取当前节点与连线模型",
            "RelationGraph.GetNodePresenter" =>
                "var presenter = graph.GetNodePresenter(node);\npresenter?.BringIntoView(); // node 是当前图中的模型实例",
            "Pagination.GoToPage" => "bool changed = pagination.GoToPage(3); // 页码从 1 开始",
            "Pagination.GoPrevious" => "bool changed = pagination.GoPrevious();",
            "Pagination.GoNext" => "bool changed = pagination.GoNext();",
            "Pagination.GoFirst" => "bool changed = pagination.GoFirst();",
            "Pagination.GoLast" => "bool changed = pagination.GoLast();",
            "Upload.SelectAsync" => "await upload.SelectAsync(); // 在已显示的窗口中选择文件",
            "Upload.UploadAsync" => "// 先配置 UploadApi / HttpClient，或应用的 UploadCommand。\nawait upload.UploadAsync();",
            "Upload.ReloadPreview" => "upload.ReloadPreview(); // 重新生成当前选中文件的图片预览",
            "UploadFile.OpenReadAsync" => "await using Stream stream = await file.OpenReadAsync();\n// 在此读取内容；释放返回的流。",
            "UploadFile.FromStorageFileAsync" =>
                "using var uploadFile = await UploadFile.FromStorageFileAsync(storageFile);\n// storageFile 来自 Avalonia StorageProvider。",
            "UploadFile.Dispose" => "file.Dispose(); // 宿主不再使用该 UploadFile 时释放其资源",
            "LoginWindow.ShowErrorToast" => "login.ShowErrorToast(\"用户名或密码不正确\"); // owner 固定为登录窗口",
            "LoginWindow.ShowErrorNotification" => "login.ShowErrorNotification(\"登录服务暂不可用\", title: \"登录失败\");",
            "LoginWindow.ShowErrorDialog" => "await login.ShowErrorDialog(\"请检查账号信息后重试\", title: \"登录失败\");",
            "LoginWindow.CompleteLogin" when member.Signature.Contains("Func<OneWindow>", StringComparison.Ordinal) =>
                "bool completed = login.CompleteLogin(() => new MainWindow());\n// MainWindow 继承 OneWindow；成功后完成窗口交接。",
            "LoginWindow.CompleteLogin" =>
                "bool completed = login.CompleteLogin(new MainWindow());\n// MainWindow 继承 OneWindow。",
            "LoginWindow.CompleteLoginAsync" when member.Signature.Contains("Func<OneWindow>", StringComparison.Ordinal)
                => "bool completed = await login.CompleteLoginAsync(() => new MainWindow());\n// MainWindow 继承 OneWindow；等待窗口交接完成。",
            "LoginWindow.CompleteLoginAsync" =>
                "bool completed = await login.CompleteLoginAsync(new MainWindow());\n// MainWindow 继承 OneWindow。",
            "OneDialog.ShowAsync" when member.Signature.Contains("<TView", StringComparison.Ordinal) =>
                "var result = await OneDialog.ShowAsync<EditView, EditViewModel>(\n    viewModel, owner: ownerWindow);\n// EditView : UserControl, new()；ownerWindow 是实际所属窗口。",
            "OneDialog.ShowAsync" =>
                "var result = await OneDialog.ShowAsync(\"保存修改\", \"是否保存当前内容？\",\n    OneDialogButtons.OkCancel, owner: ownerWindow);",
            "OneDialog.ShowContentAsync" =>
                "var result = await OneDialog.ShowContentAsync(\n    new TextBlock { Text = \"自定义对话框内容\" }, title: \"详情\", owner: ownerWindow);",
            "OneMessageBox.ConfirmAsync" =>
                "bool confirmed = await OneMessageBox.ConfirmAsync(\"删除记录\", \"确认删除所选记录？\", owner: ownerWindow);",
            "OneToast.Show" when member.Signature.Contains("INotification notification", StringComparison.Ordinal) =>
                "OneToast.Show(new Notification(\"提示\", \"已保存\", NotificationType.Success), owner: ownerWindow);",
            "OneToast.Show" => "OneToast.Show(\"已保存\", NotificationType.Success, owner: ownerWindow);",
            "OneToast.ShowAsync" => "await OneToast.ShowAsync(\"已保存\", NotificationType.Success, owner: ownerWindow);",
            "OneToast.CloseAll" => "OneToast.CloseAll(owner: ownerWindow); // 只关闭该窗口的 Toast",
            "OneNotification.Show" when
                member.Signature.Contains("INotification notification", StringComparison.Ordinal) =>
                "OneNotification.Show(new Notification(\"保存完成\", \"工作区已更新\", NotificationType.Success), owner: ownerWindow);",
            "OneNotification.Show" =>
                "OneNotification.Show(\"保存完成\", \"工作区已更新\", NotificationType.Success, owner: ownerWindow);",
            "OneNotification.ShowAsync" =>
                "await OneNotification.ShowAsync(\"保存完成\", \"工作区已更新\", NotificationType.Success, owner: ownerWindow);",
            "OneNotification.CloseAll" => "OneNotification.CloseAll(owner: ownerWindow); // 只关闭该窗口的通知",
            "DialogContext.RequestClose" => "context.RequestClose(OneDialogResult.Ok); // 自定义对话框提交成功后关闭",
            "DataGridFeatures.ApplyFilter" =>
                "var result = DataGridFeatures.ApplyFilter(grid, nameColumn,\n    new TextFilterCondition(\"工作区\"));\n// nameColumn 必须属于 grid，且其原始字段为字符串。",
            "DataGridFeatures.ClearFilter" => "var result = DataGridFeatures.ClearFilter(grid, nameColumn);",
            "DataGridFeatures.ClearAllFilters" => "var result = DataGridFeatures.ClearAllFilters(grid);",
            "DataGridFeatures.GetSummaryResult" =>
                "var summary = DataGridFeatures.GetSummaryResult(grid, amountColumn);\n// amountColumn 必须已启用统计。",
            "DataGridRowMetadata.Register" =>
                "DataGridRowMetadata.Register<OrderRow>();\n// 在创建 DataGrid 视图前注册具体行类型，OrderRow 为应用模型。",
            "DataGridRowMetadata.RegisterCollection" =>
                "DataGridRowMetadata.RegisterCollection<List<OrderRow>, OrderRow>();",
            "DataGridAot.PreserveRowType" => "DataGridAot.PreserveRowType<OrderRow>(); // 注册具体行类型的裁剪元数据",
            "DataGridColumn.CycleSort" => "nameColumn.CycleSort(); // 切换列排序方向",
            "DataGridTreeCollectionView.DeferRefresh" =>
                "using (view.DeferRefresh())\n{\n    view.TrySetExpanded(firstRow, true);\n    view.TrySetExpanded(secondRow, true);\n} // 合并刷新，行实例来自当前树数据源",
            "DataGridTreeCollectionView.IsFilterMatch" or "IDataGridTreeQueryView.IsFilterMatch" =>
                "bool matches = view.IsFilterMatch(row); // row 是当前树中的数据项",
            "DataGridTreeCollectionView.IsLeaf" or "IDataGridTreeQueryView.IsLeaf" =>
                "bool leaf = view.IsLeaf(row); // 判断该数据项是否没有子节点",
            "DataGridTreeCollectionView.TryGetNodeState" or "IDataGridTreeCollectionView.TryGetNodeState" =>
                "if (view.TryGetNodeState(row, out var state))\n    Console.WriteLine(state.Depth); // 当前数据行的树深度",
            "DataGridTreeCollectionView.TrySetExpanded" or "IDataGridTreeCollectionView.TrySetExpanded" =>
                "bool changed = view.TrySetExpanded(row, expanded: true); // row 是当前树中的数据项",
            "DataGridTreeCollectionView.RefreshTree" => "bool refreshed = view.RefreshTree(); // 外部修改树结构后重新读取",
            "DataGridValueAccessor.Create" =>
                "var accessor = DataGridValueAccessor.Create<OrderRow, decimal>(row => row.Amount);\nDataGridColumnOptions.SetValueAccessor(amountColumn, accessor);",
            "IDataGridValueAccessor.GetValue" => "object? value = accessor.GetValue(row); // row 必须匹配已注册的具体行类型",
            "DataGridFilterCondition.Matches" or "TextFilterCondition.Matches" =>
                "bool matches = new TextFilterCondition(\"工作区\").Matches(\"工作区 0001\");",
            "NumericFilterCondition.Matches" =>
                "var condition = new NumericFilterCondition<decimal>(NumericFilterMode.Range, minimum: 10m, maximum: 100m);\nbool matches = condition.Matches(25m);",
            "TabFeatures.Close" when member.Signature.Contains("TabStripItem item", StringComparison.Ordinal) =>
                "bool closed = TabFeatures.Close(tabStripItem); // 先处理业务层的关闭确认",
            "TabFeatures.Close" => "bool closed = TabFeatures.Close(tabItem); // 先处理业务层的关闭确认",
            "TabFeatures.Move" => "bool moved = TabFeatures.Move(tabs, 0, 2); // 将第一项移至第三项位置",
            "TabFeatures.Pin" => "TabFeatures.Pin(tabItem); // 固定到左侧\nTabFeatures.Pin(tabItem, false); // 取消固定",
            "TabFeatures.CloseAll" => "int closed = TabFeatures.CloseAll(tabs); // 保留固定、不可关闭和取消关闭的页",
            "NavPage.OpenPage" => "shell.OpenPage(route); // 相同模型实例只激活已打开页面",
            "NavPage.ClosePage" => "bool closed = shell.ClosePage(route); // 遵循固定、关闭权限和取消事件",
            "NavPage.CloseAllPages" => "int closed = shell.CloseAllPages(); // 仅关闭允许关闭的页面",
            "Virtualization.BringIntoView" =>
                "var item = Virtualization.BringIntoView(tree, 0, 1);\n// 定位根节点 0 的子节点 1，按需实现容器。",
            "Virtualization.SelectAll" => "Virtualization.SelectAll(tree); // 选择数据项，不强制创建所有容器",
            "ReportTemplate.CreateSalesDetail" => "designer.Document = ReportTemplate.CreateSalesDetail(); // 内置销售明细模板",
            "ReportTemplate.Save" =>
                "string json = ReportTemplate.Save(document);\nawait File.WriteAllTextAsync(\"template.reportdesign.json\", json);",
            "ReportTemplate.Load" =>
                "string json = await File.ReadAllTextAsync(\"template.reportdesign.json\");\nReportDocument document = ReportTemplate.Load(json);",
            "ReportDocument.Save" => "await File.WriteAllTextAsync(\"template.reportdesign.json\", document.Save());",
            "ReportDocument.Load" =>
                "ReportDocument document = ReportDocument.Load(\n    await File.ReadAllTextAsync(\"template.reportdesign.json\"));",
            "ReportDataSource.Create" =>
                "var source = ReportDataSource.Create(\"订单\", orders,\n    new ReportField<OrderRow>(\"Amount\", \"金额\", row => row.Amount, ReportValueKind.Number));\ndesigner.DataSource = source; // OrderRow 是应用模型",
            "ReportDataSource.GetValue" => "object? amount = source.GetValue(0, \"Amount\"); // 第一行的金额字段",
            "ReportPaginator.Paginate" =>
                "ReportLayout layout = ReportPaginator.Paginate(document, source);\nint pageCount = layout.Pages.Count;",
            "ReportPdfExporter.ExportAsync" =>
                "await using var pdf = File.Create(\"report.pdf\");\nawait ReportPdfExporter.ExportAsync(document, source, pdf);",
            "ReportJsonData.IsRootField" => "bool rootField = ReportJsonData.IsRootField(\"$root/customer\");",
            "ReportJsonData.ReadRootValue" => "object? value = ReportJsonData.ReadRootValue(json, \"/customer/name\");",
            "ReportJsonData.CreateTree" => "var nodes = ReportJsonData.CreateTree(json); // 将 API JSON 映射为字段树",
            "ReportJsonData.FindBodies" => "var bodies = ReportJsonData.FindBodies(json); // 找出可作为明细表的数组",
            "ReportJsonData.CreateSource" when member.Signature.Contains("rootFieldKeys", StringComparison.Ordinal) =>
                "var source = ReportJsonData.CreateSource(json, \"/orders\", new[] { \"$root/customer\" });",
            "ReportJsonData.CreateSource" => "var source = ReportJsonData.CreateSource(json, \"/orders\");",
            "ReportResources.FetchJsonAsync" =>
                "string json = await ReportResources.FetchJsonAsync(\n    new ReportApiRequest(\"https://example.com/api/orders\"));",
            "ReportResources.LoadDataAsync" =>
                "ReportDataSource? source = await ReportResources.LoadDataAsync(document);",
            "ReportResources.LoadImagesAsync" when member.Signature.Contains("ReportDataSource? source",
                    StringComparison.Ordinal) =>
                "await ReportResources.LoadImagesAsync(document, client, CancellationToken.None, source);",
            "ReportResources.LoadImagesAsync" => "await ReportResources.LoadImagesAsync(document); // 解析并载入文档图片",
            "PrintTool.PrintPdfAsync" =>
                "var result = await PrintTool.PrintPdfAsync(\"report.pdf\", new ReportPrintOptions(Copies: 1));",
            "PrintTool.CreateStartInfo" =>
                "var startInfo = PrintTool.CreateStartInfo(\"report.pdf\", ReportPrintPlatform.MacOS);\n// 宿主决定是否启动此平台的打印进程。",
            "PrintTool.CleanupTemporaryDocuments" => "PrintTool.CleanupTemporaryDocuments(); // 清理打印工具管理的临时文档",
            "WorkflowNodeEditor.Refresh" => "nodeEditor.Refresh(); // 外部更改模型后刷新属性编辑视图",
            "WorkflowPropertyEditor.Refresh" => "propertyEditor.Refresh(); // 外部更改模型后刷新该字段视图",
            "WorkflowCanvas.ToWorld" =>
                "Point world = canvas.ToWorld(new Point(120, 80));\n// 输入是画布局部坐标；返回应用平移和缩放后的流程坐标。",
            "WorkflowCanvas.CancelGesture" => "canvas.CancelGesture(); // 取消当前拖拽、平移或连线手势",
            "ContentTransitionOptions.GetConfiguredTransition" =>
                "var transition = ContentTransitionOptions.GetConfiguredTransition(contentHost);\n// contentHost 是 TransitioningContentControl。",
            "OneTheme.SetColor" => "OneTheme.SetColor(ThemeColor.Atlas); // 启动时或运行中切换主题颜色资源",
            "IUploadFilePicker.PickAsync" =>
                "UploadFile? file = await filePicker.PickAsync(ownerWindow, CancellationToken.None);",
            "UploadHttp.SendAsync" =>
                "using var client = new HttpClient();\nvar result = await UploadHttp.SendAsync(client, uploadApi, requestType, model, \".png,.jpg\");",
            "UploadHttp.ExtractFileUrl" => "string? url = UploadHttp.ExtractFileUrl(responseBody, \".png,.jpg\");",
            _ => ChartMethod(name, member.Signature)
        };
        return example.Length > 0 ? example : FallbackCall(owner, member, name);
    }

    private static string ChartMethod(string name, string signature)
    {
        return name switch
        {
            "ExportPng" when signature.Contains("string path", StringComparison.Ordinal) =>
                "chart.ExportPng(\"chart.png\", new Size(1200, 600), dpi: 144);",
            "ExportPng" =>
                "using var png = File.Create(\"chart.png\");\nchart.ExportPng(png, new Size(1200, 600), dpi: 144);",
            "Zoom" => "chart.Zoom(1.25); // 围绕视口中心放大",
            "Pan" => "chart.Pan(0.25, 0); // 向更大的 X 值平移当前视口跨度的 25%",
            "ResetViewport" => "chart.ResetViewport(); // 恢复自动计算的数据范围",
            "BringDataPointIntoView" => "bool found = chart.BringDataPointIntoView(0, 10); // 系列与点的索引均从 0 开始",
            "RefreshData" => "chart.RefreshData(); // 在数据变化未发出通知时重新读取",
            "ToggleSeriesVisibility" => "bool changed = chart.ToggleSeriesVisibility(0); // 第一个系列",
            "PageLegend" => "bool moved = chart.PageLegend(1); // 下一页图例",
            "GetStackPercentage" => "double? percentage = chart.GetStackPercentage(0, 0); // 堆叠系列第一个点",
            "GetDataPointPosition" => "Point? position = chart.GetDataPointPosition(0, 0); // 控件本地坐标",
            "GetSharedDataPoints" => "var selections = chart.GetSharedDataPoints(new Point(120, 80)); // 控件本地坐标",
            "GetDataLabelBounds" => "Rect? labelBounds = chart.GetDataLabelBounds(0, 0);",
            "GetLegendItemBounds" => "Rect? itemBounds = chart.GetLegendItemBounds(0);",
            "GetLegendPageNavigationBounds" => "Rect? nextBounds = chart.GetLegendPageNavigationBounds(next: true);",
            "HitTestLegend" => "int? seriesIndex = chart.HitTestLegend(new Point(120, 80)); // 控件本地坐标",
            "HitTestDataPoint" => "var selection = chart.HitTestDataPoint(new Point(120, 80)); // 控件本地坐标",
            _ => ""
        };
    }

    private static string ModelProperty(TypeContract owner, PropertyContract property)
    {
        if (owner.Name == "WorkflowNodeDefinition")
            return
                "var definition = new WorkflowNodeDefinition\n{\n    TypeId = \"approval\", Title = \"审批\",\n    CreateNode = () => new WorkflowNode { TypeId = \"approval\", Title = \"审批\" },\n    " +
                (property.Name is "TypeId" or "Title" or "CreateNode" ? "// " : "") + property.Name + " = viewModel." +
                property.Name + "\n};";
        var creation = owner.Name switch
        {
            "WorkflowPort" => "new WorkflowPort(\"out\", \"输出\", WorkflowPortDirection.Output)",
            "WorkflowOutputField" => "new WorkflowOutputField(\"amount\", \"金额\")",
            "WorkflowDataReference" => "new WorkflowDataReference(\"source\", \"out\")",
            "WorkflowValidationIssue" => "new WorkflowValidationIssue(\"请填写必填字段\")",
            "WorkflowReferenceOption" =>
                "new WorkflowReferenceOption(\"上游金额\", new WorkflowDataReference(\"source\", \"out\", \"amount\"))",
            "WorkflowNodeGroup" => "new WorkflowNodeGroup(\"业务节点\", [])",
            "ReportApiRequest" => "new ReportApiRequest(\"https://example.com/api/orders\")",
            "ReportPrintOptions" => "new ReportPrintOptions()",
            "ReportPrintResult" => "new ReportPrintResult(\"已提交打印\", false)",
            "ReportFieldInfo" => "new ReportFieldInfo(\"Amount\", \"金额\", ReportValueKind.Number)",
            "ReportJsonBody" => "new ReportJsonBody(\"/orders\", \"订单\")",
            "DataGridFilterApplyResult" => "new DataGridFilterApplyResult(true)",
            "DataGridSummaryResult" => "new DataGridSummaryResult(0m, 0)",
            "StepValidationError" => "new StepValidationError(\"Email\", \"邮箱格式不正确\")",
            "ChartSelection" => "new ChartSelection(0, 0, series, point)",
            "ChartPoint" => "new ChartPoint(0, 0)",
            "GanttSelection" => "new GanttSelection(task, task.Start, task.EndExclusive, task.Progress, false)",
            "ReportLayoutColumn" => "new ReportLayoutColumn(\"金额\", \"Amount\", 120, true)",
            "ReportLayoutRow" => "new ReportLayoutRow(0, [], 28)",
            "ReportPage" => "new ReportPage(1, [], false)",
            _ => "new " + owner.Name + "()"
        };
        return $"var value = {creation}\n{{\n    {property.Name} = viewModel.{property.Name}\n}};";
    }

    private static string FallbackCall(TypeContract owner, MemberContract member, string name)
    {
        // Keep a call-site example for future parameterless business APIs. Methods with required
        // application objects use those documented parameter names, rather than fake constructors.
        var open = member.Signature.IndexOf('(');
        var close = member.Signature.LastIndexOf(')');
        if (open < 0 || close <= open) return "";
        var parameters = SplitParameters(member.Signature[(open + 1)..close]);
        var arguments = parameters.Where(parameter => !parameter.Contains('=')).Select(Argument).ToArray();
        var target = member.Signature.StartsWith("static ", StringComparison.Ordinal)
            ? owner.Name
            : Variable(owner.Name);
        var awaitCall = member.Signature.Contains("Task", StringComparison.Ordinal);
        var call = $"{target}.{name}({string.Join(", ", arguments)})";
        var prefix = awaitCall ? "await " :
            member.Signature.StartsWith("void ", StringComparison.Ordinal) ||
            member.Signature.StartsWith("static void ", StringComparison.Ordinal) ? "" : "var result = ";
        return (arguments.Length > 0 ? "// 参数使用当前业务模型中的实例。\n" : "// 在已初始化并绑定数据的控件上调用。\n") + prefix + call + ";";
    }

    private static string Argument(string parameter)
    {
        var words = parameter.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var name = words.LastOrDefault() ?? "value";
        if (words.Contains("out")) return "out var " + name;
        if (words.Contains("ref")) return "ref " + name;
        var type = words.Length > 1 ? words[^2] : "";
        return type switch
        {
            "int" or "long" => "0", "double" => "0d", "bool" => "false",
            "CancellationToken" => "CancellationToken.None", _ => name
        };
    }

    private static string[] SplitParameters(string parameters)
    {
        var parts = new List<string>();
        var start = 0;
        var nesting = 0;
        for (var index = 0; index < parameters.Length; index++)
            if (parameters[index] is '<' or '(' or '[')
            {
                nesting++;
            }
            else if (parameters[index] is '>' or ')' or ']')
            {
                nesting--;
            }
            else if (parameters[index] == ',' && nesting == 0)
            {
                parts.Add(parameters[start..index].Trim());
                start = index + 1;
            }

        if (start < parameters.Length) parts.Add(parameters[start..].Trim());
        return parts.ToArray();
    }

    private static string MethodName(string signature)
    {
        var open = signature.IndexOf('(');
        if (open < 0) return "";
        var head = signature[..open];
        var dot = head.LastIndexOf('.');
        var token = dot >= 0 ? head[(dot + 1)..] : head.Trim();
        var generic = token.IndexOf('<');
        return generic >= 0 ? token[..generic] : token;
    }

    private static string AttachedTarget(TypeContract owner, PropertyContract property)
    {
        return owner.Name switch
        {
            "DataGridColumnOptions" => "DataGridTextColumn",
            "CalendarOptions" => property.Name == "WheelNavigation" ? "Calendar" : "CalendarDatePicker",
            "PickerSizing" => "DatePicker",
            "ControlSizing" or "Motion" => "StackPanel",
            "RefreshOptions" => "RefreshContainer",
            "NotificationMotion" => "notifications:NotificationCard",
            "NavMenuSearchText" => "one:NavMenuSearchText",
            "Virtualization" => "TreeView",
            _ => property.Target.Split('.').Last() switch
            {
                "Control" => "Border", "ItemsControl" => "ItemsControl", "DataGridColumn" => "DataGridTextColumn",
                var target => target
            }
        };
    }

    private static string Element(TypeContract owner)
    {
        return owner.Name switch
        {
            "ChartBase" => "one:LineChart",
            "DataGridColumn" => "DataGridTextColumn",
            _ => Prefix(owner.Namespace) + owner.Name
        };
    }

    private static string Prefix(string ns)
    {
        return ns switch
        {
            "TheOne.Avalonia.Styling" => "styling:", "TheOne.Avalonia.Themes" => "themes:",
            "TheOne.Avalonia.Reporting" => "report:", "TheOne.Avalonia.Workflow" => "workflow:",
            _ => ns.StartsWith("TheOne.Avalonia", StringComparison.Ordinal) ? "one:" : ""
        };
    }

    private static string NamespaceHint(string ns, string prefix)
    {
        return prefix.Length == 0 ? "" : $"<!-- xmlns:{prefix.TrimEnd(':')}=\"using:{ns}\" -->\n";
    }

    private static string TargetVariable(string target)
    {
        return target.Split('.').Last() switch
        {
            "DataGridColumn" => "column", "Control" => "control", var name => Camel(name)
        };
    }

    private static string Variable(string name)
    {
        return name switch
        {
            "ChartBase" => "chart", "ReportDesign" => "designer", "ReportPreview" => "preview",
            "WorkflowEditor" => "editor",
            "WorkflowDocument" or "ReportDocument" => "document", "MultiSelectBox" => "multiSelect",
            "GanttChart" => "gantt", "NavMenu" => "menu",
            "LoginWindow" => "login", "RelationGraph" => "graph", "DataGridColumn" => "column", _ => Camel(name)
        };
    }

    private static string Camel(string value)
    {
        return value.Length == 0 ? "value" : char.ToLowerInvariant(value[0]) + value[1..];
    }
}
