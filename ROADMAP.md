# Awaitable 로드맵

설계 근거: 하네스 `decisions/ADR-0011-awaitable-composition-package.md` (Accepted),
`architecture/awaitable.md`.

## 원칙

- Unity 6 공식 `Awaitable`/`Awaitable<T>`/`AwaitableCompletionSource`를 핵심 primitive로 유지.
  자체 Task-like primitive를 만들지 않는다.
- Unity가 이미 계약까지 제공하는 것(프레임·시간 대기, 스레드 전환, 기본 취소, `AsyncOperation`
  연동)은 재구현하지 않는다.
- 처음부터 UniTask 전체 기능을 복제하지 않고, 두 번째 실사용 사례가 확인되기 전까지 추상화를
  앞당기지 않는다 (`ADR-0004` 원칙).
- PlayerLoop 삽입은 교체가 아니라 기존 loop 보존 삽입으로만 한다.

## 단계

| 버전 | 범위 | 상태 |
| --- | --- | --- |
| 0.1 | `WhenAll` / `WhenAll<T>`, `WaitUntil`, `WaitWhile` | **구현 중** |
| 0.2 | `WhenAny -> Awaitable<int>`, `Timeout`, 다중 `CancellationToken` 결합 helper | 예정 |
| 0.3 | `PlayerLoopTiming` enum + `Yield(timing)` + 타이밍별 Scheduler Runner (삽입, Domain Reload 재등록) | 예정 |
| 0.4 | `UNITY_EDITOR`/`DEVELOPMENT_BUILD` 전용 진단 (double-await, 스레드 오용 검출) | 예정 |
| 0.5 | 실사용 확인 후 `AsyncOperation`/Task interop 문서화. UniTask interop은 별도 확장 패키지 후보 | 예정 |
| 0.6+ | 다량 타이머가 실제 병목으로 측정될 때만 `TimerScheduler` (priority queue / timer wheel) | 조건부 |

## 착수 시 스파이크 (ADR-0011)

- `PlayerLoopTiming` enum 멤버를 6000.6.0f1의 실제 `PlayerLoop.Get()` subsystem 트리와 대조.
- Domain Reload 후 Scheduler Runner 재등록이 중복 삽입 없이 idempotent한지.
- `WhenAll<T>` 결과 순서 보장을 위한 배열 allocation 허용 범위.
- 진단 활성화 스위치: define 심볼 vs `Project Settings > Jeomseon > Awaitable`.

## 완료 조건 (각 단계, `AGENTS.md`)

컴파일 클린 + PlayMode 테스트 통과 + Sample 수동 검증 + `handoffs/current.md` 갱신.
