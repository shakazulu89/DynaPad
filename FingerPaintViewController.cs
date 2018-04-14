using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using CoreGraphics;
using Foundation;
using Plugin.Connectivity;
using UIKit;

namespace DynaPad
{
    public partial class FingerPaintViewController : UIViewController
    {
        public bool MREditing;
        public string MREditPath;
        public string MREditId;
        public string MREditName;
        public string MREditType;
        public CGRect fingsize { get; set; }
        public string apptId;
        public string patientId;
        public string doctorId;
        public string locationId;
        public string filename;
        public bool IsDoctorForm;

        UIView contentView;

		LoadingOverlay loadingOverlay;

        // UITextField derivative used for invoking picker views
        class NoCaretField : UITextField
        {
            public NoCaretField() : base(new CGRect())
            {
                BorderStyle = UITextBorderStyle.Line;
            }

            public override CGRect GetCaretRectForPosition(UITextPosition position)
            {
                return new CGRect();
            }
        }

        public FingerPaintViewController() {}

        //protected void HandleImageDownload()
        //{
        //    UIImage img = new UIImage();

        //    //var webClient = new WebClient();
        //    //webClient.DownloadDataCompleted += (s, e) =>
        //    //{
        //    //    var bytes = e.Result; // get the downloaded data
        //    //    string documentsPath = Path.GetTempPath(); //Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        //    //    string localFilename = MREditName;
        //    //    string localPath = Path.Combine(documentsPath, localFilename);
        //    //    File.WriteAllBytes(localPath, bytes); // writes to local storage

        //    //    InvokeOnMainThread(() =>
        //    //    {
        //    //        if (!string.IsNullOrEmpty(localPath))
        //    //        {
        //    //            var imgView = new UIImageView(contentView.Bounds);
        //    //            imgView.Image = UIImage.FromFile(localPath);
        //    //            imgView.ContentMode = UIViewContentMode.ScaleAspectFit; // or ScaleAspectFill
        //    //            contentView.BackgroundColor = UIColor.FromPatternImage(img);
        //    //        }
        //    //    });
        //    //};
        //    //var url = new Uri(MREditPath); // Html home page
        //    //webClient.DownloadStringAsync(url);




        //    string fileIdentifier = "value to remember";
        //    string documentsPath = Path.GetTempPath();
        //    string localFilename = MREditName;
        //    string localFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), localFilename);
        //    WebClient webClient = new WebClient();
        //    webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(DownloadFileCompleted);
        //    webClient.QueryString.Add("file", fileIdentifier); // here you can add values
        //    //webClient.DownloadFileAsync(new Uri((string)MREditPath), localFilePath);
        //    webClient.DownloadFile(new Uri((string)MREditPath), MREditName);
        //}

        //private void DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        //{
        //    string fileIdentifier = ((System.Net.WebClient)(sender)).QueryString["file"];
        //    // process with fileIdentifier

        //    UIImage img = new UIImage();
        //    string localFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), MREditName);
        //    img =  UIImage.FromFile(localFilePath);
        //    var imgView = new UIImageView(contentView.Bounds);
        //    imgView.Image = img;
        //    imgView.ContentMode = UIViewContentMode.ScaleAspectFit; // or ScaleAspectFill
        //    contentView.BackgroundColor = UIColor.FromPatternImage(img);
        //}



        public override void LoadView()
        {
            base.LoadView();

            // White view covering entire screen 
            contentView = new UIView();
            //{
            //    BackgroundColor = UIColor.White
            //};
            if (!MREditing)
            {
                contentView.BackgroundColor = UIColor.White;
                filename = "Doctor-Notes" + "_" + doctorId + "-" + patientId + "_" + DateTime.Now.ToString("s").Replace(":", "_") + ".jpg";
            }



            //var img = UIImage.FromFile("dynapadscreenshot.png");
            //var imgView = new UIImageView(contentView.Bounds);
            //imgView.Image = img;
            //imgView.ContentMode = UIViewContentMode.ScaleAspectFit; // or ScaleAspectFill
            ////contentView.AddSubview(imgView);
            ////contentView.SendSubviewToBack(imgView);
            //contentView.BackgroundColor = UIColor.FromPatternImage(img);


            if (MREditing)
            {
                filename = MREditName + "_" + "edit" + "_" + DateTime.Now.ToString("s").Replace(":", "_") + ".jpg";
                //var img = UIImage.FromFile("dynapadscreenshot.png");
                var img = new UIImage();

                //switch (MREditType)
                //{
                //    case "jpg":
                //    case "gif":
                //    case "png":
                //        img = FromUrl(MREditPath);
                //        break;
                //    case "pdf":
                //    case "doc":
                //    case "docx":
                //        var dps = new DynaPadService.DynaPadService();
                //        //img = dps.ConvertToJPG(CommonFunctions.GetUserConfig(), MREditPath);
                //        break;
                //    default:
                //        img = FromUrl(MREditPath);
                //        break;
                //}


                try
                {
                    loadingOverlay = new LoadingOverlay(contentView.Bounds);
                    contentView.Add(loadingOverlay);

                    ////string localPath = "";
                    var webClient = new WebClient();
                    //webClient.DownloadDataCompleted += (s, e) =>
                    //{
                    //    var bytes = e.Result; // get the downloaded data
                    //    string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal); //Path.GetTempPath(); //Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                    //    string localFilename = MREditName;
                    //    string localPath = Path.Combine(documentsPath, localFilename);
                    //    File.WriteAllBytes(localPath, bytes); // writes to local storage

                    //    InvokeOnMainThread(() =>
                    //    {
                    //        if (!string.IsNullOrEmpty(localPath))
                    //        {
                    //            var imgView = new UIImageView(contentView.Bounds);
                    //            imgView.Image = UIImage.FromFile(localPath);
                    //            imgView.ContentMode = UIViewContentMode.ScaleAspectFit; // or ScaleAspectFill
                    //            contentView.BackgroundColor = UIColor.FromPatternImage(img);
                    //        }
                    //    });
                    //};
                    var url = new Uri(MREditPath);
                    // Html home page
                    ////webClient.DownloadStringAsync(url);
                    ////webClient.DownloadFileAsync(url, MREditName);
                    //webClient.DownloadFile(url, Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), MREditName));
                    ////HandleImageDownload();
                    //img = UIImage.FromFile(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), MREditName));
                    //var imgView2 = new UIImageView(contentView.Bounds);
                    //imgView2.Image = UIImage.FromFile(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), MREditName));
                    //imgView2.ContentMode = UIViewContentMode.ScaleAspectFit; // or ScaleAspectFill
                    //contentView.BackgroundColor = UIColor.White;
                    //contentView.BackgroundColor = UIColor.FromPatternImage(img);



                    var downloadPath = Path.Combine(Path.GetTempPath(), MREditName);

                    var bw = new BackgroundWorker();

                    // this allows our worker to report progress during work
                    bw.WorkerReportsProgress = true;

                    // what to do in the background thread
                    bw.DoWork += delegate (object o, DoWorkEventArgs argss)
                    {
                        var b = o as BackgroundWorker;

                        webClient.DownloadFile(url, downloadPath);
                    };

                    // what to do when worker completes its task (notify the user)
                    bw.RunWorkerCompleted += delegate
                    {
                        var imgPath = Path.Combine(Path.GetTempPath(), MREditName);
                        //NSData imageData = NSData.FromArray(UIImage.FromFile(downloadPath).AsJPEG(1).ToArray());
                        //img = UIImage.FromFile(downloadPath);
                        //img = UIImage.LoadFromData(imageData);
                        img = UIImage.LoadFromData(UIImage.FromFile(downloadPath).AsJPEG(0),0);
                        var imgView = new UIImageView(contentView.Bounds);
                        imgView.Image = UIImage.LoadFromData(UIImage.FromFile(downloadPath).AsJPEG());
                        imgView.ContentMode = UIViewContentMode.Center; // or ScaleAspectFill
                        //contentView.BackgroundColor = UIColor.White;
                        contentView.BackgroundColor = UIColor.FromPatternImage(img);
                    };

                    bw.RunWorkerAsync();








                    ////img = UIImage.FromFile("dynapadscreenshot.png");
                    ////img = UIImage.FromFile("telerikautocomplete.png");
                    ////img = UIImage.FromFile("dynapad_danestreet_screenshot.jpg");
                    ////img = UIImage.FromFile("Dane Street_COLOR.png");
                    ////img = UIImage.FromFile("John_Doe_True_sig_2017-05-09T10_59_08.gif");
                    ////img = UIImage.FromFile(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), MREditName));
                    ////var imgarray = UIImage.FromFile("John_Doe_True_sig_2017-05-09T10_59_08.gif").AsPNG();
                    ////var imgarray = UIImage.FromFile(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), MREditName)).AsPNG();
                    ////img = UIImage.LoadFromData(imgarray);
                    //var imgView = new UIImageView(contentView.Bounds);
                    //imgView.Image = img;
                    //imgView.ContentMode = UIViewContentMode.ScaleAspectFit; // or ScaleAspectFill
                    //contentView.BackgroundColor = UIColor.FromPatternImage(img);
                }
                catch (Exception ex)
				{
					DismissViewController(true, null);
					CommonFunctions.sendErrorEmail(ex);
                    PresentViewController(CommonFunctions.ExceptionAlertPrompt(), true, null);
                }
                finally
                {
                    loadingOverlay.Hide();
                }
            }






            View = contentView;


            // Vertical UIStackView offset from status bar
            CGRect rect = UIScreen.MainScreen.Bounds;
            rect.Y += 20;
            rect.Height -= 20;

            var vertStackView = new UIStackView(rect)
            {
                Axis = UILayoutConstraintAxis.Vertical
            };
            contentView.Add(vertStackView);

            // Horizontal UIStackView for tools
            var horzStackView = new UIStackView
            {
                Axis = UILayoutConstraintAxis.Horizontal,
                Alignment = UIStackViewAlignment.Center,
                Distribution = UIStackViewDistribution.EqualSpacing
            };
            vertStackView.AddArrangedSubview(horzStackView);

            // FingerPaintCanvasView for drawing
            var canvasView = new FingerPaintCanvasView();
            vertStackView.AddArrangedSubview(canvasView);

            // Add space at left to horizontal UIStackView
            horzStackView.AddArrangedSubview(new UILabel(new CGRect(0, 0, 10, 10)));

            // Construct UIPickerView for choosing color, but don't add it to any view
            var colorModel = new PickerDataModel<UIColor>
            {
                Items =
                {
                    new NamedValue<UIColor>("Red", UIColor.Red),
                    new NamedValue<UIColor>("Green", UIColor.Green),
                    new NamedValue<UIColor>("Blue", UIColor.Blue),
                    //new NamedValue<UIColor>("Cyan", UIColor.Cyan),
                    //new NamedValue<UIColor>("Magenta", UIColor.Magenta),
                    new NamedValue<UIColor>("Yellow", UIColor.Yellow),
                    new NamedValue<UIColor>("Black", UIColor.Black),
                    //new NamedValue<UIColor>("Gray", UIColor.Gray),
                    new NamedValue<UIColor>("White", UIColor.White),
                    new NamedValue<UIColor>("Highlight", UIColor.FromRGBA(255,0,255,0.2f)),
                    new NamedValue<UIColor>("Clear", new UIColor(0,0))
                }
            };

            var colorPicker = new UIPickerView
            {
                Model = colorModel
            };

            // Ditto for UIPickerView for stroke thickness
            var thicknessModel = new PickerDataModel<float>
            {
                Items =
				{
					new NamedValue<float>("Pen", 1),
                    new NamedValue<float>("Thin", 2),
                    new NamedValue<float>("Thinish", 5),
                    new NamedValue<float>("Medium", 10),
                    new NamedValue<float>("Thickish", 20),
                    new NamedValue<float>("Thick", 50)
                }
            };

            var thicknessPicker = new UIPickerView
            {
                Model = thicknessModel
            };

            // Create UIToolbar for dismissing picker when it's displayed
            var toolbar = new UIToolbar(new CGRect(0, 0, UIScreen.MainScreen.Bounds.Width, 44))
            {
                BarStyle = UIBarStyle.Default,
                Translucent = true
            };

            // Set Font to be used in tools
            var font = UIFont.SystemFontOfSize(24);

            // Create a NoCaretField text field for invoking color picker & add to horizontal UIStackView
            //  (technique from Xamarin.Forms iOS PickerRenderer
            UITextField colorTextField = new NoCaretField
            {
                Text = "Red",
                InputView = colorPicker,
                InputAccessoryView = toolbar,
                Font = font
            };
            horzStackView.AddArrangedSubview(colorTextField);

            // Use ValueChanged handler to change the color
            colorModel.ValueChanged += (sender, args) =>
            {
                colorTextField.Text = colorModel.SelectedItem.Name;
                canvasView.StrokeColor = colorModel.SelectedItem.Value.CGColor;
            };

            // Ditto for the thickness
            UITextField thicknessTextField = new NoCaretField
            {
                Text = "Pen",
                InputView = thicknessPicker,
                InputAccessoryView = toolbar,
                Font = font
            };
            horzStackView.AddArrangedSubview(thicknessTextField);

            thicknessModel.ValueChanged += (sender, args) =>
            {
                thicknessTextField.Text = thicknessModel.SelectedItem.Name;
                canvasView.StrokeWidth = thicknessModel.SelectedItem.Value;
            };

            // Now add a Done button to the toolbar to rest text fields
            var spacer = new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace);
            var doneButton = new UIBarButtonItem(UIBarButtonSystemItem.Done, (o, a) =>
            {
                colorTextField.ResignFirstResponder();
                thicknessTextField.ResignFirstResponder();
            });

            toolbar.SetItems(new[] { spacer, doneButton }, false);

            // Create the Clear button 
            var button = new UIButton(UIButtonType.RoundedRect);
            button.TitleLabel.Font = font;

            horzStackView.AddArrangedSubview(button);

            button.Layer.BorderColor = UIColor.Black.CGColor;
            button.Layer.BorderWidth = 1;
            button.Layer.CornerRadius = 10;
            button.SetTitle("Clear", UIControlState.Normal);
            button.SetTitleColor(UIColor.Black, UIControlState.Normal);

            button.TouchUpInside += (sender, args) =>
            {
                canvasView.Clear();
            };


            var savebutton = new UIButton(UIButtonType.RoundedRect);
            savebutton.TitleLabel.Font = font;
            horzStackView.AddArrangedSubview(savebutton);

            savebutton.Layer.BorderColor = UIColor.Black.CGColor;
            savebutton.Layer.BorderWidth = 1;
            savebutton.Layer.CornerRadius = 10;
            savebutton.SetTitle("Save", UIControlState.Normal);
            savebutton.SetTitleColor(UIColor.Black, UIControlState.Normal);

            savebutton.TouchUpInside += SaveButtonAction;


            var closebutton = new UIButton(UIButtonType.RoundedRect);
            closebutton.TitleLabel.Font = font;
            horzStackView.AddArrangedSubview(closebutton);

            closebutton.Layer.BorderColor = UIColor.Black.CGColor;
            closebutton.Layer.BorderWidth = 1;
            closebutton.Layer.CornerRadius = 10;
            closebutton.SetTitle("Close", UIControlState.Normal);
            closebutton.SetTitleColor(UIColor.Black, UIControlState.Normal);

            closebutton.TouchUpInside += (sender, args) =>
            {
                DismissViewController(true, null);
            };

            // Add space at right to horizontal UIStackView
            horzStackView.AddArrangedSubview(new UILabel(new CGRect(0, 0, 10, 10)));
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();

            // Release any cached data, images, etc that aren't in use.
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // Perform any additional setup after loading the view, typically from a nib.
        }


        void SaveButtonAction(object sender, EventArgs e)
        {
            try
            {
                loadingOverlay = new LoadingOverlay(contentView.Bounds);
                contentView.Add(loadingOverlay);

                var im = AsImage(contentView);
                if (CrossConnectivity.Current.IsConnected)
                {

                    var bw = new BackgroundWorker();

                    // this allows our worker to report progress during work
                    bw.WorkerReportsProgress = true;

                    // what to do in the background thread
                    bw.DoWork += delegate (object o, DoWorkEventArgs argss)
                    {
                        var b = o as BackgroundWorker;

                        var editArr = im.AsJPEG(0.5f).ToArray();
                        var saveType = MREditing ? "DynaPad Edit" : "DynaPad Note";

                        var dps = new DynaPadService.DynaPadService();
                        //byte[] saveArr = MREditType == "jpg" ? editArr : dps.ConvertToType(CommonFunctions.GetUserConfig(), editArr, MREditType);
                        byte[] saveArr = editArr;
                        var savefile = dps.SaveFile(CommonFunctions.GetUserConfig(), apptId, patientId, doctorId, locationId, filename, saveType, "DynaPad", "", "", saveArr, IsDoctorForm, false);
                    };

                    // what to do when worker completes its task (notify the user)
                    bw.RunWorkerCompleted += delegate
                    {
                        var savestring = MREditing ? "Edited File Saved" : "New Note Saved";
                        PresentViewController(CommonFunctions.AlertPrompt(savestring, "A new edit has been saved to medical records", true, null, false, null), true, null);
                    };

                    bw.RunWorkerAsync();

                    //byte[] editArr = im.AsJPEG(0.5f).ToArray();

                    //var dps = new DynaPadService.DynaPadService();
                    ////byte[] saveArr = MREditType == "jpg" ? editArr : dps.ConvertToType(CommonFunctions.GetUserConfig(), editArr, MREditType);
                    //byte[] saveArr = editArr;
                    //var savefile = dps.SaveFile(CommonFunctions.GetUserConfig(), apptId, patientId, doctorId, locationId, filename, "DynaPad Edit", "DynaPad", "", saveArr, IsDoctorForm, false);
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
			}
            finally
            {
                loadingOverlay.Hide();
            }
        }

        public UIImage AsImage(UIView view)
        {
            try
            {
                UIGraphics.BeginImageContextWithOptions(view.Bounds.Size, view.Opaque, 1);
                view.DrawViewHierarchy(view.Frame, true); //this was key line
                var img = UIGraphics.GetImageFromCurrentImageContext();
                UIGraphics.EndImageContext();

                return img;
			}
			catch (Exception ex)
			{
				CommonFunctions.sendErrorEmail(ex);
                PresentViewController(CommonFunctions.ExceptionAlertPrompt(), true, null);
                return null;
			}
        }

        static UIImage FromUrl(string uri)
        {
            try
            {
                using (var url = new NSUrl(uri))
                using (var data = NSData.FromUrl(url))
                    return UIImage.LoadFromData(data, 100);


                //byte[] encodedDataAsBytes = NSData.FromUrl(new NSUrl(uri)).ToArray();
                //NSData data = NSData.FromArray(encodedDataAsBytes);
                //var uiImage = UIImage.LoadFromData(data);
                //return uiImage;
            }
			catch (Exception ex)
			{
				CommonFunctions.sendErrorEmail(ex);
                //PresentViewController(CommonFunctions.ExceptionAlertPrompt(), true, null);
                return null;
			}
        }
    }
}

 