using Microsoft.Phone.Scheduler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZdezClientForWP.Service
{
    class ScheduledAgentService
    {
        private const string PERIODIC_TASK_NAME = "ZdezClientForWPScheduledPeriodicAgent";
        private const string PERIODIC_TASK_DESCRIBE = "此后台任务用于“找得着校园通知客户端”维护通知功能的稳定性，请同学们不要关闭。此外，在桌面添加“找得着”磁贴可以更好的接收校园通知！";

        private static ScheduledAgentService _instance;
        public static ScheduledAgentService Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ScheduledAgentService();
                }
                return _instance;
            }
        }


        protected ScheduledAgentService()
        {
        }

        private DateTime? lastCheckTime;

        public bool AutoCheck(bool isOnForce = false)
        {
            if (isOnForce || lastCheckTime == null || lastCheckTime.Value.AddHours(1) < DateTime.Now)
            {
                lastCheckTime = DateTime.Now;
                string s;
                return CheckOpen(out s);
            }
            else
            {
                return true;
            }
        }

        protected bool CheckOpen(out string message)
        {
            var task = ScheduledActionService.Find(PERIODIC_TASK_NAME) as PeriodicTask;
            if (task != null)
            {
                ScheduledActionService.Remove(PERIODIC_TASK_NAME);
            }
            task = new PeriodicTask(PERIODIC_TASK_NAME);
            task.Description = PERIODIC_TASK_DESCRIBE;
            try
            {
                ScheduledActionService.Add(task);
            }
            catch (InvalidOperationException e)
            {
                // 存在同名任务、用户禁止后台代理、超出类型的最大代理数6
                message = e.Message;
                return false;
            }
            catch (SchedulerServiceException e)
            {
                // 系统内部错误
                message = e.Message;
                return false;
            }
            if (System.Diagnostics.Debugger.IsAttached)
            {
                ScheduledActionService.LaunchForTest(task.Name, System.TimeSpan.FromSeconds(1));
            }
            message = "";
            return true;
        }

        protected void SetClose()
        {
            var task = ScheduledActionService.Find(PERIODIC_TASK_NAME) as PeriodicTask;
            if (task != null)
            {
                ScheduledActionService.Remove(PERIODIC_TASK_NAME);
            }
        }
    }
}