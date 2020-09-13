using System;
using System.Reactive;
using System.Reactive.Linq;

using ClipThief.Ui.Command;

namespace ClipThief.Ui
{
    public static class ObservableExtensions
    {
        public static IGestureService GestureService { get; set; }

        public static IObservable<T> ActivateGestures<T>(this IObservable<T> observable)
        {
            if (GestureService == null)
            {
                throw new Exception("GestureService has not been initialised");
            }

            return observable.Do(x => GestureService.SetBusy());
        }

        public static IObservable<Unit> AsUnit<T>(this IObservable<T> observable)
        {
            return observable.Select(x => Unit.Default);
        }

        public static ReactiveCommand<object> ToCommand(this IObservable<bool> canExecute)
        {
            return ReactiveCommand.Create(canExecute);
        }
    }
}