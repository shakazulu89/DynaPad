using System;
using System.ComponentModel;
using System.Linq;

namespace DynaPad
{
    public class GridAppt : INotifyPropertyChanged
    {
        private string apptId;
        private string locationId;
        private string patientId;
        private string patientName;
        private string doctorName;
        private DateTime apptDate;
        private DateTime? patientForm;
        private DateTime? doctorForm;
        private DateTime? reportGenerated;

        public GridAppt(string aId, string lId, string pId, string pName, string dName, DateTime aDate, DateTime? pForm, DateTime? dForm, DateTime? rGenerated)
        {
            apptId = aId;
            locationId = lId;
            patientId = pId;
            patientName = pName;
            doctorName = dName;
            apptDate = aDate;
            patientForm = pForm;
            doctorForm = dForm;
            reportGenerated = rGenerated;
        }

        public string ApptID
        {
            get { return apptId; }
            set
            {
                if (apptId != value)
                {
                    apptId = value;
                    this.RaisedOnPropertyChanged("ApptID");
                }
            }
        }

        public string LocationID
        {
            get { return locationId; }
            set
            {
                if (locationId != value)
                {
                    locationId = value;
                    this.RaisedOnPropertyChanged("LocationID");
                }
            }
        }

        public string PatientID
        {
            get { return patientId; }
            set
            {
                if (patientId != value)
                {
                    patientId = value;
                    this.RaisedOnPropertyChanged("PatientID");
                }
            }
        }

        public string PatientName
        {
            get { return patientName; }
            set
            {
                if (patientName != value)
                {
                    patientName = value;
                    this.RaisedOnPropertyChanged("PatientName");
                }
            }
        }

        public string DoctorName
        {
            get { return doctorName; }
            set
            {
                if (doctorName != value)
                {
                    doctorName = value;
                    this.RaisedOnPropertyChanged("DoctorName");
                }
            }
        }

        public DateTime ApptDate
        {
            get { return apptDate; }
            set
            {
                if (apptDate != value)
                {
                    apptDate = value;
                    this.RaisedOnPropertyChanged("ApptDate");
                }
            }
        }

        public DateTime? PatientForm
        {
            get { return patientForm; }
            set
            {
                if (patientForm != value)
                {
                    patientForm = value;
                    this.RaisedOnPropertyChanged("PatientForm");
                }
            }
        }

        public DateTime? DoctorForm
        {
            get { return doctorForm; }
            set
            {
                if (doctorForm != value)
                {
                    doctorForm = value;
                    this.RaisedOnPropertyChanged("DoctorForm");
                }
            }
        }

        public DateTime? ReportGenerated
        {
            get { return reportGenerated; }
            set
            {
                if (reportGenerated != value)
                {
                    reportGenerated = value;
                    this.RaisedOnPropertyChanged("ReportGenerated");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisedOnPropertyChanged(string _PropertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(_PropertyName));
            }
        }
    }
}
