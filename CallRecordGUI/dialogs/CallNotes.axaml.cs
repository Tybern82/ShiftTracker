using System.Linq.Expressions;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using com.tybern.CallRecordCore;
using com.tybern.CallRecordCore.dialogs;
using static com.tybern.CallRecordCore.dialogs.CallNotesResult;
using static com.tybern.CallRecordCore.dialogs.TransferRequestResult;

namespace CallRecordGUI.dialogs;

public partial class CallNotes : Window {

    private CallType Result { get; set; } = CallType.Helpdesk;
    private string Text { get; set; } = string.Empty;

    public CallNotes() {
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

        txtOther.TextChanged += (sender, args) => { Text = (txtOther != null && txtOther.Text != null) ? txtOther.Text : string.Empty; };
        rCallTypeNBN.IsChecked = true;
        updateChecked(rCallTypeNBN, CallType.NBN);

        rCallTypeSME.IsCheckedChanged += (sender, args) => updateChecked(rCallTypeSME, CallType.Helpdesk);
        rCallTypeMobile.IsCheckedChanged += (sender, args) => updateChecked(rCallTypeMobile, CallType.Mobile);
        rCallTypeNBN.IsCheckedChanged += (sender, args) => updateChecked(rCallTypeNBN, CallType.NBN);
        rCallTypeADSL.IsCheckedChanged += (sender, args) => updateChecked(rCallTypeADSL, CallType.ADSL);
        rCallTypeeMail.IsCheckedChanged += (sender, args) => updateChecked(rCallTypeeMail, CallType.eMail);
        rCallTypeBilling.IsCheckedChanged += (sender, args) => updateChecked(rCallTypeBilling, CallType.Billing);
        rCallTypePA.IsCheckedChanged += (sender, args) => updateChecked(rCallTypePA, CallType.PA);
        rCallTypePrepaid.IsCheckedChanged += (sender, args) => updateChecked(rCallTypePrepaid, CallType.Prepaid);
        rCallTypePSTN.IsCheckedChanged += (sender, args) => updateChecked(rCallTypePSTN, CallType.PSTN);
        rCallTypeOpticomm.IsCheckedChanged += (sender, args) => updateChecked(rCallTypeOpticomm, CallType.Opticomm);
        rCallTypeFetchTV.IsCheckedChanged += (sender, args) => updateChecked(rCallTypeFetchTV, CallType.FetchTV);
        rCallTypeHomeWireless.IsCheckedChanged += (sender, args) => updateChecked(rCallTypeHomeWireless, CallType.HomeWireless);
        rCallTypePlatinum.IsCheckedChanged += (sender, args) => updateChecked(rCallTypePlatinum, CallType.Platinum);
        rCallTypeMisrouted.IsCheckedChanged += (sender, args) => updateChecked(rCallTypeMisrouted, CallType.Misrouted);
        rCallTypeOther.IsCheckedChanged += (sender, args) => updateChecked(rCallTypeOther, CallType.Other);

        btnSaveNotes.Click += (sender, args) => {
            CallRecordCore.Instance.Messages.Enqueue(new CallNotesResult(Result, Text));
            this.Close();
        };
    }

    private void setContent(RadioButton rControl, com.tybern.CallRecordCore.dialogs.CallNotesResult.CallType option) {
        rControl.Content = com.tybern.CallRecordCore.dialogs.CallNotesResult.GetText(option);
        ToolTip.SetTip(rControl, com.tybern.CallRecordCore.dialogs.CallNotesResult.GetToolTip(option));
    }

    private void updateChecked(RadioButton rControl, CallType value) {
        if (rControl.IsChecked == true) Result = value;
    }
}