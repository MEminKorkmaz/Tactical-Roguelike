using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedRotator : MonoBehaviour
{
    public float RotatingSpeed;


    void Update()
    {
        transform.Rotate(0f , RotatingSpeed * Time.deltaTime , 0f);
    }
}
