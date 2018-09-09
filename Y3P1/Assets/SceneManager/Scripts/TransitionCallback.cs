using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Y3P1;

public class TransitionCallback : MonoBehaviour 
{

    public void AnimationEventTransitionComplete()
    {
        if (SceneManager.isLoadingLevel)
        {
            SceneManager.transitionComplete = true;
        }
    }
}
