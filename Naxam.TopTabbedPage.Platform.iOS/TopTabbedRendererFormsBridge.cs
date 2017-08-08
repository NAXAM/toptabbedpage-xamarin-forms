using System;
using System.Collections.Specialized;
using System.Linq;
using Naxam.Controls.Platform.iOS.Utils;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

namespace Naxam.Controls.Platform.iOS
{
    public partial class TopTabbedRenderer : IVisualElementRenderer, IEffectControlProvider
    {
        public event EventHandler<VisualElementChangedEventArgs> ElementChanged;

        public VisualElement Element { get; private set; }

        public UIView NativeView => View;

        public UIViewController ViewController => this;

        public SizeRequest GetDesiredSize(double widthConstraint, double heightConstraint)
        {
            return NativeView.GetSizeRequest(widthConstraint, heightConstraint);
        }

        void IEffectControlProvider.RegisterEffect(Effect effect)
        {
            VisualElementRenderer<VisualElement>.RegisterEffect(effect, View);
        }

        public void SetElement(VisualElement element)
        {
            var oldElement = Element;
            Element = element;

            Tabbed.PropertyChanged += OnPropertyChanged;
            Tabbed.PagesChanged += OnPagesChanged;

            OnElementChanged(new VisualElementChangedEventArgs(oldElement, element));

            OnPagesChanged(null, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

            if (element != null)
            {
                element.SendViewInitialized(NativeView);
            }

            UpdateBarBackgroundColor();
            UpdateBarTextColor();
            UpdateBarIndicatorColor();

            EffectUtilities.RegisterEffectControlProvider(this, oldElement, element);
        }

        public void SetElementSize(Size size)
        {
            if (_loaded)
                Element.Layout(new Rectangle(Element.X, Element.Y, size.Width, size.Height));
            else
                _queuedSize = size;
        }
    }

    static class ElementExtensions
    {
        public static void SendViewInitialized(this VisualElement element, UIView nativeView)
        {
            var method = typeof(Xamarin.Forms.Forms).GetMethods(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)
                                                    .FirstOrDefault(x => x.Name == nameof(SendViewInitialized));

            method.Invoke(null, new object[] { element, nativeView });
        }
    }
}