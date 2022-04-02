using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAt : MonoBehaviour
{
    [SerializeField] private Transform objectToLook;
    // Update is called once per frame
    void Update()
    {
        transform.LookAt(objectToLook);
    }
}
