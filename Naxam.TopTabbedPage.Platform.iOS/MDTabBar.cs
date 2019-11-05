using System;
using System.Diagnostics;
using CoreAnimation;
using CoreGraphics;
using Foundation;
using UIKit;
using Xamarin.Forms;

namespace Naxam.Controls.Platform.iOS
{
    public class MDSegmentedControl : UISegmentedControl
    {
        static readonly int kMDTabBarHeight = 48;
        static readonly int kMDIndicatorHeight = 2;
        IDisposable frameObserver;
        UIView IndicatorView;
        UIView BeingTouchedView;
        public UIFont Font;
        MDTabBar TabBar;

        public nfloat HorizontalPadding { get; set; }
        public UIColor RippleColor { get; set; }
        public UIColor IndicatorColor { get; set; }
        public NSMutableArray<UIView> Tabs { get; set; }

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
            if (newsuper != null) frameObserver = newsuper.AddObserver("frame", NSKeyValueObservingOptions.Initial | NSKeyValueObservingOptions.New, OnFrameChanged);
        }

        private void OnFrameChanged(NSObservedChange obj)
        {
            ResizeItems();
            UpdateSegmentsList();
            MoveIndicatorToSelectedIndexWithAnimated(false);
        }

        public override void RemoveFromSuperview()
        {
            if (frameObserver != null) frameObserver.Dispose();
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

        private void SelectionChanged(object sender, EventArgs e)
        {
            MoveIndicatorToSelectedIndexWithAnimated(true);
            TabBar.UpdateSelectedIndex(SelectedSegment);
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

        public void SetIndicatorColor(UIColor color)
        {
            IndicatorColor = color;
            IndicatorView.BackgroundColor = color;
        }

        public void SetTextFont(UIFont textFont, UIColor textColor)
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

        public void MoveIndicatorToFrameWithAnimated(CGRect frame, bool animated)
        {
            if (animated)
            {
                UIView.Animate(.2f, () =>
                {
                    IndicatorView.Frame = new CGRect(frame.Location.X, Bounds.Size.Height - kMDIndicatorHeight, frame.Size.Width, kMDIndicatorHeight);
                }, null);
            }
            else
            {
                IndicatorView.Frame = new CGRect(frame.Location.X, Bounds.Size.Height - kMDIndicatorHeight, frame.Size.Width, kMDIndicatorHeight);
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
            if ((Tabs != null) && (index >= 0))
            {
                if ((index >= NumberOfSegments) || (index >= (nint)Tabs.Count))
                {
                    return;
                }
                frame = Tabs[(nuint)SelectedSegment].Frame;
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
                    var width = height / image.Size.Height * image.Size.Width;
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
            if (SelectedSegment >= 0 && Tabs.Count > 0)
            {
                var frame = Tabs[(nuint)SelectedSegment].Frame;
                return frame;
            }
            return CGRect.Empty;
        }

        void UpdateSegmentsList()
        {
            var segments = GetSegmentList().MutableCopy();
            if (segments is NSArray tabs)
            {
                Tabs = new NSMutableArray<UIView>(NSArray.FromArray<UIView>(tabs));
            }

        }

        NSArray GetSegmentList()
        {
            LayoutIfNeeded();
            var segments = new NSMutableArray((nuint)NumberOfSegments);
            foreach (UIView view in Subviews)
            {
                if (view.Class.Name == "UISegment")
                {
                    segments.Add(view);
                }
            }
            var sortedSegments = segments.Sort((a, b) =>
            {
                if (a is UIView viewA && b is UIView viewB)
                {
                    if (viewA.Frame.Location.X < viewB.Frame.Location.X)
                    {
                        return NSComparisonResult.Ascending;
                    }
                    else if (viewA.Frame.Location.X > viewB.Frame.Location.X)
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
            UITouch touch = touches.AnyObject as UITouch;
            var point = touch.LocationInView(this);
            foreach (UIView view in Subviews)
            {
                if (view.Tag != int.MaxValue && view.Frame.Contains(point))
                {
                    BeingTouchedView = view;
                }
            }
        }

        public override void TouchesCancelled(NSSet touches, UIEvent evt)
        {
            base.TouchesCancelled(touches, evt);
            BeingTouchedView = null;
        }

        public override void TouchesEnded(NSSet touches, UIEvent evt)
        {
            base.TouchesEnded(touches, evt);
            BeingTouchedView = null;
        }
    }

    public class MDTabBar : UIView
    {
        IDisposable frameObserver;
        public MDSegmentedControl SegmentedControl { get; set; }
        public UIScrollView ScrollView { get; set; }

        public IMDTabBarDelegate WeakDelegate { get; set; }

        public nint NumberOfItem { get => SegmentedControl.NumberOfSegments; }
        public NSMutableArray<UIView> Tabs
        {
            get
            {
                return (Foundation.NSMutableArray<UIKit.UIView>)SegmentedControl?.Tabs.Copy();
            }
        }

        public MDTabBar(NSCoder nSCoder) : base(nSCoder)
        {
            InitContent();
        }

        public MDTabBar(CGRect frame) : base(frame)
        {
            InitContent();
        }

        public MDTabBar()
        {
            InitContent();
        }

        public override UIColor BackgroundColor
        {
            get => base.BackgroundColor;
            set
            {
                base.BackgroundColor = value;
                ScrollView.BackgroundColor = value;
            }
        }

        private UIColor _TextColor;
        public UIColor TextColor
        {
            get { return _TextColor; }
            set
            {
                _TextColor = value;
                UpdateItemAppearance();
            }
        }

        private UIColor _NormalTextColor;
        public UIColor NormalTextColor
        {
            get { return _NormalTextColor; }
            set
            {
                _NormalTextColor = value;
                UpdateItemAppearance();
            }
        }

        private UIColor _IndicatorColor;
        public UIColor IndicatorColor
        {
            get { return _IndicatorColor; }
            set
            {
                _IndicatorColor = value;
                SegmentedControl?.SetIndicatorColor(value);
            }
        }

        nfloat _HorizontalPaddingPerItem;
        public nfloat HorizontalPaddingPerItem
        {
            get => _HorizontalPaddingPerItem;
            set
            {
                _HorizontalPaddingPerItem = value;
                SegmentedControl.HorizontalPadding = _HorizontalPaddingPerItem;
            }
        }

        private UIFont _TextFont;
        public UIFont TextFont
        {
            get => _TextFont;
            set
            {
                _TextFont = value;
                UpdateItemAppearance();
            }
        }

        private UIFont _NormalTextFont;
        public UIFont NormalTextFont
        {
            get => _NormalTextFont;
            set
            {
                _NormalTextFont = value;
                UpdateItemAppearance();
            }
        }

        nuint _SelectedIndex;
        public nuint SelectedIndex
        {
            get => _SelectedIndex;
            set
            {
                if (value < (nuint)SegmentedControl?.NumberOfSegments)
                {
                    _SelectedIndex = value;
                    if ((nuint)SegmentedControl?.SelectedSegment != _SelectedIndex)
                    {
                        SegmentedControl.SelectedSegment = (nint)_SelectedIndex;
                        ScrollToSelectedIndex();
                    }
                }
            }
        }

        float _HorizontalInset;
        public float HorizontalInset
        {
            get => _HorizontalInset;
            set
            {
                _HorizontalInset = value;
                SetNeedsLayout();
            }
        }

        void InitContent()
        {
            SegmentedControl = new MDSegmentedControl(this);
            var image = new UIImage();
            SegmentedControl.SetBackgroundImage(image, UIControlState.Normal, UIBarMetrics.Default);
            SegmentedControl.SetDividerImage(image, UIControlState.Normal, UIControlState.Normal, UIBarMetrics.Default);
            if (UIDevice.CurrentDevice.CheckSystemVersion(13, 0))
            {
                SegmentedControl.SelectedSegmentTintColor = UIColor.Clear;
            }
            SegmentedControl.BackgroundColor = UIColor.Clear;
            ScrollView = new UIScrollView();
            ScrollView.ShowsHorizontalScrollIndicator = false;
            ScrollView.ShowsVerticalScrollIndicator = false;
            ScrollView.Bounces = false;
            ScrollView.AddSubview(SegmentedControl);
            AddSubview(ScrollView);
            HorizontalPaddingPerItem = Device.Idiom == TargetIdiom.Tablet ? 24 : 12;
            SegmentedControl.HorizontalPadding = HorizontalPaddingPerItem;
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            ScrollView.Frame = new CGRect(0, 0, Bounds.Size.Width, Bounds.Size.Height);
            ScrollView.ContentInset = new UIEdgeInsets(0, HorizontalInset, 0, HorizontalInset);
            ScrollView.ContentSize = SegmentedControl.Bounds.Size;
        }

        public override void WillMoveToSuperview(UIView newsuper)
        {
            base.WillMoveToSuperview(newsuper);
            frameObserver = SegmentedControl.AddObserver("frame", NSKeyValueObservingOptions.Initial | NSKeyValueObservingOptions.New, OnFrameChanged);
        }

        private void OnFrameChanged(NSObservedChange obj)
        {
            ScrollView.ContentSize = SegmentedControl.Bounds.Size;
        }

        public override void RemoveFromSuperview()
        {
            if (frameObserver != null) frameObserver.Dispose();
            base.RemoveFromSuperview();
        }

        void UpdateItemAppearance()
        {
            if (TextColor != null || TextFont != null)
            {
                SegmentedControl?.SetTextFont(TextFont, TextColor);
            }
        }

        void ScrollToSelectedIndex()
        {
            var frame = SegmentedControl.GetSelectedSegmentFrame();
            nfloat horizontalInset = HorizontalInset;
            var contentOffset = frame.Location.X + horizontalInset - (Frame.Size.Width - frame.Size.Width) / 2;
            if (contentOffset > ScrollView.ContentSize.Width + horizontalInset - Frame.Size.Width)
            {
                contentOffset = ScrollView.ContentSize.Width + horizontalInset - Frame.Size.Width;
            }
            else if (contentOffset < -horizontalInset)
            {
                contentOffset = -horizontalInset;
            }
            ScrollView.SetContentOffset(new CGPoint(contentOffset, 0), true);
            Debug.WriteLine($"==>contentOffset: {contentOffset}");
        }

        public void UpdateSelectedIndex(nint selectedIndex)
        {
            SelectedIndex = (nuint)selectedIndex;
            ScrollToSelectedIndex();
            if (WeakDelegate != null)
            {
                WeakDelegate.DidChangeSelectedIndex(this, (nuint)selectedIndex);
            }
        }

        void InsertItemAtIndex(NSObject item, nint index, bool animated)
        {
            if (item is NSString str)
            {
                SegmentedControl?.InsertSegment((string)str, index, animated);
            }
            else if (item is UIImage image)
            {
                SegmentedControl?.InsertSegment(image, index, animated);
            }
        }

        void RemoveItemAtIndex(nint index, bool animated)
        {
            SegmentedControl?.RemoveSegmentAtIndex(index, animated);
        }

        public void SetItems(NSObject[] items)
        {
            SegmentedControl?.RemoveAllSegments();
            nint index = 0;
            foreach (var item in items)
            {
                InsertItemAtIndex(item, index, false);
                index++;
            }
            SelectedIndex = 0;
        }

        public void ReplaceItem(NSObject item, nint index)
        {
            if (item is NSString str)
            {
                SegmentedControl?.SetTitle((string)str, index);
            }
            else if (item is UIImage image)
            {
                SegmentedControl?.SetImage(image, index);
            }
        }

        public void MoveIndicatorToFrame(CGRect frame, bool animated)
        {
            SegmentedControl?.MoveIndicatorToFrameWithAnimated(frame, animated);
        }
    }

    public interface IMDTabBarDelegate
    {
        void DidChangeSelectedIndex(MDTabBar tabBar, nuint selectedIndex);
    }
}
