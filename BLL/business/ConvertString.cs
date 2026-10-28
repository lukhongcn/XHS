using System;

namespace ModuleWorkFlow.business
{
	/// <summary>
	/// Summary description for Lang.
	/// </summary>
	public class ConvertString
	{
//		public static string INPUT_NUMBER = "¥s¶ô¡¼­p¡¼";  //µe­É¡¼¦î¢s÷çµj¡¼¡¼­píå
		public static bool ConvertToEnglish(string txt)
		{
			char temp;

			for (int i = 0; i < txt.Length; i++)
			{
				temp = txt[i];

                if (('a' <= temp && temp <= 'z') || ('A' <= temp && temp <= 'Z') || ('0' <= temp && temp <= '9'))
				{

				}
				else
				{
					return false;
				}



//				byte[] sarr = System.Text.Encoding.GetEncoding("gb2312").GetBytes(temp);
//
//				if (sarr.Length == 2)
//				{
//					return false;
//				}
			}
			return true;
		}

		public static bool ConvertToPrat(string txt)
		{
			char temp;

			for (int i = 0; i < txt.Length; i++)
			{
				temp = txt[i];

				if (('a'<=temp && temp<='z') || ('A' <= temp && temp<='Z') || ('0'<= temp && temp <= '9') || ( temp == '~'))
				{

				}
				else
				{
					return false;
				}
			}
			return true;
		}

        public static bool ConvertToInt(string txt)
        {
            char temp;

            for (int i = 0; i < txt.Length; i++)
            {
                temp = txt[i];

                if (('0' <= temp && temp <= '9'))
                {

                }
                else
                {
                    return false;
                }
            }
            return true;
        }

		public static bool ConvertToPrattomile(string txt)
		{
			char temp;

			for (int i = 0; i < txt.Length; i++)
			{
				temp = txt[i];

				if (('a'<=temp && temp<='z') || ('A' <= temp && temp<='Z') || ('0'<= temp && temp <= '9') || ( temp == '-'))
				{

				}
				else
				{
					return false;
				}
			}
			return true;
		}


		public static bool ConvertToPratNo(string txt)
		{
            if (txt.IndexOf(',') < 0)
            {
                return true;
            }
            else
            {
                return false;
            }
		}

		public static bool ConvertToPartNoToG(string txt)
		{
			bool flag = true;
			char prestr='a';
			for(int i =0;i<txt.Length;i++)
			{
				if (txt[i].Equals(',') && prestr.Equals(','))
				{
					flag = true;
				}
				else if (txt[i].Equals(',') && !prestr.Equals(','))
				{
					flag = false;
				}
				else
				{
					if (!txt[i].Equals(',') && !flag)
					{
						break;
					}
				}
				prestr = txt[i];
			}
			return flag;
		}


		public static bool ConvertToModuleId(string txt)
		{

            if (txt.IndexOf('-') > 0)
            {
                return false;
            }
            else
            {
                return true;
            }

			
		}

		public ConvertString()
		{
			//
			// TODO: Add constructor logic here
			//
		}
	}
}
