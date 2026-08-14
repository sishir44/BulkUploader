using OfficeOpenXml;
using System;
using System.Collections.Generic;
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
                    dr[c - 1] = cell.Text.Trim() ?? "";
                    //dr[c - 1] = Convert.ToString(cell.Value)?.Trim() ?? "";
                }
            }
            dt.Rows.Add(dr);
        }
        return dt;
    }

    public static DataTable ExcelToCommissionDataTable(ExcelWorksheet ws)
    {
        if (ws == null)
            throw new ArgumentNullException(nameof(ws));

        if (ws.Dimension == null)
            return new DataTable();

        int firstRow = ws.Dimension.Start.Row;
        int lastRow = ws.Dimension.End.Row;
        int firstCol = ws.Dimension.Start.Column;
        int lastCol = ws.Dimension.End.Column;

        int rowCount = lastRow - firstRow;
        int colCount = lastCol - firstCol + 1;

        DataTable dt = new DataTable
        {
            MinimumCapacity = rowCount
        };

        // Headers
        for (int c = firstCol; c <= lastCol; c++)
        {
            string columnName = Convert.ToString(
                ws.Cells[firstRow, c].Value)?.Trim();

            if (string.IsNullOrWhiteSpace(columnName))
                columnName = "Column" + c;

            dt.Columns.Add(columnName, typeof(string));
        }

        // Cache Excel formats
        Dictionary<int, string> formatCache =
            new Dictionary<int, string>();

        dt.BeginLoadData();

        try
        {
            for (int r = firstRow + 1; r <= lastRow; r++)
            {
                object[] row = new object[colCount];

                for (int c = firstCol; c <= lastCol; c++)
                {
                    int index = c - firstCol;

                    try
                    {
                        var cell = ws.Cells[r, c];
                        object value = cell.Value;

                        if (value == null)
                        {
                            row[index] = DBNull.Value;
                            continue;
                        }

                        // Get cached format
                        string format = "";

                        try
                        {
                            int styleId = cell.StyleID;

                            if (!formatCache.TryGetValue(styleId, out format))
                            {
                                format = cell.Style.Numberformat.Format ?? "";
                                format = format.ToLowerInvariant();
                                formatCache[styleId] = format;
                            }
                        }
                        catch
                        {
                            format = "";
                        }

                        // DateTime
                        if (value is DateTime date)
                        {
                            row[index] =
                                (format.Contains("hh") ||
                                 format.Contains("am/pm"))
                                ? date.ToString("MM-dd-yyyy hh:mm:ss tt")
                                : date.ToString("MM-dd-yyyy");

                            continue;
                        }

                        // Excel serial date
                        if (value is double || value is decimal)
                        {
                            double number = Convert.ToDouble(value);

                            if (number > 0 && number < 2958465)
                            {
                                 date =
                                    DateTime.FromOADate(number);

                                row[index] =
                                    (format.Contains("hh") ||
                                     format.Contains("am/pm"))
                                    ? date.ToString("MM-dd-yyyy hh:mm:ss tt")
                                    : date.ToString("MM-dd-yyyy");

                                continue;
                            }
                        }

                        // Normal value
                        row[index] =
                            Convert.ToString(value)?.Trim() ?? "";
                    }
                    catch (Exception ex)
                    {
                        // Skip problematic cell
                        row[index] = DBNull.Value;

                        System.Diagnostics.Debug.WriteLine(
                            $"Excel cell error - Row: {r}, " +
                            $"Column: {c}, Error: {ex.Message}");
                    }
                }

                dt.LoadDataRow(row, false);
            }
        }
        finally
        {
            dt.EndLoadData();
        }

        return dt;
    }
}

