using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text;

namespace com.tybern.ShiftTracker {

    public delegate void DataUpdate();

    public delegate void CommandEvent();

    public class EnumConverter {

        public static string GetEnumDescription(Enum enumObj) {
            FieldInfo? fieldInfo = enumObj.GetType().GetField(enumObj.ToString());
            object[] attribArray = fieldInfo?.GetCustomAttributes(false) ?? new object[0];

            if (attribArray.Length == 0)
                return enumObj.ToString();
            else {
                DescriptionAttribute? attrib = null;

                foreach (var att in attribArray) {
                    if (att is DescriptionAttribute)
                        attrib = att as DescriptionAttribute;
                }

                if (attrib != null)
                    return attrib.Description;

                return enumObj.ToString();
            }
        }
    }
}
