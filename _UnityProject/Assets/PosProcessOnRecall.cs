using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PosProcessOnRecall : MonoBehaviour
{
    [SerializeField] private GhostBehavior ghostBehavior;
    [SerializeField] private GameManager Gamemanager;
    [SerializeField] private Material _postProcessToAsign;

    private void Start()
    {

    }
    void Update()
    {

        if(ghostBehavior._isOnTravel == true)
        {
            GetComponent<PostProcessExample>().PostProcessMat = _postProcessToAsign;
        }
        else
        {
            GetComponent<PostProcessExample>().PostProcessMat = null;
        }
    }
}
