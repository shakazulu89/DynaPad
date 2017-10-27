using System;
using CoreGraphics;

namespace DynaPad
{
	class FingerPaintPolyline
	{
		public FingerPaintPolyline()
		{
			Path = new CGPath();
		}

		public CGColor Color { set; get; }

		public float StrokeAlpha { get; set; }

		public float StrokeWidth { set; get; }

		public CGPath Path { private set; get; }
	}
}
