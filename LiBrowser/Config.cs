using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using Microsoft.Phone.Info;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;

namespace LiBrowser
{
    public class Config
    {
        //自定义背景
        public static bool IsBackground
        {
            get
            {
                return IsolatedStorageSettings.ApplicationSettings.Contains("IsBackground") ?
                       (bool)IsolatedStorageSettings.ApplicationSettings["IsBackground"] : true;
            }
            set
            {
                IsolatedStorageSettings.ApplicationSettings["IsBackground"] = value;
                IsolatedStorageSettings.ApplicationSettings.Save();
            }
        }

        //背景图
        public static string BackImg
        {
            get
            {
                return IsolatedStorageSettings.ApplicationSettings.Contains("BackImg") ?
                       (string)IsolatedStorageSettings.ApplicationSettings["BackImg"] : null;
            }
            set
            {
                IsolatedStorageSettings.ApplicationSettings["BackImg"] = value;
                IsolatedStorageSettings.ApplicationSettings.Save();
            }
        }

        //是否全屏
        public static bool IsFullScreen
        {
            get
            {
                return IsolatedStorageSettings.ApplicationSettings.Contains("IsFullScreen") ?
                       (bool)IsolatedStorageSettings.ApplicationSettings["IsFullScreen"] : false;
            }
            set
            {
                IsolatedStorageSettings.ApplicationSettings["IsFullScreen"] = value;
                IsolatedStorageSettings.ApplicationSettings.Save();
            }
        }
    }
}
