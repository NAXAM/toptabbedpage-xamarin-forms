using System;
using UIKit;
using Xamarin.Forms;

namespace Naxam.Controls.Platform.iOS
{
    partial class TopTabbedRenderer
    {
        public override void DidMoveToParentViewController(UIViewController parent)
		{
			base.DidMoveToParentViewController(parent);

			var parentFrame = ParentViewController.View.Frame;

			tabBarHeight.Constant = 48;

			//SetElementSize(new Size(parentFrame.Width, parentFrame.Height));
		}

		public override void ViewDidLayoutSubviews()
		{
			base.ViewDidLayoutSubviews();

			if (Element == null)
				return;
            if (!Element.Bounds.IsEmpty)
			{
				View.Frame = new System.Drawing.RectangleF((float)Element.X, (float)Element.Y, (float)Element.Width, (float)Element.Height);
			}

			var tabsFrame = TabBar.Frame;
			var frame = ParentViewController != null ? ParentViewController.View.Frame : View.Frame;
			var height = frame.Height - tabsFrame.Height;
			PageController.ContainerArea = new Rectangle(0, 0, frame.Width, height);

			if (!_queuedSize.IsZero)
			{
				Element.Layout(new Rectangle(Element.X, Element.Y, _queuedSize.Width, _queuedSize.Height));
				_queuedSize = Size.Zero;
			}

            pageViewController.SetViewControllers(pageViewController.ViewControllers, UIPageViewControllerNavigationDirection.Forward, false, null);

			_loaded = true;
		}

	}
}
