using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interrupteur : MonoBehaviour
{
    public SimpleDoor _linkedObject;
    [SerializeField] private int _count = 0; 

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
       
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.tag == "Player" || other.gameObject.tag == "Clone")
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
