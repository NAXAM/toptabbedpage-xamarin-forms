using System;
using UIKit;
using System.Linq;
using Xamarin.Forms.Platform.iOS;
using System.Collections.Generic;
using Xamarin.Forms;

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
            UpdateToolbarItems(index);
        }

        void HandleTabsSelectionChanged(object sender, TabsSelectionChangedEventArgs e)
        {
            MoveToByIndex((int)e.SelectedIndex);
        }

        void MoveToByIndex(int selectedIndex, bool forced = false)
        {
            if (selectedIndex == lastSelectedIndex && !forced) return;

            var direction = lastSelectedIndex < selectedIndex
                             ? UIPageViewControllerNavigationDirection.Forward
                             : UIPageViewControllerNavigationDirection.Reverse;

            lastSelectedIndex = selectedIndex;

            SelectedViewController = ViewControllers[lastSelectedIndex];

            UpdateToolbarItems(selectedIndex);

            InvokeOnMainThread(() =>
            {
                pageViewController.SetViewControllers(
                    new[] { SelectedViewController },
                    direction,
                    true, (finished) =>
                    {
                        if (pageViewController.ViewControllers.Length == 0
                            || pageViewController.ViewControllers[0] != SelectedViewController)
                        { //Sometimes setViewControllers doesn't work as expected
                            pageViewController.SetViewControllers(
                            new[] { SelectedViewController },
                            direction,
                            false, null);
                        }
                    }
                );
            });
        }

        void UpdateToolbarItems(int selectedIndex)
        {
            var toolbarItems = new List<ToolbarItem>(Tabbed.ToolbarItems);

            var newChild = Tabbed.Children[selectedIndex];
            var navigationItem = this.NavigationController.TopViewController.NavigationItem;
            toolbarItems.AddRange(newChild.ToolbarItems);

            navigationItem.SetRightBarButtonItems(toolbarItems.Select(x => x.ToUIBarButtonItem()).ToArray(), false);
        }
    }
}
