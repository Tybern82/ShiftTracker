using System;
using System.Collections.Generic;
using System.Text;
using com.tybern.CMDProcessor;

namespace com.tybern.CallRecordCore.commands {

    /// <summary>
    /// Command to append text to the existing value in the <c>UIProperties.Notes</c> field.
    /// </summary>
    public class AppendNote : Command {

        protected static NLog.Logger LOG = NLog.LogManager.GetCurrentClassLogger();

        private readonly UIProperties properties;
        private readonly string note;

        /// <summary>
        /// Create command used to append the given text into the <c>UIProperties.Notes</c> field.
        /// </summary>
        /// <param name="note">text to add to the <c>Notes</c> field</param>
        public AppendNote(string note) {
            this.properties = CallRecordCore.Instance.UIProperties;
            this.note = note;
        }

        /// <summary>
        /// Append the text to the existing value in the <c>UIProperties.Notes</c> field.
        /// </summary>
        public void Process() {
            LOG.Info("APPEND: " + note);
            string currValue = properties.Notes;
            // append the note to the existing value; separate with newline if there is already data in the field
            properties.Notes = (string.IsNullOrWhiteSpace(currValue)) ? note : currValue + "\n" + note;
        }
    }
}
