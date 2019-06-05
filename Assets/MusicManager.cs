using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{

    public AudioClip defaultSong;
    AudioSource music;

    private void Awake()
    {

    }

    // Use this for initialization
    void Start()
    {
        if (defaultSong == null) Debug.Log("Song is null");
        //we want the music to loop from the start
        music = GetComponent<AudioSource>();
        music.clip = defaultSong;
        music.loop = true;
        music.Play();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void setSong(AudioClip s)
    {
        music.clip = s;
        music.Play();
    }

    void pauseMusic()
    {
        music.Pause();
    }

    void playMusic()
    {
        music.Play();
    }
}
