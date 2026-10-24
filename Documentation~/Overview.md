# Jeomseon Unity Awaitable

[English](Overview.en.md) · [설치](../README.md)

## 목적

Unity Awaitable에 WhenAll과 WaitUntil/WaitWhile만 추가합니다. 자체 비동기 primitive나 PlayerLoop scheduler는 제공하지 않습니다.

## 사용 흐름

각 비동기 작업을 시작 → WhenAll에 각 Awaitable을 한 번 전달 → 반환 결과 한 번 await. 조건 대기는 predicate와 선택적 CancellationToken을 전달합니다.

## 계약과 제한

같은 Awaitable을 중복 소비하지 마세요. WhenAll은 전부 관찰한 뒤 입력 순서로 처음 관찰한 예외 하나를 던집니다. 조건이 이미 만족되면 취소된 토큰이어도 대기 없이 완료할 수 있습니다.

## 패키지 요구 사항

- Package ID: `com.jeomseon.unity.awaitable`
- Version: `0.1.0`
- Unity: `6000.6` / `0f1`

## 샘플과 검증

Package Manager > Samples에서 다음 예제를 Import하세요. 포함된 Scene 또는 Editor 도구를 열고 샘플 README의 Setup 절차를 먼저 수행합니다.

- [Basic Usage](../Samples~/BasicUsage/README.md)
  - [AwaitableBasicUsage](../Samples~/BasicUsage/AwaitableBasicUsage.unity)

[테스트 코드](../Tests)

소비 프로젝트 manifest의 `testables`에 패키지 ID를 추가하고 Unity Test Runner의 관련 어셈블리를 실행합니다. 위 링크는 테스트 위치이며 이번 문서 수정으로 새 테스트 실행을 주장하지 않습니다.

## 코드 지도

- [AwaitableUtility](../Runtime/AwaitableUtility.cs)
