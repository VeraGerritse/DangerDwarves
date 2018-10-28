using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PrefabPlacerSettings")]
public class PrefabPlacerSettings : ScriptableObject
{

    [Tooltip("True: spawn at 0,0,0. False: spawn at prefab position.")]
    public bool zeroOutPosition;
    [Tooltip("True: rotate to 0,0,0. False: rotate to prefab rotation.")]
    public bool zeroOutRotation;
    [Tooltip("True: overrides the zeroOutPosition boolean and spawns the prefab at screen center. False: checks zeroOutPosition and positions accordingly")]
    public bool spawnAtScreenCenter = true;

    [Space(10)]

    public List<Prefab> prefabs = new List<Prefab>();

    [System.Serializable]
    public class Prefab
    {
        public GameObject prefab;
        [Tooltip("This is optional, if left blank the prefab name will be used.")]
        public string editorWindowName;
        public Color editorButtonColor = Color.white;
        public FontStyle editorButtonFontStyle;
    }
}
