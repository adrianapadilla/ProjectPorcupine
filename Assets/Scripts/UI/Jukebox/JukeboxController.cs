using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JukeboxController : MonoBehaviour
{
    [SerializeField] private MusicManager m_musicManager;

    [SerializeField] private Dropdown m_musicDropDown;
    [SerializeField] private Button prevSongBtn;
    [SerializeField] private Button nextSongBtn;


    void Start()
    {
        PopulatePlaylistToDropDown(m_musicManager.GetPlayList());

        m_musicDropDown.value = m_musicManager.GetCurrentPlayingMusicIndex();
        m_musicDropDown.onValueChanged.AddListener(OnUserSelectSong);
        prevSongBtn.onClick.AddListener(OnUserClickPrevSong);
        nextSongBtn.onClick.AddListener(OnUserClickNextSong);
    }

    void OnDestroy()
    {
        m_musicDropDown.onValueChanged.RemoveListener(OnUserSelectSong);
        prevSongBtn.onClick.RemoveListener(OnUserClickPrevSong);
        nextSongBtn.onClick.RemoveListener(OnUserClickNextSong);
    }


    private void PopulatePlaylistToDropDown(List<string> playlist)
    {
        List<Dropdown.OptionData> items = new List<Dropdown.OptionData>(playlist.Count);

        playlist.ForEach((string name) =>
        {
            Dropdown.OptionData option = new Dropdown.OptionData();
            option.text = name;
            items.Add(option);
        });

        m_musicDropDown.options = items;
    }


    private void OnUserSelectSong(int value)
    {
        m_musicManager.SetCurrentPlayingMusicIndex(value);
    }

    private void OnUserClickNextSong()
    {
        int next = (m_musicManager.GetCurrentPlayingMusicIndex() + 1) % m_musicManager.GetTotalNumberOfMusic();
        m_musicManager.SetCurrentPlayingMusicIndex(next);
        m_musicDropDown.value = next;
    }

    private void OnUserClickPrevSong()
    {
        int total = m_musicManager.GetTotalNumberOfMusic();
        int next = (total + m_musicManager.GetCurrentPlayingMusicIndex() - 1) % total;
        m_musicManager.SetCurrentPlayingMusicIndex(next);
        m_musicDropDown.value = next;
    }
}
