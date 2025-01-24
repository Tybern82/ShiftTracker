using System;
using System.Collections.Generic;
using System.Text;

namespace com.tybern.CallRecordCore.commands {
    /// <summary>
    /// Clear the current contents of the <c>UIProperties.Notes</c> field.
    /// </summary>
    public class ClearNotes : CMDProcessor.Command {

        protected static NLog.Logger LOG = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Create command to clear the <c>UIProperties.Notes</c> field
        /// </summary>
        public ClearNotes() {}

        /// <summary>
        /// Clear the <c>UIProperties.Notes</c> field to empty string
        /// </summary>
        public void Process() {
            LOG.Info("CLEAR NOTES");
            CallRecordCore.Instance.UIProperties.Notes = "";      // set the field to empty value
        }
    }
}
