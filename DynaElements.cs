using System;
using System.Collections.Generic;
using System.Drawing;
using CoreGraphics;
using Foundation;
using MonoTouch.Dialog;
using UIKit;
using System.Diagnostics;
using LoginScreen;
using System.IO;
using System.Threading;
//using System.IO;
//using static DynaClassLibrary.DynaClasses;
//using Syncfusion.SfImageEditor.iOS;
//using System.Data;

#if __UNIFIED__

using NSAction = System.Action;
using Syncfusion.SfAutoComplete.iOS;
using System.ComponentModel;
using System.Linq;
using SlackHQ;
using Newtonsoft.Json;
using Plugin.Connectivity;
using System.Threading.Tasks;
using Syncfusion.SfDataGrid;
using System.Collections.ObjectModel;
//using DynaClassLibrary;
using System.Net.Http;
using System.Text;
#else
using MonoTouch.UIKit;
using MonoTouch.CoreGraphics;
using MonoTouch.Foundation;
using MonoTouch.CoreAnimation;
#endif

#if !__UNIFIED__
using nint = global::System.Int32;
using nuint = global::System.UInt32;
using nfloat = global::System.Single;

using CGSize = global::System.Drawing.SizeF;
using CGPoint = global::System.Drawing.PointF;
using CGRect = global::System.Drawing.RectangleF;
#endif


namespace DynaPad
{



    public class CanvasContainerView : UIView
    {
        UIView canvasView;

        UIView documentView;
        public UIView DocumentView
        {
            get
            {
                return documentView;
            }
            set
            {
                var previousView = documentView;
                if (previousView != null)
                    previousView.RemoveFromSuperview();

                documentView = value;
                if (documentView != null)
                {
                    documentView.Frame = canvasView.Bounds;
                    canvasView.AddSubview(documentView);
                }
            }
        }

        CanvasContainerView(CGRect frame, UIView canvasView) : base(frame)
        {
            this.canvasView = canvasView;

            BackgroundColor = UIColor.LightGray;

            AddSubview(canvasView);
            SetNeedsDisplay();
        }

        public static CanvasContainerView FromCanvasSize(CGSize canvasSize)
        {
            var screenBounds = UIScreen.MainScreen.Bounds;
            var minDimension = NMath.Min(screenBounds.Width, screenBounds.Height);
            var size = canvasSize;
            size.Width = NMath.Max(minDimension, size.Width);
            size.Height = NMath.Max(minDimension, size.Height);

            var frame = new CGRect(CGPoint.Empty, size);

            var canvasOrigin = new CGPoint((frame.Width - canvasSize.Width) / 2, (frame.Height - canvasSize.Height) / 2);
            var canvasFrame = new CGRect(canvasOrigin, canvasSize);

            var canvasView = new UIView(canvasFrame);
            canvasView.Layer.ShadowOffset = new CGSize(0, 3);
            canvasView.Layer.ShadowRadius = 4;
            canvasView.Layer.ShadowColor = UIColor.DarkGray.CGColor;
            canvasView.Layer.ShadowOpacity = 1f;

            return new CanvasContainerView(frame, canvasView);
        }
	}


    public class CredentialsProvider : ICredentialsProvider
    {
        public string locid;
        //int failCount = 0;

        // Constructor without parameters is required

        public bool NeedLoginAfterRegistration
        {
            get
            {
                // If you want your user to login after he/she has been registered
                return true;

                // Otherwise you can:
                // return false;
            }
		}


        public static NSDictionary getDictionary(string text, string value)
        {
            object[] objects = new object[2];
            object[] keys = new object[2];
            keys.SetValue("Text", 0);
            keys.SetValue("Value", 1);
            objects.SetValue((NSString)text, 0);
            objects.SetValue((NSString)value, 1);

            return NSDictionary.FromObjectsAndKeys(objects, keys);
        }


        public void Login(string userName, string password, Action successCallback, Action<LoginScreenFaultDetails> failCallback)
        {
            try
            {
                // Do some operations to login user
                //bool isValid = (userName == Constants.Username && password == Constants.Password);
                bool isValid = false;
                bool userFault = false;
                bool passFault = false;
                bool generalFault = false;

                //for (int i = 0; i < Constants.Logins.GetLength(0); i++)
                //{
                //	if (userName == Constants.Logins[i, 0])
                //	{
                //		userFault = false;
                //		if (password == Constants.Logins[i, 1])
                //		{
                //			isValid = true;
                //			locid = Constants.Logins[i, 2];
                //			Constants.DocLocID = Constants.Logins[i, 2];
                //			userFault = false;
                //			passFault = false;
                //			generalFault = false;
                //		}
                //		else
                //		{
                //			passFault = true;
                //		}
                //	}
                //	else
                //	{
                //		userFault = true;
                //	}
                //}

                if (CrossConnectivity.Current.IsConnected)
                {
                    var domain = NSUserDefaults.StandardUserDefaults.StringForKey("Domain_Name");
                    var device = NSUserDefaults.StandardUserDefaults.StringForKey("Dyna_Device_Name");

                    var loginRequest = new DynaClassLibrary.LoginRequest()
                    {
                        domain = domain,
                        deviceName = device,
                        username = userName,
                        password = password
                    };

                    var requestJson = JsonConvert.SerializeObject(loginRequest);

                    var requestStringContent = new StringContent(requestJson, UnicodeEncoding.UTF8, "application/json");

                    var response = DynaClientClass.DynaClient.PostAsync("Login", requestStringContent);

                    if (response.Result.IsSuccessStatusCode)
                    {
                        var jsonUser = response.Result.Content.ReadAsStringAsync();

                        JsonHandler.OriginalFormJsonString = jsonUser.Result;
                        DynaClassLibrary.DynaClasses.LoginContainer.User = new DynaClassLibrary.DynaClasses.User();
                        DynaClassLibrary.DynaClasses.LoginContainer.User = JsonConvert.DeserializeObject<DynaClassLibrary.DynaClasses.User>(jsonUser.Result);


                        //               var dds = new DynaPadService.DynaPadService { Timeout = 20000 };
                        //var jsonUser = dds.Login(NSUserDefaults.StandardUserDefaults.StringForKey("Domain_Name"), NSUserDefaults.StandardUserDefaults.StringForKey("Dyna_Device_Name"), userName, password);
                        //JsonHandler.OriginalFormJsonString = jsonUser;
                        //DynaClassLibrary.DynaClasses.LoginContainer.User = new DynaClassLibrary.DynaClasses.User();
                        //DynaClassLibrary.DynaClasses.LoginContainer.User = JsonConvert.DeserializeObject<DynaClassLibrary.DynaClasses.User>(jsonUser);

                        switch (DynaClassLibrary.DynaClasses.LoginContainer.User.LoginStatus)
                        {
                            case "Valid":
                                isValid = true;
                                userFault = false;
                                passFault = false;
                                generalFault = false;
                                break;
                            case "userFault":
                                isValid = false;
                                userFault = true;
                                passFault = false;
                                generalFault = false;
                                break;
                            case "passFault":
                                isValid = false;
                                userFault = false;
                                passFault = true;
                                generalFault = false;
                                break;
                            case "generalFault":
                                isValid = false;
                                userFault = false;
                                passFault = false;
                                generalFault = true;
                                break;
                            default: // do nothing;
                                break;
                        }

                        if (isValid)
                        {
                            var logFileName = "Log_" + DynaClassLibrary.DynaClasses.LoginContainer.User.DynaConfig.DeviceId + "_" + DateTime.Now.ToString("yyyyMMddhhmmss") + ".txt";
                            DynaClassLibrary.DynaClasses.LoginContainer.User.LogFileName = logFileName;

                            var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                            var logDirectoryPath = Path.Combine(documents, "DynaLog");
                            var logFilePath = Path.Combine(logDirectoryPath, logFileName);

                            if (!Directory.Exists(logDirectoryPath))
                            {
                                Directory.CreateDirectory(logDirectoryPath);
                            }

                            if (!File.Exists(logFilePath))
                            {
                                //File.Create(logFilePath);
                                var fs = new FileStream(logFilePath, FileMode.Create);
                                fs.Dispose();
                                //string text = File.ReadAllText(myfile);
                            }

                            var dynaConfig = CommonFunctions.GetUserConfig();

                            var eventItems_Login = new List<NSDictionary>                         {
                            getDictionary("Email Support", dynaConfig.EmailSupport),
                            getDictionary("Email Postmaster", dynaConfig.EmailPostmaster),
                            getDictionary("Email Roy", dynaConfig.EmailRoy),
                            getDictionary("Email SMTP", dynaConfig.EmailSmtp),
                            getDictionary("Email User", dynaConfig.EmailUser),
                            getDictionary("Email Password", "*"),
                            getDictionary("Email Port", dynaConfig.EmailPort.ToString()),
                            getDictionary("Connection String", "*"),
                            getDictionary("Connection Name", dynaConfig.ConnectionName),
                            getDictionary("Database Name", dynaConfig.DatabaseName),
                            getDictionary("Domain Host", dynaConfig.DomainHost),
                            getDictionary("Domain Root Path Virtual", dynaConfig.DomainRootPathVirtual),
                            getDictionary("Domain Root Path Physical", dynaConfig.DomainRootPathPhysical),
                            getDictionary("Domain Claimants Path Virtual", dynaConfig.DomainClaimantsPathVirtual),
                            getDictionary("Domain Claimants Path Physical", dynaConfig.DomainClaimantsPathPhysical),
                            getDictionary("Device ID", dynaConfig.DeviceId)
                        };

                            int dpc = 1;
                            foreach (var dp in dynaConfig.DomainPaths)
                            {
                                eventItems_Login.Add(getDictionary("Domain Path Name " + dpc, dp.DomainPathName));
                                eventItems_Login.Add(getDictionary("Domain Path Physical " + dpc, dp.DomainPathPhysical));
                                eventItems_Login.Add(getDictionary("Domain Path Virtual " + dpc, dp.DomainPathVirtual));

                                dpc++;
                            }

                            CommonFunctions.AddLogEvent(DateTime.Now, "Login", false, eventItems_Login, "Dyna Config");

                            // If login was successfully completed
                            successCallback();
                        }
                        else
                        {
                            generalFault |= (userFault == false && passFault == false);
                            var loginDetails = new LoginScreenFaultDetails();

                            if (userFault)
                            {
                                loginDetails.UserNameErrorMessage = "User name does not exist";
                            }
                            else if (passFault)
                            {
                                loginDetails.PasswordErrorMessage = "Password does not match user name";
                            }
                            else if (generalFault)
                            {
                                loginDetails.CommonErrorMessage = "Login error";
                            }

                            // Otherwise
                            failCallback(loginDetails);

                            //if (failCount > 3)
                            //{
                            //	var SetDomainPrompt = UIAlertController.Create("Set Domain Name", "Enter domain name: ", UIAlertControllerStyle.Alert);
                            //	SetDomainPrompt.AddTextField((field) =>
                            //	{
                            //		field.Placeholder = "Domain Name";
                            //	});
                            //	//Add Actions
                            //	SetDomainPrompt.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, action => SaveDomain(SetDomainPrompt.TextFields[0].Text)));
                            //	SetDomainPrompt.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, null));
                            //	//Present Alert
                            //	var MasterViewController = (MasterViewController)((UINavigationController)SplitViewController.ViewControllers[0]).ViewControllers[0];
                            //	var ;
                            //	failCallback( new LoginScreenFaultDetails()
                            //	 PresentViewController(SetDomainPrompt, true, null);
                            //}

                        }
                    }
                    else
                    {
                        var loginDetails = new LoginScreenFaultDetails { CommonErrorMessage = "Service failure" };
                        failCallback(loginDetails);
                    }
                }
                else
                {
                    var loginDetails = new LoginScreenFaultDetails { CommonErrorMessage = "Internet connection is required to login" };
                    failCallback(loginDetails);
                }
            }
            catch (Exception ex)
            {
                CommonFunctions.sendErrorEmail(ex);
                var exceptionLoginDetails = new LoginScreenFaultDetails { CommonErrorMessage = "Error. An exception occured. If issue persists contact support." };
                failCallback(exceptionLoginDetails);
                //throw new Exception(ex.Message + Environment.NewLine + ex.StackTrace, ex.InnerException);
            }
        }

        public void Register(string email, string userName, string password, Action successCallback, Action<LoginScreenFaultDetails> failCallback)
        {
            // Do some operations to register user

            // If registration was successfully completed
            successCallback();

            // Otherwise
            // failCallback(new LoginScreenFaultDetails {
            //  CommonErrorMessage = "Some error message relative to whole form",
            //  EmailErrorMessage = "Some error message relative to e-mail form field",
            //  UserNameErrorMessage = "Some error message relative to user name form field",
            //  PasswordErrorMessage = "Some error message relative to password form field"
            // });
        }

        public void ResetPassword(string email, Action successCallback, Action<LoginScreenFaultDetails> failCallback)
        {
            // Do some operations to reset user's password

            // If password was successfully reset
            successCallback();

            // Otherwise
            // failCallback(new LoginScreenFaultDetails {
            //  CommonErrorMessage = "Some error message relative to whole form",
            //  EmailErrorMessage = "Some error message relative to e-mail form field"
            // });
        }

        public bool ShowPasswordResetLink
        {
            get
            {
                // If you want your login screen to have a forgot password button
                //return true;

                // Otherwise you can:
                return false;
            }
        }

        public bool ShowRegistration
        {
            get
            {
                // If you want your login screen to have a register new user button
                //return true;

                // Otherwise you can:
                return false;
            }
        }
    }



    public class DynaDialogViewController : DialogViewController
    {
        public bool IsForm;
        //public LoadingOverlay loadingOverlay = new LoadingOverlay(UIScreen.MainScreen.Bounds);

        public DynaDialogViewController(IntPtr handle) : base(handle)
        {
            Style = UITableViewStyle.Plain;
            //TableView.EstimatedSectionHeaderHeight = 0;
        }

        public DynaDialogViewController(RootElement root) : base(root)
        {
            Style = UITableViewStyle.Plain;
            Title = root.Caption;
            //TableView.EstimatedSectionHeaderHeight = 0;

            if (!IsForm)
            {
                RefreshRequested += delegate
                {
                    var MasterViewController = (MasterViewController)((UINavigationController)SplitViewController.ViewControllers[0]).ViewControllers[0];

                    MasterViewController.PresentViewController(CommonFunctions.AlertPrompt("Reload", "Reload location?", true, action =>
                    {
                        // Wait 3 seconds, to simulate some network activity
                        NSTimer.CreateScheduledTimer(1, delegate
                        {

                            //root[0].Add(new StringElement("Added " + (++i)));
                            //this.ViewDidLoad();
                            //this.ReloadData();
                            //this.TriggerRefresh();
                            //this.NavigationController.View.ReloadInputViews();
                            //this.NavigationController.View.SetNeedsDisplay();
                            //this.NavigationController.PopViewController(true);
                            //Root.TableView.ReloadData();
                            //this.TableView.SetNeedsDisplay();
                            //NavigationController.PopToRootViewController(true);
                            //NavigationController.ViewControllers[0].ViewDidLoad();
                            //MasterViewController.DynaStart();
                            //MasterViewController.TableView.SelectRow(null, true, UITableViewScrollPosition.Top);
                            //MasterViewController.TableView.SetNeedsDisplay();
                            //MasterViewController.NavigationController.PopToRootViewController(true);

                            MasterViewController.DynaLocations();
                            NavigationController.PopToRootViewController(true);
                            MasterViewController.ReloadData();

                            // Notify the dialog view controller that we are done
                            // this will hide the progress info
                            ReloadComplete();
                        });
                    }, true, action =>
                    {
                        ReloadComplete();
                    }), true, null);
                };
            }
        }

        public DynaDialogViewController(RootElement root, bool pushing) : base(root, pushing)
        {
            Style = UITableViewStyle.Plain;
            Title = root.Caption;
            //TableView.EstimatedSectionHeaderHeight = 0;

            if (!IsForm)
            {
                RefreshRequested += delegate
                {
                    var MasterViewController = (MasterViewController)((UINavigationController)SplitViewController.ViewControllers[0]).ViewControllers[0];

                    MasterViewController.PresentViewController(CommonFunctions.AlertPrompt("Reload", "Reload location?", true, action =>
                    {
                        // Wait 3 seconds, to simulate some network activity
                        NSTimer.CreateScheduledTimer(1, delegate
                        {
                            //MasterViewController.DynaStart();
                            //MasterViewController.TableView.SetNeedsDisplay();
                            //MasterViewController.NavigationController.PopToRootViewController(true);

                            MasterViewController.DynaLocations();
                            NavigationController.PopToRootViewController(true);
                            MasterViewController.ReloadData();

                            // Notify the dialog view controller that we are done
                            // this will hide the progress info
                            ReloadComplete();
                        });
                    }, true, action =>
                    {
                        ReloadComplete();
                    }), true, null);
                };
            }
        }

        public DynaDialogViewController(RootElement root, bool pushing, bool pull) : base(root, pushing)
        {
            Style = UITableViewStyle.Plain;
            Title = root.Caption;
            //TableView.EstimatedSectionHeaderHeight = 0;

            if (pull)
            {
                RefreshRequested += delegate
                {
                    var MasterViewController = (MasterViewController)((UINavigationController)SplitViewController.ViewControllers[0]).ViewControllers[0];

                    MasterViewController.PresentViewController(CommonFunctions.AlertPrompt("Reload", "Reload location?", true, action =>
                    {
                        // Wait 3 seconds, to simulate some network activity
                        NSTimer.CreateScheduledTimer(1, delegate
                        {

                            //MasterViewController.DynaStart();
                            //MasterViewController.NavigationController.PopToRootViewController(true);
                            //MasterViewController.TableView.SetNeedsDisplay();

                            MasterViewController.DynaLocations();
                            NavigationController.PopToRootViewController(true);
                            MasterViewController.ReloadData();

                            // Notify the dialog view controller that we are done
                            // this will hide the progress info
                            ReloadComplete();
                        });
                    }, true, action =>
                    {
                        ReloadComplete();
                    }), true, null);
                };
            }
        }

        //public int dosleep(int Milliseconds)
        //{
        //    System.Threading.Thread.Sleep(Milliseconds);
        //    //waittest = "changeddd";
        //    Console.WriteLine("Task finished");
        //    return Milliseconds;
        //}

        public override void LoadView()
        {
            base.LoadView();

            //var loadingOverlay = new LoadingOverlay(UIScreen.MainScreen.Bounds);
            //SplitViewController.Add(loadingOverlay);
            //var t2 = await Task.Factory.StartNew(() => dosleep(3000));

            var myTitleLabel = new UILabel(new CGRect(0, 0, 1, 1))
            {
                Text = Title,
                LineBreakMode = UILineBreakMode.WordWrap,
                Lines = 0,
                TextAlignment = UITextAlignment.Center
            };

            NavigationItem.TitleView = myTitleLabel;
            NavigationItem.TitleView.SizeToFit();
            NavigationItem.BackBarButtonItem = new UIBarButtonItem(@"", UIBarButtonItemStyle.Plain, null, null);

            if (NavigationItem.LeftBarButtonItem == null && this != NavigationController.ViewControllers[0])
            {
                var btnBack = new UIBarButtonItem(UIImage.FromBundle("Back"), UIBarButtonItemStyle.Plain, (sender, args) =>
                {
                    NavigationController.PopViewController(true);
                });
                NavigationItem.SetLeftBarButtonItem(btnBack, true);
            }

            //loadingOverlay.Hide();
        }

        //public override void ViewWillAppear(bool animated)
        //{
        //    base.ViewWillAppear(true);
        //    //loadingOverlay.Hide();
        //}

        //public override async void ViewWillDisappear(bool animated)
        //{
        //    base.ViewWillDisappear(true);
        //    SplitViewController.Add(loadingOverlay);
        //    var t2 = await Task.Factory.StartNew(() => dosleep(3000));
        //}

    }



    public class SectionStringElement : Element
    {
        static NSString skey = new NSString("StringElement");
        static NSString skeyvalue = new NSString("StringElementValue");
        public UITextAlignment Alignment = UITextAlignment.Left;
        public string Value;
        public bool selected;
        public bool vsection;
        public bool vquestion;
        public bool mrdownload;
        public bool fileupload;
        public bool IsSelectedReport;

        //public bool selected = false;
        //public NSIndexPath prevsel;

        public SectionStringElement(string caption) : base(caption) { }

        public SectionStringElement(string caption, string value) : base(caption)
        {
            Value = value;
        }

        public SectionStringElement(string caption, NSAction tapped) : base(caption)
        {
            Tapped += tapped;
        }

        public event NSAction Tapped;

        public override UITableViewCell GetCell(UITableView tv)
        {
            var cell = tv.DequeueReusableCell(Value == null ? skey : skeyvalue);
            if (cell == null)
            {
                cell = new UITableViewCell(Value == null ? UITableViewCellStyle.Default : UITableViewCellStyle.Value1, Value == null ? skey : skeyvalue);
                cell.SelectionStyle = (Tapped != null) ? UITableViewCellSelectionStyle.Blue : UITableViewCellSelectionStyle.None;
            }
            cell.Accessory = UITableViewCellAccessory.None;
            cell.TextLabel.Text = Caption;
            cell.TextLabel.TextAlignment = Alignment;
            cell.BackgroundColor = UIColor.White;
            cell.SelectionStyle = UITableViewCellSelectionStyle.None;

            cell.TextLabel.LineBreakMode = UILineBreakMode.WordWrap;
            cell.TextLabel.Lines = 0;

            if (cell.TextLabel.Text == "Causal Relationship" || cell.TextLabel.Text == "Treatment" || cell.TextLabel.Text == "Diagnostic Testing" || cell.TextLabel.Text == "Disability")
            {
                cell.BackgroundColor = UIColor.FromRGB(242, 211, 121);
            }

            if (selected)
            {
                cell.BackgroundColor = UIColor.FromRGB(220, 237, 185);
            }

            if (vsection)
            {
                cell.TextLabel.Font = UIFont.BoldSystemFontOfSize(13);
            }

            if (vquestion)
            {
                cell.TextLabel.Font = UIFont.SystemFontOfSize(13);
            }

            if (mrdownload)
            {
                cell.BackgroundColor = UIColor.FromRGB(232, 247, 238);
            }

            if (fileupload)
            {
                cell.BackgroundColor = UIColor.FromRGB(100, 202, 90);
            }

            if (IsSelectedReport)
            {
                cell.BackgroundColor = UIColor.FromRGB(133, 203, 51);
            }

            // The check is needed because the cell might have been recycled.
            if (cell.DetailTextLabel != null)
                cell.DetailTextLabel.Text = Value ?? "";

            return cell;
        }

        public override string Summary()
        {
            return Caption;
        }

        public override void Selected(DialogViewController dvc, UITableView tableView, NSIndexPath path)
        {
            try
            {
                var cell = base.GetCell(tableView);
                cell.BackgroundColor = UIColor.Red;

                base.Selected(dvc, tableView, path);

                //prevsel = tableView.IndexPathForSelectedRow;

                if (Tapped != null)
                    Tapped();
                selected = !selected;
                tableView.SelectRow(path, true, UITableViewScrollPosition.None);
            }
            catch (Exception ex)
			{
                var eventItems = new List<NSDictionary>
                {
                    getDictionary("Exception Message", ex.Message),
                    getDictionary("Exception Stacktrace", ex.StackTrace)
                };
                CommonFunctions.AddLogEvent(DateTime.Now, "SectionStringElement", true, eventItems, "Selected catch block");
                
                CommonFunctions.sendErrorEmail(ex);
                dvc.InvokeOnMainThread(() =>
                {
                    dvc.PresentViewController(CommonFunctions.ExceptionAlertPrompt(ex), true, null);
                });
            }
        }

        public override bool Matches(string text)
        {
            return (Value != null && Value.IndexOf(text, StringComparison.CurrentCultureIgnoreCase) != -1) || base.Matches(text);
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
    }




    public class PresetRadioElement : RadioElement
    {
        readonly static NSString ReuseId = new NSString("PresetRadioElement");
        public int? Index { get; protected set; }
        public string PresetID;
        public string PresetName;
        public string PresetJson;
        public UIAlertController UpdatePresetPrompt;
        public UIButton editPresetBtn = new UIButton(new CGRect(0, 0, 50, 50));
        public UIButton deletePresetBtn = new UIButton(new CGRect(50, 0, 50, 50));

        public PresetRadioElement(string cCaption, string cGroup) : base(cCaption, cGroup) { Group = cGroup; }
        //public MyRadioElement(string s) : base(s) { }

        public override void Selected(DialogViewController dvc, UITableView tableView, NSIndexPath indexPath)
        {
            base.Selected(dvc, tableView, indexPath);
            var selected = OnSelected;
            if (selected != null)
                selected(this, EventArgs.Empty);

            dvc.DeactivateController(true);
        }

        protected override NSString CellKey
        {
            get
            {
                return ReuseId;
            }
        }

        public override UITableViewCell GetCell(UITableView tv)
        {
            EnsureIndex();

            var cell = tv.DequeueReusableCell(CellKey);
            if (cell == null)
            {
                cell = new UITableViewCell(UITableViewCellStyle.Subtitle, CellKey);
            }

            //var cell = base.GetCell(tv);
            cell.ContentView.AutosizesSubviews = false;
            cell.TextLabel.Text = Caption;

            //if (!string.IsNullOrEmpty(_subtitle))
            //{
            //	cell.DetailTextLabel.Text = _subtitle;
            //}

            var selected = false;

            var slRoot = Parent.Parent as DynaRootElement;

            if (slRoot != null)
            {
                if (Index == slRoot.RadioSelected)
                {
                    selected = true;
                }
                else selected |= PresetID == slRoot.RadioSelected.ToString();
                //selected = Index == slRoot.RadioSelected;
                //selected = PresetID == slRoot.RadioSelected.ToString();
            }
            else
            {
                var root = (RootElement)Parent.Parent;
                selected = Index == root.RadioSelected;
            }

            cell.Accessory = selected ? UITableViewCellAccessory.Checkmark : UITableViewCellAccessory.None;
            cell.Selected = selected;
            cell.UserInteractionEnabled = true;
            cell.TextLabel.TextColor = UIColor.Black;
            cell.BackgroundColor = UIColor.White;
            cell.SelectionStyle = UITableViewCellSelectionStyle.None;
            cell.TextLabel.LineBreakMode = UILineBreakMode.WordWrap;
            cell.TextLabel.Lines = 0;

            if (selected)
            {
                cell.BackgroundColor = UIColor.FromRGB(239, 246, 223);
            }

            if (!cell.UserInteractionEnabled)
            {
                cell.TextLabel.TextColor = UIColor.LightGray;
                cell.BackgroundColor = UIColor.GroupTableViewBackgroundColor;
                if (selected)
                {
                    cell.Accessory = UITableViewCellAccessory.None;
                }
            }

            editPresetBtn.Enabled = selected;

            cell.Accessory = UITableViewCellAccessory.None;

            if (PresetName != "No Preset")
            {
                editPresetBtn.SetImage(UIImage.FromBundle("Save"), UIControlState.Normal);
                deletePresetBtn.SetImage(UIImage.FromBundle("Delete"), UIControlState.Normal);

                var cellPreset = new UITableViewCell(UITableViewCellStyle.Default, null) { Frame = new CGRect(0, 0, 100, 50) };
                //cellPreset.BackgroundColor = UIColor.FromRGB(224, 238, 240);
                ////cellPreset.ImageView.Image = UIImage.FromBundle("CircledPlay");
                //cellPreset.ContentView.Add(mre);
                cellPreset.ContentView.Add(editPresetBtn);
                cellPreset.ContentView.Add(deletePresetBtn);

                cell.AccessoryView = cellPreset;
            }

            return cell;
        }

        void EnsureIndex()
        {
            if (!Index.HasValue)
            {
                var parent = Parent as Section;

                Index = parent.Elements.IndexOf(this);
            }
        }

        public event EventHandler<EventArgs> OnSelected;
    }






    public class PaddedUIView<T> : UIView where T : UIView, new()
    {
        nfloat _padding;
        T _nestedView;
        public bool Enabled;
        public string Type;
        public bool Required;

        public PaddedUIView()
        {
            Initialize();
        }

        public PaddedUIView(CGRect bounds)
            : base(bounds)
        {
            Initialize();
        }

        void Initialize()
        {
            if (_nestedView == null)
            {
                _nestedView = new T();
                AddSubview(_nestedView);
            }

            _nestedView.Frame = new CGRect(_padding + 5, _padding, Frame.Width - 2 * _padding, Frame.Height - 2 * _padding);
        }

        public void setStyle()
        {
            (_nestedView as UILabel).LineBreakMode = UILineBreakMode.WordWrap;
            (_nestedView as UILabel).Lines = 0;

            switch (Type)
            {
                case "Question":
                    (_nestedView as UILabel).TextColor = UIColor.DarkGray;
                    (_nestedView as UILabel).Font = UIFont.SystemFontOfSize(13);

                    BackgroundColor = Enabled ? UIColor.FromRGB(230, 230, 250) : UIColor.GroupTableViewBackgroundColor;

                    NSMutableAttributedString AttributedText;

                    var reqStringAttributes = new UIStringAttributes { ForegroundColor = Enabled ? UIColor.Red : UIColor.LightGray };

                    var stringAttributes = new UIStringAttributes { ForegroundColor = Enabled ? UIColor.DarkGray : UIColor.LightGray };

                    var textstring = (_nestedView as UILabel).Text;

                    if (Required)
                    {
                        textstring = textstring + " *";

                        AttributedText = new NSMutableAttributedString(textstring);
                        AttributedText.SetAttributes(stringAttributes, new NSRange(0, textstring.Length - 1));
                        AttributedText.SetAttributes(reqStringAttributes.Dictionary, new NSRange(textstring.Length - 1, 1));
                    }
                    else
                    {
                        AttributedText = new NSMutableAttributedString(textstring);
                        AttributedText.SetAttributes(stringAttributes.Dictionary, new NSRange(0, textstring.Length));
                    }

                    (_nestedView as UILabel).AttributedText = AttributedText;
                    break;
                case "Subtitle":
                    (_nestedView as UILabel).TextColor = UIColor.Black;
                    (_nestedView as UILabel).Font = UIFont.SystemFontOfSize(10);
                    if (Enabled)
                    {
                        BackgroundColor = UIColor.FromRGB(250, 250, 229);
                        (_nestedView as UILabel).TextColor = UIColor.Black;
                    }
                    else
                    {
                        BackgroundColor = UIColor.GroupTableViewBackgroundColor;
                        (_nestedView as UILabel).TextColor = UIColor.LightGray;
                    }
                    break;
                case "Preset":
                    (_nestedView as UILabel).TextColor = UIColor.DarkGray;
                    (_nestedView as UILabel).Font = UIFont.SystemFontOfSize(13);
                    BackgroundColor = UIColor.FromRGB(216, 219, 226);
                    break;
                case "Section":
                    (_nestedView as UILabel).TextColor = UIColor.Black;
                    (_nestedView as UILabel).Font = UIFont.SystemFontOfSize(17);
                    BackgroundColor = UIColor.FromRGB(169, 188, 208);
                    break;
                case "Validation":
                    (_nestedView as UILabel).TextColor = UIColor.Black;
                    (_nestedView as UILabel).Font = UIFont.SystemFontOfSize(13);
                    BackgroundColor = UIColor.FromRGB(169, 188, 208);
                    break;
                case "White":
                    (_nestedView as UILabel).TextColor = UIColor.Black;
                    (_nestedView as UILabel).Font = UIFont.SystemFontOfSize(13);
                    BackgroundColor = UIColor.White;
                    break;
                default:
                    (_nestedView as UILabel).TextColor = UIColor.DarkGray;
                    (_nestedView as UILabel).Font = UIFont.SystemFontOfSize(13);
                    BackgroundColor = UIColor.GroupTableViewBackgroundColor;
                    break;
            }
        }

        public T NestedView
        {
            get { return _nestedView; }
        }

        public nfloat Padding
        {
            get { return _padding; }
            set { if (value != _padding) { _padding = value; Initialize(); } }
        }

        public override CGRect Frame
        {
            get { return base.Frame; }
            set { base.Frame = value; Initialize(); }
        }
    }






    public class DynaSection : Section
    {
        public bool Enabled;
        public string ConditionTriggerId;
        public string ActiveTriggerId = "";
        public List<QuestionOption> QuestionOptions;
        public List<QuestionOption> QuestionAnswers;
        public string QuestionId { get; set; }
        public string QuestionParentId { get; set; }
        public string QuestionText { get; set; }
        public string QuestionType { get; set; }
        public string QuestionKeyboardType { get; set; }
        public bool Answered { get; set; }
        public bool Disabled { get; set; }
        public string AnswerId { get; set; }
        public string AnswerText { get; set; }
        public string ParentConditionTriggerId { get; set; }
        public bool IsConditional { get; set; }
        public bool IsRequired { get; set; }
        public bool IsInvalid { get; set; }
        public Group thisGroup { get; set; }

        public DynaSection(string caption) : base(caption) { }

        public override string Summary()
        {
            //UITableView viewty = base.GetContainerTableView();
            return base.Summary();
        }
        static NSString cellKey = new NSString("Identifier");
        protected override NSString CellKey
        {
            get
            {
                return (NSString)"Identifier";
            }
        }
        public override UITableViewCell GetCell(UITableView tv)
        {
            var cell = tv.DequeueReusableCell(CellKey);
            if (cell == null)
            {
                cell = base.GetCell(tv);
            }

            //cell.PrepareForReuse();
            cell.ContentView.AutosizesSubviews = false;
            cell.UserInteractionEnabled = Enabled;

            return cell;
        }
    }





    public class DynaRootElement : RootElement
    {
        public bool Enabled;
        public string ConditionTriggerId;
        public string ActiveTriggerId = "";
        public List<QuestionOption> QuestionOptions;
        public List<QuestionOption> QuestionAnswers;
        public string QuestionId { get; set; }
        public string QuestionParentId { get; set; }
        public string QuestionText { get; set; }
        public string QuestionType { get; set; }
        public string QuestionKeyboardType { get; set; }
        public bool Answered { get; set; }
        public bool Disabled { get; set; }
        public string AnswerId { get; set; }
        public string AnswerText { get; set; }
        public string ParentConditionTriggerId { get; set; }
        public bool IsConditional { get; set; }
        public Group thisGroup { get; set; }
        public bool IsPreset { get; set; }

        public DynaRootElement(string caption) : base(caption)
        {
            createOnSelected = (RootElement arg) =>
            {
                return new DynaDialogViewController(arg);
            };
        }

        public DynaRootElement(string caption, Func<RootElement, UIViewController> createOnSelected) : base(caption, createOnSelected)
        {
            createOnSelected = (RootElement arg) =>
            {
                return new DynaDialogViewController(arg);
            };
        }

        public DynaRootElement(string caption, int section, int element) : base(caption, section, element)
        {
            createOnSelected = (RootElement arg) =>
            {
                return new DynaDialogViewController(arg);
            };
        }

        public DynaRootElement(string caption, Group group) : base(caption, group)
        {
            thisGroup = group;

            createOnSelected = (RootElement arg) =>
            {
                return new DynaDialogViewController(arg);
            };
        }

        public override string Summary()
        {
            //UITableView viewty = base.GetContainerTableView();
            return base.Summary();
        }
        static NSString cellKey = new NSString("Identifier");
        protected override NSString CellKey
        {
            get
            {
                return (NSString)"Identifier";
            }
        }
        public override UITableViewCell GetCell(UITableView tv)
        {
            //var cell = base.GetCell(tv);
            //cell.TextLabel.Font = UIFont.BoldSystemFontOfSize(17);
            //return cell;
            var cell = tv.DequeueReusableCell(CellKey);
            if (cell == null)
            {
                cell = base.GetCell(tv);

                if (RadioSelected == -1)
                {
                    cell.DetailTextLabel.Text = "";
                }
            }
            //cell.PrepareForReuse();

            cell.UserInteractionEnabled = Enabled;
            cell.TextLabel.TextColor = UIColor.Black;
            cell.BackgroundColor = UIColor.White;

            if (!Enabled)
            {
                cell.TextLabel.TextColor = UIColor.LightGray;
                cell.BackgroundColor = UIColor.GroupTableViewBackgroundColor;
            }
            if (IsPreset)
            {
                cell.BackgroundColor = UIColor.FromRGB(224, 238, 240);
            }

            return cell;

        }

        protected override void PrepareDialogViewController(UIViewController dvc)
        {
            //dvc.View.BackgroundColor = Settings.RootBackgroundColour;
            base.PrepareDialogViewController(dvc);
        }
    }





    public class DynaFormRootElement : RootElement
    {
        public bool Enabled;
        public string FormID { get; set; }
        public string FormName { get; set; }
        public bool IsDoctorForm { get; set; }
        public string MenuValue { get; set; }
        public string MenuAction { get; set; }
        public string PatientID { get; set; }
        public string PatientName { get; set; }
        public string DoctorID { get; set; }
        public string DoctorName { get; set; }
        public string LocationID { get; set; }
        public string ApptID { get; set; }
        public DateTime ApptDate { get; set; }
        public string ReportID { get; set; }
        public string CaseID { get; set; }
        public string ApptNotes { get; set; }
        public string PatientNotes { get; set; }
        public List<Report> ApptReports { get; set; }
        public Group thisGroup { get; set; }
        public bool ShowLoading { get; set; }
        public FileInfo RestoreFile { get; set; }
        public UIColor Color { get; set; }
        public bool PendingUpdate { get; set; }
        public DateTime? DateSubmittedPatientForm { get; set; }
        public DateTime? DateSubmittedDoctorForm { get; set; }
        public DateTime? DateGeneratedReport { get; set; }

        public Boolean isRunning = false;
        Object _lockObject = new object();

        CancellationTokenSource cts;

        nfloat labelHeight = 22;
        nfloat labelWidth = 70;
        nfloat centerX;
        nfloat centerY;
        UIButton cancelButton;

        public void HandleCancel(DialogViewController dvc, UITableView tableView, NSIndexPath path)
        {
            base.Deselected(dvc, tableView, path);
            tableView.DeselectRow(path, true);
            var deselected = OnDeSelected;
            if (deselected != null)
            {
                deselected(null, EventArgs.Empty);
            }
        }

        public async Task<string> SetSelected(DialogViewController dvc, UITableView tableView, NSIndexPath path, CancellationToken cts)
        {
            await Task.Delay(10, cts);

            base.Selected(dvc, tableView, path);

            var selected = OnSelected;
            if (selected != null)
            {
                selected(this, EventArgs.Empty);
            }

            return "selected";
        }

        public override async void Selected(DialogViewController dvc, UITableView tableView, NSIndexPath path)
        {
            var loadingOverlay = new LoadingOverlay(tableView.Bounds);

            try
            {
                _lockObject = this;

                lock (_lockObject)
                {
                    if (isRunning)
                    {
                        return;
                    }
                    else
                    {
                        isRunning = true;
                    }
                }

                if (ShowLoading)
                {
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
                            var eventItems = new List<NSDictionary>
                            {
                                getDictionary("Exception Message", oex.Message),
                                getDictionary("Exception Stacktrace", oex.StackTrace)
                            };
                            CommonFunctions.AddLogEvent(DateTime.Now, "DynaFormRootElement", true, eventItems, "Selected cancelButton catch block");
                            
                            Console.WriteLine($"\nObjectDisposedException in cancelButton.TouchUpInside with: {oex.Message}");
                        }
                    };

                    loadingOverlay.AddSubview(cancelButton);

                    dvc.Add(loadingOverlay);

                    await Task.Delay(10);

                    using (cts = new CancellationTokenSource())
                    {
                        try
                        {
                            cts.CancelAfter(TimeSpan.FromSeconds(30));
                            await Task.Delay(TimeSpan.FromSeconds(1), cts.Token);

                            //Console.WriteLine($"\nGot section with: {result}");

                            var task = SetSelected(dvc, tableView, path, cts.Token);

                            var result = await task;
                        }
                        catch (TaskCanceledException tex)       // if the operation is cancelled, do nothing
						{
                            var eventItems = new List<NSDictionary>
                            {
                                getDictionary("Exception Message", tex.Message),
                                getDictionary("Exception Stacktrace", tex.StackTrace)
                            };
                            CommonFunctions.AddLogEvent(DateTime.Now, "DynaFormRootElement", true, eventItems, "Selected cts catch block");
                            
                            Console.WriteLine($"\nCanceled with: {tex.Message}");

                            HandleCancel(dvc, tableView, path);

                            dvc.InvokeOnMainThread(() =>
                            {
                                dvc.PresentViewController(CommonFunctions.AlertPrompt("Canceled/Timeout", "Operation was canceled or timed out, please try again", true, null, false, null), true, null);
                            });
                        }
                    }
                }
                else
                {
                    base.Selected(dvc, tableView, path);

                    var selected = OnSelected;
                    if (selected != null)
                    {
                        selected(this, EventArgs.Empty);
                    }
                }

                //loadingOverlay.Hide();
            }
            catch (Exception ex)
			{
                var eventItems = new List<NSDictionary>
                {
                    getDictionary("Exception Message", ex.Message),
                    getDictionary("Exception Stacktrace", ex.StackTrace)
                };
                CommonFunctions.AddLogEvent(DateTime.Now, "DynaFormRootElement", true, eventItems, "Selected catch block");
                
                CommonFunctions.sendErrorEmail(ex);
                dvc.InvokeOnMainThread(() =>
                {
                    dvc.PresentViewController(CommonFunctions.ExceptionAlertPrompt(ex), true, null);
                }); 
            }
            finally
            {
                isRunning = false;
                loadingOverlay.Hide();
            }
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

        public event EventHandler<EventArgs> OnSelected;
        public event EventHandler<EventArgs> OnDeSelected;

        public DynaFormRootElement(string caption) : base(caption)
        {
            createOnSelected = (RootElement arg) =>
            {
                return new DynaDialogViewController(arg);
            };
        }

        public DynaFormRootElement(string caption, Func<RootElement, UIViewController> createOnSelected) : base(caption, createOnSelected)
        {
            createOnSelected = (RootElement arg) =>
            {
                return new DynaDialogViewController(arg);
            };
        }

        public DynaFormRootElement(string caption, int section, int element) : base(caption, section, element)
        {
            createOnSelected = (RootElement arg) =>
            {
                return new DynaDialogViewController(arg);
            };
        }

        public DynaFormRootElement(string caption, Group group) : base(caption, group)
        {
            thisGroup = group;

            createOnSelected = (RootElement arg) =>
            {
                return new DynaDialogViewController(arg);
            };
        }

        public override string Summary()
        {
            //UITableView viewty = base.GetContainerTableView();
            return base.Summary();
        }
        static NSString cellKey = new NSString("Identifier");

        protected override NSString CellKey
        {
            get
            {
                return (NSString)"Identifier";
            }
        }

        public override UITableViewCell GetCell(UITableView tv)
        {
            //var cell = base.GetCell(tv);
            //cell.TextLabel.Font = UIFont.BoldSystemFontOfSize(17);
            //return cell;
            var cell = tv.DequeueReusableCell(CellKey);
            if (cell == null)
            {
                cell = base.GetCell(tv);
            }
            //cell.PrepareForReuse();

            cell.UserInteractionEnabled = Enabled;
            cell.TextLabel.TextColor = UIColor.Black;
            cell.TextLabel.LineBreakMode = UILineBreakMode.WordWrap;
            cell.TextLabel.Lines = 0;
            cell.BackgroundColor = UIColor.White;

            if (!Enabled)
            {
                cell.TextLabel.TextColor = UIColor.LightGray;
                cell.BackgroundColor = UIColor.GroupTableViewBackgroundColor;
            }

            if (Color != null)
            {
                cell.BackgroundColor = Color;
            }

            if (PendingUpdate)
            {
                //cell.AccessoryView = new UIImageView(UIImage.FromBundle("Save"));
                cell.Accessory = UITableViewCellAccessory.DetailDisclosureButton;
            }

            if (DateSubmittedPatientForm.GetHashCode() != 0)
            {
                cell.BackgroundColor = UIColor.Yellow;
            }

            if (DateSubmittedDoctorForm.GetHashCode() != 0)
            {
                cell.BackgroundColor = UIColor.Blue;
            }

            if (DateGeneratedReport.GetHashCode() != 0)
            {
                cell.BackgroundColor = UIColor.Green;
            }

            return cell;
        }

        //public override void Selected(DialogViewController dvc, UITableView tableView, NSIndexPath indexPath)
        //{
        //	// dostuff
        //	tableView.SelectRow(indexPath, true, UITableViewScrollPosition.Top);
        //}

        protected override void PrepareDialogViewController(UIViewController dvc)
        {
            //dvc.View.BackgroundColor = Settings.RootBackgroundColour;
            base.PrepareDialogViewController(dvc);
        }
    }






    public class PlaceholderEnabledUITextView : UITextView
    {
        public bool Enabled;
        public string ConditionTriggerId;
        public string ActiveTriggerId = "";
        public List<QuestionOption> QuestionOptions;
        public List<QuestionOption> QuestionAnswers;
        public string QuestionId { get; set; }
        public string QuestionParentId { get; set; }
        public string QuestionText { get; set; }
        public string QuestionType { get; set; }
        public string QuestionKeyboardType { get; set; }
        public bool Answered { get; set; }
        public bool Disabled { get; set; }
        public string AnswerId { get; set; }
        public string AnswerText { get; set; }
        public string ParentConditionTriggerId { get; set; }
        public bool IsConditional { get; set; }
        public bool Required { get; set; }
        public bool Invalid { get; set; }
        public DynaSection parentSec { get; set; }
        UILabel placeholderLabel;

        public PlaceholderEnabledUITextView()
        {
            CommonInit();
        }

        public PlaceholderEnabledUITextView(CGRect frame)
            : base(frame)
        {
            CommonInit();
        }

        public PlaceholderEnabledUITextView(CGRect frame, NSTextContainer container)
            : base(frame, container)
        {
            CommonInit();
        }

        public PlaceholderEnabledUITextView(NSCoder coder)
            : base(coder)
        {
            CommonInit();
        }

        public PlaceholderEnabledUITextView(NSObjectFlag t)
            : base(t)
        {
            CommonInit();
        }

        public PlaceholderEnabledUITextView(IntPtr handler)
            : base(handler)
        {
            CommonInit();
        }

        public string Placeholder { get; set; }

        public UIColor PlaceholderColor { get; set; }

        public UIFont PlaceholderFont { get; set; }

        public bool AllowWhiteSpace { get; set; }

        public override void Draw(CGRect rect)
        {
            base.Draw(rect);

            UserInteractionEnabled = Enabled;

            //this.placeholderLabel = new UILabel(frame)
            //{
            //	BackgroundColor = UIColor.Clear,
            //	Font = this.PlaceholderFont,
            //	LineBreakMode = UILineBreakMode.WordWrap,
            //	Lines = 0,
            //	TextColor = this.PlaceholderColor
            //};

            ((UILabel)Subviews[1]).BackgroundColor = UIColor.Clear;
            ((UILabel)Subviews[1]).Font = PlaceholderFont;
            ((UILabel)Subviews[1]).LineBreakMode = UILineBreakMode.WordWrap;
            ((UILabel)Subviews[1]).Lines = 0;
            ((UILabel)Subviews[1]).TextColor = PlaceholderColor;

            if (string.IsNullOrEmpty(Text))
            {
                placeholderLabel.Text = Placeholder;
                ((UILabel)Subviews[1]).Text = Placeholder;

            }
            else
            {
                placeholderLabel.Hidden = true;
                ((UILabel)Subviews[1]).Hidden = true;
            }

            //if (Invalid)
            //{
            //	this.Layer.BorderWidth = 1;
            //	this.Layer.BorderColor = UIColor.Red.CGColor;
            //}

            parentSec.HeaderView.Layer.BorderWidth = 0;
            parentSec.HeaderView.Layer.BorderColor = UIColor.Black.CGColor;

            if (Invalid)
            {
                parentSec.HeaderView.Layer.BorderWidth = 1;
                parentSec.HeaderView.Layer.BorderColor = UIColor.Red.CGColor;
            }

            //if (!Enabled)
            //{
            //	//if (string.IsNullOrEmpty(this.Text))
            //	//{
            //	//	placeholderLabel.Text = "Not applicable";
            //	//}
            //	//else
            //	//{
            //	//	placeholderLabel.Text = this.Text;
            //	//}
            //	placeholderLabel.TextColor = UIColor.LightGray;
            //	//placeholderLabel.BackgroundColor = UIColor.GroupTableViewBackgroundColor;
            //	TextColor = UIColor.LightGray;
            //	BackgroundColor = UIColor.GroupTableViewBackgroundColor;
            //}
            //else
            //{
            //	//if (string.IsNullOrEmpty(this.Text))
            //	//{
            //	//	this.placeholderLabel.Text = this.Placeholder;
            //	//}
            //	//else
            //	//{
            //	//	this.placeholderLabel.Hidden = true;
            //	//}
            //	//placeholderLabel.Hidden = true;
            //	placeholderLabel.BackgroundColor = UIColor.Clear;
            //	placeholderLabel.TextColor = this.PlaceholderColor;
            //	BackgroundColor = UIColor.White;
            //	TextColor = UIColor.Black;
            //}
            //this.Add(this.placeholderLabel);

        }

        void CommonInit()
        {
            PlaceholderFont = UIFont.SystemFontOfSize(17, UIFontWeight.Light);
            Placeholder = "Placeholder";

            var inset = TextContainerInset;
            var leftInset = inset.Left + TextContainer.LineFragmentPadding + 13;
            var rightInset = inset.Left + TextContainer.LineFragmentPadding;
            var maxSize = new CGSize
            {
                Width = Frame.Width - (leftInset + rightInset),
                Height = Frame.Height
            };

            var size = new NSString(Placeholder).GetBoundingRect(maxSize, NSStringDrawingOptions.UsesLineFragmentOrigin, new UIStringAttributes { Font = PlaceholderFont }, null).Size;
            size.Width = maxSize.Width;
            var frame = new CGRect(new CGPoint(leftInset, inset.Top), size);
            placeholderLabel = new UILabel(frame);

            Add(placeholderLabel);

            Started += OnStarted;
            Ended += OnEnded;
        }

        void OnStarted(object sender, EventArgs e)
        {
            placeholderLabel.Hidden = true;
        }

        void OnEnded(object sender, EventArgs e)
        {
            //AnswerText = Text;

            if (AllowWhiteSpace)
            {
                placeholderLabel.Hidden = !string.IsNullOrEmpty(Text);
            }
            else
            {
                placeholderLabel.Hidden = !string.IsNullOrWhiteSpace(Text);
            }
        }
    }








    public class DynaEntryElement : EntryElement
    {
        public bool Enabled;
        public string ConditionTriggerId;
        public string ActiveTriggerId = "";
        public List<QuestionOption> QuestionOptions;
        public List<QuestionOption> QuestionAnswers;
        public string QuestionId { get; set; }
        public string QuestionParentId { get; set; }
        public string QuestionText { get; set; }
        public string QuestionType { get; set; }
        public string QuestionKeyboardType { get; set; }
        public bool Answered { get; set; }
        public bool Disabled { get; set; }
        public string AnswerId { get; set; }
        public string AnswerText { get; set; }
        public string ParentConditionTriggerId { get; set; }
        public bool IsConditional { get; set; }
        public bool Required { get; set; }
        public bool Invalid { get; set; }
        public int MaxChars { get; set; }
        public UITextField EntryTextField { get; set; }
        //static NSString MyCellId = new NSString("MyCellId");
        //public EventHandler EntryEnded { get; set; }

        public DynaEntryElement(string cCaption, string cPlaceHolder, string cValue) : base(cCaption, cPlaceHolder, cValue) { }

        protected override UITextField CreateTextField(CGRect frame)
        {
            var tf = base.CreateTextField(frame);
            EntryTextField = tf;

            return tf;
        }

        public override UITableViewCell GetCell(UITableView tv)
        {
            var cell = base.GetCell(tv);

            cell.ContentView.AutosizesSubviews = false;
            cell.ContentView.Bounds = new CGRect(cell.Bounds.X, cell.Bounds.Y, tv.Bounds.Width, cell.Bounds.Height);
            cell.ContentView.Frame = new CGRect(cell.Frame.X, cell.Frame.Y, tv.Frame.Width, cell.Frame.Height);
            cell.UserInteractionEnabled = Enabled;
            cell.TextLabel.TextColor = UIColor.Black;
            cell.BackgroundColor = UIColor.White;
            EntryTextField.TextColor = UIColor.Black;
            EntryTextField.Placeholder = "Enter your answer here";
            cell.TextLabel.LineBreakMode = UILineBreakMode.WordWrap;
            cell.TextLabel.Lines = 0;
            var offset = 20;
            cell.Frame = new RectangleF((float)cell.Frame.X, (float)cell.Frame.Y, (float)(tv.Frame.Width - offset), (float)cell.Frame.Height);
            var size = GetEntryPosition(UIFont.BoldSystemFontOfSize(17));
            var yOffset = (float)((cell.ContentView.Bounds.Height - size.Height) / 2 - 1);
            var width = (float)(cell.ContentView.Bounds.Width - size.Width);

            if (TextAlignment == UITextAlignment.Right)
            {
                // Add padding if right aligned
                width -= 10;
            }

            var entryFrame = new RectangleF(size.Width, yOffset, width, size.Height);
            EntryTextField.Frame = entryFrame;

            if (!Enabled)
            {
                cell.TextLabel.TextColor = UIColor.LightGray;
                cell.BackgroundColor = UIColor.GroupTableViewBackgroundColor;
                EntryTextField.TextColor = UIColor.LightGray;
                EntryTextField.Placeholder = "Not applicable";
            }

            //if (Invalid)
            //{
            //	cell.Layer.BorderWidth = 1;
            //	cell.Layer.BorderColor = UIColor.Red.CGColor;
            //}

            var parentSec = (DynaSection)Parent;
            parentSec.HeaderView.Layer.BorderWidth = 0;
            parentSec.HeaderView.Layer.BorderColor = UIColor.Clear.CGColor;

            if (Invalid)
            {
                parentSec.HeaderView.Layer.BorderWidth = 1;
                parentSec.HeaderView.Layer.BorderColor = UIColor.Red.CGColor;
            }

            if (QuestionKeyboardType == "4")
            {
                EntryTextField.ShouldChangeCharacters = (textField, range, replacement) =>
                {
                    var newContent = new NSString(textField.Text).Replace(range, new NSString(replacement)).ToString();

                    return newContent.Length <= MaxChars && (replacement.Length == 0 || int.TryParse(replacement, out int number));
                };
            }

            return cell;
        }

        SizeF GetEntryPosition(UIFont font)
        {
            var s = Parent as Section;

            var max = new SizeF(-15, 17);

            foreach (var e in s.Elements)
            {
                var ee = e as DynaEntryElement;
                if (ee == null)
                    continue;

                if (ee.Caption != null)
                {
                    var size = new NSString(ee.Caption).GetSizeUsingAttributes(new UIStringAttributes { Font = font });
                    if (size.Width > max.Width)
                        max = (SizeF)size;
                }
            }

            s.EntryAlignment = new SizeF(20 + Math.Min(max.Width, 160), max.Height);

            return (SizeF)s.EntryAlignment;
        }
    }



    public class DynaGrid : SfDataGrid
    {
        public bool IsEnabled;
        public string ConditionTriggerId;
        public string ActiveTriggerId = "";
        public List<QuestionOption> QuestionOptions;
        public List<QuestionOption> QuestionAnswers;
        public string QuestionId { get; set; }
        public string QuestionParentId { get; set; }
        public string QuestionText { get; set; }
        public string QuestionType { get; set; }
        public string QuestionKeyboardType { get; set; }
        public bool Answered { get; set; }
        public bool Disabled { get; set; }
        public string AnswerId { get; set; }
        public string AnswerText { get; set; }
        public string ParentConditionTriggerId { get; set; }
        public bool IsConditional { get; set; }
        public bool Required { get; set; }
        public bool Invalid { get; set; }
        public int MaxChars { get; set; }
        public DynaSection parentSec { get; set; }
        //static NSString MyCellId = new NSString("MyCellId");

        //public DynaGrid() { }

        public override void Draw(CGRect rect)
        {
            base.Draw(rect);

            UserInteractionEnabled = IsEnabled;

            parentSec.HeaderView.Layer.BorderWidth = 0;
            parentSec.HeaderView.Layer.BorderColor = UIColor.Black.CGColor;

            if (Invalid)
            {
                parentSec.HeaderView.Layer.BorderWidth = 1;
                parentSec.HeaderView.Layer.BorderColor = UIColor.Red.CGColor;
            }

            Layer.BorderWidth = 0;

            if (!IsEnabled)
            {
                GridStyle = new DisabledGrid();
            }
            else
            {
                GridStyle = new EnabledGrid();
            }
        }
    }

    public class DisabledGrid : DataGridStyle
    {
        //public DisabledGrid() { }

        public override UIColor GetRecordBackgroundColor()
        {
            return UIColor.GroupTableViewBackgroundColor;
        }

        public override UIColor GetHeaderBackgroundColor()
        {
            return UIColor.GroupTableViewBackgroundColor;
        }
    }

    public class EnabledGrid : DataGridStyle
    {
        //public EnabledGrid() { }

        public override UIColor GetRecordBackgroundColor()
        {
            return UIColor.White;
        }

        public override UIColor GetHeaderBackgroundColor()
        {
            return base.GetHeaderBackgroundColor();
        }
    }

    public class GridCellTextViewRendererExt : GridCellTextViewRenderer
    {
        //public GridCellTextViewRendererExt() { }

        public async override void CommitCellValue(bool isNewValue)
        {
            var newValue = GetControlValue();
            var editingColumn = DataGrid.Columns[CurrentCellIndex.ColumnIndex];
            var columnName = editingColumn.MappingName.Substring(7, editingColumn.MappingName.Length - 8);
            var dataColumn = (CurrentCellElement as GridCell).DataColumn;
            (dataColumn.RowData as DynamicModel).Values[columnName] = newValue;

            await Task.Delay(10);

            UpdateCellValue(dataColumn);
            RefreshDisplayValue(dataColumn);
        }
    }





    //public class DynamicModel : INotifyPropertyChanged
    //{
    //	public Dictionary<string, object> _values;
    //	//public object _groupProperty;
    //	public event PropertyChangedEventHandler PropertyChanged;
    //	//public object GroupProperty
    //	//{
    //	//	get
    //	//	{
    //	//		return _groupProperty;
    //	//	}
    //	//	set
    //	//	{
    //	//		_groupProperty = value;
    //	//		if (PropertyChanged != null)
    //	//			PropertyChanged(this, new PropertyChangedEventArgs("GroupProperty"));
    //	//	}
    //	//}
    //	public Dictionary<string, object> Values
    //	{
    //		get
    //		{
    //			return _values;
    //		}
    //		set
    //		{
    //			_values = value;
    //			if (PropertyChanged != null)
    //				PropertyChanged(this, new PropertyChangedEventArgs("Values"));
    //		}
    //	}
    //	public DynamicModel()
    //	{
    //		this._values = new Dictionary<string, object>();
    //	}
    //	public void RefreshGroupProperty(string key)
    //	{
    //		object value = null;
    //		this.Values.TryGetValue(key, out value);
    //		//this.GroupProperty = value;
    //	}
    //}






    public class DynamicModel : INotifyPropertyChanged
    {
        public Dictionary<string, object> _values;
        public object _groupProperty;

        public event PropertyChangedEventHandler PropertyChanged;

        public object GroupProperty
        {
            get
            {
                return _groupProperty;
            }
            set
            {
                _groupProperty = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("GroupProperty"));
            }
        }

        public Dictionary<string, object> Values
        {
            get
            {
                return _values;
            }
            set
            {
                _values = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Values"));
            }

        }
        public DynamicModel()
        {
            _values = new Dictionary<string, object>();

        }

        public void RefreshGroupProperty(string key)
        {
            Values.TryGetValue(key, out object value);
            GroupProperty = value;
        }
    }




    public class ViewModel
    {
        public ObservableCollection<DynamicModel> DynamicCollection { get; set; }
        List<ItemColumn> columns;
        List<QuestionRowItem> rows;

        public ViewModel(List<ItemColumn> columns, List<QuestionRowItem> rows)
        {
            this.columns = columns;
            this.rows = rows;
            DynamicCollection = GetData(rows);
        }

        ObservableCollection<DynamicModel> GetData(List<QuestionRowItem> itemRows)
        {
            var data = new ObservableCollection<DynamicModel>();
            foreach (QuestionRowItem qri in itemRows)
            {
                var obj = GetDynamicModel(qri.ItemColumns);
                data.Add(obj);
            }
            return data;
        }

        public DynamicModel GetDynamicModel(List<ItemColumn> itemColumns)
        {
            var obj = new DynamicModel();
            foreach (ItemColumn ic in itemColumns)
            {
                switch (ic.Type)
                {
                    case "Text":
                        obj.Values[ic.Header.Replace(" ", "")] = ic.AnswerText;
                        break;
                    case "Switch":
                        obj.Values[ic.Header.Replace(" ", "")] = true;
                        break;
                    case "Numeric":
                        obj.Values[ic.Header.Replace(" ", "")] = ic.AnswerText;
                        break;
                    case "Date":
                        obj.Values[ic.Header.Replace(" ", "")] = ic.AnswerText;
                        break;
                    case "Picker":
                        obj.Values[ic.Header.Replace(" ", "")] = ic.AnswerText;
						break;
                    default: // do nothing;
                        break;
                }
            }

            return obj;
        }

        public void RefreshGroup(string key)
        {
            foreach (var dynamicItem in DynamicCollection)
            {
                dynamicItem.RefreshGroupProperty(key);
            }
        }
    }









    public class DynaAuto : SfAutoComplete
    {
        //public DynaAuto(AutoCompleteDelegate ddelegate)
        //{
        //    //this.Delegate = new Delegateclass();
        //    Delegate = ddelegate;
        //}

		//public DynaAuto(){}

        public bool IsEnabled;
        public string ConditionTriggerId;
        public string ActiveTriggerId = "";
        public List<QuestionOption> QuestionOptions;
        public List<QuestionOption> QuestionAnswers;
        public string QuestionId { get; set; }
        public string QuestionParentId { get; set; }
        public string QuestionText { get; set; }
        public string QuestionType { get; set; }
        public string QuestionKeyboardType { get; set; }
        public bool Answered { get; set; }
        public bool Disabled { get; set; }
        public string AnswerId { get; set; }
        public string AnswerText { get; set; }
        public string ParentConditionTriggerId { get; set; }
        public bool IsConditional { get; set; }
        public bool Required { get; set; }
        public bool Invalid { get; set; }
        public int MaxChars { get; set; }
        public UITextField EntryTextField { get; set; }
        public DynaSection parentSec { get; set; }
        public EventHandler EntryEnded { get; set; }
        //static NSString MyCellId = new NSString("MyCellId");

        public override void Draw(CGRect rect)
        {
            base.Draw(rect);

            UserInteractionEnabled = IsEnabled;

            //if (Invalid)
            //{
            //	this.Layer.BorderWidth = 1;
            //	this.Layer.BorderColor = UIColor.Red.CGColor;
            //}

            TextField.BorderStyle = UITextBorderStyle.None;

            parentSec.HeaderView.Layer.BorderWidth = 0;
            parentSec.HeaderView.Layer.BorderColor = UIColor.Black.CGColor;

            if (Invalid)
            {
                parentSec.HeaderView.Layer.BorderWidth = 1;
                parentSec.HeaderView.Layer.BorderColor = UIColor.Red.CGColor;
            }

            Layer.BorderWidth = 0;

            if (!IsEnabled)
            {
                TextColor = UIColor.LightGray;
                BorderColor = UIColor.GroupTableViewBackgroundColor;
                BackgroundColor = UIColor.GroupTableViewBackgroundColor;
                TextField.BackgroundColor = UIColor.GroupTableViewBackgroundColor;
                Watermark = (NSString)"Not applicable";
                TextField.TextColor = UIColor.LightGray;
                TextField.Placeholder = (NSString)"Not applicable";
                Layer.BorderColor = UIColor.GroupTableViewBackgroundColor.CGColor;
                Layer.BackgroundColor = UIColor.GroupTableViewBackgroundColor.CGColor;
            }
            else
            {
                TextColor = UIColor.Black;
                BorderColor = UIColor.White;
                BackgroundColor = UIColor.White;
                TextField.BackgroundColor = UIColor.White;
                Watermark = (NSString)"Enter your answer here";
                TextField.TextColor = UIColor.Black;
                TextField.Placeholder = (NSString)"Enter your answer here";
                Layer.BorderColor = UIColor.White.CGColor;
                Layer.BackgroundColor = UIColor.White.CGColor;
            }
        }
    }






    public class DynaSegmented : UISegmentedControl
    {

        public bool IsEnabled;
        public string ConditionTriggerId;
        public string ActiveTriggerId = "";
        public List<QuestionOption> QuestionOptions;
        public List<QuestionOption> QuestionAnswers;
        public string QuestionId { get; set; }
        public string QuestionParentId { get; set; }
        public string QuestionText { get; set; }
        public string QuestionType { get; set; }
        public string QuestionKeyboardType { get; set; }
        public bool Answered { get; set; }
        public bool Disabled { get; set; }
        public string AnswerId { get; set; }
        public string AnswerText { get; set; }
        public string ParentConditionTriggerId { get; set; }
        public bool IsConditional { get; set; }
        public bool Required { get; set; }
        public bool Invalid { get; set; }
        public int MaxChars { get; set; }
        public DynaSection parentSec { get; set; }
        public EventHandler EntryEnded { get; set; }
        //static NSString MyCellId = new NSString("MyCellId");

        public override void Draw(CGRect rect)
        {
            base.Draw(rect);

            UserInteractionEnabled = IsEnabled;

            //if (Invalid)
            //{
            //	this.Layer.BorderWidth = 1;
            //	this.Layer.BorderColor = UIColor.Red.CGColor;
            //}

            parentSec.HeaderView.Layer.BorderWidth = 0;
            parentSec.HeaderView.Layer.BorderColor = UIColor.Black.CGColor;

            if (Invalid)
            {
                parentSec.HeaderView.Layer.BorderWidth = 1;
                parentSec.HeaderView.Layer.BorderColor = UIColor.Red.CGColor;
            }
        }
    }









    public class DynaCheckBoxElement : CheckboxElement
    {
        public bool Enabled;
        public string ParentQuestionId { get; set; }
        public string OptionText { get; set; }
        public string OptionId { get; set; }
        public string ConditionTriggerId { get; set; }
        public bool Chosen { get; set; }
        public string QuestionId { get; set; }
        public bool Required { get; set; }
        public bool Invalid { get; set; }

        public DynaCheckBoxElement(string cCaption, bool cValue, string cGroup) : base(cCaption, cValue, cGroup) { }

        public override void Selected(DialogViewController dvc, UITableView tableView, NSIndexPath path)
        {
            base.Selected(dvc, tableView, path);
            var selected = OnSelected;
            if (selected != null)
                //base.GetActiveCell().Highlighted = true;
                //Value = true;
                selected(this, EventArgs.Empty);

            tableView.SelectRow(path, true, UITableViewScrollPosition.None);
        }

        public override void Deselected(DialogViewController dvc, UITableView tableView, NSIndexPath path)
        {
            base.Deselected(dvc, tableView, path);
            var deselected = OnDeselected;
            if (deselected != null)
                //base.GetActiveCell().Highlighted = false;	
                //Value = false;
                deselected(this, EventArgs.Empty);
        }

        public override UITableViewCell GetCell(UITableView tv)
        {
            var cell = base.GetCell(tv);
            cell.ContentView.AutosizesSubviews = false;
            cell.UserInteractionEnabled = Enabled;
            cell.BackgroundColor = UIColor.White;
            cell.TextLabel.TextColor = UIColor.Black;
            cell.TextLabel.LineBreakMode = UILineBreakMode.WordWrap;
            cell.TextLabel.Lines = 0;
            cell.SelectionStyle = UITableViewCellSelectionStyle.None;

            if (Caption.Length > 78)
            {
                var ww = (decimal)Caption.Length / 78;
                var wlines = (int)Math.Ceiling(ww);
                var cellAdjSize = Caption.StringSize(cell.TextLabel.Font, new CGSize(cell.Bounds.Width, 300), UILineBreakMode.WordWrap);
                cell.TextLabel.Lines = wlines;
                var bounds = cell.Bounds;
                bounds.Height = cellAdjSize.Height;
                cell.TextLabel.Bounds = bounds;
                cell.Bounds = bounds;
                cell.TextLabel.Frame = bounds;
                cell.Frame = bounds;
                cell.SizeToFit();
                cell.AutosizesSubviews = true;
                cell.TextLabel.SizeToFit();
                cell.TextLabel.AutosizesSubviews = true;

                var font = cell.TextLabel.Font;
                var size = cell.Frame.Size;

                for (var maxSize = cell.TextLabel.Font.PointSize; maxSize >= cell.TextLabel.MinimumScaleFactor * cell.TextLabel.Font.PointSize; maxSize -= 1f)
                {
                    font = font.WithSize(maxSize);
                    var constraintSize = new CGSize(size.Width, float.MaxValue);
                    var labelSize = (new NSString(cell.TextLabel.Text)).StringSize(font, constraintSize, UILineBreakMode.WordWrap);
                    if (labelSize.Height <= size.Height)
                    {
                        cell.TextLabel.Font = font;
                        cell.TextLabel.SetNeedsLayout();
                        break;
                    }
                }

                // set the font to the minimum size anyway
                cell.TextLabel.Font = font;
                cell.TextLabel.SetNeedsLayout();

                //var text = cell.TextLabel.Text;
                //var bounds = cell.Bounds;
                //var width = cell.TextLabel.Bounds.Width;
                //var height = text.StringSize(cell.TextLabel.Font);
                //var minHeight = string.Empty.StringSize(cell.TextLabel.Font);
                //var requiredLines = Math.Round(height.Height / minHeight.Height, MidpointRounding.AwayFromZero);
                //var supportedLines = Math.Round(bounds.Height / minHeight.Height, MidpointRounding.ToEven);
                //if (supportedLines != requiredLines)
                //{
                //	bounds.Height += (float)(minHeight.Height * (requiredLines - supportedLines));
                //	cell.TextLabel.Bounds = bounds;
                //	cell.Bounds = bounds;
                //	cell.TextLabel.Frame = bounds;
                //	cell.Frame = bounds;
                //	//this.Element.HeightRequest = bounds.Height;
                //}
                //var text = cell.TextLabel.Text;
                //var bounds = cell.Bounds;
                //var ass = cell.TextLabel.Bounds;
                //var width = cell.TextLabel.Bounds.Width;
                //var imageHeight = cell.ImageView.Bounds.Height;
                //var textHeight = text.StringSize(cell.TextLabel.Font, new CGSize(width, nfloat.MaxValue)).Height;
                //bounds.Height = (nfloat)Math.Max(imageHeight, textHeight);
                //cell.TextLabel.Bounds = bounds;
                //Element.HeightRequest = bounds.Height;

            }

            if (Value)
            {
                cell.BackgroundColor = UIColor.FromRGB(239, 246, 223);
            }

            if (!Enabled)
            {
                cell.TextLabel.TextColor = UIColor.LightGray;
                cell.BackgroundColor = UIColor.GroupTableViewBackgroundColor;
                cell.Accessory = UITableViewCellAccessory.None;
            }

            var parentSec = (DynaSection)Parent;
            parentSec.HeaderView.Layer.BorderWidth = 0;
            parentSec.HeaderView.Layer.BorderColor = UIColor.Clear.CGColor;

            if (Invalid)
            {
                parentSec.HeaderView.Layer.BorderWidth = 1;
                parentSec.HeaderView.Layer.BorderColor = UIColor.Red.CGColor;
            }

            return cell;
        }

        public event EventHandler<EventArgs> OnSelected;
        public event EventHandler<EventArgs> OnDeselected;
    }



    public class DynaMultiRootElement : RootElement
    {
        public bool Enabled;
        public string QuestionId { get; set; }
        RadioGroup _defaultGroup = new RadioGroup(0);
        Dictionary<string, RadioGroup> _groups = new Dictionary<string, RadioGroup>();

        public DynaMultiRootElement(string caption = "") : base(caption, new RadioGroup("default", -1)) { }

        public DynaMultiRootElement(string caption, Group group, Func<RootElement, UIViewController> createOnSelected) : base(caption, group)
        {
            var radioGroup = group as RadioGroup;
            if (radioGroup != null)
            {
                _groups.Add(radioGroup.Key.ToLower(), radioGroup);
            }
            this.createOnSelected = createOnSelected;
        }

        public override UITableViewCell GetCell(UITableView tv)
        {
            var cell = base.GetCell(tv);
            cell.SelectionStyle = UITableViewCellSelectionStyle.None;

            return cell;
        }

        public int SelectedGroup(string group)
        {
            if (string.IsNullOrEmpty(group))
            {
                throw new ArgumentNullException(nameof(group));
            }

            group = group.ToLower();
            if (_groups.ContainsKey(group))
            {
                return _groups[group].Selected;
            }

            return -1;
        }

        public void Select(string group, int selected)
        {
            if (string.IsNullOrEmpty(group))
            {
                throw new ArgumentNullException(nameof(group));
            }

            var radioGroup = GetGroup(group);
            radioGroup.Selected = selected;
        }


        //public override async void Selected(DialogViewController dvc, UITableView tableView, NSIndexPath path)
        //{
        //    var loadingOverlay = new LoadingOverlay(tableView.Bounds);
        //    dvc.Add(loadingOverlay);
        //    await Task.Delay(1);
        //    base.Selected(dvc, tableView, path);
        //    loadingOverlay.Hide();
        //    var selected = OnSelected;
        //    if (selected != null)
        //    {
        //        selected(this, EventArgs.Empty);
        //    }
        //}

        //static public event EventHandler<EventArgs> OnSelected;


        internal RadioGroup GetGroup(string group)
        {
            if (string.IsNullOrEmpty(group))
            {
                throw new ArgumentNullException(nameof(group));
            }

            group = group.ToLower();
            if (!_groups.ContainsKey(group))
            {
                _groups[group] = new RadioGroup(group, -1);
            }

            return _groups[group];
        }

        internal NSIndexPath PathForRadioElement(string group, int index)
        {

            foreach (var section in this)
            {
                foreach (var e in section.Elements)
                {
                    var re = e as DynaMultiRadioElement;
                    if (re != null
                        && string.Equals(re.Group, group, StringComparison.InvariantCultureIgnoreCase)
                        && re.Index == index)
                    {
                        return e.IndexPath;
                    }
                }
            }

            return null;
        }
    }



    public class DynaMultiRadioElement : RadioElement
    {
        public bool Enabled;
        public string ParentQuestionId { get; set; }
        public string OptionText { get; set; }
        public string OptionId { get; set; }
        public string ConditionTriggerId { get; set; }
        public bool Chosen { get; set; }
        public string QuestionId { get; set; }
        public bool Required { get; set; }
        public bool Invalid { get; set; }
        public event Action<DynaMultiRadioElement> OnDeselected;
        public event Action<DynaMultiRadioElement> ElementSelected;
        readonly static NSString ReuseId = new NSString("DynaMultiRadioElement");
        string _subtitle;
        public int? Index { get; protected set; }

        public DynaMultiRadioElement(string caption, string group = null, string subtitle = null) : base(caption, group)
        {
            _subtitle = subtitle;
        }

        protected override NSString CellKey
        {
            get
            {
                return ReuseId;
            }
        }

        public override UITableViewCell GetCell(UITableView tv)
        {
            EnsureIndex();
            //tv.CellLayoutMarginsFollowReadableWidth = false;

            var cell = tv.DequeueReusableCell(CellKey);
            if (cell == null)
            {
                cell = new UITableViewCell(UITableViewCellStyle.Subtitle, CellKey);
            }

            //cell.SetNeedsDisplay();
            //cell = new UITableViewCell(UITableViewCellStyle.Subtitle, CellKey);
            //cell.ApplyStyle(this);
            //this.GetContainerTableView().CellLayoutMarginsFollowReadableWidth = false;

            cell.ContentView.AutosizesSubviews = false;
            cell.TextLabel.Text = Caption;
            cell.TextLabel.LineBreakMode = UILineBreakMode.WordWrap;
            cell.TextLabel.Lines = 0;

            if (Caption.Length > 78)
            {
                var ww = (decimal)Caption.Length / 78;
                var wlines = (int)Math.Ceiling(ww);
                var cellAdjSize = Caption.StringSize(cell.TextLabel.Font, new CGSize(cell.Bounds.Width, 300), UILineBreakMode.WordWrap);
                cell.TextLabel.Lines = wlines;
                var bounds = cell.Bounds;
                bounds.Height = cellAdjSize.Height;
                cell.TextLabel.Bounds = bounds;
                cell.Bounds = bounds;
                cell.TextLabel.Frame = bounds;
                cell.Frame = bounds;
                cell.SizeToFit();
                cell.AutosizesSubviews = true;
                cell.TextLabel.SizeToFit();
                cell.TextLabel.AutosizesSubviews = true;

                var font = cell.TextLabel.Font;
                var size = cell.Frame.Size;

                for (var maxSize = cell.TextLabel.Font.PointSize; maxSize >= cell.TextLabel.MinimumScaleFactor * cell.TextLabel.Font.PointSize; maxSize -= 1f)
                {
                    font = font.WithSize(maxSize);
                    var constraintSize = new CGSize(size.Width, float.MaxValue);
                    var labelSize = (new NSString(cell.TextLabel.Text)).StringSize(font, constraintSize, UILineBreakMode.WordWrap);
                    if (labelSize.Height <= size.Height)
                    {
                        cell.TextLabel.Font = font;
                        cell.TextLabel.SetNeedsLayout();
                        break;
                    }
                }

                // set the font to the minimum size anyway
                cell.TextLabel.Font = font;
                cell.TextLabel.SetNeedsLayout();


                //var text = cell.TextLabel.Text;
                //var bounds = cell.Bounds;
                //var width = cell.TextLabel.Bounds.Width;
                //var height = text.StringSize(cell.TextLabel.Font);
                //var minHeight = string.Empty.StringSize(cell.TextLabel.Font);
                //var requiredLines = Math.Round(height.Height / minHeight.Height, MidpointRounding.AwayFromZero);
                //var supportedLines = Math.Round(bounds.Height / minHeight.Height, MidpointRounding.ToEven);
                //if (supportedLines != requiredLines)
                //{
                //	bounds.Height += (float)(minHeight.Height * (requiredLines - supportedLines));
                //	cell.TextLabel.Bounds = bounds;
                //	cell.Bounds = bounds;
                //	cell.TextLabel.Frame = bounds;
                //	cell.Frame = bounds;
                //	//this.Element.HeightRequest = bounds.Height;
                //}
                //var text = cell.TextLabel.Text;
                //var bounds = cell.Bounds;
                //var ass = cell.TextLabel.Bounds;
                //var width = cell.TextLabel.Bounds.Width;
                //var imageHeight = cell.ImageView.Bounds.Height;
                //var textHeight = text.StringSize(cell.TextLabel.Font, new CGSize(width, nfloat.MaxValue)).Height;
                //bounds.Height = (nfloat)Math.Max(imageHeight, textHeight);
                //cell.TextLabel.Bounds = bounds;
                //Element.HeightRequest = bounds.Height;

            }

            if (!string.IsNullOrEmpty(_subtitle))
            {
                cell.DetailTextLabel.Text = _subtitle;
            }

            var selected = false;
            var slRoot = Parent.Parent as DynaMultiRootElement;

            if (slRoot != null)
            {
                selected = Index == slRoot.SelectedGroup(Group);

            }
            else
            {
                var root = (RootElement)Parent.Parent;
                selected = Index == root.RadioSelected;
            }

            cell.Accessory = selected ? UITableViewCellAccessory.Checkmark : UITableViewCellAccessory.None;
            cell.Selected = selected;
            cell.UserInteractionEnabled = Enabled;
            cell.TextLabel.TextColor = UIColor.Black;
            cell.BackgroundColor = UIColor.White;
            cell.SelectionStyle = UITableViewCellSelectionStyle.None;

            if (selected)
            {
                cell.BackgroundColor = UIColor.FromRGB(239, 246, 223);
            }

            if (!Enabled)
            {
                cell.TextLabel.TextColor = UIColor.LightGray;
                cell.BackgroundColor = UIColor.GroupTableViewBackgroundColor;
                if (selected)
                {
                    cell.Accessory = UITableViewCellAccessory.None;
                }
            }

            var parentSec = (DynaSection)Parent;
            parentSec.HeaderView.Layer.BorderWidth = 0;
            parentSec.HeaderView.Layer.BorderColor = UIColor.Clear.CGColor;

            if (Invalid)
            {
                parentSec.HeaderView.Layer.BorderWidth = 1;
                parentSec.HeaderView.Layer.BorderColor = UIColor.Red.CGColor;
            }

            return cell;
        }


        public override void Selected(DialogViewController dvc, UITableView tableView, NSIndexPath indexPath)
        {
            //var loadingOverlay = new LoadingOverlay(tableView.Bounds);
            //dvc.Add(loadingOverlay);
            //await Task.Delay(1);

            var slRoot = Parent.Parent as DynaMultiRootElement;

            if (slRoot != null)
            {
                var radioGroup = slRoot.GetGroup(Group);

                UITableViewCell cell;

                if (radioGroup.Selected == Index)
                {
                    var deSelectedIndex = slRoot.PathForRadioElement(Group, radioGroup.Selected);
                    if (deSelectedIndex != null)
                    {
                        cell = tableView.CellAt(deSelectedIndex);
                        if (cell != null)
                        {
                            cell.Selected = false;
                            cell.BackgroundColor = UIColor.White;
                            cell.Accessory = UITableViewCellAccessory.None;
                        }
                    }

                    var unhandler = OnDeselected;
                    if (unhandler != null)
                    {
                        unhandler(this);
                    }

                    radioGroup.Selected = -1;
                    slRoot.Deselected(dvc, tableView, indexPath);

                    return;
                }

                var selectedIndex = slRoot.PathForRadioElement(Group, radioGroup.Selected);
                if (selectedIndex != null)
                {
                    cell = tableView.CellAt(selectedIndex);
                    if (cell != null)
                    {
                        cell.Accessory = UITableViewCellAccessory.None;
                    }
                }

                cell = tableView.CellAt(indexPath);
                if (cell != null)
                {
                    cell.Accessory = UITableViewCellAccessory.Checkmark;
                }

                radioGroup.Selected = Index.Value;

                var handler = ElementSelected;
                if (handler != null)
                {
                    handler(this);
                }
            }
            else
            {
                base.Selected(dvc, tableView, indexPath);
            }

            //loadingOverlay.Hide();
        }

        public override void Deselected(DialogViewController dvc, UITableView tableView, NSIndexPath path)
        {
            base.Deselected(dvc, tableView, path);
            var deselected = OnDeselected;
            if (deselected != null)
                //base.GetActiveCell().Highlighted = false;	
                //Value = false;
                deselected(this);
        }

        void EnsureIndex()
        {
            if (!Index.HasValue)
            {
                var parent = Parent as Section;

                Index = parent.Elements.IndexOf(this);
            }
        }
    }




    public class NullableDateElementInline : StringElement
    {
        public bool Enabled;
        public string ConditionTriggerId;
        public string ActiveTriggerId = "";
        public List<QuestionOption> QuestionOptions;
        public List<QuestionOption> QuestionAnswers;
        public string QuestionId { get; set; }
        public string QuestionParentId { get; set; }
        public string QuestionText { get; set; }
        public string QuestionType { get; set; }
        public string QuestionKeyboardType { get; set; }
        public bool Answered { get; set; }
        public bool Disabled { get; set; }
        public string AnswerId { get; set; }
        public string AnswerText { get; set; }
        public string ParentConditionTriggerId { get; set; }
        public bool IsConditional { get; set; }
        public bool Required { get; set; }
        public bool Invalid { get; set; }
        static NSString skey = new NSString("NullableDateTimeElementInline");
        public DateTime? DateValue;
        public event NSAction DateSelected;
        public event NSAction PickerClosed;
        public event NSAction PickerOpened;
        InlineDateElement _inline_date_element;
        bool _picker_present;
        //InlineDateElement _inline_date_element = null;
        //private bool _picker_present = false;

        public NullableDateElementInline(string caption, DateTime? date) : base(caption)
        {
            DateValue = date;
            Value = FormatDate(date);
            AnswerText = Value;
        }

        /// <summary>
        /// Returns true iff the picker is currently open
        /// </summary>
        /// <returns></returns>
        public bool IsPickerOpen()
        {
            return _picker_present;
        }

        protected internal NSDateFormatter fmt = new NSDateFormatter
        {
            DateStyle = NSDateFormatterStyle.Medium
        };

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                if (fmt != null)
                {
                    fmt.Dispose();
                    fmt = null;
                }
            }
        }

        public virtual string FormatDate(DateTime? dt)
        {
            if (!dt.HasValue)
                return " ";

            dt = GetDateWithKind(dt);
            return fmt.ToString((NSDate)dt);
        }

        protected DateTime? GetDateWithKind(DateTime? dt)
        {
            if (!dt.HasValue)
                return dt;

            if (dt.Value.Kind == DateTimeKind.Unspecified)
                return DateTime.SpecifyKind(dt.Value, DateTimeKind.Local);

            return dt;
        }

        public void ClosePickerIfOpen(DialogViewController dvc)
        {
            if (_picker_present)
            {
                var index_path = IndexPath;
                var table_view = GetContainerTableView();
                Selected(dvc, table_view, index_path);
            }
        }

        public void SetDate(DateTime? date)
        {
            DateValue = date;
            AnswerText = date.ToString();
            Value = FormatDate(date);
            var r = GetImmediateRootElement();
            r.Reload(this, UITableViewRowAnimation.None);
        }

        public override void Selected(DialogViewController dvc, UITableView tableView, NSIndexPath indexPath)
        {
            TogglePicker(dvc, tableView, indexPath);
            // Deselect the row so the row highlint tint fades away.
            tableView.DeselectRow(indexPath, true);
        }

        /// <summary>
        /// Shows or hides the nullable picker
        /// </summary>
        /// <param name="dvc"></param>
        /// <param name="tableView"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        void TogglePicker(DialogViewController dvc, UITableView tableView, NSIndexPath path)
        {
            var sectionAndIndex = GetMySectionAndIndex(dvc);
            if (sectionAndIndex.Key != null)
            {
                Section section = sectionAndIndex.Key;
                int index = sectionAndIndex.Value;

                var cell = tableView.CellAt(path);

                if (_picker_present)
                {
                    // Remove the picker.
                    cell.DetailTextLabel.TextColor = UIColor.Black;
                    section.Remove(_inline_date_element);
                    _picker_present = false;
                    if (PickerClosed != null)
                        PickerClosed();
                }
                else
                {
                    // Show the picker.
                    cell.DetailTextLabel.TextColor = UIColor.Red;
                    _inline_date_element = new InlineDateElement(DateValue);

                    _inline_date_element.DateSelected += (DateTime? date) =>
                    {
                        DateValue = date;
                        AnswerText = FormatDate(date);
                        cell.DetailTextLabel.Text = FormatDate(date);
                        Value = cell.DetailTextLabel.Text;
                        if (DateSelected != null)       // Fire our changed event.
                            DateSelected();
                    };

                    _inline_date_element.ClearPressed += () =>
                    {
                        DateTime? null_date = null;
                        DateValue = null_date;
                        AnswerText = "";
                        cell.DetailTextLabel.Text = " ";
                        Value = cell.DetailTextLabel.Text;
                        cell.DetailTextLabel.TextColor = UIColor.Black;
                        section.Remove(_inline_date_element);
                        _picker_present = false;
                        if (PickerClosed != null)
                            PickerClosed();

                        Invalid |= Required;
                    };

                    _inline_date_element.ClosePressed += () =>
                    {
                        TogglePicker(dvc, tableView, path);
                    };

                    section.Insert(index + 1, UITableViewRowAnimation.Bottom, _inline_date_element);
                    _picker_present = true;
                    tableView.ScrollToRow(_inline_date_element.IndexPath, UITableViewScrollPosition.None, true);

                    if (PickerOpened != null)
                        PickerOpened();
                }
            }
        }

        /// <summary>
        /// Locates this instance of this Element within a given DialogViewController.
        /// </summary>
        /// <returns>The Section instance and the index within that Section of this instance.</returns>
        /// <param name="dvc">Dvc.</param>
        KeyValuePair<Section, int> GetMySectionAndIndex(DialogViewController dvc)
        {
            foreach (var section in dvc.Root)
            {
                for (int i = 0; i < section.Count; i++)
                {
                    if (section[i] == this)
                    {
                        return new KeyValuePair<Section, int>(section, i);
                    }
                }
            }
            return new KeyValuePair<Section, int>();
        }

        public override UITableViewCell GetCell(UITableView tv)
        {
            var cell = base.GetCell(tv);
            cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;
            cell.DetailTextLabel.Font = UIFont.SystemFontOfSize(14);
            cell.DetailTextLabel.TextAlignment = UITextAlignment.Left;
            cell.UserInteractionEnabled = Enabled;
            cell.DetailTextLabel.TextColor = UIColor.Black;
            cell.TextLabel.TextColor = UIColor.FromRGB(200, 200, 205);
            cell.BackgroundColor = UIColor.White;

            if (!Enabled)
            {
                cell.TextLabel.TextColor = UIColor.LightGray;
                cell.BackgroundColor = UIColor.GroupTableViewBackgroundColor;
            }

            var parentSec = (DynaSection)Parent;
            parentSec.HeaderView.Layer.BorderWidth = 0;
            parentSec.HeaderView.Layer.BorderColor = UIColor.Clear.CGColor;

            if (Invalid)
            {
                parentSec.HeaderView.Layer.BorderWidth = 1;
                parentSec.HeaderView.Layer.BorderColor = UIColor.Red.CGColor;
            }

            return cell;
        }

        /// <summary>
        /// Class that has the UIDatePicker and a button for clearing/cancelling
        /// </summary>
        public class InlineDateElement : Element, IElementSizing
        {
            public UIDatePicker _date_picker;
            UIButton _clear_cancel_button;
            UIButton _close_cancel_button;
            static NSString iskey = new NSString("InlineDateElement");
            public event Action<DateTime?> DateSelected;
            public event Action ClearPressed;
            public event Action ClosePressed;
            DateTime? _current_date;
            SizeF _picker_size;
            readonly SizeF _cell_size;

            public InlineDateElement(DateTime? current_date) : base("")
            {
                _current_date = current_date;
                _date_picker = new UIDatePicker { Mode = UIDatePickerMode.Date };
                _picker_size = (SizeF)_date_picker.SizeThatFits(SizeF.Empty);
                _cell_size = _picker_size;
                _cell_size.Height += 30f; // Add a little bit for the clear button
            }

            /// <summary>
            /// Returns the cell, with some additions
            /// </summary>
            /// <param name="tv"></param>
            /// <returns></returns>
            public override UITableViewCell GetCell(UITableView tv)
            {
                Debug.Assert(_date_picker != null);

                var cell = base.GetCell(tv);

                if (!_current_date.HasValue && DateSelected != null)
                    DateSelected(DateTime.Now);
                else if (_current_date.HasValue)
                    _date_picker.Date = ((DateTime)_current_date).ToNSDate();

                _date_picker.ValueChanged += (object sender, EventArgs e) =>
                {
                    if (DateSelected != null)
                        DateSelected((DateTime?)_date_picker.Date);
                };

                if (_close_cancel_button == null)
                {
                    _close_cancel_button = UIButton.FromType(UIButtonType.RoundedRect);
                    _close_cancel_button.SetTitle("Close", UIControlState.Normal);
                }

                _close_cancel_button.Frame = new RectangleF((float)(tv.Frame.Width / 2 - 20f), _cell_size.Height - 40f, 40f, 40f);

                //if (_clear_cancel_button == null)
                //{
                //	_clear_cancel_button = UIButton.FromType(UIButtonType.RoundedRect);
                //	_clear_cancel_button.SetTitle("Clear", UIControlState.Normal);
                //}
                ////_clear_cancel_button.Frame = new RectangleF((float)(tv.Frame.Width / 2 - 20f), _cell_size.Height - 40f, 40f, 40f);
                //_clear_cancel_button.Frame = new RectangleF((float)(tv.Frame.Width / 2 - 40f), _cell_size.Height - 40f, 40f, 40f);

                _date_picker.Frame = new RectangleF((float)(tv.Frame.Width / 2 - _picker_size.Width / 2), _cell_size.Height / 2 - _picker_size.Height / 2, _picker_size.Width, _picker_size.Height);

                //_clear_cancel_button.TouchUpInside += (object sender, EventArgs e) =>
                //{
                //	// Clear button pressed. 
                //	if (ClearPressed != null)
                //		ClearPressed();
                //};

                _close_cancel_button.TouchUpInside += (object sender, EventArgs e) =>
                {
                    // Clear button pressed. 
                    if (ClosePressed != null)
                        ClosePressed();
                };

                cell.AddSubview(_date_picker);
                cell.AddSubview(_close_cancel_button);

                return cell;
            }

            /// <summary>
            /// Returns the height of the cell
            /// </summary>
            /// <param name="tableView"></param>
            /// <param name="indexPath"></param>
            /// <returns></returns>
            public nfloat GetHeight(UITableView tableView, NSIndexPath indexPath)
            {
                return _cell_size.Height;
            }
        }
    }

    public static class NSDateExtensions
    {
        static readonly DateTime reference = new DateTime(2001, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

        public static DateTime ToDateTime(this NSDate date)
        {
            var utcDateTime = reference.AddSeconds(date.SecondsSinceReferenceDate);
            var dateTime = utcDateTime.ToLocalTime();
            return dateTime;
        }

        public static NSDate ToNSDate(this DateTime datetime)
        {
            var utcDateTime = datetime.ToUniversalTime();
            var date = NSDate.FromTimeIntervalSinceReferenceDate((utcDateTime - reference).TotalSeconds);
            return date;
        }
    }





    public class DynaSlider : Element
    {
        public bool Enabled;
        public string ConditionTriggerId;
        public string ActiveTriggerId = "";
        public List<QuestionOption> QuestionOptions;
        public List<QuestionOption> QuestionAnswers;
        public string QuestionId { get; set; }
        public string QuestionParentId { get; set; }
        public string QuestionText { get; set; }
        public string QuestionType { get; set; }
        public string QuestionKeyboardType { get; set; }
        public bool Answered { get; set; }
        public bool Disabled { get; set; }
        public string AnswerId { get; set; }
        public string AnswerText { get; set; }
        public string ParentConditionTriggerId { get; set; }
        public bool IsConditional { get; set; }
        public bool Required { get; set; }
        public bool Invalid { get; set; }
        public SectionQuestion SQuestion;
        public bool ShowCaption;
        public float Value;
        public float MinValue, MaxValue;
        public float Increment;
        static NSString skey = new NSString("DynaSlider");
        UISlider slider;
        Action<int> _valueChangedCallback;
        //UIImage Left, Right;

        public DynaSlider(float value, SectionQuestion sQuestion, Action<int> valueChanged = null) : this(null, null, value, sQuestion)
        {
            Value = value;
            SQuestion = sQuestion;
            _valueChangedCallback = valueChanged;
        }

        public DynaSlider(UIImage left, UIImage right, float value, SectionQuestion sQuestion, Action<int> valueChanged = null) : base(null)
        {
            //Left = left;
            //Right = right;
            MinValue = 0;
            MaxValue = 10;
            Increment = 1;
            Value = value;
            SQuestion = sQuestion;
            _valueChangedCallback = valueChanged;
        }

        protected override NSString CellKey
        {
            get
            {
                return skey;
            }
        }

        public Func<object> ValueChanged { get; internal set; }

        public override UITableViewCell GetCell(UITableView tv)
        {
            var cell = tv.DequeueReusableCell(CellKey);
            if (cell == null)
            {
                cell = new UITableViewCell(UITableViewCellStyle.Default, CellKey);
                cell.SelectionStyle = UITableViewCellSelectionStyle.None;
            }
            else
                RemoveTag(cell, 1);

            var captionSize = new CGSize(0, 0);
            if (Caption != null && ShowCaption)
            {
                cell.TextLabel.Text = Caption;
                captionSize = Caption.StringSize(UIFont.FromName(cell.TextLabel.Font.Name, UIFont.LabelFontSize));
                captionSize.Width += 10; // Spacing
            }
            if (slider == null)
            {
                slider = new UISlider(new CGRect(10f + captionSize.Width, UIDevice.CurrentDevice.CheckSystemVersion(7, 0) ? 18f : 12f, cell.ContentView.Bounds.Width - 20 - captionSize.Width, 7f))
                {

                    BackgroundColor = UIColor.Clear,
                    MinValue = MinValue,
                    MaxValue = MaxValue,
                    Continuous = false,
                    Value = Value,
                    Tag = 1,
                    AutoresizingMask = UIViewAutoresizing.FlexibleWidth
                };
                slider.ValueChanged += delegate
                {
                    // Get the closet "step" 
                    var nextStep = (float)Math.Round(slider.Value / Increment);
                    // Convert that step to a value used by the slider
                    slider.Value = (nextStep * Increment);
                    Value = (int)slider.Value;
                    SQuestion.AnswerText = Value.ToString();
                    AnswerText = Value.ToString();
                    Caption = Value.ToString();
                    captionSize = Caption.StringSize(UIFont.FromName(cell.TextLabel.Font.Name, UIFont.LabelFontSize));
                    captionSize.Width += 10; // Spacing
                    cell.TextLabel.Text = Caption;
                    slider.Frame = new CGRect(10f + captionSize.Width, UIDevice.CurrentDevice.CheckSystemVersion(7, 0) ? 18f : 12f, cell.ContentView.Bounds.Width - 20 - captionSize.Width, 7f);
                    if (_valueChangedCallback != null)
                        _valueChangedCallback((int)Value);

                    if (Required && Enabled)
                    {
                        if (string.IsNullOrEmpty(AnswerText))
                        {
                            Invalid = true;
                        }
                        else Invalid = false;
                    }
                };
            }
            else
            {
                slider.Value = Value;
            }

            cell.ContentView.AddSubview(slider);
            cell.UserInteractionEnabled = Enabled;
            cell.TextLabel.TextColor = UIColor.Black;
            cell.BackgroundColor = UIColor.White;

            if (!Enabled)
            {
                cell.TextLabel.TextColor = UIColor.LightGray;
                cell.BackgroundColor = UIColor.GroupTableViewBackgroundColor;
            }

            var parentSec = (DynaSection)Parent;
            parentSec.HeaderView.Layer.BorderWidth = 0;
            parentSec.HeaderView.Layer.BorderColor = UIColor.Clear.CGColor;

            if (Invalid)
            {
                parentSec.HeaderView.Layer.BorderWidth = 1;
                parentSec.HeaderView.Layer.BorderColor = UIColor.Red.CGColor;
            }

            return cell;
        }

        public override string Summary()
        {
            return Value.ToString();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (slider != null)
                {
                    slider.Dispose();
                    slider = null;
                }
            }
        }
    }



    public class LoadingOverlay : UIView
    {
        UIActivityIndicatorView activitySpinner;
        UILabel loadingLabel;
        UIView loadingBox;

        // control declarations
        //UIButton cancelButton;
        //public bool showCancelButton;
        //System.Threading.CancellationTokenSource cts;
        //public string loadingLabelText { get; set; }// = "Loading Data...";
        //public bool showLoadingBox;

        public LoadingOverlay(CGRect frame, bool showLoadingBox = false, bool MasterLoadingBox = false) : base(frame)
        {
            // configurable bits
            BackgroundColor = UIColor.Black;
            Alpha = 0.75f;
            AutoresizingMask = UIViewAutoresizing.All;

            nfloat labelHeight = 22;
            nfloat labelWidth = Frame.Width - 20;

            // derive the center x and y
            nfloat centerX = Frame.Width / 2;
            nfloat centerY = Frame.Height / 2;

            // create the activity spinner, center it horizontall and put it 5 points above center x
            activitySpinner = new UIActivityIndicatorView(UIActivityIndicatorViewStyle.WhiteLarge) { AutoresizingMask = UIViewAutoresizing.All };
            activitySpinner.Frame = new CGRect(
                centerX - (activitySpinner.Frame.Width / 2),
                centerY - activitySpinner.Frame.Height - 20,
                activitySpinner.Frame.Width,
                activitySpinner.Frame.Height);

            //AddSubview(activitySpinner);
            //activitySpinner.StartAnimating();

            // create and configure the "Loading Data" label
            loadingLabel = new UILabel(new CGRect(
                centerX - (labelWidth / 2),
                centerY + 5,
                labelWidth,
                labelHeight
                ))
            {
                BackgroundColor = UIColor.Clear,
                TextColor = UIColor.White,
                Text = "Loading...",
                //loadingLabel.Text = string.IsNullOrEmpty(loadingLabelText) ? "Loading Data..." : loadingLabelText;
                TextAlignment = UITextAlignment.Center,
                AutoresizingMask = UIViewAutoresizing.All
            };

            if (showLoadingBox)
            {
                int boxwidth;
                int boxheight;

                if (MasterLoadingBox)
                {
                    boxwidth = 150;
                    boxheight = 130;
                }
                else
                {
                    boxwidth = 250;
                    boxheight = 170;
                }

                loadingBox = new UIView(new CGRect(centerX - (boxwidth / 2), centerY - (boxheight / 2), boxwidth, boxheight))
                {
                    BackgroundColor = UIColor.DarkGray,
                    Alpha = 1
                };
                loadingBox.Layer.CornerRadius = 40;

                AddSubview(loadingBox);
            }

            AddSubview(activitySpinner);
            AddSubview(loadingLabel);

            activitySpinner.StartAnimating();
        }

        /// <summary>
        /// Fades out the control and then removes it from the super view
        /// </summary>
        public void Hide()
        {
            Animate(
                0.5, // duration
                () => { Alpha = 0; },
                () => { RemoveFromSuperview(); }
            );
        }

        public void SetText(string lbltext, bool twolines = false, string secondline = null)
        {
            loadingLabel.Text = lbltext;
            if (twolines)
            {
                loadingBox.Frame = new CGRect(loadingBox.Frame.X - 50, loadingBox.Frame.Y, loadingBox.Frame.Width + 100, loadingBox.Frame.Height + 27);

                loadingLabel.Text = lbltext + "\r\n" + secondline;
                loadingLabel.Frame = new CGRect(
                    loadingLabel.Frame.X,
                    loadingLabel.Frame.Y,
                    loadingLabel.Frame.Width,
                    loadingLabel.Frame.Height + 50
                );
                loadingLabel.Lines = 0;
                loadingLabel.LineBreakMode = UILineBreakMode.WordWrap;
            }
        }
    }






    //[Register("DynaPadView")]
    //[DesignTimeVisible(true)]
    public class DynaPadView : UIView
    {
        const int ThinPad = 3;
        const int ThickPad = 10;
        const int LineHeight = 1;
        UIImageView imageView;
        UIBezierPath currentPath;
        List<UIBezierPath> paths;
        List<CGPoint> currentPoints;
        List<CGPoint[]> points;
        //Used to determine rectangle that needs to be redrawn.
        nfloat minX, minY, maxX, maxY;

        //Create an array containing all of the points used to draw the signature.  Uses CGPoint.Empty
        //to indicate a new line.
        public CGPoint[] Points
        {
            get
            {
                if (points == null || !points.Any())
                    return new CGPoint[0];

                IEnumerable<CGPoint> pointsList = points[0];

                for (var i = 1; i < points.Count; i++)
                {
                    pointsList = pointsList.Concat(new[] { CGPoint.Empty });
                    pointsList = pointsList.Concat(points[i]);
                }

                return pointsList.ToArray();
            }
        }

        public bool IsBlank
        {
            get { return points == null || !points.Any() || !(points.Any(p => p.Any())); }
        }

        UIColor strokeColor;
        [Export("StrokeColor"), Browsable(true)]
        public UIColor StrokeColor
        {
            get { return strokeColor; }
            set
            {
                strokeColor = value ?? strokeColor;
                if (!IsBlank)
                    imageView.Image = GetImage(false);
            }
        }

        float strokeWidth;
        [Export("StrokeWidth"), Browsable(true)]
        public float StrokeWidth
        {
            get { return strokeWidth; }
            set
            {
                strokeWidth = value;
                if (!IsBlank)
                    imageView.Image = GetImage(false);
            }
        }

        /// <summary>
        /// The prompt displayed at the beginning of the signature line.
        /// </summary>
        /// <remarks>
        /// Text value defaults to 'X'.
        /// </remarks>
        /// <value>The signature prompt.</value>
        public UILabel SignaturePrompt
        {
            get;
            private set;
        }

        /// <summary>
        /// The caption displayed under the signature line.
        /// </summary>
        /// <remarks>
        /// Text value defaults to 'Sign here.'
        /// </remarks>
        /// <value>The caption.</value>
        public UILabel Caption
        {
            get;
            private set;
        }

        /// <summary>
        /// The color of the signature line.
        /// </summary>
        /// <value>The color of the signature line.</value>
        [Export("SignatureLineColor"), Browsable(true)]
        public UIColor SignatureLineColor
        {
            get { return SignatureLine.BackgroundColor; }
            set { SignatureLine.BackgroundColor = value; }
        }

        /// <summary>
        ///  An image view that may be used as a watermark or as a texture
        ///  for the signature pad.
        /// </summary>
        /// <value>The background image view.</value>
        public UIImageView BackgroundImageView { get; private set; }

        /// <summary>
        ///  An image view that may be used as a watermark or as a texture
        ///  for the signature pad.
        /// </summary>
        /// <value>The background image.</value>
        [Export("BackgroundImage"), Browsable(true)]
        public UIImage BackgroundImage
        {
            get { return BackgroundImageView.Image; }
            set { BackgroundImageView.Image = value; }
        }

        /// <summary>
        ///  An image view that may be used as a watermark or as a texture
        ///  for the signature pad.
        /// </summary>
        /// <value>The background image.</value>
        [Export("BackgroundImageContentMode"), Browsable(true)]
        public UIViewContentMode BackgroundImageContentMode
        {
            get { return BackgroundImageView.ContentMode; }
            set { BackgroundImageView.ContentMode = value; }
        }

        /// <summary>
        ///  The transparency of the watermark.
        /// </summary>
        /// <value>The background image.</value>
        [Export("BackgroundImageAlpha"), Browsable(true)]
        public nfloat BackgroundImageAlpha
        {
            get { return BackgroundImageView.Alpha; }
            set { BackgroundImageView.Alpha = value; }
        }

        /// <summary>
        /// The text for the prompt displayed at the beginning of the signature line.
        /// </summary>
        /// <remarks>
        /// Text value defaults to 'X'.
        /// </remarks>
        /// <value>The signature prompt.</value>
        [Export("SignaturePromptText"), Browsable(true)]
        public string SignaturePromptText
        {
            get { return SignaturePrompt.Text; }
            set
            {
                SignaturePrompt.Text = value;
                SetNeedsLayout();
            }
        }

        /// <summary>
        /// The text for the caption displayed under the signature line.
        /// </summary>
        /// <remarks>
        /// Text value defaults to 'Sign here.'
        /// </remarks>
        /// <value>The caption.</value>
        [Export("CaptionText"), Browsable(true)]
        public string CaptionText
        {
            get { return Caption.Text; }
            set { Caption.Text = value; }
        }

        /// <summary>
        /// Gets the text for the label that clears the pad when clicked.
        /// </summary>
        /// <value>The clear label.</value>
        [Export("ClearLabelText"), Browsable(true)]
        public string ClearLabelText
        {
            get { return ClearLabel.Title(UIControlState.Normal); }
            set
            {
                ClearLabel.SetTitle(value, UIControlState.Normal);
                SetNeedsLayout();
            }
        }

        /// <summary>
        /// Gets the label that clears the pad when clicked.
        /// </summary>
        /// <value>The clear label.</value>
        public UIButton ClearLabel
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the horizontal line that goes in the lower part of the pad.
        /// </summary>
        /// <value>The signature line.</value>
        public UIView SignatureLine
        {
            get;
            private set;
        }

        public DynaPadView()
        {
            Initialize();
        }

        public DynaPadView(NSCoder coder) : base(coder)
        {
            Initialize(/* ? baseProperties: false ? */);
        }

        public DynaPadView(IntPtr ptr) : base(ptr)
        {
            Initialize(false);
        }

        public DynaPadView(CGRect frame)
        {
            Frame = frame;
            Initialize();
        }

        void Initialize(bool baseProperties = true)
        {
            if (baseProperties)
            {
                BackgroundColor = UIColor.FromRGB(225, 225, 225);
            }
            strokeColor = UIColor.Black;
            StrokeWidth = 2f;

            Layer.ShadowColor = UIColor.Black.CGColor;
            Layer.ShadowOffset = new CGSize(2, 2);
            Layer.ShadowOpacity = 1f;
            Layer.ShadowRadius = 2f;

            #region Add Subviews
            BackgroundImageView = new UIImageView();
            AddSubview(BackgroundImageView);

            //Add an image that covers the entire signature view, used to display already drawn
            //elements instead of having to redraw them every time the user touches the screen.
            imageView = new UIImageView();
            AddSubview(imageView);

            Caption = new UILabel
            {
                Text = "Sign here.",
                Font = UIFont.BoldSystemFontOfSize(11f),
                BackgroundColor = UIColor.Clear,
                TextColor = UIColor.Gray,
                TextAlignment = UITextAlignment.Center
            };
            AddSubview(Caption);

            //Display the base line for the user to sign on.
            SignatureLine = new UIView { BackgroundColor = UIColor.Gray };

            AddSubview(SignatureLine);

            //Display the X on the left hand side of the line where the user signs.
            SignaturePrompt = new UILabel
            {
                Text = "X",
                Font = UIFont.BoldSystemFontOfSize(20f),
                BackgroundColor = UIColor.Clear,
                TextColor = UIColor.Gray
            };

            AddSubview(SignaturePrompt);

            ClearLabel = UIButton.FromType(UIButtonType.Custom);
            ClearLabel.SetTitle("Clear", UIControlState.Normal);
            ClearLabel.TitleLabel.Font = UIFont.BoldSystemFontOfSize(11f);
            ClearLabel.BackgroundColor = UIColor.Clear;
            ClearLabel.Hidden = true;
            ClearLabel.SetTitleColor(UIColor.Gray, UIControlState.Normal);
            //btn_clear.SetBackgroundImage (UIImage.FromFile ("Images/closebox.png"), UIControlState.Normal);
            //btn_clear.SetBackgroundImage (UIImage.FromFile ("Images/closebox_pressed.png"), 
            //                             UIControlState.Selected);
            ClearLabel.TouchUpInside += (sender, e) =>
            {
                Clear();
            };

            AddSubview(ClearLabel);
            #endregion

            paths = new List<UIBezierPath>();
            points = new List<CGPoint[]>();
            currentPoints = new List<CGPoint>();
        }

        //Delete the current signature.
        public void Clear()
        {
            paths = new List<UIBezierPath>();
            currentPath = UIBezierPath.Create();
            points = new List<CGPoint[]>();
            currentPoints.Clear();
            imageView.Image = null;
            ClearLabel.Hidden = true;

            SetNeedsDisplay();
        }

        //Create a UIImage of the currently drawn signature with default colors.
        public UIImage GetImage(bool shouldCrop = true, bool keepAspectRatio = true)
        {
            return GetImage(strokeColor, UIColor.Clear,
                             getSizeFromScale(UIScreen.MainScreen.Scale, Bounds),
                             UIScreen.MainScreen.Scale, shouldCrop, keepAspectRatio);
        }

        public UIImage GetImage(CGSize size, bool shouldCrop = true, bool keepAspectRatio = true)
        {
            return GetImage(strokeColor, UIColor.Clear, size, getScaleFromSize(size, Bounds), shouldCrop, keepAspectRatio);
        }

        public UIImage GetImage(nfloat scale, bool shouldCrop = true, bool keepAspectRatio = true)
        {
            return GetImage(strokeColor, UIColor.Clear, getSizeFromScale(scale, Bounds), scale, shouldCrop, keepAspectRatio);
        }

        //Create a UIImage of the currently drawn signature with the specified Stroke color.
        public UIImage GetImage(UIColor strokeColor, bool shouldCrop = true, bool keepAspectRatio = true)
        {
            return GetImage(strokeColor, UIColor.Clear,
                             getSizeFromScale(UIScreen.MainScreen.Scale, Bounds),
                             UIScreen.MainScreen.Scale, shouldCrop, keepAspectRatio);
        }

        public UIImage GetImage(UIColor strokeColor, CGSize size, bool shouldCrop = true, bool keepAspectRatio = true)
        {
            return GetImage(strokeColor, UIColor.Clear, size, getScaleFromSize(size, Bounds), shouldCrop, keepAspectRatio);
        }

        public UIImage GetImage(UIColor strokeColor, nfloat scale, bool shouldCrop = true, bool keepAspectRatio = true)
        {
            return GetImage(strokeColor, UIColor.Clear, getSizeFromScale(scale, Bounds), scale, shouldCrop, keepAspectRatio);
        }

        //Create a UIImage of the currently drawn signature with the specified Stroke and Fill colors.
        public UIImage GetImage(UIColor strokeColor, UIColor fillColor, bool shouldCrop = true, bool keepAspectRatio = true)
        {
            return GetImage(strokeColor, fillColor,
                             getSizeFromScale(UIScreen.MainScreen.Scale, Bounds),
                             UIScreen.MainScreen.Scale, shouldCrop, keepAspectRatio);
        }

        public UIImage GetImage(UIColor strokeColor, UIColor fillColor, CGSize size, bool shouldCrop = true, bool keepAspectRatio = true)
        {
            return GetImage(strokeColor, fillColor, size, getScaleFromSize(size, Bounds), shouldCrop, keepAspectRatio);
        }

        public UIImage GetImage(UIColor strokeColor, UIColor fillColor, nfloat scale, bool shouldCrop = true, bool keepAspectRatio = true)
        {
            return GetImage(strokeColor, fillColor, getSizeFromScale(scale, Bounds), scale, shouldCrop, keepAspectRatio);
        }

        UIImage GetImage(UIColor strokeColor, UIColor fillColor, CGSize size, nfloat scale, bool shouldCrop = true, bool keepAspectRatio = true)
        {
            if (size.Width == 0 || size.Height == 0 || scale <= 0 || strokeColor == null || fillColor == null)
                return null;

            nfloat uncroppedScale;
            var croppedRectangle = new CGRect();

            CGPoint[] cachedPoints;

            if (shouldCrop && (cachedPoints = Points).Any())
            {
                croppedRectangle = getCroppedRectangle(cachedPoints);
                croppedRectangle.Width /= scale;
                croppedRectangle.Height /= scale;
                if (croppedRectangle.X >= 5)
                {
                    croppedRectangle.X -= 5;
                    croppedRectangle.Width += 5;
                }
                if (croppedRectangle.Y >= 5)
                {
                    croppedRectangle.Y -= 5;
                    croppedRectangle.Height += 5;
                }
                if (croppedRectangle.X + croppedRectangle.Width <= size.Width - 5)
                    croppedRectangle.Width += 5;
                if (croppedRectangle.Y + croppedRectangle.Height <= size.Height - 5)
                    croppedRectangle.Height += 5;

                nfloat scaleX = croppedRectangle.Width / Bounds.Width;
                nfloat scaleY = croppedRectangle.Height / Bounds.Height;
                uncroppedScale = 1 / (nfloat)Math.Max(scaleX, scaleY);
            }
            else
            {
                uncroppedScale = scale;
            }

            //Make sure the image is scaled to the screen resolution in case of Retina display.
            if (keepAspectRatio)
                UIGraphics.BeginImageContext(size);
            else
                UIGraphics.BeginImageContext(new CGSize(croppedRectangle.Width * uncroppedScale, croppedRectangle.Height * uncroppedScale));

            //Create context and set the desired options
            var context = UIGraphics.GetCurrentContext();
            context.SetFillColor(fillColor.CGColor);
            context.FillRect(new CGRect(0, 0, size.Width, size.Height));
            context.SetStrokeColor(strokeColor.CGColor);
            context.SetLineWidth(StrokeWidth);
            context.SetLineCap(CGLineCap.Round);
            context.SetLineJoin(CGLineJoin.Round);
            context.ScaleCTM(uncroppedScale, uncroppedScale);

            //Obtain all drawn paths from the array
            foreach (var bezierPath in paths)
            {
                var tempPath = (UIBezierPath)bezierPath.Copy();
                if (shouldCrop)
                    tempPath.ApplyTransform(CGAffineTransform.MakeTranslation(-croppedRectangle.X, -croppedRectangle.Y));
                CGPath path = tempPath.CGPath;
                context.AddPath(path);
                tempPath = null;
            }
            context.StrokePath();

            var image = UIGraphics.GetImageFromCurrentImageContext();

            UIGraphics.EndImageContext();

            return image;
        }

        CGRect getCroppedRectangle(CGPoint[] cachedPoints)
        {
            var xMin = cachedPoints.Where(point => !point.IsEmpty).Min(point => point.X) - strokeWidth / 2;
            var xMax = cachedPoints.Where(point => !point.IsEmpty).Max(point => point.X) + strokeWidth / 2;
            var yMin = cachedPoints.Where(point => !point.IsEmpty).Min(point => point.Y) - strokeWidth / 2;
            var yMax = cachedPoints.Where(point => !point.IsEmpty).Max(point => point.Y) + strokeWidth / 2;

            xMin = (nfloat)Math.Max(xMin, 0);
            xMax = (nfloat)Math.Min(xMax, Bounds.Width);
            yMin = (nfloat)Math.Max(yMin, 0);
            yMax = (nfloat)Math.Min(yMax, Bounds.Height);

            return new CGRect(xMin, yMin, xMax - xMin, yMax - yMin);
        }

        nfloat getScaleFromSize(CGSize size, CGRect rectangle)
        {
            nfloat scaleX = size.Width / rectangle.Width;
            nfloat scaleY = size.Height / rectangle.Height;

            return (nfloat)Math.Min(scaleX, scaleY);
        }

        CGSize getSizeFromScale(nfloat scale, CGRect rectangle)
        {
            nfloat width = rectangle.Width * scale;
            nfloat height = rectangle.Height * scale;

            return new CGSize(width, height);
        }

        //Allow the user to import an array of points to be used to draw a signature in the view, with new
        //lines indicated by a CGPoint.Empty in the array.
        public void LoadPoints(CGPoint[] loadedPoints)
        {
            if (loadedPoints == null || !loadedPoints.Any())
                return;

            var startIndex = 0;
            var emptyIndex = loadedPoints.ToList().IndexOf(CGPoint.Empty);

            if (emptyIndex == -1)
                emptyIndex = loadedPoints.Count();

            //Clear any existing paths or points.
            paths = new List<UIBezierPath>();
            points = new List<CGPoint[]>();

            do
            {
                //Create a new path and set the line options
                currentPath = UIBezierPath.Create();
                currentPath.LineWidth = StrokeWidth;
                currentPath.LineJoinStyle = CGLineJoin.Round;

                currentPoints = new List<CGPoint>();

                //Move to the first point and add that point to the current_points array.
                currentPath.MoveTo(loadedPoints[startIndex]);
                currentPoints.Add(loadedPoints[startIndex]);

                //Iterate through the array until an empty point (or the end of the array) is reached,
                //adding each point to the current_path and to the current_points array.
                for (var i = startIndex + 1; i < emptyIndex; i++)
                {
                    currentPath.AddLineTo(loadedPoints[i]);
                    currentPoints.Add(loadedPoints[i]);
                }

                //Add the current_path and current_points list to their respective Lists before
                //starting on the next line to be drawn.
                paths.Add(currentPath);
                points.Add(currentPoints.ToArray());

                //Obtain the indices for the next line to be drawn.
                startIndex = emptyIndex + 1;
                if (startIndex < loadedPoints.Count() - 1)
                {
                    emptyIndex = loadedPoints.ToList().IndexOf(CGPoint.Empty, startIndex);

                    if (emptyIndex == -1)
                        emptyIndex = loadedPoints.Count();
                }
                else
                    emptyIndex = startIndex;
            } while (startIndex < emptyIndex);

            //Obtain the image for the imported signature and display it in the image view.
            imageView.Image = GetImage(false);
            //Display the clear button.
            ClearLabel.Hidden = false;
            SetNeedsDisplay();
        }

        //Update the bounds for the rectangle to be redrawn if necessary for the given point.
        void updateBounds(CGPoint point)
        {
            if (point.X < minX + 1)
                minX = point.X - 1;
            if (point.X > maxX - 1)
                maxX = point.X + 1;
            if (point.Y < minY + 1)
                minY = point.Y - 1;
            if (point.Y > maxY - 1)
                maxY = point.Y + 1;
        }

        //Set the bounds for the rectangle that will need to be redrawn to show the drawn path.
        void resetBounds(CGPoint point)
        {
            minX = point.X - 1;
            maxX = point.X + 1;
            minY = point.Y - 1;
            maxY = point.Y + 1;
        }

        /*
		 *Obtain a smoothed path with the specified granularity from the current path using Catmull-Rom spline.  
		 *Implemented using a modified version of the code in the solution at 
		 *http://stackoverflow.com/questions/8702696/drawing-smooth-curves-methods-needed.
		 *Also outputs a List of the points corresponding to the smoothed path.
		 */
        UIBezierPath smoothedPathWithGranularity(int granularity, out List<CGPoint> smoothedPoints)
        {
            List<CGPoint> pointsArray = currentPoints;
            smoothedPoints = new List<CGPoint>();

            //Not enough points to smooth effectively, so return the original path and points.
            if (pointsArray.Count < 4)
            {
                smoothedPoints = pointsArray;
                return currentPath;
            }

            //Create a new bezier path to hold the smoothed path.
            var smoothedPath = UIBezierPath.Create();
            smoothedPath.LineWidth = StrokeWidth;
            smoothedPath.LineJoinStyle = CGLineJoin.Round;

            //Duplicate the first and last points as control points.
            pointsArray.Insert(0, pointsArray[0]);
            pointsArray.Add(pointsArray[pointsArray.Count - 1]);

            //Add the first point
            smoothedPath.MoveTo(pointsArray[0]);
            smoothedPoints.Add(pointsArray[0]);

            for (var index = 1; index < pointsArray.Count - 2; index++)
            {
                CGPoint p0 = pointsArray[index - 1];
                CGPoint p1 = pointsArray[index];
                CGPoint p2 = pointsArray[index + 1];
                CGPoint p3 = pointsArray[index + 2];

                //Add n points starting at p1 + dx/dy up until p2 using Catmull-Rom splines
                for (var i = 1; i < granularity; i++)
                {
                    float t = i * (1f / granularity);
                    float tt = t * t;
                    float ttt = tt * t;

                    //Intermediate point
                    CGPoint mid = default(CGPoint);
                    mid.X = 0.5f * (2f * p1.X + (p2.X - p0.X) * t +
                        (2f * p0.X - 5f * p1.X + 4f * p2.X - p3.X) * tt +
                        (3f * p1.X - p0.X - 3f * p2.X + p3.X) * ttt);
                    mid.Y = 0.5f * (2 * p1.Y + (p2.Y - p0.Y) * t +
                        (2 * p0.Y - 5 * p1.Y + 4 * p2.Y - p3.Y) * tt +
                        (3 * p1.Y - p0.Y - 3 * p2.Y + p3.Y) * ttt);

                    smoothedPath.AddLineTo(mid);
                    smoothedPoints.Add(mid);
                }

                //Add p2
                smoothedPath.AddLineTo(p2);
                smoothedPoints.Add(p2);
            }

            //Add the last point
            smoothedPath.AddLineTo(pointsArray[pointsArray.Count - 1]);
            smoothedPoints.Add(pointsArray[pointsArray.Count - 1]);

            return smoothedPath;
        }

        #region Touch Events
        public override void TouchesBegan(NSSet touches, UIEvent evt)
        {
            //Create a new path and set the options.
            currentPath = UIBezierPath.Create();
            currentPath.LineWidth = StrokeWidth;
            currentPath.LineJoinStyle = CGLineJoin.Round;

            currentPoints.Clear();

            var touch = touches.AnyObject as UITouch;

            //Obtain the location of the touch, move the path to that position and add it to the
            //current_points array.
            var touchLocation = touch.LocationInView(this);
            currentPath.MoveTo(touchLocation);
            currentPoints.Add(touchLocation);

            resetBounds(touchLocation);

            ClearLabel.Hidden = false;
        }

        public override void TouchesMoved(NSSet touches, UIEvent evt)
        {
            var touch = touches.AnyObject as UITouch;

            //Obtain the location of the touch and add it to the current path and current_points array.
            var touchLocation = touch.LocationInView(this);
            currentPath.AddLineTo(touchLocation);
            currentPoints.Add(touchLocation);

            updateBounds(touchLocation);
            SetNeedsDisplayInRect(new CGRect(minX, minY, (nfloat)Math.Abs(maxX - minX), (nfloat)Math.Abs(maxY - minY)));
        }

        public override void TouchesEnded(NSSet touches, UIEvent evt)
        {
            var touch = touches.AnyObject as UITouch;

            //Obtain the location of the touch and add it to the current path and current_points array.
            var touchLocation = touch.LocationInView(this);
            currentPath.AddLineTo(touchLocation);
            currentPoints.Add(touchLocation);

            //Obtain the smoothed path and the points array for that path.
            currentPath = smoothedPathWithGranularity(40, out currentPoints);
            //Add the smoothed path and points array to their Lists.
            paths.Add(currentPath);
            points.Add(currentPoints.ToArray());

            //Obtain the image for the imported signature and display it in the image view.
            imageView.Image = GetImage(false);
            updateBounds(touchLocation);
            SetNeedsDisplay();
        }
        #endregion

        public override void Draw(CGRect rect)
        {
            if (currentPath == null || currentPath.Empty)
                return;

            strokeColor.SetStroke();
            currentPath.Stroke();
        }

        public override void LayoutSubviews()
        {
            var w = Frame.Width;
            var h = Frame.Height;

            SignaturePrompt.SizeToFit();
            ClearLabel.SizeToFit();

            var captionHeight = Caption.SizeThatFits(Caption.Frame.Size).Height;
            var clearButtonHeight = (int)ClearLabel.TitleLabel.Font.LineHeight + 1;

            var rect = new CGRect(0, 0, w, h);
            imageView.Frame = rect;
            BackgroundImageView.Frame = rect;

            var top = h;
            top = top - ThinPad - captionHeight;
            Caption.Frame = new CGRect(ThickPad, top, w - ThickPad - ThickPad, captionHeight);
            top = top - ThinPad - SignatureLine.Frame.Height;
            SignatureLine.Frame = new CGRect(ThickPad, top, w - ThickPad - ThickPad, LineHeight);
            top = top - ThinPad - SignaturePrompt.Frame.Height;
            SignaturePrompt.Frame = new CGRect(ThickPad, top, SignaturePrompt.Frame.Width, SignaturePrompt.Frame.Height);
            ClearLabel.Frame = new CGRect(w - ThickPad - ClearLabel.Frame.Width, ThickPad, ClearLabel.Frame.Width, clearButtonHeight);
            imageView.Image = GetImage(false);
            SetNeedsDisplay();
        }
    }







    public class Message
    {
        public string Username { get; set; }
        public string Text { get; set; }
        public UIColor ProfileColor { get; set; }
    }

    public class MessageTextView : SlackTextView
    {
        public MessageTextView()
        {
            BackgroundColor = UIColor.White;
            Placeholder = "Message";
            PlaceholderColor = UIColor.LightGray;
            PastableMediaTypes = PastableMediaType.All;
            Layer.BorderColor = UIColor.FromRGB(217, 217, 217).CGColor;
        }
    }

    public class MessageTableViewCell : UITableViewCell
    {
        public static nfloat AvatarHeight = 30.0f;
        public static nfloat MinimumHeight = 50.0f;

        public static nfloat DefaultFontSize
        {
            get
            {
                nfloat pointSize = 16.0f;
                var contentSizeCategory = UIApplication.SharedApplication.PreferredContentSizeCategory;
                pointSize += SlackTextView.SlackPointSizeDifference(contentSizeCategory);

                return pointSize;
            }
        }

        protected MessageTableViewCell(IntPtr handle) : base(handle)
        {
            SelectionStyle = UITableViewCellSelectionStyle.None;
            BackgroundColor = UIColor.White;

            TitleLabel = new UILabel
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                BackgroundColor = UIColor.Clear,
                UserInteractionEnabled = false,
                Lines = 0,
                TextColor = UIColor.Gray,
                Font = UIFont.BoldSystemFontOfSize(DefaultFontSize)
            };

            BodyLabel = new UILabel
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                BackgroundColor = UIColor.Clear,
                UserInteractionEnabled = false,
                Lines = 0,
                TextColor = UIColor.DarkGray,
                Font = UIFont.SystemFontOfSize(DefaultFontSize)
            };

            ThumbnailView = new UIImageView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                UserInteractionEnabled = false,
                BackgroundColor = UIColor.FromWhiteAlpha(0.9f, 1.0f)
            };
            ThumbnailView.Layer.CornerRadius = AvatarHeight / 2.0f;
            ThumbnailView.Layer.MasksToBounds = true;

            ContentView.AddSubview(ThumbnailView);
            ContentView.AddSubview(TitleLabel);
            ContentView.AddSubview(BodyLabel);

            var views = NSDictionary.FromObjectsAndKeys(new NSObject[] {
                ThumbnailView,
                TitleLabel,
                BodyLabel
            }, new NSObject[] {
                (NSString)"thumbnailView",
                (NSString)"titleLabel",
                (NSString)"bodyLabel"
            });

            var metrics = NSDictionary.FromObjectsAndKeys(new[] {
                NSNumber.FromNFloat (AvatarHeight),
                NSNumber.FromNFloat (15f),
                NSNumber.FromNFloat (10f),
                NSNumber.FromNFloat (5f),
                NSNumber.FromNFloat (80f)
            }, new[] {
                "thumbSize",
                "padding",
                "right",
                "left",
                "attchSize"
            });

            ContentView.AddConstraints(NSLayoutConstraint.FromVisualFormat("H:|-left-[thumbnailView(thumbSize)]-right-[titleLabel(>=0)]-right-|", 0, metrics, views));
            ContentView.AddConstraints(NSLayoutConstraint.FromVisualFormat("H:|-left-[thumbnailView(thumbSize)]-right-[bodyLabel(>=0)]-right-|", 0, metrics, views));
            ContentView.AddConstraints(NSLayoutConstraint.FromVisualFormat("V:|-right-[titleLabel(>=0@999)]-left-[bodyLabel(>=0@999)]-left-|", 0, metrics, views));
            ContentView.AddConstraints(NSLayoutConstraint.FromVisualFormat("V:|-right-[thumbnailView(thumbSize)]-(>=0)-|", 0, metrics, views));
        }

        public UILabel TitleLabel { get; private set; }
        public UILabel BodyLabel { get; private set; }
        public UIImageView ThumbnailView { get; private set; }
        public NSIndexPath IndexPath { get; set; }

        public bool UsedForMessage { get; set; }

        public override void PrepareForReuse()
        {
            base.PrepareForReuse();

            SelectionStyle = UITableViewCellSelectionStyle.None;

            var pointSize = DefaultFontSize;

            TitleLabel.Font = UIFont.BoldSystemFontOfSize(pointSize);
            BodyLabel.Font = UIFont.SystemFontOfSize(pointSize);
            TitleLabel.Text = "";
            BodyLabel.Text = "";
        }
    }


}

