using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public Transform _test;
    private GameObject _yolo;
    float timer = 0;
    float recallPeriod = 2.0f;
    public List<Vector3> positions = new List<Vector3>();



    private void OnEnable()
    {
        Transform _testIns = Instantiate(_test);
        _test = _testIns;
    }
    private void Update()
    {
        TrackPositions();
        _test.position = positions[0];
    }
    void TrackPositions()
    {
        Debug.Log(positions);
        if (timer > recallPeriod)
        {
            positions.RemoveAt(0);
            positions.Add(transform.position);
        }
        else
        {
            positions.Add(transform.position);
            timer += Time.deltaTime;
        }
    }
}
