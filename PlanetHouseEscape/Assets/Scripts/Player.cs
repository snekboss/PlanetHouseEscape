using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class Player : MonoBehaviour
{
    public LayerMask defaultLayer;

    // Movement related
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

    // Rotation related
    Transform eyes;
    float playerYaw;
    float eyesPitch;
    float eyesPitchThreshold = 89.0f;

    // Pickup related
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

    bool pickupWillBeDropped = false;
    bool pickupWillBeThrown = false;

    // Inputs related
    bool isPressedLMB;
    bool isHoldingRMB;
    bool isPressedSpace;

    // Interact related
    bool isPressedE;

    RaycastHit hitInfo;
    bool raycastHitResult;

    string tagPickup = "Pickup";
    string tagInteractable = "Interactable";
    string tagEscapeKey = "EscapeKey";

    void IfPickupObjectIsNull()
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
    /// </summary>
    void HandleRaycast()
    {
        Ray r = new Ray(eyes.position, eyes.forward);
        raycastHitResult = Physics.Raycast(r, out hitInfo, pickupRayLength, defaultLayer);
    }

    /// <summary>
    /// Check for inputs which involve interaction with the scene, and stores them in fields for easier readability later.
    /// </summary>
    void ReadInteractionInputs()
    {
        isPressedLMB = Input.GetMouseButtonDown((int)MouseButton.LeftMouse);
        isHoldingRMB = Input.GetMouseButton((int)MouseButton.RightMouse);
        isPressedSpace = Input.GetKeyDown(KeyCode.Space);
        isPressedE = Input.GetKeyDown(KeyCode.E);
    }

    /// <summary>
    /// Checks and handles inputs regarding player's camera movement.
    /// </summary>
    void HandleRotation()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        playerYaw += StaticVariables.mouseSensitivity * mouseX * Time.deltaTime;
        eyesPitch -= StaticVariables.mouseSensitivity * mouseY * Time.deltaTime; // Subtracting, because negative angle about X axis is up.

        eyesPitch = Mathf.Clamp(eyesPitch, -eyesPitchThreshold, eyesPitchThreshold);

        // First, reset all rotations.
        transform.rotation = Quaternion.identity;
        eyes.rotation = Quaternion.identity;

        // Then, rotate with the new angles.
        transform.Rotate(Vector3.up, playerYaw);
        eyes.Rotate(Vector3.right, eyesPitch);
    }

    /// <summary>
    /// Checks and handles inputs regarding players movement in the scene.
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

    void HandlePickingUp()
    {
        pickupCooldownTimer += Time.deltaTime;

        if (pickupObject != null || raycastHitResult == false || (pickupCooldownTimer <= pickupCooldown))
        {
            return;
        }

        if (isHoldingRMB)
        {
            if (raycastHitResult && hitInfo.transform.gameObject.tag != tagPickup)
            {
                return;
            }

            RaycastHit hInfo = (RaycastHit)hitInfo;

            // Pick up the object.
            pickupObject = hInfo.transform.gameObject;
            pickupRbody = pickupObject.GetComponent<Rigidbody>();
            pickupCollider = pickupObject.GetComponent<Collider>();

            Physics.IgnoreCollision(pickupCollider, playerCollider, true);
            pickupRbody.useGravity = false;
            pickupRbody.constraints = RigidbodyConstraints.FreezeRotation;
            pickupZoomCurValue = pickupZoomInitialValue;
        }
    }

    /// <summary>
    /// Enables gravity and response to collisions for the pickup object.
    /// After calling this method, it is strongly recommended to remove the pickup object from the player's control
    /// by setting the necessary variables to null.
    /// </summary>
    void EnablePhysicsForPickupObject()
    {
        Physics.IgnoreCollision(pickupCollider, playerCollider, false);
        pickupRbody.constraints = RigidbodyConstraints.None;
        pickupRbody.useGravity = true;
    }

    /// <summary>
    /// Set the necessary variables to null and thus remove the player's control over the pickup object.
    /// Before calling this method, it is strongly recommended to enable physics for the pickup object.
    /// </summary>
    void DetachPickupObject()
    {
        pickupObject = null;
        pickupRbody = null;
        pickupCollider = null;
    }

    void ControlPickupObject()
    {
        if (pickupObject == null || pickupWillBeThrown == true || pickupWillBeDropped == true)
        {
            return;
        }

        pickupRbody.velocity = Vector3.zero;
        pickupRbody.angularVelocity = Vector3.zero;

        pickupZoomCurValue += Input.mouseScrollDelta.y * Time.deltaTime * pickupZoomSpeed;
        pickupZoomCurValue = Mathf.Clamp(pickupZoomCurValue, pickupZoomMin, pickupZoomMax);

        if (!isHoldingRMB)
        {
            EnablePhysicsForPickupObject();
            pickupWillBeDropped = true;
            //DropPickupObject();
        }
        else if (isHoldingRMB && isPressedLMB)
        {
            pickupWillBeThrown = true;
            EnablePhysicsForPickupObject();
            //ThrowPickupObject();
        }
        else if (Vector3.Distance(eyes.position, pickupRbody.position) > pickupRayLength)
        {
            pickupWillBeDropped = true;
            EnablePhysicsForPickupObject();
            //DropPickupObject();
        }
    }

    /// <summary>
    /// Checks and handles inputs for interacting with the scene, such as opening doors.
    /// If the player is holding an object, then interactions with the scene won't work.
    /// For example, the player cannot open doors while holding an object.
    /// </summary>
    void HandleInteract()
    {
        if (pickupObject != null || raycastHitResult == false)
        {
            return;
        }

        if (isPressedE)
        {
            if (hitInfo.transform.gameObject.tag != tagInteractable)
            {
                return;
            }

            IInteractable interactable = hitInfo.transform.gameObject.GetComponent<IInteractable>();
            interactable.BeInteracted(this.gameObject, null);
        }
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
    /// The player keeps track of the in-game time. TODO: Create a separate game object for this?... Seriosuly... ugh...
    /// </summary>
    void IncrementTime()
    {
        StaticVariables.time += Time.deltaTime * Time.timeScale;
    }

    /// <summary>
    /// Unity's Start method. Start is called before the first frame update.
    /// </summary>
    void Start()
    {
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
        IfPickupObjectIsNull();
        HandleRaycast();
        ReadInteractionInputs();
        HandleRotation();
        HandleMovement();
        HandlePickingUp();
        ControlPickupObject();
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

            if (pickupWillBeThrown)
            {
                pickupRbody.AddForce(eyes.forward * pickupThrowPower, ForceMode.VelocityChange);
                DetachPickupObject();
                pickupCooldownTimer = 0; // So that the player doesn't grab the object mid-air after being thrown.

            }
            else if (pickupWillBeDropped)
            {
                DetachPickupObject();
            }

            pickupWillBeDropped = false;
            pickupWillBeThrown = false;
        }
    }
}