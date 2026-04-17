using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MoveBackwards : MonoBehaviour
{
    public int forwardSpeed = 8;
    [HideInInspector] public int maxSpeed;
    [HideInInspector] public int minSpeed;

    void Start()
    {
        maxSpeed = forwardSpeed * 4;
        minSpeed = forwardSpeed;
    }

    private void Update()
    {
        transform.Translate(Vector3.left * forwardSpeed * Time.deltaTime);
    }
}
