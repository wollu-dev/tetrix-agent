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

        // 보드 상태 추적용 (delta 보상 계산)
        private int prevHoles;
        private int prevBumpiness;

        public override void OnEpisodeBegin()
        {
            board.ResetBoard();
            // 보드가 비었으므로 초기값 0
            prevHoles     = 0;
            prevBumpiness = 0;
        }

        // 총 15 observations (Space Size 15 그대로, 인스펙터 수정 불필요)
        public override void CollectObservations(VectorSensor sensor)
        {
            for (int x = 0; x < 10; x++)
                sensor.AddObservation(board.GetColumnHeight(x) / 20f);

            sensor.AddObservation(board.GetHoleCount()  / 50f);
            sensor.AddObservation(board.GetMaxHeight()  / 20f);
            sensor.AddObservation((int)board.ActivePiece.Type / 6f);
            sensor.AddObservation((int)board.NextType         / 6f);
            sensor.AddObservation(board.ActivePiece.Position.x / 9f);
        }

        // Branch 0: 회전 (0-3), Branch 1: 목표 컬럼 (0-9)
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

            // ---------- 게임 오버 ----------
            if (board.IsGameOver())
            {
                AddReward(-1f);
                EndEpisode();
                return;
            }

            // ---------- 1. 생존 보상 (오래 버티기) ----------
            AddReward(0.02f);

            // ---------- 2. 줄 제거 보상 (가장 강한 신호) ----------
            int lines = board.GetLastClearedLines();
            if (lines > 0)
                AddReward(lines * lines * 1.0f);   // 1, 4, 9, 16

            // ---------- 3. delta 기반 보드 상태 평가 ----------
            int holes     = board.GetHoleCount();
            int bumpiness = GetBumpiness();

            int dHoles = holes     - prevHoles;       // 늘면 +, 줄면 -
            int dBump  = bumpiness - prevBumpiness;

            // 구멍이 늘면 벌점 (줄면 보상)
            AddReward(-0.5f * dHoles);
            // 표면이 울퉁불퉁해지면 벌점 → 가운데 쌓기 직접 억제
            AddReward(-0.2f * dBump);

            prevHoles     = holes;
            prevBumpiness = bumpiness;
        }

        // 인접 컬럼 높이차의 합 (표면 거칠기)
        private int GetBumpiness()
        {
            int sum = 0;
            for (int x = 0; x < 9; x++)
                sum += Mathf.Abs(board.GetColumnHeight(x) - board.GetColumnHeight(x + 1));
            return sum;
        }

        public override void Heuristic(in ActionBuffers actionsOut)
        {
            var d = actionsOut.DiscreteActions;
            d[0] = 0;
            d[1] = 5;
        }
    }
}