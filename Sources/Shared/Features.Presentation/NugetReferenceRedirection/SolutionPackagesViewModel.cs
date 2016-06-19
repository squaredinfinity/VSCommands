using SquaredInfinity.Foundation.Presentation.ViewModels;
using SquaredInfinity.Foundation.Extensions;
using SquaredInfinity.VSCommands.Foundation.VisualStudioEvents;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using SquaredInfinity.Foundation.Collections;

namespace SquaredInfinity.VSCommands.Features.NugetReferenceRedirection
{
    public class SolutionPackagesViewModel : ViewModel
    {
        readonly IVisualStudioEventsService VisualStudioEventsService;
        readonly INugetReferenceRedirectionService NugetReferenceRedirectionService;
        readonly IServiceProvider ServiceProvider;

        CompositeDisposable Subscriptions = new CompositeDisposable();
        public ObservableCollectionEx<NugetPackage> AllPackages { get; private set; } = new ObservableCollectionEx<NugetPackage>();

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
                .WeakSubscribe(this, (target, args) => target.RefreshPackages()));

            Subscriptions.Add(
                Observable.FromEventPattern<EventArgs>(VisualStudioEventsService, nameof(VisualStudioEventsService.AfterSolutionOpened))
                .WeakSubscribe(this, (target, args) => target.RefreshPackages()));
        }

        void RefreshPackages()
        {
            var dte2 = ServiceProvider.GetDte2();

            var solution = dte2.Solution;

            if (solution == null || solution.FullName.IsNullOrEmpty())
                CleanUp();

            var solution_dir = new DirectoryInfo(Path.GetDirectoryName(solution.FullName));

            if (!solution_dir.Exists)
            {
                CleanUp();
            }
            else
            {
                var all_packages = NugetReferenceRedirectionService.GetAllUsedPackages(solution_dir);
                AllPackages.Reset(all_packages);
            }
        }

        void CleanUp()
        {
            AllPackages.Clear();
        }

        #region Filtering

        bool _filterChanged = false;
        public bool FilterChanged
        {
            get { return _filterChanged; }
            set { RaisePropertyChanged(nameof(FilterChanged)); }
        }

        void RaiseFilterChanged()
        {
            FilterChanged = true;
        }

        string _filterText = "";
        public string FilterText
        {
            get { return _filterText; }
            set
            {
                if(TrySetThisPropertyValue(ref _filterText, value))
                    RaiseFilterChanged();
            }
        }

        public bool FilterPackage(NugetPackage package)
        {
            return FilterPackage(package, FilterText);
        }

        bool FilterPackage(NugetPackage package, string filterText)
        {
            //# Contains COMPLETE filter text
            if(package.Id.Contains(filterText, StringComparison.InvariantCultureIgnoreCase))
            {
                return true;
            }

            //# Contains ALL filter terms

            var filter_terms = filterText.Split(' ');

            // assume true unless proven otherwise
            bool contains_all = true;

            foreach(var term in filter_terms)
            {
                if(!package.Id.Contains(term, StringComparison.InvariantCultureIgnoreCase))
                {
                    contains_all = false;
                    break;
                }
            }

            return contains_all;
        }

        #endregion
    }

    public static class IObservableExtensions
    {
        /// <summary>
        /// Creates a weak subscription.
        /// Unlike default Subscribe method, this will not keep the target alive (unless it is included in clousure of onNext method)
        /// </summary>
        /// <typeparam name="TItem"></typeparam>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="observable"></param>
        /// <param name="target">Target of the subscription. When target is collected the subscription will be disposed.</param>
        /// <param name="onNext"> </param>
        /// <returns></returns>
        public static IDisposable WeakSubscribe<TItem, TTarget>(this IObservable<TItem> observable, TTarget target, Action<TTarget, TItem> onNext) where TTarget : class
        {
            // keep weak reference to the target
            var reference = new WeakReference(target);

            // if onNext is an instance method then there's a possiblity it will hold reference to the target (if it's instance on the target object)
            // just to be sure and avoid confusion all instance methods are disallowed
            // only explicitly static methods or methods with static implementation (e.g. anonymous lambdas) are allowed.
            // note that user of this method must take extra care not to create a closure over target itself
            
            if (onNext.Target != null && object.ReferenceEquals(onNext.Target, target))
                throw new ArgumentException("onNext action cannot be an instance method on target. Use static method or lambda expression (t,i) => t.method()");

            var subscription = (IDisposable)null;

            subscription =
                observable.Subscribe(item =>
                {
                    var currentTarget = reference.Target as TTarget;

                    if (currentTarget != null)
                        onNext(currentTarget, item);
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
