// Copyright (c) Jaeyun Jung. All rights reserved.

using TMPro;
using UnityEngine;

namespace DU.TetrisAgent.Core {
    /// <summary>
    /// TetrisBoard 게임오브젝트에 붙이면 보드 좌측 상단에 점수 텍스트를 자동 생성합니다.
    /// </summary>
    [RequireComponent(typeof(TetrisBoard))]
    public class ScoreDisplay : MonoBehaviour {
        [Header("표시 설정")]
        [SerializeField] private float fontSize     = 8.0f;
        [SerializeField] private Color textColor    = Color.white;

        private TetrisBoard _board;
        private TextMeshPro _text;

        private void Start() {
            _board = GetComponent<TetrisBoard>();

            var go = new GameObject("ScoreText");
            go.transform.SetParent(transform, false);

            // 보드 좌측 상단 바깥 (보더 왼쪽, 보드 상단 위)
            go.transform.localPosition = new Vector3(-6f, _board.Height - 2f, 0f);

            _text = go.AddComponent<TextMeshPro>();
            _text.fontSize              = fontSize;
            _text.color                 = textColor;
            _text.alignment             = TextAlignmentOptions.TopLeft;
            _text.rectTransform.sizeDelta = new Vector2(8f, 4f);

            // 타일맵보다 앞에 렌더링
            var mr = go.GetComponent<MeshRenderer>();
            mr.sortingLayerName = "Default";
            mr.sortingOrder     = 10;
        }

        private void LateUpdate() {
            if (_text == null) return;
            _text.text = "<mspace=0.65em>"
                       + $"SCORE {_board.GetScore(),7:#,0}\n"
                       + $"LEVEL {_board.GetLevel(),7}\n"
                       + $"LINES {_board.GetTotalLinesCleared(),7}"
                       + "</mspace>";
        }
    }
}
