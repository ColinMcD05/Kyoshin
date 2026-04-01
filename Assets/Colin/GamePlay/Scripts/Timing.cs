using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Timing : MonoBehaviour
{
    // Variables
    #region
    // References
    [SerializeField] GameObject player;
    [SerializeField] Rewind rewind;
    [SerializeField] Songs songClass;
    Songs.SongData currentSong;
    [SerializeField] AudioSource musicPlayer;

    // Variables for timing
    float songStartTime; // Time when the song started playing
    float songPosition; // Rough position of the song position of the song
    float songPositionInBeats; // find where it lands on the beat for correct timing
    float songTimePassed; // How much time has passsed since the song has played
    float rewindTimeUsed; // How much time has been rewinded
    #endregion

    // Start
    #region
    private void Start()
    {
        player = GameObject.Find("Player");
        rewind = player.GetComponent<Rewind>();
        foreach (Songs.SongData song in songClass.songs)
        {
            if (SceneManager.GetActiveScene().buildIndex == song.levelIndex)
            {
                currentSong = song;
            }
        }
        SceneManager.sceneLoaded += ChangeSong;
        StartCoroutine(StartMusic());
    }
    #endregion

    // Update
    #region
    private void Update()
    {
        SongPosition(out songPosition, out songPositionInBeats);
    }
    #endregion


    // StartMusic
    #region
    // Starts playing the music and sets up the timing for inputs
    IEnumerator StartMusic()
    {
        yield return new WaitForSeconds(1);

        songStartTime = (float)AudioSettings.dspTime;
        musicPlayer.clip = currentSong.song;
        musicPlayer.Play();
    }
    #endregion

    // SongPosition
    void SongPosition(out float songPosition, out float songPositionInBeats)
    {
        songPosition = (float)(AudioSettings.dspTime - songStartTime);
        songPositionInBeats = songPosition / currentSong.bps;
    }

    // ChangeSong
    #region
    // Changes song once a new scene is loaded allowing for this script to be permanent
    void ChangeSong(Scene scene, LoadSceneMode mode)
    {
        foreach (Songs.SongData song in songClass.songs)
        {
            if (scene.buildIndex == song.levelIndex)
            {
                currentSong = song;
            }
        }
    }
    #endregion
}
