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
using Microsoft.Gaming.XboxGameBar;
using System.Diagnostics;

using Quobject.SocketIoClientDotNet.Client;
using Newtonsoft.Json;
using Windows.UI.Core;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Devices.Bluetooth;
using Windows.UI.ViewManagement;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace YouTubeMusicDesktopWidget
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Widget1 : Page
    {
        private XboxGameBarWidget widget = null;
        private XboxGameBarWidgetControl widgetControl = null;

        Socket socket = IO.Socket("http://127.0.0.1:9863");
        Models.PlayerInfo playerInfo = new Models.PlayerInfo();
        Models.TrackInfo trackInfo = new Models.TrackInfo();

        BitmapImage playPauseButtonImage = new BitmapImage();
        BitmapImage thumbUpButtonImage = new BitmapImage();
        BitmapImage thumbDownButtonImage = new BitmapImage();
        private string lastTrackId = "";
        private bool _isConnected = false;
        private bool _canSeek = false;

        public Widget1()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //widget = e.Parameter as XboxGameBarWidget;
            //widget.SettingsSupported = false;
            /*widgetControl = new XboxGameBarWidgetControl(widget);

            // Hook up events for when the ui is updated.
            widget.SettingsClicked += Widget_SettingsClicked;
            widget.VisibleChanged += Widget_VisibleChanged;
            widget.WindowStateChanged += Widget_WindowStateChanged;
            widget.GameBarDisplayModeChanged += Widget_GameBarDisplayModeChanged;

            OutputVisibleState();
            OutputWindowState();
            OutputGameBarDisplayMode();

            Size size = new Size(410, 130);
            widget.MinWindowSize = size;
            widget.MaxWindowSize = size;

            widget.SettingsSupported = false;
            widget.HorizontalResizeSupported = true;
            widget.VerticalResizeSupported = true;*/

            socket.On(Socket.EVENT_CONNECT, () =>
            {
                _isConnected = true;
            });

            socket.On(Socket.EVENT_DISCONNECT, () =>
            {
                _isConnected = false;
                resetValues();
            });

            socket.On("query", (data) =>
            {
                UpdateValues(data);
            });
        }

        private async void UpdateValues(dynamic data)
        {
            //System.Diagnostics.Debug.WriteLine(_isConnected);

            playerInfo.FromJson(data);
            trackInfo.FromJson(data);

            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (lastTrackId != trackInfo.Id)
                {
                    lastTrackId = trackInfo.Id;

                    if (trackInfo.Cover != "")
                    {
                        coverImage.Source = new BitmapImage(new Uri(trackInfo.Cover));
                    }
                }

                if (playerInfo.IsPaused == true)
                {
                    playPauseButtonImage.UriSource = new Uri("ms-appx:///Assets/Icons/ic_play_arrow_white_48dp.png");
                }
                else
                {
                    playPauseButtonImage.UriSource = new Uri("ms-appx:///Assets/Icons/ic_pause_white_48dp.png");
                }
                playPauseButtonBg.ImageSource = playPauseButtonImage;

                switch (playerInfo.LikeStatus)
                {
                    case "LIKE":
                        thumbUpButtonImage.UriSource = new Uri("ms-appx:///Assets/Icons/ic_thumb_up_white_36dp.png");
                        thumbDownButtonImage.UriSource = new Uri("ms-appx:///Assets/Icons/ic_thumb_down_outlined_white_36dp.png");
                        break;

                    case "DISLIKE":
                        thumbUpButtonImage.UriSource = new Uri("ms-appx:///Assets/Icons/ic_thumb_up_outlined_white_36dp.png");
                        thumbDownButtonImage.UriSource = new Uri("ms-appx:///Assets/Icons/ic_thumb_down_white_36dp.png");
                        break;

                    case "INDIFFERENT":
                        thumbUpButtonImage.UriSource = new Uri("ms-appx:///Assets/Icons/ic_thumb_up_outlined_white_36dp.png");
                        thumbDownButtonImage.UriSource = new Uri("ms-appx:///Assets/Icons/ic_thumb_down_outlined_white_36dp.png");
                        break;
                }

                trackTitle.Text = trackInfo.Title;
                trackAuthor.Text = trackInfo.Author;

                thumbUpButtonBg.ImageSource = thumbUpButtonImage;
                thumbDownButtonBg.ImageSource = thumbDownButtonImage;

                playerCurrentTime.Text = playerInfo.SeekbarCurrentPositionHuman;
                trackDurationTime.Text = trackInfo.DurationHuman;

                trackSeekTime.Maximum = trackInfo.Duration;
                trackSeekTime.Value = playerInfo.SeekbarCurrentPosition;
            }
            );
        }

        private async void resetValues()
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                coverImage.Source = new BitmapImage();

                trackTitle.Text = "-";
                trackAuthor.Text = "-";

                playerCurrentTime.Text = "0:00";
                trackSeekTime.Value = 0;
                trackDurationTime.Text = "0:00";

                playPauseButtonImage.UriSource = new Uri("ms-appx:///Assets/Icons/ic_play_arrow_white_48dp.png");
                playPauseButtonBg.ImageSource = playPauseButtonImage;
            }
            );
        }
        private void ThumbDownButton_Click(object sender, RoutedEventArgs e)
        {
            socket.Emit("media-commands", "track-thumbs-down");
        }

        private void PreviousButton_Click(object sender, RoutedEventArgs e)
        {
            socket.Emit("media-commands", "track-previous");
        }

        private void PlayPauseButton_Click(object sender, RoutedEventArgs e)
        {
            socket.Emit("media-commands", "track-play");
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            socket.Emit("media-commands", "track-next");
        }

        private void ThumbUpButton_Click(object sender, RoutedEventArgs e)
        {
            socket.Emit("media-commands", "track-thumbs-up");
        }

        private async void Widget_SettingsClicked(XboxGameBarWidget sender, object args)
        {
            await widget.ActivateSettingsAsync();
        }

        private void Widget_VisibleChanged(XboxGameBarWidget sender, object args)
        {
            OutputVisibleState();
        }

        private void Widget_WindowStateChanged(XboxGameBarWidget sender, object args)
        {
            OutputWindowState();
        }

        private void Widget_GameBarDisplayModeChanged(XboxGameBarWidget sender, object args)
        {
            OutputGameBarDisplayMode();
        }

        private void OutputVisibleState()
        {
            Debug.WriteLine("Visible: " + widget.Visible.ToString());
        }

        private void OutputWindowState()
        {
            Debug.WriteLine("Window State: " + widget.WindowState.ToString());
        }

        private void OutputGameBarDisplayMode()
        {
            Debug.WriteLine("Game Bar View Mode: " + widget.GameBarDisplayMode.ToString());
        }

        private void Slider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(_canSeek);
            if (_canSeek)
            {
                socket.Emit("media-commands", "player-set-seekbar", e.NewValue);
                _canSeek = false;
            }
        }

        private void Slider_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            _canSeek = false;
        }

        private void Slider_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            _canSeek = true;
        }
    }
}
