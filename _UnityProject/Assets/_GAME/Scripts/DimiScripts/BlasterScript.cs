using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlasterScript : MonoBehaviour
{
    public float speedVelocity;

    private void Update()
    {
        transform.Translate(0, 0, speedVelocity, Space.Self);
    }
}
