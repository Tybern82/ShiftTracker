using System.Linq.Expressions;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using CallRecordCore;
using static CallRecordCore.CallNotes;

namespace CallRecordGUI.dialogs;

public partial class CallNotes : Window {

    public CallNotesResult Result { get; set; } = new CallNotesResult(CallType.Helpdesk);

    private CallUI CallUI { get; }

    public CallNotes(CallUI callUI) {
        this.CallUI = callUI;

        InitializeComponent();

        setContent(rCallTypeSME, CallType.Helpdesk);
        setContent(rCallTypeMobile, CallType.Mobile);
        setContent(rCallTypeNBN, CallType.NBN);
        setContent(rCallTypeADSL, CallType.ADSL);
        setContent(rCallTypeeMail, CallType.eMail);
        setContent(rCallTypeBilling, CallType.Billing);
        setContent(rCallTypePA, CallType.PA);
        setContent(rCallTypePrepaid, CallType.Prepaid);
        setContent(rCallTypePSTN, CallType.PSTN);
        setContent(rCallTypeOpticomm, CallType.Opticomm);
        setContent(rCallTypeFetchTV, CallType.FetchTV);
        setContent(rCallTypeHomeWireless, CallType.HomeWireless);
        setContent(rCallTypePlatinum, CallType.Platinum);
        setContent(rCallTypeMisrouted, CallType.Misrouted);
        setContent(rCallTypeOther, CallType.Other);

        rCallTypeSME.IsCheckedChanged += (sender, args) => { if (rCallTypeSME.IsChecked == true) Result.CallType = CallType.Helpdesk; };
        rCallTypeMobile.IsCheckedChanged += (sender, args) => { if (rCallTypeMobile.IsChecked == true) Result.CallType = CallType.Mobile; };
        rCallTypeNBN.IsCheckedChanged += (sender, args) => { if (rCallTypeNBN.IsChecked == true) Result.CallType = CallType.NBN; };
        rCallTypeADSL.IsCheckedChanged += (sender, args) => { if (rCallTypeADSL.IsChecked == true) Result.CallType = CallType.ADSL; };
        rCallTypeeMail.IsCheckedChanged += (sender, args) => { if (rCallTypeeMail.IsChecked == true) Result.CallType = CallType.eMail; };
        rCallTypeBilling.IsCheckedChanged += (sender, args) => { if (rCallTypeBilling.IsChecked == true) Result.CallType = CallType.Billing; };
        rCallTypePA.IsCheckedChanged += (sender, args) => { if (rCallTypePA.IsChecked == true) Result.CallType = CallType.PA; };
        rCallTypePrepaid.IsCheckedChanged += (sender, args) => { if (rCallTypePrepaid.IsChecked == true) Result.CallType = CallType.Prepaid; };
        rCallTypePSTN.IsCheckedChanged += (sender, args) => { if (rCallTypePSTN.IsChecked == true) Result.CallType = CallType.PSTN; };
        rCallTypeOpticomm.IsCheckedChanged += (sender, args) => { if (rCallTypeOpticomm.IsChecked == true) Result.CallType = CallType.Opticomm; };
        rCallTypeFetchTV.IsCheckedChanged += (sender, args) => { if (rCallTypeFetchTV.IsChecked == true) Result.CallType = CallType.FetchTV; };
        rCallTypeHomeWireless.IsCheckedChanged += (sender, args) => { if (rCallTypeHomeWireless.IsChecked == true) Result.CallType = CallType.HomeWireless; };
        rCallTypePlatinum.IsCheckedChanged += (sender, args) => { if (rCallTypePlatinum.IsChecked == true) Result.CallType = CallType.Platinum; };
        rCallTypeMisrouted.IsCheckedChanged += (sender, args) => { if (rCallTypeMisrouted.IsChecked == true) Result.CallType = CallType.Misrouted; };
        rCallTypeOther.IsCheckedChanged += (sender, args) => { if (rCallTypeOther.IsChecked == true) Result.CallType = CallType.Other; };

        btnSaveNotes.Click += (sender, args) => { this.Close(); callUI.updateCallType(Result.CallType); };
        txtOther.TextChanged += onChange_txtOther;
    }

    private void setContent(RadioButton rControl, CallRecordCore.CallNotes.CallType option) {
        rControl.Content = CallRecordCore.CallNotes.getCallNotesOptionText(option);
        ToolTip.SetTip(rControl, CallRecordCore.CallNotes.getCallNotesOptionTooltip(option));
    }

    private void onChange_txtOther(object? sender, TextChangedEventArgs e) {
        Result.Text = (txtOther != null && txtOther.Text != null) ? txtOther.Text : string.Empty;
    }
}