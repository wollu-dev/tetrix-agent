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

        private const float Gamma = 0.99f;   // config의 gamma와 동일
        private float prevPotential;

        public override void OnEpisodeBegin()
        {
            board.ResetBoard();
            prevPotential = ComputePotential();   // 빈 보드 → 0
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

            // ---------- 1. 생존 보상 (지배적 신호: 오래 살수록 무조건 이득) ----------
            AddReward(0.1f);

            // ---------- 2. 줄 제거 보상 ----------
            int lines = board.GetLastClearedLines();
            if (lines > 0)
                AddReward(lines * lines * 1.0f);   // 1, 4, 9, 16

            // ---------- 3. potential 기반 shaping (자살 유인 없음) ----------
            float curPotential = ComputePotential();
            AddReward(Gamma * curPotential - prevPotential);
            prevPotential = curPotential;
        }

        // 보드가 깨끗할수록 0에 가깝고, 나쁠수록 큰 음수
        private float ComputePotential()
        {
            int agg   = GetAggregateHeight();
            int holes = board.GetHoleCount();
            int bump  = GetBumpiness();
            return -(0.02f * agg + 0.4f * holes + 0.12f * bump);
        }

        private int GetAggregateHeight()
        {
            int sum = 0;
            for (int x = 0; x < 10; x++)
                sum += board.GetColumnHeight(x);
            return sum;
        }

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