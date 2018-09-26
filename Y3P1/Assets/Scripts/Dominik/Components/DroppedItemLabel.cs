using TMPro;
using UnityEngine;

public class DroppedItemLabel : MonoBehaviour
{

    private Light light;
    private TextMeshProUGUI labelText;
    private Vector3 legendaryMarkDefaultPos;

    [SerializeField] private GameObject legendaryMark;
    [HideInInspector] public Animator anim;

    private void Awake()
    {
        light = GetComponentInChildren<Light>();
        labelText = GetComponentInChildren<TextMeshProUGUI>();
        anim = GetComponent<Animator>();

        legendaryMarkDefaultPos = legendaryMark.transform.position;
        legendaryMark.SetActive(false);
    }

    private void Update()
    {
        if (legendaryMark.activeInHierarchy)
        {
            legendaryMark.transform.localPosition = Vector3.zero;
            legendaryMark.transform.eulerAngles = Vector3.zero;
        }
    }

    public void SetText(string text, Item.ItemRarity rarity)
    {
        switch (rarity)
        {
            case Item.ItemRarity.common:

                labelText.color = Color.white;
                light.enabled = false;
                break;
            case Item.ItemRarity.rare:

                labelText.color = Color.blue;
                light.enabled = false;
                break;
            case Item.ItemRarity.epic:

                light.color = Color.magenta;
                labelText.color = Color.magenta;
                light.enabled = true;
                break;
            case Item.ItemRarity.legendary:

                light.color = Color.yellow;
                labelText.color = Color.yellow;
                light.enabled = true;

                legendaryMark.SetActive(true);
                break;
        }

        labelText.text = text;
    }

    public void ReturnToPool()
    {
        legendaryMark.SetActive(false);
        ObjectPooler.instance.AddToPool("DroppedItemLabel", gameObject);
    }
}
