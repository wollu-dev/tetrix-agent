using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using UnityEngine;
using DU.TetrisAgent.Core;

namespace DU.TetrisAgent.Agent
{
    public class TetrisAgent : Agent
    {
        [SerializeField] private TetrisBoard board;

        public override void OnEpisodeBegin()
        {
            board.ResetBoard();
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
            AddReward(board.GetStepReward()); // 게임오버 시 -1.0 포함
            if (board.IsGameOver())
                EndEpisode();
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
