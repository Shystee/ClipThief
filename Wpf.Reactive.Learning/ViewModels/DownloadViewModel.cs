using System;
using System.Reactive;
using System.Reactive.Linq;
using Wpf.Reactive.Learning.Command;

namespace Wpf.Reactive.Learning.ViewModels
{
    public class DownloadViewModel : ReactiveObject, IRoutableViewModel
    {
        private readonly IApplicationService applicationService;

        public DownloadViewModel(IApplicationService applicationService)
        {
            this.applicationService = applicationService;

            SelectFormatCommand = ReactiveCommand<Unit>.Create().DisposeWith(this);
            SelectFormatCommand.Subscribe(x => OpenVideoFormatSelection()).DisposeWith(this);
        }

        public ReactiveCommand<Unit> SelectFormatCommand { get; }

        private void OpenVideoFormatSelection()
        {
            applicationService.Post(new VideoFormatSelectionViewModel());
        }
    }
}