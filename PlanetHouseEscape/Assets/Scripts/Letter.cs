using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A class which designates the attached game object as a Letter.
/// Used for the LetterEater puzzle.
/// </summary>
public class Letter : MonoBehaviour
{
    public char character;

    BoxCollider col;
    Rigidbody rbody;

    const float scaleX = 0.25f;
    const float scaleY = 0.025f;
    const float scaleZ = 0.25f;


    static Dictionary<char, Letter> alphabetPrefabs; // chars are upper case
    /// <summary>
    /// Dictionary of alphabet prefabs. The keys are of type <see cref="System.Char"/>, and the values are of type <see cref="Letter"/>.
    /// The keys are upper case.
    /// WARNING: The letters 'W' and 'Z' do not exist in the game because they look like 'M' and 'N' (which do exist in the game).
    /// Thankfully, the planet names do not use either of 'W' or 'Z'.
    /// </summary>
    public static Dictionary<char, Letter> AlphabetPrefabs
    {
        get
        {
            if (alphabetPrefabs == null)
            {
                alphabetPrefabs = new Dictionary<char, Letter>();

                char c = 'A';
                while (c <= 'Z')
                {
                    Letter L = Resources.Load<Letter>("Alphabet/Letter" + c);
                    if (L != null)
                    {
                        if (alphabetPrefabs.ContainsKey(c) == false)
                        {
                            alphabetPrefabs.Add(c, L);
                        }
                    }

                    c++;
                }
            }

            return alphabetPrefabs;
        }
    }

    /// <summary>
    /// Unity's Awake method. Awake is called when the script instance is being loaded.
    /// In this case, it is used to initialize the components of the Letter.
    /// </summary>
    void Awake()
    {
        if (rbody == null)
        {
            rbody = gameObject.AddComponent<Rigidbody>();
        }
        rbody.useGravity = true;
        rbody.isKinematic = false;

        gameObject.tag = StaticVariables.TagPickup;
        if (col == null)
        {
            col = gameObject.AddComponent<BoxCollider>();
        }

        transform.localScale = new Vector3(scaleX, scaleY, scaleZ);
        transform.Rotate(Vector3.up, 180.0f, Space.World);

        character = char.ToUpper(character);
    }
}
