﻿using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;

using ClipThief.Ui.Extensions;

namespace ClipThief.Ui.Core
{
    public abstract class DisposableObject : IDisposable
    {
        private readonly CompositeDisposable disposable;

        private readonly string disposeMessage;

        protected DisposableObject()
        {
            disposable = new CompositeDisposable();
            disposeMessage = string.Intern("Dispose - " + GetType().Name);
        }

        protected bool SuppressDebugWriteline { get; set; }

        public static implicit operator CompositeDisposable(DisposableObject disposable)
        {
            return disposable.disposable;
        }

        public virtual void Dispose()
        {
            if (SuppressDebugWriteline)
            {
                disposable.Dispose();
            }
            else
            {
                disposable.Dispose();
            }
        }

        protected void DisposeOfAsync(IEnumerable<IDisposable> disposables, IScheduler scheduler)
        {
            Observable.Return(disposables, scheduler).Subscribe(x => x.ForEach(y => y.Dispose())).DisposeWith(this);
        }
    }
}