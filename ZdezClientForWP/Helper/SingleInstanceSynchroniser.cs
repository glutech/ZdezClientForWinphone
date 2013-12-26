using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ZdezClientForWP.Helper
{
    /// <summary>
    /// 前台APP和后台代理的同步管理
    /// http://stackoverflow.com/questions/11079721/multi-process-access-to-database-in-windows-phone
    /// </summary>
    public class SingleInstanceSynchroniser : IDisposable
    {
        private bool hasHandle = false;
        Mutex mutex;

        private void InitMutex()
        {
            string mutexId = "Global\\ZdezClientForWP-APP-ScheduledTaskAgent-SingleInstanceSynchroniser";
            mutex = new Mutex(false, mutexId);
        }

        public SingleInstanceSynchroniser()
        {
            InitMutex();
            hasHandle = mutex.WaitOne(0);
        }

        public void Dispose()
        {
            if (hasHandle && mutex != null)
                mutex.ReleaseMutex();
        }

        public bool HasExclusiveHandle { get { return hasHandle; } }

    }
}
