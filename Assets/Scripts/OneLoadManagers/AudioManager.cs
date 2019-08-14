using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine;
using System;
using UnityEngine.UI;
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public Sound[] sounds;
    private Sound menu;
    private Sound ingame;
    private bool bgEnd;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void Initialize()
    {
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            if (s.name == "music_menu") menu = s;
            if (s.name == "music_ingame") ingame = s;
        }
        if (!GameData.Instance.isSoundOn) setMute(true);
        StartCoroutine(startBackGroundEffect(menu));
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        s.source.Play();
    }

    public void PlayPipeSound()
    {
        System.Random random = new System.Random();
        string name = "pipe_" + random.Next(1, 5);
        Sound s = Array.Find(sounds, sound => sound.name == name);
        Debug.Log(name);
        s.source.Play();
    }

    public void PlayValveSound()
    {
        System.Random random = new System.Random();
        string name = "valve_" + random.Next(1, 2);
        Sound s = Array.Find(sounds, sound => sound.name == name);
        Debug.Log(name);
        s.source.Play();
    }

    public void Stop(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        s.source.Stop();
    }

    public void setMute(bool mute)
    {
        foreach (Sound s in sounds)
        {
            s.source.mute = mute;
        }
    }

    public void changeBackground(int type)
    {
        if(type == 0)
        {
            StartCoroutine(endBackGroundEffect(ingame, ()=> { StartCoroutine(startBackGroundEffect(menu)); }));
        }
        else
        {
            StartCoroutine(endBackGroundEffect(menu, () => { StartCoroutine(startBackGroundEffect(ingame)); }));
        }
    }

    IEnumerator startBackGroundEffect(Sound sound)
    {
        sound.source.volume = 0f;
        sound.source.Play();
        float speed = 1f;
        while (!bgEnd && sound.source.volume < sound.volume)
        {
            sound.source.volume += speed * Time.deltaTime;
            yield return null;
        }
        if(!bgEnd) sound.source.volume = sound.volume;
    }

    IEnumerator endBackGroundEffect(Sound sound, Action action)
    {
        bgEnd = true;
        float speed = 3f;
        while (sound.source.volume > 0)
        {
            sound.source.volume -= speed * Time.deltaTime;
            yield return null;
        }
        sound.source.Stop();
        bgEnd = false;
        action();
    }
}


[System.Serializable]
public class Sound
{
    public string name;

    public AudioClip clip;
    [Range(0f, 1f)]
    public float volume;
    [Range(.1f, 3f)]
    public float pitch;

    public bool loop;

    [HideInInspector]
    public AudioSource source;
}