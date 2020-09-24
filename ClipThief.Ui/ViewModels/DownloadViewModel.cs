using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

using ClipThief.Ui.Contexts;
using ClipThief.Ui.Core;
using ClipThief.Ui.Extensions;
using ClipThief.Ui.Factories;
using ClipThief.Ui.Services;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace ClipThief.Ui.ViewModels
{
    public interface IDownloadViewModel : IRoutableViewModel
    {
    }

    public class DownloadViewModel : ViewModelBase, IDownloadViewModel
    {
        private readonly IApplicationService applicationService;

        private readonly ApplicationContext applicationContext;

        private readonly IFormatSelectionViewModelFactory factory;

        private readonly IVideoDownloadService videoDownloadService;

        public DownloadViewModel(ApplicationContext applicationContext, IFormatSelectionViewModelFactory factory, IVideoDownloadService videoDownloadService, IApplicationService applicationService)
        {
            this.applicationContext = applicationContext;
            this.factory = factory;
            this.videoDownloadService = videoDownloadService;
            this.applicationService = applicationService;

            VideoUrl = applicationContext.ToReactivePropertyAsSynchronized(x => x.VideoUrl);

            SelectFormatCommand = new AsyncReactiveCommand().DisposeWith(this);
            SelectFormatCommand.Subscribe(x => OpenVideoFormatSelectionAsync()).DisposeWith(this);
        }

        // todo: add validation
        public AsyncReactiveCommand SelectFormatCommand { get; }

        public ReactiveProperty<string> VideoUrl { get; }

        private async Task OpenVideoFormatSelectionAsync()
        {
            var videoFormats = videoDownloadService.GetVideoQualitiesAsync(applicationContext.VideoUrl).ConfigureAwait(false);
            var audioFormats = videoDownloadService.GetAudioQualitiesAsync(applicationContext.VideoUrl).ConfigureAwait(false);
            applicationService.Post(factory.Create(applicationContext.VideoUrl, await videoFormats, await audioFormats));
        }
    }
}