using System.IO;

using ClipThief.Ui.Core;

namespace ClipThief.Ui.ViewModels
{
    public class VideoCuttingViewModel : ViewModelBase, IRoutableViewModel
    {
        public VideoCuttingViewModel(string fileName)
        {
            Source = Directory.GetCurrentDirectory() + @"\" + fileName + ".mp4";
        }

        public string Source { get; set; }
    }
}