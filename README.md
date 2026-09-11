# Jeomseon Unity Awaitable

한국어 | [English](./README.en.md)

Unity 6 공식 `Awaitable` 위에 **공식 API에 없는 조합만** 얇게 얹은 확장입니다. UniTask 복제가
아니고, 자체 `Task`/`ValueTask`류 비동기 primitive를 만들지 않습니다. 프레임·시간 대기, 스레드
전환, 기본 취소는 Unity `Awaitable`이 이미 계약까지 제공하므로 그대로 씁니다. 설계 근거는 하네스
`ADR-0011`.

## 요구 사항

- Unity 6000.6.0f1 이상
- 워크스페이스 패키지 의존성 없음 (`Awaitable`, `CancellationToken`만 사용)

## OpenUPM으로 설치

프로젝트의 `Packages/manifest.json`에 OpenUPM scoped registry를 한 번 등록합니다.

```json
{
  "scopedRegistries": [
    {
      "name": "OpenUPM",
      "url": "https://package.openupm.com",
      "scopes": [
        "com.jeomseon"
      ]
    }
  ],
  "dependencies": {
    "com.jeomseon.unity.awaitable": "0.1.0"
  }
}
```

## Git URL로 설치

Unity Package Manager의 `Install package from git URL`에 다음 주소를 사용합니다.

```text
https://github.com/jeomseon0516/Unity.Awaitable.git#v0.1.0
```

## 포함 API (0.1.0)

```csharp
using Jeomseon.Unity.Awaitable;

// 조합 — Task 경유 없이 Awaitable 네이티브
await AwaitableUtility.WhenAll(LoadA(), LoadB(), LoadC());
int[] sizes = await AwaitableUtility.WhenAll(SizeA(), SizeB());   // 결과 인덱스 = 입력 인덱스

// 조건 대기 — 매 프레임 predicate 재평가, 이미 참이면 프레임 소비 없이 즉시 완료
await AwaitableUtility.WaitUntil(() => _ready);
await AwaitableUtility.WaitWhile(() => _loading, destroyCancellationToken);
```

- `WhenAll`은 모든 자식을 관찰한 뒤 **첫 예외만** 다시 던집니다(`Task.WhenAll` 관례,
  `AggregateException` 안 씀).
- 취소는 `CancellationToken` 하나로만 합니다. 취소 시 `OperationCanceledException`.
  `MonoBehaviour.destroyCancellationToken`을 그대로 넘겨 쓰는 것을 권장합니다.

## 샘플

Package Manager에서 **Basic Usage** 샘플을 Import한 뒤
`Assets/Samples/Jeomseon Unity Awaitable/0.1.0/Basic Usage/AwaitableBasicUsage.unity`를 열고
Play를 누르세요. 세 개의 큐브가 각 대기를 마치면 초록색으로 바뀌고, `WhenAll`과 조건 대기가
모두 끝나면 상태 큐브가 파란색으로 바뀝니다.

## Unity가 이미 제공하는 것 (재구현하지 않음)

`NextFrameAsync` / `WaitForSecondsAsync` / `EndOfFrameAsync` / `FixedUpdateAsync`,
`Awaitable.MainThreadAsync` / `BackgroundThreadAsync`, `Awaitable.FromAsyncOperation`,
대부분의 정적 메서드 `CancellationToken` 오버로드. 이들은 Unity `Awaitable`을 직접 쓰세요.

## 다른 패키지와의 관계

- `Jeomseon.Unity.Coroutines` — 대체 대상이 아닙니다. Coroutine(암묵적 Player Loop 실행)과
  Awaitable(명시적 async/await)은 다른 패턴으로 공존하며, 이 패키지는 Coroutines에 의존하지
  않습니다.
- `Jeomseon.Unity.Dispatcher` — `ADR-0005`로 Edit Mode 전용으로 축소됐습니다. Play Mode/Player의
  스레드 전환은 Unity `Awaitable.MainThreadAsync`/`BackgroundThreadAsync`를 직접 쓰세요.
- `Jeomseon.Unity.Reactive` — 무관합니다. Reactive는 이벤트 스트림, 이 패키지는 단발성 비동기
  작업입니다.

## 단계별 계획

`ROADMAP.md` 참고. 0.2 `WhenAny`/`Timeout`/취소 helper, 0.3 `PlayerLoopTiming` 기반 `Yield`,
0.4 Editor/Development 전용 진단.
