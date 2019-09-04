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
        private ILiveData<int> _liveDataSimpleType;
        private ILiveData<string> _liveDataObjType;
        private Mock<ILifecycleManager> _lifecycleManagerMock;
        private Mock<IRxSchedulersFacade> _testRxSchedulerFacade;

        private readonly IList<int> _emittedVerificationListSimple = new List<int>();
        private readonly IList<string> _emittedVerificationListObj = new List<string>();
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
            _liveDataSimpleType = new LiveData<int>();
            Action<int> onNextMock = i => { };
            Action<Exception> onErrorMock = exception => { }; 

            _liveDataSimpleType.Observe(_lifecycleManagerMock.Object, onNextMock, onErrorMock);

            _lifecycleManagerMock.Verify(manager => manager.Register(_liveDataSimpleType, onNextMock, onErrorMock), Times.Once());
        }

        /// <summary>
        /// Test initial value not emitted if type default, simple type
        /// </summary>
        [TestMethod]
        public void InitValueIgnoreTypeDefaultSimpleTest()
        {
            _liveDataSimpleType = new LiveData<int>(default(int), _testRxSchedulerFacade.Object);

            _singleDisposable = _liveDataSimpleType.Subscribe(OnNextMock, OnErrorMock, OnCompletedMock);

            Assert.AreEqual(0, _emittedVerificationListSimple.Count);
        }

        /// <summary>
        /// Test initial value not emitted if type default, object type
        /// </summary>
        [TestMethod]
        public void InitValueIgnoreTypeDefaultObjTest()
        {
            _liveDataObjType = new LiveData<string>(default(string), _testRxSchedulerFacade.Object);

            _singleDisposable = _liveDataObjType.Subscribe(OnNextMock, OnErrorMock, OnCompletedMock);

            Assert.AreEqual(0, _emittedVerificationListSimple.Count);
        }

        /// <summary>
        /// Test initial value, simple type
        /// </summary>
        [TestMethod]
        public void InitValueSimpleTest()
        {
            var randomGen = new Random();
            var val0 = randomGen.Next();
            _liveDataSimpleType = new LiveData<int>(val0, _testRxSchedulerFacade.Object);

            _singleDisposable = _liveDataSimpleType.Subscribe(OnNextMock, OnErrorMock, OnCompletedMock);

            Assert.AreEqual(1, _emittedVerificationListSimple.Count);
            Assert.AreEqual(val0, _emittedVerificationListSimple[0]);
        }

        /// <summary>
        /// Test initial value, object type
        /// </summary>
        [TestMethod]
        public void InitValueObjTest()
        {
            var val0 = TestUtils.RandomString(32);
            _liveDataObjType = new LiveData<string>(val0, _testRxSchedulerFacade.Object);

            _singleDisposable = _liveDataObjType.Subscribe(OnNextMock, OnErrorMock, OnCompletedMock);

            Assert.AreEqual(1, _emittedVerificationListObj.Count);
            Assert.AreEqual(val0, _emittedVerificationListObj[0]);
        }

        /// <summary>
        /// Test default simple type, default settings
        /// allowDuplicatesInSequence = false
        /// </summary>
        [TestMethod]
        public void SubscribeSimpleTypeDefaultSettingsTest()
        {
            var randomGen = new Random();
            var val0 = randomGen.Next();
            var val1 = randomGen.Next();
            var val2 = randomGen.Next();
            _liveDataSimpleType = new LiveData<int>(val0, _testRxSchedulerFacade.Object);

            _singleDisposable = _liveDataSimpleType.Subscribe(OnNextMock, OnErrorMock, OnCompletedMock);
            var testSequence = new List<int>(new []{ val1, val2, val2, val1, val2, val1, val1});
            testSequence.ForEach(i => _liveDataSimpleType.PostValue(i));

            Assert.AreEqual(6, _emittedVerificationListSimple.Count);
            Assert.AreEqual(val0, _emittedVerificationListSimple[0]);
            Assert.AreEqual(val1, _emittedVerificationListSimple[1]);
            Assert.AreEqual(val2, _emittedVerificationListSimple[2]);
            Assert.AreEqual(val1, _emittedVerificationListSimple[3]);
            Assert.AreEqual(val2, _emittedVerificationListSimple[4]);
            Assert.AreEqual(val1, _emittedVerificationListSimple[5]);
        }

        /// <summary>
        /// Test default simple type, duplicates in sequence allowed
        /// allowDuplicatesInSequence = true
        /// </summary>
        [TestMethod]
        public void SubscribeSimpleTypeAllowDuplicatesTest()
        {
            var randomGen = new Random();
            var val0 = randomGen.Next();
            var val1 = randomGen.Next();
            var val2 = randomGen.Next();
            _liveDataSimpleType = new LiveData<int>(val0, _testRxSchedulerFacade.Object, true);

            _singleDisposable = _liveDataSimpleType.Subscribe(OnNextMock, OnErrorMock, OnCompletedMock);
            var testSequence = new List<int>(new[] { val1, val2, val2, val1, val2, val1, val1 });
            testSequence.ForEach(i => _liveDataSimpleType.PostValue(i));

            Assert.AreEqual(8, _emittedVerificationListSimple.Count);
            Assert.AreEqual(val0, _emittedVerificationListSimple[0]);
            for (var i = 1; i < _emittedVerificationListSimple.Count; ++i)
            {
                Assert.AreEqual(testSequence[i - 1], _emittedVerificationListSimple[i]);
            }
        }

        /// <summary>
        /// Test default object type, default settings
        /// allowDuplicatesInSequence = false
        /// </summary>
        [TestMethod]
        public void SubscribeObjectTypeDefaultSettingsTest()
        {
            var val0 = TestUtils.RandomString(32);
            var val1 = TestUtils.RandomString(32);
            var val2 = TestUtils.RandomString(32);
            _liveDataObjType = new LiveData<string>(val0, _testRxSchedulerFacade.Object);

            _singleDisposable = _liveDataObjType.Subscribe(OnNextMock, OnErrorMock, OnCompletedMock);
            var testSequence = new List<string>(new[] { val1, val2, val2, val1, val2, val1, val1 });
            testSequence.ForEach(s => _liveDataObjType.PostValue(s));

            Assert.AreEqual(6, _emittedVerificationListObj.Count);
            Assert.AreEqual(val0, _emittedVerificationListObj[0]);
            Assert.AreEqual(val1, _emittedVerificationListObj[1]);
            Assert.AreEqual(val2, _emittedVerificationListObj[2]);
            Assert.AreEqual(val1, _emittedVerificationListObj[3]);
            Assert.AreEqual(val2, _emittedVerificationListObj[4]);
            Assert.AreEqual(val1, _emittedVerificationListObj[5]);
        }

        /// <summary>
        /// Test default object type, duplicates in sequence allowed
        /// allowDuplicatesInSequence = true
        /// </summary>
        [TestMethod]
        public void SubscribeObjectTypeAllowDuplicatesTest()
        {
            var val0 = TestUtils.RandomString(32);
            var val1 = TestUtils.RandomString(32);
            var val2 = TestUtils.RandomString(32);
            _liveDataObjType = new LiveData<string>(val0, _testRxSchedulerFacade.Object, true);

            _singleDisposable = _liveDataObjType.Subscribe(OnNextMock, OnErrorMock, OnCompletedMock);
            var testSequence = new List<string>(new[] { val1, val2, val2, val1, val2, val1, val1 });
            testSequence.ForEach(s => _liveDataObjType.PostValue(s));

            Assert.AreEqual(8, _emittedVerificationListObj.Count);
            Assert.AreEqual(val0, _emittedVerificationListObj[0]);
            for (var i = 1; i < _emittedVerificationListObj.Count; ++i)
            {
                Assert.AreEqual(testSequence[i - 1], _emittedVerificationListObj[i]);
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
            _liveDataObjType = new LiveData<string>(val0, _testRxSchedulerFacade.Object);

            _singleDisposable = _liveDataObjType.Subscribe(OnNextMock, OnErrorMock, OnCompletedMock);
            var testSequence = new List<string>(new[] { val1, val2, val2, val1, val2, val1, val1 });
            _liveDataObjType.PostValue(testSequence[0]);
            _liveDataObjType.PostValue(testSequence[1]);
            _liveDataObjType.PostValue(testSequence[2]);
            _singleDisposable.Dispose();
            _singleDisposable = _liveDataObjType.Subscribe(OnNextMock, OnErrorMock, OnCompletedMock);
            _liveDataObjType.PostValue(testSequence[3]);
            _liveDataObjType.PostValue(testSequence[4]);
            _singleDisposable.Dispose();
            _singleDisposable = _liveDataObjType.Subscribe(OnNextMock, OnErrorMock, OnCompletedMock);
            _liveDataObjType.PostValue(testSequence[5]);
            _liveDataObjType.PostValue(testSequence[6]);

            Assert.AreEqual(6, _emittedVerificationListObj.Count);
            Assert.AreEqual(val0, _emittedVerificationListObj[0]);
            Assert.AreEqual(val1, _emittedVerificationListObj[1]);
            Assert.AreEqual(val2, _emittedVerificationListObj[2]);
            Assert.AreEqual(val1, _emittedVerificationListObj[3]);
            Assert.AreEqual(val2, _emittedVerificationListObj[4]);
            Assert.AreEqual(val1, _emittedVerificationListObj[5]);
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
            _liveDataObjType = new LiveData<string>(val0, _testRxSchedulerFacade.Object, true);

            _singleDisposable = _liveDataObjType.Subscribe(OnNextMock, OnErrorMock, OnCompletedMock);
            var testSequence = new List<string>(new[] { val1, val2, val2, val1, val2, val1, val1 });
            _liveDataObjType.PostValue(testSequence[0]);
            _liveDataObjType.PostValue(testSequence[1]);
            _liveDataObjType.PostValue(testSequence[2]);
            _singleDisposable.Dispose();
            _singleDisposable = _liveDataObjType.Subscribe(OnNextMock, OnErrorMock, OnCompletedMock);
            _liveDataObjType.PostValue(testSequence[3]);
            _liveDataObjType.PostValue(testSequence[4]);
            _singleDisposable.Dispose();
            _singleDisposable = _liveDataObjType.Subscribe(OnNextMock, OnErrorMock, OnCompletedMock);
            _liveDataObjType.PostValue(testSequence[5]);
            _liveDataObjType.PostValue(testSequence[6]);

            Assert.AreEqual(10, _emittedVerificationListObj.Count);
            Assert.AreEqual(val0, _emittedVerificationListObj[0]);
            Assert.AreEqual(val1, _emittedVerificationListObj[1]);
            Assert.AreEqual(val2, _emittedVerificationListObj[2]);
            Assert.AreEqual(val2, _emittedVerificationListObj[3]);
            Assert.AreEqual(val2, _emittedVerificationListObj[4]);
            Assert.AreEqual(val1, _emittedVerificationListObj[5]);
            Assert.AreEqual(val2, _emittedVerificationListObj[6]);
            Assert.AreEqual(val2, _emittedVerificationListObj[7]);
            Assert.AreEqual(val1, _emittedVerificationListObj[8]);
            Assert.AreEqual(val1, _emittedVerificationListObj[9]);
        }


        private void OnNextMock(int i) => _emittedVerificationListSimple.Add(i);
        private void OnNextMock(string obj) => _emittedVerificationListObj.Add(obj);
        private void OnErrorMock(Exception exception) => Assert.Fail("onError called");
        private void OnCompletedMock() { }
    }
}
