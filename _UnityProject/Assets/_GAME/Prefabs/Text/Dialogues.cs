using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Dialogues : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI textDisplay;
    [SerializeField] private string[] Phrases;
    [SerializeField] private float typingSpeed;
    private int index;


    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == 9)
        {
            StartCoroutine(Type());
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 9)
        {
            index++;
        }
    }



    IEnumerator Type()
    {
        foreach (char letter in Phrases[index].ToCharArray())
        {
            textDisplay.text += letter;
            yield return new WaitForSeconds(typingSpeed);

        }

    }

    [SerializeField]
    private void NextPhrase()
    {
        if (index < Phrases.Length - 1)
        {
            index++;
            textDisplay.text = "";
            StartCoroutine(Type());
        }
        else
        {
            textDisplay.text = "";
        }
    }
}
