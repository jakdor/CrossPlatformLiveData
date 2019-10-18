using System;

namespace CrossPlatformLiveData
{
    /// <summary>
    /// Rx event wrapper template - optional model class bundling data, status and exception in single response.
    /// </summary>
    public class RxWrapper<T>
    {
        public RxStatus Status { get; set; }
        public T Data { get; set; }
        public Exception Exception { get; set; }

        public RxWrapper(RxStatus status, T data, Exception e = null)
        {
            Status = status;
            Data = data;
            Exception = e;
        }

        public RxWrapper(T data, bool isUpdate = false)
        {
            Status = isUpdate ? RxStatus.Update : RxStatus.Ok;
            Data = data;
        }

        public RxWrapper(RxStatus status)
        {
            Status = status;
        }

        public RxWrapper(Exception e)
        {
            Status = RxStatus.Error;
            Exception = e;
        }

        /// <summary>
        /// Init state
        /// </summary>
        /// <returns></returns>
        public static RxWrapper<T> NoData() => new RxWrapper<T>(RxStatus.NoData);

        /// <summary>
        /// New data
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static RxWrapper<T> Ok(T data) => new RxWrapper<T>(data);

        /// <summary>
        /// Data request pending
        /// </summary>
        /// <returns></returns>
        public static RxWrapper<T> Pending() => new RxWrapper<T>(RxStatus.Pending);

        /// <summary>
        /// Data update - for example new list items
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static RxWrapper<T> Update(T data) => new RxWrapper<T>(data, true);

        /// <summary>
        /// Data update request pending - use if you want to show different loading indicator
        /// </summary>
        /// <returns></returns>
        public static RxWrapper<T> PendingUpdate() => new RxWrapper<T>(RxStatus.PendingUpdate);

        /// <summary>
        /// Request returned error
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static RxWrapper<T> Error(Exception e) => new RxWrapper<T>(e);
    }

    public enum RxStatus
    {
        NoData, Ok, Pending, Update, PendingUpdate, Error
    }
}
