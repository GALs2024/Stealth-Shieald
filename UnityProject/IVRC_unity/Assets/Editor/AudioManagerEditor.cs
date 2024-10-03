using UnityEditor;
using UnityEngine;
using System.Linq;

[CustomEditor(typeof(BGMManager))]
public class BGMManagerEditor : Editor
{
    private SerializedProperty sceneBGMDataListProperty;
    private string[] sceneNames;

    void OnEnable()
    {
        // SerializedObjectのプロパティを取得
        sceneBGMDataListProperty = serializedObject.FindProperty("sceneBGMDataList");

        // ビルド設定からシーン名を取得
        sceneNames = GetSceneNamesFromBuildSettings();
    }

    public override void OnInspectorGUI()
    {
        // SerializedObjectの更新
        serializedObject.Update();

        // デフォルトのインスペクタを描画
        DrawDefaultInspector();

        // シーンごとのBGMデータの設定を表示
        EditorGUILayout.LabelField("Scene BGM Data");

        for (int i = 0; i < sceneBGMDataListProperty.arraySize; i++)
        {
            SerializedProperty bgmDataProperty = sceneBGMDataListProperty.GetArrayElementAtIndex(i);
            SerializedProperty sceneNameProperty = bgmDataProperty.FindPropertyRelative("sceneName");
            SerializedProperty clipProperty = bgmDataProperty.FindPropertyRelative("clip");
            SerializedProperty volumeProperty = bgmDataProperty.FindPropertyRelative("volume");

            // シーン名のプルダウンメニュー
            int selectedIndex = System.Array.IndexOf(sceneNames, sceneNameProperty.stringValue);
            if (selectedIndex == -1) selectedIndex = 0;

            selectedIndex = EditorGUILayout.Popup("Scene " + (i + 1), selectedIndex, sceneNames);
            sceneNameProperty.stringValue = sceneNames[selectedIndex];

            // BGMクリップのフィールド
            EditorGUILayout.PropertyField(clipProperty, new GUIContent("BGM Clip"));

            // 音量のスライダー
            volumeProperty.floatValue = EditorGUILayout.Slider("Volume", volumeProperty.floatValue, 0f, 1f);
        }

        // ボタンの表示を横並びにするためにHorizontalGroupを使用
        EditorGUILayout.BeginHorizontal();

        // Add BGM Dataボタン
        if (GUILayout.Button("Add Scene BGM Data", GUILayout.MaxWidth(150)))
        {
            sceneBGMDataListProperty.InsertArrayElementAtIndex(sceneBGMDataListProperty.arraySize);
            SerializedProperty newElement = sceneBGMDataListProperty.GetArrayElementAtIndex(sceneBGMDataListProperty.arraySize - 1);
            newElement.FindPropertyRelative("sceneName").stringValue = sceneNames[0]; // 初期値として最初のシーン名を設定
        }

        // Remove Last Scene BGM Dataボタン
        if (sceneBGMDataListProperty.arraySize > 0 && GUILayout.Button("Remove Last Scene BGM Data", GUILayout.MaxWidth(200)))
        {
            sceneBGMDataListProperty.DeleteArrayElementAtIndex(sceneBGMDataListProperty.arraySize - 1);
        }

        EditorGUILayout.EndHorizontal();  // 横並びの終了

        // SerializedObjectの変更を反映
        serializedObject.ApplyModifiedProperties();
    }

    // ビルド設定からシーン名を取得するメソッド
    private string[] GetSceneNamesFromBuildSettings()
    {
        return EditorBuildSettings.scenes
            .Where(scene => scene.enabled)  // 有効なシーンのみ
            .Select(scene => System.IO.Path.GetFileNameWithoutExtension(scene.path))  // シーンの名前を取得
            .ToArray();
    }
}
