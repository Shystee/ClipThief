using System;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace ClipThief.Ui.Controls
{
    /// <summary>
    ///     Interaction logic for SliderExtension.xaml
    /// </summary>
    public partial class SliderExtension : Slider
    {
        public delegate void OnDragEndedEventHandler(object sender, DragCompletedEventArgs e);

        public delegate void OnDragStartedEventHandler(object sender, DragStartedEventArgs e);

        public event OnDragEndedEventHandler OnDragEnded;

        public event OnDragStartedEventHandler OnDragStarted;

        public SliderExtension()
        {
            InitializeComponent();
        }

        protected override void OnThumbDragCompleted(DragCompletedEventArgs e)
        {
            base.OnThumbDragCompleted(e);
            OnDragEnded?.Invoke(this, e);
        }

        protected override void OnThumbDragStarted(DragStartedEventArgs e)
        {
            base.OnThumbDragStarted(e);
            OnDragStarted?.Invoke(this, e);
        }
    }
}