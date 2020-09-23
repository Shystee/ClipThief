using ClipThief.Ui.Core;

namespace ClipThief.Ui.Contexts
{
    public class ApplicationContext : ReactiveObject
    {
        private string videoUrl;

        public ApplicationContext()
        {
        }

        public string VideoUrl
        {
            get => videoUrl;
            set => SetPropertyAndNotify(ref videoUrl, value);
        }
    }
}