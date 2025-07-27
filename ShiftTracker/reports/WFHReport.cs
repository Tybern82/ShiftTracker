using System;
using System.Collections.Generic;
using System.Text;
using com.tybern.ShiftTracker.data;
using com.tybern.ShiftTracker.db;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Fields;
using MigraDoc.Rendering;
using PdfSharp.Pdf;

namespace com.tybern.ShiftTracker.reports {
    public class WFHReport : BaseReport {

        public int StartYear { get; private set; }

        public WFHReport(DateTime dt) : base(false) {
            if (dt.Month < 7) {
                // Jan-Jun - set to prev year
                StartYear = dt.Year - 1;
            } else {
                // Jul-Dec - set to current year
                StartYear = dt.Year;
            }
        }

        protected override void composeDocument(Document body) {

            var section = body.AddSection();

            var paragraph = section.AddParagraph();
            paragraph.AddFormattedText("WFH Annual Report <" + StartYear + "-" + (StartYear + 1) + ">", StyleNames.Heading1);

            paragraph = section.AddParagraph();
            paragraph.AddFormattedText("Summary", StyleNames.Heading2);

            var summaryTotalHours = section.AddParagraph();
            var summaryTotalWithLunch = section.AddParagraph();

            paragraph = section.AddParagraph();
            paragraph.AddFormattedText("Daily Records", StyleNames.Heading2);
            var shiftTable = section.AddTable();
            var stDate = shiftTable.AddColumn();
            var stStart = shiftTable.AddColumn();
            var stEnd = shiftTable.AddColumn();
            var stShiftTime = shiftTable.AddColumn();
            var stUnpaidTime = shiftTable.AddColumn();

            var stHeaderRow = shiftTable.AddRow();
            stHeaderRow.HeadingFormat = true;
            stHeaderRow[0].AddParagraph("Date");
            stHeaderRow[1].AddParagraph("Start Time");
            stHeaderRow[2].AddParagraph("End Time");
            stHeaderRow[3].AddParagraph("Shift Hours");
            stHeaderRow[4].AddParagraph("Unpaid Hours");

            DateTime currDate = new DateTime(StartYear, 7, 1);     // 1-Jul-<StartYear>
            DateTime endDate = new DateTime(StartYear+1, 7, 1);     // 1-Jul-<StartYear+1>

            TimeSpan totalHours = TimeSpan.Zero;
            TimeSpan totalWithLunch = TimeSpan.Zero;

            while (currDate < endDate) {
                WorkShift? shift = DBShiftTracker.Instance.loadWorkShift(currDate);
                if (shift != null) {
                    TimeSpan totalShiftTime = shift.Length;
                    TimeSpan unpaidHours = TimeSpan.Zero;
                    TimeSpan unpaidWLunch = TimeSpan.Zero;
                    foreach (WorkBreak b in shift.Breaks) {
                        switch(b.Type) {
                            case enums.BreakType.LunchBreak:
                                unpaidHours += b.Length; break;

                            case enums.BreakType.PersonalLeave:
                            case enums.BreakType.UnpaidLeave:
                            case enums.BreakType.AnnualLeave:
                            case enums.BreakType.PublicHoliday:
                                unpaidHours += b.Length;
                                unpaidWLunch += b.Length;
                                break;
                        }
                    }

                    var row = shiftTable.AddRow();
                    row[0].AddParagraph(shift.CurrentDate.ToString(DBShiftTracker.FORMAT_DATE));
                    row[1].AddParagraph(shift.StartTime.ToString(DBShiftTracker.FORMAT_TIME));
                    row[2].AddParagraph(shift.EndTime.ToString(DBShiftTracker.FORMAT_TIME));
                    row[3].AddParagraph((totalShiftTime - unpaidHours).ToString(DBShiftTracker.FORMAT_TIME));
                    row[4].AddParagraph(unpaidWLunch.ToString(DBShiftTracker.FORMAT_TIME));

                    totalHours += (totalShiftTime - unpaidHours);
                    totalWithLunch += (totalShiftTime - unpaidWLunch);
                } // not every date will have a shift...
                currDate = currDate.AddDays(1);
            }

            summaryTotalHours.Add(new Text("Total Hours: " + totalHours.ToString("c") + " (" + getTotalHours(totalHours) + ")"));
            summaryTotalWithLunch.Add(new Text("Total Hours (with lunch): " + totalWithLunch.ToString("c") + " (" + getTotalHours(totalWithLunch) + ")"));
        }

        private string getTotalHours(TimeSpan ts) => ts.TotalHours.ToString("#.##");
    }
}
