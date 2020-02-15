using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using Windows.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Media.Animation;
using Windows.Media.Playback;
using Windows.Media.Core;
using Windows.UI.ViewManagement;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace Soul_Detective
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    /// 
    public sealed partial class MainPage : Page
    {
        private void ShowControl(UIElement subControl, double timeSpan, EventHandler<object> complete = null)
        {
            subControl.Visibility = Visibility.Visible;
            DoubleAnimation doubleAnimation = new DoubleAnimation();
            doubleAnimation.From = 0.0d;
            doubleAnimation.To = 1.0d;
            doubleAnimation.Duration = TimeSpan.FromSeconds(timeSpan);
            doubleAnimation.FillBehavior = FillBehavior.HoldEnd;

            Storyboard myStoryboard = new Storyboard();
            myStoryboard.Children.Add(doubleAnimation);
            Storyboard.SetTarget(doubleAnimation, subControl);
            Storyboard.SetTargetProperty(doubleAnimation, "(Opacity)");
            if (complete != null) myStoryboard.Completed += complete;
            myStoryboard.Begin();
        }
        private void HideControl(UIElement subControl, double timeSpan, EventHandler<object> complete = null)
        {
            DoubleAnimation doubleAnimation = new DoubleAnimation();
            doubleAnimation.From = 1.0d;
            doubleAnimation.To = 0.0d;
            doubleAnimation.Duration = TimeSpan.FromSeconds(timeSpan);
            doubleAnimation.FillBehavior = FillBehavior.HoldEnd;

            Storyboard myStoryboard = new Storyboard();
            myStoryboard.Children.Add(doubleAnimation);
            Storyboard.SetTarget(doubleAnimation, subControl);
            Storyboard.SetTargetProperty(doubleAnimation, "(Opacity)");
            myStoryboard.Completed += delegate (object a, object b) { subControl.Visibility = Visibility.Collapsed; };
            if (complete != null) myStoryboard.Completed += complete;
            myStoryboard.Begin();
        }
        public MainPage()
        {
            this.InitializeComponent();
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
            ApplicationView.PreferredLaunchViewSize = new Size(1280, 720);
            Grid_menu.Visibility = Visibility.Collapsed;
        }
        public MediaPlayer player = new MediaPlayer();
        bool triggering = false;
        private void ButtonDemo_Click(object sender, RoutedEventArgs e)
        {
            if (triggering)
                return;
            triggering=  true;

         //  HideControl(Grid_menu,0.5,delegate(object a,object b) {

               this.Frame.Navigate(typeof(GamePage), null, new EntranceNavigationTransitionInfo());

               player.Dispose();
               triggering = false;
         //   });
           
        }
        private void Grid_menu_Loaded(object sender, RoutedEventArgs e)
        {
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
            ApplicationView.PreferredLaunchViewSize = new Size(1280, 720);

            ShowControl(Grid_menu,2);
            player.IsLoopingEnabled = true;
            player.Source = MediaSource.CreateFromUri(new Uri("ms-appx:///Assets/BGM/10.心底的颜色 Ver. Orange.wav"));
            player.Play();

            //Grid_menu.f
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //  App.Current.Exit();
           
        }
    }
}
