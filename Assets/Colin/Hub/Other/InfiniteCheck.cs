using Microsoft.Unity.VisualStudio.Editor;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InfiniteCheck : MonoBehaviour
{

    GameManager gameManager;
    public TextMeshPro canvas, arrow;

    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        if (gameManager.levels[3].lockStatus == Levels.LockStatus.Unlocked)
        {
            GetComponent<Collider>().isTrigger = true;
            canvas.enabled = true;
            arrow.enabled = true;
        }
        else
        {
            GetComponent<Collider>().isTrigger = false;
            canvas.enabled = false;
            arrow.enabled = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            SceneManager.LoadScene("Infinite");
        }
    }
}
