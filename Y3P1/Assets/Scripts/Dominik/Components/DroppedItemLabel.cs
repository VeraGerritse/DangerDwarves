using UnityEngine;
using TMPro;

public class DroppedItemLabel : MonoBehaviour
{

    private Light light;
    private TextMeshProUGUI labelText;

    [HideInInspector] public Animator anim;

    private void Awake()
    {
        light = GetComponentInChildren<Light>();
        labelText = GetComponentInChildren<TextMeshProUGUI>();
        anim = GetComponent<Animator>();
    }

    public void SetText(string text, Item.ItemRarity rarity)
    {
        switch (rarity)
        {
            case Item.ItemRarity.common:

                light.color = Color.white;
                labelText.color = Color.white;
                break;
            case Item.ItemRarity.rare:

                light.color = Color.blue;
                labelText.color = Color.blue;
                break;
            case Item.ItemRarity.epic:

                light.color = Color.magenta;
                labelText.color = Color.magenta;
                break;
            case Item.ItemRarity.legendary:

                light.color = Color.yellow;
                labelText.color = Color.yellow;
                break;
        }

        labelText.text = text;
    }

    public void ReturnToPool()
    {
        ObjectPooler.instance.AddToPool("DroppedItemLabel", gameObject);
    }
}
