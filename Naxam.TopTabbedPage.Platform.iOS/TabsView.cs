using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using MaterialControls;
using UIKit;

namespace Naxam.Controls.Platform.iOS
{
    public class TabsView : UIView, IMDTabBarDelegate
    {
        public event EventHandler<TabsSelectionChangedEventArgs> TabsSelectionChanged;

        readonly MDTabBar _tabBar;

        public override UIColor BackgroundColor
        {
            get => base.BackgroundColor;
            set
            {
                base.BackgroundColor = value;

                if (_tabBar != null)
                {
                    _tabBar.BackgroundColor = value;
                }
            }
        }

        public UIColor IndicatorColor
        {
            get => _tabBar.IndicatorColor;
            set
            {
                _tabBar.IndicatorColor = value;
            }
        }

        public UIColor TextColor
        {
            get { return _tabBar.TextColor; }
            set
            {
                _tabBar.TextColor = value;
            }
        }

        public int SelectedIndex
        {
            get { return (int)_tabBar.SelectedIndex; }
            set
            {
                _tabBar.SelectedIndex = (nuint)value;
            }
        }

        public TabsView()
        {
            _tabBar = new MDTabBar
            {
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            _tabBar.WeakDelegate = this;

            AddSubview(_tabBar);

            var layoutAttributes = new[] {
                NSLayoutAttribute.Bottom,
                NSLayoutAttribute.Trailing,
                NSLayoutAttribute.Leading
            };
            for (int i = 0; i < 3; i++)
            {
                AddConstraint(NSLayoutConstraint.Create(
                    _tabBar,
                    layoutAttributes[i],
                    NSLayoutRelation.Equal,
                    this,
                    layoutAttributes[i],
                    1, 0
                ));
            }

            AddConstraint(NSLayoutConstraint.Create(
                _tabBar,
                NSLayoutAttribute.Top,
                NSLayoutRelation.GreaterThanOrEqual,
                this,
                NSLayoutAttribute.Top,
                1, 0
            ));

            _tabBar.AddConstraint(NSLayoutConstraint.Create(
                _tabBar,
                NSLayoutAttribute.Height,
                NSLayoutRelation.Equal,
                1, 48
            ));
        }

        internal void SetItems(IEnumerable<string> titles)
        {
            _tabBar.SetItems(titles.Select(x => new NSString(x)).Cast<NSObject>().ToArray());
        }

        internal void ReplaceItem(string title, int index)
        {
           _tabBar.ReplaceItem(new NSString(title), (nuint)index);
        }

        public void DidChangeSelectedIndex(MDTabBar tabBar, nuint selectedIndex)
        {
            TabsSelectionChanged?.Invoke(this, new TabsSelectionChangedEventArgs(selectedIndex));
        }
    }

    public class TabsSelectionChangedEventArgs : EventArgs
    {
        public nuint SelectedIndex { get; }

        public TabsSelectionChangedEventArgs(nuint selectedIndex)
        {
            SelectedIndex = selectedIndex;
        }
    }
}