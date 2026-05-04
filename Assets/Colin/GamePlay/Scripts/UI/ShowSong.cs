using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class ShowSong : MonoBehaviour
{
    Songs song;
    public Songs.SongData currentSong;
    Dictionary<string, TextMeshProUGUI> text;

    Canvas canvas;

    void Start()
    {
        canvas = GetComponent<Canvas>();
        song = GameObject.Find("GameManager").GetComponent<Songs>();
        foreach (Songs.SongData song in song.songs)
        {
            if (SceneManager.GetActiveScene().name == song.levelName)
            {
                currentSong = song;
            }
        }
        text = new Dictionary<string, TextMeshProUGUI>()
        {
            {"Name", transform.GetChild(1).GetComponent<TextMeshProUGUI>()},
            {"BPM", transform.GetChild(3).GetComponent<TextMeshProUGUI>()}
        };
        if (currentSong != null)
        {
            SetCanvas();
        }
    }

    public void SetCanvas()
    {
        text["Name"].text = currentSong.name;
        int minutes = (int)Mathf.Floor(currentSong.length / 60);
        int second = (int)currentSong.length - (minutes * 60);

        text["BPM"].text = currentSong.bpm + " bpm";
    }

    public void SetCanvas(Songs.SongData currentSong)
    {
        text["Name"].text = currentSong.name;
        int minutes = (int)Mathf.Floor(currentSong.length / 60);
        int second = (int)currentSong.length - (minutes * 60);

        text["BPM"].text = currentSong.bpm + " bpm";
    }
}