using System;
using System.Runtime.ExceptionServices;
using System.Threading;

namespace Jeomseon.Unity.Awaitable
{
    /// <summary>
    /// Unity 6 공식 <see cref="UnityEngine.Awaitable"/> 위에 조합(<c>WhenAll</c>)과 조건 대기
    /// (<c>WaitUntil</c>/<c>WaitWhile</c>)만 얇게 얹은 확장입니다. 자체 Task-like primitive를
    /// 만들지 않고, 매 호출마다 새 <see cref="UnityEngine.Awaitable"/>만 반환합니다. 취소는
    /// <see cref="CancellationToken"/> 하나로만 하며, 취소 시 <see cref="OperationCanceledException"/>을
    /// 던져 Unity <see cref="UnityEngine.Awaitable"/> 계약과 통일합니다. 근거: 하네스 ADR-0011.
    /// </summary>
    public static class AwaitableUtility
    {
        /// <summary>
        /// 모든 <paramref name="awaitables"/>가 완료될 때까지 기다립니다. 하나 이상이 예외로
        /// 끝나면 나머지까지 모두 관찰한 뒤 <b>첫 예외</b>를 원본 스택 그대로 다시 던집니다
        /// (<see cref="System.Threading.Tasks.Task.WhenAll(System.Threading.Tasks.Task[])"/> 관례).
        /// 여러 실패를 모으는 <see cref="AggregateException"/>은 쓰지 않습니다. 빈 배열이면 즉시
        /// 완료됩니다.
        /// </summary>
        /// <remarks>
        /// 넘긴 <see cref="UnityEngine.Awaitable"/>들은 이미 실행 중(hot)이므로, 이 메서드가
        /// 순서대로 await해도 전체 대기 시간은 가장 오래 걸리는 하나와 같습니다. 각 항목은 한 번씩만
        /// await되어 관찰되지 않은 예외가 남지 않습니다.
        /// </remarks>
        public static UnityEngine.Awaitable WhenAll(params UnityEngine.Awaitable[] awaitables)
        {
            if (awaitables == null) throw new ArgumentNullException(nameof(awaitables));
            return WhenAllCore((UnityEngine.Awaitable[])awaitables.Clone());
        }

        /// <summary>
        /// 결과가 있는 <see cref="WhenAll(UnityEngine.Awaitable[])"/>. 반환 배열의 인덱스는 입력
        /// 인덱스와 일치합니다. 예외 정책은 비제네릭 오버로드와 같습니다.
        /// </summary>
        public static UnityEngine.Awaitable<T[]> WhenAll<T>(params UnityEngine.Awaitable<T>[] awaitables)
        {
            if (awaitables == null) throw new ArgumentNullException(nameof(awaitables));
            return WhenAllCore((UnityEngine.Awaitable<T>[])awaitables.Clone());
        }

        /// <summary>
        /// <paramref name="predicate"/>가 <see langword="true"/>가 될 때까지 매 프레임
        /// (<see cref="UnityEngine.Awaitable.NextFrameAsync(CancellationToken)"/>) 다시 평가하며
        /// 기다립니다. 호출 시점에 이미 <see langword="true"/>면 프레임을 소비하지 않고 즉시
        /// 완료됩니다. <paramref name="cancellationToken"/>이 취소되면
        /// <see cref="OperationCanceledException"/>을 던집니다.
        /// </summary>
        public static UnityEngine.Awaitable WaitUntil(Func<bool> predicate, CancellationToken cancellationToken = default)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            return WaitUntilCore(predicate, cancellationToken);
        }

        private static async UnityEngine.Awaitable WaitUntilCore(Func<bool> predicate, CancellationToken cancellationToken)
        {
            while (!predicate())
            {
                cancellationToken.ThrowIfCancellationRequested();
                await UnityEngine.Awaitable.NextFrameAsync(cancellationToken);
            }
        }

        /// <summary>
        /// <see cref="WaitUntil"/>의 반대. <paramref name="predicate"/>가 <see langword="false"/>가
        /// 될 때까지 매 프레임 다시 평가하며 기다립니다. 호출 시점에 이미 <see langword="false"/>면
        /// 즉시 완료됩니다.
        /// </summary>
        public static UnityEngine.Awaitable WaitWhile(Func<bool> predicate, CancellationToken cancellationToken = default)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            return WaitWhileCore(predicate, cancellationToken);
        }

        private static async UnityEngine.Awaitable WaitWhileCore(Func<bool> predicate, CancellationToken cancellationToken)
        {
            while (predicate())
            {
                cancellationToken.ThrowIfCancellationRequested();
                await UnityEngine.Awaitable.NextFrameAsync(cancellationToken);
            }
        }

        private static async UnityEngine.Awaitable WhenAllCore(UnityEngine.Awaitable[] awaitables)
        {
            ExceptionDispatchInfo first = null;
            for (int i = 0; i < awaitables.Length; i++)
            {
                UnityEngine.Awaitable awaitable = awaitables[i];
                if (awaitable == null)
                {
                    first ??= ExceptionDispatchInfo.Capture(
                        new ArgumentException($"awaitables[{i}] is null", nameof(awaitables)));
                    continue;
                }

                try
                {
                    await awaitable;
                }
                catch (Exception exception)
                {
                    first ??= ExceptionDispatchInfo.Capture(exception);
                }
            }

            first?.Throw();
        }

        private static async UnityEngine.Awaitable<T[]> WhenAllCore<T>(UnityEngine.Awaitable<T>[] awaitables)
        {
            var results = new T[awaitables.Length];
            ExceptionDispatchInfo first = null;
            for (int i = 0; i < awaitables.Length; i++)
            {
                UnityEngine.Awaitable<T> awaitable = awaitables[i];
                if (awaitable == null)
                {
                    first ??= ExceptionDispatchInfo.Capture(
                        new ArgumentException($"awaitables[{i}] is null", nameof(awaitables)));
                    continue;
                }

                try
                {
                    results[i] = await awaitable;
                }
                catch (Exception exception)
                {
                    first ??= ExceptionDispatchInfo.Capture(exception);
                }
            }

            first?.Throw();
            return results;
        }
    }
}
