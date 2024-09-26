using UnityEditor;
using UnityEngine;
using System.Linq;

[CustomEditor(typeof(AudioManager))]
public class AudioManagerEditor : Editor
{
    private SerializedProperty bgmInheritedSceneNamesProperty;
    private string[] sceneNames;

    void OnEnable()
    {
        // SerializedObjectのプロパティを取得
        bgmInheritedSceneNamesProperty = serializedObject.FindProperty("bgmInheritedSceneNames");

        // ビルド設定からシーン名を取得
        sceneNames = GetSceneNamesFromBuildSettings();
    }

    public override void OnInspectorGUI()
    {
        // SerializedObjectの更新
        serializedObject.Update();

        // デフォルトのインスペクタを描画
        DrawDefaultInspector();

        // BGMを引き継ぐシーンの選択リストを表示
        EditorGUILayout.LabelField("BGM Inherited Scenes");

        for (int i = 0; i < bgmInheritedSceneNamesProperty.arraySize; i++)
        {
            SerializedProperty sceneNameProperty = bgmInheritedSceneNamesProperty.GetArrayElementAtIndex(i);
            int selectedIndex = System.Array.IndexOf(sceneNames, sceneNameProperty.stringValue);

            if (selectedIndex == -1) selectedIndex = 0;  // 無効なインデックスは0に設定

            // プルダウンメニューを表示
            selectedIndex = EditorGUILayout.Popup("Scene " + (i + 1), selectedIndex, sceneNames);

            // 選択されたシーン名を更新
            sceneNameProperty.stringValue = sceneNames[selectedIndex];
        }

        // ボタンの表示を横並びにするためにHorizontalGroupを使用
        EditorGUILayout.BeginHorizontal();

        // Add Sceneボタン
        if (GUILayout.Button("Add Scene", GUILayout.MaxWidth(100)))  // 横幅を制限
        {
            bgmInheritedSceneNamesProperty.InsertArrayElementAtIndex(bgmInheritedSceneNamesProperty.arraySize);
            bgmInheritedSceneNamesProperty.GetArrayElementAtIndex(bgmInheritedSceneNamesProperty.arraySize - 1).stringValue = sceneNames[0]; // 初期値として最初のシーン名を設定
        }

        // Remove Last Sceneボタン
        if (bgmInheritedSceneNamesProperty.arraySize > 0 && GUILayout.Button("Remove Last Scene", GUILayout.MaxWidth(150)))  // 横幅を制限
        {
            bgmInheritedSceneNamesProperty.DeleteArrayElementAtIndex(bgmInheritedSceneNamesProperty.arraySize - 1);
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
