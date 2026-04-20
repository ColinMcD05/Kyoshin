using UnityEngine;
using UnityEngine.SceneManagement;

public class LoseScreen : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Invoke("Reset", 2);
    }

    private void Reset()
    {
        SceneManager.LoadScene(0);
    }
}
