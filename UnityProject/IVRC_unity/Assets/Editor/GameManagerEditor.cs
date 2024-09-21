using UnityEditor;
using UnityEngine;
using System.Linq;
using System.Reflection;

[CustomEditor(typeof(GameManager))]
public class GameManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GameManager gameManager = (GameManager)target;

        if (gameManager.TargetScript != null)
        {
            // ターゲットスクリプトの全てのメソッドを取得
            MethodInfo[] methods = gameManager.TargetScript.GetType()
                .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly)
                .Where(m => m.GetParameters().Length == 0)  // 引数がないメソッドだけをリストにする
                .ToArray();

            // メソッド名のリストを作成
            string[] methodNames = methods.Select(m => m.Name).ToArray();

            // 現在選択されているメソッドをプルダウンで表示
            int selectedIndex = System.Array.IndexOf(methodNames, gameManager.selectedMethod);
            selectedIndex = EditorGUILayout.Popup("Select Method", selectedIndex, methodNames);

            // 選択したメソッドを更新
            if (selectedIndex >= 0 && selectedIndex < methodNames.Length)
            {
                gameManager.selectedMethod = methodNames[selectedIndex];
            }
        }
    }
}
