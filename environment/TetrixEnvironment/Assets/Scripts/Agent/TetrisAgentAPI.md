# TetrisBoard Agent API

`TetrisBoard` 가 제공하는 ML-Agents 연동용 공개 메서드 목록입니다.
에피소드 루프에서 아래 순서대로 호출합니다.

```
OnEpisodeBegin  →  board.ResetBoard()
                   board.SpawnNextPiece()  (이미 ResetBoard 내부에서 호출됨)

매 스텝         →  액션 실행 (Move / Rotate / HardDrop)
                   board.LockPiece()
                   보상 = board.GetStepReward()
                   종료 = board.IsGameOver()

CollectObs      →  아래 관찰값 API 사용
```

---

## 에피소드 제어

| 메서드 | 반환 | 설명 |
|--------|------|------|
| `ResetBoard()` | void | 보드·점수 전체 초기화. 에피소드 시작 시 1회 호출 |
| `SpawnNextPiece()` | bool | 다음 피스 스폰. false 이면 게임오버 |
| `LockPiece()` | void | 현재 피스 고정 + 라인 클리어 + 점수 갱신. 스텝 종료 시 호출 |
| `IsGameOver()` | bool | 게임오버 여부. LockPiece() 이후 확인 |

---

## 점수 시스템 API

`LockPiece()` 직후 호출해야 최신 값이 반영됩니다.

| 메서드 | 반환 | 설명 |
|--------|------|------|
| `GetScore()` | int | 누적 점수 |
| `GetLevel()` | int | 현재 레벨 (1 시작, 10줄마다 +1) |
| `GetTotalLinesCleared()` | int | 누적 클리어 라인 수 |
| `GetScoreDelta()` | int | 이번 스텝 점수 증분 (라인 클리어 없으면 0) |
| `GetStepReward()` | float | B-style 보상 (아래 공식 참고) |
| `GetLastClearedLines()` | int | 이번 스텝에서 클리어한 라인 수 |

### 점수표 (최소 구현)

| 클리어 라인 | 점수 |
|------------|------|
| 1줄 | 100 × 레벨 |
| 2줄 | 300 × 레벨 |
| 3줄 | 500 × 레벨 |
| 4줄 (Tetris) | 800 × 레벨 |

### GetStepReward() 공식

```
reward = GetScoreDelta() × 0.01          // 줄 삭제 보상
       - (구멍 증가량)    × 0.1           // 구멍 생성 패널티
       - (울퉁불퉁 증가량) × 0.05         // 높이 불균형 패널티
       + 0.01                             // 생존 보너스

게임오버 시 → -1.0 고정
```

계수를 직접 조정하고 싶다면 `GetScoreDelta()`, `GetHoleCount()`, `GetBumpiness()` 를
개별 호출해 커스텀 보상 함수를 만드세요.

---

## 관찰값(Observation) API

`CollectObservations()` 에서 사용할 보드 상태 메서드입니다.

| 메서드 | 반환 | 설명 |
|--------|------|------|
| `GetBoardFlat()` | float[200] | 10×20 보드 전체 (1=채워짐, 0=빔). y축 정방향 순 |
| `GetCell(x, y)` | bool | 특정 셀 상태. (0,0) = 좌측 하단 |
| `GetColumnHeight(x)` | int | 컬럼 x의 최고 높이 (0 = 비어있음) |
| `GetMaxHeight()` | int | 전체 컬럼 중 최고 높이 |
| `GetHoleCount()` | int | 채워진 셀 아래 빈 셀 수 (구멍) |
| `GetBumpiness()` | int | 인접 컬럼 높이 차의 합 |
| `GetWeightedHoles()` | int | 깊이 가중 구멍 수 (깊을수록 값 큼) |

### 피스 상태 (ActivePiece 프로퍼티)

```csharp
board.ActivePiece.Type      // TetrominoType (I/O/T/S/Z/J/L)
board.ActivePiece.Position  // Vector3Int 현재 위치
board.ActivePiece.Rotation  // int 0~3
board.NextType              // TetrominoType 다음 피스
```

---

## 액션 API (TetrisPiece)

`board.ActivePiece` 를 통해 호출합니다.

| 메서드 | 반환 | 설명 |
|--------|------|------|
| `Move(Vector2Int.left)` | bool | 왼쪽 이동. 실패 시 false |
| `Move(Vector2Int.right)` | bool | 오른쪽 이동. 실패 시 false |
| `SoftDrop()` | bool | 한 칸 아래로. 실패 시 false |
| `HardDrop()` | int | 바닥까지 즉시 하강. 내려간 칸 수 반환 |
| `Rotate(1)` | bool | 시계방향 회전 (SRS 포함) |
| `Rotate(-1)` | bool | 반시계방향 회전 (SRS 포함) |

---

## 사용 예시 (에피소드 루프)

```csharp
// OnEpisodeBegin
board.ResetBoard();

// OnActionReceived
public override void OnActionReceived(ActionBuffers actions) {
    int action = actions.DiscreteActions[0];

    switch (action) {
        case 0: board.ActivePiece.Move(Vector2Int.left);  break;
        case 1: board.ActivePiece.Move(Vector2Int.right); break;
        case 2: board.ActivePiece.Rotate(1);              break;
        case 3: board.ActivePiece.Rotate(-1);             break;
        case 4: board.ActivePiece.HardDrop();
                board.LockPiece();
                AddReward(board.GetStepReward());
                if (board.IsGameOver()) EndEpisode();
                break;
    }
}

// CollectObservations
public override void CollectObservations(VectorSensor sensor) {
    sensor.AddObservation(board.GetBoardFlat());          // 200
    sensor.AddObservation((int)board.ActivePiece.Type);   // 1
    sensor.AddObservation((int)board.NextType);           // 1
    sensor.AddObservation(board.ActivePiece.Position.x);  // 1
    sensor.AddObservation(board.ActivePiece.Rotation);    // 1
    // 총 204 observations
}
```