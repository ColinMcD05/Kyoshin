using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;


public class Timing : MonoBehaviour
{
    // Variables
    #region
    // References
    GameManager gameManager;
    [SerializeField] InputActionReference move;
    [SerializeField] GameObject player;
    Rewind rewind;
    PlayerControllerLevel playerControllerLevel;
    [SerializeField] Songs songClass;
    Songs.SongData currentSong;
    [SerializeField] AudioSource musicPlayer;

    // Variables for timing
    float songStartTime; // Time when the song started playing
    float songPosition; // Rough position of the song position of the song
    float songPositionInBeats; // find where it lands on the beat for correct timing
    float songTimePassed; // How much time has passsed since the song has played
    [Range(0, 0.25f)] float messUpRange;
    float rewindTimeUsed; // How much time has been rewinded
    int comboNeeded;
    #endregion

    // Start
    #region
    private void Start()
    {
        player = GameObject.Find("Player");
        rewind = player.GetComponent<Rewind>();
        playerControllerLevel = player.GetComponent<PlayerControllerLevel>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        foreach (Songs.SongData song in songClass.songs)
        {
            if (SceneManager.GetActiveScene().buildIndex == song.levelIndex)
            {
                currentSong = song;
            }
        }
        move.action.performed += CheckTime;
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
        songPositionInBeats = 1 + songPosition / currentSong.bps;
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

    // CheckTiming
    #region
    // Check if input if hit at correct time
    void CheckTime(InputAction.CallbackContext context)
    {
        float positionDecimal = GetDecimal(songPositionInBeats); // Getting position
        if (positionDecimal <= messUpRange || positionDecimal >= 1 - messUpRange)
        {
            // Do correct movement
            // Check player speed, if not at max speed go faster
            if (playerControllerLevel.forwardSpeed <= playerControllerLevel.maxSpeed && gameManager.combo % comboNeeded == 0)
            {
                playerControllerLevel.forwardSpeed *= 2;
            }
            gameManager.combo++;
        }
        else
        {
            // Else bad move
            gameManager.LoseLife();
            gameManager.combo = 0;
        }
    }
    #endregion

    // Decimal checks
    #region
    float GetDecimal(float position)
    {
        return Mathf.Abs(position - Mathf.Floor(position));
    }
    #endregion
}
