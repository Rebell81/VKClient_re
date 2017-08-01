using Microsoft.Phone.Shell;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using VKClient.Common.UC;
using VKClient.Common.Utils;

namespace VKClient.Common.Framework
{
  public class FullscreenLoader
  {
    private Panel _container;
    private FrameworkElement _overlay;
    private Border _childElement;
    private PageBase _currentPage;
    private IApplicationBar _applicationBar;

    public bool HideOnBackKeyPress { get; set; }

    public bool IsShowed { get; private set; }

    public Action ShowedCallback { get; set; }

    public Action<FullscreenLoaderHiddenEventArgs> HiddenCallback { get; set; }

    public void Show(FrameworkElement childElement = null, bool addOverlay = true)
    {
      if (this.IsShowed)
        return;
      Execute.ExecuteOnUIThread((Action) (() =>
      {
        this._currentPage = FramePageUtils.CurrentPage;
        if (this._currentPage == null)
          return;
        this._container = ((UserControl) this._currentPage).Content as Panel;
        if (this._container == null)
          return;
        this._childElement = FullscreenLoader.CreatePopupContainer();
        this._childElement.Child = ((UIElement) (childElement ?? (FrameworkElement) new FullscreenLoadUC()));
        if (addOverlay)
        {
          this._overlay = FullscreenLoader.CreateOverlay();
          ((PresentationFrameworkCollection<UIElement>) this._container.Children).Add((UIElement) this._overlay);
        }
        ((PresentationFrameworkCollection<UIElement>) this._container.Children).Add((UIElement) this._childElement);
        this.PreparePage(true);
        this.IsShowed = true;
        Action showedCallback = this.ShowedCallback;
        if (showedCallback == null)
          return;
        showedCallback();
      }));
    }

    public void Hide(bool byBackKey = false)
    {
      if (!this.IsShowed)
        return;
      Execute.ExecuteOnUIThread((Action) (() =>
      {
        if (this._overlay != null && ((PresentationFrameworkCollection<UIElement>) this._container.Children).Contains((UIElement) this._overlay))
          ((PresentationFrameworkCollection<UIElement>) this._container.Children).Remove((UIElement) this._overlay);
        if (((PresentationFrameworkCollection<UIElement>) this._container.Children).Contains((UIElement) this._childElement))
          ((PresentationFrameworkCollection<UIElement>) this._container.Children).Remove((UIElement) this._childElement);
        this.PreparePage(false);
        this.IsShowed = false;
        Action<FullscreenLoaderHiddenEventArgs> hiddenCallback = this.HiddenCallback;
        if (hiddenCallback == null)
          return;
        FullscreenLoaderHiddenEventArgs loaderHiddenEventArgs = new FullscreenLoaderHiddenEventArgs();
        int num = byBackKey ? 1 : 0;
        loaderHiddenEventArgs.ByBackKey = num != 0;
        hiddenCallback(loaderHiddenEventArgs);
      }));
    }

    private static FrameworkElement CreateOverlay()
    {
      Border border = new Border();
      SolidColorBrush solidColorBrush = new SolidColorBrush(Colors.Transparent);
      border.Background = ((Brush) solidColorBrush);
      int num = 0;
      ((FrameworkElement) border).UseOptimizedManipulationRouting=(num != 0);
      int maxValue1 = int.MaxValue;
      Grid.SetRowSpan((FrameworkElement) border, maxValue1);
      int maxValue2 = int.MaxValue;
      Grid.SetColumnSpan((FrameworkElement) border, maxValue2);
      return (FrameworkElement) border;
    }

    private void PreparePage(bool showLoader)
    {
      if (showLoader)
      {
        if (!this._currentPage.FullscreenLoaders.Contains(this))
          this._currentPage.FullscreenLoaders.Add(this);
        this._applicationBar = this._currentPage.ApplicationBar;
        this._currentPage.ApplicationBar = ( null);
      }
      else
      {
        if (this._currentPage.FullscreenLoaders.Contains(this))
          this._currentPage.FullscreenLoaders.Remove(this);
        if (this._applicationBar == null)
          return;
        this._currentPage.ApplicationBar = this._applicationBar;
        this._applicationBar =  null;
      }
    }

    private static Border CreatePopupContainer()
    {
      Border border = new Border();
      string str = Guid.NewGuid().ToString();
      ((FrameworkElement) border).Name = str;
      int maxValue1 = int.MaxValue;
      Grid.SetColumnSpan((FrameworkElement) border, maxValue1);
      int maxValue2 = int.MaxValue;
      Grid.SetRowSpan((FrameworkElement) border, maxValue2);
      return border;
    }
  }
}
