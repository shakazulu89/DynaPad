using System;
using System.Collections.Generic;
using UIKit;
using Foundation;
using MonoTouch.Dialog;
using Newtonsoft.Json;
using System.Drawing;
using System.IO;
using Newtonsoft.Json.Linq;
using LoginScreen;
using Plugin.Connectivity;
using HockeyApp.iOS;
using CoreGraphics;
using System.Threading.Tasks;
using System.Linq;
using System.Net;
using System.Net.Http;
//using System.Security.AccessControl;
//using System.Xml;
//using DynaPad.DynaPadService;
//using MultiThreading.Controls;
//using Syncfusion.SfBusyIndicator.iOS;
//using static DynaClassLibrary.DynaClasses;
//using DynaClassLibrary;
using System.Net.Sockets;
using ToastIOS;
using MimeKit;
using System.Diagnostics;
using System.Threading;
using Syncfusion.iOS.TabView;
using Syncfusion.iOS.PopupLayout;
using DynaClassLibrary;
using CoreImage;
using MapKit;
using Syncfusion.SfPdfViewer.iOS;
using System.Resources;
using System.Reflection;
using PdfKit;

namespace DynaPad
{
    public partial class MasterViewController : DynaDialogViewController
    {
        public DetailViewController DetailViewController { get; set; }
        public DialogViewController mvc { get; set; }
        UILabel messageLabel;
        LoadingOverlay loadingOverlay;
        Menu myDynaMenu;
        bool needLogin = true;
        bool didLogin;// = false;
        public string DocLocID { get; set; }
        public string DynaDomain { get; set; }
        public string DynaDeviceUniqueName { get; set; }
        NSUserDefaults plist = NSUserDefaults.StandardUserDefaults;
        HttpClient DynaClient;


        protected MasterViewController(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
            //Title = "";
        }


        //public override void ViewWillAppear(bool animated)
        //{
        //	base.ViewWillAppear(animated);
        //	var con = CrossConnectivity.Current;
        //	if (con.IsConnected)
        //	{
        //		if (needLogin)
        //		{
        //			//var ass = new CredentialsProvider();
        //			LoginScreenControl<CredentialsProvider, DefaultLoginScreenMessages>.Activate(this);
        //			needLogin = false;
        //		}
        //		else
        //		{
        //			Title = NSBundle.MainBundle.LocalizedString("Menu", "Form Sections");
        //			DetailViewController = (DetailViewController)((UINavigationController)SplitViewController.ViewControllers[1]).TopViewController;
        //			DetailViewController.Root.Clear();
        //			DetailViewController.Root.Add(new Section("Logged in"));
        //			DynaStart();
        //		}
        //	}
        //	else
        //	{
        //		var ConPrompt = UIAlertController.Create("No Internet Connection", "An internet connection is required in order to run this app", UIAlertControllerStyle.Alert);
        //		ConPrompt.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
        //		//Present Alert
        //		PresentViewController(ConPrompt, true, null);
        //		var logRoot = new RootElement("Login");
        //		var logSec = new Section();
        //		logSec.Add(new StringElement("Login", Login));
        //		logRoot.Add(logSec);
        //		Root = logRoot;
        //	}
        //}


        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            DetailViewController = (DetailViewController)((UINavigationController)SplitViewController.ViewControllers[1]).TopViewController;
            DetailViewController.Style = UITableViewStyle.Plain;

            //var clientBaseUri = new Uri("http://{dynahost}/dynahub/foremost/services/dynapad/xamarinservices/");

            //DynaClient = new HttpClient();
            //DynaClient.MaxResponseContentBufferSize = 256000;
            //DynaClient.Timeout = new TimeSpan(0, 0, 20);
            //DynaClient.BaseAddress = clientBaseUri;
        }



        protected void CheckStopwatch(Stopwatch sWatch, int seconds, string subject, string message, bool addLog = true)
        {
            sWatch.Stop();
            if (addLog)
            {
                CommonFunctions.AddLogEvent(DateTime.Now, subject, false, null, message);
            }

            int checkSeconds = seconds != 0 ? seconds : 10;
            if (sWatch.Elapsed.Seconds > checkSeconds)
            {
                Console.WriteLine(subject + " - " + message);
                CommonFunctions.sendAlertEmail("Subject: " + subject + "<br/>Message: " + message);
            }
            else
            {
                Console.WriteLine(subject + " - " + message);
            }
            sWatch.Reset();
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



        //Stopwatch timer;

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(true);

            //string ass = plist.StringForKey("Domain_Name");
            //plist.SetString("DynaDomain", NSUserDefaults.StandardUserDefaults.StringForKey("Domain_Name"));
            //plist.Synchronize();
            //DynaDomain = plist.StringForKey("DynaDomain");

            var timer = new Stopwatch();
            try
            {
                timer.Start();


                //loadingOverlay = new LoadingOverlay(SplitViewController.View.Bounds, true);
                //loadingOverlay.SetText("Initializing...");
                //SplitViewController.Add(loadingOverlay);


                //var documentsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "DynaFilesAwaitingUpload");
                //var files = Directory.GetFiles(documentsPath, "*.*", SearchOption.AllDirectories);
                //foreach (string file in files)
                //{
                //    File.Delete(file);
                //}

                //plist.SetString("Foremost", "Domain_Name");
                //plist.Synchronize();

                DynaDomain = plist.StringForKey("Domain_Name");
                DynaDeviceUniqueName = plist.StringForKey("Dyna_Device_Name");

                if (!string.IsNullOrEmpty(DynaDomain))
                {
                    //plist.SetString("DynaDomain", DynaDomain);
                    //plist.Synchronize();

                    CheckUniqueName();

                    var con = CrossConnectivity.Current;

                    if (con.IsConnected)
                    {
                        if (needLogin)
                        {
                            LoginScreenControl<CredentialsProvider, DefaultLoginScreenMessages>.Activate(this);
                            needLogin = false;
                        }
                        else
                        {
                            Title = NSBundle.MainBundle.LocalizedString("Menu", "Form Sections");
                            DetailViewController = (DetailViewController)((UINavigationController)SplitViewController.ViewControllers[1]).ViewControllers[0];//.TopViewController;
                            DetailViewController.Root.Clear();
                            DetailViewController.Root.Add(new Section("Select a location from the left menu")
                            {
                                FooterView = new UIView(new CGRect(0, 0, 0, 0))
                                {
                                    Hidden = true
                                }
                            });

                            if (!didLogin)
                            {
                                DynaLocations();
                            }

                            didLogin = true;
                        }
                    }
                    else
                    {
                        PresentViewController(CommonFunctions.InternetAlertPrompt(), true, null);

                        var logRoot = new RootElement("Login");
                        var logSec = new Section
                    {
                        new StringElement("Login", Login)
                    };

                        logRoot.Add(logSec);

                        Root = logRoot;
                    }
                }
                else
                {
                    var SetDomainPrompt = UIAlertController.Create("Set Domain Name", "Enter domain name: ", UIAlertControllerStyle.Alert);
                    SetDomainPrompt.AddTextField((field) =>
                    {
                        field.Placeholder = "Domain Name";
                    });

                    //Add Actions
                    SetDomainPrompt.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, action => SaveDomain(SetDomainPrompt.TextFields[0].Text)));
                    SetDomainPrompt.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, null));
                    //Present Alert
                    PresentViewController(SetDomainPrompt, true, null);

                    var logRoot = new RootElement("Login");
                    var logSec = new Section
                {
                    new StringElement("Login", Login)
                };

                    logRoot.Add(logSec);

                    Root = logRoot;
                }

                CheckStopwatch(timer, 0, "ViewDidAppear", "ViewDidAppear took " + timer.Elapsed.Seconds + " seconds", false);
            }
            catch (Exception ex)
            {
                CommonFunctions.sendErrorEmail(ex);
                PresentViewController(CommonFunctions.ExceptionAlertPrompt(ex), true, null);

                CheckStopwatch(timer, 0, "ViewDidAppear Error", "ViewDidAppear took " + timer.Elapsed.Seconds + " seconds", false);
            }
            //finally
            //{
            //    loadingOverlay.Hide();
            //}
        }

        public void SaveDomain(string domainname)
        {
            try
            {
                plist.SetString(domainname, "Domain_Name");
                plist.Synchronize();

                DynaDeviceUniqueName = null;
                CheckUniqueName();
            }
            catch (Exception ex)
            {
                var eventItems = new List<NSDictionary>
                {
                    getDictionary("Exception Message", ex.Message),
                    getDictionary("Exception Stacktrace", ex.StackTrace)
                };
                CommonFunctions.AddLogEvent(DateTime.Now, "SaveDomain", true, eventItems, "catch block");

                CommonFunctions.sendErrorEmail(ex);
                PresentViewController(CommonFunctions.ExceptionAlertPrompt(ex), true, null);
            }
        }

        public void CheckUniqueName()
        {
            try
            {
                if (string.IsNullOrEmpty(DynaDeviceUniqueName))
                {
                    var uniquename = DynaDomain + "_" + UIDevice.CurrentDevice.IdentifierForVendor;
                    plist.SetString(uniquename, "Dyna_Device_Name");
                    plist.Synchronize();
                }

                DynaDeviceUniqueName = plist.StringForKey("Dyna_Device_Name");
            }
            catch (Exception ex)
            {
                var eventItems = new List<NSDictionary>
                {
                    getDictionary("Exception Message", ex.Message),
                    getDictionary("Exception Stacktrace", ex.StackTrace)
                };
                CommonFunctions.AddLogEvent(DateTime.Now, "CheckUniqueName", true, eventItems, "catch block");

                CommonFunctions.sendErrorEmail(ex);
                PresentViewController(CommonFunctions.ExceptionAlertPrompt(ex), true, null);
            }
        }

        public void Login()
        {
            needLogin = true;
            ViewDidAppear(true);
        }


        public bool didCrashInLastSessionOnStartup()
        {
            return BITHockeyManager.SharedHockeyManager.CrashManager.DidCrashInLastSession;// && BITHockeyManager.SharedHockeyManager.CrashManager.TimeIntervalCrashInLastSessionOccurred < 5;
        }

        public void DynaLocations()
        {
            var timer = new Stopwatch();
            timer.Start();

            var rootMainMenu = new DynaFormRootElement("Locations");
            var sectionMainMenu = new Section();

            try
            {

                loadingOverlay = new LoadingOverlay(SplitViewController.View.Bounds, true);
                loadingOverlay.SetText("Initializing...");
                SplitViewController.Add(loadingOverlay);


                rootMainMenu.UnevenRows = true;
                rootMainMenu.Enabled = true;



                if (didCrashInLastSessionOnStartup())
                {
                    var restoreRootMenu = new DynaFormRootElement("Restore")
                    {
                        UnevenRows = true,
                        Enabled = true,
                        createOnSelected = GetRestoreStart,
                        ShowLoading = true,
                        MenuValue = "Restore",
                        Color = UIColor.FromRGB(218, 245, 255)
                    };
                    sectionMainMenu.Add(restoreRootMenu);
                }



                foreach (DynaClassLibrary.DynaClasses.Location loc in DynaClassLibrary.DynaClasses.LoginContainer.User.Locations)
                {
                    var rootMenu = new DynaFormRootElement(loc.LocationName)
                    {
                        UnevenRows = true,
                        Enabled = true,
                        createOnSelected = GetDynaStart,
                        ShowLoading = true,
                        MenuValue = loc.LocationId
                    };
                    sectionMainMenu.Add(rootMenu);
                }

                var settingsStringElement = new StyledStringElement("Settings", ShowSettings);

                var feedbackStringElement = new StyledStringElement("Feedback (v" + NSBundle.MainBundle.InfoDictionary["CFBundleVersion"] + ")", ShowFeedbackList);

                sectionMainMenu.Add(settingsStringElement);
                sectionMainMenu.Add(feedbackStringElement);
                sectionMainMenu.Add(GetLogoutElement());

                //var qsStringElement = new StyledStringElement("QS Sample", OpenQS);
                //sectionMainMenu.Add(qsStringElement);

                rootMainMenu.Add(sectionMainMenu);

                Root = rootMainMenu;

                SaveAutoData();
                SavePresetData();
            }
            catch (Exception ex)
            {
                var eventItems = new List<NSDictionary>
                {
                    getDictionary("Exception Message", ex.Message),
                    getDictionary("Exception Stacktrace", ex.StackTrace)
                };
                CommonFunctions.AddLogEvent(DateTime.Now, "DynaLocations", true, eventItems, "catch block");

                CommonFunctions.sendErrorEmail(ex);
                PresentViewController(CommonFunctions.ExceptionAlertPrompt(ex), true, null);

                CheckStopwatch(timer, 0, "DynaLocations Error", "DynaLocations took " + timer.Elapsed.Seconds + " seconds");
                DetailViewController.Root.Clear();
                DetailViewController.Root.Add(CommonFunctions.ErrorDetailSection());
                sectionMainMenu.Add(GetLogoutElement());
                rootMainMenu.Add(sectionMainMenu);
                Root = rootMainMenu;
            }
            finally
            {
                loadingOverlay.Hide();
            }

            CheckStopwatch(timer, 0, "DynaLocations", "DynaLocations took " + timer.Elapsed.Seconds + " seconds");
        }

        public void OpenQS()
        {
            try
            {
                //var documentURL = NSBundle.MainBundle.GetUrlForResource("QuickSheetSample", "pdf");
                //var document = new PdfDocument(documentURL);
                //var PDFView = new PdfView(new CGRect(0, 0, UIScreen.MainScreen.Bounds.Size.Height, UIScreen.MainScreen.Bounds.Size.Width));
                //PDFView.Document = document;
                //DetailViewController.View.AddSubview(PDFView);


                var pdfViewerControl = new SfPdfViewer() { Frame = new CGRect(this.View.Frame.X, 0, this.View.Frame.Width, 500) };
                DetailViewController.View.AddSubview(pdfViewerControl);
                var qsFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                var qsFilePath = Path.Combine(qsFolder, "QuickSheetSample.pdf");
                //var fileStream = File.OpenRead("GISSuccinctly.pdf");
                //var ass = typeof(MasterViewController).GetTypeInfo().Assembly;
                //var hole = typeof(MasterViewController).GetTypeInfo();
                //var fileStream = typeof(MasterViewController).GetTypeInfo().Assembly.GetManifestResourceStream("QuickSheetSample.pdf");
                //var assembly = Assembly.GetExecutingAssembly();
                //var resourceName = "GISSuccinctly.pdf";
                //Stream fileStream;
                //using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                //using (StreamReader reader = new StreamReader(stream))
                //{
                //    string result = reader.ReadToEnd();
                //    //fileStream = reader.BaseStream;
                //    fileStream = stream;
                //}
                Assembly assembly = Assembly.GetExecutingAssembly();
                string[] resources = assembly.GetManifestResourceNames();
                Stream fileStream = assembly.GetManifestResourceStream("DynaPad.Resources.QuickSheetSample.pdf");
                pdfViewerControl.LoadDocument(fileStream);
                //pdfViewerControl.Toolbar.Enabled = true;
                //pdfViewerControl.AnnotationMode = AnnotationMode.Ink;
            }
            catch (Exception ex)
            {
                var eventItems = new List<NSDictionary>
                {
                    getDictionary("Exception Message", ex.Message),
                    getDictionary("Exception Stacktrace", ex.StackTrace)
                };
                CommonFunctions.AddLogEvent(DateTime.Now, "OpenQS", true, eventItems, "catch block");

                CommonFunctions.sendErrorEmail(ex);
                PresentViewController(CommonFunctions.ExceptionAlertPrompt(ex), true, null);
            }
        }

        //public void OpenQS()
        //{
        //    try
        //    {
        //        var nlab = new UILabel(new CGRect(10, 0, UIScreen.MainScreen.Bounds.Width - 310, 50)) { Text = "QS" };
        //        var ncellHeader = new UITableViewCell(UITableViewCellStyle.Default, null) { Frame = new CGRect(0, 0, UIScreen.MainScreen.Bounds.Width - 250, 50) };
        //        var nheadclosebtn = new UIButton(new CGRect(UIScreen.MainScreen.Bounds.Width - 300, 0, 50, 50));
        //        nheadclosebtn.SetImage(UIImage.FromBundle("Close"), UIControlState.Normal);
        //        ncellHeader.ContentView.Add(nlab);
        //        ncellHeader.ContentView.Add(nheadclosebtn);
        //        var nsec = new Section(ncellHeader) { FooterView = new UIView(new CGRect(0, 0, 0, 0)) { Hidden = true } };
        //        var QSView = new DynaMultiRootElement();
        //        var sSection = new DynaSection("")
        //        {
        //            HeaderView = new UIView(new CGRect(0, 0, UIScreen.MainScreen.Bounds.Width, 15)),
        //            FooterView = new UIView(new CGRect(0, 0, 0, 0))
        //        };
        //        sSection.HeaderView.BackgroundColor = UIColor.White;
        //        sSection.FooterView.Hidden = true;

        //        var pdfViewerControl = new SfPdfViewer() { Frame = new CGRect(0, 0, UIScreen.MainScreen.Bounds.Size.Height, UIScreen.MainScreen.Bounds.Size.Width) };
        //        var qsFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        //        var qsFilePath = Path.Combine(qsFolder, "QuickSheetSample.pdf");
        //        //var fileStream = File.OpenRead("GISSuccinctly.pdf");
        //        //var ass = typeof(MasterViewController).GetTypeInfo().Assembly;
        //        //var hole = typeof(MasterViewController).GetTypeInfo();
        //        //var fileStream = typeof(MasterViewController).GetTypeInfo().Assembly.GetManifestResourceStream("QuickSheetSample.pdf");
        //        //var assembly = Assembly.GetExecutingAssembly();
        //        //var resourceName = "GISSuccinctly.pdf";
        //        //Stream fileStream;
        //        //using (Stream stream = assembly.GetManifestResourceStream(resourceName))
        //        //using (StreamReader reader = new StreamReader(stream))
        //        //{
        //        //    string result = reader.ReadToEnd();
        //        //    //fileStream = reader.BaseStream;
        //        //    fileStream = stream;
        //        //}
        //        Assembly assembly = Assembly.GetExecutingAssembly();
        //        string[] resources = assembly.GetManifestResourceNames();
        //        Stream fileStream = assembly.GetManifestResourceStream("DynaPad.Resources.QuickSheetSample.pdf");
        //        pdfViewerControl.LoadDocument(fileStream);

        //        //pdfViewerControl.AnnotationMode = AnnotationMode.Ink;

        //        sSection.Add(pdfViewerControl);

        //        var ndia = new DialogViewController(QSView)
        //        {
        //            ModalInPopover = true,
        //            ModalPresentationStyle = UIModalPresentationStyle.PageSheet,
        //            PreferredContentSize = new CGSize(View.Bounds.Size)
        //        };

        //        QSView.Add(nsec);
        //        QSView.Add(sSection);

        //        var nroo = new RootElement("QS") { nsec };

        //        nheadclosebtn.TouchUpInside += delegate
        //        {
        //            pdfViewerControl.Unload();
        //            NavigationController.DismissViewController(true, null);
        //        };

        //        NavigationController.PreferredContentSize = new CGSize(View.Bounds.Size);
        //        NavigationController.PresentViewController(ndia, true, null);
        //    }
        //    catch (Exception ex)
        //    {
        //        var eventItems = new List<NSDictionary>
        //        {
        //            getDictionary("Exception Message", ex.Message),
        //            getDictionary("Exception Stacktrace", ex.StackTrace)
        //        };
        //        CommonFunctions.AddLogEvent(DateTime.Now, "OpenQS", true, eventItems, "catch block");

        //        CommonFunctions.sendErrorEmail(ex);
        //        PresentViewController(CommonFunctions.ExceptionAlertPrompt(ex), true, null);
        //    }
        //}

        UIViewController GetRestoreStart(RootElement arg)
        {
            try
            {
                DetailViewController.Root.Clear();
                DetailViewController.Root.Add(new Section("Select a form to restore from the left menu")
                {
                    FooterView = new UIView(new CGRect(0, 0, 0, 0))
                    {
                        Hidden = true
                    }
                });

                var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                var directoryname = Path.Combine(documents, "DynaRestore");
                messageLabel = new UILabel();
                var backups = new DirectoryInfo(directoryname).GetFiles().Where(x => x.LastWriteTime.Date == DateTime.Today.Date).OrderByDescending(x => x.LastWriteTime);
                var restoreSection = new Section { HeaderView = null };

                if (backups.Any())
                {
                    foreach (var fi in backups)
                    {
                        var splits = fi.Name.Split('_');
                        var IsDoctorForm = splits[2] == "doctor";// ? true : false;
                        var formtype = new System.Globalization.CultureInfo("en-US").TextInfo.ToTitleCase(splits[2]);

                        var restoreRoot = new DynaFormRootElement(splits[3] + " - " + formtype + " (" + fi.CreationTime.ToShortTimeString() + ")")
                        {
                            RestoreFile = fi,
                            Enabled = true,
                            UnevenRows = true,
                            ShowLoading = true,
                            MenuAction = IsDoctorForm ? "GetDoctorForm" : "GetPatientForm",
                            IsDoctorForm = IsDoctorForm
                        };

                        restoreRoot.createOnSelected = GetFormService;

                        restoreSection.Add(restoreRoot);
                    }
                }
                else
                {
                    restoreSection.Add(new SectionStringElement("No Forms Found"));
                }

                arg.Add(restoreSection);

                var formDVC = new DynaDialogViewController(arg, true);
                formDVC.NavigationItem.LeftBarButtonItem = new UIBarButtonItem(UIImage.FromBundle("Back"), UIBarButtonItemStyle.Plain, delegate
                {
                    DetailViewController.QuestionsView = null; //.Clear();
                    DetailViewController.NavigationItem.RightBarButtonItem = null;
                    DetailViewController.Root.Clear();
                    DetailViewController.Root.Caption = "";
                    DetailViewController.ReloadData();

                    NavigationController.PopViewController(true);
                });

                return formDVC;
            }
            catch (Exception ex)
            {
                var eventItems = new List<NSDictionary>
                {
                    getDictionary("Exception Message", ex.Message),
                    getDictionary("Exception Stacktrace", ex.StackTrace)
                };
                CommonFunctions.AddLogEvent(DateTime.Now, "GetRestoreStart", true, eventItems, "catch block");

                CommonFunctions.sendErrorEmail(ex);
                PresentViewController(CommonFunctions.ExceptionAlertPrompt(ex), true, null);
                return new DynaDialogViewController(CommonFunctions.ErrorRootElement(), true);
            }
        }

        public void ShowSettings()
        {
            try
            {
                var nlab = new UILabel(new CGRect(10, 0, UIScreen.MainScreen.Bounds.Width - 310, 50)) { Text = "Settings" };

                var ncellHeader = new UITableViewCell(UITableViewCellStyle.Default, null) { Frame = new CGRect(0, 0, UIScreen.MainScreen.Bounds.Width - 250, 50) };
                var nheadclosebtn = new UIButton(new CGRect(UIScreen.MainScreen.Bounds.Width - 300, 0, 50, 50));
                nheadclosebtn.SetImage(UIImage.FromBundle("Close"), UIControlState.Normal);
                ncellHeader.ContentView.Add(nlab);
                ncellHeader.ContentView.Add(nheadclosebtn);

                var nsec = new Section(ncellHeader) { FooterView = new UIView(new CGRect(0, 0, 0, 0)) { Hidden = true } };

                var SettingsView = new DynaMultiRootElement();

                var ndia = new DialogViewController(SettingsView)
                {
                    ModalInPopover = true,
                    ModalPresentationStyle = UIModalPresentationStyle.PageSheet,
                    PreferredContentSize = new CGSize(View.Bounds.Size)
                };

                var sSection = new DynaSection("")
                {
                    HeaderView = new UIView(new CGRect(0, 0, UIScreen.MainScreen.Bounds.Width, 15)),
                    FooterView = new UIView(new CGRect(0, 0, 0, 0))
                };
                sSection.HeaderView.BackgroundColor = UIColor.White;
                sSection.FooterView.Hidden = true;

                var sPaddedView = new PaddedUIView<UILabel>
                {
                    Frame = new CGRect(0, 0, UIScreen.MainScreen.Bounds.Width - 250, 30),
                    Padding = 5f
                };
                sPaddedView.NestedView.Text = "DOMAIN NAME:";
                sPaddedView.setStyle();

                sSection.Add(sPaddedView);

                var txtDomain = new UITextView(new CGRect(0, 0, UIScreen.MainScreen.Bounds.Width - 250, 30)) { Text = plist.StringForKey("Domain_Name"), Font = UIFont.SystemFontOfSize(13) };

                sSection.Add(txtDomain);

                var btnDomain = new UIButton(new CGRect(0, 0, UIScreen.MainScreen.Bounds.Width - 250, 40))
                {
                    BackgroundColor = UIColor.FromRGB(224, 238, 240),
                    HorizontalAlignment = UIControlContentHorizontalAlignment.Center
                };
                btnDomain.SetTitle("Update Domain", UIControlState.Normal);
                btnDomain.SetTitleColor(UIColor.Black, UIControlState.Normal);
                //UpdateDomain(txtDomain, btnDomain);
                btnDomain.TouchUpInside += delegate
                {
                    var UpdateDomainPrompt = UIAlertController.Create("Update Domain", "Updating the domain will require a re-login, continue?", UIAlertControllerStyle.Alert);
                    UpdateDomainPrompt.AddAction(UIAlertAction.Create("Yes", UIAlertActionStyle.Default, action =>
                    {
                        try
                        {
                            _lockObject = btnDomain;
                            lock (_lockObject)
                            {
                                if (isRunning)
                                    return;
                                isRunning = true;
                            }

                            plist.SetString(txtDomain.Text, "Domain_Name");
                            plist.Synchronize();
                            NavigationController.DismissViewController(true, null);
                            Logout();
                        }
                        catch (Exception ex)
                        {
                            var eventItems = new List<NSDictionary>
                        {
                            getDictionary("Exception Message", ex.Message),
                            getDictionary("Exception Stacktrace", ex.StackTrace)
                        };
                            CommonFunctions.AddLogEvent(DateTime.Now, "UpdateDomain", true, eventItems, "catch block");

                            CommonFunctions.sendErrorEmail(ex);
                            PresentViewController(CommonFunctions.ExceptionAlertPrompt(ex), true, null);
                        }
                        finally
                        {
                            isRunning = false;
                        }
                    }));
                    UpdateDomainPrompt.AddAction(UIAlertAction.Create("No", UIAlertActionStyle.Cancel, null));
                    //Present Alert
                    ndia.PresentViewController(UpdateDomainPrompt, true, null);
                };

                sSection.Add(btnDomain);

                var uniqueNameWhite = GetBlankWhitePaddedView(15);
                sSection.Add(uniqueNameWhite);

                var uniqueNameLbl = new PaddedUIView<UILabel>
                {
                    Frame = new CGRect(0, 0, UIScreen.MainScreen.Bounds.Width - 250, 30),
                    Padding = 5f
                };
                uniqueNameLbl.NestedView.Text = "UNIQUE DEVICE NAME:";
                uniqueNameLbl.setStyle();

                sSection.Add(uniqueNameLbl);

                var uniqueName = new UILabel(new CGRect(5, 0, UIScreen.MainScreen.Bounds.Width - 250, 30)) { Text = plist.StringForKey("Dyna_Device_Name") };

                sSection.Add(uniqueName);

                var uploadOnSubmitWhite = GetBlankWhitePaddedView(15);
                sSection.Add(uploadOnSubmitWhite);

                var uploadOnSubmitLbl = new PaddedUIView<UILabel>
                {
                    Frame = new CGRect(0, 0, UIScreen.MainScreen.Bounds.Width - 250, 30),
                    Padding = 5f
                };
                uploadOnSubmitLbl.NestedView.Text = "UPLOAD FORM ON SUBMIT?";
                uploadOnSubmitLbl.setStyle();

                sSection.Add(uploadOnSubmitLbl);

                var boo = string.IsNullOrEmpty(plist.StringForKey("Upload_On_Submit")) || bool.Parse(plist.StringForKey("Upload_On_Submit"));
                var switchUploadOnSubmit = new UISwitch { On = boo };
                switchUploadOnSubmit.Frame = new CGRect(5, 0, switchUploadOnSubmit.Frame.Width, switchUploadOnSubmit.Frame.Height);
                switchUploadOnSubmit.ValueChanged += delegate
                {
                    try
                    {
                        plist.SetString(switchUploadOnSubmit.On.ToString(), "Upload_On_Submit");
                        plist.Synchronize();
                    }
                    catch (Exception ex)
                    {
                        var eventItems = new List<NSDictionary>
                        {
                            getDictionary("Exception Message", ex.Message),
                            getDictionary("Exception Stacktrace", ex.StackTrace)
                        };
                        CommonFunctions.AddLogEvent(DateTime.Now, "ShowSettings", true, eventItems, "switchUploadOnSubmit ValueChanged catch block");

                        CommonFunctions.sendErrorEmail(ex);
                        ndia.PresentViewController(CommonFunctions.ExceptionAlertPrompt(ex), true, null);
                    }
                };

                sSection.Add(switchUploadOnSubmit);

                var linksWhite = GetBlankWhitePaddedView(15);
                sSection.Add(linksWhite);

                var btnRefreshAutoFiles = new UIButton(UIButtonType.System)
                {
                    Frame = new CGRect(5, 0, UIScreen.MainScreen.Bounds.Width - 250, 40),
                    HorizontalAlignment = UIControlContentHorizontalAlignment.Left
                };
                btnRefreshAutoFiles.SetTitle("Reload Auto Complete Files", UIControlState.Normal);
                btnRefreshAutoFiles.TouchUpInside += delegate
                {
                    var ReloadAutoDataPrompt = UIAlertController.Create("Reload Auto Data", "Re-download the auto complete files?", UIAlertControllerStyle.Alert);
                    ReloadAutoData(ndia, btnRefreshAutoFiles, ReloadAutoDataPrompt);
                    ReloadAutoDataPrompt.AddAction(UIAlertAction.Create("No", UIAlertActionStyle.Cancel, null));
                    //Present Alert
                    ndia.PresentViewController(ReloadAutoDataPrompt, true, null);
                };

                sSection.Add(btnRefreshAutoFiles);

                var btnRefreshPresetFiles = new UIButton(UIButtonType.System)
                {
                    Frame = new CGRect(5, 0, UIScreen.MainScreen.Bounds.Width - 250, 40),
                    HorizontalAlignment = UIControlContentHorizontalAlignment.Left
                };
                btnRefreshPresetFiles.SetTitle("Reload Preset Files", UIControlState.Normal);
                btnRefreshPresetFiles.TouchUpInside += delegate
                {
                    var ReloadPresetPrompt = UIAlertController.Create("Reload Presets", "Re-download the preset files?", UIAlertControllerStyle.Alert);
                    ReloadPresets(ndia, btnRefreshPresetFiles, ReloadPresetPrompt);
                    ReloadPresetPrompt.AddAction(UIAlertAction.Create("No", UIAlertActionStyle.Cancel, null));
                    //Present Alert
                    ndia.PresentViewController(ReloadPresetPrompt, true, null);
                };

                sSection.Add(btnRefreshPresetFiles);

                var btnDeleteMRFiles = new UIButton(UIButtonType.System)
                {
                    Frame = new CGRect(5, 0, UIScreen.MainScreen.Bounds.Width - 250, 40),
                    HorizontalAlignment = UIControlContentHorizontalAlignment.Left
                };
                btnDeleteMRFiles.SetTitle("Delete Medical Records", UIControlState.Normal);
                DeleteMRFiles(ndia, btnDeleteMRFiles);

                sSection.Add(btnDeleteMRFiles);

                var btnDeletePendingFiles = new UIButton(UIButtonType.System)
                {
                    Frame = new CGRect(5, 0, UIScreen.MainScreen.Bounds.Width - 250, 40),
                    HorizontalAlignment = UIControlContentHorizontalAlignment.Left
                };
                btnDeletePendingFiles.SetTitle("Delete All Pending Files", UIControlState.Normal);
                DeletePendingFiles(ndia, btnDeletePendingFiles);

                sSection.Add(btnDeletePendingFiles);

                var btnUploadLogFiles = new UIButton(UIButtonType.System)
                {
                    Frame = new CGRect(5, 0, UIScreen.MainScreen.Bounds.Width - 250, 40),
                    HorizontalAlignment = UIControlContentHorizontalAlignment.Left
                };
                btnUploadLogFiles.SetTitle("Upload Log Files", UIControlState.Normal);
                btnUploadLogFiles.TouchUpInside += delegate
                {
                    var UploadLogFilesPrompt = UIAlertController.Create("Upload Logs", "Upload and submit log files for evaluation?", UIAlertControllerStyle.Alert);
                    UploadLogFiles(ndia, btnUploadLogFiles, UploadLogFilesPrompt);
                    UploadLogFilesPrompt.AddAction(UIAlertAction.Create("No", UIAlertActionStyle.Cancel, null));
                    //Present Alert
                    ndia.PresentViewController(UploadLogFilesPrompt, true, null);
                };

                sSection.Add(btnUploadLogFiles);

                var btnDeleteLogFiles = new UIButton(UIButtonType.System)
                {
                    Frame = new CGRect(5, 0, UIScreen.MainScreen.Bounds.Width - 250, 40),
                    HorizontalAlignment = UIControlContentHorizontalAlignment.Left
                };
                btnDeleteLogFiles.SetTitle("Delete Logs", UIControlState.Normal);
                DeleteLogFiles(ndia, btnDeleteLogFiles);

                sSection.Add(btnDeleteLogFiles);

                SettingsView.Add(nsec);
                SettingsView.Add(sSection);

                var nroo = new RootElement("Settings") { nsec };

                nheadclosebtn.TouchUpInside += delegate
                {
                    NavigationController.DismissViewController(true, null);
                };

                NavigationController.PreferredContentSize = new CGSize(View.Bounds.Size);
                NavigationController.PresentViewController(ndia, true, null);
            }
            catch (Exception ex)
            {
                var eventItems = new List<NSDictionary>
                {
                    getDictionary("Exception Message", ex.Message),
                    getDictionary("Exception Stacktrace", ex.StackTrace)
                };
                CommonFunctions.AddLogEvent(DateTime.Now, "ShowSettings", true, eventItems, "catch block");

                CommonFunctions.sendErrorEmail(ex);
                PresentViewController(CommonFunctions.ExceptionAlertPrompt(ex), true, null);
            }
        }

        void UpdateDomain(UITextView txtDomain, UIButton btnDomain)
        {
            btnDomain.TouchUpInside += delegate
            {
                var UpdateDomainPrompt = UIAlertController.Create("Update Domain", "Updating the domain will require a re-login, continue?", UIAlertControllerStyle.Alert);
                UpdateDomainPrompt.AddAction(UIAlertAction.Create("Yes", UIAlertActionStyle.Default, action =>
                {
                    try
                    {
                        _lockObject = btnDomain;
                        lock (_lockObject)
                        {
                            if (isRunning)
                                return;
                            isRunning = true;
                        }

                        plist.SetString(txtDomain.Text, "Domain_Name");
                        plist.Synchronize();
                        NavigationController.DismissViewController(true, null);
                        Logout();
                    }
                    catch (Exception ex)
                    {
                        var eventItems = new List<NSDictionary>
                        {
                            getDictionary("Exception Message", ex.Message),
                            getDictionary("Exception Stacktrace", ex.StackTrace)
                        };
                        CommonFunctions.AddLogEvent(DateTime.Now, "UpdateDomain", true, eventItems, "catch block");

                        CommonFunctions.sendErrorEmail(ex);
                        PresentViewController(CommonFunctions.ExceptionAlertPrompt(ex), true, null);
                    }
                    finally
                    {
                        isRunning = false;
                    }
                }));
                UpdateDomainPrompt.AddAction(UIAlertAction.Create("No", UIAlertActionStyle.Cancel, null));
                //Present Alert
                PresentViewController(UpdateDomainPrompt, true, null);
            };
        }

        void DeletePendingFiles(DialogViewController ndia, UIButton btnDeletePendingFiles)
        {
            btnDeletePendingFiles.TouchUpInside += delegate
            {
                try
                {
                    var directoryname = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "DynaFilesAwaitingUpload/");
                    var di = new DirectoryInfo(directoryname);
                    var pendingfiles = di.GetFiles("*", SearchOption.AllDirectories);
                    long size = 0;
                    foreach (FileInfo fi in pendingfiles)
                    {
                        size += fi.Length;
                    }
                    double mbsize = (size / 1024f) / 1024f;

                    UIAlertController DeletePendingFilesPrompt;
                    string msg;

                    if (!pendingfiles.Any())
                    {
                        msg = "Pending files folder is empty";
                        DeletePendingFilesPrompt = UIAlertController.Create("Delete All Pending Files", msg, UIAlertControllerStyle.Alert);
                        DeletePendingFilesPrompt.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, null));
                    }
                    else
                    {
                        msg = "Delete " + pendingfiles.Count() + " files (" + mbsize.ToString("0.00") + "mb)?";
                        DeletePendingFilesPrompt = UIAlertController.Create("Delete All Pending Files", msg, UIAlertControllerStyle.Alert);
                        DeletePendingFilesPrompt.AddTextField((field) =>
                        {
                            field.SecureTextEntry = true;
                            field.Placeholder = "Password";
                        });
                        DeletePendingFilesPrompt.AddAction(UIAlertAction.Create("Yes", UIAlertActionStyle.Default, action =>
                        {
                            try
                            {
                                _lockObject = btnDeletePendingFiles;
                                lock (_lockObject)
                                {
                                    if (isRunning)
                                        return;
                                    isRunning = true;
                                }

                                loadingOverlay = new LoadingOverlay(ndia.TableView.Bounds, true);
                                loadingOverlay.SetText("Deleting Files...");
                                ndia.Add(loadingOverlay);

                                bool isValid = false;
                                isValid |= DeletePendingFilesPrompt.TextFields[0].Text == DynaClassLibrary.DynaClasses.LoginContainer.User.DynaPassword;

                                if (isValid)
                                {
                                    DeletePendingFiles(di);
                                }
                                else
                                {
                                    var failPass = "Wrong password. ";
                                    PresentViewController(CommonFunctions.AlertPrompt("Error", failPass, true, null, false, null), true, null);
                                }
                            }
                            catch (Exception ex)
                            {
                                var eventItems = new List<NSDictionary>
                                {
                                    getDictionary("Exception Message", ex.Message),
                                    getDictionary("Exception Stacktrace", ex.StackTrace)
                                };
                                CommonFunctions.AddLogEvent(DateTime.Now, "DeletePendingFiles", true, eventItems, "DeletePendingFilesPrompt action catch block");

                                CommonFunctions.sendErrorEmail(ex);
                                PresentViewController(CommonFunctions.ExceptionAlertPrompt(ex), true, null);
                            }
                            finally
                            {
                                isRunning = false;
                                loadingOverlay.Hide();
                            }
                        }));
                        DeletePendingFilesPrompt.AddAction(UIAlertAction.Create("No", UIAlertActionStyle.Cancel, null));
                    }

                    //Present Alert
                    ndia.PresentViewController(DeletePendingFilesPrompt, true, null);
                }
                catch (Exception ex)
                {
                    var eventItems = new List<NSDictionary>
                    {
                        getDictionary("Exception Message", ex.Message),
                        getDictionary("Exception Stacktrace", ex.StackTrace)
                    };
                    CommonFunctions.AddLogEvent(DateTime.Now, "DeletePendingFiles", true, eventItems, "catch block");

                    CommonFunctions.sendErrorEmail(ex);
                    PresentViewController(CommonFunctions.ExceptionAlertPrompt(ex), true, null);
                }
            };
        }

        void DeleteMRFiles(DialogViewController ndia, UIButton btnDeleteMRFiles)
        {
            btnDeleteMRFiles.TouchUpInside += delegate
            {
                var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                var cache = Path.Combine(documents, "..", "Library", "Caches");
                var directoryname = Path.Combine(cache, "DynaMedicalRecords");

                UIAlertController DeleteMRFilesPrompt;
                string msg;

                if (Directory.Exists(directoryname))
                {
                    var di = new DirectoryInfo(directoryname);
                    var mrfiles = di.GetFiles("*", SearchOption.AllDirectories);
                    long size = 0;
                    foreach (FileInfo fi in mrfiles)
                    {
                        size += fi.Length;
                    }
                    double mbsize = (size / 1024f) / 1024f;

                    if (!mrfiles.Any())
                    {
                        msg = "Medical records folder is empty";
                        DeleteMRFilesPrompt = UIAlertController.Create("Delete Medical Records", msg, UIAlertControllerStyle.Alert);
                        DeleteMRFilesPrompt.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, null));
                    }
                    else
                    {
                        msg = "Delete " + mrfiles.Count() + " files (" + mbsize.ToString("0.00") + "mb)?";
                        DeleteMRFilesPrompt = UIAlertController.Create("Delete Medical Records", msg, UIAlertControllerStyle.Alert);
                        DeleteMRFilesPrompt.AddTextField((field) =>
                        {
                            field.SecureTextEntry = true;
                            field.Placeholder = "Password";
                        });
                        DeleteMRFilesPrompt.AddAction(UIAlertAction.Create("Yes", UIAlertActionStyle.Default, action =>
                        {
                            try
                            {
                                _lockObject = btnDeleteMRFiles;
                                lock (_lockObject)
                                {
                                    if (isRunning)
                                        return;
                                    isRunning = true;
                                }

                                loadingOverlay = new LoadingOverlay(ndia.TableView.Bounds, true);
                                loadingOverlay.SetText("Deleting Files...");
                                ndia.Add(loadingOverlay);

                                bool isValid = false;
                                isValid |= DeleteMRFilesPrompt.TextFields[0].Text == DynaClassLibrary.DynaClasses.LoginContainer.User.DynaPassword;

                                if (isValid)
                                {
                                    DeleteMRFiles(di);
                                }
                                else
                                {
                                    var failPass = "Wrong password. ";
                                    PresentViewController(CommonFunctions.AlertPrompt("Error", failPass, true, null, false, null), true, null);
                                }
                            }
                            catch (Exception ex)
                            {
                                var eventItems = new List<NSDictionary>
                                {
                                getDictionary("Exception Message", ex.Message),
                                getDictionary("Exception Stacktrace", ex.StackTrace)
                                };
                                CommonFunctions.AddLogEvent(DateTime.Now, "DeleteMrFiles", true, eventItems, "catch block");

                                CommonFunctions.sendErrorEmail(ex);
                                PresentViewController(CommonFunctions.ExceptionAlertPrompt(ex), true, null);
                            }
                            finally
                            {
                                isRunning = false;
                                loadingOverlay.Hide();
                            }
                        }));
                        DeleteMRFilesPrompt.AddAction(UIAlertAction.Create("No", UIAlertActionStyle.Cancel, null));
                    }
                }
                else
                {
                    msg = "Medical records folder not found";
                    DeleteMRFilesPrompt = UIAlertController.Create("Delete Medical Records", msg, UIAlertControllerStyle.Alert);
                    DeleteMRFilesPrompt.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, null));
                }

                //Present Alert
                ndia.PresentViewController(DeleteMRFilesPrompt, true, null);
            };
        }

        void DeleteLogFiles(DialogViewController ndia, UIButton btnDeleteLogFiles)
        {
            btnDeleteLogFiles.TouchUpInside += delegate
            {
                var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                var directoryname = Path.Combine(documents, "DynaLog");
                var di = new DirectoryInfo(directoryname);
                var logfiles = di.GetFiles("*", SearchOption.AllDirectories);
                long size = 0;
                foreach (FileInfo fi in logfiles)
                {
                    size += fi.Length;
                }
                double mbsize = (size / 1024f) / 1024f;

                UIAlertController DeleteLogFilesPrompt;
                string msg;

                if (!logfiles.Any())
                {
                    msg = "There are no log files";
                    DeleteLogFilesPrompt = UIAlertController.Create("Delete Logs", msg, UIAlertControllerStyle.Alert);
                    DeleteLogFilesPrompt.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, null));
                }
                else
                {
                    msg = "Delete " + logfiles.Count() + " files (" + mbsize.ToString("0.00") + "mb)? (Re-login will be required)";
                    DeleteLogFilesPrompt = UIAlertController.Create("Delete Logs", msg, UIAlertControllerStyle.Alert);
                    DeleteLogFilesPrompt.AddTextField((field) =>
                    {
                        field.SecureTextEntry = true;
                        field.Placeholder = "Password";
                    });
                    DeleteLogFilesPrompt.AddAction(UIAlertAction.Create("Yes", UIAlertActionStyle.Default, action =>
                    {
                        try
                        {
                            _lockObject = btnDeleteLogFiles;
                            lock (_lockObject)
                            {
                                if (isRunning)
                                    return;
                                isRunning = true;
                            }

                            loadingOverlay = new LoadingOverlay(ndia.TableView.Bounds, true);
                            loadingOverlay.SetText("Deleting Files...");
                            ndia.Add(loadingOverlay);

                            bool isValid = false;
                            isValid |= DeleteLogFilesPrompt.TextFields[0].Text == DynaClasses.LoginContainer.User.DynaPassword;

                            if (isValid)
                            {
                                DeleteLogFiles(di);
                            }
                            else
                            {
                                var failPass = "Wrong password. ";
                                PresentViewController(CommonFunctions.AlertPrompt("Error", failPass, true, null, false, null), true, null);
                            }
                        }
                        catch (Exception ex)
                        {
                            var eventItems = new List<NSDictionary>
                            {
                                getDictionary("Exception Message", ex.Message),
                                getDictionary("Exception Stacktrace", ex.StackTrace)
                            };
                            CommonFunctions.AddLogEvent(DateTime.Now, "DeleteMrFiles", true, eventItems, "catch block");

                            CommonFunctions.sendErrorEmail(ex);
                            PresentViewController(CommonFunctions.ExceptionAlertPrompt(ex), true, null);
                        }
                        finally
                        {
                            isRunning = false;
                            loadingOverlay.Hide();
                        }
                    }));
                    DeleteLogFilesPrompt.AddAction(UIAlertAction.Create("No", UIAlertActionStyle.Cancel, null));
                }

                //Present Alert
                ndia.PresentViewController(DeleteLogFilesPrompt, true, null);
            };
        }

        void ReloadPresets(DialogViewController ndia, UIButton btnRefreshPresetFiles, UIAlertController ReloadPresetPrompt)
        {
            ReloadPresetPrompt.AddAction(UIAlertAction.Create("Yes", UIAlertActionStyle.Default, action =>
            {
                try
                {
                    _lockObject = btnRefreshPresetFiles;
                    lock (_lockObject)
                    {
                        if (isRunning)
                            return;
                        isRunning = true;
                    }

                    loadingOverlay = new LoadingOverlay(ndia.TableView.Bounds, true);
                    loadingOverlay.SetText("Downloading Files...");
                    ndia.Add(loadingOverlay);

                    SavePresetData(true);
                }
                catch (Exception ex)
                {
                    var eventItems = new List<NSDictionary>
                    {
                        getDictionary("Exception Message", ex.Message),
                        getDictionary("Exception Stacktrace", ex.StackTrace)
                    };
                    CommonFunctions.AddLogEvent(DateTime.Now, "ReloadPresets", true, eventItems, "catch block");

                    CommonFunctions.sendErrorEmail(ex);
                    PresentViewController(CommonFunctions.ExceptionAlertPrompt(ex), true, null);
                }
                finally
                {
                    isRunning = false;
                    loadingOverlay.Hide();
                }
            }));
        }

        void UploadLogFiles(DialogViewController ndia, UIButton btnUploadLogFiles, UIAlertController UploadLogFilesPrompt)
        {
            UploadLogFilesPrompt.AddAction(UIAlertAction.Create("Yes", UIAlertActionStyle.Default, action =>
            {
                try
                {
                    _lockObject = btnUploadLogFiles;
                    lock (_lockObject)
                    {
                        if (isRunning)
                            return;
                        isRunning = true;
                    }

                    loadingOverlay = new LoadingOverlay(ndia.TableView.Bounds, true);
                    loadingOverlay.SetText("Uploading Files...");
                    ndia.Add(loadingOverlay);

                    DoUploadLogFiles();
                }
                catch (Exception ex)
                {
                    var eventItems = new List<NSDictionary>
                    {
                        getDictionary("Exception Message", ex.Message),
                        getDictionary("Exception Stacktrace", ex.StackTrace)
                    };
                    CommonFunctions.AddLogEvent(DateTime.Now, "UploadLogFiles", true, eventItems, "catch block");

                    CommonFunctions.sendErrorEmail(ex);
                    PresentViewController(CommonFunctions.ExceptionAlertPrompt(ex), true, null);
                }
                finally
                {
                    isRunning = false;
                    loadingOverlay.Hide();
                }
            }));
        }

        void DoUploadLogFiles()
        {
            try
            {
                CommonFunctions.AddLogEvent(DateTime.Now, "DoUploadLogFiles", false, null, "start");

                var array = new List<DynaFile>();

                var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                var logDirectoryPath = Path.Combine(documents, "DynaLog");

                //var files = Directory.GetFiles(logDirectoryPath, "*.*", SearchOption.AllDirectories).o;

                var logDirectoryInfo = new DirectoryInfo(logDirectoryPath);
                var files = logDirectoryInfo.GetFiles("*.*", SearchOption.AllDirectories).OrderByDescending(p => p.CreationTime).Take(10).ToArray();

                foreach (var file in files)
                {
                    //var fi = new FileInfo(file);
                    var upload = new DynaFile
                    {
                        Json = "[" + File.ReadAllText(file.FullName) + "]",
                        DateCreated = file.CreationTime,
                        DateUploaded = DateTime.Now,
                        FileName = file.Name,
                        Type = "Log",
                        UserId = DynaClassLibrary.DynaClasses.LoginContainer.User.UserId,
                        UserConfig = CommonFunctions.GetUserConfig()
                    };

                    array.Add(upload);
                }

                CommonFunctions.AddLogEvent(DateTime.Now, "DoUploadLogFiles", false, null, "new DynaPadService.DynaPadService");

                var dds = new DynaPadService.DynaPadService { Timeout = 30000 };
                var result = dds.ProcessDynaFiles(CommonFunctions.GetUserConfig(), JsonConvert.SerializeObject(array));

                CommonFunctions.AddLogEvent(DateTime.Now, "DoUploadLogFiles", false, null, "after dds.ProcessDynaFiles");

                var toast = new Toast("Logs were uploaded");
                toast.SetDuration(5000);
                toast.SetType(ToastType.Info);
                toast.SetGravity(ToastGravity.Bottom);
                toast.Show();
            }
            catch (Exception ex)
            {
                var eventItems = new List<NSDictionary>
                    {
                        getDictionary("Exception Message", ex.Message),
                        getDictionary("Exception Stacktrace", ex.StackTrace)
                    };
                CommonFunctions.AddLogEvent(DateTime.Now, "DoUploadLogFiles", true, eventItems, "catch block");

                CommonFunctions.sendErrorEmail(ex);
                PresentViewController(CommonFunctions.ExceptionAlertPrompt(ex), true, null);
            }
            finally
            {
                CommonFunctions.AddLogEvent(DateTime.Now, "DoUploadLogFiles", false, null, "end");
            }
        }

        void ReloadAutoData(DialogViewController ndia, UIButton btnRefreshAutoFiles, UIAlertController ReloadAutoDataPrompt)
        {
            ReloadAutoDataPrompt.AddAction(UIAlertAction.Create("Yes", UIAlertActionStyle.Default, action =>
            {
                try
                {
                    _lockObject = btnRefreshAutoFiles;
                    lock (_lockObject)
                    {
                        if (isRunning)
                            return;
                        isRunning = true;
                    }

                    loadingOverlay = new LoadingOverlay(ndia.TableView.Bounds, true);
                    loadingOverlay.SetText("Downloading Files...");
                    ndia.Add(loadingOverlay);
                    SaveAutoData();
                }
                catch (Exception ex)
                {
                    var eventItems = new List<NSDictionary>
                    {
                        getDictionary("Exception Message", ex.Message),
                        getDictionary("Exception Stacktrace", ex.StackTrace)
                    };
                    CommonFunctions.AddLogEvent(DateTime.Now, "ReloadAutoData", true, eventItems, "catch block");

                    CommonFunctions.sendErrorEmail(ex);
                    PresentViewController(CommonFunctions.ExceptionAlertPrompt(ex), true, null);
                }
                finally
                {
                    isRunning = false;
                    loadingOverlay.Hide();
                }
            }));
        }

        public PaddedUIView<UILabel> GetBlankWhitePaddedView(int height)
        {
            var bwpv = new PaddedUIView<UILabel>
            {
                Frame = new CGRect(0, 0, UIScreen.MainScreen.Bounds.Width, height),
                Padding = 5f,
                Type = "White"
            };
            bwpv.setStyle();

            return bwpv;
        }

        public void DeleteLogFiles(DirectoryInfo di)
        {
            try
            {
                foreach (FileInfo file in di.EnumerateFiles())
                {
                    file.Delete();
                }

                var toast = new Toast("All logs were deleted");
                toast.SetDuration(5000);
                toast.SetType(ToastType.Info);
                toast.SetGravity(ToastGravity.Bottom);
                toast.Show();

                Logout();
            }
            catch (Exception ex)
            {
                var eventItems = new List<NSDictionary>
                {
                    getDictionary("Exception Message", ex.Message),
                    getDictionary("Exception Stacktrace", ex.StackTrace)
                };
                CommonFunctions.AddLogEvent(DateTime.Now, "DeleteLogFiles", true, eventItems, "catch block");

                CommonFunctions.sendErrorEmail(ex);
                PresentViewController(CommonFunctions.ExceptionAlertPrompt(ex), true, null);
            }
        }

        public void DeleteMRFiles(DirectoryInfo di)
        {
            try
            {
                foreach (FileInfo file in di.EnumerateFiles())
                {
                    file.Delete();
                }
                foreach (DirectoryInfo dir in di.EnumerateDirectories())
                {
                    dir.Delete(true);
                }

                var toast = new Toast("All medical records were deleted");
                toast.SetDuration(5000);
                toast.SetType(ToastType.Info);
                toast.SetGravity(ToastGravity.Bottom);
                toast.Show();
            }
            catch (Exception ex)
            {
                var eventItems = new List<NSDictionary>
                {
                    getDictionary("Exception Message", ex.Message),
                    getDictionary("Exception Stacktrace", ex.StackTrace)
                };
                CommonFunctions.AddLogEvent(DateTime.Now, "DeleteMRFiles", true, eventItems, "catch block");

                CommonFunctions.sendErrorEmail(ex);
                PresentViewController(CommonFunctions.ExceptionAlertPrompt(ex), true, null);
            }
        }



        public void DeletePendingFiles(DirectoryInfo di)
        {
            try
            {
                foreach (FileInfo file in di.EnumerateFiles())
                {
                    file.Delete();
                }
                foreach (DirectoryInfo dir in di.EnumerateDirectories())
                {
                    dir.Delete(true);
                }

                var toast = new Toast("All pending files were deleted");
                toast.SetDuration(5000);
                toast.SetType(ToastType.Info);
                toast.SetGravity(ToastGravity.Bottom);
                toast.Show();
            }
            catch (Exception ex)
            {
                var eventItems = new List<NSDictionary>
                {
                    getDictionary("Exception Message", ex.Message),
                    getDictionary("Exception Stacktrace", ex.StackTrace)
                };
                CommonFunctions.AddLogEvent(DateTime.Now, "DeletePendingFiles", true, eventItems, "catch block");

                CommonFunctions.sendErrorEmail(ex);
                PresentViewController(CommonFunctions.ExceptionAlertPrompt(ex), true, null);
            }
        }



        void ShowFeedbackList()
        {
            // This is where the feedback form gets displayed
            var feedbackManager = BITHockeyManager.SharedHockeyManager.FeedbackManager;
            feedbackManager.ShowFeedbackListView();
        }



        public StyledStringElement GetLogoutElement()
        {
            var logoutStringElement = new StyledStringElement("Logout", delegate
            {
                var LogoutPrompt = UIAlertController.Create("Logout", "Administrative use only. Logout?", UIAlertControllerStyle.Alert);
                LogoutPrompt.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, action => Logout()));
                LogoutPrompt.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, null));
                //Present Alert
                PresentViewController(LogoutPrompt, true, null);
            })
            {
                BackgroundColor = UIColor.FromRGB(255, 172, 172)
            };

            return logoutStringElement;
        }



        public void Logout()
        {
            try
            {
                DetailViewController = (DetailViewController)((UINavigationController)SplitViewController.ViewControllers[1]).TopViewController;
                DetailViewController.NavigationController.PopToRootViewController(true);
                DetailViewController.Root.Clear();

                mvc = (DialogViewController)((UINavigationController)SplitViewController.ViewControllers[0]).TopViewController;
                mvc.NavigationController.PopToRootViewController(true);
                mvc.Root.Clear();

                needLogin = true;
                didLogin = false;

                ViewDidAppear(true);
            }
            catch (Exception ex)
            {
                var eventItems = new List<NSDictionary>
                {
                    getDictionary("Exception Message", ex.Message),
                    getDictionary("Exception Stacktrace", ex.StackTrace)
                };
                CommonFunctions.AddLogEvent(DateTime.Now, "Logout", true, eventItems, "catch block");

                CommonFunctions.sendErrorEmail(ex);
                PresentViewController(CommonFunctions.ExceptionAlertPrompt(ex), true, null);
            }
        }



        public UIViewController GetDynaStart(RootElement rElement)
        {
            var timer = new Stopwatch();
            timer.Start();

            CommonFunctions.AddLogEvent(DateTime.Now, "GetDynaStart", false, null, "start");

            string menujson = "";
            try
            {
                var con = CrossConnectivity.Current;
                if (con.IsConnected)
                {
                    var dfElemet = (DynaFormRootElement)rElement;
                    DynaClassLibrary.DynaClasses.LoginContainer.User.SelectedLocation = DynaClassLibrary.DynaClasses.LoginContainer.User.Locations.Find(l => l.LocationId == dfElemet.MenuValue);

                    CommonFunctions.AddLogEvent(DateTime.Now, "GetDynaStart", false, null, "before new DynaPadService.DynaPadService");

                    var dds = new DynaPadService.DynaPadService { Timeout = 30000, AllowAutoRedirect = true };
                    var locid = string.IsNullOrEmpty(DynaClassLibrary.DynaClasses.LoginContainer.User.SelectedLocation.LocationId) ? null : DynaClassLibrary.DynaClasses.LoginContainer.User.SelectedLocation.LocationId;

                    if (string.IsNullOrEmpty(locid))
                    {
                        CheckStopwatch(timer, 0, "GetDynaStart Location Error", "GetDynaStart took " + timer.Elapsed.Seconds + " seconds");

                        PresentViewController(CommonFunctions.AlertPrompt("Location Error", "An active location is required in order to run this app, please contact administration", true, null, false, null), true, null);
                        return new DynaDialogViewController(new RootElement("No Location"), true);
                    }

                    menujson = dds.BuildDynaMenu(CommonFunctions.GetUserConfig(), locid, DynaClassLibrary.DynaClasses.LoginContainer.User.SelectedLocation.LocationName);

                    var eventItems = new List<NSDictionary>
                    {
                        getDictionary("Location ID", locid),
                        getDictionary("Location Name", DynaClassLibrary.DynaClasses.LoginContainer.User.SelectedLocation.LocationName)
                    };
                    CommonFunctions.AddLogEvent(DateTime.Now, "GetDynaStart", false, eventItems, "after dds.BuildDynaMenu");

                    myDynaMenu = JsonConvert.DeserializeObject<Menu>(menujson);
                    DetailViewController.DynaMenu = myDynaMenu;

                    DetailViewController.Root.Clear();

                    var rootMainMenu = new DynaFormRootElement(myDynaMenu.MenuCaption)
                    {
                        UnevenRows = true,
                        Enabled = true
                    };

                    var sectionMainMenu = new Section { HeaderView = null, FooterView = null };

                    var bmtimer = new Stopwatch();
                    bmtimer.Start();

                    GridSourceToday = new List<MenuItem>();
                    GridSourceToday = myDynaMenu.MenuItems.Find(x => x.MenuItemCaption == "Find By Patient").Menus[0].MenuItems.Find(y => y.MenuItemCaption == "Today").Menus[0].MenuItems;
                    DetailViewController.todayMenu = myDynaMenu;
                    IsGridSourceTodaySet = true;
                    GridSourceWeek = new List<MenuItem>();
                    IsGridSourceWeekSet = false;
                    GridSourceMonth = new List<MenuItem>();
                    IsGridSourceMonthSet = false;
                    GridSourceYesterday = new List<MenuItem>();
                    IsGridSourceYesterdaySet = false;
                    GridSourceUpcoming = new List<MenuItem>();
                    IsGridSourceUpcomingSet = false;
                    GridSourceOlder = new List<MenuItem>();
                    IsGridSourceOlderSet = false;

                    BuildMenu(myDynaMenu, sectionMainMenu);

                    CheckStopwatch(bmtimer, 0, "BuildMenu", "BuildMenu took " + bmtimer.Elapsed.Seconds + " seconds");

                    sectionMainMenu.Add(new SectionStringElement("Download Medical Records", delegate
                    {
                        DetailViewController.SetDetailItem(new Section("Download Medical Records"), "MRDownload", locid, null, false);
                    })
                    { mrdownload = true });

                    sectionMainMenu.Add(new SectionStringElement("Upload Submitted Forms", delegate
                    {
                        DetailViewController.SetDetailItem(new Section("Upload Submitted Forms"), "UploadSubmittedForms", locid, null, false);
                    })
                    { fileupload = true });

                    rootMainMenu.Add(sectionMainMenu);

                    var formDVC = new DynaDialogViewController(rootMainMenu, true);
                    formDVC.NavigationItem.LeftBarButtonItem = new UIBarButtonItem(UIImage.FromBundle("Back"), UIBarButtonItemStyle.Plain, delegate
                    {
                        DetailViewController.QuestionsView = null; //.Clear();
                        DetailViewController.Root.Clear();
                        DetailViewController.Root.Caption = "";

                        DetailViewController.NavigationItem.RightBarButtonItems = null;

                        DetailViewController.ReloadData();

                        NavigationController.PopViewController(true);
                    });

                    DetailViewController.SetSearch();

                    DetailViewController.SetDetailItem(new Section("Appointments Grid"), "ApptGrid", "Today", null, false);

                    CheckStopwatch(timer, 0, "GetDynaStart", "GetDynaStart took " + timer.Elapsed.Seconds + " seconds");

                    return formDVC;
                }

                CommonFunctions.AddLogEvent(DateTime.Now, "GetDynaStart", false, null, "end");

                CheckStopwatch(timer, 0, "GetDynaStart No Internet Error", "GetDynaStart took " + timer.Elapsed.Seconds + " seconds");

                PresentViewController(CommonFunctions.InternetAlertPrompt(), true, null);
                return new DynaDialogViewController(new RootElement("No internet"), true);
            }
            catch (Exception ex)
            {
                var eventItems = new List<NSDictionary>
                {
                    getDictionary("Exception Message", ex.Message),
                    getDictionary("Exception Stacktrace", ex.StackTrace)
                };
                CommonFunctions.AddLogEvent(DateTime.Now, "GetDynaStart", true, eventItems, "catch block");

                CommonFunctions.sendErrorEmail(ex);

                CheckStopwatch(timer, 0, "GetDynaStart Error", "GetDynaStart took " + timer.Elapsed.Seconds + " seconds");

                PresentViewController(CommonFunctions.ExceptionAlertPrompt(ex), true, null);
                return new DynaDialogViewController(CommonFunctions.ErrorRootElement(), true);
            }
        }


        public List<MenuItem> GridSourceToday;
        public List<MenuItem> GridSourceWeek;
        public List<MenuItem> GridSourceMonth;
        public List<MenuItem> GridSourceUpcoming;
        public List<MenuItem> GridSourceYesterday;
        public List<MenuItem> GridSourceOlder;
        public bool IsGridSourceTodaySet;
        public bool IsGridSourceWeekSet;
        public bool IsGridSourceMonthSet;
        public bool IsGridSourceUpcomingSet;
        public bool IsGridSourceYesterdaySet;
        public bool IsGridSourceOlderSet;

        public RootElement BuildMenu(Menu myMenu, Section sectionMenu)
        {
            try
            {
                if (myMenu.MenuItems == null) return null;

                CommonFunctions.AddLogEvent(DateTime.Now, "BuildMenu", false, null, "start");

                foreach (MenuItem mItem in myMenu.MenuItems)
                {
                    var rootMenu = new DynaFormRootElement(mItem.MenuItemCaption)
                    {
                        UnevenRows = true,
                        Enabled = true,
                        FormID = mItem.MenuItemValue,
                        FormName = mItem.MenuItemCaption,
                        MenuAction = mItem.MenuItemAction,
                        MenuValue = mItem.MenuItemValue,
                        PatientID = mItem.PatientId,
                        PatientName = mItem.PatientName,
                        DoctorID = mItem.DoctorId,
                        DoctorName = mItem.DoctorName,
                        PatientNotes = mItem.PatientNotes,
                        ApptNotes = mItem.ApptNotes,
                        LocationID = mItem.LocationId,
                        ApptID = mItem.ApptId,
                        CaseID = mItem.CaseId,
                        ApptDate = mItem.ApptDate,
                        ReportID = mItem.ReportId,
                        DateSubmittedPatientForm = mItem.DatePatientFormSubmitted,
                        DateSubmittedDoctorForm = mItem.DateDoctorFormSubmitted,
                        DateGeneratedReport = mItem.DateReportGenerated,
                        IsDoctorForm = mItem.MenuItemAction == "GetDoctorForm"
                    };

                    switch (mItem.MenuItemAction)
                    {

                        case "GetApptsUpcoming":
                        case "GetApptsYesterday":
                        case "GetApptsWeek":
                        case "GetApptsMonth":
                        case "GetApptsOlder":

                            rootMenu.ShowLoading = true;
                            rootMenu.createOnSelected = GetApptDatesService;
                            //rootMenu.OnSelected += delegate {

                            //    var dds = new DynaPadService.DynaPadService { Timeout = 30000, AllowAutoRedirect = true };
                            //    var locid = string.IsNullOrEmpty(DynaClassLibrary.DynaClasses.LoginContainer.User.SelectedLocation.LocationId) ? null : DynaClassLibrary.DynaClasses.LoginContainer.User.SelectedLocation.LocationId;

                            //    var menujson = dds.GetAppointmentsByDateFrame(CommonFunctions.GetUserConfig().ConnectionString, locid, mItem.MenuItemAction);
                            //    myDynaMenu = JsonConvert.DeserializeObject<Menu>(menujson);
                            //    DetailViewController.DynaMenu = myDynaMenu;

                            //    if (mItem.MenuItemAction == "GetApptsWeek")
                            //    {
                            //        GridSourceWeek = new List<MenuItem>();
                            //        GridSourceWeek = myDynaMenu.MenuItems.FindAll(x => x.GetType().ToString() == "MenuItem");
                            //    }
                            //    else if (mItem.MenuItemAction == "GetApptsMonth")
                            //    {
                            //        GridSourceMonth = new List<MenuItem>();
                            //        GridSourceMonth = myDynaMenu.MenuItems.FindAll(x => x.GetType().ToString() == "MenuItem");
                            //    }

                            //    var rootMainMenu = new DynaFormRootElement(myDynaMenu.MenuCaption)
                            //    {
                            //        UnevenRows = true,
                            //        Enabled = true
                            //    };

                            //    var sectionMainMenu = new Section { HeaderView = null, FooterView = null };

                            //    BuildMenu(myDynaMenu, sectionMainMenu);

                            //    var formDVC = new DynaDialogViewController(rootMainMenu, true);
                            //    formDVC.NavigationItem.LeftBarButtonItem = new UIBarButtonItem(UIImage.FromBundle("Back"), UIBarButtonItemStyle.Plain, delegate
                            //    {
                            //        DetailViewController.QuestionsView = null; //.Clear();
                            //        DetailViewController.Root.Clear();
                            //        DetailViewController.Root.Caption = "";
                            //        DetailViewController.ReloadData();

                            //        NavigationController.PopViewController(true);
                            //    });

                            //    NavigationController.PushViewController(formDVC, true);

                            //    DetailViewController.SetDetailItem(new Section("Appointments Grid"), "ApptGrid", "Today", null, false);
                            //};


                            break;
                        case "GetPatientForm":
                        case "GetDoctorForm":
                            rootMenu.ShowLoading = true;
                            rootMenu.createOnSelected = GetFormService;
                            break;
                        case "GetPatient":
                            rootMenu.createOnSelected = GetPatientService;

                            var pdocumentsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "DynaFilesAwaitingUpload/" + mItem.PatientId);

                            if (Directory.Exists(pdocumentsPath) && Directory.GetFiles(pdocumentsPath, "*.*", SearchOption.AllDirectories).Length > 0)
                            {
                                rootMenu.PendingUpdate = true;
                            }
                            break;
                        case "GetAppt":
                            rootMenu.createOnSelected = GetApptService;

                            var documentsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "DynaFilesAwaitingUpload/" + mItem.PatientId + "/" + mItem.ApptId);

                            if (Directory.Exists(documentsPath) && Directory.GetFiles(documentsPath, "*.*", SearchOption.AllDirectories).Length > 0)
                            {
                                rootMenu.PendingUpdate = true;
                            }

                            //DateTime startOfWeek = DateTime.Today;
                            //int delta = DayOfWeek.Monday - startOfWeek.DayOfWeek;
                            //startOfWeek = startOfWeek.AddDays(delta);
                            //DateTime endOfWeek = startOfWeek.AddDays(7);

                            //if (rootMenu.ApptDate >= startOfWeek && rootMenu.ApptDate < endOfWeek)
                            //{
                            //    GridSourceWeek.Add(mItem);
                            //}
                            //if (rootMenu.ApptDate.Month == mItem.ApptDate.Month && rootMenu.ApptDate.Year == mItem.ApptDate.Year)
                            //{
                            //    GridSourceMonth.Add(mItem);
                            //}
                            //if (rootMenu.ApptDate.Date == DateTime.Today.Date)
                            //{
                            //    GridSourceToday.Add(mItem);
                            //}
                            break;
                        case "GetApptForm":
                            rootMenu.createOnSelected = GetApptFormService;
                            break;
                        case "GetFiles":
                            rootMenu.ShowLoading = true;
                            rootMenu.createOnSelected = GetMRFoldersService;
                            break;
                        case "GetReport":
                            bool IsSelectedReport = false || mItem.MenuItemValue == mItem.ReportId;
                            var reportElement = new SectionStringElement(mItem.MenuItemCaption, delegate
                            {
                                secforcancel = sectionMenu;

                                LoadReportView(mItem.MenuItemValue, "Report", mItem.MenuItemCaption);
                                foreach (Element d in sectionMenu.Elements)
                                {
                                    var t = d.GetType();
                                    if (t == typeof(SectionStringElement))
                                    {
                                        var di = (SectionStringElement)d;
                                        if (di.selected)
                                        {
                                            oldsel = di.IndexPath;
                                        }
                                        di.selected = false;
                                    }
                                }
                                sectionMenu.GetContainerTableView().ReloadData();
                            })
                            { IsSelectedReport = IsSelectedReport };

                            sectionMenu.Add(reportElement);

                            //rootMenu.createOnSelected = GetReportService;
                            //Section sectionReport = new Section();
                            //sectionReport.Add(new StringElement(rootMenu.MenuValue, delegate { LoadReportView("Report", rootMenu.MenuValue); }));
                            //rootMenu.Add(sectionReport);
                            //DetailViewController.Root.Caption = mItem.MenuItemValue;
                            //DetailViewController.ReloadData();

                            break;
                        case "GetSummary":
                            sectionMenu.Add(new StringElement(mItem.MenuItemCaption, delegate { LoadSummaryView(mItem.MenuItemValue, "Summary"); }));
                            break;
                        case "UploadAppt":
                            sectionMenu.Add(new SectionStringElement(mItem.MenuItemCaption, delegate
                            {
                                DetailViewController.SetDetailItem(new Section(mItem.MenuItemCaption), "UploadSubmittedPatientForms", mItem.MenuItemValue, null, false);
                            })
                            { fileupload = true });
                            break;
                        default:
                            rootMenu.OnSelected += delegate
                            {
                                if (mItem.MenuItemCaption == "Today" || mItem.MenuItemCaption == "Week" || mItem.MenuItemCaption == "Month" || mItem.MenuItemCaption == "Upcoming" || mItem.MenuItemCaption == "Yesterday" || mItem.MenuItemCaption == "Older")
                                {
                                    CurrentApptGridType = mItem.MenuItemCaption;
                                    DetailViewController.SetDetailItem(new Section("Appointments Grid"), "ApptGrid", mItem.MenuItemCaption, null, false);
                                }
                                else if (mItem.MenuItemCaption != "Find By Patient")
                                {
                                    DetailViewController.QuestionsView = null; //.Clear();
                                    DetailViewController.Root.Clear();
                                    DetailViewController.Root.Caption = "";
                                    DetailViewController.ReloadData();
                                }
                            };
                            break;

                            //case "Logout":
                            //	var feedbackStringElement = new StyledStringElement("Feedback", ShowFeedbackList);
                            //	var logoutStringElement = new StyledStringElement(mItem.MenuItemCaption, delegate
                            //	{
                            //		var LogoutPrompt = UIAlertController.Create("Logout", "Administrative use only. Logout?", UIAlertControllerStyle.Alert);
                            //		LogoutPrompt.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, action => Logout()));
                            //		LogoutPrompt.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, null));
                            //	//Present Alert
                            //	PresentViewController(LogoutPrompt, true, null);
                            //	});
                            //	logoutStringElement.BackgroundColor = UIColor.FromRGB(255, 172, 172);
                            //	sectionMenu.Add(feedbackStringElement);
                            //	sectionMenu.Add(logoutStringElement);
                            //	break;

                    }

                    if (mItem.MenuItemAction != "GetReport" && mItem.MenuItemAction != "GetSummary" && mItem.MenuItemAction != "UploadAppt" && mItem.MenuItemAction != "Logout")
                    {
                        sectionMenu.Add(rootMenu);
                    }

                    if (mItem.Menus == null) return null;

                    foreach (Menu mRoot in mItem.Menus)
                    {
                        var newSection = new Section();
                        BuildMenu(mRoot, newSection);
                        rootMenu.Add(newSection);
                    }
                }

                CommonFunctions.AddLogEvent(DateTime.Now, "BuildMenu", false, null, "end");

                return null;
            }
            catch (Exception ex)
            {
                var eventItems = new List<NSDictionary>
                {
                    getDictionary("Exception Message", ex.Message),
                    getDictionary("Exception Stacktrace", ex.StackTrace)
                };
                CommonFunctions.AddLogEvent(DateTime.Now, "BuildMenu", true, eventItems, "catch block");

                CommonFunctions.sendErrorEmail(ex);
                PresentViewController(CommonFunctions.ExceptionAlertPrompt(ex), true, null);

                return null;
            }
        }

        string CurrentApptGridType;


        void SaveAutoData()//(string qid)
        {
            var timer = new Stopwatch();

            CommonFunctions.AddLogEvent(DateTime.Now, "SaveAutoData", false, null, "start");

            try
            {
                timer.Start();

                var array = new NSMutableArray();

                CommonFunctions.AddLogEvent(DateTime.Now, "SaveAutoData", false, null, "new DynaPadService.DynaPadService");

                var dds = new DynaPadService.DynaPadService { Timeout = 30000 };

                var autofiles = dds.GetAllAutoBoxData(DynaClassLibrary.DynaClasses.LoginContainer.User.DynaConfig.DomainRootPathPhysical, DynaClassLibrary.DynaClasses.LoginContainer.User.DynaConfig.DomainRootPathVirtual);

                var eventItems = new List<NSDictionary>
                {
                    getDictionary("Root Physical", DynaClassLibrary.DynaClasses.LoginContainer.User.DynaConfig.DomainRootPathPhysical),
                    getDictionary("Root Virtual", DynaClassLibrary.DynaClasses.LoginContainer.User.DynaConfig.DomainRootPathVirtual)
                };
                CommonFunctions.AddLogEvent(DateTime.Now, "SaveAutoData", false, eventItems, "after dds.GetAllAutoBoxData");

                //var autofiles = new List<KeyValuePair<string, string>>();
                //autofiles.Add(new KeyValuePair<string, string>("2893", DynaClassLibrary.DynaClasses.LoginContainer.User.DynaConfig.DomainRootPathVirtual + "DynaForms/106/AutoBoxLists/2893.txt"));
                //autofiles.Add(new KeyValuePair<string, string>("3208", DynaClassLibrary.DynaClasses.LoginContainer.User.DynaConfig.DomainRootPathVirtual + "DynaForms/106/AutoBoxLists/3208.txt"));
                //autofiles.Add(new KeyValuePair<string, string>("1043", DynaClassLibrary.DynaClasses.LoginContainer.User.DynaConfig.DomainRootPathVirtual + "DynaForms/76/AutoBoxLists/1043.txt"));

                var deserializedautofiles = JsonConvert.DeserializeObject<List<Tuple<string, string>>>(autofiles);

                foreach (Tuple<string, string> autofile in deserializedautofiles)
                {
                    var url = new Uri(autofile.Item2);

                    var webClient = new WebClient();

                    webClient.DownloadStringCompleted += (s, e) =>
                    {
                        var text = e.Result; // get the downloaded text
                        //Console.WriteLine($"\nDownloaded: {e.Result}");

                        var fileidentity = autofile.Item1;

                        //var backups = new DirectoryInfo(directoryname).GetFiles("*" + fileidentity + "*", SearchOption.AllDirectories).OrderByDescending(x => x.LastWriteTime);

                        var documentsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "DynaAutoBoxes");
                        if (!Directory.Exists(documentsPath))
                        {
                            Directory.CreateDirectory(documentsPath);
                        }
                        string localFilename = autofile.Item1 + ".txt";
                        var localPath = Path.Combine(documentsPath, localFilename);
                        if (File.Exists(localPath))
                        {
                            var existingFileInfo = new FileInfo(localPath);
                            if (existingFileInfo.Length != text.Length)
                            {
                                File.Delete(localPath);
                                File.WriteAllText(localPath, text); // writes to local storage
                                Console.WriteLine("New auto data file REPLACED to : {0}", localPath);
                            }
                            else
                            {
                                Console.WriteLine("New auto data file matches old file, IGNORED : {0}", localPath);
                            }
                        }
                        else
                        {
                            File.WriteAllText(localPath, text); // writes to local storage
                            Console.WriteLine("New auto data file SAVED to : {0}", localPath);
                        }
                    };

                    webClient.Encoding = System.Text.Encoding.UTF8;
                    webClient.DownloadStringAsync(url);
                }

                CommonFunctions.AddLogEvent(DateTime.Now, "SaveAutoData", false, null, "end");

                CheckStopwatch(timer, 0, "SaveAutoData", "SaveAutoData took " + timer.Elapsed.Seconds + " seconds");
            }
            catch (Exception ex)
            {
                var eventItems = new List<NSDictionary>
                {
                    getDictionary("Exception Message", ex.Message),
                    getDictionary("Exception Stacktrace", ex.StackTrace)
                };
                CommonFunctions.AddLogEvent(DateTime.Now, "SaveAutoData", true, eventItems, "catch block");

                CommonFunctions.sendErrorEmail(ex);

                CheckStopwatch(timer, 0, "SaveAutoData Error", "SaveAutoData took " + timer.Elapsed.Seconds + " seconds");

                PresentViewController(CommonFunctions.AlertPrompt("Download Auto Data Files Failure", "There was a problem downloading location auto data files, if problem persists please re-login to the app.", true, null, false, null), true, null);
            }
        }


        // if in future can create presets outside of ipad might cause problems here!
        public void SavePresetData(bool ForceUpdate = false)//(string qid)
        {
            var timer = new Stopwatch();

            CommonFunctions.AddLogEvent(DateTime.Now, "SavePresetData", false, null, "start");

            try
            {
                timer.Start();

                var array = new NSMutableArray();

                CommonFunctions.AddLogEvent(DateTime.Now, "SavePresetData", false, null, "new DynaPadService.DynaPadService");

                var dds = new DynaPadService.DynaPadService { Timeout = 30000 };
                var presetfiles = dds.GetAllAnswerPresets(CommonFunctions.GetUserConfig(), DynaClassLibrary.DynaClasses.LoginContainer.User.UserId, ForceUpdate);

                var eventItems_GetAllAnswerPresets = new List<NSDictionary>
                {
                    getDictionary("User ID", DynaClassLibrary.DynaClasses.LoginContainer.User.UserId),
                    getDictionary("Is Force Update", ForceUpdate.ToString())
                };
                CommonFunctions.AddLogEvent(DateTime.Now, "SavePresetData", false, eventItems_GetAllAnswerPresets, "after dds.GetAllAnswerPresets");

                var deserializedpresetfiles = JsonConvert.DeserializeObject<List<DynaPreset>>(presetfiles);

                var dcount = deserializedpresetfiles.Count;

                //deserializedpresetfiles.Remove(deserializedpresetfiles.Find(y => y.PresetName == "dan"));

                var localPresets = GetPresetData(null, null, true);

                if (ForceUpdate)
                {
                    foreach (DynaPreset lfile in localPresets)
                    {
                        //if (!deserializedpresetfiles.Exists(x => x.PresetId == lfile.PresetId))
                        //{
                        var lfileidentity = lfile.PresetId;
                        string llocalFilename = lfileidentity + ".txt";
                        string llocalPath;

                        var ldocumentsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "DynaPresets/" + lfile.FormId + "/" + lfile.DoctorId);
                        if (!string.IsNullOrEmpty(lfile.SectionId))
                        {
                            var ldocumentsSectionPath = Path.Combine(ldocumentsPath, lfile.SectionId);

                            llocalPath = Path.Combine(ldocumentsSectionPath, llocalFilename);
                        }
                        else
                        {
                            llocalPath = Path.Combine(ldocumentsPath, llocalFilename);
                        }

                        if (File.Exists(llocalPath))
                        {
                            File.Delete(llocalPath);
                            Console.WriteLine("Local preset file DELETED at : {0}", llocalPath);
                        }
                        //}
                    }
                }

                Console.WriteLine("Starting download of : {0} preset files", dcount);

                if (dcount > 0)
                {
                    var toast = new Toast("Starting download of " + dcount + " preset files");
                    toast.SetDuration(5000);
                    toast.SetType(ToastType.Info);
                    toast.SetGravity(ToastGravity.Bottom);
                    toast.Show();
                }

                foreach (DynaPreset presetfile in deserializedpresetfiles)
                {
                    var url = new Uri(presetfile.PresetFileUrl);

                    var webClient = new WebClient();

                    webClient.DownloadStringCompleted += (s, e) =>
                    {
                        var text = e.Result; // get the downloaded text
                        //Console.WriteLine($"\nDownloaded: {e.Result}");

                        presetfile.PresetJson = text;
                        var newFileInfo = JsonConvert.SerializeObject(presetfile);

                        var fileidentity = presetfile.PresetId;
                        string localFilename = fileidentity + ".txt";
                        string localPath;

                        var documentsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "DynaPresets/" + presetfile.FormId + "/" + presetfile.DoctorId);
                        if (!Directory.Exists(documentsPath))
                        {
                            Directory.CreateDirectory(documentsPath);
                        }
                        if (!string.IsNullOrEmpty(presetfile.SectionId))
                        {
                            var documentsSectionPath = Path.Combine(documentsPath, presetfile.SectionId);
                            if (!Directory.Exists(documentsSectionPath))
                            {
                                Directory.CreateDirectory(documentsSectionPath);
                            }

                            localPath = Path.Combine(documentsSectionPath, localFilename);
                        }
                        else
                        {
                            localPath = Path.Combine(documentsPath, localFilename);
                        }

                        if (File.Exists(localPath))
                        {
                            var existingFileInfo = new FileInfo(localPath);
                            //if (existingFileInfo.Length != text.Length || ForceUpdate)
                            if (existingFileInfo.Length != newFileInfo.Length || ForceUpdate)
                            {
                                File.Delete(localPath);
                                File.WriteAllText(localPath, newFileInfo); // writes to local storage
                                Console.WriteLine("New preset file REPLACED to : {0}", localPath);
                            }
                            else
                            {
                                Console.WriteLine("New preset file matches old file, IGNORED : {0}", localPath);
                            }
                        }
                        else
                        {
                            File.WriteAllText(localPath, newFileInfo); // writes to local storage
                            Console.WriteLine("New preset file SAVED to : {0}", localPath);
                        }
                    };

                    webClient.Encoding = System.Text.Encoding.UTF8;
                    webClient.DownloadStringAsync(url);

                    if (dcount > 0)
                    {
                        var ftoast = new Toast("Finished download of " + dcount + " preset files");
                        ftoast.SetDuration(5000);
                        ftoast.SetType(ToastType.Info);
                        ftoast.SetGravity(ToastGravity.Bottom);
                        ftoast.Show();
                    }
                }

                var requestDateTimeUTC = DateTime.UtcNow.ToString();
                dds.LogPresetRequest(CommonFunctions.GetUserConfig(), DynaDeviceUniqueName, requestDateTimeUTC, dcount);

                var eventItems_LogPresetRequest = new List<NSDictionary>
                {
                    getDictionary("Device Name", DynaDeviceUniqueName),
                    getDictionary("Request DateTimeUTC", requestDateTimeUTC),
                    getDictionary("Preset Count", dcount.ToString())
                };
                CommonFunctions.AddLogEvent(DateTime.Now, "SavePresetData", false, eventItems_LogPresetRequest, "after dds.LogPresetRequest");

                CommonFunctions.AddLogEvent(DateTime.Now, "SavePresetData", false, null, "end");

                CheckStopwatch(timer, 0, "SavePresetData", "SavePresetData took " + timer.Elapsed.Seconds + " seconds");
            }
            catch (Exception ex)
            {
                var eventItems = new List<NSDictionary>
                {
                    getDictionary("Exception Message", ex.Message),
                    getDictionary("Exception Stacktrace", ex.StackTrace)
                };
                CommonFunctions.AddLogEvent(DateTime.Now, "SavePresetData", true, eventItems, "catch block");

                CommonFunctions.sendErrorEmail(ex);
                CheckStopwatch(timer, 0, "SavePresetData Error", "SavePresetData took " + timer.Elapsed.Seconds + " seconds");
                PresentViewController(CommonFunctions.AlertPrompt("Download Preset Files Failure", "There was a problem downloading location preset files, if problem persists please re-login to the app.", true, null, false, null), true, null);
            }
        }



        //void SavePresetData(bool ForceUpdate = false)//(string qid)
        //{
        //    var timer = new Stopwatch();

        //    try
        //    {
        //        timer.Start();

        //        var array = new NSMutableArray();

        //        var dds = new DynaPadService.DynaPadService { Timeout = 30000 };
        //        var presetfiles = dds.GetAllAnswerPresets(CommonFunctions.GetUserConfig(), DynaClassLibrary.DynaClasses.LoginContainer.User.UserId);
        //        var deserializedpresetfiles = JsonConvert.DeserializeObject<List<DynaPreset>>(presetfiles);
        //        //{ presetFormId, presetDoctorId, presetLocationId, presetSectionId, presetName, presetJson, presetId, domainConfig.DomainRootPathPhysical + presetPath }

        //        foreach (DynaPreset presetfile in deserializedpresetfiles)
        //        {
        //            //var url = new Uri(presetfile.PresetFileUrl);

        //            //var webClient = new WebClient();

        //            //webClient.DownloadStringCompleted += (s, e) =>
        //            //{
        //                //var text = e.Result; // get the downloaded text
        //                var newFileInfo = JsonConvert.SerializeObject(presetfile);

        //                var fileidentity = presetfile.PresetId;
        //                string localFilename = fileidentity + ".txt";
        //                string localPath;

        //                var documentsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "DynaPresets/" + presetfile.FormId + "/" + presetfile.DoctorId);
        //                if (!Directory.Exists(documentsPath))
        //                {
        //                    Directory.CreateDirectory(documentsPath);
        //                }
        //                if (!string.IsNullOrEmpty(presetfile.SectionId))
        //                {
        //                    var documentsSectionPath = Path.Combine(documentsPath, presetfile.SectionId);
        //                    if (!Directory.Exists(documentsSectionPath))
        //                    {
        //                        Directory.CreateDirectory(documentsSectionPath);
        //                    }

        //                    localPath = Path.Combine(documentsSectionPath, localFilename);
        //                }
        //                else
        //                {
        //                    localPath = Path.Combine(documentsPath, localFilename);
        //                }

        //                if (File.Exists(localPath))
        //                {
        //                    var existingFileInfo = new FileInfo(localPath);
        //                    //if (existingFileInfo.Length != text.Length || ForceUpdate)
        //                    if (existingFileInfo.Length != newFileInfo.Length || ForceUpdate)
        //                    {
        //                        File.Delete(localPath);
        //                        File.WriteAllText(localPath, newFileInfo); // writes to local storage
        //                        Console.WriteLine("New preset file REPLACED to : {0}", localPath);
        //                    }
        //                    else
        //                    {
        //                        Console.WriteLine("New preset file matches old file, IGNORED : {0}", localPath);
        //                    }
        //                }
        //                else
        //                {
        //                    File.WriteAllText(localPath, newFileInfo); // writes to local storage
        //                    Console.WriteLine("New preset file MOVED to : {0}", localPath);
        //                }
        //            //};

        //            //webClient.Encoding = System.Text.Encoding.UTF8;
        //            //webClient.DownloadStringAsync(url);
        //        }

        //        CheckStopwatch(timer, 0, "SavePresetData", "SavePresetData took " + timer.Elapsed.Seconds + " seconds");
        //    }
        //    catch (Exception ex)
        //    {
        //        CommonFunctions.sendErrorEmail(ex);

        //        CheckStopwatch(timer, 0, "SavePresetData Error", "SavePresetData took " + timer.Elapsed.Seconds + " seconds");

        //        PresentViewController(CommonFunctions.AlertPrompt("Download Preset Files Failure", "There was a problem downloading location preset files, if problem persists please re-login to the app.", true, null, false, null), true, null);
        //    }
        //}


        List<DynaPreset> GetPresetData(string formid, string doctorid, bool getall)
        {
            var array = new List<DynaPreset>();

            string documentsPath;
            string[] files;

            if (getall)
            {
                documentsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "DynaPresets/");
                if (!Directory.Exists(documentsPath))
                {
                    Directory.CreateDirectory(documentsPath);
                }
                files = Directory.GetFiles(documentsPath, "*.*", SearchOption.AllDirectories);
            }
            else
            {
                documentsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "DynaPresets/" + formid + "/" + doctorid);
                if (!Directory.Exists(documentsPath))
                {
                    Directory.CreateDirectory(documentsPath);
                }
                files = Directory.GetFiles(documentsPath);
            }

            foreach (string file in files)
            {
                array.Add(JsonConvert.DeserializeObject<DynaPreset>(File.ReadAllText(file)));
            }

            return array;
        }

        //[Outlet]
        //public UIPopoverController DetailViewPopover { get; set; }
        //public UIPopoverController MainPopoverController { get; set; }

        [Outlet]
        public NSObject LastTappedButton { get; set; }
        public object ResourceLoader { get; private set; }

        UIBarButtonItem restorebtn;

        public Boolean isRunning;// = false;
        Object _lockObject = new object();

        public UIViewController GetFormService(RootElement rElement)
        {
            var timer = new Stopwatch();

            try
            {
                timer.Start();

                CommonFunctions.AddLogEvent(DateTime.Now, "GetFormService", false, null, "start");

                DetailViewController.NavigationItem.RightBarButtonItem = null;
                DetailViewController.NavigationItem.LeftBarButtonItems = null;

                //if (DetailViewController.QuestionsView != null)
                //{
                //  DetailViewController.Title = "";
                //  DetailViewController.QuestionsView = null; //.Clear();
                //}
                //if (CrossConnectivity.Current.IsConnected)
                //{
                //var bounds = UIScreen.MainScreen.Bounds;
                //var bounds = base.TableView.Frame;
                // show the loading overlay on the UI thread using the correct orientation sizing
                //loadingOverlay = new LoadingOverlay(bounds);
                //mvc = (DialogViewController)((UINavigationController)SplitViewController.ViewControllers[0]).TopViewController;
                //SplitViewController.Add(loadingOverlay);

                var dfElemet = (DynaFormRootElement)rElement;

                bool IsDoctorForm = dfElemet.IsDoctorForm;


                string origJson;// = dds.GetFormQuestions(CommonFunctions.GetUserConfig(), dfElemet.FormID, dfElemet.DoctorID, dfElemet.LocationID, dfElemet.PatientID, dfElemet.PatientName, SelectedAppointment.CaseId, SelectedAppointment.ApptId, dfElemet.IsDoctorForm);

                if (dfElemet.RestoreFile == null)
                {
                    var documentsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "DynaFilesAwaitingUpload/" + dfElemet.PatientID + "/" + SelectedAppointment.ApptId);
                    IOrderedEnumerable<FileInfo> dfap = null;
                    if (Directory.Exists(documentsPath))
                    {
                        var dfaptype = "patient";
                        if (IsDoctorForm)
                        {
                            dfaptype = "doctor";
                        }

                        dfap = new DirectoryInfo(documentsPath).GetFiles().Where(x => x.Name.StartsWith("Form_" + dfaptype, StringComparison.CurrentCulture)).OrderByDescending(x => x.LastWriteTime);
                    }

                    if (dfap != null && dfap.Any())
                    {
                        SelectedAppointment.SelectedQForm = JsonConvert.DeserializeObject<QForm>(JsonConvert.DeserializeObject<DynaFile>(File.ReadAllText(dfap.First().FullName)).Json);
                        origJson = JsonConvert.SerializeObject(SelectedAppointment.SelectedQForm);
                    }
                    else
                    {
                        CommonFunctions.AddLogEvent(DateTime.Now, "GetFormService", false, null, "new DynaPadService.DynaPadService");

                        var dds = new DynaPadService.DynaPadService { Timeout = 30000 };
                        origJson = dds.GetFormQuestions(CommonFunctions.GetUserConfig(), dfElemet.FormID, dfElemet.DoctorID, dfElemet.LocationID, dfElemet.PatientID, dfElemet.PatientName, SelectedAppointment.CaseId, SelectedAppointment.ApptId, dfElemet.IsDoctorForm);

                        var eventItems_GetFormQuestions = new List<NSDictionary>                         {
                            getDictionary("Form ID", dfElemet.FormID),
                            getDictionary("Doctor ID", dfElemet.DoctorID),
                            getDictionary("Location ID", dfElemet.LocationID),
                            getDictionary("Patient ID", dfElemet.PatientID),
                            getDictionary("Patient Name", dfElemet.PatientName),
                            getDictionary("Case ID", SelectedAppointment.CaseId),
                            getDictionary("Appointment ID", SelectedAppointment.ApptId),
                            getDictionary("Is Doctor Form", dfElemet.IsDoctorForm.ToString())
                        };
                        CommonFunctions.AddLogEvent(DateTime.Now, "GetFormService", false, eventItems_GetFormQuestions, "after dds.GetFormQuestions");

                        JsonHandler.OriginalFormJsonString = origJson;
                        SelectedAppointment.SelectedQForm = JsonConvert.DeserializeObject<QForm>(origJson);
                    }
                }
                else
                {
                    var restoreFile = File.ReadAllText(dfElemet.RestoreFile.FullName);

                    origJson = restoreFile;

                    JsonHandler.OriginalFormJsonString = restoreFile;

                    SelectedAppointment.SelectedQForm = JsonConvert.DeserializeObject<QForm>(origJson);

                    DynaClassLibrary.DynaClasses.LoginContainer.User.SelectedLocation = DynaClassLibrary.DynaClasses.LoginContainer.User.Locations.Find(l => l.LocationId == SelectedAppointment.SelectedQForm.LocationId);

                    SelectedAppointment.ApptPatientId = SelectedAppointment.SelectedQForm.PatientId;
                    SelectedAppointment.ApptPatientName = SelectedAppointment.SelectedQForm.PatientName;
                    SelectedAppointment.ApptDoctorId = SelectedAppointment.SelectedQForm.DoctorId;
                    SelectedAppointment.ApptLocationId = SelectedAppointment.SelectedQForm.LocationId;
                    SelectedAppointment.ApptId = SelectedAppointment.SelectedQForm.ApptId;
                    //SelectedAppointment.CaseId = SelectedAppointment.SelectedQForm.FormId;
                    SelectedAppointment.ApptFormId = SelectedAppointment.SelectedQForm.FormId;
                    SelectedAppointment.ApptFormName = SelectedAppointment.SelectedQForm.FormName;

                    dfElemet.UnevenRows = true;
                    dfElemet.Enabled = true;
                    dfElemet.FormID = SelectedAppointment.SelectedQForm.FormId;
                    dfElemet.FormName = SelectedAppointment.SelectedQForm.FormName;
                    dfElemet.MenuValue = SelectedAppointment.SelectedQForm.LocationId;
                    dfElemet.PatientID = SelectedAppointment.SelectedQForm.PatientId;
                    dfElemet.PatientName = SelectedAppointment.SelectedQForm.PatientName;
                    dfElemet.DoctorID = SelectedAppointment.SelectedQForm.DoctorId;
                    dfElemet.LocationID = SelectedAppointment.SelectedQForm.LocationId;
                    dfElemet.ApptID = SelectedAppointment.SelectedQForm.ApptId;
                    dfElemet.ReportID = SelectedAppointment.ApptReportId;
                    dfElemet.ShowLoading = true;
                }

                DetailViewController.Root.Caption = SelectedAppointment.SelectedQForm.FormName + " - " + SelectedAppointment.ApptPatientName;
                DetailViewController.ReloadData();

                var navTitle = IsDoctorForm ? "Doctor Form" : "Patient Form";
                var sectionsGroup = new RadioGroup("sections", -1);
                var rootFormSections = new RootElement(navTitle, sectionsGroup);
                var sectionFormSections = new Section();

                if (IsDoctorForm)
                {
                    //var FormPresetNames = dds.GetAnswerPresets(CommonFunctions.GetUserConfig(), SelectedAppointment.ApptFormId, null, SelectedAppointment.ApptDoctorId, SelectedAppointment.ApptLocationId);
                    var SectionPresets = GetPresetData(SelectedAppointment.ApptFormId, SelectedAppointment.ApptDoctorId, false);

                    var formPresetSection = new DynaSection("Form Presets") { Enabled = true };
                    var formPresetGroup = new RadioGroup("FormPresetAnswers", SelectedAppointment.SelectedQForm.FormSelectedTemplateId);
                    var formPresetsRoot = new DynaRootElement("Form Presets", formPresetGroup) { IsPreset = true };

                    var noPresetRadio = new PresetRadioElement("No Preset", "FormPresetAnswers") { PresetName = "No Preset" };
                    noPresetRadio.OnSelected += delegate
                    {
                        var NoPresetPrompt = UIAlertController.Create("No Preset", "You might lose any unsaved data, continue?", UIAlertControllerStyle.Alert);
                        NoPresetPrompt.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, action =>
                        {
                            JsonHandler.OriginalFormJsonString = origJson;
                            SelectedAppointment.SelectedQForm = JsonConvert.DeserializeObject<QForm>(origJson);

                            LoadSectionView(SelectedAppointment.SelectedQForm.FormSections[0].SectionId, SelectedAppointment.SelectedQForm.FormSections[0].SectionName, SelectedAppointment.SelectedQForm.FormSections[0], IsDoctorForm, sectionFormSections);
                        }));
                        NoPresetPrompt.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, null));
                        //Present Alert
                        PresentViewController(NoPresetPrompt, true, null);
                    };

                    formPresetSection.Add(noPresetRadio);

                    var fs = SelectedAppointment.SelectedQForm.FormSections.IndexOf(SelectedAppointment.SelectedQForm.FormSections[0]);

                    //foreach (string[] arrPreset in FormPresetNames)
                    //{
                    //    var radioPreset = GetPreset(arrPreset[3], arrPreset[1], arrPreset[2], SelectedAppointment.SelectedQForm.FormSections[0].SectionId, formPresetGroup, formPresetSection, origJson, sectionFormSections, IsDoctorForm);

                    //    //var radioPreset = new PresetRadioElement(arrPreset[1], "FormPresetAnswers");
                    //    //radioPreset.PresetName = arrPreset[1];
                    //    //radioPreset.PresetJson = arrPreset[2];
                    //    //radioPreset.OnSelected += delegate (object sender, EventArgs e)
                    //    //{
                    //    //  string presetJson = arrPreset[2];
                    //    //  JsonHandler.OriginalFormJsonString = presetJson;
                    //    //  SelectedAppointment.SelectedQForm = JsonConvert.DeserializeObject<QForm>(presetJson);
                    //    //  LoadSectionView(SelectedAppointment.SelectedQForm.FormSections[0].SectionId, SelectedAppointment.SelectedQForm.FormSections[0].SectionName, SelectedAppointment.SelectedQForm.FormSections[0], IsDoctorForm);
                    //    //};

                    //    formPresetSection.Add(radioPreset);
                    //}

                    foreach (var Preset in SectionPresets)
                    {
                        var radioPreset = GetPreset(Preset.PresetId, Preset.PresetName, Preset.PresetJson, SelectedAppointment.SelectedQForm.FormSections[0].SectionId, formPresetGroup, formPresetSection, origJson, sectionFormSections, IsDoctorForm);

                        formPresetSection.Add(radioPreset);
                    }

                    var btnNewFormPreset = new GlassButton(new RectangleF(0, 0, (float)View.Frame.Width, 50))
                    {
                        NormalColor = UIColor.FromRGB(224, 238, 240)
                    };
                    btnNewFormPreset.TitleLabel.Font = UIFont.BoldSystemFontOfSize(17);
                    btnNewFormPreset.SetTitleColor(UIColor.Black, UIControlState.Normal);
                    btnNewFormPreset.SetTitle("Save New Form Preset", UIControlState.Normal);
                    btnNewFormPreset.TouchUpInside += (sender, e) =>
                    {
                        //Create Alert
                        var SavePresetPrompt = UIAlertController.Create("New Form Preset", "Enter preset name: ", UIAlertControllerStyle.Alert);
                        SavePresetPrompt.AddTextField((field) =>
                        {
                            field.Placeholder = "Preset Name";
                        });
                        //Add Actions
                        SavePresetPrompt.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, action => SaveFormPreset(null, SavePresetPrompt.TextFields[0].Text, SelectedAppointment.SelectedQForm.FormSections[0].SectionId, formPresetSection, null, formPresetGroup, origJson, sectionFormSections, IsDoctorForm)));
                        SavePresetPrompt.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, null));
                        //Present Alert
                        PresentViewController(SavePresetPrompt, true, null);
                    };

                    formPresetSection.Add(btnNewFormPreset);
                    formPresetsRoot.Add(formPresetSection);
                    formPresetsRoot.Enabled = true;

                    sectionFormSections.Add(formPresetsRoot);
                }

                foreach (FormSection fSection in SelectedAppointment.SelectedQForm.FormSections)
                {
                    //var section = new SectionStringElement(fSection.SectionName, delegate 
                    //{
                    //    LoadSectionView(fSection.SectionId, fSection.SectionName, fSection, IsDoctorForm, sectionFormSections);
                    //    foreach (Element d in sectionFormSections.Elements)
                    //    {
                    //        var t = d.GetType();
                    //        if (t == typeof(SectionStringElement))
                    //        {
                    //            var di = (SectionStringElement)d;
                    //            di.selected = false;
                    //        }
                    //    }
                    //        //var shhh = sectionFormSections.GetContainerTableView();
                    //        sectionFormSections.GetContainerTableView().ReloadData();
                    //});

                    var section = new SectionStringElement(fSection.SectionName);
                    section.Tapped += delegate
                    {
                        _lockObject = section;

                        lock (_lockObject)
                        {
                            if (isRunning)
                            {
                                return;
                            }
                            isRunning = true;
                        }

                        LoadSectionView(fSection.SectionId, fSection.SectionName, fSection, IsDoctorForm, sectionFormSections);
                        foreach (Element d in sectionFormSections.Elements)
                        {
                            var t = d.GetType();
                            if (t == typeof(SectionStringElement))
                            {
                                var di = (SectionStringElement)d;
                                if (di.selected)
                                {
                                    oldsel = di.IndexPath;
                                }
                                di.selected = false;
                            }
                        }

                        sectionFormSections.GetContainerTableView().ReloadData();

                        //isRunning = false;
                    };
                    sectionFormSections.Add(section);
                }

                var finalizeSection = new SectionStringElement("Finalize");
                finalizeSection.Tapped += delegate
                {
                    _lockObject = finalizeSection;

                    lock (_lockObject)
                    {
                        if (isRunning)
                        {
                            return;
                        }
                        isRunning = true;
                    }

                    LoadSectionView("", "Finalize", null, IsDoctorForm, sectionFormSections);

                    foreach (Element d in sectionFormSections.Elements)
                    {
                        var t = d.GetType();
                        if (t == typeof(SectionStringElement))
                        {
                            var di = (SectionStringElement)d;
                            if (di.selected)
                            {
                                oldsel = di.IndexPath;
                            }
                            di.selected = false;
                        }
                    }
                    sectionFormSections.GetContainerTableView().ReloadData();
                };

                sectionFormSections.Add(finalizeSection);

                rootFormSections.Add(sectionFormSections);

                var formDVC = new DynaDialogViewController(rootFormSections, true, false);

                string jsonEnding = IsDoctorForm ? "doctor" : "patient";
                var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                var directoryname = Path.Combine(documents, "DynaRestore");
                var fileidentity = SelectedAppointment.ApptId + "_" + SelectedAppointment.SelectedQForm.FormId + "_" + jsonEnding;

                var backups = new DirectoryInfo(directoryname).GetFiles("*" + fileidentity + "*", SearchOption.AllDirectories).OrderByDescending(x => x.LastWriteTime);

                if (backups.Any())
                {
                    //showrestore = true;
                    //if (!JToken.DeepEquals(sourceJObject, targetJObject))
                    //{
                    //    messageLabel = new UILabel();
                    //    restorebtn = new UIBarButtonItem(UIImage.FromBundle("Restore"), UIBarButtonItemStyle.Bordered, delegate
                    //    {
                    //        //Create Alert
                    //        var RestorePrompt = UIAlertController.Create("Restore Form", "Administrative use only. Please enter password to restore", UIAlertControllerStyle.Alert);
                    //        RestorePrompt.AddTextField((field) =>
                    //        {
                    //            field.SecureTextEntry = true;
                    //            field.Placeholder = "Password";
                    //        });
                    //        RestorePrompt.Add(messageLabel);
                    //        RestorePrompt.AddAction(UIAlertAction.Create("Restore", UIAlertActionStyle.Default, action => RestoreForm(RestorePrompt.TextFields[0].Text, restoreFile, IsDoctorForm, sourceJObject, targetJObject, sectionFormSections)));
                    //        RestorePrompt.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, null));
                    //        //Present Alert
                    //        PresentViewController(RestorePrompt, true, null);
                    //    });
                    //}

                    restorebtn = GetRestoreBtn(IsDoctorForm, sectionFormSections, backups);

                    DetailViewController.NavigationItem.SetRightBarButtonItem(restorebtn, true);
                }

                if (IsDoctorForm)
                {
                    messageLabel = new UILabel();

                    var backdiscard = new UIBarButtonItem(UIBarButtonSystemItem.Cancel, delegate
                    {
                        var BackPrompt_DoctorDiscard = UIAlertController.Create("Discard and Exit", "** WARNING **" + Environment.NewLine + "This will discard any changes, do you wish to continue? " + Environment.NewLine + " ** WARNING **", UIAlertControllerStyle.Alert);
                        BackPrompt_DoctorDiscard.Add(messageLabel);
                        BackPrompt_DoctorDiscard.AddAction(UIAlertAction.Create("Yes", UIAlertActionStyle.Default, action => PopBack(null, IsDoctorForm, false, sectionFormSections)));
                        BackPrompt_DoctorDiscard.AddAction(UIAlertAction.Create("No", UIAlertActionStyle.Cancel, null));

                        //Present Alert
                        PresentViewController(BackPrompt_DoctorDiscard, true, null);
                    });

                    var backsave = new UIBarButtonItem(UIBarButtonSystemItem.Done, delegate
                    {
                        var BackPrompt_DoctorDiscard = UIAlertController.Create("Save and Exit", "Save changes?", UIAlertControllerStyle.Alert);
                        BackPrompt_DoctorDiscard.Add(messageLabel);
                        BackPrompt_DoctorDiscard.AddAction(UIAlertAction.Create("Yes", UIAlertActionStyle.Default, action => PopBack(null, IsDoctorForm, true, sectionFormSections)));
                        BackPrompt_DoctorDiscard.AddAction(UIAlertAction.Create("No", UIAlertActionStyle.Cancel, null));

                        //Present Alert
                        PresentViewController(BackPrompt_DoctorDiscard, true, null);
                    });

                    formDVC.NavigationItem.SetLeftBarButtonItem(backsave, true);
                    formDVC.NavigationItem.SetRightBarButtonItem(backdiscard, true);

                    //if (showrestore)
                    //{
                    //    //formDVC.NavigationItem.SetRightBarButtonItems(new UIBarButtonItem[] { restorebtn, backdiscard }, true);
                    //    DetailViewController.NavigationItem.SetRightBarButtonItem(restorebtn, true);
                    //}
                    //else
                    //{
                    //    formDVC.NavigationItem.SetRightBarButtonItem(backdiscard, true);
                    //}
                    //formDVC.NavigationItem.LeftBarButtonItem = new UIBarButtonItem(UIImage.FromBundle("Back"), UIBarButtonItemStyle.Plain, delegate (object sender, EventArgs e)
                    //{
                    //    var BackPrompt_Doctor = UIAlertController.Create("Exit Form", "Save changes?", UIAlertControllerStyle.Alert);
                    //    BackPrompt_Doctor.Add(messageLabel);
                    //    BackPrompt_Doctor.AddAction(UIAlertAction.Create("Yes", UIAlertActionStyle.Default, action => PopBack(null, IsDoctorForm, true, sectionFormSections)));
                    //    BackPrompt_Doctor.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, null));
                    //    BackPrompt_Doctor.AddAction(UIAlertAction.Create("No", UIAlertActionStyle.Default, action => PopBack(null, IsDoctorForm, false, sectionFormSections)));
                    //    //Present Alert
                    //    PresentViewController(BackPrompt_Doctor, true, null);
                    //});
                }
                else
                {
                    messageLabel = new UILabel();
                    var clientbackbtn = new UIBarButtonItem(UIImage.FromBundle("LockedBack"), UIBarButtonItemStyle.Plain, delegate
                    {
                        //Create Alert
                        var BackPrompt_Patient = UIAlertController.Create("Exit Form", "Administrative use only. Please enter password to continue, save changes?", UIAlertControllerStyle.Alert);
                        BackPrompt_Patient.AddTextField((field) =>
                        {
                            field.SecureTextEntry = true;
                            field.Placeholder = "Password";
                        });

                        BackPrompt_Patient.Add(messageLabel);
                        BackPrompt_Patient.AddAction(UIAlertAction.Create("Yes", UIAlertActionStyle.Default, action => PopBack(BackPrompt_Patient.TextFields[0].Text, IsDoctorForm, true, sectionFormSections)));
                        BackPrompt_Patient.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, null));
                        BackPrompt_Patient.AddAction(UIAlertAction.Create("No", UIAlertActionStyle.Default, action => PopBack(BackPrompt_Patient.TextFields[0].Text, IsDoctorForm, false, sectionFormSections)));

                        //Present Alert
                        PresentViewController(BackPrompt_Patient, true, null);
                    });

                    formDVC.NavigationItem.SetLeftBarButtonItem(clientbackbtn, true);

                    //if (showrestore)
                    //{
                    //    formDVC.NavigationItem.SetRightBarButtonItem(restorebtn, true);
                    //}
                }

                //if (!IsDoctorForm)
                //{
                //  messageLabel = new UILabel();
                //  formDVC.NavigationItem.LeftBarButtonItem = new UIBarButtonItem(UIImage.FromBundle("Lock"), UIBarButtonItemStyle.Bordered, delegate (object sender, EventArgs e)
                //      {
                //        //Create Alert
                //        var BackPrompt = UIAlertController.Create("Exit Form", "Administrative use only. Please enter password to continue", UIAlertControllerStyle.Alert);
                //        BackPrompt.AddTextField((field) =>
                //        {
                //            field.SecureTextEntry = true;
                //            field.Placeholder = "Password";
                //        });
                //        BackPrompt.Add(messageLabel);
                //        BackPrompt.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, action => PopBack(BackPrompt.TextFields[0].Text)));
                //        BackPrompt.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, null));
                //        //Present Alert
                //        PresentViewController(BackPrompt, true, null);
                //      });
                //  //formDVC.NavigationItem.LeftBarButtonItem.Title = "Back";
                //}

                var firstid = 0;
                if (IsDoctorForm) { firstid = 1; }
                var q = (SectionStringElement)sectionFormSections[firstid];
                q.selected = true;

                //if (!IsStartupRestore)
                //{
                LoadSectionView(SelectedAppointment.SelectedQForm.FormSections[0].SectionId, SelectedAppointment.SelectedQForm.FormSections[0].SectionName, SelectedAppointment.SelectedQForm.FormSections[0], IsDoctorForm, sectionFormSections);
                //}

                CommonFunctions.AddLogEvent(DateTime.Now, "GetFormService", false, null, "end");

                CheckStopwatch(timer, 0, "GetFormService", "GetFormService took " + timer.Elapsed.Seconds + " seconds");

                return formDVC;
            }
            catch (Exception ex)
            {
                var eventItems = new List<NSDictionary>
                {
                    getDictionary("Exception Message", ex.Message),
                    getDictionary("Exception Stacktrace", ex.StackTrace)
                };
                CommonFunctions.AddLogEvent(DateTime.Now, "GetFormService", true, eventItems, "catch block");

                CommonFunctions.sendErrorEmail(ex);

                CheckStopwatch(timer, 0, "GetFormService Error", "GetFormService took " + timer.Elapsed.Seconds + " seconds");

                PresentViewController(CommonFunctions.ExceptionAlertPrompt(ex), true, null);
                return new DynaDialogViewController(CommonFunctions.ErrorRootElement(), true);
            }
            //finally
            //{
            //    loadingOverlay.Hide();
            //}
        }

        UIBarButtonItem GetRestoreBtn(bool IsDoctorForm, Section sectionFormSections, IOrderedEnumerable<FileInfo> backups)//, bool IsStartupRestore = false, DynaFormRootElement dfre = null)
        {
            return new UIBarButtonItem(UIImage.FromBundle("Restore"), UIBarButtonItemStyle.Plain, delegate
            {
                var nlab = new UILabel(new CGRect(5, 5, 350, 50)) { Text = "Restore Form" };

                var ncellHeader = new UITableViewCell(UITableViewCellStyle.Default, null) { Frame = new CGRect(0, 0, 350, 50) };

                var nheadclosebtn = new UIButton(new CGRect(300, 5, 50, 50));
                nheadclosebtn.SetImage(UIImage.FromBundle("Close"), UIControlState.Normal);

                ncellHeader.ContentView.Add(nlab);
                ncellHeader.ContentView.Add(nheadclosebtn);

                var nsec = new Section(ncellHeader) { FooterView = new UIView(new CGRect(0, 0, 0, 0)) { Hidden = true } };

                foreach (var fi in backups)
                {
                    var splits = fi.Name.Split('_');

                    var btn = new UIButton(new CGRect(0, 0, 350, 50));
                    btn.SetTitle(splits[2] + " Form " + fi.CreationTime.ToShortTimeString(), UIControlState.Normal);
                    btn.SetTitleColor(UIColor.Blue, UIControlState.Normal);
                    btn.TouchUpInside += delegate
                    {
                        DetailViewController.DismissViewController(true, null);

                        var restoreFile = File.ReadAllText(fi.FullName);
                        var sourceJson = JsonConvert.SerializeObject(SelectedAppointment.SelectedQForm);
                        var sourceJObject = JsonConvert.DeserializeObject<JObject>(sourceJson);
                        var targetJObject = JsonConvert.DeserializeObject<JObject>(restoreFile);

                        LastTappedButton = btn;

                        //Create Alert
                        var RestorePrompt = UIAlertController.Create("Restore Form", "Administrative use only. Please enter password to restore", UIAlertControllerStyle.Alert);
                        RestorePrompt.AddTextField((field) =>
                        {
                            field.SecureTextEntry = true;
                            field.Placeholder = "Password";
                        });

                        //if (IsStartupRestore)
                        //{
                        //    IsDoctorForm = splits[2] == "doctor" ? true : false;
                        //}

                        RestorePrompt.Add(messageLabel);
                        RestorePrompt.AddAction(UIAlertAction.Create("Restore", UIAlertActionStyle.Default, action => RestoreForm(RestorePrompt.TextFields[0].Text, restoreFile, IsDoctorForm, sourceJObject, targetJObject, sectionFormSections)));
                        RestorePrompt.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, null));

                        //Present Alert
                        DetailViewController.PresentViewController(RestorePrompt, true, null);
                    };

                    nsec.Add(btn);
                }

                //DetailViewPopover = new UIPopoverController(content);
                //DetailViewPopover.PopoverContentSize = new CGSize(320, 320);
                //DetailViewPopover.DidDismiss += delegate { LastTappedButton = null; };
                //PresentViewController(content, true, null);
                //DetailViewPopover.PresentFromRect(DetailViewController.NavigationController.NavigationBar.Frame, DetailViewPopover, UIPopoverArrowDirection.Any, true);

                var nroo = new RootElement("Restore") { nsec };

                var ndia = new DialogViewController(nroo)
                {
                    ModalInPopover = true,
                    ModalPresentationStyle = UIModalPresentationStyle.FormSheet,
                    PreferredContentSize = new CGSize(350, 300)
                };
                ndia.TableView.ScrollEnabled = false;

                nheadclosebtn.TouchUpInside += delegate
                {
                    DetailViewController.DismissViewController(true, null);
                };

                DetailViewController.PresentViewController(ndia, true, null);
            });
        }


        void SaveFormPreset(string presetId, string presetName, string sectionId, Section presetSection, PresetRadioElement pre, RadioGroup presetGroup, string origS, Section sectionFormSections, bool IsDoctorForm = true)
        {
            try
            {
                CommonFunctions.AddLogEvent(DateTime.Now, "SaveFormPreset", false, null, "start");

                if (CrossConnectivity.Current.IsConnected)
                {
                    var sectionQuestions = SelectedAppointment.SelectedQForm.FormSections.Find((FormSection obj) => obj.SectionId == sectionId);
                    var fs = SelectedAppointment.SelectedQForm.FormSections.IndexOf(sectionQuestions);

                    var presetJson = JsonConvert.SerializeObject(SelectedAppointment.SelectedQForm);

                    CommonFunctions.AddLogEvent(DateTime.Now, "SaveFormPreset", false, null, "new DynaPadService.DynaPadService");

                    var dds = new DynaPadService.DynaPadService { Timeout = 30000 };
                    dds.SaveAnswerPreset(CommonFunctions.GetUserConfig(), SelectedAppointment.SelectedQForm.FormId, null, SelectedAppointment.ApptDoctorId, true, presetName, presetJson, SelectedAppointment.ApptLocationId, presetId);

                    var eventItems_SaveAnswerPreset = new List<NSDictionary>                     {
                        getDictionary("Form ID", SelectedAppointment.SelectedQForm.FormId),
                        getDictionary("Section ID", "null"),
                        getDictionary("Doctor ID", SelectedAppointment.ApptDoctorId),
                        getDictionary("Is Doctor Form", "true"),
                        getDictionary("Preset Name", presetName),
                        getDictionary("Preset JSON", presetJson),
                        getDictionary("Appointment Location ID", SelectedAppointment.ApptLocationId),
                        getDictionary("Preset ID", presetId)
                    };
                    CommonFunctions.AddLogEvent(DateTime.Now, "SaveFormPreset", false, eventItems_SaveAnswerPreset, "after dds.SaveAnswerPreset");

                    SavePresetData();

                    if (presetId == null)
                    {
                        var mre = GetPreset(presetId, presetName, presetJson, sectionId, presetGroup, presetSection, origS, sectionFormSections, IsDoctorForm);

                        presetSection.Insert(presetSection.Count - 1, UITableViewRowAnimation.Fade, mre);
                        presetSection.GetImmediateRootElement().RadioSelected = presetSection.Count - 2;

                        presetSection.GetImmediateRootElement().Reload(presetSection, UITableViewRowAnimation.Fade);
                    }
                    else
                    {
                        presetSection.GetImmediateRootElement().RadioSelected = presetGroup.Selected;
                        pre.PresetName = presetName;
                        pre.Caption = presetName;
                        presetSection.GetImmediateRootElement().Reload(pre, UITableViewRowAnimation.Fade);
                    }

                    foreach (Element d in sectionFormSections.Elements)
                    {
                        var t = d.GetType();
                        if (t == typeof(SectionStringElement))
                        {
                            var di = (SectionStringElement)d;
                            if (di.selected == true)
                            {
                                di.selected = false;
                            }
                        }
                    }

                    var q = (SectionStringElement)sectionFormSections[1];
                    q.selected = true;

                    CommonFunctions.AddLogEvent(DateTime.Now, "SaveFormPreset", false, null, "end");

                    LoadSectionView(SelectedAppointment.SelectedQForm.FormSections[0].SectionId, SelectedAppointment.SelectedQForm.FormSections[0].SectionName, SelectedAppointment.SelectedQForm.FormSections[0], IsDoctorForm, sectionFormSections);

                    NavigationController.PopViewController(true);
                }
                else
                {
                    PresentViewController(CommonFunctions.InternetAlertPrompt(), true, null);
                }
            }
            catch (Exception ex)
            {
                var eventItems = new List<NSDictionary>
                {
                    getDictionary("Exception Message", ex.Message),
                    getDictionary("Exception Stacktrace", ex.StackTrace)
                };
                CommonFunctions.AddLogEvent(DateTime.Now, "SaveFormPreset", true, eventItems, "catch block");

                NavigationController.PopViewController(true);
                CommonFunctions.sendErrorEmail(ex);
                PresentViewController(CommonFunctions.ExceptionAlertPrompt(ex), true, null);
            }
        }



        public Section secforcancel;
        public NSIndexPath oldsel;

        void LoadSectionView(string sectionId, string sectionName, FormSection OrigSection, bool IsDoctorForm, Section sections = null, bool overrideValidation = false)//, bool IsStartupRestore = false)
        {
            try
            {
                var eventItems_LoadSectionView = new List<NSDictionary>
                {
                    getDictionary("Section ID", sectionId),
                    getDictionary("Section Name", sectionName),
                    getDictionary("Form ID", SelectedAppointment.SelectedQForm.FormId)
                };
                CommonFunctions.AddLogEvent(DateTime.Now, "LoadSectionView", false, eventItems_LoadSectionView, "event fired");

                //if (!IsStartupRestore)
                //{
                BackUp(IsDoctorForm);
                //}

                ReloadData();

                var btnNextSection = GetNextBtn(sections, IsDoctorForm);

                secforcancel = sections;

                if (sectionName != "Report" && sectionName != "Finalize" && sectionName != "Photos")
                {
                    var origSectionJson = JsonConvert.SerializeObject(OrigSection);
                    if (DetailViewController.NavigationController != null) DetailViewController.NavigationController.PopViewController(true);
                    DetailViewController.SetDetailItem(new Section(sectionName), sectionName, sectionId, origSectionJson, IsDoctorForm, btnNextSection);
                }
                else if (sectionName == "Finalize")
                {
                    //FinalizationLogic(OrigSection, sectionName, sectionId, IsDoctorForm, btnNextSection, sections, true);
                    var CanContinue = true;
                    string qlist = "";
                    var qelements = new List<SectionStringElement>();

                    object[,] vsecs = new object[SelectedAppointment.SelectedQForm.FormSections.Count, 2];
                    if (!overrideValidation)
                    {
                        for (int x = 0; x < SelectedAppointment.SelectedQForm.FormSections.Count; x++)
                        {
                            var vsec = SelectedAppointment.SelectedQForm.FormSections[x];
                            vsecs[x, 0] = vsec;
                            //if (ValidateSection(vsec).Count == 0)
                            var invalidquestions = ValidateSection(vsec);
                            //if (ValidateSection(vsec))
                            if (invalidquestions.Count == 0)
                            {
                                vsecs[x, 1] = true;
                                vsec.Revalidating = false;
                            }
                            else
                            {
                                vsecs[x, 1] = false;
                                CanContinue = false;
                                qelements.Add(new SectionStringElement(vsec.SectionName + ":") { vsection = true });

                                foreach (InvalidQuestion q in invalidquestions)
                                {
                                    qelements.Add(new SectionStringElement(" - " + q.QuestionText) { vquestion = true });
                                    qlist = qlist + "\n" + vsec.SectionName + " - " + q.QuestionText;
                                }
                            }
                        }
                    }

                    if (CanContinue)
                    {
                        var origSectionJson = JsonConvert.SerializeObject(OrigSection);
                        if (DetailViewController.NavigationController != null) DetailViewController.NavigationController.PopViewController(true);
                        DetailViewController.SetDetailItem(new Section(sectionName), sectionName, sectionId, origSectionJson, IsDoctorForm, btnNextSection);
                    }
                    else
                    {
                        FormSection qSection = null;
                        int firstinvalid = 0;
                        //nfloat firstinvalidquestiony = 0;

                        for (int i = 0; i < SelectedAppointment.SelectedQForm.FormSections.Count; i++)
                        {
                            var test = vsecs[i, 1];
                            if (Equals(test, false))
                            {
                                qSection = vsecs[i, 0] as FormSection;
                                firstinvalid = i;

                                for (int qi = 0; qi < qSection.SectionQuestions.Count; qi++)
                                {
                                    var firstinvalidquestion = qSection.SectionQuestions[qi];
                                    if (firstinvalidquestion.IsInvalid == true)
                                    {
                                        //firstinvalidquestiony = firstinvalidquestion.ScrollY;
                                        qSection.RevalidatingRow = qi;
                                        break;
                                    }
                                }

                                break;
                            }
                        }

                        if (qSection == null)
                        {
                            return;
                        }

                        //foreach (Element d in sections.Elements)
                        //{
                        //  var t = d.GetType();
                        //  if (t == typeof(SectionStringElement))
                        //  {
                        //      var di = (SectionStringElement)d;
                        //      di.selected = false;
                        //  }
                        //}
                        //if (IsDoctorForm) { firstinvalid = firstinvalid + 1; }
                        //var q = (SectionStringElement)sections.Elements[firstinvalid];
                        //q.selected = true;
                        //      sections.GetContainerTableView().SelectRow(sections.Elements[firstinvalid].IndexPath, true, UITableViewScrollPosition.Top);
                        //sections.GetContainerTableView().ReloadData();

                        var nextSectionQuestions = SelectedAppointment.SelectedQForm.FormSections[SelectedAppointment.SelectedQForm.FormSections.IndexOf(qSection)];
                        nextSectionQuestions.Revalidating = true;

                        ValidationAlert(qelements, nextSectionQuestions, IsDoctorForm, sections, firstinvalid);
                    }
                }
                else
                {
                    var origSectionJson = JsonConvert.SerializeObject(OrigSection);
                    if (DetailViewController.NavigationController != null) DetailViewController.NavigationController.PopViewController(true);
                    DetailViewController.SetDetailItem(new Section(sectionName), sectionName, sectionId, origSectionJson, IsDoctorForm, btnNextSection);
                }
            }
            catch (Exception ex)
            {
                var eventItems = new List<NSDictionary>
                {
                    getDictionary("Exception Message", ex.Message),
                    getDictionary("Exception Stacktrace", ex.StackTrace)
                };
                CommonFunctions.AddLogEvent(DateTime.Now, "LoadSectionView", true, eventItems, "catch block");

                CommonFunctions.sendErrorEmail(ex);
                PresentViewController(CommonFunctions.ExceptionAlertPrompt(ex), true, null);
            }
        }



        public GlassButton GetNextBtn(Section sections, bool IsDoctorForm)
        {
            try
            {
                var btnNextSection = new GlassButton(new RectangleF(0, 0, (float)DetailViewController.View.Frame.Width, 50))
                {
                    NormalColor = UIColor.FromRGB(224, 238, 240)
                };
                btnNextSection.TitleLabel.Font = UIFont.BoldSystemFontOfSize(17);
                btnNextSection.SetTitleColor(UIColor.Black, UIControlState.Normal);
                btnNextSection.SetTitle("Continue", UIControlState.Normal);
                btnNextSection.TouchUpInside += (sender, e) =>
                {
                    var nextSectionIndex = new int();

                    foreach (Element d in sections.Elements)
                    {
                        var t = d.GetType();
                        if (t == typeof(SectionStringElement))
                        {
                            var di = (SectionStringElement)d;
                            if (di.selected == true)
                            {
                                nextSectionIndex = sections.Elements.IndexOf(di) + 1;
                                oldsel = di.IndexPath;
                            }
                            di.selected = false;
                        }
                    }

                    if (nextSectionIndex == -1 || nextSectionIndex >= sections.Elements.Count)
                    {
                        return;
                    }

                    var q = (SectionStringElement)sections.Elements[nextSectionIndex];
                    q.selected = true;

                    sections.GetContainerTableView().SelectRow(sections.Elements[nextSectionIndex].IndexPath, true, UITableViewScrollPosition.Top);
                    sections.GetContainerTableView().ReloadData();

                    if (IsDoctorForm)
                    {
                        nextSectionIndex = nextSectionIndex - 1;
                    }

                    if (q.Caption == "Finalize")
                    {
                        btnNextSection.SetTitle("Finalize", UIControlState.Normal);
                        LoadSectionView("", "Finalize", null, IsDoctorForm, sections);
                    }
                    else
                    {
                        var nextSectionQuestions = SelectedAppointment.SelectedQForm.FormSections[nextSectionIndex];
                        var nextSectionJson = JsonConvert.SerializeObject(nextSectionQuestions);
                        LoadSectionView(nextSectionQuestions.SectionId, nextSectionQuestions.SectionName, nextSectionQuestions, IsDoctorForm, sections);
                    }

                    //BackUp(IsDoctorForm);
                    //string jsonEnding = IsDoctorForm ? "doctor" : "patient";
                    //var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                    //var directoryname = Path.Combine(documents, "DynaRestore");
                    //var filename = Path.Combine(directoryname, SelectedAppointment.ApptId + "_" + SelectedAppointment.SelectedQForm.FormId + "_" + jsonEnding + ".json");
                    //var sourceJson = JsonConvert.SerializeObject(SelectedAppointment.SelectedQForm);
                    //if (File.Exists(filename))
                    //{
                    //  var restoreFile = File.ReadAllText(filename);
                    //  var sourceJObject = JsonConvert.DeserializeObject<JObject>(sourceJson);
                    //  var targetJObject = JsonConvert.DeserializeObject<JObject>(restoreFile);
                    //  if (!JToken.DeepEquals(sourceJObject, targetJObject))
                    //  {
                    //      Directory.CreateDirectory(directoryname);
                    //      File.WriteAllText(filename, sourceJson);
                    //  }
                    //}
                    //else
                    //{
                    //  Directory.CreateDirectory(directoryname);
                    //  File.WriteAllText(filename, sourceJson);
                    //}

                };

                return btnNextSection;
            }
            catch (Exception ex)
            {
                var eventItems = new List<NSDictionary>
                {
                    getDictionary("Exception Message", ex.Message),
                    getDictionary("Exception Stacktrace", ex.StackTrace)
                };
                CommonFunctions.AddLogEvent(DateTime.Now, "GetNextBtn", true, eventItems, "catch block");

                CommonFunctions.sendErrorEmail(ex);
                PresentViewController(CommonFunctions.ExceptionAlertPrompt(ex, true), true, null);
                return null;
            }
        }


        void BackUp(bool IsDoctorForm)
        {
            try
            {
                string jsonEnding = IsDoctorForm ? "doctor" : "patient";
                var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                var directoryname = Path.Combine(documents, "DynaRestore");
                var fileidentity = SelectedAppointment.ApptId + "_" + SelectedAppointment.SelectedQForm.FormId + "_" + jsonEnding;
                var filename = fileidentity + "_" + SelectedAppointment.ApptPatientName + "_" + DateTime.Now.ToFileTimeUtc() + ".json";
                var filefullpath = Path.Combine(directoryname, filename);
                var sourceJson = JsonConvert.SerializeObject(SelectedAppointment.SelectedQForm);

                var backups = new DirectoryInfo(directoryname).GetFiles("*" + fileidentity + "*", SearchOption.AllDirectories).OrderByDescending(x => x.LastWriteTime);

                if (backups.Any())
                {
                    var restoreFile = File.ReadAllText(backups.First().FullName);
                    var sourceJObject = JsonConvert.DeserializeObject<JObject>(sourceJson);
                    var targetJObject = JsonConvert.DeserializeObject<JObject>(restoreFile);

                    if (!JToken.DeepEquals(sourceJObject, targetJObject))
                    {
                        foreach (var fi in backups.Skip(4))
                        {
                            fi.Delete();
                        }

                        File.WriteAllText(filefullpath, sourceJson);
                    }
                }
                else
                {
                    Directory.CreateDirectory(directoryname);
                    File.WriteAllText(filefullpath, sourceJson);
                }
            }
            catch (Exception ex)
            {
                var eventItems = new List<NSDictionary>
                {
                    getDictionary("Exception Message", ex.Message),
                    getDictionary("Exception Stacktrace", ex.StackTrace)
                };
                CommonFunctions.AddLogEvent(DateTime.Now, "BackUp", true, eventItems, "catch block");

                CommonFunctions.sendErrorEmail(ex);
                PresentViewController(CommonFunctions.ExceptionAlertPrompt(ex, true), true, null);
            }
        }



        public PresetRadioElement GetPreset(string presetId, string presetName, string presetJson, string sectionId, RadioGroup presetGroup, Section presetSection, string origS, Section sectionFormSections, bool IsDoctorForm)
        {
            try
            {
                var PatientId = SelectedAppointment.SelectedQForm.PatientId;
                var PatientName = SelectedAppointment.SelectedQForm.PatientName;
                var DoctorId = SelectedAppointment.SelectedQForm.DoctorId;
                var LocationId = SelectedAppointment.SelectedQForm.LocationId;
                var ApptId = SelectedAppointment.SelectedQForm.ApptId;
                var DateCompleted = SelectedAppointment.SelectedQForm.DateCompleted;
                var DateUpdated = SelectedAppointment.SelectedQForm.DateUpdated;

                var mre = new PresetRadioElement(presetName, "FormPresetAnswers")
                {
                    PresetID = presetId,
                    PresetName = presetName,
                    PresetJson = presetJson
                };
                mre.OnSelected += delegate
                {
                    SelectedAppointment.SelectedQForm = JsonConvert.DeserializeObject<QForm>(presetJson);
                    SelectedAppointment.SelectedQForm.PatientId = PatientId;
                    SelectedAppointment.SelectedQForm.PatientName = PatientName;
                    SelectedAppointment.SelectedQForm.DoctorId = DoctorId;
                    SelectedAppointment.SelectedQForm.LocationId = LocationId;
                    SelectedAppointment.SelectedQForm.ApptId = ApptId;
                    SelectedAppointment.SelectedQForm.DateCompleted = DateCompleted;
                    SelectedAppointment.SelectedQForm.DateUpdated = DateUpdated;

                    var selectedSection = SelectedAppointment.SelectedQForm;
                    if (selectedSection != null)
                    {
                        selectedSection.FormSelectedTemplateId = presetGroup.Selected;
                    }

                    foreach (Element d in sectionFormSections.Elements)
                    {
                        var t = d.GetType();
                        if (t == typeof(SectionStringElement))
                        {
                            var di = (SectionStringElement)d;
                            if (di.selected == true)
                            {
                                di.selected = false;
                            }
                        }
                    }

                    var q = (SectionStringElement)sectionFormSections[1];
                    q.selected = true;

                    sectionFormSections.GetContainerTableView().ReloadData();

                    LoadSectionView(SelectedAppointment.SelectedQForm.FormSections[0].SectionId, SelectedAppointment.SelectedQForm.FormSections[0].SectionName, SelectedAppointment.SelectedQForm.FormSections[0], IsDoctorForm, sectionFormSections);
                };
                mre.editPresetBtn.TouchUpInside += (sender, e) =>
                {
                    var UpdatePresetPrompt = UIAlertController.Create("Update Form Preset", "Overwriting preset '" + mre.PresetName + "', do you wish to continue?", UIAlertControllerStyle.Alert);
                    //Add Actions
                    UpdatePresetPrompt.AddTextField((field) =>
                    {
                        field.Placeholder = "Preset Name";
                        field.Text = mre.PresetName;
                    });
                    UpdatePresetPrompt.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, action => SaveFormPreset(mre.PresetID, UpdatePresetPrompt.TextFields[0].Text, sectionId, presetSection, mre, presetGroup, origS, sectionFormSections)));
                    UpdatePresetPrompt.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, null));
                    //Present Alert

                    PresentViewController(UpdatePresetPrompt, true, null);
                };
                mre.deletePresetBtn.TouchUpInside += (sender, e) =>
                {
                    var UpdatePresetPrompt = UIAlertController.Create("Delete Form Preset", "Deleting preset '" + mre.PresetName + "', do you wish to continue?", UIAlertControllerStyle.Alert);
                    //Add Actions
                    UpdatePresetPrompt.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, action => DeleteFormPreset(mre.PresetID, sectionId, presetSection, mre)));
                    UpdatePresetPrompt.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, null));
                    //Present Alert

                    PresentViewController(UpdatePresetPrompt, true, null);
                };

                return mre;
            }
            catch (Exception ex)
            {
                var eventItems = new List<NSDictionary>
                {
                    getDictionary("Exception Message", ex.Message),
                    getDictionary("Exception Stacktrace", ex.StackTrace)
                };
                CommonFunctions.AddLogEvent(DateTime.Now, "GetPreset", true, eventItems, "catch block");

                CommonFunctions.sendErrorEmail(ex);
                PresentViewController(CommonFunctions.ExceptionAlertPrompt(ex, true), true, null);
                return null;
            }
        }



        void DeleteFormPreset(string presetId, string sectionId, Section presetSection, PresetRadioElement pre)
        {
            try
            {
                CommonFunctions.AddLogEvent(DateTime.Now, "DeleteFormPreset", false, null, "start");

                if (CrossConnectivity.Current.IsConnected)
                {
                    var sectionQuestions = SelectedAppointment.SelectedQForm.FormSections.Find((FormSection obj) => obj.SectionId == sectionId);
                    var fs = SelectedAppointment.SelectedQForm.FormSections.IndexOf(sectionQuestions);

                    var presetJson = JsonConvert.SerializeObject(SelectedAppointment.SelectedQForm);

                    CommonFunctions.AddLogEvent(DateTime.Now, "DeleteFormPreset", false, null, "new DynaPadService.DynaPadService");

                    var dds = new DynaPadService.DynaPadService { Timeout = 30000 };
                    dds.DeleteAnswerPreset(CommonFunctions.GetUserConfig(), SelectedAppointment.SelectedQForm.FormId, null, SelectedAppointment.ApptDoctorId, presetId);

                    var eventItems_DeleteAnswerPreset = new List<NSDictionary>                     {
                        getDictionary("Form ID", SelectedAppointment.SelectedQForm.FormId),
                        getDictionary("Section ID", "null"),
                        getDictionary("Doctor ID", SelectedAppointment.ApptDoctorId),
                        getDictionary("Preset ID", presetId)
                    };
                    CommonFunctions.AddLogEvent(DateTime.Now, "DeleteFormPreset", false, eventItems_DeleteAnswerPreset, "after dds.DeleteAnswerPreset");

                    var presetPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "DynaPresets/" + SelectedAppointment.SelectedQForm.FormId + "/" + SelectedAppointment.ApptDoctorId + "/" + presetId + ".txt");
                    if (File.Exists(presetPath))
                    {
                        File.Delete(presetPath);

                        var toast = new Toast("Preset file was deleted");
                        toast.SetDuration(5000);
                        toast.SetType(ToastType.Info);
                        toast.SetGravity(ToastGravity.Bottom);
                        toast.Show();
                    }

                    SavePresetData();

                    if (presetSection.GetImmediateRootElement().RadioSelected == pre.Index)
                    {
                        presetSection.GetImmediateRootElement().RadioSelected = 0;
                    }
                    presetSection.Remove(pre);
                    presetSection.GetImmediateRootElement().Reload(presetSection, UITableViewRowAnimation.Fade);

                    CommonFunctions.AddLogEvent(DateTime.Now, "DeleteFormPreset", false, null, "end");

                    NavigationController.PopViewController(true);
                }
                else
                {
                    PresentViewController(CommonFunctions.InternetAlertPrompt(), true, null);
                }
            }
            catch (Exception ex)
            {
                var eventItems = new List<NSDictionary>
                {
                    getDictionary("Exception Message", ex.Message),
                    getDictionary("Exception Stacktrace", ex.StackTrace)
                };
                CommonFunctions.AddLogEvent(DateTime.Now, "DeleteFormPreset", true, eventItems, "catch block");

                NavigationController.PopViewController(true);
                CommonFunctions.sendErrorEmail(ex);
                PresentViewController(CommonFunctions.ExceptionAlertPrompt(ex), true, null);
            }
        }



        void RestoreForm(string password, string restoreFile, bool IsDoctorForm, JObject sourceJObject, JObject targetJObject, Section sections)//, bool IsStartUpRestore = false, DynaFormRootElement dfre = null)
        {
            try
            {
                //if (IsStartUpRestore)
                //{
                //    JsonHandler.OriginalFormJsonString = restoreFile;
                //    SelectedAppointment.SelectedQForm = JsonConvert.DeserializeObject<QForm>(restoreFile);
                //    DynaClassLibrary.DynaClasses.LoginContainer.User.SelectedLocation = DynaClassLibrary.DynaClasses.LoginContainer.User.Locations.Find(l => l.LocationId == SelectedAppointment.SelectedQForm.LocationId);
                //    SelectedAppointment.ApptPatientId = SelectedAppointment.SelectedQForm.PatientId;
                //    SelectedAppointment.ApptPatientName = SelectedAppointment.SelectedQForm.PatientName;
                //    SelectedAppointment.ApptDoctorId = SelectedAppointment.SelectedQForm.DoctorId;
                //    SelectedAppointment.ApptLocationId = SelectedAppointment.SelectedQForm.LocationId;
                //    SelectedAppointment.ApptId = SelectedAppointment.SelectedQForm.ApptId;
                //    //SelectedAppointment.CaseId = SelectedAppointment.SelectedQForm.FormId;
                //    SelectedAppointment.ApptFormId = SelectedAppointment.SelectedQForm.FormId;
                //    SelectedAppointment.ApptFormName = SelectedAppointment.SelectedQForm.FormName;
                //}

                bool isValid = false;

                if (SelectedAppointment.ApptLocationId == DynaClassLibrary.DynaClasses.LoginContainer.User.SelectedLocation.LocationId)
                {
                    isValid |= password == DynaClassLibrary.DynaClasses.LoginContainer.User.DynaPassword;
                }

                if (isValid)
                {
                    if (!JToken.DeepEquals(sourceJObject, targetJObject))
                    {
                        JsonHandler.OriginalFormJsonString = restoreFile;
                        SelectedAppointment.SelectedQForm = JsonConvert.DeserializeObject<QForm>(restoreFile);
                        LoadSectionView(SelectedAppointment.SelectedQForm.FormSections[0].SectionId, SelectedAppointment.SelectedQForm.FormSections[0].SectionName, SelectedAppointment.SelectedQForm.FormSections[0], IsDoctorForm, sections);
                    }
                    else
                    {
                        PresentViewController(CommonFunctions.AlertPrompt("Unnecessary Restore", "There were no changes made to the form, restore canceled.", true, null, false, null), true, null);
                    }
                }
                else
                {
                    PresentViewController(CommonFunctions.AlertPrompt("Error", "Wrong password", true, null, false, null), true, null);
                }
            }
            catch (Exception ex)
            {
                var eventItems = new List<NSDictionary>
                {
                    getDictionary("Exception Message", ex.Message),
                    getDictionary("Exception Stacktrace", ex.StackTrace)
                };
                CommonFunctions.AddLogEvent(DateTime.Now, "RestoreForm", true, eventItems, "catch block");

                CommonFunctions.sendErrorEmail(ex);
                PresentViewController(CommonFunctions.ExceptionAlertPrompt(ex), true, null);
            }
        }



        async void PopBack(string password, bool IsDoctorForm, bool save, Section sections = null)
        {
            try
            {
                loadingOverlay = new LoadingOverlay(SplitViewController.View.Bounds);
                var loadmes = save ? "Saving form. Please wait patiently..." : "Exiting form...";
                loadingOverlay.SetText(loadmes);
                SplitViewController.Add(loadingOverlay);

                await Task.Delay(10);

                bool isValid = false;

                //for (int i = 0; i < Constants.Logins.GetLength(0); i++)
                //{
                //  if (SelectedAppointment.ApptLocationId == Constants.Logins[i, 2])
                //  {
                //      isValid |= password == Constants.Logins[i, 1];
                //  }
                //}

                if (SelectedAppointment.ApptLocationId == DynaClassLibrary.DynaClasses.LoginContainer.User.SelectedLocation.LocationId)
                {
                    isValid |= (password == DynaClassLibrary.DynaClasses.LoginContainer.User.DynaPassword || IsDoctorForm);
                }

                if (isValid)
                {
                    if (save)
                    {
                        var CanContinue = true;
                        string qlist = "";
                        var qelements = new List<SectionStringElement>();

                        object[,] vsecs = new object[SelectedAppointment.SelectedQForm.FormSections.Count, 2];
                        for (int x = 0; x < SelectedAppointment.SelectedQForm.FormSections.Count; x++)
                        {
                            var vsec = SelectedAppointment.SelectedQForm.FormSections[x];
                            vsecs[x, 0] = vsec;
                            var invalidquestions = ValidateSection(vsec);

                            if (invalidquestions.Count == 0)
                            {
                                vsecs[x, 1] = true;
                                vsec.Revalidating = false;
                            }
                            else
                            {
                                vsecs[x, 1] = false;
                                CanContinue = false;
                                qelements.Add(new SectionStringElement(vsec.SectionName + ":") { vsection = true });

                                foreach (InvalidQuestion q in invalidquestions)
                                {
                                    qelements.Add(new SectionStringElement(" - " + q.QuestionText) { vquestion = true });
                                    qlist = qlist + "\n" + vsec.SectionName + " - " + q.QuestionText;
                                }
                            }
                        }

                        if (CanContinue)
                        {
                            BackSubmitForm(IsDoctorForm, sections);
                        }
                        else
                        {
                            FormSection qSection = null;
                            int firstinvalid = 0;

                            for (int i = 0; i < SelectedAppointment.SelectedQForm.FormSections.Count; i++)
                            {
                                var test = vsecs[i, 1];
                                if (Equals(test, false))
                                {
                                    qSection = vsecs[i, 0] as FormSection;
                                    firstinvalid = i;

                                    for (int qi = 0; qi < qSection.SectionQuestions.Count; qi++)
                                    {
                                        var firstinvalidquestion = qSection.SectionQuestions[qi];
                                        if (firstinvalidquestion.IsInvalid == true)
                                        {
                                            qSection.RevalidatingRow = qi;
                                            break;
                                        }
                                    }

                                    break;
                                }
                            }

                            if (qSection == null)
                            {
                                //loadingOverlay.Hide();

                                return;
                            }

                            var nextSectionQuestions = SelectedAppointment.SelectedQForm.FormSections[SelectedAppointment.SelectedQForm.FormSections.IndexOf(qSection)];
                            nextSectionQuestions.Revalidating = true;
                            ValidationAlert(qelements, nextSectionQuestions, IsDoctorForm, sections, firstinvalid);

                            //loadingOverlay.Hide();

                            return;
                        }
                    }

                    BackUp(IsDoctorForm);

                    //string jsonEnding = IsDoctorForm ? "doctor" : "patient";
                    //var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                    //var directoryname = Path.Combine(documents, "DynaRestore");
                    //var filename = Path.Combine(directoryname, SelectedAppointment.ApptId + "_" + SelectedAppointment.SelectedQForm.FormId + "_" + jsonEnding + ".json");
                    //var sourceJson = JsonConvert.SerializeObject(SelectedAppointment.SelectedQForm);
                    //if (File.Exists(filename))
                    //{
                    //  var restoreFile = File.ReadAllText(filename);
                    //  var sourceJObject = JsonConvert.DeserializeObject<JObject>(sourceJson);
                    //  var targetJObject = JsonConvert.DeserializeObject<JObject>(restoreFile);
                    //  if (!JToken.DeepEquals(sourceJObject, targetJObject))
                    //  {
                    //      // Serialize object
                    //      //string restoreJson = JsonConvert.SerializeObject(SelectedAppointment.SelectedQForm);
                    //      //string jsonEnding = IsDoctorForm ? "doctor" : "patient";
                    //      // Save to file
                    //      //var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                    //      //var directoryname = Path.Combine(documents, "DynaRestore");
                    //      Directory.CreateDirectory(directoryname);
                    //      //var filename = Path.Combine(directoryname, SelectedAppointment.ApptId + "_" + SelectedAppointment.SelectedQForm.FormId + "_" + jsonEnding + ".json");
                    //      File.WriteAllText(filename, sourceJson);
                    //  }
                    //}
                    //else
                    //{
                    //  Directory.CreateDirectory(directoryname);
                    //  File.WriteAllText(filename, sourceJson);
                    //}
                    //DetailViewController.Title = "Welcome to Dynapad";

                    DetailViewController.QuestionsView = null; //.Clear();
                    DetailViewController.Root.Clear();
                    DetailViewController.Root.Caption = SelectedAppointment.ApptFormName + " - " + SelectedAppointment.ApptPatientName;
                    DetailViewController.NavigationItem.LeftBarButtonItems = null;
                    DetailViewController.NavigationItem.RightBarButtonItems = null;
                    DetailViewController.NavigationController.PopToRootViewController(true);

                    DetailViewController.SetSearch();

                    DetailViewController.ReloadData();

                    DetailViewController.SetDetailItem(new Section("Patient Info"), "PatientInfo", null, null, false);

                    //loadingOverlay.Hide();

                    NavigationController.PopViewController(true);
                }
                else
                {
                    //loadingOverlay.Hide();

                    PresentViewController(CommonFunctions.AlertPrompt("Error", "Wrong password", true, null, false, null), true, null);
                }
            }
            catch (Exception ex)
            {
                var eventItems = new List<NSDictionary>
                {
                    getDictionary("Exception Message", ex.Message),
                    getDictionary("Exception Stacktrace", ex.StackTrace)
                };
                CommonFunctions.AddLogEvent(DateTime.Now, "PopBack", true, eventItems, "catch block");

                //loadingOverlay.Hide();

                foreach (Element d in sections.Elements)
                {
                    var t = d.GetType();
                    if (t == typeof(SectionStringElement))
                    {
                        var di = (SectionStringElement)d;
                        di.selected = false;
                    }
                }

                sections.GetContainerTableView().ReloadData();

                CommonFunctions.sendErrorEmail(ex);
                PresentViewController(CommonFunctions.ExceptionAlertPrompt(ex), true, null);
                DetailViewController.Root.Clear();
                DetailViewController.Root.Add(CommonFunctions.ErrorDetailSection());
                DetailViewController.ReloadData();
            }
            finally
            {
                loadingOverlay.Hide();
            }
        }



        void BackSubmitForm(bool IsDoctorForm, Section sections = null)
        {
            try
            {
                var finalJson = JsonConvert.SerializeObject(SelectedAppointment.SelectedQForm);
                //var dds = new DynaPadService.DynaPadService { Timeout = 30000 };
                //dds.SubmitFormAnswers(CommonFunctions.GetUserConfig(), finalJson, true, IsDoctorForm, false);
                //dds.GenerateSummary(CommonFunctions.GetUserConfig(), finalJson);

                var userid = DynaClassLibrary.DynaClasses.LoginContainer.User.UserId;
                var userconfig = CommonFunctions.GetUserConfig();
                var apptid = SelectedAppointment.ApptId;
                var formid = SelectedAppointment.ApptFormId;
                var doctorid = SelectedAppointment.ApptDoctorId;
                var locationid = SelectedAppointment.ApptLocationId;
                var patientid = SelectedAppointment.ApptPatientId;
                var patientname = SelectedAppointment.ApptPatientName;
                var isdoctorform = SelectedAppointment.SelectedQForm.IsDoctorForm;

                var formtype = "patient";
                if (IsDoctorForm)
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
                    IsDoctorForm = IsDoctorForm,
                    ApptId = apptid,
                    FormId = formid,
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
            }
            catch (Exception ex)
            {
                var eventItems = new List<NSDictionary>
                {
                    getDictionary("Exception Message", ex.Message),
                    getDictionary("Exception Stacktrace", ex.StackTrace)
                };
                CommonFunctions.AddLogEvent(DateTime.Now, "BackSubmitForm", true, eventItems, "catch block");

                foreach (Element d in sections.Elements)
                {
                    var t = d.GetType();
                    if (t == typeof(SectionStringElement))
                    {
                        var di = (SectionStringElement)d;
                        di.selected = false;
                    }
                }
                sections.GetContainerTableView().ReloadData();
                CommonFunctions.sendErrorEmail(ex);
                PresentViewController(CommonFunctions.ExceptionAlertPrompt(ex), true, null);
            }
        }



        public UIViewController GetApptDatesService(RootElement rElement)
        {
            try
            {
                CommonFunctions.AddLogEvent(DateTime.Now, "GetApptDatesService", false, null, "start");

                var dfElemet = (DynaFormRootElement)rElement;
                //SelectedAppointment.ApptPatientId = dfElemet.PatientID;
                //SelectedAppointment.ApptPatientName = dfElemet.PatientName;
                //SelectedAppointment.ApptLocationId = dfElemet.LocationID;
                //SelectedAppointment.PatientNotes = dfElemet.PatientNotes;

                CommonFunctions.AddLogEvent(DateTime.Now, "GetApptDatesService", false, null, "new DynaPadService.DynaPadService");

                var dds = new DynaPadService.DynaPadService { Timeout = 30000, AllowAutoRedirect = true };
                var locid = string.IsNullOrEmpty(DynaClassLibrary.DynaClasses.LoginContainer.User.SelectedLocation.LocationId) ? null : DynaClassLibrary.DynaClasses.LoginContainer.User.SelectedLocation.LocationId;

                var menujson = dds.GetAppointmentsByDateFrame(CommonFunctions.GetUserConfig(), locid, dfElemet.MenuAction);

                var eventItems_GetAppointmentsByDateFrame = new List<NSDictionary>                 {
                    getDictionary("Location ID", locid),
                    getDictionary("Menu Action (Date Frame)", dfElemet.MenuAction)
                };
                CommonFunctions.AddLogEvent(DateTime.Now, "GetApptDatesService", false, eventItems_GetAppointmentsByDateFrame, "after dds.GetAppointmentsByDateFrame");

                myDynaMenu = JsonConvert.DeserializeObject<Menu>(menujson);
                DetailViewController.DynaMenu = myDynaMenu;

                switch (dfElemet.MenuAction)
                {
                    case "GetApptsWeek":
                        CurrentApptGridType = "Week";
                        GridSourceWeek = myDynaMenu.MenuItems.FindAll(x => x.GetType().ToString() == "MenuItem");
                        IsGridSourceWeekSet = true;
                        DetailViewController.weekMenu = myDynaMenu;
                        break;
                    case "GetApptsMonth":
                        CurrentApptGridType = "Month";
                        GridSourceMonth = myDynaMenu.MenuItems.FindAll(x => x.GetType().ToString() == "MenuItem");
                        IsGridSourceMonthSet = true;
                        DetailViewController.monthMenu = myDynaMenu;
                        break;
                    case "GetApptsUpcoming":
                        CurrentApptGridType = "Upcoming";
                        GridSourceUpcoming = myDynaMenu.MenuItems.FindAll(x => x.GetType().ToString() == "MenuItem");
                        IsGridSourceUpcomingSet = true;
                        DetailViewController.upcomingMenu = myDynaMenu;
                        break;
                    case "GetApptsYesterday":
                        CurrentApptGridType = "Yesterday";
                        GridSourceYesterday = myDynaMenu.MenuItems.FindAll(x => x.GetType().ToString() == "MenuItem");
                        IsGridSourceYesterdaySet = true;
                        DetailViewController.yesterdayMenu = myDynaMenu;
                        break;
                    case "GetApptsOlder":
                        CurrentApptGridType = "Older";
                        GridSourceOlder = myDynaMenu.MenuItems.FindAll(x => x.GetType().ToString() == "MenuItem");
                        IsGridSourceOlderSet = true;
                        DetailViewController.olderMenu = myDynaMenu;
                        break;
                    default: // do nothing;
                        break;
                }

                //if (dfElemet.MenuAction == "GetApptsWeek")
                //{
                //    CurrentApptGridType = "Week";
                //    //GridSourceWeek = new List<MenuItem>();
                //    GridSourceWeek = myDynaMenu.MenuItems.FindAll(x => x.GetType().ToString() == "MenuItem");
                //}
                //else if (dfElemet.MenuAction == "GetApptsMonth")
                //{
                //    CurrentApptGridType = "Month";
                //    //GridSourceMonth = new List<MenuItem>();
                //    GridSourceMonth = myDynaMenu.MenuItems.FindAll(x => x.GetType().ToString() == "MenuItem");
                //}

                var rootMainMenu = new DynaFormRootElement(myDynaMenu.MenuCaption)
                {
                    UnevenRows = true,
                    Enabled = true
                };

                var sectionMainMenu = new Section { HeaderView = null, FooterView = null };

                BuildMenu(myDynaMenu, sectionMainMenu);

                rootMainMenu.Add(sectionMainMenu);

                var formDVC = new DynaDialogViewController(rootMainMenu, true);
                formDVC.NavigationItem.LeftBarButtonItem = new UIBarButtonItem(UIImage.FromBundle("Back"), UIBarButtonItemStyle.Plain, delegate
                {
                    DetailViewController.QuestionsView = null; //.Clear();
                    DetailViewController.Root.Clear();
                    DetailViewController.Root.Caption = "";
                    DetailViewController.ReloadData();

                    DetailViewController.SetDetailItem(new Section("Appointments Grid"), "ApptGrid", "Today", null, false);

                    NavigationController.PopViewController(true);
                });

                //NavigationController.PushViewController(formDVC, true);

                CommonFunctions.AddLogEvent(DateTime.Now, "GetApptDateService", false, null, "end");

                DetailViewController.SetDetailItem(new Section("Appointments Grid"), "ApptGrid", dfElemet.MenuAction, null, false);

                return formDVC;
            }
            catch (Exception ex)
            {
                var eventItems = new List<NSDictionary>
                {
                    getDictionary("Exception Message", ex.Message),
                    getDictionary("Exception Stacktrace", ex.StackTrace)
                };
                CommonFunctions.AddLogEvent(DateTime.Now, "GetApptDateService", true, eventItems, "catch block");

                CommonFunctions.sendErrorEmail(ex);
                PresentViewController(CommonFunctions.ExceptionAlertPrompt(ex), true, null);
                return new DynaDialogViewController(CommonFunctions.ErrorRootElement(), true);
            }
        }







        public UIViewController GetPatientService(RootElement rElement)
        {
            try
            {
                var dfElemet = (DynaFormRootElement)rElement;
                SelectedAppointment.ApptPatientId = dfElemet.PatientID;
                SelectedAppointment.ApptPatientName = dfElemet.PatientName;
                SelectedAppointment.ApptLocationId = dfElemet.LocationID;
                SelectedAppointment.PatientNotes = dfElemet.PatientNotes;

                DetailViewController.SetDetailItem(new Section("Patient Info"), "PatientInfo", null, null, false);

                var formDVC = new DynaDialogViewController(rElement, true);
                formDVC.NavigationItem.LeftBarButtonItem = new UIBarButtonItem(UIImage.FromBundle("Back"), UIBarButtonItemStyle.Plain, delegate
                {
                    DetailViewController.QuestionsView = null;
                    DetailViewController.Root.Clear();
                    DetailViewController.Root.Caption = "";
                    DetailViewController.ReloadData();

                    NavigationController.PopViewController(true);

                    if (CurrentApptGridType == "Today" || CurrentApptGridType == "Week" || CurrentApptGridType == "Month" || CurrentApptGridType == "Upcoming" || CurrentApptGridType == "Yesterday" || CurrentApptGridType == "Older")
                    {
                        DetailViewController.SetDetailItem(new Section("Appointments Grid"), "ApptGrid", CurrentApptGridType, null, false);
                    }
                });
                return formDVC;
            }
            catch (Exception ex)
            {
                var eventItems = new List<NSDictionary>
                {
                    getDictionary("Exception Message", ex.Message),
                    getDictionary("Exception Stacktrace", ex.StackTrace)
                };
                CommonFunctions.AddLogEvent(DateTime.Now, "GetPatientService", true, eventItems, "catch block");

                CommonFunctions.sendErrorEmail(ex);
                PresentViewController(CommonFunctions.ExceptionAlertPrompt(ex), true, null);
                return new DynaDialogViewController(CommonFunctions.ErrorRootElement(), true);
            }
        }



        public UIViewController GetApptService(RootElement rElement)
        {
            try
            {
                //if (DetailViewController.QuestionsView != null)
                //{
                //	DetailViewController.Title = "";
                //	DetailViewController.QuestionsView = null; //.Clear();
                //}

                var dfElemet = (DynaFormRootElement)rElement;
                SelectedAppointment.ApptPatientId = dfElemet.PatientID;
                SelectedAppointment.ApptPatientName = dfElemet.PatientName;
                SelectedAppointment.ApptDoctorId = dfElemet.DoctorID;
                SelectedAppointment.ApptDoctorName = dfElemet.DoctorName;
                SelectedAppointment.ApptLocationId = dfElemet.LocationID;
                SelectedAppointment.ApptId = dfElemet.ApptID;
                SelectedAppointment.ApptReportId = dfElemet.ReportID;
                SelectedAppointment.CaseId = dfElemet.CaseID;
                SelectedAppointment.ApptNotes = dfElemet.ApptNotes;
                SelectedAppointment.PatientNotes = dfElemet.PatientNotes;

                DetailViewController.SetDetailItem(new Section("Appointment Info"), "ApptInfo", null, null, false);

                var formDVC = new DynaDialogViewController(rElement, true);
                formDVC.NavigationItem.LeftBarButtonItem = new UIBarButtonItem(UIImage.FromBundle("Back"), UIBarButtonItemStyle.Plain, delegate
                {
                    DetailViewController.QuestionsView = null;
                    DetailViewController.Root.Clear();
                    DetailViewController.Root.Caption = "";
                    DetailViewController.ReloadData();

                    NavigationController.PopViewController(true);

                    DetailViewController.SetDetailItem(new Section("Patient Info"), "PatientInfo", null, null, false);
                });
                return formDVC;
            }
            catch (Exception ex)
            {
                var eventItems = new List<NSDictionary>
                {
                    getDictionary("Exception Message", ex.Message),
                    getDictionary("Exception Stacktrace", ex.StackTrace)
                };
                CommonFunctions.AddLogEvent(DateTime.Now, "GetApptService", true, eventItems, "catch block");

                CommonFunctions.sendErrorEmail(ex);
                PresentViewController(CommonFunctions.ExceptionAlertPrompt(ex), true, null);
                return new DynaDialogViewController(CommonFunctions.ErrorRootElement(), true);
            }
        }



        public UIViewController GetApptFormService(RootElement rElement)
        {
            try
            {
                //if (DetailViewController.QuestionsView != null)
                //{
                //	DetailViewController.Title = "";
                //	DetailViewController.QuestionsView = null; //.Clear();
                //}

                var dfElemet = (DynaFormRootElement)rElement;
                SelectedAppointment.ApptFormId = dfElemet.FormID;
                SelectedAppointment.ApptFormName = dfElemet.FormName;

                DetailViewController.Root.Caption = SelectedAppointment.ApptFormName + " - " + SelectedAppointment.ApptPatientName;
                DetailViewController.ReloadData();

                var formDVC = new DynaDialogViewController(rElement, true);
                formDVC.NavigationItem.LeftBarButtonItem = new UIBarButtonItem(UIImage.FromBundle("Back"), UIBarButtonItemStyle.Plain, delegate
                  {
                      DetailViewController.QuestionsView = null;
                      //DetailViewController.NavigationItem.RightBarButtonItem = null;
                      //DetailViewController.SetSearch();
                      DetailViewController.Root.Clear();
                      DetailViewController.Root.Caption = "";
                      DetailViewController.ReloadData();

                      NavigationController.PopViewController(true);

                      DetailViewController.SetDetailItem(new Section("Appointment Info"), "ApptInfo", null, null, false);
                  });

                return formDVC;
            }
            catch (Exception ex)
            {
                var eventItems = new List<NSDictionary>
                {
                    getDictionary("Exception Message", ex.Message),
                    getDictionary("Exception Stacktrace", ex.StackTrace)
                };
                CommonFunctions.AddLogEvent(DateTime.Now, "GetApptFormService", true, eventItems, "catch block");

                CommonFunctions.sendErrorEmail(ex);
                PresentViewController(CommonFunctions.ExceptionAlertPrompt(ex), true, null);
                return new DynaDialogViewController(CommonFunctions.ErrorRootElement(), true);
            }
        }


        //public UIViewController GetReportService(RootElement rElement)
        //{
        //	try
        //	{
        //		//if (DetailViewController.QuestionsView != null)
        //		//{
        //		//	DetailViewController.Title = "";
        //		//	DetailViewController.QuestionsView = null; //.Clear();
        //		//}
        //		if (CrossConnectivity.Current.IsConnected)
        //		{
        //			var dfElemet = (DynaFormRootElement)rElement;
        //			//var DynaReport = SelectedAppointment.ApptDynaReports.Find((DynaReport obj) => obj.FormId == dfElemet.MenuValue);
        //			var dds = new DynaPadService.DynaPadService() { Timeout = 30000};
        //			var origJson = dds.GetDynaReports(CommonFunctions.GetUserConfig(), dfElemet.FormID, dfElemet.DoctorID, false);
        //			JsonHandler.OriginalFormJsonString = origJson;
        //			//var rootReports = new RootElement(dfElemet.FormName);
        //			var reportsGroup = new RadioGroup("reports", -1);
        //			var rootReports = new RootElement("Reports", reportsGroup);
        //			var sectionReports = new Section();
        //			//foreach (Report esf in SelectedAppointment.ApptReports)
        //			//{
        //			//	sectionReports.Add(new SectionStringElement(esf.ReportName, delegate { LoadSectionView(esf.ReportId, "Report", null, false); }));
        //			//}
        //			foreach (Report esf in SelectedAppointment.ApptReports)
        //			{
        //				var report = new SectionStringElement(esf.ReportName, delegate
        //				{
        //					LoadReportView(esf.ReportId, "Report", esf.ReportName);
        //				//LoadSectionView(esf.ReportId, "Report", null, false);
        //				foreach (Element d in sectionReports.Elements)
        //					{
        //						var t = d.GetType();
        //						if (t == typeof(SectionStringElement))
        //						{
        //                                  var di = (SectionStringElement)d;
        //							di.selected = false;
        //						}
        //					}
        //					sectionReports.GetContainerTableView().ReloadData();
        //				});
        //				sectionReports.Add(report);
        //			}
        //			rootReports.Add(sectionReports);
        //			var formDVC = new DynaDialogViewController(rootReports, true);
        //			DetailViewController.Root.Caption = dfElemet.FormName;
        //			DetailViewController.ReloadData();
        //			formDVC.NavigationItem.LeftBarButtonItem = new UIBarButtonItem(UIImage.FromBundle("Back"), UIBarButtonItemStyle.Plain, delegate 
        //			  {
        //			  //DetailViewController.Title = "Welcome to Dynapad";
        //			  DetailViewController.QuestionsView = null; //.Clear();
        //			  DetailViewController.NavigationItem.RightBarButtonItem = null;
        //				  DetailViewController.Root.Clear();
        //				  //DetailViewController.Root.Add(new Section("Logged in")
        //				  //{
        //					 // FooterView = new UIView(new CGRect(0, 0, 0, 0))
        //					 // {
        //						//  Hidden = true
        //					 // }
        //				  //});
        //				  DetailViewController.Root.Caption = SelectedAppointment.ApptFormName + " - " + SelectedAppointment.ApptPatientName;
        //				  DetailViewController.ReloadData();
        //				  NavigationController.PopViewController(true);
        //			  });
        //                  secforcancel = sectionReports;
        //			return formDVC;
        //		}
        //		PresentViewController(CommonFunctions.InternetAlertPrompt(), true, null);
        //		return new DynaDialogViewController(new RootElement("No internet"), true);
        //	}
        //	catch (Exception ex)
        //	{
        //		CommonFunctions.sendErrorEmail(ex);
        //              PresentViewController(CommonFunctions.ExceptionAlertPrompt(ex), true, null);
        //              return new DynaDialogViewController(CommonFunctions.ErrorRootElement(), true);
        //		//throw new Exception(ex.Message + Environment.NewLine + ex.StackTrace, ex.InnerException);
        //	}
        //}



        public UIViewController GetMRFoldersService(RootElement rElement)
        {
            try
            {
                CommonFunctions.AddLogEvent(DateTime.Now, "GetMRFoldersService", false, null, "start");

                //var bounds = base.TableView.Frame;
                // show the loading overlay on the UI thread using the correct orientation sizing
                //loadingOverlay = new LoadingOverlay(bounds);
                //SplitViewController.Add(loadingOverlay);

                if (CrossConnectivity.Current.IsConnected)
                {
                    CommonFunctions.AddLogEvent(DateTime.Now, "GetMRFoldersService", false, null, "new DynaPadService.DynaPadService");

                    var dds = new DynaPadService.DynaPadService { Timeout = 30000 };
                    var dfElemet = (DynaFormRootElement)rElement;

                    var origJson = dds.GetFiles(CommonFunctions.GetUserConfig(), dfElemet.ApptID, dfElemet.PatientID, dfElemet.PatientName, dfElemet.DoctorID, dfElemet.LocationID);

                    var eventItems_GetFiles = new List<NSDictionary>                     {
                        getDictionary("Appointment ID", dfElemet.ApptID),
                        getDictionary("Patient ID", dfElemet.PatientID),
                        getDictionary("Patient Name", dfElemet.PatientName),
                        getDictionary("Doctor ID", dfElemet.DoctorID),
                        getDictionary("Location ID", dfElemet.LocationID)
                    };
                    CommonFunctions.AddLogEvent(DateTime.Now, "GetMRFoldersService", false, eventItems_GetFiles, "after dds.GetFiles");

                    JsonHandler.OriginalFormJsonString = origJson;
                    SelectedAppointment.ApptMRFolders = JsonConvert.DeserializeObject<List<MRFolder>>(origJson);

                    DetailViewController.Root.Clear();
                    DetailViewController.Root.Caption = "Medical Records: " + SelectedAppointment.ApptPatientName;
                    DetailViewController.ReloadData();

                    var mrFolderGroup = new RadioGroup("mrfolders", -1);
                    var rootMRFolders = new RootElement("Medical Records", mrFolderGroup);
                    var mrFolderSections = new Section();

                    foreach (MRFolder mrf in SelectedAppointment.ApptMRFolders)
                    {
                        var mrfolder = new SectionStringElement(mrf.MRFolderName, delegate
                        {
                            try
                            {
                                _lockObject = this;
                                lock (_lockObject)
                                {
                                    if (isRunning)
                                    {
                                        return;
                                    }
                                    isRunning = true;
                                }

                                LoadMRView(mrf.MRFolderName, mrf.MRFolderId);

                                foreach (Element d in mrFolderSections.Elements)
                                {
                                    var t = d.GetType();
                                    if (t == typeof(SectionStringElement))
                                    {
                                        var di = (SectionStringElement)d;
                                        di.selected = false;
                                    }
                                }

                                mrFolderSections.GetContainerTableView().ReloadData();
                            }
                            catch (Exception ex)
                            {
                                var eventItems = new List<NSDictionary>
                                {
                                    getDictionary("Exception Message", ex.Message),
                                    getDictionary("Exception Stacktrace", ex.StackTrace)
                                };
                                CommonFunctions.AddLogEvent(DateTime.Now, "GetMRFoldersService", true, eventItems, "mrfolder delegate catch block");

                                CommonFunctions.sendErrorEmail(ex);
                                PresentViewController(CommonFunctions.ExceptionAlertPrompt(ex), true, null);
                            }
                            finally
                            {
                                isRunning = false;
                            }
                        });

                        mrFolderSections.Add(mrfolder);
                    }

                    mrFolderSections.Add(new SectionStringElement("Download Medical Records", delegate
                    {
                        try
                        {
                            _lockObject = this;
                            lock (_lockObject)
                            {
                                if (isRunning)
                                {
                                    return;
                                }
                                isRunning = true;
                            }

                            DetailViewController.SetDetailItem(new Section("Download Medical Records"), "MRPatientDownload", null, null, false);

                            foreach (Element d in mrFolderSections.Elements)
                            {
                                var t = d.GetType();
                                if (t == typeof(SectionStringElement))
                                {
                                    var di = (SectionStringElement)d;
                                    di.selected = false;
                                }
                            }

                            mrFolderSections.GetContainerTableView().ReloadData();
                        }
                        catch (Exception ex)
                        {
                            var eventItems = new List<NSDictionary>
                            {
                                getDictionary("Exception Message", ex.Message),
                                getDictionary("Exception Stacktrace", ex.StackTrace)
                            };
                            CommonFunctions.AddLogEvent(DateTime.Now, "GetMRFoldersService", true, eventItems, "mrFolderSections delegate catch block");

                            CommonFunctions.sendErrorEmail(ex);
                            PresentViewController(CommonFunctions.ExceptionAlertPrompt(ex), true, null);
                        }
                        finally
                        {
                            isRunning = false;
                        }
                    })
                    { mrdownload = true });

                    rootMRFolders.Add(mrFolderSections);

                    var formDVC = new DynaDialogViewController(rootMRFolders, true);
                    formDVC.NavigationItem.LeftBarButtonItem = new UIBarButtonItem(UIImage.FromBundle("Back"), UIBarButtonItemStyle.Plain, delegate
                    {
                        DetailViewController.QuestionsView = null; //.Clear();
                        DetailViewController.NavigationItem.RightBarButtonItem = null;
                        DetailViewController.Root.Clear();
                        DetailViewController.Root.Caption = "";
                        DetailViewController.ReloadData();

                        NavigationController.PopViewController(true);

                        DetailViewController.SetSearch();

                        DetailViewController.SetDetailItem(new Section("Appointment Info"), "ApptInfo", null, null, false);
                    });

                    //LoadMRView(SelectedAppointment.ApptMRFolders[0].MRFolderName, SelectedAppointment.ApptMRFolders[0].MRFolderId);

                    CommonFunctions.AddLogEvent(DateTime.Now, "GetMRFoldersService", false, null, "end");

                    return formDVC;
                }

                PresentViewController(CommonFunctions.InternetAlertPrompt(), true, null);

                return new DynaDialogViewController(new RootElement("No internet"), true);
            }
            catch (Exception ex)
            {
                var eventItems = new List<NSDictionary>
                {
                    getDictionary("Exception Message", ex.Message),
                    getDictionary("Exception Stacktrace", ex.StackTrace)
                };
                CommonFunctions.AddLogEvent(DateTime.Now, "GetMRFoldersService", true, eventItems, "catch block");

                CommonFunctions.sendErrorEmail(ex);
                PresentViewController(CommonFunctions.ExceptionAlertPrompt(ex), true, null);
                return new DynaDialogViewController(CommonFunctions.ErrorRootElement(), true);
            }

            //finally
            //{
            //    loadingOverlay.Hide();
            //}

        }



        void LoadMRView(string folderName, string folderID)
        {
            try
            {
                if (DetailViewController.NavigationController.ViewControllers.Length > 1)
                {
                    DetailViewController.NavigationController.PopViewController(true);
                }

                NavigationController.TopViewController.NavigationItem.LeftBarButtonItem = new UIBarButtonItem(UIImage.FromBundle("Back"), UIBarButtonItemStyle.Plain, delegate
                {
                    DetailViewController.NavigationItem.RightBarButtonItem = null;
                    DetailViewController.Root.Clear();
                    DetailViewController.Root.Caption = "";
                    DetailViewController.ReloadData();

                    NavigationController.PopViewController(true);

                    DetailViewController.SetDetailItem(new Section("Appointment Info"), "ApptInfo", null, null, false);
                });

                DetailViewController.SetDetailItem(new Section(folderName), "MR", folderID, "", false, null, true, folderName);
            }
            catch (Exception ex)
            {
                var eventItems = new List<NSDictionary>
                {
                    getDictionary("Exception Message", ex.Message),
                    getDictionary("Exception Stacktrace", ex.StackTrace)
                };
                CommonFunctions.AddLogEvent(DateTime.Now, "LoadMRView", true, eventItems, "catch block");

                NavigationController.PopViewController(true);
                CommonFunctions.sendErrorEmail(ex);
                PresentViewController(CommonFunctions.ExceptionAlertPrompt(ex), true, null);
            }
        }



        void LoadReportView(string valueId, string sectionName, string reportName)
        {
            try
            {
                NavigationController.TopViewController.NavigationItem.LeftBarButtonItem = new UIBarButtonItem(UIImage.FromBundle("Back"), UIBarButtonItemStyle.Plain, delegate
                {
                    DetailViewController.NavigationItem.RightBarButtonItem = null;
                    DetailViewController.Root.Clear();
                    DetailViewController.Root.Caption = SelectedAppointment.ApptFormName + " - " + SelectedAppointment.ApptPatientName;
                    DetailViewController.ReloadData();

                    NavigationController.PopViewController(true);

                    DetailViewController.SetDetailItem(new Section("Appointment Info"), "ApptInfo", null, null, false);
                });

                //loadingOverlay = new LoadingOverlay(View.Bounds);
                //SplitViewController.Add(loadingOverlay);
                //await Task.Run(() =>
                //{
                //    DetailViewController.SetDetailItem(new Section(sectionName), "Report", valueId, "", false, null, false, null, reportName);
                //});//.ContinueWith(task =>
                //{
                //    loadingOverlay.Hide();
                //});
                //loadingOverlay.Hide();

                DetailViewController.SetDetailItem(new Section(sectionName), "Report", valueId, "", false, null, false, null, reportName);
            }
            catch (Exception ex)
            {
                var eventItems = new List<NSDictionary>
                {
                    getDictionary("Exception Message", ex.Message),
                    getDictionary("Exception Stacktrace", ex.StackTrace)
                };
                CommonFunctions.AddLogEvent(DateTime.Now, "LoadReportView", true, eventItems, "catch block");

                DetailViewController.Root.Clear();
                DetailViewController.Root.Add(CommonFunctions.ErrorDetailSection());
                DetailViewController.ReloadData();
                CommonFunctions.sendErrorEmail(ex);
                PresentViewController(CommonFunctions.ExceptionAlertPrompt(ex), true, null);
            }

            //finally
            //{
            //    loadingOverlay.Hide();
            //}

        }



        void LoadSummaryView(string fileName, string sectionName)
        {
            try
            {
                NavigationController.TopViewController.NavigationItem.LeftBarButtonItem = new UIBarButtonItem(UIImage.FromBundle("Back"), UIBarButtonItemStyle.Plain, delegate
                {
                    DetailViewController.NavigationItem.RightBarButtonItem = null;
                    DetailViewController.Root.Clear();
                    DetailViewController.Root.Caption = "";
                    DetailViewController.ReloadData();

                    NavigationController.PopViewController(true);

                    DetailViewController.SetDetailItem(new Section("Appointment Info"), "ApptInfo", null, null, false);
                });

                DetailViewController.SetDetailItem(new Section(sectionName), "Summary", fileName, "", false, null, true, fileName);

                //loadingOverlay = new LoadingOverlay(View.Bounds);
                //SplitViewController.Add(loadingOverlay);
                //await Task.Run(() =>
                //{
                //    DetailViewController.SetDetailItem(new Section(sectionName), "Summary", fileName, "", false, null, true, fileName);
                //});//.ContinueWith(task =>
                //{
                //    loadingOverlay.Hide();
                //});
                //loadingOverlay.Hide();

            }
            catch (Exception ex)
            {
                var eventItems = new List<NSDictionary>
                {
                    getDictionary("Exception Message", ex.Message),
                    getDictionary("Exception Stacktrace", ex.StackTrace)
                };
                CommonFunctions.AddLogEvent(DateTime.Now, "LoadSummaryView", true, eventItems, "catch block");

                DetailViewController.Root.Clear();
                DetailViewController.Root.Add(CommonFunctions.ErrorDetailSection());
                DetailViewController.ReloadData();
                CommonFunctions.sendErrorEmail(ex);
                PresentViewController(CommonFunctions.ExceptionAlertPrompt(ex), true, null);
            }

            //finally
            //{
            //    loadingOverlay.Hide();
            //}

        }



        void LoadUploadApptView(string locid, string sectionName)
        {
            try
            {
                NavigationController.TopViewController.NavigationItem.LeftBarButtonItem = new UIBarButtonItem(UIImage.FromBundle("Back"), UIBarButtonItemStyle.Plain, delegate
                {
                    DetailViewController.NavigationItem.RightBarButtonItem = null;
                    DetailViewController.Root.Clear();
                    DetailViewController.Root.Caption = "";
                    DetailViewController.ReloadData();

                    NavigationController.PopViewController(true);

                    DetailViewController.SetDetailItem(new Section("Appointment Info"), "ApptInfo", null, null, false);
                });

                DetailViewController.SetDetailItem(new Section("Upload Submitted Forms"), "UploadSubmittedForms", locid, null, false);

                //loadingOverlay = new LoadingOverlay(View.Bounds);
                //SplitViewController.Add(loadingOverlay);
                //await Task.Run(() =>
                //{
                //    DetailViewController.SetDetailItem(new Section(sectionName), "Summary", fileName, "", false, null, true, fileName);
                //});//.ContinueWith(task =>
                //{
                //    loadingOverlay.Hide();
                //});
                //loadingOverlay.Hide();

            }
            catch (Exception ex)
            {
                var eventItems = new List<NSDictionary>
                {
                    getDictionary("Exception Message", ex.Message),
                    getDictionary("Exception Stacktrace", ex.StackTrace)
                };
                CommonFunctions.AddLogEvent(DateTime.Now, "LoadUploadApptView", true, eventItems, "catch block");

                DetailViewController.Root.Clear();
                DetailViewController.Root.Add(CommonFunctions.ErrorDetailSection());
                DetailViewController.ReloadData();
                CommonFunctions.sendErrorEmail(ex);
                PresentViewController(CommonFunctions.ExceptionAlertPrompt(ex), true, null);
            }

            //finally
            //{
            //    loadingOverlay.Hide();
            //}

        }


        public class InvalidQuestion
        {
            public string QuestionID { get; set; }
            public string QuestionText { get; set; }
        }

        public List<InvalidQuestion> ValidateSection(FormSection OrigSection)
        {
            try
            {
                var valid = true;
                var invalidQuestions = new List<InvalidQuestion>();

                foreach (SectionQuestion question in OrigSection.SectionQuestions)
                {
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

                        if (question.IsInvalid)
                        {
                            invalidQuestions.Add(new InvalidQuestion { QuestionID = question.QuestionId, QuestionText = question.QuestionText });
                        }
                    }
                }

                return invalidQuestions;
                //return valid;
            }
            catch (Exception ex)
            {
                var eventItems = new List<NSDictionary>
                {
                    getDictionary("Exception Message", ex.Message),
                    getDictionary("Exception Stacktrace", ex.StackTrace)
                };
                CommonFunctions.AddLogEvent(DateTime.Now, "ValidateSection", true, eventItems, "catch block");

                CommonFunctions.sendErrorEmail(ex);
                PresentViewController(CommonFunctions.ExceptionAlertPrompt(ex), true, null);
                return null;
            }
        }



        void ValidationAlert(List<SectionStringElement> qlist, FormSection nextSectionQuestions, bool isDoctorForm, Section sections, int firstinvalid)
        {
            try
            {
                var nlab = new UILabel(new CGRect(10, 0, UIScreen.MainScreen.Bounds.Width - 250, 50))
                {
                    Text = "Validation Error",
                    TextAlignment = UITextAlignment.Center
                };

                var ncellHeader = new UITableViewCell(UITableViewCellStyle.Default, null) { Frame = new CGRect(0, 0, UIScreen.MainScreen.Bounds.Width - 250, 50) };
                ncellHeader.ContentView.Add(nlab);

                var nsec = new Section(ncellHeader) { FooterView = new UIView(new CGRect(0, 0, 0, 0)) { Hidden = true } };

                var SettingsView = new DynaMultiRootElement();

                var sSection = new DynaSection("ValidationError")
                {
                    HeaderView = new UIView(new CGRect(0, 0, UIScreen.MainScreen.Bounds.Width - 250, 30)),
                    FooterView = new UIView(new CGRect(0, 0, 0, 0))
                };
                sSection.FooterView.Hidden = true;

                var sPaddedView = new PaddedUIView<UILabel>
                {
                    Frame = new CGRect(0, 0, UIScreen.MainScreen.Bounds.Width - 250, 30),
                    Padding = 5f
                };
                sPaddedView.NestedView.Text = "Please provide the required information (marked with a red asterisk):";
                sPaddedView.Type = "Validation";
                sPaddedView.setStyle();

                sSection.HeaderView.Add(sPaddedView);

                sSection.AddAll(qlist);

                var btnOverride = new UIButton(new CGRect(0, 0, UIScreen.MainScreen.Bounds.Size.Height, 40));
                btnOverride.SetTitle("Override", UIControlState.Normal);
                btnOverride.SetTitleColor(UIColor.Black, UIControlState.Normal);
                btnOverride.BackgroundColor = UIColor.FromRGB(255, 172, 172);
                btnOverride.HorizontalAlignment = UIControlContentHorizontalAlignment.Center;
                btnOverride.TouchUpInside += delegate
                {
                    NavigationController.DismissViewController(true, null);

                    UIAlertController OverridePrompt;
                    string msg;

                    msg = "AUTHORIZED PERSONNEL ONLY! Override required field(s)?";
                    OverridePrompt = UIAlertController.Create("Required Fields", msg, UIAlertControllerStyle.Alert);
                    OverridePrompt.AddTextField((field) =>
                    {
                        field.SecureTextEntry = true;
                        field.Placeholder = "Password";
                    });
                    OverridePrompt.AddAction(UIAlertAction.Create("Yes", UIAlertActionStyle.Default, action =>
                    {
                        try
                        {
                            bool isValid = false;
                            isValid |= OverridePrompt.TextFields[0].Text == DynaClassLibrary.DynaClasses.LoginContainer.User.DynaPassword;

                            if (isValid)
                            {
                                LoadSectionView("", "Finalize", null, isDoctorForm, sections, true);
                                //NavigationController.DismissViewController(true, null);
                            }
                            else
                            {
                                var failPass = "Wrong password.";
                                PresentViewController(CommonFunctions.AlertPrompt("Error", failPass, true, null, false, null), true, null);
                                ReValidate(nextSectionQuestions, isDoctorForm, sections, firstinvalid);
                            }
                        }
                        catch (Exception ex)
                        {
                            var eventItems = new List<NSDictionary>
                            {
                                getDictionary("Exception Message", ex.Message),
                                getDictionary("Exception Stacktrace", ex.StackTrace)
                            };
                            CommonFunctions.AddLogEvent(DateTime.Now, "ValidationAlert", true, eventItems, "OverridePrompt action catch block");

                            CommonFunctions.sendErrorEmail(ex);
                            PresentViewController(CommonFunctions.ExceptionAlertPrompt(ex), true, null);
                        }
                        finally
                        {
                            isRunning = false;
                            loadingOverlay.Hide();
                        }
                    }));
                    OverridePrompt.AddAction(UIAlertAction.Create("No", UIAlertActionStyle.Cancel, action =>
                    {
                        ReValidate(nextSectionQuestions, isDoctorForm, sections, firstinvalid);
                    }));

                    //Present Alert
                    PresentViewController(OverridePrompt, true, null);
                };

                sSection.Add(btnOverride);

                var btnDismiss = new UIButton(new CGRect(0, 0, UIScreen.MainScreen.Bounds.Size.Height, 40));
                btnDismiss.SetTitle("Return", UIControlState.Normal);
                btnDismiss.SetTitleColor(UIColor.Black, UIControlState.Normal);
                btnDismiss.BackgroundColor = UIColor.FromRGB(224, 238, 240);
                btnDismiss.HorizontalAlignment = UIControlContentHorizontalAlignment.Center;
                btnDismiss.TouchUpInside += delegate
                {
                    ReValidate(nextSectionQuestions, isDoctorForm, sections, firstinvalid);
                    NavigationController.DismissViewController(true, null);
                };

                sSection.Add(btnDismiss);

                SettingsView.Add(nsec);
                SettingsView.Add(sSection);

                var nroo = new RootElement("Validations") { nsec };

                var ndia = new DialogViewController(SettingsView)
                {
                    ModalInPopover = true,
                    ModalPresentationStyle = UIModalPresentationStyle.PageSheet,
                    PreferredContentSize = new CGSize(View.Bounds.Size)
                };

                NavigationController.PreferredContentSize = new CGSize(View.Bounds.Size);
                NavigationController.PresentViewController(ndia, true, null);
            }
            catch (Exception ex)
            {
                var eventItems = new List<NSDictionary>
                {
                    getDictionary("Exception Message", ex.Message),
                    getDictionary("Exception Stacktrace", ex.StackTrace)
                };
                CommonFunctions.AddLogEvent(DateTime.Now, "ValidationAlert", true, eventItems, "catch block");

                CommonFunctions.sendErrorEmail(ex);
                PresentViewController(CommonFunctions.ExceptionAlertPrompt(ex, true), true, null);
            }
        }


        void ReValidate(FormSection nextSectionQuestions, bool IsDoctorForm, Section sections, int firstinvalid)
        {
            try
            {
                foreach (Element d in sections.Elements)
                {
                    var t = d.GetType();
                    if (t == typeof(SectionStringElement))
                    {
                        var di = (SectionStringElement)d;
                        di.selected = false;
                    }
                }

                if (IsDoctorForm) { firstinvalid = firstinvalid + 1; }

                var q = (SectionStringElement)sections.Elements[firstinvalid];
                q.selected = true;

                sections.GetContainerTableView().SelectRow(sections.Elements[firstinvalid].IndexPath, true, UITableViewScrollPosition.Top);
                sections.GetContainerTableView().ReloadData();

                LoadSectionView(nextSectionQuestions.SectionId, nextSectionQuestions.SectionName, nextSectionQuestions, IsDoctorForm, sections);
            }
            catch (Exception ex)
            {
                var eventItems = new List<NSDictionary>
                {
                    getDictionary("Exception Message", ex.Message),
                    getDictionary("Exception Stacktrace", ex.StackTrace)
                };
                CommonFunctions.AddLogEvent(DateTime.Now, "ReValidate", true, eventItems, "catch block");

                CommonFunctions.sendErrorEmail(ex);
                PresentViewController(CommonFunctions.ExceptionAlertPrompt(ex), true, null);
            }
        }



        //public void FinalizationLogic(FormSection OrigSection, string sectionName, string sectionId, bool IsDoctorForm, GlassButton btnNextSection, Section sections, bool IsFinalize)
        //{
        //          //try
        //          //{
        //              var CanContinue = true;
        //              string qlist = "";
        //              List<SectionStringElement> qelements = new List<SectionStringElement>();
        //              object[,] vsecs = new object[SelectedAppointment.SelectedQForm.FormSections.Count, 2];
        //              for (int x = 0; x < SelectedAppointment.SelectedQForm.FormSections.Count; x++)
        //              {
        //                  var vsec = SelectedAppointment.SelectedQForm.FormSections[x];
        //                  vsecs[x, 0] = vsec;
        //                  var invalidquestions = ValidateSection(vsec);
        //                  if (invalidquestions.Count == 0)
        //                  {
        //                      vsecs[x, 1] = true;
        //                      vsec.Revalidating = false;
        //                  }
        //                  else
        //                  {
        //                      vsecs[x, 1] = false;
        //                      CanContinue = false;
        //                      qelements.Add(new SectionStringElement(vsec.SectionName + ":") { vsection = true });
        //                      foreach (InvalidQuestion q in invalidquestions)
        //                      {
        //                          qelements.Add(new SectionStringElement(" - " + q.QuestionText) { vquestion = true });
        //                          qlist = qlist + "\n" + vsec.SectionName + " - " + q.QuestionText;
        //                      }
        //                  }
        //              }
        //              if (CanContinue)
        //              {
        //                  if (IsFinalize)
        //                  {
        //                      var origSectionJson = JsonConvert.SerializeObject(OrigSection);
        //                      if (DetailViewController.NavigationController != null) DetailViewController.NavigationController.PopViewController(true);
        //                      DetailViewController.SetDetailItem(new Section(sectionName), sectionName, sectionId, origSectionJson, IsDoctorForm, btnNextSection);
        //                  }
        //                  else
        //                  {
        //                      BackSubmitForm(IsDoctorForm, sections);
        //                      string jsonEnding = IsDoctorForm ? "doctor" : "patient";
        //                      var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        //                      var directoryname = Path.Combine(documents, "DynaRestore");
        //                      var filename = Path.Combine(directoryname, SelectedAppointment.ApptId + "_" + SelectedAppointment.SelectedQForm.FormId + "_" + jsonEnding + ".json");
        //                      var sourceJson = JsonConvert.SerializeObject(SelectedAppointment.SelectedQForm);
        //                      if (File.Exists(filename))
        //                      {
        //                          var restoreFile = File.ReadAllText(filename);
        //                          var sourceJObject = JsonConvert.DeserializeObject<JObject>(sourceJson);
        //                          var targetJObject = JsonConvert.DeserializeObject<JObject>(restoreFile)
        //                          if (!JToken.DeepEquals(sourceJObject, targetJObject))
        //                          {
        //                              Directory.CreateDirectory(directoryname);
        //                              File.WriteAllText(filename, sourceJson);
        //                          }
        //                      }
        //                      else
        //                      {
        //                          Directory.CreateDirectory(directoryname);
        //                          File.WriteAllText(filename, sourceJson);
        //                      }
        //                      DetailViewController.QuestionsView = null; //.Clear();
        //  					DetailViewController.Root.Clear();
        //                      //DetailViewController.Root.Add(new Section("Logged in")
        //                      //{
        //                      //    FooterView = new UIView(new CGRect(0, 0, 0, 0))
        //                      //    {
        //                      //        Hidden = true
        //                      //    }
        //                      //});
        //                      DetailViewController.Root.Caption = "";
        //                      DetailViewController.NavigationItem.LeftBarButtonItems = null;
        //                      DetailViewController.NavigationItem.RightBarButtonItems = null;
        //                      DetailViewController.ReloadData();
        //                      NavigationController.PopViewController(true);
        //                  }
        //              }
        //              else
        //              {
        //                  FormSection qSection = null;
        //                  int firstinvalid = 0;
        //                  for (int i = 0; i < SelectedAppointment.SelectedQForm.FormSections.Count; i++)
        //                  {
        //                      var test = vsecs[i, 1];
        //                      if (Equals(test, false))
        //                      {
        //                          qSection = vsecs[i, 0] as FormSection;
        //                          firstinvalid = i;
        //                          for (int qi = 0; qi < qSection.SectionQuestions.Count; qi++)
        //                          {
        //                              var firstinvalidquestion = qSection.SectionQuestions[qi];
        //                              if (firstinvalidquestion.IsInvalid == true)
        //                              {
        //                                  qSection.RevalidatingRow = qi;
        //                                  break;
        //                              }
        //                          }
        //                          break;
        //                      }
        //                  }
        //                  if (qSection == null)
        //                  {
        //                      return;
        //                  }
        //                  var nextSectionQuestions = SelectedAppointment.SelectedQForm.FormSections[SelectedAppointment.SelectedQForm.FormSections.IndexOf(qSection)];
        //                  nextSectionQuestions.Revalidating = true;
        //                  ValidationAlert(qelements, nextSectionQuestions, IsDoctorForm, sections, firstinvalid);
        //              }
        //	//}
        //	//catch (Exception ex)
        //	//{
        //	//	CommonFunctions.sendErrorEmail(ex);
        //              PresentViewController(CommonFunctions.ExceptionAlertPrompt(ex), true, null);
        //	//	//throw new Exception(ex.Message + Environment.NewLine + ex.StackTrace, ex.InnerException);
        //	//}
        //}



        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, Resources, etc that aren't in use.
        }


        void AddNewItem(object sender, EventArgs args)
        {
            //dataSource.Objects.Insert(0, DateTime.Now);
            using (var indexPath = NSIndexPath.FromRowSection(0, 0))
                TableView.InsertRows(new[] { indexPath }, UITableViewRowAnimation.Automatic);
        }


        public override void PrepareForSegue(UIStoryboardSegue segue, NSObject sender)
        {
            if (segue.Identifier == "showDetail")
            {
                var indexPath = TableView.IndexPathForSelectedRow;
            }
        }


        class DataSource : UITableViewSource
        {
            static readonly NSString CellIdentifier = new NSString("Cell");
            //readonly List<Section> sectionItems = new List<Section>();
            readonly List<RootElement> MenuRoot = new List<RootElement>();
            readonly MasterViewController controller;
            //public DataSource(List<Section> sections, MasterViewController controller)
            public DataSource(RootElement menuRoot, MasterViewController controller)
            {
                MenuRoot.Add(menuRoot);
                this.controller = controller;
            }

            //public IList<Section> Objects
            //{
            //	get { return sectionItems; }
            //}
            public IList<RootElement> Objects
            {
                get { return MenuRoot; }
            }

            // Customize the number of sections in the table view.
            public override nint NumberOfSections(UITableView tableView)
            {
                return 1;
            }

            public override nint RowsInSection(UITableView tableview, nint section)
            {
                return MenuRoot.Count;
            }

            // Customize the appearance of table view cells.
            public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
            {
                var cell = tableView.DequeueReusableCell(CellIdentifier, indexPath);
                cell.TextLabel.Text = MenuRoot[indexPath.Row].Caption;

                return cell;
            }

            public override bool CanEditRow(UITableView tableView, NSIndexPath indexPath)
            {
                // Return false if you do not want the specified item to be editable.
                return true;
            }

            public override void CommitEditingStyle(UITableView tableView, UITableViewCellEditingStyle editingStyle, NSIndexPath indexPath)
            {
                if (editingStyle == UITableViewCellEditingStyle.Delete)
                {
                    // Delete the row from the data source.
                    MenuRoot.RemoveAt(indexPath.Row);
                    controller.TableView.DeleteRows(new[] { indexPath }, UITableViewRowAnimation.Fade);
                }
                else if (editingStyle == UITableViewCellEditingStyle.Insert)
                {
                    // Create a new instance of the appropriate class, insert it into the array, and add a new row to the table view.
                }
            }

            public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
            {
                //if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad)
                //controller.DetailViewController.SetDetailItem(sectionItems[indexPath.Row]);
            }
        }
    }
}
