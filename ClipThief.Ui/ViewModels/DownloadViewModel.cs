using System;
using System.Reactive;

using ClipThief.Ui.Command;

namespace ClipThief.Ui.ViewModels
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