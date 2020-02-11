using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteruptorTimingDoor : MonoBehaviour
{
    public TimingDoor _linkedObject;



    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 9 || other.gameObject.layer == 13)
        {
            _linkedObject.Open();
        }
    }
}
