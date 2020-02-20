using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Dialogues : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI textDisplay;
    [SerializeField] private string[] Phrases;
    [SerializeField] private float typingSpeed;
    [SerializeField] private float TimeBeforeDestroy;
    [SerializeField] private TextMeshProUGUI _DidacticielBefore;
    private int index;


    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == 9)
        {
            Destroy(_DidacticielBefore);
            StartCoroutine(Type());
            StartCoroutine(TimeBeforeDeletText());
        }
    }

    IEnumerator TimeBeforeDeletText()
    {
        yield return new WaitForSeconds(TimeBeforeDestroy);
        Destroy(textDisplay);
    }

    IEnumerator Type()
    {
        foreach (char letter in Phrases[index].ToCharArray())
        {
            textDisplay.text += letter;
            yield return new WaitForSeconds(typingSpeed);

        }

    }

}
