using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform Player1; // Reference to the player
    public float smoothSpeed = 0.125f; // Speed of the camera’s smoothing
    public Vector3 offset; // Offset from the player
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 desiredPosition = Player1.position + offset; // Desired position of the camera
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed); // Smooth transition
        transform.position = smoothedPosition; // Apply the new position
    }
}
