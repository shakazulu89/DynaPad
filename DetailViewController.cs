using System;
using System.Collections.Generic;
using MonoTouch.Dialog;
using Newtonsoft.Json;
using UIKit;
using Foundation;
using System.Drawing;
using CoreGraphics;
using AVFoundation;
using System.Diagnostics;
using System.IO;
using Xamarin.Media;
using System.Threading.Tasks;
using System.ComponentModel;
using Syncfusion.SfAutoComplete.iOS;
using Syncfusion.SfDataGrid;
//using Syncfusion.SfImageEditor.iOS;
//using Syncfusion.SfPdfViewer.iOS;
using Plugin.Connectivity;
using System.Windows.Input;
using System.Xml;
using System.Net;
using System.Collections.ObjectModel;
//using DynaClassLibrary;
//using static DynaClassLibrary.DynaClasses;
//using MessageUI;
using SafariServices;
using WebKit;
//using PdfKit;

namespace DynaPad
{
	public partial class DetailViewController : DialogViewController
	{
		public Section DetailItem { get; set; }
		public DynaMultiRootElement QuestionsView { get; set; }
		public DialogViewController mvc { get; set; }
		UILabel messageLabel;
		LoadingOverlay loadingOverlay;
		AVAudioSession session;
		AVAudioRecorder recorder;
		AVAudioPlayer player;
		Stopwatch stopwatch;
		NSUrl audioFilePath;
		NSObject observer;
		UILabel RecordingStatusLabel = new UILabel();
		UILabel LengthOfRecordingLabel = new UILabel();
		UILabel PlayRecordedSoundStatusLabel = new UILabel();
		UIButton StartRecordingButton = new UIButton();
		UIButton StopRecordingButton = new UIButton();
		UIButton PlayRecordedSoundButton = new UIButton();
		UIButton SaveRecordedSound = new UIButton();
		UIButton CancelRecording = new UIButton();
		UITableViewCell cellRecord = new UITableViewCell(UITableViewCellStyle.Default, null);
		UITableViewCell cellStop = new UITableViewCell(UITableViewCellStyle.Default, null);
		UITableViewCell cellPlay = new UITableViewCell(UITableViewCellStyle.Default, null);
		UITableViewCell cellSave = new UITableViewCell(UITableViewCellStyle.Default, null);
		UIButton PlaySavedDictationButton = new UIButton();
		UIButton DeleteSavedDictationButton = new UIButton();
		UITableViewCell cellDict = new UITableViewCell(UITableViewCellStyle.Default, null);
		UIPopoverController pop;

		public Menu DynaMenu { get; set; }

		protected DetailViewController(IntPtr handle) : base(handle)
		{
			// Note: this .ctor should not contain any initialization logic.
			//this.TableView.CellLayoutMarginsFollowReadableWidth = false;
		}


		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			Root.Caption = "Welcome to Dynapad";
			Root.Add(new Section("Login to the app"));

			base.TableView.CellLayoutMarginsFollowReadableWidth = false;
			// Perform any additional setup after loading the view, typically from a nib.
			base.TableView.ScrollsToTop = true;

			ModalInPopover = true;

			var tap = new UITapGestureRecognizer();
			tap.AddTarget(() =>
			{
				if (!base.View.IsFirstResponder)
				{
					base.View.EndEditing(true);
				}
			});
			base.View.AddGestureRecognizer(tap);
			tap.CancelsTouchesInView = false;



			////var drawNavBtn = GetDrawNavBtn("1");
			//var ass = new UIBarButtonItem("edit", UIBarButtonItemStyle.Plain ,delegate
			//{
			//	//var dcanvas = new CanvasMainViewController { MREditing = false };
			//	//var dcanvas = new FingerPaintViewController(); 
			//	//var dcanvas = new FingerPaintViewController() { MREditing = true, MREditPath = "https://amato.dynadox.pro/data/amato.dynadox.pro/claimantfiles/18/130.gif", MREditId = "130", MREditName = "John_Doe_True_sig_2017-05-09T10_59_08.gif", apptId = "41", patientId = "18", doctorId = "14", locationId = "12", IsDoctorForm = true };
			//	//var dcanvas = new FingerPaintViewController() { MREditing = true, MREditPath = "https://amato.dynadox.pro/data/testpng.png", MREditId = "130", MREditName = "testpng.png", apptId = "41", patientId = "18", doctorId = "14", locationId = "12", IsDoctorForm = true };
			//	var dcanvas = new FingerPaintViewController() { MREditing = true, MREditPath = "https://amato.dynadox.pro/data/testjpg.jpg", MREditId = "130", MREditName = "testjpg.jpg", apptId = "41", patientId = "18", doctorId = "14", locationId = "12", IsDoctorForm = true };
			//	//PreferredContentSize = new CGSize(View.Bounds.Size);
			//	//PresentViewController(dcanvas, true, null);

			//	var asss = new Section();
			//             asss.Add(dcanvas.View);
			//             var rr = new RootElement("edit");
			//             rr.Add(asss);
			//             var ndia = new DialogViewController(rr);
			//	ndia.ModalInPopover = true;
			//             ndia.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
			//	ndia.PreferredContentSize = new CGSize(View.Bounds.Size); 
			//	PreferredContentSize = new CGSize(View.Bounds.Size);
			//             PresentViewController(dcanvas, true, null);

			// //            Root.Clear();
			//	//var asss = new Section();
			//	//asss.Add(dcanvas.View);
			// //            var rr = new RootElement("ass");
			// //            rr.Add(asss);
			//	//Root = rr;
			//});
			//NavigationItem.SetRightBarButtonItem(ass, true);





			//var ass = new UIBarButtonItem("edit", UIBarButtonItemStyle.Plain ,delegate
			//{
			//	PreferredContentSize = new CGSize(View.Bounds.Size);
			//	SfImageEditor imageEditor = new SfImageEditor();
			//             //imageEditor.Frame = new CGRect(0, 0, 500, 500);
			//             imageEditor.Frame = View.Frame;
			//	var downloadPath = Path.Combine(Path.GetTempPath(), "testjpg.jpg");
			//	var url = "https://amato.dynadox.pro/data/testjpg.jpg";
			//	var webClient = new WebClient();
			//	webClient.DownloadFile(url, downloadPath);
			//             UIImage img = UIImage.LoadFromData(UIImage.FromFile(downloadPath).AsJPEG(), 1);
			//             imageEditor.Image = img;
			//             var sec = new Section();
			//             sec.Add(imageEditor);
			//             var rr = new RootElement("edit");
			//             rr.Add(sec);
			//             var dvc = new DialogViewController(rr);
			//             var vvv = new UIViewController();
			//             vvv.View = imageEditor;
			//             //dvc.ModalPresentationStyle = UIModalPresentationStyle.Popover;
			//             //dvc.PreferredContentSize = new CGSize(View.Bounds.Size);
			//             //View.AddSubview(imageEditor);
			//             PresentViewController(vvv, true, null);
			//});
			//NavigationItem.SetRightBarButtonItem(ass, true);

   //         string URL = "http://www.adobe.com/content/dam/Adobe/en/devnet/acrobat/pdfs/pdf_reference_1-7.pdf";
			//var pdfViewerControl = new SfPdfViewer();
			//using (MemoryStream memoryStream = new MemoryStream())
			//{
			//	ConvertToStream(URL, memoryStream);
			//	memoryStream.Seek(0, SeekOrigin.Begin);
			//	pdfViewerControl.LoadDocument(memoryStream);
			//}
			//pdfViewerControl.Frame = new CGRect(0, 0, View.Bounds.Width, View.Bounds.Height);
            //base.View.AddSubview(pdfViewerControl);
            ////var s = new Section();
            ////s.Add(pdfViewerControl);
            ////var ff = new RootElement("pdf");
            ////ff.Add(s);
            ////Root.Add(s);
		}


        async void SubmitForm(string password, bool isDoctorForm, Xamarin.Controls.SignaturePadView sig)
		{
			try
			{
                //var bounds = base.TableView.Frame;
                loadingOverlay = new LoadingOverlay(SplitViewController.View.Bounds);// { loadingLabelText = "Submitting Form..."};
                loadingOverlay.SetText("Submitting a form may take a few minutes. Please wait patiently...");
				//mvc = (DialogViewController)((UINavigationController)SplitViewController.ViewControllers[1]).TopViewController;
				//mvc.Add(loadingOverlay);
                SplitViewController.Add(loadingOverlay);

                await Task.Delay(10);

				//bool isValid = password == Constants.Password;
				bool isValid = false;
				bool isSigned = !sig.IsBlank;

				//for (int i = 0; i < Constants.Logins.GetLength(0); i++)
				//{
				//	if (SelectedAppointment.ApptLocationId == Constants.Logins[i, 2])
				//	{
				//		isValid |= password == Constants.Logins[i, 1];
				//	}
				//}

				if (SelectedAppointment.ApptLocationId == DynaClassLibrary.DynaClasses.LoginContainer.User.SelectedLocation.LocationId)
				{
                    if (isDoctorForm)
                    {
                        isValid = true;
                    }
                    else
                    {
                        isValid |= password == DynaClassLibrary.DynaClasses.LoginContainer.User.DynaPassword;
                    }
				}

				if (CrossConnectivity.Current.IsConnected)
				{
					if (isValid && isSigned)
					{
						var finalJson = JsonConvert.SerializeObject(SelectedAppointment.SelectedQForm);

                        var dds = new DynaPadService.DynaPadService() { Timeout = 60000 };

						dds.SubmitFormAnswers(CommonFunctions.GetUserConfig(), finalJson, true, isDoctorForm);

						var filename = SelectedAppointment.ApptPatientName.Replace(" ", "_") + "_" + isDoctorForm + "_sig_" + DateTime.Now.ToString("s").Replace(":", "_") + ".gif";
						var file = sig.GetImage(new CGSize(600, 400), true, true).AsPNG().ToArray();

						var fileid = dds.SaveFile(CommonFunctions.GetUserConfig(), SelectedAppointment.ApptId, SelectedAppointment.ApptPatientId, SelectedAppointment.ApptDoctorId, SelectedAppointment.ApptLocationId, filename, "Signature", "DynaPad", "", "", file, isDoctorForm, true);

                        if (fileid.StartsWith("error", StringComparison.CurrentCulture))
                        {
                            CommonFunctions.sendErrorEmail(new Exception("dds.SaveFile error: apptid = " + SelectedAppointment.ApptId));
                        }

                        loadingOverlay.Hide();
						
                        SetDetailItem(new Section("Summary"), "Summary", "", null, false);
					}
					else
                    {
                        loadingOverlay.Hide();

						messageLabel.Text = "Submit failed";
						var failPass = "";
						var failSign = "";
						if (!isValid) failPass = "Wrong password. ";
						if (!isSigned) failSign = "Form was not signed!";

						PresentViewController(CommonFunctions.AlertPrompt("Error", failPass + failSign, true, null, false, null), true, null);
					}
				}
				else
                {
                    loadingOverlay.Hide();

					PresentViewController(CommonFunctions.InternetAlertPrompt(), true, null);
				}
			}
			catch (Exception ex)
			{
                loadingOverlay.Hide();
				CommonFunctions.sendErrorEmail(ex);
                PresentViewController(CommonFunctions.ExceptionAlertPrompt(), true, null);
				//throw new Exception(ex.Message + Environment.NewLine + ex.StackTrace, ex.InnerException);
			}
		}

        void Print(string jobname, UIViewPrintFormatter printFormatter)//, UIWebView webView)
		{
			try
			{
				var printInfo = UIPrintInfo.PrintInfo;
				printInfo.OutputType = UIPrintInfoOutputType.General;
				printInfo.JobName = jobname;

				var printer = UIPrintInteractionController.SharedPrintController;
				printer.PrintInfo = printInfo;
                //printer.PrintFormatter = webView.ViewPrintFormatter;
                printer.PrintFormatter = printFormatter;
				//printer.ShowsPageRange = true;
				printer.Present(true, (handler, completed, err) =>
				{
					if (!completed && err != null)
					{
						System.Console.WriteLine("error");
					}
				});
			}
			catch (Exception ex)
			{
				CommonFunctions.sendErrorEmail(ex);
                PresentViewController(CommonFunctions.ExceptionAlertPrompt(), true, null);
				//throw new Exception(ex.Message + Environment.NewLine + ex.StackTrace, ex.InnerException);
			}
		}


        //		void DataGridForm_GridDoubleTapped(object sender, GridDoubleTappedEventsArgs e)
        //		{
        //			try
        //			{
        //				//if (e.RowData.GetType() == typeof(Syncfusion.Data.Group))
        //				if (e.RowData.GetType() == typeof(MR))
        //				{
        //					var rowIndex = e.RowColumnindex.RowIndex;
        //					var rowData = (MR)e.RowData;
        //					var columnIndex = e.RowColumnindex.ColumnIndex;
        //					var filepath = rowData.MRPath;

        //					if (filepath.StartsWith("Error:", StringComparison.CurrentCulture))
        //					{
        //                        DismissViewController(true, null);
        //						PresentViewController(CommonFunctions.AlertPrompt("File Error", "File unavailable, contact administration", true, null, false, null), true, null);
        //						return;
        //					}

        //					var webViews = new UIWebView(View.Bounds);
        //					webViews.Frame = new CGRect(0, 0, View.Bounds.Width, View.Bounds.Height);
        //					//webViews.LoadRequest(new NSUrlRequest(new NSUrl("https://test.dynadox.pro/dynawcfservice/Summaries/3_10_29_patient.pdf")));
        //					//var filepath = rowData.MRPath.Replace(@"\", "/");
        //					//filepath = filepath.Replace("C:/inetpub/wwwroot/dynadox/", "https://test.dynadox.pro/");
        //					webViews.LoadRequest(new NSUrlRequest(new NSUrl(filepath)));
        //					webViews.ScalesPageToFit = true;


        //					var nlab = new UILabel(new CGRect(10, 10, View.Bounds.Width - 110, 50));
        //					nlab.Text = rowData.MRName;

        //					var ncellHeader = new UITableViewCell(UITableViewCellStyle.Default, null);
        //					ncellHeader.Frame = new CGRect(0, 0, View.Bounds.Width, 50);

        //					var nheadeditbtn = new UIButton(new CGRect(View.Bounds.Width - 100, 10, 50, 50));
        //					nheadeditbtn.SetImage(UIImage.FromBundle("Writing"), UIControlState.Normal);

        //					var nheadclosebtn = new UIButton(new CGRect(View.Bounds.Width - 50, 10, 50, 50));
        //					nheadclosebtn.SetImage(UIImage.FromBundle("Close"), UIControlState.Normal);

        //					ncellHeader.ContentView.Add(nlab);
        //					ncellHeader.ContentView.Add(nheadeditbtn);
        //					ncellHeader.ContentView.Add(nheadclosebtn);

        //					var nsec = new Section(ncellHeader);
        //					nsec.FooterView = new UIView(new CGRect(0, 0, 0, 0));
        //					nsec.FooterView.Hidden = true;

        //					//var dcanvas = new CanvasMainViewController();
        //					nsec.Add(webViews);

        //					var nroo = new RootElement("File");
        //					nroo.Add(nsec);

        //					var ndia = new DialogViewController(nroo);
        //					ndia.ModalInPopover = true;
        //					ndia.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
        //					ndia.PreferredContentSize = new CGSize(View.Bounds.Size);
        //					//ndia.ProvidesPresentationContextTransitionStyle = true;
        //					//ndia.DefinesPresentationContext = true;
        //					//ndia.ModalPresentationStyle = UIModalPresentationStyle.OverCurrentContext;

        //					nheadclosebtn.TouchUpInside += delegate
        //					{
        //						DismissViewController(true, null);
        //						//NavigationController.PopViewController(true);
        //					};

        //					nheadeditbtn.TouchUpInside += delegate
        //					{
        //						DismissViewController(true, null);
        //var dcanvas = new CanvasMainViewController { MREditing = true, MREditPath = rowData.MRPath, MREditId = rowData.MRId, apptId = SelectedAppointment.ApptId, patientId = SelectedAppointment.ApptPatientId, doctorId = SelectedAppointment.ApptDoctorId, locationId = SelectedAppointment.ApptLocationId, IsDoctorForm = true };
        //						PreferredContentSize = new CGSize(View.Bounds.Size);
        //						//NavigationController.View.BackgroundColor = UIColor.Clear;
        //						PresentViewController(dcanvas, true, null);
        //						//NavigationController.View.SizeToFit();
        //					};

        //					PreferredContentSize = new CGSize(View.Bounds.Size);
        //					//NavigationController.PushViewController(ndia, true);
        //					DismissViewController(true, null);
        //					PresentViewController(ndia, true, null);
        //					//View.SizeToFit();
        //				}
        //			}
        //			catch (Exception ex)
        //			{
        //				throw new Exception(ex.Message + Environment.NewLine + ex.StackTrace, ex.InnerException);
        //			}
        //		}

        //SfPdfViewer pdfViewerControl;
        //MFMailComposeViewController mailController;
        //WKWebView MRWebView;
        //UIProgressView progressView;
		//void DataGrid_GridDoubleTapped(object sender, GridDoubleTappedEventsArgs e)
		//{
		//	try
		//	{
  //              if (e.RowData.GetType() == typeof(MR))
  //              {
  //                  //var bounds = base.TableView.Frame;
  //                  //loadingOverlay = new LoadingOverlay(bounds);
  //                  //mvc = (DialogViewController)((UINavigationController)SplitViewController.ViewControllers[1]).TopViewController;
  //                  //mvc.Add(loadingOverlay);

  //                  var rowIndex = e.RowColumnindex.RowIndex;
  //                  var rowData = (MR)e.RowData;
  //                  var columnIndex = e.RowColumnindex.ColumnIndex;
  //                  var filepath = rowData.MRPath;
  //                  var filetype = rowData.MRFileType;

  //                  if (filepath.StartsWith("Error:", StringComparison.CurrentCulture))
  //                  {
  //                      PresentViewController(CommonFunctions.AlertPrompt("File Error", "File unavailable, contact administration", true, null, false, null), true, null);
  //                      return;
  //                  }

  //                  var nlab = new UILabel(new CGRect(10, 10, View.Bounds.Width - 110, 50))
  //                  {
  //                      Text = rowData.MRName
  //                  };

		//			var ncellHeader = new UITableViewCell(UITableViewCellStyle.Default, null)
  //                  {
  //                      Frame = new CGRect(0, 0, View.Bounds.Width, 50)
  //                  };

		//			var nheadeditbtn = new UIButton(new CGRect(View.Bounds.Width - 200, 10, 50, 50));
		//			nheadeditbtn.SetImage(UIImage.FromBundle("Writing"), UIControlState.Normal);
		//			var nheadclosebtn = new UIButton(new CGRect(View.Bounds.Width - 50, 10, 50, 50));
		//			nheadclosebtn.SetImage(UIImage.FromBundle("Close"), UIControlState.Normal);
		//			var nheadprintbtn = new UIButton(new CGRect(View.Bounds.Width - 100, 10, 50, 50));
		//			nheadprintbtn.SetImage(UIImage.FromBundle("Print"), UIControlState.Normal);
		//			var nheadsharebtn = new UIButton(new CGRect(View.Bounds.Width - 150, 10, 50, 50));
		//			nheadsharebtn.SetImage(UIImage.FromBundle("Email"), UIControlState.Normal);

		//			//ncellHeader.ContentView.Add(nlab);
		//			//ncellHeader.ContentView.Add(nheadeditbtn);
		//			ncellHeader.ContentView.Add(nheadclosebtn);
		//			ncellHeader.ContentView.Add(nheadprintbtn);
  //                  ncellHeader.ContentView.Add(nheadsharebtn);
  //                  if (filetype == "jpg" || filetype == "jpeg" || filetype == "png" || filetype == "gif")
  //                  {
  //                      ncellHeader.ContentView.Add(nheadeditbtn);
  //                  }

  //                  var nsec = new Section(ncellHeader);
		//			nsec.FooterView = new UIView(new CGRect(0, 0, 0, 0));
		//			nsec.FooterView.Hidden = true;
  //                  //nsec.HeaderView = new UIView(new CGRect(0, 0, 0, 0));
  //                  //nsec.HeaderView.Hidden = true;

  //                  //UIBarButtonItem printNavBtn;

  //                  //               switch (filetype)
  //                  //               {
  //                  //  //                 case "pdf":
  //                  //  //                     nheadeditbtn.Enabled = false;
  //                  //		//			var pdfViewerControl = new SfPdfViewer();
  //                  //		//			using (MemoryStream mem = new MemoryStream())
  //                  //		//			{
  //                  //		//				ConvertToStream(filepath, mem);
  //                  //		//				mem.Seek(0, SeekOrigin.Begin);
  //                  //		//				pdfViewerControl.LoadDocument(mem);
  //                  //		//			}

  //                  //  //                     //pdfViewerControl.LoadDocument(DownloadPdfStream(filepath, rowData.MRName));

  //                  //  //                     pdfViewerControl.Frame = new CGRect(0, 0, View.Bounds.Width, View.Bounds.Height);
  //                  //		//nsec.Add(pdfViewerControl);

  //                  //                       //break;
  //                  //                   case "jpg":
  //                  //                   case "jpeg":
  //                  //                   case "png":
  //                  //                   case "gif":
  //                  //                       ncellHeader.ContentView.Add(nheadeditbtn);

  //                  //		bool didStart = false;
  //                  //		bool didFinish = false;
  //                  //		var webViews = new UIWebView(View.Bounds);
  //                  //		webViews.LoadStarted += (object lssender, EventArgs lse) => {
  //                  //			if (didStart == false)
  //                  //			{
  //                  //				loadingOverlay = new LoadingOverlay(webViews.Bounds);
  //                  //				webViews.Add(loadingOverlay);
  //                  //				UIApplication.SharedApplication.NetworkActivityIndicatorVisible = true;
  //                  //				didStart = true;
  //                  //				didFinish = false;
  //                  //			}
  //                  //		};

  //                  //		//When the web view is finished loading
  //                  //		webViews.LoadFinished += (object lfsender, EventArgs lfe) => {
  //                  //			if (didFinish == false)
  //                  //			{
  //                  //				loadingOverlay.Hide();
  //                  //				UIApplication.SharedApplication.NetworkActivityIndicatorVisible = false;
  //                  //				didStart = false;
  //                  //				didFinish = true;
  //                  //			}
  //                  //		};

  //                  //		//If there is a load error
  //                  //		webViews.LoadError += (object lesender, UIWebErrorArgs lee) => {
  //                  //			if (didFinish == false)
  //                  //			{
  //                  //				loadingOverlay.Hide();
  //                  //				UIApplication.SharedApplication.NetworkActivityIndicatorVisible = false;
  //                  //				didStart = false;
  //                  //				didFinish = true;
  //                  //			}
  //                  //		};
  //                  //		webViews.Frame = new CGRect(View.Bounds.X, 0, View.Bounds.Width, View.Bounds.Height);
  //                  //		webViews.LoadRequest(new NSUrlRequest(new NSUrl(filepath)));
  //                  //		webViews.ScalesPageToFit = true;

  //                  //		var nheadprintbtn = new UIButton(new CGRect(View.Bounds.Width - 150, 10, 50, 50));
  //                  //		nheadprintbtn.SetImage(UIImage.FromBundle("Print"), UIControlState.Normal);
  //                  //		nheadprintbtn.TouchUpInside += delegate {
  //                  //                           Print(rowData.MRName, webViews.ViewPrintFormatter);
  //                  //		};
  //                  //                       ncellHeader.ContentView.Add(nheadprintbtn);

  //                  //		printNavBtn = new UIBarButtonItem(UIImage.FromBundle("Print"), UIBarButtonItemStyle.Plain, delegate
  //                  //		{ Print(rowData.MRName, webViews.ViewPrintFormatter); });

  //                  //		nsec.Add(webViews);
  //                  //		break;
  //                  //                   default:
  //                  //		bool defaultDidStart = false;
  //                  //		bool defaultDidFinish = false;
  //                  //		var defaultWebViews = new UIWebView(View.Bounds);
  //                  //		defaultWebViews.LoadStarted += (object lssender, EventArgs lse) => {
  //                  //			if (defaultDidStart == false)
  //                  //			{
  //                  //				loadingOverlay = new LoadingOverlay(defaultWebViews.Bounds);
  //                  //				defaultWebViews.Add(loadingOverlay);
  //                  //				UIApplication.SharedApplication.NetworkActivityIndicatorVisible = true;
  //                  //				defaultDidStart = true;
  //                  //				defaultDidFinish = false;
  //                  //			}
  //                  //		};

  //                  //		//When the web view is finished loading
  //                  //		defaultWebViews.LoadFinished += (object lfsender, EventArgs lfe) => {
  //                  //			if (defaultDidFinish == false)
  //                  //			{
  //                  //				loadingOverlay.Hide();
  //                  //				UIApplication.SharedApplication.NetworkActivityIndicatorVisible = false;
  //                  //				defaultDidStart = false;
  //                  //				defaultDidFinish = true;
  //                  //			}
  //                  //		};

  //                  //		//If there is a load error
  //                  //		defaultWebViews.LoadError += (object lesender, UIWebErrorArgs lee) => {
  //                  //			if (defaultDidFinish == false)
  //                  //			{
  //                  //				loadingOverlay.Hide();
  //                  //				UIApplication.SharedApplication.NetworkActivityIndicatorVisible = false;
  //                  //				defaultDidStart = false;
  //                  //				defaultDidFinish = true;
  //                  //			}
  //                  //		};
  //                  //                       defaultWebViews.Frame = new CGRect(View.Bounds.X, 0, View.Bounds.Width, View.Bounds.Height);
  //                  //		//webViews.LoadRequest(new NSUrlRequest(new NSUrl("https://test.dynadox.pro/dynawcfservice/Summaries/3_10_29_patient.pdf")));
  //                  //		//var filepath = rowData.MRPath.Replace(@"\", "/");
  //                  //		//filepath = filepath.Replace("C:/inetpub/wwwroot/dynadox/", "https://test.dynadox.pro/");
  //                  //		defaultWebViews.LoadRequest(new NSUrlRequest(new NSUrl(filepath)));
  //                  //		defaultWebViews.ScalesPageToFit = true;

  //                  //		var dheadprintbtn = new UIButton(new CGRect(View.Bounds.Width - 100, 10, 50, 50));
  //                  //		dheadprintbtn.SetImage(UIImage.FromBundle("Print"), UIControlState.Normal);
  //                  //		dheadprintbtn.TouchUpInside += delegate {
  //                  //			Print(rowData.MRName, defaultWebViews.ViewPrintFormatter);
  //                  //		};
  //                  //		ncellHeader.ContentView.Add(dheadprintbtn);


  //                  //                       printNavBtn = new UIBarButtonItem(UIImage.FromBundle("Print"), UIBarButtonItemStyle.Plain, delegate
  //                  //                               { Print(rowData.MRName, defaultWebViews.ViewPrintFormatter); });

  //                  //		nsec.Add(defaultWebViews);
  //                  //                       break;
  //                  //}

  //                  //var dcanvas = new CanvasMainViewController();



  //                  //bool didStart = false;
  //                  //bool didFinish = false;
  //                  //var webViews = new UIWebView(View.Bounds);
  //                  //webViews.LoadStarted += (object lssender, EventArgs lse) => {
  //                  //	if (didStart == false)
  //                  //	{
  //                  //		loadingOverlay = new LoadingOverlay(webViews.Bounds);
  //                  //		webViews.Add(loadingOverlay);
  //                  //		UIApplication.SharedApplication.NetworkActivityIndicatorVisible = true;
  //                  //		didStart = true;
  //                  //		didFinish = false;
  //                  //	}
  //                  //};

  //                  ////When the web view is finished loading
  //                  //webViews.LoadFinished += (object lfsender, EventArgs lfe) => {
  //                  //	if (didFinish == false)
  //                  //	{
  //                  //		loadingOverlay.Hide();
  //                  //		UIApplication.SharedApplication.NetworkActivityIndicatorVisible = false;
  //                  //		didStart = false;
  //                  //		didFinish = true;
  //                  //	}
  //                  //};

  //                  ////If there is a load error
  //                  //webViews.LoadError += (object lesender, UIWebErrorArgs lee) => {
  //                  //	if (didFinish == false)
  //                  //	{
  //                  //		loadingOverlay.Hide();
  //                  //		UIApplication.SharedApplication.NetworkActivityIndicatorVisible = false;
  //                  //		didStart = false;
  //                  //		didFinish = true;
  //                  //	}
  //                  //};
  //                  //webViews.Frame = new CGRect(View.Bounds.X, 0, View.Bounds.Width, View.Bounds.Height);
  //                  //webViews.LoadRequest(new NSUrlRequest(new NSUrl(filepath)));
  //                  //webViews.ScalesPageToFit = true;

  //                  //"https://test.dynadox.pro/data/test.dynadox.pro/claimantfiles/37/361.pdf"
  //                  //var wkurl = new NSUrl("https://test.dynadox.pro/data/test.dynadox.pro/claimantfiles/darwinmeds.pdf");
  //                  //var wkurl = new NSUrl(rowData.MRPath.Replace("https", "http"));//"https://test.dynadox.pro/data/test.dynadox.pro/claimantfiles/darwinmeds.pdf"
  //                  var wkurl = new NSUrl(rowData.MRPath);
		//			if (rowData.MRFileType == "External")
  //                  {
		//				var sfVC = new SFSafariViewController(wkurl);
		//				PresentViewController(sfVC, true, null);
  //                      return;
		//			}

  //                  var wkrequest = new NSUrlRequest(wkurl);
  //                  MRWebView = new WKWebView(View.Bounds, new WKWebViewConfiguration() { SuppressesIncrementalRendering = false });
  //                  MRWebView.Frame = new CGRect(View.Bounds.X, 0, View.Bounds.Width, View.Bounds.Height);
  //                  progressView = new UIProgressView(UIProgressViewStyle.Default);
  //                  progressView.Frame = new CGRect(0, 0, MRWebView.Frame.Width, 50);

  //                  //MRWebView.NavigationDelegate = self;
  //                  MRWebView.AddObserver("estimatedProgress", NSKeyValueObservingOptions.New, ProgressObserver);
  //                  MRWebView.AddSubview(progressView);
  //                  MRWebView.LoadRequest(wkrequest);



  //                  //var sfViewController = new SFSafariViewController(wkurl);
  //                  //PresentViewController(sfViewController, true, null);



  //                  nsec.Add(MRWebView);

  //                  var nroo = new DynaMultiRootElement(rowData.MRName);
  //                  nroo.Add(nsec);

  //                  var ndia = new DialogViewController(nroo);

		//			nheadclosebtn.TouchUpInside += delegate
		//			{
  //                      NavigationController.PopViewController(true);
		//			};

		//			nheadprintbtn.TouchUpInside += delegate {
  //                      Print(rowData.MRName, MRWebView.ViewPrintFormatter);
		//			};

  //                  string mimetype;
  //                  switch (filetype)
  //                  {
  //                      case "jpg":
  //                      case "jpeg":
  //                          mimetype = "image/jpeg";
  //                          break;
  //                      case "png":
  //                          mimetype = "image/png";
		//					break;
		//				case "gif":
		//					mimetype = "image/gif";
		//					break;
		//				case "doc":
		//					mimetype = "application/msword";
		//					break;
		//				case "docm":
		//					mimetype = "application/vnd.ms-word.document.macroEnabled.12";
		//					break;
		//				case "docx":
		//					mimetype = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
		//					break;
		//				case "pdf":
		//					mimetype = "application/pdf";
		//					break;
  //                      default:
  //                          mimetype = "application/unknown";
  //                          break;
  //                  }

  //                  nheadsharebtn.TouchUpInside += delegate {
		//				if (MFMailComposeViewController.CanSendMail)
		//				{

		//					//var to = new string[] { "john@doe.com" };

		//					if (MFMailComposeViewController.CanSendMail)
		//					{
		//						mailController = new MFMailComposeViewController();
  //                              //mailController.SetToRecipients(to);
		//						mailController.SetSubject("Dynapad MR Attachment - " + rowData.MRName);
		//						//mailController.SetMessageBody("this is a test", false);
  //                              var attachmentfilename = rowData.MRName + "." + rowData.MRFileType;
		//						var attachmentDownloadPath = Path.Combine(Path.GetTempPath(), attachmentfilename);
		//						var attachmenturl = rowData.MRPath;
		//						var attachmentWebClient = new WebClient();
		//						attachmentWebClient.DownloadFile(attachmenturl, attachmentDownloadPath);
  //                              //NSData adata = new NSData();
  //                              mailController.AddAttachmentData(NSData.FromFile(attachmentDownloadPath), mimetype, attachmentfilename);
		//						mailController.Finished += (object s, MFComposeResultEventArgs args) => {

		//							//Console.WriteLine("result: " + args.Result.ToString()); // sent or cancelled

		//							BeginInvokeOnMainThread(() => {
		//								args.Controller.DismissViewController(true, null);
		//							});
		//						};
		//					}

		//					PresentViewController(mailController, true, null);
		//				}
		//				else
		//				{
		//					//new UIAlertView("Mail not supported", "Can't send mail from this device", null, "OK");
		//					PresentViewController(CommonFunctions.AlertPrompt("Mail not supported", "Can't send mail from this device", true, null, false, null), true, null);
		//				}
		//			};

		//			nheadeditbtn.TouchUpInside += delegate
		//			{
		//				//DismissViewController(true, null);

		//				//var dcanvas = new CanvasMainViewController { MREditing = true, MREditPath = rowData.MRPath, MREditId = rowData.MRId, MREditName = rowData.MRName, apptId = SelectedAppointment.ApptId, patientId = SelectedAppointment.ApptPatientId, doctorId = SelectedAppointment.ApptDoctorId, locationId = SelectedAppointment.ApptLocationId, IsDoctorForm = true };
		//				//var dcanvas = new FingerPaintViewController() { MREditing = true, MREditPath = rowData.MRPath, MREditId = rowData.MRId, MREditName = rowData.MRName, apptId = SelectedAppointment.ApptId, patientId = SelectedAppointment.ApptPatientId, doctorId = SelectedAppointment.ApptDoctorId, locationId = SelectedAppointment.ApptLocationId, IsDoctorForm = true };



		//				SfImageEditor imageEditor = new SfImageEditor();
  //                      imageEditor.Frame = new CGRect(View.Bounds.X, 0, View.Bounds.Width, View.Bounds.Height - 50);
  //                      var editfilename = rowData.MRName + "_" + "edit" + "_" + DateTime.Now.ToString("s").Replace(":", "_") + ".jpg";
		//				var downloadPath = Path.Combine(Path.GetTempPath(), editfilename);
		//				var url = rowData.MRPath;
		//				var webClient = new WebClient();
		//				webClient.DownloadFile(url, downloadPath);
		//				UIImage img = UIImage.LoadFromData(UIImage.FromFile(downloadPath).AsJPEG(), 1);
		//				imageEditor.Image = img;
  //                      imageEditor.ImageSaved += delegate {
		//					var file = imageEditor.Image.AsPNG().ToArray();
		//					var dds = new DynaPadService.DynaPadService() { Timeout = 180000};
		//					dds.SaveFile(CommonFunctions.GetUserConfig(), SelectedAppointment.ApptId, SelectedAppointment.ApptPatientId, SelectedAppointment.ApptDoctorId, SelectedAppointment.ApptLocationId, editfilename, "Edit", "DynaPad", "", "", file, true, true);
		//				    DismissViewController(true, null);
  //                      };

		//				var ielab = new UILabel(new CGRect(10, 10, View.Bounds.Width - 110, 50))
		//				{
		//					Text = rowData.MRName
		//				};
		//				var iecellHeader = new UITableViewCell(UITableViewCellStyle.Default, null)
		//				{
		//					Frame = new CGRect(0, 0, View.Bounds.Width, 50)
		//				};

		//				//var ieheadprintbtn = new UIButton(new CGRect(View.Bounds.Width - 100, 10, 50, 50));
		//				//ieheadprintbtn.SetImage(UIImage.FromBundle("Print"), UIControlState.Normal);
		//				//ieheadprintbtn.TouchUpInside += delegate {
  //    //                      Print(rowData.MRName, imageEditor.ViewPrintFormatter);
		//				//};
		//				//iecellHeader.ContentView.Add(ieheadprintbtn);

		//				var ieheadclosebtn = new UIButton(new CGRect(View.Bounds.Width - 50, 10, 50, 50));
		//				ieheadclosebtn.SetImage(UIImage.FromBundle("Close"), UIControlState.Normal);
		//				ieheadclosebtn.TouchUpInside += delegate
		//				{
		//					DismissViewController(true, null);
		//				};
		//				iecellHeader.ContentView.Add(ielab);
		//			    iecellHeader.ContentView.Add(ieheadclosebtn);
		//				var iesec = new Section(iecellHeader);
		//				iesec.FooterView = new UIView(new CGRect(0, 0, 0, 0));
		//				iesec.FooterView.Hidden = true;
  //                      iesec.Add(imageEditor);
		//				var ieroo = new RootElement("File Edit");
		//				ieroo.Add(iesec);
		//				var iedia = new DialogViewController(ieroo);
  //                      iedia.TableView.ScrollEnabled = false;
  //                      iedia.ModalInPopover = true;
		//				iedia.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
		//				iedia.PreferredContentSize = new CGSize(View.Bounds.Size);
  //                      PresentViewController(iedia, true, null);


		//				//var asss = new Section();
		//				//asss.Add(dcanvas.View); 		//				//var rr = new RootElement("nass");
		//				//rr.Add(asss); 		//				//var nndia = new DialogViewController(rr);
		//				//nndia.ModalInPopover = true; 		//				//nndia.ModalPresentationStyle = UIModalPresentationStyle.FullScreen; 		//				//nndia.PreferredContentSize = new CGSize(View.Bounds.Size);



		//				//PreferredContentSize = new CGSize(View.Bounds.Size);
		//				//NavigationController.View.BackgroundColor = UIColor.Clear;
		//				//dcanvas.AutomaticallyAdjustsScrollViewInsets = true;
		//				//dcanvas.LoadView();
		//				//PresentViewController(dcanvas, true, null);
		//				//NavigationController.View.SizeToFit();




		//				//var ass = new UIBarButtonItem("edit", UIBarButtonItemStyle.Plain ,delegate
		//				//{
		//				//  PreferredContentSize = new CGSize(View.Bounds.Size);
		//				//  SfImageEditor imageEditor = new SfImageEditor();
		//				//             //imageEditor.Frame = new CGRect(0, 0, 500, 500);
		//				//             imageEditor.Frame = View.Frame;
		//				//  var downloadPath = Path.Combine(Path.GetTempPath(), "testjpg.jpg");
		//				//  var url = "https://amato.dynadox.pro/data/testjpg.jpg";
		//				//  var webClient = new WebClient();
		//				//  webClient.DownloadFile(url, downloadPath);
		//				//             UIImage img = UIImage.LoadFromData(UIImage.FromFile(downloadPath).AsJPEG(), 1);
		//				//             imageEditor.Image = img;
		//				//             var sec = new Section();
		//				//             sec.Add(imageEditor);
		//				//             var rr = new RootElement("edit");
		//				//             rr.Add(sec);
		//				//             var dvc = new DialogViewController(rr);
		//				//             var vvv = new UIViewController();
		//				//             vvv.View = imageEditor;
		//				//             //dvc.ModalPresentationStyle = UIModalPresentationStyle.Popover;
		//				//             //dvc.PreferredContentSize = new CGSize(View.Bounds.Size);
		//				//             //View.AddSubview(imageEditor);
		//				//             PresentViewController(vvv, true, null);
		//				//});
		//				//NavigationItem.SetRightBarButtonItem(ass, true);
		//			};

		//			if (rowData.IsShortcut)
		//			{
		//				DismissViewController(true, null);
		//			}

		//			//PreferredContentSize = new CGSize(View.Bounds.Size);
		//			//PresentViewController(ndia, true, null);
		//			NavigationController.PushViewController(ndia, true);

		//			//var closeNavBtn = new UIBarButtonItem(UIImage.FromBundle("Close"), UIBarButtonItemStyle.Plain, delegate
		//			//{ NavigationController.PopViewController(true); });
		//			//printNavBtn = new UIBarButtonItem(UIImage.FromBundle("Print"), UIBarButtonItemStyle.Plain, delegate
		//			//{ Print(rowData.MRName, webViews.ViewPrintFormatter); });
  //                  //ndia.NavigationItem.SetRightBarButtonItems(new UIBarButtonItem[] { closeNavBtn, printNavBtn }, true);
  //                  //NavigationController.NavigationBar.Hidden = true;
  //                  //ndia.NavigationController.NavigationBarHidden = true;
		//			//NavigationItem.SetRightBarButtonItems(new UIBarButtonItem[] { closeNavBtn, printNavBtn }, true);
		//		}
		//	}
		//	catch (Exception ex)
		//	{
  //              var errordata = (MR)e.RowData;
  //              var errorfile = "<br/><br/><br/>FILE PATH:<br/><br/>" + errordata.MRPath;
  //              CommonFunctions.sendErrorEmail(ex, errorfile);
  //              PresentViewController(CommonFunctions.ExceptionAlertPrompt(), true, null);
		//		//throw new Exception(ex.Message + Environment.NewLine + ex.StackTrace, ex.InnerException);
		//	}
  //          //finally
  //          //{
  //          //    loadingOverlay.Hide();
  //          //}
		//}
            
  //      public void ProgressObserver(NSObservedChange nsObservedChange)
		//{
        //    Console.WriteLine("Progress {0}", MRWebView.EstimatedProgress);
        //    progressView.Progress = (float)MRWebView.EstimatedProgress;
        //    if (progressView.Progress >= 1.0)
        //    {
        //        progressView.Progress = 0;
        //    }
        //}









        void DataGrid_GridDoubleTapped(object sender, GridDoubleTappedEventsArgs e)
        {
            try
            {
                if (e.RowData.GetType() == typeof(MR))
                {
                    var rowIndex = e.RowColumnindex.RowIndex;
                    var rowData = (MR)e.RowData;
                    var columnIndex = e.RowColumnindex.ColumnIndex;
                    var filepath = rowData.MRPath;
                    var filetype = rowData.MRFileType;

                    if (filepath.StartsWith("Error:", StringComparison.CurrentCulture))
                    {
                        PresentViewController(CommonFunctions.AlertPrompt("File Error", "File unavailable, contact administration", true, null, false, null), true, null);
                        return;
                    }

                    if (rowData.IsShortcut)
                    {
                        DismissViewController(true, null);
                    }

                    //"https://test.dynadox.pro/data/test.dynadox.pro/claimantfiles/37/361.pdf"
                    //var wkurl = new NSUrl("https://test.dynadox.pro/data/test.dynadox.pro/claimantfiles/darwinmeds.pdf");
                    //var wkurl = new NSUrl(rowData.MRPath.Replace("https", "http"));//"https://test.dynadox.pro/data/test.dynadox.pro/claimantfiles/darwinmeds.pdf"
                    var wkurl = new NSUrl(rowData.MRPath);

                    var sfViewController = new SFSafariViewController(wkurl);
                    PresentViewController(sfViewController, true, null);
                }
            }
            catch (Exception ex)
            {
                var errordata = (MR)e.RowData;
                var errorfile = "<br/><br/><br/>FILE PATH:<br/><br/>" + errordata.MRPath;
                CommonFunctions.sendErrorEmail(ex, errorfile);
                PresentViewController(CommonFunctions.ExceptionAlertPrompt(), true, null);
            }
        }









		void GridAutoGeneratingColumns(object sender, AutoGeneratingColumnArgs e)
		{
			switch (e.Column.MappingName)
			{
				case "MRId":
					e.Cancel = true;
					break;
				case "MRName":
					e.Column.HeaderText = "File Name";
					break;
				case "MRApptId":
					e.Cancel = true;
					break;
				case "MRApptDate":
					e.Column.HeaderText = "Appt Date";
					break;
				case "MRDoctorId":
					e.Cancel = true;
					break;
				case "MRDoctor":
					e.Column.HeaderText = "Doctor";
					break;
				case "MRDoctorLocationId":
					e.Cancel = true;
					break;
				case "MRLocation":
					e.Column.HeaderText = "Location";
					break;
				case "MRPatientId":
					e.Cancel = true;
					break;
				case "MRPath":
					e.Cancel = true;
					break;
				case "MRFileType":
					e.Cancel = true;
					break;
			}
		}


		public class GridCommand : ICommand
		{
			//Action execute;
			Action<SfDataGrid, string> execute;
			bool canExecute = true;
			SfDataGrid grid;
			string gv;

			public event EventHandler CanExecuteChanged;

			//public GridCommand(Action action)
			public GridCommand(Action<SfDataGrid, string> action, SfDataGrid g, string v)
			{
				execute = action;
				grid = g;
				gv = v;
			}

			public bool CanExecute(object parameter)
			{
				return canExecute;
			}

			public void Execute(object parameter)
			{
				changeCanExecute(true);
				execute.Invoke(grid, gv);
			}

			void changeCanExecute(bool parCanExecute)
			{
				canExecute = parCanExecute;
				if (CanExecuteChanged != null)
					CanExecuteChanged(this, new EventArgs());
			}
		}


		async void ExecutePullToRefreshCommand(SfDataGrid dataGrid, string valueId)
		{
			try
			{
				dataGrid.IsBusy = true;
				await Task.Delay(new TimeSpan(0, 0, 5));
				ItemsSourceRefresh(dataGrid, valueId);
				dataGrid.IsBusy = false;
			}
			catch (Exception ex)
			{
				CommonFunctions.sendErrorEmail(ex);
				PresentViewController(CommonFunctions.ExceptionAlertPrompt(), true, null);
				//throw new Exception(ex.Message + Environment.NewLine + ex.StackTrace, ex.InnerException);
			}
		}

		//ViewModel.cs
		internal void ItemsSourceRefresh(SfDataGrid dataGrid, string valueId)
		{
            try
			{
				//var bounds = base.TableView.Frame;
				//loadingOverlay = new LoadingOverlay(bounds);
				//mvc = (DialogViewController)((UINavigationController)SplitViewController.ViewControllers[1]).TopViewController;
				//mvc.Add(loadingOverlay);

                var dds = new DynaPadService.DynaPadService() { Timeout = 60000 };
                var origJson = dds.GetFiles(CommonFunctions.GetUserConfig(), SelectedAppointment.ApptId, SelectedAppointment.ApptPatientId, SelectedAppointment.ApptPatientName, SelectedAppointment.ApptDoctorId, SelectedAppointment.ApptLocationId);
                JsonHandler.OriginalFormJsonString = origJson;
                SelectedAppointment.ApptMRFolders = JsonConvert.DeserializeObject<List<MRFolder>>(origJson);
                var mrs = SelectedAppointment.ApptMRFolders.Find(mr => mr.MRFolderId == valueId).MrFolderMRs;
                dataGrid.ItemsSource = mrs;
			}
			catch (Exception ex)
			{
				CommonFunctions.sendErrorEmail(ex);
                PresentViewController(CommonFunctions.ExceptionAlertPrompt(), true, null);
				//throw new Exception(ex.Message + Environment.NewLine + ex.StackTrace, ex.InnerException);
			}
            //finally
            //{
            //    loadingOverlay.Hide();
            //}
		}


		public DynaMultiRootElement GetMRElement(string valueId)
		{
			try
			{
				var mrElement = new DynaMultiRootElement(SelectedAppointment.ApptFormName);

				var mrPaddedView = new PaddedUIView<UILabel>();
				mrPaddedView.Enabled = true;
				mrPaddedView.Type = "Section";
				mrPaddedView.Frame = new CGRect(0, 0, 0, 40);
				mrPaddedView.Padding = 5f;
				mrPaddedView.NestedView.Text = "MR - " + SelectedAppointment.ApptPatientName;
				mrPaddedView.NestedView.TextAlignment = UITextAlignment.Center;
				mrPaddedView.NestedView.Font = UIFont.BoldSystemFontOfSize(17);
				mrPaddedView.setStyle();

				var mrSection = new DynaSection("MR");
				//mrSection.HeaderView = mrPaddedView;
				mrSection.HeaderView = new UIView(new CGRect(0, 0, 0, 0));
				mrSection.FooterView = new UIView(new CGRect(0, 0, 0, 0));
				mrSection.FooterView.Hidden = true;

				var mrs = SelectedAppointment.ApptMRFolders.Find(mr => mr.MRFolderId == valueId).MrFolderMRs;
				var fgrid = new SfDataGrid();
				fgrid.Frame = new CGRect(0, 0, View.Frame.Width, View.Frame.Height);
				fgrid.ItemsSource = mrs;
				fgrid.GridDoubleTapped += DataGrid_GridDoubleTapped;
				fgrid.AutoGenerateColumns = false;
				//fgrid.AutoGeneratingColumn += GridAutoGeneratingColumns;
				fgrid.ColumnSizer = ColumnSizer.None;
				//fgrid.BackgroundColor = UIColor.Green;
                fgrid.SelectionMode = SelectionMode.SingleDeselect;
				fgrid.AllowPullToRefresh = true;
                fgrid.PullToRefreshCommand = new GridCommand(ExecutePullToRefreshCommand, fgrid, valueId);
				fgrid.AllowSorting = true;
				//fgrid.View.LiveDataUpdateMode = Syncfusion.Data.LiveDataUpdateMode.AllowDataShaping;

				var mrNameColumn = new GridTextColumn();
				mrNameColumn.MappingName = "MRName";
                mrNameColumn.HeaderText = " File Name (Double tap to view)";
				mrNameColumn.Width = fgrid.Frame.Width * 0.55;
				//mrNameColumn.MinimumWidth = fgrid.Frame.Width * 0.40;
				mrNameColumn.HeaderTextAlignment = UITextAlignment.Left;
				mrNameColumn.TextAlignment = UITextAlignment.Left;

				var mrDateColumn = new GridTextColumn();
				mrDateColumn.MappingName = "MRApptDate";
				mrDateColumn.HeaderText = "Appt Date";
				mrDateColumn.Width = fgrid.Frame.Width * 0.15;
				//mrDateColumn.MinimumWidth = fgrid.Frame.Width * 0.20;
				mrDateColumn.HeaderTextAlignment = UITextAlignment.Left;
				mrDateColumn.TextAlignment = UITextAlignment.Left;

				var mrDoctorColumn = new GridTextColumn();
				mrDoctorColumn.MappingName = "MRDoctor";
				mrDoctorColumn.HeaderText = "Doctor";
				mrDoctorColumn.Width = fgrid.Frame.Width * 0.15;
				//mrDoctorColumn.MinimumWidth = fgrid.Frame.Width * 0.20;
				mrDoctorColumn.HeaderTextAlignment = UITextAlignment.Left;
				mrDoctorColumn.TextAlignment = UITextAlignment.Left;

				var mrLocationColumn = new GridTextColumn();
				mrLocationColumn.MappingName = "MRLocation";
				mrLocationColumn.HeaderText = "Location";
				mrLocationColumn.Width = fgrid.Frame.Width * 0.15;
				//mrLocationColumn.MinimumWidth = fgrid.Frame.Width * 0.20;
				mrLocationColumn.HeaderTextAlignment = UITextAlignment.Left;
				mrLocationColumn.TextAlignment = UITextAlignment.Left;

				var mrFileTypeColumn = new GridTextColumn();
				mrFileTypeColumn.MappingName = "MRFileType";
				mrFileTypeColumn.HeaderText = "File Type";
				//mrFileTypeColumn.Width = fgrid.Frame.Width * 0.15;
				mrFileTypeColumn.HeaderTextAlignment = UITextAlignment.Left;
				mrFileTypeColumn.TextAlignment = UITextAlignment.Left;
                mrFileTypeColumn.IsHidden = true;

				fgrid.Columns.Add(mrNameColumn);
				fgrid.Columns.Add(mrDateColumn);
				fgrid.Columns.Add(mrDoctorColumn);
				fgrid.Columns.Add(mrLocationColumn);
				fgrid.Columns.Add(mrFileTypeColumn);

				mrSection.Add(fgrid);
				mrElement.Add(mrSection);

				return mrElement;
			}
			catch (Exception ex)
			{
				CommonFunctions.sendErrorEmail(ex);
                PresentViewController(CommonFunctions.ExceptionAlertPrompt(), true, null);
                return null;
				//throw new Exception(ex.Message + Environment.NewLine + ex.StackTrace, ex.InnerException);
			}
		}



        //public void ProgressObservers(NSObservedChange nsObservedChange)
        //{
        //    try
        //    {
        //        //Console.WriteLine("Progress {0}", webViews.EstimatedProgress);
        //        progressViews.Progress = (float)MRWebViews.EstimatedProgress;
        //        if (progressViews.Progress >= 1.0)
        //        {
        //            progressViews.Progress = 0;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        CommonFunctions.sendErrorEmail(ex);
        //        PresentViewController(CommonFunctions.ExceptionAlertPrompt(), true, null);
        //    }
        //}

        //WKWebView MRWebViews;
        //UIProgressView progressViews;

		public async void SetDetailItem(Section newDetailItem, string context, string valueId, string origSectionJson, bool IsDoctorForm, GlassButton nextbtn = null, bool IsViewSummary = false, string SummaryFileName = null, string ReportName = null)
		{
            try
            {
                //var boundsf = UIScreen.MainScreen.Bounds;
                //var boundsf = SplitViewController.View.Bounds;
                var boundsh = base.TableView.Frame;
                //loadingOverlay = new LoadingOverlay(boundsh);
                mvc = (DialogViewController)((UINavigationController)SplitViewController.ViewControllers[1]).TopViewController;
                //mvc.Add(loadingOverlay);
                //SplitViewController.Add(loadingOverlay);
                //await Task.Delay(10);

                if (DetailItem != newDetailItem)
                {
                    ReloadData();
                    DetailItem = newDetailItem;
                    switch (context)
                    {
                        //                 case "URL":
                        //                     Root.Clear();
                        //                     string URL = "http://www.adobe.com/content/dam/Adobe/en/devnet/acrobat/pdfs/pdf_reference_1-7.pdf";
                        ////var defaultWebViews = new UIWebView(View.Bounds);
                        ////                     defaultWebViews.LoadRequest(new NSUrlRequest(new NSUrl(valueId, false)));
                        ////defaultWebViews.ScalesPageToFit = true;

                        //var pdfViewerControl = new SfPdfViewer();
                        //pdfViewerControl.Frame = View.Bounds;//new CGRect(0, 0, 300, 350);
                        //using (MemoryStream mem = new MemoryStream())
                        //{
                        //                         ConvertToStream(URL, mem);
                        //	mem.Seek(0, SeekOrigin.Begin);
                        //	pdfViewerControl.LoadDocument(mem);
                        //}

                        ////var q = new PdfDocument(URL);

                        //var s = new DynaSection("file");
                        //s.Add(pdfViewerControl);
                        //var f = new DynaMultiRootElement("File");
                        //f.Add(s);
                        //Root = f;
                        //Root.TableView.ScrollEnabled = false;
                        //break;
                        case "MR":
                            loadingOverlay = new LoadingOverlay(boundsh);// { loadingLabelText = "Loading MR..." };
                            loadingOverlay.SetText("Loading MR...");
                            mvc.Add(loadingOverlay);
                            await Task.Delay(10);

                            var mrElement = GetMRElement(valueId);
                            Root = mrElement;
                            //Root.TableView.ScrollEnabled = false;

                            break;
                        case "Summary":
                            loadingOverlay = new LoadingOverlay(boundsh);// { loadingLabelText = "Loading Summary..." };
                            if (IsViewSummary)
                            {
                                loadingOverlay.SetText("Loading Summary...");
                                mvc.Add(loadingOverlay);
                            }
                            await Task.Delay(10);

                            var summaryElement = new DynaMultiRootElement(SelectedAppointment.ApptFormName);

                            var summaryPaddedView = new PaddedUIView<UILabel>();
                            summaryPaddedView.Enabled = true;
                            summaryPaddedView.Type = "Section";
                            summaryPaddedView.Frame = new CGRect(0, 0, 0, 40);
                            summaryPaddedView.Padding = 5f;
                            summaryPaddedView.NestedView.Text = "SUMMARY";
                            summaryPaddedView.NestedView.TextAlignment = UITextAlignment.Center;
                            summaryPaddedView.NestedView.Font = UIFont.BoldSystemFontOfSize(17);
                            summaryPaddedView.setStyle();

                            var summarySection = new DynaSection("SUMMARY");
                            summarySection.HeaderView = summaryPaddedView;
                            summarySection.FooterView = new UIView(new CGRect(0, 0, 0, 0));
                            summarySection.FooterView.Hidden = true;

                            var summaryFileName = "";

                            if (!IsViewSummary)
                            {
                                if (CrossConnectivity.Current.IsConnected)
                                {
                                    var dds = new DynaPadService.DynaPadService() { Timeout = 60000 };
                                    var finalJson = JsonConvert.SerializeObject(SelectedAppointment.SelectedQForm);
                                    summaryFileName = dds.GenerateSummary(CommonFunctions.GetUserConfig(), finalJson);
                                    //SplitViewController.NavigationController.PopViewController(true);
                                    //mvc.NavigationController.PopViewController(true);
                                    //NavigationController.PopViewController(true);
                                    SFSafariViewController sfViewController = new SFSafariViewController(new NSUrl(summaryFileName));
                                    PresentViewController(sfViewController, true, null);

                                    var mas = (DialogViewController)((UINavigationController)SplitViewController.ViewControllers[0]).TopViewController;
                                    mas.NavigationController.PopViewController(true);
                                }
                                else
                                {
                                    PresentViewController(CommonFunctions.InternetAlertPrompt(), true, null);
                                }
                            }
                            else
                            {
                                summaryFileName = SummaryFileName;
                            }

                            if (!summaryFileName.StartsWith("Error:", StringComparison.CurrentCulture))
                            {
                                if (IsViewSummary)
                                {
                                    var webView = new WKWebView(View.Bounds, new WKWebViewConfiguration());
                                    webView.Frame = new CGRect(View.Bounds.X, 0, View.Bounds.Width, View.Bounds.Height);
                                    webView.LoadRequest(new NSUrlRequest(new NSUrl(summaryFileName)));
                                    summarySection.Add(webView);
                                    summaryElement.Add(summarySection);
                                }
                                else
                                {
                                    var sucmes = IsDoctorForm ? "Doctor form submitted successfully. A report has been generated." : "Patient form submitted successfully.";
                                    summarySection.Add(new StringElement(sucmes));
                                    summaryElement.Add(summarySection);
                                }
                                Root = summaryElement;
                            }
                            else
                            {
                                summarySection.Add(new StringElement("File unavailable, contact administration"));
                                summaryElement.Add(summarySection);
                                Root = summaryElement;
                                Root.TableView.ScrollEnabled = false;
                            }

                            break;
                        case "Finalize":
                            loadingOverlay = new LoadingOverlay(boundsh);// { loadingLabelText = "Finalizing..." };
                            loadingOverlay.SetText("Finalizing...");
                            mvc.Add(loadingOverlay);
                            await Task.Delay(10);

                            var rootElement = new DynaMultiRootElement(SelectedAppointment.SelectedQForm.FormName + " - " + SelectedAppointment.ApptPatientName);

                            var rootPaddedView = new PaddedUIView<UILabel>();
                            rootPaddedView.Enabled = true;
                            rootPaddedView.Type = "Section";
                            rootPaddedView.Frame = new CGRect(0, 0, 0, 40);
                            rootPaddedView.Padding = 5f;
                            rootPaddedView.NestedView.Text = "FINALIZE FORM";
                            rootPaddedView.NestedView.TextAlignment = UITextAlignment.Center;
                            rootPaddedView.NestedView.Font = UIFont.BoldSystemFontOfSize(17);
                            rootPaddedView.setStyle();

                            var rootSection = new DynaSection("FINALIZE FORM");
                            rootSection.HeaderView = rootPaddedView;
                            rootSection.FooterView = new UIView(new CGRect(0, 0, 0, 0));
                            rootSection.FooterView.Hidden = true;

                            var sigPad = new Xamarin.Controls.SignaturePadView(new CGRect(0, 0, View.Frame.Width, 600));
                            sigPad.CaptionText = "Signature here:";
                            sigPad.BackgroundColor = UIColor.White;

                            messageLabel = new UILabel();

                            var btnSubmit = new GlassButton(new RectangleF(0, 0, (float)View.Frame.Width, 50));
                            //btnSubmit.Font = UIFont.BoldSystemFontOfSize(17);
                            btnSubmit.TitleLabel.Font = UIFont.BoldSystemFontOfSize(17);
                            btnSubmit.NormalColor = UIColor.Green;
                            btnSubmit.DisabledColor = UIColor.Gray;
                            btnSubmit.SetTitle("Submit Form", UIControlState.Normal);
                            btnSubmit.TouchUpInside += (sender, e) =>
                            {
                                UIAlertController SubmitPrompt;

                                if (IsDoctorForm)
                                {
                                    SubmitPrompt = UIAlertController.Create("Submit Form", "Submit the form?", UIAlertControllerStyle.Alert);
                                    SubmitPrompt.Add(messageLabel);
                                    SubmitPrompt.AddAction(UIAlertAction.Create("Yes", UIAlertActionStyle.Default, action => SubmitForm(null, IsDoctorForm, sigPad)));
                                    SubmitPrompt.AddAction(UIAlertAction.Create("No", UIAlertActionStyle.Cancel, null));
                                    //Present Alert
                                    PresentViewController(SubmitPrompt, true, null);
                                }
                                else
                                {
                                    SubmitPrompt = UIAlertController.Create("Submit Form", "Please hand back the IPad to submit", UIAlertControllerStyle.Alert);
                                    SubmitPrompt.AddTextField((field) =>
                                    {
                                        field.SecureTextEntry = true;
                                        field.Placeholder = "Password";
                                    });
                                    SubmitPrompt.Add(messageLabel);
                                    SubmitPrompt.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, action => SubmitForm(SubmitPrompt.TextFields[0].Text, IsDoctorForm, sigPad)));
                                    SubmitPrompt.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, null));
                                    //Present Alert
                                    PresentViewController(SubmitPrompt, true, null);
                                }
                            };

                            rootSection.Add(sigPad);
                            rootSection.Add(btnSubmit);
                            rootElement.Add(rootSection);

                            Root = rootElement;
                            Root.TableView.ScrollEnabled = false;

                            break;
                        case "Report":
                            loadingOverlay = new LoadingOverlay(boundsh);// { loadingLabelText = "Loading Report..." };
                            loadingOverlay.SetText("Loading Report...");
                            mvc.Add(loadingOverlay);
                            await Task.Delay(10);

                            //var Report = SelectedAppointment.ApptReports.Find((Report obj) => obj.FormId == sectionId);

                            var reportElement = new DynaMultiRootElement(SelectedAppointment.ApptFormName);

                            var reportPaddedView = new PaddedUIView<UILabel>();
                            reportPaddedView.Enabled = true;
                            reportPaddedView.Type = "Section";
                            reportPaddedView.Frame = new CGRect(0, 0, 0, 40);
                            reportPaddedView.Padding = 5f;
                            reportPaddedView.NestedView.Text = ReportName;//"REPORT"; //Report.ReportName.ToUpper();
                            reportPaddedView.NestedView.TextAlignment = UITextAlignment.Center;
                            reportPaddedView.NestedView.Font = UIFont.BoldSystemFontOfSize(17);
                            reportPaddedView.setStyle();

                            var reportSection = new DynaSection("REPORT");
                            reportSection.HeaderView = reportPaddedView;
                            reportSection.FooterView = new UIView(new CGRect(0, 0, 0, 0));
                            reportSection.FooterView.Hidden = true;

                            var reportUrl = "";
                            if (CrossConnectivity.Current.IsConnected)
                            {
                                var dps = new DynaPadService.DynaPadService();
                                reportUrl = dps.GenerateReport(CommonFunctions.GetUserConfig(), SelectedAppointment.ApptId, SelectedAppointment.ApptFormId, DateTime.Today.ToShortDateString(), "file", valueId);
                                //string report = dps.GenerateReport("123", SelectedQForm.ApptPatientID, DateTime.Today.ToShortDateString(), "file", SelectedQForm.ApptPatientFormID);
                                //var asdf = SelectedAppointment.ApptPatientId;
                            }

                            if (!reportUrl.StartsWith("Error:", StringComparison.CurrentCulture))
                            {
                                //var wkurl = new NSUrl(reportUrl);
                                //var wkrequest = new NSUrlRequest(wkurl);
                                //MRWebViews = new WKWebView(View.Bounds, new WKWebViewConfiguration());// { SuppressesIncrementalRendering = true });
                                //MRWebViews.Frame = new CGRect(View.Bounds.X, 0, View.Bounds.Width, View.Bounds.Height);
                                //progressViews = new UIProgressView(UIProgressViewStyle.Bar);
                                //progressViews.Frame = new CGRect(0, 0, MRWebViews.Frame.Width, 5);


                                //MRWebViews.AddObserver("estimatedProgress", NSKeyValueObservingOptions.New, ProgressObservers);
                                //MRWebViews.AddSubview(progressViews);
                                //MRWebViews.LoadRequest(wkrequest);





                                //var bb = View.Frame;
                                //var webView = new UIWebView(View.Bounds);
                                //webView.ScalesPageToFit = true;
                                //var myurl = "https://test.dynadox.pro/dynawcfservice/" + report; // NOTE: https secure request
                                //var myurl = "https://test.dynadox.pro/dynawcfservice/test.pdf";// + report; // NOTE: https secure request
                                //var url = "https://www.princexml.com/samples/invoice/invoicesample.pdf"; // NOTE: https secure request


                                var webView = new WKWebView(View.Bounds, new WKWebViewConfiguration());
                                webView.Frame = new CGRect(View.Bounds.X, 0, View.Bounds.Width, View.Bounds.Height);
                                webView.LoadRequest(new NSUrlRequest(new NSUrl(reportUrl)));

                                reportSection.Add(webView);

                                NavigationItem.SetRightBarButtonItem(new UIBarButtonItem(UIImage.FromBundle("Print"), UIBarButtonItemStyle.Plain, (sender, args) =>
                                { Print(SelectedAppointment.ApptFormName, webView.ViewPrintFormatter); }), true);
                            }
                            else
                            {
                                reportSection.Add(new StringElement("File unavailable, contact administration"));
                            }

                            reportElement.Add(reportSection);

                            Root = reportElement;
                            Root.TableView.ScrollEnabled = true;

                            break;
                        default:
                            loadingOverlay = new LoadingOverlay(boundsh);// { loadingLabelText = "Loading Section..." };
                            loadingOverlay.SetText("Loading Section...");
                            mvc.Add(loadingOverlay);
                            await Task.Delay(10);

                            ConfigureView(context, valueId, origSectionJson, IsDoctorForm, nextbtn);
                            break;
                    }
                }
				//loadingOverlay.Hide();
			}
			catch (Exception ex)
			{
				Root.Clear();
				Root.Add(CommonFunctions.ErrorDetailSection());
				ReloadData();
				CommonFunctions.sendErrorEmail(ex);
                PresentViewController(CommonFunctions.ExceptionAlertPrompt(), true, null);
				//throw new Exception(ex.Message + Environment.NewLine + ex.StackTrace, ex.InnerException);
			}
            finally
            {
                loadingOverlay.Hide();
            }
		}


		void ConfigureView(string context, string sectionId, string origS, bool IsDoctorForm, GlassButton nextbtn)
		{
			try
			{
				// Update the user interface for the detail item
				if (DetailItem != null)
				{
					var sectionQuestions = SelectedAppointment.SelectedQForm.FormSections.Find((FormSection obj) => obj.SectionId == sectionId);

					var headPaddedView = new PaddedUIView<UILabel>();
					headPaddedView.Enabled = true;
					headPaddedView.Type = "Section";
					headPaddedView.Frame = new CGRect(0, 0, 0, 40);
					headPaddedView.Padding = 5f;
					headPaddedView.NestedView.Text = sectionQuestions.SectionName.ToUpper();
					headPaddedView.NestedView.TextAlignment = UITextAlignment.Center;
					headPaddedView.NestedView.Font = UIFont.BoldSystemFontOfSize(17);
					headPaddedView.setStyle();

					var headSection = new DynaSection(sectionQuestions.SectionName);
					headSection.HeaderView = headPaddedView;
					headSection.FooterView = new UIView(new CGRect(0, 0, 0, 0));
					headSection.FooterView.Hidden = true;

					if (IsDoctorForm)
					{
						// Notes/Dictation
						var drawNavBtn = GetDrawNavBtn(sectionId);
						//var dicNavBtn = GetDicNavBtn(sectionId, IsDoctorForm);
						var mrNavBtn = GetMRNavBtn();

						//if (CrossConnectivity.Current.IsConnected)
						//{
							var picNavBtn = GetPhotoNavBtn(sectionId, context, SelectedAppointment.ApptPatientName, SelectedAppointment.ApptId, SelectedAppointment.ApptPatientId, SelectedAppointment.ApptDoctorId, SelectedAppointment.ApptLocationId, IsDoctorForm);
							//NavigationItem.SetLeftBarButtonItem(picNavBtn, true);
							NavigationItem.SetLeftBarButtonItems(new UIBarButtonItem[] { picNavBtn, mrNavBtn }, true);
						//}
						//else
						//{
						//	PresentViewController(CommonFunctions.InternetAlertPrompt(), true, null);
						//}

						//NavigationItem.SetRightBarButtonItems(new UIBarButtonItem[] { dicNavBtn, drawNavBtn }, true);

						// Presets
						var presetPaddedView = new PaddedUIView<UILabel>();
						presetPaddedView.Enabled = true;
						presetPaddedView.Type = "Preset";
						presetPaddedView.Frame = new CGRect(0, 0, 0, 30);
						presetPaddedView.Padding = 5f;
						presetPaddedView.NestedView.Text = "Section Presets";
						presetPaddedView.setStyle();

						var presetSection = new DynaSection("Section Presets");
						presetSection.Enabled = true;
						presetSection.HeaderView = presetPaddedView;
						presetSection.FooterView = new UIView(new CGRect(0, 0, 0, 0));
						presetSection.FooterView.Hidden = true;

						var fs = SelectedAppointment.SelectedQForm.FormSections.IndexOf(sectionQuestions);

						string[][] FormPresetNames = { };
						//if (CrossConnectivity.Current.IsConnected)
						//{
                        var dds = new DynaPadService.DynaPadService() { Timeout = 60000 };
							FormPresetNames = dds.GetAnswerPresets(CommonFunctions.GetUserConfig(), SelectedAppointment.SelectedQForm.FormId, sectionId, SelectedAppointment.ApptDoctorId, true, SelectedAppointment.ApptLocationId);
						//}
						//else
						//{
						//	PresentViewController(CommonFunctions.InternetAlertPrompt(), true, null);
						//}

						var presetGroup = new RadioGroup("PresetAnswers", sectionQuestions.SectionSelectedTemplateId);
						var presetsRoot = new DynaRootElement("Section Presets", presetGroup);
						presetsRoot.IsPreset = true;

						var noPresetRadio = new PresetRadioElement("No Preset", "PresetAnswers");
						noPresetRadio.PresetName = "No Preset";
						noPresetRadio.OnSelected += delegate
						{
							string presetJson = origS;
							SelectedAppointment.SelectedQForm.FormSections[fs] = JsonConvert.DeserializeObject<FormSection>(presetJson);
							var selectedSection = SelectedAppointment.SelectedQForm.FormSections.Find((FormSection obj) => obj.SectionId == sectionId);
							if (selectedSection != null)
							{
								selectedSection.SectionSelectedTemplateId = presetGroup.Selected;
							}

							SetDetailItem(new Section(sectionQuestions.SectionName), "", sectionId, origS, IsDoctorForm, nextbtn);
						};

						presetSection.Add(noPresetRadio);

						foreach (string[] arrPreset in FormPresetNames)
						{
							var mre = GetPreset(arrPreset[3], arrPreset[1], arrPreset[2], fs, sectionId, presetGroup, sectionQuestions, presetSection, origS, IsDoctorForm, nextbtn);

							presetSection.Add(mre);
						}

						var btnNewSectionPreset = new GlassButton(new RectangleF(0, 0, (float)View.Frame.Width, 50));
						//btnNewSectionPreset.Font = UIFont.BoldSystemFontOfSize(17);
						btnNewSectionPreset.TitleLabel.Font = UIFont.BoldSystemFontOfSize(17);
						btnNewSectionPreset.SetTitleColor(UIColor.Black, UIControlState.Normal);
						btnNewSectionPreset.NormalColor = UIColor.FromRGB(224, 238, 240);
						btnNewSectionPreset.SetTitle("Save New Section Preset", UIControlState.Normal);
						btnNewSectionPreset.TouchUpInside += (sender, e) =>
						{
							var SavePresetPrompt = UIAlertController.Create("New Section Preset", "Enter preset name: ", UIAlertControllerStyle.Alert);
							SavePresetPrompt.AddTextField((field) =>
							{
								field.Placeholder = "Preset Name";
							});
							//Add Actions
							SavePresetPrompt.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, action => SaveSectionPreset(null, SavePresetPrompt.TextFields[0].Text, sectionId, presetSection, null, presetGroup, origS, nextbtn)));
							SavePresetPrompt.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, null));
							//Present Alert
							PresentViewController(SavePresetPrompt, true, null);

						};

						presetSection.Add(btnNewSectionPreset);

						presetsRoot.Add(presetSection);
						presetsRoot.Enabled = true;

						headSection.Add(presetsRoot);
					}
					//else
					//{
					//	NavigationItem.SetRightBarButtonItem(null, false);
					//}

					QuestionsView = new DynaMultiRootElement(SelectedAppointment.SelectedQForm.FormName + " - " + SelectedAppointment.ApptPatientName);
					QuestionsView.Add(headSection);

					foreach (SectionQuestion question in sectionQuestions.SectionQuestions)
					{
						bool enabled = !question.IsConditional || (question.IsConditional && question.IsEnabled);
						var qSection = new DynaSection(question.QuestionText);
						qSection.QuestionId = question.QuestionId;
						qSection.Enabled = enabled;
						qSection.IsInvalid = question.IsInvalid;

						//question.IsRequired = false;

						nfloat qWidth = !IsDoctorForm ? View.Frame.Width - 50 : View.Frame.Width;
						//if (question.IsRequired)
						//{
						//	qWidth = qWidth - 5;
						//}

						var ww = (decimal)question.QuestionText.Length / 100;
						var wlines = (int)Math.Ceiling(ww);
						var wheight = 30 * wlines;
						//var cellHeight = !string.IsNullOrEmpty(question.Subtitle) ? 50 : 30;
						//var cellAdjSize = question.QuestionText.StringSize(UIFont.SystemFontOfSize(13), new CGSize(qWidth, 300), UILineBreakMode.WordWrap);
						var cellHeight = wheight;
						var cellHeaderHeight = wheight;

						if (!string.IsNullOrEmpty(question.Subtitle))
						{
							cellHeaderHeight = cellHeaderHeight + 20;
						}

						qSection.HeaderView = new UIView(new CGRect(0, 0, View.Frame.Width, cellHeaderHeight));

						if (!string.IsNullOrEmpty(question.Subtitle))
						{
							var qsPaddedView = new PaddedUIView<UILabel>();
							qsPaddedView.Enabled = enabled;
							qsPaddedView.Frame = new CGRect(0, 30, View.Frame.Width, 20);
							qsPaddedView.Padding = 5f;
							qsPaddedView.NestedView.Text = question.Subtitle.ToUpper();
							qsPaddedView.Type = "Subtitle";
							qsPaddedView.setStyle();

							qSection.HeaderView.Add(qsPaddedView);
						}

						var qPaddedView = new PaddedUIView<UILabel>();
						qPaddedView.Enabled = enabled;
						qPaddedView.Frame = new CGRect(0, 0, qWidth, cellHeight);
						qPaddedView.Padding = 5f;
						qPaddedView.NestedView.Text = question.QuestionText.ToUpper();
						qPaddedView.Type = "Question";
						qPaddedView.Required = question.IsRequired;

						qPaddedView.setStyle();

						if (!IsDoctorForm)
						{
							var qDictationButton = new UIButton(new CGRect(View.Frame.GetMaxX() - 50, 0, 50, 30));
							qDictationButton.Enabled = enabled;
							qDictationButton.SetImage(UIImage.FromBundle("QRecord"), UIControlState.Normal);
							if (enabled)
							{
								qDictationButton.BackgroundColor = UIColor.FromRGB(230, 230, 250);
							}
							else
							{
								qDictationButton.BackgroundColor = UIColor.GroupTableViewBackgroundColor;
							}

							if (question.QuestionText.Length < 250)
							{
								var que = question.QuestionText;
								var opts = "";

								if (question.QuestionOptions.Count >= 1)
								{
									opts = "... The options are: ";

									foreach (QuestionOption opt in question.QuestionOptions)
									{
										opts += (question.QuestionOptions.IndexOf(opt) + 1) + ". " + opt.OptionText + ".";
									}
								}

								qDictationButton.TouchUpInside += (sender, e) =>
								{
									ExecuteSpeechCommand(question.QuestionText + opts);
								};
							}

							qSection.HeaderView.Add(qPaddedView);

							//if (question.IsRequired)
							//{
							//	var reqLbl = new UILabel(new CGRect(qWidth, 0, 5, cellHeight));
							//	reqLbl.Text = "*";
							//	reqLbl.TextColor = UIColor.Red;
							//	reqLbl.BackgroundColor = UIColor.Clear;

							//	qSection.Add(reqLbl);
							//}

							qSection.HeaderView.Add(qDictationButton);
						}
						else
						{
							qSection.HeaderView.Add(qPaddedView);

							//if (question.IsRequired)
							//{
							//	var reqLbl = new UILabel(new CGRect(qWidth, 0, 5, cellHeight));
							//	reqLbl.Text = "*";
							//	reqLbl.TextColor = UIColor.Red;
							//	reqLbl.BackgroundColor = UIColor.Clear;

							//	qSection.Add(reqLbl);
							//}
						}

						qSection.FooterView = new UIView(new CGRect(0, 0, 0, 0));
						qSection.FooterView.Hidden = true;

						switch (question.QuestionType)
						{
							case "BodyParts":
							case "Check":

								foreach (QuestionOption opt in question.QuestionOptions)
								{
									var chk = new DynaCheckBoxElement(opt.OptionText, false, opt.OptionId);
									chk.Enabled = enabled;
									chk.Required = question.IsRequired;
									chk.Invalid = question.IsInvalid;
									chk.ConditionTriggerId = question.ParentConditionTriggerId;
									chk.Value = opt.Chosen;
									chk.Tapped += delegate
									{
										Chk_Tapped(question, opt, chk.Value, sectionId);
										chk.Invalid = ValidateQuestion(question);
										foreach (Element elm in qSection.Elements)
										{
											if (elm is DynaCheckBoxElement) ((DynaCheckBoxElement)elm).Invalid = chk.Invalid;
										}
										qSection.GetContainerTableView().ReloadData();
									};

									if (opt.Chosen && opt.ConditionTriggerIds != null && opt.ConditionTriggerIds.Count > 0)
									{
										ConditionalCheck(null, opt.ConditionTriggerIds, sectionId);
									}

									qSection.Add(chk);
								}

								qSection.HeaderView.Layer.BorderWidth = 5;

								QuestionsView.Add(qSection);

								break;

							case "Radio":
							case "Bool":
							case "YesNo":

								foreach (QuestionOption opt in question.QuestionOptions)
								{
									var radio = new DynaMultiRadioElement(opt.OptionText, question.QuestionId);
									radio.Enabled = enabled;
									radio.Chosen = opt.Chosen;
									radio.Required = question.IsRequired;
									radio.Invalid = question.IsInvalid;
									radio.ConditionTriggerId = question.ParentConditionTriggerId;
									radio.ElementSelected += delegate
									{
										Radio_Tapped(question, opt);
										NavigationController.PopViewController(true);
										radio.Invalid = ValidateQuestion(question);
										foreach (Element elm in qSection.Elements)
										{
											if (elm is DynaMultiRadioElement) ((DynaMultiRadioElement)elm).Invalid = radio.Invalid;
										}
										qSection.GetContainerTableView().ReloadData();
									};
									radio.OnDeselected += delegate
									{
										Radio_UnTapped(question, opt);
										NavigationController.PopViewController(true);
										radio.Invalid = ValidateQuestion(question);
										foreach (Element elm in qSection.Elements)
										{
											if (elm is DynaMultiRadioElement) ((DynaMultiRadioElement)elm).Invalid = radio.Invalid;
										}
										qSection.GetContainerTableView().ReloadData();
									};

									qSection.Add(radio);

									if (opt.Chosen)
									{
										QuestionsView.Select(question.QuestionId, question.QuestionOptions.IndexOf(opt));

										if (opt.ConditionTriggerIds != null && opt.ConditionTriggerIds.Count > 0)
										{
											if (question.ActiveTriggerIds == null)
											{
												question.ActiveTriggerIds = new List<string>();
											}
											question.ActiveTriggerIds.AddRange(opt.ConditionTriggerIds);
											ConditionalCheck(null, opt.ConditionTriggerIds, sectionId);
										}
									}
								}

								QuestionsView.Add(qSection);

								break;

							case "TextView":

								var tvdval = question.AnswerText;
								if (string.IsNullOrEmpty(SelectedAppointment.SelectedQForm.DateCompleted) && string.IsNullOrEmpty(question.AnswerText) && !string.IsNullOrEmpty(question.DefaultValue))
								{
									tvdval = question.DefaultValue;
								}

								var viewEntryElement = new PlaceholderEnabledUITextView(new CGRect(0, 0, View.Frame.Width, 100));
								viewEntryElement.AutocorrectionType = UITextAutocorrectionType.No;
								viewEntryElement.SpellCheckingType = UITextSpellCheckingType.No;
								viewEntryElement.EnablesReturnKeyAutomatically = false;
								viewEntryElement.Placeholder = "Enter your answer here";
								viewEntryElement.Text = tvdval;
								viewEntryElement.Required = question.IsRequired;
								viewEntryElement.Invalid = question.IsInvalid;
								viewEntryElement.PlaceholderColor = UIColor.LightGray;
								viewEntryElement.Editable = true;
								viewEntryElement.Enabled = enabled;
								viewEntryElement.QuestionId = question.QuestionId;
								viewEntryElement.ConditionTriggerId = question.ParentConditionTriggerId;
								viewEntryElement.AllowWhiteSpace = true;
								viewEntryElement.Ended += (sender, e) =>
								{
									question.AnswerText = viewEntryElement.Text;
									viewEntryElement.Invalid = ValidateQuestion(question);
									if (qSection.GetContainerTableView() != null)
									{
										qSection.GetContainerTableView().ReloadData();
									}
								};
								viewEntryElement.parentSec = qSection;

								//entryElement.PlaceholderColor = UIColor.LightGray;

								if (!enabled)
								{
									//entryElement.PlaceholderColor = UIColor.GroupTableViewBackgroundColor;
									viewEntryElement.TextColor = UIColor.LightGray;
									viewEntryElement.BackgroundColor = UIColor.GroupTableViewBackgroundColor;
									viewEntryElement.Placeholder = "Not applicable";
								}
								else
								{
									viewEntryElement.TextColor = UIColor.Black;
									viewEntryElement.BackgroundColor = UIColor.White;
								}

								qSection.Add(viewEntryElement);
								QuestionsView.Add(qSection);

								break;

							case "TextInput":
								var tidval = question.AnswerText;
								if (string.IsNullOrEmpty(SelectedAppointment.SelectedQForm.DateCompleted) && string.IsNullOrEmpty(question.AnswerText) && !string.IsNullOrEmpty(question.DefaultValue))
								{
									tidval = question.DefaultValue;
								}

								var entryElement = new DynaEntryElement("", "Enter your answer here", tidval);

								entryElement.EntryEnded += (sender, e) =>
								{
									question.AnswerText = entryElement.Value;
									entryElement.Invalid = ValidateQuestion(question);
									//ReloadData();
									if (qSection.GetContainerTableView() != null)
									{
										qSection.GetContainerTableView().ReloadData();
									}
									//this.TableView.ReloadData();
								};

								entryElement.QuestionKeyboardType = question.QuestionKeyboardTypeId;
								entryElement.MaxChars = string.IsNullOrEmpty(question.MaxValue) ? 99 : Convert.ToInt16(question.MaxValue);

								switch (question.QuestionKeyboardTypeId)
								{
									case "1"://"Normal":
										entryElement.KeyboardType = UIKeyboardType.Default;
										break;
									case "2"://"Email":
										entryElement.KeyboardType = UIKeyboardType.EmailAddress;
										break;
									case "3"://"Numeric":
										entryElement.KeyboardType = UIKeyboardType.NumberPad;
										break;
									case "4"://"Decimal":
										entryElement.KeyboardType = UIKeyboardType.DecimalPad;
										break;
									case "5"://"NumericPunctuation":
										entryElement.KeyboardType = UIKeyboardType.NumbersAndPunctuation;
										break;
									case "6"://"Phone":
										entryElement.KeyboardType = UIKeyboardType.PhonePad;
										break;
									default:
										entryElement.KeyboardType = UIKeyboardType.Default;
										break;
								}

								entryElement.Required = question.IsRequired;
								entryElement.Invalid = question.IsInvalid;
								entryElement.AutocorrectionType = UITextAutocorrectionType.No;
								//entryElement.EnablesReturnKeyAutomatically = false;
								entryElement.ShouldReturn += delegate
								{
									entryElement.ResignFirstResponder(true);
									return false;
								};
								entryElement.EntryStarted += delegate
								{
									question.AnswerText = entryElement.Value;
									//entryElement.Invalid = ValidateQuestion(question);
									//entryElement.BecomeFirstResponder(true);
									//if (qSection.GetContainerTableView() != null)
									//{
									//	qSection.GetContainerTableView().ReloadData();
									//}
								};
								entryElement.ClearButtonMode = UITextFieldViewMode.Always;
								entryElement.Enabled = enabled;
								entryElement.QuestionId = question.QuestionId;
								entryElement.ConditionTriggerId = question.ParentConditionTriggerId;

								qSection.Add(entryElement);
								QuestionsView.Add(qSection);

								break;
							case "AutoTextInput":

								var atidval = question.AnswerText;
								if (string.IsNullOrEmpty(SelectedAppointment.SelectedQForm.DateCompleted) && string.IsNullOrEmpty(question.AnswerText) && !string.IsNullOrEmpty(question.DefaultValue))
								{
									atidval = question.DefaultValue;
								}

								//var answerList = new NSMutableArray();
								//var listurl = DynaClassLibrary.DynaClasses.LoginContainer.User.DynaConfig.DomainRootPathVirtual + "DynaForms/" + SelectedAppointment.ApptFormId + "/AutoBoxLists/" + question.QuestionId + ".txt";
								//if (URLExists(listurl))
								//{
								//	XmlDocument listxml = new XmlDocument();
								//	//doc1.Load("https://amato.dynadox.pro/data/testautobox.txt");
								//	listxml.Load(listurl);
								//	XmlElement root = listxml.DocumentElement;
								//	XmlNodeList nodes = root.SelectNodes("/Items/Item");

								//	foreach (XmlNode node in nodes)
								//	{
								//		answerList.Add((NSString)node.Attributes[0].Value);
								//	}
								//}

								GetAutoData(question.QuestionId);

								var entryAutoComplete = new DynaAuto(new SFControlAutoCompleteDelegate(question, qSection));
								entryAutoComplete.Text = atidval;
								//entryAutoComplete.AutoCompleteSource = answerList;
								entryAutoComplete.DataSource = AutoDetails;
								entryAutoComplete.ShowSuggestionsOnFocus = false;
								entryAutoComplete.QuestionId = question.QuestionId;
								entryAutoComplete.Frame = new CGRect(0, 0, View.Frame.Width, 30);
								entryAutoComplete.TextField.BorderStyle = UITextBorderStyle.None;
								//entryAutoComplete.Bounds = new CGRect(0, 0, View.Frame.Width - 8, 30);
								entryAutoComplete.TextField.Bounds = new CGRect(0, 0, View.Frame.Width - 10, 30);
								entryAutoComplete.AutosizesSubviews = true;
								entryAutoComplete.BorderColor = UIColor.White;
								entryAutoComplete.Layer.BorderWidth = 0;
								//entryAutoComplete.HorizontalAlignment = UIControlContentHorizontalAlignment.Center;
								entryAutoComplete.SuggestionBoxPlacement = SFAutoCompleteSuggestionBoxPlacement.SFAutoCompleteSuggestionBoxPlacementBottom;
								entryAutoComplete.SuggestionMode = SFAutoCompleteSuggestionMode.SFAutoCompleteSuggestionModeContains;
								entryAutoComplete.Watermark = (NSString)"Enter your answer here";
								entryAutoComplete.MaxDropDownHeight = 150;
								entryAutoComplete.AutoCompleteMode = SFAutoCompleteAutoCompleteMode.SFAutoCompleteAutoCompleteModeSuggest;
								//entryAutoComplete.PopUpDelay = 100;
								entryAutoComplete.IsEnabled = enabled;
								entryAutoComplete.Enabled = enabled;
								entryAutoComplete.ConditionTriggerId = question.ParentConditionTriggerId;
								entryAutoComplete.parentSec = qSection;
								//entryAutoComplete.TextField.ShouldClear = (textField) => true;
								entryAutoComplete.TextField.ClearButtonMode = UITextFieldViewMode.Always;
								entryAutoComplete.Required = question.IsRequired;
								entryAutoComplete.Invalid = question.IsInvalid;
								entryAutoComplete.DisplayMemberPath = "Text";
								entryAutoComplete.SelectedValuePath = "Value";
								entryAutoComplete.MinimumPrefixCharacters = 2;

								entryAutoComplete.TextField.EditingDidEnd += (sender, e) =>
								{
									//entryAutoComplete.Invalid = ValidateQuestion(question);
									if (qSection.GetContainerTableView() != null)
									{
										qSection.GetContainerTableView().ReloadData();
									}
								};

								if (!enabled)
								{
									entryAutoComplete.TextColor = UIColor.LightGray;
									entryAutoComplete.BorderColor = UIColor.GroupTableViewBackgroundColor;
									entryAutoComplete.Layer.BorderColor = UIColor.GroupTableViewBackgroundColor.CGColor;
									entryAutoComplete.BackgroundColor = UIColor.GroupTableViewBackgroundColor;
									entryAutoComplete.TextField.BackgroundColor = UIColor.GroupTableViewBackgroundColor;
									entryAutoComplete.Watermark = (NSString)"Not applicable";
									entryAutoComplete.Layer.BackgroundColor = UIColor.GroupTableViewBackgroundColor.CGColor;
								}
								else
								{
									entryAutoComplete.TextColor = UIColor.Black;
									entryAutoComplete.BorderColor = UIColor.White;
									entryAutoComplete.Layer.BorderColor = UIColor.White.CGColor;
									entryAutoComplete.BackgroundColor = UIColor.White;
									entryAutoComplete.TextField.BackgroundColor = UIColor.White;
									entryAutoComplete.Watermark = (NSString)"Enter your answer here";
								}

								qSection.Add(entryAutoComplete);
								QuestionsView.Add(qSection);

								break;

							case "Date":

								//var dt = new DateTime();
								//dt = !string.IsNullOrEmpty(question.AnswerText) ? Convert.ToDateTime(question.AnswerText) : DateTime.Today;
								//dt.ToUniversalTime();
								DateTime dt;
								NullableDateElementInline dateElement;
								if (!string.IsNullOrEmpty(question.AnswerText))
								{
									dt = Convert.ToDateTime(question.AnswerText);
									dateElement = new NullableDateElementInline("", dt);
								}
								else
								{
									dateElement = new NullableDateElementInline("", null);
								}

								//var dateElement = new NullableDateElementInline("", dt);

								if (string.IsNullOrEmpty(SelectedAppointment.SelectedQForm.DateCompleted) && string.IsNullOrEmpty(question.AnswerText) && !string.IsNullOrEmpty(question.DefaultValue))
								{
									dateElement.SetDate(Convert.ToDateTime(question.DefaultValue));
								}

								dateElement.Caption = "Tap to select date";
								dateElement.Required = question.IsRequired;
								dateElement.Invalid = question.IsInvalid;
								dateElement.Enabled = enabled;
								dateElement.Alignment = UITextAlignment.Left;
								dateElement.QuestionId = question.QuestionId;
								dateElement.ConditionTriggerId = question.ParentConditionTriggerId;

								dateElement.DateSelected += delegate
								{
									question.AnswerText = dateElement.DateValue.Value.ToShortDateString();
									//dateElement.Invalid = ValidateQuestion(question);
									//qSection.GetContainerTableView().ReloadData();
									//dateElement.ClosePickerIfOpen(this);
								};
								dateElement.PickerClosed += delegate
								{
									dateElement.Invalid = ValidateQuestion(question);
									qSection.GetContainerTableView().ReloadData();
								};

								qSection.Add(dateElement);
								//QuestionsView.UnevenRows = true;
								QuestionsView.Add(qSection);
								break;
							case "Height":
								var htidval = question.AnswerText;
								if (string.IsNullOrEmpty(SelectedAppointment.SelectedQForm.DateCompleted) && string.IsNullOrEmpty(question.AnswerText) && !string.IsNullOrEmpty(question.DefaultValue))
								{
									htidval = question.DefaultValue;
								}

								var seg = new DynaSegmented();
								seg.Frame = new CGRect(0, 0, View.Frame.Width, 30);
								for (int i = 1; i <= 12; i++)
								{
									seg.InsertSegment(i.ToString(), i - 1, true);
								}
								seg.ValueChanged += (sender, e) =>
								{
									var selectedSegmentId = (sender as UISegmentedControl).SelectedSegment;
									question.AnswerText = (sender as UISegmentedControl).TitleAt(selectedSegmentId);

								};
								seg.Required = question.IsRequired;
								seg.Invalid = question.IsInvalid;
								seg.Enabled = enabled;
								seg.IsEnabled = enabled;
								seg.QuestionId = question.QuestionId;
								seg.ConditionTriggerId = question.ParentConditionTriggerId;
								seg.parentSec = qSection;
                                if (!string.IsNullOrEmpty(htidval) && IsDigitsOnly(htidval))
								{
                                    seg.SelectedSegment = Convert.ToInt16(htidval) - 1;
								}
								qSection.Add(seg);
								QuestionsView.Add(qSection);
								break;
							case "Weight":
							case "Amount":
							case "Numeric":
							case "Slider":

								/*
								 *  TODO: options: UIStepper, Slider, Segmented Controls
								 * custom: migueldeicaza CounterElement
								 * components: BetterPickers, 
								 * have types: percent, currency, decimal, etc....
								*/

								float questionmin = 0;
								float questionmax = 20;
								float increment = 1;
								float qanswer = 0;

								switch (question.QuestionType)
								{
									case "Height":
										questionmax = 12;
										break;
									case "Weight":
										questionmax = 350;
										break;
								}

								if (!string.IsNullOrEmpty(question.MinValue))
								{
									questionmin = Convert.ToInt32(question.MinValue);
								}

								if (!string.IsNullOrEmpty(question.MaxValue))
								{
									questionmax = Convert.ToInt32(question.MaxValue);
								}

								if (!string.IsNullOrEmpty(question.Increment))
								{
									increment = Convert.ToInt32(question.Increment);
								}

								if (!string.IsNullOrEmpty(question.AnswerText))
								{
									qanswer = Convert.ToInt32(question.AnswerText);
								}
								else if (string.IsNullOrEmpty(SelectedAppointment.SelectedQForm.DateCompleted) && string.IsNullOrEmpty(question.AnswerText) && !string.IsNullOrEmpty(question.DefaultValue))
								{
									qanswer = Convert.ToInt32(question.DefaultValue);
								}
								else if (!string.IsNullOrEmpty(question.Increment) && !string.IsNullOrEmpty(question.MinValue))
								{
									qanswer = Convert.ToInt32(question.MinValue);
								}

								//var sliderElement = new DynaSlider(qanswer, question, (val) => SliderValueChanged(val, question));
								var sliderElement = new DynaSlider(qanswer, question);
								sliderElement.MinValue = questionmin;
								sliderElement.MaxValue = questionmax;
								sliderElement.Increment = increment;
								sliderElement.ShowCaption = true;
								sliderElement.Caption = qanswer.ToString();
								sliderElement.Enabled = enabled;
								sliderElement.Required = question.IsRequired;
								sliderElement.Invalid = question.IsInvalid;
								sliderElement.QuestionId = question.QuestionId;
								sliderElement.ConditionTriggerId = question.ParentConditionTriggerId;

								qSection.Add(sliderElement);
								//QuestionsView.UnevenRows = true;
								QuestionsView.Add(qSection);
								break;
							case "Grid":
								var columns = new List<ItemColumn>();// question.QuestionRowItem.ItemColumns;
								columns.Add(new ItemColumn() { Header = "first", Type = "Text", AnswerText = "dani" });
								columns.Add(new ItemColumn() { Header = "last", Type = "Text", AnswerText = "harel" });
								columns.Add(new ItemColumn() { Header = "ass", Type = "Switch", AnswerText = "true" });
								var rows = new List<QuestionRowItem>();
								rows.Add(new QuestionRowItem() { ItemColumns = columns });
								var viewModel = new ViewModel(columns, rows);//(question.QuestionRowItem.ItemColumns, question.ItemRows);

								var grid = new DynaGrid();

								grid.CellRenderers.Remove("TextView");
								grid.CellRenderers.Add("TextView", new GridCellTextViewRendererExt());

								grid.Frame = new CGRect(0, 0, View.Frame.Width, 190);
								grid.Required = question.IsRequired;
								grid.Invalid = question.IsInvalid;
								grid.IsEnabled = enabled;
								grid.QuestionId = question.QuestionId;
								grid.ConditionTriggerId = question.ParentConditionTriggerId;
								grid.parentSec = qSection;
								grid.AutoGenerateColumns = false;
								grid.ColumnSizer = ColumnSizer.LastColumnFill;
								grid.SelectionMode = SelectionMode.None;
								grid.AllowPullToRefresh = false;
								grid.AllowSorting = false;
								grid.AllowEditing = true;
								grid.EditTapAction = TapAction.OnTap;
								grid.EditorSelectionBehavior = EditorSelectionBehavior.SelectAll;

								grid.GridTapped += (object sender, GridTappedEventsArgs e) => { viewModel.RefreshGroup(columns[0].Header); };
								grid.ItemsSource = viewModel.DynamicCollection;
								grid.GridLoaded += (sender, e) => { grid.View.LiveDataUpdateMode = Syncfusion.Data.LiveDataUpdateMode.AllowDataShaping; };
								//grid.CurrentCellEndEdit += (object sender, GridCurrentCellEndEditEventArgs args) =>
								//{
								//	var a = sender;
								//	var b = args;
								//	DynamicModel c = (DynaPad.DynamicModel)grid.GetRecordAtRowIndex(args.RowColumnIndex.RowIndex);
								//	var d = grid.GetRecordAtRowIndex(args.RowColumnIndex.RowIndex);
								//	//var e = c.Values[grid.Columns[args.RowColumnIndex.ColumnIndex].MappingName];
								//	var n = viewModel.DynamicCollection[args.RowColumnIndex.RowIndex - 1].Values[grid.Columns[args.RowColumnIndex.ColumnIndex].MappingName.Substring(7, grid.Columns[args.RowColumnIndex.ColumnIndex].MappingName.Length - 8)].ToString();
								//	var newvalue = grid.GetCellValue(viewModel.DynamicCollection[args.RowColumnIndex.RowIndex - 1], grid.Columns[args.RowColumnIndex.ColumnIndex].MappingName).ToString();
								//	Debug.WriteLine(n);
								//	Debug.WriteLine(newvalue);
								//};
								grid.CurrentCellEndEdit += async (object sender, GridCurrentCellEndEditEventArgs args) =>
								{
									await Task.Delay(100);
									var a = sender;
									var b = args;
									DynamicModel c = (DynamicModel)grid.GetRecordAtRowIndex(args.RowColumnIndex.RowIndex);
									var d = grid.GetRecordAtRowIndex(args.RowColumnIndex.RowIndex);
									//var e = c.Values[grid.Columns[args.RowColumnIndex.ColumnIndex].MappingName];
									var n = viewModel.DynamicCollection[args.RowColumnIndex.RowIndex - 1].Values[grid.Columns[args.RowColumnIndex.ColumnIndex].MappingName.Substring(7, grid.Columns[args.RowColumnIndex.ColumnIndex].MappingName.Length - 8)].ToString();
									var newvalue = grid.GetCellValue(viewModel.DynamicCollection[args.RowColumnIndex.RowIndex - 1], grid.Columns[args.RowColumnIndex.ColumnIndex].MappingName).ToString();
									Debug.WriteLine(n);
									Debug.WriteLine(newvalue);
								};
								//grid.CurrentCellEndEdit += Grid_CurrentCellEndEdit;

								foreach (var c in columns)
								{
									switch (c.Type)
									{
										case "Text":
											var cText = new GridTextColumn();
											cText.MappingName = "Values[" + c.Header.Replace(" ", "") + "]";
											cText.HeaderText = c.Header;
											cText.AllowEditing = true;
											cText.HeaderTextAlignment = UITextAlignment.Left;
											cText.TextAlignment = UITextAlignment.Left;
											cText.LineBreakMode = UILineBreakMode.WordWrap;
											grid.Columns.Add(cText);
											break;
										case "Switch":
											var cSwitch = new GridSwitchColumn();
											cSwitch.MappingName = "Values[" + c.Header.Replace(" ", "") + "]";
											cSwitch.HeaderText = c.Header;
											cSwitch.AllowEditing = true;
											cSwitch.HeaderTextAlignment = UITextAlignment.Left;
											cSwitch.TextAlignment = UITextAlignment.Left;
											grid.Columns.Add(cSwitch);
											break;
										case "Numeric":
											var cNumeric = new GridNumericColumn();
											cNumeric.MappingName = "Values[" + c.Header.Replace(" ", "") + "]";
											cNumeric.HeaderText = c.Header;
											cNumeric.AllowEditing = true;
											cNumeric.HeaderTextAlignment = UITextAlignment.Left;
											cNumeric.TextAlignment = UITextAlignment.Left;
											grid.Columns.Add(cNumeric);
											break;
										case "Date":
											var cDate = new GridDateTimeColumn();
											cDate.MappingName = "Values[" + c.Header.Replace(" ", "") + "]";
											cDate.HeaderText = c.Header;
											cDate.AllowEditing = true;
											cDate.HeaderTextAlignment = UITextAlignment.Left;
											cDate.TextAlignment = UITextAlignment.Left;
											grid.Columns.Add(cDate);
											break;
										case "Picker":
											var cPicker = new GridPickerColumn();
											cPicker.MappingName = "Values[" + c.Header.Replace(" ", "") + "]";
											cPicker.HeaderText = c.Header;
											ObservableCollection<string> coptions = new ObservableCollection<string>();
											foreach (var o in c.Options)
											{
												coptions.Add(o);
											}
											cPicker.AllowEditing = true;
											cPicker.HeaderTextAlignment = UITextAlignment.Left;
											grid.Columns.Add(cPicker);
											break;
									}
								}

								//Dummy column need to be added since we have internally checked grouping column against grid column collection.
								grid.Columns.Add(new GridTextColumn()
								{
									HeaderText = "GroupProperty",
									MappingName = "GroupProperty",
									Width = 0
								});
								//Need to refresh the GroupProperty of DynamicModel in the given collection before applying groupdescription to the grid.
								//viewModel.RefreshGroup(columns[0].Header);
								//grid.GroupColumnDescriptions.Add(new GroupColumnDescription() { ColumnName = "GroupProperty" });

								var addRowButton = new UIButton(new CGRect(0, 0, View.Frame.Width, 30));
								addRowButton.SetTitle("Add Row", UIControlState.Normal);
								addRowButton.SetTitleColor(UIColor.Black, UIControlState.Normal);
								addRowButton.BackgroundColor = UIColor.LightGray;
								addRowButton.TouchUpInside += (sender, e) =>
								{
									viewModel.DynamicCollection.Add(new DynamicModel() { Values = viewModel.GetDynamicModel(columns).Values });
								};

								qSection.Add(grid);
								qSection.Add(addRowButton);
								QuestionsView.Add(qSection);
								break;
						}
						//question.ScrollY = qSection.HeaderView.Frame.Y;
					}

					var qNext = new DynaSection("Next");
					qNext.HeaderView = new UIView(new CGRect(0, 0, 0, 10));
					qNext.FooterView = new UIView(new CGRect(0, 0, 0, 10));
					qNext.Add(nextbtn);

					QuestionsView.UnevenRows = true;

					QuestionsView.Add(qNext);
					Root.TableView.AutosizesSubviews = true;
					Root = QuestionsView;
					Root.TableView.ScrollEnabled = true;
					if (sectionQuestions.Revalidating && (sectionQuestions.RevalidatingRow > 0 && sectionQuestions.RevalidatingRow < Root.TableView.IndexPathsForVisibleRows.Length - 1))
					{
						var firstinvalidrow = Root.TableView.IndexPathsForVisibleRows[sectionQuestions.RevalidatingRow];
						Root.TableView.ScrollsToTop = false;
						Root.TableView.ScrollToRow(firstinvalidrow, UITableViewScrollPosition.Top, true);
						//Root.TableView.ScrollRectToVisible(new CGRect(0, sectionQuestions.RevalidatingY, 1, 1), true);
					}
					else
					{
						Root.TableView.ScrollsToTop = true;
						Root.TableView.ScrollRectToVisible(new CGRect(0, 0, 1, 1), true);
					}
				}
			}
			catch (Exception ex)
			{
				Root.Clear();
				Root.Add(CommonFunctions.ErrorDetailSection());
				ReloadData();
				CommonFunctions.sendErrorEmail(ex);
                PresentViewController(CommonFunctions.ExceptionAlertPrompt(), true, null);
				//throw new Exception(ex.Message + Environment.NewLine + ex.StackTrace, ex.InnerException);
			}
		}




		bool IsDigitsOnly(string str)
		{
			foreach (char c in str)
			{
				if (c < '0' || c > '9')
					return false;
			}

			return true;
		}




		async void Grid_CurrentCellEndEdit(object sender, GridCurrentCellEndEditEventArgs args)
		{
			await Task.Delay(100);
			var grid = (DynaGrid)sender;
			ObservableCollection<DynamicModel> viewModel_dynamicCollection = (ObservableCollection<DynamicModel>)grid.ItemsSource;
			var n = viewModel_dynamicCollection[args.RowColumnIndex.RowIndex - 1].Values[grid.Columns[args.RowColumnIndex.ColumnIndex].MappingName.Substring(7, grid.Columns[args.RowColumnIndex.ColumnIndex].MappingName.Length - 8)].ToString();
			var newvalue = grid.GetCellValue(viewModel_dynamicCollection[args.RowColumnIndex.RowIndex - 1], grid.Columns[args.RowColumnIndex.ColumnIndex].MappingName).ToString();
			System.Console.Write(newvalue);
		}




		public bool URLExists(string url)
		{
			bool result = true;

			WebRequest webRequest = WebRequest.Create(url);
			webRequest.Timeout = 1200; // miliseconds
			webRequest.Method = "HEAD";

			try
			{
				webRequest.GetResponse();
			}
			catch
			{
				result = false;
			}

			return result;
		}




		public class SFControlAutoCompleteDelegate : AutoCompleteDelegate
		{
			public SectionQuestion question;
			public DynaSection qSection;
			public SFControlAutoCompleteDelegate(SectionQuestion inq, DynaSection ds)
			{
				question = inq;
				qSection = ds;
			}
			public override void DidSelectionChange(SFAutoComplete SFAutoComplete, NSObject value)
			{
				question.AnswerText = value.ValueForKey((NSString)"Value").ToString();
				//var truequestion = SelectedAppointment.SelectedQForm.FormSections[0].SectionQuestions.Find((SectionQuestion obj) => obj.QuestionId == question.QuestionId);
				//truequestion.AnswerText = value.ToString();
				(SFAutoComplete as DynaAuto).Invalid = ValidateAuto(question);
				if (qSection.GetContainerTableView() != null)
				{
					qSection.GetContainerTableView().ReloadData();
				}
			}
			public override void DidTextChange(SFAutoComplete SFAutoComplete, string value)
			{
				question.AnswerText = value;
				(SFAutoComplete as DynaAuto).Invalid = ValidateAuto(question);
				//if (string.IsNullOrEmpty(value) && qSection.GetContainerTableView() != null)
				//{
				//	question.AnswerText = "";
				//	(SFAutoComplete as DynaAuto).Text = "";
				//	qSection.GetContainerTableView().ReloadData();
				//}
			}
			public bool ValidateAuto(SectionQuestion question)
			{
				try
				{
					var valid = true;
					if (question.IsRequired && question.IsEnabled)
					{
						if (string.IsNullOrEmpty(question.AnswerText))
						{
							valid = false;
							question.IsInvalid = true;
						}
						else question.IsInvalid = false;
					}
					return !valid;
				}
				catch (Exception ex)
				{
                    CommonFunctions.sendErrorEmail(ex);
                    //PresentViewController(CommonFunctions.ExceptionAlertPrompt(), true, null);
                    return false;
					//throw new Exception(ex.Message + Environment.NewLine + ex.StackTrace, ex.InnerException);
				}
			}
		}

		//public class AutoData
		//{
		//	string Text;//Name;
		//	string Value;//Age;
		//	public AutoData(string Text, string Value)
		//	{
		//		this.Text = Text;
		//		this.Value = Value;

		//	}
		//	public string getText()
		//	{
		//		return Text;
		//	}
		//	public string getValue()
		//	{
		//		return Value;
		//	}
		//}

		public NSMutableArray AutoDetails
		{
			get;
			set;
		}

		void GetAutoData(string qid)
		{
			NSMutableArray array = new NSMutableArray();
			//array.Add(getDictionary("John", "24"));
			//array.Add(getDictionary("James", "37"));

			var listurl = DynaClassLibrary.DynaClasses.LoginContainer.User.DynaConfig.DomainRootPathVirtual + "DynaForms/" + SelectedAppointment.ApptFormId + "/AutoBoxLists/" + qid + ".txt";
			if (URLExists(listurl))
			{
				XmlDocument listxml = new XmlDocument();
				listxml.Load(listurl);
				XmlElement root = listxml.DocumentElement;
				XmlNodeList nodes = root.SelectNodes("/Items/Item");

				foreach (XmlNode node in nodes)
				{
					array.Add(getDictionary(node.Attributes[0].Value, node.Attributes[1].Value));
				}
			}

			AutoDetails = array;
		}

		NSDictionary getDictionary(string text, string value)
		{

			object[] objects = new object[2];
			object[] keys = new object[2];
			keys.SetValue("Text", 0);
			keys.SetValue("Value", 1);
			objects.SetValue((NSString)text, 0);
			objects.SetValue((NSString)value, 1);
			return NSDictionary.FromObjectsAndKeys(objects, keys);
		}




		public TextToSpeech tts;
		void ExecuteSpeechCommand(string speechText)
		{
			if (tts != null && tts.IsSpeaking)
			{
				tts.StopSpeach();
				//Title = "Start speech";
			}
			else
			{
				tts = new TextToSpeech();
				//Title = "Stop Speech";
				tts.Speak(speechText);
			}
		}



		public bool ValidateQuestion(SectionQuestion question)
		{
			try
			{
				var valid = true;

				if (question.IsRequired && question.IsEnabled)
				{
					switch (question.QuestionType)
					{
						case "BodyParts":
						case "Check":
							foreach (QuestionOption opt in question.QuestionOptions)
							{
								if (opt.Chosen)
								{
									question.IsInvalid = false;
									break;
								}
								question.IsInvalid = true;
							}
							//if (question.IsInvalid) valid = false;
							valid &= !question.IsInvalid;
							break;
						case "Radio":
						case "Bool":
						case "YesNo":
							foreach (QuestionOption opt in question.QuestionOptions)
							{
								if (opt.Chosen)
								{
									question.IsInvalid = false;
									break;
								}
								question.IsInvalid = true;
							}
							//if (question.IsInvalid) valid = false;
							valid &= !question.IsInvalid;
							break;
						case "TextView":
							if (string.IsNullOrEmpty(question.AnswerText))
							{
								valid = false;
								question.IsInvalid = true;
							}
							else question.IsInvalid = false;
							break;
						case "TextInput":
							if (string.IsNullOrEmpty(question.AnswerText))
							{
								valid = false;
								question.IsInvalid = true;
							}
							else question.IsInvalid = false;
							break;
						case "Date":
							if (string.IsNullOrEmpty(question.AnswerText))
							{
								valid = false;
								question.IsInvalid = true;
							}
							else question.IsInvalid = false;
							break;
						case "Height":
						case "Weight":
						case "Amount":
						case "Numeric":
						case "Slider":
							if (string.IsNullOrEmpty(question.AnswerText))
							{
								valid = false;
								question.IsInvalid = true;
							}
							else question.IsInvalid = false;
							break;
						case "Grid":
							foreach (QuestionRowItem row in question.ItemRows)
							{
								foreach (ItemColumn col in row.ItemColumns)
								{
									if (col.Required)
									{
										if (string.IsNullOrEmpty(col.AnswerText))
										{
											question.IsInvalid = false;
											break;
										}
										question.IsInvalid = true;
									}
								}
							}
							valid &= !question.IsInvalid;
							break;
						default:
							if (string.IsNullOrEmpty(question.AnswerText))
							{
								valid = false;
								question.IsInvalid = true;
							}
							else question.IsInvalid = false;
							break;
					}
				}

				return !valid;
			}
			catch (Exception ex)
			{
				CommonFunctions.sendErrorEmail(ex);
                PresentViewController(CommonFunctions.ExceptionAlertPrompt(), true, null);
                return false;
				//throw new Exception(ex.Message + Environment.NewLine + ex.StackTrace, ex.InnerException);
			}
		}




		public static bool IsCameraAuthorized()
		{
			var authStatus = AVCaptureDevice.GetAuthorizationStatus(AVMediaType.Video);
			switch (authStatus)
			{
				case AVAuthorizationStatus.Authorized:
					// do your logic
					return true;
				case AVAuthorizationStatus.Denied:
					// denied
					return false;
				case AVAuthorizationStatus.Restricted:
					// restricted, normally won't happen
					return false;
				case AVAuthorizationStatus.NotDetermined:
					// not determined?!
					return false;
				default:
					return false;
			}
		}

		public UIBarButtonItem GetPhotoNavBtn(string sectionId, string sectionName, string patientName, string apptId, string patientId, string doctorId, string locationId, bool IsDoctorForm)
		{
			try
			{
				var cameraButton = new UIBarButtonItem(UIBarButtonSystemItem.Camera, (sender, args) =>
				{
					var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
					var directoryname = Path.Combine(documents, "DynaPhotos");
					//var d = new DirectoryInfo(directoryname);

					if (!Directory.Exists(directoryname))
					{
						Directory.CreateDirectory(directoryname);
					}

					//var adocuments = NSFileManager.DefaultManager.GetUrls(NSSearchPathDirectory.DocumentDirectory, NSSearchPathDomain.User)[0].Path;
					//var tmp = Path.Combine(adocuments, "../", "tmp");
					//var trytmp = Path.GetTempPath();

					//var rdocuments = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
					//var ret = Path.Combine(rdocuments, "..", "tmp");

					var picker = new MediaPicker();
					var controller = picker.GetTakePhotoUI(new StoreCameraMediaOptions
					{
						Name = patientName.Replace(" ", "_") + "_" + sectionName + "_" + DateTime.Now.ToString("s").Replace(":", "_") + ".jpg",
						Directory = "MediaPickerSample",
						DefaultCamera = CameraDevice.Rear
					});
					controller.CameraCaptureMode = UIImagePickerControllerCameraCaptureMode.Photo;

					// On iPad, you'll use UIPopoverController to present the controller
					//PresentViewController(controller, true, null);
					NavigationController.PresentViewController(controller, true, null);

					controller.GetResultAsync().ContinueWith(t =>
					{
						// Dismiss the UI yourself
						controller.DismissViewController(true, () =>
							{
								if (!t.IsCanceled)
								{
									MediaFile file = t.Result;
									var testByte = ByteArrayFromStream(file.Path);
									var ass = testByte.Length;
									var filename = patientName.Replace(" ", "_") + "_" + sectionName + "_" + DateTime.Now.ToString("s").Replace(":", "_") + ".jpg";

									//var dps = new DynaPadService.DynaPadService();
									//var savefile = dps.SaveFile(apptId, patientId, doctorId, locationId, filename, "DynaPad", testByte, IsDoctorForm, false);

									//if (CrossConnectivity.Current.IsConnected)
									//{

										var bw = new BackgroundWorker();

										// this allows our worker to report progress during work
										bw.WorkerReportsProgress = true;

										// what to do in the background thread
										bw.DoWork += delegate (object o, DoWorkEventArgs argss)
										{
											var b = o as BackgroundWorker;

											var dps = new DynaPadService.DynaPadService();
											var savefile = dps.SaveFile(CommonFunctions.GetUserConfig(), apptId, patientId, doctorId, locationId, filename, "DynaPad Photo", "DynaPad", "", "", testByte, IsDoctorForm, false);
										};

										// what to do when worker completes its task (notify the user)
										bw.RunWorkerCompleted += delegate
										{
											PresentViewController(CommonFunctions.AlertPrompt("Photo Saved", "A new photo has been saved to medical records", true, null, false, null), true, null);
										};

										bw.RunWorkerAsync();
									//}
									//else
									//{
									//	PresentViewController(CommonFunctions.InternetAlertPrompt(), true, null);
									//}
								}
							});

					}, TaskScheduler.FromCurrentSynchronizationContext());
				});

				var camauth = IsCameraAuthorized();

				if (!UIImagePickerController.IsSourceTypeAvailable(UIImagePickerControllerSourceType.Camera) && !camauth)
				{
					cameraButton.Enabled = false;
				}

				return cameraButton;
			}
			catch (Exception ex)
			{
				CommonFunctions.sendErrorEmail(ex);
                PresentViewController(CommonFunctions.ExceptionAlertPrompt(), true, null);
                return null;
				//throw new Exception(ex.Message + Environment.NewLine + ex.StackTrace, ex.InnerException);
			}
		}






		public byte[] ByteArrayFromStream(string path)
		{
			try
			{
				var originalImage = UIImage.FromFile(path);
				return originalImage.AsJPEG(0.5f).ToArray();
			}
			catch (Exception ex)
			{
				CommonFunctions.sendErrorEmail(ex);
                PresentViewController(CommonFunctions.ExceptionAlertPrompt(), true, null);
                return null;
				//throw new Exception(ex.Message + Environment.NewLine + ex.StackTrace, ex.InnerException);
			}
		}

		public UIBarButtonItem GetDrawNavBtn(string sectionId)
		{
			try
			{
				var navdraw = new UIBarButtonItem(UIImage.FromBundle("Writing"), UIBarButtonItemStyle.Plain, (sender, args) =>
				{
					var nlab = new UILabel(new CGRect(10, 0, View.Bounds.Width - 60, 50));
					//nlab.Text = "NOTES: (" + SelectedAppointment.SelectedQForm.FormName + ")";
					nlab.Text = "NOTES:";

					var ncellHeader = new UITableViewCell(UITableViewCellStyle.Default, null);
					ncellHeader.Frame = new CGRect(0, 0, View.Bounds.Width, 50);

					var nheadclosebtn = new UIButton(new CGRect(View.Bounds.Width - 50, 0, 50, 50));
					nheadclosebtn.SetImage(UIImage.FromBundle("Close"), UIControlState.Normal);

					ncellHeader.ContentView.Add(nlab);
					ncellHeader.ContentView.Add(nheadclosebtn);

					var nsec = new Section(ncellHeader);
					nsec.FooterView = new UIView(new CGRect(0, 0, 0, 0));
					nsec.FooterView.Hidden = true;

					//var dcanvas = new CanvasMainViewController { MREditing = false };
					//nsec.Add(dcanvas.View);
					var dcanvas = new FingerPaintViewController { MREditing = false };
					nsec.Add(dcanvas.View);

					//CanvasContainerView notesCanvas = CanvasContainerView.FromCanvasSize(new CGSize(800, 800));
					//nsec.Add(notesCanvas);



					//var sigPad = new SignaturePad.SignaturePadView(new CGRect(0, 0, 400, 400));
					//sigPad.CaptionText = "Signature here:";
					//sigPad.BackgroundColor = UIColor.White;

					//var img = UIImage.FromFile("dynapadscreenshot.png");
					//var imgView = new UIImageView(sigPad.Bounds);
					//imgView.Image = img;
					//imgView.ContentMode = UIViewContentMode.ScaleAspectFit; //
					//sigPad.BackgroundImage = imgView.Image;
					//nsec.Add(sigPad);


					var nroo = new RootElement("Notes");
					nroo.Add(nsec);

					var ndia = new DialogViewController(nroo);
					ndia.ModalInPopover = true;
					ndia.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
					//ndia.PreferredContentSize = new CGSize(View.Bounds.Size);

					//var sig = new Intersoft.Crosslight.UI.iOS.UISignaturePadView(View.Bounds);

					nheadclosebtn.TouchUpInside += delegate
						{
							NavigationController.DismissViewController(true, null);
						};

					NavigationController.PreferredContentSize = new CGSize(View.Bounds.Size);
					//NavigationController.View.BackgroundColor = UIColor.Clear;
					NavigationController.PresentViewController(dcanvas, true, null);
					NavigationController.View.SizeToFit();

					//viewController = new xamarin_sampleViewController();
					//viewController.View.SizeToFit();

					//var nnsec = new Section(ncellHeader);
					//nnsec.FooterView = new UIView(new CGRect(0, 0, 0, 0));
					//nnsec.FooterView.Hidden = true;

					//nnsec.Add(viewController.View);

					//var nnroo = new RootElement("Notes");
					//nnroo.Add(nnsec);

					//var nndia = new DialogViewController(nnroo);
					//nndia.ModalInPopover = true;
					//nndia.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
					//nndia.View.Frame = new CGRect(0, 0, 600, 600);
					////nndia.PreferredContentSize = new CGSize(View.Bounds.Size);
					//NavigationController.PresentViewController(viewController, true, null);
					//NavigationController.View.SizeToFit();

				});

				return navdraw;
			}
			catch (Exception ex)
			{
				CommonFunctions.sendErrorEmail(ex);
                PresentViewController(CommonFunctions.ExceptionAlertPrompt(), true, null);
                return null;
				//throw new Exception(ex.Message + Environment.NewLine + ex.StackTrace, ex.InnerException);
			}
		}


		public UIBarButtonItem GetMRNavBtn()
		{
			try
			{
				var navmr = new UIBarButtonItem(UIBarButtonSystemItem.Organize, (sender, args) =>
				{
					var nlab = new UILabel(new CGRect(10, 0, View.Bounds.Width - 60, 50));
					//nlab.Text = "NOTES: (" + SelectedAppointment.SelectedQForm.FormName + ")";
					nlab.Text = "Medical Records: " + SelectedAppointment.ApptPatientName;

					var ncellHeader = new UITableViewCell(UITableViewCellStyle.Default, null);
					ncellHeader.Frame = new CGRect(0, 0, View.Bounds.Width, 50);

					var nheadclosebtn = new UIButton(new CGRect(View.Bounds.Width - 50, 0, 50, 50));
					nheadclosebtn.SetImage(UIImage.FromBundle("Close"), UIControlState.Normal);

					ncellHeader.ContentView.Add(nlab);
					ncellHeader.ContentView.Add(nheadclosebtn);

					var nsec = new Section(ncellHeader);
					nsec.FooterView = new UIView(new CGRect(0, 0, 0, 0));
					nsec.FooterView.Hidden = true;


                    var dds = new DynaPadService.DynaPadService() { Timeout = 60000 };
					var origJson = dds.GetFiles(CommonFunctions.GetUserConfig(), SelectedAppointment.ApptId, SelectedAppointment.ApptPatientId, SelectedAppointment.ApptPatientName, SelectedAppointment.ApptDoctorId, SelectedAppointment.ApptLocationId);
					JsonHandler.OriginalFormJsonString = origJson;
					SelectedAppointment.ApptMRFolders = JsonConvert.DeserializeObject<List<MRFolder>>(origJson);

					var mrs = new List<MR>();
					foreach (MRFolder mrf in SelectedAppointment.ApptMRFolders)
					{
						foreach (MR m in mrf.MrFolderMRs)
						{
							m.MRFolderName = mrf.MRFolderName;
							m.IsShortcut = true;
						}
						mrs.AddRange(SelectedAppointment.ApptMRFolders.Find(mr => mr.MRFolderId == mrf.MRFolderId).MrFolderMRs);
					}

					var fgrid = new SfDataGrid();
					fgrid.Frame = new CGRect(0, 0, View.Bounds.Width, View.Bounds.Height);
					fgrid.ItemsSource = mrs;
					fgrid.GridDoubleTapped += DataGrid_GridDoubleTapped;
					fgrid.AutoGenerateColumns = false;
					fgrid.ColumnSizer = ColumnSizer.None;
					fgrid.SelectionMode = SelectionMode.SingleDeselect;
					//fgrid.AllowPullToRefresh = true;
					//fgrid.PullToRefreshCommand = new GridFormCommand(ExecutePullToRefreshFormCommand, fgrid);
					//fgrid.AllowSorting = true;
					//var ass = new Syncfusion.SfDataGrid.GroupColumnDescription();

					var mrFolderColumn = new GridTextColumn();
					mrFolderColumn.MappingName = "MRFolderName";
					mrFolderColumn.HeaderText = "Folder";
					//mrFolderColumn.Width = fgrid.Frame.Width * 0.55;
					//mrFolderColumn.HeaderTextAlignment = UITextAlignment.Left;
					//mrFolderColumn.TextAlignment = UITextAlignment.Left;

					var mrNameColumn = new GridTextColumn();
					mrNameColumn.MappingName = "MRName";
					mrNameColumn.HeaderText = " File Name";
					mrNameColumn.Width = fgrid.Frame.Width * 0.55;
					//mrNameColumn.MinimumWidth = fgrid.Frame.Width * 0.40;
					mrNameColumn.HeaderTextAlignment = UITextAlignment.Left;
					mrNameColumn.TextAlignment = UITextAlignment.Left;

					var mrDateColumn = new GridTextColumn();
					mrDateColumn.MappingName = "MRApptDate";
					mrDateColumn.HeaderText = "Appt Date";
					mrDateColumn.Width = fgrid.Frame.Width * 0.15;
					//mrDateColumn.MinimumWidth = fgrid.Frame.Width * 0.20;
					mrDateColumn.HeaderTextAlignment = UITextAlignment.Left;
					mrDateColumn.TextAlignment = UITextAlignment.Left;

					var mrDoctorColumn = new GridTextColumn();
					mrDoctorColumn.MappingName = "MRDoctor";
					mrDoctorColumn.HeaderText = "Doctor";
					mrDoctorColumn.Width = fgrid.Frame.Width * 0.15;
					//mrDoctorColumn.MinimumWidth = fgrid.Frame.Width * 0.20;
					mrDoctorColumn.HeaderTextAlignment = UITextAlignment.Left;
					mrDoctorColumn.TextAlignment = UITextAlignment.Left;

					var mrLocationColumn = new GridTextColumn();
					mrLocationColumn.MappingName = "MRLocation";
					mrLocationColumn.HeaderText = "Location";
					mrLocationColumn.Width = fgrid.Frame.Width * 0.15;
					//mrLocationColumn.MinimumWidth = fgrid.Frame.Width * 0.20;
					mrLocationColumn.HeaderTextAlignment = UITextAlignment.Left;
					mrLocationColumn.TextAlignment = UITextAlignment.Left;

					var mrFileTypeColumn = new GridTextColumn();
					mrFileTypeColumn.MappingName = "MRFileType";
					mrFileTypeColumn.HeaderText = "File Type";
					//mrFileTypeColumn.Width = fgrid.Frame.Width * 0.15;
					mrFileTypeColumn.HeaderTextAlignment = UITextAlignment.Left;
					mrFileTypeColumn.TextAlignment = UITextAlignment.Left;
					mrFileTypeColumn.IsHidden = true;

					fgrid.Columns.Add(mrFolderColumn);
					fgrid.Columns.Add(mrNameColumn);
					fgrid.Columns.Add(mrDateColumn);
					fgrid.Columns.Add(mrDoctorColumn);
					fgrid.Columns.Add(mrLocationColumn);
                    fgrid.Columns.Add(mrFileTypeColumn);

					fgrid.GroupColumnDescriptions.Add(new GroupColumnDescription { ColumnName = "MRFolderName" });
					fgrid.AllowGroupExpandCollapse = true;

					nsec.Add(fgrid);

					var nroo = new RootElement("MR");
					nroo.Add(nsec);

					var ndia = new DialogViewController(nroo);
					ndia.ModalInPopover = true;
					ndia.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
					ndia.PreferredContentSize = new CGSize(View.Bounds.Size);

					nheadclosebtn.TouchUpInside += delegate
						{
							NavigationController.DismissViewController(true, null);
						};

					NavigationController.PreferredContentSize = new CGSize(View.Bounds.Size);
					//NavigationController.View.BackgroundColor = UIColor.Clear;
					NavigationController.PresentViewController(ndia, true, null);
					//NavigationController.View.SizeToFit();

				});

				return navmr;
			}
			catch (Exception ex)
			{
				CommonFunctions.sendErrorEmail(ex);
                PresentViewController(CommonFunctions.ExceptionAlertPrompt(), true, null);
                return null;
				//throw new Exception(ex.Message + Environment.NewLine + ex.StackTrace, ex.InnerException);
			}
		}



		public UIBarButtonItem GetDynaDrawNavBtn(string sectionId)
		{
			try
			{
				var navdraw = new UIBarButtonItem(UIImage.FromBundle("Writing"), UIBarButtonItemStyle.Plain, (sender, args) =>
				{
					var nlab = new UILabel(new CGRect(10, 0, View.Bounds.Width - 60, 50));
					//nlab.Text = "NOTES: (" + SelectedAppointment.SelectedQForm.FormName + ")";
					nlab.Text = "NOTES:";

					var ncellHeader = new UITableViewCell(UITableViewCellStyle.Default, null);
					ncellHeader.Frame = new CGRect(0, 0, View.Bounds.Width, 50);

					var nheadclosebtn = new UIButton(new CGRect(View.Bounds.Width - 50, 0, 50, 50));
					nheadclosebtn.SetImage(UIImage.FromBundle("Close"), UIControlState.Normal);

					ncellHeader.ContentView.Add(nlab);
					ncellHeader.ContentView.Add(nheadclosebtn);

					var nsec = new Section(ncellHeader);
					nsec.FooterView = new UIView(new CGRect(0, 0, 0, 0));
					nsec.FooterView.Hidden = true;

					var img = UIImage.FromFile("dynapadscreenshot.png");
					var imgView = new UIImageView(View.Bounds);
					imgView.Image = img;
					imgView.ContentMode = UIViewContentMode.ScaleAspectFit; // or ScaleAspectFill

					var dcanvas = new DynaPadView(new CGRect(0, 0, 500, 500));
					dcanvas.BackgroundImage = imgView.Image;
					nsec.Add(dcanvas);
					//CanvasContainerView notesCanvas = CanvasContainerView.FromCanvasSize(new CGSize(800, 800));
					//nsec.Add(notesCanvas);
					var ass = new UIViewController();
					ass.Add(dcanvas);

					var nroo = new RootElement("Notes");
					nroo.Add(nsec);

					var ndia = new DialogViewController(nroo);
					ndia.ModalInPopover = true;
					ndia.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
					//ndia.PreferredContentSize = new CGSize(View.Bounds.Size);

					nheadclosebtn.TouchUpInside += delegate
						{
							NavigationController.DismissViewController(true, null);
						};

					NavigationController.PreferredContentSize = new CGSize(View.Bounds.Size);
					//NavigationController.View.BackgroundColor = UIColor.Clear;
					NavigationController.PresentViewController(ass, true, null);
					NavigationController.View.SizeToFit();

					//viewController = new xamarin_sampleViewController();
					//viewController.View.SizeToFit();

					//var nnsec = new Section(ncellHeader);
					//nnsec.FooterView = new UIView(new CGRect(0, 0, 0, 0));
					//nnsec.FooterView.Hidden = true;

					//nnsec.Add(viewController.View);

					//var nnroo = new RootElement("Notes");
					//nnroo.Add(nnsec);

					//var nndia = new DialogViewController(nnroo);
					//nndia.ModalInPopover = true;
					//nndia.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
					//nndia.View.Frame = new CGRect(0, 0, 600, 600);
					////nndia.PreferredContentSize = new CGSize(View.Bounds.Size);
					//NavigationController.PresentViewController(viewController, true, null);
					//NavigationController.View.SizeToFit();

				});

				return navdraw;
			}
			catch (Exception ex)
			{
				CommonFunctions.sendErrorEmail(ex);
                PresentViewController(CommonFunctions.ExceptionAlertPrompt(), true, null);
                return null;
				//throw new Exception(ex.Message + Environment.NewLine + ex.StackTrace, ex.InnerException);
			}
		}


		public UIBarButtonItem GetDicNavBtn(string sectionId, bool IsDoctorForm)
		{
			try
			{
				var navdic = new UIBarButtonItem(UIImage.FromBundle("Dictation"), UIBarButtonItemStyle.Plain, (sender, args) =>
				{
					audioFilePath = null;

					CancelRecording.Frame = new CGRect(0, 0, 350, 50);
					CancelRecording.SetTitle("Close", UIControlState.Normal);
					CancelRecording.SetTitleColor(UIColor.Black, UIControlState.Normal);
					//CancelRecording.TouchUpInside += OnCancelRecording;

					var clab = new UILabel(new CGRect(10, 0, 290, 50));
					//clab.TextAlignment = UITextAlignment.Center;
					clab.Text = "DICTATION(S):";
					//clab.Font = UIFont.BoldSystemFontOfSize(17);

					//var segDict = new UISegmentedControl();
					//segDict.Frame = new CGRect(0, 0, 350, 50);
					//segDict.Momentary = true;
					//segDict.InsertSegment(UIImage.FromBundle("Delete"), 0, true);
					//segDict.InsertSegment("Dictation", 1, true);
					//segDict.SetWidth(50, 0);
					//segDict.SetWidth(324, 1);

					var cellHeader = new UITableViewCell(UITableViewCellStyle.Default, null);
					cellHeader.Frame = new CGRect(0, 0, 350, 50);
					//cellHeader.ImageView.Image = UIImage.FromBundle("Close");

					var headclosebtn = new UIButton(new CGRect(300, 0, 50, 50));
					headclosebtn.SetImage(UIImage.FromBundle("Close"), UIControlState.Normal);

					cellHeader.ContentView.Add(clab);
					cellHeader.ContentView.Add(headclosebtn);

					var cellFooter = new UITableViewCell(UITableViewCellStyle.Default, null);
					cellFooter.Frame = new CGRect(0, 0, 350, 50);
					//cellHeader.ImageView.Image = UIImage.FromBundle("Close");
					cellFooter.ContentView.Add(CancelRecording);


					var sec = new Section(cellHeader, cellFooter);
					sec.FooterView.Frame = new CGRect(0, 0, 350, 50);

					RecordingStatusLabel.Text = string.Empty;
					RecordingStatusLabel.Frame = new CGRect(210, 0, 120, 50);

					LengthOfRecordingLabel.Text = string.Empty;
					LengthOfRecordingLabel.Frame = new CGRect(210, 0, 120, 50);

					StartRecordingButton.Frame = new CGRect(20, 0, 160, 50);
					//StartRecordingButton.SetImage(UIImage.FromBundle("Record"), UIControlState.Normal);
					StartRecordingButton.TouchUpInside += OnStartRecording;
					StartRecordingButton.SetTitle("Start Recording", UIControlState.Normal);
					StartRecordingButton.SetTitleColor(UIColor.FromRGB(45, 137, 221), UIControlState.Normal);

					StopRecordingButton.Frame = new CGRect(20, 0, 160, 50);
					//StopRecordingButton.SetImage(UIImage.FromBundle("Stop"), UIControlState.Normal);
					StopRecordingButton.SetTitle("Stop Recording", UIControlState.Normal);
					StopRecordingButton.SetTitleColor(UIColor.FromRGB(45, 137, 221), UIControlState.Normal);
					StopRecordingButton.TouchUpInside += OnStopRecording;
					StopRecordingButton.Enabled = false;
					StopRecordingButton.Alpha = (nfloat)0.5;

					PlayRecordedSoundButton.Frame = new CGRect(20, 0, 160, 50);
					//PlayRecordedSoundButton.SetImage(UIImage.FromBundle("Play"), UIControlState.Normal);
					PlayRecordedSoundButton.SetTitle("Play Recording", UIControlState.Normal);
					PlayRecordedSoundButton.SetTitleColor(UIColor.FromRGB(45, 137, 221), UIControlState.Normal);
					PlayRecordedSoundButton.TouchUpInside += OnPlayRecordedSound;
					PlayRecordedSoundButton.Enabled = false;
					PlayRecordedSoundButton.Alpha = (nfloat)0.5;

					SaveRecordedSound.Enabled = false;
					SaveRecordedSound.Alpha = (nfloat)0.5;
					SaveRecordedSound.Frame = new CGRect(20, 0, 160, 50);
					//SaveRecordedSound.SetImage(UIImage.FromBundle("Save"), UIControlState.Normal);
					SaveRecordedSound.SetTitle("Save Recording", UIControlState.Normal);
					SaveRecordedSound.SetTitleColor(UIColor.FromRGB(45, 137, 221), UIControlState.Normal);

					observer = AVPlayerItem.Notifications.ObserveDidPlayToEndTime(OnDidPlayToEndTime);

					//var cellRecord = new UITableViewCell(UITableViewCellStyle.Default, null);
					cellRecord.Frame = new CGRect(0, 0, 350, 50);
					cellRecord.ImageView.Image = UIImage.FromBundle("Record");
					cellRecord.ContentView.Add(StartRecordingButton);
					cellRecord.ContentView.Add(RecordingStatusLabel);

					sec.Add(cellRecord);

					//var cellStop = new UITableViewCell(UITableViewCellStyle.Default, null);
					cellStop.Frame = new CGRect(0, 0, 350, 50);
					cellStop.ImageView.Image = UIImage.FromBundle("Stop");
					cellStop.ContentView.Add(StopRecordingButton);
					cellStop.ContentView.Add(LengthOfRecordingLabel);

					sec.Add(cellStop);

					//var cellPlay = new UITableViewCell(UITableViewCellStyle.Default, null);
					cellPlay.Frame = new CGRect(0, 0, 350, 50);
					cellPlay.ImageView.Image = UIImage.FromBundle("Play");
					cellPlay.ContentView.Add(PlayRecordedSoundButton);

					sec.Add(cellPlay);

					//var cellSave = new UITableViewCell(UITableViewCellStyle.Default, null);
					cellSave.Frame = new CGRect(0, 0, 350, 50);
					cellSave.ImageView.Image = UIImage.FromBundle("Save");
					cellSave.ContentView.Add(SaveRecordedSound);

					sec.Add(cellSave);

					string[][] dictations = { };
					if (CrossConnectivity.Current.IsConnected)
					{
						var dps = new DynaPadService.DynaPadService();
						dictations = dps.GetFormDictations(CommonFunctions.GetUserConfig(), SelectedAppointment.SelectedQForm.FormId, sectionId, SelectedAppointment.ApptDoctorId, true, SelectedAppointment.SelectedQForm.LocationId);
					}
					else
					{
						PresentViewController(CommonFunctions.InternetAlertPrompt(), true, null);
					}

					foreach (string[] dictation in dictations)
					{
						var bytes = Convert.FromBase64String(dictation[2]);
						var dataDictation = NSData.FromArray(bytes);
						NSError err;
						var dicplayer = new AVAudioPlayer(dataDictation, "aac", out err);

						var duration = TimeSpan.FromSeconds(dicplayer.Duration).ToString(@"hh\:mm\:ss");

						var statusLabel = new UILabel(new CGRect(200, 0, 100, 50));
						statusLabel.Text = duration;

						PlaySavedDictationButton = new UIButton();
						PlaySavedDictationButton.Frame = new CGRect(10, 0, 190, 50);
						PlaySavedDictationButton.SetTitle(dictation[1], UIControlState.Normal);
						PlaySavedDictationButton.SetTitleColor(UIColor.Blue, UIControlState.Normal);
						PlaySavedDictationButton.HorizontalAlignment = UIControlContentHorizontalAlignment.Left;
						PlaySavedDictationButton.SetImage(UIImage.FromBundle("CircledPlay"), UIControlState.Normal);
						PlaySavedDictationButton.ImageEdgeInsets = new UIEdgeInsets(0, 0, 0, 5);
						PlaySavedDictationButton.TitleEdgeInsets = new UIEdgeInsets(0, 5, 0, 0);

						messageLabel = new UILabel();

						DeleteSavedDictationButton = new UIButton();
						DeleteSavedDictationButton.Frame = new CGRect(300, 0, 50, 50);
						DeleteSavedDictationButton.SetTitleColor(UIColor.Black, UIControlState.Normal);
						DeleteSavedDictationButton.SetImage(UIImage.FromBundle("Delete"), UIControlState.Normal);
						DeleteSavedDictationButton.TouchUpInside += (sende, er) =>
						{
							var DeletePrompt = UIAlertController.Create("Delete Dictation", "Administrative use only. Deleteing dictation '" + dictation[1] + "', continue?", UIAlertControllerStyle.Alert);
							DeletePrompt.AddTextField((field) =>
							{
								field.SecureTextEntry = true;
								field.Placeholder = "Password";
							});
							DeletePrompt.Add(messageLabel);
							DeletePrompt.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, action => DeleteSavedDictation(DeletePrompt.TextFields[0].Text, SelectedAppointment.SelectedQForm.FormId, sectionId, dictation)));
							DeletePrompt.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, null));
							//Present Alert
							ParentViewController.PresentedViewController.PresentViewController(DeletePrompt, true, null);

						};

						cellDict = new UITableViewCell(UITableViewCellStyle.Default, null);
						cellDict.Frame = new CGRect(0, 0, 350, 50);
						cellDict.BackgroundColor = UIColor.LightGray;
						//cellDict.ImageView.Frame = new CGRect(0, 0, 20, 50);
						//cellDict.ImageView.Image = UIImage.FromBundle("CircledPlay");
						cellDict.ContentView.Add(PlaySavedDictationButton);
						cellDict.ContentView.Add(statusLabel);
						cellDict.ContentView.Add(DeleteSavedDictationButton);

						sec.Add(cellDict);

						PlaySavedDictationButton.TouchUpInside += delegate
						{
							OnPlaySavedDictation(dictation[1], dictation[2], dicplayer, statusLabel, PlaySavedDictationButton, sec);
						};
					}

					var popHeight = sec.Elements.Count > 8 ? 500 : sec.Elements.Count * 50 + 100;

					var roo = new RootElement("Dictation");
					roo.Add(sec);

					var dia = new DialogViewController(roo);
					dia.ModalInPopover = true;
					dia.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
					dia.PreferredContentSize = new CGSize(350, popHeight);

					var vie = new UIView();

					var con = new UIViewController();

					con.Add(vie);

					NavigationController.PreferredContentSize = new CGSize(350, popHeight);
					NavigationController.PresentViewController(dia, true, null);

					pop = new UIPopoverController(dia);
					pop.PopoverContentSize = new CGSize(350, popHeight);
					pop.ShouldDismiss = (popoverController) => false;
					pop.DidDismiss += delegate
					{
						//AVAudioSession.SharedInstance().Dispose();
						session.Dispose();
						session = null;

						observer.Dispose();
						observer = null;
						//recorder.Dispose();
						stopwatch = null;
						recorder = null;
						player = null;
						pop.Dispose();
						pop = null;
						audioFilePath = null;
					};

					//segDict.ValueChanged += (s, e) =>
					//{
					//	if (segDict.SelectedSegment == 0)
					//	{
					//		pop.Dismiss(true);
					//	}
					//};

					SaveRecordedSound.TouchUpInside += delegate
						{
							OnSaveRecordedSound(sectionId, sec, pop);
						};

					CancelRecording.TouchUpInside += delegate
					{
						//pop.Dismiss(true); 
						NavigationController.DismissViewController(true, null);
					};

					headclosebtn.TouchUpInside += delegate
					{
						//pop.Dismiss(true); 
						NavigationController.DismissViewController(true, null);
					};

					//pop.PresentFromBarButtonItem(NavigationItem.RightBarButtonItems[0], UIPopoverArrowDirection.Unknown, true);
				});

				return navdic;
			}
			catch (Exception ex)
			{
				CommonFunctions.sendErrorEmail(ex);
                PresentViewController(CommonFunctions.ExceptionAlertPrompt(), true, null);
                return null;
				//throw new Exception(ex.Message + Environment.NewLine + ex.StackTrace, ex.InnerException);
			}
		}


		void Chk_Tapped(SectionQuestion cQuestion, QuestionOption cOption, bool selected, string sectionId)
		{
			try
			{
				//string newTriggerId = cOption.ConditionTriggerIds;
				List<string> newTriggerIds = cOption.ConditionTriggerIds;
				cOption.Chosen = selected;

				MultiConditionalCheck(cQuestion, sectionId);

				QuestionsView.TableView.ReloadData();
			}
			catch (Exception ex)
			{
				CommonFunctions.sendErrorEmail(ex);
                PresentViewController(CommonFunctions.ExceptionAlertPrompt(), true, null);
				//throw new Exception(ex.Message + Environment.NewLine + ex.StackTrace, ex.InnerException);
			}
		}


		void Radio_Tapped(SectionQuestion rQuestion, QuestionOption rOption)
		{
			try
			{
				//string newTriggerId = rOption.ConditionTriggerIds;
				List<string> newTriggerIds = rOption.ConditionTriggerIds;
				rQuestion.QuestionOptions.ForEach((obj) => obj.Chosen = false);
				rOption.Chosen = true;
				ConditionalCheck(rQuestion.ActiveTriggerIds, newTriggerIds, rQuestion.SectionId);
				rQuestion.ActiveTriggerIds = newTriggerIds;
				QuestionsView.TableView.ReloadData();
			}
			catch (Exception ex)
			{
				CommonFunctions.sendErrorEmail(ex);
                PresentViewController(CommonFunctions.ExceptionAlertPrompt(), true, null);
				//throw new Exception(ex.Message + Environment.NewLine + ex.StackTrace, ex.InnerException);
			}
		}

		void Radio_UnTapped(SectionQuestion rQuestion, QuestionOption rOption)
		{
			try
			{
				//string newTriggerId = rOption.ConditionTriggerIds;
				var newTriggerIds = new List<string>();
				rQuestion.QuestionOptions.ForEach((obj) => obj.Chosen = false);
				rOption.Chosen = false;
				ConditionalCheck(rQuestion.ActiveTriggerIds, newTriggerIds, rQuestion.SectionId);
				rQuestion.ActiveTriggerIds = newTriggerIds;
				QuestionsView.TableView.ReloadData();
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message + Environment.NewLine + ex.StackTrace, ex.InnerException);
			}
		}


		void Boolean_Changed(SectionQuestion bQuestion, List<string> activeTriggerId, List<string> newTriggerIds, bool selected)
		{
			try
			{
				if (selected)
				{
					bQuestion.QuestionOptions[0].Chosen = true;
					bQuestion.QuestionOptions[1].Chosen = false;
				}
				else
				{
					bQuestion.QuestionOptions[0].Chosen = false;
					bQuestion.QuestionOptions[1].Chosen = true;
				}

				ConditionalCheck(activeTriggerId, newTriggerIds, bQuestion.SectionId);

				bQuestion.ActiveTriggerIds = newTriggerIds;
			}
			catch (Exception ex)
			{
				CommonFunctions.sendErrorEmail(ex);
                PresentViewController(CommonFunctions.ExceptionAlertPrompt(), true, null);
				//throw new Exception(ex.Message + Environment.NewLine + ex.StackTrace, ex.InnerException);
			}
		}


		void ConditionalCheck(List<string> activeTriggerIds, List<string> newTriggerIds, string sectionId)
		{
			try
			{
				if (activeTriggerIds != newTriggerIds)
				{
					var sectionQuestions = SelectedAppointment.SelectedQForm.FormSections.Find((FormSection obj) => obj.SectionId == sectionId);

					if (activeTriggerIds != null && activeTriggerIds.Count > 0 && !string.IsNullOrEmpty(activeTriggerIds[0]))
					{
						var untriggeredQuestions = sectionQuestions.SectionQuestions.FindAll(((obj) => activeTriggerIds.Contains(((dynamic)obj).ParentConditionTriggerId) && !string.IsNullOrEmpty(((dynamic)obj).ParentConditionTriggerId)));

						TriggerCheck(untriggeredQuestions, false, sectionId);
					}

                    if (newTriggerIds != null && newTriggerIds.Count > 0 && !string.IsNullOrEmpty(newTriggerIds[0]))
					{
						var triggeredQuestions = sectionQuestions.SectionQuestions.FindAll(((obj) => newTriggerIds.Contains(((dynamic)obj).ParentConditionTriggerId) && !string.IsNullOrEmpty(((dynamic)obj).ParentConditionTriggerId)));

						TriggerCheck(triggeredQuestions, true, sectionId);
					}
				}
			}
			catch (Exception ex)
			{
				CommonFunctions.sendErrorEmail(ex);
                PresentViewController(CommonFunctions.ExceptionAlertPrompt(), true, null);
				//throw new Exception(ex.Message + Environment.NewLine + ex.StackTrace, ex.InnerException);
			}
		}


		void MultiConditionalCheck(SectionQuestion activeQuestion, string sectionId)
		{
			try
			{
				var sectionQuestions = SelectedAppointment.SelectedQForm.FormSections.Find((FormSection obj) => obj.SectionId == sectionId);
				var untriggeredQuestions = new List<SectionQuestion>();
				var triggeredQuestions = new List<SectionQuestion>();
				foreach (QuestionOption qOption in activeQuestion.QuestionOptions)
				{
					if (qOption.Chosen)
					{
						if (qOption.ConditionTriggerIds != null && qOption.ConditionTriggerIds.Count > 0 && !string.IsNullOrEmpty(qOption.ConditionTriggerIds[0]))
						{
							triggeredQuestions.AddRange(sectionQuestions.SectionQuestions.FindAll(((obj) => qOption.ConditionTriggerIds.Contains(((dynamic)obj).ParentConditionTriggerId))));
						}
					}
					else
					{
						if (qOption.ConditionTriggerIds != null && qOption.ConditionTriggerIds.Count > 0 && !string.IsNullOrEmpty(qOption.ConditionTriggerIds[0]))
						{
							untriggeredQuestions.AddRange(sectionQuestions.SectionQuestions.FindAll(((obj) => qOption.ConditionTriggerIds.Contains(((dynamic)obj).ParentConditionTriggerId))));
						}
					}
				}

				TriggerCheck(untriggeredQuestions, false, sectionId);

				TriggerCheck(triggeredQuestions, true, sectionId);
			}
			catch (Exception ex)
			{
				CommonFunctions.sendErrorEmail(ex);
                PresentViewController(CommonFunctions.ExceptionAlertPrompt(), true, null);
				//throw new Exception(ex.Message + Environment.NewLine + ex.StackTrace, ex.InnerException);
			}
		}


		void TriggerCheck(List<SectionQuestion> triggerQuestions, bool triggered, string sectionId)
		{
			try
			{
				foreach (SectionQuestion tQuestion in triggerQuestions)
				{
					if (tQuestion.QuestionOptions != null)
					{
						foreach (QuestionOption tOption in tQuestion.QuestionOptions)
						{
							if (tOption.ConditionTriggerIds != null && tOption.ConditionTriggerIds.Count > 0 && !string.IsNullOrEmpty(tOption.ConditionTriggerIds[0]) && tOption.Chosen)
							{
								List<string> optionTriggerIds = tQuestion.ActiveTriggerIds;

								if (optionTriggerIds != null && optionTriggerIds.Count > 0 && !string.IsNullOrEmpty(optionTriggerIds[0]))
								{
									if (triggered)
									{
										ConditionalCheck(null, tOption.ConditionTriggerIds, sectionId);
									}
									else
									{
										ConditionalCheck(tOption.ConditionTriggerIds, null, sectionId);
									}
								}
							}

						}
					}

					tQuestion.IsEnabled = triggered;

					foreach (DynaSection sec in QuestionsView)
					{
						if (sec.QuestionId == tQuestion.QuestionId)
						{
							var headtype = sec.HeaderView.GetType();

							if (sec.HeaderView is UITableViewCell)
							{
								var headcell = (UITableViewCell)sec.HeaderView;
								var headerLabel = (PaddedUIView<UILabel>)headcell.ContentView.Subviews[0];
								headerLabel.NestedView.Text = tQuestion.QuestionText.ToUpper();
								UIButton headerDic = null;
								if (headcell.ContentView.Subviews[1] != null)
								{
									headerDic = (UIButton)headcell.ContentView.Subviews[1];
								}

								if (headerLabel != null)
								{
									headerLabel.Enabled = triggered;
									headerLabel.setStyle();

									if (headerDic != null)
									{
										headerDic.Enabled = triggered;
										if (headerDic.Enabled)
										{
											headerDic.BackgroundColor = UIColor.FromRGB(230, 230, 250);
										}
										else
										{
											headerDic.BackgroundColor = UIColor.GroupTableViewBackgroundColor;
										}
									}
								}
							}
							else
							{
								var headerLabel = (PaddedUIView<UILabel>)sec.HeaderView.Subviews[0];
								switch (headerLabel.Type)
								{
									case "Question":
										headerLabel.NestedView.Text = tQuestion.QuestionText.ToUpper();
										break;
									case "Subtitle":
										headerLabel.NestedView.Text = tQuestion.Subtitle.ToUpper();
										break;
								}
								UIButton headerDic = null;
								if (sec.HeaderView.Subviews.Length > 1)
								{
									if (sec.HeaderView.Subviews.Length == 2)
									{
										headerDic = (UIButton)sec.HeaderView.Subviews[1];
									}
									else if (sec.HeaderView.Subviews.Length == 3)
									{
										headerDic = (UIButton)sec.HeaderView.Subviews[2];
										var trueheaderLabel = (PaddedUIView<UILabel>)sec.HeaderView.Subviews[1];

										if (trueheaderLabel != null)
										{
											trueheaderLabel.Enabled = triggered;
											trueheaderLabel.setStyle();

											//if (headerDic != null)
											//{
											//	headerDic.Enabled = triggered;
											//	if (headerDic.Enabled)
											//	{
											//		headerDic.BackgroundColor = UIColor.FromRGB(230, 230, 250);
											//	}
											//	else
											//	{
											//		headerDic.BackgroundColor = UIColor.GroupTableViewBackgroundColor;
											//	}

											//}
										}
									}
								}

								if (headerLabel != null)
								{
									headerLabel.Enabled = triggered;
									headerLabel.setStyle();

									if (headerDic != null)
									{
										headerDic.Enabled = triggered;
										if (headerDic.Enabled)
										{
											headerDic.BackgroundColor = UIColor.FromRGB(230, 230, 250);
										}
										else
										{
											headerDic.BackgroundColor = UIColor.GroupTableViewBackgroundColor;
										}
									}
								}
							}

							foreach (dynamic element in sec.Elements)
							{
								if (element != null)
								{
									if (!(element is UIViewElement))
									{
										element.Enabled = triggered;
									}
									else
									{
										//PlaceholderEnabledUITextView pelement = (PlaceholderEnabledUITextView)element as PlaceholderEnabledUITextView;
										UIViewElement pelement = element;

										switch (pelement.ContainerView.Subviews[0].GetType().ToString())
										{
											case "DynaPad.PlaceholderEnabledUITextView":
												var peui = (PlaceholderEnabledUITextView)pelement.ContainerView.Subviews[0];
												peui.Enabled = triggered;
												peui.UserInteractionEnabled = triggered;
												//peui.Draw(peui.Frame);
												if (!triggered)
												{
													//peui.PlaceholderColor = UIColor.GroupTableViewBackgroundColor;
													peui.TextColor = UIColor.LightGray;
													peui.BackgroundColor = UIColor.GroupTableViewBackgroundColor;
													peui.Placeholder = "Not applicable";
												}
												else
												{
													//peui.PlaceholderColor = UIColor.Clear;
													peui.Placeholder = "Enter your answer here";
													peui.TextColor = UIColor.Black;
													peui.BackgroundColor = UIColor.White;
												}
												break;
											case "DynaPad.DynaAuto":
												var apeui = (DynaAuto)pelement.ContainerView.Subviews[0];
												apeui.Enabled = triggered;
												apeui.IsEnabled = triggered;
												apeui.UserInteractionEnabled = triggered;
												//peui.Draw(peui.Frame);
												if (!triggered)
												{
													//peui.PlaceholderColor = UIColor.GroupTableViewBackgroundColor;
													apeui.TextColor = UIColor.LightGray;
													apeui.BackgroundColor = UIColor.GroupTableViewBackgroundColor;
													apeui.Watermark = "Not applicable";
												}
												else
												{
													//peui.PlaceholderColor = UIColor.Clear;
													apeui.Watermark = "Enter your answer here";
													apeui.TextColor = UIColor.Black;
													apeui.BackgroundColor = UIColor.White;
												}
												break;
											case "DynaPad.DynaSegmented":
												var speui = (DynaSegmented)pelement.ContainerView.Subviews[0];
												speui.Enabled = triggered;
												speui.IsEnabled = triggered;
												speui.UserInteractionEnabled = triggered;
												break;
											case "DynaPad.DynaGrid":
												var gpeui = (DynaGrid)pelement.ContainerView.Subviews[0];
												gpeui.IsEnabled = triggered;
												gpeui.UserInteractionEnabled = triggered;
												if (!triggered)
												{
													gpeui.GridStyle = new DisabledGrid();
												}
												else
												{
													gpeui.GridStyle = new EnabledGrid();
												}
												break;
										}
									}

									if (element.GetContainerTableView() != null)
									{
										element.GetContainerTableView().ReloadData();
									}
								}
							}

							sec.Enabled = triggered;

							break;
						}
					}
				}
			}
			catch (Exception ex)
			{
				CommonFunctions.sendErrorEmail(ex);
                PresentViewController(CommonFunctions.ExceptionAlertPrompt(), true, null);
				//throw new Exception(ex.Message + Environment.NewLine + ex.StackTrace, ex.InnerException);
			}
		}


		public override void DidReceiveMemoryWarning()
		{
			base.DidReceiveMemoryWarning();
			// Release any cached data, images, etc that aren't in use.
		}


		void DeleteSavedDictation(string password, string formId, string sectionId, string[] dictation)
		{
			try
			{
				//bool isValid = password == Constants.Password;
				bool isValid = false;

				//for (int i = 0; i < Constants.Logins.GetLength(0); i++)
				//{
				//	if (SelectedAppointment.ApptLocationId == Constants.Logins[i, 2])
				//	{
				//		isValid |= password == Constants.Logins[i, 1];
				//	}
				//}

				if (SelectedAppointment.ApptLocationId == DynaClassLibrary.DynaClasses.LoginContainer.User.SelectedLocation.LocationId)
				{
					isValid |= password == DynaClassLibrary.DynaClasses.LoginContainer.User.DynaPassword;
				}

				if (CrossConnectivity.Current.IsConnected)
				{
					if (isValid)
					{
                        var dds = new DynaPadService.DynaPadService() { Timeout = 60000 };
						var deletedDictation = dds.DeleteDicatation(CommonFunctions.GetUserConfig(), dictation[3], formId, sectionId, SelectedAppointment.SelectedQForm.DoctorId);
						NavigationController.DismissViewController(true, null);
					}
					else
					{
						messageLabel.Text = "Wrong password";
						var FailAlert = UIAlertController.Create("Error", "Wrong password", UIAlertControllerStyle.Alert);
						FailAlert.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Cancel, null));
						// Present Alert
						PresentViewController(CommonFunctions.AlertPrompt("Error", "Wrong password", true, null, false, null), true, null);
					}
				}
				else
				{
					PresentViewController(CommonFunctions.InternetAlertPrompt(), true, null);
				}
			}
			catch (Exception ex)
			{
				CommonFunctions.sendErrorEmail(ex);
                PresentViewController(CommonFunctions.ExceptionAlertPrompt(), true, null);
				//throw new Exception(ex.Message + Environment.NewLine + ex.StackTrace, ex.InnerException);
			}
		}


		void OnStopRecording(object sender, EventArgs e)
		{
			//if (player.Playing)
			//	player.Stop();

			if (recorder == null)
				return;

			recorder.Stop();
			stopwatch.Stop();

			cellRecord.ImageView.Image = UIImage.FromBundle("Record");
			LengthOfRecordingLabel.Text = string.Format("{0:hh\\:mm\\:ss}", stopwatch.Elapsed);
			RecordingStatusLabel.Text = "";
			StartRecordingButton.Enabled = true;
			StartRecordingButton.Alpha = 1;
			StopRecordingButton.Enabled = false;
			StopRecordingButton.Alpha = (nfloat)0.5;
			cellStop.ImageView.Image = UIImage.FromBundle("Stop");
			PlayRecordedSoundButton.Enabled = true;
			PlayRecordedSoundButton.Alpha = 1;
			SaveRecordedSound.Enabled = true;
			SaveRecordedSound.Alpha = 1;
		}


		void OnStartRecording(object sender, EventArgs e)
		{
			//System.Console.WriteLine("Begin Recording");

			session = AVAudioSession.SharedInstance();

			NSError error = null;
			session.SetCategory(AVAudioSession.CategoryRecord, out error);
			if (error != null)
			{
				CommonFunctions.sendNSErrorEmail(error);
                PresentViewController(CommonFunctions.ExceptionAlertPrompt(), true, null);
				//System.Console.WriteLine(error);
				return;
			}

			session.SetActive(true, out error);
			if (error != null)
			{
				CommonFunctions.sendNSErrorEmail(error);
                PresentViewController(CommonFunctions.ExceptionAlertPrompt(), true, null);
				//System.Console.WriteLine(error);
				return;
			}

			if (!PrepareAudioRecording())
			{
				RecordingStatusLabel.Text = "Error preparing";
				return;
			}

			if (!recorder.Record())
			{
				RecordingStatusLabel.Text = "Error preparing";
				return;
			}

			stopwatch = new Stopwatch();
			stopwatch.Start();

			cellRecord.ImageView.Image = UIImage.FromBundle("RecordRed");
			LengthOfRecordingLabel.Text = "";
			RecordingStatusLabel.Text = "Recording";
			StartRecordingButton.Enabled = false;
			StartRecordingButton.Alpha = (nfloat)0.5;
			StopRecordingButton.Enabled = true;
			StopRecordingButton.Alpha = 1;
			cellStop.ImageView.Image = UIImage.FromBundle("StopBlack");
			PlayRecordedSoundButton.Enabled = false;
			PlayRecordedSoundButton.Alpha = (nfloat)0.5;
			SaveRecordedSound.Enabled = false;
			SaveRecordedSound.Alpha = (nfloat)0.5;
		}


		NSUrl CreateOutputUrl()
		{
			var fileName = string.Format("Myfile{0}.aac", DateTime.Now.ToString("yyyyMMddHHmmss"));
			var tempRecording = Path.Combine(Path.GetTempPath(), fileName);
			//string tempRecording = Path.Combine(Environment.GetFolderPath(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), fileName);
			return NSUrl.FromFilename(tempRecording);
		}


		void OnDidPlayToEndTime(object sender, NSNotificationEventArgs e)
		{
			//StartRecordingButton.Enabled = true;
			//StartRecordingButton.Alpha = 1;
			//StopRecordingButton.Enabled = false;
			//StopRecordingButton.Alpha = (nfloat)0.5;
			PlayRecordedSoundButton.Enabled = false;
			PlayRecordedSoundButton.Alpha = (nfloat)0.5;

			//StopRecordingButton.TouchUpInside += OnStopRecording;

			player.Dispose();
			player = null;
		}


		void StopRecordingPlayback(object sender, EventArgs e)
		{
			player.Stop();
			//player.PrepareToPlay();

			StartRecordingButton.Enabled = true;
			StartRecordingButton.Alpha = 1;
			StopRecordingButton.Enabled = false;
			StopRecordingButton.Alpha = (nfloat)0.5;
			StopRecordingButton.SetTitle("Stop Recording", UIControlState.Normal);
			cellStop.ImageView.Image = UIImage.FromBundle("Stop");
			PlayRecordedSoundButton.Enabled = true;
			PlayRecordedSoundButton.Alpha = 1;
			RecordingStatusLabel.Text = "";
			cellPlay.ImageView.Image = UIImage.FromBundle("Play");
			StopRecordingButton.TouchUpInside += OnStopRecording;
		}


		void OnPlayRecordedSound(object sender, EventArgs e)
		{
			StartRecordingButton.Enabled = false;
			StartRecordingButton.Alpha = (nfloat)0.5;
			StopRecordingButton.Enabled = true;
			StopRecordingButton.Alpha = 1;
			StopRecordingButton.SetTitle("Stop Playback", UIControlState.Normal);
			cellStop.ImageView.Image = UIImage.FromBundle("StopBlack");
			cellPlay.ImageView.Image = UIImage.FromBundle("PlayGreen");
			PlayRecordedSoundButton.Enabled = false;
			PlayRecordedSoundButton.Alpha = (nfloat)0.5;

			try
			{
				//System.Console.WriteLine("Playing Back Recording {0}", audioFilePath);

				// The following line prevents the audio from stopping
				// when the device autolocks. will also make sure that it plays, even
				// if the device is in mute
				NSError error = null;
				AVAudioSession.SharedInstance().SetCategory(AVAudioSession.CategoryPlayback, out error);
                if (error != null)
				{
					CommonFunctions.sendNSErrorEmail(error);
                PresentViewController(CommonFunctions.ExceptionAlertPrompt(), true, null);
                    //throw new Exception(error.DebugDescription);
                }
				NSError audioError;
				//player = new AVPlayer(audioFilePath);
				player = new AVAudioPlayer(audioFilePath, "aac", out audioError);

				player.FinishedPlaying += (sen, ee) =>
				{
					StartRecordingButton.Enabled = true;
					StartRecordingButton.Alpha = 1;
					StopRecordingButton.Enabled = false;
					StopRecordingButton.Alpha = (nfloat)0.5;
					StopRecordingButton.SetTitle("Stop Recording", UIControlState.Normal);
					cellStop.ImageView.Image = UIImage.FromBundle("Stop");
					PlayRecordedSoundButton.Enabled = true;
					PlayRecordedSoundButton.Alpha = 1;
					RecordingStatusLabel.Text = "";
					cellPlay.ImageView.Image = UIImage.FromBundle("Play");
					StopRecordingButton.TouchUpInside += OnStopRecording;
					//PlayRecordedSoundButton.TouchUpInside += OnPlayRecordedSound;
				};

				//PlayRecordedSoundButton.TouchUpInside += (sendi, eve) =>
				//{
				//	StartRecordingButton.Enabled = true;
				//	StartRecordingButton.Alpha = 1;
				//	StopRecordingButton.Enabled = false;
				//	StopRecordingButton.Alpha = (nfloat)0.5;
				//	PlayRecordedSoundButton.Enabled = true;
				//	PlayRecordedSoundButton.Alpha = 1;
				//	RecordingStatusLabel.Text = "";
				//	cellPlay.ImageView.Image = UIImage.FromBundle("Play");
				//	PlayRecordedSoundButton.TouchUpInside += OnPlayRecordedSound;

				//	player.Stop();
				//};

				StopRecordingButton.TouchUpInside += StopRecordingPlayback;

				RecordingStatusLabel.Text = "Playing";

				player.Play();

			}
			catch (Exception ex)
			{
				CommonFunctions.sendErrorEmail(ex);
                PresentViewController(CommonFunctions.ExceptionAlertPrompt(), true, null);
				//System.Console.WriteLine("There was a problem playing back audio: ");
				//System.Console.WriteLine(ex.Message);
			}
		}


		void OnPlaySavedDictation(string title, string dictationBytes, AVAudioPlayer pplayer, UILabel statusLabel, UIButton cd, Section cdsec)
		{
			try
			{
				//System.Console.WriteLine("Playing Back Recording {0}", title);

				// The following line prevents the audio from stopping
				// when the device autolocks. will also make sure that it plays, even
				// if the device is in mute
				NSError error = null;
				AVAudioSession.SharedInstance().SetCategory(AVAudioSession.CategoryPlayback, out error);
                if (error != null)
				{
					CommonFunctions.sendNSErrorEmail(error);
                    PresentViewController(CommonFunctions.ExceptionAlertPrompt(), true, null);
                    //throw new Exception(error.DebugDescription);
                }
				//byte[] bytes = Convert.FromBase64String(dictationBytes);
				//NSData dataDictation = NSData.FromArray(bytes);
				//NSError audioError;

				statusLabel.Text = "Playing";

				//player = new AVAudioPlayer(dataDictation, "aac", out audioError);
				//player.FinishedPlaying += (se, ea) => { PlayRecordedSoundStatusLabel.Text = string.Format("{0:hh\\:mm\\:ss}", player.Data.Length); };
				//player.Play();

				var duration = TimeSpan.FromSeconds(pplayer.Duration).ToString(@"hh\:mm\:ss");

				//cd.SetImage(UIImage.FromBundle("CircledStop"), UIControlState.Normal);
				//cdsec.GetImmediateRootElement().Reload(cdsec, UITableViewRowAnimation.Fade);

				pplayer.FinishedPlaying += (se, ea) =>
				{
					statusLabel.Text = duration;
					//cd.SetImage(UIImage.FromBundle("CircledPlay"), UIControlState.Normal);
					//PlaySavedDictationButton.TouchUpInside += delegate
					//  {
					//   OnPlaySavedDictation(title, dictationBytes, pplayer, statusLabel, cd);
					//  };
				};

				//PlaySavedDictationButton.TouchUpInside += (sender, e) => 
				//{ 
				//	pplayer.Stop();
				//	//pplayer.Dispose();
				//	statusLabel.Text = duration;
				//	cd.ImageView.Image = UIImage.FromBundle("CircledPlay");
				//	PlaySavedDictationButton.TouchUpInside += delegate
				//   {
				//	   OnPlaySavedDictation(title, dictationBytes, pplayer, statusLabel, cd);
				//   };
				//};

				pplayer.Play();
			}
			catch (Exception ex)
			{
				CommonFunctions.sendErrorEmail(ex);
                PresentViewController(CommonFunctions.ExceptionAlertPrompt(), true, null);
				//System.Console.WriteLine("There was a problem playing back audio: ");
				//System.Console.WriteLine(ex.Message);
			}
		}


		bool PrepareAudioRecording()
		{
			audioFilePath = CreateOutputUrl();

			var audioSettings = new AudioSettings
			{
				SampleRate = 44100,
				Format = AudioToolbox.AudioFormatType.MPEG4AAC,
				NumberChannels = 1,
				AudioQuality = AVAudioQuality.High
			};

			//Set recorder parameters
			NSError error;
			recorder = AVAudioRecorder.Create(audioFilePath, audioSettings, out error);
			if (error != null)
			{
				CommonFunctions.sendNSErrorEmail(error);
                PresentViewController(CommonFunctions.ExceptionAlertPrompt(), true, null);
				//System.Console.WriteLine(error);
				return false;
			}

			//Set Recorder to Prepare To Record
			try
			{
				if (!recorder.PrepareToRecord())
				{
					recorder.Dispose();
					recorder = null;
					return false;
				}
			}
			catch (Exception ex)
			{
				CommonFunctions.sendErrorEmail(ex);
                PresentViewController(CommonFunctions.ExceptionAlertPrompt(), true, null);
				//System.Console.WriteLine("record error: " + ex.Message);
			}

			recorder.FinishedRecording += OnFinishedRecording;

			return true;
		}


		void OnFinishedRecording(object sender, AVStatusEventArgs e)
		{
			recorder.Dispose();
			recorder = null;
			//System.Console.WriteLine("Done Recording (status: {0})", e.Status);
		}


		protected override void Dispose(bool disposing)
		{
			observer.Dispose();
			base.Dispose(disposing);
		}


		void OnSaveRecordedSound(string sectionId, Section dicSec, UIPopoverController popd)
		{
			var bounds = pop.PopoverContentSize;
			loadingOverlay = new LoadingOverlay(new CGRect(new CGPoint(0, 0), bounds));
			//mvc = (DialogViewController)((UINavigationController)SplitViewController.ViewControllers[1]).TopViewController;
			//mvc.Add(loadingOverlay);
			popd.ContentViewController.Add(loadingOverlay);
			try
			{
				if (CrossConnectivity.Current.IsConnected)
				{
					var dictationData = NSData.FromUrl(audioFilePath); //the path here can be a path to a video on the camera roll
					var dictationArray = dictationData.ToArray();

                    var dds = new DynaPadService.DynaPadService() { Timeout = 60000 };
					//DynaPadService.DynaPadService dds = new DynaPadService.DynaPadService();
					var dictationPath = dds.SaveDictation(CommonFunctions.GetUserConfig(), SelectedAppointment.SelectedQForm.FormId, sectionId, SelectedAppointment.ApptDoctorId, true, SelectedAppointment.SelectedQForm.LocationId, sectionId + "_" + DateTime.Now.ToShortTimeString(), dictationArray);
					//System.Console.WriteLine("Saving Recording {0}", audioFilePath);

					//var dps = new DynaPadService.DynaPadService();
					//var dictations = dps.GetFormDictations(SelectedAppointment.SelectedQForm.FormId, sectionId, SelectedAppointment.ApptDoctorId, true, SelectedAppointment.SelectedQForm.LocationId);
					//dicSec.RemoveRange(4, dicSec.Elements.Count - 4);

					//foreach (string[] dictation in dictations)
					//{
					//	byte[] bytes = Convert.FromBase64String(dictation[2]);
					//	NSData dataDictation = NSData.FromArray(bytes);
					//	NSError err;

					//	var dicplayer = new AVAudioPlayer(dataDictation, "aac", out err);

					//	var duration = TimeSpan.FromSeconds(dicplayer.Duration).ToString(@"hh\:mm\:ss");

					//	var statusLabel = new UILabel(new CGRect(210, 0, 120, 50));
					//	statusLabel.Text = duration;

					//	//var PlaySavedDictationButton = new UIButton();
					//	PlaySavedDictationButton.Frame = new CGRect(0, 0, 160, 50);
					//	PlaySavedDictationButton.SetTitle(dictation[1], UIControlState.Normal);
					//	PlaySavedDictationButton.SetTitleColor(UIColor.Black, UIControlState.Normal);
					//	//PlaySavedDictationButton.SetImage(UIImage.FromBundle("CircledPlay"), UIControlState.Normal);

					//	cellDict = new UITableViewCell(UITableViewCellStyle.Default, null);
					//	cellDict.Frame = new CGRect(0, 0, 350, 50);
					//	cellDict.BackgroundColor = UIColor.LightGray;
					//	cellDict.ImageView.Image = UIImage.FromBundle("CircledPlay");
					//	cellDict.ContentView.Add(PlaySavedDictationButton);
					//	cellDict.ContentView.Add(statusLabel);

					//	PlaySavedDictationButton.TouchUpInside += delegate
					//	{
					//		OnPlaySavedDictation(dictation[1], dictation[2], dicplayer, statusLabel, cellDict);
					//	};

					//	dicSec.Add(cellDict);
					//	dicSec.GetImmediateRootElement().Reload(dicSec, UITableViewRowAnimation.Fade);
					//}

					//loadingOverlay.Hide();

					//pop.Dismiss(true);
					NavigationController.DismissViewController(true, null);
				}
				else
				{
					PresentViewController(CommonFunctions.InternetAlertPrompt(), true, null);
				}
			}
			catch (Exception ex)
			{
				CommonFunctions.sendErrorEmail(ex);
                PresentViewController(CommonFunctions.ExceptionAlertPrompt(), true, null);
				//throw new Exception(ex.Message + Environment.NewLine + ex.StackTrace, ex.InnerException);
			}
            finally
            {
                loadingOverlay.Hide();
            }
		}


		void OnCancelRecording(object sender, EventArgs e)
		{
			try
			{
				Console.WriteLine("Canceled Recording");
			}
			catch (Exception ex)
			{
				CommonFunctions.sendErrorEmail(ex);
                PresentViewController(CommonFunctions.ExceptionAlertPrompt(), true, null);
				System.Console.WriteLine("There was a problem canceling audio: ");
				System.Console.WriteLine(ex.Message);
			}
		}

		void SaveSectionPreset(string presetId, string presetName, string sectionId, Section presetSection, PresetRadioElement pre, RadioGroup presetGroup, string origS, GlassButton nextbtn, bool isDoctorInput = true)
		{
			try
			{
				// doctorid = 123 / 321
				// locationid = 321 / 123

				if (CrossConnectivity.Current.IsConnected)
				{
					var sectionQuestions = SelectedAppointment.SelectedQForm.FormSections.Find((FormSection obj) => obj.SectionId == sectionId);
					var fs = SelectedAppointment.SelectedQForm.FormSections.IndexOf(sectionQuestions);

					var presetJson = JsonConvert.SerializeObject(SelectedAppointment.SelectedQForm.FormSections[fs]);
                    var dds = new DynaPadService.DynaPadService() { Timeout = 60000 };
					dds.SaveAnswerPreset(CommonFunctions.GetUserConfig(), SelectedAppointment.SelectedQForm.FormId, sectionId, SelectedAppointment.ApptDoctorId, true, presetName, presetJson, SelectedAppointment.ApptLocationId, presetId);

					if (presetId == null)
					{
						var mre = GetPreset(presetId, presetName, presetJson, fs, sectionId, presetGroup, sectionQuestions, presetSection, origS, isDoctorInput, nextbtn);

						presetSection.Insert(presetSection.Count - 1, UITableViewRowAnimation.Fade, mre);
						presetSection.GetImmediateRootElement().RadioSelected = presetSection.Count - 2;

						presetSection.GetImmediateRootElement().Reload(presetSection, UITableViewRowAnimation.Fade);
					}
					else
					{
						presetSection.GetImmediateRootElement().RadioSelected = presetGroup.Selected;
						pre.PresetName = presetName;
						pre.Caption = presetName;
						//pre = GetPreset(presetId, presetName, presetJson, fs, sectionId, presetGroup, sectionQuestions, presetSection, origS, isDoctorInput, nextbtn);

						//pre.GetImmediateRootElement().Reload(pre, UITableViewRowAnimation.Fade);
						//var p = pre.Parent.Parent.Parent;
						//var pp = pre.Parent.Parent.Parent.Parent;

						presetSection.GetImmediateRootElement().Reload(pre, UITableViewRowAnimation.Fade);
						//presetSection.GetImmediateRootElement().Reload(presetSection, UITableViewRowAnimation.Fade);
					}
					//presetSection.GetContainerTableView().RemoveFromSuperview();
					//QuestionsView.TableView.ReloadData();
					//SetDetailItem(new Section(sectionQuestions.SectionName), "", sectionId, origS, isDoctorInput, nextbtn);
					NavigationController.PopViewController(true);
				}
				else
				{
					PresentViewController(CommonFunctions.InternetAlertPrompt(), true, null);
				}
			}
			catch (Exception ex)
			{
                NavigationController.PopViewController(true);
				CommonFunctions.sendErrorEmail(ex);
                PresentViewController(CommonFunctions.ExceptionAlertPrompt(), true, null);
				//throw new Exception(ex.Message + Environment.NewLine + ex.StackTrace, ex.InnerException);
			}
		}

		void DeleteSectionPreset(string presetId, string presetName, string sectionId, Section presetSection, PresetRadioElement pre, RadioGroup presetGroup, string origS, GlassButton nextbtn, bool isDoctorInput = true)
		{
			try
			{
				if (CrossConnectivity.Current.IsConnected)
				{
					var sectionQuestions = SelectedAppointment.SelectedQForm.FormSections.Find((FormSection obj) => obj.SectionId == sectionId);
					var fs = SelectedAppointment.SelectedQForm.FormSections.IndexOf(sectionQuestions);

					var presetJson = JsonConvert.SerializeObject(SelectedAppointment.SelectedQForm.FormSections[fs]);
                    var dds = new DynaPadService.DynaPadService() { Timeout = 60000 };
					dds.DeleteAnswerPreset(CommonFunctions.GetUserConfig(), SelectedAppointment.SelectedQForm.FormId, sectionId, SelectedAppointment.ApptDoctorId, presetId);

					//var mre = GetPreset(presetId, presetName, presetJson, fs, sectionId, presetGroup, sectionQuestions, presetSection, origS, isDoctorInput, nextbtn);

					//presetSection.Insert(presetSection.Count - 1, UITableViewRowAnimation.Automatic, mre);
					if (presetSection.GetImmediateRootElement().RadioSelected == pre.Index)
					{
						presetSection.GetImmediateRootElement().RadioSelected = 0;
					}
					presetSection.Remove(pre);
					//presetSection.GetImmediateRootElement().Reload(pre, UITableViewRowAnimation.Fade);
					presetSection.GetImmediateRootElement().Reload(presetSection, UITableViewRowAnimation.Fade);

					NavigationController.PopViewController(true);
				}
				else
				{
					PresentViewController(CommonFunctions.InternetAlertPrompt(), true, null);
				}
			}
			catch (Exception ex)
			{
                NavigationController.PopViewController(true);
				CommonFunctions.sendErrorEmail(ex);
                PresentViewController(CommonFunctions.ExceptionAlertPrompt(), true, null);
				//throw new Exception(ex.Message + Environment.NewLine + ex.StackTrace, ex.InnerException);
			}
		}

		public PresetRadioElement GetPreset(string presetId, string presetName, string presetJson, int fs, string sectionId, RadioGroup presetGroup, FormSection sectionQuestions, Section presetSection, string origS, bool isDoctorInput, GlassButton nextbtn)
		{
			try
			{
				var mre = new PresetRadioElement(presetName, "PresetAnswers");
				mre.PresetID = presetId;
				mre.PresetName = presetName;
				mre.PresetJson = presetJson;
				mre.OnSelected += delegate
				{
					SelectedAppointment.SelectedQForm.FormSections[fs] = JsonConvert.DeserializeObject<FormSection>(presetJson);
					var selectedSection = SelectedAppointment.SelectedQForm.FormSections.Find((FormSection obj) => obj.SectionId == sectionId);
					if (selectedSection != null)
					{
						selectedSection.SectionSelectedTemplateId = presetGroup.Selected;
					}

					SetDetailItem(new Section(sectionQuestions.SectionName), "", sectionId, origS, isDoctorInput, nextbtn);
				};
				mre.editPresetBtn.TouchUpInside += (sender, e) =>
				{
					var UpdatePresetPrompt = UIAlertController.Create("Update Section Preset", "Overwriting preset '" + mre.PresetName + "', do you wish to continue?", UIAlertControllerStyle.Alert);
					//Add Actions
					UpdatePresetPrompt.AddTextField((field) =>
						{
							field.Placeholder = "Preset Name";
							field.Text = mre.PresetName;
						});
					UpdatePresetPrompt.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, action => SaveSectionPreset(mre.PresetID, UpdatePresetPrompt.TextFields[0].Text, sectionId, presetSection, mre, presetGroup, origS, nextbtn)));
					UpdatePresetPrompt.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, null));
					//Present Alert

					PresentViewController(UpdatePresetPrompt, true, null);
				};
				mre.deletePresetBtn.TouchUpInside += (sender, e) =>
				{
					var UpdatePresetPrompt = UIAlertController.Create("Delete Section Preset", "Deleting preset '" + mre.PresetName + "', do you wish to continue?", UIAlertControllerStyle.Alert);
					//Add Actions
					UpdatePresetPrompt.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, action => DeleteSectionPreset(mre.PresetID, mre.PresetName, sectionId, presetSection, mre, presetGroup, origS, nextbtn)));
					UpdatePresetPrompt.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, null));
					//Present Alert

					PresentViewController(UpdatePresetPrompt, true, null);
				};

				return mre;
			}
			catch (Exception ex)
			{
				CommonFunctions.sendErrorEmail(ex);
                PresentViewController(CommonFunctions.ExceptionAlertPrompt(), true, null);
                return null;
				//throw new Exception(ex.Message + Environment.NewLine + ex.StackTrace, ex.InnerException);
			}
		}



	}
}
