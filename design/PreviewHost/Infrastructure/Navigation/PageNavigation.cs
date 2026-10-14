using System.Collections.ObjectModel;
namespace TheOne.DesignPreview;
internal static partial class PreviewNavigation
{
    public static ObservableCollection<NavEntry> CreatePages()
    {
        var groups=new ObservableCollection<NavEntry>();
        foreach(var name in new[]{"基础控件","数据展示","导航与布局","图表控件","扩展控件","消息与对话框"})
        {
            var group=new NavEntry(name,isGroup:true);
            foreach(var page in PreviewPageCatalog.Pages.Where(page=>page.Group==name)) group.Entries.Add(new(page.Title,page.Id));
            groups.Add(group);
        }
        return groups;
    }
}
