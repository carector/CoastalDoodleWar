using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraIntro : MonoBehaviour
{
    public Text topText;
    public Text bottomText;

    public string topStr;
    public string bottomStr;

    public string topStr2;
    public string bottomStr2;

    public CameraScript cam;
    public PlayerController player;

    bool hasDoneFirstText;
    AudioSource audio;

    // Start is called before the first frame update
    void Start()
    {
        audio = GetComponent<AudioSource>();    
        StartCoroutine(StartAnim());
    }

    IEnumerator StartAnim()
    {
        if(!hasDoneFirstText)
            yield return new WaitForSeconds(1.5f);

        topText.color = Color.black;
        bottomText.color = Color.black;
        topText.text = "";
        bottomText.text = "";

        for (int i = 0; i <= topStr.Length; i++)
        {
            topText.text = topStr.Substring(0, i);
            yield return new WaitForSeconds(0.02f);
        }

        yield return new WaitForSeconds(0.5f);

        for (int i = 0; i <= bottomStr.Length; i++)
        {
            bottomText.text = bottomStr.Substring(0, i);
            yield return new WaitForSeconds(0.02f);
        }

        yield return new WaitForSeconds(1f);

        while (topText.color.a >= 0.05f)
        {
            topText.color = new Color(topText.color.r, topText.color.g, topText.color.b, topText.color.a - 0.05f);
            bottomText.color = new Color(topText.color.r, topText.color.g, topText.color.b, topText.color.a);

            yield return new WaitForSeconds(0.075f);
        }

        topText.color = Color.clear;
        bottomText.color = Color.clear;

        if (!hasDoneFirstText)
        {
            hasDoneFirstText = true;
            topStr = topStr2;
            bottomStr = bottomStr2;

            StartCoroutine(StartAnim());

        }
        else
        {
            cam.enabled = true;
            audio.enabled = true;
            player.canMove = true;
        }
    }
}
