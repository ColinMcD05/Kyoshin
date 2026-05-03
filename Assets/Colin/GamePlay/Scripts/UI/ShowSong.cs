using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class ShowSong : MonoBehaviour
{
    Songs song;
    Songs.SongData currentSong;
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
            {"Name", transform.GetChild(0).GetComponent<TextMeshProUGUI>()},
            {"Length", transform.GetChild(2).GetComponent<TextMeshProUGUI>()},
            {"BPM", transform.GetChild(3).GetComponent<TextMeshProUGUI>()}
        };
        SetCanvas();
    }

    public void SetCanvas()
    {
        text["Name"].text = currentSong.name;
        int minutes = (int)Mathf.Floor(currentSong.length / 60);
        int second = (int)currentSong.length - (minutes * 60);

        text["Length"].text = minutes + ": " + second;

        text["BPM"].text = "BPM: " + currentSong.bpm;
    }
}