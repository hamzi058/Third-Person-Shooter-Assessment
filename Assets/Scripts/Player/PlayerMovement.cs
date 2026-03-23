using UnityEngine;
using TMPro;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public enum ControlMode
    {
        PC,
        Mobile
    }

    [Header("Control Mode")]
    public ControlMode controlMode = ControlMode.PC;

    [Header("Mobile")]
    public FixedJoystick joystick;
    public MobileAimTrackpad aimTrackpad;

    [Header("Mobile UI")]
    public GameObject mobileControlUICanvas;
    public GameObject CtrlInfoPanel;

    [Header("Mode Text")]
    public TextMeshProUGUI controlModeText;

    [Header("Movement")]
    public float walkSpeed = 4f;
    public float runSpeed = 8f;
    public float rotationSpeed = 15f;

    [Header("Camera Pitch")]
    public Transform cameraPivot;
    public float pitchSpeed = 0.25f;
    public float minPitch = -35f;
    public float maxPitch = 45f;

    CharacterController controller;
    Camera cam;

    Vector3 localInput;
    bool isRunning;

    float pitch;
    float yaw;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        cam = Camera.main;
    }

    void Start()
    {
        UpdateControlModeUI();

        // ⭐ initialise pitch
        float angle = cameraPivot.localEulerAngles.x;
        if (angle > 180) angle -= 360;
        pitch = angle;

        // ⭐ initialise yaw
        yaw = transform.eulerAngles.y;
    }

    void Update()
    {
        HandleRunInput();
        ReadInput();
        Aim();
        Move();
    }

    void HandleRunInput()
    {
        if (controlMode == ControlMode.PC)
            isRunning = Input.GetKey(KeyCode.LeftShift);
    }

    public void MobileRunButtonDown() => isRunning = true;
    public void MobileRunButtonUp() => isRunning = false;

    void ReadInput()
    {
        float h;
        float v;

        if (controlMode == ControlMode.PC)
        {
            h = Input.GetAxisRaw("Horizontal");
            v = Input.GetAxisRaw("Vertical");
        }
        else
        {
            if (!joystick)
            {
                localInput = Vector3.zero;
                return;
            }

            h = joystick.Horizontal;
            v = joystick.Vertical;
        }

        localInput = new Vector3(h, 0, v);
        localInput = Vector3.ClampMagnitude(localInput, 1f);
    }

    void Move()
    {
        float speed = isRunning ? runSpeed : walkSpeed;

        Vector3 move =
            transform.forward * localInput.z +
            transform.right * localInput.x;

        controller.Move(move * speed * Time.deltaTime);
    }

    void Aim()
    {
        if (controlMode == ControlMode.PC)
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            Plane plane = new Plane(Vector3.up, transform.position);

            if (plane.Raycast(ray, out float enter))
            {
                Vector3 hitPoint = ray.GetPoint(enter);

                Vector3 dir = hitPoint - transform.position;
                dir.y = 0f;

                if (dir.sqrMagnitude > 0.001f)
                {
                    Quaternion targetRot = Quaternion.LookRotation(dir);

                    transform.rotation = Quaternion.Slerp(
                        transform.rotation,
                        targetRot,
                        rotationSpeed * Time.deltaTime);

                    yaw = transform.eulerAngles.y;
                }
            }
        }
        else
        {
            Vector2 delta = aimTrackpad.AimDelta;

            yaw += delta.x * 0.25f;

            pitch -= delta.y * pitchSpeed;
            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

            transform.rotation = Quaternion.Euler(0f, yaw, 0f);
            cameraPivot.localRotation = Quaternion.Euler(pitch, 0f, 0f);
        }
    }

    void UpdateControlModeUI()
    {
        if (mobileControlUICanvas)
            mobileControlUICanvas.SetActive(controlMode == ControlMode.Mobile);

        if (CtrlInfoPanel)
            CtrlInfoPanel.SetActive(controlMode == ControlMode.PC);

        if (controlModeText)
        {
            controlModeText.text =
                controlMode == ControlMode.PC
                ? "PC Control Active"
                : "Mobile Control Active";
        }
    }

    void OnValidate()
    {
        UpdateControlModeUI();
    }
}