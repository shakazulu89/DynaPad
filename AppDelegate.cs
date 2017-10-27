using System;
using System.IO;
using Foundation;
using HockeyApp.iOS;
using UIKit;

namespace DynaPad
{
	// The UIApplicationDelegate for the application. This class is responsible for launching the
	// User Interface of the application, as well as listening (and optionally responding) to application events from iOS.
	[Register("AppDelegate")]
	public class AppDelegate : UIApplicationDelegate, IUISplitViewControllerDelegate
	{
		// class-level declarations

		public override UIWindow Window
		{
			get;
			set;
		}

		//UINavigationController navigation;
		//const string footer =
		//"These show the two sets of APIs\n" +
		//"available in MonoTouch.Dialogs";

		public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
		{
			var manager = BITHockeyManager.SharedHockeyManager;
			manager.Configure("25bed55ebff84002b3e57c0ed3c0d66f");
			//manager.Authenticator.IdentificationType = BITAuthenticatorIdentificationType.HockeyAppUser;
			//manager.DisableUpdateManager = true;
			manager.StartManager();
			manager.Authenticator.AuthenticateInstallation(); // This line is obsolete in crash only builds

   //         System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();
   //         timer.Start();
   //         var dds = new DynaPadService.DynaPadService();
   //         var hello = dds.HelloWorld();
   //         timer.Stop();

   //         if(timer.Elapsed.Seconds > 4)
			//{
            //    Exception ex = new Exception("hellow world took " + timer.Elapsed.Seconds + " seconds");
            //    CommonFunctions.sendErrorEmail(ex);
            //}

			//WritePadAPI.recoInit();
			//WritePadAPI.initializeFlags();

			// Override point for customization after application launch.
			var splitViewController = (UISplitViewController)Window.RootViewController;
			var navigationController = (UINavigationController)splitViewController.ViewControllers[1];
			navigationController.TopViewController.NavigationItem.LeftBarButtonItem = splitViewController.DisplayModeButtonItem;
			splitViewController.WeakDelegate = this;

			var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			var directoryname = Path.Combine(documents, "DynaRestore");
            //var d = new DirectoryInfo(directoryname);

            //foreach (FileInfo fi in d.GetFiles())
            //{
            //	if fi.CreationTime
            //}
            try
            {
                if (Directory.Exists(directoryname))
                {
                    //string[] restorefiles = Directory.GetFiles(directoryname);
                    foreach (var file in Directory.GetFiles(directoryname))
                    {
                        if ((File.GetCreationTime(file) - DateTime.Today).TotalDays > 2)
                        {
                            File.Delete(file);
                        }
                    }
                }
            }
            catch(Exception ex)
			{
				CommonFunctions.sendErrorEmail(ex);
                return false;
            }

			return true;
		}

		public override void OnResignActivation(UIApplication application)
		{
			// Invoked when the application is about to move from active to inactive state.
			// This can occur for certain types of temporary interruptions (such as an incoming phone call or SMS message) 
			// or when the user quits the application and it begins the transition to the background state.
			// Games should use this method to pause the game.
		}

		public override void DidEnterBackground(UIApplication application)
		{
			// Use this method to release shared resources, save user data, invalidate timers and store the application state.
			// If your application supports background exection this method is called instead of WillTerminate when the user quits.
		}

		public override void WillEnterForeground(UIApplication application)
		{
			// Called as part of the transiton from background to active state.
			// Here you can undo many of the changes made on entering the background.
		}

		public override void OnActivated(UIApplication application)
		{
			// Restart any tasks that were paused (or not yet started) while the application was inactive. 
			// If the application was previously in the background, optionally refresh the user interface.
		}

		public override void WillTerminate(UIApplication application)
		{
			// Called when the application is about to terminate. Save data, if needed. See also DidEnterBackground.
		}

		[Export("splitViewController:collapseSecondaryViewController:ontoPrimaryViewController:")]
		public bool CollapseSecondViewController(UISplitViewController splitViewController, UIViewController secondaryViewController, UIViewController primaryViewController)
		{
			if (secondaryViewController.GetType() == typeof(UINavigationController) &&
				((UINavigationController)secondaryViewController).TopViewController.GetType() == typeof(DetailViewController) &&
				((DetailViewController)((UINavigationController)secondaryViewController).TopViewController).DetailItem == null)
			{
				// Return YES to indicate that we have handled the collapse by doing nothing; the secondary controller will be discarded.
				return true;
			}
			return false;
		}
	}
}


