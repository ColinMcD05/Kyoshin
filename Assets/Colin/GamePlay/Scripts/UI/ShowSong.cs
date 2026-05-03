using UnityEngine;
using UnityEngine.SceneManagement;

public class ShowSong : MonoBehaviour
{
    Songs song;
    Songs.SongData currentSong;

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
    }

    public void SetCanvas()
    {

    }
}
