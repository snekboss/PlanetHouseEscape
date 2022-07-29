using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A class which designates the attached object as a Planetarium.
/// The planetarium is the final puzzle where all the planets are gathered, and the exit key is dropped.
/// - How to use this script?:
/// Create a game object which is supposed to be your Planetarium.
/// It should have 2 different parts:
/// - 1) The visuals
/// - 2) The collider
/// -
/// - 1) The visuals is where the planet visuals should be put, so that they can be shown gradually as the player throws each planet into the collider.
/// - 2) The collider is about detecting which planets actually are thrown into the planetarium by the player.
/// - 
/// - In this script, the planetNames must match <see cref="Planet.PlanetName"/>, so be careful about that.
/// However, throughout the project, a convention of using uppercase names was used via <see cref="System.String.ToUpper"/>.
/// </summary>
public class Planetarium : MonoBehaviour
{
    public GameObject keyContainer;
    Rigidbody keyRbody;
    Collider keyCollider;

    public List<string> planetNames = new List<string>();
    public List<GameObject> planetVisuals = new List<GameObject>();

    Dictionary<string, GameObject> dict_planetName_planetGO = new Dictionary<string, GameObject>();

    /// <summary>
    /// Sets the pickup status of the exit key.
    /// Meaning, it enables/disables physics related parameters based on whether the key should be picked up.
    /// </summary>
    /// <param name="canBePickedUp">True if the key is meant to be picked up; false otherwise.</param>
    void SetExitKeyPickupStatus(bool canBePickedUp)
    {
        keyRbody.isKinematic = !canBePickedUp;
        keyRbody.useGravity = canBePickedUp;
        keyCollider.enabled = canBePickedUp;
    }

    /// <summary>
    /// This method is supposed to be subscribed to the a SceneTrigger's triggerEvent.
    /// That particular SceneTrigger acts as the trigger area of this Planetarium.
    /// The trigger area object is not known by the Planetarium, but the trigger areaknows which method to invoke, which happens to be this one.
    /// Due to Unity's usual uncooperativeness, the actual SceneTrigger script itself has to be passed in the Editor in order to let things work.
    /// Therefore, the argument has to exactly *that* SceneTrigger instance.
    /// </summary>
    /// <param name="col">Reference of the SceneTrigger instance which was triggered.</param>
    public void OnPlanetariumTriggerEnter(SceneTrigger sceneTriggerItself)
    {
        Collider other = sceneTriggerItself.otherCol;
        Planet planet = other.gameObject.GetComponent<Planet>();

        if (planet != null)
        {
            string planetName = planet.name;

            if (dict_planetName_planetGO.ContainsKey(planetName) == false)
            {
                return; // for REarth (red earth). It is a Planet object, but it's not meant to be counted for the Planetarium.
            }

            if (dict_planetName_planetGO[planetName].activeSelf == true)
            {
                return;
            }

            dict_planetName_planetGO[planetName].SetActive(true);
            Destroy(other.gameObject);

            bool allPlanetsHaveBeenFound = true;
            foreach (var kvp in dict_planetName_planetGO)
            {
                allPlanetsHaveBeenFound &= kvp.Value.activeSelf;
            }

            if (allPlanetsHaveBeenFound)
            {
                SetExitKeyPickupStatus(true);
                sceneTriggerItself.enabled = false;
            }
        }
    }

    /// <summary>
    /// Unity's Awake Method.
    /// In this  case, it is used to set up the initial state of the planetarium visuals.
    /// </summary>
    void Awake()
    {
        keyContainer.name = StaticVariables.EscapeKeyName;

        keyRbody = keyContainer.GetComponent<Rigidbody>();
        keyCollider = keyContainer.GetComponent<Collider>();

        SetExitKeyPickupStatus(false);

        for (int i = 0; i < planetVisuals.Count; i++)
        {
            planetVisuals[i].SetActive(false);
            dict_planetName_planetGO.Add(planetNames[i], planetVisuals[i]);
        }
    }
}
