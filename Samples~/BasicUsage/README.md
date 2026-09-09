# Basic Usage

`AwaitableBasicUsage` Scene을 열고 Play Mode를 시작합니다.

- 위쪽 큐브 세 개가 서로 다른 시간에 회색에서 초록색으로 바뀝니다.
- 세 작업을 모두 관찰한 `WhenAll`과 같은 조건을 기다린 `WaitUntil`/`WaitWhile`가 완료되면 아래쪽 큐브가 주황색에서 파란색으로 바뀝니다.
- Console에 완료 로그가 한 번 표시되고 예외가 없어야 합니다.

Scene을 종료하면 `destroyCancellationToken`이 진행 중인 대기를 취소합니다.
