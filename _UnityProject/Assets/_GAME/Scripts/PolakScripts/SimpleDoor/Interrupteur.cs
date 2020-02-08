using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interrupteur : MonoBehaviour
{
    public SimpleDoor _linkedObject;
    [SerializeField] private int _count = 0;



    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("hi");
        if (other.gameObject.layer == 9 || other.gameObject.layer == 13)
        {
            _count++;

            if (_count > 1)
            {
                _count = 0;
            }

            if (_count == 1)
            {
                _linkedObject.Open();
            }


            if (_count == 0)
            {
                _linkedObject.Close();
            }
        }
    }
}
