using UnityEditor;
using UnityEngine;
using System.IO;


public class FolderPathAttribute : PropertyAttribute
{
    public FolderPathAttribute() { }
}


[CustomPropertyDrawer(typeof(FolderPathAttribute))]
public class FolderPathDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property.propertyType == SerializedPropertyType.String)
        {
            // プロパティの表示開始
            EditorGUI.BeginProperty(position, label, property);

            // ラベルの表示
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            // テキストフィールドの表示
            Rect textFieldPosition = new Rect(position.x, position.y, position.width - 70, position.height);
            property.stringValue = EditorGUI.TextField(textFieldPosition, property.stringValue);

            // 「Browse」ボタンを表示
            Rect buttonPosition = new Rect(position.x + position.width - 65, position.y, 65, position.height);
            if (GUI.Button(buttonPosition, "Browse"))
            {
                // フォルダ選択ダイアログの表示
                string selectedPath = EditorUtility.OpenFolderPanel("Select Folder", Application.dataPath, "");
                if (!string.IsNullOrEmpty(selectedPath))
                {
                    // プロジェクト内のパスかどうかを確認
                    if (selectedPath.StartsWith(Application.dataPath))
                    {
                        // 相対パスに変換（"Assets"フォルダ以下のパスにする）
                        selectedPath = "Assets" + selectedPath.Substring(Application.dataPath.Length);
                    }
                    else
                    {
                        Debug.LogWarning("Selected folder is outside the project. Absolute path will be used.");
                    }

                    // 選択されたフォルダパスをプロパティに設定
                    property.stringValue = selectedPath;
                }
            }

            EditorGUI.EndProperty();
        }
        else
        {
            EditorGUI.LabelField(position, label.text, "Use FolderPath with string.");
        }
    }
}
