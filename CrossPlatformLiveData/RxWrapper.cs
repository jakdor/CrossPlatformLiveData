using System;

namespace CrossPlatformLiveData
{
    /// <summary>
    /// Rx event wrapper template - optional model class bundling data, status and exception in single response.
    /// </summary>
    public class RxWrapper<T>
    {
        public Enum Status { get; set; }
        public T Data { get; set; }
        public Exception Exception { get; set; }

        public RxWrapper(T data)
        {
            Status = RxStatus.Ok;
            Data = data;
        }

        public RxWrapper(Enum status)
        {
            Status = status;
        }

        public RxWrapper(Enum status, T data)
        {
            Status = status;
            Data = data;
        }

        public RxWrapper(Exception e)
        {
            Status = RxStatus.Error;
            Exception = e;
        }

        public static RxWrapper<T> Ok(T data) => new RxWrapper<T>(data);
        public static RxWrapper<T> Pending() => new RxWrapper<T>(RxStatus.Pending);
        public static RxWrapper<T> Error(Exception e) => new RxWrapper<T>(e);
    }

    public enum RxStatus
    {
        Ok, Pending, Error
    }
}
