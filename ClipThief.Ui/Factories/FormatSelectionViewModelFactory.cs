using System.Collections.Generic;

using ClipThief.Ui.Models;
using ClipThief.Ui.Services;
using ClipThief.Ui.ViewModels;

namespace ClipThief.Ui.Factories
{
    public interface IFormatSelectionViewModelFactory
    {
        IVideoFormatSelectionViewModel Create(List<VideoFormat> videoFormats, List<AudioFormat> audioFormats);
    }

    public class FormatSelectionViewModelFactory : IFormatSelectionViewModelFactory
    {
        private readonly IApplicationService applicationService;

        public FormatSelectionViewModelFactory(IApplicationService applicationService)
        {
            this.applicationService = applicationService;
        }

        public IVideoFormatSelectionViewModel Create(List<VideoFormat> videoFormats, List<AudioFormat> audioFormats) => new VideoFormatSelectionViewModel(videoFormats, audioFormats,applicationService);
    }
}