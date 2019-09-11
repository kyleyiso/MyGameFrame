using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager :MonoBehaviour
{
    public static MusicManager Instance;
    public AudioSource BGAudioSource;
    public AudioSource EffectAudioSource;
    private AudioClip[] AudioClipArray;
    private Dictionary<string,AudioClip> MusicDictionary=new Dictionary<string, AudioClip>();

	protected  void Awake ()
	{
	    Instance = this;
	    AudioClipArray = Resources.LoadAll<AudioClip>("Sound");
        for (int i = 0; i < AudioClipArray.Length; i++)
	    {
            MusicDictionary.Add(AudioClipArray[i].name, AudioClipArray[i]);
	    }
    }
    AudioClip clip;
    //播放背景音乐
    public void PlayMusicBG(string audioName, AudioSource otherAudioSource=null)
    {  
        if (MusicDictionary.TryGetValue(audioName, out clip))
        {
            if (otherAudioSource == null)
            {
                BGAudioSource.clip = clip;
                BGAudioSource.Play();
            }
            else
            {
                otherAudioSource.clip = clip;
                otherAudioSource.Play(); 
            }           
        }    
    }

    //播放音效
    public void PlayMusicEff(string audioName,float pitch=1, AudioSource otherAudioSource=null)
    {
        if (MusicDictionary.TryGetValue(audioName, out clip))
        {
            EffectAudioSource.pitch = pitch;
            if (otherAudioSource == null)
            {
                EffectAudioSource.PlayOneShot(clip);
            }
            else
            {
                otherAudioSource.PlayOneShot(clip);
            }
        }
    }

    public void StopBG()
    {
        BGAudioSource.Stop();
    }

    public void PauseBG()
    {
        BGAudioSource.Pause();
    }

    public void ResumBG()
    {
        BGAudioSource.UnPause();
    }

    public void StopAll()
    {
        BGAudioSource.Stop();
        EffectAudioSource.Stop();
    }

    public void MuteAll(bool IsMute)
    {
        if (IsMute)
        {
            BGAudioSource.mute = true;
            EffectAudioSource.mute = true;
        }
        else
        {
            BGAudioSource.mute = false;
            EffectAudioSource.mute = false;
        }
    }

    public void MuteEffect(bool IsMute)
    {
        if (IsMute)
        {
            EffectAudioSource.mute = true;
        }
        else
        {
            EffectAudioSource.mute = false;
        }
    }

    public void MuteBG(bool IsMute)
    {
        if (IsMute)
        {
            BGAudioSource.mute = true;
        }
        else
        {
            BGAudioSource.mute = false;
        }
    }
}
