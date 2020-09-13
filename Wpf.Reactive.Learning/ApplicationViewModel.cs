using System;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;

using Wpf.Reactive.Learning.Command;
using Wpf.Reactive.Learning.ViewModels;

namespace Wpf.Reactive.Learning
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