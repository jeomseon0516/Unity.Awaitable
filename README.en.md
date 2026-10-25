# Jeomseon Unity Awaitable

[한국어](README.md) | English

Adds WhenAll and WaitUntil/WaitWhile to Unity Awaitable, without a custom async primitive or PlayerLoop scheduler.

## Quick start

Start operations, pass each Awaitable once to WhenAll, and await the result once. Condition waits accept a predicate and optional CancellationToken.

## Contracts and limitations

Never consume the same Awaitable twice. WhenAll observes all entries and rethrows the first exception observed in input order. An already satisfied predicate may complete immediately even with a canceled token.

`WaitUntil`/`WaitWhile` use `Awaitable.NextFrameAsync` when waiting is required. Call them from the main thread. [Official threading contract](https://docs.unity3d.com/6000.0/Documentation/ScriptReference/Awaitable.NextFrameAsync.html).

## Requirements

- Unity `6000.6.0f1`
- Package: `com.jeomseon.unity.awaitable` `0.1.0`

## Installation

Merge this into your project's `Packages/manifest.json`; preserve existing dependencies and registries.

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

Or use **Install package from git URL** in Package Manager:

```text
https://github.com/jeomseon0516/Unity.Awaitable.git#v0.1.0
```

## API usage and details

### Included API (0.1.0)

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

### Sample

Import **Basic Usage** from Package Manager, open
`Assets/Samples/Jeomseon Unity Awaitable/0.1.0/Basic Usage/AwaitableBasicUsage.unity`, and enter
Play Mode. Three cubes turn green as their waits finish, and the status cube turns blue after
`WhenAll` and both condition waits complete.

### What Unity already provides (not reimplemented)

`NextFrameAsync` / `WaitForSecondsAsync` / `EndOfFrameAsync` / `FixedUpdateAsync`,
`Awaitable.MainThreadAsync` / `BackgroundThreadAsync`, `Awaitable.FromAsyncOperation`, and the
`CancellationToken` overloads on most static methods. Use Unity `Awaitable` directly for those.

### Relationship to other packages

- `Jeomseon.Unity.Coroutines` -- not a replacement. Coroutines (implicit Player Loop execution)
  and Awaitable (explicit async/await) coexist as different patterns; this package does not
  depend on Coroutines.
- `Jeomseon.Unity.Dispatcher` -- narrowed to Edit Mode only. For Play Mode/Player
  thread hops use Unity `Awaitable.MainThreadAsync`/`BackgroundThreadAsync` directly.
- `Jeomseon.Unity.Reactive` -- unrelated. Reactive is event streams; this package is one-shot
  async work.

### Staged plan

See `ROADMAP.md`. 0.2 `WhenAny`/`Timeout`/cancellation helpers, 0.3 `PlayerLoopTiming`-based
`Yield`, 0.4 Editor/Development-only diagnostics.

## Samples

Import the desired entry from **Package Manager > Samples**. Use the included Scene/Editor tool and complete its Setup step first, if required.

- [Basic Usage](Samples~/BasicUsage/README.md)
  - [AwaitableBasicUsage](Samples~/BasicUsage/AwaitableBasicUsage.unity)

## Tests

[Test sources](Tests)

Add the package ID to manifest `testables`, then execute its relevant EditMode/PlayMode assemblies in Unity Test Runner.

## Further reading

[Complete source map and package metadata](Documentation~/Overview.en.md)
- [Changelog](CHANGELOG.md)

## License

[License](LICENSE.md)
