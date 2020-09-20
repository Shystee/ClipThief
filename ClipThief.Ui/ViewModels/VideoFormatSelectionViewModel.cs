using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        private VideoFormat selectedVideoFormat;

        private AudioFormat selectedAudioFormat;

        public VideoFormatSelectionViewModel(List<VideoFormat> videoFormats, List<AudioFormat> audioFormats, IApplicationService applicationService)
        {
            VideoFormats = videoFormats;
            AudioFormats = audioFormats;
            this.applicationService = applicationService;

            OpenVideoCuttingCommand = ReactiveCommand<Unit>.Create().DisposeWith(this);
            OpenVideoCuttingCommand.Subscribe(x => OpenVideoCutting()).DisposeWith(this);
        }

        public List<VideoFormat> VideoFormats { get; }

        public VideoFormat SelectedVideoFormat
        {
            get => selectedVideoFormat;
            set => SetPropertyAndNotify(ref selectedVideoFormat, value);
        }

        public List<AudioFormat> AudioFormats { get; }

        public AudioFormat SelectedFormat
        {
            get => selectedAudioFormat;
            set => SetPropertyAndNotify(ref selectedAudioFormat, value);
        }

        public ReactiveCommand<Unit> OpenVideoCuttingCommand { get; }

        private void OpenVideoCutting()
        {
            applicationService.Post(new VideoCuttingViewModel());
        }
    }
}