using System;
using System.Diagnostics;
using System.Reactive;

using ClipThief.Ui.Command;
using ClipThief.Ui.Core;
using ClipThief.Ui.Extensions;
using ClipThief.Ui.Services;

namespace ClipThief.Ui.ViewModels
{
    public class DownloadViewModel : ViewModelBase, IRoutableViewModel
    {
        private readonly IApplicationService applicationService;


        private string videoUrl;

        public DownloadViewModel(IApplicationService applicationService)
        {
            this.applicationService = applicationService;

            SelectFormatCommand = ReactiveCommand<Unit>.Create().DisposeWith(this);
            SelectFormatCommand.Subscribe(x => OpenVideoFormatSelection()).DisposeWith(this);
        }

        // todo: add validation
        public ReactiveCommand<Unit> SelectFormatCommand { get; }

        public string VideoUrl
        {
            get => videoUrl;
            set => SetPropertyAndNotify(ref videoUrl, value);
        }

        private void OpenVideoFormatSelection()
        {
            Debug.WriteLine($"Starting format selection on {VideoUrl}");

            // todo: inject factory to create the viewmodel
            applicationService.Post(new VideoFormatSelectionViewModel(applicationService));
        }
    }
}