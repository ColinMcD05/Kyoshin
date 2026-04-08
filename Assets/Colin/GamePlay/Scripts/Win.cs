using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Win : MonoBehaviour
{
    [SerializeField] Image moveImage;
    [SerializeField] Image stillImage;
    [SerializeField] Image image;
    public float fadeOutTime;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            moveImage.enabled = false;
            stillImage.enabled = false;
            other.GetComponent<Timing>().enabled = false;
            StartCoroutine(FadeOut());
        }
    }

    IEnumerator FadeOut()
    {
        float alpha = 0;
        Color color = image.color;
        while (image.color.a <= 1)
        {
            alpha += Time.deltaTime / fadeOutTime;
            color.a = alpha;
            image.color = color;
            yield return null;
        }
        SceneManager.LoadScene("WinScene");
    }
}
