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

    /// <summary>
    /// Unity's Start method. Start is called before the first frame update
    /// </summary>
    void Start()
    {
        transform.localScale = new Vector3(scaleX, scaleY, scaleZ);
        transform.Rotate(Vector3.up, 180.0f, Space.World);
        rbody = gameObject.AddComponent<Rigidbody>();
        rbody.useGravity = true;
        rbody.isKinematic = false;
        gameObject.tag = StaticVariables.TagPickup;
        col = gameObject.AddComponent<BoxCollider>();
        character = char.ToUpper(character);
    }
}
