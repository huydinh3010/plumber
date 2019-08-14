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
    private Sound background;
    //private bool bgPlaying;

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
            if (GameData.Instance.isSoundOn) s.source.volume = s.volume;
            else s.source.volume = 0;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            if (s.name == "background") background = s;
        }
        StartCoroutine(playBackGroundEffect());
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

    public void Stop(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        s.source.Stop();
    }

    IEnumerator playBackGroundEffect()
    {
        yield return new WaitForSeconds(0.5f);
        float volume = 0;
        float speed = 1f;
        background.source.Play();
        while (volume < 1f)
        {
            volume += speed * Time.deltaTime;
            background.source.volume = volume;
            yield return null;
        }
    }

    public void setMute(bool mute)
    {
        foreach (Sound s in sounds)
        {
            s.source.mute = mute;
        }
    }

    public void backgroundVolume(float volume)
    {
        StartCoroutine(bgVolumeChangeEffect(volume));
    }

    IEnumerator bgVolumeChangeEffect(float volume)
    {
        float c_vol = background.source.volume;
        float speed = 1f;
        float delta = Mathf.Abs(volume - c_vol);
        if (volume > c_vol)
        {
            while (delta > 0)
            {
                delta -= speed * Time.deltaTime;
                background.source.volume += speed * Time.deltaTime + (delta < 0 ? delta : 0);
                yield return null;
            }
        }
        else
        {
            while (delta > 0)
            {
                delta -= speed * Time.deltaTime;
                background.source.volume -= (speed * Time.deltaTime + (delta < 0 ? delta : 0));
                yield return null;
            }
        }
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