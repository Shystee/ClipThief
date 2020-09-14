using System;
using System.Windows;
using System.Windows.Controls;

namespace ClipThief.Ui.Controls
{
    /// <summary>
    ///     Interaction logic for RangeSlider.xaml
    /// </summary>
    public partial class RangeSlider : UserControl
    {
        public static readonly DependencyProperty LowerValueProperty =
            DependencyProperty.Register(
                                        nameof(LowerValue),
                                        typeof(double),
                                        typeof(RangeSlider),
                                        new UIPropertyMetadata(0d, null, LowerValueCoerceValueCallback));

        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register(
                                        nameof(Maximum),
                                        typeof(double),
                                        typeof(RangeSlider),
                                        new UIPropertyMetadata(0d));

        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register(
                                        nameof(Minimum),
                                        typeof(double),
                                        typeof(RangeSlider),
                                        new UIPropertyMetadata(0d));

        public static readonly DependencyProperty UpperValueProperty =
            DependencyProperty.Register(
                                        nameof(UpperValue),
                                        typeof(double),
                                        typeof(RangeSlider),
                                        new UIPropertyMetadata(1d, null, UpperValueCoerceValueCallback));

        public RangeSlider()
        {
            InitializeComponent();
        }

        public double LowerValue
        {
            get => (double)GetValue(LowerValueProperty);
            set => SetValue(LowerValueProperty, value);
        }

        public double Maximum
        {
            get => (double)GetValue(MaximumProperty);
            set => SetValue(MaximumProperty, value);
        }

        public double Minimum
        {
            get => (double)GetValue(MinimumProperty);
            set => SetValue(MinimumProperty, value);
        }

        public double UpperValue
        {
            get => (double)GetValue(UpperValueProperty);
            set => SetValue(UpperValueProperty, value);
        }

        private static object LowerValueCoerceValueCallback(DependencyObject d, object valueObject)
        {
            var targetSlider = (RangeSlider)d;
            var value = (double)valueObject;

            return Math.Min(value, targetSlider.UpperValue);
        }

        private static object UpperValueCoerceValueCallback(DependencyObject d, object valueObject)
        {
            var targetSlider = (RangeSlider)d;
            var value = (double)valueObject;

            return Math.Max(value, targetSlider.LowerValue);
        }
    }
}