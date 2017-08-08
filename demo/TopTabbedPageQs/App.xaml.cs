using System;
using System.Collections.Generic;
using Naxam.Controls.Forms;
using Xamarin.Forms;

namespace TopTabbedPageQs
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            var tabs = new TopTabbedPage
            {
                Title = "Top Tabs",
                BarBackgroundColor = Color.FromHex("9C27B0"),
				//BarIndicatorColor = Color.DeepPink,
				//BarTextColor = Color.DeepPink
			};
            tabs.Children.Add(new MyPage
			{
				Title = "My Page",
				BackgroundColor = Color.Aquamarine
			});
            tabs.Children.Add(new ContentPage {
                Title = "Tab 1",
				BackgroundColor = Color.Aqua,
				Content = new Label
				{
					HorizontalTextAlignment = TextAlignment.Center,
					VerticalTextAlignment = TextAlignment.Center,
					Text = "TopTabbedPage - A Xamarin.Forms page with tabs at the top.",
                    TextColor = Color.DarkCyan,
					Margin = new Thickness(16)
				}
			});
			tabs.Children.Add(new ContentPage
			{
				Title = "Tab 2",
				BackgroundColor = Color.Beige,
				Content = new Label
				{
					HorizontalTextAlignment = TextAlignment.Center,
					VerticalTextAlignment = TextAlignment.Center,
					Text = "TabsView internally wrapps MDTabBar.",
                    TextColor = Color.Green,
					Margin = new Thickness(16)
				}
			});
			tabs.Children.Add(new ContentPage
			{
				Title = "Tab 3",
				BackgroundColor = Color.BlueViolet,
				Content = new Label
				{
					HorizontalTextAlignment = TextAlignment.Center,
					VerticalTextAlignment = TextAlignment.Center,
					Text = "TopTabbedPage could be embedded inside a NavigationPage.",
                    TextColor = Color.Aqua,
					Margin = new Thickness(16)
				}
			});
			tabs.Children.Add(new ContentPage
			{
				Title = "Tab 4",
				BackgroundColor = Color.LightYellow,
				Content = new Label
				{
					HorizontalTextAlignment = TextAlignment.Center,
					VerticalTextAlignment = TextAlignment.Center,
					Text = "TopTabbedPage is created while creating MyRide app showcase.",
                    TextColor = Color.DarkBlue,
					Margin = new Thickness(16)
				}
			});
			tabs.Children.Add(new ContentPage
			{
				Title = "Tab 5",
				BackgroundColor = Color.Bisque,
                Content = new Label {
                    HorizontalTextAlignment = TextAlignment.Center,
                    VerticalTextAlignment = TextAlignment.Center,
                    Text = "TopTabbedPage is a product developed by NAXAM",
                    TextColor = Color.DarkGreen,
                    Margin = new Thickness(16)
                }
			});

            MainPage = new NavigationPage(tabs) {
                BarBackgroundColor = Color.FromHex("9C27B0"),
                BarTextColor = Color.White
            };

	        //MainPage = tabs;
        }
    }
}
