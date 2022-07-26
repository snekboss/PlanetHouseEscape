using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A class which designates the attached object as a PoolOfPlanets.
/// PoolOfPlanets spawns many decoy planets, and a correct planet.
/// The puzzle is to find the correct one.
/// This class contains the settings for this puzzle.
/// Since the code uses uniform scale, it is strongly recommended to use unit spheres as planets.
/// In other words, leave their scales at Vector3.one.
/// </summary>
public class PoolOfPlanets : MonoBehaviour
{
    // Use unit spheres as planets. Leave their scales at Vector3.one.
    public Planet decoyPrefab;
    public float decoyScale;
    public Planet uniquePrefab;
    public float uniqueScale;

    [Range(0.1f, 10f)]
    public float decoySpawnWidth;
    [Range(0.1f, 10f)]
    public float decoySpawnHeight;
    [Range(0.1f, 10f)]
    public float decoySpawnDepth;

    [Range(0.001f, 1f)]
    public float decoySpawnDelimiter;

    [Range(0f, 1f)]
    public float decoySpawnRandX;
    [Range(0f, 1f)]
    public float decoySpawnRandY;
    [Range(0f, 1f)]
    public float decoySpawnRandZ;

    [Range(0f, 1f)]
    public float uniqueSpawnRadius;

    public Color gizmoColorDecoy;
    public Color gizmoColorDecoyRandom;
    public Color gizmoColorUnique;

    // Below are for the editor only.
    [SerializeField]
    int decoyCountRight;
    [SerializeField]
    int decoyCountUp;
    [SerializeField]
    int decoyCountForward;
    [SerializeField]
    int decoyCountTotal;

    List<Planet> listDecoys = new List<Planet>();
    Planet uniquePlanet;

    float decoyRadius { get { return decoyScale / 2.0f; } }

    /// <summary>
    /// Returns the number of decoy planets per dimension based on the dimensions of the spawn area.
    /// </summary>
    /// <param name="decoyWidth">Width of the spawn box.</param>
    /// <param name="decoyHeight">Height of the spawn box.</param>
    /// <param name="decoyDepth">Depth of the spawn box.</param>
    /// <returns></returns>
    Vector3Int GetPlanetCountsPerDimension(float decoyWidth, float decoyHeight, float decoyDepth)
    {
        Vector3Int counts = new Vector3Int();
        counts.x = Convert.ToInt32(decoySpawnWidth / (decoyWidth + decoySpawnDelimiter));
        counts.y = Convert.ToInt32(decoySpawnHeight / (decoyHeight + decoySpawnDelimiter));
        counts.z = Convert.ToInt32(decoySpawnDepth / (decoyDepth + decoySpawnDelimiter));
        return counts;
    }

    /// <summary>
    /// Randomly spawns decoy planets based on the chosen dimensions of the spawn area.
    /// </summary>
    void SpawnDecoyPlanets()
    {
        //vertical (y)
        //  longitudinal (z)
        //      lateral (x)

        float decoyWidth = decoyScale;
        float decoyHeight = decoyScale;
        float decoyDepth = decoyScale;

        Vector3Int counts = GetPlanetCountsPerDimension(decoyWidth, decoyHeight, decoyDepth);
        decoyCountRight = counts.x;
        decoyCountUp = counts.y;
        decoyCountForward = counts.z;
        decoyCountTotal = decoyCountRight * decoyCountUp * decoyCountForward;

        float spawnY = decoyRadius;
        for (int y = 0; y < decoyCountUp; y++)
        {
            float spawnZ = decoyRadius;
            for (int z = 0; z < decoyCountForward; z++)
            {
                float spawnX = decoyRadius;
                for (int x = 0; x < decoyCountRight; x++)
                {
                    Planet decoy = Instantiate(decoyPrefab);
                    decoy.gameObject.name = decoy.PlanetName;
                    decoy.transform.parent = this.transform;
                    decoy.transform.localPosition = new Vector3(spawnX, spawnY, spawnZ);
                    decoy.transform.localScale = Vector3.one * decoyScale;
                    listDecoys.Add(decoy);

                    spawnX += (decoyWidth + decoySpawnDelimiter);
                }

                spawnZ += (decoyDepth + decoySpawnDelimiter);
            }

            spawnY += (decoyHeight + decoySpawnDelimiter);
        }

        // Add randomization values.
        for (int i = 0; i < listDecoys.Count; i++)
        {
            float randX = UnityEngine.Random.Range(-decoySpawnRandX, decoySpawnRandX);
            float randY = UnityEngine.Random.Range(-decoySpawnRandY, decoySpawnRandY);
            float randZ = UnityEngine.Random.Range(-decoySpawnRandZ, decoySpawnRandZ);
            listDecoys[i].transform.position += new Vector3(randX, randY, randZ);
        }
    }

    /// <summary>
    /// Spawns the unique planet via <see cref="uniquePrefab"/>.
    /// </summary>
    void SpawnUniquePlanet()
    {
        Vector3 spawnCubeDiagonal = new Vector3(decoySpawnWidth, decoySpawnHeight, decoySpawnDepth);

        float randX = UnityEngine.Random.Range(-uniqueSpawnRadius, uniqueSpawnRadius);
        float randY = UnityEngine.Random.Range(-uniqueSpawnRadius, uniqueSpawnRadius);
        float randZ = UnityEngine.Random.Range(-uniqueSpawnRadius, uniqueSpawnRadius);
        Vector3 randomizationOffset = new Vector3(randX, randY, randZ);
        uniquePlanet = Instantiate(uniquePrefab);
        uniquePlanet.gameObject.name = uniquePlanet.PlanetName;
        uniquePlanet.transform.parent = this.transform;
        uniquePlanet.transform.localPosition = (spawnCubeDiagonal / 2) + randomizationOffset;
        uniquePlanet.transform.localScale = Vector3.one * uniqueScale;
    }

    /// <summary>
    /// Unity's OnDrawGizmos method. It is used to draw gizmos in the scene view.
    /// This method is automatically called by Unity, and it is strictly for editor purposes.
    /// In this case, it is used to visualiz the decoy spawn area.
    /// </summary>
    void OnDrawGizmos()
    {
        float randomOffsetCubeWidth = decoySpawnWidth + 2 * decoySpawnRandX;
        float randomOffsetCubeHeight = decoySpawnHeight + 2 * decoySpawnRandY;
        float randomOffsetCubeDepth = decoySpawnDepth + 2 * decoySpawnRandZ;

        float uniqueSpawnWidth = (decoySpawnWidth + decoySpawnRandX) * uniqueSpawnRadius;
        float uniqueSpawnHeight = (decoySpawnHeight + decoySpawnRandY) * uniqueSpawnRadius;
        float uniqueSpawnDepth = (decoySpawnDepth + decoySpawnRandZ) * uniqueSpawnRadius;

        Vector3 uniqueCubeDims = new Vector3(uniqueSpawnWidth, uniqueSpawnHeight, uniqueSpawnDepth);
        Vector3 randomOffsetCubeDims = new Vector3(randomOffsetCubeWidth, randomOffsetCubeHeight, randomOffsetCubeDepth);
        Vector3 spawnCubeDims = new Vector3(decoySpawnWidth, decoySpawnHeight, decoySpawnDepth);

        Vector3 gizmoCubeOrigin = spawnCubeDims / 2; // Because I make the planets spawn from the center of the cube. So I need to offset the gizmo to show it.
        Gizmos.matrix = this.transform.localToWorldMatrix; // Draw the gizmo WRT this object's transform.

        // Draw cube for the unique planet.
        Gizmos.color = gizmoColorUnique;
        Gizmos.DrawCube(gizmoCubeOrigin, uniqueCubeDims);

        // Draw cube with randomized values included.
        Gizmos.color = gizmoColorDecoyRandom;
        Gizmos.DrawCube(gizmoCubeOrigin, randomOffsetCubeDims);

        // Draw the main spawn cube.
        Gizmos.color = gizmoColorDecoy;
        Gizmos.DrawCube(gizmoCubeOrigin, spawnCubeDims);
    }

    /// <summary>
    /// Unity's Start Method. Start is called before the first frame update.
    /// In this case, this is where the entire logic of this puzzle is run.
    /// </summary>
    void Start()
    {
        SpawnDecoyPlanets();
        SpawnUniquePlanet();
    }
}
