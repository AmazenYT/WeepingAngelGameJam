using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CamMove : MonoBehaviour
{
    public Transform cameraPosition;

    void Update()
    {
        transform.position = cameraPosition.position;
    }

}
