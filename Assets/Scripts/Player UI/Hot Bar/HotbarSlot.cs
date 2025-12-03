using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HotbarSlot : MonoBehaviour, IPointerClickHandler
{
    public GameObject action;
    Hotbar hotbar;

    void Start()
    {
        hotbar = GetComponentInParent<Hotbar>();
    }

    public void SetSelected(bool selected)
    {
        action.SetActive(selected);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        hotbar.ClickSlot(this);
    }
}
