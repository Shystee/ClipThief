using System.Windows;
using System.Windows.Controls;

namespace ClipThief.Ui.Controls
{
    /// <summary>
    ///     Interaction logic for Popup.xaml
    /// </summary>
    public partial class Popup : UserControl
    {
        public static readonly DependencyProperty AdditionalContentProperty =
            DependencyProperty.Register("AdditionalContent", typeof(object), typeof(Popup), new PropertyMetadata(null));

        // public static readonly DependencyProperty PopupColorProperty =
        // DependencyProperty.Register("AdditionalContent", typeof(Brush), typeof(Popup), new PropertyMetadata(Color.White));
        public Popup()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Gets or sets additional content for the UserControl
        /// </summary>
        public object AdditionalContent
        {
            get => GetValue(AdditionalContentProperty);
            set => SetValue(AdditionalContentProperty, value);
        }

        // public Brush PopupColor
        // {
        // get => (Brush)GetValue(PopupColorProperty);
        // set => SetValue(PopupColorProperty, value);
        // }
    }
}