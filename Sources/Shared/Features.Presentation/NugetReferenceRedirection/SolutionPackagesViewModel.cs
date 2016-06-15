using SquaredInfinity.Foundation.Presentation.ViewModels;
using SquaredInfinity.VSCommands.Foundation.VisualStudioEvents;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading;

namespace SquaredInfinity.VSCommands.Features.NugetReferenceRedirection
{
    public class SolutionPackagesViewModel : ViewModel
    {
        readonly IVisualStudioEventsService VisualStudioEventsService;
        readonly INugetReferenceRedirectionService NugetReferenceRedirectionService;
        readonly IServiceProvider ServiceProvider;

        CompositeDisposable Subscriptions = new CompositeDisposable();

        public SolutionPackagesViewModel(
            IServiceProvider serviceProvider,
            IVisualStudioEventsService vsEventsService,
            INugetReferenceRedirectionService nugetReferenceRedirectionService)
        {
            this.ServiceProvider = serviceProvider;
            this.VisualStudioEventsService = vsEventsService;
            this.NugetReferenceRedirectionService = nugetReferenceRedirectionService;

            Subscriptions.Add(
                Observable.FromEventPattern<EventArgs>(VisualStudioEventsService, nameof(VisualStudioEventsService.AfterSolutionClosed))
                .WeakSubscribe(this, (args) =>
                {
                    RefreshPackages();
                }));

            Subscriptions.Add(
                Observable.FromEventPattern<EventArgs>(VisualStudioEventsService, nameof(VisualStudioEventsService.AfterSolutionOpened))
                .WeakSubscribe(this, (args) =>
                {
                    RefreshPackages();
                }));
        }

        void RefreshPackages()
        {
            var dte2 = ServiceProvider.GetDte2();

            var solution = dte2.Solution;

            var solution_dir = new DirectoryInfo(solution.FullName);

            if (!solution_dir.Exists)
            {

            }
            else
            {
                var a = NugetReferenceRedirectionService.GetAllUsedPackages(solution_dir);
                Console.WriteLine("");
            }
        }
    }

    public static class IObservableExtensions
    {
        /// <summary>
        /// Creates a weak subscription.
        /// Unlike default Subscribe method, this will not keep the target alive (unless it is included in clousure of onNext method)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="observable"></param>
        /// <param name="target"></param>
        /// <param name="onNext"></param>
        /// <returns></returns>
        public static IDisposable WeakSubscribe<T, TTarget>(this IObservable<T> observable, TTarget target, Action<T> onNext) where TTarget : class
        {
            // keep weak reference to the target
            var reference = new WeakReference(target);

            // if onNext is an instance method then there's a possiblity it will hold reference to the target (if it's instance on the target object)
            // just to be sure and avoid confusion all instance methods are disallowed
            // only explicitly static methods or methods with static implementation (e.g. anonymous lambdas) are allowed.
            // note that user of this method must take extra care not to create a closure over target itself
            
            if (onNext.Target != null)
                throw new ArgumentException("onNext action cannot be an instance method. Use static method or lambda expression (t,i) => xxx");

            var subscription = (IDisposable)null;

            subscription =
                observable.Subscribe(item =>
                {
                    var currentTarget = reference.Target as TTarget;

                    if (currentTarget != null)
                        onNext(item);
                    else
                    {
                        // target reference is gone, clean up
                        subscription.Dispose();
                    }
                });

            return subscription;
        }
    }

}
