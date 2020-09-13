using System;
using System.Reactive.Disposables;

namespace Wpf.Reactive.Learning
{
    public static class CompositeDisposableExtensions
    {
        public static T DisposeWith<T>(this T instance, CompositeDisposable disposable) where T : IDisposable
        {
            disposable.Add(instance);

            return instance;
        }
    }
}