using UnityEngine.Audio;
using UnityEngine;

[System.Serializable]
public class Sound
{

    public string name;

    public AudioClip Clip;

    [Range (0.0f,1.0f)]
    public float Volume;

    [Range(.1f, 3.0f)]
    public float Pitch;

    public bool Loop;

    [HideInInspector]
    public AudioSource Source;

}
