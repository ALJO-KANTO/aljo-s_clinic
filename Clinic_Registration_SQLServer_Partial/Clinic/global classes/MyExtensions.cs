using Clinic_Business;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Clinic.global_classes
{
    public static class MyExtensions
    {
        // دالة واحدة تقوم إما بتحديث الصف إذا وجدته، أو إضافته إذا لم يكن موجوداً

        public static void UpsertRow(
    this DataTable dt,
    DataRow newRow,
    int primaryKeyValue)
        {
            if (dt == null || newRow == null)
                return;

            // Set the primary key if missing
            if (dt.PrimaryKey == null || dt.PrimaryKey.Length == 0)
            {
                if (dt.Columns.Contains("AppointmentID"))
                {
                    dt.PrimaryKey = new DataColumn[]
                    {
                dt.Columns["AppointmentID"]
                    };
                }
                else
                {
                    throw new InvalidOperationException(
                        "DataTable does not contain AppointmentID.");
                }
            }

            DataRow targetRow = dt.Rows.Find(primaryKeyValue);

            bool isNew = targetRow == null;

            if (isNew)
                targetRow = dt.NewRow();

            foreach (DataColumn col in dt.Columns)
            {
                if (col.ReadOnly || col.AutoIncrement)
                    continue;

                if (newRow.Table.Columns.Contains(col.ColumnName))
                {
                    object value = newRow[col.ColumnName];

                    if (value != null && value != DBNull.Value)
                    {
                        targetRow[col.ColumnName] = value;
                    }
                    else if (!col.AllowDBNull &&
                             col.DataType == typeof(string))
                    {
                        targetRow[col.ColumnName] = "";
                    }
                    else if (col.AllowDBNull)
                    {
                        targetRow[col.ColumnName] = DBNull.Value;
                    }
                }
                else if (isNew && !col.AllowDBNull &&
                         col.DataType == typeof(string))
                {
                    targetRow[col.ColumnName] = "";
                }
            }

            if (isNew)
                dt.Rows.Add(targetRow);

            dt.AcceptChanges();
        }


        public static List<int> GetCheckedIDs(this CheckedListBox clb, string idColumnName)
        {
            return clb.CheckedItems.OfType<DataRowView>()
                      .Select(item => (int)item[idColumnName])
                      .ToList();
        }

      


    }
}
