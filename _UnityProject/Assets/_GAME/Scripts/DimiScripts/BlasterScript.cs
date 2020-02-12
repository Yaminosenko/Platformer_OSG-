using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlasterScript : MonoBehaviour
{
    public float speedVelocity;

    private void Update()
    {
        transform.Translate(Vector3.forward * speedVelocity, Space.Self);
    }
}
