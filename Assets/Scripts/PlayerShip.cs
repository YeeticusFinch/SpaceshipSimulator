using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerShip : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    public int team = 1;
    public SpaceShip ship;
    public Missile missile;
    public Camera cam;
    public Canvas canvas;
    public GameObject circle;
    public Sprite circleSprite;
    public Sprite targetSprite;
    int camIndex = 0;
    bool thirdPerson = false;
    bool turretMode = false;
    bool cannonMode = false;
    [NonSerialized]
    public bool missileCam = false;
    [NonSerialized]
    public bool missileControl = false;
    public AudioListener listener;

    [NonSerialized]
    public bool traceMissile = false;
    [NonSerialized]
    public bool traceGun = false;
    [NonSerialized]
    public bool radarObjects = false;

    public bool controllingMissile = false;

    [NonSerialized]
    public bool traceRadar = false;

    bool targetLocking = false;

    float sensitivity = 1f;

    [NonSerialized]
    public int tab = 0;
    [NonSerialized]
    public bool lockControls = false;
    public TextMeshProUGUI controlLockText;

    public Panel[] panels;

    public static KeyCode[] MOVEMENT_KEYS =
    {
        KeyCode.LeftControl,
        KeyCode.LeftShift,
        KeyCode.Q,
        KeyCode.W,
        KeyCode.E,
        KeyCode.A,
        KeyCode.S,
        KeyCode.D,
        KeyCode.Space
    };

    public static KeyCode[] NON_MOVEMENT_KEYS =
    {
        KeyCode.R,
        KeyCode.Z,
        KeyCode.X,
        KeyCode.C,
        KeyCode.Tab,
        KeyCode.Alpha0,
        KeyCode.Alpha1,
        KeyCode.Alpha2,
        KeyCode.Alpha3,
        KeyCode.Alpha4,
        KeyCode.Alpha5,
        KeyCode.Alpha6,
        KeyCode.Alpha7,
        KeyCode.Alpha8,
        KeyCode.Alpha9,
        KeyCode.F,
        KeyCode.V,
        KeyCode.T
    };

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DelayedStart());
    }

    IEnumerator DelayedStart()
    {
        yield return new WaitForSeconds(0.2f);
        if (controllingMissile)
            missileControl = true;
        tab = 0;
        if (ship == null)
            ship = GetComponent<SpaceShip>();
        ship.team = team;
        if (cam == null)
            cam = Camera.main;
        if (ship != null)
        {
            cam.transform.parent = ship.cameras[camIndex].transform;
            cam.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            Toggle3rdPersonCamera();
            ship.playerShip = true;
            ship.playSounds = true;
            ship.player = this;
            listener.transform.parent = ship.cockpit.transform;
            listener.transform.localPosition = Vector3.zero;
        }
        else if (missile != null)
        {
            cam.transform.parent = missile.transform;
            cam.transform.SetLocalPositionAndRotation(Vector3.forward * (-5), Quaternion.identity);
        }
    }

    private GameObject targetLocker;

    private int targetWeaponIndex;
    private int targetWeaponType;

    public void BeginTargetLock(int weaponIndex, int weaponType) // 0 == turret, 1 == missile, 2 == cannon
    {
        if (targetLocker != null)
            Destroy(targetLocker);
        targetLocking = true;
        targetLocker = Instantiate(circle, canvas.transform);
        targetLocker.GetComponent<Image>().color = Color.red;
        targetLocker.GetComponentInChildren<TextMeshProUGUI>().text = "Click on Target";
        targetLocker.GetComponentInChildren<TextMeshProUGUI>().color = Color.red;
        targetWeaponIndex = weaponIndex;
        targetWeaponType = weaponType;
    }

    private void FixedUpdate()
    {
        if (Game.instance != null && Game.instance.displayTimer)
        {
            timerText.text = (Mathf.Round(Game.instance.GetTimer()*100)/100).ToString();
        }
    }

    // Update is called once per frame
    void Update()
    {
        //if (Application.isEditor && Input.GetKey(KeyCode.X))
        //    UnityEditor.EditorApplication.isPaused = true;
        if (missile != null)
        {
            if (Input.GetMouseButton(1))
            {
                //Debug.Log("Rotating Camera");
                Cursor.lockState = CursorLockMode.Locked;
                //if (thirdPerson)
                //{
                    missile.camRot += sensitivity * Input.GetAxis("Mouse X") * Vector3.up - sensitivity * Input.GetAxis("Mouse Y") * Vector3.right;
                    //ship.thirdPersonAxis.transform.eulerAngles += transform.InverseTransformDirection(ship.thirdPersonCamera.transform.up) * sensitivity * Input.GetAxis("Mouse X") - transform.InverseTransformDirection(ship.thirdPersonCamera.transform.right) * sensitivity * Input.GetAxis("Mouse Y");
                //}
            }
            if (Input.GetMouseButton(2))
            {
                Cursor.lockState = CursorLockMode.Locked;
                //if (thirdPerson)
                //{
                    missile.camTrans -= cam.transform.right * Input.GetAxis("Mouse X") * sensitivity + cam.transform.up * Input.GetAxis("Mouse Y") * sensitivity;
                //}
            }
            if (controllingMissile || missileControl)
            {
                if (Input.mouseScrollDelta.x != 0 || Input.mouseScrollDelta.y != 0)
                {
                    missile.camTrans += (Input.mouseScrollDelta.x + Input.mouseScrollDelta.y) * cam.transform.forward;
                }
                if (Input.GetMouseButtonUp(1) || Input.GetMouseButtonUp(2))
                {
                    Cursor.lockState = CursorLockMode.None;
                }

                if (Input.GetKey(KeyCode.LeftControl))
                {
                    if (Input.GetKey(KeyCode.W))
                        missile.FireThrusters(missile.alt_w_thrusters, 1);
                    if (Input.GetKey(KeyCode.S))
                        missile.FireThrusters(missile.alt_s_thrusters, 1);
                    if (Input.GetKey(KeyCode.A))
                        missile.FireThrusters(missile.alt_a_thrusters, 1);
                    if (Input.GetKey(KeyCode.D))
                        missile.FireThrusters(missile.alt_d_thrusters, 1);
                    if (Input.GetKey(KeyCode.Space))
                        missile.FireThrusters(missile.alt_space_thrusters, 1);
                    if (Input.GetKey(KeyCode.LeftShift))
                        missile.FireThrusters(missile.alt_shift_thrusters, 1);
                }
                else
                {
                    if (missile.momentumTurn)
                    {
                        if (Input.GetKey(KeyCode.W))
                            missile.MomentumRotate(new Vector3(missile.turnSpeed, 0, 0));
                        if (Input.GetKey(KeyCode.S))
                            missile.MomentumRotate(new Vector3(-missile.turnSpeed, 0, 0));
                        if (Input.GetKey(KeyCode.A))
                            missile.MomentumRotate(new Vector3(0, 0, missile.turnSpeed));
                        if (Input.GetKey(KeyCode.D))
                            missile.MomentumRotate(new Vector3(0, 0, -missile.turnSpeed));
                    }
                    else
                    {
                        if (Input.GetKey(KeyCode.W))
                            missile.FireThrusters(missile.w_thrusters, 1);
                        if (Input.GetKey(KeyCode.S))
                            missile.FireThrusters(missile.s_thrusters, 1);
                        if (Input.GetKey(KeyCode.A))
                            missile.FireThrusters(missile.a_thrusters, 1);
                        if (Input.GetKey(KeyCode.D))
                            missile.FireThrusters(missile.d_thrusters, 1);
                    }
                    if (Input.GetKey(KeyCode.Space))
                        missile.FireThrusters(missile.space_thrusters, 1);
                    if (Input.GetKey(KeyCode.LeftShift))
                        missile.FireThrusters(missile.shift_thrusters, 1);
                }
                if (missile.momentumTurn)
                {
                    if (Input.GetKey(KeyCode.Q))
                        missile.MomentumRotate(new Vector3(0, -missile.turnSpeed, 0));
                    if (Input.GetKey(KeyCode.E))
                        missile.MomentumRotate(new Vector3(0, missile.turnSpeed, 0));
                }
                else
                {
                    if (Input.GetKey(KeyCode.Q))
                        missile.FireThrusters(missile.q_thrusters, 1);
                    if (Input.GetKey(KeyCode.E))
                        missile.FireThrusters(missile.e_thrusters, 1);
                }
            }
        }
        else if (ship != null)
        {
            if (targetLocking && targetLocker != null)
            {
                Vector2 mousePos = Input.mousePosition;
                //mousePos.z = 1;
                //targetLocker.transform.localPosition = cam.transform.InverseTransformPoint(cam.ScreenToWorldPoint(mousePos)).normalized * canvas.planeDistance;
                targetLocker.GetComponent<RectTransform>().anchoredPosition = mousePos - canvas.GetComponent<RectTransform>().sizeDelta / 2f;
            }
            if (lockControls)
            {
                foreach (KeyCode k in NON_MOVEMENT_KEYS)
                    CheckKey(k);
            }
            else
            {
                foreach (KeyCode k in MOVEMENT_KEYS)
                    CheckKey(k);
                foreach (KeyCode k in NON_MOVEMENT_KEYS)
                    CheckKey(k);
            }

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
                    Debug.Log("Rotate turret " + (Mathf.Clamp(sensitivity * Input.GetAxis("Mouse X"), -ship.activeGun.rotSpeed, ship.activeGun.rotSpeed) * Vector3.forward) + (Mathf.Clamp(sensitivity * Input.GetAxis("Mouse Y"), -ship.activeGun.rotSpeed, ship.activeGun.rotSpeed) * Vector3.right));
                    ship.activeGun.turretRotTarget += (Mathf.Clamp(sensitivity * Input.GetAxis("Mouse X"), -ship.activeGun.rotSpeed, ship.activeGun.rotSpeed) * Vector3.forward) + (Mathf.Clamp(sensitivity * Input.GetAxis("Mouse Y"), -ship.activeGun.rotSpeed, ship.activeGun.rotSpeed) * Vector3.right);
                } else if (cannonMode)
                {
                    //float rotSpeed = 50;
                    //Debug.Log("Rotating ship " + (Mathf.Clamp(sensitivity * Input.GetAxis("Mouse X"), -ship.activeGun.rotSpeed, ship.activeGun.rotSpeed) * Vector3.forward) + (Mathf.Clamp(sensitivity * Input.GetAxis("Mouse Y"), -ship.activeGun.rotSpeed, ship.activeGun.rotSpeed) * Vector3.right));
                    ship.activeGun.rotateGun += (sensitivity * Input.GetAxis("Mouse X") * Vector3.forward) + (sensitivity * Input.GetAxis("Mouse Y") * Vector3.right);
                    ship.activeGun.rotateShip = true;
                }
            } else
            {
                if (cannonMode)
                {
                    ship.activeGun.rotateGun = Vector3.zero;
                    ship.activeGun.rotateShip = false;
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
            if (Input.GetMouseButtonDown(0))
            {
                if (targetLocking && targetLocker != null)
                {
                    Debug.Log("TargetLockClick");
                    Indicator closestTarget = null;
                    foreach (Indicator o in GameObject.FindObjectsOfType(typeof(Indicator)))
                    {
                        if (o != targetLocker && o.GetComponentInChildren<TextMeshProUGUI>().text != targetLocker.GetComponentInChildren<TextMeshProUGUI>().text)
                        {
                            if (closestTarget == null)
                                closestTarget = o;
                            else if (Vector3.Distance(o.GetComponent<RectTransform>().anchoredPosition, targetLocker.GetComponent<RectTransform>().anchoredPosition) < Vector3.Distance(closestTarget.GetComponent<RectTransform>().anchoredPosition, targetLocker.GetComponent<RectTransform>().anchoredPosition))
                            {
                                closestTarget = o;
                            }
                            Debug.Log("Closest Target = " + o.gameObject.name);
                        }
                    }

                    float distance = Vector2.Distance(closestTarget.GetComponent<RectTransform>().anchoredPosition, targetLocker.GetComponent<RectTransform>().anchoredPosition);

                    Debug.Log("Distance = " + distance);

                    //if (distance < 1.15f)
                    if (distance < 30)
                    {
                        Destroy(targetLocker);
                        targetLocking = false;
                        ship.Target(closestTarget.target, targetWeaponIndex, targetWeaponType);
                    }
                }
            }
            if (Input.GetMouseButton(0))
            {
                if (!targetLocking)
                {
                    Vector3 panelPos = (Vector3)panels[0].gameObject.GetComponent<RectTransform>().rect.position + Vector3.right * Screen.width;
                    if (Input.mousePosition.x / canvas.scaleFactor < panelPos.x && Input.mousePosition.y / canvas.scaleFactor > 60 && Input.mousePosition.y / canvas.scaleFactor < Screen.height / canvas.scaleFactor - 60)
                    {
                        if (turretMode || cannonMode)
                        {
                            ship.activeGun.Shoot();
                        }
                        else
                        {
                            foreach (gun t in ship.turrets)
                                if (t.fireMode == 1)
                                {
                                    if (t.aimMode != 2)
                                        t.ShootIfAimingAtTarget(ship.GetThirdPersonTarget());
                                    else
                                        t.Shoot();
                                }
                            foreach (gun t in ship.statGuns)
                            {
                                if (t.fireMode == 1)
                                    t.Shoot();
                            }
                        }
                    }
                }
            }
        }
    }

    void CheckKey(KeyCode k)
    {
        if (Input.GetKeyDown(k))
            ship.pressedKeys.Add(k);
        else if (Input.GetKeyUp(k))
            ship.pressedKeys.Remove(k);
    }

    public void ToggleTurretCamera(int turretNumber)
    {
        if (!thirdPerson)
            Toggle3rdPersonCamera();
        if (thirdPerson)
        {
            Debug.Log("Accessing camera for turret #" + turretNumber);
            thirdPerson = false;
            cam.transform.parent = ship.turrets[turretNumber].cam.transform;
            cam.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            ship.activeGun = ship.turrets[turretNumber];
            turretMode = true;
        }
    }

    public void ToggleCannonCamera(int cannonNumber)
    {
        if (!thirdPerson)
            Toggle3rdPersonCamera();
        if (thirdPerson)
        {
            Debug.Log("Accessing camera for cannon #" + cannonNumber);
            thirdPerson = false;
            cam.transform.parent = ship.statGuns[cannonNumber].cam.transform;
            cam.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            ship.activeGun = ship.statGuns[cannonNumber];
            cannonMode = true;
        }
    }

    public void Toggle3rdPersonCamera()
    {
        if (thirdPerson)
        {
            thirdPerson = false;
            cam.transform.parent = ship.cameras[camIndex].transform;
            cam.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        } else
        {
            if (turretMode)
            {
                ship.activeGun = null;
                turretMode = false;
            }
            if (cannonMode)
            {
                ship.activeGun.rotateShip = false;
                ship.activeGun = null;
                cannonMode = false;
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
            ship.activeGun = null;
            turretMode = false;
        }
        if (cannonMode)
        {
            ship.activeGun.rotateShip = false;
            ship.activeGun = null;
            cannonMode = false;
        }
        camIndex++;
        camIndex %= ship.cameras.Length;
        cam.transform.parent = ship.cameras[camIndex].transform;
        cam.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
    }

    public void SwitchTab(int tab)
    {
        //Debug.Log("Switching from tab " + this.tab + " to tab " + tab);
        if (this.tab >= 0 && this.tab < panels.Length)
            panels[this.tab].gameObject.SetActive(false);
        this.tab = tab;
        if (tab >= 0 && tab < panels.Length)
            panels[tab].gameObject.SetActive(true);
    }

    public void LockControls()
    {
        foreach (KeyCode k in MOVEMENT_KEYS)
        {
            if (Input.GetKey(k))
                ship.pressedKeys.Remove(k);
        }
        lockControls = !lockControls;
        controlLockText.text = lockControls ? "Locked" : "Unlocked";
        panels[0].locked = lockControls;
    }
}
