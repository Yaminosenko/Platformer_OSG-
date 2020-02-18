using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartShadderTropCool : MonoBehaviour
{
    [SerializeField] private Material _ShaderDoorClone;
    public bool _activeLaser = false;
    private float _InMinToMax = 1f;

    private void OnEnable()
    {
        _activeLaser = true;
        _InMinToMax = 0.5f;
    }

    // Update is called once per frame
    void Update()
    {
        if(_activeLaser == true)
        {
            _InMinToMax -= Time.deltaTime * 1.3f;
            _InMinToMax =  Mathf.Clamp(_InMinToMax, 0, 1);
            _ShaderDoorClone.SetFloat("_StepValueInMINtoMax", _InMinToMax);
        }


    }
}
