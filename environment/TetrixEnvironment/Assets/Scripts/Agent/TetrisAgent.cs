using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using UnityEngine;
using DU.TetrisAgent.Core;

namespace DU.TetrisAgent.Agent
{
    // 배치 기반 에이전트: 피스 1개당 결정 1번 (회전 + 목표 컬럼)
    // Behavior Parameters: Branch 0 = 4 (회전), Branch 1 = 10 (컬럼)
    public class TetrisAgent : Unity.MLAgents.Agent
    {
        [SerializeField] private TetrisBoard board;

        private int _prevHoleCount;

        public override void OnEpisodeBegin()
        {
            board.ResetBoard();
            _prevHoleCount = 0;
        }

        // 총 15 observations
        public override void CollectObservations(VectorSensor sensor)
        {
            for (int x = 0; x < 10; x++)
                sensor.AddObservation(board.GetColumnHeight(x) / 20f);

            sensor.AddObservation(board.GetHoleCount()  / 100f);
            sensor.AddObservation(board.GetBumpiness()  / 50f);
            sensor.AddObservation(board.GetMaxHeight()  / 20f);

            sensor.AddObservation((int)board.ActivePiece.Type / 6f);
            sensor.AddObservation((int)board.NextType         / 6f);
        }

        // Branch 0: 목표 회전 (0~3), Branch 1: 목표 컬럼 (0~9)
        public override void OnActionReceived(ActionBuffers actions)
        {
            int targetRotation = actions.DiscreteActions[0];
            int targetColumn   = actions.DiscreteActions[1];

            TetrisPiece piece = board.ActivePiece;

            for (int i = 0; i < targetRotation; i++)
                piece.Rotate(1);

            int dx = targetColumn - piece.Position.x;
            Vector2Int dir = dx > 0 ? Vector2Int.right : Vector2Int.left;
            for (int i = 0; i < Mathf.Abs(dx); i++)
                if (!piece.Move(dir)) break;

            piece.HardDrop();
            board.LockPiece();

            ApplyReward();
        }

        private void ApplyReward()
        {
            if (board.IsGameOver())
            {
                AddReward(-1.0f);
                EndEpisode();
                return;
            }

            float reward = 0f;

            // 줄 제거 보상 — 1줄=1, 2줄=4, 3줄=9, 4줄=16
            int lines = board.GetLastClearedLines();
            reward += lines * lines;

            // 행 완성도 보상 — 가로줄이 채워질수록 즉시 보상
            // 9/10 채워지면 0.81×0.05=0.04, 완성에 가까울수록 강하게 유도
            for (int y = 0; y < 20; y++)
            {
                int filled = 0;
                for (int x = 0; x < 10; x++)
                    if (board.GetCell(x, y)) filled++;
                float ratio = filled / 10f;
                reward += ratio * ratio * 0.05f;
            }

            // 생존 보상
            reward += 0.01f;

            // 구멍 패널티 — 새로 생긴 구멍만
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
            d[0] = 0;
            d[1] = 4;
        }
    }
}
