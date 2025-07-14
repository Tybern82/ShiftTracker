using System;
using System.Collections.Generic;
using System.Text;

namespace com.tybern.CMDProcessor {
    public abstract class BackgroundCommand : Command {
        public Command.RunThread Type { get; } = Command.RunThread.Background;

        public abstract void Process();
    }
}
