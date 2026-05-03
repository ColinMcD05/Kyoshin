using UnityEngine;

public class RespawnCoins : MonoBehaviour
{
    public GameObject coins;

    void OnEnable()
    {
        if (coins != null)
        {
            int willSpawn = Random.Range(0, 2);
            if (willSpawn == 1) coins.SetActive(false);
            else
            {
                coins.SetActive(true);
                for (int i = 0; i < coins.transform.childCount; i++)
                {
                    coins.transform.GetChild(i).gameObject.SetActive(true);
                }
            }
        }
    }
}
