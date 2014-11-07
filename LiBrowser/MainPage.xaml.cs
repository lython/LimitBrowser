using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Threading;
using System.Windows.Controls.Primitives;
using Coding4Fun.Phone.Controls;
using Microsoft.Phone.Tasks;
using System.Windows.Media.Imaging;
using System.IO.IsolatedStorage;

namespace LiBrowser
{
    public partial class MainPage : PhoneApplicationPage
    {
        DispatcherTimer timer;
        Popup errorPop = new Popup();
        TasotControl tasotDlg = new TasotControl();
        private string myurl;

        PhotoChooserTask photoChooser = new PhotoChooserTask();

        // 构造函数
        public MainPage()
        {
            InitializeComponent();
            errorPop.Child = tasotDlg;

            photoChooser.Completed += new EventHandler<PhotoResult>(OnPhotoChooserCompleted);
        }

        // 将 siteTextBox 的值传递到 WebView 页面中的 site
        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            var targetPage = e.Content as Views.WebView;
            if (targetPage != null)
            {
                targetPage.site = myurl; //site 的参数值  
            }
        }

        // 计时器，用来设定 popup 的弹窗提示在2秒后自动隐藏
        public void Timers()
        {
            timer = new DispatcherTimer();
            timer.Tick += new EventHandler(timer_Tick);
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Start();
        }

        void timer_Tick(object sender, EventArgs e)
        {
            errorPop.IsOpen = false;
            timer.Stop();
        }

        private void GoButton_Click(object sender, RoutedEventArgs e)
        {
            webBrowse();
        }

        // siteTextBox 中输入网址，若按下 Enter 键则打开网页
        private void SiteTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                webBrowse();
        }

        // 跳转到 WebView Page 方法
        public void webBrowse()
        {
            // 判断用户输入是否正确，给出提示
            myurl = SiteTextBox.Text.ToString();
            if (String.IsNullOrEmpty(myurl))
            {
                errorPop.IsOpen = true;
                Timers();
                return;
            }
            if (myurl.Equals("about:blank"))
            {
                errorPop.IsOpen = true;
                Timers();
                return;
            }
            if (!myurl.StartsWith("http://") &&
                !myurl.StartsWith("https://"))
            {
                myurl = "http://" + myurl;
            }
            try
            {
                NavigationService.Navigate(new Uri("/Views/WebView.xaml", UriKind.Relative));
            }
            catch (System.UriFormatException)
            {
                errorPop.IsOpen = true;
                Timers();
                return;
            }
        }

        private void TileLink_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            string url = (sender as Tile).Name;
            switch (url)
            {
                case "Tile2":
                    myurl = "http://3g.163.com/";
                    break;
                case "Tile3":
                    myurl = "http://www.baidu.com/";
                    break;
                case "Tile4":
                    myurl = "http://3g.renren.com/home";
                    break;
                case "Tile5":
                    myurl = "http://3g.sina.com/";
                    break;
                case "Tile6":
                    myurl = "http://3g.qq.com/";
                    break;
                case "Tile7":
                    myurl = "http://3g.ifeng.com/";
                    break;
                case "Tile8":
                    myurl = "http://www.wpxap.com/";
                    break;
                case "Tile9":
                    myurl = "http://3g.mop.com/";
                    break;
                case "Tile10":
                    myurl = "http://xuan.3g.cn/";
                    break;
                case "Tile11":
                    myurl = "http://pt.3g.qq.com/s?aid=nLoginqz&KqqWap_Act=3";
                    break;
                case "Tile12":
                    myurl = "http://m.taobao.com/";
                    break;
                case "Tile13":
                    myurl = "http://wap.10086.cn/index.jsp";
                    break;
                case "Tile14":
                    myurl = "http://www.google.com.hk/m";
                    break;
                case "Tile15":
                    myurl = "http://app.wpxap.com/";
                    break;
                case "Tile16":
                    myurl = "https://mobile.cmbchina.com";
                    break;
                case "Tile17":
                    myurl = "http://m.hao123.com/";
                    break;
            }
            NavigationService.Navigate(new Uri("/Views/WebView.xaml", UriKind.Relative));
        }

        private void skinTile_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            photoChooser.PixelWidth = 1024;
            photoChooser.PixelHeight = 768;
            photoChooser.ShowCamera = true;
            photoChooser.Show();
            //NavigationService.Navigate(new Uri("/LoadingPage.xaml", UriKind.Relative));
        }

        void OnPhotoChooserCompleted(object sender, PhotoResult e)
        {
            if (e.Error == null && e.ChosenPhoto != null)
            {
                ImageBrush img = new ImageBrush();
                WriteableBitmap bmp = Microsoft.Phone.PictureDecoder.DecodeJpeg(e.ChosenPhoto);
                IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication();
                string filename = e.OriginalFileName.Substring(e.OriginalFileName.LastIndexOf('\\') + 1);
                if (isf.FileExists(filename)) //如果已经存在这个文件，则将这个文件删除
                {
                    isf.DeleteFile(filename);
                }
                IsolatedStorageFileStream PhotoStream = isf.CreateFile(filename);
                Extensions.SaveJpeg(bmp, PhotoStream, bmp.PixelWidth, bmp.PixelHeight, 0, 85); //这里设置保存后图片的大小
                PhotoStream.Close();    //写入完毕，关闭文件流

                img.ImageSource = bmp;
                this.myPage.Background = img;
                if (isf.FileExists(Config.BackImg)) isf.DeleteFile(Config.BackImg); //删除上一个皮肤
                Config.BackImg = filename;
                Config.IsBackground = true;
            }
        }

        #region 墓碑化
        protected override void OnNavigatedTo(NavigationEventArgs args)
        {
            //激活墓碑化程序
            //换皮肤
            try
            {
                ImageBrush img = new ImageBrush();
                BitmapImage bmp = new BitmapImage();
                if (Config.IsBackground == true)
                {
                    using (IsolatedStorageFile myIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication())
                    {
                        using (IsolatedStorageFileStream fileStream = myIsolatedStorage.OpenFile(Config.BackImg, FileMode.Open, FileAccess.Read))
                        {
                            bmp.SetSource(fileStream);
                            img.ImageSource = bmp;
                        }
                    }
                }
                else
                {
                    Uri uri = new Uri("PanoramaBackground.png", UriKind.Relative);
                    bmp.UriSource = uri;
                    img.ImageSource = bmp;
                }
                this.myPage.Background = img;
            }
            catch { }
            base.OnNavigatedTo(args);
        }
        #endregion

        private void btnRight_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            ;
        }
    }
}