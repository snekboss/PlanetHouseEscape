using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A class which designates the attached object as a BookshelfPuzzle.
/// It is the main script that controls the overall state of the puzzle.
/// This class works in the following way:
/// There are 5 game objects which the user needs to configure in order for the script to work:
/// - 1) bookLever object of type Door: This is the secret book lever which unlocks and opens the secret door. 
/// It uses the Door script, because a secret book lever is animated like a door, so we're reusing code here.
/// Create a book lever object in the scene to your liking and attach a Door script to it so that it behaves like a secret door.
/// Then, add a reference to that object in this class.
/// - 2) bookLeverVisualPhosphorous object of type GameObject: This is supposed to act as a hint to the player.
/// For example, when the lights are turned off (see below), the secret book lever can start to glow in a certain color.
/// That glow is actually a separate game object that is turned visible once the lights are off (or whatever condition you want is met).
/// - 3) bookLeverVisualRegular object of type GameObject: This is supposed to be the regular visual of the book lever object in the scene.
/// This is what is shown on the scene when the lights are turned on (or whatever condition you want is not yet met).
/// - 4) secretDoor object of type Door: This is actually the door which is unlocked and opened once the player solves the puzzle.
/// The puzzle is solved by finding and interacting the bookLever.
/// - 5) roomLightSwitch object of type SceneButton: When the player solves the puzzle by turning off the lights,
/// we want the lights to be turned back on automatically. This is a reference to that light object.
/// If it is already turned on, then nothing will happen.
/// If it is turned off, then it will be turned on again, so that the player can see their surroundings.
/// 
/// - More information
/// The puzzle is actually solved when the bookLever enters the confines of a specific SceneTrigger object in the scene.
/// The specified SceneTrigger object then calls the OnBookLeverTriggerEnter method, and then this script can decide what to do.
/// In this case, it deems the puzzle solved.
/// The details as to how the <see cref="SceneTrigger"/> is used can be found in its own page.
/// </summary>
public class BookshelfPuzzle : MonoBehaviour
{
    public Door bookLever;
    public GameObject bookLeverVisualRegular;
    public GameObject bookLeverVisualPhosphorous;

    public Door secretDoor; // will be opened when the puzzle is solved.
    public SceneButton roomLightSwitch;

    bool isPuzzleComplete = false;


    /// <summary>
    /// This method is supposed to be subscribed to a FlipFlopButton's buttonEvent.
    /// The particular FlipFlopButton will act as the light switch of the room in which this puzzle is contained.
    /// The function handles the bookLever visual, which is supposed to be the hint of this puzzle.
    /// </summary>
    public void OnLightSwitchFlip()
    {
        if (isPuzzleComplete)
        {
            return;
        }

        FlipBookLeverVisual();
    }


    /// <summary>
    /// This method is supposed to be subscribed to a SceneTrigger's triggerEvent.
    /// That particular SceneTrigger acts as the trigger of this BookshelfPuzzle's bookLever.
    /// When the bookLever enters the trigger area of that particular SceneTrigger, the puzzle will be solved.
    /// The SceneTrigger object is not known by the BookshelfPuzzle, but the trigger knows which method to invoke, which happens to be this one.
    /// Due to Unity's usual uncooperativeness, the actual SceneTrigger script itself has to be passed in the Editor in order to let things work.
    /// Therefore, the argument has to exactly *that* SceneTrigger instance.
    /// </summary>
    /// <param name="col">Reference of the SceneTrigger instance which was triggered.</param>
    public void OnBookLeverTriggerEnter(SceneTrigger sceneTriggerItself)
    {
        if (isPuzzleComplete)
        {
            return;
        }

        Collider otherCol = sceneTriggerItself.otherCol;
        Door D = otherCol.GetComponent<Door>();
        
        if (D == null)
        {
            return;
        }

        if (D == bookLever)
        {
            isPuzzleComplete = true;
            secretDoor.unlocked = true;
            secretDoor.BeInteracted(this.gameObject, null);

            if (bookLeverVisualPhosphorous.activeSelf)
            {
                // if the lights are off, then turn them on.
                roomLightSwitch.BeInteracted(this.gameObject, null);
            }

            Destroy(sceneTriggerItself.gameObject);

            InitBookLevelVisuals();
        }
    }

    /// <summary>
    /// Sets the initial active states of the visuals of the book lever.
    /// Initially, the room's light will be lit, so the regular version will be active in the scene.
    /// </summary>
    void InitBookLevelVisuals()
    {
        bookLeverVisualRegular.SetActive(true);
        bookLeverVisualPhosphorous.SetActive(false);
    }

    /// <summary>
    /// Flips the active states of the bookLever visuals.
    /// Only one of them can be active in the scene at a time.
    /// </summary>
    void FlipBookLeverVisual()
    {
        bool regularState = bookLeverVisualRegular.activeSelf;
        bool phosphorousState = bookLeverVisualPhosphorous.activeSelf;
        bookLeverVisualRegular.SetActive(!regularState);
        bookLeverVisualPhosphorous.SetActive(!phosphorousState);
    }

    /// <summary>
    /// Unity's Awake Method.
    /// In this  case, it is used to set up the initial active states of the bookLever visuals.
    /// </summary>
    void Awake()
    {
        InitBookLevelVisuals();
    }
}
