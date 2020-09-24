using System;

using ClipThief.Ui.Core;
using ClipThief.Ui.Extensions;
using ClipThief.Ui.Services;

namespace ClipThief.Ui.ViewModels
{
    public interface IApplicationViewModel
    {
        IRoutableViewModel Main { get; }
    }

    public class ApplicationViewModel : ViewModelBase, IApplicationViewModel
    {
        private IRoutableViewModel main;

        public ApplicationViewModel(IDownloadViewModel main, IApplicationService applicationService)
        {
            // Main = main;
            Main = new VideoCuttingViewModel("TESTING21");

            applicationService.Show.Subscribe(x => Main = x).DisposeWith(this);
        }

        public IRoutableViewModel Main
        {
            get => main;
            private set => SetPropertyAndNotify(ref main, value);
        }
    }
}