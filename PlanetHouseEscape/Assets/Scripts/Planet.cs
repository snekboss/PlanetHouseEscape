using UnityEngine;

/// <summary>
/// A class which designates the attached game object as a Planet.
/// </summary>
public class Planet : MonoBehaviour
{
    public string PlanetName;
    SphereCollider col;
    Rigidbody rbody;

    /// <summary>
    /// Unity's Start method. Start is called before the first frame update
    /// </summary>
    void Start()
    {
        rbody = gameObject.AddComponent<Rigidbody>();
        rbody.useGravity = true;
        rbody.isKinematic = false;
        col = gameObject.AddComponent<SphereCollider>();
        gameObject.tag = StaticVariables.TagPickup;
    }
}
