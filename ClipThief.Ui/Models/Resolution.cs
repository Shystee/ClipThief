namespace ClipThief.Ui.Models
{
    public class Resolution
    {
        public Resolution(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public Resolution()
        {
        }

        public int Height { get; set; }

        public int Width { get; set; }
    }
}