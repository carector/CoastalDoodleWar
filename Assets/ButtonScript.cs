using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonScript : MonoBehaviour
{
    public Sprite button;
    public Sprite pressed;

    public bool isPressed;

    SpriteRenderer spr;
    GameObject presser;

    // Start is called before the first frame update
    void Start()
    {
        spr = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if(other.gameObject.tag == "Player" || other.gameObject.tag == "Plant")
        {
            spr.sprite = pressed;
            presser = other.gameObject;
            isPressed = true;
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject == presser)
        {
            spr.sprite = button;
            presser = null;
            isPressed = false;
        }
    }
}
