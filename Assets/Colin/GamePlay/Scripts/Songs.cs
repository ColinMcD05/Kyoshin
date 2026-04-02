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
        public int levelIndex;
        public int bpm;
        public float bps; // time between beats
        public float length; // in seconds  

        public SongData(string name, AudioClip song, int levelIndex, int bpm, float length)
        {
            this.name = name;
            this.song = song;
            this.levelIndex = levelIndex;
            this.bpm = bpm;
            this.bps = 60f / bpm;
            this.length = length;
        }
    }

    public List<SongData> songs;
    private void Awake()
    {
        songs = new List<SongData>()
        {
            new SongData("Bullet Train", songClips[0], 0, 155, 137)
        };
    }
}
