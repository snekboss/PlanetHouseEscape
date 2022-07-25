using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A class which designates the attached game object as a Planet.
/// </summary>
public class Planet : MonoBehaviour
{
    public string PlanetName;
    SphereCollider col;
    Rigidbody rbody;

    static List<char> unusedLetters;
    static Dictionary<string, GameObject> planetPrefabs;
    public const float PlanetPrefabSpawnUniformScale = 0.5f;
    public static Dictionary<string, GameObject> PlanetPrefabs
    {
        get
        {
            if (planetPrefabs == null)
            {
                planetPrefabs = new Dictionary<string, GameObject>();
                UnityEngine.Object[] objs = Resources.LoadAll("Planets");
                for (int i = 0; i < objs.Length; i++)
                {
                    GameObject planetGO = objs[i] as GameObject;
                    Planet p = planetGO.GetComponent<Planet>();
                    p.name = p.name.ToUpper();
                    if (planetPrefabs.ContainsKey(p.name) == false)
                    {
                        planetPrefabs.Add(p.name, planetGO);
                    }
                }
            }

            return planetPrefabs;
        }
    }

    /// <summary>
    /// Loads all planet prefabs into memory, and finds out which letters are unused by the planets.
    /// The results are stored in <see cref="unusedLetters"/>.
    /// </summary>
    static void DetermineUnusedLetters()
    {
        unusedLetters = new List<char>();
        string concatNames = "";
        var prefabs = PlanetPrefabs; // forcing C# to call the getter.
        foreach (var kvp in prefabs)
        {
            string planetName = kvp.Key.ToUpper();
            concatNames += planetName;
        }

        char c = 'A';
        while (c <= 'Z')
        {
            if (concatNames.Contains(c) == false)
            {
                if (c == 'W' || c == 'Z')
                {
                    // Don't do anything.
                    // I don't have the model files for these. Sorry for the hack.
                }
                else
                {
                    unusedLetters.Add(c);
                }
            }

            c++;
        }
    }

    /// <summary>
    /// Gets a <see cref="System.Char"/> which was not used by the names of any of the planet prefabs.
    /// </summary>
    /// <returns>An uppercase character.</returns>
    public static char GetRandomUnusedLetterChar()
    {
        if (unusedLetters == null)
        {
            DetermineUnusedLetters();
        }

        return unusedLetters[UnityEngine.Random.Range(0, unusedLetters.Count - 1)];
    }

    /// <summary>
    /// Unity's Awake method. Awake is called when the script instance is being loaded.
    /// In this case, it is used to initialize some of the components of the game object to which it is associated.
    /// </summary>
    void Awake()
    {
        gameObject.tag = StaticVariables.TagPickup;
        gameObject.transform.localScale = new Vector3(PlanetPrefabSpawnUniformScale, PlanetPrefabSpawnUniformScale, PlanetPrefabSpawnUniformScale);
    }
}
