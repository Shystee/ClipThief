using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Vlc.DotNet.Core;

namespace ClipThief.Ui.Controls
{
    /// <summary>
    ///     Interaction logic for MediaPlayer.xaml
    /// </summary>
    public partial class MediaPlayer : UserControl
    {
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(string), typeof(MediaPlayer), new PropertyMetadata(null));

        public MediaPlayer()
        {
            InitializeComponent();
            Unloaded += UnloadedHandler;
            PlayButton.Click += PauseButtonOnClick;
            Player.SourceProvider.MediaPlayer.PositionChanged += MediaPlayerOnPositionChanged;
            Player.VideoLoaded += PlayerVideoLoaded;
            Player.SourceProvider.MediaPlayer.EndReached += MediaPlayer_OnEndReached;
        }

        public string Source
        {
            get => (string)GetValue(SourceProperty);
            set => SetValue(SourceProperty, value);
        }

        private void CurrentTimeDragEnded(object sender, DragCompletedEventArgs e)
        {
            Timeline.CurrentSlider.ValueChanged -= OnCurrentTimeChange;
            Player.SourceProvider.MediaPlayer.Play();
        }

        private void CurrentTimeDragStarted(object sender, DragStartedEventArgs e)
        {
            Player.SourceProvider.MediaPlayer.Pause();
            Timeline.CurrentSlider.ValueChanged += OnCurrentTimeChange;
        }

        private void LowerSliderOnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (e.NewValue < Timeline.CurrentValue) return;

            if (Player.SourceProvider.MediaPlayer.IsPlaying())
            {
                Player.SourceProvider.MediaPlayer.Pause();
            }
        }

        private void LowerTimeDragEnded(object sender, DragCompletedEventArgs e)
        {
            if (!Player.SourceProvider.MediaPlayer.IsPlaying())
            {
                Player.SourceProvider.MediaPlayer.Position = (float)Timeline.CurrentValue;
                Player.SourceProvider.MediaPlayer.Play();
            }
        }

        private void MediaPlayer_OnEndReached(object? sender, VlcMediaPlayerEndReachedEventArgs e)
        {
            Dispatcher.Invoke(() => Player.RepeatVideoFrom((float)Timeline.LowerValue));
        }

        private void MediaPlayerOnMouseEnter(object sender, MouseEventArgs e)
        {
            MediaControl.Visibility = Visibility.Visible;
        }

        private void MediaPlayerOnMouseLeave(object sender, MouseEventArgs e)
        {
            MediaControl.Visibility = Visibility.Hidden;
        }

        private void MediaPlayerOnPositionChanged(object? sender, VlcMediaPlayerPositionChangedEventArgs e)
        {
            Dispatcher.Invoke(() => { Timeline.CurrentValue = e.NewPosition; });
        }

        private void OnCurrentTimeChange(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Player.SourceProvider.MediaPlayer.Position = (float)e.NewValue;
        }

        private void PauseButtonOnClick(object sender, RoutedEventArgs e)
        {
            Player.SourceProvider.MediaPlayer.Pause();
            PlayButton.Click -= PauseButtonOnClick;
            PlayButton.Click += PlayButtonOnClick;
        }

        private void PlayButtonOnClick(object sender, RoutedEventArgs e)
        {
            Player.SourceProvider.MediaPlayer.Play();
            PlayButton.Click -= PlayButtonOnClick;
            PlayButton.Click += PauseButtonOnClick;
        }

        private void PlayerVideoLoaded(object sender, OnVideoLoadedEventArgs e)
        {
            Dispatcher.Invoke(() =>
                              {
                                  Timeline.LowerValue = 0;
                                  Timeline.Minimum = 0;
                                  Timeline.CurrentValue = 0;
                                  Timeline.Maximum = 1;
                                  Timeline.UpperValue = 1;
                              });

            // setup lower timeline events
            Timeline.LowerSlider.ValueChanged += LowerSliderOnValueChanged;
            Timeline.LowerSlider.OnDragEnded += LowerTimeDragEnded;

            // setup current timeline events
            Timeline.CurrentSlider.OnDragStarted += CurrentTimeDragStarted;
            Timeline.CurrentSlider.OnDragEnded += CurrentTimeDragEnded;

            // setup upper timeline events
            Timeline.OnUpperValueReached += TimelineOnOnUpperValueReached;
        }

        private void TimelineOnOnUpperValueReached(object sender)
        {
            Player.SourceProvider.MediaPlayer.Position = (float)Timeline.LowerValue;

            if (Player.SourceProvider.MediaPlayer.IsPlaying()) return;

            Player.SourceProvider.MediaPlayer.Play();
        }

        private void UnloadedHandler(object sender, RoutedEventArgs e)
        {
            // unhook player events
            Player.SourceProvider.MediaPlayer.PositionChanged -= MediaPlayerOnPositionChanged;
            Player.VideoLoaded -= PlayerVideoLoaded;
            Player.SourceProvider.MediaPlayer.EndReached -= MediaPlayer_OnEndReached;

            // unhook current value events
            Timeline.CurrentSlider.OnDragStarted -= CurrentTimeDragStarted;
            Timeline.CurrentSlider.OnDragEnded -= CurrentTimeDragEnded;

            // unhook lower value events
            Timeline.LowerSlider.ValueChanged -= LowerSliderOnValueChanged;
            Timeline.LowerSlider.OnDragEnded -= LowerTimeDragEnded;

            // unhook upper value event
            Timeline.OnUpperValueReached -= TimelineOnOnUpperValueReached;

            if (Player.SourceProvider.MediaPlayer.IsPlaying())
            {
                PlayButton.Click -= PauseButtonOnClick;
            }
            else
            {
                PlayButton.Click -= PlayButtonOnClick;
            }

            // unhook itself
            Unloaded -= UnloadedHandler;
        }
    }
}