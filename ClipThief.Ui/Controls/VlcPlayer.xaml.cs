using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows;
using Vlc.DotNet.Core;
using Vlc.DotNet.Wpf;

namespace ClipThief.Ui.Controls
{
    /// <summary>
    ///     Interaction logic for VlcPlayer.xaml
    /// </summary>
    public partial class VlcPlayer : VlcControl
    {
        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register("Source",
            typeof(string),
            typeof(VlcPlayer),
            new PropertyMetadata(null, null, OnSourcePropertyChange));

        private FileInfo currentFile;

        private bool newFile;

        public VlcPlayer()
        {
            InitializeComponent();
            var currentDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location);
            var vlcLibDirectory = new DirectoryInfo(Path.Combine(currentDirectory,
                "libvlc",
                IntPtr.Size == 4
                    ? "win-x86"
                    : "win-x64"));
            SourceProvider.CreatePlayer(vlcLibDirectory);
            SourceProvider.MediaPlayer.Playing += OnPlay;
        }

        public delegate void OnVideoLoadedEventHandler(object sender, OnVideoLoadedEventArgs e);

        public event OnVideoLoadedEventHandler VideoLoaded;

        public string Source
        {
            get => (string)GetValue(SourceProperty);
            set => SetValue(SourceProperty, value);
        }

        public void RepeatVideoFrom(float startTime)
        {
            // bug with vlc https://github.com/ZeBobo5/Vlc.DotNet/wiki/Vlc.DotNet-freezes-(don't-call-Vlc.DotNet-from-a-Vlc.DotNet-callback)
            ThreadPool.QueueUserWorkItem(_ =>
                                         {
                                             SourceProvider.MediaPlayer.Play(currentFile);
                                             SourceProvider.MediaPlayer.Position = startTime;
                                         });
        }

        private static object OnSourcePropertyChange(DependencyObject d, object value)
        {
            if (value == null)
            {
                return null;
            }

            var fileLocation = (string)value;
            var fileExists = File.Exists(fileLocation);

            if (!fileExists)
            {
                throw new FileNotFoundException("Media file not found");
            }

            var mediaPlayer = (VlcPlayer)d;
            mediaPlayer.currentFile = new FileInfo(fileLocation);
            mediaPlayer.newFile = true;
            mediaPlayer.SourceProvider.MediaPlayer.Play(mediaPlayer.currentFile);

            return value;
        }

        private void OnPlay(object? sender, VlcMediaPlayerPlayingEventArgs e)
        {
            if (!newFile) return;

            newFile = false;
            VideoLoaded?.Invoke(sender, new OnVideoLoadedEventArgs { VideoLength = SourceProvider.MediaPlayer.Length });
        }
    }
}