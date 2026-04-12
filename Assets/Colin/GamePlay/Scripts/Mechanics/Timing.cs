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
    [SerializeField] TimingUI timingUI;
    Rewind rewind;
    PlayerControllerLevel playerControllerLevel;
    [SerializeField] Songs songClass;
    [HideInInspector] public Songs.SongData currentSong;
    [SerializeField] AudioSource musicPlayer;

    // Variables for timing
    float songStartTime; // Time when the song started playing
    float songPosition = 0; // Rough position of the song position of the song
    [HideInInspector] public float songPositionInBeats = 0; // find where it lands on the beat for correct timing
    float songTimePassed; // How much time has passsed since the song has played
    [Range(0, 0.33f)] public float messUpRange = 0.33f;
    public float rewindTimeUsed; // How much time has been rewinded

    // Other variables
    public int comboNeeded = 3;
    public float startWaitTime = 1;
    #endregion

    // OnEnable and OnDisble
    private void OnEnable()
    {
        if (Time.timeSinceLevelLoad > startWaitTime + 1)
        {
            move.action.performed += CheckTime;
        }
    }

    private void OnDisable()
    {
        move.action.performed -= CheckTime;
    }

    // Start
    #region
    private void Start()
    {
        songStartTime = 0;
        Debug.Log((float)AudioSettings.dspTime);
        player = GameObject.FindWithTag("Player");
        rewind = player.GetComponent<Rewind>();
        playerControllerLevel = player.GetComponent<PlayerControllerLevel>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        // Changing current song based on the build index. May change
        foreach (Songs.SongData song in songClass.songs)
        {
            if (SceneManager.GetActiveScene().buildIndex == song.levelIndex)
            {
                currentSong = song;
            }
        }
        SceneManager.sceneLoaded += ChangeSong; // Should change the current song once scene is loaded
        Invoke("StartMusic", startWaitTime);
    }
    #endregion

    // Update
    #region
    private void Update()
    {
        SongPosition(out songPosition, out songPositionInBeats);
        if (songPosition >= currentSong.length && songStartTime != 0)
        {
            Debug.Log(songPosition);
            SceneManager.LoadScene("LoseScreen");
        }
    }
    #endregion


    // StartMusic
    #region
    // Starts playing the music and sets up the timing for inputs
    void StartMusic()
    {
        move.action.performed += CheckTime; // Adds the function check time to the LeftRight action so it only checks when pressed
        playerControllerLevel.enabled = true; // Lets players move
        songStartTime = (float)AudioSettings.dspTime; // Sets songStartTime based on AudioSettings clock
        musicPlayer.clip = currentSong.song; // Sets current clip to current song clip
        musicPlayer.Play(); // Players music
        StartCoroutine(timingUI.ResetCircle());
    }
    #endregion

    // SongPosition
    #region
    void SongPosition(out float songPosition, out float songPositionInBeats)
    {
        songPosition = (float)(AudioSettings.dspTime - songStartTime - rewindTimeUsed); //Calculate song position in seconds by subtracting the time the song started and how much time was rewound by the current clock in AudioSettings
        songPositionInBeats = 1 + songPosition / currentSong.bps; // Calculate song in beats by dividing song position by the sec per beat of the song
    }
    #endregion

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
    // Check if input is hit at correct time
    void CheckTime(InputAction.CallbackContext context)
    {
        float positionDecimal = GetDecimal(songPositionInBeats); // Getting decimals of beat position
        //Debug.Log(positionDecimal);
        if (positionDecimal <= messUpRange || positionDecimal >= 1 - messUpRange) // checks if action takes place in the mess up range.
        {
            // Do correct movement
            // Add combo
            gameManager.combo++;
            // Check player speed, if not at max speed go faster
            if (playerControllerLevel.forwardSpeed <= playerControllerLevel.maxSpeed && gameManager.combo % comboNeeded == 0)
            {
                playerControllerLevel.forwardSpeed *= 2;
            }
            Debug.Log("Good");
        }
        else
        {
            // Else bad move
            // camera shakes intensify
            playerControllerLevel.LoseLife();
            // reset combo and speed
            gameManager.combo = 0;
            playerControllerLevel.forwardSpeed = playerControllerLevel.minSpeed;
            Debug.Log("Bad");
        }
    }
    #endregion

    // Decimal checks
    #region
    float GetDecimal(float position)
    {
        return Mathf.Abs(position - Mathf.Floor(position)); // produces only the decimal
    }
    #endregion
}
