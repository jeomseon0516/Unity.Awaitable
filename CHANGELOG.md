# 변경 기록

## [0.1.0] - 2026-09-03

- 첫 릴리스. Unity 6 공식 `Awaitable`을 핵심 primitive로 유지한 채, 공식 API에 없는 최소 조합만
  얹습니다.
  - `AwaitableUtility.WhenAll(params Awaitable[])` / `WhenAll<T>(params Awaitable<T>[]) -> Awaitable<T[]>`
    — `Task` 경유 없이 `Awaitable` 네이티브. 모든 자식을 관찰한 뒤 첫 예외만 전파
    (`AggregateException` 안 씀). 결과 배열 인덱스는 입력 인덱스와 일치. 빈 배열은 즉시 완료.
  - `AwaitableUtility.WaitUntil(Func<bool>, CancellationToken = default)` /
    `WaitWhile(Func<bool>, CancellationToken = default)` — 매 프레임(`Awaitable.NextFrameAsync`)
    predicate 재평가. 호출 시점에 조건이 이미 충족돼 있으면 프레임을 소비하지 않고 즉시 완료.
    취소 시 `OperationCanceledException`.
- 자체 Task-like primitive 없음. `Jeomseon.Unity.Coroutines`/`Jeomseon.Unity.Dispatcher`/
  `Jeomseon.Unity.Reactive`에 의존하지 않습니다.
- PlayMode 테스트 `AwaitableUtilityTests` 11개.
