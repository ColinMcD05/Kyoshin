using UnityEngine;
using System;
public class Coin : MonoBehaviour
{
    public int coinValue = 1;
    public int dashValue = 10;
    public GameManager gameManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(gameManager == null){
            gameManager = FindFirstObjectByType<GameManager>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject);
            gameManager.AddScore(coinValue);
            //other.GetComponent<Dash>().AddDash(1 / dashValue);
        }
    }
}
