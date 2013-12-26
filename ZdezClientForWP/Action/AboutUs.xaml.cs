using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace ZdezClientForWP.Action
{
    public partial class AboutUsPage : PhoneApplicationPage
    {
        public AboutUsPage()
        {
            InitializeComponent();

            SupportTb.Text =
                "\tTel:\t\t0871-63323283\n"
              + "\tEmail:\tservice@zdez.com.cn";
            ZdezTb.Text =
                "\t\t我们（www.zdez.cn）打造人尽其用、人尽其才、人事相宜的资源环境，满足用人单位与人才的社会发展需求。\n"
              + "\t\t找得着通过手机客户端向学生推荐人力资源政策法规信息、高质量的用人单位招聘信息、校园兼职信息、认证培训信息、资格考试信息、就业心理和职业规划辅导信息；助力求职者择业成功。";
            BokeTb.Text =
                "\t\t昆明博客科技有限公司成立于2006年，是致力于手机终端商务平台技术研发和应用方案的提供商，是省内领先的客户端软件服务提供商。博客科技致力于为企业提供基于3G技术的短信互动、高效管理和个性化服务整体解决方案。\n"
              + "\t\t公司拥有员工22名，其中营销团队人员11名，包括MBA硕士及EMBA硕士各1名，营销专业本科毕业营销人员5名，普通市场营销人员4名；技术团队人员11名，包括海归博士1名，信息系统及软件工程硕士5名，高级程序员及程序员5名。";
        }
    }
}