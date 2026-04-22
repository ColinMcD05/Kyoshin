using UnityEngine;
using UnityEngine.UI;
public class Coin : MonoBehaviour
{
    public int coinValue = 1;
    public float dashValue = 10;
    public GameManager gameManager;
    Slider dashSlider;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        dashSlider = gameManager.gameObject.transform.Find("Canvas").transform.Find("Dash").GetComponent<Slider>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            float addedValue = 1 / dashValue;
            gameManager.AddScore(coinValue);
            other.GetComponent<Dash>().AddDash(addedValue);
            if (dashSlider.value < dashSlider.maxValue)
            {
                dashSlider.value += addedValue;
            }
            Destroy(gameObject);
        }
    }
}
