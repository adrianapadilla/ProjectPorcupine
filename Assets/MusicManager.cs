using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{

    public int defaultSong = 0;

    public List<AudioClip> playlist;

    AudioSource music;

    private int m_currentPlayMusicIndex = 0;

    private void Awake()
    {

    }

    // Use this for initialization
    void Start()
    {
        if (defaultSong == null) Debug.Log("Song is null");
        //we want the music to loop from the start
        music = GetComponent<AudioSource>();
        if (defaultSong < playlist.Count)
        {
            m_currentPlayMusicIndex = defaultSong;
            music.clip = playlist[m_currentPlayMusicIndex];
            music.loop = true;
            music.Play();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    // v must be between 0 and 1
    public void setVolume(float v)
    {
        music.volume = v;
    }

    public void setSong(AudioClip s)
    {
        music.clip = s;
        music.Play();
    }

    public void pauseMusic()
    {
        music.Pause();
    }

    public void playMusic()
    {
        music.Play();
    }

    public int GetTotalNumberOfMusic()
    {
        return playlist.Count;
    }

    public int GetCurrentPlayingMusicIndex()
    {
        return m_currentPlayMusicIndex;
    }

    public void SetCurrentPlayingMusicIndex(int index)
    {
        if (index < playlist.Count)
        {
            m_currentPlayMusicIndex = index;

            music.clip = playlist[m_currentPlayMusicIndex];
            music.loop = true;
            music.Play();
        }
    }

    public List<string> GetPlayList()
    {
        List<string> songNames = new List<string>();

        playlist.ForEach((clip) =>
        {
            songNames.Add(clip.name);
        });

        return songNames;
    }
}
