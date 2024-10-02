using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spectator : MonoBehaviour
{

    public SpaceShip ship;
    public Camera cam;
    public AudioListener listener;
    float sensitivity = 1f;
    int camIndex = 0;
    bool thirdPerson = false;
    bool turretMode = false;

    // Start is called before the first frame update
    void Start()
    {
        if (cam == null)
            cam = Camera.main;
        if (ship != null)
        {
            cam.transform.parent = ship.cameras[camIndex].transform;
            cam.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            Toggle3rdPersonCamera();
            //ship.playerShip = true;
            ship.playSounds = true;
            //ship.player = this;
            listener.transform.parent = ship.cockpit.transform;
            listener.transform.localPosition = Vector3.zero;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (ship != null)
        {

            if (Input.GetMouseButton(1))
            {
                //Debug.Log("Rotating Camera");
                Cursor.lockState = CursorLockMode.Locked;
                if (thirdPerson)
                {
                    ship.thirdPersonRot += sensitivity * Input.GetAxis("Mouse X") * Vector3.up - sensitivity * Input.GetAxis("Mouse Y") * Vector3.right;
                    //ship.thirdPersonAxis.transform.eulerAngles += transform.InverseTransformDirection(ship.thirdPersonCamera.transform.up) * sensitivity * Input.GetAxis("Mouse X") - transform.InverseTransformDirection(ship.thirdPersonCamera.transform.right) * sensitivity * Input.GetAxis("Mouse Y");
                }
                else if (turretMode)
                {
                    //ship.activeTurret.turretRotTarget += (Mathf.Clamp(sensitivity * Input.GetAxis("Mouse X"), -ship.activeTurret.rotSpeed, ship.activeTurret.rotSpeed) * Vector3.forward) + (Mathf.Clamp(sensitivity * Input.GetAxis("Mouse Y"), -ship.activeTurret.rotSpeed, ship.activeTurret.rotSpeed) * Vector3.right);
                }
            }
            if (Input.GetMouseButton(2))
            {
                Cursor.lockState = CursorLockMode.Locked;
                if (thirdPerson)
                {
                    ship.thirdPersonTrans -= cam.transform.right * Input.GetAxis("Mouse X") * sensitivity + cam.transform.up * Input.GetAxis("Mouse Y") * sensitivity;
                }
            }
            if (Input.mouseScrollDelta.x != 0 || Input.mouseScrollDelta.y != 0)
            {
                ship.cameraZoom(Input.mouseScrollDelta.x + Input.mouseScrollDelta.y);
            }
            if (Input.GetMouseButtonUp(1) || Input.GetMouseButtonUp(2))
            {
                Cursor.lockState = CursorLockMode.None;
            }
        }
    }

    public void ToggleTurretCamera(int turretNumber)
    {
        if (thirdPerson)
        {
            Debug.Log("Accessing camera for turret #" + turretNumber);
            thirdPerson = false;
            cam.transform.parent = ship.turrets[turretNumber].cam.transform;
            cam.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            ship.activeTurret = ship.turrets[turretNumber];
            turretMode = true;
        }
    }

    public void Toggle3rdPersonCamera()
    {
        if (thirdPerson)
        {
            thirdPerson = false;
            cam.transform.parent = ship.cameras[camIndex].transform;
            cam.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        }
        else
        {
            if (turretMode)
            {
                ship.activeTurret = null;
                turretMode = false;
            }
            thirdPerson = true;
            cam.transform.parent = ship.thirdPersonCamera.transform;
            cam.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        }
    }

    public void NextCamera()
    {
        if (turretMode)
        {
            ship.activeTurret = null;
            turretMode = false;
        }
        camIndex++;
        camIndex %= ship.cameras.Length;
        cam.transform.parent = ship.cameras[camIndex].transform;
        cam.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
    }
}
