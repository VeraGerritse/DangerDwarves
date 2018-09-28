using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatsInfo : MonoBehaviour {
    public static StatsInfo instance = new StatsInfo();
    [SerializeField] private List<TMP_Text> allText = new List<TMP_Text>();
    [SerializeField] private GameObject myPanel;
    [SerializeField] private Image myImage;



    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    public void DisablePanel()
    {
        myPanel.SetActive(false);
        myImage.enabled = false;
    }

    public void SetText(string[] item,string[] damage, string[] weapon, string[] ranged, string[] melee, string[] helmet, string[] trinket)
    {
        List<string> allTextNeeded = new List<string>();
        if(item != null)
        {
            allTextNeeded.AddRange(item);
        }
        if(damage != null)
        {
            allTextNeeded.AddRange(damage);
            allTextNeeded.Add("");
        }
        if (ranged != null)
        {
            allTextNeeded.AddRange(ranged);
            allTextNeeded.Add("");
        }
        if (melee != null)
        {
            allTextNeeded.AddRange(melee);
            allTextNeeded.Add("");
        }
        if (weapon != null)
        {
            allTextNeeded.AddRange(weapon);
            allTextNeeded.Add("");
        }

        if(helmet != null)
        {
            allTextNeeded.AddRange(helmet);
        }
        if(trinket != null)
        {
            allTextNeeded.AddRange(trinket);
        }


        for (int i = 0; i < allText.Count; i++)
        {
            if(i < allTextNeeded.Count)
            {
                if(allText[i] != null)
                {
                    myPanel.SetActive(true);
                    myImage.enabled = true;
                    allText[i].text = allTextNeeded[i];
                    allText[i].enabled = true;
                }
            }
            else
            {
                allText[i].enabled = false;
            }
        }
    }
}
