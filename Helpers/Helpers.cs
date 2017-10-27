using UIKit;
using Foundation;

namespace DynaPad
{
	public static class Helpers
	{
		public static NSNumber [] TouchTypes (UITouchType type)
		{
			return new NSNumber [] { new NSNumber ((long)type) };
		}
	}
}
