using System.Collections.Generic;

using ClipThief.Ui.Models;
using ClipThief.Ui.Services;
using ClipThief.Ui.ViewModels;

namespace ClipThief.Ui.Factories
{
    public interface IFormatSelectionViewModelFactory
    {
        IVideoFormatSelectionViewModel Create(string url, List<VideoFormat> videoFormats, List<AudioFormat> audioFormats);
    }

    public class FormatSelectionViewModelFactory : IFormatSelectionViewModelFactory
    {
        private readonly IApplicationService applicationService;

        private readonly IVideoDownloadService videoDownloadService;

        public FormatSelectionViewModelFactory(
            IApplicationService applicationService,
            IVideoDownloadService videoDownloadService)
        {
            this.applicationService = applicationService;
            this.videoDownloadService = videoDownloadService;
        }

        public IVideoFormatSelectionViewModel Create(string url, List<VideoFormat> videoFormats, List<AudioFormat> audioFormats)
        {
            return new VideoFormatSelectionViewModel(
                                                     url,
                                                     videoFormats,
                                                     audioFormats,
                                                     videoDownloadService,
                                                     applicationService);
        }
    }
}