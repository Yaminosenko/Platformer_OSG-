using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interuptor_trap_Player : MonoBehaviour
{
    [SerializeField] private GameObject _playerTrap;
    [SerializeField] private GameObject _cloneTrap;

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
        if (other.gameObject.layer == 9 || other.gameObject.layer == 13)
        {
            _playerTrap.SetActive(true);
            _cloneTrap.SetActive(false);

        }
    }
}
