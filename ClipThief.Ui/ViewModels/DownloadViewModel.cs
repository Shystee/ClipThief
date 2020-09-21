using System;
using System.Diagnostics;
using System.Reactive;
using System.Threading.Tasks;

using ClipThief.Ui.Command;
using ClipThief.Ui.Core;
using ClipThief.Ui.Extensions;
using ClipThief.Ui.Factories;
using ClipThief.Ui.Services;

namespace ClipThief.Ui.ViewModels
{
    public interface IDownloadViewModel : IRoutableViewModel
    {
    }

    public class DownloadViewModel : ViewModelBase, IDownloadViewModel
    {
        private readonly IApplicationService applicationService;

        private readonly IFormatSelectionViewModelFactory factory;

        private readonly IVideoDownloadService videoDownloadService;

        private string videoUrl;

        public DownloadViewModel(IFormatSelectionViewModelFactory factory, IVideoDownloadService videoDownloadService, IApplicationService applicationService)
        {
            this.factory = factory;
            this.videoDownloadService = videoDownloadService;
            this.applicationService = applicationService;

            SelectFormatCommand = ReactiveCommand<Unit>.Create().DisposeWith(this);
            SelectFormatCommand.Subscribe(x => OpenVideoFormatSelectionAsync()).DisposeWith(this);
        }

        // todo: add validation
        public ReactiveCommand<Unit> SelectFormatCommand { get; }

        public string VideoUrl
        {
            get => videoUrl;
            set => SetPropertyAndNotify(ref videoUrl, value);
        }

        private async Task OpenVideoFormatSelectionAsync()
        {
            var videoFormats = await videoDownloadService.GetVideoQualitiesAsync(VideoUrl).ConfigureAwait(false);
            var audioFormats = await videoDownloadService.GetAudioQualitiesAsync(VideoUrl).ConfigureAwait(false);
            applicationService.Post(factory.Create(VideoUrl, videoFormats, audioFormats));
        }
    }
}