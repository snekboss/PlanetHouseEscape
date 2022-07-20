using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LetterEater : MonoBehaviour
{
    public string password;
    public string eatenLetters;
    public Transform spawnLetterInit;
    public Transform spawnReward;
    public Planet rewardPlanet;
    public static List<Letter> alphabetResource; // make this private after debugging


    static void LoadAlphabetResource()
    {
        if (alphabetResource != null)
        {
            return;
        }

        alphabetResource = new List<Letter>();

        char c = 'A';
        while (c <= 'Z')
        {
            Letter L = Resources.Load<Letter>("Alphabet/Letter" + c);
            if (L != null)
            {
                alphabetResource.Add(L);
            }

            c++;
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        Letter L = other.GetComponent<Letter>();
    }

    /// <summary>
    /// Unity's Start method. Start is called before the first frame update.
    /// </summary>
    void Start()
    {
        LoadAlphabetResource();
        // Instantiate<Letter>(alphabetResource[0]);
    }

    /// <summary>
    /// Unity's Update method. Update is called once per frame.
    /// </summary>
    void Update()
    {

    }
}
