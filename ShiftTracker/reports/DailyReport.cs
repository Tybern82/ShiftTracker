using System;
using System.Collections.Generic;
using System.Text;
using com.tybern.ShiftTracker.data;
using com.tybern.ShiftTracker.db;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Fields;
using MigraDoc.Rendering;
using PdfSharp.Fonts;
using PdfSharp.Pdf;
using PdfSharp.Quality;

namespace com.tybern.ShiftTracker.reports {
    public class DailyReport {

        private DateTime reportDate;

        public DailyReport(DateTime dt) {
            this.reportDate = dt;
        }

        private Document? GeneratedDocument { get; set; }

        public PdfDocument generate() {
            Document reportDoc = (GeneratedDocument == null) ? createDocument() : GeneratedDocument;
            var pdfRenderer = new PdfDocumentRenderer {
                Document = reportDoc,
                PdfDocument = {
                    PageLayout = PdfPageLayout.SinglePage,
                    ViewerPreferences = {
                        FitWindow = true
                    }
                }
            };
            pdfRenderer.RenderDocument();

            PdfDocument report = pdfRenderer.PdfDocument;

            // Set Security settings on document
            report.SecuritySettings.OwnerPassword = TrackerSettings.Instance.PDFPassword;
            report.SecuritySettings.UserPassword = TrackerSettings.Instance.PDFPassword;
            report.SecurityHandler.SetEncryptionToV5();
            report.SecuritySettings.PermitPrint = true;
            report.SecuritySettings.PermitModifyDocument = false;

            return report;
        }

        public Document createDocument() {
            Document _result = new Document();

            var h1 = _result.Styles[StyleNames.Heading1];
            if (h1 != null) {
                h1.Font.Name = "Glance Cherry";
                h1.ParagraphFormat.SpaceAfter = Unit.FromPoint(48);
                h1.ParagraphFormat.Alignment = ParagraphAlignment.Center;
                h1.Font.Size = Unit.FromPoint(48);
            }

            var h2 = _result.Styles[StyleNames.Heading2];
            if (h2 != null) {
                h2.Font.Name = "Glance Cherry";
                h2.ParagraphFormat.Alignment = ParagraphAlignment.Left;
                h2.ParagraphFormat.SpaceBefore = Unit.FromPoint(24);
                h2.ParagraphFormat.SpaceAfter = Unit.FromPoint(12);
                h2.Font.Size = Unit.FromPoint(24);
            }

            var norm = _result.Styles[StyleNames.Normal];
            if (norm != null) {
                norm.Font.Name = "Maragsa Display";
                norm.Font.Size = Unit.FromPoint(8);
                norm.ParagraphFormat.SpaceAfter = Unit.FromPoint(4);
                norm.ParagraphFormat.SpaceBefore = Unit.FromPoint(4);
            }

            var section = _result.AddSection();

            var paragraph = section.AddParagraph();
            paragraph.AddFormattedText("Daily Report <" + reportDate.ToString(DBShiftTracker.FORMAT_DATE) + ">", StyleNames.Heading1);

            var summaryA = section.AddParagraph();   // create summary here so it sits above the table, will be filled in below once the numbers are known
            var summaryB = section.AddParagraph();
            section.AddParagraph();

            var autoNotesHeader = section.AddParagraph();
            autoNotesHeader.AddFormattedText("Auto Notes", StyleNames.Heading2);
            var autoNotesA = section.AddParagraph();
            var autoNotesB = section.AddParagraph();
            var autoNotesC = section.AddParagraph();
            var autoNotesD = section.AddParagraph();
            var autoNotesE = section.AddParagraph();
            section.AddParagraph();

            var callTableHeader = section.AddParagraph();
            callTableHeader.AddFormattedText("Calls", StyleNames.Heading2);
            var callTable = section.AddTable();
            var cCallNumber = callTable.AddColumn(Unit.FromCentimeter(1));
            var cCallType = callTable.AddColumn(Unit.FromCentimeter(3));
            var cCallTime = callTable.AddColumn(Unit.FromCentimeter(2));
            var cWrapTime = callTable.AddColumn(Unit.FromCentimeter(2));
            var cHoldTime = callTable.AddColumn(Unit.FromCentimeter(2));
            var cSurvey = callTable.AddColumn(Unit.FromCentimeter(3));
            var cAutoNotes = callTable.AddColumn(Unit.FromCentimeter(3));
            section.AddParagraph();

            var headerRow = callTable.AddRow();
            headerRow.HeadingFormat = true;
            headerRow[0].AddParagraph("Call");
            headerRow[1].AddParagraph("Call Type");
            headerRow[2].AddParagraph("Call Length");
            headerRow[3].AddParagraph("Wrap Length");
            headerRow[4].AddParagraph("SME / Transfer");
            headerRow[5].AddParagraph("Survey");
            headerRow[6].AddParagraph("AutoNotes");

            SortedSet<CallRecord> calls = DBShiftTracker.Instance.loadCallRecords(reportDate);
            TimeSpan totalSMETime = TimeSpan.Zero;
            TimeSpan totalTransferTime = TimeSpan.Zero;
            TimeSpan totalWrapTime = TimeSpan.Zero;
            TimeSpan totalCallTime = TimeSpan.Zero;

            int totalCallbacks = 0; int totalTransfers = 0;
            int anGenerated = 0; int anEdited = 0; int anSaved = 0; int anManual = 0;

            int callCount = 0;
            foreach (CallRecord c in calls) {
                callCount++;    // index starts from 1
                totalSMETime += c.SMETime;
                totalTransferTime += c.TransferTime;
                totalWrapTime += c.WrapTime;
                TimeSpan callTime = c.CallTime;
                totalCallTime += callTime;

                totalCallbacks += c.CallbackCount;
                totalTransfers += c.TransferCount;

                anGenerated += (c.AutoNotesStatus.HasFlag(enums.AutoNotesStatus.Generated)) ? 1 : 0;
                anEdited += (c.AutoNotesStatus.HasFlag(enums.AutoNotesStatus.Edited)) ? 1 : 0;
                anSaved += (c.AutoNotesStatus.HasFlag(enums.AutoNotesStatus.Saved)) ? 1 : 0;
                anManual += (c.AutoNotesStatus.HasFlag(enums.AutoNotesStatus.Manual)) ? 1 : 0;

                var recordRow = callTable.AddRow();
                recordRow[0].AddParagraph(callCount.ToString());
                recordRow[1].AddParagraph(EnumConverter.GetEnumDescription(c.Type));
                recordRow[2].AddParagraph(callTime.ToString(DBShiftTracker.FORMAT_TIME));
                recordRow[3].AddParagraph(c.WrapTime.ToString(DBShiftTracker.FORMAT_TIME));
                recordRow[4].AddParagraph((c.SMETime + c.TransferTime).ToString(DBShiftTracker.FORMAT_TIME));
                recordRow[5].AddParagraph(EnumConverter.GetEnumDescription(c.Survey));
                recordRow[6].AddParagraph(c.AutoNotesStatus.ToString());
            }

            summaryA.Add(new Text("Calls: " + callCount));
            summaryA.Add(new Text("; Callbacks: " + totalCallbacks));
            summaryA.Add(new Text("; Transfers: " + totalTransfers));

            summaryB.Add(new Text("Total Call Time: " + totalCallTime.ToString(DBShiftTracker.FORMAT_TIME)));
            summaryB.Add(new Text("; Total Wrap: " + totalWrapTime.ToString(DBShiftTracker.FORMAT_TIME)));
            summaryB.Add(new Text("; Total SME: " + totalSMETime.ToString(DBShiftTracker.FORMAT_TIME)));
            summaryB.Add(new Text("; Total Transfer: " + totalTransferTime.ToString(DBShiftTracker.FORMAT_TIME)));

            autoNotesA.Add(new Text("Total Calls: " + callCount + "\n"));
            autoNotesB.Add(new Text("Total Generated Notes: " + anGenerated + " (" + (anGenerated * 100.0 / callCount).ToString("0.#") + "%)"));
            autoNotesC.Add(new Text("AutoNotes Edited: " + anEdited + " (" + (anEdited * 100.0 / anGenerated).ToString("0.#") + "%)"));
            autoNotesD.Add(new Text("AutoNotes Saved: " + anSaved + " (" + (anSaved * 100.0 / anGenerated).ToString("0.#") + "%)"));
            autoNotesE.Add(new Text("Manual Notes Added: " + anManual + " (" + (anManual * 100.0 / callCount).ToString("0.#") + "%)"));

            var notes = DBShiftTracker.Instance.loadNCNotes(reportDate);
            if (notes.Count > 0) {  // only add section if there are notes present on this date
                var notesHeader = section.AddParagraph();
                notesHeader.AddFormattedText("Notes", StyleNames.Heading2);
                var notesTable = section.AddTable();
                var nTime = notesTable.AddColumn(Unit.FromCentimeter(2));
                var nNote = notesTable.AddColumn(Unit.FromCentimeter(8));

                var nHeaderRow = notesTable.AddRow();
                nHeaderRow.HeadingFormat = true;
                nHeaderRow[0].AddParagraph("Time");
                nHeaderRow[1].AddParagraph("Note");

                foreach (var note in notes) {
                    var recordRow = notesTable.AddRow();
                    recordRow[0].AddParagraph(note.StartTime.ToString("HH\\:mm\\:ss"));
                    recordRow[1].AddParagraph(note.NoteContent);
                }
            }

            var footer = section.Footers.Primary;
            var footerPara = footer.AddParagraph();
            footerPara.Add(new DateField { Format = "yyyy-mm-dd" });
            footerPara.Format.Alignment = ParagraphAlignment.Center;

            GeneratedDocument = _result;
            return _result;
        }
    }
}
