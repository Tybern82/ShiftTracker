using System;
using System.Collections.Generic;
using System.Text;

namespace com.tybern.CMDProcessor {

    /// <summary>
    /// Interface implemented by individual commands to be processed by the CommandProcessor worker thread(s)
    /// </summary>
    public interface Command {

        /// <summary>
        /// Enumeration to identify where to run the requested command
        /// </summary>
        public enum RunThread {
            /// <summary>
            /// Run this command on the UI Thread. Used for commands that require access to UI/GUI elements that are locked
            /// to a single thread. 
            /// </summary>
            UIThread, 

            /// <summary>
            /// Run this command on the regular background Thread. 
            /// </summary>
            Background
        }

        /// <summary>
        /// Process this command.
        /// This method is implemented by each command to perform the body of the operation requested. 
        /// </summary>
        public void Process();

        /// <summary>
        /// Identify which thread to use to run this command
        /// </summary>
        public RunThread Type { get; }
    }
}
