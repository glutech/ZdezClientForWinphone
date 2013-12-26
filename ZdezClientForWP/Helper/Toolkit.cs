using Microsoft.Devices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace ZdezClientForWP.Helper
{
    internal static class Toolkit
    {
        public static T GetChildUIElement<T>(DependencyObject parent, int indexOffset = 0) where T : UIElement
        {
            var findCount = 0;
            for (int i = 0, l = VisualTreeHelper.GetChildrenCount(parent); i < l; i++)
            {
                var c = VisualTreeHelper.GetChild(parent, i);
                if (c is T)
                {
                    if (findCount == indexOffset)
                    {
                        return (T)c;
                    }
                    else
                    {
                        findCount++;
                    }
                }
            }
            return null;
        }

        public static string ConvertToUnicode(string strSource)
        {
            /*
            var sourceEncoding = Encoding.UTF8;
            var destEncoding = Encoding.Unicode;
            var source = sourceEncoding.GetBytes(strSource);
            var dest = Encoding.Convert(sourceEncoding, destEncoding, source);
            return destEncoding.GetString(dest, 0, dest.Length);
             * */

            /*
            using (var reader = new System.IO.StreamReader(new System.IO.MemoryStream(Encoding.UTF8.GetBytes(strSource))))
            {
                return reader.ReadToEnd();
            }
             */
            using (var ms = new System.IO.MemoryStream(Encoding.UTF8.GetBytes(strSource)))
            {
                using (var reader = new System.IO.StreamReader(ms, Encoding.Unicode))
                {
                    return reader.ReadToEnd();
                }
            }
            /*
            System.IO.MemoryStream mstream = new System.IO.MemoryStream(Encoding.UTF8.GetBytes(strSource));
            System.IO.StreamReader reader = new System.IO.StreamReader(mstream, Encoding.Unicode);
            string strResult = reader.ReadToEnd();
            reader.Close();
            mstream.Dispose();
            reader.Dispose();
            return strResult;
            */
        }
        /*
         * 
         * WP7 无法使用 怨念
         * 
        /// <summary>
        /// 将object所拥有的公开属性用反射的方法包装为字典<br />
        /// 可用于用匿名类的方式传递字典
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static IDictionary<string, object> ToDict(this object obj)
        {
            //TODO: 支持数组化的对象，类似javascript的对象
            var properties = obj.GetType().GetProperties();
            var dict = new Dictionary<string, object>(properties.Length);
            foreach (var property in properties)
            {
                dict.Add(property.Name, property.GetValue(obj, null));
            }
            return dict;
        }
        */

        public class DictWrapper : Dictionary<string, object>
        {

            public DictWrapper()
                : base()
            { }

            public DictWrapper OR(string key, object value)
            {
                base.Add(key, value);
                return this;
            }
        }
        public static IEnumerable<TargetType> Map<SourceType, TargetType>(this IEnumerable<SourceType> source, Func<SourceType, TargetType> coverter)
        {
            foreach (var tmp in source)
            {
                yield return coverter(tmp);
            }
        }

        public static bool IsEmpty<T>(this IEnumerable<T> source)
        {
            foreach (var t in source)
            {
                return false;
            }
            return true;
        }

        public static void PlayNoticeSound(string path = "Assets/ListeningEarcon.wav")
        {
            var sound = SoundEffect.FromStream(TitleContainer.OpenStream(path));
            FrameworkDispatcher.Update();
            sound.Play();
        }

        public static void Vibrate(int seconds = 1)
        {
            VibrateController.Default.Start(new TimeSpan(0, 0, seconds));
        }

        public static void ToastMessage(string message, bool isThemeStyle = true)
        {
            if (isThemeStyle)
            {
                new Coding4Fun.Toolkit.Controls.ToastPrompt()
                {
                    Message = message,
                    Foreground = (Brush)App.Current.Resources["PhoneAccentBrush"],
                    Background = (Brush)App.Current.Resources["PhoneForegroundBrush"],
                    FontSize = 24
                }.Show();
            }
            else
            {
                new Coding4Fun.Toolkit.Controls.ToastPrompt()
                {
                    Message = message,
                    Foreground = (Brush)App.Current.Resources["PhoneForegroundBrush"],
                    Background = (Brush)App.Current.Resources["PhoneAccentBrush"],
                    FontSize = 24
                }.Show();
            }
        }
    }
}