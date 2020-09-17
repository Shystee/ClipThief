using System;
using System.Reactive;

using ClipThief.Ui.Command;
using ClipThief.Ui.Core;
using ClipThief.Ui.Extensions;
using ClipThief.Ui.Services;

namespace ClipThief.Ui.ViewModels
{
    public class VideoFormatSelectionViewModel : ViewModelBase, IRoutableViewModel
    {
        private readonly IApplicationService applicationService;

        public VideoFormatSelectionViewModel(IApplicationService applicationService)
        {
            this.applicationService = applicationService;

            OpenVideoCuttingCommand = ReactiveCommand<Unit>.Create().DisposeWith(this);
            OpenVideoCuttingCommand.Subscribe(x => OpenVideoCutting()).DisposeWith(this);
        }

        public ReactiveCommand<Unit> OpenVideoCuttingCommand { get; }

        private void OpenVideoCutting()
        {
            applicationService.Post(new VideoCuttingViewModel());
        }
    }
}