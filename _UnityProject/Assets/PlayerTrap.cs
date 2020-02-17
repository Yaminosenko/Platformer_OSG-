using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTrap : MonoBehaviour
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
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == 9)
        {
            _firstTrap.SetActive(true);
            _secondTrap.SetActive(false);
        }
    }
}
