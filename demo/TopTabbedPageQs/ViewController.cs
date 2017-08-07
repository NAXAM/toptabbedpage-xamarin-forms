using System;
using Foundation;
using MaterialControls;
using UIKit;

namespace TopTabbedPageQs
{
    public partial class ViewController : MDTabBarViewController
    {
        protected ViewController(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Perform any addi tional setup after loading the view, typically from a nib.

            var tabBar = new MDTabBar
            {
                TranslatesAutoresizingMaskIntoConstraints = false
            };

            tabBar.SetItems(new NSObject[] {
                new NSString("Tab 1"),
                new NSString("Tab 2")
            });

            View.AddSubview(tabBar);

            tabBar.AddConstraint(NSLayoutConstraint.Create(
                tabBar,
                NSLayoutAttribute.Height,
                NSLayoutRelation.Equal,
                1,
                56
            ));

            View.AddConstraint(NSLayoutConstraint.Create(
                tabBar,
                NSLayoutAttribute.Top,
                NSLayoutRelation.Equal,
                View,
                NSLayoutAttribute.Top,
                1,
                0
            ));
			View.AddConstraint(NSLayoutConstraint.Create(
				tabBar,
                NSLayoutAttribute.Leading,
				NSLayoutRelation.Equal,
				View,
                NSLayoutAttribute.Leading,
				1,
				0
			));
			View.AddConstraint(NSLayoutConstraint.Create(
				tabBar,
                NSLayoutAttribute.Trailing,
				NSLayoutRelation.Equal,
				View,
                NSLayoutAttribute.Trailing,
				1,
				0
			));
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
    }
}
