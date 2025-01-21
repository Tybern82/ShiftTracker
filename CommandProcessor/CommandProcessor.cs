using System;
using System.Collections.Concurrent;
using System.Threading;

namespace CommandProcessor {
    public class CommandProcessor {

        private readonly BlockingCollection<Command> _commands = new BlockingCollection<Command>(new ConcurrentQueue<Command>());

        private bool _terminated = false;
        public bool Terminated { 
            // Lock as we may be running multiple worker threads all accessing this value
            private get { lock(this) return _terminated; } 
            set { lock(this) _terminated = value; }
        }

        public void enqueue(Command cmd) => _commands.Add(cmd);
        public void terminate() => enqueue(new CTerminate(this));

        public CommandProcessor(uint threads = 1) {
            if (threads < 1) threads = 1;                       // ensure at least 1 thread
            for (int i = 0; i < threads; i++) startThread();    // start requested number of worker threads (default 1)
        }

        private void startThread() {
            Thread t = new Thread(() => {
                while (!Terminated) {
                    Command cmd = _commands.Take();     // NOTE: Blocking method - will wait for new command to proceed
                    if (cmd is CTerminate) {
                        Terminated = true;      // ensure any other threads identify termination
                        break;                  // auto-close this thread
                    } else {
                        cmd.Process();
                    }
                }
            });
            t.Start();
        }
    }
}
