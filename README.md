TopTabbedPage - Xamarin.Forms control
=====

A Xamarin.Forms page based TabbedPage to show tabs at top on iOS.


|![Tab 1](./art/tab1.png)|![Tab 5](./art/tab5.png)|
|:---:|:---:|
## Installation

    Install-Package Naxam.TopTabbedPage.Forms

## Usage
This control is used the same as standard tabbed page, except it has one more options to set selected tab indictor color, `BarIndicatorColor`.

```xml
<forms:TopTabbedPage
    xmlns="http://xamarin.com/schemas/2014/forms"
    xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
    x:Class="Naxam.Demo.TestPage"
    xmlns:views="clr-namespace:Naxam.Demo"
    xmlns:forms="clr-namespace:Naxam.Controls.Forms;assembly=Naxam.TopTabbedPage.Forms"
    BarTextColor="#00b9e1"
    BarIndicatorColor="#00b9e1"
    BarBackgroundColor="#ffffff"
    Title="MyRide">
    <views:Page1 />
    <views:Page2 />
</forms:TopTabbedPage>
```
