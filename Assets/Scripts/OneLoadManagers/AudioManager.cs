using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine;
using System;
using UnityEngine.UI;
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public enum SoundName
    {
        BUTTON = 0,
        COIN_DECREASE,
        COIN_REWARD,
        WINNING_POPUP,
        WATER,
    }
    [SerializeField] AudioSource[] sounds;
    [SerializeField] AudioSource[] pipeSounds;
    [SerializeField] AudioSource[] valveSounds;
    [SerializeField] AudioSource menu;
    [SerializeField] AudioSource ingame;
    private float menuVol;
    private float ingameVol;
    private bool bgEnd;
    private int type = 1;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void Initialize()
    {
        menuVol = menu.volume;
        ingameVol = ingame.volume;
        if (GameData.Instance.isSoundOn) setMute(false);
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Play(SoundName name)
    {
        sounds[(int)name].Play();
        
    }

    public void Stop(SoundName name)
    {
        sounds[(int)name].Stop();
    }

    public void PlayPipeSound()
    {
        System.Random random = new System.Random();
        pipeSounds[random.Next(0, pipeSounds.Length - 1)].Play();
    }

    public void PlayValveSound()
    {
        System.Random random = new System.Random();
        valveSounds[random.Next(0, valveSounds.Length - 1)].Play();
    }

    public void setMute(bool mute)
    {
        foreach (AudioSource s in sounds)
        {
            s.mute = mute;
        }
        foreach (AudioSource s in pipeSounds)
        {
            s.mute = mute;
        }
        foreach (AudioSource s in valveSounds)
        {
            s.mute = mute;
        }
        menu.mute = mute;
        ingame.mute = mute;
    }

    public void changeBackground(int type)
    {
        if (type == this.type) return;
        this.type = type;
        if(this.type == 0)
        {
            StartCoroutine(endBackGroundEffect(ingame, ()=> { StartCoroutine(startBackGroundEffect(menu, menuVol)); }));
        }
        else
        {
            StartCoroutine(endBackGroundEffect(menu, () => { StartCoroutine(startBackGroundEffect(ingame, ingameVol)); }));
        }
    }

    IEnumerator startBackGroundEffect(AudioSource sound, float volume)
    {
        sound.volume = 0f;
        sound.Play();
        float speed = 1f;
        while (!bgEnd && sound.volume < volume)
        {
            sound.volume += speed * Time.deltaTime;
            yield return null;
        }
        if(!bgEnd) sound.volume = volume;
    }

    IEnumerator endBackGroundEffect(AudioSource sound, Action action)
    {
        bgEnd = true;
        float speed = 3f;
        while (sound.volume > 0)
        {
            sound.volume -= speed * Time.deltaTime;
            yield return null;
        }
        sound.Stop();
        bgEnd = false;
        action();
    }
}