using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ClipThief.Ui.Services
{
    public interface IVideoCuttingService
    {
        Task CutVideoAsync(string fileName, TimeSpan startTime, TimeSpan length);
    }

    public class FfmpegCuttingService : IVideoCuttingService
    {
        private Process process;

        public Task CutVideoAsync(string fileName, TimeSpan startTime, TimeSpan length)
        {
            var arguments = $"ffmpeg -i {fileName}.mp4 -ss {startTime:hh:mm:ss} -t {length:hh:mm:ss} -async 1 cut.mp4";

            // setup the process that will fire youtube-dl
            process = new Process
                          {
                              StartInfo =
                                  {
                                      UseShellExecute = false,
                                      RedirectStandardOutput = true,
                                      WindowStyle = ProcessWindowStyle.Hidden,
                                      FileName = "ffmpeg.exe",
                                      Arguments = arguments,
                                      CreateNoWindow = true
                                  },
                              EnableRaisingEvents = true
                          };

            return Task.Run(() => process.WaitForExit());
        }
    }
}