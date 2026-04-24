using UnityEngine;
using UnityEngine.SceneManagement;

public class InfiniteCheck : MonoBehaviour
{

    GameManager gameManager;

    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        if (gameManager.levels[3].lockStatus == Levels.LockStatus.Unlocked)
        {
            GetComponent<Collider>().isTrigger = true;
        }
        else
        {
            GetComponent<Collider>().isTrigger = false;
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
