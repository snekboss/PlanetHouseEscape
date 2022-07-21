using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A class which designates the attached object as a LetterEater machine.
/// The machine eats letters. If the sequence of letters were correct, it spits out a planet as reward.
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
