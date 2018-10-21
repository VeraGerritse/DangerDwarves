using UnityEditor;
using UnityEngine;

public class CustomPrefabPlacer : EditorWindow
{

    private PrefabPlacerSettings settings;
    private Vector2 scrollPos;

    private void OnEnable()
    {
        if (!settings)
        {
            settings = Resources.Load<PrefabPlacerSettings>("PrefabPlacerSettings");
        }
    }

    [MenuItem("Window/PrefabPlacer")]
    private static void ShowWindow()
    {
        GetWindow(typeof(CustomPrefabPlacer));
    }

    private void OnInspectorUpdate()
    {
        Repaint();
    }

    private void OnGUI()
    {
        GUILayout.Label("Placable Prefabs", EditorStyles.boldLabel);

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        for (int i = 0; i < settings.prefabs.Count; i++)
        {
            if (settings.prefabs[i].prefab)
            {
                string buttonName = !string.IsNullOrEmpty(settings.prefabs[i].editorWindowName) ? settings.prefabs[i].editorWindowName : settings.prefabs[i].prefab.name;

                GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
                buttonStyle.normal.textColor = settings.prefabs[i].editorButtonColor;
                buttonStyle.fontStyle = settings.prefabs[i].editorButtonFontStyle;

                if (GUILayout.Button(buttonName, buttonStyle, GUILayout.Height(40)))
                {
                    PlacePrefab(settings.prefabs[i].prefab);
                }

                GUILayout.Space(5);
            }
        }

        EditorGUILayout.EndScrollView();
    }

    private void PlacePrefab(GameObject prefab)
    {
        GameObject newPrefab = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        Selection.activeGameObject = newPrefab;

        Undo.RegisterCreatedObjectUndo(newPrefab, "Prefab Placed");

        if (settings.zeroOutPosition)
        {
            newPrefab.transform.position = Vector3.zero;
        }

        if (settings.zeroOutRotation)
        {
            newPrefab.transform.eulerAngles = Vector3.zero;
        }

        if (settings.spawnAtScreenCenter)
        {
            Camera sceneCam = SceneView.lastActiveSceneView.camera;
            Vector3 pos = sceneCam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 10f));

            newPrefab.transform.position = pos;
        }
    }
}
