using System;

namespace ClipThief.Ui.Events
{
    public class ProgressEventArgs : EventArgs
    {
        public string Error { get; set; }

        public decimal Percentage { get; set; }

        public object ProcessObject { get; set; }
    }
}