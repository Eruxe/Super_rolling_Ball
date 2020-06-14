using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fading : MonoBehaviour
{
    
    public void OnFadeOutCompleted()
    {
        LevelManager.Instance.LaunchScene();
    }

    public void OnGoalCompleted()
    {
        LevelManager.Instance.PlayNext();
    }

    public void OnFallingOrTimoutCompleted()
    {
        GameManager.Instance.Restart();
    }

    public void OnReadyCompleted()
    {
        GameManager.Instance.SpawnBall(GameObject.Find("Spawn"));
    }

}
