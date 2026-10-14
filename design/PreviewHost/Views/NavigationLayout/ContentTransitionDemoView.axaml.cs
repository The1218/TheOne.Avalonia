using Avalonia.Controls;
using Avalonia.Threading;
using Avalonia.VisualTree;
using TheOne.Avalonia.Styling;
namespace TheOne.DesignPreview;
public partial class ContentTransitionDemoView : UserControl
{
 public ContentTransitionDemoViewModel Model {get;}=new();
 public IReadOnlyList<TransitionPage> Pages=>Model.Pages;
 public int PageIndex=>Model.PageIndex; public int CompletedCount=>Model.CompletedCount; public int AnimatedCount=>Model.AnimatedCount; public int ActionCount=>Model.ActionCount; public bool IsRapidSwitching=>Model.IsRapidSwitching;
 private bool _syncingMotion;
 private readonly DispatcherTimer _metricsTimer=new(){Interval=TimeSpan.FromMilliseconds(250)};
 public ContentTransitionDemoView()
 {
  DataContext=Model;InitializeComponent();
  Loaded+=(_,_)=>{UpdateMetrics();_metricsTimer.Start();};_metricsTimer.Tick+=(_,_)=>UpdateMetrics();
  DetachedFromVisualTree+=(_,_)=>{Model.StopRapid();_metricsTimer.Stop();};
  Model.PropertyChanged+=(_,args)=>{if(args.PropertyName==nameof(ContentTransitionDemoViewModel.MotionEnabled)&&!_syncingMotion)Motion.SetEnabled(ContentTransitionSample,Model.MotionEnabled);};
  ContentTransitionSample.PropertyChanged+=(_,args)=>{if(args.Property==Motion.EnabledProperty)UpdateMetrics();};
  Model.ScrollRequested+=(_,_)=>Dispatcher.UIThread.Post(()=>{if(((TransitionRecords)Pages[2]).SelectedRow is { } selected) CurrentList()?.ScrollIntoView(selected);UpdateMetrics();},DispatcherPriority.Loaded);
  Model.ResetMotionRequested+=(_,_)=>{ContentTransitionSample.ClearValue(Motion.EnabledProperty);UpdateMetrics();};
 }
 public void SelectPage(int index){Model.SelectPage(index);UpdateMetrics();}
 public ListBox? CurrentList()=>ContentTransitionSample.GetVisualDescendants().OfType<ListBox>().FirstOrDefault(list=>ReferenceEquals(list.DataContext,ContentTransitionSample.Content)&&list.IsEffectivelyVisible);
 private void OnTransitionCompleted(object? sender,TransitionCompletedEventArgs args){Model.CompleteTransition(args.HasRunToCompletion);Dispatcher.UIThread.Post(UpdateMetrics,DispatcherPriority.Background);}
 public void UpdateMetrics()
 {
  if(ContentTransitionSample is null)return;_syncingMotion=true;try{Model.MotionEnabled=Motion.GetEnabled(ContentTransitionSample);}finally{_syncingMotion=false;}
  var presenters=ContentTransitionSample.GetVisualDescendants().OfType<global::Avalonia.Controls.Presenters.ContentPresenter>().Count(presenter=>presenter.TemplatedParent==ContentTransitionSample&&presenter.Content is not null);
  Model.Metrics=$"保留内容 {presenters} / 2 · 完成事件 {CompletedCount} 次 · 动画完成 {AnimatedCount} 次 · {(Model.MotionEnabled?"动效开启":"直接切换")}";
  if(CurrentList() is { } list)Model.Metrics+=$" · 已创建 {list.GetRealizedContainers().Count()} / 1,000 条记录";
 }
}
