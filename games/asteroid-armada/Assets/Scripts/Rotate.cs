using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    public bool rotateObject = true;

    [SerializeField]
    private float rotationSpeed = 15f;

    void Update()
    {
        if (rotateObject == true)
        {
            transform.Rotate(new Vector3(0, 0, 20) * Time.deltaTime * rotationSpeed);
        }
    }
}
