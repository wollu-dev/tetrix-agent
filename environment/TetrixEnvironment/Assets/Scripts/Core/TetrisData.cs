// Copyright (c) Jaeyun Jung. All rights reserved.

using UnityEngine;

namespace DU.TetrixAgent.Core {
    public enum TetrominoType {
        I, O, T, S, Z, J, L
    }
     
    public static class TetrisData {
        // 각 피스의 회전별 셀 좌표 (피벗 기준 상대 좌표)
        // [피스타입][회전(0~3)][셀인덱스] = Vector2Int
        public static readonly Vector2Int[,,] Cells = new Vector2Int[7, 4, 4] {
            // ── I ─────────────────────────────
            {
                { new(-1,0), new(0,0), new(1,0), new(2,0) },
                { new(1,-2), new(1,-1), new(1,0), new(1,1) },
                { new(-1,-1),new(0,-1),new(1,-1),new(2,-1) },
                { new(0,-2), new(0,-1), new(0,0), new(0,1) },
            },
            // ── O ─────────────────────────────
            {
                { new(0,0), new(1,0), new(0,-1), new(1,-1) },
                { new(0,0), new(1,0), new(0,-1), new(1,-1) },
                { new(0,0), new(1,0), new(0,-1), new(1,-1) },
                { new(0,0), new(1,0), new(0,-1), new(1,-1) },
            },
            // ── T ─────────────────────────────
            {
                { new(-1,0), new(0,0), new(1,0), new(0,1) },
                { new(0,-1), new(0,0), new(0,1), new(1,0) },
                { new(-1,0), new(0,0), new(1,0), new(0,-1) },
                { new(0,-1), new(0,0), new(0,1), new(-1,0) },
            },
            // ── S ─────────────────────────────
            {
                { new(-1,0), new(0,0), new(0,1), new(1,1) },
                { new(0,1),  new(0,0), new(1,0), new(1,-1) },
                { new(-1,-1),new(0,-1),new(0,0), new(1,0) },
                { new(-1,1), new(-1,0),new(0,0), new(0,-1) },
            },
            // ── Z ─────────────────────────────
            {
                { new(-1,1), new(0,1), new(0,0), new(1,0) },
                { new(1,1),  new(1,0), new(0,0), new(0,-1) },
                { new(-1,0), new(0,0), new(0,-1),new(1,-1) },
                { new(0,1),  new(0,0), new(-1,0),new(-1,-1)},
            },
            // ── J ─────────────────────────────
            {
                { new(-1,0), new(0,0), new(1,0), new(-1,1) },
                { new(0,1),  new(0,0), new(0,-1),new(1,1) },
                { new(-1,0), new(0,0), new(1,0), new(1,-1) },
                { new(0,1),  new(0,0), new(0,-1),new(-1,-1)},
            },
            // ── L ─────────────────────────────
            {
                { new(-1,0), new(0,0), new(1,0), new(1,1) },
                { new(0,1),  new(0,0), new(0,-1),new(1,-1) },
                { new(-1,0), new(0,0), new(1,0), new(-1,-1)},
                { new(0,1),  new(0,0), new(0,-1),new(-1,1) },
            },
        };
     
        // SRS Wall Kick 데이터 (J, L, S, T, Z 공통)
        // [현재회전 * 4 + 목표회전 방향] → kick 후보 목록
        public static readonly Vector2Int[,] WallKicksJLSTZ = new Vector2Int[8, 5] {
            { new(0,0), new(-1,0), new(-1,1), new(0,-2), new(-1,-2) }, // 0→1
            { new(0,0), new(1,0),  new(1,-1), new(0,2),  new(1,2)  }, // 1→0
            { new(0,0), new(1,0),  new(1,-1), new(0,2),  new(1,2)  }, // 1→2
            { new(0,0), new(-1,0), new(-1,1), new(0,-2), new(-1,-2) }, // 2→1
            { new(0,0), new(1,0),  new(1,1),  new(0,-2), new(1,-2) }, // 2→3
            { new(0,0), new(-1,0), new(-1,-1),new(0,2),  new(-1,2) }, // 3→2
            { new(0,0), new(-1,0), new(-1,-1),new(0,2),  new(-1,2) }, // 3→0
            { new(0,0), new(1,0),  new(1,1),  new(0,-2), new(1,-2) }, // 0→3
        };
     
        // I 피스 전용 Wall Kick
        public static readonly Vector2Int[,] WallKicksI = new Vector2Int[8, 5] {
            { new(0,0), new(-2,0), new(1,0),  new(-2,-1),new(1,2)  }, // 0→1
            { new(0,0), new(2,0),  new(-1,0), new(2,1),  new(-1,-2)}, // 1→0
            { new(0,0), new(-1,0), new(2,0),  new(-1,2), new(2,-1) }, // 1→2
            { new(0,0), new(1,0),  new(-2,0), new(1,-2), new(-2,1) }, // 2→1
            { new(0,0), new(2,0),  new(-1,0), new(2,1),  new(-1,-2)}, // 2→3
            { new(0,0), new(-2,0), new(1,0),  new(-2,-1),new(1,2)  }, // 3→2
            { new(0,0), new(1,0),  new(-2,0), new(1,-2), new(-2,1) }, // 3→0
            { new(0,0), new(-1,0), new(2,0),  new(-1,2), new(2,-1) }, // 0→3
        };
     
        // 피스별 색상
        public static readonly Color[] Colors = new Color[] {
            new Color(0.0f, 0.8f, 0.8f), // I - 시안
            new Color(0.8f, 0.8f, 0.0f), // O - 노랑
            new Color(0.6f, 0.0f, 0.8f), // T - 보라
            new Color(0.0f, 0.8f, 0.0f), // S - 초록
            new Color(0.8f, 0.0f, 0.0f), // Z - 빨강
            new Color(0.0f, 0.0f, 0.8f), // J - 파랑
            new Color(0.8f, 0.4f, 0.0f), // L - 주황
        };
    }
}
