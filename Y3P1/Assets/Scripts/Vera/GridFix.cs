using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridFix : MonoBehaviour {
    public GridLayoutGroup myGrid;
    public RectTransform myPanel;

    private void Update()
    {
        Vector3 panelSize = myPanel.sizeDelta;
        float x = Mathf.Abs(panelSize.x / 5 * 10);
        myGrid.cellSize = new Vector2(x,x);
    }
}
