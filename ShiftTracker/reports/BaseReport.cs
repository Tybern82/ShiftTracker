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
    public abstract class BaseReport {

        private bool isPasswordProtected = true;

        public BaseReport(bool isPasswordProtected = true) {
            this.isPasswordProtected = isPasswordProtected;
        }

        protected Document? GeneratedDocument { get; set; }

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
            if (isPasswordProtected) {
                report.SecuritySettings.OwnerPassword = TrackerSettings.Instance.PDFPassword;
                report.SecuritySettings.UserPassword = TrackerSettings.Instance.PDFPassword;
                report.SecurityHandler.SetEncryptionToV5();
            }
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

            composeDocument(_result);

            GeneratedDocument = _result;
            return _result;
        }

        protected abstract void composeDocument(Document body);
    }
}
