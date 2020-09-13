using System;
using System.Reactive.Subjects;

namespace ClipThief.Ui
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

        public void Post(IRoutableViewModel viewModel)
        {
            show.OnNext(viewModel);
        }

        public IObservable<IRoutableViewModel> Show => show;
    }
}