using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using com.tybern.CMDProcessor;
using com.tybern.ShiftTracker;
using com.tybern.ShiftTracker.data;
using com.tybern.ShiftTracker.db;
using com.tybern.ShiftTracker.enums;
using com.tybern.ShiftTracker.reports;
using EZFontResolverNS;
using MimeKit;
using PdfSharp.Fonts;
using ShiftTrackerGUI.ViewModels;
using StateMachine;

namespace ShiftTrackerGUI.Views;

public partial class MainWindow : Window {

    private static bool isFRSet = false;
    private static void setFontResolver() {
        if (!isFRSet) {
            EZFontResolver fonts = EZFontResolver.Get;
            GlobalFontSettings.FontResolver = fonts;
            GlobalFontSettings.FallbackFontResolver = new SubstitutingFontResolver.SubstitutingFontResolver();
            string path = Path.GetDirectoryName(Environment.ProcessPath) ?? Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location) ?? ".";   // Try ProcessPath; Fallback to Assembly path; Fallback to current directory
            LOG.Info("Program Path: " + path);
            LOG.Debug("Loading Font: ZT Otez");
            fonts.AddFont("ZT Otez", PdfSharp.Drawing.XFontStyleEx.Regular, Path.Combine(path, @"fonts\ZtOtezRegular-0v504.ttf"));
            fonts.AddFont("ZT Otez", PdfSharp.Drawing.XFontStyleEx.Italic, Path.Combine(path, @"fonts\ZtOtezItalic-ovYyV.ttf"));
            LOG.Debug("Loading Font: Maragsâ");
            fonts.AddFont("Maragsa Display", PdfSharp.Drawing.XFontStyleEx.Regular, Path.Combine(path, @"fonts\MaragsaDisplay-GO6PD.ttf"));
        }
        isFRSet = true;
    }

    private StatusBarModel statusBar;

    protected static NLog.Logger LOG = NLog.LogManager.GetCurrentClassLogger();
    public MainWindow() {
        InitializeComponent();
        setFontResolver();

        CommandProcessor.RunAsUIThread += (cmd) => {
            if (Dispatcher.UIThread.CheckAccess()) {
                cmd.Process();  // already on UIThread, just run
            } else {
                Dispatcher.UIThread.Invoke(() => cmd.Process());    // delegate to run later on the UIThread
            }
        };

        /* Used to load old CallRecordDB
        string dbName = "CallRecordGUI.db";
        com.tybern.ShiftTracker.data.old.CallLog cLog = new (dbName);
        com.tybern.ShiftTracker.data.old.SurveyLog sLog = new (cLog.conn);
        com.tybern.ShiftTracker.data.old.CallLogConversionResult callLogs = cLog.convertAll();
        SortedSet<CallRecord> calls = callLogs.Calls;
        bool surveyCheck = sLog.updateSurveyStatus(calls, sLog.LoadAll());

        LOG.Info("Found <" + callLogs.Notes.Count + ">");
        LOG.Info("Found <" + calls.Count + "> calls");
        LOG.Info("Survey Update: " + surveyCheck);

        foreach (NoteRecord note in callLogs.Notes) DBShiftTracker.Instance.save(note);
        foreach (CallRecord call in calls) DBShiftTracker.Instance.save(call);
        */

        // MAKE SURE we don't try to access anything from TrackerSettings before this line
        TrackerSettings.Instance.loadConfigFile();

        Utility.setPosition(this, TrackerSettings.Instance.MainWindowPosition);

        statusBar = new StatusBarModel();
        pStatusBar.DataContext = statusBar;

        statusBar.StatusVersion = TrackerSettings.Instance.VersionString;

        pMainView.vSettings.pSMTPSettings.SMTP = TrackerSettings.Instance.SMTP;
        pMainView.ViewModel.PDFPassword = TrackerSettings.Instance.PDFPassword; // ensure reload now the config file has been loaded
        pMainView.ViewModel.MeetingTime = TrackerSettings.Instance.MeetingTime;

        pMainView.btnDailyReportSend.Click += async (sender, args) => {
            // TODO: Move to separate thread (CMDProcessor?)
            DateTime dt = pMainView.dtDailyReport.SelectedDate ?? DateTime.Today;
            DailyReport dr = new DailyReport(dt);
            string reportName = "Report " + dt.ToString(DBShiftTracker.FORMAT_DATE);
            await doSendReport(dr, reportName);
        };

        pMainView.btnDailyReportSave.Click += async (sender, args) => {
            DateTime dt = pMainView.dtDailyReport.SelectedDate ?? DateTime.Today;
            DailyReport dr = new DailyReport(dt);
            string reportName = "Report " + dt.ToString(DBShiftTracker.FORMAT_DATE);
            await doSaveReport(dr, reportName);
        };

        pMainView.btnEOFYReportSend.Click += async (sender, args) => {
            DateTime dt = pMainView.dtEOFYReport.SelectedDate ?? DateTime.Today;
            WFHReport dr = new WFHReport(dt);
            string reportName = "WFH Annual Report " + dr.StartYear + "-" + (dr.StartYear + 1);
            await doSendReport(dr, reportName);
        };

        pMainView.btnEOFYReportSave.Click += async (sender, args) => {
            DateTime dt = pMainView.dtEOFYReport.SelectedDate ?? DateTime.Today;
            WFHReport dr = new WFHReport(dt);
            string reportName = "WFH Annual Report " + dr.StartYear + "-" + (dr.StartYear + 1);
            await doSaveReport(dr, reportName);
        };

        pMainView.btnSystemIssue.Click += (sender, args) => {
            doImmediateBreak(BreakType.SystemIssue);
        };

        pMainView.btnCoaching.Click += (sender, args) => {
            doImmediateBreak(BreakType.Coaching);
        };

        pMainView.pShiftTimes.onEditWeek += () => {
            DateTime currentDate = pMainView.pShiftTimes.fDateSelector.SelectedDate.HasValue ? pMainView.pShiftTimes.fDateSelector.SelectedDate.Value.Date : DateTime.Now.Date;
            DBShiftTracker.Instance.save(pMainView.pShiftTimes.ActiveShift);    // save any current edits before trying to load
            ShiftWeekWindow dlgEditWeek = new(currentDate);
            // Reload the date from the current page once the dialog is closed
            dlgEditWeek.Closed += (sender, args) => {
                pMainView.pShiftTimes.ActiveShift = DBShiftTracker.Instance.loadWorkShift(currentDate) ?? new WorkShift(currentDate);
                pMainView.ViewModel.ShiftState.ActiveShift = pMainView.pShiftTimes.ActiveShift;
            };
            dlgEditWeek.ShowDialog(this);
        };

        pMainView.pExtraControls.onAddNote += () => {
            ExtraNotesWindow wndNotes = new ExtraNotesWindow();
            wndNotes.onCancel += () => {
                // Just close - don't save
                wndNotes.Close();
            };
            wndNotes.onSave += (notes) => {
                NoteRecord nr = new NoteRecord(DateTime.Now) {NoteContent = notes};
                pMainView.ViewModel.CallState.updateNote(nr);
                DBShiftTracker.Instance.save(nr);
                wndNotes.Close();
            };
            wndNotes.ShowDialog(this);
        };

        pMainView.pExtraControls.onSkipSurvey += () => {
            SkipSurveyWindow wndSkipSurvey = new SkipSurveyWindow();
            wndSkipSurvey.vSkipSurvey.onSkipSurvey += (reason) => {
                if (pMainView.ViewModel.CallState.CurrentCall != null) {
                    pMainView.ViewModel.CallState.CurrentCall.Survey = reason;
                    LOG.Info("Update Survey: " + reason.ToString());
                }
            };
            wndSkipSurvey.vSkipSurvey.onSave += () => {
                string note = wndSkipSurvey.vSkipSurvey.pNotes.NoteContent;
                if (!string.IsNullOrWhiteSpace(note)) pMainView.ViewModel.CallState.appendNote("Survey: [" + note + "]");
                pMainView.pExtraControls.DisableButton(ExtraControlsView.ExtraControls.SurveyControls); // disable survey controls
                wndSkipSurvey.Close();
            };
            wndSkipSurvey.ShowDialog(this);
        };

        pMainView.pCallControls.onCallback += () => {
            CallbackWindow wndCallback = new CallbackWindow();

            wndCallback.vCallback.onCallback += (type, details) => {
                pMainView.ViewModel.CallState.appendNote("Callback - " + type);
                pMainView.ViewModel.CallState.appendNote(details);
                wndCallback.Close();
            };

            wndCallback.ShowDialog(this);
        };

        pMainView.pCallControls.onCallTransfer += () => {
            CallTransferWindow wndCallTransfer = new CallTransferWindow(pMainView.ViewModel.CallState); // link to Common Notes

            wndCallTransfer.vCallTransfer.onTransferCall += (type, time, notes) => {
                wndCallTransfer.Close();
                pMainView.ViewModel.CallState.callState.gotoState(State.getState(CallSM.CALL_INWRAP));
            };
            wndCallTransfer.vCallTransfer.onTransferClose += (type, time, notes) => {
                wndCallTransfer.Close();
                pMainView.ViewModel.CallState.callState.gotoState(State.getState(CallSM.CALL_ACTIVE));
            };

            wndCallTransfer.ShowDialog(this);
        };

        Transition? initialCall = pMainView.ViewModel.CallState.callState.getTransition(State.getState(CallSM.CALL_WAITING), State.getState(CallSM.CALL_ACTIVE));
        if (initialCall != null) {
            initialCall.onTransition += (initial, final) => {
                pMainView.cmbCallType.IsEnabled = true;
                if (pMainView.ViewModel.CallState.CurrentCall != null)
                    pMainView.cmbCallType.SelectedItem = pMainView.ViewModel.CallState.CurrentCall.Type;
            };
        } else {
            LOG.Error("Missing Transition: <Waiting> -> <Active>");
        }

        Transition? endCall = pMainView.ViewModel.CallState.callState.getTransition(State.getState(CallSM.CALL_INWRAP), State.getState(CallSM.CALL_WAITING));
        if (endCall != null) {
            endCall.onTransition += (oldState, newState) => {
                if (pMainView.ViewModel.CallState.CurrentCall != null) {
                    if (pMainView.ViewModel.CallState.CurrentCall.Survey == com.tybern.ShiftTracker.enums.SurveyStatus.Missing) {
                        // No survey marker recorded on current call - show the dialog automatically
                        // TODO: showDialog SkipSurvey
                    }
                }
                pMainView.cmbCallType.IsEnabled = false;
            };
        } else {
            LOG.Error("Mising Transition: <Wrap> -> <Waiting>");
        }

        Transition? endTransfer = pMainView.ViewModel.CallState.callState.getTransition(State.getState(CallSM.CALL_TRANSFER), State.getState(CallSM.CALL_INWRAP));
        if (endTransfer != null) {
            endTransfer.onTransition += (oldState, newState) => {
                if ((pMainView.ViewModel.CallState.CurrentCall != null) && (pMainView.ViewModel.CallState.CurrentCall?.Survey == SurveyStatus.Missing)) {
                    // Record Transfer for Survey if not already set on the call (if already set, will retain existing value)
                    pMainView.ViewModel.CallState.CurrentCall.Survey = SurveyStatus.Transfer;
                    pMainView.pExtraControls.DisableButton(ExtraControlsView.ExtraControls.SurveyControls); // disable survey controls
                }
            };
        } else {
            LOG.Error("Missing Transition: <Transfer> -> <Wrap>");
        }

        State.getState(CallSM.CALL_SME).enterState += (s, param) => {
            SMECallWindow wndSMECall = new SMECallWindow(pMainView.ViewModel.CallState);    // link to common notes

            State nextState = ((param != null) && (param is State)) ? (State)param : State.getState(CallSM.CALL_WAITING);   // should always be non-null, but default return to Waiting

            wndSMECall.Closed += (sender, args) => {
                pMainView.ViewModel.CallState.callState.gotoState(nextState);   // change state when closing SME call window
            };

            wndSMECall.ShowDialog(this);
        };

        this.Closing += (sender, args) => {
            // Save the current date to the DB before we close
            WorkShift currentShift = pMainView.pShiftTimes.ActiveShift;
            DBShiftTracker.Instance.save(currentShift);

            // Update the current window position and save the current config settings
            TrackerSettings.Instance.MainWindowPosition.PositionX = this.Position.X;
            TrackerSettings.Instance.MainWindowPosition.PositionY = this.Position.Y;
            TrackerSettings.Instance.MainWindowPosition.Width = this.Width;
            TrackerSettings.Instance.MainWindowPosition.Height = this.Height;

            TrackerSettings.Instance.saveConfigFile();
        };
    }

    private void doImmediateBreak(BreakType type) {
        ImmediateBreakWindow wndBreak = new ImmediateBreakWindow();
        string desc = EnumConverter.GetEnumDescription(type);
        wndBreak.Title = desc;
        wndBreak.vImmediateBreak.pNotes.Title = desc;
        wndBreak.vImmediateBreak.onClose += () => {
            wndBreak.Close();
            string notes = wndBreak.vImmediateBreak.pNotes.NoteContent;
            if (!string.IsNullOrWhiteSpace(notes)) {
                NoteRecord nr = wndBreak.vImmediateBreak.pTimer.createNote(notes);
                nr.prependNote(desc);
                pMainView.ViewModel.CallState.updateNote(nr);
                DBShiftTracker.Instance.save(nr);
            }
            WorkBreak brk = wndBreak.vImmediateBreak.pTimer.createBreak(type);
            pMainView.ViewModel.ShiftState.ActiveShift.Breaks.Add(brk); // don't update NextBreak
        };
        wndBreak.ShowDialog(this);
    }

    private async Task doSendReport(BaseReport report, string reportName) {
        statusBar.StatusText = "Generating report...";
        PdfSharp.Pdf.PdfDocument reportDocument = report.generate();
        MemoryStream output = new MemoryStream();
        reportDocument.Save(output);
        statusBar.StatusText = "Sending report...";
        List<MimeEntity> attachments = new List<MimeEntity>();
        MimeEntity e = MimeEntity.Load(new ContentType("application", "pdf"), output);
        e.ContentType.Name = reportName + ".pdf";
        attachments.Add(e);
        SMTPRecord.SMTPSendResponse sendResult = TrackerSettings.Instance.SMTP.sendMail(reportName, "see attached", attachments);
        statusBar.StatusText = ((sendResult.Success ? "Sent" : "Not Sent") + (string.IsNullOrWhiteSpace(sendResult.Error) ? string.Empty : ": " + sendResult.Error));
    }

    private async Task doSaveReport(BaseReport report, string reportName) {
        statusBar.StatusText = "Generating report...";
        PdfSharp.Pdf.PdfDocument reportDocument = report.generate();

        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel != null) {
            var file = await topLevel.StorageProvider.SaveFilePickerAsync(new Avalonia.Platform.Storage.FilePickerSaveOptions {
                Title = "Save Report File",
                DefaultExtension = "pdf",
                SuggestedFileName = reportName + ".pdf",
                SuggestedStartLocation = await topLevel.StorageProvider.TryGetWellKnownFolderAsync(WellKnownFolder.Documents)
            });

            if (file is not null) {
                string fname = file.TryGetLocalPath() ?? Path.Combine(file.Path.AbsolutePath, file.Name);
                reportDocument.Save(fname);
                statusBar.StatusText = "Report saved...";
            }
        }
    }
}
