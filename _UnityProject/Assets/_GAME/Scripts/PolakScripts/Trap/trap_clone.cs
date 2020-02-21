using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class trap_clone : MonoBehaviour
{

    public GameObject _firstTrap;
    public GameObject _secondTrap;
    [SerializeField] private GameObject _InterupteurClone1;
    [SerializeField] private GameObject _InterupteurClone2;
    [SerializeField] private GameObject _InterupteurPlayer1;
    [SerializeField] private GameObject _InterupteurPlayer2;

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
        _InterupteurClone1.SetActive(true);
        _InterupteurClone2.SetActive(true);
        _InterupteurPlayer1.SetActive(false);
        _InterupteurPlayer2.SetActive(false);

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
