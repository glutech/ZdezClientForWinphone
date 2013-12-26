using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.IsolatedStorage;
using ZdezClientForWP.Helper;

namespace ZdezClientForWP.Service
{
    public class SettingService
    {

        private static SettingService _instance;
        public static SettingService Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new SettingService();
                }
                return _instance;
            }
        }

        const string PullSwitchKey = "PullSwitch";
        const string PullDetailKey = "PullDetail";
        const string PullSoundKey = "PullSound";
        const string PullVibrateKey = "PullVibrate";
        const string UEPlanKey = "UEPlan";
        const string FeedbackKey = "Feedback";
        const string AboutAppKey = "AboutApp";
        const string AboutUsKey = "AboutUs";

        const bool PullSwitchDefault = true;
        const bool PullDetailDefault = true;
        const bool PullSoundDefault = true;
        const bool PullVibrateDefault = true;
        const bool UEPlanDefault = false;

        public const string HOST_NAME = "http://www.zdez.com.cn:9080";
        public const string HOST = HOST_NAME + "/zdezServer/";

        public bool PullSwitch
        {
            get { return AppSettingHelper.GetValueOrDefault<bool>(PullSwitchKey, PullSwitchDefault); }
            set { if (AppSettingHelper.AddOrUpdateValue(PullSwitchKey, value)) AppSettingHelper.Save(); NotificationService.Instance.Toggle.DeviceAllowable = PullSwitch; }
        }

        public bool PullSound
        {
            get { return AppSettingHelper.GetValueOrDefault<bool>(PullSoundKey, PullSoundDefault); }
            set { if (AppSettingHelper.AddOrUpdateValue(PullSoundKey, value)) AppSettingHelper.Save(); }
        }

        public bool PullVibrate
        {
            get { return AppSettingHelper.GetValueOrDefault<bool>(PullVibrateKey, PullVibrateDefault); }
            set { if (AppSettingHelper.AddOrUpdateValue(PullVibrateKey, value)) AppSettingHelper.Save(); }
        }

        public bool UEPlan
        {
            get { return AppSettingHelper.GetValueOrDefault<bool>(UEPlanKey, UEPlanDefault); }
            set { if (AppSettingHelper.AddOrUpdateValue(UEPlanKey, value)) AppSettingHelper.Save(); }
        }

    }
}