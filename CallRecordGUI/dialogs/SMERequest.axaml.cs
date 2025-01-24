using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using com.tybern.CallRecordCore;
using com.tybern.CallRecordCore.dialogs;
using static com.tybern.CallRecordCore.dialogs.SMERequestResult;

namespace CallRecordGUI.dialogs;

public partial class SMERequest : Window {

    private OptionSMERequest Result { get; set; } = OptionSMERequest.Tools;
    private string Text { get; set; } = string.Empty;

    public SMERequest() {
        InitializeComponent();

        DataContext = CallRecordCore.Instance.UIProperties;

        setContent(rSMETools, OptionSMERequest.Tools);
        setContent(rSMEQuery, OptionSMERequest.Query);

        txtDetails.TextChanged += (sender, args) => { Text = (txtDetails != null && txtDetails.Text != null) ? txtDetails.Text : string.Empty; };

        rSMETools.IsCheckedChanged += (sender, args) => updateChecked(rSMETools, OptionSMERequest.Tools);
        rSMEQuery.IsCheckedChanged += (sender, args) => updateChecked(rSMEQuery, OptionSMERequest.Query);

        btnSaveSME.Click += (sender, args) => {
            CallRecordCore.Instance.Messages.Enqueue(new SMERequestResult(Result, Text));
            this.Close();
        };
    }

    private void updateChecked(RadioButton rControl, OptionSMERequest value) {
        if (rControl.IsChecked == true) Result = value;
    }

    private void setContent(RadioButton rControl, OptionSMERequest option) {
        rControl.Content = SMERequestResult.GetText(option);
        ToolTip.SetTip(rControl, SMERequestResult.GetToolTip(option));
    }
}