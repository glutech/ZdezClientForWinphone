using Microsoft.Phone.Shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZdezClientForWP.Service
{
    class ShellTileService
    {

        private static ShellTileService _instance;
        public static ShellTileService Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ShellTileService();
                }
                return _instance;
            }
        }

        protected ShellTileService()
        {
        }

        public void UpdateOnSystemNotification(string notifyTitle)
        {
            int count;
            int id;
            string title;
            count = LocalService.GetTopNoticeForShellTile(out id, out title);
            UpdateShellTile(count + 1, notifyTitle);
        }

        public void UpdateOnPullRefreash()
        {
            int count;
            int id;
            string title;
            count = LocalService.GetTopNoticeForShellTile(out id, out title);
            if (count != 0)
            {
                UpdateShellTile(count, title);
                FlushSettings(count, title);
            }
            else
            {
                UpdateShellTile();
                FlushSettings();
            }
        }

        public void UpdateOnReadStatusChanged()
        {
            UpdateOnPullRefreash();
        }

        private void FlushSettings(int count = 0, string title = "")
        {
            Helper.AppSettingHelper.AddOrUpdateValue("ShellTileLastCount", count);
            Helper.AppSettingHelper.AddOrUpdateValue("ShellTileLastTitle", title);
        }

        private static void UpdateShellTile()
        {
            ShellTile.ActiveTiles.First().Update(new StandardTileData()
                {
                    Title = "找得着",
                    BackgroundImage = new Uri("Background.png", UriKind.Relative)
                });
        }

        private static void UpdateShellTile(int count, string title)
        {
            ShellTile.ActiveTiles.First().Update(new StandardTileData()
                {
                    Title = "新通知",
                    BackgroundImage = new Uri("Background.png", UriKind.Relative),
                    Count = count > 99 ? 99 : count,
                    BackContent = title,
                    BackTitle = "找得着校园通知系统"
                });
        }

    }
}