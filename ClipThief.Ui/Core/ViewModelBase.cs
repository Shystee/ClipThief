using System;
using System.Reactive.Linq;

using ClipThief.Ui.Extensions;
using ClipThief.Ui.Services;

namespace ClipThief.Ui.Core
{
    public class ViewModelBase : ReactiveObject
    {
        public ViewModelBase()
        {
            Observable.Return(this)
                      .SelectMany(x => CultureService.CultureChanged.Skip(1), (x, y) => x)
                      .ActivateGestures()
                      .Subscribe(x => x.OnPropertyChanged())
                      .DisposeWith(this);
        }
    }
}