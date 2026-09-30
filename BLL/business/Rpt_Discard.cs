using System;
using System.Collections;
using ModuleWorkFlow.BLL;
using ModuleWorkFlow.Model;
using ModuleWorkFlow.BLL.Unnormal;
using ModuleWorkFlow.Model.Unnormal;


namespace ModuleWorkFlow.report
{
	/// <summary>
	/// Summary description for Rpt_Discard.
	/// </summary>
	public class Rpt_Discard
	{
		public ArrayList getRptDiscard(ArrayList aldiscardinfo)
		{
			ArrayList alRpt = new ArrayList();
			Rpt_DiscardInfo rd = null;
			Rpt_DiscardPartNoInfo rdp = null;
			foreach(DiscardInfo di in aldiscardinfo)
			{
				if(rd == null) rd = new Rpt_DiscardInfo();
				if (rd.ModuleId == null)
				{
					rd.ModuleId = di.ModuleId;
					rd.TotalCount = 1;
					rd.TotalMins = di.FactMinutes;
					rd.ALPartNos = new ArrayList();
					rdp = new Rpt_DiscardPartNoInfo();
					rdp.PartNoId = di.OldPartNoId;
					rdp.Count = 1;
					rdp.Mins = di.FactMinutes;
					rd.ALPartNos.Add(rdp);
					alRpt.Add(rd);
				}
				else
				{
					if (rd.ModuleId.Equals(di.ModuleId))
					{
						rd.TotalCount ++;
						rd.TotalMins += di.FactMinutes;
						rdp = (Rpt_DiscardPartNoInfo)rd.ALPartNos[rd.ALPartNos.Count-1];
						if (rdp.PartNoId.Equals(di.OldPartNoId))
						{
							rdp.Count ++;
							rdp.Mins += di.FactMinutes;
						}
						else
						{
							rdp = new Rpt_DiscardPartNoInfo();
							rdp.PartNoId = di.OldPartNoId;
							rdp.Count = 1;
							rdp.Mins = di.FactMinutes;
							rd.ALPartNos.Add(rdp);
						}
					}
					else
					{
						rd = new Rpt_DiscardInfo();
						rd.ModuleId = di.ModuleId;
						rd.TotalCount = 1;
						rd.TotalMins = di.FactMinutes;
						rd.ALPartNos = new ArrayList();
						rdp = new Rpt_DiscardPartNoInfo();
						rdp.PartNoId = di.OldPartNoId;
						rdp.Count = 1;
						rdp.Mins = di.FactMinutes;
						rd.ALPartNos.Add(rdp);
						alRpt.Add(rd);
					}
				}
			}
			return alRpt;
		}
	}
}
