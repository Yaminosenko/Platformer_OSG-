using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructiblePlatform : MonoBehaviour
{
    [SerializeField] private GameObject platform;
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
        if(other.gameObject.layer == 9)
        {
            StartCoroutine("Timetodestroy");
        }
    }

    IEnumerator Timetodestroy()
    {
        yield return new WaitForSeconds(2);
        platform.SetActive(false);
        StartCoroutine("Timetorespawn");
    }

    IEnumerator Timetorespawn()
    {
        yield return new WaitForSeconds(4);
        platform.SetActive(true);
    }
}
