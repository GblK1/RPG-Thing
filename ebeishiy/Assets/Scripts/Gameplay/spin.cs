using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spin : MonoBehaviour
{
    [SerializeField] private float spinSpeed;
    public bool pickedUp;
    private void FixedUpdate()
    {
        if (!pickedUp)
        {
            transform.Rotate(0, spinSpeed, 0);
        }
    }
}
