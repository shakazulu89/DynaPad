using System;
using System.Collections.Generic;
using System.Linq;
using CoreGraphics;
using Foundation;
using UIKit;

namespace DynaPad
{
	class FingerPaintCanvasView : UIView
	{
		// Two collections for storing polylines
		Dictionary<IntPtr, FingerPaintPolyline> inProgressPolylines = new Dictionary<IntPtr, FingerPaintPolyline>();
		List<FingerPaintPolyline> completedPolylines = new List<FingerPaintPolyline>();

		public FingerPaintCanvasView()
		{
			// filename = MREditName + "_" + "edit" + "_" + DateTime.Now.ToString("s").Replace(":", "_") + ".jpg";
//			var img = UIImage.FromFile("dynapadscreenshot.png");

//			//UIImage img = new UIImage();
//			//switch (MREditType)
//			//{
//			//	case "jpg":
//			//	case "gif":
//			//	case "png":
//			//		img = FromUrl(MREditPath);
//			//		break;
//			//	case "pdf":
//			//	case "doc":
//			//	case "docx":
//			//		var dps = new DynaPadService.DynaPadService();
//			//		//img = dps.ConvertToJPG(CommonFunctions.GetUserConfig(), MREditPath);
//			//		break;
//			//	default:
//			//		img = FromUrl(MREditPath);
//			//		break;
//			//}

//			var imgView = new UIImageView(Bounds);
//imgView.Image = img;
//			imgView.ContentMode = UIViewContentMode.ScaleAspectFit; // or ScaleAspectFill
//			AddSubview(imgView);
//			SendSubviewToBack(imgView);
			//BackgroundColor = UIColor.FromPatternImage(img);
			BackgroundColor = UIColor.Clear;
			//BackgroundColor = UIColor.FromWhiteAlpha(1.0f, 0.4f);
			Alpha = 1.0f;
			Opaque = false;
			//BackgroundColor = UIColor.White;
			MultipleTouchEnabled = true;
		}

		public CGColor StrokeColor { set; get; } = new CGColor(1.0f, 0, 0);
		//public CGColor StrokeColor { set; get; } = new CGColor(255, 0, 255, 0.2f);

		public float StrokeWidth { set; get; } = 1;

		public void Clear()
		{
			completedPolylines.Clear();
			SetNeedsDisplay();
		}

		public override void TouchesBegan(NSSet touches, UIEvent evt)
		{
			base.TouchesBegan(touches, evt);

			foreach (UITouch touch in touches.Cast<UITouch>())
			{
				// Create a FingerPaintPolyline, set the initial point, and store it
				FingerPaintPolyline polyline = new FingerPaintPolyline
				{
					Color = StrokeColor,
					StrokeWidth = StrokeWidth,
					StrokeAlpha = 0.3f
				};

				polyline.Path.MoveToPoint(touch.LocationInView(this));
				inProgressPolylines.Add(touch.Handle, polyline);
			}
			SetNeedsDisplay();
		}

		public override void TouchesMoved(NSSet touches, UIEvent evt)
		{
			base.TouchesMoved(touches, evt);

			foreach (UITouch touch in touches.Cast<UITouch>())
			{
				// Add point to path
				inProgressPolylines[touch.Handle].Path.AddLineToPoint(touch.LocationInView(this));
			}
			SetNeedsDisplay();
		}

		public override void TouchesEnded(NSSet touches, UIEvent evt)
		{
			base.TouchesEnded(touches, evt);

			foreach (UITouch touch in touches.Cast<UITouch>())
			{
				// Get polyline from dictionary and remove it from dictionary
				FingerPaintPolyline polyline = inProgressPolylines[touch.Handle];
				inProgressPolylines.Remove(touch.Handle);

				// Add final point to path and save with completed polylines
				polyline.Path.AddLineToPoint(touch.LocationInView(this));
				completedPolylines.Add(polyline);
			}
			SetNeedsDisplay();
		}

		public override void TouchesCancelled(NSSet touches, UIEvent evt)
		{
			base.TouchesCancelled(touches, evt);

			foreach (UITouch touch in touches.Cast<UITouch>())
			{
				inProgressPolylines.Remove(touch.Handle);
			}
			SetNeedsDisplay();
		}

		public override void Draw(CGRect rect)
		{
			base.Draw(rect);

			using (CGContext context = UIGraphics.GetCurrentContext())
			{
				// Stroke settings
				context.SetLineCap(CGLineCap.Round);
				context.SetLineJoin(CGLineJoin.Round);

				// Draw the completed polylines
				foreach (FingerPaintPolyline polyline in completedPolylines)
				{
					context.SetStrokeColor(polyline.Color);
					context.SetLineWidth(polyline.StrokeWidth);
					context.AddPath(polyline.Path);
					context.DrawPath(CGPathDrawingMode.Stroke);
				}

				// Draw the in-progress polylines
				foreach (FingerPaintPolyline polyline in inProgressPolylines.Values)
				{
					context.SetStrokeColor(polyline.Color);
					context.SetLineWidth(polyline.StrokeWidth);
					context.AddPath(polyline.Path);
					context.DrawPath(CGPathDrawingMode.Stroke);
				}
			}
		}



		public UIImage AsImage(UIView view)
		{
			UIGraphics.BeginImageContextWithOptions(view.Bounds.Size, view.Opaque, 1);
			view.DrawViewHierarchy(view.Frame, true); //this was key line
			UIImage img = UIGraphics.GetImageFromCurrentImageContext();
			UIGraphics.EndImageContext();

			return img;
		}

		static UIImage FromUrl(string uri)
		{
			using (var url = new NSUrl(uri))
			using (var data = NSData.FromUrl(url))
			return UIImage.LoadFromData(data);
		}
	}
}
