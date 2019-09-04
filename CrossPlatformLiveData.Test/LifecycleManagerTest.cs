using System;
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

        private void OnNextMock(string obj) { }
        private void OnErrorMock(Exception exception) => Assert.Fail("onError called");
    }
}
