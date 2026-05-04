using UnityEngine;
using System.Collections.Generic;

public class Songs : MonoBehaviour
{
    // Variables
    #region
    // References
    [SerializeField] AudioClip[] songClips; // Holds all the songs
    #endregion

    public class SongData
    {
        public string name;
        public string levelName;
        public AudioClip song;
        public int level;
        public int bpm;
        public float bps; // time between beats. should really be called spb but too late now.
        public float length; // in seconds
        public float bestTime;

        public SongData(string name, string levelName, AudioClip song, int level, int bpm, float length) // When new class instantiated assign variables correctly
        {
            this.name = name;
            this.levelName = levelName;
            this.song = song;
            this.level = level;
            this.bpm = bpm;
            this.bps = 60f / bpm; // Calculates bps
            this.length = length;
        }

        public SongData(string name, string levelName, AudioClip song, int level, int bpm, float length, float bestTime) // When new class instantiated assign variables correctly
        {
            this.name = name;
            this.levelName = levelName;
            this.song = song;
            this.level = level;
            this.bpm = bpm;
            this.bps = 60f / bpm; // Calculates bps
            this.length = length;
            this.bestTime = bestTime;
        }
    }

    public List<SongData> songs;
    private void Awake()
    {
        songs = new List<SongData>()
        {
            new SongData("Torii Gate", "Hakone", songClips[0], 1, 125, 146, 64),
            new SongData("Adrift", "Kyoto", songClips[1], 2, 133, 130, 44),
            new SongData("BulletTrain", "Tokyo", songClips[2], 3, 155, 137, 65),
            new SongData("DayDreamer", "none", songClips[3], 0, 127, 144),
            new SongData("Heatwave", "HUB", songClips[4], 0, 120, 168)
        };
    }
}
