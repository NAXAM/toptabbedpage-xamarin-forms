using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Foundation;
using MaterialControls;
using Naxam.Controls.Forms;
using Naxam.Controls.Platform.iOS;
using Naxam.Controls.Platform.iOS.Utils;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(TopTabbedPage), typeof(TopTabbedRenderer))]

namespace Naxam.Controls.Platform.iOS
{
    using Platform = Xamarin.Forms.Platform.iOS.Platform;
    using Forms = Xamarin.Forms.Forms;

    public partial class TopTabbedRenderer :
        UIViewController
    {
        public static void Init()
        {
        }

        UIColor _defaultBarColor;
        bool _defaultBarColorSet;
        bool _loaded;
        Size _queuedSize;
        int lastSelectedIndex;

        Page Page => Element as Page;

        UIPageViewController pageViewController;

        protected UIViewController SelectedViewController;
        protected IList<UIViewController> ViewControllers;

        protected IPageController PageController
        {
            get { return Page; }
        }

        protected TopTabbedPage Tabbed
        {
            get { return (TopTabbedPage)Element; }
        }

        protected TabsView TabBar;
        private NSLayoutConstraint tabBarHeight;

        public TopTabbedRenderer()
        {
            ViewControllers = new UIViewController[0];

            pageViewController = new UIPageViewController(
                UIPageViewControllerTransitionStyle.Scroll,
                UIPageViewControllerNavigationOrientation.Horizontal,
                UIPageViewControllerSpineLocation.None
            );

            TabBar = new TabsView
            {
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            TabBar.TabsSelectionChanged += HandleTabsSelectionChanged;
        }

        public override void DidRotate(UIInterfaceOrientation fromInterfaceOrientation)
        {
            base.DidRotate(fromInterfaceOrientation);

            View.SetNeedsLayout();
        }

        public override void ViewDidAppear(bool animated)
        {
            PageController.SendAppearing();
            base.ViewDidAppear(animated);
        }

        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);
            PageController.SendDisappearing();
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            View.AddSubview(TabBar);

            AddChildViewController(pageViewController);
            View.AddSubview(pageViewController.View);
            pageViewController.View.TranslatesAutoresizingMaskIntoConstraints = false;
            pageViewController.DidMoveToParentViewController(this);


            var views = NSDictionary.FromObjectsAndKeys(
                new NSObject[] {
                TabBar,
                pageViewController.View
            },
                new NSObject[] {
                (NSString) "tabbar",
                (NSString) "content"
            }
            );

            View.AddConstraints(NSLayoutConstraint.FromVisualFormat("V:|-0-[tabbar]-0-[content]-0-|",
                                                                    0,
                                                                    null,
                                                                    views));
            View.AddConstraints(NSLayoutConstraint.FromVisualFormat("H:|-0-[tabbar]-0-|",
                                                                    0,
                                                                    null,
                                                                    views));
            View.AddConstraints(NSLayoutConstraint.FromVisualFormat("H:|-0-[content]-0-|",
                                                                    0,
                                                                    null,
                                                                    views));


            tabBarHeight = NSLayoutConstraint.Create(
                TabBar,
                NSLayoutAttribute.Height,
                NSLayoutRelation.Equal,
                1, 68
            );
            TabBar.AddConstraint(tabBarHeight);

            if (pageViewController.ViewControllers.Length == 0
                && lastSelectedIndex >= 0 || lastSelectedIndex < ViewControllers.Count)
            {
                pageViewController.SetViewControllers(
                    new[] { ViewControllers[lastSelectedIndex] },
                   UIPageViewControllerNavigationDirection.Forward,
                   true, null
                );
            }

            UpdateSwipe(Tabbed.SwipeEnabled);
            pageViewController.DidFinishAnimating += HandlePageViewControllerDidFinishAnimating;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                PageController?.SendDisappearing();

                if (Tabbed != null)
                {
                    Tabbed.PropertyChanged -= OnPropertyChanged;
                    Tabbed.PagesChanged -= OnPagesChanged;
                    TabBar.TabsSelectionChanged -= HandleTabsSelectionChanged;
                }

                if (pageViewController != null)
                {
                    pageViewController.WeakDataSource = null;
                    pageViewController.DidFinishAnimating -= HandlePageViewControllerDidFinishAnimating;
                    pageViewController?.Dispose();
                }
            }

            base.Dispose(disposing);
        }

        protected virtual void OnElementChanged(VisualElementChangedEventArgs e)
        {
            ElementChanged?.Invoke(this, e);
        }

        UIViewController GetViewController(Page page)
        {
            var renderer = Platform.GetRenderer(page);
            return renderer?.ViewController;
        }

        void OnPagePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != Page.TitleProperty.PropertyName)
                return;

            if (!(sender is Page page) || page.Title is null)
                return;

            TabBar.ReplaceItem(page.Title, Tabbed.Children.IndexOf(page));
        }

        void OnPagesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            e.Apply((o, i, c) => SetupPage((Page)o, i), (o, i) => TeardownPage((Page)o, i), Reset);

            SetControllers();

            UIViewController controller = null;
            if (Tabbed.CurrentPage != null)
            {
                controller = GetViewController(Tabbed.CurrentPage);
            }

            if (controller != null && controller != SelectedViewController)
            {
                SelectedViewController = controller;
                var index = ViewControllers.IndexOf(SelectedViewController);
                MoveToByIndex(index);
                TabBar.SelectedIndex = index;
            }
        }

        void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(TabbedPage.CurrentPage))
            {
                var current = Tabbed.CurrentPage;
                if (current == null)
                    return;

                var controller = GetViewController(current);
                if (controller == null)
                    return;

                SelectedViewController = controller;
                var index = ViewControllers.IndexOf(SelectedViewController);
                MoveToByIndex(index);
                TabBar.SelectedIndex = index;
            }
            else if (e.PropertyName == TabbedPage.BarBackgroundColorProperty.PropertyName)
            {
                UpdateBarBackgroundColor();
            }
            else if (e.PropertyName == TabbedPage.BarTextColorProperty.PropertyName)
            {
                UpdateBarTextColor();
            }
            else if (e.PropertyName == TopTabbedPage.BarIndicatorColorProperty.PropertyName)
            {
                UpdateBarIndicatorColor();
            }
            else if (e.PropertyName == TopTabbedPage.SwipeEnabledColorProperty.PropertyName)
            {
                UpdateSwipe(Tabbed.SwipeEnabled);
            }
        }

        public override UIViewController ChildViewControllerForStatusBarHidden()
        {
            var current = Tabbed.CurrentPage;
            if (current == null)
                return null;

            return GetViewController(current);
        }

        void UpdateSwipe(bool swipeEnabled)
        {
            pageViewController.WeakDataSource = swipeEnabled ? this : null;
        }

        void Reset()
        {
            var i = 0;
            foreach (var page in Tabbed.Children)
            {
                SetupPage(page, i++);
            }
        }

        void SetControllers()
        {
            var list = new List<UIViewController>();
            var titles = new List<string>();
            for (var i = 0; i < Tabbed.Children.Count; i++)
            {
                var child = Tabbed.Children[i];
                var v = child as VisualElement;
                if (v == null)
                    continue;

                var renderer = Platform.GetRenderer(v);
                if (renderer == null) continue;

                list.Add(renderer.ViewController);

                titles.Add(Tabbed.Children[i].Title);
            }
            ViewControllers = list.ToArray();
            TabBar.SetItems(titles);
        }

        void SetupPage(Page page, int index)
        {
            IVisualElementRenderer renderer = Platform.GetRenderer(page);
            if (renderer == null)
            {
                renderer = Platform.CreateRenderer(page);
                Platform.SetRenderer(page, renderer);
            }

            page.PropertyChanged -= OnPagePropertyChanged;
            page.PropertyChanged += OnPagePropertyChanged;
        }

        void TeardownPage(Page page, int index)
        {
            page.PropertyChanged -= OnPagePropertyChanged;

            Platform.SetRenderer(page, null);
        }

        void UpdateBarBackgroundColor()
        {
            if (Tabbed == null || TabBar == null)
                return;

            var barBackgroundColor = Tabbed.BarBackgroundColor;

            if (!_defaultBarColorSet)
            {
                _defaultBarColor = TabBar.BackgroundColor;

                _defaultBarColorSet = true;
            }

            TabBar.BackgroundColor = barBackgroundColor.ToUIColor();
        }

        void UpdateBarTextColor()
        {
            TabBar.TextColor = Tabbed.BarTextColor.ToUIColor();
        }

        void UpdateBarIndicatorColor()
        {
            TabBar.IndicatorColor = Tabbed.BarIndicatorColor.ToUIColor();
        }
    }
}
