using System.Collections.Generic;
using ClipThief.Ui.Contexts;
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
        private readonly ApplicationContext applicationContext;

        private readonly IApplicationService applicationService;

        private readonly IVideoDownloadService videoDownloadService;

        public FormatSelectionViewModelFactory(
            ApplicationContext applicationContext,
            IApplicationService applicationService,
            IVideoDownloadService videoDownloadService)
        {
            this.applicationContext = applicationContext;
            this.applicationService = applicationService;
            this.videoDownloadService = videoDownloadService;
        }

        public IVideoFormatSelectionViewModel Create(string url, List<VideoFormat> videoFormats, List<AudioFormat> audioFormats)
        {
            return new VideoFormatSelectionViewModel(
                                                     videoFormats,
                                                     audioFormats,
                                                     applicationContext,
                                                     videoDownloadService,
                                                     applicationService);
        }
    }
}