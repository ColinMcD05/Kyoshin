using System.Collections;
using TMPro;
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
    [SerializeField] InputActionReference slide, trick;
    [SerializeField] GameObject player;
    [SerializeField] TimingUI timingUI;
    PlayerLevelMovement playerLevelMovement;
    PlayerControllerLevel playerControllerLevel;
    [SerializeField] MoveBackwards moveBackwards;
    [SerializeField] Songs songClass;
    [HideInInspector] public Songs.SongData currentSong;
    AudioSource musicPlayer;
    [SerializeField] AudioSource countDownSound;
    public AudioClip ScratchSound;
    public AudioSource scratchSource;

    // IEnumerators
    IEnumerator resetCircle;

    // Variables for timing
    float songStartTime; // Time when the song started playing
    [HideInInspector] public float songPosition = 0; // Rough position of the song position of the song
    [HideInInspector] public float songPositionInBeats = 0; // find where it lands on the beat for correct timing
    float songTimePassed; // How much time has passsed since the song has played
    [Range(0, 0.33f)] public float messUpRange = 0.33f;
    public float rewindTimeUsed; // How much time has been rewinded

    public GameObject[] indicators;

    // Other variables
    public int comboNeeded = 3;
    public float startWaitTime = 1;
    public int goodScore = 5;
    string currentScene;
    int lastBeat;
    #endregion

    // OnEnable and OnDisble
    private void OnEnable()
    {
        if (songPosition > startWaitTime + 1)
        {
            SubscribeActions();
        }
    }

    private void OnDestroy()
    {
        UnSubscribeActions();
    }

    // Start
    #region
    private void Start()
    {
        currentScene = SceneManager.GetActiveScene().name;
        resetCircle = timingUI.IndicateBeat();
        songStartTime = 0;
        songPosition = 0;
        if (player == null)
            player = GameObject.FindWithTag("Player");
        playerControllerLevel = player.GetComponent<PlayerControllerLevel>();
        playerLevelMovement = player.GetComponent<PlayerLevelMovement>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        musicPlayer = GameObject.Find("Audio").transform.Find("Music").GetComponent<AudioSource>();
        musicPlayer.pitch = 1;

        // Changing current song based on the build index. May change
        foreach (Songs.SongData song in songClass.songs)
        {
            if (SceneManager.GetActiveScene().name == song.levelName)
            {
                currentSong = song;
                break;
            }
        }
        if (currentScene == "Infinite")
        {
            ChangeSong();
        }
        else
        {
            Invoke("StartMusic", startWaitTime);
        }
    }
    #endregion

    // Update
    #region
    private void Update()
    {
        SongPosition(out songPosition, out songPositionInBeats);
        if (songPosition >= currentSong.length && currentScene != "Infinite" && Time.timeSinceLevelLoad > 10)
        {
            SceneManager.LoadScene("LoseScreen");
        }
    }
    #endregion


    // StartMusic
    #region
    // Starts playing the music and sets up the timing for inputs
    public void StartMusic()
    {
        Invoke("SubscribeActions", currentSong.bps * 16);
        playerLevelMovement.SubscribeActions();
        StartCoroutine(CountDown());
        if (!playerLevelMovement.enabled)
        {
            playerLevelMovement.enabled = true; // Lets players move
        }
        playerControllerLevel.enabled = true;
        songStartTime = (float)AudioSettings.dspTime + 0.1f; // Sets songStartTime based on AudioSettings clock
        musicPlayer.clip = currentSong.song; // Sets current clip to current song clip
        musicPlayer.PlayScheduled(0.1f); // Players music
        StartCoroutine(resetCircle);
        StartCoroutine(Resync());

        float pitch = ((float)currentSong.bpm / 125f) * 2f;
        countDownSound.pitch = pitch;
        countDownSound.PlayScheduled(AudioSettings.dspTime + currentSong.bps * 13 + 0.1f);
    }
    #endregion

    // SongPosition
    #region
    void SongPosition(out float songPosition, out float songPositionInBeats)
    {
        if (musicPlayer.clip != null) {
            songPosition = (float)AudioSettings.dspTime - songStartTime - rewindTimeUsed; //Calculate song position in seconds by subtracting the time the song started and how much time was rewound by the current clock in AudioSettings
            songPositionInBeats = songPosition / currentSong.bps; // Calculate song in beats by dividing song position by the sec per beat of the song
            float thisBeat = songPositionInBeats;
            if (Mathf.Floor(songPositionInBeats) > lastBeat)
            {
                lastBeat = (int)Mathf.Floor(thisBeat);
            }
        }
        else
        {
            songPosition = 0;
            songPositionInBeats = 0;
        }

    }
    #endregion

    IEnumerator Resync()
    {
        float currentAudioTime = 0;
        while (musicPlayer.isPlaying)
        {
            currentAudioTime = (float)musicPlayer.timeSamples / musicPlayer.clip.frequency;
            songStartTime = (float)AudioSettings.dspTime - currentAudioTime;
            rewindTimeUsed = 0;
            yield return new WaitForSeconds(2);
        }
    }

    // ChangeSong
    #region
    // Changes song once a new scene is loaded allowing for this script to be permanent

    public void ChangeSong()
    {
        int newSong = Random.Range(0, songClass.songs.Count);
        if (currentSong != null)
        {
            while (songClass.songs[newSong].name == currentSong.name)
            {
                newSong = Random.Range(0, songClass.songs.Count);
            }
            StopCoroutine(resetCircle);
            UnSubscribeActions();
            currentSong = songClass.songs[newSong];
            StartMusic();
        }
        else
        {
            currentSong = songClass.songs[newSong];
        }
    }
    #endregion

    // CheckTiming
    #region
    // Check if input is hit at correct time
    public void CheckTime(InputAction.CallbackContext context)
    {
        float positionDecimal = GetDecimal(songPositionInBeats); // Getting decimals of beat position
        //Debug.Log(positionDecimal);
        if (positionDecimal <= messUpRange || positionDecimal >= 1 - messUpRange) // checks if action takes place in the mess up range.
        {
            // Do correct movement
            // Add combo
            gameManager.IncreaseCombo();
            // Check player speed, if not at max speed go faster
            if (moveBackwards.forwardSpeed < moveBackwards.maxSpeed && gameManager.combo % comboNeeded == 0)
            {
                moveBackwards.forwardSpeed *= 2;
                /*
                if (indicators != null)
                {
                    if (moveBackwards.forwardSpeed == 16)
                    {
                        indicators[0].SetActive(false);
                        indicators[1].SetActive(true);
                    }
                    else if (moveBackwards.forwardSpeed == 32)
                    {
                        indicators[1].SetActive(false);
                        indicators[2].SetActive(true);
                    }
                }*/
            }
            gameManager.AddScore(goodScore);
            //Debug.Log("Good");
        }
        else
        {
            // Else bad move
            // camera shakes intensify
            // Put sound effect here
            scratchSource.PlayOneShot(ScratchSound);
            playerControllerLevel.LoseLife();
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

    // Subscribe and Unsubscribe actions
    #region
    // Subscribes actions for both timing and player movement, ensureing CheckTime is infront
    public void SubscribeActions()
    {
        move.action.performed += CheckTime;
        jump.action.performed += CheckTime;
        slide.action.performed += CheckTime;
        trick.action.performed += CheckTime;
        // Unsubscribes player movement actions and subscribes them ensuring CheckTime happens first
        playerLevelMovement.UnSubscribeActions();
        playerLevelMovement.SubscribeActions();
    }

    // Unsubcribes timing actions only
    public void UnSubscribeActions()
    {
        move.action.performed -= CheckTime;
        jump.action.performed -= CheckTime;
        slide.action.performed -= CheckTime;
        trick.action.performed -= CheckTime;
    }
    #endregion

    // CountDown
    #region
    // Function that handles that count down UI
    IEnumerator CountDown()
    {
        // Gets the countDown text component
        TextMeshProUGUI countDown = timingUI.transform.Find("Countdown").GetComponent<TextMeshProUGUI>();
        // Waits 3 measures
        yield return new WaitForSeconds(currentSong.bps * 13 + 0.1f);

        // enabled countdown text
        countDown.enabled = true;

        // Changes countdown pitch based on bpm of the song


        // Play count down sound and displays #
        countDown.text = "3";

        // Waits one beat then changes text until it shows GO
        yield return new WaitForSeconds(currentSong.bps);
        countDown.text = "2";
        yield return new WaitForSeconds(currentSong.bps);
        countDown.text = "1";
        yield return new WaitForSeconds(currentSong.bps);
        countDown.text = "GO!";
        yield return new WaitForSeconds(currentSong.bps);
        // Disables countdown
        countDown.enabled = false;
    }
    #endregion
}
