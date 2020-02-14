using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergieCharge : MonoBehaviour
{
    public SimpleDoor _linkedObject;
    public SimpleDoor _linkedObject2;
    [SerializeField] private int _currentCharge = 0;
    private bool DelaiIsActivate = false;
    private bool DelaiIsActivateIncrement = false;

    //LED
    [SerializeField] private GameObject _LedChargement1;
    [SerializeField] private GameObject _LedChargement2;
    [SerializeField] private GameObject _LedChargement3;
    [SerializeField] private Material _LedMatActive;
    [SerializeField] private Material _LedMatDesactive;

    [SerializeField] private float _TimerDesincrementation = 4f;
    [SerializeField] private float _TimerIncrementation = 0.3f;

    

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
        if(_currentCharge >= 3)
        {
            _linkedObject.Open();
            if(_linkedObject2 != null)
            {
            _linkedObject2.Open();

            }
        }
        else
        {
            if (_linkedObject2 != null)
            {
                _linkedObject2.Close();

            }
            _linkedObject.Close();
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
        Debug.Log("why");
        DelaiIsActivate = true;
        yield return new WaitForSeconds(_TimerDesincrementation);
        DelaiIsActivate = false;
        _currentCharge --;
    }
    IEnumerator DelaiIncrement()
    {
        DelaiIsActivateIncrement = true;
        StopCoroutine(DelaiDecrement());
        yield return new WaitForSeconds(_TimerIncrementation);
        DelaiIsActivateIncrement = false;
        _currentCharge++;
    }

}
