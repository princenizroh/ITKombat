using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudMovement : MonoBehaviour
{
    public float speed = 1.0f; // Kecepatan awan
    public float leftEdge; // Posisi x untuk reset di ujung kiri
    public float rightEdge; // Posisi x di ujung kanan

    // Update is called once per frame
    void Update()
    {
        // Menggerakkan awan ke arah kanan
        transform.Translate(Vector3.right * speed * Time.deltaTime);

        // Jika awan mencapai ujung kanan, reset ke ujung kiri
        if (transform.position.x > rightEdge)
        {
            ResetCloudPosition();
        }
    }

    void ResetCloudPosition()
    {
        // Reset posisi awan ke ujung kiri
        transform.position = new Vector3(leftEdge, transform.position.y, transform.position.z);
    }
}
