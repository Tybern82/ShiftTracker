using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using com.tybern.ShiftTracker;
using com.tybern.ShiftTracker.data;
using com.tybern.ShiftTracker.db;

namespace ShiftTrackerGUI;

public partial class ShiftWeekView : UserControl {

    public event CommandEvent? onClose;

    private bool isLoaded = false;

    public ShiftWeekView() {
        InitializeComponent();

        pShiftMonday.addDefaultHandlers();
        pShiftTuesday.addDefaultHandlers();
        pShiftWednesday.addDefaultHandlers();
        pShiftThursday.addDefaultHandlers();
        pShiftFriday.addDefaultHandlers();
        pShiftSaturday.addDefaultHandlers();
        pShiftSunday.addDefaultHandlers();

        dtSelectWeek.SelectedDate = DateTime.Now.Date;
        loadDate(DateTime.Now.Date);

        dtSelectWeek.SelectedDateChanged += (sender, args) => {
            saveAll();
            if (dtSelectWeek.SelectedDate.HasValue) loadDate(dtSelectWeek.SelectedDate.Value.Date);
        };

        btnCloseDialog.Click += (sender, args) => onClose?.Invoke();

        onClose += () => saveAll(); // save on closing
    }

    private void loadDate(DateTime dt) {
        // Loads the week containing the requested date, and switches to the tab for that day
        DayOfWeek day = dt.DayOfWeek;
        switch (day) {
            case DayOfWeek.Monday:
                loadMonday(dt);
                tabDays.SelectedItem = tMonday;
                break;

            case DayOfWeek.Tuesday:
                loadMonday(dt - TimeSpan.FromDays(1));
                tabDays.SelectedItem = tTuesday;
                break;

            case DayOfWeek.Wednesday:
                loadMonday(dt - TimeSpan.FromDays(2));
                tabDays.SelectedItem = tWednesday;
                break;

            case DayOfWeek.Thursday:
                loadMonday(dt - TimeSpan.FromDays(3));
                tabDays.SelectedItem = tThursday;
                break;

            case DayOfWeek.Friday:
                loadMonday(dt - TimeSpan.FromDays(4));
                tabDays.SelectedItem = tFriday;
                break;

            case DayOfWeek.Saturday:
                loadMonday(dt - TimeSpan.FromDays(5));
                tabDays.SelectedItem = tSaturday;
                break;

            case DayOfWeek.Sunday:
                loadMonday(dt - TimeSpan.FromDays(6));
                tabDays.SelectedItem = tSunday;
                break;
        }
    }

    private void loadMonday(DateTime monDate) {
        pShiftMonday.ActiveShift = DBShiftTracker.Instance.loadWorkShift(monDate) ?? new WorkShift(monDate);
        pShiftTuesday.ActiveShift = DBShiftTracker.Instance.loadWorkShift(monDate + TimeSpan.FromDays(1)) ?? new WorkShift(monDate + TimeSpan.FromDays(1));
        pShiftWednesday.ActiveShift = DBShiftTracker.Instance.loadWorkShift(monDate + TimeSpan.FromDays(2)) ?? new WorkShift(monDate + TimeSpan.FromDays(2));
        pShiftThursday.ActiveShift = DBShiftTracker.Instance.loadWorkShift(monDate + TimeSpan.FromDays(3)) ?? new WorkShift(monDate + TimeSpan.FromDays(3));
        pShiftFriday.ActiveShift = DBShiftTracker.Instance.loadWorkShift(monDate + TimeSpan.FromDays(4)) ?? new WorkShift(monDate + TimeSpan.FromDays(4));
        pShiftSaturday.ActiveShift = DBShiftTracker.Instance.loadWorkShift(monDate + TimeSpan.FromDays(5)) ?? new WorkShift(monDate + TimeSpan.FromDays(5));
        pShiftSunday.ActiveShift = DBShiftTracker.Instance.loadWorkShift(monDate + TimeSpan.FromDays(6)) ?? new WorkShift(monDate + TimeSpan.FromDays(6));

        isLoaded = true;
    }

    public void saveAll() {
        // Called to save the current view of dates
        if (isLoaded) {
            // only save if the fields were loaded (skips potential saving on first load)
            DBShiftTracker.Instance.save(pShiftMonday.ActiveShift);
            DBShiftTracker.Instance.save(pShiftTuesday.ActiveShift);
            DBShiftTracker.Instance.save(pShiftWednesday.ActiveShift);
            DBShiftTracker.Instance.save(pShiftThursday.ActiveShift);
            DBShiftTracker.Instance.save(pShiftFriday.ActiveShift);
            DBShiftTracker.Instance.save(pShiftSaturday.ActiveShift);
            DBShiftTracker.Instance.save(pShiftSunday.ActiveShift);
        }
    }
}