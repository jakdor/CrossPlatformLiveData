using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using XFTests.Utils;

namespace CrossPlatformLiveData.Test
{
    [TestClass]
    public class LiveDataTest
    {
        private ILiveData<int> _liveDataValueType;
        private ILiveData<string> _liveDataReferenceType;
        private Mock<ILifecycleManager> _lifecycleManagerMock;
        private Mock<IRxSchedulersFacade> _testRxSchedulerFacade;

        private readonly IList<int> _emittedVerificationListValue = new List<int>();
        private readonly IList<string> _emittedVerificationListReference = new List<string>();
        private IDisposable _singleDisposable;

        [TestInitialize]
        public void SetUp()
        {
            _lifecycleManagerMock = new Mock<ILifecycleManager>();
            _testRxSchedulerFacade = new Mock<IRxSchedulersFacade>();
            _testRxSchedulerFacade.Setup(facade => facade.Ui()).Returns(CurrentThreadScheduler.Instance);
            _testRxSchedulerFacade.Setup(facade => facade.Io()).Returns(CurrentThreadScheduler.Instance);
        }

        [TestCleanup]
        public void CleanUp()
        {
            _singleDisposable?.Dispose();
        }

        /// <summary>
        /// Test LifecycleManager gets correctly registered to provided LiveData instance
        /// </summary>
        [TestMethod]
        public void ObserveTest()
        {
            _liveDataValueType = new LiveData<int>();
            Action<int> onNextMock = i => { };
            Action<Exception> onErrorMock = exception => { }; 

            _liveDataValueType.Observe(_lifecycleManagerMock.Object, onNextMock, onErrorMock);

            _lifecycleManagerMock.Verify(manager => manager.Register(_liveDataValueType, onNextMock, onErrorMock), Times.Once());
        }

        /// <summary>
        /// Test initial value not emitted if type default, value type
        /// </summary>
        [TestMethod]
        public void InitValueIgnoreTypeDefaultValueTypeTest()
        {
            _liveDataValueType = new LiveData<int>(default(int), _testRxSchedulerFacade.Object);

            _singleDisposable = _liveDataValueType.Subscribe(OnNextMock, OnErrorMock, OnCompletedMock);

            Assert.AreEqual(0, _emittedVerificationListValue.Count);
        }

        /// <summary>
        /// Test initial value not emitted if type default, reference type
        /// </summary>
        [TestMethod]
        public void InitValueIgnoreTypeDefaultReferenceTypeTest()
        {
            _liveDataReferenceType = new LiveData<string>(default(string), _testRxSchedulerFacade.Object);

            _singleDisposable = _liveDataReferenceType.Subscribe(OnNextMock, OnErrorMock, OnCompletedMock);

            Assert.AreEqual(0, _emittedVerificationListValue.Count);
        }

        /// <summary>
        /// Test initial value, value type
        /// </summary>
        [TestMethod]
        public void InitValueValueTypeTest()
        {
            var randomGen = new Random();
            var val0 = randomGen.Next();
            _liveDataValueType = new LiveData<int>(val0, _testRxSchedulerFacade.Object);

            _singleDisposable = _liveDataValueType.Subscribe(OnNextMock, OnErrorMock, OnCompletedMock);

            Assert.AreEqual(1, _emittedVerificationListValue.Count);
            Assert.AreEqual(val0, _emittedVerificationListValue[0]);
        }

        /// <summary>
        /// Test initial value, reference type
        /// </summary>
        [TestMethod]
        public void InitValueReferenceTypeTest()
        {
            var val0 = TestUtils.RandomString(32);
            _liveDataReferenceType = new LiveData<string>(val0, _testRxSchedulerFacade.Object);

            _singleDisposable = _liveDataReferenceType.Subscribe(OnNextMock, OnErrorMock, OnCompletedMock);

            Assert.AreEqual(1, _emittedVerificationListReference.Count);
            Assert.AreEqual(val0, _emittedVerificationListReference[0]);
        }

        /// <summary>
        /// Test default value type, default settings
        /// allowDuplicatesInSequence = false
        /// </summary>
        [TestMethod]
        public void SubscribeValueTypeDefaultSettingsTest()
        {
            var randomGen = new Random();
            var val0 = randomGen.Next();
            var val1 = randomGen.Next();
            var val2 = randomGen.Next();
            _liveDataValueType = new LiveData<int>(val0, _testRxSchedulerFacade.Object);

            _singleDisposable = _liveDataValueType.Subscribe(OnNextMock, OnErrorMock, OnCompletedMock);
            var testSequence = new List<int>(new []{ val1, val2, val2, val1, val2, val1, val1});
            testSequence.ForEach(i => _liveDataValueType.PostValue(i));

            Assert.AreEqual(6, _emittedVerificationListValue.Count);
            Assert.AreEqual(val0, _emittedVerificationListValue[0]);
            Assert.AreEqual(val1, _emittedVerificationListValue[1]);
            Assert.AreEqual(val2, _emittedVerificationListValue[2]);
            Assert.AreEqual(val1, _emittedVerificationListValue[3]);
            Assert.AreEqual(val2, _emittedVerificationListValue[4]);
            Assert.AreEqual(val1, _emittedVerificationListValue[5]);
        }

        /// <summary>
        /// Test default value type, duplicates in sequence allowed
        /// allowDuplicatesInSequence = true
        /// </summary>
        [TestMethod]
        public void SubscribeValueTypeAllowDuplicatesTest()
        {
            var randomGen = new Random();
            var val0 = randomGen.Next();
            var val1 = randomGen.Next();
            var val2 = randomGen.Next();
            _liveDataValueType = new LiveData<int>(val0, _testRxSchedulerFacade.Object, true);

            _singleDisposable = _liveDataValueType.Subscribe(OnNextMock, OnErrorMock, OnCompletedMock);
            var testSequence = new List<int>(new[] { val1, val2, val2, val1, val2, val1, val1 });
            testSequence.ForEach(i => _liveDataValueType.PostValue(i));

            Assert.AreEqual(8, _emittedVerificationListValue.Count);
            Assert.AreEqual(val0, _emittedVerificationListValue[0]);
            for (var i = 1; i < _emittedVerificationListValue.Count; ++i)
            {
                Assert.AreEqual(testSequence[i - 1], _emittedVerificationListValue[i]);
            }
        }

        /// <summary>
        /// Test default reference type, default settings
        /// allowDuplicatesInSequence = false
        /// </summary>
        [TestMethod]
        public void SubscribeReferenceTypeDefaultSettingsTest()
        {
            var val0 = TestUtils.RandomString(32);
            var val1 = TestUtils.RandomString(32);
            var val2 = TestUtils.RandomString(32);
            _liveDataReferenceType = new LiveData<string>(val0, _testRxSchedulerFacade.Object);

            _singleDisposable = _liveDataReferenceType.Subscribe(OnNextMock, OnErrorMock, OnCompletedMock);
            var testSequence = new List<string>(new[] { val1, val2, val2, val1, val2, val1, val1 });
            testSequence.ForEach(s => _liveDataReferenceType.PostValue(s));

            Assert.AreEqual(6, _emittedVerificationListReference.Count);
            Assert.AreEqual(val0, _emittedVerificationListReference[0]);
            Assert.AreEqual(val1, _emittedVerificationListReference[1]);
            Assert.AreEqual(val2, _emittedVerificationListReference[2]);
            Assert.AreEqual(val1, _emittedVerificationListReference[3]);
            Assert.AreEqual(val2, _emittedVerificationListReference[4]);
            Assert.AreEqual(val1, _emittedVerificationListReference[5]);
        }

        /// <summary>
        /// Test default reference type, duplicates in sequence allowed
        /// allowDuplicatesInSequence = true
        /// </summary>
        [TestMethod]
        public void SubscribeReferenceTypeAllowDuplicatesTest()
        {
            var val0 = TestUtils.RandomString(32);
            var val1 = TestUtils.RandomString(32);
            var val2 = TestUtils.RandomString(32);
            _liveDataReferenceType = new LiveData<string>(val0, _testRxSchedulerFacade.Object, true);

            _singleDisposable = _liveDataReferenceType.Subscribe(OnNextMock, OnErrorMock, OnCompletedMock);
            var testSequence = new List<string>(new[] { val1, val2, val2, val1, val2, val1, val1 });
            testSequence.ForEach(s => _liveDataReferenceType.PostValue(s));

            Assert.AreEqual(8, _emittedVerificationListReference.Count);
            Assert.AreEqual(val0, _emittedVerificationListReference[0]);
            for (var i = 1; i < _emittedVerificationListReference.Count; ++i)
            {
                Assert.AreEqual(testSequence[i - 1], _emittedVerificationListReference[i]);
            }
        }

        /// <summary>
        /// Test if onResume (resubscribe) doesn't cause last value re-emission with default settings
        /// allowDuplicatesInSequence = false
        /// </summary>
        [TestMethod]
        public void SubscribeOnResumeDefaultSettingsNoReEmissionTest()
        {
            var val0 = TestUtils.RandomString(32);
            var val1 = TestUtils.RandomString(32);
            var val2 = TestUtils.RandomString(32);
            _liveDataReferenceType = new LiveData<string>(val0, _testRxSchedulerFacade.Object);

            _singleDisposable = _liveDataReferenceType.Subscribe(OnNextMock, OnErrorMock, OnCompletedMock);
            var testSequence = new List<string>(new[] { val1, val2, val2, val1, val2, val1, val1 });
            _liveDataReferenceType.PostValue(testSequence[0]);
            _liveDataReferenceType.PostValue(testSequence[1]);
            _liveDataReferenceType.PostValue(testSequence[2]);
            _singleDisposable.Dispose();
            _singleDisposable = _liveDataReferenceType.Subscribe(OnNextMock, OnErrorMock, OnCompletedMock);
            _liveDataReferenceType.PostValue(testSequence[3]);
            _liveDataReferenceType.PostValue(testSequence[4]);
            _singleDisposable.Dispose();
            _singleDisposable = _liveDataReferenceType.Subscribe(OnNextMock, OnErrorMock, OnCompletedMock);
            _liveDataReferenceType.PostValue(testSequence[5]);
            _liveDataReferenceType.PostValue(testSequence[6]);

            Assert.AreEqual(6, _emittedVerificationListReference.Count);
            Assert.AreEqual(val0, _emittedVerificationListReference[0]);
            Assert.AreEqual(val1, _emittedVerificationListReference[1]);
            Assert.AreEqual(val2, _emittedVerificationListReference[2]);
            Assert.AreEqual(val1, _emittedVerificationListReference[3]);
            Assert.AreEqual(val2, _emittedVerificationListReference[4]);
            Assert.AreEqual(val1, _emittedVerificationListReference[5]);
        }

        /// <summary>
        /// Test if onResume (resubscribe) will trigger last value re-emission if duplicates are allowed
        /// allowDuplicatesInSequence = true
        /// </summary>
        [TestMethod]
        public void SubscribeOnResumeAllowDuplicatesReEmissionTest()
        {
            var val0 = TestUtils.RandomString(32);
            var val1 = TestUtils.RandomString(32);
            var val2 = TestUtils.RandomString(32);
            _liveDataReferenceType = new LiveData<string>(val0, _testRxSchedulerFacade.Object, true);

            _singleDisposable = _liveDataReferenceType.Subscribe(OnNextMock, OnErrorMock, OnCompletedMock);
            var testSequence = new List<string>(new[] { val1, val2, val2, val1, val2, val1, val1 });
            _liveDataReferenceType.PostValue(testSequence[0]);
            _liveDataReferenceType.PostValue(testSequence[1]);
            _liveDataReferenceType.PostValue(testSequence[2]);
            _singleDisposable.Dispose();
            _singleDisposable = _liveDataReferenceType.Subscribe(OnNextMock, OnErrorMock, OnCompletedMock);
            _liveDataReferenceType.PostValue(testSequence[3]);
            _liveDataReferenceType.PostValue(testSequence[4]);
            _singleDisposable.Dispose();
            _singleDisposable = _liveDataReferenceType.Subscribe(OnNextMock, OnErrorMock, OnCompletedMock);
            _liveDataReferenceType.PostValue(testSequence[5]);
            _liveDataReferenceType.PostValue(testSequence[6]);

            Assert.AreEqual(10, _emittedVerificationListReference.Count);
            Assert.AreEqual(val0, _emittedVerificationListReference[0]);
            Assert.AreEqual(val1, _emittedVerificationListReference[1]);
            Assert.AreEqual(val2, _emittedVerificationListReference[2]);
            Assert.AreEqual(val2, _emittedVerificationListReference[3]);
            Assert.AreEqual(val2, _emittedVerificationListReference[4]);
            Assert.AreEqual(val1, _emittedVerificationListReference[5]);
            Assert.AreEqual(val2, _emittedVerificationListReference[6]);
            Assert.AreEqual(val2, _emittedVerificationListReference[7]);
            Assert.AreEqual(val1, _emittedVerificationListReference[8]);
            Assert.AreEqual(val1, _emittedVerificationListReference[9]);
        }

        private void OnNextMock(int i) => _emittedVerificationListValue.Add(i);
        private void OnNextMock(string obj) => _emittedVerificationListReference.Add(obj);
        private void OnErrorMock(Exception exception) => Assert.Fail("onError called");
        private void OnCompletedMock() { }
    }
}
