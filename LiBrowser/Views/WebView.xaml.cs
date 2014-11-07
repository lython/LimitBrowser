using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using System.Text.RegularExpressions;
using System.Windows.Controls.Primitives;

namespace LiBrowser.Views
{
    public partial class WebView : PhoneApplicationPage
    {   // 建立一个浏览历史的list,实现浏览过程中的后退前进
        // 方法参看:
        /* http://stackoverflow.com/questions/7873632/adding-back-and-forward-button-for-webbrowser-control */
        // 这种方法只是简单的记住了浏览过的地址,后退前进都需要重新加载页面,保存浏览的页面比较复杂(乱码问题),以后慢慢研究
        // 通过 InvokeScript 也可以实现，但是在重新加载页面后页面自动放大了

        List<Uri> HistoryStack;
        int HistoryStack_Index;
        bool fromHistory;
        bool trayFlag = true;
        string current_uri;

        public WebView()
        {
            InitializeComponent();
            HistoryStack = new List<Uri>();
            HistoryStack_Index = 0;
            fromHistory = false;
            liWebBrowser.Navigated += new EventHandler<System.Windows.Navigation.NavigationEventArgs>(liWebBrowser_Navigated);
            liWebBrowser.Navigating += new EventHandler<NavigatingEventArgs>(liWebBrowser_Navigating);
            if (Config.IsFullScreen)
            {
                liWebBrowser.Height = 728;
                uriTextBox.Visibility = Visibility.Collapsed;
                (this.ApplicationBar.Buttons[2] as ApplicationBarIconButton).Text = "窗口";
                (this.ApplicationBar.Buttons[2] as ApplicationBarIconButton).IconUri = new Uri("Images/appbar.normal.png", UriKind.Relative);
            }
        }

        // 保存浏览的地址,堆栈的数据结构
        void liWebBrowser_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            if (!fromHistory)
            {
                if (HistoryStack_Index < HistoryStack.Count)
                {
                    HistoryStack.RemoveRange(HistoryStack_Index, HistoryStack.Count - HistoryStack_Index);
                }
                HistoryStack.Add(e.Uri);
                HistoryStack_Index += 1;
                if (HistoryStack_Index > 1)
                {
                    (this.ApplicationBar.Buttons[0] as ApplicationBarIconButton).IsEnabled = true;
                }
            }
            fromHistory = false;
        }

        // 接受自 MainPage 的网址
        public string site { get; set; }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            (this.ApplicationBar.Buttons[0] as ApplicationBarIconButton).IsEnabled = false;
            (this.ApplicationBar.Buttons[1] as ApplicationBarIconButton).IsEnabled = false;
            liWebBrowser.Navigate(new Uri(site, UriKind.Absolute));
        }

        //正在加载，进度条
        private void liWebBrowser_Navigating(object sender, NavigatingEventArgs e)
        {
            current_uri = e.Uri.ToString();
            uriTextBox.Text = current_uri;
            progressBar.Visibility = Visibility.Visible;
        }

        //加载完成后
        private void liWebBrowser_LoadCompleted(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            progressBar.Visibility = Visibility.Collapsed;
            try
            {
                uriTextBox.Text = liWebBrowser.InvokeScript("eval", "document.title").ToString();
            }
            catch (SystemException)
            {
                ;
            }
        }

        //加载失败后
        private void liWebBrowser_NavigationFailed(object sender, System.Windows.Navigation.NavigationFailedEventArgs e)
        {
            progressBar.Visibility = Visibility.Collapsed;
        }

        //后退
        private void backUri_Click(object sender, EventArgs e)
        {
            if (HistoryStack_Index > 1)
            {
                HistoryStack_Index -= 1;
                fromHistory = true;
                liWebBrowser.Navigate(HistoryStack[HistoryStack_Index - 1]);
                (this.ApplicationBar.Buttons[1] as ApplicationBarIconButton).IsEnabled = true;
            }
            if (HistoryStack_Index == 1)
            {
                (this.ApplicationBar.Buttons[0] as ApplicationBarIconButton).IsEnabled = false;
            }
        }

        // 前进
        private void forwardUri_Click(object sender, EventArgs e)
        {
            if (HistoryStack_Index < HistoryStack.Count)
            {
                HistoryStack_Index += 1;
                fromHistory = true;
                liWebBrowser.Navigate(HistoryStack[HistoryStack_Index - 1]);
                (this.ApplicationBar.Buttons[0] as ApplicationBarIconButton).IsEnabled = true;
            }
            if (HistoryStack_Index == HistoryStack.Count)
            {
                (this.ApplicationBar.Buttons[1] as ApplicationBarIconButton).IsEnabled = false;
            }
        }

        //拦截返回键
        private void Page_BackKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (HistoryStack_Index > 1)
            {
                backUri_Click(sender, e);
                e.Cancel = true;
                return;
            }
            else
            {
                this.NavigationService.GoBack();
            }
        }

        //返回主页
        private void homePage_Click(object sender, EventArgs e)
        {
            HistoryStack.Clear();
            HistoryStack_Index = 0;
            this.NavigationService.GoBack();
        }

        //手动创建异常让其退出
        private void exit_Click(object sender, EventArgs e)
        {
            HistoryStack.Clear();
            string aa = "Exception";
            Convert.ToInt16(aa);
        }

        //根据工具栏的状态变化来显示托盘
        private void ApplicationBar_StateChanged(object sender, ApplicationBarStateChangedEventArgs e)
        {
            if (trayFlag == true)
            {
                Microsoft.Phone.Shell.SystemTray.IsVisible = true;
                trayFlag = false;
            }
            else
            {
                Microsoft.Phone.Shell.SystemTray.IsVisible = false;
                trayFlag = true;
            }
        }

        //刷新
        private void refreshUri_Click(object sender, EventArgs e)
        {
            liWebBrowser.Navigate(new Uri(current_uri));
        }

        //弹出网址输入框
        private void newUri_Click(object sender, EventArgs e)
        {
            if (Config.IsFullScreen == true)
            {
                liWebBrowser.Height = 660;
                uriTextBox.Visibility = Visibility.Visible;
                Config.IsFullScreen = false;
                (this.ApplicationBar.Buttons[2] as ApplicationBarIconButton).Text = "全屏";
                (this.ApplicationBar.Buttons[2] as ApplicationBarIconButton).IconUri = new Uri("Images/appbar.fullscreen.png", UriKind.Relative);
            }
            else
            {
                liWebBrowser.Height = 728;
                uriTextBox.Visibility = Visibility.Collapsed;
                Config.IsFullScreen = true;
                (this.ApplicationBar.Buttons[2] as ApplicationBarIconButton).Text = "窗口";
                (this.ApplicationBar.Buttons[2] as ApplicationBarIconButton).IconUri = new Uri("/Images/appbar.normal.png", UriKind.Relative);
            }
        }

        //网址框回车
        private void uriTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                string myurl = uriTextBox.Text.ToString();
                if (String.IsNullOrEmpty(myurl)) return;
                if (myurl.Equals("about:blank")) return;
                if (!myurl.StartsWith("http://") &&
                    !myurl.StartsWith("https://"))
                {
                    myurl = "http://" + myurl;
                }
                try
                {
                    liWebBrowser.Navigate(new Uri(myurl));
                    liWebBrowser.Focus();
                }
                catch (System.UriFormatException)
                {
                    return;
                }
            }
        }

        //PhoneTextBox获得焦点
        private void uriTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            uriTextBox.Text = current_uri;
            uriTextBox.SelectAll();
        }

        //PhoneTextBox中的图片按钮
        private void uriTextBox_ActionIconTapped(object sender, EventArgs e)
        {
            uriTextBox.Text = "";
        }

        private void liWebBrowser_ScriptNotify(object sender, NotifyEventArgs e)
        {
            int height = Convert.ToInt32(e.Value);
            this.liWebBrowser.Height = height;
        }
    }
}