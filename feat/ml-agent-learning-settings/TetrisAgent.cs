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

        public override void OnEpisodeBegin()
        {
            board.ResetBoard();
        }

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

            // 게임 오버
            if (board.IsGameOver())
            {
                AddReward(-1f);
                EndEpisode();
                return;
            }

            // 1. 생존 보상 — 블럭 하나 버틸 때마다 +. 항상 양수라 자살 자체가 불가능
            AddReward(0.05f);

            // 2. 줄 제거 — 24개 벽을 넘는 유일한 길. 크게 보상
            int lines = board.GetLastClearedLines();
            if (lines > 0)
                AddReward(lines * lines * 2.0f);   // 2, 8, 18, 32
        }

        public override void Heuristic(in ActionBuffers actionsOut)
        {
            var d = actionsOut.DiscreteActions;
            d[0] = 0;
            d[1] = 5;
        }
    }
}