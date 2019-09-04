using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace CrossPlatformLiveData.Test
{
    [TestClass]
    public class LifecycleManagerTest
    {
        private ILifecycleManager _lifecycleManager;
        private Mock<ILiveData<string>> _liveDataMock;

        [TestInitialize]
        public void SetUp()
        {
            _lifecycleManager = new LifecycleManager();

            _liveDataMock = new Mock<ILiveData<string>>();
            _liveDataMock.Setup(liveData => liveData.Subscribe(
                    It.IsAny<Action<string>>(), It.IsAny<Action<Exception>>(), It.IsAny<Action>()))
                .Returns(new Mock<IDisposable>().Object);
        }

        [TestCleanup]
        public void CleanUp()
        {
            _lifecycleManager.Dispose();
        }

        /// <summary>
        /// Test correct subscribe behaviour with simulated UI lifecycle
        /// </summary>
        [TestMethod]
        public void LifecycleTest()
        {
            _lifecycleManager.Register(_liveDataMock.Object, OnNextMock, OnErrorMock);

            _liveDataMock.Verify(liveData => liveData.Subscribe(
                OnNextMock, OnErrorMock, It.IsAny<Action>()), Times.Never);

            _lifecycleManager.OnResume();

            _liveDataMock.Verify(liveData => liveData.Subscribe(
                OnNextMock, OnErrorMock, It.IsAny<Action>()), Times.Once);

            _lifecycleManager.OnPause();

            _liveDataMock.Verify(liveData => liveData.Subscribe(
                OnNextMock, OnErrorMock, It.IsAny<Action>()), Times.Once);

            _lifecycleManager.OnResume();

            _liveDataMock.Verify(liveData => liveData.Subscribe(
                OnNextMock, OnErrorMock, It.IsAny<Action>()), Times.Exactly(2));

            _lifecycleManager.OnPause();

            _liveDataMock.Verify(liveData => liveData.Subscribe(
                OnNextMock, OnErrorMock, It.IsAny<Action>()), Times.Exactly(2));

            _lifecycleManager.OnDestroyView();

            _liveDataMock.Verify(liveData => liveData.Subscribe(
                OnNextMock, OnErrorMock, It.IsAny<Action>()), Times.Exactly(2));

            _lifecycleManager.OnResume();

            _liveDataMock.Verify(liveData => liveData.Subscribe(
                OnNextMock, OnErrorMock, It.IsAny<Action>()), Times.Exactly(2));

            _lifecycleManager.Register(_liveDataMock.Object, OnNextMock, OnErrorMock);

            _liveDataMock.Verify(liveData => liveData.Subscribe(
                OnNextMock, OnErrorMock, It.IsAny<Action>()), Times.Exactly(2));

            _lifecycleManager.OnPause();

            _liveDataMock.Verify(liveData => liveData.Subscribe(
                OnNextMock, OnErrorMock, It.IsAny<Action>()), Times.Exactly(2));

            _lifecycleManager.OnResume();

            _liveDataMock.Verify(liveData => liveData.Subscribe(
                OnNextMock, OnErrorMock, It.IsAny<Action>()), Times.Exactly(3));

            _lifecycleManager.Dispose();

            _liveDataMock.Verify(liveData => liveData.Subscribe(
                OnNextMock, OnErrorMock, It.IsAny<Action>()), Times.Exactly(3));

            _liveDataMock.Verify(liveData => liveData.PostValue(It.IsAny<string>()), Times.Never);
        }

        /// <summary>
        /// Test if LifecycleManager manages multiple LiveData instances,
        /// all instances will be subscribed and unsubscribed on lifecycle events
        /// </summary>
        [TestMethod]
        public void MultipleLiveDataTest()
        {
            var mockNumber = new Random().Next(100);
            var liveDataMocks = new List<Mock<ILiveData<string>>>(mockNumber);
            for (var i = 0; i < mockNumber; ++i)
            {
                var liveDataMock = new Mock<ILiveData<string>>();
                liveDataMock.Setup(liveData => liveData.Subscribe(
                        It.IsAny<Action<string>>(), It.IsAny<Action<Exception>>(), It.IsAny<Action>()))
                    .Returns(new Mock<IDisposable>().Object);

                liveDataMocks.Add(liveDataMock);
            }

            foreach (var mock in liveDataMocks)
            {
                _lifecycleManager.Register(mock.Object, OnNextMock, OnErrorMock);
            }

            foreach (var mock in liveDataMocks)
            {
                mock.Verify(liveData => liveData.Subscribe(
                    OnNextMock, OnErrorMock, It.IsAny<Action>()), Times.Never);
            }

            _lifecycleManager.OnResume();

            foreach (var mock in liveDataMocks)
            {
                mock.Verify(liveData => liveData.Subscribe(
                    OnNextMock, OnErrorMock, It.IsAny<Action>()), Times.Once);
            }

            _lifecycleManager.OnPause();

            foreach (var mock in liveDataMocks)
            {
                mock.Verify(liveData => liveData.Subscribe(
                    OnNextMock, OnErrorMock, It.IsAny<Action>()), Times.Once);
            }

            _lifecycleManager.OnResume();

            foreach (var mock in liveDataMocks)
            {
                mock.Verify(liveData => liveData.Subscribe(
                    OnNextMock, OnErrorMock, It.IsAny<Action>()), Times.Exactly(2));
            }

            _lifecycleManager.OnPause();

            foreach (var mock in liveDataMocks)
            {
                mock.Verify(liveData => liveData.Subscribe(
                    OnNextMock, OnErrorMock, It.IsAny<Action>()), Times.Exactly(2));
            }

            _lifecycleManager.OnDestroyView();

            foreach (var mock in liveDataMocks)
            {
                mock.Verify(liveData => liveData.Subscribe(
                    OnNextMock, OnErrorMock, It.IsAny<Action>()), Times.Exactly(2));
            }

            _lifecycleManager.Dispose();

            foreach (var mock in liveDataMocks)
            {
                mock.Verify(liveData => liveData.Subscribe(
                    OnNextMock, OnErrorMock, It.IsAny<Action>()), Times.Exactly(2));
            }
        }

        private void OnNextMock(string obj) { }
        private void OnErrorMock(Exception exception) => Assert.Fail("onError called");
    }
}
