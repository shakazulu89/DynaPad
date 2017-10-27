using System.Collections.Generic;
using MonoTouch.Dialog;
//using static DynaClassLibrary.DynaClasses;

namespace DynaPad
{
	public static class JsonHandler
	{
		public static string OriginalFormJsonString { get; set; }
		public static JsonElement OriginalFormJson
		{
			get;
			set;
		}
		public static List<Section> FormSections
		{
			get;
			set;
		}

		public static List<Section> FormQuestions
		{
			get;
			set;
		}

		public static List<Report> DynaReports
		{
			get;
			set;
		}
	}
}

