using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using ClipThief.Ui.Events;
using ClipThief.Ui.Exceptions;
using ClipThief.Ui.Models;

namespace ClipThief.Ui.Services
{
    public interface IVideoDownloadService
    {
        event YoutubeDownloadService.ErrorEventHandler ErrorDownload;

        event YoutubeDownloadService.FinishedDownloadEventHandler FinishedDownload;

        event YoutubeDownloadService.ProgressEventHandler ProgressDownload;

        event YoutubeDownloadService.StartedDownloadEventHandler StartedDownload;

        Task DownloadAsync(string url, string fileName, int videoFormat, int audioFormat);

        Task<List<AudioFormat>> GetAudioQualitiesAsync(string url);

        Task<List<VideoFormat>> GetVideoQualitiesAsync(string url);
    }

    public class YoutubeDownloadService : IVideoDownloadService
    {
        private const string AudioFormatPattern =
            @"(^[0-9]+)\s+([\w]+)\s+([a-z]+\s[a-z]+)\s+([\w]+)\s+([0-9]+)k , ([\w&\s&@]+)\(([0-9]+)Hz\),\s([0-9|.]+)MiB";

        private const string VideoFormatPattern =
            @"(^[0-9]+)\s+([\w]+)\s+([0-9]+)x([0-9]+)\s+([\w]+)\s+([0-9]+)k[\s|,]+([\w.]+)[\s|,]+([0-9]+)fps[\s|,]+([\w|\s]+)[\s|,]+([0-9|.]+)MiB";

        private Process process;

        public delegate void ErrorEventHandler(object sender, ProgressEventArgs e);

        public delegate void FinishedDownloadEventHandler(object sender, DownloadEventArgs e);

        public delegate void ProgressEventHandler(object sender, ProgressEventArgs e);

        public delegate void StartedDownloadEventHandler(object sender, DownloadEventArgs e);

        public event ErrorEventHandler ErrorDownload;

        public event FinishedDownloadEventHandler FinishedDownload;

        public event ProgressEventHandler ProgressDownload;

        public event StartedDownloadEventHandler StartedDownload;

        public bool Finished { get; private set; }

        public decimal Percentage { get; private set; }

        public Task DownloadAsync(string url, string fileName, int videoFormat, int audioFormat)
        {
            Finished = false;
            Percentage = 0;

            var arguments =
                $"--continue  --no-overwrites --restrict-filenames -o \"{fileName}.%(ext)s\" --format {videoFormat}+{audioFormat} --merge-output-format mp4 {url}";

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

            process.OutputDataReceived += DownloadOutputHandler;
            process.ErrorDataReceived += DownloadErrorDataReceived;
            process.Exited += ProcessExited;
            process.Start();
            process.BeginOutputReadLine();
            OnDownloadStarted(new DownloadEventArgs { ProcessObject = process });

            return Task.Run(() => process.WaitForExit());
        }

        public Task<List<AudioFormat>> GetAudioQualitiesAsync(string url)
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

            var result = new List<AudioFormat>();

            void OnOutputDataReceived(object sender, DataReceivedEventArgs e)
            {
                if (string.IsNullOrEmpty(e.Data))
                {
                    return;
                }

                var videoFormat = GetAudioFormat(e.Data);

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

        protected virtual void OnDownloadError(ProgressEventArgs e)
        {
            ErrorDownload?.Invoke(this, e);
        }

        protected virtual void OnDownloadFinished(DownloadEventArgs e)
        {
            Finished = true;
            process.OutputDataReceived -= DownloadOutputHandler;
            process.ErrorDataReceived -= DownloadErrorDataReceived;
            process.Exited -= ProcessExited;
            FinishedDownload?.Invoke(this, e);
        }

        protected virtual void OnDownloadStarted(DownloadEventArgs e)
        {
            StartedDownload?.Invoke(this, e);
        }

        protected virtual void OnProgress(ProgressEventArgs e)
        {
            ProgressDownload?.Invoke(this, e);
        }

        private void DownloadErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                OnDownloadError(
                                new ProgressEventArgs
                                    {
                                        Error = e.Data,
                                        ProcessObject = process
                                    });
            }
        }

        private void DownloadOutputHandler(object sender, DataReceivedEventArgs e)
        {
            // extract the percentage from process output
            if (string.IsNullOrEmpty(e.Data))
            {
                return;
            }

            // this.ConsoleLog += outLine.Data;
            if (e.Data.Contains("ERROR"))
            {
                OnDownloadError(
                                new ProgressEventArgs
                                    {
                                        Error = e.Data,
                                        ProcessObject = process
                                    });

                return;
            }

            if (!e.Data.Contains("[download]"))
            {
                return;
            }

            var pattern = new Regex(@"\b\d+([\.,]\d+)?", RegexOptions.None);

            if (!pattern.IsMatch(e.Data))
            {
                return;
            }

            // fire the process event
            var percentage = Convert.ToDecimal(Regex.Match(e.Data, @"\b\d+([\.,]\d+)?").Value);

            if (percentage > 100 || percentage < 0)
            {
                throw new WrongPercentageDecimalException("Weird percentage parsed.");
            }

            Percentage = percentage;
            OnProgress(
                       new ProgressEventArgs
                           {
                               ProcessObject = process,
                               Percentage = percentage
                           });

            // is it finished?
            if (percentage < 100)
            {
                return;
            }

            if (percentage == 100)
            {
                OnDownloadFinished(new DownloadEventArgs { ProcessObject = process });
            }
        }

        private AudioFormat GetAudioFormat(string dataLine)
        {
            var regex = new Regex(AudioFormatPattern);

            if (!regex.IsMatch(dataLine))
            {
                return null;
            }

            var matches = regex.Matches(dataLine)[0].Groups;

            // start form 1 index
            return new AudioFormat
                       {
                           FormatCode = Convert.ToInt32(matches[1].Value),
                           Format = matches[2].Value,
                           Description = matches[3].Value + matches[4].Value,
                           BitRate = Convert.ToInt32(matches[5].Value),
                           Encoder = matches[6].Value,
                           Hertz = Convert.ToInt32(matches[7].Value),
                           Size = Convert.ToDecimal(matches[8].Value)
                       };
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

        private void ProcessExited(object sender, EventArgs e)
        {
            OnDownloadFinished(new DownloadEventArgs { ProcessObject = process });
        }
    }
}