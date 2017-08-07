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
                Title = "Top Tabs"
            };
            tabs.Children.Add(new ContentPage {
                Title = "Tab 1",
                BackgroundColor = Color.Aqua
			});
			tabs.Children.Add(new ContentPage
			{
				Title = "Tab 2",
                BackgroundColor = Color.Beige
			});
			tabs.Children.Add(new ContentPage
			{
				Title = "Tab 3",
                BackgroundColor = Color.BlueViolet
			});
            //MainPage = new NavigationPage(tabs) { };

	        MainPage = tabs;
        }
    }
}
