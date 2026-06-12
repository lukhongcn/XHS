using System;

namespace ModuleWorkFlow.business
{
	/// <summary>
	/// Summary description for PartData.
	/// </summary>
	public class PartData
	{
		string PartNo_Id;
		int NeedMinutes;
		int NeedProduct;

		public PartData(string partNo_Id,int needMinutes,int needProduct)
		{
			this.PartNo_Id=partNo_Id;
			this.NeedMinutes=needMinutes;
			this.NeedProduct=needProduct;
			
		}

		public string getPartNo_Id()
		{
			return this.PartNo_Id;
		}

		public int getNeedMinutes()
		{
			return this.NeedMinutes;
		}

		public int getNeedProduct()
		{
			return this.NeedProduct;
		}
	}
}
