using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A class which designates the attached object as a LetterEater machine.
/// The machine eats letters. If the sequence of letters were correct, it spits out a planet as reward.
/// </summary>
public class LetterEater : MonoBehaviour
{
    public string password;
    public string eatenLetters; // must match the password to get the reward
    public Transform spawnLetterInit; // Initial spawn point for the letters
    public Transform spawnReward; // Spawn point of the reward
    public Planet rewardPlanet;
    static List<Letter> alphabetResource = new List<Letter>();


    /// <summary>
    /// Load alphabet prefabs into memory via lazy initialization.
    /// </summary>
    static void LoadAlphabetResource()
    {
        if (alphabetResource.Count > 0)
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

    /// <summary>
    /// Unity's OnTriggerEnter method. This one in particular is used to eat Letters.
    /// </summary>
    /// <param name="other">The other Collider instance which entered the trigger zone.</param>
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
