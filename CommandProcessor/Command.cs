using System;
using System.Collections.Generic;
using System.Text;

namespace com.tybern.CommandProcessor {

    /// <summary>
    /// Interface implemented by individual commands to be processed by the CommandProcessor worker thread(s)
    /// </summary>
    public interface Command {

        /// <summary>
        /// Process this command.
        /// This method is implemented by each command to perform the body of the operation requested. 
        /// </summary>
        public void Process();
    }
}
