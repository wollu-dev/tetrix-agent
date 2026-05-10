# 🧱 Tetris ML-Agent

Unity ML-Agents를 활용하여 테트리스를 스스로 학습하는 강화학습 에이전트 프로젝트입니다.

> 오픈소스소프트웨어 수업 과제 — 정재윤, 허준혁

---

## 📌 프로젝트 개요

Unity로 테트리스 게임 환경을 직접 구현하고, ML-Agents Toolkit을 통해 PPO(Proximal Policy Optimization) 알고리즘으로 에이전트를 학습시킵니다. 에이전트는 블록의 배치와 줄 제거를 스스로 터득하며 최고 점수를 목표로 플레이합니다.

---

## 🎮 데모

> 학습 완료 후 추가 예정

| 학습 전 | 학습 후 |
|--------|--------|
| *(GIF 예정)* | *(GIF 예정)* |

### 학습 곡선 (Cumulative Reward)

> *(TensorBoard 그래프 이미지 예정)*

---

## 🛠 기술 스택

| 구분 | 내용 |
|------|------|
| 게임 엔진 | Unity 6000.0.40f1 |
| ML 프레임워크 | [Unity ML-Agents Toolkit](https://github.com/Unity-Technologies/ml-agents) |
| 학습 알고리즘 | PPO (Proximal Policy Optimization) |
| 언어 | C# (Unity), Python (ML-Agents 학습) |
| Python 버전 | 3.10.x (ML-Agents 권장) |

---

## 📐 환경 설계

### 관찰 공간 (Observation Space)

- 테트리스 보드 전체 셀 상태 (10×20 grid)
- 현재 블록의 종류 및 위치
- 다음 블록 정보

### 행동 공간 (Action Space)

| 행동 | 설명 |
|------|------|
| 이동 (좌/우) | 블록을 좌우로 이동 |
| 회전 | 블록 시계방향 회전 |
| 소프트 드롭 | 블록을 빠르게 아래로 이동 |
| 하드 드롭 | 블록을 즉시 바닥으로 낙하 |

### 보상 설계 (Reward Shaping)

| 조건 | 보상 |
|------|------|
| 줄 제거 (1줄) | +1.0 |
| 줄 제거 (2줄 이상) | +줄 수² (콤보 보너스) |
| 블록 착지 | +0.01 (생존 보상) |
| 게임 오버 | -1.0 |

---

## 📦 설치 및 실행

### 요구사항

- Unity **6000.0.40f1**
- Python **3.10.x**
- ml-agents **1.x** (`pip install mlagents`)

### 설치

```bash
# 레포지토리 클론
git clone https://github.com/{YOUR_REPO}/tetris-ml-agent.git
cd tetris-ml-agent

# Python 패키지 설치
pip install mlagents
```

### 학습 실행

```bash
# ML-Agents 학습 시작
mlagents-learn config/tetris_config.yaml --run-id=tetris_run_01
```

이후 Unity Editor에서 **Play** 버튼을 눌러 학습을 시작합니다.

### 학습 결과 확인 (TensorBoard)

```bash
tensorboard --logdir results/tetris_run_01
```

### 학습된 모델로 플레이

1. `results/tetris_run_01/` 안의 `.onnx` 파일을 Unity 프로젝트의 `Assets/Models/`에 복사
2. Unity Editor에서 Agent 오브젝트의 **Model** 필드에 해당 파일 할당
3. **Behavior Type**을 `Inference`로 변경 후 Play

---

## 📁 프로젝트 구조

```
tetris-ml-agent/
├── Assets/
│   ├── Scripts/
│   │   ├── TetrisAgent.cs       # ML-Agents Agent 구현
│   │   ├── TetrisBoard.cs       # 게임 보드 로직
│   │   ├── TetrisPiece.cs       # 블록 동작
│   │   └── GameManager.cs       # 게임 상태 관리
│   └── Models/                  # 학습된 .onnx 모델
├── config/
│   └── tetris_config.yaml       # PPO 하이퍼파라미터 설정
├── results/                     # 학습 로그 및 모델 저장
├── CONTRIBUTING.md
├── LICENSE
└── README.md
```

---

## 📊 결과

> 학습 완료 후 업데이트 예정

| 지표 | 값 |
|------|-----|
| 최고 점수 | - |
| 평균 줄 제거 수 | - |
| 학습 스텝 수 | - |
| 학습 소요 시간 | - |

---

## 📜 라이선스

이 프로젝트는 **MIT License** 하에 배포됩니다. 자세한 내용은 [LICENSE](./LICENSE) 파일을 참고하세요.

### 사용한 오픈소스 라이선스

| 라이브러리 | 라이선스 | 링크 |
|-----------|---------|------|
| Unity ML-Agents Toolkit | Apache License 2.0 | [GitHub](https://github.com/Unity-Technologies/ml-agents/blob/main/LICENSE.md) |

#### Apache License 2.0 주요 내용

- 소스코드 공개 의무 없이 자유롭게 사용, 수정, 배포 가능
- 수정 및 배포 시 원본 라이선스 및 저작권 고지 포함 필요
- 특허 사용 허가 및 특허 소송 제기 시 라이선스 자동 종료 조항 포함

---

## 👥 참여자

| 이름 | 역할 |
|------|------|
| 정재윤 | Unity 환경 구현, 보상 설계 |
| 허준혁 | ML-Agents 학습 설정, 결과 분석 |

---

## 🔗 참고 자료

- [Unity ML-Agents 공식 문서](https://unity-technologies.github.io/ml-agents/)
- [ML-Agents GitHub](https://github.com/Unity-Technologies/ml-agents)
- [PPO 논문 (Schulman et al., 2017)](https://arxiv.org/abs/1707.06347)
