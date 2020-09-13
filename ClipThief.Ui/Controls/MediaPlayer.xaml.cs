using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace ClipThief.Ui.Controls
{
    /// <summary>
    ///     Interaction logic for MediaPlayer.xaml
    /// </summary>
    public partial class MediaPlayer : UserControl
    {
        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register(
                                                                                               "Source",
                                                                                               typeof(Uri),
                                                                                               typeof(MediaPlayer),
                                                                                               new PropertyMetadata(
                                                                                                                    null,
                                                                                                                    null,
                                                                                                                    OnSourcePropertyChange));

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

        private static object OnSourcePropertyChange(DependencyObject d, object value)
        {
            if (value == null)
            {
                return value;
            }

            var mediaPlayer = (MediaPlayer)d;
            mediaPlayer.Player.Play();

            return value;
        }

        private void MediaPlayerOnMouseEnter(object sender, MouseEventArgs e)
        {
            MediaControl.Visibility = Visibility.Visible;
        }

        private void MediaPlayerOnMouseLeave(object sender, MouseEventArgs e)
        {
            MediaControl.Visibility = Visibility.Hidden;
        }

        private void PauseButtonOnClick(object sender, RoutedEventArgs e)
        {
            Player.Pause();
            PlayButton.Click -= PauseButtonOnClick;
            PlayButton.Click += PlayButtonOnClick;
        }

        private void PlayButtonOnClick(object sender, RoutedEventArgs e)
        {
            Player.Play();
            PlayButton.Click -= PlayButtonOnClick;
            PlayButton.Click += PauseButtonOnClick;
        }
    }
}