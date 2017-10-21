using System;
using UIKit;

namespace Naxam.Controls.Platform.iOS
{
    partial class TopTabbedRenderer
    {
        void HandlePageViewControllerDidFinishAnimating(object sender, UIPageViewFinishedAnimationEventArgs e)
        {
            if (pageViewController.ViewControllers.Length == 0) return;

            SelectedViewController = pageViewController.ViewControllers[0];
            var index = ViewControllers.IndexOf(SelectedViewController);

            TabBar.SelectedIndex = index;
            lastSelectedIndex = index;
        }

        void HandleTabsSelectionChanged(object sender, TabsSelectionChangedEventArgs e)
        {
            MoveToByIndex((int)e.SelectedIndex);
            var navigationItem = this.NavigationController.TopViewController.NavigationItem;
            navigationItem.SetRightBarButtonItems(newChild.ToolbarItems.Select(x=>x.ToUIBarButtonItem()).ToArray(), false);            
        }

        void MoveToByIndex(int selectedIndex, bool forced = false)
        {
            if (selectedIndex == lastSelectedIndex && !forced) return;

            var direction = lastSelectedIndex < selectedIndex
                             ? UIPageViewControllerNavigationDirection.Forward
                             : UIPageViewControllerNavigationDirection.Reverse;

            lastSelectedIndex = selectedIndex;

            SelectedViewController = ViewControllers[lastSelectedIndex];
            InvokeOnMainThread(() =>
            {
                pageViewController.SetViewControllers(
                    new[] { SelectedViewController },
                    direction,
                    true, (finished) => {
                    if (pageViewController.ViewControllers.Length == 0 
                        || pageViewController.ViewControllers[0] != SelectedViewController) { //Sometimes setViewControllers doesn't work as expected
                            pageViewController.SetViewControllers(
                            new[] { SelectedViewController },
                            direction,
                            false, null);
                    }
                }
                );
            });
        }
    }
}
