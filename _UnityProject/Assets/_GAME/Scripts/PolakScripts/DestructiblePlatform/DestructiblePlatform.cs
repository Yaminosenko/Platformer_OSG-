using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructiblePlatform : MonoBehaviour
{
     private GameObject platform;
     private MeshRenderer mesh;
    [SerializeField] private GameObject child;
     private BoxCollider collider;
    
    [SerializeField] private float _timeBeforeDestruction = 2;
    [SerializeField] private float _timeReconstruction = 4;
    // Start is called before the first frame update
    void Start()
    {
        platform = GetComponent<GameObject>();
        collider = GetComponentInChildren<BoxCollider>();
        mesh = GetComponentInChildren<MeshRenderer>();
        //child = GetComponentInChildren<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == 9)
        {
            StartCoroutine("Timetodestroy");
        }
    }

    IEnumerator Timetodestroy()
    {
        yield return new WaitForSeconds(_timeBeforeDestruction);
        collider.enabled = false;
        //mesh.enabled = false;
        child.SetActive(false);

        StartCoroutine("Timetorespawn");
    }

    IEnumerator Timetorespawn()
    {
        yield return new WaitForSeconds(_timeReconstruction);
        child.SetActive(true);
        collider.enabled = true;
        //collider.isTrigger = false;
        //mesh.enabled = true;
    }
}
