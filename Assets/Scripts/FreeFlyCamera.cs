using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeFlyCamera : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float rotationSpeed;

    // Update is called once per frame
    void Update()
    {
        float deltaX = Input.GetAxis("Horizontal");
        float deltaY = Input.GetAxis("Vertical");
        float deltaZ = Input.GetAxis("Forward");
        transform.position += transform.right * deltaX * speed * Time.deltaTime;
        transform.position += transform.forward * deltaY * speed * Time.deltaTime;
        transform.position += transform.up * deltaZ * speed * Time.deltaTime;


        if (Input.GetMouseButton(1))
        {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");
            Vector3 newRotation = new Vector3(-mouseY, mouseX, 0f) * rotationSpeed * Time.deltaTime;

            transform.localRotation = Quaternion.Euler(transform.localRotation.eulerAngles + newRotation);

            //transform.Rotate(Vector3.up, mouseX, Space.World);
            //transform.Rotate(Vector3.right, -mouseY, Space.World);            
        }
    }
}
