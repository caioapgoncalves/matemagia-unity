using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CAMERACON : MonoBehaviour
{
    public float speed = 5.0f;
    public float sensitivity = 5.0f;
    private bool isRotating = false;
    void Update()
{
    // Move the camera forward, backward, left, and right
    transform.position += transform.forward * Input.GetAxis("Vertical") * speed * Time.deltaTime;
    transform.position += transform.right * Input.GetAxis("Horizontal") * speed * Time.deltaTime;

    if (Input.GetMouseButtonDown(1)) 
        {
            isRotating = true;
          
        }

        if (Input.GetMouseButtonUp(1))
        {
            isRotating = false;
        }

        if (isRotating)
        {
            RotateCamera();
        }


    // Rotate the camera based on the mouse movement

            
        float verticalMovement = Input.GetKey(KeyCode.Q) ? 1.0f : (Input.GetKey(KeyCode.E) ? -1.0f : 0.0f);
        MoveCameraVertical(verticalMovement);
    
    
}
 void RotateCamera()
 {
    float mouseX = Input.GetAxis("Mouse X");
    float mouseY = Input.GetAxis("Mouse Y");
    transform.eulerAngles += new Vector3(-mouseY * sensitivity, mouseX * sensitivity, 0);
 }

 void MoveCameraVertical(float direction)
    {
        Vector3 verticalMovement = Vector3.up * direction * speed * Time.deltaTime;
        transform.Translate(verticalMovement, Space.World);
    }
}
