using TMPro;
using UnityEngine;

public class RewindTracker : MonoBehaviour
{
    TextMeshProUGUI rewindText;
    GameManager gameManager;

    private void Awake()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        rewindText = GetComponent<TextMeshProUGUI>();
        ChangeText();
    }

    public void ChangeText()
    {
        rewindText.text = "Lives: " + gameManager.lives;
    }
}
