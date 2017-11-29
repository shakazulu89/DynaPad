using System;
using System.Collections.Generic;
//using System.Net.Mail;
using MonoTouch.Dialog;
//using DynaClassLibrary;
using UIKit;
using MailKit.Net.Smtp;
using MailKit;
using MimeKit;
using CoreGraphics;

namespace DynaPad
{
	public static class CommonFunctions
	{
		//public static CommonFunctions()
		//{
		//}

		public static DynaPadService.ConfigurationObjects GetUserConfig()
		{
			
			try
			{
				var UserConfig = new DynaPadService.ConfigurationObjects()
				{
					EmailSupport = DynaClassLibrary.DynaClasses.LoginContainer.User.DynaConfig.EmailSupport,
					EmailPostmaster = DynaClassLibrary.DynaClasses.LoginContainer.User.DynaConfig.EmailPostmaster,
					EmailRoy = DynaClassLibrary.DynaClasses.LoginContainer.User.DynaConfig.EmailRoy,
					EmailSmtp = DynaClassLibrary.DynaClasses.LoginContainer.User.DynaConfig.EmailSmtp,
					EmailUser = DynaClassLibrary.DynaClasses.LoginContainer.User.DynaConfig.EmailUser,
					EmailPass = DynaClassLibrary.DynaClasses.LoginContainer.User.DynaConfig.EmailPass,
					EmailPort = DynaClassLibrary.DynaClasses.LoginContainer.User.DynaConfig.EmailPort,
					ConnectionString = DynaClassLibrary.DynaClasses.LoginContainer.User.DynaConfig.ConnectionString,
					ConnectionName = DynaClassLibrary.DynaClasses.LoginContainer.User.DynaConfig.ConnectionName,
					DatabaseName = DynaClassLibrary.DynaClasses.LoginContainer.User.DynaConfig.DatabaseName,
					DomainRootPathVirtual = DynaClassLibrary.DynaClasses.LoginContainer.User.DynaConfig.DomainRootPathVirtual,
					DomainRootPathPhysical = DynaClassLibrary.DynaClasses.LoginContainer.User.DynaConfig.DomainRootPathPhysical,
					DomainClaimantsPathVirtual = DynaClassLibrary.DynaClasses.LoginContainer.User.DynaConfig.DomainClaimantsPathVirtual,
					DomainClaimantsPathPhysical = DynaClassLibrary.DynaClasses.LoginContainer.User.DynaConfig.DomainClaimantsPathPhysical,
					DomainHost = DynaClassLibrary.DynaClasses.LoginContainer.User.DynaConfig.DomainHost
					//DomainPaths = DynaClassLibrary.DynaClasses.LoginContainer.User.DynaConfig.DomainPaths.Select(dPath => new DynaClassLibrary.DynaClasses.DomainPath[]
					//{
					//	DomainPathName = dPath.DomainPathName,
					//	DomainPathVirtual = dPath.DomainPathVirtual,
					//	DomainPathPhysical = dPath.DomainPathPhysical
					//}).ToArray()
				};

				var domList = new List<DynaPadService.DomainPath>();
                foreach (var dPath in DynaClassLibrary.DynaClasses.LoginContainer.User.DynaConfig.DomainPaths)
                {
                    var ServiceDomainPath = new DynaPadService.DomainPath
					{
						DomainPathName = dPath.DomainPathName,
						DomainPathVirtual = dPath.DomainPathVirtual,
						DomainPathPhysical = dPath.DomainPathPhysical
					};
				domList.Add(ServiceDomainPath);
                }
                UserConfig.DomainPaths = domList.ToArray();
				//foreach (var item in domList)
				//{
				//	UserConfig.DomainPaths.Add(item);
				//}

				return UserConfig;
			}
			catch (Exception ex)
			{
                CommonFunctions.sendErrorEmail(ex);
                return null;
				//throw new Exception(ex.Message + Environment.NewLine + ex.StackTrace, ex.InnerException);
			}
		}

		public static UIAlertController AlertPrompt(string alertTitle, string alertMessage, bool OKButton, Action<UIAlertAction> OKAction, bool CancelButton, Action<UIAlertAction> CancelAction)
		{
			var prompt = UIAlertController.Create(alertTitle, alertMessage, UIAlertControllerStyle.Alert);
			if (OKButton)
			{
				prompt.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, OKAction));
			}
			if (CancelButton)
			{
				prompt.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, CancelAction));
			}

			return prompt;
		}

		public static UIAlertController InternetAlertPrompt()
		{
			var prompt = UIAlertController.Create("No Internet Connection", "An active internet connection is required", UIAlertControllerStyle.Alert);
			prompt.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
			return prompt;
		}

		public static UIAlertController ExceptionAlertPrompt(bool ReloginPrompt = false)
		{
            var relogin = "try again.";
            if (ReloginPrompt)
            {
                relogin = "re-login to ensure form loads properly.";
            }
            var message = "An error has occurred, " + relogin;
            var prompt = UIAlertController.Create("Error", message, UIAlertControllerStyle.Alert);
			prompt.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
			return prompt;
		}

        public static RootElement ErrorRootElement()
        {
            var ErrorRoot = new RootElement("Error");
            return ErrorRoot;
        }

        public static Section ErrorDetailSection()
        {
            var ErrorSection = new Section("Error. Please retry. If issue persists, contact support.")
            {
                FooterView = new UIView(new CGRect(0, 0, 0, 0))
                {
                    Hidden = true
                }
            };
            return ErrorSection;
        }

        static string emailFrom = "info@dynadox.pro";//"dharel.cm@gmail.com";//DynaClassLibrary.DynaClasses.LoginContainer.User.DynaConfig.EmailSupport;//ConfigurationManager.AppSettings.Get("emailFrom");
        static string emailTo = "dharel.cm@gmail.com";//DynaClassLibrary.DynaClasses.LoginContainer.User.DynaConfig.EmailSupport;//ConfigurationManager.AppSettings.Get("emailRoy");
        static string emailCc1 = "rharel89@gmail.com";
        static string emailSMTP = "mewebmail.dynadox.pro";//"smtp.gmail.com";//DynaClassLibrary.DynaClasses.LoginContainer.User.DynaConfig.EmailSmtp;//ConfigurationManager.AppSettings.Get("emailSMTP");
        static string emailUser = "info@dynadox.pro";//"dharel.cm@gmail.com";//DynaClassLibrary.DynaClasses.LoginContainer.User.DynaConfig.EmailUser;//ConfigurationManager.AppSettings.Get("emailUser");
        static string emailPass = "jxKr:9e";//'"madona007";//DynaClassLibrary.DynaClasses.LoginContainer.User.DynaConfig.EmailPass;//ConfigurationManager.AppSettings.Get("emailPass");
        static int emailPort = 25;//587;//Convert.ToInt32(DynaClassLibrary.DynaClasses.LoginContainer.User.DynaConfig.EmailPort);//ConfigurationManager.AppSettings.Get("emailPort"));   

        public static void sendNSErrorEmail(Foundation.NSError exception)
        {
			try
			{
				string subject = "DynaPad Error - on " + DateTime.Now.ToShortDateString();
                string errorMessage = "CODE:<br/><br/>" + exception.Code + "<br/><br/><br/>DOMAIN:<br/><br/>" + exception.Domain + "<br/><br/><br/>USER INFO:<br/><br/>" + exception.UserInfo + "<br/><br/><br/>LOCAL DESC:<br/><br/>" + exception.LocalizedDescription + "<br/><br/><br/>LOCAL FAIL REASON:<br/><br/>" + exception.LocalizedFailureReason + "<br/><br/><br/>LOCAL RECOVER SUGGESTION:<br/><br/>" + exception.LocalizedRecoverySuggestion;

				var message = new MimeMessage();
				message.From.Add(new MailboxAddress("DynaPad App", emailFrom));
                message.To.Add(new MailboxAddress("DynaDox Support", emailTo));
                message.Cc.Add(new MailboxAddress("DynaDox Support", emailCc1));
				message.Subject = subject;

				message.Body = new TextPart("html")
				{
					Text = errorMessage
				};

				using (var client = new SmtpClient())
				{
					// For demo-purposes, accept all SSL certificates (in case the server supports STARTTLS)
					//client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                    client.Connect(emailSMTP, emailPort);

					// Note: since we don't have an OAuth2 token, disable
					// the XOAUTH2 authentication mechanism.
					client.AuthenticationMechanisms.Remove("XOAUTH2");

					// Note: only needed if the SMTP server requires authentication
                    client.Authenticate(emailUser, emailPass);

					client.Send(message);
					client.Disconnect(true);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
            }
        }

        public static void sendErrorEmail(Exception exception, string additionalInfo = null)
		{
			try
			{
                string subject = "DynaPad Error - on " + DateTime.Now.ToShortDateString();
                string errorMessage = "MESSAGE:<br/><br/>" + exception.Message + "<br/><br/><br/>TRACE:<br/><br/>" + exception.StackTrace;
                if (!string.IsNullOrEmpty(additionalInfo))
                {
                    errorMessage = errorMessage + additionalInfo;
                }                          

				var message = new MimeMessage();
                message.From.Add(new MailboxAddress("DynaPad App", emailFrom));
                message.To.Add(new MailboxAddress("DynaDox Support", emailTo));
                message.Cc.Add(new MailboxAddress("DynaDox Support", emailCc1));
				message.Subject = subject;

				message.Body = new TextPart("html")
				{
                    Text = errorMessage
				};

				using (var client = new SmtpClient())
				{
					// For demo-purposes, accept all SSL certificates (in case the server supports STARTTLS)
					//client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                    client.Connect(emailSMTP, emailPort);

					// Note: since we don't have an OAuth2 token, disable
					// the XOAUTH2 authentication mechanism.
					client.AuthenticationMechanisms.Remove("XOAUTH2");

					// Note: only needed if the SMTP server requires authentication
                    client.Authenticate(emailUser, emailPass);

					client.Send(message);
					client.Disconnect(true);
				}

				//MailMessage ErrorEmail = new MailMessage
				//{
				//	Subject = subject,
				//	From = new MailAddress(emailFrom, "DynaPad Error"),
				//	Body = errorMessage
				//};
				//ErrorEmail.To.Add(new MailAddress(emailTo, "DynaDox"));

				//SmtpClient smtp = new SmtpClient(emailSMTP, emailPort)
				//{
				//	EnableSsl = false,
				//	DeliveryMethod = SmtpDeliveryMethod.Network,
				//	Credentials = new System.Net.NetworkCredential(emailUser, emailPass)
				//};

				//smtp.Send(ErrorEmail);
			}
			catch (Exception ex)
			{
                Console.WriteLine(ex.Message);
            }

		}
	}
}
