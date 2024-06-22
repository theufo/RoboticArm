using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotation : MonoBehaviour
{
    int _rotationSpeed = 15;

    void Update () 
    {
        transform.Rotate(0, _rotationSpeed * Time.deltaTime, 0);
    }
}
