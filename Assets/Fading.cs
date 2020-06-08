using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fading : MonoBehaviour
{
    
    public void OnFadeOutCompleted()
    {
        LevelManager.Instance.LaunchScene();
    }

}
