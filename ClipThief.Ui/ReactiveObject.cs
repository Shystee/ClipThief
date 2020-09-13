using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;

namespace ClipThief.Ui
{
    public abstract class ReactiveObject : DisposableObject, INotifyPropertyChanged
    {
        private static readonly IDictionary<string, PropertyChangedEventArgs> ChangedProperties =
            new Dictionary<string, PropertyChangedEventArgs>();

        private static readonly PropertyChangedEventArgs EmptyChangeArgs = new PropertyChangedEventArgs(string.Empty);

        private SuspendedNotifications suspendedNotifications;

        protected ReactiveObject()
        {
            Observable.Return(this)
                      .SelectMany(x => CultureService.CultureChanged.Skip(1), (x, y) => x)
                      .ActivateGestures()
                      .Subscribe(x => x.OnPropertyChanged())
                      .DisposeWith(this);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public IDisposable SuspendNotifications()
        {
            if (suspendedNotifications == null)
            {
                suspendedNotifications = new SuspendedNotifications(this);
            }

            return suspendedNotifications.AddRef();
        }

        protected virtual void OnPropertyChanged()
        {
            OnPropertyChanged(string.Empty);
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (suspendedNotifications != null)
            {
                suspendedNotifications.Add(propertyName);
            }
            else
            {
                var handler = PropertyChanged;

                if (handler != null)
                {
                    if (string.IsNullOrEmpty(propertyName))
                    {
                        handler(this, EmptyChangeArgs);
                    }
                    else
                    {
                        if (!ChangedProperties.TryGetValue(propertyName, out var args))
                        {
                            args = new PropertyChangedEventArgs(propertyName);
                            ChangedProperties.Add(propertyName, args);
                        }

                        handler(this, args);
                    }
                }
            }
        }

        protected virtual bool SetProperty<T>(
            ref T existingValue,
            T newValue,
            [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(existingValue, newValue))
            {
                return false;
            }

            existingValue = newValue;
            OnPropertyChanged(propertyName);

            return true;
        }

        protected virtual void SetPropertyAndNotify<T>(
            ref T existingValue,
            T newValue,
            [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(existingValue, newValue))
            {
                return;
            }

            existingValue = newValue;
            OnPropertyChanged(propertyName);
        }

        private sealed class SuspendedNotifications : IDisposable
        {
            private readonly Counter counter;

            private readonly HashSet<string> properties = new HashSet<string>();

            private readonly ReactiveObject target;

            public SuspendedNotifications(ReactiveObject target)
            {
                this.target = target;

                counter = new Counter(Dispose);
            }

            public void Dispose()
            {
                target.suspendedNotifications = null;

                foreach (var property in properties) target.OnPropertyChanged(property);
            }

            public void Add(string propertyName)
            {
                properties.Add(propertyName);
            }

            public IDisposable AddRef()
            {
                counter.Increment();

                return counter;
            }

            // Using an internal class to avoid using closure which would cases the creation of a class
            // under the covers at runtime - gives better performance for this base class...
            private sealed class Counter : IDisposable
            {
                private readonly Action dispose;

                private int refCount;

                public Counter(Action dispose)
                {
                    this.dispose = dispose;
                }

                public void Dispose()
                {
                    if (--refCount == 0)
                    {
                        dispose();
                    }
                }

                public void Increment()
                {
                    ++refCount;
                }
            }
        }
    }
}