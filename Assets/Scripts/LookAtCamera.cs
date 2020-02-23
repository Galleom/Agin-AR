using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class LookAtCamera : MonoBehaviour
{
    public Camera cameraToLookAt;
    void Update()
    {
        transform.LookAt(cameraToLookAt.transform);
    }
}

