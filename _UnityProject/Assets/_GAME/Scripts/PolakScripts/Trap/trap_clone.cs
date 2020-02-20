using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class trap_clone : MonoBehaviour
{

    public GameObject _firstTrap;
    public GameObject _secondTrap;


    // Start is called before the first frame update
    void Start()
    {
       // _firstTrap.SetActive(true);
       // _secondTrap.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        



    }

    public void Reset()
    {
        _firstTrap.SetActive(true);
        _secondTrap.SetActive(false);

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
                _firstTrap.SetActive(true);
                _secondTrap.SetActive(false);

            }
        }
    }
}
