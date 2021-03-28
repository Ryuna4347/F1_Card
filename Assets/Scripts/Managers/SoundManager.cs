using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private static SoundManager instance;
    public static SoundManager Instance { get { return instance; } }

    public AudioSource bgm;
    public AudioSource sound;
    public List<AudioClip> bgmList;
    public List<AudioClip> soundList;

    // Start is called before the first frame update
    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }

        bgm = transform.GetChild(0).GetComponent<AudioSource>();
        sound = transform.GetChild(1).GetComponent<AudioSource>();
    }
    public void PlayBGM(string name)
    {
        AudioClip selected = bgmList.Find(x => x.name.Contains(name));
        bgm.clip = selected;
        bgm.loop = true;
        bgm.Play();
    }
    public void PlayOnceBGM(string name)
    {
        AudioClip selected = bgmList.Find(x => x.name.Contains(name));
        bgm.clip = selected;
        bgm.loop = false;
        bgm.Play();
    }

    public void PlaySound(string name)
    {
        AudioClip selected = soundList.Find(x => x.name.Contains(name));
        sound.clip = selected;
        sound.Play();
    }
}
