using System;
using System.Reactive.Subjects;

using ClipThief.Ui.Core;
using ClipThief.Ui.Extensions;
using ClipThief.Ui.ViewModels;

namespace ClipThief.Ui.Services
{
    public interface IApplicationService
    {
        IObservable<IRoutableViewModel> Show { get; }

        void Post(IRoutableViewModel viewModel);
    }

    public sealed class ApplicationService : DisposableObject, IApplicationService
    {
        private readonly Subject<IRoutableViewModel> show;

        public ApplicationService()
        {
            show = new Subject<IRoutableViewModel>().DisposeWith(this);
        }

        public IObservable<IRoutableViewModel> Show => show;

        public void Post(IRoutableViewModel viewModel)
        {
            show.OnNext(viewModel);
        }
    }
}