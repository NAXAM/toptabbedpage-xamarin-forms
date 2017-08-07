using Foundation;
using Naxam.Controls.Platform.iOS;
using UIKit;

namespace TopTabbedPageQs.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the
    // User Interface of the application, as well as listening (and optionally responding) to application events from iOS.
    [Register("AppDelegate")]
    public class AppDelegate : Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            // Override point for customization after application launch.
            // If not required for your application you can safely delete this method
            TopTabbedRenderer.Init();
            Xamarin.Forms.Forms.Init();

            LoadApplication(new App());

            return base.FinishedLaunching(app, options);
        }
    }
}

