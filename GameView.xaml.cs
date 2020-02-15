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
using Windows.Graphics.Display;
using System.Text.RegularExpressions;
using System.Reflection;
using System.ComponentModel;
using Windows.Storage.Streams;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace Soul_Detective
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class GamePage : Page
    {
        // static private bool backgroundMode = true; //标志当前的背景是媒体或是静态的图片,`媒体状态会禁用背景音乐,并且无法在媒体和图片之间应用过渡动画

        public RoutedEventHandler FirstEvent { get; set; }
        public void pauseMedia(object a, RoutedEventArgs b)
        {
            BkMedia.Pause();
        }
        public void firstEventTotal(object a, RoutedEventArgs b)
        {
            enablejump = false;
            thispage = datastream.getSignalPage(0);
            _Text.Text = thispage.text;
            _Name.Text = thispage.name;
            BkImage.Source = new BitmapImage(new Uri(thispage.BkSoure));
            m_BGkMusic.Source = MediaSource.CreateFromUri(new Uri(thispage.music));//设置BGM资源路径
            ShowControl(BkImage, 2.5, delegate (object x, object y)
              {
                  EnableNext = true;
                  m_BGkMusic.Play(); 
                  BkMedia.Visibility = Visibility.Collapsed;
                  ChangeAllContentOfTextbar(1, false);
                  n_tick++;
              });
        }
        static public int n_tick = 0;//当前的帖数
 
        private Datastream datastream = new Datastream();
       private  Datastream.PageInfo thispage = new Datastream.PageInfo();
        private bool EnableNext = false;//标志任意位置点击鼠标进入下一贴是否可用
        public List<Button> UiButtons = new List<Button>();//记录UI系统的所有按钮
        public MediaPlayer m_BGkMusic = new MediaPlayer();//初始化BGM播放器
        public void ChangeAllContentOfTextbar//隐藏文本框,以及所属的所有UI
            (
            double time,//应用过渡动画的时间
            bool a, //布尔值,true时隐藏,false时显示
            EventHandler<object> completed = null//事件,将在动画完成时触发
            )
        {
            if (a)
            {
                foreach (Button inSet in UiButtons)
                {
                    HideControl(inSet, time);
                }
                HideControl(_Name, time);
                HideControl(_Text, time);
                HideControl(TextBar, time, completed);
            }
            else
            {
                foreach (Button inSet in UiButtons)
                {
                    ShowControl(inSet, time);
                }
                ShowControl(_Name, time);
                ShowControl(_Text, time);
                ShowControl(TextBar, time, completed);
            }
        }
        public GamePage()
        {
         
            this.InitializeComponent();
        }
        public void ChangeImageSlowly(//在两个Image背景之间进行转换
            BitmapImage img,//转换后的图片
            double tick,//动画时间
            Image target,
            EventHandler<object> complete = null//完成后触发的动画
           
            )
        {
            DoubleAnimation doubleAnimation = new DoubleAnimation
            {
                From = target.Opacity,
                To = 0.0d,
                BeginTime = TimeSpan.FromSeconds(0),
                Duration = TimeSpan.FromSeconds(tick/2),
                FillBehavior = FillBehavior.HoldEnd
            };

            Storyboard myStoryboard = new Storyboard();
            myStoryboard.Children.Add(doubleAnimation);
            Storyboard.SetTarget(doubleAnimation, target);
            Storyboard.SetTargetProperty(doubleAnimation, "(Opacity)");
            myStoryboard.Completed += delegate (object a,object b)
            {
                target.Source = img;
                DoubleAnimation resdoubleAnimation = new DoubleAnimation
                {
                    From = 0.0d,
                    To = 1.0d,
                    BeginTime = TimeSpan.FromSeconds(0),
                    Duration = TimeSpan.FromSeconds(tick / 2),
                    FillBehavior = FillBehavior.HoldEnd
                };
                Storyboard resStoryboard = new Storyboard();
                resStoryboard.Children.Add(resdoubleAnimation);
                Storyboard.SetTarget(resdoubleAnimation, target);
                Storyboard.SetTargetProperty(resdoubleAnimation, "(Opacity)");
                if (complete != null) resStoryboard.Completed += complete;
                resStoryboard.Begin();
            };
         //   if (complete != null) myStoryboard.Completed += complete;
            myStoryboard.Begin(); 

        }
        public void ShowControl(//显示一个控件
            UIElement subControl, //目标控件
            double timeSpan, //动画时间
            EventHandler<object> complete = null//完成后触发的事件
            )
        {
            subControl.Visibility = Visibility.Visible;
            DoubleAnimation doubleAnimation = new DoubleAnimation
            {
                From = 0.0d,
                To = 1.0d,
                Duration = TimeSpan.FromSeconds(timeSpan),
                FillBehavior = FillBehavior.HoldEnd
            };

            Storyboard myStoryboard = new Storyboard();
            myStoryboard.Children.Add(doubleAnimation);
            Storyboard.SetTarget(doubleAnimation, subControl);
            Storyboard.SetTargetProperty(doubleAnimation, "(Opacity)");
            if (complete != null) myStoryboard.Completed += complete;
            myStoryboard.Begin();
        }
        public void HideControl(//隐藏控件
            UIElement subControl, //目标控件
            double timeSpan, //动画时间
            EventHandler<object> complete = null//完成后触发的事件
            )
        {
            DoubleAnimation doubleAnimation = new DoubleAnimation
            {
                From = 1.0d,
                To = 0.0d,
                Duration = TimeSpan.FromSeconds(timeSpan),
                FillBehavior = FillBehavior.HoldEnd
            };

            Storyboard myStoryboard = new Storyboard();
            myStoryboard.Children.Add(doubleAnimation);
            Storyboard.SetTarget(doubleAnimation, subControl);
            Storyboard.SetTargetProperty(doubleAnimation, "(Opacity)");
            myStoryboard.Completed += delegate (object a, object b) { subControl.Visibility = Visibility.Collapsed; };
            if (complete != null) myStoryboard.Completed += complete;
            myStoryboard.Begin();
        }

        public void SetTextWithStepping(//以渐变的方式改变文本框类的文字
            TextBlock target,//目标的文本框
            string text,//更改后的文字
            double time//动画时间
            )
        {
            DoubleAnimation hidden = new DoubleAnimation
            {
                From = 1.0d,
                To = 0.0d,
                Duration = TimeSpan.FromSeconds(time / 2),
                FillBehavior = FillBehavior.HoldEnd
            };

            DoubleAnimation showler = new DoubleAnimation
            {
                From = 0.0d,
                To = 1.0d,
                Duration = TimeSpan.FromSeconds(time / 2),
                FillBehavior = FillBehavior.HoldEnd
            };

            Storyboard myStoryboard = new Storyboard();
            myStoryboard.Children.Add(hidden);
            Storyboard.SetTarget(hidden, target);
            Storyboard.SetTargetProperty(hidden, "(Opacity)");
            myStoryboard.Completed += delegate (object a, object b) 
            {
                target.Text = text;
                Storyboard showlerBoard = new Storyboard();
                showlerBoard.Children.Add(showler);
                Storyboard.SetTarget(showler, target);
                Storyboard.SetTargetProperty(showler, "(Opacity)");
                showlerBoard.Begin();
            };
            myStoryboard.Begin();
        }

        private void GameGrid_Loaded(object sender, RoutedEventArgs e)
        {
            BkImage.Loaded += delegate (object a, RoutedEventArgs b) { System.Diagnostics.Debug.WriteLine("LOADED"); };
            m_BGkMusic.IsLoopingEnabled = true;
            m_BGkMusic.Volume = 0.35;
            UiButtons.Add(_Load);//添加按钮控件到列表
            UiButtons.Add(_Save);//
            UiButtons.Add(_ToTitle);//

            BkMedia.PlaybackRate = 1.0d;
            BkMedia.Visibility = Visibility.Visible;//因为第一幕是媒体,所以把Image初始化为不可见
            BkImage.Visibility = Visibility.Collapsed;

          // RandomAccessStreamOverStream video_01= new RandomAccessStreamOverStream(0,0);
            BkMedia.Source = new Uri("ms-appx:///Assets/VIDEO/video_01.wmv");   //设置第一幕的视频
           // BkMedia.SetSource(video_01,"");
            TextBar.Visibility = Visibility.Collapsed;//因为视频全屏播放,把文本框以及所属控件全部设置为不可见
            _Name.Visibility = Visibility.Collapsed;//
            _Text.Visibility = Visibility.Collapsed;//

            BkMedia.IsLooping = false;//媒体不循环播放
            FirstEvent = firstEventTotal;
            BkMedia.MediaEnded += FirstEvent; //媒体(第一幕)结束后进行的操作


           foreach (Button inSet in UiButtons)
            {
                inSet.Visibility = Visibility.Collapsed;
            }
            BkMedia.Play();


            
        }
        bool enablejump = true;
        private void GamePage_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            MediaPlayer soundPlayer = new MediaPlayer();
            soundPlayer.IsLoopingEnabled = false;

            if ((!EnableNext)&& enablejump)
            {
                enablejump = false;
                BkMedia.Stop();

                thispage = datastream.getSignalPage(0);
                _Text.Text = thispage.text;
                _Name.Text = thispage.name;
                BkImage.Source = new BitmapImage(new Uri(thispage.BkSoure));
                m_BGkMusic.Source = MediaSource.CreateFromUri(new Uri(thispage.music));//设置BGM资源路径
                ShowControl(BkImage, 2.5, delegate (object x, object y)
                {
                    EnableNext = true;
                    m_BGkMusic.Play();
                    BkMedia.Visibility = Visibility.Collapsed;
                    ChangeAllContentOfTextbar(1, false);
                    n_tick++;
                });
                return;
            }         //  DoNextEvent();
            if (!EnableNext) return;
            thispage = datastream.getSignalPage(n_tick);
            switch (n_tick)
            {
                /*这些代码定义了data.xml中的资源应该怎样被导入*/
                case 1:
                     EnableNext = false;
                     ChangeAllContentOfTextbar(1.5,true,delegate(object a,object b)
                     {
                         ChangeImageSlowly(new BitmapImage(new Uri(thispage.BkSoure)), 1.8, BkImage,delegate(object aa, object bb)
                          {
                             _Text.Text = "";
                             ChangeAllContentOfTextbar(1.5, false, delegate (object ac, object bc)
                               {
                                   SetTextWithStepping(_Text,thispage.text, 0.5);
                                   n_tick++;
                                   EnableNext = true;
                               });
                         });
                     });
                    break;
                case 2:
                    SetTextWithStepping(_Text, thispage.text, 0.5);
                    n_tick++;
                    break;
                case 3:
                    EnableNext = false;
                    ChangeAllContentOfTextbar(1.5, true, delegate (object a, object b)
                    {
                        ChangeImageSlowly(new BitmapImage(new Uri(thispage.BkSoure)), 1.8, BkImage, delegate (object aa, object bb)
                        {
                            _Text.Text = "";
                            ChangeAllContentOfTextbar(1.5, false, delegate (object ac, object bc)
                            {
                                SetTextWithStepping(_Text, thispage.text, 0.5);
                                n_tick++;
                                EnableNext = true;
                            });
                        });
                    });
                    break;
                case 4:
                    SetTextWithStepping(_Text, thispage.text, 0.5);
                    n_tick++;
                    break;
                case 5:
                    EnableNext = false;
                    ChangeAllContentOfTextbar(1.5, true, delegate (object a, object b)
                    {
                        ChangeImageSlowly(new BitmapImage(new Uri(thispage.BkSoure)), 1.8, BkImage, delegate (object aa, object bb)
                        {
                            _Text.Text = "";
                            ChangeAllContentOfTextbar(1.5, false, delegate (object ac, object bc)
                            {
                                SetTextWithStepping(_Text, thispage.text, 0.5);
                                n_tick++;
                                EnableNext = true;
                            });
                        });
                    });
                    break;
                case 6:
                    EnableNext = false;
                    BkImage.Visibility = Visibility.Collapsed;
 
                    BkMedia.Source = new Uri(thispage.BkSoure);
                    BkMedia.MediaEnded -= FirstEvent;
                    FirstEvent = pauseMedia;
                    BkMedia.MediaEnded += FirstEvent    ;
                    BkMedia.Play();
                    ShowControl(BkMedia, 2.5,delegate(object a,object b)
                    {
                        EnableNext = true;
                    });
                    SetTextWithStepping(_Text, thispage.text, 0.5);
                    n_tick++; 

                    break;
                case 7:
                    SetTextWithStepping(_Text, thispage.text, 0.5);
                    n_tick++;
                    break;
                case 8:
                    SetTextWithStepping(_Text, thispage.text, 0.5);
                    n_tick++;
                    break;
                case 9:
                    SetTextWithStepping(_Text, thispage.text, 0.5);
                    n_tick++;
                    break;
                case 10:
                    SetTextWithStepping(_Text, thispage.text, 0.5);
                    n_tick++;
                    break;
                case 11:
                    EnableNext = false;

                    ChangeAllContentOfTextbar(1,true,delegate(object a,object b) //首先隐藏文本框所有控件
                    {
                        HideControl(BkMedia, 1);//隐藏视频控件

                        BkImage.Source = new BitmapImage(new Uri(thispage.BkSoure));//更改图片控件的源
                        
                        ShowControl(BkImage, 1,delegate(object c,object d)
                        {
                            _Text.Text = thispage.text;
                            _Name.Text = thispage.name;

                            ChangeAllContentOfTextbar(1,false,delegate(object f,object g)
                            {
                                BkMedia.Stop();
                                EnableNext = true;
                                n_tick++;
                            });
                        });//显示图片控件
                    });
                   
                    break;
                case 12:
                    SetTextWithStepping(_Name, thispage.name, 0.5);
                    SetTextWithStepping(_Text, thispage.text, 0.5);
                    n_tick++;
                    break;
                case 13:
                    SetTextWithStepping(_Name, thispage.name, 0.5);
                    SetTextWithStepping(_Text, thispage.text, 0.5);
                    n_tick++;
                    break;
                case 14:
                    SetTextWithStepping(_Name, thispage.name, 0.5);
                    SetTextWithStepping(_Text, thispage.text, 0.5);
                    n_tick++;
                    break;
                case 15:
                    SetTextWithStepping(_Name, thispage.name, 0.5);
                    SetTextWithStepping(_Text, thispage.text, 0.5);
                    n_tick++;
                    break;
                case 16:
                    SetTextWithStepping(_Name, thispage.name, 0.5);
                    SetTextWithStepping(_Text, thispage.text, 0.5);
                    n_tick++;
                    break;
                case 17:
                    SetTextWithStepping(_Name, thispage.name, 0.5);
                    SetTextWithStepping(_Text, thispage.text, 0.5);
                    n_tick++;
                    break;
                case 18:
                    SetTextWithStepping(_Name, thispage.name, 0.5);
                    SetTextWithStepping(_Text, thispage.text, 0.5);
                    n_tick++;
                    break;
                case 19:
                    SetTextWithStepping(_Name, thispage.name, 0.5);
                    SetTextWithStepping(_Text, thispage.text, 0.5);
                    n_tick++;
                    break;
                case 20:
                    EnableNext = false;
                    ChangeAllContentOfTextbar(1.5, true, delegate (object a, object b)
                    {
                        ChangeImageSlowly(new BitmapImage(new Uri(thispage.BkSoure)), 1.8, BkImage, delegate (object aa, object bb)
                        {
                            _Text.Text = "";
                            ChangeAllContentOfTextbar(1.5, false, delegate (object ac, object bc)
                            {
                                SetTextWithStepping(_Text, thispage.text, 0.5);
                                n_tick++;
                                EnableNext = true;
                            });
                        });
                    });
                    break;
                case 21:
                    SetTextWithStepping(_Name, thispage.name, 0.5);
                    SetTextWithStepping(_Text, thispage.text, 0.5);
                    n_tick++;
                    break;
                case 22:
                    SetTextWithStepping(_Name, thispage.name, 0.5);
                    SetTextWithStepping(_Text, thispage.text, 0.5);
                    n_tick++;
                    break;
                case 23:
                    SetTextWithStepping(_Name, thispage.name, 0.5);
                    SetTextWithStepping(_Text, thispage.text, 0.5);
                    n_tick++;
                    break;
                case 24:
                    SetTextWithStepping(_Name, thispage.name, 0.5);
                    SetTextWithStepping(_Text, thispage.text, 0.5);
                    n_tick++;
                    break;
                case 25:
                    SetTextWithStepping(_Name, thispage.name, 0.5);
                    SetTextWithStepping(_Text, thispage.text, 0.5);
                    n_tick++;
                    break;
                case 26:
                    SetTextWithStepping(_Name, thispage.name, 0.5);
                    SetTextWithStepping(_Text, thispage.text, 0.5);
                    soundPlayer.Source = MediaSource.CreateFromUri(new Uri("ms-appx:///Assets/sound/mxj_00001.wav"));
                    soundPlayer.Play();
                    n_tick++;
                    break;
                case 27:
                    EnableNext = false;
                    BkImage.Visibility = Visibility.Collapsed;
                    m_BGkMusic.Source = MediaSource.CreateFromUri(new Uri(thispage.music));
                    m_BGkMusic.Play();
                    BkMedia.Source = new Uri(thispage.BkSoure);
                    BkMedia.Play();
                    ShowControl(BkMedia, 1, delegate (object a, object b)
                    {
                       
                        EnableNext = true;
                    });
                    SetTextWithStepping(_Text, thispage.text, 0.5);
                    n_tick++;
                    break;
                case 28:
                    SetTextWithStepping(_Name, thispage.name, 0.5);
                    SetTextWithStepping(_Text, thispage.text, 0.5);
                    soundPlayer.Source = MediaSource.CreateFromUri(new Uri("ms-appx:///Assets/sound/mxj_00002.wav"));
                    soundPlayer.Play();
                    n_tick++;
                    break;
                case 29:
                    SetTextWithStepping(_Name, thispage.name, 0.5);
                    SetTextWithStepping(_Text, thispage.text, 0.5);
                    n_tick++;
                    break;
                case 30:
                    SetTextWithStepping(_Name, thispage.name, 0.5);
                    SetTextWithStepping(_Text, thispage.text, 0.5);
                    soundPlayer.Source = MediaSource.CreateFromUri(new Uri("ms-appx:///Assets/sound/mxj_00003.wav"));
                    soundPlayer.Play();
                    n_tick++;
                    break;
                case 31:
                    SetTextWithStepping(_Name, thispage.name, 0.5);
                    SetTextWithStepping(_Text, thispage.text, 0.5);
                    soundPlayer.Source = MediaSource.CreateFromUri(new Uri("ms-appx:///Assets/sound/mxj_00004.wav"));
                    soundPlayer.Play();
                    n_tick++;
                    break;
                case 32:
                    SetTextWithStepping(_Name, thispage.name, 0.5);
                    SetTextWithStepping(_Text, thispage.text, 0.5);
                    n_tick++;
                    break;
                case 33:
                    SetTextWithStepping(_Name, thispage.name, 0.5);
                    SetTextWithStepping(_Text, thispage.text, 0.5);
                    soundPlayer.Source = MediaSource.CreateFromUri(new Uri("ms-appx:///Assets/sound/mxj_00005.wav"));
                    soundPlayer.Play();
                    n_tick++;
                    break;
                case 34:
                    SetTextWithStepping(_Name, thispage.name, 0.5);
                    SetTextWithStepping(_Text, thispage.text, 0.5);
                    soundPlayer.Source = MediaSource.CreateFromUri(new Uri("ms-appx:///Assets/sound/mxj_00006.wav"));
                    soundPlayer.Play();
                    n_tick++;
                    break;
                case 35:
                    SetTextWithStepping(_Name, thispage.name, 0.5);
                    SetTextWithStepping(_Text, thispage.text, 0.5);
                    n_tick++;
                    break;
                case 36:
                    SetTextWithStepping(_Name, thispage.name, 0.5);
                    SetTextWithStepping(_Text, thispage.text, 0.5);
                    n_tick++;
                    break;
                case 37:
                    SetTextWithStepping(_Name, thispage.name, 0.5);
                    SetTextWithStepping(_Text, thispage.text, 0.5);
                    n_tick++;
                    break;
                case 38:
                    SetTextWithStepping(_Name, thispage.name, 0.5);
                    SetTextWithStepping(_Text, thispage.text, 0.5);
                    n_tick++;
                    break;
                case 39:
                    SetTextWithStepping(_Name, thispage.name, 0.5);
                    SetTextWithStepping(_Text, thispage.text, 0.5);
                    n_tick++;
                    break;
                case 40:
                    EnableNext = false;
                    HideControl(BkMedia, 1,delegate(object a,object b)
                    {
                        BkMedia.Stop();
                        BkMedia.Source = new Uri(thispage.BkSoure);
                        BkMedia.Play();
                        ShowControl(BkMedia, 1, delegate (object c, object d)
                        {
                            soundPlayer.Source = MediaSource.CreateFromUri(new Uri("ms-appx:///Assets/sound/mxj_00007.wav"));
                            soundPlayer.Play();
                            SetTextWithStepping(_Name,thispage.name,1);
                            SetTextWithStepping(_Text, thispage.text, 0.5);
                            n_tick++;
                            EnableNext = true;
                        });
                    });
                    break;
                case 41:
                    SetTextWithStepping(_Name, thispage.name, 0.5);
                    SetTextWithStepping(_Text, thispage.text, 0.5);
                    soundPlayer.Source = MediaSource.CreateFromUri(new Uri("ms-appx:///Assets/sound/mxj_00008.wav"));
                    soundPlayer.Play();
                    n_tick++;
                    break;
                case 42:
                    SetTextWithStepping(_Name, thispage.name, 0.5);
                    SetTextWithStepping(_Text, thispage.text, 0.5);
                    n_tick++;
                    break;
                case 43:
                    SetTextWithStepping(_Name, thispage.name, 0.5);
                    SetTextWithStepping(_Text, thispage.text, 0.5);
                    n_tick++;
                    break;
                case 44:
                    SetTextWithStepping(_Name, thispage.name, 0.5);
                    SetTextWithStepping(_Text, thispage.text, 0.5);
                    soundPlayer.Source = MediaSource.CreateFromUri(new Uri("ms-appx:///Assets/sound/mxj_00009.wav"));
                    soundPlayer.Play();
                    n_tick++;
                    break;
                case 45:
                    SetTextWithStepping(_Name, thispage.name, 0.5);
                    SetTextWithStepping(_Text, thispage.text, 0.5);
                    soundPlayer.Source = MediaSource.CreateFromUri(new Uri("ms-appx:///Assets/sound/mxj_00010.wav"));
                    soundPlayer.Play();
                    n_tick++;
                    break;
                case 46:
                    SetTextWithStepping(_Name, thispage.name, 0.5);
                    SetTextWithStepping(_Text, thispage.text, 0.5);
                    soundPlayer.Source = MediaSource.CreateFromUri(new Uri("ms-appx:///Assets/sound/mxj_00011.wav"));
                    soundPlayer.Play();
                    n_tick++;
                    break;
                case 47:
                    SetTextWithStepping(_Name, thispage.name, 0.5);
                    SetTextWithStepping(_Text, thispage.text, 0.5);
                    soundPlayer.Source = MediaSource.CreateFromUri(new Uri("ms-appx:///Assets/sound/mxj_00012.wav"));
                    soundPlayer.Play();
                    n_tick++;
                    break;
                case 48:
                    SetTextWithStepping(_Name, thispage.name, 0.5);
                    SetTextWithStepping(_Text, thispage.text, 0.5);
                    n_tick++;
                    break;
                case 49:
                    SetTextWithStepping(_Name, thispage.name, 0.5);
                    SetTextWithStepping(_Text, thispage.text, 0.5);
                    soundPlayer.Source = MediaSource.CreateFromUri(new Uri("ms-appx:///Assets/sound/mxj_00013.wav"));
                    soundPlayer.Play();
                    n_tick++;
                    break;
                case 50:
                    SetTextWithStepping(_Name, thispage.name, 0.5);
                    SetTextWithStepping(_Text, thispage.text, 0.5);
                    n_tick++;
                    break;
                case 51:
                    EnableNext = false;

                    ChangeAllContentOfTextbar(1, true, delegate (object a, object b) //首先隐藏文本框所有控件
                    {
                        HideControl(BkMedia, 1);//隐藏视频控件

                        BkImage.Source = new BitmapImage(new Uri(thispage.BkSoure));//更改图片控件的源

                        ShowControl(BkImage, 1, delegate (object c, object d)
                        {
                            _Text.Text = thispage.text;
                            _Name.Text = thispage.name;

                            ChangeAllContentOfTextbar(1, false, delegate (object f, object g)
                            {
                                BkMedia.Stop();
                                EnableNext = true;
                                n_tick++;
                            });
                        });//显示图片控件
                    });
                    break;
                case 52:
                    SetTextWithStepping(_Name, thispage.name, 0.5);
                    SetTextWithStepping(_Text, thispage.text, 0.5);
                    n_tick++;
                    break;
                case 53:
                    EnableNext = false;
                    BkImage.Visibility = Visibility.Collapsed;

                    BkMedia.Source = new Uri(thispage.BkSoure);
                    BkMedia.Play();
                    ShowControl(BkMedia, 1, delegate (object a, object b)
                    {
                        SetTextWithStepping(_Name, thispage.name, 0.5);
                        SetTextWithStepping(_Text, thispage.text, 0.5);
                        EnableNext = true;
                    });
                 
                    n_tick++;

                    break;
                case 54:
                    SetTextWithStepping(_Name, thispage.name, 0.5);
                    SetTextWithStepping(_Text, thispage.text, 0.5);
                    n_tick++;
                    break;
                case 55:
                    SetTextWithStepping(_Name, thispage.name, 0.5);
                    SetTextWithStepping(_Text, thispage.text, 0.5);
                    n_tick++;
                    break;
                case 56:
                    SetTextWithStepping(_Name, thispage.name, 0.5);
                    SetTextWithStepping(_Text, thispage.text, 0.5);
                    n_tick++;
                    break;
                case 57:
                    SetTextWithStepping(_Name, thispage.name, 0.5);
                    SetTextWithStepping(_Text, thispage.text, 0.5);
                    n_tick++;
                    break;
                case 58:
                    EnableNext = false;

                    ChangeAllContentOfTextbar(1, true, delegate (object a, object b) //首先隐藏文本框所有控件
                    {
                        HideControl(BkMedia, 1);//隐藏视频控件

                        BkImage.Source = new BitmapImage(new Uri(thispage.BkSoure));//更改图片控件的源

                        ShowControl(BkImage, 1, delegate (object c, object d)
                        {
                            _Text.Text = thispage.text;
                            _Name.Text = thispage.name;

                            ChangeAllContentOfTextbar(1, false, delegate (object f, object g)
                            {
                                BkMedia.Stop();
                                EnableNext = true;
                                n_tick++;
                                soundPlayer.Source = MediaSource.CreateFromUri(new Uri("ms-appx:///Assets/sound/mxj_00014.wav"));
                                soundPlayer.Play();
                            });
                        });//显示图片控件
                    });
                    break;
                case 59:
                    SetTextWithStepping(_Name, thispage.name, 0.5);
                    SetTextWithStepping(_Text, thispage.text, 0.5);
                    soundPlayer.Source = MediaSource.CreateFromUri(new Uri("ms-appx:///Assets/sound/mxj_00015.wav"));
                    soundPlayer.Play();
                    n_tick++;
                    break;
                case 60:
                    SetTextWithStepping(_Name, thispage.name, 0.5);
                    SetTextWithStepping(_Text, thispage.text, 0.5);
                    n_tick++;
                    break;
                case 61:
                    SetTextWithStepping(_Name, thispage.name, 0.5);
                    SetTextWithStepping(_Text, thispage.text, 0.5);
                    n_tick++;
                    break;
                case 62:
                    SetTextWithStepping(_Name, thispage.name, 0.5);
                    SetTextWithStepping(_Text, thispage.text, 0.5);
                    n_tick++;
                    break;
                case 63:
                    EnableNext = false;

                    ChangeAllContentOfTextbar(1, true, delegate (object a, object b) //首先隐藏文本框所有控件
                    {
                        HideControl(BkImage, 1);

                        BkImage.Source = new BitmapImage(new Uri(thispage.BkSoure));//更改图片控件的源

                        ShowControl(BkImage, 1, delegate (object c, object d)
                        {
                            _Text.Text = thispage.text;
                            _Name.Text = thispage.name;

                            ChangeAllContentOfTextbar(1, false, delegate (object f, object g)
                            {
                                soundPlayer.Source = MediaSource.CreateFromUri(new Uri("ms-appx:///Assets/sound/mxj_00015.wav"));
                                soundPlayer.Play();
                                BkMedia.Stop();
                                EnableNext = true;
                                n_tick++;
                            });
                        });//显示图片控件
                    });
                    break;
                case 64:
                    SetTextWithStepping(_Name, thispage.name, 0.5);
                    SetTextWithStepping(_Text, thispage.text, 0.5);
                    n_tick++;
                    break;
                case 65:
                    SetTextWithStepping(_Name, thispage.name, 0.5);
                    SetTextWithStepping(_Text, thispage.text, 0.5);
                    n_tick++;
                    break;
                case 66:
                    SetTextWithStepping(_Name, thispage.name, 0.5);
                    SetTextWithStepping(_Text, thispage.text, 0.5);
                    n_tick++;
                    break;
                case 67:
                    SetTextWithStepping(_Name, thispage.name, 0.5);
                    SetTextWithStepping(_Text, thispage.text, 0.5);
                    n_tick++;
                    break;
                case 68:
                    SetTextWithStepping(_Name, thispage.name, 0.5);
                    SetTextWithStepping(_Text, thispage.text, 0.5);
                    soundPlayer.Source = MediaSource.CreateFromUri(new Uri("ms-appx:///Assets/sound/mxj_00017.wav"));
                    soundPlayer.Play();
                    n_tick++;
                    break;
                case 69:
                    SetTextWithStepping(_Name, thispage.name, 0.5);
                    SetTextWithStepping(_Text, thispage.text, 0.5);
                    n_tick++;
                    break;
                case 70:
                    SetTextWithStepping(_Name, thispage.name, 0.5);
                    SetTextWithStepping(_Text, thispage.text, 0.5);
                    n_tick++;
                    break;
                case 71:
                    SetTextWithStepping(_Name, thispage.name, 0.5);
                    SetTextWithStepping(_Text, thispage.text, 0.5);
                    soundPlayer.Source = MediaSource.CreateFromUri(new Uri("ms-appx:///Assets/sound/mxj_00018.wav"));
                    soundPlayer.Play();
                    n_tick++;
                    break;
                case 72:
                    SetTextWithStepping(_Name, thispage.name, 0.5);
                    SetTextWithStepping(_Text, thispage.text, 0.5);
                    n_tick++;
                    break;
                case 73:
                    SetTextWithStepping(_Name, thispage.name, 0.5);
                    SetTextWithStepping(_Text, thispage.text, 0.5);
                    n_tick++;
                    break;
                case 74:
                    SetTextWithStepping(_Name, thispage.name, 0.5);
                    SetTextWithStepping(_Text, thispage.text, 0.5);
                    n_tick++;
                    break;
                case 75:
                    SetTextWithStepping(_Name, thispage.name, 0.5);
                    SetTextWithStepping(_Text, thispage.text, 0.5);
                    soundPlayer.Source = MediaSource.CreateFromUri(new Uri("ms-appx:///Assets/sound/mxj_00019.wav"));
                    soundPlayer.Play();
                    n_tick++;
                    break;
                case 76:
                    SetTextWithStepping(_Name, thispage.name, 0.5);
                    SetTextWithStepping(_Text, thispage.text, 0.5);
                    n_tick++;
                    break;
                case 77:
                    SetTextWithStepping(_Name, thispage.name, 0.5);
                    SetTextWithStepping(_Text, thispage.text, 0.5);
                    n_tick++;
                    break;
                case 78:
                    SetTextWithStepping(_Name, thispage.name, 0.5);
                    SetTextWithStepping(_Text, thispage.text, 0.5);
                    soundPlayer.Source = MediaSource.CreateFromUri(new Uri("ms-appx:///Assets/sound/mxj_00020.wav"));
                    soundPlayer.Play();
                    n_tick++;
                    break;
                case 79:
                    SetTextWithStepping(_Name, thispage.name, 0.5);
                    SetTextWithStepping(_Text, thispage.text, 0.5);
                    n_tick++;
                    break;
                case 80:
                    EnableNext = false;
                    ChangeAllContentOfTextbar(1.5, true, delegate (object a, object b)
                    {
                        ChangeImageSlowly(new BitmapImage(new Uri(thispage.BkSoure)), 1.8, BkImage, delegate (object aa, object bb)
                        {
                            _Text.Text = "";
                            ChangeAllContentOfTextbar(1.5, false, delegate (object ac, object bc)
                            {
                                m_BGkMusic.Pause();
                                SetTextWithStepping(_Text, thispage.text, 0.5);
                                n_tick++;
                                EnableNext = true;
                            });
                        });
                    });
                    break;
                case 81:
                    SetTextWithStepping(_Name, thispage.name, 0.5);
                    SetTextWithStepping(_Text, thispage.text, 0.5);
                    n_tick++;
                    break;
                case 82:
                    SetTextWithStepping(_Name, thispage.name, 0.5);
                    SetTextWithStepping(_Text, thispage.text, 0.5);
                    soundPlayer.Source = MediaSource.CreateFromUri(new Uri("ms-appx:///Assets/sound/mxj_00021.wav"));
                    soundPlayer.Play();
                    n_tick++;
                    break;
                case 83:
                    SetTextWithStepping(_Name, thispage.name, 0.5);
                    SetTextWithStepping(_Text, thispage.text, 0.5);
                    soundPlayer.Source = MediaSource.CreateFromUri(new Uri("ms-appx:///Assets/sound/mxj_00022.wav"));
                    soundPlayer.Play();
                    n_tick++;
                    break;
                case 84:
                    SetTextWithStepping(_Name, thispage.name, 0.5);
                    SetTextWithStepping(_Text, thispage.text, 0.5);
                    soundPlayer.Source = MediaSource.CreateFromUri(new Uri("ms-appx:///Assets/sound/mxj_00023.wav"));
                    soundPlayer.Play();
                    n_tick++;
                    break;
                case 85:
                    EnableNext = false;
                    ChangeAllContentOfTextbar(1.5,true,delegate(object a,object b)
                    {
                        BkImage.Visibility = Visibility.Collapsed;
                        BkMedia.Source = new Uri("ms-appx:///Assets/VIDEO/video_05.wmv");
                        BkMedia.MediaEnded -= FirstEvent;
                        BkMedia.MediaEnded += ToTiitle_Click;
                        BkMedia.Play();
                        ShowControl(BkMedia,2);
                    });
                    break;
            }

                                                                          
        }
        bool triggering = false;//这个变量防止连续点击按钮引起错误
        private void ToTiitle_Click(object sender, RoutedEventArgs e)//这是点击了Title按钮后的处理代码
        {
            if (triggering)//如果已经点击过一次,直接返回不做处理
                return;

            triggering = true;

            this.Frame.Navigate(typeof(MainPage), null, new EntranceNavigationTransitionInfo());//切换为MainPage

            n_tick = 0;//把当前游戏进度设置为0,下一次点击Start从头开始
            m_BGkMusic.Dispose();//停止BGM播放
        }

        private void GameGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
           
        }
        public void NarrowVisual(object sender, VisualStateChangedEventArgs e)

        {
            /*这些代码使被废弃的,但没有删除(就是懒)*/
            double CurrentDisplayHeight = Window.Current.Bounds.Width;
            double CurrentDisplayWidth = Window.Current.Bounds.Height;


            //Window.Current.Bounds.Width

            //Window.Current.Bounds.Height

        }

        private void gamePage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            /*这些代码使控件的大小以及位置随窗口大小的改变而改变*/
            double CurrentDisplayWidth = Window.Current.Bounds.Width;//获取客户区大小(宽)
            double CurrentDisplayHeight = Window.Current.Bounds.Height;//获取客户区大小(长)

            BkImage.Width = CurrentDisplayWidth;//使背景图片的大小适配屏幕
            BkImage.Height = CurrentDisplayHeight;

            BkMedia.Width = CurrentDisplayWidth;//使媒体背景的大小适配屏幕
            BkMedia.Height = CurrentDisplayHeight;

            TextBar.Width = CurrentDisplayWidth;//调整文本框大小
            TextBar.Height = CurrentDisplayHeight / 2;
            TextBar.Margin = new Thickness(0, CurrentDisplayHeight / 2, 0, 0);

            gameGrid.Width = BkImage.Width;//调整线性布局的大小
            gameGrid.Height = BkImage.Height;
          //  gameGrid.SizeChanged += GameGrid_SizeChanged;

            _Name.Margin = new Thickness(CurrentDisplayWidth * 0.0726, CurrentDisplayHeight * 0.7861111, 0, 0);//调整各种文本框和按钮的位置,不调整大小
            _Text.Margin = new Thickness(CurrentDisplayWidth * 0.1179, CurrentDisplayHeight * 0.8486111, 0, 0);

            _Save.Margin = new Thickness(CurrentDisplayWidth * 0.5828125, CurrentDisplayHeight * 0.95138888, 0, 0);
            _Load.Margin = new Thickness(CurrentDisplayWidth * 0.68671875, CurrentDisplayHeight * 0.95138888, 0, 0);
            _ToTitle.Margin = new Thickness(CurrentDisplayWidth * 0.834375, CurrentDisplayHeight * 0.95138888, 0, 0);
        }

        private void _Load_Click(object sender, RoutedEventArgs e)
        {
            n_tick = 25;
        }

        private void _Save_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}

