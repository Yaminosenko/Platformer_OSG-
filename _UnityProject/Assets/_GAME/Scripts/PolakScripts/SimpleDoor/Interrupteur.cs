using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;


public class Interrupteur : MonoBehaviour
{
    public SimpleDoor _linkedObject;
    [SerializeField] private int _count = 0;
    [SerializeField] private GameObject _uxInteractionFeedback;
    [SerializeField] private Material _materailEmissive;


    private bool _isOntrigger = false;


     private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 9)
        {
        _isOntrigger = true;
    
        }
     
        if (other.gameObject.layer == 9 || other.gameObject.layer == 13 )
        {
         
                _count++;

                if (_count > 1)
                {
                    _count = 0;
                }

                if (_count == 1)
                {
                if(_materailEmissive!=null)
                _materailEmissive.EnableKeyword("_EMISSION");
                    _linkedObject.Open();
                }

                if (_count == 0)
                {
                if(_materailEmissive!=null)
                _materailEmissive.DisableKeyword("_EMISSION");
                _linkedObject.Close();
                }
            
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 9)
        {
             _isOntrigger = false;
          
        }
    }
}
