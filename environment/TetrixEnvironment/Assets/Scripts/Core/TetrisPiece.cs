// Copyright (c) Jaeyun Jung. All rights reserved.

using UnityEngine;
using UnityEngine.Tilemaps;

namespace DU.TetrisAgent.Core {
    /// <summary>
    /// 현재 낙하 중인 피스의 상태 및 이동/회전 처리
    /// </summary>
    public class TetrisPiece : MonoBehaviour {
        // ─────────────────────────────────────────────────────────────
        // 참조
        // ─────────────────────────────────────────────────────────────
        public TetrisBoard Board    { get; private set; }
        public TileBase    Tile     { get; private set; }
     
        // ─────────────────────────────────────────────────────────────
        // 상태
        // ─────────────────────────────────────────────────────────────
        public TetrominoType Type     { get; private set; }
        public Vector3Int    Position { get; private set; }
        public Vector3Int[]  Cells    { get; private set; }
        public int           Rotation { get; private set; }
     
        // ─────────────────────────────────────────────────────────────
        // 초기화
        // ─────────────────────────────────────────────────────────────
        public void Initialize(TetrisBoard board, Vector3Int position, TetrominoType type, TileBase tile) {
            Board    = board;
            Position = position;
            Type     = type;
            Tile     = tile;
            Rotation = 0;
            Cells    = new Vector3Int[4];
     
            SetRotation(0);
        }
     
        // ─────────────────────────────────────────────────────────────
        // 이동
        // ─────────────────────────────────────────────────────────────
     
        /// <summary>
        /// 방향으로 이동 시도. 성공 여부 반환.
        /// </summary>
        public bool Move(Vector2Int direction) {
            Vector3Int newPos = Position + new Vector3Int(direction.x, direction.y, 0);
     
            if (!Board.IsValidPosition(this, newPos))
                return false;

            Position = newPos;
            Board.RefreshPiece();
            return true;
        }
     
        // ─────────────────────────────────────────────────────────────
        // 회전 (SRS Wall Kick 포함)
        // ─────────────────────────────────────────────────────────────
     
        /// <summary>
        /// direction: 1 = 시계방향, -1 = 반시계방향
        /// </summary>
        public bool Rotate(int direction) {
            int originalRotation = Rotation;
            int newRotation = WrapRotation(Rotation + direction);
     
            SetRotation(newRotation);

            if (TryWallKick(originalRotation, newRotation))
                return true;

            // 회전 불가 → 원복
            SetRotation(originalRotation);
            Board.RefreshPiece();
            return false;
        }
     
        private bool TryWallKick(int from, int to) {
            int kickIndex = GetWallKickIndex(from, to);
            var kicks = (Type == TetrominoType.I)
                ? TetrisData.WallKicksI
                : TetrisData.WallKicksJLSTZ;
     
            for (int i = 0; i < 5; i++) {
                Vector2Int kick = kicks[kickIndex, i];
                Vector3Int testPos = Position + new Vector3Int(kick.x, kick.y, 0);
     
                if (Board.IsValidPosition(this, testPos)) {
                    Position = testPos;
                    Board.RefreshPiece();
                    return true;
                }
            }
            return false;
        }
     
        private int GetWallKickIndex(int from, int to) {
            // SRS 표 인덱스 매핑
            if (from == 0 && to == 1) return 0;
            if (from == 1 && to == 0) return 1;
            if (from == 1 && to == 2) return 2;
            if (from == 2 && to == 1) return 3;
            if (from == 2 && to == 3) return 4;
            if (from == 3 && to == 2) return 5;
            if (from == 3 && to == 0) return 6;
            if (from == 0 && to == 3) return 7;
            return 0;
        }
     
        // ─────────────────────────────────────────────────────────────
        // 드롭
        // ─────────────────────────────────────────────────────────────
     
        /// <summary>
        /// 소프트 드롭: 한 칸 아래로
        /// </summary>
        public bool SoftDrop() => Move(Vector2Int.down);
     
        /// <summary>
        /// 하드 드롭: 바닥까지 즉시 내려서 고정
        /// </summary>
        public int HardDrop() {
            int dropped = 0;
            while (Move(Vector2Int.down))
                dropped++;
            return dropped;
        }
     
        // ─────────────────────────────────────────────────────────────
        // 내부 유틸
        // ─────────────────────────────────────────────────────────────
     
        private void SetRotation(int rotation) {
            Rotation = rotation;
            int t = (int)Type;
     
            for (int i = 0; i < 4; i++)
            {
                Vector2Int cell = TetrisData.Cells[t, rotation, i];
                Cells[i] = new Vector3Int(cell.x, cell.y, 0);
            }
        }
     
        private int WrapRotation(int rotation) => ((rotation % 4) + 4) % 4;
    }
}
