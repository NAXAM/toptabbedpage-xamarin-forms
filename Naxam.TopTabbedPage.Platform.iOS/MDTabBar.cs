using System;
using CoreAnimation;
using CoreGraphics;
using Foundation;
using UIKit;

namespace Naxam.Controls.Platform.iOS
{
    public class MDSegmentedControl : UISegmentedControl
    {
        static readonly int kMDTabBarHeight = 48;
        static readonly int kMDIndicatorHeight = 48;

        UIView IndicatorView;
        UIView BeingTouchedView;
        UIFont Font;
        MDTabBar TabBar;

        public nfloat HorizontalPadding { get; set; }
        public UIColor RippleColor { get; set; }
        public UIColor IndicatorColor { get; set; }
        NSMutableArray<UIView> Tabs { get; set; }

        public MDSegmentedControl(MDTabBar bar)
        {
            Tabs = new NSMutableArray<UIView>();
            IndicatorView = new UIView(new CGRect(0, kMDTabBarHeight - kMDIndicatorHeight, 0, kMDIndicatorHeight));
            IndicatorView.Tag = int.MaxValue;
            AddSubview(IndicatorView);
            AddTarget(SelectionChanged, UIControlEvent.ValueChanged);
            TabBar = bar;
        }

        public override void WillMoveToSuperview(UIView newsuper)
        {
            base.WillMoveToSuperview(newsuper);
            newsuper.AddObserver("frame", 0, null);
        }

        public override void RemoveFromSuperview()
        {
            Superview.RemoveObserver(this, "frame");
            base.RemoveFromSuperview();
        }

        public override nint SelectedSegment
        {
            get => base.SelectedSegment;
            set
            {
                base.SelectedSegment = value;
                MoveIndicatorToSelectedIndexWithAnimated(true);
            }
        }

        public override void Select(NSObject sender)
        {
            base.Select(sender);
            TabBar.UpdateSelectedIndex(SelectedSegment);
        }

        public override void ObserveValue(NSString keyPath, NSObject ofObject, NSDictionary change, IntPtr context)
        {
            base.ObserveValue(keyPath, ofObject, change, context);
            if (ofObject == Superview && keyPath == "frame")
            {
                ResizeItems();
                UpdateSegmentsList();
                MoveIndicatorToSelectedIndexWithAnimated(false);
            }
        }

        private void SelectionChanged(object sender, EventArgs e)
        {

        }

        public override void InsertSegment(UIImage image, nint pos, bool animated)
        {
            base.InsertSegment(image, pos, animated);
            ResizeItems();
            UpdateSegmentsList();
            PerformSelector(new ObjCRuntime.Selector("MoveIndicatorToSelectedIndexWithAnimated:"), NSNumber.FromBoolean(animated), 0.001f);
        }

        public override void InsertSegment(string title, nint pos, bool animated)
        {
            base.InsertSegment(title, pos, animated);
            ResizeItems();
            UpdateSegmentsList();
            PerformSelector(new ObjCRuntime.Selector("MoveIndicatorToSelectedIndexWithAnimated:"), NSNumber.FromBoolean(animated), 0.001f);
        }

        public override void SetImage(UIImage image, nint segment)
        {
            base.SetImage(image, segment);
            ResizeItems();
            PerformSelector(new ObjCRuntime.Selector("MoveIndicatorToSelectedIndexWithAnimated:"), NSNumber.FromBoolean(true), 0.001f);
        }

        public override void RemoveSegmentAtIndex(nint segment, bool animated)
        {
            base.RemoveSegmentAtIndex(segment, animated);
            UpdateSegmentsList();
            ResizeItems();
            PerformSelector(new ObjCRuntime.Selector("MoveIndicatorToSelectedIndexWithAnimated:"), NSNumber.FromBoolean(animated), 0.001f);
        }

        void SetIndicatorColor(UIColor color)
        {
            IndicatorColor = color;
            IndicatorView.BackgroundColor = color;
        }

        void SetTextFont(UIFont textFont, UIColor textColor)
        {
            Font = textFont;
            nfloat disabledTextAlpha = 0.6f;
            var normalTextColor = TabBar.NormalTextColor;
            if (normalTextColor == null)
            {
                normalTextColor = textColor.ColorWithAlpha(disabledTextAlpha);
            }
            var normalTextFont = TabBar.NormalTextFont;
            if (normalTextFont == null)
            {
                normalTextFont = textFont;
            }
            var attributes = new UITextAttributes()
            {
                Font = normalTextFont,
                TextColor = normalTextColor
            };
            SetTitleTextAttributes(attributes, UIControlState.Normal);
            var selectedAttributes = new UITextAttributes()
            {
                Font = textFont,
                TextColor = textColor
            };
            SetTitleTextAttributes(selectedAttributes, UIControlState.Selected);
        }

        void MoveIndicatorToFrameWithAnimated(CGRect frame, bool animated)
        {
            if (animated)
            {
                UIView.Animate(.2f, () =>
                {
                    IndicatorView.Frame = new CGRect(frame.Left, Bounds.Size.Height - kMDIndicatorHeight, frame.Size.Width, kMDIndicatorHeight);
                }, null);
            }
            else
            {
                IndicatorView.Frame = new CGRect(frame.Left, Bounds.Size.Height - kMDIndicatorHeight, frame.Size.Width, kMDIndicatorHeight);
            }
        }

        [Export("MoveIndicatorToSelectedIndexWithAnimated:")]
        public void MoveIndicatorToSelectedIndexWithAnimated(bool animated)
        {
            if (SelectedSegment < 0 && NumberOfSegments > 0)
            {
                SelectedSegment = 0;
            }
            var index = SelectedSegment;
            var frame = CGRect.Empty;
            if (index > 0)
            {
                if ((index >= SelectedSegment) || (index >= (nint)Tabs.Count))
                {
                    return;
                }
                frame = Tabs[(nuint)index].Frame;
            }
            MoveIndicatorToFrameWithAnimated(frame, animated);
        }

        void ResizeItems()
        {
            if (NumberOfSegments <= 0) return;
            nfloat maxItemSize = 0;
            nfloat segmentedControlWidth = 0;
            var attributes = new UIStringAttributes()
            {
                Font = Font
            };
            for (int i = 0; i < NumberOfSegments; i++)
            {
                var title = TitleAt(i);
                var itemSize = CGSize.Empty;
                if (!string.IsNullOrEmpty(title))
                {
                    itemSize = new NSString(title).GetSizeUsingAttributes(attributes);
                }
                else
                {
                    var image = ImageAt(i);
                    var height = Bounds.Size.Height;
                    var width = Bounds.Size.Width;
                    itemSize = new CGSize(width, height);
                }
                itemSize.Width += HorizontalPadding * 2;
                SetWidth(itemSize.Width, i);
                segmentedControlWidth += (itemSize.Width);
                maxItemSize = (nfloat)Math.Max(maxItemSize, itemSize.Width);
            }

            var holderWidth = Superview.Bounds.Size.Width - TabBar.HorizontalInset * 2;
            if (segmentedControlWidth < holderWidth)
            {
                if (NumberOfSegments * maxItemSize < holderWidth)
                {
                    maxItemSize = holderWidth / NumberOfSegments;
                }
                segmentedControlWidth = 0;
                for (int i = 0; i < NumberOfSegments; i++)
                {
                    SetWidth(maxItemSize, i);
                    segmentedControlWidth += maxItemSize;
                }
            }
            Frame = new CGRect(0, 0, segmentedControlWidth, kMDTabBarHeight);
        }

        public CGRect GetSelectedSegmentFrame()
        {
            if (SelectedSegment >= 0)
            {
                return Tabs[(System.nuint)SelectedSegment].Frame;
            }
            return CGRect.Empty;
        }

        void UpdateSegmentsList()
        {
            var segments = GetSegmentList().MutableCopy();
            if (segments is NSMutableArray<UIView> tabs)
                Tabs = tabs;
        }

        NSArray GetSegmentList()
        {
            LayoutIfNeeded();
            var segments = new NSMutableArray((nuint)NumberOfSegments);
            foreach (UIView view in Subviews)
            {
                if (view.GetType().Name == "UISegment")
                {
                    segments.Add(view);
                }
            }
            var sortedSegments = segments.Sort((a, b) =>
            {
                if (a is UIView viewA && b is UIView viewB)
                {
                    if (viewA.Frame.Left < viewB.Frame.Left)
                    {
                        return NSComparisonResult.Ascending;
                    }
                    else if (viewA.Frame.Left > viewB.Frame.Left)
                    {
                        return NSComparisonResult.Descending;
                    }
                    return NSComparisonResult.Same;
                }
                return NSComparisonResult.Same;
            });
            return sortedSegments;
        }

        public override void TouchesBegan(NSSet touches, UIEvent evt)
        {
            base.TouchesBegan(touches, evt);
            if (BeingTouchedView != null)
                return;
            
        }

        public override void TouchesCancelled(NSSet touches, UIEvent evt)
        {
            base.TouchesCancelled(touches, evt);
        }

        public override void TouchesEnded(NSSet touches, UIEvent evt)
        {
            base.TouchesEnded(touches, evt);
        }
    }

    public class MDTabBar : UIView
    {
        public MDSegmentedControl SegmentedControl { get; set; }
        public UIScrollView ScrollView { get; set; }

        public MDTabBar()
        {
        }

        public UIColor TextColor { get; set; }
        public UIColor NormalTextColor { get; set; }
        public UIColor IndicatorColor { get; set; }
        public float HorizontalInset { get; set; }
        public float HorizontalPaddingPerItem { get; set; }
        public UIColor RippleColor { get; set; }
        public UIFont TextFont { get; set; }
        public UIFont NormalTextFont { get; set; }
        public nuint SelectedIndex { get; set; }
        public NSObject WeakDelegate { get; set; }
        public MDTabBarDelegate Delegate { get; set; }
        public nint NumberOfItems { get; }
        public void SetItems(NSObject[] items) { }
        public void ReplaceItem(NSObject item, nuint index) { }
        public NSMutableArray<UIView> Tabs()
        {
            return new NSMutableArray<UIView>();
        }

        public void UpdateSelectedIndex(nint selectedIndex)
        {

        }
        public void MoveIndicatorToFrame(CGRect frame, bool animated) { }

        void InitContent()
        {
            HorizontalInset = 8;
            SegmentedControl = new MDSegmentedControl(this);
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            ScrollView.Frame = new CGRect(0, 0, Bounds.Size.Width, Bounds.Size.Height);
            ScrollView.ContentInset = new UIEdgeInsets(0, HorizontalInset, 0, HorizontalInset);
            ScrollView.ContentSize = SegmentedControl.Bounds.Size;
        }
    }

    public interface MDTabBarDelegate
    {
        void DidChangeSelectedIndex(MDTabBar tabBar, nuint selectedIndex);
    }
}
