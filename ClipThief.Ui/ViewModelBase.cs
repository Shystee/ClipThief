using System;
using System.Reactive.Linq;

namespace ClipThief.Ui
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