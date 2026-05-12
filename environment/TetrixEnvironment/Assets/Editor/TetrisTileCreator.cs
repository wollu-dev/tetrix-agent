// Copyright (c) Jaeyun Jung. All rights reserved.

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace DU.TetrixAgent.Editor {
    /// <summary>
    /// 테트리스용 단색 Tile 에셋을 자동 생성하는 에디터 유틸리티
    /// 메뉴: Tools > Tetris > Create Tiles
    /// </summary>
    public static class TetrisTileCreator {
        // TetrisData.cs의 Colors 배열과 순서 동일 (I O T S Z J L)
        private static readonly (string name, Color color)[] TileInfos = {
            ("Tile_I", new Color(0.0f, 0.8f, 0.8f)),  // 시안
            ("Tile_O", new Color(0.8f, 0.8f, 0.0f)),  // 노랑
            ("Tile_T", new Color(0.6f, 0.0f, 0.8f)),  // 보라
            ("Tile_S", new Color(0.0f, 0.8f, 0.0f)),  // 초록
            ("Tile_Z", new Color(0.8f, 0.0f, 0.0f)),  // 빨강
            ("Tile_J", new Color(0.0f, 0.0f, 0.8f)),  // 파랑
            ("Tile_L", new Color(0.8f, 0.4f, 0.0f)),  // 주황
        };

        [MenuItem("Tools/Tetris/Create Tiles")]
        public static void CreateAllTiles() {
            string folderPath = "Assets/Resources/Tiles";

            // 폴더 없으면 생성
            if (!AssetDatabase.IsValidFolder("Assets/Resources"))
                AssetDatabase.CreateFolder("Assets", "Resources");
            if (!AssetDatabase.IsValidFolder(folderPath))
                AssetDatabase.CreateFolder("Assets/Resources", "Tiles");

            // 피스 타일 7개 생성
            foreach (var info in TileInfos)
                CreateTileAsset(info.name, info.color, folderPath);

            // 고스트 타일 생성 (반투명 흰색)
            CreateTileAsset("Tile_Ghost", new Color(1f, 1f, 1f, 0.2f), folderPath);
            
            // 배경 타일 생성 (회색)
            CreateTileAsset("Tile_BG", new Color(0.2f, 0.2f, 0.2f), folderPath);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("[TetrisTileCreator] 타일 생성 완료!");
            EditorUtility.DisplayDialog("완료", "타일 8개 생성 완료!\nAssets/Resources/Tiles 폴더를 확인하세요.", "확인");
        }

        private static void CreateTileAsset(string name, Color color, string folderPath) {
            const int size = 32;
            Color borderColor = new Color(color.r * 0.6f, color.g * 0.6f, color.b * 0.6f, color.a);

            Texture2D tex = new Texture2D(size, size) { filterMode = FilterMode.Point, name = name };
            Color[] pixels = new Color[size * size];
            for (int y = 0; y < size; y++)
                for (int x = 0; x < size; x++) {
                    bool isBorder = x == 0 || y == 0 || x == size - 1 || y == size - 1;
                    pixels[y * size + x] = isBorder ? borderColor : color;
                }
            tex.SetPixels(pixels);
            tex.Apply();

            Sprite sprite = Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
            sprite.name = name;

            Tile tile = ScriptableObject.CreateInstance<Tile>();
            tile.sprite = sprite;
            tile.color  = Color.white;

            string path = $"{folderPath}/{name}.asset";
            AssetDatabase.CreateAsset(tile, path);
            // Texture2D와 Sprite를 타일 에셋에 내장 — 없으면 재로드 시 소실됨
            AssetDatabase.AddObjectToAsset(tex, path);
            AssetDatabase.AddObjectToAsset(sprite, path);
            Debug.Log($"[TetrisTileCreator] 생성: {path}");
        }
    }
}
#endif
