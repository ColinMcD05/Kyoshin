using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MoveBackwards : MonoBehaviour
{
    public int forwardSpeed;
    [HideInInspector] public int maxSpeed;
    [HideInInspector] public int minSpeed;

    IEnumerator delete;
    int countDown = 4;

    void Start()
    {
        maxSpeed = forwardSpeed * 4;
        minSpeed = forwardSpeed;
        delete = Delete();
    }

    private void Update()
    {
        transform.Translate(Vector3.left * forwardSpeed * Time.deltaTime);
    }

    private void OnBecameInvisible()
    {
        if (SceneManager.GetActiveScene().name == "Infinite")
        {
            StartCoroutine(delete);
        }
    }

    private void OnBecameVisible()
    {
        if (SceneManager.GetActiveScene().name == "Infinite")
        {
            StopCoroutine(delete);
        }
    }

    IEnumerator Delete()
    {
        yield return new WaitForSeconds(countDown);
        Destroy(gameObject);
    }
}
