using System;

namespace YCPU.Library.ParallelTasks
{
    class DelegateWork
            : IWork
    {
        static Pool<DelegateWork> instances = new Pool<DelegateWork>();

        public Action Action { get; set; }
        public WorkOptions Options { get; set; }

        public DelegateWork()
        {
        }

        public void DoWork()
        {
            Action();
            instances.Return(this);
        }

        internal static DelegateWork GetInstance()
        {
            return instances.Get();
        }
    }
}
