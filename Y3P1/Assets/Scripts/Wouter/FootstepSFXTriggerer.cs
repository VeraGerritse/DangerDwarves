using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootstepSFXTriggerer : MonoBehaviour {

    public List<AudioSource> SFXFootstepsStone = new List<AudioSource>();
    public List<AudioSource> SFXFootstepsWooden = new List<AudioSource>();
    public List<AudioSource> SFXFootstepsSoil = new List<AudioSource>();


    public void Update()
    {
        //PickAndPlaySound();
    }
    public void PickAndPlaySound()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, 1))
        {
            if(hit.transform.GetComponent<MaterialTagger>() != null)
            {
                if(hit.transform.GetComponent<MaterialTagger>().materialType == MaterialTagger.MaterialType.Stone)
                {
                    SFXFootstepsStone[Random.Range(0,SFXFootstepsStone.Count)].Play();
                }
                if (hit.transform.GetComponent<MaterialTagger>().materialType == MaterialTagger.MaterialType.Wooden)
                {
                    SFXFootstepsWooden[Random.Range(0, SFXFootstepsWooden.Count)].Play();
                }
                if (hit.transform.GetComponent<MaterialTagger>().materialType == MaterialTagger.MaterialType.Soil)
                {
                    SFXFootstepsSoil[Random.Range(0, SFXFootstepsSoil.Count)].Play();
                }
            }
        }
        
    }
}
