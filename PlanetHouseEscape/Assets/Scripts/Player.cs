using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// The game object to which this script is attached is deemed as the sole player of the puzzle game scene,
/// and this class handles the movement of said player.
/// </summary>
public class Player : MonoBehaviour
{
    #region Player movement related fields
    // Player movement related fields
    [Range(0f, 10f)]
    public float movementSpeed = 5.0f;
    [Range(0f, 10f)]
    public float jumpPower = 4.0f;
    [Range(0f, 10f)]
    public float groundDistance = 0.3f;
    Transform feet;

    Rigidbody playerRbody;
    Collider playerCollider;
    Vector3 playerMoveDir = Vector3.zero;
    public bool isGrounded;
    #endregion

    #region Player and pickup object rotation related fields
    // Player and pickup object rotation related fields
    Transform eyes;
    float playerYaw;
    float eyesPitch;
    float eyesPitchThreshold = 89.0f;
    float mouseX;
    float mouseY;
    #endregion

    #region Pickup related fields
    // Pickup related fields
    [Range(0.1f, 10.0f)]
    public float pickupRayLength = 3.5f;

    [Range(0.1f, 10.0f)]
    public float pickupZoomMax = 2.5f;
    [Range(0.1f, 10.0f)]
    public float pickupZoomMin = 1.5f;
    [Range(0.1f, 10.0f)]
    public float pickupZoomInitialValue = 2.0f;
    float pickupZoomCurValue;
    [Range(0.1f, 100.0f)]
    public float pickupZoomSpeed = 10.0f;
    [Range(0f, 100f)]
    public float pickupThrowPower = 10.0f;
    [Range(0f, 1f)]
    public float pickupLerpSpeed = 0.1f;
    [Range(0f, 3f)]
    public float pickupCooldown = 0.5f;
    float pickupCooldownTimer;

    GameObject pickupObject;
    Rigidbody pickupRbody;
    Collider pickupCollider;
    bool pickupObjectCoroutineIsRunning;
    bool dropObjectCoroutineIsRunning;

    public LayerMask defaultLayer;
    public Axes3D pickupRotAxis;
    RaycastHit hitInfo;
    bool raycastHitResult;
    #endregion

    #region Interaction related fields
    // Player inputs related fields
    bool isPressedLMB;
    bool isHoldingRMB;
    bool isPressedSpace;
    bool isPressedE;
    bool isHoldingX;
    bool isHoldingY;
    bool isHoldingZ;
    #endregion

    #region Tags related fields
    // Tags related fields
    string tagEscapeKey = "EscapeKey"; // The tag of the key game object which lets the player escape the house. TODO: Might need to refer to it by name.
    #endregion

    /// <summary>
    /// If the pickup object is null, then the associated rigidbody and collider references will also be set to null by this method.
    /// This acts as a countermeasure to potential bugs that might be caused by the use of coroutines for example.
    /// See <see cref="PickupObjectCoroutine"/> and <see cref="DropObjectCoroutine"/>.
    /// </summary>
    void DoCleanupIfPickupObjectIsNull()
    {
        if (pickupObject == null)
        {
            pickupRbody = null;
            pickupCollider = null;
        }
    }

    /// <summary>
    /// Casts a ray into the scene, and stores the result in raycastHitResult.
    /// The result can later be used for dealing with interactable or pickup objects.
    /// See <see cref="HandlePickingUp"/> and <see cref="HandleInteract"/>.
    /// </summary>
    void HandleRaycast()
    {
        Ray r = new Ray(eyes.position, eyes.forward);
        raycastHitResult = Physics.Raycast(r, out hitInfo, pickupRayLength, defaultLayer);
    }

    /// <summary>
    /// Check for inputs which involve interaction with the scene, and stores them in fields for easier readability later.
    /// <see cref="HandleMovement"/>, <see cref="HandlePickingUp"/>, <see cref="HandleControlPickupObject"/>,
    /// <see cref="HandleInteract"/>.
    /// </summary>
    void ReadInteractionInputs()
    {
        isPressedLMB = Input.GetMouseButtonDown((int)MouseButton.LeftMouse);
        isHoldingRMB = Input.GetMouseButton((int)MouseButton.RightMouse);
        isPressedSpace = Input.GetKeyDown(KeyCode.Space);
        isPressedE = Input.GetKeyDown(KeyCode.E);
        isHoldingX = Input.GetKey(KeyCode.X);
        isHoldingY = Input.GetKey(KeyCode.Y);
        isHoldingZ = Input.GetKey(KeyCode.Z);
    }

    /// <summary>
    /// Checks and handles inputs regarding player's camera movement.
    /// </summary>
    void HandleRotation()
    {
        mouseX = Input.GetAxis("Mouse X");
        mouseY = Input.GetAxis("Mouse Y");

        if (!isHoldingX && !isHoldingY && !isHoldingZ)
        {
            playerYaw += StaticVariables.mouseSensitivity * mouseX * Time.deltaTime;
            eyesPitch -= StaticVariables.mouseSensitivity * mouseY * Time.deltaTime; // Subtracting, because negative angle about X axis is up.

            eyesPitch = Mathf.Clamp(eyesPitch, -eyesPitchThreshold, eyesPitchThreshold);
        }

        // First, reset all rotations.
        transform.rotation = Quaternion.identity;
        eyes.rotation = Quaternion.identity;

        // Then, rotate with the new angles.
        transform.Rotate(Vector3.up, playerYaw);
        eyes.Rotate(Vector3.right, eyesPitch);
    }

    /// <summary>
    /// Checks and handles inputs regarding players movement in the scene.
    /// The input checking for jumps should have been done in a separate method.
    /// See <see cref="ReadInteractionInputs"/>.
    /// </summary>
    void HandleMovement()
    {
        isGrounded = Physics.CheckSphere(feet.position, groundDistance, defaultLayer, QueryTriggerInteraction.Ignore);

        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");
        playerMoveDir = Vector3.ClampMagnitude(new Vector3(moveX, 0, moveY), 1);

        playerMoveDir = transform.TransformDirection(playerMoveDir);

        if (isPressedSpace && isGrounded)
        {
            playerRbody.AddForce(Vector3.up * jumpPower, ForceMode.VelocityChange);
        }
    }

    /// <summary>
    /// Handles the picking up of objects with the use of previously checked input.
    /// See <see cref="ReadInteractionInputs"/>.
    /// </summary>
    void HandlePickingUp()
    {
        pickupCooldownTimer += Time.deltaTime;

        if (pickupObject != null || raycastHitResult == false || (pickupCooldownTimer <= pickupCooldown))
        {
            return;
        }

        if (isHoldingRMB)
        {
            if (raycastHitResult && hitInfo.transform.gameObject.tag != StaticVariables.TagPickup)
            {
                return;
            }

            if (!pickupObjectCoroutineIsRunning)
            {
                StartCoroutine("PickupObjectCoroutine", (object)hitInfo);
            }
        }
    }

    /// <summary>
    /// Handles the inputs for controlling a pickup object which is currently being held.
    /// The object can be dropped, thrown, or zoomed in/out; or rotated along the three axes.
    /// See See <see cref="ReadInteractionInputs"/>.
    /// </summary>
    void HandleControlPickupObject()
    {
        if (pickupObject == null)
        {
            return;
        }

        pickupZoomCurValue += Input.mouseScrollDelta.y * Time.deltaTime * pickupZoomSpeed;
        pickupZoomCurValue = Mathf.Clamp(pickupZoomCurValue, pickupZoomMin, pickupZoomMax);

        // To prevent the pickup object from accumulating "potential energy".
        pickupRbody.velocity = Vector3.zero;
        pickupRbody.angularVelocity = Vector3.zero;

        pickupRotAxis.SetActiveAxes(isHoldingX || isHoldingY || isHoldingZ);

        if (isHoldingX || isHoldingY || isHoldingZ)
        {
            pickupRotAxis.transform.position = pickupObject.transform.position;
            pickupRotAxis.transform.rotation = Quaternion.identity;
            pickupRotAxis.transform.Rotate(Vector3.up, playerYaw);
        }

        if (!isHoldingRMB)
        {
            if (!dropObjectCoroutineIsRunning)
            {
                StartCoroutine("DropObjectCoroutine", false);
            }
        }
        else if (isHoldingRMB && isPressedLMB)
        {
            if (!dropObjectCoroutineIsRunning)
            {
                StartCoroutine("DropObjectCoroutine", true);
            }
        }
        else if (isHoldingX)
        {
            float speed = StaticVariables.mouseSensitivity * mouseY * Time.deltaTime;
            pickupObject.transform.Rotate(eyes.transform.right, speed, Space.World);

            pickupRotAxis.SetChosenAxis(Axes3D.Axis.X);
        }
        else if (isHoldingY)
        {
            float speed = StaticVariables.mouseSensitivity * (-mouseX) * Time.deltaTime;
            pickupObject.transform.Rotate(eyes.transform.transform.up, speed, Space.World);

            pickupRotAxis.SetChosenAxis(Axes3D.Axis.Y);
        }
        else if (isHoldingZ)
        {
            float speed = StaticVariables.mouseSensitivity * (-mouseX) * Time.deltaTime;
            pickupObject.transform.Rotate(eyes.transform.transform.forward, speed, Space.World);

            pickupRotAxis.SetChosenAxis(Axes3D.Axis.Z);
        }

        if (Vector3.Distance(eyes.position, pickupRbody.position) > pickupRayLength && !dropObjectCoroutineIsRunning)
        {
            // Drop if the object ends up being too far from the player's reach.
            StartCoroutine("DropObjectCoroutine", false);
        }
    }

    /// <summary>
    /// Handles the inputs for interacting with the scene, such as opening doors.
    /// If the player is holding an object, then interactions with the scene won't work.
    /// For example, the player cannot open doors while holding an object.
    /// See <see cref="ReadInteractionInputs"/>.
    /// </summary>
    void HandleInteract()
    {
        if (/*pickupObject != null || */raycastHitResult == false)
        {
            return;
        }

        if (isPressedE)
        {
            IInteractable interactable = hitInfo.transform.gameObject.GetComponent<IInteractable>();
            interactable?.BeInteracted(this.gameObject, null);
        }
    }

    /// <summary>
    /// The player keeps track of the in-game time. TODO: Create a separate game object for this?... Seriosuly... ugh...
    /// </summary>
    void IncrementTime()
    {
        StaticVariables.time += Time.deltaTime * Time.timeScale;
    }

    /// <summary>
    /// A method which is meant to be invoked as a Unity coroutine, called by <see cref="MonoBehaviour.StartCoroutine(string, object)"/>.
    /// Handles variable assignments which are required for picking up an object.
    /// Picking up an object over several frames helps with the intended behavior of moving the object as the player moves.
    /// </summary>
    /// <param name="hitInfo">hitInfo is of type RaycastHit. It is used to learn information about the pickup object.</param>
    /// <returns>Returns some kind of yield return IEnumerator magic thing made by Unity.</returns>
    IEnumerator PickupObjectCoroutine(object hitInfo)
    {
        pickupObjectCoroutineIsRunning = true;

        RaycastHit hInfo = (RaycastHit)hitInfo;
        // Pick up the object.
        pickupObject = hInfo.transform.gameObject;
        pickupRbody = pickupObject.GetComponent<Rigidbody>();
        pickupCollider = pickupObject.GetComponent<Collider>();
        Physics.IgnoreCollision(pickupCollider, playerCollider, true);
        pickupZoomCurValue = pickupZoomInitialValue;

        // Disable physics for this frame. The object will go through walls for this frame.
        pickupRbody.isKinematic = true;
        pickupRbody.constraints = RigidbodyConstraints.FreezeAll;
        yield return new WaitForFixedUpdate(); // Just in case (like in DropObjectCoroutine)

        // Re-enable physics in the next frame. The object will not go through walls.
        pickupRbody.isKinematic = false;
        pickupRbody.useGravity = false;
        pickupRbody.constraints = RigidbodyConstraints.None;

        pickupObjectCoroutineIsRunning = false;
    }

    /// <summary>
    /// A method which is meant to be invoked as a Unity coroutine, called by <see cref="MonoBehaviour.StartCoroutine(string, object)"/>.
    /// Handles variable assignments which are required for dropping/throwin an object.
    /// Dropping/throwing an object over several frames helps with the intended behavior of moving the object as the player moves.
    /// </summary>
    /// <param name="hitInfo">isThrown is of type bool. If true, the pickup object will be thrown. If false, it will be dropped.</param>
    /// <returns>Returns some kind of yield return IEnumerator magic thing made by Unity.</returns>
    IEnumerator DropObjectCoroutine(object isThrown)
    {
        dropObjectCoroutineIsRunning = true;

        bool throwIt = (bool)isThrown;

        // Disable physics for this frame. This will "reset" the rigidbody.
        pickupRbody.constraints = RigidbodyConstraints.FreezeAll;
        pickupRbody.isKinematic = true;
        yield return new WaitForFixedUpdate(); // None of this works unless I WaitForFixedUpdate()

        // Re-enable physics in the next frame. The object will respond to player's movement.
        pickupRbody.constraints = RigidbodyConstraints.None;
        pickupRbody.isKinematic = false;

        pickupRbody.useGravity = true;
        Physics.IgnoreCollision(pickupCollider, playerCollider, false);

        if (throwIt)
        {
            pickupRbody.AddForce(eyes.forward * pickupThrowPower, ForceMode.VelocityChange);
            pickupCooldownTimer = 0;
        }

        pickupObject = null;
        pickupRbody = null;
        pickupCollider = null;

        dropObjectCoroutineIsRunning = false;
    }

    /// <summary>
    /// Checks whether the player is currently holding the key to escape the house in his hands.
    /// It will be used by the exit door of the house, which will immediately finish the game if the key collides with the door.
    /// </summary>
    /// <returns>Returns true if the player is currently holding the key to escape the house in his hands; false if not.</returns>
    public bool IsHoldingTheKey()
    {
        return pickupObject != null && pickupObject.tag == tagEscapeKey;
    }


    /// <summary>
    /// Unity's Start method. Start is called before the first frame update.
    /// </summary>
    void Start()
    {
        defaultLayer = 1 << LayerMask.NameToLayer("Default"); // Notice the bitshift. NameToLayer returns the bit index...

        playerRbody = gameObject.AddComponent<Rigidbody>();
        playerRbody.mass = 70.0f;
        playerRbody.constraints = RigidbodyConstraints.FreezeRotation;

        playerCollider = GetComponent<Collider>();

        eyes = transform.Find("Eyes");
        feet = transform.Find("Feet");

    }

    /// <summary>
    /// Unity's Update method. Update is called once per frame.
    /// </summary>
    void Update()
    {
        DoCleanupIfPickupObjectIsNull();
        HandleRaycast();
        ReadInteractionInputs();
        HandleRotation();
        HandleMovement();
        HandlePickingUp();
        HandleControlPickupObject();
        HandleInteract();

        IncrementTime();
    }

    /// <summary>
    /// Unity's FixedUpdate method.
    /// </summary>
    void FixedUpdate()
    {
        playerRbody.MovePosition(playerRbody.position + playerMoveDir * movementSpeed * Time.fixedDeltaTime);

        if (pickupObject != null)
        {
            Vector3 pickupDestinationPos = eyes.position + eyes.forward * pickupZoomCurValue;
            Vector3 pickupLerpedPos = Vector3.Lerp(pickupObject.transform.position, pickupDestinationPos, pickupLerpSpeed);
            pickupRbody.MovePosition(pickupLerpedPos);
        }
    }
}