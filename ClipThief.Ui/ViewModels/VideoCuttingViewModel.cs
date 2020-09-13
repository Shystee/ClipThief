using System;

namespace ClipThief.Ui.ViewModels
{
    public class VideoCuttingViewModel : ViewModelBase, IRoutableViewModel
    {
        private string source;

        public VideoCuttingViewModel()
        {
            Source = "C:\\Users\\armin\\Documents\\Workplace\\yt-trimmer-master\\fatnig.mp4";
        }

        public string Source
        {
            get => source;
            set => source = value;
        }
    }
}