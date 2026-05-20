using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using UnityEngine;
using DU.TetrisAgent.Core;

namespace DU.TetrisAgent.Agent
{
    public class TetrisAgent : Unity.MLAgents.Agent
    {
        [SerializeField] private TetrisBoard board;

        private int _prevHoleCount;

        public override void OnEpisodeBegin()
        {
            board.ResetBoard();
            _prevHoleCount = 0;
        }

        // 총 204 observations
        public override void CollectObservations(VectorSensor sensor)
        {
            sensor.AddObservation(board.GetBoardFlat());          // 200: 10×20 보드
            sensor.AddObservation((int)board.ActivePiece.Type);   // 1: 현재 피스 타입
            sensor.AddObservation((int)board.NextType);           // 1: 다음 피스 타입
            sensor.AddObservation(board.ActivePiece.Position.x);  // 1: 피스 X 위치
            sensor.AddObservation(board.ActivePiece.Rotation);    // 1: 피스 회전 상태
        }

        // 액션: 0=왼쪽, 1=오른쪽, 2=시계회전, 3=반시계회전, 4=하드드롭, 5=대기
        public override void OnActionReceived(ActionBuffers actions)
        {
            TetrisPiece piece = board.ActivePiece;
            int action = actions.DiscreteActions[0];

            switch (action)
            {
                case 0: piece.Move(Vector2Int.left);  break;
                case 1: piece.Move(Vector2Int.right); break;
                case 2: piece.Rotate(1);              break;
                case 3: piece.Rotate(-1);             break;
                case 4:
                    piece.HardDrop();
                    LockAndReward();
                    return;
                case 5: break;
            }

            // 매 스텝 중력 적용 — 아래로 못 내려가면 고정
            if (!piece.Move(Vector2Int.down))
                LockAndReward();
        }

        private void LockAndReward()
        {
            board.LockPiece();

            // 게임오버: 큰 패널티
            if (board.IsGameOver())
            {
                AddReward(-1f);
                EndEpisode();
                return;
            }

            int lines   = board.GetLastClearedLines();
            int holes   = board.GetHoleCount();
            int height  = board.GetMaxHeight();

            float reward = 0f;

            // 줄 제거: 제곱 보상으로 멀티라인 강하게 유도 (1줄=0.5, 2줄=2.0, 3줄=4.5, 4줄=8.0)
            reward += lines * lines * 0.5f;

            // 생존 보너스
            reward += 0.01f;

            // 높이 패널티: 쌓일수록 감점
            reward -= height * 0.005f;

            // 구멍 패널티: 이번 스텝에서 새로 생긴 구멍만 감점
            reward -= (holes - _prevHoleCount) * 0.1f;

            _prevHoleCount = holes;
            AddReward(reward);
        }

        public override void Heuristic(in ActionBuffers actionsOut)
        {
            var d = actionsOut.DiscreteActions;
            d[0] = 5;
            if (Input.GetKey(KeyCode.LeftArrow))  d[0] = 0;
            if (Input.GetKey(KeyCode.RightArrow)) d[0] = 1;
            if (Input.GetKey(KeyCode.UpArrow))    d[0] = 2;
            if (Input.GetKey(KeyCode.Z))          d[0] = 3;
            if (Input.GetKey(KeyCode.Space))      d[0] = 4;
        }
    }
}
