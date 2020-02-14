using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class trap_clone : MonoBehaviour
{
   
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
        if(other.gameObject.layer == 13)
        {
            CharacterController _character = FindObjectOfType<CharacterController>();
            if(_character._recallDisableHit == true)
            {
                _character._recallDisableHit = false;
                _character.Hit();
                _character._recallDisableHit = true;
            }
            else
            {
                _character.Hit();
            }
        }
    }
}
