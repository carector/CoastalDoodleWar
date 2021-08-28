using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonEvent : MonoBehaviour
{
    public enum EventType
    {
        moveToLocation
    }

    bool isPressed;

    public Collider2D wall;

    public ButtonScript[] buttons;
    public EventType eventType;

    public Vector3 newLocation;

    Vector3 originalLocation;

    // Start is called before the first frame update
    void Start()
    {
        originalLocation = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (eventType == EventType.moveToLocation)
        {
            for (int i = 0; i < buttons.Length; i++)
            {
                if (buttons[i].isPressed)
                {
                    isPressed = true;
                    break;
                }
                else
                {
                    isPressed = false;
                }
            }

            if (isPressed)
            {
                transform.position = Vector3.Lerp(transform.position, newLocation, 0.25f);
                wall.isTrigger = true;
            }

            else
            {
                transform.position = Vector3.Lerp(transform.position, originalLocation, 0.25f);
                wall.isTrigger = false;
            }
        }
    }
}
