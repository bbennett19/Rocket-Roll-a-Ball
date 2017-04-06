using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Rotates the object
public class Rotator : MonoBehaviour
{
    public int RotateSpeedX;
    public int RotateSpeedY;
    public int RotateSpeedZ;

    private void Update()
    {
        transform.Rotate(new Vector3(RotateSpeedX, RotateSpeedY, RotateSpeedZ) * Time.deltaTime);
    }

}
