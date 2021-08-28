using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant : MonoBehaviour
{
    public GameObject objectToPull;
    public bool pullSelf;

    // Start is called before the first frame update
    void Start()
    {
        if(pullSelf)
        {
            objectToPull = this.gameObject;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
