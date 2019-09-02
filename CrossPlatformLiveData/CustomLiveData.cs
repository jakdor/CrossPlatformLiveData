using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using CrossPlatformLiveData.Internal.Facade;

namespace CrossPlatformLiveData
{
    /// <inheritdoc />
    /// <summary>
    /// Custom cross-platform LiveData impl
    /// </summary>
    /// <typeparam name="T">Type</typeparam>
    public class CustomLiveData<T> : ICustomLiveData<T>
    {
        private readonly BehaviorSubject<T> _subject;
        private readonly IRxSchedulersFacade _rxSchedulers = new RxSchedulersFacade();
        private readonly bool _reEmitOnLifecycleFlag;
        private T _lastEmitted;

        public T Value { get; private set; }

        /// <summary>
        /// First emitted value will be type default
        /// </summary>
        /// <param name="reEmitOnLifecycle">If set value will be always remitted on resume</param>
        public CustomLiveData(bool reEmitOnLifecycle = false)
        {
            _subject = new BehaviorSubject<T>(default(T));
            _reEmitOnLifecycleFlag = reEmitOnLifecycle;
        }

        /// <summary>
        /// Emit first value with provided default
        /// </summary>
        /// <param name="initValue">Initial emitted value</param>
        /// <param name="reEmitOnLifecycle">If set value will be always remitted on resume</param>
        public CustomLiveData(T initValue, bool reEmitOnLifecycle = false)
        {
            _subject = new BehaviorSubject<T>(initValue);
            _reEmitOnLifecycleFlag = reEmitOnLifecycle;
        }

        /// <summary>
        /// Emit first value with provided default
        /// </summary>
        /// <param name="initValue">Initial emitted value</param>
        /// <param name="rxSchedulers">Use custom IRxSchedulersFacade implementation</param>
        /// <param name="reEmitOnLifecycle">If set value will be always remitted on resume</param>
        public CustomLiveData(T initValue, IRxSchedulersFacade rxSchedulers, bool reEmitOnLifecycle = false)
        {
            _subject = new BehaviorSubject<T>(initValue);
            _rxSchedulers = rxSchedulers;
            _reEmitOnLifecycleFlag = reEmitOnLifecycle;
        }

        /// <summary>
        /// Post new value asynchronously
        /// </summary>
        /// <param name="value"></param>
        public void PostValue(T value)
        {
            _subject.OnNext(value);
        }

        /// <summary>
        /// Subscribes to LiveData,
        /// if reEmitOnLifecycle is false only emits new value if not equal to last one
        /// </summary>
        /// <param name="onNext">Emits only non null objects</param>
        /// <param name="onError"></param>
        /// <param name="onCompleted"></param>
        /// <returns>IDisposable</returns>
        public IDisposable Subscribe(Action<T> onNext, Action<Exception> onError, Action onCompleted)
        {
            return _subject.SubscribeOn(_rxSchedulers.Io())
                .ObserveOn(_rxSchedulers.Ui())
                .Subscribe(obj =>
                {
                    if (obj != null && _reEmitOnLifecycleFlag)
                    {
                        Value = obj;
                        onNext.Invoke(obj);
                    }
                    else
                    {
                        if (obj != null && !obj.Equals(_lastEmitted))
                        {
                            Value = obj;
                            onNext.Invoke(obj);
                        }
                        _lastEmitted = obj;
                    }
                }, onError, onCompleted);
        }

        /// <summary>
        /// Observe with lifecycle managed by ILifecycleManager
        /// </summary>
        /// <param name="lifecycleManager">lifecycle manager</param>
        /// <param name="onNext"></param>
        /// <param name="onError"></param>
        public void Observe(ILifecycleManager lifecycleManager, Action<T> onNext, Action<Exception> onError)
        {
            lifecycleManager.Register(this, onNext, onError);
        }
    }
}
