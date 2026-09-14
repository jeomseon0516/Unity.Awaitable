# Jeomseon Unity Awaitable

[한국어](./README.md) | English

A thin extension over Unity 6's built-in `Awaitable` that adds **only the composition the
official API lacks**. It is not a UniTask clone and does not introduce a new `Task`/`ValueTask`
style primitive. Frame/time waiting, thread hops, and basic cancellation are already contract-
complete in Unity `Awaitable`, so they are used as-is.

## Requirements

- Unity 6000.6.0f1 or newer
- No workspace package dependencies (only `Awaitable` and `CancellationToken`)

## Install via OpenUPM

Register the OpenUPM scoped registry once in your project's `Packages/manifest.json`.

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

## Install via Git URL

Enter the following URL in Unity Package Manager's `Install package from git URL`.

```text
https://github.com/jeomseon0516/Unity.Awaitable.git#v0.1.0
```

## Included API (0.1.0)

```csharp
using Jeomseon.Unity.Awaitable;

// Composition -- Awaitable-native, no Task round-trip
await AwaitableUtility.WhenAll(LoadA(), LoadB(), LoadC());
int[] sizes = await AwaitableUtility.WhenAll(SizeA(), SizeB());   // result index == input index

// Condition wait -- re-evaluates the predicate each frame; completes without
// consuming a frame if it is already satisfied
await AwaitableUtility.WaitUntil(() => _ready);
await AwaitableUtility.WaitWhile(() => _loading, destroyCancellationToken);
```

- `WhenAll` observes every child, then rethrows **only the first exception** (`Task.WhenAll`
  convention; no `AggregateException`).
- Cancellation is `CancellationToken` only. Cancellation throws `OperationCanceledException`.
  Passing `MonoBehaviour.destroyCancellationToken` straight through is the recommended pattern.

## Sample

Import **Basic Usage** from Package Manager, open
`Assets/Samples/Jeomseon Unity Awaitable/0.1.0/Basic Usage/AwaitableBasicUsage.unity`, and enter
Play Mode. Three cubes turn green as their waits finish, and the status cube turns blue after
`WhenAll` and both condition waits complete.

## What Unity already provides (not reimplemented)

`NextFrameAsync` / `WaitForSecondsAsync` / `EndOfFrameAsync` / `FixedUpdateAsync`,
`Awaitable.MainThreadAsync` / `BackgroundThreadAsync`, `Awaitable.FromAsyncOperation`, and the
`CancellationToken` overloads on most static methods. Use Unity `Awaitable` directly for those.

## Relationship to other packages

- `Jeomseon.Unity.Coroutines` -- not a replacement. Coroutines (implicit Player Loop execution)
  and Awaitable (explicit async/await) coexist as different patterns; this package does not
  depend on Coroutines.
- `Jeomseon.Unity.Dispatcher` -- narrowed to Edit Mode only. For Play Mode/Player
  thread hops use Unity `Awaitable.MainThreadAsync`/`BackgroundThreadAsync` directly.
- `Jeomseon.Unity.Reactive` -- unrelated. Reactive is event streams; this package is one-shot
  async work.

## Staged plan

See `ROADMAP.md`. 0.2 `WhenAny`/`Timeout`/cancellation helpers, 0.3 `PlayerLoopTiming`-based
`Yield`, 0.4 Editor/Development-only diagnostics.
