using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using UIKit;
using Syncfusion.SfDataGrid;
using Foundation;

namespace DynaPad
{
	public class GridApptRepository : INotifyPropertyChanged
	{
		SfDataGrid agrid;

        public NSDictionary getDictionary(string text, string value)
        {
            object[] objects = new object[2];
            object[] keys = new object[2];
            keys.SetValue("Text", 0);
            keys.SetValue("Value", 1);
            objects.SetValue((NSString)text, 0);
            objects.SetValue((NSString)value, 1);

            return NSDictionary.FromObjectsAndKeys(objects, keys);
        }

		ObservableCollection<GridAppt> gridAppt;
		public ObservableCollection<GridAppt> GridApptCollection
		{
			get { return gridAppt; }
			set { this.gridAppt = value; }
		}

		public GridApptRepository(List<MenuItem> appts, UISearchBar filterText, SfDataGrid thegrid)
		{
			agrid = thegrid;
			gridAppt = new ObservableCollection<GridAppt>();
			this.GenerateAppts(appts, filterText);
		}

		void GenerateAppts(List<MenuItem> appts, UISearchBar thefilterText)
		{
			foreach (MenuItem item in appts)
			{
				gridAppt.Add(new GridAppt(item.ApptId, item.LocationId, item.PatientId, item.PatientName, item.DoctorName, item.ApptDate, item.DatePatientFormSubmitted, item.DateDoctorFormSubmitted, item.DateReportGenerated));
			}

			agrid.ItemsSource = GridApptCollection;
			agrid.GridViewCreated += DataGrid_GridViewCreated;

			thefilterText.Placeholder = "Search here";
			thefilterText.TextChanged += OnFilterTextChanged;
			thefilterText.CancelButtonClicked += CancelButtonClicked;

			filterTextChanged = OnFilterChanged;
		}

		void DataGrid_GridViewCreated(object sender, GridViewCreatedEventArgs e)
		{
			agrid.View.LiveDataUpdateMode = Syncfusion.Data.LiveDataUpdateMode.AllowDataShaping;
		}

		public event PropertyChangedEventHandler PropertyChanged;

		public void RaisedOnPropertyChanged(string _PropertyName)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(_PropertyName));
			}
		}

		void OnFilterTextChanged(object sender, UISearchBarTextChangedEventArgs e)
		{
			FilterText = e.SearchText;
		}

		void CancelButtonClicked(object sender, EventArgs e)
		{
			var cfilterText = (UISearchBar)sender;
			cfilterText.ResignFirstResponder();
			cfilterText.SetShowsCancelButton(false, true);
			cfilterText.ResignFirstResponder();
		}

		void OnFilterChanged()
		{
			if (agrid.View != null)
			{
				this.agrid.View.Filter = FilerRecords;
				this.agrid.View.RefreshFilter();
			}
		}

		#region Filtering

		#region Fields

		string filterText = "";
		string selectedColumn = "All Columns";
		string selectedCondition = "Equals";
		internal delegate void FilterChanged();
		internal FilterChanged filterTextChanged;

		#endregion

		#region Property

		public string FilterText
		{
			get { return filterText; }
			set
			{
				filterText = value;
				OnFilterTextChanged();
				RaisedOnPropertyChanged("FilterText");
			}
		}

		public string SelectedCondition
		{
			get { return selectedCondition; }
			set { selectedCondition = value; }
		}

		public string SelectedColumn
		{
			get { return selectedColumn; }
			set { selectedColumn = value; }
		}

		#endregion

		#region Private Methods

		void OnFilterTextChanged()
		{
			filterTextChanged();
		}

		bool MakeStringFilter(GridAppt o, string option, string condition)
		{
			var value = o.GetType().GetProperty(option);
			var exactValue = value.GetValue(o, null);
			exactValue = exactValue.ToString().ToLower();
			var text = FilterText.ToLower();
			var methods = typeof(string).GetMethods();
			if (methods.Any())
			{
				if (condition == "Contains")
				{
					var methodInfo = methods.FirstOrDefault(method => method.Name == condition);
					var result1 = (bool)methodInfo.Invoke(exactValue, new object[] { text });
					return result1;
				}
				if (exactValue.ToString() == text.ToString())
				{
					var result1 = String.Equals(exactValue.ToString(), text);
					switch (condition)
					{
						case "Equals":
							return result1;
						case "NotEquals":
							return false;
						default:
							break;
					}
				}
				else if (condition == "NotEquals")
				{
					return true;
				}
				return false;
			}
			return false;
		}

		bool MakeNumericFilter(GridAppt o, string option, string condition)
		{
			var value = o.GetType().GetProperty(option);
			var exactValue = value.GetValue(o, null);
			var checkNumeric = double.TryParse(exactValue.ToString(), out double res);
			if (checkNumeric)
			{
				switch (condition)
				{
					case "Equals":
						try
						{
							if (exactValue.ToString() == FilterText)
							{
								if (Convert.ToDouble(exactValue) == (Convert.ToDouble(FilterText)))
									return true;
							}
						}
						catch (Exception e)
						{
                            var eventItems = new List<NSDictionary>
                            {
                                getDictionary("Exception Message", e.Message),
                                getDictionary("Exception Stacktrace", e.StackTrace)
                            };
							CommonFunctions.AddLogEvent(DateTime.Now, "MakeNumericFilter", true, eventItems, "Equals catch block");

							Console.WriteLine(e);
						}
						break;
					case "NotEquals":
						try
						{
							if (Convert.ToDouble(FilterText) != Convert.ToDouble(exactValue))
								return true;
						}
						catch (Exception e)
						{
                            var eventItems = new List<NSDictionary>
                            {
                                getDictionary("Exception Message", e.Message),
                                getDictionary("Exception Stacktrace", e.StackTrace)
                            };
                            CommonFunctions.AddLogEvent(DateTime.Now, "MakeNumericFilter", true, eventItems, "NotEquals catch block");

							Console.WriteLine(e);
							return true;
						}
						break;
					default: // do nothing;
						break;
				}
			}
			return false;
		}

		#endregion

		#region Public Methods

		public bool FilerRecords(object o)
		{
			var checkNumeric = double.TryParse(FilterText, out double res);
			var item = o as GridAppt;
			if (item != null && FilterText.Equals(""))
			{
				return true;
			}
			if (item != null)
			{
				if (checkNumeric && !SelectedColumn.Equals("All Columns"))
				{
					var result = MakeNumericFilter(item, SelectedColumn, SelectedCondition);
					return result;
				}
				if (SelectedColumn.Equals("All Columns"))
				{
					if (item.PatientName.ToLower().Contains(FilterText.ToLower()) ||
						item.DoctorName.ToLower().Contains(FilterText.ToLower()))
						return true;
					return false;
				}
				else
				{
					var result = MakeStringFilter(item, SelectedColumn, SelectedCondition);
					return result;
				}
			}
			return false;
		}

		#endregion

		#endregion
	}
}
