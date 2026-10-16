using System.Collections.ObjectModel;
namespace TheOne.DesignPreview;
internal static partial class PreviewNavigation
{
    public static ObservableCollection<NavEntry> CreatePages()
    {
        var groups=new ObservableCollection<NavEntry>
        {
            new(PreviewPageCatalog.Home.Title, PreviewPageCatalog.Home.Id) { Icon = PreviewNavigationIcons.Page(0) }
        };
        foreach(var name in new[]{"基础控件","数据展示","导航与布局","图表控件","扩展控件","消息与对话框"})
        {
            var group=new NavEntry(name,isGroup:true) { Icon = PreviewNavigationIcons.Group(name) };
            foreach(var page in PreviewPageCatalog.Pages.Where(page=>page.Group==name)) group.Entries.Add(new(page.Title,page.Id) { Icon = PreviewNavigationIcons.Page(page.Id) });
            groups.Add(group);
        }
        return groups;
    }
}
