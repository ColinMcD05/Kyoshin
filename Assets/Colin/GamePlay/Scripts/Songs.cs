using UnityEngine;
using System.Collections.Generic;

public class Songs : MonoBehaviour
{
    [SerializeField] AudioClip songs;

    public class SongData
    {
        public string name;
        public AudioClip song;
        public int levelIndex;
        public int bpm;
        public float bps; // time between beats
        public float length; // in seconds  

        void Awake()
        {
            bps = 60 / bpm;
        }
    }

    public List<SongData> songs;
    private void Awake()
    {
        songs = new List<SongData>()
        {
            new SongData
            {
                name = "Bullet Train",
                bpm = 155,
                length = 137
            }
        };
    }
}
