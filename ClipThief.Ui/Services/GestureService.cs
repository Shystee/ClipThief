using System;
using System.Reactive.Disposables;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

using ClipThief.Ui.Core;
using ClipThief.Ui.Extensions;

namespace ClipThief.Ui.Services
{
    public interface IGestureService
    {
        void SetBusy();
    }

    public sealed class GestureService : DisposableObject, IGestureService
    {
        private readonly DispatcherTimer timer;

        private bool isBusy;

        public GestureService()
        {
            timer = new DispatcherTimer(
                                        TimeSpan.Zero,
                                        DispatcherPriority.ApplicationIdle,
                                        TimerCallback,
                                        Application.Current.Dispatcher);
            timer.Stop();

            Disposable.Create(() => timer.Stop()).DisposeWith(this);
        }

        public void SetBusy()
        {
            SetBusyState(true);
        }

        private void SetBusyState(bool busy)
        {
            if (busy != isBusy)
            {
                isBusy = busy;
                Mouse.OverrideCursor = busy ? Cursors.Wait : null;

                if (isBusy)
                {
                    timer.Start();
                }
            }
        }

        private void TimerCallback(object sender, EventArgs e)
        {
            SetBusyState(false);
            timer.Stop();
        }
    }
}