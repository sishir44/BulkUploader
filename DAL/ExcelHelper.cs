using OfficeOpenXml;
using System.Data;

public static class ExcelHelper
{
    public static DataTable ExcelToDataTable(ExcelWorksheet ws)
    {
        DataTable dt = new DataTable();

        int rows = ws.Dimension.End.Row;
        int cols = ws.Dimension.End.Column;

        // Header row
        for (int c = 1; c <= cols; c++)
        {
            string colName = ws.Cells[1, c].Text.Trim();
            dt.Columns.Add(colName);
        }

        // Data rows
        for (int r = 2; r <= rows; r++)
        {
            DataRow dr = dt.NewRow();

            for (int c = 1; c <= cols; c++)
                dr[c - 1] = ws.Cells[r, c].Text.Trim();

            dt.Rows.Add(dr);
        }

        return dt;
    }
}
