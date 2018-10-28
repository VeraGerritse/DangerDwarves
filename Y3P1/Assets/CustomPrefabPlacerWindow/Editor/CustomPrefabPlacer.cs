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

    // Creates a new menu item. When clicking on this new item this PrefabPlacer window will show up.
    [MenuItem("Window/PrefabPlacer")]
    private static void ShowWindow()
    {
        GetWindow(typeof(CustomPrefabPlacer));
    }

    // Creates a new menu item under the Assets folder. The 'Add To Prefab Placer' option will show up when right clicking an object.
    // When you select this option on a prefab it will add that prefab to the PrefabPlacerSettings and showup in the PrefabPlacer window.
    [MenuItem("Assets/Add To Prefab Placer")]
    private static void AddToPrefabPlacer()
    {
        GameObject selected = Selection.activeGameObject;
        if (PrefabUtility.GetPrefabObject(selected))
        {
            Resources.Load<PrefabPlacerSettings>("PrefabPlacerSettings").prefabs.Add(new PrefabPlacerSettings.Prefab { prefab = selected });
        }
    }

    private void OnInspectorUpdate()
    {
        Repaint();
    }

    private void OnGUI()
    {
        GUILayout.Label("Placable Prefabs", EditorStyles.boldLabel);

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        // Creates a GUI Button for every prefab in the PrefabPlacerSettings. Clicking on that button will call PlacePrefab() which.. well. places the prefab.
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

    // Instantiate the prefab at a certain position and rotation. Settings can be changed in the PrefabPlacerSettings ScriptableObject.
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
