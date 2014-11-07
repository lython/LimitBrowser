﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.Windows.Threading;

namespace LiBrowser
{
    public partial class LoadingPage : PhoneApplicationPage
    {
        private DispatcherTimer loadtimer = new DispatcherTimer(); //定时器

        public LoadingPage()
        {
            InitializeComponent();
            loadtimer.Interval = TimeSpan.FromSeconds(1);
            loadtimer.Tick += OnTimerLoop;
            storyboard.Begin();
            loadtimer.Start();
        }

        void OnTimerLoop(object sender, EventArgs e)
        {
            storyboard.Begin();
        }
    }
}