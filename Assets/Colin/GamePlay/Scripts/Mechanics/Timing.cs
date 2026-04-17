using System.Collections;
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
    [SerializeField] InputActionReference jump;
    [SerializeField] InputActionReference slide;
    [SerializeField] GameObject player;
    [SerializeField] TimingUI timingUI;
    Rewind rewind;
    PlayerLevelMovement playerLevelMovement;
    PlayerControllerLevel playerControllerLevel;
    PlayerMoveForward playerForward;
    [SerializeField] Songs songClass;
    [HideInInspector] public Songs.SongData currentSong;
    [SerializeField] AudioSource musicPlayer;

    // IEnumerators
    IEnumerator resetCircle;

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
    public int maxMult = 10;
    public int comboNeededMult = 5;
    public int goodScore = 5;
    string currentScene;
    #endregion

    // OnEnable and OnDisble
    private void OnEnable()
    {
        if (Time.timeSinceLevelLoad > startWaitTime + 1)
        {
            move.action.performed += CheckTime;
            jump.action.performed += CheckTime;
            slide.action.performed += CheckTime;
        }
    }

    private void OnDisable()
    {
        move.action.performed -= CheckTime;
        jump.action.performed -= CheckTime;
        slide.action.performed -= CheckTime;
    }

    // Start
    #region
    private void Start()
    {
        currentScene = SceneManager.GetActiveScene().name;
        resetCircle = timingUI.ResetCircle();
        songStartTime = 0;
        if (player == null)
            player = GameObject.FindWithTag("Player");
        playerControllerLevel = player.GetComponent<PlayerControllerLevel>();
        rewind = player.GetComponent<Rewind>();
        playerLevelMovement = player.GetComponent<PlayerLevelMovement>();
        playerForward = player.GetComponent<PlayerMoveForward>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        // Changing current song based on the build index. May change
        foreach (Songs.SongData song in songClass.songs)
        {
            if (SceneManager.GetActiveScene().name == song.levelName)
            {
                currentSong = song;
                break;
            }
        }
        if (currentSong == null)
        {
            ChangeSong(currentSong != null);
        }
        Invoke("StartMusic", startWaitTime);
    }
    #endregion

    // Update
    #region
    private void Update()
    {
        SongPosition(out songPosition, out songPositionInBeats);
        if (songPosition >= currentSong.length && songStartTime != 0 && currentScene != "Infinite")
        {
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
        jump.action.performed += CheckTime;
        slide.action.performed += CheckTime;
        playerLevelMovement.enabled = true; // Lets players move
        songStartTime = (float)AudioSettings.dspTime; // Sets songStartTime based on AudioSettings clock
        musicPlayer.clip = currentSong.song; // Sets current clip to current song clip
        musicPlayer.Play(); // Players music
        StartCoroutine(resetCircle);
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

    public void ChangeSong(bool hasSong)
    {
        int newSong = Random.Range(0, songClass.songs.Count);
        if (hasSong)
        {
            while (songClass.songs[newSong].name == currentSong.name)
            {
                newSong = Random.Range(0, songClass.songs.Count);
            }
        }
        currentSong = songClass.songs[newSong];
    }
    #endregion

    // CheckTiming
    #region
    // Check if input is hit at correct time
    public void CheckTime(InputAction.CallbackContext context)
    {
        int mult = goodScore + gameManager.combo / comboNeededMult;
        if (mult > maxMult)
        {
            mult = maxMult;
        }
        float positionDecimal = GetDecimal(songPositionInBeats); // Getting decimals of beat position
        //Debug.Log(positionDecimal);
        if (positionDecimal <= messUpRange*0.5 || positionDecimal >= 1 - messUpRange) // checks if action takes place in the mess up range.
        {
            // Do correct movement
            // Add combo
            gameManager.combo++;
            // Check player speed, if not at max speed go faster
            if (playerForward.forwardSpeed < playerForward.maxSpeed && gameManager.combo % comboNeeded == 0)
            {
                playerForward.forwardSpeed *= 2;
            }
            gameManager.AddScore(mult);
            playerLevelMovement.goodMove = true;
            //Debug.Log("Good");
        }
        else
        {
            // Else bad move
            // camera shakes intensify
            playerControllerLevel.LoseLife();
            // reset combo and speed
            gameManager.combo = 0;
            playerForward.forwardSpeed = playerForward.minSpeed;
            playerLevelMovement.goodMove = false;
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
