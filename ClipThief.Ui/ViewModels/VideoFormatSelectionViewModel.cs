using System;
using System.Collections.Generic;
using System.Reactive;

using ClipThief.Ui.Command;
using ClipThief.Ui.Core;
using ClipThief.Ui.Extensions;
using ClipThief.Ui.Models;
using ClipThief.Ui.Services;

namespace ClipThief.Ui.ViewModels
{
    public interface IVideoFormatSelectionViewModel : IRoutableViewModel
    {
    }

    public class VideoFormatSelectionViewModel : ViewModelBase, IVideoFormatSelectionViewModel
    {
        private readonly IApplicationService applicationService;

        private readonly string url;

        private readonly IVideoDownloadService videoService;

        private string fileName;

        private AudioFormat selectedAudioFormat;

        private VideoFormat selectedVideoFormat;

        public VideoFormatSelectionViewModel(
            string url,
            List<VideoFormat> videoFormats,
            List<AudioFormat> audioFormats,
            IVideoDownloadService videoService,
            IApplicationService applicationService)
        {
            VideoFormats = videoFormats;
            AudioFormats = audioFormats;
            this.url = url;
            this.videoService = videoService;
            this.applicationService = applicationService;

            OpenVideoCuttingCommand = ReactiveCommand<Unit>.Create().DisposeWith(this);
            OpenVideoCuttingCommand.Subscribe(x => OpenVideoCutting()).DisposeWith(this);
        }

        public List<AudioFormat> AudioFormats { get; }

        public string FileName
        {
            get => fileName;
            set => SetPropertyAndNotify(ref fileName, value);
        }

        public ReactiveCommand<Unit> OpenVideoCuttingCommand { get; }

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

        private void OpenVideoCutting()
        {
            videoService.FinishedDownload += (sender, args) =>
                                                 {
                                                     applicationService.Post(new VideoCuttingViewModel(FileName));
                                                 };
            videoService.DownloadAsync(
                                       url,
                                       FileName,
                                       selectedVideoFormat.FormatCode,
                                       selectedAudioFormat.FormatCode);
        }
    }
}