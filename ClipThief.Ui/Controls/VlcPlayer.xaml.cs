using System;
using System.IO;
using System.Reflection;
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
        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register(
                                                                                               "Source",
                                                                                               typeof(string),
                                                                                               typeof(VlcPlayer),
                                                                                               new PropertyMetadata(
                                                                                                                    null,
                                                                                                                    null,
                                                                                                                    OnSourcePropertyChange));

        public VlcPlayer()
        {
            InitializeComponent();
            var currentDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location);
            var vlcLibDirectory = new DirectoryInfo(
                                                    Path.Combine(
                                                                 currentDirectory,
                                                                 "libvlc",
                                                                 IntPtr.Size == 4
                                                                     ? "win-x86"
                                                                     : "win-x64"));
            SourceProvider.CreatePlayer(vlcLibDirectory);
        }

        public string Source
        {
            get => (string)GetValue(SourceProperty);
            set => SetValue(SourceProperty, value);
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

            var file = new FileInfo(fileLocation);
            var mediaPlayer = (VlcPlayer)d;
            mediaPlayer.SourceProvider.MediaPlayer.Play(file);

            return value;
        }
    }
}