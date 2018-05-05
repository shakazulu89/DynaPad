using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using UIKit;
using Syncfusion.SfDataGrid;

namespace DynaPad
{
    public class GridApptRepository : INotifyPropertyChanged
    {
        SfDataGrid agrid;

        private ObservableCollection<GridAppt> gridAppt;
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

        private void GenerateAppts(List<MenuItem> appts, UISearchBar thefilterText)
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

        private void DataGrid_GridViewCreated(object sender, GridViewCreatedEventArgs e)
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

        private void OnFilterTextChanged(object sender, UISearchBarTextChangedEventArgs e)
        {
            FilterText = e.SearchText;
        }

        private void CancelButtonClicked(object sender, EventArgs e)
        {
            var cfilterText = (UISearchBar)sender;
            cfilterText.ResignFirstResponder();
            cfilterText.SetShowsCancelButton(false, true);
            cfilterText.ResignFirstResponder();
        }

        private void OnFilterChanged()
        {
            if (agrid.View != null)
            {
                this.agrid.View.Filter = FilerRecords;
                this.agrid.View.RefreshFilter();
            }
        }

        #region Filtering

        #region Fields

        private string filterText = "";
        private string selectedColumn = "All Columns";
        private string selectedCondition = "Equals";
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

        private void OnFilterTextChanged()
        {
            filterTextChanged();
        }

        private bool MakeStringFilter(GridAppt o, string option, string condition)
        {
            var value = o.GetType().GetProperty(option);
            var exactValue = value.GetValue(o, null);
            exactValue = exactValue.ToString().ToLower();
            string text = FilterText.ToLower();
            var methods = typeof(string).GetMethods();
            if (methods.Count() != 0)
            {
                if (condition == "Contains")
                {
                    var methodInfo = methods.FirstOrDefault(method => method.Name == condition);
                    bool result1 = (bool)methodInfo.Invoke(exactValue, new object[] { text });
                    return result1;
                }
                else if (exactValue.ToString() == text.ToString())
                {
                    bool result1 = String.Equals(exactValue.ToString(), text.ToString());
                    if (condition == "Equals")
                        return result1;
                    else if (condition == "NotEquals")
                        return false;
                }
                else if (condition == "NotEquals")
                {
                    return true;
                }
                return false;
            }
            else
                return false;
        }

        private bool MakeNumericFilter(GridAppt o, string option, string condition)
        {
            var value = o.GetType().GetProperty(option);
            var exactValue = value.GetValue(o, null);
            double res;
            bool checkNumeric = double.TryParse(exactValue.ToString(), out res);
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
                            Console.WriteLine(e);
                            return true;
                        }
                        break;
                }
            }
            return false;
        }

        #endregion

        #region Public Methods

        public bool FilerRecords(object o)
        {
            double res;
            bool checkNumeric = double.TryParse(FilterText, out res);
            var item = o as GridAppt;
            if (item != null && FilterText.Equals(""))
            {
                return true;
            }
            else
            {
                if (item != null)
                {
                    if (checkNumeric && !SelectedColumn.Equals("All Columns"))
                    {
                        bool result = MakeNumericFilter(item, SelectedColumn, SelectedCondition);
                        return result;
                    }
                    else if (SelectedColumn.Equals("All Columns"))
                    {
                        if (item.PatientName.ToLower().Contains(FilterText.ToLower()) ||
                            item.DoctorName.ToLower().Contains(FilterText.ToLower()))
                            return true;
                        return false;
                    }
                    else
                    {
                        bool result = MakeStringFilter(item, SelectedColumn, SelectedCondition);
                        return result;
                    }
                }
            }
            return false;
        }

        #endregion

        #endregion
    }
}
