using ClipThief.Ui.Core;

namespace ClipThief.Ui.ViewModels
{
    public class VideoCuttingViewModel : ViewModelBase, IRoutableViewModel
    {
        public VideoCuttingViewModel()
        {
            Source = "C:\\Users\\armin\\Documents\\Workplace\\yt-trimmer-master\\fatnig.mp4";
        }

        public string Source { get; set; }
    }
}