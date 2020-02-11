using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergieCharge : MonoBehaviour
{
   [SerializeField] private int _currentCharge = 0;
    private bool DelaiIsActivate = false;

    void Update()
    {
        if (_currentCharge > 0 && DelaiIsActivate == false)
        {
            StartCoroutine(DelaiDecrement());
        }
    }

    void chargerecieve()
    {
        _currentCharge++;
    }
    IEnumerator DelaiDecrement()
    {
        DelaiIsActivate = true;
        yield return new WaitForSeconds(5);
        DelaiIsActivate = false;
        _currentCharge --;
    }

}
