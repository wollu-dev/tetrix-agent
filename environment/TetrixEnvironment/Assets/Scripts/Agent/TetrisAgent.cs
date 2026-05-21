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

        // 총 15 observations
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

            if (board.IsGameOver())
            {
                AddReward(-1f);
                EndEpisode();
                return;
            }

            // 줄 제거만 보상! (단순하고 강한 신호)
            int lines = board.GetLastClearedLines();
            if (lines > 0)
                AddReward(lines * lines * 1.0f);

            // 구멍 패널티만 (약하게)
            AddReward(-board.GetHoleCount() * 0.005f);
        }

        public override void Heuristic(in ActionBuffers actionsOut)
        {
            var d = actionsOut.DiscreteActions;
            d[0] = 0;
            d[1] = 5;
        }
    }
}