using System;
using Xamarin.Forms;

namespace Naxam.Controls.Platform.iOS.Utils
{
	internal static class EffectUtilities
	{
		public static void RegisterEffectControlProvider(
            IEffectControlProvider self, 
            IElementController oldElement, 
            IElementController newElement)
		{
			IElementController controller = oldElement;
			if (controller != null && controller.EffectControlProvider == self)
				controller.EffectControlProvider = null;

			controller = newElement;
			if (controller != null)
				controller.EffectControlProvider = self;
		}
	}
}
