using System;
using System.Collections.Concurrent;
using System.Threading;

namespace com.tybern.CommandProcessor {

    /// <summary>
    /// Core component of the CommandProcessor. This class should be instantiated to create a worker thread / thread pool for 
    /// processing application requests on an independent thread(s). Note that this currently creates foreground threads to 
    /// process commands and application should call Terminate() or enqueue a CTerminate instance to actually terminate these
    /// when exiting the application. This has been implemented this way to ensure any queued processing will be completed 
    /// prior to closing the application. (ie any currently queued requests will complete processing prior to processing the
    /// CTerminate to close the worker threads)
    /// </summary>
    public class CommandProcessor {

        private readonly BlockingCollection<Command> _commands = new BlockingCollection<Command>(new ConcurrentQueue<Command>());

        private bool _terminated = false;
        /// <summary>
        /// Identifies whether worker threads should terminate or wait for a new request. Locked as multiple worker threads may 
        /// be attempting to access, and public to allow CTerminate to trigger termination.
        /// </summary>
        public bool Terminated { 
            // Lock as we may be running multiple worker threads all accessing this value
            private get { lock(this) return _terminated; } 
            set { lock(this) _terminated = value; }
        }

        /// <summary>
        /// Enqueue a new command for processing, this should be picked up by the next available worker thread to process.
        /// </summary>
        /// <param name="cmd">Command to process in queue</param>
        public void Enqueue(Command cmd) => _commands.Add(cmd);

        /// <summary>
        /// Trigger termination of all worker threads for this <c>CommandProcessor</c>. This is the normal, expected means of terminating
        /// the threads, however callers can choose to manually inject the <c>CTerminate</c> instance into the queue using the normal
        /// processing and never call this method. 
        /// </summary>
        public void Terminate() => Enqueue(new CTerminate(this));

        /// <summary>
        /// Initiate new CommandProcessor instance. This will also start the worker threads for processing commands.
        /// </summary>
        /// <remarks>
        /// Default <c>CommandProcessor</c> is a single-threaded worker. Primary expected operation for this class is to act as a central
        /// 'message processing' core to separate the application logic from the UI/GUI interface. Applications can also use this class
        /// to implement worker pools to process multiple independent requests (ie handling remote requests for a server) where multiple
        /// concurrent processing tasks would be advantageous. Applications can then instantiate this class with the number of threads 
        /// they would like in the pool, and inject all the requests to a common thread to be processed by the next available worker thread.
        /// </remarks>
        /// <param name="threads">Number of threads to run; default : 1 (single-threaded); minimum : 1 (will enforce minimum of single-thread)</param>
        /// <param name="makeBackground">Specify whether the worker threads should be set as Background threads; default : false (workers are foreground threads)</param>
        public CommandProcessor(uint threads = 1, bool makeBackground = false) {
            if (threads < 1) threads = 1;                                       // ensure at least 1 thread
            for (int i = 0; i < threads; i++) startThread(makeBackground);      // start requested number of worker threads (default 1)
        }

        private void startThread(bool makeBackground) {
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
            if (makeBackground) t.IsBackground = true;
            t.Start();
        }
    }
}
