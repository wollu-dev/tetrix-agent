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
        private int _prevMaxHeight;

        public override void OnEpisodeBegin()
        {
            board.ResetBoard();
            _prevHoleCount = 0;
            _prevMaxHeight = 0;
        }

        // 총 14 observations
        public override void CollectObservations(VectorSensor sensor)
        {
            // 보드 요약 (3개)
            sensor.AddObservation(board.GetHoleCount()  / 20f);
            sensor.AddObservation(board.GetMaxHeight()  / 20f);
            sensor.AddObservation(board.GetLastClearedLines() / 4f);

            // 현재 피스 정보 (4개)
            sensor.AddObservation((int)board.ActivePiece.Type    / 6f);
            sensor.AddObservation((int)board.NextType            / 6f);
            sensor.AddObservation(board.ActivePiece.Position.x   / 9f);
            sensor.AddObservation(board.ActivePiece.Position.y   / 20f);
        }

        public override void OnActionReceived(ActionBuffers actions)
        {
            TetrisPiece piece = board.ActivePiece;

            switch (actions.DiscreteActions[0])
            {
                case 0: piece.Move(Vector2Int.left);  break;
                case 1: piece.Move(Vector2Int.right); break;
                case 2: piece.Rotate(1);              break;
                case 3: piece.Rotate(-1);             break;
                case 4: piece.HardDrop(); break;
                case 5: break; // 대기
            }

            if (!piece.Move(Vector2Int.down))
            {
                board.LockPiece();
                ApplyReward();
            }
        }

        private void ApplyReward()
        {
            if (board.IsGameOver())
            {
                AddReward(-2.0f);
                EndEpisode();
                return;
            }

            float reward = 0f;

            // 줄 제거 보상 (핵심 보상!)
            int lines = board.GetLastClearedLines();
            if (lines == 1) reward += 1.0f;
            if (lines == 2) reward += 3.0f;
            if (lines == 3) reward += 6.0f;
            if (lines == 4) reward += 10.0f;

            // 생존 보상
            reward += 0.05f;

            // 높이 낮출수록 보상
            int curHeight = board.GetMaxHeight();
            if (curHeight < _prevMaxHeight)
                reward += 0.1f;
            _prevMaxHeight = curHeight;

            // 구멍 패널티
            int holes = board.GetHoleCount();
            int newHoles = holes - _prevHoleCount;
            if (newHoles > 0)
                reward -= newHoles * 0.3f;
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