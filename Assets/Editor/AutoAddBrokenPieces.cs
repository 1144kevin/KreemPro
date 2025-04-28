using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Fusion;

[CustomEditor(typeof(TreasureBox))]
public class AutoAddBrokenPieces : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        TreasureBox treasureBox = (TreasureBox)target;

        if (GUILayout.Button("🔵 自動加所有 BrokenPieces 資料夾內Prefab"))
        {
            string path = "Assets/Prefab/Enviroment/BrokenPieces";

            string[] guids = AssetDatabase.FindAssets("t:Prefab", new[] { path });
            var prefabList = new List<NetworkObject>();

            foreach (string guid in guids)
            {
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(guid));
                if (prefab != null && prefab.GetComponent<NetworkObject>() != null)
                {
                    prefabList.Add(prefab.GetComponent<NetworkObject>());
                }
            }

            treasureBox.brokenPiecePrefabs = prefabList;
            EditorUtility.SetDirty(treasureBox);

            Debug.Log($"✅ 自動加了 {prefabList.Count} 個碎片Prefab！");
        }
    }
}
