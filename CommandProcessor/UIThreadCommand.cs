using System;
using System.Collections.Generic;
using System.Text;

namespace com.tybern.CMDProcessor {
    public abstract class UIThreadCommand : Command {
        public Command.RunThread Type { get; } = Command.RunThread.UIThread;

        public abstract void Process();
    }
}
