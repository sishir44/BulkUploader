using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace BulkUploader.Models
{
    public class Generate_dt
    {
        public static DataTable GetDataTable(object[,] arr)
        {
            int rowLen = arr.GetLength(0);
            int colLen = arr.GetLength(1);
            DataTable dt = new DataTable();
            for (int i = 0; i < rowLen; i++)
            {
                if (i == 0)
                {
                    for (int j = 0; j < colLen; j++)
                    {
                        if (arr[i, j].ToString() != "")
                        {
                            dt.Columns.Add(arr[i, j].ToString());
                        }
                    }
                }
                else if (i != 0)
                {
                    DataRow dr = dt.NewRow();
                    for (int k = 0; k < colLen; k++)
                    {
                        if (arr[i, k] != null)
                        {
                            dr[k] = arr[i, k].ToString();
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