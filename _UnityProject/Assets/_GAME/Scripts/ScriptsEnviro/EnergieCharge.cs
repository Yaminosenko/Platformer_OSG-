using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergieCharge : MonoBehaviour
{
   [SerializeField] private int _currentCharge = 0;
    private bool DelaiIsActivate = false;
    private bool DelaiIsActivateIncrement = false;

    //LED
    [SerializeField] private GameObject _LedChargement1;
    [SerializeField] private GameObject _LedChargement2;
    [SerializeField] private GameObject _LedChargement3;
    [SerializeField] private Material _LedMatActive;
    [SerializeField] private Material _LedMatDesactive;
    

    void Update()
    {
        switch (_currentCharge)
        {
            case 1:
                _LedChargement1.GetComponent<MeshRenderer>().material = _LedMatActive;
                _LedChargement2.GetComponent<MeshRenderer>().material = _LedMatDesactive;
                _LedChargement3.GetComponent<MeshRenderer>().material = _LedMatDesactive;

                break;
            case 2:
                _LedChargement1.GetComponent<MeshRenderer>().material = _LedMatActive;
                _LedChargement2.GetComponent<MeshRenderer>().material = _LedMatActive;
                _LedChargement3.GetComponent<MeshRenderer>().material = _LedMatDesactive;
                break;
            case 3:
                _LedChargement1.GetComponent<MeshRenderer>().material = _LedMatActive;
                _LedChargement2.GetComponent<MeshRenderer>().material = _LedMatActive;
                _LedChargement3.GetComponent<MeshRenderer>().material = _LedMatActive;
                break;
            default:
                _LedChargement1.GetComponent<MeshRenderer>().material = _LedMatDesactive;
                _LedChargement2.GetComponent<MeshRenderer>().material = _LedMatDesactive;
                _LedChargement3.GetComponent<MeshRenderer>().material = _LedMatDesactive;
                break;
        }



        if (_currentCharge > 0 && DelaiIsActivate == false)
        {
            StartCoroutine(DelaiDecrement());
        }
    }

    public void chargerecieve()
    {
        if (_currentCharge < 3 && DelaiIsActivateIncrement == false)
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
        yield return new WaitForSeconds(0.3f);
        DelaiIsActivateIncrement = false;
        _currentCharge++;
    }

}
