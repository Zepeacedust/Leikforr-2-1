using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CameraFollow : MonoBehaviour
{
    public Vector3 offset = new Vector3(0, 0, 1);
    public Transform target;


    // Update is called once per frame
    void Update()
    {
        this.gameObject.transform.position = target.position + offset;
    }
}
