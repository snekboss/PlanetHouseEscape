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
    bool pickupObjectCoroutineIsRunning;
    bool dropObjectCoroutineIsRunning;

    // Inputs related
    bool isPressedLMB;
    bool isHoldingRMB;
    bool isPressedSpace;

    // Interact related
    bool isHoldingE;
    bool isPressedE;

    RaycastHit hitInfo;
    bool raycastHitResult;

    string tagPickup = "Pickup";
    string tagInteractable = "Interactable";

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

            if (!pickupObjectCoroutineIsRunning)
            {
                StartCoroutine("PickupObjectCoroutine", (object)hitInfo);
            }
        }
    }

    void ControlPickupObject()
    {
        if (pickupObject == null)
        {
            return;
        }

        pickupZoomCurValue += Input.mouseScrollDelta.y * Time.deltaTime * pickupZoomSpeed;
        pickupZoomCurValue = Mathf.Clamp(pickupZoomCurValue, pickupZoomMin, pickupZoomMax);

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

        if (Vector3.Distance(eyes.position, pickupRbody.position) > pickupRayLength && !dropObjectCoroutineIsRunning)
        {
            StartCoroutine("DropObjectCoroutine", false);
        }
    }

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


    public bool IsHoldingTheKey()
    {
        return pickupObject != null && pickupObject.name == "KeyContainer";
    }

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
        }
    }
}