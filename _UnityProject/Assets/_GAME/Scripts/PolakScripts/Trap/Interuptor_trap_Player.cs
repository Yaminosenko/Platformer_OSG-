using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interuptor_trap_Player : MonoBehaviour
{
    [SerializeField] private GameObject _playerTrap;
    [SerializeField] private GameObject _cloneTrap;
    [SerializeField] private GameObject _InterupteurClone1;
    [SerializeField] private GameObject _InterupteurClone2;
    [SerializeField] private GameObject _InterupteurPlayer1;
    [SerializeField] private GameObject _InterupteurPlayer2;
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
            _InterupteurClone1.SetActive(true);
            _InterupteurClone2.SetActive(true);
            _InterupteurPlayer1.SetActive(false);
            _InterupteurPlayer2.SetActive(false);

        }
    }
}
