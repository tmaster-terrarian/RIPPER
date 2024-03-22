using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using System;
using UnityEngine;

[System.Serializable]
public class Sound
{

    public string name;

    public AudioClip clip;

    public float volume = 1f;
    public float pitch = 1f;

    public bool loop;

    [HideInInspector]
    public AudioSource source;
}


//Only thing I took from an older project. Unity's Sound management is stupid and cringe!!!!
public class SFX : MonoBehaviour
{
    static Sound[] SND;
    static Sound[] MUS;

    public Sound[] sounds;
    public Sound[] musics;

    public static string cmus;

    void Awake()
    {
        SND = sounds;
        MUS = musics;
    }

    public static void PlayMus(string name = "")
    {
        foreach (Sound s in MUS)
        {
            if (s.source.loop)
                s.source.Stop();
        }
        if (name != "" && name != "Stop")
            cmus = name;
        if (name != "Stop")
        {
            Sound s = Array.Find(MUS, sound => sound.name == name);
            if (s == null)
            {
                Debug.LogWarning("Music " + name + " Doesnt Exist");
                return;
            }
            s.source.Play();
        }

    }

    public static void Stop(string name)
    {
        Sound s = Array.Find(SND, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound " + name + " Doesnt Exist");
            return;
        }
        s.source.Stop();
    }

    public static void Play(string name, float volume = 1, float pitch = 0, bool CreateNew = true)
    {
        Sound s = Array.Find(SND, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound '" + name + "' aint exist dummy");
            return;
        }

        //Creatnew thing
        bool Check = false;
        if (!CreateNew)
        {
            AudioSource s0 = Array.Find(GameObject.FindObjectOfType<SFX>().GetComponents<AudioSource>(), AudioSource => AudioSource.clip == s.clip);
            if (s0) Check = true;
        }

        //Make sound to play and it's stacked
        if (!Check)
        {
            AudioSource SOUND = GameObject.FindObjectOfType<SFX>().gameObject.AddComponent<AudioSource>();
            SOUND.clip = s.clip;
            SOUND.loop = s.loop;

            SOUND.volume = Mathf.Min(1, s.volume * volume);
            if (pitch != 0) SOUND.pitch = pitch; else SOUND.pitch = s.pitch;

            GameObject.FindObjectOfType<SFX>().StartCoroutine("Soundy", SOUND);
        }
    }

    public IEnumerator Soundy(AudioSource S)
    {
        S.Play();
        while (S.isPlaying || S.loop) { yield return null; }
        Destroy(S);
    }
}
