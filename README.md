# Jeomseon Unity Awaitable

한국어 | [English](README.en.md)

Unity Awaitable에 WhenAll과 WaitUntil/WaitWhile만 추가합니다. 자체 비동기 primitive나 PlayerLoop scheduler는 제공하지 않습니다.

## 빠르게 시작하기

각 비동기 작업을 시작 → WhenAll에 각 Awaitable을 한 번 전달 → 반환 결과 한 번 await. 조건 대기는 predicate와 선택적 CancellationToken을 전달합니다.

## 계약과 주의사항

같은 Awaitable을 중복 소비하지 마세요. WhenAll은 전부 관찰한 뒤 입력 순서로 처음 관찰한 예외 하나를 던집니다. 조건이 이미 만족되면 취소된 토큰이어도 대기 없이 완료할 수 있습니다.

## 요구 사항

- Unity `6000.6.0f1`
- 패키지: `com.jeomseon.unity.awaitable` `0.1.0`

## 설치

프로젝트 `Packages/manifest.json`의 기존 dependencies와 registry를 유지하면서 다음 항목을 병합합니다.

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

또는 Package Manager의 **Install package from git URL**을 사용합니다:

```text
https://github.com/jeomseon0516/Unity.Awaitable.git#v0.1.0
```

## API 사용과 상세 설명

### 포함 API (0.1.0)

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

### 샘플

Package Manager에서 **Basic Usage** 샘플을 Import한 뒤
`Assets/Samples/Jeomseon Unity Awaitable/0.1.0/Basic Usage/AwaitableBasicUsage.unity`를 열고
Play를 누르세요. 세 개의 큐브가 각 대기를 마치면 초록색으로 바뀌고, `WhenAll`과 조건 대기가
모두 끝나면 상태 큐브가 파란색으로 바뀝니다.

### Unity가 이미 제공하는 것 (재구현하지 않음)

`NextFrameAsync` / `WaitForSecondsAsync` / `EndOfFrameAsync` / `FixedUpdateAsync`,
`Awaitable.MainThreadAsync` / `BackgroundThreadAsync`, `Awaitable.FromAsyncOperation`,
대부분의 정적 메서드 `CancellationToken` 오버로드. 이들은 Unity `Awaitable`을 직접 쓰세요.

### 다른 패키지와의 관계

- `Jeomseon.Unity.Coroutines` — 대체 대상이 아닙니다. Coroutine(암묵적 Player Loop 실행)과
  Awaitable(명시적 async/await)은 다른 패턴으로 공존하며, 이 패키지는 Coroutines에 의존하지
  않습니다.
- `Jeomseon.Unity.Dispatcher` — Edit Mode 전용으로 축소됐습니다. Play Mode/Player의
  스레드 전환은 Unity `Awaitable.MainThreadAsync`/`BackgroundThreadAsync`를 직접 쓰세요.
- `Jeomseon.Unity.Reactive` — 무관합니다. Reactive는 이벤트 스트림, 이 패키지는 단발성 비동기
  작업입니다.

### 단계별 계획

`ROADMAP.md` 참고. 0.2 `WhenAny`/`Timeout`/취소 helper, 0.3 `PlayerLoopTiming` 기반 `Yield`,
0.4 Editor/Development 전용 진단.

## 샘플 실행

**Package Manager > Samples**에서 예제를 Import합니다. 포함된 Scene 또는 Editor 도구를 열고 필요한 Setup 메뉴를 먼저 실행합니다.

- [Basic Usage](Samples~/BasicUsage/README.md)
  - [AwaitableBasicUsage](Samples~/BasicUsage/AwaitableBasicUsage.unity)

## 테스트

[테스트 코드](Tests)

manifest의 `testables`에 패키지 ID를 추가하고 Unity Test Runner의 관련 EditMode/PlayMode 어셈블리를 실행합니다.

## 추가 문서

[전체 코드 지도와 패키지 구성](Documentation~/Overview.md)
- [Changelog](CHANGELOG.md)

## 라이선스

[License](LICENSE.md)
