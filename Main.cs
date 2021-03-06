﻿using System;
using System.Collections.Generic;
using UIKit;
using System.Threading.Tasks;
//using DynaPad.DynaPadService;
using DynaClassLibrary;

namespace DynaPad
{
	public class Application
	{
		// This is the main entry point of the application.
		static void Main(string[] args)
		{
            //AppDomain.CurrentDomain.UnhandledException += (o, e) =>
            //{
            //    var dd = "tesf";
            //};

            //TaskScheduler.UnobservedTaskException += (o, e) =>
            //{

            //    var dd = "tesf";
            //};



			//AppDomain.CurrentDomain.UnhandledException += (sender, argss) =>
			//{
			//	var exc = argss.ExceptionObject as Exception;
			//};
			// if you want to use a different Application Delegate class from "AppDelegate"
			// you can specify it here.
			UIApplication.Main(args, null, "AppDelegate");
		}

		static void JsonCallback(object data)
		{
			//foreach (JsonElement qElement in
			//Console.WriteLine("Invoked");
		}
	}
}

namespace DynaClassLibrary
{
	public static class DynaClasses
    {
        public class LoginListRequest
        {
            public string domain { get; set; }
            public string deviceName { get; set; }
            public string username { get; set; }
            public string password { get; set; }
        }

		public static class LoginContainer
		{
			public static User User { get; set; }
		}

		public class User
		{
			public string UserId { get; set; }
			public string Username { get; set; }
			public string Password { get; set; }
			public string DisplayName { get; set; }
			public string Email { get; set; }
			public List<Location> Locations { get; set; }
			public Location SelectedLocation { get; set; }
			public string LoginStatus { get; set; }
			public string DynaPassword { get; set; }
			public DynaClasses.ConfigurationObjects DynaConfig { get; set; }
            public string LogFileName { get; set; }
		}

		public class Location
		{
			public string LocationId { get; set; }
			public string LocationName { get; set; }
			public string LocationCity { get; set; }
		}

		public class ConfigurationObjects
		{
            public string EmailSupport { get; set; } = "dharel.cm@gmail.com";
			public string EmailPostmaster { get; set; }
            public string EmailRoy { get; set; } = "rharel@hotmail.com";
            public string EmailSmtp { get; set; } = "smtp.postmarkapp.com";
            public string EmailUser { get; set; } = "5b07926f-24fe-4ed3-9a14-ce010c8e5e3a";
            public string EmailPass { get; set; } = "5b07926f-24fe-4ed3-9a14-ce010c8e5e3a";
            public int EmailPort { get; set; } = 2525;
			public string ConnectionString { get; set; }
			public string ConnectionName { get; set; }
            public string DatabaseName { get; set; }
            public string DomainName { get; set; }
			public string DomainHost { get; set; }
			public string DomainRootPathVirtual { get; set; }
			public string DomainRootPathPhysical { get; set; }
			public string DomainClaimantsPathVirtual { get; set; }
			public string DomainClaimantsPathPhysical { get; set; }
			//public DataTable DomainPaths { get; set; }
            public List<DomainPath> DomainPaths { get; set; }
            public string DeviceId { get; set; }
		}

		public class DomainPath
		{
			//public int DomainID { get; set; }
			//public string DomainName { get; set; }
			//public string DatabaseName { get; set; }
			//public string DatabaseHost { get; set; }
			//public string DatabaseUser { get; set; }
			//public string DatabasePw { get; set; }
			//public int ClientID { get; set; }
			//public int DomainPathID { get; set; }
			public string DomainPathName { get; set; }
			public string DomainPathPhysical { get; set; }
			public string DomainPathVirtual { get; set; }
        }

        public class DynaFile
        {
            public string FileName { get; set; }
            public string UserId { get; set; }
            public DynaClasses.ConfigurationObjects UserConfig { get; set; }
            public string ApptId { get; set; }
            public DateTime ApptDate { get; set; }
            public string FormId { get; set; }
            public bool IsDoctorForm { get; set; }
            public string DoctorId { get; set; }
            public string LocationId { get; set; }
            public string PatientId { get; set; }
            public string PatientName { get; set; }
            public string Type { get; set; }
            public string Json { get; set; }
            public string Html { get; set; }
            public byte[] Bytes { get; set; }
            public string FileUrl { get; set; }
            public string Status { get; set; }
            public DateTime DateCreated { get; set; }
            public DateTime DateUploaded { get; set; }
        }
	}

        public class AppointmentsByDateFrameRequest
    {
        public DynaClasses.ConfigurationObjects configurationObjects { get; set; }
        public string locationId { get; set; }
        public string dateFrame { get; set; }
    }

    public class BuildDynaMenuRequest
    {
        public DynaClasses.ConfigurationObjects configurationObjects { get; set; }
        public string locationId { get; set; }
        public string locationName { get; set; }
    }

    public class ProcessDynaFilesRequest
    {
        public DynaClasses.ConfigurationObjects configurationObjects { get; set; }
        public string stringDynaFiles { get; set; }
    }

    public class GetFilesByDateRequest
    {
        public DynaClasses.ConfigurationObjects configurationObjects { get; set; }
        public string locationId { get; set; }
        public string dateToDownload { get; set; }
    }

    public class GetFilesRequest
    {
        public DynaClasses.ConfigurationObjects configurationObjects { get; set; }
        public string locationId { get; set; }
        public string apptId { get; set; }
        public string patientId { get; set; }
        public string patientName { get; set; }
        public string doctorId { get; set; }
    }

    public class SaveFileRequest
    {
        public DynaClasses.ConfigurationObjects configurationObjects { get; set; }
        public string locationId { get; set; }
        public string apptId { get; set; }
        public string patientId { get; set; }
        public string doctorId { get; set; }
        public string fileName { get; set; }
        public string fileType { get; set; }
        public string folderName { get; set; }
        public string filePathPhysical { get; set; } // need '= ""'??
        public string filePathVirtual { get; set; } // need '= ""'??
        public byte[] arrFile { get; set; } // need '= null'??
        public bool isDoctorForm { get; set; } // need '= false'??
        public bool isSignature { get; set; } // need '= false'??
    }

    public class SaveLogRequest
    {
        public DynaClasses.ConfigurationObjects configurationObjects { get; set; }
        public DynaClasses.DynaFile dynaLogFile { get; set; }
    }

    public class GetAnswerPresetsRequest
    {
        public DynaClasses.ConfigurationObjects configurationObjects { get; set; }
        public string formId { get; set; }
        public string sectionId { get; set; }
        public string doctorId { get; set; }
        public string locationId { get; set; }
    }

    public class GetAllAnswerPresetsRequest
    {
        public DynaClasses.ConfigurationObjects configurationObjects { get; set; }
        public string userId { get; set; }
        public bool isForceUpdate { get; set; }
    }

    public class GetFormPresetsRequest
    {
        public DynaClasses.ConfigurationObjects configurationObjects { get; set; }
        public string formId { get; set; }
        public bool isDocInput { get; set; }
    }

    public class LogPresetRequest
    {
        public DynaClasses.ConfigurationObjects configurationObjects { get; set; }
        public string deviceName { get; set; }
        public string requestDateTimeUTC { get; set; }
        public int presetCount { get; set; }
    }

    public class SaveAnswerPresetRequest
    {
        public DynaClasses.ConfigurationObjects configurationObjects { get; set; }
        public string formId { get; set; }
        public string sectionId { get; set; }
        public string doctorId { get; set; }
        public bool isDocInput { get; set; }
        public string presetName { get; set; }
        public string presetJson { get; set; }
        public string locationId { get; set; }
        public string existingPresetId { get; set; } // need '= ""'??
    }

    public class UpdateAnswerPresetJsonRequest
    {
        public DynaClasses.ConfigurationObjects configurationObjects { get; set; }
        public string formId { get; set; }
        public string sectionId { get; set; }
        public string doctorId { get; set; }
        public string presetJson { get; set; }
        public string existingPresetId { get; set; }
    }

    public class DeleteAnswerPresetRequest
    {
        public DynaClasses.ConfigurationObjects configurationObjects { get; set; }
        public string formId { get; set; }
        public string sectionId { get; set; }
        public string doctorId { get; set; }
        public string existingPresetId { get; set; }
    }

    public class SaveDictationRequest
    {
        public DynaClasses.ConfigurationObjects configurationObjects { get; set; }
        public string formId { get; set; }
        public string sectionId { get; set; }
        public string doctorId { get; set; }
        public bool isDocInput { get; set; }
        public string locationId { get; set; }
        public string dictationTitle { get; set; }
        public byte[] arrDictation { get; set; }
    }

    public class DeleteDicatationRequest
    {
        public DynaClasses.ConfigurationObjects configurationObjects { get; set; }
        public string dictationId { get; set; }
        public string formId { get; set; }
        public string sectionId { get; set; }
        public string doctorId { get; set; }
    }

    public class GetFormDictationsRequest
    {
        public DynaClasses.ConfigurationObjects configurationObjects { get; set; }
        public string formId { get; set; }
        public string sectionId { get; set; }
        public string doctorId { get; set; }
        public bool isDocInput { get; set; }
        public string locationId { get; set; }
    }

    public class GetAllAutoBoxDataRequest
    {
        public DynaClasses.ConfigurationObjects configurationObjects { get; set; }
        public string rootPhysical { get; set; }
        public string rootVirtual { get; set; }
    }

    public class GenerateIME4Request
    {
        public DynaClasses.ConfigurationObjects configurationObjects { get; set; }
        public string apptId { get; set; }
    }

    public class GenerateSummaryFromHtmlRequest
    {
        public DynaClasses.ConfigurationObjects configurationObjects { get; set; }
        public string summaryHtml { get; set; }
        public string patientId { get; set; }
        public string patientName { get; set; }
        public string apptId { get; set; }
        public string locationId { get; set; }
        public string doctorId { get; set; }
        public bool isDoctorForm { get; set; }
    }

    public class GenerateSummaryRequest
    {
        public DynaClasses.ConfigurationObjects configurationObjects { get; set; }
        public string answers { get; set; }
    }

    public class SubmitFormAnswersRequest
    {
        public DynaClasses.ConfigurationObjects configurationObjects { get; set; }
        public string answers { get; set; }
        public bool update { get; set; }
        public bool isDoctorInput { get; set; }
        public bool doGenerateReport { get; set; } // need '= false'??
    }

    public class GetFormQuestionsRequest
    {
        public DynaClasses.ConfigurationObjects configurationObjects { get; set; }
        public string formId { get; set; }
        public string doctorId { get; set; }
        public string locationId { get; set; }
        public string patientId { get; set; }
        public string patientName { get; set; }
        public string caseId { get; set; }
        public string apptId { get; set; }
        public bool isDocInput { get; set; }
    }

    public class GetDoctorInputRequest
    {
        public DynaClasses.ConfigurationObjects configurationObjects { get; set; }
        public string formId { get; set; }
    }

    public class GetDynaReportsRequest
    {
        public DynaClasses.ConfigurationObjects configurationObjects { get; set; }
        public string qFormID { get; set; }
        public string docId { get; set; }
        public bool showcase { get; set; }
    }

    public class CreateDynaReportRequest
    {
        public DynaClasses.ConfigurationObjects configurationObjects { get; set; }
        public string formId { get; set; }
    }

    public class GenerateReportRequest
    {
        public DynaClasses.ConfigurationObjects configurationObjects { get; set; }
        public string apptId { get; set; }
        public string qFormId { get; set; }
        public string dateCompleted { get; set; }
        public string fileName { get; set; }
        public string reportId { get; set; }
    }

    public class LoginRequest
    {
        public string domain { get; set; }
        public string deviceName { get; set; }
        public string username { get; set; }
        public string password { get; set; }
    }
}


public static class ActiveMenu
{
	public static Menu activeMenu { get; set; }
}


public class MenuItem
{
	public MenuItem() { Menus = new List<Menu>(); }
	public string MenuItemValue { get; set; }
	public string MenuItemAction { get; set; }
	public string MenuItemCaption { get; set; }
	public string PatientId { get; set; }
	public string PatientName { get; set; }
    public string DoctorId { get; set; }
    public string DoctorName { get; set; }
	public string LocationId { get; set; }
    public string ApptId { get; set; }
    public DateTime ApptDate { get; set; }
    public string ReportId { get; set; }
    public string Status { get; set; }
    public string CaseId { get; set; }
    public string ApptNotes { get; set; }
    public string PatientNotes { get; set; }
    public DateTime? DatePatientFormSubmitted { get; set; }
    public DateTime? DateDoctorFormSubmitted { get; set; }
    public DateTime? DateReportGenerated { get; set; }
	public List<Menu> Menus { get; set; }
}


public class Menu
{
	public Menu() { MenuItems = new List<MenuItem>(); }
	public string MenuValue { get; set; }
	public string MenuAction { get; set; }
	public string MenuCaption { get; set; }
	public string PatientId { get; set; }
	public string PatientName { get; set; }
    public string DoctorId { get; set; }
    public string DoctorName { get; set; }
	public string LocationId { get; set; }
    public string ApptId { get; set; }
    public DateTime ApptDate { get; set; }
    public string ReportId { get; set; }
    public string Status { get; set; }
    public string ApptNotes { get; set; }
    public string PatientNotes { get; set; }
    public DateTime? DatePatientFormSubmitted { get; set; }
    public DateTime? DateDoctorFormSubmitted { get; set; }
    public DateTime? DateReportGenerated { get; set; }
	public List<MenuItem> MenuItems { get; set; }
}


public class DynaMenu { }


public static class SelectedAppointment
{
	public static string ApptFormId { get; set; }
	public static string ApptFormName { get; set; }
    public static string ApptId { get; set; }
    public static string ApptTime { get; set; }
    public static string ApptReportId { get; set; }
    public static string ApptStatus { get; set; }
    public static string CaseId { get; set; }
    public static string ApptNotes { get; set; }
    public static string PatientNotes { get; set; }
	public static string ApptPatientId { get; set; }
	public static string ApptPatientName { get; set; }
    public static string ApptDoctorId { get; set; }
    public static string ApptDoctorName { get; set; }
	public static string ApptLocationId { get; set; }
	public static string ApptCompanyId { get; set; }
	public static List<Report> ApptReports { get; set; }
	public static List<MRFolder> ApptMRFolders { get; set; }
	public static QForm SelectedQForm { get; set; }
	public static QForm AnsweredQForm { get; set; }
}


public class MRFolder
{
	public List<MRFolder> MrFolderMRFolders { get; set; }
	public List<MR> MrFolderMRs { get; set; }
	public string MRFolderId { get; set; }
	public string MRFolderName { get; set; }
	public string MRFolderApptId { get; set; }
	public string MRFolderDoctorId { get; set; }
	public string MRFolderDoctorLocationId { get; set; }
    public string MRFolderPatientId { get; set; }
    public string MRFolderPatientName { get; set; }
	public string MRFolderPath { get; set; }
}


public class MR
{
	public string MRId { get; set; }
	public string MRName { get; set; }
	public string MRFolderName { get; set; }
	public string MRApptId { get; set; }
	public string MRApptDate { get; set; }
	public string MRDoctorId { get; set; }
	public string MRDoctor { get; set; }
	public string MRDoctorLocationId { get; set; }
	public string MRLocation { get; set; }
    public string MRPatientId { get; set; }
    public string MRPatientName { get; set; }
	public string MRPath { get; set; }
	public string MRFileType { get; set; }
    public bool IsShortcut { get; set; }
    public bool IsLocal { get; set; }
}


public class Report
{
	public string ReportId { get; set; }
	public string ReportCompanyId { get; set; }
	public string ReportName { get; set; }
	public string ReportDescription { get; set; }
	public string DoctorId { get; set; }
	public string FormId { get; set; }
}


public class ActiveTriggerId
{
	public string ParentQuestionId { get; set; }
	public string ParentOptionId { get; set; }
	public string TriggerId { get; set; }
	public bool Triggered { get; set; }
}


public class QuestionOption
{
	public string ParentQuestionId { get; set; }
	public string OptionId { get; set; }
	public string OptionText { get; set; }
	public string InputType { get; set; }
	public bool Chosen { get; set; }
	public List<string> ConditionTriggerIds { get; set; }
}


public class QuestionRowItem
{
	public string MaxItems { get; set; }
	public List<ItemColumn> ItemColumns { get; set; }
}

public class ItemColumn
{
	public string Header { get; set; }
	public string Type { get; set; }
	public string AnswerText { get; set; }
	public bool Required { get; set; }
	public List<string> Options { get; set; }
}


public class SectionQuestion
{
	public string SectionId { get; set; }
	public string QuestionId { get; set; }
	public string QuestionParentId { get; set; }
	public string QuestionType { get; set; }
	public string QuestionText { get; set; }
	public string QuestionKeyboardType { get; set; }
	public string QuestionKeyboardTypeId { get; set; }
	public string ParentConditionTriggerId { get; set; }
	public string AnswerId { get; set; }
	public string AnswerText { get; set; }
	public string AnswerOptionIndex { get; set; }
	public string MinValue { get; set; }
	public string MaxValue { get; set; }
	public string Increment { get; set; }
	public string DefaultValue { get; set; }
	public string Subtitle { get; set; }
	public string Mask { get; set; }
	public bool IsConditional { get; set; }
	public bool IsAnswered { get; set; }
	public bool IsEnabled { get; set; }
	public bool IsRequired { get; set; }
	public bool IsInvalid { get; set; } = false;
	public List<string> ActiveTriggerIds { get; set; }
	public List<QuestionOption> QuestionOptions { get; set; }
	//public nfloat ScrollY { get; set; }
	//public NSIndexPath GetIndexPath { get; set; }
	public QuestionRowItem QuestionRowItem { get; set; }
	public List<QuestionRowItem> ItemRows { get; set; }
}


public class FormSection
{
	public string SectionId { get; set; }
	public string SectionName { get; set; }
	public int SectionSelectedTemplateId { get; set; }
	public List<SectionQuestion> SectionQuestions { get; set; }
	public FormSection() { SectionSelectedTemplateId = 0; }
	public bool Revalidating { get; set; }
	public int RevalidatingRow { get; set; }
	//public nfloat RevalidatingY { get; set; }
	//public NSIndexPath InvalidIndexPath { get; set; }
}


public class QForm
{
	public string FormId { get; set; }
	public string FormName { get; set; }
	public string PatientId { get; set; }
	public string PatientName { get; set; }
	public string DoctorId { get; set; }
	public string LocationId { get; set; }
	public string ApptId { get; set; }
	public string DateCompleted { get; set; }
	public string DateUpdated { get; set; }
	public bool IsDoctorForm { get; set; }
	public int FormSelectedTemplateId { get; set; }
	public List<FormSection> FormSections { get; set; }
	public QForm() { FormSelectedTemplateId = 0; }

	//public static explicit operator string(QForm v)
	//{
	//	throw new NotImplementedException();
	//}
}

//{ presetFormId, presetDoctorId, presetLocationId, presetSectionId, presetName, presetJson, presetId, domainConfig.DomainRootPathPhysical + presetPath }

public class DynaPreset
{
    public string UserId { get; set; }
    public DynaClasses.ConfigurationObjects UserConfig { get; set; }
    public string FormId { get; set; }
    public string DoctorId { get; set; }
    public string LocationId { get; set; }
    public string SectionId { get; set; }
    public string PresetName { get; set; }
    public string PresetJson { get; set; }
    public string PresetId { get; set; }
    public string PresetFileUrl { get; set; }
    public string DateCreated { get; set; }
    public string DateUpdated { get; set; }
}

//public class DynaFile
//{
//    public string FileName { get; set; }
//    public string UserId { get; set; }
//    public DynaClasses.ConfigurationObjects UserConfig { get; set; }
//    public string ApptId { get; set; }
//    public DateTime ApptDate { get; set; }
//    public string FormId { get; set; }
//    public bool IsDoctorForm { get; set; }
//    public string DoctorId { get; set; }
//    public string LocationId { get; set; }
//    public string PatientId { get; set; }
//    public string PatientName { get; set; }
//    public string Type { get; set; }
//    public string Json { get; set; }
//    public string Html { get; set; }
//    public byte[] Bytes { get; set; }
//    public string FileUrl { get; set; }
//    public string Status { get; set; }
//    public DateTime DateCreated { get; set; }
//    public DateTime DateUploaded { get; set; }
//}
