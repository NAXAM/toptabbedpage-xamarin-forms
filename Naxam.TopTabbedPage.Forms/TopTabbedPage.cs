using Xamarin.Forms;

namespace Naxam.Controls.Forms
{
    public class TopTabbedPage : TabbedPage
    {
        public TopTabbedPage()
        {
            //BarBackgroundColor = Color.Blue;
            //BarTextColor = Color.White;
        }

        public static readonly BindableProperty BarIndicatorColorProperty = BindableProperty.Create(
            nameof(BarIndicatorColor),
            typeof(Color),
            typeof(TopTabbedPage),
            Color.White,
            BindingMode.OneWay);
        public Color BarIndicatorColor
        {
            get { return (Color)GetValue(BarIndicatorColorProperty); }
            set { SetValue(BarIndicatorColorProperty, value); }
        }


        public static readonly BindableProperty SwipeEnabledColorProperty = BindableProperty.Create(
            nameof(SwipeEnabled),
            typeof(bool),
            typeof(TopTabbedPage),
            true,
            BindingMode.OneWay);
        public bool SwipeEnabled
        {
            get { return (bool)GetValue(SwipeEnabledColorProperty); }
            set { SetValue(SwipeEnabledColorProperty, value); }
        }
    }
}
