# Jeomseon Unity Awaitable

[한국어](Overview.md) · [Installation](../README.en.md)

## Purpose

Adds WhenAll and WaitUntil/WaitWhile to Unity Awaitable, without a custom async primitive or PlayerLoop scheduler.

## Usage flow

Start operations, pass each Awaitable once to WhenAll, and await the result once. Condition waits accept a predicate and optional CancellationToken.

## Contracts and limitations

Never consume the same Awaitable twice. WhenAll observes all entries and rethrows the first exception observed in input order. An already satisfied predicate may complete immediately even with a canceled token.

## Package requirements

- Package ID: `com.jeomseon.unity.awaitable`
- Version: `0.1.0`
- Unity: `6000.6` / `0f1`

## Samples and verification

Import the following from Package Manager > Samples. Open the included Scene or Editor tool; follow any Setup step in its sample README.

- [Basic Usage](../Samples~/BasicUsage/README.md)
  - [AwaitableBasicUsage](../Samples~/BasicUsage/AwaitableBasicUsage.unity)

[Test sources](../Tests)

Add the package ID to manifest `testables`, then run the relevant Unity Test Runner assemblies. These links describe available tests, not a new successful test run.

## Source map

- [AwaitableUtility](../Runtime/AwaitableUtility.cs)
