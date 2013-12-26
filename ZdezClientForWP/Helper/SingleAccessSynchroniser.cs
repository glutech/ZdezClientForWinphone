using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ZdezClientForWP.Helper
{
    /// <summary>
    /// 同步单线程访问资源
    /// http://stackoverflow.com/questions/11079721/multi-process-access-to-database-in-windows-phone
    /// </summary>
    public class SingleAccessSynchroniser2 : IDisposable
    {
        public bool hasHandle = false;
        Mutex mutex;

        private void InitMutex()
        {
            string mutexId = "Global\\SingleAccessSynchroniser";
            mutex = new Mutex(false, mutexId);
        }

        public SingleAccessSynchroniser2()
            : this(0)
        { }

        public SingleAccessSynchroniser2(int TimeOut)
        {
            InitMutex();

            if (TimeOut <= 0)
                hasHandle = mutex.WaitOne();
            else
                hasHandle = mutex.WaitOne(TimeOut);

            if (hasHandle == false)
                throw new TimeoutException("Timeout waiting for exclusive access on SingleInstance");
        }

        public void Release()
        {
            if (hasHandle && mutex != null)
            {
                mutex.ReleaseMutex();
                hasHandle = false;
            }
        }

        public void Dispose()
        {
            Release();
        }
    }
}
