using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using ClipThief.Ui.Events;
using ClipThief.Ui.Models;

namespace ClipThief.Ui.Services
{
    public interface IVideoDownloadService
    {
        Task DownloadAsync(string url);

        Task<List<VideoFormat>> GetVideoQualitiesAsync(string url);
    }

    public class YoutubeDownloadService : IVideoDownloadService
    {
        private const string VideoFormatPattern =
            @"(^[0-9]+)\s+([\w]+)\s+([0-9]+)x([0-9]+)\s+([\w]+)\s+([0-9]+)k[\s|,]+([\w]+)[\s|,]+([0-9]+)fps[\s|,]+([\w|\s]+)[\s|,]+([0-9|.]+)MiB";

        private Process process;

        public delegate void ErrorEventHandler(object sender, ProgressEventArgs e);

        public delegate void FinishedDownloadEventHandler(object sender, DownloadEventArgs e);

        public delegate void ProgressEventHandler(object sender, ProgressEventArgs e);

        public delegate void StartedDownloadEventHandler(object sender, DownloadEventArgs e);

        public event ErrorEventHandler ErrorDownload;

        public event FinishedDownloadEventHandler FinishedDownload;

        public event ProgressEventHandler ProgressDownload;

        public event StartedDownloadEventHandler StartedDownload;

        public Task DownloadAsync(string url)
        {
            throw new NotImplementedException();
        }

        public Task<List<VideoFormat>> GetVideoQualitiesAsync(string url)
        {
            // var arguments = $@"--continue  --no-overwrites --restrict-filenames --extract-audio --audio-format mp3 {url} -o ""{destinationPath}""";
            var arguments = $@"--list-formats {url}";

            // setup the process that will fire youtube-dl
            process = new Process
                          {
                              StartInfo =
                                  {
                                      UseShellExecute = false,
                                      RedirectStandardOutput = true,
                                      WindowStyle = ProcessWindowStyle.Hidden,
                                      FileName = "youtube-dl.exe",
                                      Arguments = arguments,
                                      CreateNoWindow = true
                                  },
                              EnableRaisingEvents = true
                          };

            var result = new List<VideoFormat>();

            void OnOutputDataReceived(object sender, DataReceivedEventArgs e)
            {
                if (string.IsNullOrEmpty(e.Data))
                {
                    return;
                }

                var videoFormat = GetVideoFormat(e.Data);

                if (videoFormat != null)
                {
                    result.Add(videoFormat);
                }
            }

            process.OutputDataReceived += OnOutputDataReceived;

            process.Start();
            process.BeginOutputReadLine();
            process.WaitForExit();

            // unhook
            process.OutputDataReceived -= OnOutputDataReceived;

            return Task.FromResult(result);
        }

        private VideoFormat GetVideoFormat(string dataLine)
        {
            var regex = new Regex(VideoFormatPattern);

            if (!regex.IsMatch(dataLine))
            {
                return null;
            }

            var matches = regex.Matches(dataLine)[0].Groups;

            // start form 1 index
            return new VideoFormat
                       {
                           FormatCode = Convert.ToInt32(matches[1].Value),
                           Format = matches[2].Value,
                           Resolution =
                               new Resolution(
                                              Convert.ToInt32(matches[3].Value),
                                              Convert.ToInt32(matches[4].Value)),
                           Quality = matches[5].Value,
                           BitRate = Convert.ToInt32(matches[6].Value),
                           Encoder = matches[7].Value,
                           Fps = Convert.ToInt32(matches[8].Value),
                           Description = matches[9].Value,
                           Size = Convert.ToDecimal(matches[10].Value)
                       };
        }
    }
}