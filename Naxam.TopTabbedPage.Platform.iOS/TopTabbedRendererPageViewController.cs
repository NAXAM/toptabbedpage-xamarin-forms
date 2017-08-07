using UIKit;

namespace Naxam.Controls.Platform.iOS
{
    public partial class TopTabbedRenderer : IUIPageViewControllerDelegate, IUIPageViewControllerDataSource
    {
        public UIViewController GetPreviousViewController(UIPageViewController pageViewController, UIViewController referenceViewController)
        {
            var index = ViewControllers.IndexOf(referenceViewController) - 1;
            if (index < 0) return null;

            return ViewControllers[index];
        }

        public UIViewController GetNextViewController(UIPageViewController pageViewController, UIViewController referenceViewController)
        {
            var index = ViewControllers.IndexOf(referenceViewController) + 1;
            if (index == ViewControllers.Count) return null;

            return ViewControllers[index];
        }
    }
}