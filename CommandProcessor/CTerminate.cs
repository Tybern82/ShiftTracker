using System;
using System.Collections.Generic;
using System.Text;

namespace CommandProcessor {
    internal class CTerminate : Command {

        private CommandProcessor _proc;

        public CTerminate(CommandProcessor proc) {
            this._proc = proc;
        }

        public void Process() {
            _proc.Terminated = true;
        }
    }
}
