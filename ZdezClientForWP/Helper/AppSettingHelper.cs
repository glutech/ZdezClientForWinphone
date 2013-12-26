using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.IsolatedStorage;

namespace ZdezClientForWP.Helper
{
    static class AppSettingHelper
    {
        public static readonly IsolatedStorageSettings AppSettingIsoStoreage = IsolatedStorageSettings.ApplicationSettings;

        public static bool AddOrUpdateValue(string key, object value)
        {
            var valueChanged = false;
            if (AppSettingIsoStoreage.Contains(key))
            {
                if (AppSettingIsoStoreage[key] != value)
                {
                    AppSettingIsoStoreage[key] = value;
                    valueChanged = true;
                }
            }
            else
            {
                AppSettingIsoStoreage.Add(key, value);
                valueChanged = true;
            }
            return valueChanged;
        }

        public static T GetValueOrDefault<T>(string key, T defaultValue)
        {
            T value;
            if (AppSettingIsoStoreage.Contains(key))
            {
                value = (T)AppSettingIsoStoreage[key];
            }
            else
            {
                value = defaultValue;
                AppSettingIsoStoreage.Add(key, value);
                AppSettingIsoStoreage.Save();
            }
            return value;
        }

        public static void Save()
        {
            AppSettingIsoStoreage.Save();
        }
    }
}