using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.IsolatedStorage;

using ZdezClientForWP.Helper;

namespace ZdezClientForWP.Service
{
    class AuthService
    {

        private static AuthService instance;

        public static AuthService Instance
        {
            get
            {
                if (instance == null)
                {

                    instance = new AuthService();
                }
                return instance;
            }
        }


        public bool IsAuth
        {
            get { return AppSettingHelper.GetValueOrDefault<bool>(IS_AUTH_KEY, false); }
        }

        public int AuthId
        {
            get
            {
                if (IsAuth)
                {
                    return AppSettingHelper.GetValueOrDefault<int>(AUTH_ID_KEY, -1);
                }
                else
                {
                    throw new InvalidOperationException("not login");
                }
            }
        }

        public string AuthUsername
        {
            get
            {
                if (IsAuth)
                {
                    return AppSettingHelper.GetValueOrDefault<string>(AUTH_USERNAME_KEY, "");
                }
                else
                {
                    throw new InvalidOperationException("not login");
                }
            }
        }

        const string IS_AUTH_KEY = "IS_AUTH";
        const string AUTH_ID_KEY = "AUTH_ID";
        const string AUTH_USERNAME_KEY = "AUTH_USERNAME";
        const string AUTH_NAME_KEY = "AUTH_NAME";
        const string AUTH_GENDER_KEY = "AUTH_GENDER";
        const string AUTH_GRADE_KEY = "AUTH_GRADE";
        const string AUTH_MAJOR_KEY = "AUTH_MAJOR";
        const string AUTH_DEPARTMENT_KEY = "AUTH_DEPARTMENT";

        public AuthUserStruct GetUserStruct()
        {
            if (IsAuth)
            {
                return new AuthUserStruct(
                    AppSettingHelper.GetValueOrDefault<int>(AUTH_ID_KEY, 0),
                    AppSettingHelper.GetValueOrDefault<string>(AUTH_USERNAME_KEY, ""),
                    AppSettingHelper.GetValueOrDefault<string>(AUTH_NAME_KEY, ""),
                    AppSettingHelper.GetValueOrDefault<string>(AUTH_GENDER_KEY, ""),
                    AppSettingHelper.GetValueOrDefault<string>(AUTH_GRADE_KEY, ""),
                    AppSettingHelper.GetValueOrDefault<string>(AUTH_MAJOR_KEY, ""),
                    AppSettingHelper.GetValueOrDefault<string>(AUTH_DEPARTMENT_KEY, "")
                );
            }
            else
            {
                throw new Exception();
            }
        }

        public void SetLogin(AuthUserStruct user)
        {
            AppSettingHelper.AddOrUpdateValue(IS_AUTH_KEY, true);
            AppSettingHelper.AddOrUpdateValue(AUTH_ID_KEY, user.Id);
            AppSettingHelper.AddOrUpdateValue(AUTH_USERNAME_KEY, user.Username);
            AppSettingHelper.AddOrUpdateValue(AUTH_NAME_KEY, user.Name);
            AppSettingHelper.AddOrUpdateValue(AUTH_GENDER_KEY, user.Gender);
            AppSettingHelper.AddOrUpdateValue(AUTH_GRADE_KEY, user.Grade);
            AppSettingHelper.AddOrUpdateValue(AUTH_MAJOR_KEY, user.Major);
            AppSettingHelper.AddOrUpdateValue(AUTH_DEPARTMENT_KEY, user.Department);
            AppSettingHelper.Save();
            NotificationService.Instance.Toggle.AuthAllowable = true;
        }

        public void SetLogout()
        {
            AppSettingHelper.AddOrUpdateValue(IS_AUTH_KEY, false);
            AppSettingHelper.Save();
            NotificationService.Instance.Toggle.AuthAllowable = false;
        }

    }

    // WindowsPhone7 不支持匿名反射和内部类反射
    // 故做成public实体类
    public class AuthUserStruct
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Name { get; set; }
        public string Gender { get; set; }
        public string Grade { get; set; }
        public string Major { get; set; }
        public string Department { get; set; }
        public AuthUserStruct() { }
        public AuthUserStruct(int id, string username, string name, string gender, string grade, string major, string department)
        {
            this.Id = id;
            this.Username = username;
            this.Name = name;
            this.Gender = gender;
            this.Grade = grade;
            this.Major = major;
            this.Department = department;
        }
    }
}