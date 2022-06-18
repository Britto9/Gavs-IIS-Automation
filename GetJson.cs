using System;
using System.Collections.Generic;
using System.Data;

using System.Text;
using System.Web;

namespace IN_SOA
{
    public class GetJson
    {
        public GetJson()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        public static string DataSetToJson(DataSet ds)
        {
            StringBuilder json = new StringBuilder();

            json.Append("{");
            json.Append("\"");
            json.Append("MSG");
            json.Append("\":");
            json.Append("[");

            for (int k = 0; k <= ds.Tables.Count - 1; k++)
            {
                json.Append("{");
                json.Append("\"");
                json.Append(ds.Tables[k].TableName.ToString().ToUpper());
                json.Append("\":");
                json.Append("[");
                foreach (DataRow dr in ds.Tables[k].Rows)
                {
                    json.Append("{");

                    int i = 0;
                    int colcount = dr.Table.Columns.Count;


                    foreach (DataColumn dc in dr.Table.Columns)
                    {
                        if (dc.ColumnName.ToString().ToUpper().Contains("JSON"))
                        {
                            json.Append("\"");
                            json.Append(dc.ColumnName.ToUpper());
                            json.Append("\":");
                            if (string.IsNullOrEmpty(dr[dc].ToString()))
                            {
                                json.Append("[]");
                            }
                            else
                            {
                                //json.Append(dr[dc]);
                                //json.Append(dr[dc].ToString().Replace('"', ' ')); //Added by Amreshit Reason : if double quote Replace WITH SPACE 
                                //json.Append(dr[dc].ToString().Replace("'", " ").Replace('"', ' '));//Added by Amreshit Reason : if SINGLE quote Replace  WITH SPACE
                                // json.Append(dr[dc].ToString().Replace("'", " ").Replace('"', ' ').Replace(@"\"," "));//Added by Amreshit Reason : if \  Replace  WITH SPACE on 16/02/2016
                                json.Append(dr[dc].ToString().Replace('"', ' ').Replace(@"\", " "));//Added by Amreshit Reason : if \  Replace  WITH SPACE on 16/02/2016
                            }

                        }
                        else
                        {
                            json.Append("\"");
                            json.Append(dc.ColumnName.ToUpper());
                            json.Append("\":\"");
                            //json.Append(dr[dc]);
                            // json.Append(dr[dc].ToString().Replace('"', ' ')); //Added by Amreshit Reason : if double quote Replace 
                            //json.Append(dr[dc].ToString().Replace("'", " ").Replace('"', ' '));//Added by Amreshit Reason : if SINGLE quote Replace  WITH SPACE
                            //  json.Append(dr[dc].ToString().Replace("'", " ").Replace('"', ' ').Replace(@"\", " "));//Added by Amreshit Reason : if \  Replace  WITH SPACE on 16/02/2016
                            json.Append(dr[dc].ToString().Replace('"', ' ').Replace(@"\", " "));//Added by Amreshit Reason : if \  Replace  WITH SPACE on 16/02/2016
                            json.Append("\"");

                        }

                        i += 1;
                        if (i < colcount)
                        {
                            json.Append(",");

                        }
                    }
                    //If dr.Table.Rows.Count = ds.Tables[k].Rows.Count Then
                    //json.Append("}")
                    //Else
                    json.Append("}");
                    json.Append(",");
                    //End If

                }
                if (ds.Tables[k].Rows.Count > 0)
                {
                    json.Remove(json.ToString().LastIndexOf(","), 1);
                }
                json.Append("]");
                json.Append("}");
                json.Append(",");

            }

            json.Remove(json.ToString().LastIndexOf(","), 1);
            json.Append("]");
            json.Append("}");
            //return json.ToString();
            return StripControlChars(json.ToString());
        }
        //To strip control characters:
        //A character that does not represent a printable character but //serves to initiate a particular action.
        public static string StripControlChars(string s)
        {
            return System.Text.RegularExpressions.Regex.Replace(s, @"[^\x20-\x7F]", "");
        }
        public static string DataSetToJson_spl_char(DataSet ds)
        {
            StringBuilder json = new StringBuilder();

            json.Append("{");
            json.Append("\"");
            json.Append("MSG");
            json.Append("\":");
            json.Append("[");

            for (int k = 0; k <= ds.Tables.Count - 1; k++)
            {
                json.Append("{");
                json.Append("\"");
                json.Append(ds.Tables[k].TableName.ToString().ToUpper());
                json.Append("\":");
                json.Append("[");
                foreach (DataRow dr in ds.Tables[k].Rows)
                {
                    json.Append("{");

                    int i = 0;
                    int colcount = dr.Table.Columns.Count;


                    foreach (DataColumn dc in dr.Table.Columns)
                    {
                        if (dc.ColumnName.ToString().ToUpper().Contains("JSON"))
                        {
                            json.Append("\"");
                            json.Append(dc.ColumnName.ToUpper());
                            json.Append("\":");
                            if (string.IsNullOrEmpty(dr[dc].ToString()))
                            {
                                json.Append("[]");
                            }
                            else
                            {
                                json.Append(dr[dc].ToString().Replace('"', ' ').Replace(@"\", " "));//Added by Amreshit Reason : if \  Replace  WITH SPACE on 16/02/2016
                            }

                        }
                        else
                        {
                            json.Append("\"");
                            json.Append(dc.ColumnName.ToUpper());
                            json.Append("\":\"");
                            json.Append(dr[dc].ToString().Replace('"', ' ').Replace(@"\", " "));//Added by Amreshit Reason : if \  Replace  WITH SPACE on 16/02/2016
                            json.Append("\"");

                        }

                        i += 1;
                        if (i < colcount)
                        {
                            json.Append(",");

                        }
                    }
                    json.Append("}");
                    json.Append(",");

                }
                if (ds.Tables[k].Rows.Count > 0)
                {
                    json.Remove(json.ToString().LastIndexOf(","), 1);
                }
                json.Append("]");
                json.Append("}");
                json.Append(",");

            }

            json.Remove(json.ToString().LastIndexOf(","), 1);
            json.Append("]");
            json.Append("}");
            return json.ToString();
            //  return StripControlChars(json.ToString());
        }
    }
}