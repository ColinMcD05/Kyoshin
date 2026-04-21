using TMPro;
using UnityEngine;

public class RewindTracker : MonoBehaviour
{
    TextMeshProUGUI rewindText;
    GameManager gameManager;

    private void Awake()
    {
        gameManager = transform.parent.GetComponentInParent<GameManager>();
        rewindText = GetComponent<TextMeshProUGUI>();
        ChangeText();
    }

    public void ChangeText()
    {
        rewindText.text = "Rewinds: " + gameManager.lives;
    }
}
