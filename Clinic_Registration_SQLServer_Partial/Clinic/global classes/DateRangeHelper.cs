using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Clinic.global_classes
{
    public static class DateRangeHelper
    {
        public static void SetDateRange(string option, DateTimePicker dtpFrom, DateTimePicker dtpTo)
        {
            DateTime today = DateTime.Today;
            DateTime fromDate = today;
            DateTime toDate = today;

            switch (option)
            {
                case "All":
                    fromDate = dtpFrom.MinDate;
                    toDate = dtpTo.MaxDate;
                    break;

                case "Today":
                    fromDate = today;
                    toDate = today;
                    break;

                case "Yesterday":
                    fromDate = today.AddDays(-1);
                    toDate = today.AddDays(-1);
                    break;

                case "This Week":
                    int diff = (7 + (today.DayOfWeek - DayOfWeek.Sunday)) % 7;
                    fromDate = today.AddDays(-diff);
                    toDate = fromDate.AddDays(6);
                    break;

                case "This Week-To-Date":
                    diff = (7 + (today.DayOfWeek - DayOfWeek.Sunday)) % 7;
                    fromDate = today.AddDays(-diff);
                    toDate = today;
                    break;

                case "This Month":
                    fromDate = new DateTime(today.Year, today.Month, 1);
                    toDate = fromDate.AddMonths(1).AddDays(-1);
                    break;

                case "This Month-to-Date":
                    fromDate = new DateTime(today.Year, today.Month, 1);
                    toDate = today;
                    break;

                case "This Fiscal Quarter":
                    GetFiscalQuarter(today, out fromDate, out toDate);
                    break;

                case "This Fiscal Quarter-To-Date":
                    GetFiscalQuarter(today, out fromDate, out toDate);
                    toDate = today;
                    break;

                case "This Fiscal Quarter Year":
                    GetFiscalYear(today, out fromDate, out toDate);
                    break;

                case "This Fiscal Quarter Year-To-Last Month":
                    GetFiscalYear(today, out fromDate, out toDate);
                    toDate = new DateTime(today.Year, today.Month, 1).AddDays(-1);
                    break;

                case "This Fiscal Quarter Year-To-Date":
                    GetFiscalYear(today, out fromDate, out toDate);
                    toDate = today;
                    break;
            }

            dtpFrom.Value = fromDate;
            dtpTo.Value = toDate;
        }

        private static void GetFiscalQuarter(DateTime today, out DateTime start, out DateTime end)
        {
            int quarterNumber = (today.Month - 1) / 3 + 1;
            start = new DateTime(today.Year, (quarterNumber - 1) * 3 + 1, 1);
            end = start.AddMonths(3).AddDays(-1);
        }

        private static void GetFiscalYear(DateTime today, out DateTime start, out DateTime end)
        {
            // Example: Fiscal year = Jan 1 – Dec 31
            start = new DateTime(today.Year, 1, 1);
            end = new DateTime(today.Year, 12, 31);
        }
    }
}
