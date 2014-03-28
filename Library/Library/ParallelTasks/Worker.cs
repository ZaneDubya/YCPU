using System;
using System.Threading;

namespace YCPU.Library.ParallelTasks
{
    class Worker
    {
        Thread thread;
        Deque<Task> tasks;
        WorkStealingScheduler scheduler;

        public bool LookingForWork { get; private set; }
        public AutoResetEvent Gate { get; private set; }

#if WINDOWS_PHONE
        // Cannot access Environment.ProcessorCount in phone app. (Security issue).
        static Hashtable<Thread, Worker> workers = new Hashtable<Thread, Worker>(1);
#else
        static Hashtable<Thread, Worker> workers = new Hashtable<Thread, Worker>(Environment.ProcessorCount);
#endif
        public static Worker CurrentWorker
        {
            get
            {
                var currentThread = Thread.CurrentThread;
                Worker worker;
                if (workers.TryGet(currentThread, out worker))
                    return worker;
                return null;
            }
        }

#if XBOX
        static int affinityIndex;
#endif

        public Worker(WorkStealingScheduler scheduler, int index)
        {
            this.thread = new Thread(Work);
            this.thread.Name = "YCPU.Library.ParallelTasks Worker " + index;
            this.thread.IsBackground = true;
            this.tasks = new Deque<Task>();
            this.scheduler = scheduler;
            this.Gate = new AutoResetEvent(false);

            workers.Add(thread, this);
        }

        public void Start()
        {
            thread.Start();
        }

        public void AddWork(Task task)
        {
            tasks.LocalPush(task);
        }

        private void Work()
        {
#if XBOX
            int i = Interlocked.Increment(ref affinityIndex) - 1;
            int affinity = Parallel.ProcessorAffinity[i % Parallel.ProcessorAffinity.Length];
            Thread.CurrentThread.SetProcessorAffinity((int)affinity);
#endif

            Task task = new Task();
            while (true)
            {
                if (tasks.LocalPop(ref task))
                {
                    task.DoWork();
                }
                else
                    FindWork();
            }
        }

        private void FindWork()
        {
            LookingForWork = true;
            Task task;
            bool foundWork = false;
            do
            {
                if (scheduler.TryGetTask(out task))
                {
                    foundWork = true;
                    break;
                }

                var replicable = WorkItem.Replicable;
                if (replicable.HasValue)
                {
                    replicable.Value.DoWork();
                    WorkItem.SetReplicableNull(replicable);

                    // MartinG@DigitalRune: Continue checking local queue and replicables. 
                    // No need to steal work yet.
                    continue;
                }

                for (int i = 0; i < scheduler.Workers.Count; i++)
                {
                    var worker = scheduler.Workers[i];
                    if (worker == this)
                        continue;

                    if (worker.tasks.TrySteal(ref task))
                    {
                        foundWork = true;
                        break;
                    }
                }

                if (!foundWork)
                {
                    // Wait until a new task gets scheduled.
                    Gate.WaitOne();
                }
            } while (!foundWork);

            LookingForWork = false;
            tasks.LocalPush(task);
        }
    }
}
