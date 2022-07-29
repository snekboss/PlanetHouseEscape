using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A class which designates the attached object as a LetterEater machine.
/// The machine eats letters. If the sequence of letters were correct, it spits out a planet as reward.
/// - How does this script work?:
/// The letter eater is a puzzle machine which eats <see cref="Letter"/>s and spits out a <see cref="Planet"/> as a reward.
/// - 1) The password of type string: When the letters which the machine has eaten match the string written in as the password,
/// then reward will be spit out. The sequence of the eaten letters is important.
/// According to what you write as the password, those exact letters will be spawned by the machine when the game starts.
/// - 2) numExtraLetters of type int: The number of letters which should be spawned that are different from the letters specified in the password.
/// These letters are meant to act as a "challenge" to the player.
/// - 3) transSpawnReward of type Transform: Create a game object in the scene, and make a reference to that game object in this script.
/// It acts as a convenient way to specify the spawn point of the reward of the planet object.
/// The planet will be spawned at the same position as this transform, and it will be "thrown" in the forward direction relative to transSpawnReward.
/// The throw power can be specified via rewardThrowForce.
/// - 4) rewardPlanetPrefab of type Planet: This is the PREFAB (not the game object in the scene) of the planet with which you wish to reward the player.
/// So, you need to add the reference from your project directory.
/// - 5) rewardThrowForce of type float: The strength of the force when the reward planet is spit out.
/// - 6) transSpawnLetterInitial of type Transform: Create a game object in the scene, and make a reference to that game object in this script.
/// It acts as a convenient way to specify the INITIAL spawn point of the letter objects when the game starts.
/// - 7) letterSpawnRadius of type float: Using the position of transSpawnLetterInitial, the radius is used to determine the range of the spawned letters.
/// The letters are spawned in the correct order, but at random locations, and this acts as a radius to determine how far the letters can spawn.
/// Note that the radius does not count the Y axis. It only considers the X and Z axes, so it actually acts as a 2D square.
/// - 8) letterSpawnPeriod of type float: It's just the time waited between each letter spawned.
/// It is useful because it prevents the letters from being spawned at the same time (which could cause some physical explosions in the scene).
/// - 9) transSpawnLetterRefund of type Transform: Create a game object in the scene, and make a refrence to that game object in this script.
/// It acts as a convenient way to specify the spawn position when the player decides to refund the letters.
/// Ihe forward direction of this transform object is also used to throw the letters in that direction.
/// - 10) letterThrowForce of type float: The strength of the force when the letters are refunded.
/// -
/// - More information:
/// - The "mouth" of the LetterEater is not known by this script.
/// For the sake of modularity, you're supposed to use a <see cref="SceneTrigger"/> for that purpose.
/// The SceneTrigger which acts as the mouth will have to call <see cref="OnMouthTriggerEnter"/>, which actually performs the "eating".
/// Check out the documentation page of <see cref="SceneTrigger"/>for more details on how to use it.
/// - The "refund button" is not known by this script.
/// For the sake of modularity, you're supposed to use something else which calls <see cref="RefundLetters"/> for that.
/// For example, a <see cref="PressableButton"/> can be used for this purpose. Check out its documentation page on how to use it.
/// </summary>
public class LetterEater : MonoBehaviour
{
    // Password related
    public string password;
    public string eatenLetters = ""; // must match the password to get the reward
    public int numExtraLetters; // extra letters to confuse the player

    // Reward related
    public Transform transSpawnReward; // Spawn point of the reward
    public Planet rewardPlanetPrefab;
    [Range(0.0f, 100.0f)]
    public float rewardThrowForce; // throw force for the reward

    // Letter related
    public Transform transSpawnLetterInitial; // Initial spawn point for the letters
    [Range(0.0f, 100.0f)]
    public float letterSpawnRadius; // Spawn the letters randomly within this area.
    [Range(0.0f, 10.0f)]
    public float letterSpawnPeriod; // in seconds

    public Transform transSpawnLetterRefund; // Spawn point for refunding letters
    [Range(0.0f, 100.0f)]
    public float letterThrowForce; // throw force for refunding letters

    bool isInitializing;
    bool isRefunding;
    bool isPuzzleSolved = false;

    /// <summary>
    /// A method which is meant to be invoked as a Unity coroutine, called by <see cref="MonoBehaviour.StartCoroutine(string, object)"/>.
    /// Handles the instantiation of Letter objects for the LetterEater puzzle.
    /// To avoid collisions, some time is waited between each letter spawn, hence the use of a coroutine.
    /// It is strongly recommended to start this coroutine ONCE via <see cref="LetterEater.Start"/>.
    /// </summary>
    /// <returns>Returns some kind of yield return IEnumerator magic thing made by Unity.</returns>
    IEnumerator InitializePasswordLetters()
    {
        isInitializing = true;
        int length = password.Length + numExtraLetters;
        for (int i = 0; i < length; i++)
        {
            Letter L = null;

            // Spawn legit letter.
            if (i < password.Length)
            {
                L = Instantiate<Letter>(Letter.AlphabetPrefabs[password[i]]);
            }
            else
            {
                // Turns out we're spawning extra characters.
                char c = Planet.GetRandomUnusedLetterChar();
                L = Instantiate<Letter>(Letter.AlphabetPrefabs[c]);
            }

            L.transform.parent = null;

            float randX = UnityEngine.Random.Range(-letterSpawnRadius, letterSpawnRadius);
            float randZ = UnityEngine.Random.Range(-letterSpawnRadius, letterSpawnRadius);
            Vector3 randDir = new Vector3(randX, 0, randZ);
            L.transform.position = transSpawnLetterInitial.position + randDir;

            yield return new WaitForSeconds(letterSpawnPeriod);
        }
        isInitializing = false;
    }

    /// <summary>
    /// A method which is meant to be invoked as a Unity coroutine, called by <see cref="MonoBehaviour.StartCoroutine(string, object)"/>.
    /// Handles the refunding of Letter objects for the LetterEater puzzle.
    /// To avoid collisions, some time is waited between each letter spawn, hence the use of a coroutine.
    /// It is strongly recommended to start this coroutine via <see cref="LetterEater.RefundLetters"/>.
    /// </summary>
    /// <returns>Returns some kind of yield return IEnumerator magic thing made by Unity.</returns>
    IEnumerator RefundEatenLetters()
    {
        isRefunding = true;
        for (int i = 0; i < eatenLetters.Length; i++)
        {
            Letter L = Instantiate<Letter>(Letter.AlphabetPrefabs[eatenLetters[i]]);
            L.transform.parent = null;
            L.transform.position = transSpawnLetterRefund.position;
            L.transform.rotation = transSpawnLetterRefund.rotation;

            Rigidbody letterRbody = L.GetComponent<Rigidbody>();
            letterRbody.AddForce(transSpawnLetterRefund.forward * letterThrowForce, ForceMode.VelocityChange);
            yield return new WaitForSeconds(letterSpawnPeriod);
        }
        eatenLetters = "";
        isRefunding = false;
    }

    /// <summary>
    /// Refunds the letters which are currently in the "stomach" of the LetterEater.
    /// The refund request will be ignored if the puzzle is solved;
    /// or if the LetterEater is initializing or already in the process of refunding.
    /// The actual refunding happens via <see cref="RefundEatenLetters"/>.
    /// </summary>
    public void RefundLetters()
    {
        if (eatenLetters == "" || isInitializing || isRefunding || isPuzzleSolved)
        {
            return;
        }

        StartCoroutine("RefundEatenLetters");
    }

    /// <summary>
    /// This method is supposed to be subscribed to the a SceneTrigger's triggerEvent.
    /// That particular SceneTrigger acts as the "mouth" of this LetterEater.
    /// The mouth object is not known by the LetterEater, but the mouth knows which method to invoke, which happens to be this one.
    /// Due to Unity's usual uncooperativeness, the actual SceneTrigger script itself has to be passed in the Editor in order to let things work.
    /// Therefore, the argument has to exactly *that* SceneTrigger instance.
    /// </summary>
    /// <param name="col">Reference of the SceneTrigger instance which was triggered.</param>
    public void OnMouthTriggerEnter(SceneTrigger sceneTriggerItself)
    {
        if (isInitializing || isRefunding || isPuzzleSolved)
        {
            return;
        }
        Collider otherCol = sceneTriggerItself.otherCol;
        Letter L = otherCol.GetComponent<Letter>();
        if (L == null)
        {
            return;
        }

        eatenLetters += char.ToUpper(L.character);
        Destroy(L.gameObject);

        if (eatenLetters == password)
        {
            isPuzzleSolved = true;

            Planet reward = Instantiate(rewardPlanetPrefab);
            reward.gameObject.name = reward.PlanetName;
            reward.transform.parent = null;
            reward.transform.position = transSpawnReward.position;
            reward.transform.rotation = transSpawnReward.rotation;

            Rigidbody rewardRbody = reward.GetComponent<Rigidbody>();
            rewardRbody.AddForce(transSpawnReward.forward * rewardThrowForce, ForceMode.VelocityChange);
        }
    }

    /// <summary>
    /// Unity's Start method. Start is called before the first frame update.
    /// In this case, it was used to spawn the letters for the puzzle.
    /// </summary>
    void Start()
    {
        password = password.ToUpper();
        StartCoroutine("InitializePasswordLetters");
    }
}
