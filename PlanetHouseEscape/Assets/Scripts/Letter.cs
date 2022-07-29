using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A class which designates the attached game object as a Letter.
/// Used for the LetterEater puzzle.
/// - How to use this script?:
/// Well, Letters are pickup objects, so this script only contains data that describes a Letter object.
/// The only code it contains is to load the Letter prefabs into the memory,
/// and reference it via <see cref="AlphabetPrefabs"/>, which is a singleton field.
/// The script automatically adds a BoxCollider and a Rigidbody if the user of the script does not add those things in the Inspector menu.
/// However, if you wish to add a custom weight to the rigidbody, then you're going to need to add your own rigidbody in the inspector menu,
/// and set the weight yourself.
/// The only thing this script does in terms of physics is to force it to use gravity, and not let it be kinematic.
/// Finally, the script sets the scale of the object in the scene using the constants scaleX, scaleY and scaleZ.
/// You can change those constant values to your liking, but the current values seem to work well.
/// </summary>
public class Letter : MonoBehaviour
{
    public char character;

    BoxCollider col;
    Rigidbody rbody;

    const float scaleX = 0.125f;
    const float scaleY = 0.0125f;
    const float scaleZ = 0.125f;


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
