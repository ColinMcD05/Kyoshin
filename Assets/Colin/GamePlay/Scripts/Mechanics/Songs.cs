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
        public AudioClip song;
        public int level;
        public int levelIndex;
        public int bpm;
        public float bps; // time between beats. should really be called spb but too late now.
        public float length; // in seconds  

        public SongData(string name, AudioClip song, int level, int levelIndex, int bpm, float length) // When new class instantiated assign variables correctly
        {
            this.name = name;
            this.song = song;
            this.level = level;
            this.levelIndex = levelIndex;
            this.bpm = bpm;
            this.bps = 60f / bpm; // Calculates bps
            this.length = length;
        }
    }

    public List<SongData> songs;
    private void Awake()
    {
        songs = new List<SongData>()
        {
            new SongData("Bullet Train", songClips[0], 4, 0, 155, 137),
            new SongData("Hakone", songClips[1], 1, 1, 125, 146)
        };
    }
}
