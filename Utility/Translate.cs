using System;
using System.Collections;
using ModuleWorkFlow.Model;
using System.Data;

namespace Utility
{

    public class Translate
    {
        public static Hashtable hDictionary = null;
        private static readonly object _dictLock = new object();

        public static string translateString(string sourcestring)
        {

            if (string.IsNullOrEmpty(sourcestring))
                return sourcestring;

            string resultstring = sourcestring;

            // 使用已載入字典（若有），避免每次呼叫都打 DB，且可降低偶發連線問題造成的頁面錯誤
            if (hDictionary == null && ModelTranslate.hDictionary != null)
                hDictionary = ModelTranslate.hDictionary;

            if (hDictionary == null)
            {
                lock (_dictLock)
                {
                    if (hDictionary == null && ModelTranslate.hDictionary != null)
                        hDictionary = ModelTranslate.hDictionary;

                    if (hDictionary == null)
                    {
                        hDictionary = new Hashtable();
                        try
                        {
                            string Language = System.Configuration.ConfigurationSettings.AppSettings["Language"];
                            if (Language != null && (Language.Trim().Equals("English") || (Language.Trim().Equals("Simple"))))
                            {
                                string queryString = "select * from tb_resource";
                                DataSet ds = Data.getDataSet(queryString, null);
                                if (ds != null && ds.Tables.Count > 0)
                                {
                                    foreach (DataRow row in ds.Tables[0].Rows)
                                    {
                                        string traditional = Convert.ToString(row["traditional"]);
                                        if (string.IsNullOrEmpty(traditional))
                                            continue;

                                        if (!hDictionary.ContainsKey(traditional))
                                        {
                                            if (Language.Trim().Equals("English"))
                                            {
                                                if (!Convert.IsDBNull(row["English"]))
                                                    hDictionary.Add(traditional, row["English"].ToString());
                                            }
                                            else
                                            {
                                                if (!Convert.IsDBNull(row["SimpleChinese"]))
                                                    hDictionary.Add(traditional, row["SimpleChinese"].ToString());
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        catch
                        {
                            // DB/連線偶發失敗時：降級為不翻譯（回傳原字串），避免需要多次刷新碰運氣
                            hDictionary = new Hashtable();
                        }

                        ModelTranslate.hDictionary = hDictionary;
                    }
                }
            }

            if (hDictionary != null && hDictionary.ContainsKey(sourcestring))
                return hDictionary[sourcestring] as string;

            char[] characters = sourcestring.ToCharArray();
            string chineseString = "";
            for (int j = 0; j < characters.Length; j++)
            {
                if (characters[j] == ' ')
                {
                    continue;
                }
                if (isChinese(characters[j]) || characters[j] == ',')
                {
                    chineseString += characters[j];
                }
                else
                {
                    if (!chineseString.Equals(""))
                    {
                        if (hDictionary.ContainsKey(chineseString))
                        {
                            if (!(hDictionary[chineseString] as string).Trim().Equals(""))
                            {
                                resultstring = hDictionary[chineseString] as string;
                                if (sourcestring.IndexOf(chineseString) == 0)
                                {
                                    resultstring += sourcestring.Substring(sourcestring.IndexOf(chineseString) + chineseString.Length);
                                }
                                else
                                {
                                    resultstring = sourcestring.Substring(0, sourcestring.IndexOf(chineseString)) + resultstring + sourcestring.Substring(sourcestring.IndexOf(chineseString) + chineseString.Length);
                                }
                            }
                        }

                        chineseString = "";
                    }
                }
            }

            if (!chineseString.Equals(""))
            {
                if (hDictionary.ContainsKey(chineseString))
                {
                    if (!(hDictionary[chineseString] as string).Trim().Equals(""))
                    {
                        resultstring = hDictionary[chineseString] as string;
                        if (sourcestring.IndexOf(chineseString) == 0)
                        {
                            resultstring += sourcestring.Substring(sourcestring.IndexOf(chineseString) + chineseString.Length);
                        }
                        else
                        {
                            resultstring = sourcestring.Substring(0, sourcestring.IndexOf(chineseString)) + resultstring + sourcestring.Substring(sourcestring.IndexOf(chineseString) + chineseString.Length);
                        }
                    }
                }
            }

            return resultstring;

        }


        private static bool isChinese(char c)
        {

            //[\u2e80-\u2fd5]
            if (c >= 11904 && c <= 12245)
            {
                return true;
            }

            //[\u3190-\u319f]
            if (c >= 12688 && c <= 12703)
            {
                return true;
            }

            //[\u3400-\u4dbf]
            if (c >= 13312 && c <= 19903)
            {
                return true;
            }

            //[\u4e00-\u9fcc]
            if (c >= 19968 && c <= 40908)
            {
                return true;
            }

            //[\uf900-\ufaad]
            if (c >= 63744 && c <= 64173)
            {
                return true;
            }

            return false;
        }
    }
}
