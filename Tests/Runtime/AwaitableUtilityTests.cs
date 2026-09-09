using System;
using System.Collections;
using System.Threading;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Jeomseon.Unity.Awaitable.Tests
{
    public sealed class AwaitableUtilityTests
    {
        [Test]
        public void WhenAll_NullArray_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => AwaitableUtility.WhenAll((UnityEngine.Awaitable[])null));
        }

        [Test]
        public void WaitUntil_NullPredicate_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => AwaitableUtility.WaitUntil(null));
        }

        [Test]
        public void WaitWhile_NullPredicate_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => AwaitableUtility.WaitWhile(null));
        }

        [UnityTest]
        public IEnumerator WhenAll_Empty_CompletesImmediately()
        {
            var awaiter = AwaitableUtility.WhenAll().GetAwaiter();
            Assert.IsTrue(awaiter.IsCompleted, "empty WhenAll should complete synchronously");
            awaiter.GetResult();
            yield break;
        }

        [UnityTest]
        public IEnumerator WhenAll_WaitsForTheLongestChild()
        {
            int completed = 0;
            UnityEngine.Awaitable Track(int frames) => TrackFrames(frames, () => completed++);

            UnityEngine.Awaitable all = AwaitableUtility.WhenAll(Track(1), Track(5), Track(3));
            var awaiter = all.GetAwaiter();

            int guard = 0;
            while (!awaiter.IsCompleted)
            {
                Assert.Less(++guard, 600, "WhenAll did not complete");
                yield return null;
            }

            awaiter.GetResult();
            Assert.AreEqual(3, completed);
        }

        [UnityTest]
        public IEnumerator WhenAll_Generic_ReturnsResultsInInputOrder()
        {
            UnityEngine.Awaitable<int>[] children =
            {
                ValueAfterFrames(4, 10),
                ValueAfterFrames(1, 20),
                ValueAfterFrames(2, 30),
            };

            var awaiter = AwaitableUtility.WhenAll(children).GetAwaiter();
            while (!awaiter.IsCompleted) yield return null;

            int[] results = awaiter.GetResult();
            Assert.AreEqual(new[] { 10, 20, 30 }, results);
        }

        [UnityTest]
        public IEnumerator WhenAll_PropagatesFirstException_AndStillObservesEveryChild()
        {
            int observed = 0;
            UnityEngine.Awaitable Ok(int frames) => TrackFrames(frames, () => observed++);

            async UnityEngine.Awaitable Fail(int frames)
            {
                for (int i = 0; i < frames; i++) await UnityEngine.Awaitable.NextFrameAsync();
                observed++;
                throw new InvalidOperationException("boom");
            }

            var awaiter = AwaitableUtility.WhenAll(Ok(1), Fail(2), Ok(4)).GetAwaiter();
            while (!awaiter.IsCompleted) yield return null;

            InvalidOperationException thrown = Assert.Throws<InvalidOperationException>(() => awaiter.GetResult());
            Assert.AreEqual("boom", thrown.Message);
            Assert.AreEqual(3, observed, "every child must be awaited even after one faults");
        }

        [UnityTest]
        public IEnumerator WaitUntil_AlreadyTrue_CompletesWithoutYieldingAFrame()
        {
            var awaiter = AwaitableUtility.WaitUntil(() => true).GetAwaiter();
            Assert.IsTrue(awaiter.IsCompleted);
            awaiter.GetResult();
            yield break;
        }

        [UnityTest]
        public IEnumerator WaitUntil_CompletesWhenPredicateBecomesTrue()
        {
            bool flag = false;
            var awaiter = AwaitableUtility.WaitUntil(() => flag).GetAwaiter();

            yield return null;
            yield return null;
            Assert.IsFalse(awaiter.IsCompleted);

            flag = true;
            while (!awaiter.IsCompleted) yield return null;
            awaiter.GetResult();
        }

        [UnityTest]
        public IEnumerator WaitWhile_CompletesWhenPredicateBecomesFalse()
        {
            bool flag = true;
            var awaiter = AwaitableUtility.WaitWhile(() => flag).GetAwaiter();

            yield return null;
            Assert.IsFalse(awaiter.IsCompleted);

            flag = false;
            while (!awaiter.IsCompleted) yield return null;
            awaiter.GetResult();
        }

        [UnityTest]
        public IEnumerator WaitUntil_WhenCancelled_ThrowsOperationCanceledException()
        {
            var cts = new CancellationTokenSource();
            var awaiter = AwaitableUtility.WaitUntil(() => false, cts.Token).GetAwaiter();

            yield return null;
            cts.Cancel();

            int guard = 0;
            while (!awaiter.IsCompleted)
            {
                Assert.Less(++guard, 600, "cancelled WaitUntil did not settle");
                yield return null;
            }

            Assert.Throws<OperationCanceledException>(() => awaiter.GetResult());
        }

        private static async UnityEngine.Awaitable TrackFrames(int frames, Action onComplete)
        {
            for (int i = 0; i < frames; i++) await UnityEngine.Awaitable.NextFrameAsync();
            onComplete();
        }

        private static async UnityEngine.Awaitable<int> ValueAfterFrames(int frames, int value)
        {
            for (int i = 0; i < frames; i++) await UnityEngine.Awaitable.NextFrameAsync();
            return value;
        }
    }
}
