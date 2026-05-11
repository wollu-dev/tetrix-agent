// Copyright (c) Jaeyun Jung. All rights reserved.

using DU.TetrisAgent.Core;
using UnityEngine;

namespace DU.TetrisAgent.Agent {
    /// <summary>
    /// 사람이 직접 플레이 하는 에이전트
    /// </summary>
    public class Player : MonoBehaviour {
        // ─────────────────────────────────────────────────────────────
        // 참조
        // ─────────────────────────────────────────────────────────────
        [SerializeField] private TetrisBoard board;

        // ─────────────────────────────────────────────────────────────
        // 게임 시작 및 조작
        // ─────────────────────────────────────────────────────────────
        private float _fallTimer;
        private const float FALL_INTERVAL = 1f;

        private void Start() => board.ResetBoard();

        private void Update() {
            if (board.IsGameOver()) return;

            TetrisPiece piece = board.ActivePiece;

            // 이동
            if (Input.GetKeyDown(KeyCode.LeftArrow))  piece.Move(Vector2Int.left);
            if (Input.GetKeyDown(KeyCode.RightArrow)) piece.Move(Vector2Int.right);

            // 회전
            if (Input.GetKeyDown(KeyCode.UpArrow)) piece.Rotate(1);
            if (Input.GetKeyDown(KeyCode.Z))       piece.Rotate(-1);

            // 하드 드롭 — 즉시 고정 후 리턴 (중력 타이머와 충돌 방지)
            if (Input.GetKeyDown(KeyCode.Space)) {
                piece.HardDrop();
                board.LockPiece();
                _fallTimer = 0f;
                return;
            }

            // 소프트 드롭 (↓ 누르는 동안 중력 가속)
            if (Input.GetKey(KeyCode.DownArrow))
                _fallTimer += Time.deltaTime * 9f;

            // 중력 — 타이머가 다 차면 한 칸 낙하, 못 내려가면 고정
            _fallTimer += Time.deltaTime;
            if (_fallTimer >= FALL_INTERVAL) {
                _fallTimer = 0f;
                if (!piece.Move(Vector2Int.down))
                    board.LockPiece();
            }
        }
    }
}