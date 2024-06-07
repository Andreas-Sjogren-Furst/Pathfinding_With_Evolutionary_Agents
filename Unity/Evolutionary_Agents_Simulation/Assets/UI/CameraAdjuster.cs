// code mainly from https://gist.github.com/ashleydavis/f025c03a9221bc840a2b with slight modifications
// Written by: Andreas Sjögren Fürst (s201189)
using UnityEngine;
public class CameraAdjuster : MonoBehaviour
{

    private int mapSize;
    private readonly int cameraHeight = 100;
    private readonly int frameSize = 10;
    void Start()
    {
        AdjustCamera();
    }

    public void AdjustCamera()
    {
        ScreenViewModel screenViewModel = WebView.Instance.screenPresenter.PackageData();
        mapSize = screenViewModel.map.GetLength(0) / 2 + frameSize;
        int x = screenViewModel.spawnPoint.ArrayPosition.x;
        int z = screenViewModel.spawnPoint.ArrayPosition.y;
        Vector3 cameraPosition = new(x, cameraHeight, z);
        Camera.main.orthographicSize = mapSize;
        Camera.main.transform.position = cameraPosition;
        Camera.main.fieldOfView = mapSize;
        Camera.main.transform.rotation = Quaternion.Euler(90, 0, 0);
    }


    /// <summary>
    /// Normal speed of camera movement.
    /// </summary>
    public float movementSpeed = 10f;

    /// <summary>
    /// Speed of camera movement when shift is held down,
    /// </summary>
    public float fastMovementSpeed = 100f;

    /// <summary>
    /// Sensitivity for free look.
    /// </summary>
    public float freeLookSensitivity = 3f;

    /// <summary>
    /// Amount to zoom the camera when using the mouse wheel.
    /// </summary>
    public float zoomSensitivity = 10f;

    /// <summary>
    /// Amount to zoom the camera when using the mouse wheel (fast mode).
    /// </summary>
    public float fastZoomSensitivity = 50f;

    /// <summary>
    /// Set to true when free looking (on right mouse button).
    /// </summary>
    private bool looking = false;

    void Update()
    {
        var fastMode = Input.GetKey(KeyCode.LeftShift);
        var movementSpeed = fastMode ? this.fastMovementSpeed : this.movementSpeed;


        if (Input.GetKeyUp(KeyCode.Space))
        {
            AdjustCamera();
        }

        if (Input.GetKey(KeyCode.A))
        {
            transform.position = transform.position + (-transform.right * movementSpeed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.D))
        {
            transform.position = transform.position + (transform.right * movementSpeed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.W))
        {
            transform.position = transform.position + (transform.forward * movementSpeed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.S))
        {
            transform.position = transform.position + (-transform.forward * movementSpeed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.Q))
        {
            transform.position = transform.position + (transform.up * movementSpeed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.E))
        {
            transform.position = transform.position + (-transform.up * movementSpeed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.R) || Input.GetKey(KeyCode.PageUp))
        {
            transform.position = transform.position + (Vector3.up * movementSpeed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.F) || Input.GetKey(KeyCode.PageDown))
        {
            transform.position = transform.position + (-Vector3.up * movementSpeed * Time.deltaTime);
        }

        if (looking)
        {

            float newRotationX = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * freeLookSensitivity;
            float newRotationY = transform.localEulerAngles.x - Input.GetAxis("Mouse Y") * freeLookSensitivity;
            newRotationY = Mathf.Clamp(newRotationY, -90f, 90f);
            transform.localEulerAngles = new Vector3(newRotationY, newRotationX, 0f);
        }

        float axis = Input.GetAxis("Mouse ScrollWheel");
        if (axis != 0)
        {
            var zoomSensitivity = fastMode ? this.fastZoomSensitivity : this.zoomSensitivity;
            transform.position = transform.position + transform.forward * axis * zoomSensitivity;
        }

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            StartLooking();
        }
        else if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            StopLooking();
        }
    }

    void OnDisable()
    {
        StopLooking();
    }

    /// <summary>
    /// Enable free looking.
    /// </summary>
    public void StartLooking()
    {
        looking = true;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    /// <summary>
    /// Disable free looking.
    /// </summary>
    public void StopLooking()
    {
        looking = false;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }


}
