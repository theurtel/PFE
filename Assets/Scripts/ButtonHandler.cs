using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonHandler : MonoBehaviour,IPointerDownHandler,IPointerUpHandler
{
    [SerializeField]
    public Button button;

    private bool m_isPressed;
    private string currentSkin;

    public void displaySkin(string skinPath)
    {
        Sprite skin = Resources.Load<Sprite>(skinPath);
        button.image.sprite = skin;
    }

    public void OnPointerDown(PointerEventData data)
    {
        m_isPressed = true;
    }
    public void OnPointerUp(PointerEventData data)
    {
        m_isPressed = false;
    }

    public bool isPressed()
    {
        return m_isPressed;
    }
}
