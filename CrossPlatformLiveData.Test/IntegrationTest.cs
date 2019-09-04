using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using XFTests.Utils;

namespace CrossPlatformLiveData.Test
{
    [TestClass]
    public class IntegrationTest
    {
        private LiveData<int> _liveDataValueType;
        private LiveData<string> _liveDataReferenceType;
        private LifecycleManager _lifecycleManager;

        private Mock<IRxSchedulersFacade> _testRxSchedulerFacade;

        private readonly IList<int> _emittedVerificationListValue = new List<int>();
        private readonly IList<string> _emittedVerificationListReference = new List<string>();

        [TestInitialize]
        public void SetUp()
        {
            _lifecycleManager = new LifecycleManager();

            _testRxSchedulerFacade = new Mock<IRxSchedulersFacade>();
            _testRxSchedulerFacade.Setup(facade => facade.Ui()).Returns(CurrentThreadScheduler.Instance);
            _testRxSchedulerFacade.Setup(facade => facade.Io()).Returns(CurrentThreadScheduler.Instance);
        }

        /// <summary>
        /// Integration test LiveData & LifecycleManager - default settings
        /// allowDuplicatesInSequence = false
        /// </summary>
        [TestMethod]
        public void LiveDataValueTypeDefaultSettingsIntegrationTest()
        {
            var randomGen = new Random();
            var val0 = randomGen.Next();
            var val1 = randomGen.Next();
            var val2 = randomGen.Next();
            var testSequence = new List<int>(new[] { val1, val2, val2, val1, val2, val1, val1 });

            _liveDataValueType = new LiveData<int>(val0, _testRxSchedulerFacade.Object);

            _liveDataValueType.Observe(_lifecycleManager,
                i => _emittedVerificationListValue.Add(i),
                e => Assert.Fail("onError called"));

            Assert.AreEqual(0, _emittedVerificationListValue.Count);

            _lifecycleManager.OnResume();

            Assert.AreEqual(1, _emittedVerificationListValue.Count);
            Assert.AreEqual(val0, _emittedVerificationListValue[0]);

            testSequence.ForEach(i => _liveDataValueType.PostValue(i));

            Assert.AreEqual(6, _emittedVerificationListValue.Count);
            Assert.AreEqual(val0, _emittedVerificationListValue[0]);
            Assert.AreEqual(val1, _emittedVerificationListValue[1]);
            Assert.AreEqual(val2, _emittedVerificationListValue[2]);
            Assert.AreEqual(val1, _emittedVerificationListValue[3]);
            Assert.AreEqual(val2, _emittedVerificationListValue[4]);
            Assert.AreEqual(val1, _emittedVerificationListValue[5]);

            _lifecycleManager.OnPause();

            testSequence.ForEach(i => _liveDataValueType.PostValue(i));

            Assert.AreEqual(6, _emittedVerificationListValue.Count);

            _lifecycleManager.OnResume();

            Assert.AreEqual(6, _emittedVerificationListValue.Count);

            _lifecycleManager.OnPause();

            testSequence.ForEach(i => _liveDataValueType.PostValue(i));
            _liveDataValueType.PostValue(val0);
            _liveDataValueType.PostValue(val1);
            _liveDataValueType.PostValue(val2);

            _lifecycleManager.OnResume();

            Assert.AreEqual(7, _emittedVerificationListValue.Count);
            Assert.AreEqual(val2, _emittedVerificationListValue[6]);

            _lifecycleManager.OnDestroyView();

            Assert.AreEqual(7, _emittedVerificationListValue.Count);

            _liveDataValueType.Observe(_lifecycleManager,
                i => _emittedVerificationListValue.Add(i),
                e => Assert.Fail("onError called"));

            Assert.AreEqual(7, _emittedVerificationListValue.Count);

            _lifecycleManager.OnResume();

            Assert.AreEqual(8, _emittedVerificationListValue.Count);
            Assert.AreEqual(val2, _emittedVerificationListValue[7]);

            _lifecycleManager.OnPause();

            Assert.AreEqual(8, _emittedVerificationListValue.Count);

            _lifecycleManager.OnDestroyView();

            testSequence.ForEach(i => _liveDataValueType.PostValue(i));
            _liveDataValueType.PostValue(val0);
            _liveDataValueType.PostValue(val1);

            _liveDataValueType.Observe(_lifecycleManager,
                i => _emittedVerificationListValue.Add(i),
                e => Assert.Fail("onError called"));

            Assert.AreEqual(8, _emittedVerificationListValue.Count);

            _lifecycleManager.OnResume();

            Assert.AreEqual(9, _emittedVerificationListValue.Count);
            Assert.AreEqual(val1, _emittedVerificationListValue[8]);
        }

        /// <summary>
        /// Integration test LiveData & LifecycleManager - duplicates allowed & re-emission onResume
        /// allowDuplicatesInSequence = true
        /// </summary>
        [TestMethod]
        public void LiveDataValueTypeAllowDuplicatesIntegrationTest()
        {
            var randomGen = new Random();
            var val0 = randomGen.Next();
            var val1 = randomGen.Next();
            var val2 = randomGen.Next();
            var testSequence = new List<int>(new[] { val1, val2, val2, val1, val2, val1, val1 });

            _liveDataValueType = new LiveData<int>(val0, _testRxSchedulerFacade.Object, true);

            _liveDataValueType.Observe(_lifecycleManager,
                i => _emittedVerificationListValue.Add(i),
                e => Assert.Fail("onError called"));

            Assert.AreEqual(0, _emittedVerificationListValue.Count);

            _lifecycleManager.OnResume();

            Assert.AreEqual(1, _emittedVerificationListValue.Count);
            Assert.AreEqual(val0, _emittedVerificationListValue[0]);

            testSequence.ForEach(i => _liveDataValueType.PostValue(i));

            Assert.AreEqual(8, _emittedVerificationListValue.Count);
            for (var i = 1; i < _emittedVerificationListValue.Count; ++i)
            {
                Assert.AreEqual(testSequence[i - 1], _emittedVerificationListValue[i]);
            }

            _lifecycleManager.OnPause();

            testSequence.ForEach(i => _liveDataValueType.PostValue(i));

            Assert.AreEqual(8, _emittedVerificationListValue.Count);

            _lifecycleManager.OnResume();

            Assert.AreEqual(9, _emittedVerificationListValue.Count);
            Assert.AreEqual(val1, _emittedVerificationListValue[8]);

            _lifecycleManager.OnPause();

            testSequence.ForEach(i => _liveDataValueType.PostValue(i));
            _liveDataValueType.PostValue(val0);
            _liveDataValueType.PostValue(val1);
            _liveDataValueType.PostValue(val2);

            _lifecycleManager.OnResume();

            Assert.AreEqual(10, _emittedVerificationListValue.Count);
            Assert.AreEqual(val2, _emittedVerificationListValue[9]);

            _lifecycleManager.OnDestroyView();

            Assert.AreEqual(10, _emittedVerificationListValue.Count);

            _liveDataValueType.Observe(_lifecycleManager,
                i => _emittedVerificationListValue.Add(i),
                e => Assert.Fail("onError called"));

            Assert.AreEqual(10, _emittedVerificationListValue.Count);

            _lifecycleManager.OnResume();

            Assert.AreEqual(11, _emittedVerificationListValue.Count);
            Assert.AreEqual(val2, _emittedVerificationListValue[10]);

            _lifecycleManager.OnPause();

            Assert.AreEqual(11, _emittedVerificationListValue.Count);

            _lifecycleManager.OnDestroyView();

            testSequence.ForEach(i => _liveDataValueType.PostValue(i));
            _liveDataValueType.PostValue(val0);
            _liveDataValueType.PostValue(val1);

            _liveDataValueType.Observe(_lifecycleManager,
                i => _emittedVerificationListValue.Add(i),
                e => Assert.Fail("onError called"));

            Assert.AreEqual(11, _emittedVerificationListValue.Count);

            _lifecycleManager.OnResume();

            Assert.AreEqual(12, _emittedVerificationListValue.Count);
            Assert.AreEqual(val1, _emittedVerificationListValue[11]);
        }

        /// <summary>
        /// Integration test LiveData & LifecycleManager - default settings
        /// allowDuplicatesInSequence = false
        /// </summary>
        [TestMethod]
        public void LiveDataReferenceTypeDefaultSettingsIntegrationTest()
        {
            var val0 = TestUtils.RandomString(32);
            var val1 = TestUtils.RandomString(32);
            var val2 = TestUtils.RandomString(32);
            var testSequence = new List<string>(new[] { val1, val2, val2, val1, val2, val1, val1 });

            _liveDataReferenceType = new LiveData<string>(val0, _testRxSchedulerFacade.Object);

            _liveDataReferenceType.Observe(_lifecycleManager,
                s => _emittedVerificationListReference.Add(s),
                e => Assert.Fail("onError called"));

            Assert.AreEqual(0, _emittedVerificationListReference.Count);

            _lifecycleManager.OnResume();

            Assert.AreEqual(1, _emittedVerificationListReference.Count);
            Assert.AreEqual(val0, _emittedVerificationListReference[0]);

            testSequence.ForEach(i => _liveDataReferenceType.PostValue(i));

            Assert.AreEqual(6, _emittedVerificationListReference.Count);
            Assert.AreEqual(val0, _emittedVerificationListReference[0]);
            Assert.AreEqual(val1, _emittedVerificationListReference[1]);
            Assert.AreEqual(val2, _emittedVerificationListReference[2]);
            Assert.AreEqual(val1, _emittedVerificationListReference[3]);
            Assert.AreEqual(val2, _emittedVerificationListReference[4]);
            Assert.AreEqual(val1, _emittedVerificationListReference[5]);

            _lifecycleManager.OnPause();

            testSequence.ForEach(i => _liveDataReferenceType.PostValue(i));

            Assert.AreEqual(6, _emittedVerificationListReference.Count);

            _lifecycleManager.OnResume();

            Assert.AreEqual(6, _emittedVerificationListReference.Count);

            _lifecycleManager.OnPause();

            testSequence.ForEach(i => _liveDataReferenceType.PostValue(i));
            _liveDataReferenceType.PostValue(val0);
            _liveDataReferenceType.PostValue(val1);
            _liveDataReferenceType.PostValue(val2);

            _lifecycleManager.OnResume();

            Assert.AreEqual(7, _emittedVerificationListReference.Count);
            Assert.AreEqual(val2, _emittedVerificationListReference[6]);

            _lifecycleManager.OnDestroyView();

            Assert.AreEqual(7, _emittedVerificationListReference.Count);

            _liveDataReferenceType.Observe(_lifecycleManager,
                s => _emittedVerificationListReference.Add(s),
                e => Assert.Fail("onError called"));

            Assert.AreEqual(7, _emittedVerificationListReference.Count);

            _lifecycleManager.OnResume();

            Assert.AreEqual(8, _emittedVerificationListReference.Count);
            Assert.AreEqual(val2, _emittedVerificationListReference[7]);

            _lifecycleManager.OnPause();

            Assert.AreEqual(8, _emittedVerificationListReference.Count);

            _lifecycleManager.OnDestroyView();

            testSequence.ForEach(i => _liveDataReferenceType.PostValue(i));
            _liveDataReferenceType.PostValue(val0);
            _liveDataReferenceType.PostValue(val1);

            _liveDataReferenceType.Observe(_lifecycleManager,
                s => _emittedVerificationListReference.Add(s),
                e => Assert.Fail("onError called"));

            Assert.AreEqual(8, _emittedVerificationListReference.Count);

            _lifecycleManager.OnResume();

            Assert.AreEqual(9, _emittedVerificationListReference.Count);
            Assert.AreEqual(val1, _emittedVerificationListReference[8]);
        }

        /// <summary>
        /// Integration test LiveData & LifecycleManager - duplicates allowed & re-emission onResume
        /// allowDuplicatesInSequence = true
        /// </summary>
        [TestMethod]
        public void LiveDataReferenceTypeAllowDuplicatesIntegrationTest()
        {
            var val0 = TestUtils.RandomString(32);
            var val1 = TestUtils.RandomString(32);
            var val2 = TestUtils.RandomString(32);
            var testSequence = new List<string>(new[] { val1, val2, val2, val1, val2, val1, val1 });

            _liveDataReferenceType = new LiveData<string>(val0, _testRxSchedulerFacade.Object, true);

            _liveDataReferenceType.Observe(_lifecycleManager,
                s => _emittedVerificationListReference.Add(s),
                e => Assert.Fail("onError called"));

            Assert.AreEqual(0, _emittedVerificationListReference.Count);

            _lifecycleManager.OnResume();

            Assert.AreEqual(1, _emittedVerificationListReference.Count);
            Assert.AreEqual(val0, _emittedVerificationListReference[0]);

            testSequence.ForEach(i => _liveDataReferenceType.PostValue(i));

            Assert.AreEqual(8, _emittedVerificationListReference.Count);
            for (var i = 1; i < _emittedVerificationListReference.Count; ++i)
            {
                Assert.AreEqual(testSequence[i - 1], _emittedVerificationListReference[i]);
            }

            _lifecycleManager.OnPause();

            testSequence.ForEach(s => _liveDataReferenceType.PostValue(s));

            Assert.AreEqual(8, _emittedVerificationListReference.Count);

            _lifecycleManager.OnResume();

            Assert.AreEqual(9, _emittedVerificationListReference.Count);
            Assert.AreEqual(val1, _emittedVerificationListReference[8]);

            _lifecycleManager.OnPause();

            testSequence.ForEach(s => _liveDataReferenceType.PostValue(s));
            _liveDataReferenceType.PostValue(val0);
            _liveDataReferenceType.PostValue(val1);
            _liveDataReferenceType.PostValue(val2);

            _lifecycleManager.OnResume();

            Assert.AreEqual(10, _emittedVerificationListReference.Count);
            Assert.AreEqual(val2, _emittedVerificationListReference[9]);

            _lifecycleManager.OnDestroyView();

            Assert.AreEqual(10, _emittedVerificationListReference.Count);

            _liveDataReferenceType.Observe(_lifecycleManager,
                s => _emittedVerificationListReference.Add(s),
                e => Assert.Fail("onError called"));

            Assert.AreEqual(10, _emittedVerificationListReference.Count);

            _lifecycleManager.OnResume();

            Assert.AreEqual(11, _emittedVerificationListReference.Count);
            Assert.AreEqual(val2, _emittedVerificationListReference[10]);

            _lifecycleManager.OnPause();

            Assert.AreEqual(11, _emittedVerificationListReference.Count);

            _lifecycleManager.OnDestroyView();

            testSequence.ForEach(i => _liveDataReferenceType.PostValue(i));
            _liveDataReferenceType.PostValue(val0);
            _liveDataReferenceType.PostValue(val1);

            _liveDataReferenceType.Observe(_lifecycleManager,
                s => _emittedVerificationListReference.Add(s),
                e => Assert.Fail("onError called"));

            Assert.AreEqual(11, _emittedVerificationListReference.Count);

            _lifecycleManager.OnResume();

            Assert.AreEqual(12, _emittedVerificationListReference.Count);
            Assert.AreEqual(val1, _emittedVerificationListReference[11]);
        }
    }
}
