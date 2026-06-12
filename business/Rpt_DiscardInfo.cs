using System;
using System.Collections;

namespace ModuleWorkFlow.report
{
	/// <summary>
	/// Summary description for Rpt_DiscardInfo.
	/// </summary>
	public class Rpt_DiscardInfo
	{
		private string _moduleid;
		private int _totalmins;
		private int _totalcount;
		private ArrayList alparts;

		public string ModuleId
		{
			set { _moduleid=value; }
			get { return _moduleid; }
		}

		public int TotalMins
		{
			set { _totalmins=value; }
			get { return _totalmins; }
		}

		public int TotalCount
		{
			set { _totalcount=value; }
			get { return _totalcount; }
		}

		public ArrayList ALPartNos
		{
			set { alparts=value; }
			get { return alparts; }
		}
	}

	public class Rpt_DiscardPartNoInfo
	{
		private string _partnoid;
		private int _mins;
		private int _count;

		public string PartNoId
		{
			set { _partnoid=value; }
			get { return _partnoid; }
		}

		public int Mins
		{
			set { _mins=value; }
			get { return _mins; }
		}

		public int Count
		{
			set { _count=value; }
			get { return _count; }
		}
	}
}
