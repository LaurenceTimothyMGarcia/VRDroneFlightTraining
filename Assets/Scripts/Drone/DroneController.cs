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
    Rigidbody wz;
    //[SerializeField] private float windStrength = 10f;

    //float windStrength;
    //RaycastHit hit;
    //Collider[] hitColliders;
    //public Vector3 windDirection = new Vector3(0, 0, -1);
    // public Vector3 directionofobj;

    //drone height
    public float height;
    RaycastHit hit;


    [Header("Drone type")]
    public DroneType drone;

    // Controller required in scene to player can move controller
    // If NULL then player can move while controller moving
    [Header("Controller")]
    public GrabbableExtraEvents grabbableExtraEvents;

    [Header("Orientation")]
    public Transform direction;

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
    private float currentThrottlePower = 0f; //added initilization
    private Rigidbody rb;
    private GameObject droneModel;

    //varaibles for propellers on drone (set parameters subject to change)
    public float maxThrottleSpeed = 1000f;
    public float minThrottleSpeed = 0f;
    public float throttleSpeedIncrement = 10f;
    public GameObject[] propellers; //creates an empty array variable for the propellers

    private AudioSource droneSound;//variable for drone sound

    public void Initialize(GameObject drone) { //creates the function for propellers array to be filled with the different propellers onces the game starts.
        propellers = new GameObject[4];
        propellers[0] = drone.transform.Find("NorthWestProp").gameObject;
        propellers[1] = drone.transform.Find("NorthEastProp").gameObject;
        propellers[2] = drone.transform.Find("SouthWestProp").gameObject;
        propellers[3] = drone.transform.Find("SouthWestProp").gameObject;
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

        DroneController droneController = new DroneController();//creates a new instance of the DroneController class and assigns it to a variable
        //droneController.Initialize(gameObject);//calls the function to begin initlizaing the propellers and their animation 

        //droneSound = gameObject.transform.Find("drone_sound").GetComponent<AudioSource>();//Call the Audio for the drone
        //not sure if the above code should be droneModel or gameObject  


        // Initializes input controls
        InputManager.Instance.playerActions.DroneControls.VerticalMovementRotation.performed += OnLeftStick;
        InputManager.Instance.playerActions.DroneControls.VerticalMovementRotation.canceled += OnLeftStick;
        InputManager.Instance.playerActions.DroneControls.HorizontalMovement.performed += OnRightStick;
        InputManager.Instance.playerActions.DroneControls.HorizontalMovement.canceled += OnRightStick;
    }

    
    //updates the speed of the propeller based on input (Maybe can be moved to fixedupdate)
    void Update () {
        // Deals with animation
        if (throttleInput > 0) {
            IncreaseThrottleSpeed();
        }
        else if (throttleInput < 0) {
            DecreaseThrottleSpeed();
        }

        UpdateThrottleSpeed();
    }

    // Any physics related drone movement goes here
    private void FixedUpdate()
    {
        // Drone Movement
        MoveDrone();
        SpeedControl();
        DroneSound(); //function call for Drone sound in propeller
        //rotation method (Foward movement needs to be replaced here with the correct command or the tilt method)
        //Yaw();
        //rb.rotation = rb.rotation * Quaternion.AngleAxis(yawSpeed * yawInput, Vector3.up);

        // //This code gets the horizontal input from the player (e.g., from the arrow keys or joystick) and rotates the drone around its y-axis based on that input.
        // float horizontal = Input.GetAxis("Horizontal");
        // transform.Rotate(0, horizontal * yawSpeed * Time.deltaTime, 0);

        // //This code gets the vertical input from the player (e.g., from the up/down arrow keys or joystick) and calculates the movement vector based on the drone's forward direction and the player's input. It then moves the drone's Rigidbody component in that direction using the MovePosition() function.
        // float vertical = Input.GetAxis("Vertical");
        // Vector3 movement = transform.forward * vertical * pitchSpeed * Time.deltaTime;
        // rb.MovePosition(rb.position + movement);

        // Wind
        //Vector3 directionofobj = new Vector3(Random.Range(0, 10), Random.Range(0, 10), Random.Range(0, 10));

        //if (inWindZone)
        // {
        //  int max = 5;
        // int min = -5;
        // Vector3 directionofwobj = new Vector3(Random.Range(min, max), Random.Range(min, max), Random.Range(min, max));
        // wz.AddForce(directionofwobj * windZone.GetComponent<windarea>().strength);

        //}
        if (inWindZone)
        {
            //windStrength = Random.Range(windZone.GetComponent<windarea>().WindStrengthMin, (windZone.GetComponent<windarea>().WindStrengthMax));

            //hitColliders = Physics.OverlapSphere(transform.position, (windZone.GetComponent<windarea>().radius));

            //for (int i = 0; i < hitColliders.Length; i++)
            //{
            //if (wz = hitColliders[i].GetComponent<Rigidbody>())
            //if (Physics.Raycast(transform.position, wz.position - transform.position, out hit))
            //if (hit.transform.GetComponent<Rigidbody>())
            // wz.AddRelativeForce(windDirection * windStrength, ForceMode.Acceleration);
            //}
            Vector3 windDirection = new Vector3(Random.Range(-20f, 20f), Random.Range(-20f, 20f), Random.Range(-20f, 20f));
            rb.AddForce(windDirection * windZone.GetComponent<windarea>().windStrength);

        }

        //Drone height calculation for warning
        Ray ray = new Ray(transform.position, -Vector3.up);

        if(Physics.Raycast(ray, out hit))
        {
            if (hit.collider.tag == "ground")
            {
                float height_above = hit.distance - 2.5f;
                Debug.Log(height_above);
            }
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
        // Adds direction into the input
        Vector3 moveDir = rollInput * direction.right + pitchInput * direction.forward;
        // Movement direction
        Vector3 movement = new Vector3((moveDir.normalized.x * rollSpeed * 2f), (throttleInput * throttlePower), (moveDir.normalized.z * pitchSpeed * 2f));

        //Tilt calculations
        tiltAmountFoward = Mathf.SmoothDamp(tiltAmountFoward, 20 * pitchInput, ref tiltVelocityFoward, 0.5f); //tilting method and speed which can be changed
        tiltAmountHorizontal = Mathf.SmoothDamp(tiltAmountHorizontal, 20 * -rollInput, ref tiltVelocityHorizontal, 0.5f); //tilting method and speed which can be changed

        //rotation of drone
        float droneRotY = yawInput * yawSpeed * Time.deltaTime;
        currentYawSpeed += droneRotY;
        //Applies tilt and the rotation of the drone
        rb.rotation = Quaternion.Euler(new Vector3(tiltAmountFoward, currentYawSpeed, tiltAmountHorizontal));

        //Debug.Log(rb.rotation);
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

    //Functions which control the speed increase, decreasem and update time.
    void IncreaseThrottleSpeed() {
        currentThrottlePower += throttleSpeedIncrement * Time.deltaTime;
        currentThrottlePower = Mathf.Clamp(currentThrottlePower, minThrottleSpeed, maxThrottleSpeed);
    }
    void DecreaseThrottleSpeed() {
        currentThrottlePower -= throttleSpeedIncrement * Time.deltaTime;
        currentThrottlePower = Mathf.Clamp(currentThrottlePower, minThrottleSpeed, maxThrottleSpeed);
    }
    void UpdateThrottleSpeed() {
        foreach (GameObject propeller in propellers) {
            propeller.transform.Rotate(Vector3.up * currentThrottlePower * Time.deltaTime);
        }
    }


    //Function for Drone Sound using imported sound in propellers
     void DroneSound(){
         //droneSound.pitch = 1 + (rb.velocity.magnitude / 100);//drone sound will change based speed of drone. (I really hope its not supposed to be drone instead of rb)
     }//Honestly not sure what to put instead of velocity here.

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
