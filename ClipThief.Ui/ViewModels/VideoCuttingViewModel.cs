using System;
using System.IO;

using ClipThief.Ui.Core;

namespace ClipThief.Ui.ViewModels
{
    public class VideoCuttingViewModel : ViewModelBase, IRoutableViewModel
    {
        private TimeSpan startTime;

        private TimeSpan endTime;

        public VideoCuttingViewModel(string fileName)
        {
            Source = Directory.GetCurrentDirectory() + @"\" + fileName + ".mp4";
        }

        public string Source { get; }

        public TimeSpan StartTime
        {
            get => startTime;
            set => SetPropertyAndNotify(ref startTime, value);
        }

        public TimeSpan EndTime
        {
            get => endTime;
            set => SetPropertyAndNotify(ref endTime, value);
        }
    }
}