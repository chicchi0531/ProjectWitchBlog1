using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEditor.SceneManagement;

public class MyFont : ScriptableObject
{
    public Font font;
}


public class FontMissing : EditorWindow
{
    static SerializedProperty sp;

    [MenuItem("Tools/Replace All Fonts")]
    public static void ShowWindow()
    {
        EditorWindow a = EditorWindow.GetWindow(typeof(FontMissing), true, "Font Replacer");
        var obj = ScriptableObject.CreateInstance<MyFont>();
        var serializedObject = new UnityEditor.SerializedObject(obj);

        sp = serializedObject.FindProperty("font");

        Debug.Log("font " + sp.propertyType);
    }

    void OnGUI()
    {
        EditorGUILayout.PropertyField(sp);
        if (GUILayout.Button("Replace All Fonts"))
        {
            Debug.Log("you are trying to replace all fonts to new one");

            var textComponents = Resources.FindObjectsOfTypeAll(typeof(Text)) as Text[];
            foreach (var component in textComponents)
            {
                component.font = sp.objectReferenceValue as Font;
            }
            // ※追記 : シーンに変更があることをUnity側に通知しないと、シーンを切り替えたときに変更が破棄されてしまうので、↓が必要
            EditorSceneManager.MarkAllScenesDirty();
        }
    }
}