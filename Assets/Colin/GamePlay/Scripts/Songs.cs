using UnityEngine;
using System.Collections.Generic;

public class Songs : MonoBehaviour
{
    public class SongData
    {
        public string name;
        public int levelIndex;
        public int bpm;
        public float timeBetween; // time between beats
        public float length; // in seconds  

        void Start()
        {
            timeBetween = 60 / bpm;
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
