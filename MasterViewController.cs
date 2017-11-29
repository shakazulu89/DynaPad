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
//using static DynaClassLibrary.DynaClasses;
//using DynaClassLibrary;
using System.Threading.Tasks;
//using MultiThreading.Controls;
//using Syncfusion.SfBusyIndicator.iOS;
using System.Linq;


namespace DynaPad
{
	public partial class MasterViewController : DynaDialogViewController
	{
		public DetailViewController DetailViewController { get; set; }
		public DialogViewController mvc { get; set; }
		UILabel messageLabel;
		LoadingOverlay loadingOverlay;
		Menu myDynaMenu;

		protected MasterViewController(IntPtr handle) : base(handle)
		{
			// Note: this .ctor should not contain any initialization logic.
			//Title = "";
		}

		bool needLogin = true;
		public string DocLocID { get; set; }
		public string DynaDomain { get; set; }
		NSUserDefaults plist = NSUserDefaults.StandardUserDefaults;

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

		public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear(animated);

			//string ass = plist.StringForKey("Domain_Name");
			//plist.SetString("DynaDomain", NSUserDefaults.StandardUserDefaults.StringForKey("Domain_Name"));
			//plist.Synchronize();
			DynaDomain = plist.StringForKey("Domain_Name");
			//DynaDomain = plist.StringForKey("DynaDomain");
			if (!string.IsNullOrEmpty(DynaDomain))
			{
				//plist.SetString("DynaDomain", DynaDomain);
				//plist.Synchronize();
				var con = CrossConnectivity.Current;
				if (con.IsConnected)
				{
					if (needLogin)
					{
						LoginScreenControl<CredentialsProvider, DefaultLoginScreenMessages>.Activate(this);
						//DynaLogin<CredentialsProvider, DefaultLoginScreenMessages>.Activate(this);
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

						DynaLocations();
						//DynaStart();
					}
				}
				else
				{
					PresentViewController(CommonFunctions.InternetAlertPrompt(), true, null);

					var logRoot = new RootElement("Login");
					var logSec = new Section();
					logSec.Add(new StringElement("Login", Login));
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
				var logSec = new Section();
				logSec.Add(new StringElement("Login", Login));
				logRoot.Add(logSec);
				Root = logRoot;
			}
		}

		public void SaveDomain(string domainname)
		{
			//plist.SetString(domainname, "DynaDomain");
			//plist.Synchronize();
			plist.SetString(domainname, "Domain_Name");
			plist.Synchronize();
		}

		public void Login()
		{
			needLogin = true;
			ViewDidAppear(true);
		}


		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			DetailViewController = (DetailViewController)((UINavigationController)SplitViewController.ViewControllers[1]).TopViewController;
			DetailViewController.Style = UITableViewStyle.Plain;
		}


        public void DynaLocations()
        {
            var rootMainMenu = new DynaFormRootElement("Locations");
            var sectionMainMenu = new Section();

            try
            {
                rootMainMenu.UnevenRows = true;
                rootMainMenu.Enabled = true;

                foreach (DynaClassLibrary.DynaClasses.Location loc in DynaClassLibrary.DynaClasses.LoginContainer.User.Locations)
                {
                    var rootMenu = new DynaFormRootElement(loc.LocationName);
                    rootMenu.UnevenRows = true;
                    rootMenu.Enabled = true;
                    rootMenu.createOnSelected = GetDynaStart;
                    rootMenu.ShowLoading = true;
                    rootMenu.MenuValue = loc.LocationId;
                    sectionMainMenu.Add(rootMenu);
                }

                var settingsStringElement = new StyledStringElement("Settings", ShowSettings);

                var feedbackStringElement = new StyledStringElement("Feedback (v" + NSBundle.MainBundle.InfoDictionary["CFBundleVersion"] + ")", ShowFeedbackList);

                sectionMainMenu.Add(settingsStringElement);
                sectionMainMenu.Add(feedbackStringElement);
                sectionMainMenu.Add(GetLogoutElement());

                rootMainMenu.Add(sectionMainMenu);

                Root = rootMainMenu;
            }
            catch (Exception ex)
            {
                CommonFunctions.sendErrorEmail(ex);
                PresentViewController(CommonFunctions.ExceptionAlertPrompt(), true, null);
                DetailViewController.Root.Clear();
                DetailViewController.Root.Add(CommonFunctions.ErrorDetailSection());
                sectionMainMenu.Add(GetLogoutElement());
                rootMainMenu.Add(sectionMainMenu);
                Root = rootMainMenu;
            }
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
            });
            logoutStringElement.BackgroundColor = UIColor.FromRGB(255, 172, 172);

            return logoutStringElement;
        }



		public UIViewController GetDynaStart(RootElement rElement)
		{
			string menujson = "";
			try
			{
				var con = CrossConnectivity.Current;
				if (con.IsConnected)
				{
					var dfElemet = (DynaFormRootElement)rElement;
					DynaClassLibrary.DynaClasses.LoginContainer.User.SelectedLocation = DynaClassLibrary.DynaClasses.LoginContainer.User.Locations.Find(l => l.LocationId == dfElemet.MenuValue);

                    var dds = new DynaPadService.DynaPadService() { Timeout = 60000};
					//var locid = string.IsNullOrEmpty(Constants.DocLocID) ? "1" : Constants.DocLocID;
					var locid = string.IsNullOrEmpty(DynaClassLibrary.DynaClasses.LoginContainer.User.SelectedLocation.LocationId) ? null : DynaClassLibrary.DynaClasses.LoginContainer.User.SelectedLocation.LocationId;
					if (string.IsNullOrEmpty(locid))
					{
						PresentViewController(CommonFunctions.AlertPrompt("Location Error", "An active location is required in order to run this app, please contact administration", true, null, false, null), true, null);
						return new DynaDialogViewController(new RootElement("No Location"), true);
					}
					menujson = dds.BuildDynaMenu(CommonFunctions.GetUserConfig(), locid, DynaClassLibrary.DynaClasses.LoginContainer.User.SelectedLocation.LocationName);
					myDynaMenu = JsonConvert.DeserializeObject<Menu>(menujson);
					DetailViewController.DynaMenu = myDynaMenu;

                    DetailViewController.Root.Clear();

					var rootMainMenu = new DynaFormRootElement(myDynaMenu.MenuCaption); 
					rootMainMenu.UnevenRows = true;
					rootMainMenu.Enabled = true;
					//rootMainMenu.RadioSelected = -1;
					//Root.RadioSelected = -1;

					var sectionMainMenu = new Section();
					sectionMainMenu.HeaderView = null;
					BuildMenu(myDynaMenu, sectionMainMenu);
					rootMainMenu.Add(sectionMainMenu);
                    //DetailViewController.SetDetailItem(new Section(), "URL", "http://www.adobe.com/content/dam/Adobe/en/devnet/acrobat/pdfs/pdf_reference_1-7.pdf", "", true, null, false, null, null);
					var formDVC = new DynaDialogViewController(rootMainMenu, true);
					return formDVC;
				}

				PresentViewController(CommonFunctions.InternetAlertPrompt(), true, null);
				return new DynaDialogViewController(new RootElement("No internet") , true);
			}
			catch (Exception ex)
			{
				CommonFunctions.sendErrorEmail(ex);
                PresentViewController(CommonFunctions.ExceptionAlertPrompt(), true, null);
                return new DynaDialogViewController(CommonFunctions.ErrorRootElement() , true);
				//throw new Exception(ex.Message + Environment.NewLine + ex.StackTrace, ex.InnerException);
			}
		}



		public void Logout()
		{
			DetailViewController = (DetailViewController)((UINavigationController)SplitViewController.ViewControllers[1]).TopViewController;
			DetailViewController.NavigationController.PopToRootViewController(true);
			DetailViewController.Root.Clear();

			//DetailViewController.ReloadData();
			//ReloadData();

			needLogin = true;
			ViewDidAppear(true);
		}


		public RootElement BuildMenu(Menu myMenu, Section sectionMenu)
		{
			try
			{
				if (myMenu.MenuItems == null) return null;

				foreach (MenuItem mItem in myMenu.MenuItems)
				{
					var rootMenu = new DynaFormRootElement(mItem.MenuItemCaption);

					rootMenu.UnevenRows = true;

					rootMenu.Enabled = true;
					rootMenu.FormID = mItem.MenuItemValue;
					rootMenu.FormName = mItem.MenuItemCaption;
					rootMenu.MenuAction = mItem.MenuItemAction;
					rootMenu.MenuValue = mItem.MenuItemValue;
					rootMenu.PatientID = mItem.PatientId;
					rootMenu.PatientName = mItem.PatientName;
					rootMenu.DoctorID = mItem.DoctorId;
					rootMenu.LocationID = mItem.LocationId;
					rootMenu.ApptID = mItem.ApptId;
					rootMenu.IsDoctorForm = mItem.MenuItemAction == "GetDoctorForm";

					switch (mItem.MenuItemAction)
					{
						case "GetPatientForm":
						case "GetDoctorForm":
                            rootMenu.ShowLoading = true;
							rootMenu.createOnSelected = GetFormService;
							break;
						case "GetAppt":
							rootMenu.createOnSelected = GetApptService;
							break;
						case "GetApptForm":
							rootMenu.createOnSelected = GetApptFormService;
							break;
                        case "GetFiles":
                            rootMenu.ShowLoading = true;
							rootMenu.createOnSelected = GetMRFoldersService;
							break;
						case "GetReport":
							//sectionMenu.Add(new StringElement(mItem.MenuItemCaption, delegate { LoadReportView(mItem.MenuItemValue, "Report", rootMenu); }));
							sectionMenu.Add(new SectionStringElement(mItem.MenuItemCaption, delegate
							{
								LoadReportView(mItem.MenuItemValue, "Report", rootMenu, mItem.MenuItemCaption);
								foreach (Element d in sectionMenu.Elements)
								{
									var t = d.GetType();
									if (t == typeof(SectionStringElement))
									{
										var di = (SectionStringElement)d;
										di.selected = false;
									}
								}
								sectionMenu.GetContainerTableView().ReloadData();
							}));
							//rootMenu.createOnSelected = GetReportService;
							//Section sectionReport = new Section();
							//sectionReport.Add(new StringElement(rootMenu.MenuValue, delegate { LoadReportView("Report", rootMenu.MenuValue); }));
							//rootMenu.Add(sectionReport);

							//DetailViewController.Root.Caption = mItem.MenuItemValue;
							//DetailViewController.ReloadData();
							break;
						case "GetSummary":
							sectionMenu.Add(new StringElement(mItem.MenuItemCaption, delegate { LoadSummaryView(mItem.MenuItemValue, "Summary", rootMenu); }));
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
					if (mItem.MenuItemAction != "GetReport" && mItem.MenuItemAction != "GetSummary" && mItem.MenuItemAction != "Logout")
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

				return null;
			}
			catch (Exception ex)
			{
				CommonFunctions.sendErrorEmail(ex);
                PresentViewController(CommonFunctions.ExceptionAlertPrompt(), true, null);
                return null;
				//throw new Exception(ex.Message + Environment.NewLine + ex.StackTrace, ex.InnerException);
			}
		}


		void ShowFeedbackList()
		{
			// This is where the feedback form gets displayed
			var feedbackManager = BITHockeyManager.SharedHockeyManager.FeedbackManager;
			feedbackManager.ShowFeedbackListView();
		}



		public void ShowSettings()
		{
			//var nlab = new UILabel(new CGRect(10, 0, View.Bounds.Width - 60, 50));
			//var nlab = new UILabel(new CGRect(10, 0, 710, 50));
			var nlab = new UILabel(new CGRect(10, 0, UIScreen.MainScreen.Bounds.Width - 310, 50));
			nlab.Text = "Settings";

			var ncellHeader = new UITableViewCell(UITableViewCellStyle.Default, null);
			//ncellHeader.Frame = new CGRect(0, 0, 770, 50);
			ncellHeader.Frame = new CGRect(0, 0, UIScreen.MainScreen.Bounds.Width - 250, 50);

			//var nheadclosebtn = new UIButton(new CGRect(View.Bounds.Width - 50, 0, 50, 50));
			//var nheadclosebtn = new UIButton(new CGRect(720, 0, 50, 50));
			var nheadclosebtn = new UIButton(new CGRect(UIScreen.MainScreen.Bounds.Width - 300, 0, 50, 50));
			nheadclosebtn.SetImage(UIImage.FromBundle("Close"), UIControlState.Normal);

			ncellHeader.ContentView.Add(nlab);
			ncellHeader.ContentView.Add(nheadclosebtn);

			var nsec = new Section(ncellHeader);
			nsec.FooterView = new UIView(new CGRect(0, 0, 0, 0));
			nsec.FooterView.Hidden = true;

			var SettingsView = new DynaMultiRootElement();

			var sSection = new DynaSection("Settings");
			sSection.HeaderView = new UIView(new CGRect(0, 0, UIScreen.MainScreen.Bounds.Width - 250, 30));
			sSection.FooterView = new UIView(new CGRect(0, 0, 0, 0));
			sSection.FooterView.Hidden = true;

			var sPaddedView = new PaddedUIView<UILabel>();
			sPaddedView.Frame = new CGRect(0, 0, UIScreen.MainScreen.Bounds.Width - 250, 30);
			sPaddedView.Padding = 5f;
			sPaddedView.NestedView.Text = "DOMAIN NAME:";
			sPaddedView.setStyle();

			sSection.HeaderView.Add(sPaddedView);

			var txtDomain = new UITextView(new CGRect(0, 0, UIScreen.MainScreen.Bounds.Width - 250, 30));
			//txtDomain.Text = plist.StringForKey("DynaDomain");
			txtDomain.Text = plist.StringForKey("Domain_Name");

			sSection.Add(txtDomain);

			var btnDomain = new UIButton(new CGRect(0, 0, UIScreen.MainScreen.Bounds.Width - 250, 40));
			btnDomain.SetTitle("Update", UIControlState.Normal);
			btnDomain.SetTitleColor(UIColor.Black, UIControlState.Normal);
			btnDomain.BackgroundColor = UIColor.FromRGB(224, 238, 240);
			btnDomain.HorizontalAlignment = UIControlContentHorizontalAlignment.Center;
			btnDomain.TouchUpInside += delegate {
				plist.SetString(txtDomain.Text, "Domain_Name");
				plist.Synchronize();
				//NSUserDefaults.StandardUserDefaults.SetString("domain_preference", txtDomain.Text);
				//NSUserDefaults.StandardUserDefaults.Synchronize();
				NavigationController.DismissViewController(true, null);
				Logout();
			};

			sSection.Add(btnDomain);

			SettingsView.Add(nsec);
			SettingsView.Add(sSection);

			//nsec.Add(SettingsView);

			var nroo = new RootElement("Settings");
			nroo.Add(nsec);

			var ndia = new DialogViewController(SettingsView);
			ndia.ModalInPopover = true;
			ndia.ModalPresentationStyle = UIModalPresentationStyle.PageSheet;
			ndia.PreferredContentSize = new CGSize(View.Bounds.Size);

			nheadclosebtn.TouchUpInside += delegate
				{
					NavigationController.DismissViewController(true, null);
				};

			NavigationController.PreferredContentSize = new CGSize(View.Bounds.Size);
			//NavigationController.View.BackgroundColor = UIColor.Clear;
			NavigationController.PresentViewController(ndia, true, null);
			//NavigationController.View.SizeToFit();
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
				//SelectedAppointment.ApptFormId = dfElemet.FormID;
				//SelectedAppointment.ApptFormName = dfElemet.FormName;
				SelectedAppointment.ApptPatientId = dfElemet.PatientID;
				SelectedAppointment.ApptPatientName = dfElemet.PatientName;
				SelectedAppointment.ApptDoctorId = dfElemet.DoctorID;
				SelectedAppointment.ApptLocationId = dfElemet.LocationID;
				SelectedAppointment.ApptId = dfElemet.ApptID;
                SelectedAppointment.CaseId = dfElemet.CaseID;

				var formDVC = new DynaDialogViewController(rElement, true);
				return formDVC;

				//return new DynaDialogViewController(rElement, true);
			}
			catch (Exception ex)
			{
                CommonFunctions.sendErrorEmail(ex);
                PresentViewController(CommonFunctions.ExceptionAlertPrompt(), true, null);
                return new DynaDialogViewController(CommonFunctions.ErrorRootElement(), true);
				//throw new Exception(ex.Message + Environment.NewLine + ex.StackTrace, ex.InnerException);
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
				  //DetailViewController.Title = "Welcome to Dynapad";
				  DetailViewController.QuestionsView = null; //.Clear();
				  DetailViewController.NavigationItem.RightBarButtonItem = null;
					  DetailViewController.Root.Clear();
					  //DetailViewController.Root.Add(new Section("Logged in")
					  //{
						 // FooterView = new UIView(new CGRect(0, 0, 0, 0))
						 // {
							//  Hidden = true
						 // }
					  //});
					  DetailViewController.Root.Caption = "";
					  DetailViewController.ReloadData();

					  NavigationController.PopViewController(true);
				  });

				return formDVC;
				//return new DynaDialogViewController(rElement, true);
			}
			catch (Exception ex)
			{
				CommonFunctions.sendErrorEmail(ex);
                PresentViewController(CommonFunctions.ExceptionAlertPrompt(), true, null);
                return new DynaDialogViewController(CommonFunctions.ErrorRootElement(), true);
				//throw new Exception(ex.Message + Environment.NewLine + ex.StackTrace, ex.InnerException);
			}
		}


		public UIViewController GetReportService(RootElement rElement)
		{
			try
			{
				//if (DetailViewController.QuestionsView != null)
				//{
				//	DetailViewController.Title = "";
				//	DetailViewController.QuestionsView = null; //.Clear();
				//}

				if (CrossConnectivity.Current.IsConnected)
				{
					var dfElemet = (DynaFormRootElement)rElement;
					//var DynaReport = SelectedAppointment.ApptDynaReports.Find((DynaReport obj) => obj.FormId == dfElemet.MenuValue);
					var dds = new DynaPadService.DynaPadService() { Timeout = 180000};
					var origJson = dds.GetDynaReports(CommonFunctions.GetUserConfig(), dfElemet.FormID, dfElemet.DoctorID, false);
					JsonHandler.OriginalFormJsonString = origJson;
					//var rootReports = new RootElement(dfElemet.FormName);
					var reportsGroup = new RadioGroup("reports", -1);
					var rootReports = new RootElement("Reports", reportsGroup);
					var sectionReports = new Section();

					//foreach (Report esf in SelectedAppointment.ApptReports)
					//{
					//	sectionReports.Add(new SectionStringElement(esf.ReportName, delegate { LoadSectionView(esf.ReportId, "Report", null, false); }));
					//}

					foreach (Report esf in SelectedAppointment.ApptReports)
					{
						var report = new SectionStringElement(esf.ReportName, delegate
						{
							LoadReportView(esf.ReportId, "Report", rootReports, esf.ReportName);
						//LoadSectionView(esf.ReportId, "Report", null, false);
						foreach (Element d in sectionReports.Elements)
							{
								var t = d.GetType();
								if (t == typeof(SectionStringElement))
								{
									var di = (SectionStringElement)d;
									di.selected = false;
								}
							}

							sectionReports.GetContainerTableView().ReloadData();
						});

						sectionReports.Add(report);
					}

					rootReports.Add(sectionReports);

					var formDVC = new DynaDialogViewController(rootReports, true);

					DetailViewController.Root.Caption = dfElemet.FormName;
					DetailViewController.ReloadData();

					formDVC.NavigationItem.LeftBarButtonItem = new UIBarButtonItem(UIImage.FromBundle("Back"), UIBarButtonItemStyle.Plain, delegate 
					  {
					  //DetailViewController.Title = "Welcome to Dynapad";
					  DetailViewController.QuestionsView = null; //.Clear();
					  DetailViewController.NavigationItem.RightBarButtonItem = null;
						  DetailViewController.Root.Clear();
						  //DetailViewController.Root.Add(new Section("Logged in")
						  //{
							 // FooterView = new UIView(new CGRect(0, 0, 0, 0))
							 // {
								//  Hidden = true
							 // }
						  //});
						  DetailViewController.Root.Caption = SelectedAppointment.ApptFormName + " - " + SelectedAppointment.ApptPatientName;
						  DetailViewController.ReloadData();

						  NavigationController.PopViewController(true);
					  });

					return formDVC;
				}

				PresentViewController(CommonFunctions.InternetAlertPrompt(), true, null);
				return new DynaDialogViewController(new RootElement("No internet"), true);
			}
			catch (Exception ex)
			{
				CommonFunctions.sendErrorEmail(ex);
                PresentViewController(CommonFunctions.ExceptionAlertPrompt(), true, null);
                return new DynaDialogViewController(CommonFunctions.ErrorRootElement(), true);
				//throw new Exception(ex.Message + Environment.NewLine + ex.StackTrace, ex.InnerException);
			}
		}


		public UIViewController GetMRFoldersService(RootElement rElement)
		{
			try
			{
				//var bounds = base.TableView.Frame;
				// show the loading overlay on the UI thread using the correct orientation sizing
				//loadingOverlay = new LoadingOverlay(bounds);
				//SplitViewController.Add(loadingOverlay);

				if (CrossConnectivity.Current.IsConnected)
				{
					var dds = new DynaPadService.DynaPadService() { Timeout = 180000};
					var dfElemet = (DynaFormRootElement)rElement;

					var origJson = dds.GetFiles(CommonFunctions.GetUserConfig(), dfElemet.ApptID, dfElemet.PatientID, dfElemet.PatientName, dfElemet.DoctorID, dfElemet.LocationID);
					JsonHandler.OriginalFormJsonString = origJson;
					SelectedAppointment.ApptMRFolders = JsonConvert.DeserializeObject<List<MRFolder>>(origJson);

					DetailViewController.Root.Caption = "Medical Records: " + SelectedAppointment.ApptPatientName;
					DetailViewController.ReloadData();

					var mrFolderGroup = new RadioGroup("mrfolders", -1);
					var rootMRFolders = new RootElement("Medical Records", mrFolderGroup);
					var mrFolderSections = new Section();

					foreach (MRFolder mrf in SelectedAppointment.ApptMRFolders)
					{
						//foreach (MRFolder submrf in mrf.MrFolderMRFolders)
						//{
						//	var mrSubFolderGroup = new RadioGroup("mrsubfolders", -1);
						//	var rootSubMRFolders = new RootElement(submrf.MRFolderName, mrSubFolderGroup);
						//	mrFolderSections.Add(rootSubMRFolders);
						//}
						//foreach (MR mr in mrf.MrFolderMRs)
						//{

						//}

						//mrFolderSections.Add(mrfolder);

						var mrfolder = new SectionStringElement(mrf.MRFolderName, delegate
						{
							LoadMRView(mrf.MRFolderName, mrf.MRFolderId, mrf.MRFolderPath, mrf, rootMRFolders);
						//LoadSectionView(esf.ReportId, "Report", null, false);
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
						});

						mrFolderSections.Add(mrfolder);
					}

					rootMRFolders.Add(mrFolderSections);

					var formDVC = new DynaDialogViewController(rootMRFolders, true);
					formDVC.NavigationItem.LeftBarButtonItem = new UIBarButtonItem(UIImage.FromBundle("Back"), UIBarButtonItemStyle.Plain, delegate 
						  {
						  //DetailViewController.Title = "Welcome to Dynapad";
						  DetailViewController.QuestionsView = null; //.Clear();
						  DetailViewController.NavigationItem.RightBarButtonItem = null;
							  DetailViewController.Root.Clear();
							  //DetailViewController.Root.Add(new Section("Logged in")
							  //{
								 // FooterView = new UIView(new CGRect(0, 0, 0, 0))
								 // {
									//  Hidden = true
								 // }
							  //});
                              DetailViewController.Root.Caption = "";
							  DetailViewController.ReloadData();

							  NavigationController.PopViewController(true);
						  });

					//loadingOverlay.Hide();

					return formDVC;
				}

				PresentViewController(CommonFunctions.InternetAlertPrompt(), true, null);
				return new DynaDialogViewController(new RootElement("No internet"), true);
			}
			catch (Exception ex)
			{
				CommonFunctions.sendErrorEmail(ex);
                PresentViewController(CommonFunctions.ExceptionAlertPrompt(), true, null);
				return new DynaDialogViewController(CommonFunctions.ErrorRootElement(), true);
				//throw new Exception(ex.Message + Environment.NewLine + ex.StackTrace, ex.InnerException);
			}
            //finally
            //{
            //    loadingOverlay.Hide();
            //}
		}

		void LoadMRView(string folderName, string folderID, string folderPath, MRFolder mrf, RootElement rt)
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
					//DetailViewController.Root.Add(new Section("Logged in")
					//{
						//FooterView = new UIView(new CGRect(0, 0, 0, 0))
						//{
						//	Hidden = true
						//}
                    //});
                    DetailViewController.Root.Caption = "";
					DetailViewController.ReloadData();

					NavigationController.PopViewController(true);
				});

				DetailViewController.SetDetailItem(new Section(folderName), "MR", folderID, "", false, null, true, folderName);
			}
			catch (Exception ex)
			{
				NavigationController.PopViewController(true);
				CommonFunctions.sendErrorEmail(ex);
                PresentViewController(CommonFunctions.ExceptionAlertPrompt(), true, null);
				//throw new Exception(ex.Message + Environment.NewLine + ex.StackTrace, ex.InnerException);
			}
		}



        [Outlet]
        public UIPopoverController DetailViewPopover { get; set; }

        [Outlet]
        public NSObject LastTappedButton { get; set; }

        public UIPopoverController MainPopoverController { get; set; }

        UIBarButtonItem restorebtn;

		public UIViewController GetFormService(RootElement rElement)
		{
            try
            {
                //if (DetailViewController.QuestionsView != null)
                //{
                //	DetailViewController.Title = "";
                //	DetailViewController.QuestionsView = null; //.Clear();
                //}

                //if (CrossConnectivity.Current.IsConnected)
                //{
                //var bounds = UIScreen.MainScreen.Bounds;
                //var bounds = base.TableView.Frame;
                // show the loading overlay on the UI thread using the correct orientation sizing
                //loadingOverlay = new LoadingOverlay(bounds);
                //mvc = (DialogViewController)((UINavigationController)SplitViewController.ViewControllers[0]).TopViewController;
                //SplitViewController.Add(loadingOverlay);

                var dds = new DynaPadService.DynaPadService() { Timeout = 180000};
                var dfElemet = (DynaFormRootElement)rElement;
                var origJson = dds.GetFormQuestions(CommonFunctions.GetUserConfig(), dfElemet.FormID, dfElemet.DoctorID, dfElemet.LocationID, dfElemet.PatientID, dfElemet.PatientName, SelectedAppointment.CaseId, SelectedAppointment.ApptId, dfElemet.IsDoctorForm);
                JsonHandler.OriginalFormJsonString = origJson;
                SelectedAppointment.SelectedQForm = JsonConvert.DeserializeObject<QForm>(origJson);

                DetailViewController.Root.Caption = SelectedAppointment.SelectedQForm.FormName + " - " + SelectedAppointment.ApptPatientName;
                DetailViewController.ReloadData();

                bool IsDoctorForm = dfElemet.IsDoctorForm;

                var navTitle = IsDoctorForm ? "Doctor Form" : "Patient Form";
                var sectionsGroup = new RadioGroup("sections", -1);
                //var rootFormSections = new RootElement(SelectedAppointment.SelectedQForm.FormName, sectionsGroup);
                var rootFormSections = new RootElement(navTitle, sectionsGroup);

                var sectionFormSections = new Section();

                if (IsDoctorForm)
                {
                    /*
                     * TODO: make presets password protected (maybe not, since for doctors only?)! (maybe component: Passcode)
                    */

                    var FormPresetNames = dds.GetAnswerPresets(CommonFunctions.GetUserConfig(), SelectedAppointment.ApptFormId, null, SelectedAppointment.ApptDoctorId, true, SelectedAppointment.ApptLocationId);
                    var formPresetSection = new DynaSection("Form Presets");
                    formPresetSection.Enabled = true;
                    var formPresetGroup = new RadioGroup("FormPresetAnswers", SelectedAppointment.SelectedQForm.FormSelectedTemplateId);
                    var formPresetsRoot = new DynaRootElement("Form Presets", formPresetGroup);
                    formPresetsRoot.IsPreset = true;//background color

                    //var noPresetRadio = new MyRadioElement("No Preset", "FormPresetAnswers");
                    var noPresetRadio = new PresetRadioElement("No Preset", "FormPresetAnswers");
                    noPresetRadio.PresetName = "No Preset";
                    noPresetRadio.OnSelected += delegate
                    {
                        JsonHandler.OriginalFormJsonString = origJson;
                        SelectedAppointment.SelectedQForm = JsonConvert.DeserializeObject<QForm>(origJson);

                        LoadSectionView(SelectedAppointment.SelectedQForm.FormSections[0].SectionId, SelectedAppointment.SelectedQForm.FormSections[0].SectionName, SelectedAppointment.SelectedQForm.FormSections[0], IsDoctorForm, sectionFormSections);
                    };

                    formPresetSection.Add(noPresetRadio);

                    var fs = SelectedAppointment.SelectedQForm.FormSections.IndexOf(SelectedAppointment.SelectedQForm.FormSections[0]);

                    foreach (string[] arrPreset in FormPresetNames)
                    {
                        var radioPreset = GetPreset(arrPreset[3], arrPreset[1], arrPreset[2], fs, SelectedAppointment.SelectedQForm.FormSections[0].SectionId, formPresetGroup, SelectedAppointment.SelectedQForm.FormSections[0], formPresetSection, origJson, sectionFormSections, IsDoctorForm);

                        //var radioPreset = new PresetRadioElement(arrPreset[1], "FormPresetAnswers");
                        //radioPreset.PresetName = arrPreset[1];
                        //radioPreset.PresetJson = arrPreset[2];
                        //radioPreset.OnSelected += delegate (object sender, EventArgs e)
                        //{
                        //	string presetJson = arrPreset[2];
                        //	JsonHandler.OriginalFormJsonString = presetJson;
                        //	SelectedAppointment.SelectedQForm = JsonConvert.DeserializeObject<QForm>(presetJson);
                        //	LoadSectionView(SelectedAppointment.SelectedQForm.FormSections[0].SectionId, SelectedAppointment.SelectedQForm.FormSections[0].SectionName, SelectedAppointment.SelectedQForm.FormSections[0], IsDoctorForm);
                        //};

                        formPresetSection.Add(radioPreset);
                    }

                    var btnNewFormPreset = new GlassButton(new RectangleF(0, 0, (float)View.Frame.Width, 50));
                    //btnNewFormPreset.Font = UIFont.BoldSystemFontOfSize(17);
                    btnNewFormPreset.TitleLabel.Font = UIFont.BoldSystemFontOfSize(17);
                    btnNewFormPreset.SetTitleColor(UIColor.Black, UIControlState.Normal);
                    btnNewFormPreset.NormalColor = UIColor.FromRGB(224, 238, 240);
                    btnNewFormPreset.SetTitle("Save New Form Preset", UIControlState.Normal);
                    btnNewFormPreset.TouchUpInside += (sender, e) =>
                    {
                        /*
                         * TODO: popup to enter preset name (DONE?)
                        */

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
                    var section = new SectionStringElement(fSection.SectionName, delegate
                    {
                        LoadSectionView(fSection.SectionId, fSection.SectionName, fSection, IsDoctorForm, sectionFormSections);
                        foreach (Element d in sectionFormSections.Elements)
                        {
                            var t = d.GetType();
                            if (t == typeof(SectionStringElement))
                            {
                                var di = (SectionStringElement)d;
                                di.selected = false;
                            }
                        }

                            //var shhh = sectionFormSections.GetContainerTableView();
                            sectionFormSections.GetContainerTableView().ReloadData();
                    });

                    sectionFormSections.Add(section);
                    //sectionFormSections.Add(new StringElement(fSection.SectionName, delegate { LoadSectionView(fSection.SectionId, fSection.SectionName, fSection, IsDoctorForm); }));
                }

                var finalizeSection = new SectionStringElement("Finalize", delegate
                {
                    LoadSectionView("", "Finalize", null, IsDoctorForm, sectionFormSections);

                    foreach (Element d in sectionFormSections.Elements)
                    {
                        var t = d.GetType();
                        if (t == typeof(SectionStringElement))
                        {
                            var di = (SectionStringElement)d;
                            di.selected = false;
                        }
                    }
                    sectionFormSections.GetContainerTableView().ReloadData();
                });

                sectionFormSections.Add(finalizeSection);

                //sectionFormSections.Add(new StringElement("Finalize", delegate { LoadSectionView("", "Finalize", null, IsDoctorForm); }));

                rootFormSections.Add(sectionFormSections);

                var formDVC = new DynaDialogViewController(rootFormSections, true, false);
                //formDVC.IsForm = true;

                string jsonEnding = IsDoctorForm ? "doctor" : "patient";
                var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                var directoryname = Path.Combine(documents, "DynaRestore");
                //var filename = Path.Combine(directoryname, SelectedAppointment.ApptId + "_" + SelectedAppointment.SelectedQForm.FormId + "_" + jsonEnding + ".json");
                var fileidentity = SelectedAppointment.ApptId + "_" + SelectedAppointment.SelectedQForm.FormId + "_" + jsonEnding;
                //var showrestore = false;
                //UIBarButtonItem restorebtn = new UIBarButtonItem();

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

                    restorebtn = new UIBarButtonItem(UIImage.FromBundle("Restore"), UIBarButtonItemStyle.Bordered, delegate
                    {
                        //var content = new UIViewController();

                        var nlab = new UILabel(new CGRect(5, 5, 350, 50));
                        nlab.Text = "Restore Form";

                        var ncellHeader = new UITableViewCell(UITableViewCellStyle.Default, null);
                        ncellHeader.Frame = new CGRect(0, 0, 350, 50);

                        var nheadclosebtn = new UIButton(new CGRect(300, 5, 50, 50));
                        nheadclosebtn.SetImage(UIImage.FromBundle("Close"), UIControlState.Normal);

                        ncellHeader.ContentView.Add(nlab);
                        ncellHeader.ContentView.Add(nheadclosebtn);

                        var nsec = new Section(ncellHeader);
                        nsec.FooterView = new UIView(new CGRect(0, 0, 0, 0));
                        nsec.FooterView.Hidden = true;

                        foreach (var fi in backups)
                        {
                            var btn = new UIButton(new CGRect(0, 0, 350, 50));
                            btn.SetTitle(fi.CreationTime.ToString(), UIControlState.Normal);
                            btn.SetTitleColor(UIColor.Blue, UIControlState.Normal);
                            btn.TouchUpInside += delegate {
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

                                RestorePrompt.Add(messageLabel);
                                RestorePrompt.AddAction(UIAlertAction.Create("Restore", UIAlertActionStyle.Default, action => RestoreForm(RestorePrompt.TextFields[0].Text, restoreFile, IsDoctorForm, sourceJObject, targetJObject, sectionFormSections)));
                                RestorePrompt.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, null));

                                //Present Alert
                                DetailViewController.PresentViewController(RestorePrompt, true, null);
                            };

                            //content.View.AddSubview(btn);
                            nsec.Add(btn);
                        }

                        //DetailViewPopover = new UIPopoverController(content);
                        //DetailViewPopover.PopoverContentSize = new CGSize(320, 320);
                        //DetailViewPopover.DidDismiss += delegate { LastTappedButton = null; };

                        //PresentViewController(content, true, null);
                        //DetailViewPopover.PresentFromRect(DetailViewController.NavigationController.NavigationBar.Frame, DetailViewPopover, UIPopoverArrowDirection.Any, true);



                          var nroo = new RootElement("Restore");
                          nroo.Add(nsec);

                          var ndia = new DialogViewController(nroo);
                        ndia.TableView.ScrollEnabled = false;
                          ndia.ModalInPopover = true;
                        ndia.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
                          ndia.PreferredContentSize = new CGSize(350, 300);

                          nheadclosebtn.TouchUpInside += delegate
                          {
                            DetailViewController.DismissViewController(true, null);
                          };

                        DetailViewController.PresentViewController(ndia, true, null);
                    });


                    DetailViewController.NavigationItem.SetRightBarButtonItem(restorebtn, true);
                }
                 
                if (IsDoctorForm)
                {
                    messageLabel = new UILabel();

					//var backdiscard = new UIBarButtonItem(UIImage.FromBundle("Delete"), UIBarButtonItemStyle.Plain, delegate {
                    var backdiscard = new UIBarButtonItem(UIBarButtonSystemItem.Cancel, delegate {
                        var BackPrompt_DoctorDiscard = UIAlertController.Create("Discard and Exit", "** WARNING **" + Environment.NewLine + "This will discard any changes, do you wish to continue? " + Environment.NewLine + " ** WARNING **", UIAlertControllerStyle.Alert);

						BackPrompt_DoctorDiscard.Add(messageLabel);
						BackPrompt_DoctorDiscard.AddAction(UIAlertAction.Create("Yes", UIAlertActionStyle.Default, action => PopBack(null, IsDoctorForm, false, sectionFormSections)));
						BackPrompt_DoctorDiscard.AddAction(UIAlertAction.Create("No", UIAlertActionStyle.Cancel, null));

						//Present Alert
						PresentViewController(BackPrompt_DoctorDiscard, true, null);
					});

					//var backsave = new UIBarButtonItem(UIImage.FromBundle("Back"), UIBarButtonItemStyle.Plain, delegate {
                    var backsave = new UIBarButtonItem(UIBarButtonSystemItem.Done, delegate {
						var BackPrompt_DoctorDiscard = UIAlertController.Create("Save and Exit", "Save changes?", UIAlertControllerStyle.Alert);

						BackPrompt_DoctorDiscard.Add(messageLabel);
						BackPrompt_DoctorDiscard.AddAction(UIAlertAction.Create("Yes", UIAlertActionStyle.Default, action => PopBack(null, IsDoctorForm, true, sectionFormSections)));
						BackPrompt_DoctorDiscard.AddAction(UIAlertAction.Create("No", UIAlertActionStyle.Cancel, null));

						//Present Alert
						PresentViewController(BackPrompt_DoctorDiscard, true, null);
					});

                    //formDVC.NavigationItem.SetLeftBarButtonItems(new UIBarButtonItem[] { backsave, backdiscard }, true);
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
                //	messageLabel = new UILabel();
                //	formDVC.NavigationItem.LeftBarButtonItem = new UIBarButtonItem(UIImage.FromBundle("Lock"), UIBarButtonItemStyle.Bordered, delegate (object sender, EventArgs e)
                //  	{
                //		  //Create Alert
                //		  var BackPrompt = UIAlertController.Create("Exit Form", "Administrative use only. Please enter password to continue", UIAlertControllerStyle.Alert);
                //		  BackPrompt.AddTextField((field) =>
                //		  {
                //			  field.SecureTextEntry = true;
                //			  field.Placeholder = "Password";
                //		  });

                //		  BackPrompt.Add(messageLabel);
                //		  BackPrompt.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, action => PopBack(BackPrompt.TextFields[0].Text)));
                //		  BackPrompt.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, null));

                //		  //Present Alert
                //		  PresentViewController(BackPrompt, true, null);
                //  	});
                //	//formDVC.NavigationItem.LeftBarButtonItem.Title = "Back";
                //}

                var firstid = 0;
                if (IsDoctorForm) { firstid = 1; }
                var q = (SectionStringElement)sectionFormSections[firstid];
                q.selected = true;
                //q.GetContainerTableView().ReloadData();
                //sectionFormSections.GetContainerTableView().SelectRow(sectionFormSections.Elements[1].IndexPath, true, UITableViewScrollPosition.Top);
                //var shhh = sections.GetContainerTableView();
                //sectionFormSections.GetContainerTableView().ReloadData();
                //ReValidate(SelectedAppointment.SelectedQForm.FormSections[0], IsDoctorForm, sectionFormSections, 0);
                LoadSectionView(SelectedAppointment.SelectedQForm.FormSections[0].SectionId, SelectedAppointment.SelectedQForm.FormSections[0].SectionName, SelectedAppointment.SelectedQForm.FormSections[0], IsDoctorForm, sectionFormSections);

						//loadingOverlay.Hide();

                return formDVC;
                //}

                //PresentViewController(CommonFunctions.InternetAlertPrompt(), true, null);
                //return new DynaDialogViewController(new RootElement("No internet"), true);
            }
			catch (Exception ex)
			{
                //loadingOverlay.Hide();
				CommonFunctions.sendErrorEmail(ex);
                PresentViewController(CommonFunctions.ExceptionAlertPrompt(), true, null);
				return new DynaDialogViewController(CommonFunctions.ErrorRootElement(), true);
				//throw new Exception(ex.Message + Environment.NewLine + ex.StackTrace, ex.InnerException);
			}
            //finally
            //{
            //    loadingOverlay.Hide();
            //}
		}


		void RestoreForm(string password, string restoreFile, bool IsDoctorForm, JObject sourceJObject, JObject targetJObject, Section sections)
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

				if (isValid)
				{
					//if (DetailViewController.QuestionsView != null)
					//{
					//	DetailViewController.Title = "";
					//	DetailViewController.QuestionsView = null; //.Clear();
					//}
					if (!JToken.DeepEquals(sourceJObject, targetJObject))
					{
						JsonHandler.OriginalFormJsonString = restoreFile;
						SelectedAppointment.SelectedQForm = JsonConvert.DeserializeObject<QForm>(restoreFile);
						LoadSectionView(SelectedAppointment.SelectedQForm.FormSections[0].SectionId, SelectedAppointment.SelectedQForm.FormSections[0].SectionName, SelectedAppointment.SelectedQForm.FormSections[0], IsDoctorForm, sections);
					}
				}
				else
				{
					PresentViewController(CommonFunctions.AlertPrompt("Error", "Wrong password", true, null, false, null), true, null);
				}
			}
			catch (Exception ex)
			{
				CommonFunctions.sendErrorEmail(ex);
                PresentViewController(CommonFunctions.ExceptionAlertPrompt(), true, null);
				//throw new Exception(ex.Message + Environment.NewLine + ex.StackTrace, ex.InnerException);
			}
		}


		async void PopBack(string password, bool IsDoctorForm, bool save, Section sections = null)
		{
			try
            {
                loadingOverlay = new LoadingOverlay(SplitViewController.View.Bounds);
                var loadmes = save ? "Saveing form. Please wait patiently..." : "Exiting form...";
                loadingOverlay.SetText(loadmes);
                SplitViewController.Add(loadingOverlay);

                await Task.Delay(10);

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
					//isValid |= password == DynaClassLibrary.DynaClasses.LoginContainer.User.DynaPassword;
					isValid |= (password == DynaClassLibrary.DynaClasses.LoginContainer.User.DynaPassword || IsDoctorForm);
				}

				if (isValid)
				{
					//if (DetailViewController.QuestionsView != null)
					//{
					if (save)
					{
						var CanContinue = true;
						string qlist = "";
						List<SectionStringElement> qelements = new List<SectionStringElement>();

						object[,] vsecs = new object[SelectedAppointment.SelectedQForm.FormSections.Count, 2];
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
                                loadingOverlay.Hide();

								return;
							}

							var nextSectionQuestions = SelectedAppointment.SelectedQForm.FormSections[SelectedAppointment.SelectedQForm.FormSections.IndexOf(qSection)];
							nextSectionQuestions.Revalidating = true;
							ValidationAlert(qelements, nextSectionQuestions, IsDoctorForm, sections, firstinvalid);

                            loadingOverlay.Hide();

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
					//	var restoreFile = File.ReadAllText(filename);
					//	var sourceJObject = JsonConvert.DeserializeObject<JObject>(sourceJson);
					//	var targetJObject = JsonConvert.DeserializeObject<JObject>(restoreFile);

					//	if (!JToken.DeepEquals(sourceJObject, targetJObject))
					//	{
					//		// Serialize object
					//		//string restoreJson = JsonConvert.SerializeObject(SelectedAppointment.SelectedQForm);
					//		//string jsonEnding = IsDoctorForm ? "doctor" : "patient";
					//		// Save to file
					//		//var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
					//		//var directoryname = Path.Combine(documents, "DynaRestore");
					//		Directory.CreateDirectory(directoryname);
					//		//var filename = Path.Combine(directoryname, SelectedAppointment.ApptId + "_" + SelectedAppointment.SelectedQForm.FormId + "_" + jsonEnding + ".json");
					//		File.WriteAllText(filename, sourceJson);
					//	}

					//}
					//else
					//{
					//	Directory.CreateDirectory(directoryname);
					//	File.WriteAllText(filename, sourceJson);
					//}

					//DetailViewController.Title = "Welcome to Dynapad";
					DetailViewController.QuestionsView = null; //.Clear();
					DetailViewController.Root.Clear();
					//DetailViewController.Root.Add(new Section("Logged in") 
                    //{
                    //    FooterView = new UIView(new CGRect(0, 0, 0, 0))
                    //    {
                    //        Hidden = true
                    //    }
                    //});
                    //DetailViewController.Root.Caption = "Welcome to Dynapad";
                    DetailViewController.Root.Caption = SelectedAppointment.ApptFormName + " - " + SelectedAppointment.ApptPatientName;
					
                    //DetailViewController.NavigationItem.SetLeftBarButtonItem(null, true);
					//DetailViewController.NavigationItem.SetRightBarButtonItems(null, true);
					DetailViewController.NavigationItem.LeftBarButtonItems = null;
					DetailViewController.NavigationItem.RightBarButtonItems = null;

					DetailViewController.ReloadData();
                    //}

                    loadingOverlay.Hide();

                    NavigationController.PopViewController(true);
				}
				else
                {
                    loadingOverlay.Hide();

					PresentViewController(CommonFunctions.AlertPrompt("Error", "Wrong password", true, null, false, null), true, null);
				}
			}
			catch (Exception ex)
            {
                loadingOverlay.Hide();

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
                PresentViewController(CommonFunctions.ExceptionAlertPrompt(), true, null);
				DetailViewController.Root.Clear();
				DetailViewController.Root.Add(CommonFunctions.ErrorDetailSection());
                DetailViewController.ReloadData();
				//throw new Exception(ex.Message + Environment.NewLine + ex.StackTrace, ex.InnerException);
			}
		}

        void BackSubmitForm(bool IsDoctorForm, Section sections = null)
        {
            try
            {
                var finalJson = JsonConvert.SerializeObject(SelectedAppointment.SelectedQForm);
                var dds = new DynaPadService.DynaPadService() { Timeout = 180000};
				dds.SubmitFormAnswers(CommonFunctions.GetUserConfig(), finalJson, true, IsDoctorForm);
                dds.GenerateSummary(CommonFunctions.GetUserConfig(), finalJson);
            }
            catch (Exception ex)
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
				sections.GetContainerTableView().ReloadData();
				//DetailViewController.Root.Clear();
				//DetailViewController.Root.Add(CommonFunctions.ErrorDetailSection());
				//DetailViewController.ReloadData();
				CommonFunctions.sendErrorEmail(ex);
                PresentViewController(CommonFunctions.ExceptionAlertPrompt(), true, null);
                //throw new Exception(ex.Message + Environment.NewLine + ex.StackTrace, ex.InnerException);
            }
        }


        //void SaveFormPreset(string presetName)
        //{
        //	// doctorid = 123 / 321
        //	// locationid = 321 / 123

        //	string presetJson = JsonConvert.SerializeObject(SelectedAppointment.SelectedQForm);
        //	var dds = new DynaPadService.DynaPadService() { Timeout = 180000};
        //	dds.SaveAnswerPreset(SelectedAppointment.SelectedQForm.FormId, null, SelectedAppointment.ApptDoctorId, true, presetName, presetJson, SelectedAppointment.ApptLocationId, null);
        //}






        void SaveFormPreset(string presetId, string presetName, string sectionId, Section presetSection, PresetRadioElement pre, RadioGroup presetGroup, string origS, Section sectionFormSections, bool IsDoctorForm = true)
		{
			try
			{
				if (CrossConnectivity.Current.IsConnected)
				{
					var sectionQuestions = SelectedAppointment.SelectedQForm.FormSections.Find((FormSection obj) => obj.SectionId == sectionId);
					var fs = SelectedAppointment.SelectedQForm.FormSections.IndexOf(sectionQuestions);

					var presetJson = JsonConvert.SerializeObject(SelectedAppointment.SelectedQForm);
					var dds = new DynaPadService.DynaPadService() { Timeout = 180000};
					dds.SaveAnswerPreset(CommonFunctions.GetUserConfig(), SelectedAppointment.SelectedQForm.FormId, null, SelectedAppointment.ApptDoctorId, true, presetName, presetJson, SelectedAppointment.ApptLocationId, presetId);

					if (presetId == null)
					{
						var mre = GetPreset(presetId, presetName, presetJson, fs, sectionId, presetGroup, sectionQuestions, presetSection, origS, sectionFormSections, IsDoctorForm);

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
						//presetSection.GetImmediateRootElement().RadioSelected = 0;
						presetSection.GetImmediateRootElement().Reload(pre, UITableViewRowAnimation.Fade);
						//presetSection.GetImmediateRootElement().Reload(presetSection, UITableViewRowAnimation.Fade);
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
					//presetSection.GetContainerTableView().RemoveFromSuperview();
					//QuestionsView.TableView.ReloadData();
					//SetDetailItem(new Section(sectionQuestions.SectionName), "", sectionId, origS, isDoctorInput, nextbtn);

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
				//foreach (Element d in sectionFormSections.Elements)
				//{
				//	var t = d.GetType();
				//	if (t == typeof(SectionStringElement))
				//	{
				//		var di = (SectionStringElement)d;
				//		if (di.selected == true)
				//		{
				//			di.selected = false;
				//		}
				//	}
				//}

				//var q = (SectionStringElement)sectionFormSections[1];
				//q.selected = true;

				//LoadSectionView(SelectedAppointment.SelectedQForm.FormSections[0].SectionId, SelectedAppointment.SelectedQForm.FormSections[0].SectionName, SelectedAppointment.SelectedQForm.FormSections[0], IsDoctorForm, sectionFormSections);

				NavigationController.PopViewController(true);

				CommonFunctions.sendErrorEmail(ex);
                PresentViewController(CommonFunctions.ExceptionAlertPrompt(), true, null);
				//throw new Exception(ex.Message + Environment.NewLine + ex.StackTrace, ex.InnerException);
			}
		}

		void DeleteFormPreset(string presetId, string presetName, string sectionId, Section presetSection, PresetRadioElement pre, RadioGroup presetGroup, string origS, bool IsDoctorForm = true)
		{
			try
			{
				if (CrossConnectivity.Current.IsConnected)
				{
					var sectionQuestions = SelectedAppointment.SelectedQForm.FormSections.Find((FormSection obj) => obj.SectionId == sectionId);
					var fs = SelectedAppointment.SelectedQForm.FormSections.IndexOf(sectionQuestions);

					var presetJson = JsonConvert.SerializeObject(SelectedAppointment.SelectedQForm);
					var dds = new DynaPadService.DynaPadService() { Timeout = 180000};
					dds.DeleteAnswerPreset(CommonFunctions.GetUserConfig(), SelectedAppointment.SelectedQForm.FormId, null, SelectedAppointment.ApptDoctorId, presetId);

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

		public PresetRadioElement GetPreset(string presetId, string presetName, string presetJson, int fs, string sectionId, RadioGroup presetGroup, FormSection sectionQuestions, Section presetSection, string origS, Section sectionFormSections, bool IsDoctorForm)
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

				var mre = new PresetRadioElement(presetName, "FormPresetAnswers");
				mre.PresetID = presetId;
				mre.PresetName = presetName;
				mre.PresetJson = presetJson;
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

					//var selectedSection = SelectedAppointment.SelectedQForm.FormSections.Find((FormSection obj) => obj.SectionId == sectionId);
					//if (selectedSection != null)
					//{
					//	selectedSection.SectionSelectedTemplateId = presetGroup.Selected;
					//}
					var selectedSection = SelectedAppointment.SelectedQForm;
					if (selectedSection != null)
					{
						selectedSection.FormSelectedTemplateId = presetGroup.Selected;
					}

					//var ass = SelectedAppointment.SelectedQForm.FormSections[0];
					//var nextSectionIndex = new int();

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
					//sectionFormSections.GetContainerTableView().SelectRow(sectionFormSections.Elements[1].IndexPath, true, UITableViewScrollPosition.Top);
					//var shhh = sections.GetContainerTableView();
					sectionFormSections.GetContainerTableView().ReloadData();

					//dfElement.RadioSelected = 0;
					//dfElement.GetImmediateRootElement().RadioSelected = 0;
					//dfElement.GetImmediateRootElement().Reload(dfElement, UITableViewRowAnimation.Fade);
					//presetSection.GetImmediateRootElement().Reload(presetSection, UITableViewRowAnimation.Fade);
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
					UpdatePresetPrompt.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, action => DeleteFormPreset(mre.PresetID, mre.PresetName, sectionId, presetSection, mre, presetGroup, origS)));
					UpdatePresetPrompt.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, null));
					//Present Alert

					PresentViewController(UpdatePresetPrompt, true, null);
				};

				return mre;
			}
			catch (Exception ex)
			{
				CommonFunctions.sendErrorEmail(ex);
                PresentViewController(CommonFunctions.ExceptionAlertPrompt(true), true, null);
                return null;
				//throw new Exception(ex.Message + Environment.NewLine + ex.StackTrace, ex.InnerException);
			}
		}


		public class InvalidQuestion
		{
			public string QuestionID { get; set; }
			public string QuestionText { get; set; }
		}

		//public bool ValidateSection(FormSection OrigSection)
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
				CommonFunctions.sendErrorEmail(ex);
                PresentViewController(CommonFunctions.ExceptionAlertPrompt(), true, null);
				return null;
				//throw new Exception(ex.Message + Environment.NewLine + ex.StackTrace, ex.InnerException);
			}
		}



		public void FinalizationLogic(FormSection OrigSection, string sectionName, string sectionId, bool IsDoctorForm, GlassButton btnNextSection, Section sections, bool IsFinalize)
		{
            //try
            //{
                var CanContinue = true;
                string qlist = "";
                List<SectionStringElement> qelements = new List<SectionStringElement>();

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
                    if (IsFinalize)
                    {
                        var origSectionJson = JsonConvert.SerializeObject(OrigSection);
                        if (DetailViewController.NavigationController != null) DetailViewController.NavigationController.PopViewController(true);
                        DetailViewController.SetDetailItem(new Section(sectionName), sectionName, sectionId, origSectionJson, IsDoctorForm, btnNextSection);
                    }
                    else
                    {
                        BackSubmitForm(IsDoctorForm, sections);

                        string jsonEnding = IsDoctorForm ? "doctor" : "patient";
                        var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                        var directoryname = Path.Combine(documents, "DynaRestore");
                        var filename = Path.Combine(directoryname, SelectedAppointment.ApptId + "_" + SelectedAppointment.SelectedQForm.FormId + "_" + jsonEnding + ".json");
                        var sourceJson = JsonConvert.SerializeObject(SelectedAppointment.SelectedQForm);

                        if (File.Exists(filename))
                        {
                            var restoreFile = File.ReadAllText(filename);
                            var sourceJObject = JsonConvert.DeserializeObject<JObject>(sourceJson);
                            var targetJObject = JsonConvert.DeserializeObject<JObject>(restoreFile);

                            if (!JToken.DeepEquals(sourceJObject, targetJObject))
                            {
                                Directory.CreateDirectory(directoryname);
                                File.WriteAllText(filename, sourceJson);
                            }

                        }
                        else
                        {
                            Directory.CreateDirectory(directoryname);
                            File.WriteAllText(filename, sourceJson);
                        }

                        DetailViewController.QuestionsView = null; //.Clear();
    					DetailViewController.Root.Clear();
                        //DetailViewController.Root.Add(new Section("Logged in")
                        //{
                        //    FooterView = new UIView(new CGRect(0, 0, 0, 0))
                        //    {
                        //        Hidden = true
                        //    }
                        //});
                        DetailViewController.Root.Caption = "";
                        DetailViewController.NavigationItem.LeftBarButtonItems = null;
                        DetailViewController.NavigationItem.RightBarButtonItems = null;
                        DetailViewController.ReloadData();

                        NavigationController.PopViewController(true);
                    }
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
                        return;
                    }

                    var nextSectionQuestions = SelectedAppointment.SelectedQForm.FormSections[SelectedAppointment.SelectedQForm.FormSections.IndexOf(qSection)];
                    nextSectionQuestions.Revalidating = true;

                    ValidationAlert(qelements, nextSectionQuestions, IsDoctorForm, sections, firstinvalid);
                }
			//}
			//catch (Exception ex)
			//{
			//	CommonFunctions.sendErrorEmail(ex);
                PresentViewController(CommonFunctions.ExceptionAlertPrompt(), true, null);
			//	//throw new Exception(ex.Message + Environment.NewLine + ex.StackTrace, ex.InnerException);
			//}
		}


		void LoadSectionView(string sectionId, string sectionName, FormSection OrigSection, bool IsDoctorForm, Section sections = null) 		{
			try
			{
                BackUp(IsDoctorForm);

				ReloadData();
				var btnNextSection = GetNextBtn(sections, IsDoctorForm);

				//if (sectionName != "Report" || sectionName != "Finalize" || sectionName != "Photos")
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
					List<SectionStringElement> qelements = new List<SectionStringElement>();

					object[,] vsecs = new object[SelectedAppointment.SelectedQForm.FormSections.Count, 2];
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
							qelements.Add(new SectionStringElement(vsec.SectionName + ":") { vsection = true } );

							foreach (InvalidQuestion q in invalidquestions)
							{
								qelements.Add(new SectionStringElement(" - " + q.QuestionText) { vquestion = true } );
								qlist = qlist + "\n" + vsec.SectionName + " - " + q.QuestionText;
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
						//	var t = d.GetType();
						//	if (t == typeof(SectionStringElement))
						//	{
						//		var di = (SectionStringElement)d;
						//		di.selected = false;
						//	}
						//}

						//if (IsDoctorForm) { firstinvalid = firstinvalid + 1; }

						//var q = (SectionStringElement)sections.Elements[firstinvalid];
						//q.selected = true;

						//      sections.GetContainerTableView().SelectRow(sections.Elements[firstinvalid].IndexPath, true, UITableViewScrollPosition.Top);
						//sections.GetContainerTableView().ReloadData();

						var nextSectionQuestions = SelectedAppointment.SelectedQForm.FormSections[SelectedAppointment.SelectedQForm.FormSections.IndexOf(qSection)];
						nextSectionQuestions.Revalidating = true;
						//nextSectionQuestions.RevalidatingY = firstinvalidquestiony;
						//nextSectionQuestions.InvalidIndexPath = asspath;

						//LoadSectionView(nextSectionQuestions.SectionId, nextSectionQuestions.SectionName, nextSectionQuestions, IsDoctorForm, sections);
						ValidationAlert(qelements, nextSectionQuestions, IsDoctorForm, sections, firstinvalid);
						//PresentViewController(CommonFunctions.AlertPrompt("Validation Error", "Please provide the required information (marked with a red asterisk):" + qlist, true, action => ReValidate(nextSectionQuestions, IsDoctorForm, sections, firstinvalid), false, null), true, null);
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
                //NavigationController.PopViewController(true);
				CommonFunctions.sendErrorEmail(ex);
                PresentViewController(CommonFunctions.ExceptionAlertPrompt(), true, null);
				//throw new Exception(ex.Message + Environment.NewLine + ex.StackTrace, ex.InnerException);
			} 		}


		void ValidationAlert(List<SectionStringElement> qlist, FormSection nextSectionQuestions, bool isDoctorForm, Section sections, int firstinvalid)
		{
			var nlab = new UILabel(new CGRect(10, 0, UIScreen.MainScreen.Bounds.Width - 250, 50));
			nlab.Text = "Validation Error";
			nlab.TextAlignment = UITextAlignment.Center;

			var ncellHeader = new UITableViewCell(UITableViewCellStyle.Default, null);
			ncellHeader.Frame = new CGRect(0, 0, UIScreen.MainScreen.Bounds.Width - 250, 50);

			//var nheadclosebtn = new UIButton(new CGRect(UIScreen.MainScreen.Bounds.Width - 300, 0, 50, 50));
			//nheadclosebtn.SetImage(UIImage.FromBundle("Close"), UIControlState.Normal);

			ncellHeader.ContentView.Add(nlab);
			//ncellHeader.ContentView.Add(nheadclosebtn);

			var nsec = new Section(ncellHeader);
			nsec.FooterView = new UIView(new CGRect(0, 0, 0, 0));
			nsec.FooterView.Hidden = true;

			var SettingsView = new DynaMultiRootElement();

			var sSection = new DynaSection("ValidationError");
			sSection.HeaderView = new UIView(new CGRect(0, 0, UIScreen.MainScreen.Bounds.Width - 250, 30));
			sSection.FooterView = new UIView(new CGRect(0, 0, 0, 0));
			sSection.FooterView.Hidden = true;

			var sPaddedView = new PaddedUIView<UILabel>();
			sPaddedView.Frame = new CGRect(0, 0, UIScreen.MainScreen.Bounds.Width - 250, 30);
			sPaddedView.Padding = 5f;
			sPaddedView.NestedView.Text = "Please provide the required information (marked with a red asterisk):";
			sPaddedView.Type = "Validation";
			sPaddedView.setStyle();

			sSection.HeaderView.Add(sPaddedView);

			sSection.AddAll(qlist);

			var btnDismiss = new UIButton(new CGRect(0, 0, UIScreen.MainScreen.Bounds.Width - 250, 40));
			btnDismiss.SetTitle("Return", UIControlState.Normal);
			btnDismiss.SetTitleColor(UIColor.Black, UIControlState.Normal);
			btnDismiss.BackgroundColor = UIColor.FromRGB(224, 238, 240);
			btnDismiss.HorizontalAlignment = UIControlContentHorizontalAlignment.Center;
			btnDismiss.TouchUpInside += delegate {
				ReValidate(nextSectionQuestions, isDoctorForm, sections, firstinvalid);
				NavigationController.DismissViewController(true, null);
			};

			sSection.Add(btnDismiss);

			SettingsView.Add(nsec);
			SettingsView.Add(sSection);

			var nroo = new RootElement("Validations");
			nroo.Add(nsec);

			var ndia = new DialogViewController(SettingsView);
			ndia.ModalInPopover = true;
			ndia.ModalPresentationStyle = UIModalPresentationStyle.PageSheet;
			ndia.PreferredContentSize = new CGSize(View.Bounds.Size);

			NavigationController.PreferredContentSize = new CGSize(View.Bounds.Size);
			NavigationController.PresentViewController(ndia, true, null);
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
				CommonFunctions.sendErrorEmail(ex);
                PresentViewController(CommonFunctions.ExceptionAlertPrompt(), true, null);
				//throw new Exception(ex.Message + Environment.NewLine + ex.StackTrace, ex.InnerException);
			}
		}

		public GlassButton GetNextBtn(Section sections, bool IsDoctorForm)
		{
			try
			{
				var btnNextSection = new GlassButton(new RectangleF(0, 0, (float)DetailViewController.View.Frame.Width, 50));
				//btnNextSection.Font = UIFont.BoldSystemFontOfSize(17);
				btnNextSection.TitleLabel.Font = UIFont.BoldSystemFontOfSize(17);
				btnNextSection.SetTitleColor(UIColor.Black, UIControlState.Normal);
				btnNextSection.NormalColor = UIColor.FromRGB(224, 238, 240);
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
				//var shhh = sections.GetContainerTableView();
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
					//	var restoreFile = File.ReadAllText(filename);
					//	var sourceJObject = JsonConvert.DeserializeObject<JObject>(sourceJson);
					//	var targetJObject = JsonConvert.DeserializeObject<JObject>(restoreFile);

					//	if (!JToken.DeepEquals(sourceJObject, targetJObject))
					//	{
					//		Directory.CreateDirectory(directoryname);
					//		File.WriteAllText(filename, sourceJson);
					//	}
					//}
					//else
					//{
					//	Directory.CreateDirectory(directoryname);
					//	File.WriteAllText(filename, sourceJson);
					//}
				};

				return btnNextSection;
			}
			catch (Exception ex)
			{
				CommonFunctions.sendErrorEmail(ex);
                PresentViewController(CommonFunctions.ExceptionAlertPrompt(true), true, null);
                return null;
				//throw new Exception(ex.Message + Environment.NewLine + ex.StackTrace, ex.InnerException);
			}
		}


        void BackUp(bool IsDoctorForm)
        {
            string jsonEnding = IsDoctorForm ? "doctor" : "patient";
            var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var directoryname = Path.Combine(documents, "DynaRestore");
            var fileidentity = SelectedAppointment.ApptId + "_" + SelectedAppointment.SelectedQForm.FormId + "_" + jsonEnding;
            var filename = fileidentity + "_" + DateTime.Now.ToFileTimeUtc() + ".json";
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


		//async void LoadReportView(string valueId, string sectionName, RootElement rt, string reportName)
        void LoadReportView(string valueId, string sectionName, RootElement rt, string reportName) 		{
			try
			{
				NavigationController.TopViewController.NavigationItem.LeftBarButtonItem = new UIBarButtonItem(UIImage.FromBundle("Back"), UIBarButtonItemStyle.Plain, delegate
				{
					DetailViewController.NavigationItem.RightBarButtonItem = null;
					DetailViewController.Root.Clear();
					//DetailViewController.Root.Add(new Section("Logged in")
					//{
					//	FooterView = new UIView(new CGRect(0, 0, 0, 0))
					//	{
					//		Hidden = true
					//	}
                    //});
                    DetailViewController.Root.Caption = SelectedAppointment.ApptFormName + " - " + SelectedAppointment.ApptPatientName;
					DetailViewController.ReloadData();

					NavigationController.PopViewController(true);
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
                //loadingOverlay.Hide();
				DetailViewController.Root.Clear();
				DetailViewController.Root.Add(CommonFunctions.ErrorDetailSection());
				DetailViewController.ReloadData();
				CommonFunctions.sendErrorEmail(ex);
                PresentViewController(CommonFunctions.ExceptionAlertPrompt(), true, null);
				//throw new Exception(ex.Message + Environment.NewLine + ex.StackTrace, ex.InnerException);
			}
            //finally
            //{
            //    loadingOverlay.Hide();
            //} 		}

	    void LoadSummaryView(string fileName, string sectionName, RootElement rt)
		{
			try
			{
				NavigationController.TopViewController.NavigationItem.LeftBarButtonItem = new UIBarButtonItem(UIImage.FromBundle("Back"), UIBarButtonItemStyle.Plain, delegate 
				{
					DetailViewController.NavigationItem.RightBarButtonItem = null;
					DetailViewController.Root.Clear();
					//DetailViewController.Root.Add(new Section("Logged in")
					//{
						//FooterView = new UIView(new CGRect(0, 0, 0, 0))
						//{
						//	Hidden = true
						//}
                    //});
                    DetailViewController.Root.Caption = "";
					DetailViewController.ReloadData();

					NavigationController.PopViewController(true);
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
                //loadingOverlay.Hide();
				DetailViewController.Root.Clear();
				DetailViewController.Root.Add(CommonFunctions.ErrorDetailSection());
				DetailViewController.ReloadData();
				CommonFunctions.sendErrorEmail(ex);
                PresentViewController(CommonFunctions.ExceptionAlertPrompt(), true, null);
				//throw new Exception(ex.Message + Environment.NewLine + ex.StackTrace, ex.InnerException);
            }
            //finally
            //{
            //    loadingOverlay.Hide();
            //}
		}


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
