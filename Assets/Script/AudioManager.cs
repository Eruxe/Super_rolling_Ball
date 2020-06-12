using UnityEngine.Audio;
using UnityEngine;
using System;

public class AudioManager : MonoBehaviour
{

    private static AudioManager m_Instance;
    public static AudioManager Instance { 
        get {
            if (m_Instance == null)
            {
                m_Instance = GameObject.FindObjectOfType<AudioManager>();

                //Tell unity not to destroy this object when loading a new scene!
                DontDestroyOnLoad(m_Instance.gameObject);
            }

            return m_Instance;
        } 
    }

    bool m_IsReady = false;
    public static bool IsReady { get { return m_Instance.m_IsReady; } }


    public Sound[] Sounds;

    public Sound CurrentMusic;
    public Sound NextMusic;


    // Start is called before the first frame update
    private void Awake()
    {
        if (m_Instance == null)
        {
            m_Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            if(this != m_Instance) Destroy(this.gameObject);
        }

        

        foreach (Sound s in Sounds)
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
        PlayMusic("PreMenuMusic");
    }

    public void SetNextMusic(String name)
    {
        Sound s = Array.Find(Sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("MUSIC " + name + " DOES NOT EXIST");
            NextMusic = null;
            return;
        }
        NextMusic = s;
    }

    public void PlayNextMusic()
    {
        if (!CurrentMusic.Equals(NextMusic) && NextMusic!=null)
        {
            StopMusic();
            CurrentMusic = NextMusic;
            CurrentMusic.Source.Play();
        }
        else if(NextMusic == null)
        {
            AudioManager.Instance.StopMusic();
        }
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

    public void PlayMusic(string name)
    {
        Sound s = Array.Find(Sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("MUSIC " + name + " DOES NOT EXIST");
            return;
        }
        StopMusic();
        CurrentMusic = s;
        CurrentMusic.Source.Play();
    }

    public void SetMusicFromDifficulty(int list)
    {
        switch (list)
        {
            case 0:
                SetNextMusic("MusicBeginner");
                break;
            case 1:
                SetNextMusic("MusicAdvanced");
                break;
            case 2:
                SetNextMusic("MusicExpert");
                break;
            default:
                Debug.Log("no music associated with list: " + list);
                break;
        }
    }

    public void StopMusic()
    {
        if (CurrentMusic.Source != null) CurrentMusic.Source.Stop();
    }

    public void PlayOneShot(string name,float pitch)
    {
        Sound s = Array.Find(Sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("SOUND " + name  + " DOES NOT EXIST");
            return;
        }
        s.Source.pitch = pitch;
        s.Source.PlayOneShot(s.Clip);
    }

    public bool IsPlaying(String name)
    {
        Sound s = Array.Find(Sounds, sound => sound.name == name);
        return s.Source.isPlaying;
    }


}
