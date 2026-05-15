using OfficeOpenXml;
using System;
using System.Data;

public static class ExcelHelper
{
    //public static DataTable ExcelToDataTable(ExcelWorksheet ws)
    //{
    //    DataTable dt = new DataTable();

    //    int rows = ws.Dimension.End.Row;
    //    int cols = ws.Dimension.End.Column;

    //    // Header row
    //    for (int c = 1; c <= cols; c++)
    //    {
    //        string colName = ws.Cells[1, c].Text.Trim();
    //        dt.Columns.Add(colName);
    //    }

    //    // Data rows
    //    for (int r = 2; r <= rows; r++)
    //    {
    //        DataRow dr = dt.NewRow();

    //        for (int c = 1; c <= cols; c++)
    //            dr[c - 1] = ws.Cells[r, c].Text.Trim();

    //        dt.Rows.Add(dr);
    //    }

    //    return dt;
    //}

    public static DataTable ExcelToDataTable(ExcelWorksheet ws)
    {
        DataTable dt = new DataTable();

        int rows = ws.Dimension.End.Row;
        int cols = ws.Dimension.End.Column;

        // Header
        for (int c = 1; c <= cols; c++)
        {
            string colName = ws.Cells[1, c].Text.Trim();

            if (string.IsNullOrWhiteSpace(colName))
                colName = "Column" + c;

            dt.Columns.Add(colName);
        }

        // Data
        for (int r = 2; r <= rows; r++)
        {
            DataRow dr = dt.NewRow();

            for (int c = 1; c <= cols; c++)
            {
                var cell = ws.Cells[r, c];

                if (cell.Value == null)
                {
                    dr[c - 1] = DBNull.Value;
                    continue;
                }

                string excelFormat = cell.Style.Numberformat.Format.ToLower();

                // Handle Excel Date/DateTime
                if (cell.Value is double || cell.Value is decimal)
                {
                    double oaDate = Convert.ToDouble(cell.Value);

                    // Valid Excel date range
                    if (oaDate > 0 && oaDate < 2958465)
                    {
                        DateTime dtValue = DateTime.FromOADate(oaDate);

                        // Date + Time
                        if (excelFormat.Contains("hh") || excelFormat.Contains("am/pm"))
                        {
                            dr[c - 1] = dtValue.ToString("MM-dd-yyyy hh:mm:ss tt");
                        }
                        else if (excelFormat.Contains("yy") || excelFormat.Contains("dd"))
                        {
                            // Date only
                            dr[c - 1] = dtValue.ToString("MM-dd-yyyy");
                        }
                        else
                        {
                            dr[c - 1] = cell.Text.Trim();
                        }

                        continue;
                    }
                }

                // Direct DateTime
                if (cell.Value is DateTime dateTime)
                {
                    if (excelFormat.Contains("hh") || excelFormat.Contains("am/pm"))
                    {
                        dr[c - 1] = dateTime.ToString("MM-dd-yyyy hh:mm:ss tt");
                    }
                    else
                    {
                        dr[c - 1] = dateTime.ToString("MM-dd-yyyy");
                    }
                }
                else
                {
                    dr[c - 1] = cell.Text.Trim();
                }
            }

            dt.Rows.Add(dr);
        }

        return dt;
    }
}
