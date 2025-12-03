using UnityEngine;
using UnityEngine.EventSystems;

public class Hotbar : MonoBehaviour
{
    public HotbarSlot[] slots;  
    public int currentIndex = 0; 

    void Start()
    {
        SelectSlot(currentIndex);
    }

    void Update()
    {
        HandleKeyboardInput();
        HandleScrollWheel();
    }

    public void SelectSlot(int index)
    {
        if (index < 0 || index >= slots.Length) return;

        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].SetSelected(i == index);
        }

        currentIndex = index;
    }

    void HandleKeyboardInput()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                SelectSlot(i);
            }
        }
    }

    void HandleScrollWheel()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll > 0f)
        {
            SelectSlot((currentIndex - 1 + slots.Length) % slots.Length);
        }
        else if (scroll < 0f)
        {
            SelectSlot((currentIndex + 1) % slots.Length);
        }
    }

    public void ClickSlot(HotbarSlot slot)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] == slot)
            {
                SelectSlot(i);
                break;
            }
        }
    }
}
