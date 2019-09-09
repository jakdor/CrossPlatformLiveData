using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using XFTests.Utils;

namespace CrossPlatformLiveData.Test
{
    [TestClass]
    public class RxWrapperTest
    {
        private RxWrapper<string> _rxWrapper;

        [TestMethod]
        public void RxWrapperInitTest()
        {
            var testData = TestUtils.RandomString(32);
            var testStatus = (RxStatus)new Random().Next(0, 2);
            var testException = new Exception(TestUtils.RandomString(32));

            _rxWrapper = new RxWrapper<string>(testStatus, testData, testException);

            Assert.AreEqual(testData, _rxWrapper.Data);
            Assert.AreEqual(testStatus, _rxWrapper.Status);
            Assert.AreEqual(testException, _rxWrapper.Exception);

            _rxWrapper = new RxWrapper<string>(testData);

            Assert.AreEqual(testData, _rxWrapper.Data);
            Assert.AreEqual(RxStatus.Ok, _rxWrapper.Status);
            Assert.IsNull(_rxWrapper.Exception);

            _rxWrapper = new RxWrapper<string>(testStatus);

            Assert.IsNull(_rxWrapper.Data);
            Assert.AreEqual(testStatus, _rxWrapper.Status);
            Assert.IsNull(_rxWrapper.Exception);

            _rxWrapper = new RxWrapper<string>(testException);

            Assert.IsNull(_rxWrapper.Data);
            Assert.AreEqual(RxStatus.Error, _rxWrapper.Status);
            Assert.AreEqual(testException, _rxWrapper.Exception);

            _rxWrapper = RxWrapper<string>.Ok(testData);

            Assert.AreEqual(testData, _rxWrapper.Data);
            Assert.AreEqual(RxStatus.Ok, _rxWrapper.Status);
            Assert.IsNull(_rxWrapper.Exception);

            _rxWrapper = RxWrapper<string>.Pending();

            Assert.IsNull(_rxWrapper.Data);
            Assert.AreEqual(RxStatus.Pending, _rxWrapper.Status);
            Assert.IsNull(_rxWrapper.Exception);

            _rxWrapper = RxWrapper<string>.Error(testException);

            Assert.IsNull(_rxWrapper.Data);
            Assert.AreEqual(RxStatus.Error, _rxWrapper.Status);
            Assert.AreEqual(testException, _rxWrapper.Exception);
        }
    }
}
