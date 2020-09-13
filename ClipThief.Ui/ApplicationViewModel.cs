using System;

using ClipThief.Ui.ViewModels;

namespace ClipThief.Ui
{
    public interface IRoutableViewModel
    {
    }

    public class ApplicationViewModel : ReactiveObject
    {
        private IRoutableViewModel main;

        public ApplicationViewModel(DownloadViewModel main, IApplicationService applicationService)
        {
            Main = main;

            applicationService.Show.Subscribe(x => Main = x).DisposeWith(this);
        }

        public IRoutableViewModel Main
        {
            get => main;
            set => SetPropertyAndNotify(ref main, value);
        }
    }
}