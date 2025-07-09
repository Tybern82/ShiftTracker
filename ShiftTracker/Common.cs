using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text;
using com.tybern.ShiftTracker.data;

namespace com.tybern.ShiftTracker {

    public delegate void DataUpdate();
    public delegate void CommandEvent();

    public class Utility {
        public static void appendNote(NoteStore ns, string note) {
            string sep = (string.IsNullOrEmpty(ns.NoteContent)) ? string.Empty : "\n";
            ns.NoteContent += sep + note;
        }

        public static void prependNote(NoteStore ns, string note) {
            string sep = (string.IsNullOrEmpty(ns.NoteContent)) ? string.Empty : "\n";
            ns.NoteContent = note + sep + ns.NoteContent;
        }
    }

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

        public static string GetEnumTooltip(Enum enumObj) {
            FieldInfo? fieldInfo = enumObj.GetType().GetField(enumObj.ToString());
            object[] attribArray = fieldInfo?.GetCustomAttributes(false) ?? new object[0];
            if (attribArray.Length == 0)
                return GetEnumDescription(enumObj);
            else {
                TooltipDescriptionAttribute? attrib = null;
                foreach (var att in attribArray) {
                    if (att is TooltipDescriptionAttribute)
                        attrib = att as TooltipDescriptionAttribute;
                }

                if (attrib != null) return attrib.TooltipDescription;
                return GetEnumDescription(enumObj);
            }
        }
    }

    public class TooltipDescriptionAttribute: Attribute {

        public string TooltipDescription { get; }

        public TooltipDescriptionAttribute(string tooltipDescription) {
            this.TooltipDescription = tooltipDescription;
        }
    }
}
