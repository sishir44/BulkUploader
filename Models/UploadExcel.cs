using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace BulkUploader.Models
{
    public class UploadExcel
    {
        public static DataTable Generic_dt(object[,] arr,string checker, string optional = null)
        {
            DataTable dt = Generate_dt.GetDataTable(arr);
            DataTable report1 = Logic.Sorting(dt,checker,optional);
            return report1;
        }

        public static DataTable Convert_To_Dt(object[,] arr)
        {
            DataTable dt = Generate_dt.GetDataTable(arr);
           
            return dt;
        }

        public static DataTable GetDataTable(object[,] arr)
        {
            int rowLen = arr.GetLength(0);
            int colLen = arr.GetLength(1);
            DataTable dt = new DataTable();
            int SoldCol = -1;
            double heightVal = 0;
            for (int i = 0; i < rowLen; i++)
            {
                if (i == 0)
                {
                    for (int j = 0; j < colLen; j++)
                    {
                        if (!String.IsNullOrWhiteSpace(Convert.ToString(arr[i, j])))
                        {
                            if (Convert.ToString(arr[i, j]).ToLower() == "sold on")
                            {
                                SoldCol = j;
                                
                            }
                                dt.Columns.Add(Convert.ToString(arr[i, j]));

                        }
                    }
                }
                else if (i != 0)
                {
                    DataRow dr = dt.NewRow();
                    for (int k = 0; k < colLen; k++)
                    {
                        if (!String.IsNullOrWhiteSpace(Convert.ToString(arr[i, k])))
                        {
                            if (k== SoldCol)
                            {
                                bool success = double.TryParse(Convert.ToString(arr[i, k]), out heightVal);
                                if (heightVal != 0)
                                {
                                    string SoldOn = Convert.ToString(DateTime.FromOADate(heightVal));
                                    dr[k] = SoldOn;
                                }
                                else
                                {
                                    dr[k] = "";
                                }
                            }
                            else
                            {
                                dr[k] = Convert.ToString(arr[i, k]);
                            }
                            
                        }
                        else
                        {
                            dr[k] = "";
                        }
                    }
                    dt.Rows.Add(dr);
                }
            }
            return dt;
        }
    }
}