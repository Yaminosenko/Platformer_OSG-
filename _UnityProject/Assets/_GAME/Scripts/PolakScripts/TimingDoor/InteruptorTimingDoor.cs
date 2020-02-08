using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteruptorTimingDoor : MonoBehaviour
{
    public TimingDoor _linkedObject;
    public GameObject _Ghost;



    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other);
        if (other.gameObject.layer == 9 || other.gameObject.layer == 13)
        {
            _linkedObject.Open();
        }
    }
}
