namespace ClipThief.Ui.Models
{
    public class DownloadProgress
    {
        public DownloadProgress(string fileName)
        {
            FileName = fileName;
        }

        public string FileName { get; }

        public bool Finished { get; set; }

        public decimal Percentage { get; set; }
    }
}