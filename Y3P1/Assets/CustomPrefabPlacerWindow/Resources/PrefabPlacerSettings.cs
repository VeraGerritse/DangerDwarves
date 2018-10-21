using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PrefabPlacerSettings")]
public class PrefabPlacerSettings : ScriptableObject
{

    public bool zeroOutPosition;
    public bool zeroOutRotation;
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
