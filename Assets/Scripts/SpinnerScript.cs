using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinnerScript : MonoBehaviour
{
    public float rotateSpeedX;
    public float rotateSpeedY;
    public float rotateSpeedZ;

    // Update is called once per frame
    void Update()
    {
        Vector3 newRotation = new Vector3(rotateSpeedX * Time.deltaTime,
            rotateSpeedY * Time.deltaTime, rotateSpeedZ * Time.deltaTime);
        transform.Rotate(newRotation);
    }
}
