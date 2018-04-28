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
using Plugin.Connectivity;
using System.Windows.Input;
using System.Xml;
using System.Net;
using System.Collections.ObjectModel;
using SafariServices;
using WebKit;
using System.Threading;
//using Syncfusion.SfImageEditor.iOS;
//using Syncfusion.SfPdfViewer.iOS;
//using DynaClassLibrary;
//using static DynaClassLibrary.DynaClasses;
//using MessageUI;
//using PdfKit;
using System.Linq;
using Syncfusion.SfBusyIndicator.iOS;
//using UserNotifications;
using ToastIOS;
//using System.Runtime.CompilerServices;
//using System.Data;
//using iTextSharp.text.pdf;
using DynaPad.DynaPadService;
//using iTextSharp.text;
using FileProvider;
using Syncfusion.Pdf.Xmp;

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
        NSUserDefaults plist = NSUserDefaults.StandardUserDefaults;

        public Menu DynaMenu { get; set; }

        protected DetailViewController(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
            //this.TableView.CellLayoutMarginsFollowReadableWidth = false;
        }


        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            //Root.Caption = "Welcome to Dynapad";
            Root.Add(new Section("Login to the app")
            {
                FooterView = new UIView(new CGRect(0, 0, 0, 0))
                {
                    Hidden = true
                }
            });

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



        CancellationTokenSource cts;

        nfloat labelHeight = 22;
        nfloat labelWidth = 70;
        nfloat centerX;
        nfloat centerY;
        UIButton cancelButton;

        public async void SetDetailItem(Section newDetailItem, string context, string valueId, string origSectionJson, bool IsDoctorForm, GlassButton nextbtn = null, bool IsViewSummary = false, string SummaryFileName = null, string ReportName = null)
        {
            try
            {
                var boundsh = base.TableView.Frame;
                mvc = (DialogViewController)((UINavigationController)SplitViewController.ViewControllers[1]).TopViewController;

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
                        //  mem.Seek(0, SeekOrigin.Begin);
                        //  pdfViewerControl.LoadDocument(mem);
                        //}
                        ////var q = new PdfDocument(URL);
                        //var s = new DynaSection("file");
                        //s.Add(pdfViewerControl);
                        //var f = new DynaMultiRootElement("File");
                        //f.Add(s);
                        //Root = f;
                        //Root.TableView.ScrollEnabled = false;
                        //break;
                        //case "UploadSubmittedForms":
                        //case "UploadSubmittedPatientForms":
                            //loadingOverlay = new LoadingOverlay(boundsh, true);// { loadingLabelText = "Loading MR..." };
                            //loadingOverlay.SetText("Loading...");
                            //mvc.Add(loadingOverlay);

                            //await Task.Delay(10);

                            //var UploadsView = new DynaMultiRootElement("Downloads");

                            //var uploadHeadPaddedView = new PaddedUIView<UILabel>
                            //{
                            //    Enabled = true,
                            //    Type = "Section",
                            //    Frame = new CGRect(0, 0, 0, 40),
                            //    Padding = 5f
                            //};
                            //uploadHeadPaddedView.NestedView.Text = "Uploud Submitted Forms";
                            //uploadHeadPaddedView.NestedView.TextAlignment = UITextAlignment.Center;
                            //uploadHeadPaddedView.NestedView.Font = UIFont.BoldSystemFontOfSize(17);
                            //uploadHeadPaddedView.setStyle();

                            //var uploadHeadSection = new DynaSection("Uploud")
                            //{
                            //    HeaderView = uploadHeadPaddedView,
                            //    FooterView = new UIView(new CGRect(0, 0, 0, 0))
                            //};
                            //uploadHeadSection.FooterView.Hidden = true;

                            //UploadsView.Add(uploadHeadSection);

                            //var uploadMainSection = new DynaSection("Uploud")
                            //{
                            //    HeaderView = new UIView(new CGRect(0, 0, 0, 0)),
                            //    FooterView = new UIView(new CGRect(0, 0, 0, 0))
                            //};
                            //uploadMainSection.HeaderView.Hidden = true;
                            //uploadMainSection.FooterView.Hidden = true;

                            //nfloat quWidth = View.Frame.Width;

                            //progressView = new UIProgressView(new CGRect(0, 0, quWidth, 15))
                            //{
                            //    Progress = 0,
                            //    Hidden = true
                            //};

                            //uploadMainSection.Add(progressView);

                            //var uploadPaddedView = new PaddedUIView<UILabel>
                            //{
                            //    Frame = new CGRect(0, 0, quWidth, 50),
                            //    Padding = 5f,
                            //    Type = "Question"
                            //};

                            //var btnUploud = new GlassButton(new RectangleF(0, 0, (float)quWidth, 50))
                            //{
                            //    NormalColor = UIColor.FromRGB(224, 238, 240)
                            //};
                            //btnUploud.TitleLabel.Font = UIFont.BoldSystemFontOfSize(17);
                            //btnUploud.SetTitleColor(UIColor.Black, UIControlState.Normal);
                            //btnUploud.SetTitle("Uploud Forms", UIControlState.Normal);

                            //if (context == "UploadSubmittedForms")
                            //{
                            //    uploadPaddedView.NestedView.Text = "Tap the uploud button to uploud all location forms:";
                            //    uploadPaddedView.setStyle();

                            //    uploadMainSection.Add(uploadPaddedView);

                            //    btnUploud.TouchUpInside += (sender, e) =>
                            //    {
                            //        //Create Alert
                            //        var UploudPrompt = UIAlertController.Create("Uploud Submitted Forms", "Location forms will be uploaded.", UIAlertControllerStyle.Alert);
                            //        //Add Actions
                            //        UploudPrompt.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, action => UploudSubmittedForms(valueId, false)));
                            //        UploudPrompt.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, null));
                            //        //Present Alert
                            //        PresentViewController(UploudPrompt, true, null);
                            //    };
                            //}
                            //else if (context == "UploadSubmittedPatientForms")
                            //{
                            //    uploadPaddedView.NestedView.Text = "Tap the uploud button to uploud patient form:";
                            //    uploadPaddedView.setStyle();

                            //    uploadMainSection.Add(uploadPaddedView);

                            //    btnUploud.TouchUpInside += (sender, e) =>
                            //    {
                            //        //Create Alert
                            //        var DUploudPatientPrompt = UIAlertController.Create("Uploud Submitted Forms", "Patient form will be uploaded.", UIAlertControllerStyle.Alert);
                            //        //Add Actions
                            //        DUploudPatientPrompt.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, action => UploudSubmittedForms(valueId, true)));
                            //        DUploudPatientPrompt.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, null));
                            //        //Present Alert
                            //        PresentViewController(DUploudPatientPrompt, true, null);
                            //    };
                            //}

                            //UploadsView.Add(uploadMainSection);

                            //var uploadFooterSection = new Section { HeaderView = null, FooterView = null };

                            //uploadFooterSection.Add(btnUploud);

                            //UploadsView.Add(uploadFooterSection);

                            //UploadsView.UnevenRows = true;

                            //Root.TableView.AutosizesSubviews = true;
                            //Root = UploadsView;
                            //Root.TableView.ScrollEnabled = true;

                            //break;
                        case "ApptInfo":
                            loadingOverlay = new LoadingOverlay(boundsh, true);
                            loadingOverlay.SetText("Loading...");
                            mvc.Add(loadingOverlay);

                            await Task.Delay(10);

                            var apptInfoElement = GetApptInfoElement();
                            Root = apptInfoElement;

                            break;
                        case "UploadSubmittedForms":
                        case "UploadSubmittedPatientForms":
                            loadingOverlay = new LoadingOverlay(boundsh, true);
                            loadingOverlay.SetText("Loading...");
                            mvc.Add(loadingOverlay);

                            await Task.Delay(10);

                            var uploadElement = GetUploadElement(valueId, context);
                            Root = uploadElement;

                            break;
                        case "MRDownload":
                        case "MRPatientDownload":
                            loadingOverlay = new LoadingOverlay(boundsh, true);// { loadingLabelText = "Loading MR..." };
                            loadingOverlay.SetText("Loading...");
                            mvc.Add(loadingOverlay);

                            await Task.Delay(10);

                            var DownloadsView = new DynaMultiRootElement("Downloads");

                            var downloadHeadPaddedView = new PaddedUIView<UILabel>
                            {
                                Enabled = true,
                                Type = "Section",
                                Frame = new CGRect(0, 0, 0, 40),
                                Padding = 5f
                            };
                            downloadHeadPaddedView.NestedView.Text = "Download Medical Records";
                            downloadHeadPaddedView.NestedView.TextAlignment = UITextAlignment.Center;
                            downloadHeadPaddedView.NestedView.Font = UIFont.BoldSystemFontOfSize(17);
                            downloadHeadPaddedView.setStyle();

                            var downloadHeadSection = new DynaSection("MR")
                            {
                                HeaderView = downloadHeadPaddedView,
                                FooterView = new UIView(new CGRect(0, 0, 0, 0))
                            };
                            downloadHeadSection.FooterView.Hidden = true;

                            DownloadsView.Add(downloadHeadSection);

                            var downloadMainSection = new DynaSection("MR")
                            {
                                HeaderView = new UIView(new CGRect(0, 0, 0, 0)),
                                FooterView = new UIView(new CGRect(0, 0, 0, 0))
                            };
                            downloadMainSection.HeaderView.Hidden = true;
                            downloadMainSection.FooterView.Hidden = true;

                            nfloat qWidth = View.Frame.Width;

                            progressView = new UIProgressView(new CGRect(0, 0, qWidth, 15))
                            {
                                Progress = 0,
                                Hidden = true,
                                ClipsToBounds = true
                            };
                            progressView.Layer.MasksToBounds = true;
                            //Transform = CGAffineTransform.MakeScale(1, 20)

                            //imageView = new UIImageView(new CGRect(0, 0, 100, 150));
                            //imageView.Hidden = true;

                            downloadMainSection.Add(progressView);
                            //downloadMainSection.Add(imageView);

                            var downloadPaddedView = new PaddedUIView<UILabel>
                            {
                                Frame = new CGRect(0, 0, qWidth, 50),
                                Padding = 5f,
                                Type = "Question"
                            };

                            btnDownload = new GlassButton(new RectangleF(0, 0, (float)qWidth, 50))
                            {
                                NormalColor = UIColor.FromRGB(224, 238, 240),
                                Enabled = DownloadButtonGlobalEnabled
                            };
                            btnDownload.TitleLabel.Font = UIFont.BoldSystemFontOfSize(17);
                            btnDownload.SetTitleColor(UIColor.Black, UIControlState.Normal);
                            btnDownload.SetTitle("Download Files", UIControlState.Normal);

                            if (context == "MRDownload")
                            {
                                downloadPaddedView.NestedView.Text = "Select a date to download medical records, then tap the download button:";
                                downloadPaddedView.setStyle();

                                downloadMainSection.Add(downloadPaddedView);

                                var dde = new UIDatePicker(new CGRect(0, 0, qWidth, 180))
                                {
                                    Mode = UIDatePickerMode.Date,
                                    Date = DateTime.Today.AddDays(1).ToNSDate()
                                };

                                downloadMainSection.Add(dde);

                                btnDownload.TouchUpInside += (sender, e) =>
                                {
                                    //Create Alert
                                    var DownloadMRPrompt = UIAlertController.Create("Download Medical Records", "Chosen medical records will be downloaded in the background", UIAlertControllerStyle.Alert);
                                    //Add Actions
                                    DownloadMRPrompt.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, action => DownloadMRs(valueId, dde.Date.ToDateTime(), false)));
                                    DownloadMRPrompt.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, null));
                                    //Present Alert
                                    PresentViewController(DownloadMRPrompt, true, null);
                                };
                            }
                            else if (context == "MRPatientDownload")
                            {
                                downloadPaddedView.NestedView.Text = "Tap the download button to download patient medical records:";
                                downloadPaddedView.setStyle();

                                downloadMainSection.Add(downloadPaddedView);

                                btnDownload.TouchUpInside += (sender, e) =>
                                {
                                    //Create Alert
                                    var DownloadPatientMRPrompt = UIAlertController.Create("Download Medical Records", "Patient medical records will be downloaded in the background", UIAlertControllerStyle.Alert);
                                    //Add Actions
                                    DownloadPatientMRPrompt.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, action => DownloadMRs(valueId, DateTime.Today, true)));
                                    DownloadPatientMRPrompt.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, null));
                                    //Present Alert
                                    PresentViewController(DownloadPatientMRPrompt, true, null);
                                };
                            }

                            DownloadsView.Add(downloadMainSection);

                            var downloadFooterSection = new Section { HeaderView = null, FooterView = null };

                            downloadFooterSection.Add(btnDownload);

                            //var btnCrash = new GlassButton(new RectangleF((float)qWidth - 200, 0, 200, 50))
                            //{
                            //    NormalColor = UIColor.Red
                            //};
                            //btnCrash.TitleLabel.Font = UIFont.BoldSystemFontOfSize(17);
                            //btnCrash.SetTitleColor(UIColor.Black, UIControlState.Normal);]
                            //btnCrash.SetTitle("Crash", UIControlState.Normal);
                            //btnCrash.TouchUpInside += delegate {
                            //    // Force the app to crash
                            //    string s = null;
                            //    s.ToString();
                            //};
                            //downloadFooterSection.Add(btnCrash);

                            DownloadsView.Add(downloadFooterSection);

                            DownloadsView.UnevenRows = true;

                            Root.TableView.AutosizesSubviews = true;
                            Root = DownloadsView;
                            Root.TableView.ScrollEnabled = true;

                            break;
                        case "MR":
                            loadingOverlay = new LoadingOverlay(boundsh, true);// { loadingLabelText = "Loading MR..." };
                            loadingOverlay.SetText("Loading MR...");
                            mvc.Add(loadingOverlay);

                            await Task.Delay(10);

                            var mrElement = GetMRElement(valueId);
                            Root = mrElement;

                            break;
                        case "Summary":
                            loadingOverlay = new LoadingOverlay(boundsh, true);// { loadingLabelText = "Loading Summary..." };
                            if (IsViewSummary)
                            {
                                loadingOverlay.SetText("Loading Summary...");
                                mvc.Add(loadingOverlay);
                            }
                            await Task.Delay(10);

                            var summaryElement = new DynaMultiRootElement(SelectedAppointment.ApptFormName + " - " + SelectedAppointment.ApptPatientName);

                            var summaryPaddedView = new PaddedUIView<UILabel>
                            {
                                Enabled = true,
                                Type = "Section",
                                Frame = new CGRect(0, 0, 0, 40),
                                Padding = 5f
                            };
                            summaryPaddedView.NestedView.Text = "SUMMARY";
                            summaryPaddedView.NestedView.TextAlignment = UITextAlignment.Center;
                            summaryPaddedView.NestedView.Font = UIFont.BoldSystemFontOfSize(17);
                            summaryPaddedView.setStyle();

                            var summarySection = new DynaSection("SUMMARY")
                            {
                                HeaderView = summaryPaddedView,
                                FooterView = new UIView(new CGRect(0, 0, 0, 0))
                            };
                            summarySection.FooterView.Hidden = true;

                            var summaryFileName = "";

                            if (!IsViewSummary)
                            {
                                //if (CrossConnectivity.Current.IsConnected)
                                //{
                                    //var dds = new DynaPadService.DynaPadService { Timeout = 60000 };
                                    var finalJson = JsonConvert.SerializeObject(SelectedAppointment.SelectedQForm);

                                    //summaryFileName = dds.GenerateSummary(CommonFunctions.GetUserConfig(), finalJson);
                                    summaryFileName = GenerateSummary(finalJson);

                                    //SFSafariViewController sfViewController = new SFSafariViewController(new NSUrl(summaryFileName));
                                    //PresentViewController(sfViewController, true, null);
                                    //var PreviewController = UIDocumentInteractionController.FromUrl(NSUrl.FromFilename(summaryFileName));
                                    //PreviewController.Delegate = new UIDocumentInteractionControllerDelegateClass(UIApplication.SharedApplication.KeyWindow.RootViewController);
                                    //BeginInvokeOnMainThread(() =>
                                    //{
                                    //PreviewController.PresentPreview(true);
                                    //});

                                    var mas = (DialogViewController)((UINavigationController)SplitViewController.ViewControllers[0]).TopViewController;
                                    mas.NavigationController.PopViewController(true);
                                //}
                                //else
                                //{
                                //    PresentViewController(CommonFunctions.InternetAlertPrompt(), true, null);
                                //}
                            }
                            else
                            {
                                summaryFileName = SummaryFileName;
                            }

                            if (!summaryFileName.StartsWith("Error:", StringComparison.CurrentCulture))
                            {
                                if (IsViewSummary)
                                {
                                    var webView = new WKWebView(View.Bounds, new WKWebViewConfiguration())
                                    {
                                        Frame = new CGRect(View.Bounds.X, 0, View.Bounds.Width, View.Bounds.Height)
                                    };
                                    webView.LoadRequest(new NSUrlRequest(new NSUrl(summaryFileName)));

                                    summarySection.Add(webView);
                                    summaryElement.Add(summarySection);
                                }
                                else
                                {
                                    var sucmes = SelectedAppointment.SelectedQForm.IsDoctorForm ? "Doctor form submitted successfully. If not done so already, upload appointment files to generate report." : "Patient form submitted successfully.";
                                    var se = new StringElement(sucmes);
                                    summarySection.Add(se);

                                    var directoryname = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "DynaFilesAwaitingUpload/");
                                    var summaryDynaFile = JsonConvert.DeserializeObject<DynaFile>(File.ReadAllText(summaryFileName));
                                    var webView = new UIWebView(new CGRect(View.Bounds.X + 5, View.Bounds.Y + 60, View.Bounds.Width - 5, View.Bounds.Height - 80))
                                    {
                                        ScalesPageToFit = true
                                    };
                                    webView.LoadHtmlString(summaryDynaFile.Html, new NSUrl(directoryname, true));
                                    summarySection.Add(webView);

                                    summaryElement.Add(summarySection);

                                    var boo = string.IsNullOrEmpty(plist.StringForKey("Upload_On_Submit")) || bool.Parse(plist.StringForKey("Upload_On_Submit"));
                                    if (boo)
                                    {
                                        loadingOverlay.Hide();
                                        if (CrossConnectivity.Current.IsConnected)
                                        {
                                            DoSubmitUpload();
                                        }
                                        else
                                        {
                                            PresentViewController(CommonFunctions.InternetAlertPrompt(), true, null);
                                        }
                                    }
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
                        case "Report":
                            loadingOverlay = new LoadingOverlay(boundsh, true);// { loadingLabelText = "Finalizing..." };
                            loadingOverlay.SetText("Loading Report...");

                            // derive the center x and y
                            centerX = new nfloat(loadingOverlay.Frame.Width / 2);
                            centerY = new nfloat(loadingOverlay.Frame.Height / 2);

                            cancelButton = new UIButton(UIButtonType.System)
                            {
                                Frame = new CGRect(centerX - (labelWidth / 2), centerY + 50, labelWidth, labelHeight)
                            };
                            cancelButton.SetTitle("Cancel", UIControlState.Normal);
                            cancelButton.TouchUpInside += (sender, e) =>
                            {
                                try
                                {
                                    cts?.Cancel();
                                }
                                catch (ObjectDisposedException oex)     // in case previous search completed
                                {
                                    Console.WriteLine($"\nObjectDisposedException in cancelButton.TouchUpInside with: {oex.Message}");
                                }
                            };

                            loadingOverlay.AddSubview(cancelButton);
                            mvc.Add(loadingOverlay);

                            try
                            {
                                cts?.Cancel();     // cancel previous search
                            }
                            catch (ObjectDisposedException oex)     // in case previous search completed
                            {
                                Console.WriteLine($"\nObjectDisposedException in Report with: {oex.Message}");
                            }

                            using (cts = new CancellationTokenSource())
                            {
                                try
                                {
                                    cts.CancelAfter(TimeSpan.FromSeconds(10));
                                    await Task.Delay(TimeSpan.FromSeconds(1), cts.Token);

                                    var task = ConfigureReportView(context, ReportName, valueId, cts.Token);
                                    var result = await task;
                                    //Console.WriteLine($"\nGot section with: {result}");
                                }
                                catch (TaskCanceledException tex)       // if the operation is cancelled, do nothing
                                {
                                    Console.WriteLine($"\nCanceled with: {tex.Message}");

                                    //CommonFunctions.sendErrorEmail((Exception)tex);

                                    var master = (MasterViewController)((UINavigationController)SplitViewController.ViewControllers[0]).ViewControllers[0];

                                    foreach (Element d in master.secforcancel.Elements)
                                    {
                                        var t = d.GetType();

                                        if (t == typeof(SectionStringElement))
                                        {
                                            var di = (SectionStringElement)d;
                                            if (di.IndexPath == master.oldsel)// di.prevsel)
                                            {
                                                di.selected = true;
                                                //break;
                                            }
                                            else
                                            {
                                                di.selected = false;
                                            }
                                        }
                                    }

                                    master.secforcancel.GetContainerTableView().ReloadData();

                                    PresentViewController(CommonFunctions.AlertPrompt("Canceled/Timeout", "Operation was canceled or timed out, please try again", true, null, false, null), true, null);
                                }
                            }

                            break;
                        case "Finalize":
                            loadingOverlay = new LoadingOverlay(boundsh, true);// { loadingLabelText = "Finalizing..." };
                            loadingOverlay.SetText("Finalizing...");

                            // derive the center x and y
                            centerX = new nfloat(loadingOverlay.Frame.Width / 2);
                            centerY = new nfloat(loadingOverlay.Frame.Height / 2);

                            cancelButton = new UIButton(UIButtonType.System)
                            {
                                Frame = new CGRect(centerX - (labelWidth / 2), centerY + 50, labelWidth, labelHeight)
                            };
                            cancelButton.SetTitle("Cancel", UIControlState.Normal);
                            cancelButton.TouchUpInside += (sender, e) =>
                            {
                                try
                                {
                                    cts?.Cancel();
                                }
                                catch (ObjectDisposedException oex)     // in case previous search completed
                                {
                                    Console.WriteLine($"\nObjectDisposedException in cancelButton.TouchUpInside with: {oex.Message}");
                                }
                            };

                            loadingOverlay.AddSubview(cancelButton);
                            mvc.Add(loadingOverlay);

                            try
                            {
                                cts?.Cancel();     // cancel previous search
                            }
                            catch (ObjectDisposedException oex)     // in case previous search completed
                            {
                                Console.WriteLine($"\nObjectDisposedException in Finalize with: {oex.Message}");
                            }

                            using (cts = new CancellationTokenSource())
                            {
                                try
                                {
                                    cts.CancelAfter(TimeSpan.FromSeconds(10));
                                    await Task.Delay(TimeSpan.FromSeconds(1), cts.Token);

                                    var task = ConfigureFinalizeView(context, IsDoctorForm, cts.Token);
                                    var result = await task;
                                    //Console.WriteLine($"\nGot section with: {result}");
                                }
                                catch (TaskCanceledException tex)       // if the operation is cancelled, do nothing
                                {
                                    Console.WriteLine($"\nCanceled with: {tex.Message}");

                                    //CommonFunctions.sendErrorEmail((Exception)tex);

                                    var master = (MasterViewController)((UINavigationController)SplitViewController.ViewControllers[0]).ViewControllers[0];

                                    foreach (Element d in master.secforcancel.Elements)
                                    {
                                        var t = d.GetType();

                                        if (t == typeof(SectionStringElement))
                                        {
                                            var di = (SectionStringElement)d;
                                            if (di.IndexPath == master.oldsel)// di.prevsel)
                                            {
                                                di.selected = true;
                                                //break;
                                            }
                                            else
                                            {
                                                di.selected = false;
                                            }
                                        }
                                    }

                                    master.secforcancel.GetContainerTableView().ReloadData();

                                    PresentViewController(CommonFunctions.AlertPrompt("Canceled/Timeout", "Operation was canceled or timed out, please try again", true, null, false, null), true, null);
                                }
                            }

                            break;
                        default:
                            loadingOverlay = new LoadingOverlay(boundsh, true);// { loadingLabelText = "Loading Section..." };
                            //loadingOverlay.showCancelButton = true;
                            //loadingOverlay.ShowCancel();
                            loadingOverlay.SetText("Loading Section...");

                            // derive the center x and y
                            centerX = new nfloat(loadingOverlay.Frame.Width / 2);
                            centerY = new nfloat(loadingOverlay.Frame.Height / 2);

                            cancelButton = new UIButton(UIButtonType.System)
                            {
                                Frame = new CGRect(centerX - (labelWidth / 2), centerY + 50, labelWidth, labelHeight)
                            };
                            cancelButton.SetTitle("Cancel", UIControlState.Normal);
                            cancelButton.TouchUpInside += (sender, e) =>
                            {
                                try
                                {
                                    cts?.Cancel();
                                }
                                catch (ObjectDisposedException oex)     // in case previous search completed
                                {
                                    Console.WriteLine($"\nObjectDisposedException in cancelButton.TouchUpInside with: {oex.Message}");
                                }
                            };

                            loadingOverlay.AddSubview(cancelButton);

                            mvc.Add(loadingOverlay);

                            try
                            {
                                cts?.Cancel();     // cancel previous search
                            }
                            catch (ObjectDisposedException oex)     // in case previous search completed
                            {
                                Console.WriteLine($"\nObjectDisposedException with: {oex.Message}");
                            }

                            using (cts = new CancellationTokenSource())
                            {
                                try
                                {
                                    cts.CancelAfter(TimeSpan.FromSeconds(10));
                                    //await Task.Delay(TimeSpan.FromSeconds(1), cts.Token);
                                    await Task.Delay(10, cts.Token);

                                    var task = ConfigureView(context, valueId, origSectionJson, IsDoctorForm, nextbtn, cts.Token);
                                    //var section = await ConfigureView(context, valueId, origSectionJson, IsDoctorForm, nextbtn, cts.Token);
                                    var result = await task;
                                    //Console.WriteLine($"\nGot section with: {result}");
                                }
                                catch (TaskCanceledException tex)       // if the operation is cancelled, do nothing
                                {
                                    Console.WriteLine($"\nCanceled with: {tex.Message}");

                                    //CommonFunctions.sendErrorEmail((Exception)tex);

                                    var master = (MasterViewController)((UINavigationController)SplitViewController.ViewControllers[0]).ViewControllers[0];

                                    foreach (Element d in master.secforcancel.Elements)
                                    {
                                        var t = d.GetType();

                                        if (t == typeof(SectionStringElement))
                                        {
                                            var di = (SectionStringElement)d;
                                            if (di.IndexPath == master.oldsel)// di.prevsel)
                                            {
                                                di.selected = true;
                                                //break;
                                            }
                                            else
                                            {
                                                di.selected = false;
                                            }
                                        }
                                    }

                                    master.secforcancel.GetContainerTableView().ReloadData();

                                    PresentViewController(CommonFunctions.AlertPrompt("Canceled/Timeout", "Operation was canceled or timed out, please try again", true, null, false, null), true, null);
                                }
                            }


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
                PresentViewController(CommonFunctions.ExceptionAlertPrompt(ex), true, null);
                //throw new Exception(ex.Message + Environment.NewLine + ex.StackTrace, ex.InnerException);
            }
            finally
            {
                loadingOverlay.Hide();
                var master = (MasterViewController)((UINavigationController)SplitViewController.ViewControllers[0]).ViewControllers[0];
                master.isRunning = false;
            }
        }



        public void DoSubmitUpload()
        {
            try
            {
                string documentsPath;

                documentsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "DynaFilesAwaitingUpload/" + SelectedAppointment.ApptPatientId + "/" + SelectedAppointment.ApptId);

                var array = new List<DynaFile>();
                string[] files;

                if (Directory.Exists(documentsPath) && Directory.GetFiles(documentsPath, "*.*", SearchOption.AllDirectories).Length > 0)
                {
                    files = Directory.GetFiles(documentsPath, "*.*", SearchOption.AllDirectories);

                    foreach (string file in files)
                    {
                        string contents = File.ReadAllText(file);
                        var userconfig = CommonFunctions.GetUserConfig();

                        var filename = Path.GetFileName(file);
                        var filetype = filename.Substring(0, filename.IndexOf("_", StringComparison.CurrentCulture));

                        var upload = JsonConvert.DeserializeObject<DynaFile>(File.ReadAllText(file));

                        array.Add(upload);
                    }
                }

                UploadSubmittedForms(new string[] { documentsPath }, "DoSubmitUpload", false);
            }
            catch (Exception ex)
            {
                //loadingOverlay.Hide();
                CommonFunctions.sendErrorEmail(ex);
                PresentViewController(CommonFunctions.ExceptionAlertPrompt(ex), true, null);
                //throw new Exception(ex.Message + Environment.NewLine + ex.StackTrace, ex.InnerException);
            }
        }



        public async void UploadSubmittedForms(string[] folders, string context, bool returnToGrid)
        {
            try
            {
                var boundsh = base.TableView.Frame;
                mvc = (DialogViewController)((UINavigationController)SplitViewController.ViewControllers[1]).TopViewController;
                loadingOverlay = new LoadingOverlay(boundsh, true);
                loadingOverlay.SetText("Uploading...");
                centerX = new nfloat(loadingOverlay.Frame.Width / 2);
                centerY = new nfloat(loadingOverlay.Frame.Height / 2);

                cancelButton = new UIButton(UIButtonType.System)
                {
                    Frame = new CGRect(centerX - (labelWidth / 2), centerY + 50, labelWidth, labelHeight)
                };
                cancelButton.SetTitle("Cancel", UIControlState.Normal);
                cancelButton.TouchUpInside += (sender, e) =>
                {
                    cts.Cancel();
                };

                loadingOverlay.AddSubview(cancelButton);

                mvc.Add(loadingOverlay);

                try
                {
                    cts?.Cancel();     // cancel previous search
                }
                catch (ObjectDisposedException oex)     // in case previous search completed
                {
                    Console.WriteLine($"\nObjectDisposedException with: {oex.Message}");
                }

                using (cts = new CancellationTokenSource())
                {
                    try
                    {
                        cts.CancelAfter(TimeSpan.FromSeconds(10));
                        //await Task.Delay(TimeSpan.FromSeconds(1), cts.Token);
                        await Task.Delay(10, cts.Token);

                        var task = DoUpload(folders, cts.Token, context, returnToGrid);
                        var result = await task;
                    }
                    catch (TaskCanceledException tex)       // if the operation is cancelled, do nothing
                    {
                        Console.WriteLine($"\nCanceled with: {tex.Message}");
                        loadingOverlay.Hide();
                        PresentViewController(CommonFunctions.AlertPrompt("Canceled/Timeout", "Operation was canceled or timed out, please try again", true, null, false, null), true, null);
                    }
                }
            }
            catch (Exception ex)
            {
                loadingOverlay.Hide();
                CommonFunctions.sendErrorEmail(ex);
                PresentViewController(CommonFunctions.ExceptionAlertPrompt(ex), true, null);
            }
        }




        public async Task<string> DoUpload(string[] folders, CancellationToken cts, string context, bool returnToGrid)
        {
            //await Task.Delay(TimeSpan.FromSeconds(2), cts);
            await Task.Delay(10, cts);

            try
            {
                if (CrossConnectivity.Current.IsConnected)
                {
                    var array = new List<DynaFile>();

                    foreach (var folder in folders)
                    {
                        if (Directory.Exists(folder) && Directory.GetFiles(folder, "*.*", SearchOption.AllDirectories).Length > 0)
                        {
                            var files = Directory.GetFiles(folder, "*.*", SearchOption.AllDirectories);

                            foreach (string file in files)
                            {
                                var upload = JsonConvert.DeserializeObject<DynaFile>(File.ReadAllText(file));

                                array.Add(upload);
                            }
                        }
                    }

                    var starttoast = new Toast("Uploading " + array.Count.ToString() + " files. This may take a while, please be patient.");
                    starttoast.SetDuration(20000);
                    starttoast.SetType(ToastType.Info);
                    starttoast.SetGravity(ToastGravity.Bottom);
                    starttoast.Show();

                    var dds = new DynaPadService.DynaPadService { Timeout = 180000 };
                    var result = dds.ProcessDynaFiles(CommonFunctions.GetUserConfig(), JsonConvert.SerializeObject(array));

                    if (!result.StartsWith("Error", StringComparison.CurrentCulture))
                    {
                        foreach (var file in array)
                        {
                            var documentsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "DynaFilesUploaded/" + file.PatientId + "/" + file.ApptId);
                            var awaitingDocumentsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "DynaFilesAwaitingUpload/" + file.PatientId + "/" + file.ApptId);

                            var filePath = documentsPath + "/" + file.FileName + ".txt";
                            var awaitingFilePath = awaitingDocumentsPath + "/" + file.FileName + ".txt";

                            if (!Directory.Exists(documentsPath))
                            {
                                Directory.CreateDirectory(documentsPath);
                            }

                            file.Status = "Uploaded";

                            File.WriteAllText(filePath, JsonConvert.SerializeObject(file)); // writes to local storage
                            if (File.Exists(awaitingFilePath))
                            {
                                File.Delete(awaitingFilePath);
                            }
                        }

                        var finishtoast = new Toast("The requested form(s) have been uploaded");
                        finishtoast.SetDuration(20000);
                        finishtoast.SetType(ToastType.Info);
                        finishtoast.SetGravity(ToastGravity.Bottom);
                        finishtoast.Show();


                    }
                    else
                    {
                        var errortoast = new Toast("Uploading error, try again");
                        errortoast.SetDuration(20000);
                        errortoast.SetType(ToastType.Info);
                        errortoast.SetGravity(ToastGravity.Bottom);
                        errortoast.Show();
                    }
                }
                else
                {
                    PresentViewController(CommonFunctions.InternetAlertPrompt(), true, null);
                }
            }
            catch (Exception ex)
            {
                //loadingOverlay.Hide();
                CommonFunctions.sendErrorEmail(ex);
                PresentViewController(CommonFunctions.ExceptionAlertPrompt(ex), true, null);
                //throw new Exception(ex.Message + Environment.NewLine + ex.StackTrace, ex.InnerException);
            }
            finally
            {
                loadingOverlay.Hide();
                if (returnToGrid)
                {
                    SetDetailItem(new Section("Upload Submitted Forms"), context, DynaClassLibrary.DynaClasses.LoginContainer.User.SelectedLocation.LocationId, null, false);
                }
            }

            return "Uploaded";
        }







        const string Identifier = "com.DynaPad.BackgroundSession";
        const string DownloadUrlString = "https://upload.wikimedia.org/wikipedia/commons/9/97/The_Earth_seen_from_Apollo_17.jpg";

        public NSUrlSessionDownloadTask downloadTask;
        public NSUrlSession downloadSession;

        public UIProgressView progressView;
        public GlassButton btnDownload;
        public bool DownloadButtonGlobalEnabled = true;
        //public UIImageView imageView;

        public MR downloadMR;


        List<NSUrlSessionDownloadTask> taskslist;
        // valueid is locid from master...
        void DownloadMRs(string valueId, DateTime dt, bool appt)
        {
            if (downloadSession == null)
                downloadSession = InitBackgroundSession();

            if (downloadTask != null)
                return;

            var dds = new DynaPadService.DynaPadService { Timeout = 180000 };
            string filesJson;
            if (appt)
            {
                filesJson = dds.GetFiles(CommonFunctions.GetUserConfig(), SelectedAppointment.ApptId, SelectedAppointment.ApptPatientId, SelectedAppointment.ApptPatientName, SelectedAppointment.ApptDoctorId, SelectedAppointment.ApptLocationId);
            }
            else
            {
                filesJson = dds.GetFilesByDate(CommonFunctions.GetUserConfig(), valueId, dt.ToShortDateString());
            }

            if (!filesJson.StartsWith("error", StringComparison.CurrentCulture))
            {
                //var deserializedFiles = JsonConvert.DeserializeObject<List<string>>(filesJson);
                var deserializedFiles = JsonConvert.DeserializeObject<List<MRFolder>>(filesJson);

                taskslist = new List<NSUrlSessionDownloadTask>();

                if (deserializedFiles.Any())
                {
                    btnDownload.Enabled = false;
                    DownloadButtonGlobalEnabled = false;
                    //imageView.Hidden = true;
                    progressView.Hidden = false;

                    foreach (MRFolder folder in deserializedFiles)
                    {
                        foreach (MR file in folder.MrFolderMRs)
                        {
                            using (var url = NSUrl.FromString(file.MRPath))
                            using (var request = NSUrlRequest.FromUrl(url))
                            {
                                downloadMR = file;
                                downloadTask = downloadSession.CreateDownloadTask(request);
                                taskslist.Add(downloadTask);
                                downloadTask.Resume();
                            }
                        }
                    }

                    var toast = new Toast("Starting download of " + taskslist.Count + " medical records");
                    toast.SetDuration(5000);
                    toast.SetType(ToastType.Info);
                    toast.SetGravity(ToastGravity.Bottom);
                    toast.Show();
                }
                else
                {
                    var toast = new Toast("No medical records were found for the requested date");
                    toast.SetDuration(5000);
                    toast.SetType(ToastType.Info);
                    toast.SetGravity(ToastGravity.Bottom);
                    toast.Show();

                    btnDownload.Enabled = true;
                    DownloadButtonGlobalEnabled = true;

                    //imageView.Hidden = true;
                    progressView.Hidden = false;
                }
            }
            else
            {
                var toast = new Toast("Error. No medical records were downloaded");
                toast.SetDuration(5000);
                toast.SetType(ToastType.Info);
                toast.SetGravity(ToastGravity.Bottom);
                toast.Show();

                btnDownload.Enabled = true;
                DownloadButtonGlobalEnabled = true;

                //imageView.Hidden = true;
                progressView.Hidden = false;
            }
        }

        //void DownloadPatientMRs()
        //{
        //    if (downloadSession == null)
        //        downloadSession = InitBackgroundSession();

        //    if (downloadTask != null)
        //        return;

        //    var dds = new DynaPadService.DynaPadService { Timeout = 180000 };
        //    var filesJson = dds.GetFiles(CommonFunctions.GetUserConfig(), SelectedAppointment.ApptId, SelectedAppointment.ApptPatientId, SelectedAppointment.ApptPatientName, SelectedAppointment.ApptDoctorId, SelectedAppointment.ApptLocationId);

        //    if (!filesJson.StartsWith("Error"))
        //    {
        //        //var deserializedFiles = JsonConvert.DeserializeObject<List<string>>(filesJson);
        //        var deserializedFiles = JsonConvert.DeserializeObject<List<MRFolder>>(filesJson);

        //        taskslist = new List<NSUrlSessionDownloadTask>();

        //        if (deserializedFiles.Any())
        //        {
        //            //imageView.Hidden = true;
        //            progressView.Hidden = false;
        //            btnDownload.Enabled = false;

        //            foreach (MRFolder folder in deserializedFiles)
        //            {
        //                foreach (MR file in folder.MrFolderMRs)
        //                {
        //                    using (var url = NSUrl.FromString(file.MRPath))
        //                    using (var request = NSUrlRequest.FromUrl(url))
        //                    {
        //                        downloadMR = file;
        //                        downloadTask = downloadSession.CreateDownloadTask(request);
        //                        taskslist.Add(downloadTask);
        //                        downloadTask.Resume();
        //                    }
        //                }
        //            }
        //        }
        //        else
        //        {
        //            var toast = new Toast("No medical records were found for the requested appointment");
        //            toast.SetDuration(5000);
        //            toast.SetType(ToastType.Info);
        //            toast.SetGravity(ToastGravity.Bottom);
        //            toast.Show();

        //            progressView.Hidden = false;
        //        }
        //    }
        //    else
        //    {
        //        var toast = new Toast("Error. No medical records were downloaded");
        //        toast.SetDuration(5000);
        //        toast.SetType(ToastType.Info);
        //        toast.SetGravity(ToastGravity.Bottom);
        //        toast.Show();

        //        //imageView.Hidden = true;
        //        progressView.Hidden = false;
        //    }
        //}

        public NSUrlSession InitBackgroundSession()
        {
            Console.WriteLine("InitBackgroundSession");
            //using (var configuration = NSUrlSessionConfiguration.CreateBackgroundSessionConfiguration(Identifier))
            //{
            ////return NSUrlSession.FromConfiguration(configuration, new UrlSessionDelegate(this), null);
            //return NSUrlSession.FromConfiguration(configuration, new UrlSessionDelegate(this), null);
            //}
            using (var configuration = NSUrlSessionConfiguration.DefaultSessionConfiguration)
            {
                return NSUrlSession.FromConfiguration(configuration, new UrlSessionDelegate(this) as INSUrlSessionDelegate, null);
            }
        }

        public UIProgressView ProgressView
        {
            get { return progressView; }
        }

        public GlassButton BtnDownload
        {
            get { return btnDownload; }
        }

        public bool BtnDownloadEnabled
        {
            get { return DownloadButtonGlobalEnabled; }
            set { DownloadButtonGlobalEnabled = value; }
        }

        //public UIImageView ImageView
        //{
        //    get { return imageView; }
        //}

        public MR DownloadMR
        {
            get { return downloadMR; }
        }

        public class UrlSessionDelegate : NSUrlSessionDownloadDelegate
        {
            public DetailViewController controller;

            public UrlSessionDelegate(DetailViewController controller)
            {
                this.controller = controller;
            }

            public override void DidWriteData(NSUrlSession session, NSUrlSessionDownloadTask downloadTask, long bytesWritten, long totalBytesWritten, long totalBytesExpectedToWrite)
            {
                Console.WriteLine("Set Progress");
                if (downloadTask == controller.downloadTask)
                {
                    float progress = totalBytesWritten / (float)totalBytesExpectedToWrite;
                    Console.WriteLine(string.Format("DownloadTask: {0}  progress: {1}", downloadTask, progress));
                    InvokeOnMainThread(() =>
                    {
                        controller.ProgressView.Progress = progress;
                    });
                }
            }



            public override void DidFinishDownloading(NSUrlSession session, NSUrlSessionDownloadTask downloadTask, NSUrl location)
            {
                Console.WriteLine("Finished");
                Console.WriteLine("File downloaded in : {0}", location);
#pragma warning disable XI0002 // Notifies you from using newer Apple APIs when targeting an older OS version
                NSFileManager fileManager = NSFileManager.DefaultManager;
#pragma warning restore XI0002 // Notifies you from using newer Apple APIs when targeting an older OS version

                var URLs = fileManager.GetUrls(NSSearchPathDirectory.DocumentDirectory, NSSearchPathDomain.User);
                NSUrl documentsDictionry = URLs[0];

                NSUrl originalURL = downloadTask.OriginalRequest.Url;
                var destinationURL = documentsDictionry.Append("image1.png", false);

                fileManager.Remove(destinationURL, out NSError removeCopy);
                var success = fileManager.Copy(location, destinationURL, out NSError errorCopy);

                if (success)
                {
                    var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                    var cache = Path.Combine(documents, "..", "Library", "Caches");
                    var directoryname = Path.Combine(cache, "DynaMedicalRecords");
                    if (!Directory.Exists(directoryname))
                    {
                        Directory.CreateDirectory(directoryname);
                        Console.WriteLine("DynaMedicalRecords directory created : {0}", directoryname);
                    }
                    var foldername = Path.Combine(directoryname, downloadTask.CurrentRequest.Url.PathComponents[4]);
                    if (!Directory.Exists(foldername))
                    {
                        Directory.CreateDirectory(foldername);
                        Console.WriteLine("Patient directory created : {0}", foldername);
                    }
                    var fileidentity = Path.Combine(foldername, downloadTask.CurrentRequest.Url.PathComponents[5]);
                    if (fileManager.FileExists(fileidentity))
                    {
                        var existingFileInfo = new FileInfo(fileidentity);
                        var newFileInfo = new FileInfo(destinationURL.Path);
                        if (existingFileInfo.Length != newFileInfo.Length)
                        {
                            fileManager.Remove(fileidentity, out NSError errorRemove);
                            fileManager.Move(destinationURL.Path, fileidentity, out NSError errorExistMove);
                            Console.WriteLine("New file REPLACED to : {0}", fileidentity);
                        }
                        else
                        {
                            Console.WriteLine("New file matches old file, IGNORED : {0}", fileidentity);
                        }
                    }
                    else
                    {
                        fileManager.Move(destinationURL.Path, fileidentity, out NSError errorNewMove);
                        Console.WriteLine("New file SAVED to : {0}", fileidentity);
                    }

                    // we do not need to be on the main/UI thread to load the UIImage
                    //UIImage image = UIImage.FromFile(destinationURL.Path);
                    InvokeOnMainThread(() =>
                    {
                        //controller.ImageView.Image = image;
                        //controller.ImageView.Hidden = false;
                        controller.ProgressView.Hidden = true;
                        controller.BtnDownloadEnabled = true;
                    });
                }
                else
                {
                    Console.WriteLine("Error during the copy: {0}", errorCopy.LocalizedDescription);
                }
            }

            public bool IsDivisble(int x, int n)
            {
                return (x % n) == 0;
            }

            public override void DidCompleteWithError(NSUrlSession session, NSUrlSessionTask task, NSError error)
            {
                Console.WriteLine("DidComplete");
                if (error == null)
                    Console.WriteLine("Task: {0} completed successfully", task);
                else
                    Console.WriteLine("Task: {0} completed with error: {1}", task, error.LocalizedDescription);

                float progress = task.BytesReceived / (float)task.BytesExpectedToReceive;
                InvokeOnMainThread(() =>
                {
                    controller.ProgressView.Progress = progress;
                });

                var taskindex = controller.taskslist.FindIndex(t => t.TaskIdentifier == task.TaskIdentifier);
                if (taskindex < controller.taskslist.Count)
                {
                    controller.taskslist.RemoveAt(taskindex);
                }

                controller.downloadTask = null;
                //controller.downloadSession = null;

                if (controller.taskslist.Count >= 10)
                {
                    if (IsDivisble(controller.taskslist.Count, 10))
                    {
                        InvokeOnMainThread(() =>
                        {
                            var toast = new Toast(controller.taskslist.Count + " files left in download queue");
                            toast.SetDuration(5000);
                            toast.SetType(ToastType.Info);
                            toast.SetGravity(ToastGravity.Bottom);
                            toast.Show();
                        });
                    }
                }
                else
                {
                    //if (IsDivisble(controller.taskslist.Count, 1))
                    //{
                        InvokeOnMainThread(() =>
                        {
                        var toast = new Toast(controller.taskslist.Count + " files left in download queue");
                            toast.SetDuration(5000);
                            toast.SetType(ToastType.Info);
                            toast.SetGravity(ToastGravity.Bottom);
                            toast.Show();
                        });
                    //}
                }

                if (controller.taskslist.Count == 0)
                {
                    InvokeOnMainThread(() =>
                    {
                        controller.BtnDownload.Enabled = true;
                        controller.DownloadButtonGlobalEnabled = true;
                        //Toast toast = new Toast("A requested medical record was downloaded");
                        var toast = new Toast("The requested medical records have been downloaded");
                        toast.SetDuration(20000);
                        toast.SetType(ToastType.Info);
                        toast.SetGravity(ToastGravity.Bottom);
                        toast.Show();
                        //controller.PresentViewController(CommonFunctions.AlertPrompt("Download Completed", "The requested medical records have been downloaded", true, null, false, null), true, null);
                    });
                }
            }

            public override void DidResume(NSUrlSession session, NSUrlSessionDownloadTask downloadTask, long resumeFileOffset, long expectedTotalBytes)
            {
                Console.WriteLine("DidResume");
            }

            public override void DidFinishEventsForBackgroundSession(NSUrlSession session)
            {
                using (AppDelegate appDelegate = UIApplication.SharedApplication.Delegate as AppDelegate)
                {
                    var handler = appDelegate.BackgroundSessionCompletionHandler;
                    if (handler != null)
                    {
                        appDelegate.BackgroundSessionCompletionHandler = null;
                        handler();
                    }
                }

                Console.WriteLine("All tasks are finished");

                InvokeOnMainThread(() =>
                {
                    controller.BtnDownload.Enabled = true;

                    var toast = new Toast("The requested medical records have been downloaded");
                    toast.SetType(ToastType.Info);
                    toast.SetGravity(ToastGravity.Bottom);
                    toast.Show();
                    //controller.PresentViewController(CommonFunctions.AlertPrompt("Download Completed", "The requested medical records have been downloaded", true, null, false, null), true, null);
                });
            }
        }

        public async Task<string> ConfigureReportView(string context, string ReportName, string valueId, CancellationToken cts)
        {
            //await Task.Delay(TimeSpan.FromSeconds(2), cts);
            await Task.Delay(10, cts);

            try
            {
                //var Report = SelectedAppointment.ApptReports.Find((Report obj) => obj.FormId == sectionId);

                var reportElement = new DynaMultiRootElement(SelectedAppointment.ApptFormName);

                var reportPaddedView = new PaddedUIView<UILabel>
                {
                    Enabled = true,
                    Type = "Section",
                    Frame = new CGRect(0, 0, 0, 40),
                    Padding = 5f
                };
                reportPaddedView.NestedView.Text = ReportName;//"REPORT"; //Report.ReportName.ToUpper();
                reportPaddedView.NestedView.TextAlignment = UITextAlignment.Center;
                reportPaddedView.NestedView.Font = UIFont.BoldSystemFontOfSize(17);
                reportPaddedView.setStyle();

                var reportSection = new DynaSection("REPORT")
                {
                    HeaderView = reportPaddedView,
                    FooterView = new UIView(new CGRect(0, 0, 0, 0))
                };
                reportSection.FooterView.Hidden = true;

                var reportUrl = "";
                if (CrossConnectivity.Current.IsConnected)
                {
                    var dps = new DynaPadService.DynaPadService();
                    reportUrl = dps.GenerateReport(CommonFunctions.GetUserConfig(), SelectedAppointment.ApptId, SelectedAppointment.ApptFormId, DateTime.Today.ToShortDateString(), "file", valueId);
                    //string report = dps.GenerateReport("123", SelectedQForm.ApptPatientID, DateTime.Today.ToShortDateString(), "file", SelectedQForm.ApptPatientFormID);
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

                    var webView = new WKWebView(View.Bounds, new WKWebViewConfiguration())
                    {
                        Frame = new CGRect(View.Bounds.X, 0, View.Bounds.Width, View.Bounds.Height)
                    };
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
            }
            catch (Exception ex)
            {
                Root.Clear();
                Root.Add(CommonFunctions.ErrorDetailSection());
                ReloadData();
                CommonFunctions.sendErrorEmail(ex);
                PresentViewController(CommonFunctions.ExceptionAlertPrompt(ex), true, null);
                //throw new Exception(ex.Message + Environment.NewLine + ex.StackTrace, ex.InnerException);
            }

            //Console.WriteLine($"\nFinished ConfigureView");
            return context;
        }


        public async Task<string> ConfigureFinalizeView(string context, bool IsDoctorForm, CancellationToken cts)
        {
            //await Task.Delay(TimeSpan.FromSeconds(2), cts);
            await Task.Delay(10, cts);

            try
            {
                var rootElement = new DynaMultiRootElement(SelectedAppointment.SelectedQForm.FormName + " - " + SelectedAppointment.ApptPatientName);

                var rootPaddedView = new PaddedUIView<UILabel>
                {
                    Enabled = true,
                    Type = "Section",
                    Frame = new CGRect(0, 0, 0, 40),
                    Padding = 5f
                };
                rootPaddedView.NestedView.Text = "FINALIZE FORM";
                rootPaddedView.NestedView.TextAlignment = UITextAlignment.Center;
                rootPaddedView.NestedView.Font = UIFont.BoldSystemFontOfSize(17);
                rootPaddedView.setStyle();

                var rootSection = new DynaSection("FINALIZE FORM")
                {
                    HeaderView = rootPaddedView,
                    FooterView = new UIView(new CGRect(0, 0, 0, 0))
                };
                rootSection.FooterView.Hidden = true;

                var sigPad = new Xamarin.Controls.SignaturePadView(new CGRect(0, 0, View.Frame.Width, 600))
                {
                    CaptionText = "Signature here:",
                    BackgroundColor = UIColor.White
                };

                messageLabel = new UILabel();

                var btnSubmit = new GlassButton(new RectangleF(0, 0, (float)View.Frame.Width, 50))
                {
                    NormalColor = UIColor.Green,
                    DisabledColor = UIColor.Gray
                };
                btnSubmit.TitleLabel.Font = UIFont.BoldSystemFontOfSize(17);
                btnSubmit.SetTitle("Submit Form", UIControlState.Normal);
                btnSubmit.TouchUpInside += (sender, e) =>
                {
                    UIAlertController SubmitPrompt;

                    if (IsDoctorForm)
                    {
                        SubmitPrompt = UIAlertController.Create("Submit Form", "Submit the form?", UIAlertControllerStyle.Alert);
                        SubmitPrompt.Add(messageLabel);
                        SubmitPrompt.AddAction(UIAlertAction.Create("Yes", UIAlertActionStyle.Default, action => DoSubmitForm(null, IsDoctorForm, sigPad)));
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
                        SubmitPrompt.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, action => DoSubmitForm(SubmitPrompt.TextFields[0].Text, IsDoctorForm, sigPad)));
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
            }
            catch (Exception ex)
            {
                Root.Clear();
                Root.Add(CommonFunctions.ErrorDetailSection());
                ReloadData();
                CommonFunctions.sendErrorEmail(ex);
                PresentViewController(CommonFunctions.ExceptionAlertPrompt(ex), true, null);
                //throw new Exception(ex.Message + Environment.NewLine + ex.StackTrace, ex.InnerException);
            }

            //Console.WriteLine($"\nFinished ConfigureView");
            return context;
        }


        async void DoSubmitForm(string password, bool isDoctorForm, Xamarin.Controls.SignaturePadView sig)
        {
            try
            {
                var boundsh = base.TableView.Frame;
                loadingOverlay = new LoadingOverlay(boundsh, true);// { loadingLabelText = "Finalizing..." };
                loadingOverlay.SetText("Submitting a form may take a few minutes.", true, "Please wait patiently...");

                nfloat labelHeightF = 22;
                nfloat labelWidthF = 70;
                // derive the center x and y
                nfloat centerXF = loadingOverlay.Frame.Width / 2;
                nfloat centerYF = loadingOverlay.Frame.Height / 2;

                var cancelButtonF = new UIButton(UIButtonType.System)
                {
                    Frame = new CGRect(
                    centerXF - (labelWidthF / 2),
                    centerYF + 70,
                    labelWidthF,
                    labelHeightF)
                };
                cancelButtonF.SetTitle("Cancel", UIControlState.Normal);
                cancelButtonF.TouchUpInside += (sender, e) =>
                {
                    cts.Cancel();
                };

                loadingOverlay.AddSubview(cancelButtonF);
                mvc.Add(loadingOverlay);

                try
                {
                    cts?.Cancel();     // cancel previous search
                }
                catch (ObjectDisposedException oex)     // in case previous search completed
                {
                    Console.WriteLine($"\nObjectDisposedException in Submit with: {oex.Message}");
                }

                using (cts = new CancellationTokenSource())
                {
                    try
                    {
                        cts.CancelAfter(TimeSpan.FromSeconds(10));
                        //await Task.Delay(TimeSpan.FromSeconds(1), cts.Token);
                        await Task.Delay(10, cts.Token);

                        loadingOverlay.Hide();

                        var task = SubmitForm("Submit", password, isDoctorForm, sig, cts.Token, cancelButtonF);
                        var result = await task;
                        //Console.WriteLine($"\nGot section with: {result}");
                    }
                    catch (TaskCanceledException tex)       // if the operation is cancelled, do nothing
                    {
                        Console.WriteLine($"\nCanceled with: {tex.Message}");

                        //CommonFunctions.sendErrorEmail((Exception)tex);
                        loadingOverlay.Hide();
                        var master = (MasterViewController)((UINavigationController)SplitViewController.ViewControllers[0]).ViewControllers[0];

                        //foreach (Element d in master.secforcancel.Elements)
                        //{
                        //    var t = d.GetType();
                        //    if (t == typeof(SectionStringElement))
                        //    {
                        //        var di = (SectionStringElement)d;
                        //        if (di.IndexPath == master.oldsel)// di.prevsel)
                        //        {
                        //            di.selected = true;
                        //            //break;
                        //        }
                        //        else
                        //        {
                        //            di.selected = false;
                        //        }
                        //    }
                        //}
                        //master.secforcancel.GetContainerTableView().ReloadData();

                        PresentViewController(CommonFunctions.AlertPrompt("Canceled/Timeout", "Operation was canceled or timed out, please try again", true, null, false, null), true, null);
                    }
                }
            }
            catch (Exception ex)
            {
                //Root.Clear();
                //Root.Add(CommonFunctions.ErrorDetailSection());
                //ReloadData();
                loadingOverlay.Hide();
                CommonFunctions.sendErrorEmail(ex);
                PresentViewController(CommonFunctions.ExceptionAlertPrompt(ex), true, null);
            }
            //finally
            //{
            //    loadingOverlay.Hide();
            //}
        }


        public async Task<string> SubmitForm(string context, string password, bool isDoctorForm, Xamarin.Controls.SignaturePadView sig, CancellationToken cts, UIButton cancelButtonF)
        {
            //await Task.Delay(TimeSpan.FromSeconds(2), cts);
            await Task.Delay(10, cts);

            try
            {
                var boundsh = base.TableView.Frame;
                loadingOverlay = new LoadingOverlay(boundsh, true);// { loadingLabelText = "Finalizing..." };
                loadingOverlay.SetText("Saving form...", true);
                mvc.Add(loadingOverlay);

                ////var bounds = base.TableView.Frame;
                //loadingOverlay = new LoadingOverlay(SplitViewController.View.Bounds);// { loadingLabelText = "Submitting Form..."};
                //loadingOverlay.SetText("Submitting a form may take a few minutes. Please wait patiently...");
                ////mvc = (DialogViewController)((UINavigationController)SplitViewController.ViewControllers[1]).TopViewController;
                ////mvc.Add(loadingOverlay);
                //SplitViewController.Add(loadingOverlay);
                //await Task.Delay(10);

                //bool isValid = password == Constants.Password;
                bool isValid = false;
                bool isSigned = !sig.IsBlank;

                //for (int i = 0; i < Constants.Logins.GetLength(0); i++)
                //{
                //  if (SelectedAppointment.ApptLocationId == Constants.Logins[i, 2])
                //  {
                //      isValid |= password == Constants.Logins[i, 1];
                //  }
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

                //if (CrossConnectivity.Current.IsConnected)
                //{
                    if (isValid && isSigned)
                    {
                        var finalJson = JsonConvert.SerializeObject(SelectedAppointment.SelectedQForm);

                        var userid = DynaClassLibrary.DynaClasses.LoginContainer.User.UserId;
                        var userconfig = CommonFunctions.GetUserConfig();
                        var apptid = SelectedAppointment.ApptId;
                        var formid = SelectedAppointment.ApptFormId;
                        var doctorid = SelectedAppointment.ApptDoctorId;
                        var locationid = SelectedAppointment.ApptLocationId;
                        var patientid = SelectedAppointment.ApptPatientId;
                        var patientname = SelectedAppointment.ApptPatientName;
                        var isdoctorform = SelectedAppointment.SelectedQForm.IsDoctorForm;

                        //var dds = new DynaPadService.DynaPadService { Timeout = 60000 };

                        cancelButtonF.RemoveFromSuperview();

                        //dds.SubmitFormAnswers(CommonFunctions.GetUserConfig(), finalJson, true, isDoctorForm);
                        var formtype = "patient";
                        if (SelectedAppointment.SelectedQForm.IsDoctorForm)
                        {
                            formtype = "doctor";
                        }
                        string formFileName = "Form_" + formtype + "_" + SelectedAppointment.SelectedQForm.PatientName.Replace(" ", "") + "_" + DateTime.Now.ToString("yyyyMMdd");

                        var documentsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "DynaFilesAwaitingUpload/" + SelectedAppointment.ApptPatientId + "/" + SelectedAppointment.ApptId);
                        if (!Directory.Exists(documentsPath))
                        {
                            Directory.CreateDirectory(documentsPath);
                        }
                        var formFinalPath = Path.Combine(documentsPath, formFileName + ".txt");
                        if (File.Exists(formFinalPath))
                        {
                            File.Delete(formFinalPath);
                        }

                        var formDynaUpload = new DynaFile
                        {
                            FileName = formFileName,
                            UserId = DynaClassLibrary.DynaClasses.LoginContainer.User.UserId,
                            UserConfig = userconfig,
                            ApptId = apptid,
                            FormId = formid,
                            IsDoctorForm = isdoctorform,
                            DoctorId = doctorid,
                            LocationId = locationid,
                            PatientId = patientid,
                            PatientName = patientname,
                            Type = "Form",
                            Json = finalJson,
                            FileUrl = formFinalPath,
                            Status = "Submitted",
                            DateCreated = DateTime.Today
                        };

                        File.WriteAllText(formFinalPath, JsonConvert.SerializeObject(formDynaUpload)); // writes to local storage

                        var sigfilename = "Signature_" + formtype + "_" + SelectedAppointment.ApptPatientName.Replace(" ", "") + "_" + DateTime.Now.ToString("yyyyMMdd");
                        var sigfile = sig.GetImage(new CGSize(600, 400), true, true).AsPNG().ToArray();

                        var sigFinalPath = Path.Combine(documentsPath, sigfilename + ".txt");
                        if (File.Exists(sigFinalPath))
                        {
                            File.Delete(sigFinalPath);
                        }

                        var sigDynaUpload = new DynaFile
                        {
                            FileName = sigfilename,
                            UserId = DynaClassLibrary.DynaClasses.LoginContainer.User.UserId,
                            UserConfig = userconfig,
                            ApptId = apptid,
                            FormId = formid,
                            IsDoctorForm = isdoctorform,
                            DoctorId = doctorid,
                            LocationId = locationid,
                            PatientId = patientid,
                            PatientName = patientname,
                            Type = "Signature",
                            Bytes = sigfile,
                            FileUrl = sigFinalPath,
                            Status = "Submitted",
                            DateCreated = DateTime.Today
                        };

                        File.WriteAllText(sigFinalPath, JsonConvert.SerializeObject(sigDynaUpload)); // writes to local storage

                        //var fileid = dds.SaveFile(CommonFunctions.GetUserConfig(), SelectedAppointment.ApptId, SelectedAppointment.ApptPatientId, SelectedAppointment.ApptDoctorId, SelectedAppointment.ApptLocationId, filename, "Signature", "DynaPad", "", "", file, isDoctorForm, true);
                        //if (fileid.StartsWith("error", StringComparison.CurrentCulture))
                        //{
                        //    CommonFunctions.sendErrorEmail(new Exception("dds.SaveFile error: apptid = " + SelectedAppointment.ApptId));
                        //}

                        //loadingOverlay.Hide();

                        SetDetailItem(new Section("Summary"), "Summary", "", null, false);
                    }
                    else
                    {
                        //loadingOverlay.Hide();

                        messageLabel.Text = "Submit failed";
                        var failPass = "";
                        var failSign = "";
                        if (!isValid) failPass = "Wrong password. ";
                        if (!isSigned) failSign = "Form was not signed!";

                        PresentViewController(CommonFunctions.AlertPrompt("Error", failPass + failSign, true, null, false, null), true, null);
                    }
                //}
                //else
                //{
                //    loadingOverlay.Hide();

                //    PresentViewController(CommonFunctions.InternetAlertPrompt(), true, null);
                //}
            }
            catch (Exception ex)
            {
                //loadingOverlay.Hide();
                CommonFunctions.sendErrorEmail(ex);
                PresentViewController(CommonFunctions.ExceptionAlertPrompt(ex), true, null);
                //throw new Exception(ex.Message + Environment.NewLine + ex.StackTrace, ex.InnerException);
            }
            finally
            {
                loadingOverlay.Hide();
            }

            return context;
        }


        public async Task<string> ConfigureView(string context, string sectionId, string origS, bool IsDoctorForm, GlassButton nextbtn, CancellationToken cts)
        {

            //await Task.Delay(TimeSpan.FromSeconds(3), cts);
            await Task.Delay(10, cts);

            try
            {
                // Update the user interface for the detail item
                if (DetailItem != null)
                {
                    var sectionQuestions = SelectedAppointment.SelectedQForm.FormSections.Find((FormSection obj) => obj.SectionId == sectionId);

                    var headPaddedView = new PaddedUIView<UILabel>
                    {
                        Enabled = true,
                        Type = "Section",
                        Frame = new CGRect(0, 0, 0, 40),
                        Padding = 5f
                    };
                    headPaddedView.NestedView.Text = sectionQuestions.SectionName.ToUpper();
                    headPaddedView.NestedView.TextAlignment = UITextAlignment.Center;
                    headPaddedView.NestedView.Font = UIFont.BoldSystemFontOfSize(17);
                    headPaddedView.setStyle();

                    var headSection = new DynaSection(sectionQuestions.SectionName)
                    {
                        HeaderView = headPaddedView,
                        FooterView = new UIView(new CGRect(0, 0, 0, 0))
                    };
                    headSection.FooterView.Hidden = true;

                    if (IsDoctorForm)
                    {
                        // Notes/Dictation
                        var drawNavBtn = GetDrawNavBtn();
                        //var dicNavBtn = GetDicNavBtn(sectionId, IsDoctorForm);
                        var mrNavBtn = GetMRNavBtn();

                        //if (CrossConnectivity.Current.IsConnected)
                        //{
                        var picNavBtn = GetPhotoNavBtn(context, SelectedAppointment.ApptPatientName, SelectedAppointment.ApptId, SelectedAppointment.ApptPatientId, SelectedAppointment.ApptDoctorId, SelectedAppointment.ApptLocationId, IsDoctorForm);
                        //NavigationItem.SetLeftBarButtonItem(picNavBtn, true);
                        NavigationItem.SetLeftBarButtonItems(new UIBarButtonItem[] { picNavBtn, mrNavBtn }, true);
                        //}
                        //else
                        //{
                        //  PresentViewController(CommonFunctions.InternetAlertPrompt(), true, null);
                        //}

                        //NavigationItem.SetRightBarButtonItems(new UIBarButtonItem[] { dicNavBtn, drawNavBtn }, true);

                        // Presets
                        var presetPaddedView = new PaddedUIView<UILabel>
                        {
                            Enabled = true,
                            Type = "Preset",
                            Frame = new CGRect(0, 0, 0, 30),
                            Padding = 5f
                        };
                        presetPaddedView.NestedView.Text = "Section Presets";
                        presetPaddedView.setStyle();

                        var presetSection = new DynaSection("Section Presets")
                        {
                            Enabled = true,
                            HeaderView = presetPaddedView,
                            FooterView = new UIView(new CGRect(0, 0, 0, 0))
                        };
                        presetSection.FooterView.Hidden = true;

                        var fs = SelectedAppointment.SelectedQForm.FormSections.IndexOf(sectionQuestions);

                        //string[][] FormPresetNames = { };
                        //if (CrossConnectivity.Current.IsConnected)
                        //{
                        //var dds = new DynaPadService.DynaPadService() { Timeout = 60000 };
                        //FormPresetNames = dds.GetAnswerPresets(CommonFunctions.GetUserConfig(), SelectedAppointment.SelectedQForm.FormId, sectionId, SelectedAppointment.ApptDoctorId, SelectedAppointment.ApptLocationId);
                        var SectionPresets = GetPresetData(SelectedAppointment.ApptFormId, SelectedAppointment.ApptDoctorId, sectionId);
                        //}
                        //else
                        //{
                        //  PresentViewController(CommonFunctions.InternetAlertPrompt(), true, null);
                        //}

                        var presetGroup = new RadioGroup("PresetAnswers", sectionQuestions.SectionSelectedTemplateId);
                        var presetsRoot = new DynaRootElement("Section Presets", presetGroup)
                        {
                            IsPreset = true
                        };

                        var noPresetRadio = new PresetRadioElement("No Preset", "PresetAnswers")
                        {
                            PresetName = "No Preset"
                        };
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

                        foreach (var Preset in SectionPresets)
                        {
                            var mre = GetPreset(Preset.PresetId, Preset.PresetName, Preset.PresetJson, fs, sectionId, presetGroup, sectionQuestions, presetSection, origS, IsDoctorForm, nextbtn);

                            presetSection.Add(mre);
                        }

                        //foreach (string[] arrPreset in FormPresetNames)
                        //{
                        //    var mre = GetPreset(arrPreset[3], arrPreset[1], arrPreset[2], fs, sectionId, presetGroup, sectionQuestions, presetSection, origS, IsDoctorForm, nextbtn);

                        //    presetSection.Add(mre);
                        //}

                        var btnNewSectionPreset = new GlassButton(new RectangleF(0, 0, (float)View.Frame.Width, 50))
                        {
                            NormalColor = UIColor.FromRGB(224, 238, 240)
                        };
                        btnNewSectionPreset.TitleLabel.Font = UIFont.BoldSystemFontOfSize(17);
                        btnNewSectionPreset.SetTitleColor(UIColor.Black, UIControlState.Normal);
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
                    //  NavigationItem.SetRightBarButtonItem(null, false);
                    //}

                    QuestionsView = new DynaMultiRootElement(SelectedAppointment.SelectedQForm.FormName + " - " + SelectedAppointment.ApptPatientName)
                    {
                        headSection
                    };

                    foreach (SectionQuestion question in sectionQuestions.SectionQuestions)
                    {
                        bool enabled = !question.IsConditional || (question.IsConditional && question.IsEnabled);
                        var qSection = new DynaSection(question.QuestionText)
                        {
                            QuestionId = question.QuestionId,
                            Enabled = enabled,
                            IsInvalid = question.IsInvalid
                        };

                        //question.IsRequired = false;

                        nfloat qWidth = !IsDoctorForm ? View.Frame.Width - 50 : View.Frame.Width;
                        //if (question.IsRequired)
                        //{
                        //  qWidth = qWidth - 5;
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
                            var qsPaddedView = new PaddedUIView<UILabel>
                            {
                                Enabled = enabled,
                                Frame = new CGRect(0, 30, View.Frame.Width, 20),
                                Padding = 5f,
                                Type = "Subtitle"
                            };
                            qsPaddedView.NestedView.Text = question.Subtitle.ToUpper();
                            qsPaddedView.setStyle();

                            qSection.HeaderView.Add(qsPaddedView);
                        }

                        var qPaddedView = new PaddedUIView<UILabel>
                        {
                            Enabled = enabled,
                            Frame = new CGRect(0, 0, qWidth, cellHeight),
                            Padding = 5f,
                            Type = "Question",
                            Required = question.IsRequired
                        };
                        qPaddedView.NestedView.Text = question.QuestionText.ToUpper();
                        qPaddedView.setStyle();

                        if (!IsDoctorForm)
                        {
                            var qDictationButton = new UIButton(new CGRect(View.Frame.GetMaxX() - 50, 0, 50, 30))
                            {
                                Enabled = enabled
                            };
                            qDictationButton.SetImage(UIImage.FromBundle("QRecord"), UIControlState.Normal);

                            qDictationButton.BackgroundColor = enabled ? UIColor.FromRGB(230, 230, 250) : UIColor.GroupTableViewBackgroundColor;

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
                            //  var reqLbl = new UILabel(new CGRect(qWidth, 0, 5, cellHeight));
                            //  reqLbl.Text = "*";
                            //  reqLbl.TextColor = UIColor.Red;
                            //  reqLbl.BackgroundColor = UIColor.Clear;
                            //  qSection.Add(reqLbl);
                            //}

                            qSection.HeaderView.Add(qDictationButton);
                        }
                        else
                        {
                            qSection.HeaderView.Add(qPaddedView);

                            //if (question.IsRequired)
                            //{
                            //  var reqLbl = new UILabel(new CGRect(qWidth, 0, 5, cellHeight));
                            //  reqLbl.Text = "*";
                            //  reqLbl.TextColor = UIColor.Red;
                            //  reqLbl.BackgroundColor = UIColor.Clear;
                            //  qSection.Add(reqLbl);
                            //}
                        }

                        qSection.FooterView = new UIView(new CGRect(0, 0, 0, 0))
                        {
                            Hidden = true
                        };

                        switch (question.QuestionType)
                        {
                            case "BodyParts":
                            case "Check":

                                foreach (QuestionOption opt in question.QuestionOptions)
                                {
                                    var chk = new DynaCheckBoxElement(opt.OptionText, false, opt.OptionId)
                                    {
                                        Enabled = enabled,
                                        Required = question.IsRequired,
                                        Invalid = question.IsInvalid,
                                        ConditionTriggerId = question.ParentConditionTriggerId,
                                        Value = opt.Chosen
                                    };
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
                                    var radio = new DynaMultiRadioElement(opt.OptionText, question.QuestionId)
                                    {
                                        Enabled = enabled,
                                        Chosen = opt.Chosen,
                                        Required = question.IsRequired,
                                        Invalid = question.IsInvalid,
                                        ConditionTriggerId = question.ParentConditionTriggerId
                                    };
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

                                var viewEntryElement = new PlaceholderEnabledUITextView(new CGRect(0, 0, View.Frame.Width, 100))
                                {
                                    AutocorrectionType = UITextAutocorrectionType.No,
                                    SpellCheckingType = UITextSpellCheckingType.No,
                                    EnablesReturnKeyAutomatically = false,
                                    Placeholder = "Enter your answer here",
                                    Text = tvdval,
                                    Required = question.IsRequired,
                                    Invalid = question.IsInvalid,
                                    PlaceholderColor = UIColor.LightGray,
                                    Editable = true,
                                    Enabled = enabled,
                                    QuestionId = question.QuestionId,
                                    ConditionTriggerId = question.ParentConditionTriggerId,
                                    AllowWhiteSpace = true,
                                    parentSec = qSection
                                };
                                viewEntryElement.Ended += (sender, e) =>
                                {
                                    question.AnswerText = viewEntryElement.Text;
                                    viewEntryElement.Invalid = ValidateQuestion(question);
                                    if (qSection.GetContainerTableView() != null)
                                    {
                                        qSection.GetContainerTableView().ReloadData();
                                    }
                                };

                                if (!enabled)
                                {
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

                                var entryElement = new DynaEntryElement("", "Enter your answer here", tidval)
                                {
                                    QuestionKeyboardType = question.QuestionKeyboardTypeId,
                                    MaxChars = string.IsNullOrEmpty(question.MaxValue) ? 99 : Convert.ToInt16(question.MaxValue),
                                    Required = question.IsRequired,
                                    Invalid = question.IsInvalid,
                                    AutocorrectionType = UITextAutocorrectionType.No,
                                    ClearButtonMode = UITextFieldViewMode.Always,
                                    Enabled = enabled,
                                    QuestionId = question.QuestionId,
                                    ConditionTriggerId = question.ParentConditionTriggerId
                                };

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
                                    //  qSection.GetContainerTableView().ReloadData();
                                    //}
                                };

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
                                //  XmlDocument listxml = new XmlDocument();
                                //  //doc1.Load("https://amato.dynadox.pro/data/testautobox.txt");
                                //  listxml.Load(listurl);
                                //  XmlElement root = listxml.DocumentElement;
                                //  XmlNodeList nodes = root.SelectNodes("/Items/Item");
                                //  foreach (XmlNode node in nodes)
                                //  {
                                //      answerList.Add((NSString)node.Attributes[0].Value);
                                //  }
                                //}

                                GetAutoData(question.QuestionId);

                                var entryAutoComplete = new DynaAuto(new SFControlAutoCompleteDelegate(question, qSection))
                                {
                                    Text = atidval,
                                    //entryAutoComplete.AutoCompleteSource = answerList;
                                    DataSource = AutoDetails,
                                    ShowSuggestionsOnFocus = false,
                                    QuestionId = question.QuestionId,
                                    Frame = new CGRect(0, 0, View.Frame.Width, 30),
                                    AutosizesSubviews = true,
                                    BorderColor = UIColor.White,
                                    SuggestionBoxPlacement = SFAutoCompleteSuggestionBoxPlacement.SFAutoCompleteSuggestionBoxPlacementBottom,
                                    SuggestionMode = SFAutoCompleteSuggestionMode.SFAutoCompleteSuggestionModeContains,
                                    Watermark = (NSString)"Enter your answer here",
                                    MaxDropDownHeight = 150,
                                    AutoCompleteMode = SFAutoCompleteAutoCompleteMode.SFAutoCompleteAutoCompleteModeSuggest,
                                    IsEnabled = enabled,
                                    Enabled = enabled,
                                    ConditionTriggerId = question.ParentConditionTriggerId,
                                    parentSec = qSection,
                                    Required = question.IsRequired,
                                    Invalid = question.IsInvalid,
                                    DisplayMemberPath = "Text",
                                    SelectedValuePath = "Value",
                                    MinimumPrefixCharacters = 2
                                };
                                entryAutoComplete.TextField.BorderStyle = UITextBorderStyle.None;
                                entryAutoComplete.TextField.Bounds = new CGRect(0, 0, View.Frame.Width - 10, 30);
                                entryAutoComplete.Layer.BorderWidth = 0;
                                entryAutoComplete.TextField.ClearButtonMode = UITextFieldViewMode.Always;

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

                                var seg = new DynaSegmented
                                {
                                    Frame = new CGRect(0, 0, View.Frame.Width, 30),
                                    Required = question.IsRequired,
                                    Invalid = question.IsInvalid,
                                    Enabled = enabled,
                                    IsEnabled = enabled,
                                    QuestionId = question.QuestionId,
                                    ConditionTriggerId = question.ParentConditionTriggerId,
                                    parentSec = qSection
                                };

                                for (int i = 1; i <= 13; i++)
                                {
                                    seg.InsertSegment((i - 1).ToString(), i - 1, true);
                                }

                                seg.ValueChanged += (sender, e) =>
                                {
                                    var selectedSegmentId = (sender as UISegmentedControl).SelectedSegment;
                                    question.AnswerText = (sender as UISegmentedControl).TitleAt(selectedSegmentId);

                                };

                                if (!string.IsNullOrEmpty(htidval) && IsDigitsOnly(htidval))
                                {
                                    seg.SelectedSegment = Convert.ToInt16(htidval);// - 1;
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

                                if (question.QuestionType == "Height")
                                {
                                    questionmax = 12;
                                }
                                else if (question.QuestionType == "Weight")
                                {
                                    questionmax = 350;
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
                                var sliderElement = new DynaSlider(qanswer, question)
                                {
                                    MinValue = questionmin,
                                    MaxValue = questionmax,
                                    Increment = increment,
                                    ShowCaption = true,
                                    Caption = qanswer.ToString(),
                                    Enabled = enabled,
                                    Required = question.IsRequired,
                                    Invalid = question.IsInvalid,
                                    QuestionId = question.QuestionId,
                                    ConditionTriggerId = question.ParentConditionTriggerId
                                };

                                qSection.Add(sliderElement);
                                //QuestionsView.UnevenRows = true;
                                QuestionsView.Add(qSection);
                                break;
                            case "Grid":
                                var columns = new List<ItemColumn>
                                {
                                    new ItemColumn { Header = "first", Type = "Text", AnswerText = "dani" },
                                    new ItemColumn { Header = "last", Type = "Text", AnswerText = "harel" },
                                    new ItemColumn { Header = "ass", Type = "Switch", AnswerText = "true" }
                                };// question.QuestionRowItem.ItemColumns;
                                var rows = new List<QuestionRowItem>
                                {
                                    new QuestionRowItem { ItemColumns = columns }
                                };
                                var viewModel = new ViewModel(columns, rows);//(question.QuestionRowItem.ItemColumns, question.ItemRows);

                                var grid = new DynaGrid
                                {
                                    Frame = new CGRect(0, 0, View.Frame.Width, 190),
                                    Required = question.IsRequired,
                                    Invalid = question.IsInvalid,
                                    IsEnabled = enabled,
                                    QuestionId = question.QuestionId,
                                    ConditionTriggerId = question.ParentConditionTriggerId,
                                    parentSec = qSection,
                                    AutoGenerateColumns = false,
                                    ColumnSizer = ColumnSizer.LastColumnFill,
                                    SelectionMode = SelectionMode.None,
                                    AllowPullToRefresh = false,
                                    AllowSorting = false,
                                    AllowEditing = true,
                                    EditTapAction = TapAction.OnTap,
                                    EditorSelectionBehavior = EditorSelectionBehavior.SelectAll,
                                    ItemsSource = viewModel.DynamicCollection
                                };

                                grid.CellRenderers.Remove("TextView");
                                grid.CellRenderers.Add("TextView", new GridCellTextViewRendererExt());

                                grid.GridTapped += (object sender, GridTappedEventsArgs e) => { viewModel.RefreshGroup(columns[0].Header); };
                                grid.GridLoaded += (sender, e) => { grid.View.LiveDataUpdateMode = Syncfusion.Data.LiveDataUpdateMode.AllowDataShaping; };

                                //grid.CurrentCellEndEdit += (object sender, GridCurrentCellEndEditEventArgs args) =>
                                //{
                                //  var a = sender;
                                //  var b = args;
                                //  DynamicModel c = (DynaPad.DynamicModel)grid.GetRecordAtRowIndex(args.RowColumnIndex.RowIndex);
                                //  var d = grid.GetRecordAtRowIndex(args.RowColumnIndex.RowIndex);
                                //  //var e = c.Values[grid.Columns[args.RowColumnIndex.ColumnIndex].MappingName];
                                //  var n = viewModel.DynamicCollection[args.RowColumnIndex.RowIndex - 1].Values[grid.Columns[args.RowColumnIndex.ColumnIndex].MappingName.Substring(7, grid.Columns[args.RowColumnIndex.ColumnIndex].MappingName.Length - 8)].ToString();
                                //  var newvalue = grid.GetCellValue(viewModel.DynamicCollection[args.RowColumnIndex.RowIndex - 1], grid.Columns[args.RowColumnIndex.ColumnIndex].MappingName).ToString();
                                //  Debug.WriteLine(n);
                                //  Debug.WriteLine(newvalue);
                                //};

                                grid.CurrentCellEndEdit += async (object sender, GridCurrentCellEndEditEventArgs args) =>
                                {
                                    await Task.Delay(100);
                                    var a = sender;
                                    var b = args;
                                    var c = (DynamicModel)grid.GetRecordAtRowIndex(args.RowColumnIndex.RowIndex);
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
                                            var cText = new GridTextColumn
                                            {
                                                MappingName = "Values[" + c.Header.Replace(" ", "") + "]",
                                                HeaderText = c.Header,
                                                AllowEditing = true,
                                                HeaderTextAlignment = UITextAlignment.Left,
                                                TextAlignment = UITextAlignment.Left,
                                                LineBreakMode = UILineBreakMode.WordWrap
                                            };
                                            grid.Columns.Add(cText);
                                            break;
                                        case "Switch":
                                            var cSwitch = new GridSwitchColumn
                                            {
                                                MappingName = "Values[" + c.Header.Replace(" ", "") + "]",
                                                HeaderText = c.Header,
                                                AllowEditing = true,
                                                HeaderTextAlignment = UITextAlignment.Left,
                                                TextAlignment = UITextAlignment.Left
                                            };
                                            grid.Columns.Add(cSwitch);
                                            break;
                                        case "Numeric":
                                            var cNumeric = new GridNumericColumn
                                            {
                                                MappingName = "Values[" + c.Header.Replace(" ", "") + "]",
                                                HeaderText = c.Header,
                                                AllowEditing = true,
                                                HeaderTextAlignment = UITextAlignment.Left,
                                                TextAlignment = UITextAlignment.Left
                                            };
                                            grid.Columns.Add(cNumeric);
                                            break;
                                        case "Date":
                                            var cDate = new GridDateTimeColumn
                                            {
                                                MappingName = "Values[" + c.Header.Replace(" ", "") + "]",
                                                HeaderText = c.Header,
                                                AllowEditing = true,
                                                HeaderTextAlignment = UITextAlignment.Left,
                                                TextAlignment = UITextAlignment.Left
                                            };
                                            grid.Columns.Add(cDate);
                                            break;
                                        case "Picker":
                                            var cPicker = new GridPickerColumn
                                            {
                                                MappingName = "Values[" + c.Header.Replace(" ", "") + "]",
                                                HeaderText = c.Header,
                                                AllowEditing = true,
                                                HeaderTextAlignment = UITextAlignment.Left
                                            };
                                            var coptions = new ObservableCollection<string>();
                                            foreach (var o in c.Options)
                                            {
                                                coptions.Add(o);
                                            }
                                            grid.Columns.Add(cPicker);
                                            break;
                                    }
                                }

                                //Dummy column need to be added since we have internally checked grouping column against grid column collection.
                                grid.Columns.Add(new GridTextColumn
                                {
                                    HeaderText = "GroupProperty",
                                    MappingName = "GroupProperty",
                                    Width = 0
                                });

                                //Need to refresh the GroupProperty of DynamicModel in the given collection before applying groupdescription to the grid.
                                //viewModel.RefreshGroup(columns[0].Header);
                                //grid.GroupColumnDescriptions.Add(new GroupColumnDescription() { ColumnName = "GroupProperty" });

                                var addRowButton = new UIButton(new CGRect(0, 0, View.Frame.Width, 30))
                                {
                                    BackgroundColor = UIColor.LightGray
                                };
                                addRowButton.SetTitle("Add Row", UIControlState.Normal);
                                addRowButton.SetTitleColor(UIColor.Black, UIControlState.Normal);
                                addRowButton.TouchUpInside += (sender, e) =>
                                {
                                    viewModel.DynamicCollection.Add(new DynamicModel { Values = viewModel.GetDynamicModel(columns).Values });
                                };

                                qSection.Add(grid);
                                qSection.Add(addRowButton);
                                QuestionsView.Add(qSection);
                                break;
                        }
                        //question.ScrollY = qSection.HeaderView.Frame.Y;
                    }

                    var qNext = new DynaSection("Next")
                    {
                        HeaderView = new UIView(new CGRect(0, 0, 0, 10)),
                        FooterView = new UIView(new CGRect(0, 0, 0, 10))
                    };
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
                PresentViewController(CommonFunctions.ExceptionAlertPrompt(ex), true, null);
                //throw new Exception(ex.Message + Environment.NewLine + ex.StackTrace, ex.InnerException);
            }

            //Console.WriteLine($"\nFinished ConfigureView");
            return context;
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
                PresentViewController(CommonFunctions.ExceptionAlertPrompt(ex), true, null);
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
                PresentViewController(CommonFunctions.ExceptionAlertPrompt(ex), true, null);
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
                PresentViewController(CommonFunctions.ExceptionAlertPrompt(ex), true, null);
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
                PresentViewController(CommonFunctions.ExceptionAlertPrompt(ex), true, null);
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
                PresentViewController(CommonFunctions.ExceptionAlertPrompt(ex), true, null);
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
                                    default:
                                        headerLabel.NestedView.Text = tQuestion.QuestionText.ToUpper();
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
                                            //  headerDic.Enabled = triggered;
                                            //  if (headerDic.Enabled)
                                            //  {
                                            //      headerDic.BackgroundColor = UIColor.FromRGB(230, 230, 250);
                                            //  }
                                            //  else
                                            //  {
                                            //      headerDic.BackgroundColor = UIColor.GroupTableViewBackgroundColor;
                                            //  }
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
                PresentViewController(CommonFunctions.ExceptionAlertPrompt(ex), true, null);
                //throw new Exception(ex.Message + Environment.NewLine + ex.StackTrace, ex.InnerException);
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
                    var dds = new DynaPadService.DynaPadService { Timeout = 60000 };
                    dds.SaveAnswerPreset(CommonFunctions.GetUserConfig(), SelectedAppointment.SelectedQForm.FormId, sectionId, SelectedAppointment.ApptDoctorId, true, presetName, presetJson, SelectedAppointment.ApptLocationId, presetId);

                    var master = (MasterViewController)((UINavigationController)SplitViewController.ViewControllers[0]).ViewControllers[0];
                    master.SavePresetData();

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
                PresentViewController(CommonFunctions.ExceptionAlertPrompt(ex), true, null);
                //throw new Exception(ex.Message + Environment.NewLine + ex.StackTrace, ex.InnerException);
            }
        }


        //void DeleteSectionPreset(string presetId, string presetName, string sectionId, Section presetSection, PresetRadioElement pre, RadioGroup presetGroup, string origS, GlassButton nextbtn, bool isDoctorInput = true)
        void DeleteSectionPreset(string presetId, string sectionId, Section presetSection, PresetRadioElement pre)
        {
            try
            {
                if (CrossConnectivity.Current.IsConnected)
                {
                    var sectionQuestions = SelectedAppointment.SelectedQForm.FormSections.Find((FormSection obj) => obj.SectionId == sectionId);
                    var fs = SelectedAppointment.SelectedQForm.FormSections.IndexOf(sectionQuestions);

                    var presetJson = JsonConvert.SerializeObject(SelectedAppointment.SelectedQForm.FormSections[fs]);
                    var dds = new DynaPadService.DynaPadService { Timeout = 60000 };
                    dds.DeleteAnswerPreset(CommonFunctions.GetUserConfig(), SelectedAppointment.SelectedQForm.FormId, sectionId, SelectedAppointment.ApptDoctorId, presetId);

                    var presetPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "DynaPresets/" + SelectedAppointment.SelectedQForm.FormId + "/" + SelectedAppointment.ApptDoctorId + "/" + sectionId + "/" + presetId + ".txt");
                    if (File.Exists(presetPath))
                    {
                        File.Delete(presetPath);

                        var toast = new Toast("Preset file was deleted");
                        toast.SetDuration(5000);
                        toast.SetType(ToastType.Info);
                        toast.SetGravity(ToastGravity.Bottom);
                        toast.Show();
                    }

                    var master = (MasterViewController)((UINavigationController)SplitViewController.ViewControllers[0]).ViewControllers[0];
                    master.SavePresetData();

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
                PresentViewController(CommonFunctions.ExceptionAlertPrompt(ex), true, null);
                //throw new Exception(ex.Message + Environment.NewLine + ex.StackTrace, ex.InnerException);
            }
        }



        public PresetRadioElement GetPreset(string presetId, string presetName, string presetJson, int fs, string sectionId, RadioGroup presetGroup, FormSection sectionQuestions, Section presetSection, string origS, bool isDoctorInput, GlassButton nextbtn)
        {
            try
            {
                var mre = new PresetRadioElement(presetName, "PresetAnswers")
                {
                    PresetID = presetId,
                    PresetName = presetName,
                    PresetJson = presetJson
                };
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
                    UpdatePresetPrompt.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, action => DeleteSectionPreset(mre.PresetID, sectionId, presetSection, mre)));
                    UpdatePresetPrompt.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, null));
                    //Present Alert

                    PresentViewController(UpdatePresetPrompt, true, null);
                };

                return mre;
            }
            catch (Exception ex)
            {
                CommonFunctions.sendErrorEmail(ex);
                PresentViewController(CommonFunctions.ExceptionAlertPrompt(ex), true, null);
                return null;
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
            var viewModel_dynamicCollection = (ObservableCollection<DynamicModel>)grid.ItemsSource;
            var n = viewModel_dynamicCollection[args.RowColumnIndex.RowIndex - 1].Values[grid.Columns[args.RowColumnIndex.ColumnIndex].MappingName.Substring(7, grid.Columns[args.RowColumnIndex.ColumnIndex].MappingName.Length - 8)].ToString();
            var newvalue = grid.GetCellValue(viewModel_dynamicCollection[args.RowColumnIndex.RowIndex - 1], grid.Columns[args.RowColumnIndex.ColumnIndex].MappingName).ToString();
            //Console.Write(newvalue);
        }




        public bool URLExists(string url)
        {
            bool result = true;

            var webRequest = WebRequest.Create(url);
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
                //  question.AnswerText = "";
                //  (SFAutoComplete as DynaAuto).Text = "";
                //  qSection.GetContainerTableView().ReloadData();
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
                    //PresentViewController(CommonFunctions.ExceptionAlertPrompt(ex), true, null);
                    return false;
                    //throw new Exception(ex.Message + Environment.NewLine + ex.StackTrace, ex.InnerException);
                }
            }
        }

        //public class AutoData
        //{
        //  string Text;//Name;
        //  string Value;//Age;
        //  public AutoData(string Text, string Value)
        //  {
        //      this.Text = Text;
        //      this.Value = Value;
        //  }
        //  public string getText()
        //  {
        //      return Text;
        //  }
        //  public string getValue()
        //  {
        //      return Value;
        //  }
        //}

        public NSMutableArray AutoDetails
        {
            get;
            set;
        }


        void GetAutoData(string qid)
        {
            try
            {
                var array = new NSMutableArray();
                //array.Add(getDictionary("John", "24"));
                //array.Add(getDictionary("James", "37"));

                var documentsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "DynaAutoBoxes");
                string localFilename = qid + ".txt";
                var localPath = Path.Combine(documentsPath, localFilename);

                if (File.Exists(localPath))
                {
                    var listxml = new XmlDocument();
                    listxml.Load(localPath);
                    XmlElement root = listxml.DocumentElement;
                    var nodes = root.SelectNodes("/Items/Item");

                    foreach (XmlNode node in nodes)
                    {
                        array.Add(getDictionary(node.Attributes[0].Value, node.Attributes[1].Value));
                    }
                }

                AutoDetails = array;
            }
            catch (Exception ex)
            {
                CommonFunctions.sendErrorEmail(ex);
                PresentViewController(CommonFunctions.ExceptionAlertPrompt(ex), true, null);
            }
        }


        List<DynaPreset> GetPresetData(string formid, string doctorid, string sectionid)
        {
            var array = new List<DynaPreset>();

            var documentsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "DynaPresets/" + formid + "/" + doctorid + "/" + sectionid);

            if (Directory.Exists(documentsPath))
            {
                var files = Directory.GetFiles(documentsPath);

                foreach (string file in files)
                {
                    array.Add(JsonConvert.DeserializeObject<DynaPreset>(File.ReadAllText(file)));
                }
            }

            return array;
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
                        //Console.WriteLine("error");
                        CommonFunctions.sendNSErrorEmail(err);
                        PresentViewController(CommonFunctions.NSExceptionAlertPrompt(err), true, null);
                    }
                });
            }
            catch (Exception ex)
            {
                CommonFunctions.sendErrorEmail(ex);
                PresentViewController(CommonFunctions.ExceptionAlertPrompt(ex), true, null);
                //throw new Exception(ex.Message + Environment.NewLine + ex.StackTrace, ex.InnerException);
            }
        }





        public DynaMultiRootElement GetApptInfoElement()
        {
            try
            {
                var apptInfoElement = new DynaMultiRootElement(SelectedAppointment.ApptPatientName);

                var apptInfoPaddedView = new PaddedUIView<UILabel>
                {
                    Enabled = true,
                    Type = "Section",
                    Frame = new CGRect(0, 0, 0, 40),
                    Padding = 5f
                };
                apptInfoPaddedView.NestedView.Text = "Appointment Info - " + SelectedAppointment.ApptPatientName;
                apptInfoPaddedView.NestedView.TextAlignment = UITextAlignment.Center;
                apptInfoPaddedView.NestedView.Font = UIFont.BoldSystemFontOfSize(17);
                apptInfoPaddedView.setStyle();

                var apptInfoSection = new DynaSection("ApptInfo")
                {
                    HeaderView = new UIView(new CGRect(0, 0, 0, 0)),
                    FooterView = new UIView(new CGRect(0, 0, 0, 0))
                };
                apptInfoSection.FooterView.Hidden = true;

                var patientNamePaddedView = new PaddedUIView<UILabel>
                {
                    Frame = new CGRect(0, 0, View.Bounds.Width, 30),
                    Padding = 5f
                };
                patientNamePaddedView.NestedView.Text = "Patient Name:";
                patientNamePaddedView.setStyle();
                apptInfoSection.Add(patientNamePaddedView);
                var patientNameLabel = new UILabel(new CGRect(5, 0, View.Bounds.Width, 30)) { Text = SelectedAppointment.ApptPatientName };
                apptInfoSection.Add(patientNameLabel);

                var doctorNamePaddedView = new PaddedUIView<UILabel>
                {
                    Frame = new CGRect(0, 0, View.Bounds.Width, 30),
                    Padding = 5f
                };
                doctorNamePaddedView.NestedView.Text = "Doctor Name:";
                doctorNamePaddedView.setStyle();
                apptInfoSection.Add(doctorNamePaddedView);
                var doctorNameLabel = new UILabel(new CGRect(5, 0, View.Bounds.Width, 30)) { Text = SelectedAppointment.ApptDoctorName };
                apptInfoSection.Add(doctorNameLabel);

                var apptnotes = string.IsNullOrEmpty(SelectedAppointment.ApptNotes) ? "" : SelectedAppointment.ApptNotes;
                //var apptnotes = "Resolved pending breakpoint at 'DetailViewController.cs:4449,1' to DynaPad.DynaMultiRootElement DynaPad.DetailViewController.GetUploadElement (string valueId, string context). Resolved pending breakpoint.";

                var ww = (decimal)apptnotes.Length / 100;
                var wlines = (int)Math.Ceiling(ww);
                var fwlines = wlines == 0 ? 1 : wlines;
                var wheight = 30 * fwlines;

                var apptNotesPaddedView = new PaddedUIView<UILabel>
                {
                    Frame = new CGRect(0, 0, View.Bounds.Width, 30),
                    Padding = 5f
                };
                apptNotesPaddedView.NestedView.Text = "Appointment Notes:";
                apptNotesPaddedView.setStyle();
                apptInfoSection.Add(apptNotesPaddedView);
                var apptNotesLabel = new UILabel(new CGRect(5, 0, View.Bounds.Width - 10, wheight)) { Text = apptnotes, LineBreakMode = UILineBreakMode.WordWrap, Lines = 0 };
                apptInfoSection.Add(apptNotesLabel);

                apptInfoElement.Add(apptInfoSection);

                return apptInfoElement;
            }
            catch (Exception ex)
            {
                CommonFunctions.sendErrorEmail(ex);
                PresentViewController(CommonFunctions.ExceptionAlertPrompt(ex), true, null);
                return null;
                //throw new Exception(ex.Message + Environment.NewLine + ex.StackTrace, ex.InnerException);
            }
        }

        //SFBusyIndicator busyIndicator;

        public UIBarButtonItem GetMRNavBtn()
        {
            try
            {
                var navmr = new UIBarButtonItem(UIBarButtonSystemItem.Organize, (sender, args) =>
                {
                    var nlab = new UILabel(new CGRect(10, 0, View.Bounds.Width, 50)) { Text = "Medical Records: " + SelectedAppointment.ApptPatientName };

                    var ncellHeader = new UITableViewCell(UITableViewCellStyle.Default, null) { Frame = new CGRect(0, 0, View.Bounds.Width, 50) };

                    var nheadclosebtn = new UIButton(new CGRect(View.Bounds.Width, 0, 50, 50));
                    nheadclosebtn.SetImage(UIImage.FromBundle("Close"), UIControlState.Normal);

                    ncellHeader.ContentView.Add(nlab);
                    ncellHeader.ContentView.Add(nheadclosebtn);

                    var nsec = new Section(ncellHeader) { FooterView = new UIView(new CGRect(0, 0, 0, 0)) };
                    nsec.FooterView.Hidden = true;

                    var dynanroo = new DynaMultiRootElement();

                    var dds = new DynaPadService.DynaPadService { Timeout = 60000 };
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

                            var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                            var cache = Path.Combine(documents, "..", "Library", "Caches");
                            var directoryname = Path.Combine(cache, "DynaMedicalRecords");
                            var foldername = Path.Combine(directoryname, m.MRPatientId);
                            var fileidentity = Path.Combine(foldername, m.MRId + "." + m.MRFileType);
                            if (File.Exists(fileidentity))
                            {
                                m.IsLocal = true;
                            }

                        }
                        mrs.AddRange(SelectedAppointment.ApptMRFolders.Find(mr => mr.MRFolderId == mrf.MRFolderId).MrFolderMRs);
                    }

                    //busyIndicator = new SFBusyIndicator();

                    var fgrid = new SfDataGrid
                    {
                        Frame = new CGRect(0, 0, View.Bounds.Width, View.Bounds.Height),
                        ItemsSource = mrs,
                        AutoGenerateColumns = false,
                        ColumnSizer = ColumnSizer.None,
                        SelectionMode = SelectionMode.SingleDeselect
                    };
                    fgrid.GridDoubleTapped += DataGrid_GridDoubleTapped;
                    //fgrid.AllowPullToRefresh = true;
                    //fgrid.PullToRefreshCommand = new GridFormCommand(ExecutePullToRefreshFormCommand, fgrid);
                    //fgrid.AllowSorting = true;
                    //var ass = new Syncfusion.SfDataGrid.GroupColumnDescription();

                    var mrFolderColumn = new GridTextColumn
                    {
                        MappingName = "MRFolderName",
                        HeaderText = "Folder"
                    };
                    //mrFolderColumn.Width = fgrid.Frame.Width * 0.55;
                    //mrFolderColumn.HeaderTextAlignment = UITextAlignment.Left;
                    //mrFolderColumn.TextAlignment = UITextAlignment.Left;

                    var mrNameColumn = new GridTextColumn
                    {
                        MappingName = "MRName",
                        HeaderText = " File Name",
                        Width = fgrid.Frame.Width * 0.45,
                        //mrNameColumn.MinimumWidth = fgrid.Frame.Width * 0.40;
                        HeaderTextAlignment = UITextAlignment.Left,
                        TextAlignment = UITextAlignment.Left
                    };

                    var mrDateColumn = new GridTextColumn
                    {
                        MappingName = "MRApptDate",
                        HeaderText = "Appt Date",
                        Width = fgrid.Frame.Width * 0.15,
                        //mrDateColumn.MinimumWidth = fgrid.Frame.Width * 0.20;
                        HeaderTextAlignment = UITextAlignment.Left,
                        TextAlignment = UITextAlignment.Left
                    };

                    var mrDoctorColumn = new GridTextColumn
                    {
                        MappingName = "MRDoctor",
                        HeaderText = "Doctor",
                        Width = fgrid.Frame.Width * 0.10,
                        //mrDoctorColumn.MinimumWidth = fgrid.Frame.Width * 0.20;
                        HeaderTextAlignment = UITextAlignment.Left,
                        TextAlignment = UITextAlignment.Left
                    };

                    var mrLocationColumn = new GridTextColumn
                    {
                        MappingName = "MRLocation",
                        HeaderText = "Location",
                        Width = fgrid.Frame.Width * 0.10,
                        //mrLocationColumn.MinimumWidth = fgrid.Frame.Width * 0.20;
                        HeaderTextAlignment = UITextAlignment.Left,
                        TextAlignment = UITextAlignment.Left
                    };

                    var mrFileTypeColumn = new GridTextColumn
                    {
                        MappingName = "MRFileType",
                        HeaderText = "File Type",
                        Width = fgrid.Frame.Width * 0.10,
                        HeaderTextAlignment = UITextAlignment.Left,
                        TextAlignment = UITextAlignment.Left
                    };

                    var mrDownloadedColumn = new GridTextColumn
                    {
                        UserCellType = typeof(SwitchCell),
                        MappingName = "IsLocal",
                        HeaderText = "Downloaded",
                        Width = fgrid.Frame.Width * 0.10,
                        HeaderTextAlignment = UITextAlignment.Left,
                        TextAlignment = UITextAlignment.Left,
                        AllowEditing = false
                    };

                    fgrid.Columns.Add(mrFolderColumn);
                    fgrid.Columns.Add(mrNameColumn);
                    fgrid.Columns.Add(mrDoctorColumn);
                    fgrid.Columns.Add(mrLocationColumn);
                    fgrid.Columns.Add(mrFileTypeColumn);
                    fgrid.Columns.Add(mrDateColumn);
                    fgrid.Columns.Add(mrDownloadedColumn);

                    fgrid.GroupColumnDescriptions.Add(new GroupColumnDescription { ColumnName = "MRFolderName" });
                    fgrid.AllowGroupExpandCollapse = true;

                    nsec.Add(fgrid);

                    var nroo = new RootElement("MR")
                    {
                        nsec
                    };

                    var ndia = new DialogViewController(dynanroo)
                    {
                        ModalInPopover = true,
                        ModalPresentationStyle = UIModalPresentationStyle.PageSheet,
                        //PreferredContentSize = new CGSize(View.Bounds.Size)
                    };

                    dynanroo.Add(nsec);

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
                PresentViewController(CommonFunctions.ExceptionAlertPrompt(ex), true, null);
                return null;
                //throw new Exception(ex.Message + Environment.NewLine + ex.StackTrace, ex.InnerException);
            }
        }


        async void DataGrid_GridDoubleTapped(object sender, GridDoubleTappedEventsArgs e)
        {
            try
            {
                var boundsh = base.TableView.Frame;
                mvc = (DialogViewController)((UINavigationController)SplitViewController.ViewControllers[1]).TopViewController;
                loadingOverlay = new LoadingOverlay(boundsh, true);// { loadingLabelText = "Loading MR..." };
                loadingOverlay.SetText("Loading...");
                mvc.Add(loadingOverlay);

                await Task.Delay(10);

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

                    if (rowData.IsLocal)
                    {
                        var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                        var cache = Path.Combine(documents, "..", "Library", "Caches");
                        var directoryname = Path.Combine(cache, "DynaMedicalRecords");
                        var foldername = Path.Combine(directoryname, rowData.MRPatientId);
                        var fileidentity = Path.Combine(foldername, rowData.MRId + "." + rowData.MRFileType);

                        var PreviewController = UIDocumentInteractionController.FromUrl(NSUrl.FromFilename(fileidentity));
                        PreviewController.Delegate = new UIDocumentInteractionControllerDelegateClass(UIApplication.SharedApplication.KeyWindow.RootViewController);
                        BeginInvokeOnMainThread(() =>
                        {
                            PreviewController.PresentPreview(true);
                        });
                    }
                    else
                    {
                        //"https://test.dynadox.pro/data/test.dynadox.pro/claimantfiles/37/361.pdf"
                        //var wkurl = new NSUrl("https://test.dynadox.pro/data/test.dynadox.pro/claimantfiles/darwinmeds.pdf");
                        //var wkurl = new NSUrl(rowData.MRPath.Replace("https", "http"));//"https://test.dynadox.pro/data/test.dynadox.pro/claimantfiles/darwinmeds.pdf"
                        var wkurl = new NSUrl(rowData.MRPath);

                        var sfViewController = new SFSafariViewController(wkurl);
                        PresentViewController(sfViewController, true, null);
                    }
                }
            }
            catch (Exception ex)
            {
                var errordata = (MR)e.RowData;
                var errorfile = "<br/><br/><br/>FILE PATH:<br/><br/>" + errordata.MRPath;
                CommonFunctions.sendErrorEmail(ex, errorfile);
                PresentViewController(CommonFunctions.ExceptionAlertPrompt(ex), true, null);
            }
            finally
            {
                loadingOverlay.Hide();
            }
        }



        public class UIDocumentInteractionControllerDelegateClass : UIDocumentInteractionControllerDelegate
        {
            readonly UIViewController ownerVC;

            public UIDocumentInteractionControllerDelegateClass(UIViewController vc)
            {
                ownerVC = vc;
            }

            public override UIViewController ViewControllerForPreview(UIDocumentInteractionController controller)
            {
                return ownerVC;
            }

            public override UIView ViewForPreview(UIDocumentInteractionController controller)
            {
                return ownerVC.View;
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
                default:
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
                //await Task.Delay(new TimeSpan(0, 0, 5));
                await Task.Delay(10);
                ItemsSourceRefresh(dataGrid, valueId);
                dataGrid.IsBusy = false;
            }
            catch (Exception ex)
            {
                CommonFunctions.sendErrorEmail(ex);
                PresentViewController(CommonFunctions.ExceptionAlertPrompt(ex), true, null);
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

                var dds = new DynaPadService.DynaPadService { Timeout = 60000 };
                var origJson = dds.GetFiles(CommonFunctions.GetUserConfig(), SelectedAppointment.ApptId, SelectedAppointment.ApptPatientId, SelectedAppointment.ApptPatientName, SelectedAppointment.ApptDoctorId, SelectedAppointment.ApptLocationId);
                JsonHandler.OriginalFormJsonString = origJson;
                SelectedAppointment.ApptMRFolders = JsonConvert.DeserializeObject<List<MRFolder>>(origJson);
                var mrs = SelectedAppointment.ApptMRFolders.Find(mr => mr.MRFolderId == valueId).MrFolderMRs;
                dataGrid.ItemsSource = mrs;
            }
            catch (Exception ex)
            {
                CommonFunctions.sendErrorEmail(ex);
                PresentViewController(CommonFunctions.ExceptionAlertPrompt(ex), true, null);
                //throw new Exception(ex.Message + Environment.NewLine + ex.StackTrace, ex.InnerException);
            }
            //finally
            //{
            //    loadingOverlay.Hide();
            //}
        }


        public DynaMultiRootElement GetUploadElement(string valueId, string context)
        {
            try
            {
                var uploadElement = new DynaMultiRootElement("Upload Forms");

                var uploadPaddedView = new PaddedUIView<UILabel>
                {
                    Enabled = true,
                    Type = "Section",
                    Frame = new CGRect(0, 0, 0, 40),
                    Padding = 5f
                };
                uploadPaddedView.NestedView.Text = "Upload Forms - " + DynaClassLibrary.DynaClasses.LoginContainer.User.SelectedLocation.LocationName;
                uploadPaddedView.NestedView.TextAlignment = UITextAlignment.Center;
                uploadPaddedView.NestedView.Font = UIFont.BoldSystemFontOfSize(17);
                uploadPaddedView.setStyle();

                var uploadSection = new DynaSection("Upload")
                {
                    HeaderView = new UIView(new CGRect(0, 0, 0, 0)),
                    FooterView = new UIView(new CGRect(0, 0, 0, 0))
                };
                uploadSection.FooterView.Hidden = true;

                string documentsPath;

                if (context == "UploadSubmittedPatientForms")
                {
                    documentsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "DynaFilesAwaitingUpload/" + SelectedAppointment.ApptPatientId + "/" + SelectedAppointment.ApptId);
                }
                else
                {
                    documentsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "DynaFilesAwaitingUpload/");
                }

                var array = new List<DynaFile>();
                string[] files;

                if (Directory.Exists(documentsPath) && Directory.GetFiles(documentsPath, "*.*", SearchOption.AllDirectories).Length > 0)
                {
                    files = Directory.GetFiles(documentsPath, "*.*", SearchOption.AllDirectories);

                    foreach (string file in files)
                    {
                        string contents = File.ReadAllText(file);
                        var userconfig = CommonFunctions.GetUserConfig();

                        var filename = Path.GetFileName(file);
                        var filetype = filename.Substring(0, filename.IndexOf("_", StringComparison.CurrentCulture));

                        var upload = JsonConvert.DeserializeObject<DynaFile>(File.ReadAllText(file));

                        //var upload = new DynaFile
                        //{
                        //    FileName = filename,
                        //    UserId = DynaClassLibrary.DynaClasses.LoginContainer.User.UserId,
                        //    UserConfig = userconfig,
                        //    FormId = SelectedAppointment.ApptFormId,
                        //    DoctorId = SelectedAppointment.ApptDoctorId,
                        //    LocationId = SelectedAppointment.ApptLocationId,
                        //    PatientId = SelectedAppointment.ApptPatientId,
                        //    Type = filetype,
                        //    FileUrl = file,
                        //    DateCreated = File.GetCreationTime(file),
                        //    DateUploaded = DateTime.Today
                        //};
                        //switch (filetype)
                        //{
                        //    case "SubmittedForm":
                        //        upload.Json = File.ReadAllText(file);
                        //        break;
                        //    case "SubmittedSignature":
                        //        upload.Bytes = File.ReadAllBytes(file);
                        //        break;
                        //    case "SubmittedSummary":
                        //        upload.Html = File.ReadAllText(file);
                        //        break;
                        //}

                        array.Add(upload);
                    }

                    //var dds = new DynaPadService.DynaPadService { Timeout = 180000 };

                    //var toast = new Toast("The requested form(s) have been uploaded");
                    //toast.SetType(ToastType.Info);
                    //toast.SetGravity(ToastGravity.Bottom);
                    //toast.Show();
                }



                var btnUpload = new GlassButton(new RectangleF(0, 0, (float)View.Frame.Width, 50))
                {
                    NormalColor = UIColor.Green,
                    DisabledColor = UIColor.Gray
                };
                btnUpload.TitleLabel.Font = UIFont.BoldSystemFontOfSize(17);
                btnUpload.SetTitle("Upload All Files", UIControlState.Normal);
                btnUpload.TouchUpInside += (sender, e) =>
                {
                    UIAlertController UploadPrompt;

                    UploadPrompt = UIAlertController.Create("Upload Forms", "Upload forms? This will upload all forms to the server and generate reports, an internet connection is required!", UIAlertControllerStyle.Alert);
                    UploadPrompt.AddAction(UIAlertAction.Create("Yes", UIAlertActionStyle.Default, action => UploadSubmittedForms(new string[] { documentsPath }, context, true)));
                    UploadPrompt.AddAction(UIAlertAction.Create("No", UIAlertActionStyle.Cancel, null));
                    //Present Alert
                    PresentViewController(UploadPrompt, true, null);
                };

                uploadSection.Add(btnUpload);


                var uploadgrid = new SfDataGrid
                {
                    Frame = new CGRect(0, 0, View.Frame.Width, View.Frame.Height),
                    ItemsSource = array,
                    AutoGenerateColumns = false,
                    ColumnSizer = ColumnSizer.None,
                    SelectionMode = SelectionMode.Single,
                    AllowPullToRefresh = true,
                    AllowSorting = true
                };
                uploadgrid.GridDoubleTapped += UploadGrid_GridDoubleTapped;
                //uploadgrid.PullToRefreshCommand = new GridCommand(ExecuteUploadPullToRefreshCommand, uploadgrid, valueId);

                var uploaApptColumn = new GridTextColumn
                {
                    MappingName = "ApptId",
                    HeaderText = "Appointment",
                    Width = 0
                };

                var uploadNameColumn = new GridTextColumn
                {
                    MappingName = "FileName",
                    HeaderText = " File Name (Double tap to view)",
                    Width = uploadgrid.Frame.Width * 0.55,
                    HeaderTextAlignment = UITextAlignment.Left,
                    TextAlignment = UITextAlignment.Left
                };

                var uploadTypeColumn = new GridTextColumn
                {
                    MappingName = "Type",
                    HeaderText = "Type",
                    Width = uploadgrid.Frame.Width * 0.125,
                    HeaderTextAlignment = UITextAlignment.Left,
                    TextAlignment = UITextAlignment.Left
                };

                var uploadStatusColumn = new GridTextColumn
                {
                    MappingName = "Status",
                    HeaderText = "Status",
                    Width = uploadgrid.Frame.Width * 0.125,
                    HeaderTextAlignment = UITextAlignment.Left,
                    TextAlignment = UITextAlignment.Left
                };

                var uploadDateColumn = new GridTextColumn
                {
                    MappingName = "DateCreated",
                    HeaderText = "Date",
                    Width = uploadgrid.Frame.Width * 0.10,
                    HeaderTextAlignment = UITextAlignment.Left,
                    TextAlignment = UITextAlignment.Left
                };

                var uploadUploadColumn = new GridTextColumn
                {
                    UserCellType = typeof(UploadCell),
                    MappingName = "FileName",
                    HeaderText = "U/L",
                    Width = uploadgrid.Frame.Width * 0.10,
                    HeaderTextAlignment = UITextAlignment.Left,
                    TextAlignment = UITextAlignment.Left,
                    AllowEditing = false
                };

                uploadgrid.Columns.Add(uploaApptColumn);
                uploadgrid.Columns.Add(uploadNameColumn);
                uploadgrid.Columns.Add(uploadTypeColumn);
                uploadgrid.Columns.Add(uploadStatusColumn);
                uploadgrid.Columns.Add(uploadDateColumn);
                uploadgrid.Columns.Add(uploadUploadColumn);

                uploadgrid.GroupColumnDescriptions.Add(new GroupColumnDescription
                {
                    ColumnName = "ApptId"
                });
                uploadgrid.AllowGroupExpandCollapse = true;

                uploadSection.Add(uploadgrid);

                uploadElement.Add(uploadSection);

                return uploadElement;
            }
            catch (Exception ex)
            {
                CommonFunctions.sendErrorEmail(ex);
                PresentViewController(CommonFunctions.ExceptionAlertPrompt(ex), true, null);
                return null;
                //throw new Exception(ex.Message + Environment.NewLine + ex.StackTrace, ex.InnerException);
            }
        }

        public class UploadCell : GridCell
        {
            UIButton button;
            //UILabel label;

            public UploadCell()
            {
                button = new UIButton();
                this.AddSubview(button);
                //label = new UILabel();
                //this.AddSubview(label);
                CanRenderUnLoad = false;
            }

            protected override void UnLoad()
            {
                RemoveFromSuperview();
            }

            public override void LayoutSubviews()
            {
                base.LayoutSubviews();
                //button.Frame = new CGRect(Bounds.Left, Bounds.Top, Bounds.Width, Bounds.Height);
                this.button.SetTitle("Upload", UIControlState.Normal);
                this.button.BackgroundColor = UIColor.Gray;
                //this.label.Frame = new CGRect(Bounds.Left, Bounds.Top, Bounds.Width, Bounds.Height);
                //this.label.Text = DataColumn.CellValue.ToString();
                this.button.TouchUpInside += delegate {

                    var ass = DataColumn.CellValue.ToString();
                    var hole = DataColumn;
                    string documentsPath;

                    documentsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "DynaFilesAwaitingUpload/" + SelectedAppointment.ApptPatientId + "/" + SelectedAppointment.ApptId);

                    var array = new List<DynaFile>();
                    string[] files;

                    if (Directory.Exists(documentsPath) && Directory.GetFiles(documentsPath, "*.*", SearchOption.AllDirectories).Length > 0)
                    {
                        files = Directory.GetFiles(documentsPath, "*.*", SearchOption.AllDirectories);

                        foreach (string file in files)
                        {
                            string contents = File.ReadAllText(file);
                            var userconfig = CommonFunctions.GetUserConfig();

                            var filename = Path.GetFileName(file);
                            var filetype = filename.Substring(0, filename.IndexOf("_", StringComparison.CurrentCulture));

                            var upload = JsonConvert.DeserializeObject<DynaFile>(File.ReadAllText(file));

                            array.Add(upload);
                        }
                    }

                    //UploadSubmittedForms(new string[] { documentsPath });
                };
            }
        }

        async void UploadGrid_GridDoubleTapped(object sender, GridDoubleTappedEventsArgs e)
        {
            try
            {
                var boundsh = base.TableView.Frame;
                mvc = (DialogViewController)((UINavigationController)SplitViewController.ViewControllers[1]).TopViewController;
                loadingOverlay = new LoadingOverlay(boundsh, true);// { loadingLabelText = "Loading MR..." };
                loadingOverlay.SetText("Loading...");
                mvc.Add(loadingOverlay);

                await Task.Delay(10);

                if (e.RowData.GetType() == typeof(DynaFile))
                {
                    var rowIndex = e.RowColumnindex.RowIndex;
                    var rowData = (DynaFile)e.RowData;
                    var columnIndex = e.RowColumnindex.ColumnIndex;
                    var filepath = rowData.FileUrl;
                    var filetype = rowData.Type;

                    if (filepath.StartsWith("Error:", StringComparison.CurrentCulture))
                    {
                        //loadingOverlay.Hide();
                        PresentViewController(CommonFunctions.AlertPrompt("File Error", "File unavailable, contact administration", true, null, false, null), true, null);
                        return;
                    }


                    var documentsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "DynaFilesAwaitingUpload/" + rowData.PatientId + "/" + rowData.ApptId);
                    var fileidentity = Path.Combine(documentsPath, rowData.FileName + ".txt");
                    var assfiles = Directory.GetFiles(documentsPath);
                    if (File.Exists(fileidentity))
                    {
                        var file = JsonConvert.DeserializeObject<DynaFile>(File.ReadAllText(fileidentity));

                        UIWebView webView = new UIWebView(new CGRect(View.Bounds.X + 5, View.Bounds.Y + 60, View.Bounds.Width - 5, View.Bounds.Height - 80))
                        {
                            ScalesPageToFit = true
                        };

                        switch (filetype)
                        {
                            case "Form":
                                webView.LoadHtmlString(file.Json, new NSUrl(documentsPath, true));
                                break;
                            case "Signature":
                                webView.LoadHtmlString(file.Bytes.ToString(), new NSUrl(documentsPath, true));
                                break;
                            case "Summary":
                                webView.LoadHtmlString(file.Html, new NSUrl(documentsPath, true));
                                break;
                        }
                    }
                    else
                    {
                        //loadingOverlay.Hide();
                        PresentViewController(CommonFunctions.AlertPrompt("File Error", "File unavailable, contact administration", true, null, false, null), true, null);
                        return;
                    }

                    //BeginInvokeOnMainThread(() =>
                    //{
                    //    PreviewController.PresentPreview(true);
                    //});

                    //loadingOverlay.Hide();
                }
            }
            catch (Exception ex)
            {
                var errordata = (MR)e.RowData;
                var errorfile = "<br/><br/><br/>FILE PATH:<br/><br/>" + errordata.MRPath;
                CommonFunctions.sendErrorEmail(ex, errorfile);
                PresentViewController(CommonFunctions.ExceptionAlertPrompt(ex), true, null);
            }
            finally
            {
                loadingOverlay.Hide();
            }
        }






        public class SwitchCell : GridCell
        {
            UISwitch gridswitch;
            //UILabel label;

            public SwitchCell()
            {
                gridswitch = new UISwitch();
                this.AddSubview(gridswitch);
                //label = new UILabel();
                //this.AddSubview(label);
                CanRenderUnLoad = false;
            }

            protected override void UnLoad()
            {
                RemoveFromSuperview();
            }

            public override void LayoutSubviews()
            {
                base.LayoutSubviews();
                //button.Frame = new CGRect(Bounds.Left, Bounds.Top, Bounds.Width, Bounds.Height);
                this.gridswitch.Enabled = false;
                //this.gridswitch.BackgroundColor = UIColor.Gray;
                //this.label.Frame = new CGRect(Bounds.Left, Bounds.Top, Bounds.Width, Bounds.Height);
                //this.label.Text = DataColumn.CellValue.ToString();
                this.gridswitch.On = (bool)DataColumn.CellValue;
            }
        }



        public DynaMultiRootElement GetMRElement(string valueId)
        {
            try
            {
                var mrElement = new DynaMultiRootElement(SelectedAppointment.ApptFormName);

                var mrPaddedView = new PaddedUIView<UILabel>
                {
                    Enabled = true,
                    Type = "Section",
                    Frame = new CGRect(0, 0, 0, 40),
                    Padding = 5f
                };
                mrPaddedView.NestedView.Text = "MR - " + SelectedAppointment.ApptPatientName;
                mrPaddedView.NestedView.TextAlignment = UITextAlignment.Center;
                mrPaddedView.NestedView.Font = UIFont.BoldSystemFontOfSize(17);
                mrPaddedView.setStyle();

                var mrSection = new DynaSection("MR")
                {
                    //mrSection.HeaderView = mrPaddedView;
                    HeaderView = new UIView(new CGRect(0, 0, 0, 0)),
                    FooterView = new UIView(new CGRect(0, 0, 0, 0))
                };
                mrSection.FooterView.Hidden = true;

                var mrs = SelectedAppointment.ApptMRFolders.Find(mr => mr.MRFolderId == valueId).MrFolderMRs;



                foreach (MR mr in mrs)
                {
                    var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                    var cache = Path.Combine(documents, "..", "Library", "Caches");
                    var directoryname = Path.Combine(cache, "DynaMedicalRecords");
                    var foldername = Path.Combine(directoryname, mr.MRPatientId);
                    var fileidentity = Path.Combine(foldername, mr.MRId + "." + mr.MRFileType);
                    if (File.Exists(fileidentity))
                    {
                        mr.IsLocal = true;
                    }
                }




                var fgrid = new SfDataGrid
                {
                    Frame = new CGRect(0, 0, View.Frame.Width, View.Frame.Height),
                    ItemsSource = mrs,
                    AutoGenerateColumns = false,
                    ColumnSizer = ColumnSizer.None,
                    SelectionMode = SelectionMode.Single,
                    AllowPullToRefresh = true,
                    AllowSorting = true
                };
                fgrid.GridDoubleTapped += DataGrid_GridDoubleTapped;
                //fgrid.AutoGeneratingColumn += GridAutoGeneratingColumns;
                //fgrid.BackgroundColor = UIColor.Green;
                fgrid.PullToRefreshCommand = new GridCommand(ExecutePullToRefreshCommand, fgrid, valueId);
                //fgrid.View.LiveDataUpdateMode = Syncfusion.Data.LiveDataUpdateMode.AllowDataShaping;

                var mrNameColumn = new GridTextColumn
                {
                    MappingName = "MRName",
                    HeaderText = " File Name (Double tap to view)",
                    Width = fgrid.Frame.Width * 0.45,
                    HeaderTextAlignment = UITextAlignment.Left,
                    TextAlignment = UITextAlignment.Left
                };

                var mrDoctorColumn = new GridTextColumn
                {
                    MappingName = "MRDoctor",
                    HeaderText = "Doctor",
                    Width = fgrid.Frame.Width * 0.15,
                    //mrDoctorColumn.MinimumWidth = fgrid.Frame.Width * 0.20;
                    HeaderTextAlignment = UITextAlignment.Left,
                    TextAlignment = UITextAlignment.Left
                };

                var mrLocationColumn = new GridTextColumn
                {
                    MappingName = "MRLocation",
                    HeaderText = "Location",
                    Width = fgrid.Frame.Width * 0.10,
                    //mrLocationColumn.MinimumWidth = fgrid.Frame.Width * 0.20;
                    HeaderTextAlignment = UITextAlignment.Left,
                    TextAlignment = UITextAlignment.Left
                };

                var mrFileTypeColumn = new GridTextColumn
                {
                    MappingName = "MRFileType",
                    HeaderText = "File Type",
                    Width = fgrid.Frame.Width * 0.10,
                    HeaderTextAlignment = UITextAlignment.Left,
                    TextAlignment = UITextAlignment.Left
                };

                var mrDateColumn = new GridTextColumn
                {
                    MappingName = "MRApptDate",
                    HeaderText = "Appt Date",
                    Width = fgrid.Frame.Width * 0.10,
                    HeaderTextAlignment = UITextAlignment.Left,
                    TextAlignment = UITextAlignment.Left
                };

                //var mrDownloadedColumn = new GridSwitchColumn
                //{
                //    MappingName = "IsLocal",
                //    HeaderText = "Downloaded",
                //    Width = fgrid.Frame.Width * 0.10,
                //    HeaderTextAlignment = UITextAlignment.Left,
                //    TextAlignment = UITextAlignment.Left,
                //    AllowEditing = false
                //};

                var mrDownloadedColumn = new GridTextColumn
                {
                    UserCellType = typeof(SwitchCell),
                    MappingName = "IsLocal",
                    HeaderText = "Downloaded",
                    Width = fgrid.Frame.Width * 0.10,
                    HeaderTextAlignment = UITextAlignment.Left,
                    TextAlignment = UITextAlignment.Left,
                    AllowEditing = false
                };

                fgrid.Columns.Add(mrNameColumn);
                fgrid.Columns.Add(mrDoctorColumn);
                fgrid.Columns.Add(mrLocationColumn);
                fgrid.Columns.Add(mrFileTypeColumn);
                fgrid.Columns.Add(mrDateColumn);
                fgrid.Columns.Add(mrDownloadedColumn);

                mrSection.Add(fgrid);
                mrElement.Add(mrSection);

                return mrElement;
            }
            catch (Exception ex)
            {
                CommonFunctions.sendErrorEmail(ex);
                PresentViewController(CommonFunctions.ExceptionAlertPrompt(ex), true, null);
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
        //        PresentViewController(CommonFunctions.ExceptionAlertPrompt(ex), true, null);
        //    }
        //}
        //WKWebView MRWebViews;
        //UIProgressView progressViews;






        public string GenerateSummary(string answers)
        {
            try
            {
                SelectedAppointment.AnsweredQForm = JsonConvert.DeserializeObject<QForm>(answers);

                //var claimantPathVirtual = domainConfig.DomainClaimantsPathVirtual + SelectedAppointment.AnsweredQForm.PatientId + "/"; // + "Summaries"
                //var claimantPathPhysical = domainConfig.DomainClaimantsPathPhysical + SelectedAppointment.AnsweredQForm.PatientId + "/"; // + "Summaries"

                var type = "patient";
                if (SelectedAppointment.AnsweredQForm.IsDoctorForm)
                {
                    type = "doctor";
                }

                //var filePathVirtual = Path.Combine(claimantPathVirtual, "Summary_" + type + "_" + SelectedAppointment.AnsweredQForm.PatientName + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".pdf");
                //var filePathPhysical = Path.Combine(claimantPathPhysical, "Summary_" + type + "_" + SelectedAppointment.AnsweredQForm.PatientName + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".pdf");

                //iTextSharp.text.Document document = new iTextSharp.text.Document();
                //PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(filePathPhysical, FileMode.Create));
                //document.Open();



                string myHtmlFile = "";
                var myBuilder = new System.Text.StringBuilder();

                myBuilder.Append("<html xmlns='http://www.w3.org/1999/xhtml'>");
                myBuilder.Append("<head>");
                myBuilder.Append("<meta charset='utf-8'/>");
                myBuilder.Append("<title>");
                myBuilder.Append("Summary");
                myBuilder.Append("</title>");
                myBuilder.Append("<style>");
                myBuilder.Append("@page { margin: 0; padding: 15mm; }");
                myBuilder.Append("body { margin: 0 }");
                myBuilder.Append(".sheet {");
                myBuilder.Append("margin: 0;");
                myBuilder.Append("overflow: hidden;");
                myBuilder.Append("position: relative;");
                myBuilder.Append("box - sizing: border - box;");
                myBuilder.Append("page -break-after: always;");
                myBuilder.Append("}");
                myBuilder.Append("body.A4.sheet { width: 210mm; height: 296mm }");
                myBuilder.Append(".sheet.padding - 10mm { padding: 10mm }");
                myBuilder.Append(".sheet.padding - 15mm { padding: 15mm }");
                myBuilder.Append(".sheet.padding - 20mm { padding: 20mm }");
                myBuilder.Append(".sheet.padding - 25mm { padding: 25mm }");
                myBuilder.Append("@media screen {");
                myBuilder.Append("body {");
                myBuilder.Append("background: white }");
                myBuilder.Append(".sheet {");
                myBuilder.Append("background: white;");
                myBuilder.Append("box - shadow: 0 .5mm 2mm rgba(0,0,0,.3);");
                myBuilder.Append("margin: 5mm auto;");
                myBuilder.Append("}");
                myBuilder.Append("}");
                myBuilder.Append("@media print {");
                myBuilder.Append("body.A4 { width: 210mm }");
                myBuilder.Append("}");
                myBuilder.Append("</style>");
                myBuilder.Append("</head>");
                myBuilder.Append("<body class=A4>");
                myBuilder.Append("<table border='1px' cellpadding='5' cellspacing='0' ");
                myBuilder.Append("style='border: solid 1px Silver; font-size: x-small;'>");

                myBuilder.Append("<section class='sheet padding-10mm'>");
                myBuilder.Append("<table style='width: 100%; border-collapse: collapse; margin-bottom: 5px;'>");
                myBuilder.Append("<tr style='border-bottom: 1px solid black;'>");
                myBuilder.Append("<td colspan='2'><b>Form Information</b></td>");
                myBuilder.Append("</tr></table>");

                myBuilder.Append("<table style='background-color: lightgray; width: 100%; border-collapse: collapse; margin-bottom: 15px;'>");
                myBuilder.Append("<tr>");
                myBuilder.Append("<td style='width: 100px;'>Patient Name:</td>");
                myBuilder.Append("<td>" + SelectedAppointment.AnsweredQForm.PatientName + "</td>");
                myBuilder.Append("</tr>");
                myBuilder.Append("<tr>");
                myBuilder.Append("<td style='width: 100px;'>Doctor Name:</td>");
                myBuilder.Append("<td>" + SelectedAppointment.ApptDoctorId + "</td>");
                myBuilder.Append("</tr>");
                myBuilder.Append("<tr>");
                myBuilder.Append("<td style='width: 100px;'>Date Created:</td>");
                myBuilder.Append("<td>" + DateTime.Today.ToShortDateString() + "</td>");
                myBuilder.Append("</tr>");
                myBuilder.Append("<tr>");
                myBuilder.Append("<td style='width: 100px;'>Location:</td>");
                myBuilder.Append("<td>" + SelectedAppointment.ApptLocationId + "</td>");
                myBuilder.Append("</tr>");
                myBuilder.Append("</table>");



                //System.Data.DataTable dtFormHeader = new System.Data.DataTable();
                //dtFormHeader.Columns.Add();
                //dtFormHeader.Rows.Add("Form Information");

                //AddTableToPdf(dtFormHeader, document, false, true);



                //string docName = SelectedAppointment.ApptDoctorId;
                //string location = SelectedAppointment.ApptLocationId;
                //Header and appt info
                //System.Data.DataTable dtHeader = new System.Data.DataTable();
                //dtHeader.Columns.Add();
                //dtHeader.Columns.Add();
                //dtHeader.Rows.Add("Patient Name: ", SelectedAppointment.AnsweredQForm.PatientName);
                //dtHeader.Rows.Add("Doctor Name: ", docName);
                //dtHeader.Rows.Add("Date Created: ", DateTime.Today.ToShortDateString());
                //dtHeader.Rows.Add("Location: ", location);

                //AddTableToPdf(dtHeader, document, true, false);


                foreach (FormSection section in SelectedAppointment.AnsweredQForm.FormSections)
                {
                    myBuilder.Append("<table style='width: 100%; border-collapse: collapse; margin-bottom: 5px;'>");
                    myBuilder.Append("<tr style='border-bottom: 1px solid black; padding-bottom: 5px;'>");
                    myBuilder.Append("<td><b>" + section.SectionName + "</b></td>");
                    myBuilder.Append("</tr></table>");

                    //System.Data.DataTable dtSectionHeader = new System.Data.DataTable();
                    //dtSectionHeader.Columns.Add();
                    //dtSectionHeader.Rows.Add(section.SectionName);

                    //AddTableToPdf(dtSectionHeader, document, false, true);

                    //System.Data.DataTable dt = new System.Data.DataTable();
                    //dt.Columns.Add();
                    //dt.Columns.Add();


                    myBuilder.Append("<table style='width: 100%; border-collapse: collapse; margin-bottom: 15px;'>");

                    int qIndex = 1;

                    foreach (SectionQuestion question in section.SectionQuestions)
                    {
                        if (!question.IsEnabled) continue;
                        if (question.QuestionOptions != null && question.QuestionOptions.Count > 0)
                        {
                            bool rowAdded = false;
                            foreach (QuestionOption option in question.QuestionOptions)
                            {
                                if (option.Chosen)
                                {
                                    if (!rowAdded)
                                    {
                                        myBuilder.Append("<tr>");
                                        myBuilder.Append("<td style='width: 25px; padding-bottom: 5px;'>" + qIndex + ".</td>");
                                        myBuilder.Append("<td style='padding-bottom: 5px;'>" + question.QuestionText + "</td>");
                                        myBuilder.Append("</tr>");
                                        myBuilder.Append("<tr>");
                                        myBuilder.Append("<td style='padding-bottom: 5px;'>&nbsp;</td>");
                                        myBuilder.Append("<td style='padding-bottom: 5px;'> - " + option.OptionText + "</td>");
                                        myBuilder.Append("</tr>");


                                        //dt.Rows.Add(qIndex + ".", question.QuestionText);
                                        //dt.Rows.Add("", "   - " + option.OptionText);

                                        rowAdded = true;

                                        qIndex++;
                                    }
                                    else
                                    {
                                        myBuilder.Append("<tr>");
                                        myBuilder.Append("<td style='padding-bottom: 5px;'>&nbsp;</td>");
                                        myBuilder.Append("<td style='padding-bottom: 5px;'> - " + option.OptionText + "</td>");
                                        myBuilder.Append("</tr>");

                                        //dt.Rows.Add("", "   - " + option.OptionText);
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(question.AnswerText))
                            {
                                myBuilder.Append("<tr>");
                                myBuilder.Append("<td style='width: 25px; padding-bottom: 5px;'>" + qIndex + ".</td>");
                                myBuilder.Append("<td style='padding-bottom: 5px;'>" + question.QuestionText + "</td>");
                                myBuilder.Append("</tr>");
                                myBuilder.Append("<tr>");
                                myBuilder.Append("<td style='padding-bottom: 5px;'>&nbsp;</td>");
                                myBuilder.Append("<td style='padding-bottom: 5px;'> - " + question.AnswerText + "</td>");
                                myBuilder.Append("</tr>");


                                //dt.Rows.Add(qIndex + ".", question.QuestionText);
                                //dt.Rows.Add("", "   - " + question.AnswerText);

                                qIndex++;
                            }
                        }
                    }

                    myBuilder.Append("</table>");

                    //AddTableToPdf(dt, document, false, false);
                }

                //document.Close();



                myBuilder.Append("</body>");
                myBuilder.Append("</html>");

                myHtmlFile = myBuilder.ToString();




                //disabled grey out (label and input) - like boolean
                //text alligned right no bueno
                //bold radio, check
                // check confirm button
                //extra section causing extra grey
                string fileName = "Summary_" + type + "_" + SelectedAppointment.AnsweredQForm.PatientName.Replace(" ", "") + "_" + DateTime.Now.ToString("yyyyMMdd");
                //string fileId = SaveFile(domainConfig, SelectedAppointment.AnsweredQForm.ApptId, SelectedAppointment.AnsweredQForm.PatientId,
                //   SelectedAppointment.AnsweredQForm.DoctorId,
                //   SelectedAppointment.AnsweredQForm.LocationId,
                //   fileName + ".pdf", "Summary", "Summaries", filePathPhysical, filePathVirtual, File.ReadAllBytes(filePathPhysical), SelectedAppointment.AnsweredQForm.IsDoctorForm);
                //string finalPath = Path.Combine(claimantPathPhysical, fileName + ".pdf");

                var documentsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "DynaFilesAwaitingUpload/" + SelectedAppointment.ApptPatientId + "/" + SelectedAppointment.ApptId);
                if (!Directory.Exists(documentsPath))
                {
                    Directory.CreateDirectory(documentsPath);
                }
                var finalPath = Path.Combine(documentsPath, fileName + ".txt");
                if (File.Exists(finalPath))
                {
                    File.Delete(finalPath);
                }

                var summaryDynaUpload = new DynaFile
                {
                    FileName = fileName,
                    UserId = DynaClassLibrary.DynaClasses.LoginContainer.User.UserId,
                    UserConfig = CommonFunctions.GetUserConfig(),
                    ApptId = SelectedAppointment.ApptId,
                    FormId = SelectedAppointment.ApptFormId,
                    IsDoctorForm = SelectedAppointment.SelectedQForm.IsDoctorForm,
                    DoctorId = SelectedAppointment.ApptDoctorId,
                    LocationId = SelectedAppointment.ApptLocationId,
                    PatientId = SelectedAppointment.ApptPatientId,
                    PatientName = SelectedAppointment.ApptPatientName,
                    Type = "Summary",
                    Html = myHtmlFile,
                    FileUrl = finalPath,
                    Status = "Submitted",
                    DateCreated = DateTime.Today
                };

                File.WriteAllText(finalPath, JsonConvert.SerializeObject(summaryDynaUpload)); // writes to local storage

                return finalPath;
            }
            catch (Exception ex)
            {
                CommonFunctions.sendErrorEmail(ex);
                PresentViewController(CommonFunctions.ExceptionAlertPrompt(ex), true, null);
                return "error: " + ex.Message;
            }
        }


        //void AddTableToPdf(System.Data.DataTable dt, iTextSharp.text.Document document, bool isHeader, bool isSectionHeader)
        //{
        //    PdfPTable table = new PdfPTable(dt.Columns.Count);
        //    iTextSharp.text.Font font5 = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA, 10);
        //    //PdfPRow row = null;

        //    float[] widths;
        //    if (isHeader)
        //    {
        //        widths = new float[] { 215f, 995f };
        //        table.DefaultCell.Border = iTextSharp.text.Rectangle.NO_BORDER;
        //        table.DefaultCell.BorderWidth = 0;
        //        table.DefaultCell.BorderColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
        //        table.DefaultCell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;

        //    }
        //    else if (isSectionHeader)
        //    {
        //        widths = new float[] { 150f };
        //        table.DefaultCell.Border = iTextSharp.text.Rectangle.BOTTOM_BORDER;
        //        table.DefaultCell.Colspan = 2;
        //        table.SpacingAfter = 3;
        //        table.SpacingBefore = 5;
        //    }
        //    else
        //    {
        //        //widths = new float[] { 45f, 895f, 595f };
        //        widths = new float[] { 45f, 995f };
        //        table.DefaultCell.Border = iTextSharp.text.Rectangle.NO_BORDER;
        //    }

        //    table.SetWidths(widths);
        //    //table.DefaultCell.Border = Rectangle.NO_BORDER;
        //    //table.LockedWidth = true;
        //    //table.TotalWidth = 400f;
        //    //table.WidthPercentage = 100;
        //    table.HorizontalAlignment = 0;
        //    //int iCol = 0;
        //    //string colname = "";
        //    //PdfPCell cell = new PdfPCell(new Phrase("Products"));

        //    //cell.Colspan = dt.Columns.Count;

        //    //foreach (DataColumn c in dt.Columns)
        //    //{

        //    //  table.AddCell(new Phrase(c.ColumnName, font5));
        //    //}

        //    foreach (System.Data.DataRow r in dt.Rows)
        //    {
        //        if (dt.Rows.Count > 0)
        //        {
        //            for (int i = 0; i < r.ItemArray.Length; i++)
        //            {
        //                table.AddCell(new iTextSharp.text.Phrase(r[i].ToString(), font5));
        //            }
        //            //table.AddCell(new Phrase(r[0].ToString(), font5));
        //            //table.AddCell(new Phrase(r[1].ToString(), font5));
        //        }
        //    }
        //    document.Add(table);
        //}







        public TextToSpeech tts;
        void ExecuteSpeechCommand(string speechText)
        {
            try
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
            catch (Exception ex)
            {
                CommonFunctions.sendErrorEmail(ex);
                PresentViewController(CommonFunctions.ExceptionAlertPrompt(ex), true, null);
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
                PresentViewController(CommonFunctions.ExceptionAlertPrompt(ex), true, null);
                return false;
                //throw new Exception(ex.Message + Environment.NewLine + ex.StackTrace, ex.InnerException);
            }
        }




        public UIBarButtonItem GetPhotoNavBtn(string sectionName, string patientName, string apptId, string patientId, string doctorId, string locationId, bool IsDoctorForm)
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
                                    var testLength = testByte.Length;
                                    var filename = patientName.Replace(" ", "_") + "_" + sectionName + "_" + DateTime.Now.ToString("s").Replace(":", "_") + ".jpg";

                                    //var dps = new DynaPadService.DynaPadService();
                                    //var savefile = dps.SaveFile(apptId, patientId, doctorId, locationId, filename, "DynaPad", testByte, IsDoctorForm, false);

                                    //if (CrossConnectivity.Current.IsConnected)
                                    //{

                                    var bw = new BackgroundWorker
                                    {
                                        // this allows our worker to report progress during work
                                        WorkerReportsProgress = true
                                    };

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
                PresentViewController(CommonFunctions.ExceptionAlertPrompt(ex), true, null);
                return null;
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
                PresentViewController(CommonFunctions.ExceptionAlertPrompt(ex), true, null);
                return null;
                //throw new Exception(ex.Message + Environment.NewLine + ex.StackTrace, ex.InnerException);
            }
        }



        public UIBarButtonItem GetDrawNavBtn()
        {
            try
            {
                var navdraw = new UIBarButtonItem(UIImage.FromBundle("Writing"), UIBarButtonItemStyle.Plain, (sender, args) =>
                {
                    var nlab = new UILabel(new CGRect(10, 0, View.Bounds.Width - 60, 50))
                    {
                        //nlab.Text = "NOTES: (" + SelectedAppointment.SelectedQForm.FormName + ")";
                        Text = "NOTES:"
                    };

                    var ncellHeader = new UITableViewCell(UITableViewCellStyle.Default, null)
                    {
                        Frame = new CGRect(0, 0, View.Bounds.Width, 50)
                    };

                    var nheadclosebtn = new UIButton(new CGRect(View.Bounds.Width - 50, 0, 50, 50));
                    nheadclosebtn.SetImage(UIImage.FromBundle("Close"), UIControlState.Normal);

                    ncellHeader.ContentView.Add(nlab);
                    ncellHeader.ContentView.Add(nheadclosebtn);

                    var nsec = new Section(ncellHeader) { FooterView = new UIView(new CGRect(0, 0, 0, 0)) };
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


                    var nroo = new RootElement("Notes") { nsec };

                    var ndia = new DialogViewController(nroo)
                    {
                        ModalInPopover = true,
                        ModalPresentationStyle = UIModalPresentationStyle.FullScreen
                        //ndia.PreferredContentSize = new CGSize(View.Bounds.Size);
                    };

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
                PresentViewController(CommonFunctions.ExceptionAlertPrompt(ex), true, null);
                return null;
                //throw new Exception(ex.Message + Environment.NewLine + ex.StackTrace, ex.InnerException);
            }
        }



        public UIBarButtonItem GetDynaDrawNavBtn()
        {
            try
            {
                var navdraw = new UIBarButtonItem(UIImage.FromBundle("Writing"), UIBarButtonItemStyle.Plain, (sender, args) =>
                {
                    var nlab = new UILabel(new CGRect(10, 0, View.Bounds.Width - 60, 50)) { Text = "NOTES:" };

                    var ncellHeader = new UITableViewCell(UITableViewCellStyle.Default, null) { Frame = new CGRect(0, 0, View.Bounds.Width, 50) };

                    var nheadclosebtn = new UIButton(new CGRect(View.Bounds.Width - 50, 0, 50, 50));
                    nheadclosebtn.SetImage(UIImage.FromBundle("Close"), UIControlState.Normal);

                    ncellHeader.ContentView.Add(nlab);
                    ncellHeader.ContentView.Add(nheadclosebtn);

                    var nsec = new Section(ncellHeader)
                    {
                        FooterView = new UIView(new CGRect(0, 0, 0, 0))
                    };
                    nsec.FooterView.Hidden = true;

                    var img = UIImage.FromFile("dynapadscreenshot.png");
                    var imgView = new UIImageView(View.Bounds)
                    {
                        Image = img,
                        ContentMode = UIViewContentMode.ScaleAspectFit // or ScaleAspectFill
                    };

                    var dcanvas = new DynaPadView(new CGRect(0, 0, 500, 500)) { BackgroundImage = imgView.Image };

                    nsec.Add(dcanvas);
                    //CanvasContainerView notesCanvas = CanvasContainerView.FromCanvasSize(new CGSize(800, 800));
                    //nsec.Add(notesCanvas);
                    var ass = new UIViewController { dcanvas };

                    var nroo = new RootElement("Notes") { nsec };

                    var ndia = new DialogViewController(nroo)
                    {
                        ModalInPopover = true,
                        ModalPresentationStyle = UIModalPresentationStyle.FullScreen
                    };
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
                PresentViewController(CommonFunctions.ExceptionAlertPrompt(ex), true, null);
                return null;
                //throw new Exception(ex.Message + Environment.NewLine + ex.StackTrace, ex.InnerException);
            }
        }


        public UIBarButtonItem GetDicNavBtn(string sectionId)
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

                    var clab = new UILabel(new CGRect(10, 0, 290, 50)) { Text = "DICTATION(S):" };
                    //clab.Font = UIFont.BoldSystemFontOfSize(17);

                    //var segDict = new UISegmentedControl();
                    //segDict.Frame = new CGRect(0, 0, 350, 50);
                    //segDict.Momentary = true;
                    //segDict.InsertSegment(UIImage.FromBundle("Delete"), 0, true);
                    //segDict.InsertSegment("Dictation", 1, true);
                    //segDict.SetWidth(50, 0);
                    //segDict.SetWidth(324, 1);

                    var cellHeader = new UITableViewCell(UITableViewCellStyle.Default, null) { Frame = new CGRect(0, 0, 350, 50) };
                    //cellHeader.ImageView.Image = UIImage.FromBundle("Close");

                    var headclosebtn = new UIButton(new CGRect(300, 0, 50, 50));
                    headclosebtn.SetImage(UIImage.FromBundle("Close"), UIControlState.Normal);

                    cellHeader.ContentView.Add(clab);
                    cellHeader.ContentView.Add(headclosebtn);

                    var cellFooter = new UITableViewCell(UITableViewCellStyle.Default, null) { Frame = new CGRect(0, 0, 350, 50) };
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
                        var dicplayer = new AVAudioPlayer(dataDictation, "aac", out NSError err);

                        var duration = TimeSpan.FromSeconds(dicplayer.Duration).ToString(@"hh\:mm\:ss");

                        var statusLabel = new UILabel(new CGRect(200, 0, 100, 50)) { Text = duration };

                        PlaySavedDictationButton = new UIButton
                        {
                            Frame = new CGRect(10, 0, 190, 50),
                            HorizontalAlignment = UIControlContentHorizontalAlignment.Left,
                            ImageEdgeInsets = new UIEdgeInsets(0, 0, 0, 5),
                            TitleEdgeInsets = new UIEdgeInsets(0, 5, 0, 0)
                        };
                        PlaySavedDictationButton.SetTitle(dictation[1], UIControlState.Normal);
                        PlaySavedDictationButton.SetTitleColor(UIColor.Blue, UIControlState.Normal);
                        PlaySavedDictationButton.SetImage(UIImage.FromBundle("CircledPlay"), UIControlState.Normal);

                        messageLabel = new UILabel();

                        DeleteSavedDictationButton = new UIButton { Frame = new CGRect(300, 0, 50, 50) };
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

                        cellDict = new UITableViewCell(UITableViewCellStyle.Default, null)
                        {
                            Frame = new CGRect(0, 0, 350, 50),
                            BackgroundColor = UIColor.LightGray
                        };
                        cellDict.ContentView.Add(PlaySavedDictationButton);
                        cellDict.ContentView.Add(statusLabel);
                        cellDict.ContentView.Add(DeleteSavedDictationButton);

                        sec.Add(cellDict);

                        PlaySavedDictationButton.TouchUpInside += delegate
                        {
                            OnPlaySavedDictation(dicplayer, statusLabel);
                        };
                    }

                    var popHeight = sec.Elements.Count > 8 ? 500 : sec.Elements.Count * 50 + 100;

                    var roo = new RootElement("Dictation")
                    {
                        sec
                    };

                    var dia = new DialogViewController(roo)
                    {
                        ModalInPopover = true,
                        ModalPresentationStyle = UIModalPresentationStyle.FormSheet,
                        PreferredContentSize = new CGSize(350, popHeight)
                    };

                    var vie = new UIView();

                    var con = new UIViewController
                    {
                        vie
                    };

                    NavigationController.PreferredContentSize = new CGSize(350, popHeight);
                    NavigationController.PresentViewController(dia, true, null);

                    pop = new UIPopoverController(dia) { PopoverContentSize = new CGSize(350, popHeight) };
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
                            OnSaveRecordedSound(sectionId, pop);
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
                PresentViewController(CommonFunctions.ExceptionAlertPrompt(ex), true, null);
                return null;
                //throw new Exception(ex.Message + Environment.NewLine + ex.StackTrace, ex.InnerException);
            }
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
                        var dds = new DynaPadService.DynaPadService { Timeout = 60000 };
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
                PresentViewController(CommonFunctions.ExceptionAlertPrompt(ex), true, null);
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

            session.SetCategory(AVAudioSession.CategoryRecord, out NSError error);
            if (error != null)
            {
                CommonFunctions.sendNSErrorEmail(error);
                PresentViewController(CommonFunctions.NSExceptionAlertPrompt(error), true, null);
                //System.Console.WriteLine(error);
                return;
            }

            session.SetActive(true, out error);
            if (error != null)
            {
                CommonFunctions.sendNSErrorEmail(error);
                PresentViewController(CommonFunctions.NSExceptionAlertPrompt(error), true, null);
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
                AVAudioSession.SharedInstance().SetCategory(AVAudioSession.CategoryPlayback, out NSError error);
                if (error != null)
                {
                    CommonFunctions.sendNSErrorEmail(error);
                    PresentViewController(CommonFunctions.NSExceptionAlertPrompt(error), true, null);
                    //throw new Exception(error.DebugDescription);
                }
                //player = new AVPlayer(audioFilePath);
                player = new AVAudioPlayer(audioFilePath, "aac", out NSError audioError);

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
                PresentViewController(CommonFunctions.ExceptionAlertPrompt(ex), true, null);
                //System.Console.WriteLine("There was a problem playing back audio: ");
                //System.Console.WriteLine(ex.Message);
            }
        }


        //void OnPlaySavedDictation(string title, string dictationBytes, AVAudioPlayer pplayer, UILabel statusLabel, UIButton cd, Section cdsec)
        void OnPlaySavedDictation(AVAudioPlayer pplayer, UILabel statusLabel)
        {
            try
            {
                //System.Console.WriteLine("Playing Back Recording {0}", title);

                // The following line prevents the audio from stopping
                // when the device autolocks. will also make sure that it plays, even
                // if the device is in mute
                AVAudioSession.SharedInstance().SetCategory(AVAudioSession.CategoryPlayback, out NSError error);
                if (error != null)
                {
                    CommonFunctions.sendNSErrorEmail(error);
                    PresentViewController(CommonFunctions.NSExceptionAlertPrompt(error), true, null);
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
                PresentViewController(CommonFunctions.ExceptionAlertPrompt(ex), true, null);
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
            recorder = AVAudioRecorder.Create(audioFilePath, audioSettings, out NSError error);
            if (error != null)
            {
                CommonFunctions.sendNSErrorEmail(error);
                PresentViewController(CommonFunctions.NSExceptionAlertPrompt(error), true, null);
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
                PresentViewController(CommonFunctions.ExceptionAlertPrompt(ex), true, null);
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


        void OnSaveRecordedSound(string sectionId, UIPopoverController popd)
        {
            try
            {
                var bounds = pop.PopoverContentSize;
                loadingOverlay = new LoadingOverlay(new CGRect(new CGPoint(0, 0), bounds));
                //mvc = (DialogViewController)((UINavigationController)SplitViewController.ViewControllers[1]).TopViewController;
                //mvc.Add(loadingOverlay);
                popd.ContentViewController.Add(loadingOverlay);

                if (CrossConnectivity.Current.IsConnected)
                {
                    var dictationData = NSData.FromUrl(audioFilePath); //the path here can be a path to a video on the camera roll
                    var dictationArray = dictationData.ToArray();

                    var dds = new DynaPadService.DynaPadService { Timeout = 60000 };
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
                PresentViewController(CommonFunctions.ExceptionAlertPrompt(ex), true, null);
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
                //Console.WriteLine("Canceled Recording");
            }
            catch (Exception ex)
            {
                CommonFunctions.sendErrorEmail(ex);
                PresentViewController(CommonFunctions.ExceptionAlertPrompt(ex), true, null);
                //Console.WriteLine("There was a problem canceling audio: ");
                //Console.WriteLine(ex.Message);
            }
        }



        //      void DataGridForm_GridDoubleTapped(object sender, GridDoubleTappedEventsArgs e)
        //      {
        //          try
        //          {
        //              //if (e.RowData.GetType() == typeof(Syncfusion.Data.Group))
        //              if (e.RowData.GetType() == typeof(MR))
        //              {
        //                  var rowIndex = e.RowColumnindex.RowIndex;
        //                  var rowData = (MR)e.RowData;
        //                  var columnIndex = e.RowColumnindex.ColumnIndex;
        //                  var filepath = rowData.MRPath;
        //                  if (filepath.StartsWith("Error:", StringComparison.CurrentCulture))
        //                  {
        //                        DismissViewController(true, null);
        //                      PresentViewController(CommonFunctions.AlertPrompt("File Error", "File unavailable, contact administration", true, null, false, null), true, null);
        //                      return;
        //                  }
        //                  var webViews = new UIWebView(View.Bounds);
        //                  webViews.Frame = new CGRect(0, 0, View.Bounds.Width, View.Bounds.Height);
        //                  //webViews.LoadRequest(new NSUrlRequest(new NSUrl("https://test.dynadox.pro/dynawcfservice/Summaries/3_10_29_patient.pdf")));
        //                  //var filepath = rowData.MRPath.Replace(@"\", "/");
        //                  //filepath = filepath.Replace("C:/inetpub/wwwroot/dynadox/", "https://test.dynadox.pro/");
        //                  webViews.LoadRequest(new NSUrlRequest(new NSUrl(filepath)));
        //                  webViews.ScalesPageToFit = true;
        //                  var nlab = new UILabel(new CGRect(10, 10, View.Bounds.Width - 110, 50));
        //                  nlab.Text = rowData.MRName;
        //                  var ncellHeader = new UITableViewCell(UITableViewCellStyle.Default, null);
        //                  ncellHeader.Frame = new CGRect(0, 0, View.Bounds.Width, 50);
        //                  var nheadeditbtn = new UIButton(new CGRect(View.Bounds.Width - 100, 10, 50, 50));
        //                  nheadeditbtn.SetImage(UIImage.FromBundle("Writing"), UIControlState.Normal);
        //                  var nheadclosebtn = new UIButton(new CGRect(View.Bounds.Width - 50, 10, 50, 50));
        //                  nheadclosebtn.SetImage(UIImage.FromBundle("Close"), UIControlState.Normal);
        //                  ncellHeader.ContentView.Add(nlab);
        //                  ncellHeader.ContentView.Add(nheadeditbtn);
        //                  ncellHeader.ContentView.Add(nheadclosebtn);
        //                  var nsec = new Section(ncellHeader);
        //                  nsec.FooterView = new UIView(new CGRect(0, 0, 0, 0));
        //                  nsec.FooterView.Hidden = true;
        //                  //var dcanvas = new CanvasMainViewController();
        //                  nsec.Add(webViews);
        //                  var nroo = new RootElement("File");
        //                  nroo.Add(nsec);
        //                  var ndia = new DialogViewController(nroo);
        //                  ndia.ModalInPopover = true;
        //                  ndia.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
        //                  ndia.PreferredContentSize = new CGSize(View.Bounds.Size);
        //                  //ndia.ProvidesPresentationContextTransitionStyle = true;
        //                  //ndia.DefinesPresentationContext = true;
        //                  //ndia.ModalPresentationStyle = UIModalPresentationStyle.OverCurrentContext;
        //                  nheadclosebtn.TouchUpInside += delegate
        //                  {
        //                      DismissViewController(true, null);
        //                      //NavigationController.PopViewController(true);
        //                  };
        //                  nheadeditbtn.TouchUpInside += delegate
        //                  {
        //                      DismissViewController(true, null);
        //var dcanvas = new CanvasMainViewController { MREditing = true, MREditPath = rowData.MRPath, MREditId = rowData.MRId, apptId = SelectedAppointment.ApptId, patientId = SelectedAppointment.ApptPatientId, doctorId = SelectedAppointment.ApptDoctorId, locationId = SelectedAppointment.ApptLocationId, IsDoctorForm = true };
        //                      PreferredContentSize = new CGSize(View.Bounds.Size);
        //                      //NavigationController.View.BackgroundColor = UIColor.Clear;
        //                      PresentViewController(dcanvas, true, null);
        //                      //NavigationController.View.SizeToFit();
        //                  };
        //                  PreferredContentSize = new CGSize(View.Bounds.Size);
        //                  //NavigationController.PushViewController(ndia, true);
        //                  DismissViewController(true, null);
        //                  PresentViewController(ndia, true, null);
        //                  //View.SizeToFit();
        //              }
        //          }
        //          catch (Exception ex)
        //          {
        //              throw new Exception(ex.Message + Environment.NewLine + ex.StackTrace, ex.InnerException);
        //          }
        //      }

        //SfPdfViewer pdfViewerControl;
        //MFMailComposeViewController mailController;
        //WKWebView MRWebView;
        //UIProgressView progressView;

        //void DataGrid_GridDoubleTapped(object sender, GridDoubleTappedEventsArgs e)
        //{
        //  try
        //  {
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
        //          var ncellHeader = new UITableViewCell(UITableViewCellStyle.Default, null)
        //                  {
        //                      Frame = new CGRect(0, 0, View.Bounds.Width, 50)
        //                  };
        //          var nheadeditbtn = new UIButton(new CGRect(View.Bounds.Width - 200, 10, 50, 50));
        //          nheadeditbtn.SetImage(UIImage.FromBundle("Writing"), UIControlState.Normal);
        //          var nheadclosebtn = new UIButton(new CGRect(View.Bounds.Width - 50, 10, 50, 50));
        //          nheadclosebtn.SetImage(UIImage.FromBundle("Close"), UIControlState.Normal);
        //          var nheadprintbtn = new UIButton(new CGRect(View.Bounds.Width - 100, 10, 50, 50));
        //          nheadprintbtn.SetImage(UIImage.FromBundle("Print"), UIControlState.Normal);
        //          var nheadsharebtn = new UIButton(new CGRect(View.Bounds.Width - 150, 10, 50, 50));
        //          nheadsharebtn.SetImage(UIImage.FromBundle("Email"), UIControlState.Normal);
        //          //ncellHeader.ContentView.Add(nlab);
        //          //ncellHeader.ContentView.Add(nheadeditbtn);
        //          ncellHeader.ContentView.Add(nheadclosebtn);
        //          ncellHeader.ContentView.Add(nheadprintbtn);
        //                  ncellHeader.ContentView.Add(nheadsharebtn);
        //                  if (filetype == "jpg" || filetype == "jpeg" || filetype == "png" || filetype == "gif")
        //                  {
        //                      ncellHeader.ContentView.Add(nheadeditbtn);
        //                  }
        //                  var nsec = new Section(ncellHeader);
        //          nsec.FooterView = new UIView(new CGRect(0, 0, 0, 0));
        //          nsec.FooterView.Hidden = true;
        //                  //nsec.HeaderView = new UIView(new CGRect(0, 0, 0, 0));
        //                  //nsec.HeaderView.Hidden = true;
        //                  //UIBarButtonItem printNavBtn;
        //                  //               switch (filetype)
        //                  //               {
        //                  //  //                 case "pdf":
        //                  //  //                     nheadeditbtn.Enabled = false;
        //                  //      //          var pdfViewerControl = new SfPdfViewer();
        //                  //      //          using (MemoryStream mem = new MemoryStream())
        //                  //      //          {
        //                  //      //              ConvertToStream(filepath, mem);
        //                  //      //              mem.Seek(0, SeekOrigin.Begin);
        //                  //      //              pdfViewerControl.LoadDocument(mem);
        //                  //      //          }
        //                  //  //                     //pdfViewerControl.LoadDocument(DownloadPdfStream(filepath, rowData.MRName));
        //                  //  //                     pdfViewerControl.Frame = new CGRect(0, 0, View.Bounds.Width, View.Bounds.Height);
        //                  //      //nsec.Add(pdfViewerControl);
        //                  //                       //break;
        //                  //                   case "jpg":
        //                  //                   case "jpeg":
        //                  //                   case "png":
        //                  //                   case "gif":
        //                  //                       ncellHeader.ContentView.Add(nheadeditbtn);
        //                  //      bool didStart = false;
        //                  //      bool didFinish = false;
        //                  //      var webViews = new UIWebView(View.Bounds);
        //                  //      webViews.LoadStarted += (object lssender, EventArgs lse) => {
        //                  //          if (didStart == false)
        //                  //          {
        //                  //              loadingOverlay = new LoadingOverlay(webViews.Bounds);
        //                  //              webViews.Add(loadingOverlay);
        //                  //              UIApplication.SharedApplication.NetworkActivityIndicatorVisible = true;
        //                  //              didStart = true;
        //                  //              didFinish = false;
        //                  //          }
        //                  //      };
        //                  //      //When the web view is finished loading
        //                  //      webViews.LoadFinished += (object lfsender, EventArgs lfe) => {
        //                  //          if (didFinish == false)
        //                  //          {
        //                  //              loadingOverlay.Hide();
        //                  //              UIApplication.SharedApplication.NetworkActivityIndicatorVisible = false;
        //                  //              didStart = false;
        //                  //              didFinish = true;
        //                  //          }
        //                  //      };
        //                  //      //If there is a load error
        //                  //      webViews.LoadError += (object lesender, UIWebErrorArgs lee) => {
        //                  //          if (didFinish == false)
        //                  //          {
        //                  //              loadingOverlay.Hide();
        //                  //              UIApplication.SharedApplication.NetworkActivityIndicatorVisible = false;
        //                  //              didStart = false;
        //                  //              didFinish = true;
        //                  //          }
        //                  //      };
        //                  //      webViews.Frame = new CGRect(View.Bounds.X, 0, View.Bounds.Width, View.Bounds.Height);
        //                  //      webViews.LoadRequest(new NSUrlRequest(new NSUrl(filepath)));
        //                  //      webViews.ScalesPageToFit = true;
        //                  //      var nheadprintbtn = new UIButton(new CGRect(View.Bounds.Width - 150, 10, 50, 50));
        //                  //      nheadprintbtn.SetImage(UIImage.FromBundle("Print"), UIControlState.Normal);
        //                  //      nheadprintbtn.TouchUpInside += delegate {
        //                  //                           Print(rowData.MRName, webViews.ViewPrintFormatter);
        //                  //      };
        //                  //                       ncellHeader.ContentView.Add(nheadprintbtn);
        //                  //      printNavBtn = new UIBarButtonItem(UIImage.FromBundle("Print"), UIBarButtonItemStyle.Plain, delegate
        //                  //      { Print(rowData.MRName, webViews.ViewPrintFormatter); });
        //                  //      nsec.Add(webViews);
        //                  //      break;
        //                  //                   default:
        //                  //      bool defaultDidStart = false;
        //                  //      bool defaultDidFinish = false;
        //                  //      var defaultWebViews = new UIWebView(View.Bounds);
        //                  //      defaultWebViews.LoadStarted += (object lssender, EventArgs lse) => {
        //                  //          if (defaultDidStart == false)
        //                  //          {
        //                  //              loadingOverlay = new LoadingOverlay(defaultWebViews.Bounds);
        //                  //              defaultWebViews.Add(loadingOverlay);
        //                  //              UIApplication.SharedApplication.NetworkActivityIndicatorVisible = true;
        //                  //              defaultDidStart = true;
        //                  //              defaultDidFinish = false;
        //                  //          }
        //                  //      };
        //                  //      //When the web view is finished loading
        //                  //      defaultWebViews.LoadFinished += (object lfsender, EventArgs lfe) => {
        //                  //          if (defaultDidFinish == false)
        //                  //          {
        //                  //              loadingOverlay.Hide();
        //                  //              UIApplication.SharedApplication.NetworkActivityIndicatorVisible = false;
        //                  //              defaultDidStart = false;
        //                  //              defaultDidFinish = true;
        //                  //          }
        //                  //      };
        //                  //      //If there is a load error
        //                  //      defaultWebViews.LoadError += (object lesender, UIWebErrorArgs lee) => {
        //                  //          if (defaultDidFinish == false)
        //                  //          {
        //                  //              loadingOverlay.Hide();
        //                  //              UIApplication.SharedApplication.NetworkActivityIndicatorVisible = false;
        //                  //              defaultDidStart = false;
        //                  //              defaultDidFinish = true;
        //                  //          }
        //                  //      };
        //                  //                       defaultWebViews.Frame = new CGRect(View.Bounds.X, 0, View.Bounds.Width, View.Bounds.Height);
        //                  //      //webViews.LoadRequest(new NSUrlRequest(new NSUrl("https://test.dynadox.pro/dynawcfservice/Summaries/3_10_29_patient.pdf")));
        //                  //      //var filepath = rowData.MRPath.Replace(@"\", "/");
        //                  //      //filepath = filepath.Replace("C:/inetpub/wwwroot/dynadox/", "https://test.dynadox.pro/");
        //                  //      defaultWebViews.LoadRequest(new NSUrlRequest(new NSUrl(filepath)));
        //                  //      defaultWebViews.ScalesPageToFit = true;
        //                  //      var dheadprintbtn = new UIButton(new CGRect(View.Bounds.Width - 100, 10, 50, 50));
        //                  //      dheadprintbtn.SetImage(UIImage.FromBundle("Print"), UIControlState.Normal);
        //                  //      dheadprintbtn.TouchUpInside += delegate {
        //                  //          Print(rowData.MRName, defaultWebViews.ViewPrintFormatter);
        //                  //      };
        //                  //      ncellHeader.ContentView.Add(dheadprintbtn);
        //                  //                       printNavBtn = new UIBarButtonItem(UIImage.FromBundle("Print"), UIBarButtonItemStyle.Plain, delegate
        //                  //                               { Print(rowData.MRName, defaultWebViews.ViewPrintFormatter); });
        //                  //      nsec.Add(defaultWebViews);
        //                  //                       break;
        //                  //}
        //                  //var dcanvas = new CanvasMainViewController();
        //                  //bool didStart = false;
        //                  //bool didFinish = false;
        //                  //var webViews = new UIWebView(View.Bounds);
        //                  //webViews.LoadStarted += (object lssender, EventArgs lse) => {
        //                  //  if (didStart == false)
        //                  //  {
        //                  //      loadingOverlay = new LoadingOverlay(webViews.Bounds);
        //                  //      webViews.Add(loadingOverlay);
        //                  //      UIApplication.SharedApplication.NetworkActivityIndicatorVisible = true;
        //                  //      didStart = true;
        //                  //      didFinish = false;
        //                  //  }
        //                  //};
        //                  ////When the web view is finished loading
        //                  //webViews.LoadFinished += (object lfsender, EventArgs lfe) => {
        //                  //  if (didFinish == false)
        //                  //  {
        //                  //      loadingOverlay.Hide();
        //                  //      UIApplication.SharedApplication.NetworkActivityIndicatorVisible = false;
        //                  //      didStart = false;
        //                  //      didFinish = true;
        //                  //  }
        //                  //};
        //                  ////If there is a load error
        //                  //webViews.LoadError += (object lesender, UIWebErrorArgs lee) => {
        //                  //  if (didFinish == false)
        //                  //  {
        //                  //      loadingOverlay.Hide();
        //                  //      UIApplication.SharedApplication.NetworkActivityIndicatorVisible = false;
        //                  //      didStart = false;
        //                  //      didFinish = true;
        //                  //  }
        //                  //};
        //                  //webViews.Frame = new CGRect(View.Bounds.X, 0, View.Bounds.Width, View.Bounds.Height);
        //                  //webViews.LoadRequest(new NSUrlRequest(new NSUrl(filepath)));
        //                  //webViews.ScalesPageToFit = true;
        //                  //"https://test.dynadox.pro/data/test.dynadox.pro/claimantfiles/37/361.pdf"
        //                  //var wkurl = new NSUrl("https://test.dynadox.pro/data/test.dynadox.pro/claimantfiles/darwinmeds.pdf");
        //                  //var wkurl = new NSUrl(rowData.MRPath.Replace("https", "http"));//"https://test.dynadox.pro/data/test.dynadox.pro/claimantfiles/darwinmeds.pdf"
        //                  var wkurl = new NSUrl(rowData.MRPath);
        //          if (rowData.MRFileType == "External")
        //                  {
        //              var sfVC = new SFSafariViewController(wkurl);
        //              PresentViewController(sfVC, true, null);
        //                      return;
        //          }
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
        //          nheadclosebtn.TouchUpInside += delegate
        //          {
        //                      NavigationController.PopViewController(true);
        //          };
        //          nheadprintbtn.TouchUpInside += delegate {
        //                      Print(rowData.MRName, MRWebView.ViewPrintFormatter);
        //          };
        //                  string mimetype;
        //                  switch (filetype)
        //                  {
        //                      case "jpg":
        //                      case "jpeg":
        //                          mimetype = "image/jpeg";
        //                          break;
        //                      case "png":
        //                          mimetype = "image/png";
        //                  break;
        //              case "gif":
        //                  mimetype = "image/gif";
        //                  break;
        //              case "doc":
        //                  mimetype = "application/msword";
        //                  break;
        //              case "docm":
        //                  mimetype = "application/vnd.ms-word.document.macroEnabled.12";
        //                  break;
        //              case "docx":
        //                  mimetype = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
        //                  break;
        //              case "pdf":
        //                  mimetype = "application/pdf";
        //                  break;
        //                      default:
        //                          mimetype = "application/unknown";
        //                          break;
        //                  }
        //                  nheadsharebtn.TouchUpInside += delegate {
        //              if (MFMailComposeViewController.CanSendMail)
        //              {
        //                  //var to = new string[] { "john@doe.com" };
        //                  if (MFMailComposeViewController.CanSendMail)
        //                  {
        //                      mailController = new MFMailComposeViewController();
        //                              //mailController.SetToRecipients(to);
        //                      mailController.SetSubject("Dynapad MR Attachment - " + rowData.MRName);
        //                      //mailController.SetMessageBody("this is a test", false);
        //                              var attachmentfilename = rowData.MRName + "." + rowData.MRFileType;
        //                      var attachmentDownloadPath = Path.Combine(Path.GetTempPath(), attachmentfilename);
        //                      var attachmenturl = rowData.MRPath;
        //                      var attachmentWebClient = new WebClient();
        //                      attachmentWebClient.DownloadFile(attachmenturl, attachmentDownloadPath);
        //                              //NSData adata = new NSData();
        //                              mailController.AddAttachmentData(NSData.FromFile(attachmentDownloadPath), mimetype, attachmentfilename);
        //                      mailController.Finished += (object s, MFComposeResultEventArgs args) => {
        //                          //Console.WriteLine("result: " + args.Result.ToString()); // sent or cancelled
        //                          BeginInvokeOnMainThread(() => {
        //                              args.Controller.DismissViewController(true, null);
        //                          });
        //                      };
        //                  }
        //                  PresentViewController(mailController, true, null);
        //              }
        //              else
        //              {
        //                  //new UIAlertView("Mail not supported", "Can't send mail from this device", null, "OK");
        //                  PresentViewController(CommonFunctions.AlertPrompt("Mail not supported", "Can't send mail from this device", true, null, false, null), true, null);
        //              }
        //          };
        //          nheadeditbtn.TouchUpInside += delegate
        //          {
        //              //DismissViewController(true, null);
        //              //var dcanvas = new CanvasMainViewController { MREditing = true, MREditPath = rowData.MRPath, MREditId = rowData.MRId, MREditName = rowData.MRName, apptId = SelectedAppointment.ApptId, patientId = SelectedAppointment.ApptPatientId, doctorId = SelectedAppointment.ApptDoctorId, locationId = SelectedAppointment.ApptLocationId, IsDoctorForm = true };
        //              //var dcanvas = new FingerPaintViewController() { MREditing = true, MREditPath = rowData.MRPath, MREditId = rowData.MRId, MREditName = rowData.MRName, apptId = SelectedAppointment.ApptId, patientId = SelectedAppointment.ApptPatientId, doctorId = SelectedAppointment.ApptDoctorId, locationId = SelectedAppointment.ApptLocationId, IsDoctorForm = true };
        //              SfImageEditor imageEditor = new SfImageEditor();
        //                      imageEditor.Frame = new CGRect(View.Bounds.X, 0, View.Bounds.Width, View.Bounds.Height - 50);
        //                      var editfilename = rowData.MRName + "_" + "edit" + "_" + DateTime.Now.ToString("s").Replace(":", "_") + ".jpg";
        //              var downloadPath = Path.Combine(Path.GetTempPath(), editfilename);
        //              var url = rowData.MRPath;
        //              var webClient = new WebClient();
        //              webClient.DownloadFile(url, downloadPath);
        //              UIImage img = UIImage.LoadFromData(UIImage.FromFile(downloadPath).AsJPEG(), 1);
        //              imageEditor.Image = img;
        //                      imageEditor.ImageSaved += delegate {
        //                  var file = imageEditor.Image.AsPNG().ToArray();
        //                  var dds = new DynaPadService.DynaPadService() { Timeout = 180000};
        //                  dds.SaveFile(CommonFunctions.GetUserConfig(), SelectedAppointment.ApptId, SelectedAppointment.ApptPatientId, SelectedAppointment.ApptDoctorId, SelectedAppointment.ApptLocationId, editfilename, "Edit", "DynaPad", "", "", file, true, true);
        //                  DismissViewController(true, null);
        //                      };
        //              var ielab = new UILabel(new CGRect(10, 10, View.Bounds.Width - 110, 50))
        //              {
        //                  Text = rowData.MRName
        //              };
        //              var iecellHeader = new UITableViewCell(UITableViewCellStyle.Default, null)
        //              {
        //                  Frame = new CGRect(0, 0, View.Bounds.Width, 50)
        //              };
        //              //var ieheadprintbtn = new UIButton(new CGRect(View.Bounds.Width - 100, 10, 50, 50));
        //              //ieheadprintbtn.SetImage(UIImage.FromBundle("Print"), UIControlState.Normal);
        //              //ieheadprintbtn.TouchUpInside += delegate {
        //    //                      Print(rowData.MRName, imageEditor.ViewPrintFormatter);
        //              //};
        //              //iecellHeader.ContentView.Add(ieheadprintbtn);
        //              var ieheadclosebtn = new UIButton(new CGRect(View.Bounds.Width - 50, 10, 50, 50));
        //              ieheadclosebtn.SetImage(UIImage.FromBundle("Close"), UIControlState.Normal);
        //              ieheadclosebtn.TouchUpInside += delegate
        //              {
        //                  DismissViewController(true, null);
        //              };
        //              iecellHeader.ContentView.Add(ielab);
        //              iecellHeader.ContentView.Add(ieheadclosebtn);
        //              var iesec = new Section(iecellHeader);
        //              iesec.FooterView = new UIView(new CGRect(0, 0, 0, 0));
        //              iesec.FooterView.Hidden = true;
        //                      iesec.Add(imageEditor);
        //              var ieroo = new RootElement("File Edit");
        //              ieroo.Add(iesec);
        //              var iedia = new DialogViewController(ieroo);
        //                      iedia.TableView.ScrollEnabled = false;
        //                      iedia.ModalInPopover = true;
        //              iedia.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
        //              iedia.PreferredContentSize = new CGSize(View.Bounds.Size);
        //                      PresentViewController(iedia, true, null);
        //              //var asss = new Section();
        //              //asss.Add(dcanvas.View);
        //              //var rr = new RootElement("nass");
        //              //rr.Add(asss);
        //              //var nndia = new DialogViewController(rr);
        //              //nndia.ModalInPopover = true;
        //              //nndia.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
        //              //nndia.PreferredContentSize = new CGSize(View.Bounds.Size);
        //              //PreferredContentSize = new CGSize(View.Bounds.Size);
        //              //NavigationController.View.BackgroundColor = UIColor.Clear;
        //              //dcanvas.AutomaticallyAdjustsScrollViewInsets = true;
        //              //dcanvas.LoadView();
        //              //PresentViewController(dcanvas, true, null);
        //              //NavigationController.View.SizeToFit();
        //              //var ass = new UIBarButtonItem("edit", UIBarButtonItemStyle.Plain ,delegate
        //              //{
        //              //  PreferredContentSize = new CGSize(View.Bounds.Size);
        //              //  SfImageEditor imageEditor = new SfImageEditor();
        //              //             //imageEditor.Frame = new CGRect(0, 0, 500, 500);
        //              //             imageEditor.Frame = View.Frame;
        //              //  var downloadPath = Path.Combine(Path.GetTempPath(), "testjpg.jpg");
        //              //  var url = "https://amato.dynadox.pro/data/testjpg.jpg";
        //              //  var webClient = new WebClient();
        //              //  webClient.DownloadFile(url, downloadPath);
        //              //             UIImage img = UIImage.LoadFromData(UIImage.FromFile(downloadPath).AsJPEG(), 1);
        //              //             imageEditor.Image = img;
        //              //             var sec = new Section();
        //              //             sec.Add(imageEditor);
        //              //             var rr = new RootElement("edit");
        //              //             rr.Add(sec);
        //              //             var dvc = new DialogViewController(rr);
        //              //             var vvv = new UIViewController();
        //              //             vvv.View = imageEditor;
        //              //             //dvc.ModalPresentationStyle = UIModalPresentationStyle.Popover;
        //              //             //dvc.PreferredContentSize = new CGSize(View.Bounds.Size);
        //              //             //View.AddSubview(imageEditor);
        //              //             PresentViewController(vvv, true, null);
        //              //});
        //              //NavigationItem.SetRightBarButtonItem(ass, true);
        //          };
        //          if (rowData.IsShortcut)
        //          {
        //              DismissViewController(true, null);
        //          }
        //          //PreferredContentSize = new CGSize(View.Bounds.Size);
        //          //PresentViewController(ndia, true, null);
        //          NavigationController.PushViewController(ndia, true);
        //          //var closeNavBtn = new UIBarButtonItem(UIImage.FromBundle("Close"), UIBarButtonItemStyle.Plain, delegate
        //          //{ NavigationController.PopViewController(true); });
        //          //printNavBtn = new UIBarButtonItem(UIImage.FromBundle("Print"), UIBarButtonItemStyle.Plain, delegate
        //          //{ Print(rowData.MRName, webViews.ViewPrintFormatter); });
        //                  //ndia.NavigationItem.SetRightBarButtonItems(new UIBarButtonItem[] { closeNavBtn, printNavBtn }, true);
        //                  //NavigationController.NavigationBar.Hidden = true;
        //                  //ndia.NavigationController.NavigationBarHidden = true;
        //          //NavigationItem.SetRightBarButtonItems(new UIBarButtonItem[] { closeNavBtn, printNavBtn }, true);
        //      }
        //  }
        //  catch (Exception ex)
        //  {
        //              var errordata = (MR)e.RowData;
        //              var errorfile = "<br/><br/><br/>FILE PATH:<br/><br/>" + errordata.MRPath;
        //              CommonFunctions.sendErrorEmail(ex, errorfile);
        //              PresentViewController(CommonFunctions.ExceptionAlertPrompt(ex), true, null);
        //      //throw new Exception(ex.Message + Environment.NewLine + ex.StackTrace, ex.InnerException);
        //  }
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







        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
    }
}
