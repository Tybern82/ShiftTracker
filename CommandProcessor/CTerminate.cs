using System;
using System.Collections.Generic;
using System.Text;

namespace com.tybern.CommandProcessor {

    /// <summary>
    /// Implementation of Command interface used to trigger termination of CommandProcessor worker thread(s). This class is 
    /// normally utilised directly by CommandProcessor, however is public to allow injecting termination request into command
    /// queue where direct access to the CommandProcessor is unavailable / undesirable (ie function can return an instance of this
    /// class to trigger a termination where only the calling function has access to CommandProcessor, and just injects whatever
    /// Command instance the internal function returns; caller can pass in a suitably configured CTerminate which the calling
    /// function returns, without providing direct access to the CommandProcessor instance).
    /// </summary>
    public class CTerminate : Command {

        private CommandProcessor _proc;

        /// <summary>
        /// Command used to terminate the worker thread(s) of an active CommandProcessor
        /// </summary>
        /// <param name="proc">CommandProcessor instance to terminate</param>
        public CTerminate(CommandProcessor proc) {
            this._proc = proc;
        }

        /// <summary>
        /// Process this command
        /// NOTE: This operation is normally not called as default CommandProcessor auto-detects the CTerminate
        /// command and processes internally.
        /// </summary>
        public void Process() {
            _proc.Terminated = true;
        }
    }
}
