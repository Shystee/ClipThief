using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;

namespace ClipThief.Ui.Controls
{
    /// <summary>
    ///     Interaction logic for MediaPlayer.xaml
    /// </summary>
    public partial class MediaPlayer : UserControl, IDisposable
    {
        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register(
                                                                                               "Source",
                                                                                               typeof(Uri),
                                                                                               typeof(MediaPlayer),
                                                                                               new PropertyMetadata(
                                                                                                                    null,
                                                                                                                    null,
                                                                                                                    OnSourcePropertyChange));

        private DispatcherTimer timerVideoPlayback;

        public MediaPlayer()
        {
            InitializeComponent();
            PlayButton.Click += PauseButtonOnClick;
        }

        public Uri Source
        {
            get => (Uri)GetValue(SourceProperty);
            set => SetValue(SourceProperty, value);
        }

        public void Dispose()
        {
            Timeline.CurrentSlider.OnDragStarted -= CurrentTimeDragStarted;
            Timeline.CurrentSlider.OnDragEnded -= CurrentTimeDragEnded;

            if (Player.CanPause)
            {
                PlayButton.Click -= PauseButtonOnClick;
            }
            else
            {
                PlayButton.Click -= PlayButtonOnClick;
            }

            timerVideoPlayback.Tick -= TimerVideoPlaybackTick;
            timerVideoPlayback = null;
        }

        private void MediaPlayerOnMouseEnter(object sender, MouseEventArgs e)
        {
            MediaControl.Visibility = Visibility.Visible;
        }

        private void MediaPlayerOnMouseLeave(object sender, MouseEventArgs e)
        {
            MediaControl.Visibility = Visibility.Hidden;
        }

        private static object OnSourcePropertyChange(DependencyObject d, object value)
        {
            if (value == null)
            {
                return value;
            }

            var mediaPlayer = (MediaPlayer)d;
            mediaPlayer.StartVideo();

            return value;
        }

        private void PauseButtonOnClick(object sender, RoutedEventArgs e)
        {
            Player.Pause();
            timerVideoPlayback.Stop();
            PlayButton.Click -= PauseButtonOnClick;
            PlayButton.Click += PlayButtonOnClick;
        }

        private void PlayButtonOnClick(object sender, RoutedEventArgs e)
        {
            Player.Play();
            timerVideoPlayback.Start();
            PlayButton.Click -= PlayButtonOnClick;
            PlayButton.Click += PauseButtonOnClick;
        }

        private void PlayerOnMediaOpened(object sender, RoutedEventArgs e)
        {
            Timeline.OnUpperValueReached += TimelineOnOnUpperValueReached;

            // setup sliders
            Timeline.LowerValue = 0;
            Timeline.Minimum = 0;
            Timeline.Maximum = Player.NaturalDuration.TimeSpan.TotalSeconds;
            Timeline.UpperValue = Player.NaturalDuration.TimeSpan.TotalSeconds;

            // setup current time events on change
            Timeline.CurrentSlider.OnDragStarted += CurrentTimeDragStarted;
            Timeline.CurrentSlider.OnDragEnded += CurrentTimeDragEnded;

            // setup lower value events
            Timeline.LowerSlider.ValueChanged += LowerSliderOnValueChanged;
            Timeline.LowerSlider.OnDragEnded += LowerTimeDragEnded;

            timerVideoPlayback.Start();

            // unhook
            Player.MediaOpened -= PlayerOnMediaOpened;
        }

        private void LowerTimeDragEnded(object sender, DragCompletedEventArgs e)
        {
            if (!timerVideoPlayback.IsEnabled)
                Player.Position = TimeSpan.FromSeconds(Timeline.CurrentValue);

            timerVideoPlayback.Start();
            Player.Play();
        }

        private void LowerSliderOnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (e.NewValue < Timeline.CurrentValue) return;

            Player.Pause();
            timerVideoPlayback.Stop();
        }

        private void CurrentTimeDragEnded(object sender, DragCompletedEventArgs e)
        {
            Timeline.CurrentSlider.ValueChanged -= OnCurrentTimeChange;
            timerVideoPlayback.Start();
            Player.Play();
        }

        private void CurrentTimeDragStarted(object sender, DragStartedEventArgs e)
        {
            timerVideoPlayback.Stop();
            Player.Pause();
            Timeline.CurrentSlider.ValueChanged += OnCurrentTimeChange;
        }

        private void OnCurrentTimeChange(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Player.Position = TimeSpan.FromSeconds(e.NewValue);
        }

        private void StartVideo()
        {
            Player.MediaOpened += PlayerOnMediaOpened;

            // setup timer
            timerVideoPlayback = new DispatcherTimer();
            timerVideoPlayback.Interval = TimeSpan.FromSeconds(0.05);
            timerVideoPlayback.Tick += TimerVideoPlaybackTick;

            // play the video
            Player.Play();
        }

        private void TimelineOnOnUpperValueReached(object sender)
        {
            Player.Pause();
            Player.Position = TimeSpan.FromSeconds(Timeline.LowerValue);
            Player.Play();
        }

        private void TimerVideoPlaybackTick(object sender, object e)
        {
            if (!Player.NaturalDuration.HasTimeSpan)
            {
                Timeline.CurrentValue = 0;

                return;
            }

            Timeline.CurrentValue = Player.Position.TotalSeconds;
        }
    }
}