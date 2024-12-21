using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoCameraMovement : MonoBehaviour
{
    public float speed = 5f;

    void Update()
    {
        // Move the camera to the left automatically
        transform.Translate(Vector3.left * speed * Time.deltaTime);
    }
}
