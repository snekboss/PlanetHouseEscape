using UnityEngine;

/// <summary>
/// A class which designates the attached object as a MainMenuPlanet.
/// It is recommended to also attach <see cref="Planet"/> onto the same object.
/// However, instead, it is also enough to just add a <see cref="SphereCollider"/> and a <see cref="Rigidbody"/>.
/// This class is used for the animation of the planets in the Main Menu.
/// - How to use this script?:
/// The MainMenuPlanet objects will do some things when they collide with other objects in the scene.
/// Those "some things" are: Adding force to themselves in random directions with the specified velocities and angular velocities.
/// So, choose some values to your liking, and the planets will go off in random directions when they collide with things.
/// </summary>
public class MainMenuPlanet : MonoBehaviour
{
    [Range(0f, 10f)]
    public float planetVelocityMin = 1.0f;
    [Range(0f, 10f)]
    public float planetVelocityMax = 10.0f;

    [Range(0f, 360f)]
    public float planetAngularVelocityMin = 10.0f;
    [Range(0f, 360f)]
    public float planetAngularVelocityMax = 360.0f;

    Rigidbody rb;

    /// <summary>
    /// A method which assigns a random velocity to the rigidbody of the attached object.
    /// It is used for the planet animations in the Main Menu.
    /// </summary>
    void RandomizeVelocity()
    {
        Vector3 randomVelocityDir = UnityEngine.Random.insideUnitSphere;
        float randomVelocityMagnitude = UnityEngine.Random.Range(planetVelocityMin, planetVelocityMax);
        rb.velocity = randomVelocityDir * randomVelocityMagnitude;
    }

    /// <summary>
    /// A method which assigns a random angular velocity to the rigidbody of the attached object.
    /// It is used for the planet animations in the Main Menu.
    /// </summary>
    void RandomizeAngularVelocity()
    {
        Vector3 randomAngularVelocityDir = UnityEngine.Random.insideUnitSphere;
        float randomAngularVelocityMagnitude = UnityEngine.Random.Range(planetAngularVelocityMin, planetAngularVelocityMax);
        rb.angularVelocity = randomAngularVelocityDir * randomAngularVelocityMagnitude * Mathf.Deg2Rad;
    }

    /// <summary>
    /// Unity's OnCollisionEnter method. It is called when a collision occurs with another object.
    /// In this particular case, it is used to "re-animate" the planet as it hits something.
    /// </summary>
    /// <param name="collision"></param>
    void OnCollisionEnter(Collision collision)
    {
        if (rb == null)
        {
            return;
        }

        RandomizeVelocity();
        RandomizeAngularVelocity();
    }

    /// <summary>
    /// Unity's Start method. Start is called before the first frame update.
    /// </summary>
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = false;

        RandomizeVelocity();
        RandomizeAngularVelocity();
    }
}
