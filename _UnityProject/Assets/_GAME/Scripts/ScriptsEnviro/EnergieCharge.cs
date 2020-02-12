using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergieCharge : MonoBehaviour
{
   [SerializeField] private int _currentCharge = 0;
    private bool DelaiIsActivate = false;
    private bool DelaiIsActivateIncrement = false;

    void Update()
    {
        if (_currentCharge > 0 && DelaiIsActivate == false)
        {
            StartCoroutine(DelaiDecrement());
        }
    }

    public void chargerecieve()
    {
        if (_currentCharge < 10 && DelaiIsActivateIncrement == false)
        {
            StartCoroutine(DelaiIncrement());
        }
    }
    IEnumerator DelaiDecrement()
    {
        DelaiIsActivate = true;
        yield return new WaitForSeconds(2);
        DelaiIsActivate = false;
        _currentCharge --;
    }
    IEnumerator DelaiIncrement()
    {
        DelaiIsActivateIncrement = true;
        yield return new WaitForSeconds(0.1f);
        DelaiIsActivateIncrement = false;
        _currentCharge++;
    }

}
