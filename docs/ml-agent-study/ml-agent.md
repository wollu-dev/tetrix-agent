Unity ML-Agents 설치부터 예제까지 완전 정복 가이드 (https://elice.io/ko/resources/blog/unity-ml-agents)

Unity ML-Agents란?
Unity ML-Agents는 딥러닝이라는 AI 기술을 이용해서 게임이나 시뮬레이션 속 캐릭터들을 훈련시키고, 똑똑하게 움직이도록 만들어주는 아주 강력한 도구임

ML-Agents의 세 가지 핵심 요소
1. 학습 환경, 이건 우리가 흔히 보는 Unity 게임 화면, 즉 씬과 그 안의 모든 게임 캐릭터들을 포함함 
    그리고 AI 에이전트가 이 환경 속에서 보고, 행동하고, 배우게 됨
2. Python API 파이썬이라는 프로그래밍 언어를 이용해서 AI 에이전트의 행동을 훈련시키는 데 필요한 다양한 머신러닝 알고리즘들이 들어 있음
3. 외부 통신자를 통해 Unity와 정보를 주고받음, 게임 속에서 실제로 움직이고 행동하는 에이전트는 Unity의 게임 오브젝트에 붙어 있는 
    특별한 '뇌'와 연결됨 이 뇌는 최신 버전에서는 행동이라고 불리기도 하는데 에이전트가 주변 환경을 인식하고, 어떤 행동을 할지 결정하는 역할을 함

ML-Agents의 핵심 학습 방식은 강화 학습임
마치 어린아이가 넘어지고 다시 일어서는 과정을 통해 걷는 법을 배우듯이, AI 에이전트도 시행착오를 거치면서 보상을 최대화하는 방법을 스스로 터득함


강화 학습의 기본 개념
강화 학습은 에이전트가 시도와 오류를 통해 최적의 행동을 학습하는 방식, 에이전트는 행동에 대한 보상 또는 처벌을 받아 누적 보상을 극대화하는 방향으로 학습함

에이전트(Agent): 환경과 상호작용하며 행동을 결정하는 주체
환경(Environment): 에이전트가 행동하는 공간으로, Unity에서 자유롭게 구성할 수 있음
행동(Action): 에이전트가 환경에서 수행하는 동작임
보상(Reward): 에이전트의 행동 결과로 얻는 피드백으로, 이를 통해 에이전트는 더 나은 행동을 학습함

ex) 에이전트: 게임 내의 플레이어 캐릭터, NPC 등
환경: 장애물, 몬스터 등
행동: 이동, 점프, 달리기, 공격 등
상태: 에이전트의 위치, 주변 적의 위치 및 능력치, 체력, 속도 등
보상: 죽음 및 추락(-점수), 코인 획득 및 적 처치(+점수)


Unity ML-Agents 설치 및 실습 가이드

1단계. Anaconda 설치
    1. 아래 링크로 이동해 운영체제에 맞는 Anaconda 설치 파일을 다운로드합니다.
    <https://www.anaconda.com/download>

    2. 설치 후, “Anaconda PowerShell Prompt"를 실행합니다.


2단계. Python 3.10.12 가상환경 만들기
    1. 아래 명령어를 입력해 가상환경을 생성합니다.
        conda create -n ml-agents python=3.10.12 

    2. 가상환경을 활성화합니다.
        conda activate ml-agents 

    3. Python 버전을 확인합니다.
        python --version (Python 3.10.12가 출력되면 성공)


3단계. ML-Agents Toolkit 저장소 복사
    1. 아래 명령어를 차례대로 입력합니다.
        git clone https://github.com/Unity-Technologies/ml-agents 
        cd ml-agents 

    2. 아래 명령어로 필요한 패키지를 설치합니다.
        pip install -e ./ml-agents-envs
        pip install -e ./ml-agents


4단계. 설치 확인 및 학습 알고리즘 실행
    1. 아래 명령어를 입력합니다.
        mlagents-learn config/ppo/Basic.yaml --run-id=eliceBasic 

    2. unity 메시지가 뜨면 설치완료!
