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
            _character.Hit();
        }
    }
}
