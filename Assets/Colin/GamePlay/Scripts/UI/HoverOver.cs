using UnityEngine;
using UnityEngine.EventSystems;

public class HoverOver : MonoBehaviour, IPointerEnterHandler
{
    EventSystem eventSystem;

    void Start()
    {
        eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        eventSystem.SetSelectedGameObject(gameObject);
        Debug.Log("Entered");
        Debug.Log(eventSystem.currentSelectedGameObject.name);
    }
}
