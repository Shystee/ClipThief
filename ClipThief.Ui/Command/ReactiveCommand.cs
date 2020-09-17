using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows.Input;

using ClipThief.Ui.Extensions;

namespace ClipThief.Ui.Command
{
    public sealed class ReactiveCommand : ReactiveCommand<object>
    {
        private ReactiveCommand(IObservable<bool> canExecute)
            : base(canExecute.StartWith(Constants.StartsWith.Boolean.False))
        {
        }

        public static new ReactiveCommand<object> Create()
        {
            return ReactiveCommand<object>.Create(Observable.Return(true));
        }

        public static new ReactiveCommand<object> Create(IObservable<bool> canExecute)
        {
            return ReactiveCommand<object>.Create(canExecute);
        }
    }

    public class ReactiveCommand<T> : IObservable<T>, ICommand, IDisposable
    {
        private readonly IDisposable canDisposable;

        private readonly List<EventHandler> eventHandlers;

        private readonly Subject<T> execute;

        private bool currentCanExecute;

        protected ReactiveCommand(IObservable<bool> canExecute)
        {
            eventHandlers = new List<EventHandler>(8);

            canDisposable = canExecute.Subscribe(
                                                 x =>
                                                     {
                                                         currentCanExecute = x;
                                                         CommandManager.InvalidateRequerySuggested();
                                                     });

            execute = new Subject<T>();
        }

        public event EventHandler CanExecuteChanged
        {
            add
            {
                eventHandlers.Add(value);
                CommandManager.RequerySuggested += value;
            }

            remove
            {
                eventHandlers.Remove(value);
                CommandManager.RequerySuggested -= value;
            }
        }

        public static ReactiveCommand<T> Create()
        {
            return new ReactiveCommand<T>(Observable.Return(true));
        }

        public static ReactiveCommand<T> Create(IObservable<bool> canExecute)
        {
            return new ReactiveCommand<T>(canExecute);
        }

        public virtual bool CanExecute(object parameter)
        {
            return currentCanExecute;
        }

        public void Dispose()
        {
            eventHandlers.ForEach(x => CommandManager.RequerySuggested -= x);
            eventHandlers.Clear();

            canDisposable.Dispose();

            execute.OnCompleted();
            execute.Dispose();
        }

        public virtual void Execute(object parameter)
        {
            var typedParameter = parameter is T o ? o : default;

            if (CanExecute(typedParameter))
            {
                execute.OnNext(typedParameter);
            }
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            return execute.ActivateGestures().Subscribe(observer.OnNext, observer.OnError, observer.OnCompleted);
        }
    }
}