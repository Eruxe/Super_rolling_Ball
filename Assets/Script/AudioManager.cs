using UnityEngine.Audio;
using UnityEngine;
using System;

public class AudioManager : MonoBehaviour
{

    private static AudioManager m_Instance;
    public static AudioManager Instance { get { return m_Instance; } }

    bool m_IsReady = false;
    public static bool IsReady { get { return m_Instance.m_IsReady; } }


    public Sound[] Sounds;


    // Start is called before the first frame update
    private void Awake()
    {
        if (!m_Instance)
        {
            m_Instance = this;
        }
        else Destroy(gameObject);

        foreach(Sound s in Sounds)
        {
            s.Source = gameObject.AddComponent<AudioSource>();
            s.Source.clip = s.Clip;
            s.Source.volume = s.Volume;
            s.Source.pitch = s.Pitch;
            s.Source.loop = s.Loop;
        }
    }

    void Start()
    {
        m_IsReady = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Play(string name)
    {
       Sound s = Array.Find(Sounds, sound => sound.name == name);
       if (s == null)
       {
           Debug.LogWarning("SOUND " + name + " DOES NOT EXIST");
           return;
       }
       s.Source.Play();
    }

    public void PlayOneShot(string name)
    {
        Sound s = Array.Find(Sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("SOUND " + name  + " DOES NOT EXIST");
            return;
        }
        s.Source.PlayOneShot(s.Clip);
    }


}
