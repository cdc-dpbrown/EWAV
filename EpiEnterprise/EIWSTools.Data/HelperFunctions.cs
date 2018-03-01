using System;

public static class HelperFunctions
{
    public static EwavColumnDataType ConflateEIWSFieldType(EIWSFieldType ft)
    {
        switch (ft)
        {
            case EIWSFieldType.Textbox:
                return EwavColumnDataType.Text;
            case EIWSFieldType.LabelTitle:
                return EwavColumnDataType.Ignore;
            case EIWSFieldType.Label:
                return EwavColumnDataType.Ignore;
            case EIWSFieldType.MultiLineTextBox:
                return EwavColumnDataType.Text;
            case EIWSFieldType.NumericTextBox:
                return EwavColumnDataType.Numeric;
            case EIWSFieldType.DatePicker:
                return EwavColumnDataType.DateTime;
            case EIWSFieldType.TimePicker:
                return EwavColumnDataType.DateTime;
            case EIWSFieldType.CheckBox:
                return EwavColumnDataType.Boolean;
            case EIWSFieldType.DropDownYesNo:
                return EwavColumnDataType.Text;
            case EIWSFieldType.RadioList:
                return EwavColumnDataType.Text;
            case EIWSFieldType.DropDownLegalValues:
                return EwavColumnDataType.Ignore;
            case EIWSFieldType.DropDownCodes:
                return EwavColumnDataType.Text;
            case EIWSFieldType.DropDownCommentLegal:
                return EwavColumnDataType.Ignore;
            case EIWSFieldType.GroupBox:
                return EwavColumnDataType.Ignore;
            default:
                throw new Exception(string.Format("Unknown type {0}", ft.ToString()));
        }
    }
}