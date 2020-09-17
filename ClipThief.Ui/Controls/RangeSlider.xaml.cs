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

        public static readonly DependencyProperty CurrentValueProperty =
            DependencyProperty.Register(
                                        nameof(CurrentValue),
                                        typeof(double),
                                        typeof(RangeSlider),
                                        new UIPropertyMetadata(0.5d, null, CurrentValueCoerceValueCallback));

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

        public delegate void OnUpperValueReachedEventHandler(object sender);

        public event OnUpperValueReachedEventHandler OnUpperValueReached;

        public double CurrentValue
        {
            get => (double)GetValue(CurrentValueProperty);
            set => SetValue(CurrentValueProperty, value);
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

        private static object CurrentValueCoerceValueCallback(DependencyObject d, object valueObject)
        {
            var targetSlider = (RangeSlider)d;
            var value = (double)valueObject;

            if (targetSlider.LowerValue < value && value < targetSlider.UpperValue)
            {
                return value;
            }

            if (value < targetSlider.LowerValue)
            {
                return targetSlider.LowerValue;
            }

            if (value > targetSlider.UpperValue)
            {
                targetSlider.OnUpperValueReached?.Invoke(targetSlider);

                return targetSlider.UpperValue;
            }

            return value;
        }

        private static object LowerValueCoerceValueCallback(DependencyObject d, object valueObject)
        {
            var targetSlider = (RangeSlider)d;
            var value = (double)valueObject;

            if (targetSlider.CurrentValue < value)
            {
                targetSlider.CurrentValue = value;
            }

            return Math.Min(value, targetSlider.UpperValue);
        }

        private static object UpperValueCoerceValueCallback(DependencyObject d, object valueObject)
        {
            var targetSlider = (RangeSlider)d;
            var value = (double)valueObject;

            if (targetSlider.CurrentValue > value)
            {
                targetSlider.CurrentValue = value;
            }

            return Math.Max(value, targetSlider.LowerValue);
        }
    }
}