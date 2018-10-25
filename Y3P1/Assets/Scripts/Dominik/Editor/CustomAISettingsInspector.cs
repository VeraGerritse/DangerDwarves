using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AISettings))]
public class CustomAISettingsInspector : Editor
{

    public AISettings settings;

    private void OnEnable()
    {
        settings = (AISettings)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.Space(40);

        #region Save Asset Button
        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Save Asset"))
        {
            SaveAssetsDirty();
        }

        GUILayout.EndHorizontal();
        #endregion
    }

    private void SaveAssetsDirty()
    {
        EditorUtility.SetDirty(target);
        AssetDatabase.SaveAssets();
    }
}