// Copyright (c) Jaeyun Jung. All rights reserved.

using UnityEngine;
using UnityEngine.Tilemaps;

namespace DU.TetrixAgent.Core {
    /// <summary>
    /// 테트리스 보드 핵심 로직
    /// - 보드 상태 관리
    /// - 피스 스폰 / 고정 / 라인 클리어
    /// - ML-Agents용 보드 분석 메서드 제공
    /// </summary>
    public class TetrisBoard : MonoBehaviour {
        // ─────────────────────────────────────────────────────────────
        // 프로퍼티 (Inspector / 외부 접근)
        // ─────────────────────────────────────────────────────────────
        public int Width  => width;
        public int Height => height;
        
        // ─────────────────────────────────────────────────────────────
        // 인스펙터 설정
        // ─────────────────────────────────────────────────────────────
        [Header("보드 크기")]
        [SerializeField] private int width  = 10;
        [SerializeField] private int height = 20;
     
        [Header("타일맵")]
        [SerializeField] private Tilemap boardTilemap;   // 고정된 블록
        [SerializeField] private Tilemap pieceTilemap;   // 현재 낙하 중인 피스
     
        [Header("타일 리소스")]
        [SerializeField] private TileBase[] tiles;       // 피스 종류별 타일 (7개)
        [SerializeField] private TileBase   ghostTile;   // 고스트 피스 타일
     
        [Header("스폰 설정")]
        [SerializeField] private Vector3Int spawnPosition = new(3, 18, 0);
     
        // ─────────────────────────────────────────────────────────────
        // 런타임 상태
        // ─────────────────────────────────────────────────────────────
        public  TetrisPiece ActivePiece { get; private set; }
        public  TetrominoType NextType  { get; private set; }
     
        private bool[,]  _grid;                // 고정된 셀 (true = 채워짐)
        private int      _lastClearedLines;
        private bool     _isGameOver;
     
        // 보드 경계 (왼쪽 하단 기준)
        public RectInt Bounds => new RectInt(0, 0, width, height);
     
        // ─────────────────────────────────────────────────────────────
        // 초기화
        // ─────────────────────────────────────────────────────────────
        private void Awake() {
            ActivePiece = GetComponentInChildren<TetrisPiece>();
            _grid = new bool[width, height];
        }
     
        /// <summary>
        /// 보드 완전 초기화 (에피소드 시작 시 호출)
        /// </summary>
        public void ResetBoard() {
            boardTilemap.ClearAllTiles();
            pieceTilemap.ClearAllTiles();
     
            _grid = new bool[width, height];
            _lastClearedLines = 0;
            _isGameOver       = false;
     
            NextType = RandomPieceType();
            SpawnNextPiece();
        }
     
        // ─────────────────────────────────────────────────────────────
        // 피스 스폰
        // ─────────────────────────────────────────────────────────────
     
        /// <summary>
        /// 다음 피스를 스폰. 공간이 없으면 게임오버 처리 후 false 반환.
        /// </summary>
        public bool SpawnNextPiece() {
            TetrominoType type = NextType;
            NextType = RandomPieceType();
     
            ActivePiece.Initialize(this, spawnPosition, type, tiles[(int)type]);
     
            // 스폰 위치가 막혀있으면 게임오버
            if (!IsValidPosition(ActivePiece, spawnPosition))
            {
                _isGameOver = true;
                return false;
            }
     
            SetPiece(ActivePiece);
            DrawGhost();
            return true;
        }
     
        private TetrominoType RandomPieceType()
            => (TetrominoType)Random.Range(0, System.Enum.GetValues(typeof(TetrominoType)).Length);
     
        // ─────────────────────────────────────────────────────────────
        // 피스 고정
        // ─────────────────────────────────────────────────────────────
     
        /// <summary>
        /// 현재 피스를 보드에 고정하고 라인 클리어 처리
        /// </summary>
        public void LockPiece() {
            // pieceTilemap → boardTilemap 으로 이전
            foreach (Vector3Int cell in ActivePiece.Cells) {
                Vector3Int pos = ActivePiece.Position + cell;
     
                // 보드 범위 내에서만 기록
                if (IsInBounds(pos)) {
                    boardTilemap.SetTile(pos, ActivePiece.Tile);
                    _grid[pos.x, pos.y] = true;
                }
            }
     
            pieceTilemap.ClearAllTiles();
     
            _lastClearedLines = ClearLines();
        }
     
        // ─────────────────────────────────────────────────────────────
        // 라인 클리어
        // ─────────────────────────────────────────────────────────────
        private int ClearLines() {
            int cleared = 0;
     
            for (int y = height - 1; y >= 0; y--) {
                if (IsLineFull(y)) {
                    ClearLine(y);
                    ShiftLinesDown(y);
                    y++;          // 같은 줄 다시 검사
                    cleared++;
                }
            }
     
            return cleared;
        }
     
        private bool IsLineFull(int y) {
            for (int x = 0; x < width; x++)
                if (!_grid[x, y]) return false;
            return true;
        }
     
        private void ClearLine(int y) {
            for (int x = 0; x < width; x++) {
                boardTilemap.SetTile(new Vector3Int(x, y, 0), null);
                _grid[x, y] = false;
            }
        }
     
        private void ShiftLinesDown(int clearedY) {
            for (int y = clearedY; y < height - 1; y++) {
                for (int x = 0; x < width; x++) {
                    Vector3Int from = new(x, y + 1, 0);
                    Vector3Int to   = new(x, y,     0);
     
                    boardTilemap.SetTile(to, boardTilemap.GetTile(from));
                    _grid[x, y] = _grid[x, y + 1];
                }
            }
     
            // 맨 위 줄 비우기
            for (int x = 0; x < width; x++) {
                boardTilemap.SetTile(new Vector3Int(x, height - 1, 0), null);
                _grid[x, height - 1] = false;
            }
        }
     
        // ─────────────────────────────────────────────────────────────
        // 피스 렌더링 유틸
        // ─────────────────────────────────────────────────────────────
     
        public void SetPiece(TetrisPiece piece) {
            foreach (Vector3Int cell in piece.Cells)
                pieceTilemap.SetTile(piece.Position + cell, piece.Tile);
        }
     
        public void ClearPiece(TetrisPiece piece) {
            foreach (Vector3Int cell in piece.Cells)
                pieceTilemap.SetTile(piece.Position + cell, null);
        }
     
        // ─────────────────────────────────────────────────────────────
        // 고스트 피스 (하드드롭 위치 미리보기)
        // ─────────────────────────────────────────────────────────────
        private void DrawGhost() {
            if (ghostTile == null) return;
     
            // 기존 고스트 지우기
            // (별도 ghostTilemap을 두는 것도 좋은 방법)
            Vector3Int ghostPos = GetGhostPosition();
     
            foreach (Vector3Int cell in ActivePiece.Cells) {
                Vector3Int pos = ghostPos + cell;
                if (IsInBounds(pos) && pieceTilemap.GetTile(pos) == null)
                    pieceTilemap.SetTile(pos, ghostTile);
            }
        }
     
        private Vector3Int GetGhostPosition() {
            Vector3Int pos = ActivePiece.Position;
            while (IsValidPosition(ActivePiece, pos + Vector3Int.down))
                pos += Vector3Int.down;
            return pos;
        }
     
        // ─────────────────────────────────────────────────────────────
        // 유효성 검사
        // ─────────────────────────────────────────────────────────────
     
        public bool IsValidPosition(TetrisPiece piece, Vector3Int position) {
            foreach (Vector3Int cell in piece.Cells) {
                Vector3Int pos = position + cell;
     
                if (!IsInBounds(pos)) return false;
                if (_grid[pos.x, pos.y]) return false;
            }
            return true;
        }
     
        private bool IsInBounds(Vector3Int pos)
            => pos.x >= 0 && pos.x < width && pos.y >= 0 && pos.y < height;
     
        // ─────────────────────────────────────────────────────────────
        // ML-Agents 보드 분석 API
        // ─────────────────────────────────────────────────────────────
     
        /// <summary>보드 셀 반환 (0,0 = 좌측 하단)</summary>
        public bool GetCell(int x, int y) => _grid[x, y];
     
        /// <summary>컬럼별 최고 높이 (0 = 비어있음)</summary>
        public int GetColumnHeight(int x) {
            for (int y = height - 1; y >= 0; y--)
                if (_grid[x, y]) return y + 1;
            return 0;
        }
     
        /// <summary>전체 최고 높이</summary>
        public int GetMaxHeight() {
            int max = 0;
            for (int x = 0; x < width; x++)
                max = Mathf.Max(max, GetColumnHeight(x));
            return max;
        }
     
        /// <summary>
        /// 구멍 수: 채워진 셀 아래에 있는 빈 셀 수
        /// </summary>
        public int GetHoleCount() {
            int holes = 0;
            for (int x = 0; x < width; x++) {
                bool blockAbove = false;
                for (int y = height - 1; y >= 0; y--) {
                    if (_grid[x, y])
                        blockAbove = true;
                    else if (blockAbove)
                        holes++;
                }
            }
            return holes;
        }
     
        /// <summary>
        /// 울퉁불퉁함: 인접 컬럼 높이 차의 합
        /// </summary>
        public int GetBumpiness() {
            int bumpiness = 0;
            for (int x = 0; x < width - 1; x++)
                bumpiness += Mathf.Abs(GetColumnHeight(x) - GetColumnHeight(x + 1));
            return bumpiness;
        }
     
        /// <summary>
        /// 덮인 구멍 (깊이 가중치 포함): 더 깊은 구멍일수록 높은 패널티
        /// </summary>
        public int GetWeightedHoles() {
            int score = 0;
            for (int x = 0; x < width; x++) {
                int depth = 0;
                bool blockAbove = false;
                for (int y = height - 1; y >= 0; y--) {
                    if (_grid[x, y])
                        blockAbove = true;
                    else if (blockAbove)
                        score += ++depth;
                }
            }
            return score;
        }
     
        /// <summary>
        /// 마지막 스텝에서 클리어된 라인 수
        /// </summary>
        public int GetLastClearedLines() => _lastClearedLines;
     
        /// <summary>게임 오버 여부</summary>
        public bool IsGameOver() => _isGameOver;
     
        /// <summary>
        /// 완전한 보드 플랫 배열 반환 (ML observation용, 200 float)
        /// </summary>
        public float[] GetBoardFlat() {
            float[] flat = new float[width * height];
            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                    flat[y * width + x] = _grid[x, y] ? 1f : 0f;
            return flat;
        }
    }
}
