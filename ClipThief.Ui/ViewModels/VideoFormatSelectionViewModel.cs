using System.Collections.Generic;
using System.Threading.Tasks;
using ClipThief.Ui.Contexts;
using ClipThief.Ui.Core;
using ClipThief.Ui.Events;
using ClipThief.Ui.Extensions;
using ClipThief.Ui.Models;
using ClipThief.Ui.Services;

using Reactive.Bindings;

namespace ClipThief.Ui.ViewModels
{
    public interface IVideoFormatSelectionViewModel : IRoutableViewModel
    {
    }

    public class VideoFormatSelectionViewModel : ViewModelBase, IVideoFormatSelectionViewModel
    {
        private readonly IApplicationService applicationService;

        private readonly ApplicationContext applicationContext;

        private readonly IVideoDownloadService videoService;

        private string fileName;

        private AudioFormat selectedAudioFormat;

        private VideoFormat selectedVideoFormat;

        public VideoFormatSelectionViewModel(
            List<VideoFormat> videoFormats,
            List<AudioFormat> audioFormats,
            ApplicationContext applicationContext,
            IVideoDownloadService videoService,
            IApplicationService applicationService)
        {
            VideoFormats = videoFormats;
            AudioFormats = audioFormats;
            this.applicationContext = applicationContext;
            this.videoService = videoService;
            this.applicationService = applicationService;

            OpenVideoCuttingCommand = new AsyncReactiveCommand().DisposeWith(this);
            OpenVideoCuttingCommand.Subscribe(x => OpenVideoCuttingAsync()).DisposeWith(this);
        }

        public List<AudioFormat> AudioFormats { get; }

        public string FileName
        {
            get => fileName;
            set => SetPropertyAndNotify(ref fileName, value);
        }

        public AsyncReactiveCommand OpenVideoCuttingCommand { get; }

        public AudioFormat SelectedAudioFormat
        {
            get => selectedAudioFormat;
            set => SetPropertyAndNotify(ref selectedAudioFormat, value);
        }

        public VideoFormat SelectedVideoFormat
        {
            get => selectedVideoFormat;
            set => SetPropertyAndNotify(ref selectedVideoFormat, value);
        }

        public List<VideoFormat> VideoFormats { get; }

        private void OnFinishedDownload(object sender, DownloadEventArgs args)
        {
            applicationService.Post(new VideoCuttingViewModel(FileName));
            videoService.FinishedDownload -= OnFinishedDownload;
        }

        private Task OpenVideoCuttingAsync()
        {
            videoService.FinishedDownload += OnFinishedDownload;

            return videoService.DownloadAsync(applicationContext.VideoUrl, FileName, selectedVideoFormat.FormatCode, selectedAudioFormat.FormatCode);
        }
    }
}