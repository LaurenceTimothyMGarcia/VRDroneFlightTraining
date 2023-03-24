using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Autohand;

public class DroneController : MonoBehaviour
{

    // wind implementation
    public bool inWindZone = false;
    public GameObject windZone;
    private Rigidbody wz;
    public Vector3 directionofobj;

    [Header("Drone type")]
    public DroneType drone;

    // Controller required in scene to player can move controller
    // If NULL then player can move while controller moving
    [Header("Controller")]
    public GrabbableExtraEvents grabbableExtraEvents;

    //Values of input
    private Vector2 rightStick;
    private Vector2 leftStick;

    // Drones move on 4 different controls
    // Roll - Moving Left/Right Horizontally (Left and right arrow keys)
    //      - Done with horizontal movement on Right Stick
    // Pitch - Moving Forward/Backward Horizontally  (up and down arrow keys)
    //      - Done with vertical movement on Right Stick
    // Yaw  - Rotates Drone Left or Right (A and D)
    //      - Done with horizontal movement on Left Stick
    // Throttle - Controls power of propellors moving it vertically (W and S)
    //      - Done with vertical movement of Left Stick

    //Values of control based on input
    private float rollInput;
    private float pitchInput;
    private float throttleInput;
    private float yawInput;

    // Can make this private now
    [Header("Drone Control Speeds")]
    [Tooltip("Drone Movement Left/Right controlled using the Right Stick")]
    [SerializeField] private float rollSpeed;

    [Tooltip("Drone Movement Forward/Backward controlled using the Right Stick")]
    [SerializeField] private float pitchSpeed;

    [Tooltip("Drone Rotation Left/Right controlled using the Left Stick")]
    [SerializeField] private float yawSpeed;

    [Tooltip("Drone Movement Up/Down controlled using the Left Stick")]
    [SerializeField] private float throttlePower;

    // Variables of current status of drone
    private Vector3 currentDirection;
    private float currentRollSpeed;
    private float currentPitchSpeed;
    private float currentYawSpeed;
    private float currentThrottlePower;
    private Rigidbody rb;

    //varaibles for propellers on drone (set parameters subject to change)
    public float maxPropellerSpeed = 1000f;
    public float minPropellerSpeed = 0f;
    public float propellerSpeedIncrement = 10f;
    public float currentPropellerSpeed = 0f;
    public GameObject[] propellers;

    //updates the speed of the propeller based on input (Maybe can be moved to fixedupdate)
    void Update () {
        if (Input.GetKey(KeyCode.W)) {
            IncreasePropellerSpeed();
        }
        else if (Input.GetKey(KeyCode.S)) {
            DecreasePropellerSpeed();
        }
        UpdatePropellerSpeed();
    }

    //Functions which control the speed increase, decreasem and update time.
    void IncreasePropellerSpeed() {
        currentPropellerSpeed += propellerSpeedIncrement * Time.deltaTime;
        currentPropellerSpeed = Mathf.Clamp(currentPropellerSpeed, minPropellerSpeed, maxPropellerSpeed);
    }
    void DecreasePropellerSpeed() {
        currentPropellerSpeed -= propellerSpeedIncrement * Time.deltaTime;
        currentPropellerSpeed = Mathf.Clamp(currentPropellerSpeed, minPropellerSpeed, maxPropellerSpeed);
    }
    void UpdatePropellerSpeed() {
        foreach (GameObject propeller in propellers) {
            propeller.transform.Rotate(Vector3.up * currentPropellerSpeed * Time.deltaTime);
        }
    }

    // Start is called before the first frame update
    // Initialize the input drone controls
    void Start()
    {
        // Disables Drone controls on start (must be holding controller)
        InputManager.Instance.playerActions.DroneControls.Disable();

        // For testing purposes, if controller isnt in scene, its for keyboard testing
        if (grabbableExtraEvents == null)
        {
            InputManager.Instance.playerActions.DroneControls.Enable();
        }

        // Listener for when the controller is held by 2 hands or not
        // COMMENTOUT TESTNOVR
        /*grabbableExtraEvents.OnTwoHandedGrab.AddListener(ActivateDroneMovement);
        grabbableExtraEvents.OnTwoHandedRelease.AddListener(ActivatePlayerMovement);*/

        // Wind related variables
        wz = GetComponent<Rigidbody>(); //windarea 
        rb = GetComponent<Rigidbody>();

        // Pull Data from Scriptable Object
        Instantiate(drone.droneModel, this.transform);
        rb.mass = drone.mass;
        rollSpeed = drone.rollSpeed;
        pitchSpeed = drone.pitchSpeed;
        yawSpeed = drone.yawSpeed;
        throttlePower = drone.throttlePower;

        // Initializes input controls
        InputManager.Instance.playerActions.DroneControls.VerticalMovementRotation.performed += OnLeftStick;
        InputManager.Instance.playerActions.DroneControls.VerticalMovementRotation.canceled += OnLeftStick;
        InputManager.Instance.playerActions.DroneControls.HorizontalMovement.performed += OnRightStick;
        InputManager.Instance.playerActions.DroneControls.HorizontalMovement.canceled += OnRightStick;
    }


    // Any physics related drone movement goes here
    private void FixedUpdate()
    {
        // Drone Movement
        MoveDrone();
        SpeedControl();
        //rotation method (Foward movement needs to be replaced here with the correct command or the tilt method)
        //Yaw();
        //rb.rotation = rb.rotation * Quaternion.AngleAxis(yawSpeed * yawInput, Vector3.up);


        // Wind
        Vector3 directionofobj = new Vector3(Random.Range(0, 10), Random.Range(0, 10), Random.Range(0, 10));

        if (inWindZone)
        {
            int max = 5;
            int min = -5;
            Vector3 directionofwobj = new Vector3(Random.Range(min, max), Random.Range(min, max), Random.Range(min, max));
            wz.AddForce(directionofwobj * windZone.GetComponent<windarea>().strength);
            
        }
        
    }

    public void OnRightStick(InputAction.CallbackContext value)
    {
        if (value.performed)
        {
            rightStick = value.ReadValue<Vector2>();

            rollInput = rightStick.x;
            pitchInput = rightStick.y;
        }
        else if (value.canceled)
        {
            //Set velocity to zero
            rollInput = 0;
            pitchInput = 0;
        }
    }

    public void OnLeftStick(InputAction.CallbackContext value)
    {
        if (value.performed)
        {
            leftStick = value.ReadValue<Vector2>();

            yawInput = leftStick.x;
            throttleInput = leftStick.y;
        }
        else if (value.canceled)
        {
            //Set velocity to zero
            yawInput = 0;
            throttleInput = 0;
        }
    }

    // Drone movement detection if controller has been grabbed or not
    // If controller is grabbed with 2 hands then player stops moving and drone moves, vice versa
    // COMMENTOUT TESTNOVR
    /*void ActivateDroneMovement(Hand hand, Grabbable grab)
    {
        Debug.Log("Two-handed grab detected!");

        InputManager.Instance.playerActions.DefaultControls.Disable();
        InputManager.Instance.playerActions.DroneControls.Enable();
    }

    void ActivatePlayerMovement(Hand hand, Grabbable grab)
    {
        Debug.Log("Two-handed release detected!");

        InputManager.Instance.playerActions.DroneControls.Disable();
        InputManager.Instance.playerActions.DefaultControls.Enable();
    }*/

    // Wind related PHysics
    //if enter and exit windarea
    void OnTriggerEnter(Collider colli)
    {
        if (colli.gameObject.tag == "wind area")
        {
            windZone = colli.gameObject;
            inWindZone = true;
        }
    }

    void OnTriggerExit(Collider colli)
    {
        if (colli.gameObject.tag != "wind area")
        {
            inWindZone = false;
        }
    }


    // Reference Code from DroneMovement.cs
    // Variables for tilting
    private float tiltAmountFoward;
    private float tiltAmountHorizontal;
    private float tiltVelocityFoward;
    private float tiltVelocityHorizontal;

    private float targetAngle;
    private void MoveDrone()
    {
        Vector3 movement = new Vector3((rollInput * rollSpeed), (throttleInput * throttlePower), (pitchInput * pitchSpeed));

        //Tilt calculations
        tiltAmountFoward = Mathf.SmoothDamp(tiltAmountFoward, 20 * pitchInput, ref tiltVelocityFoward, 0.5f); //tilting method and speed which can be changed
        tiltAmountHorizontal = Mathf.SmoothDamp(tiltAmountHorizontal, 20 * -rollInput, ref tiltVelocityHorizontal, 0.5f); //tilting method and speed which can be changed

        //rotation of drone
        float droneRotY = yawInput * yawSpeed * Time.deltaTime;
        currentYawSpeed += droneRotY;
        //Applies tilt and the rotation of the drone
        rb.rotation = Quaternion.Euler(new Vector3(tiltAmountFoward, currentYawSpeed, tiltAmountHorizontal));

        //Debug.Log(rb.rotation);

        //Need to apply the current rotation of the drone to the direction of the movement, then we can get what we need
        movement = new Vector3(movement.x /*multiply drone rotation here*/, movement.y, movement.z /*multiply drone rotation here*/);
        rb.AddForce(movement, ForceMode.Force);
    }

    // Sets a max speed of the drone
    private void SpeedControl()
    {
        float limitRoll = rb.velocity.x;
        float limitThrottle = rb.velocity.y;
        float limitPitch = rb.velocity.z;

        Vector3 flatVel = new Vector3(rb.velocity.x, rb.velocity.y, rb.velocity.z);

        if (flatVel.magnitude > rollSpeed)
        {
            Vector3 limitVel = flatVel.normalized * rollSpeed;
            limitRoll = limitVel.x;
        }

        if (flatVel.magnitude > throttlePower)
        {
            Vector3 limitVel = flatVel.normalized * throttlePower;
            limitThrottle = limitVel.y;
        }

        if (flatVel.magnitude > pitchSpeed)
        {
            Vector3 limitVel = flatVel.normalized * pitchSpeed;
            limitPitch = limitVel.z;
        }

        // When player stops controlling drone
        if (rollInput == 0)
        {
            if (rb.velocity.x > 0.01)
            {
                rb.AddForce(new Vector3(-rollSpeed, 0, 0), ForceMode.Force);
            }
            else if (rb.velocity.x < -0.01)
            {
                rb.AddForce(new Vector3(rollSpeed, 0, 0), ForceMode.Force);
            }
        }

        if (pitchInput == 0)
        {
            if (rb.velocity.z > 0.01)
            {
                rb.AddForce(new Vector3(0, 0, -pitchSpeed), ForceMode.Force);
            }
            else if (rb.velocity.z < -0.01)
            {
                rb.AddForce(new Vector3(0, 0, pitchSpeed), ForceMode.Force);
            }
        }

        if (throttleInput == 0)
        {
            if (rb.velocity.y > 0.01)
            {
                rb.AddForce(new Vector3(0, -throttlePower, 0), ForceMode.Force);
            }
            else if (rb.velocity.y < -0.01)
            {
                rb.AddForce(new Vector3(0, throttlePower, 0), ForceMode.Force);
            }
        }
    }

    //Variables for desired rotation and rotation amount for keys
    // Use YawInput to determine if its rotating left or right
    //private float yawAmountByKeys = 2.5f; //Amount can be changed as needed
    private float yawYVelocity; //used for smooth damping instead of instant turn speeds.
    void Yaw() { //Rotation Method

            //calculate the current y rotation from the wanted one.
            currentYawSpeed = Mathf.SmoothDamp(currentYawSpeed, yawSpeed, ref yawYVelocity, .25f);
        
    }

    // Destroys event listener for controller
    // COMMENTOUT TESTNOVR
    /*public void OnDestroy()
    {
        grabbableExtraEvents.OnTwoHandedGrab.RemoveListener(ActivateDroneMovement);
        grabbableExtraEvents.OnTwoHandedRelease.RemoveListener(ActivatePlayerMovement);
    }*/
}
