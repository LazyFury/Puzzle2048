using System;
using System.Collections;
using UnityEngine;

public class CameraAutoDistance : MonoBehaviour
{

    public float zPos = -16;
    // Start is called before the first frame update
    void Start()
    {
        var local = transform.localPosition;
        transform.localPosition = new Vector3(local.x, local.y, zPos - (Screen.height / Screen.width) * 2);
    }


    // Update is called once per frame
    void Update()
    {

    }
}
