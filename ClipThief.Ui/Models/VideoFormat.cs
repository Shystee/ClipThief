namespace ClipThief.Ui.Models
{
    public class VideoFormat : BaseFormat
    {
        public int Fps { get; set; }

        public string Quality { get; set; }

        public Resolution Resolution { get; set; }
    }
}