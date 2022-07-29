using UnityEngine;

/// <summary>
/// A class which designates the attached game object as an ExitDoorLogic.
/// This class contains the logic of opening the exit door, which in turn finishes the game.
/// - How does this class work?:
/// Create a game object which acts as the container of this script.
/// Since this is a Room Escape game, add a reference to the door which is considered the final exit door.
/// Then, add a reference to the <see cref="InGameUI"/> instance of your game.
/// When you want to finish the game and pop up the game over UI, use a <see cref="SceneTrigger"/> to call the <see cref="OnExitDoorTriggerEnter"/>.
/// This will activate the <see cref="InGameUI"/> instance which should be referenced in this class in the Inpector menu by the user of this script.
/// It will also call <see cref="Door.BeInteracted(UnityEngine.GameObject, object)"/> on the specified exitDoor.
/// That's it.
/// See <see cref="SceneTrigger"/> for more details on how to use it.
/// </summary>
public class ExitDoorLogic : MonoBehaviour
{
    public Door exitDoor;
    public InGameUI inGameUiInstance;

    /// <summary>
    /// This method is supposed to be subscribed to the a SceneTrigger's triggerEvent.
    /// That particular SceneTrigger acts as trigger area of this ExitDoorLogic.
    /// The trigger area object is not known by the ExitDoorLogic, but the trigger area knows which method to invoke, which happens to be this one.
    /// Due to Unity's usual uncooperativeness, the actual SceneTrigger script itself has to be passed in the Editor in order to let things work.
    /// Therefore, the argument has to exactly *that* SceneTrigger instance.
    /// </summary>
    /// <param name="col">Reference of the SceneTrigger instance which was triggered.</param>
    public void OnExitDoorTriggerEnter(SceneTrigger sceneTriggerItself)
    {
        if (sceneTriggerItself.otherCol.gameObject.name == StaticVariables.EscapeKeyName)
        {
            exitDoor.unlocked = true;
            exitDoor.BeInteracted(this.gameObject, null);
            InGameUI.isGameOver = true;
            inGameUiInstance.FinishGame();
            sceneTriggerItself.enabled = false;
        }
    }
}
