using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Panel : MonoBehaviour
{
    PlayerShip player;
    public int id = 0;

    [NonSerialized]
    public bool locked = false;

    public Panel[] specialPanels;

    [NonSerialized]
    public Panel[] subpanels;
    [NonSerialized]
    public Panel[] subpanels2;
    [NonSerialized]
    public Panel superiorPanel;
    [NonSerialized]
    public Panel[] subpanels3;
    
    int GetConfigSlotIndex()
    {
        if (id < 0)
            return -id - 1;
        return -1;
    }

    bool IsConfigSubPanel()
    {
        return superiorPanel != null && superiorPanel.id == 6 && id < 0;
    }

    SpaceShip GetConfigSubShip()
    {
        if (!IsConfigSubPanel() || player == null || player.ship == null || player.ship.subShips == null)
            return null;
        int slotIndex = GetConfigSlotIndex();
        if (slotIndex < 0 || slotIndex >= player.ship.subShips.Length)
            return null;
        return player.ship.subShips[slotIndex];
    }

    void UpdateConfigSubPanelTitle()
    {
        if (!IsConfigSubPanel() || transform.childCount == 0)
            return;
        TextMeshProUGUI title = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        if (title == null)
            return;
        SpaceShip sub = GetConfigSubShip();
        if (sub == null)
            title.text = "Empty Slot";
        else
            title.text = "[" + (GetConfigSlotIndex() + 1) + "] " + sub.name;
    }

    // Start is called before the first frame update
    void Start()
    {
        player = GetComponentInParent<PlayerShip>();
        if (player == null || player.ship == null)
            return;
        if (id == 4)
        {
            superiorPanel = this;

            subpanels3 = new Panel[5];
            //subpanels3[0] = specialPanels[0];
            for (int i = 0; i < subpanels3.Length; i++) {
                GameObject newPanel;
                if (i == 0)
                    newPanel = specialPanels[0].gameObject;
                else
                    newPanel = GameObject.Instantiate(specialPanels[0].gameObject, transform) as GameObject;
                newPanel.transform.SetParent(transform);
                subpanels3[i] = newPanel.GetComponent<Panel>();
                newPanel.GetComponent<RectTransform>().position -= Vector3.up * 165f * i;
                //Debug.Log("New Panel at " + newPanel.GetComponent<RectTransform>().position);
                subpanels3[i].id = -i - 1;
                newPanel.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "---";
                subpanels3[i].superiorPanel = this;
                subpanels3[i].subpanels3 = subpanels3;
            }
        }
        else if (id == 2)
        {
            superiorPanel = this;
            //Debug.Log("Id == 2");
            subpanels = new Panel[player.ship.turrets.Length + 1];
            subpanels[0] = specialPanels[0];
            subpanels[0].id = -1;
            subpanels[0].superiorPanel = this;
            subpanels[0].subpanels = subpanels;
            //Debug.Log("Subpanels = " + subpanels.Length);
            for (int i = 0; i < player.ship.turrets.Length; i++)
            {
                GameObject newPanel = GameObject.Instantiate(subpanels[0].gameObject, transform) as GameObject;
                subpanels[i + 1] = newPanel.GetComponent<Panel>();
                newPanel.SetActive(false);
                newPanel.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "[" + (i + 1) + "] " + player.ship.turrets[i].name;
                //Debug.Log("Adding subpanel " + newPanel.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text);
                subpanels[i + 1].id = -(i + 2);
                subpanels[i + 1].superiorPanel = this;
                subpanels[i + 1].subpanels = subpanels;
            }

            subpanels2 = new Panel[player.ship.launchers.Length + 1];
            subpanels2[0] = specialPanels[1];
            subpanels2[0].id = -1;
            subpanels2[0].superiorPanel = this;
            subpanels2[0].subpanels = subpanels;
            for (int i = 0; i < player.ship.launchers.Length; i++)
            {
                GameObject newPanel = GameObject.Instantiate(subpanels2[0].gameObject, transform) as GameObject;
                subpanels2[i + 1] = newPanel.GetComponent<Panel>();
                newPanel.SetActive(false);
                newPanel.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "[" + (i + 1) + "] " + player.ship.launchers[i].name;
                //Debug.Log("Adding subpanel2 " + newPanel.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text);
                subpanels2[i + 1].id = -(i + 2);
                subpanels2[i + 1].superiorPanel = this;
                subpanels2[i + 1].subpanels = subpanels;
            }
            
            subpanels3 = new Panel[player.ship.statGuns.Length + 1];
            subpanels3[0] = specialPanels[2];
            subpanels3[0].id = -1;
            subpanels3[0].superiorPanel = this;
            subpanels3[0].subpanels = subpanels;
            //Debug.Log("Subpanels = " + subpanels.Length);
            for (int i = 0; i < player.ship.statGuns.Length; i++)
            {
                GameObject newPanel = GameObject.Instantiate(subpanels3[0].gameObject, transform) as GameObject;
                subpanels3[i + 1] = newPanel.GetComponent<Panel>();
                newPanel.SetActive(false);
                newPanel.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "[" + (i + 1) + "] " + player.ship.statGuns[i].name;
                //Debug.Log("Adding subpanel " + newPanel.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text);
                subpanels3[i + 1].id = -(i + 2);
                subpanels3[i + 1].superiorPanel = this;
                subpanels3[i + 1].subpanels = subpanels;
            }
        } else if (id == 6) // Configuration Panel
        {
            int subShipCount = (player.ship.subShips == null ? 0 : player.ship.subShips.Length);
            subpanels = new Panel[subShipCount];
            if (subShipCount == 0)
            {
                if (specialPanels != null && specialPanels.Length > 0 && specialPanels[0] != null && specialPanels[0].transform.childCount > 0)
                {
                    TextMeshProUGUI title = specialPanels[0].transform.GetChild(0).GetComponent<TextMeshProUGUI>();
                    if (title != null)
                        title.text = "No Sub Ships Available";
                }
                return;
            }
            subpanels[0] = specialPanels[0];
            for (int i = 0; i < subShipCount; i++)
            {
                if (i > 0)
                {
                    GameObject newPanel = GameObject.Instantiate(subpanels[0].gameObject, transform) as GameObject;
                    subpanels[i] = newPanel.GetComponent<Panel>();
                    newPanel.SetActive(false);
                }
                subpanels[i].id = -(i + 1);
                subpanels[i].superiorPanel = this;
                subpanels[i].subpanels = subpanels;
                subpanels[i].UpdateConfigSubPanelTitle();
            }
        }
    }

    int radarListStart = 0;
    int c = 0;

    bool TryGetValidRadarTarget(GameObject[] indicators, int index, out GameObject target)
    {
        target = null;
        if (indicators == null || index < 0 || index >= indicators.Length)
            return false;
        GameObject indicatorObject = indicators[index];
        if (indicatorObject == null)
            return false;
        Indicator indicator = indicatorObject.GetComponent<Indicator>();
        if (indicator == null || indicator.target == null)
        {
            Destroy(indicatorObject);
            return false;
        }
        target = indicator.target;
        return target != null;
    }

    void FixedUpdate()
    {
        if (player == null || player.ship == null)
            return;
        c++;
        c %= 10000;

        if (c % 20 == 0)
        {
            if (id == 4)
            {
                GameObject[] indi = GameObject.FindGameObjectsWithTag("UI Circle");
                for (int i = 0; i < subpanels3.Length; i++)
                {
                    int indicatorIndex = i + radarListStart;
                    GameObject currentTarget;
                    if (subpanels3[i] != null && subpanels3[i].gameObject != null && subpanels3[i].gameObject.transform.childCount > 0 && TryGetValidRadarTarget(indi, indicatorIndex, out currentTarget))
                    {
                        subpanels3[i].gameObject.SetActive(true);
                        SpaceObject targetSpaceObject = currentTarget.GetComponent<SpaceObject>();
                        subpanels3[i].gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = targetSpaceObject == null ? "---" : targetSpaceObject.name;
                    }
                    else
                    {
                        subpanels3[i].gameObject.SetActive(false);
                    }
                }
            }
            UpdateButtons();
        }
    }

    public void PanelButton(string id)
    {
        if (player == null || player.ship == null)
            return;
        switch (id)
        {
            case "lock":
                locked = !locked;
                break;
            case "drive_power":
                if (!locked)
                {
                    player.ship.reactor.drivePower = !player.ship.reactor.drivePower;
                }
                break;
            case "weapon_power":
                if (!locked)
                {
                    player.ship.reactor.weaponPower = !player.ship.reactor.weaponPower;
                }
                break;
            case "telemetry_power":
                if (!locked)
                {
                    player.ship.reactor.telemetryPower = !player.ship.reactor.telemetryPower;
                }
                break;
            case "comms_power":
                if (!locked)
                {
                    player.ship.reactor.commsPower = !player.ship.reactor.commsPower;
                }
                break;
            case "battery_power":
                if (!locked)
                {
                    if (player.ship.reactor.batteryAmount < 0.01f)
                        player.ship.reactor.batteryPower = false;
                    else
                        player.ship.reactor.batteryPower = !player.ship.reactor.batteryPower;
                }
                break;
            case "explode_reactor":
                if (!locked)
                {
                    if (player.ship.reactor.HP > 0.001f && player.ship.reactor.power)
                        player.ship.reactor.Explode();
                }
                break;
            case "overclock":
                if (!locked)
                {
                    if (player.ship.reactor.power)
                    {
                        player.ship.reactor.highPower = !player.ship.reactor.highPower;
                        player.ship.reactor.lowPower = false;
                    }
                }
                break;
            case "low_power":
                if (!locked)
                {
                    if (player.ship.reactor.power)
                    {
                        player.ship.reactor.highPower = false;
                        player.ship.reactor.lowPower = !player.ship.reactor.lowPower;
                    }
                }
                break;
            case "drop_core":
                if (!locked)
                {
                    player.ship.reactor.DropCore();
                }
                break;
            case "reactor_power":
                if (!locked)
                {
                    player.ship.reactor.tryingToPower = !player.ship.reactor.tryingToPower;
                    player.ship.reactor.power = player.ship.reactor.tryingToPower;
                }
                break;
            case "missile_cam":
                if (!locked)
                {
                    player.missileCam = !player.missileCam;
                    if (!player.missileCam)
                        player.missileControl = false;
                }
                break;
            case "missile_ctrl":
                if (!locked)
                {
                    player.missileControl = !player.missileControl;
                    player.missileCam = player.missileControl;
                }
                break;
            case "stabalizers":
                if (!locked)
                {
                    player.ship.stabalizers = !player.ship.stabalizers;
                }
                break;
            case "stabalizer_brakes":
                if (!locked)
                {
                    player.ship.autoBrakes = !player.ship.autoBrakes;
                }
                break;
            case "stabalizer_thrust":
                if (!locked)
                {
                    player.ship.useStabalizersForMainThrust = !player.ship.useStabalizersForMainThrust;
                }
                break;
            case "main_drive_stabalizer":
                if (!locked)
                {
                    player.ship.useMainDriveForStabalization = !player.ship.useMainDriveForStabalization;
                }
                break;
            case "flip_n_burn":
                if (!locked)
                {
                    player.ship.flip_n_burn = !player.ship.flip_n_burn;
                }
                break;
            case "reboot_propulsion":
                if (!locked)
                {
                    player.ship.rebootPropulsion();
                }
                break;
            case "turret_camera":
                if (!locked)
                {
                    int currentIndex = -this.id - 2;
                    if (this.id == -1)
                        currentIndex = 0;
                    player.ToggleTurretCamera(currentIndex);
                }
                break;
            case "cannon_camera":
                if (!locked)
                {
                    int currentIndex = -this.id - 2;
                    if (this.id == -1)
                        currentIndex = 0;
                    player.ToggleCannonCamera(currentIndex);
                }
                break;
            case "turret_fold":
                if (!locked)
                {
                    int currentIndex = -this.id - 2;
                    if (this.id == -1)
                    {
                        player.ship.turrets[0].fold = !player.ship.turrets[0].fold;
                        foreach (gun t in player.ship.turrets)
                            t.fold = player.ship.turrets[0].fold;
                    } else
                    {
                        player.ship.turrets[currentIndex].fold = !player.ship.turrets[currentIndex].fold;
                    }
                }
                break;
            case "launcher_fold":
                if (!locked)
                {
                    if (player.ship.launchers == null || player.ship.launchers.Length == 0)
                        break;
                    int currentIndex = -this.id - 2;
                    if (this.id == -1)
                    {
                        player.ship.launchers[0].fold = !player.ship.launchers[0].fold;
                        foreach (MissileLauncher l in player.ship.launchers)
                            l.fold = player.ship.launchers[0].fold;
                    }
                    else if (currentIndex >= 0 && currentIndex < player.ship.launchers.Length)
                    {
                        player.ship.launchers[currentIndex].fold = !player.ship.launchers[currentIndex].fold;
                    }
                }
                break;
            case "aim_mode":
                if (!locked)
                {
                    int currentIndex = -this.id - 2;
                    if (this.id == -1)
                    {
                        player.ship.turrets[0].nextAimMode();
                        foreach (gun t in player.ship.turrets)
                            t.aimMode = player.ship.turrets[0].aimMode;
                    } else {
                        player.ship.turrets[currentIndex].nextAimMode();
                    }
                }
                break;
            case "fire_mode":
                //Debug.Log("Turret fire mode");
                if (!locked)
                {
                    int currentIndex = -this.id - 2;
                    if (this.id == -1)
                    {
                        player.ship.turrets[0].nextFireMode();
                        foreach (gun t in player.ship.turrets)
                            t.fireMode = player.ship.turrets[0].fireMode;
                    }
                    else
                    {
                        player.ship.turrets[currentIndex].nextFireMode();
                    }
                }
                break;
            case "cannon_fire_mode":
                //Debug.Log("Cannon fire mode");
                if (!locked)
                {
                    int currentIndex = -this.id - 2;
                    if (this.id == -1)
                    {
                        player.ship.statGuns[0].nextFireMode();
                        foreach (gun t in player.ship.statGuns)
                            t.fireMode = player.ship.statGuns[0].fireMode;
                    }
                    else
                    {
                        player.ship.statGuns[currentIndex].nextFireMode();
                    }
                }
                break;
            case "flashlight":
                if (!locked)
                {
                    int currentIndex = -this.id - 2;
                    if (this.id == -1)
                    {
                        if (player.ship.turrets[0].flashLightStatus())
                            foreach (gun t in player.ship.turrets)
                                t.TurnOffFlashLight();
                        else
                            foreach (gun t in player.ship.turrets)
                                t.TurnOnFlashLight();
                    }
                    else
                    {
                        if (player.ship.turrets[currentIndex].flashLightStatus())
                            player.ship.turrets[currentIndex].TurnOffFlashLight();
                        else
                            player.ship.turrets[currentIndex].TurnOnFlashLight();
                    }
                }
                break;
            case "radaredUp":
                if (!locked)
                {

                }
                break;
            case "ClearTargetLock":
                if (!locked)
                {
                    GameObject[] indi = GameObject.FindGameObjectsWithTag("UI Circle");
                    int index = -this.id - 1 + radarListStart;
                    GameObject currentTarget;
                    if (TryGetValidRadarTarget(indi, index, out currentTarget))
                    {
                        player.ship.ClearTargetLock(currentTarget);
                    }
                }
                break;
            case "PointTowardsRadar":
                if (!locked)
                {
                    GameObject[] indi = GameObject.FindGameObjectsWithTag("UI Circle");
                    int index = -this.id - 1 + radarListStart;
                    GameObject currentTarget;
                    if (TryGetValidRadarTarget(indi, index, out currentTarget))
                    {
                        if (player.ship.pointTo == currentTarget.gameObject)
                            player.ship.pointTo = null;
                        else
                            player.ship.pointTo = currentTarget.gameObject;
                    }
                }
                break;
            case "FireTurrets":
                if (!locked)
                {
                    GameObject[] indi = GameObject.FindGameObjectsWithTag("UI Circle");
                    int index = -this.id - 1 + radarListStart;
                    GameObject currentTarget;
                    if (TryGetValidRadarTarget(indi, index, out currentTarget))
                    {
                        if (!player.ship.turrets[0].targets.ContainsKey(currentTarget))
                        {
                            player.ship.Target(currentTarget, -1, 0);
                            //Debug.Log("Targeting " + currentTarget.name);
                        }
                        else
                        {
                            player.ship.ClearTargetLock(currentTarget);
                            //Debug.Log("Un-targeting " + currentTarget.name);
                        }
                    }
                }
                break;
            case "target_lock":
                if (!locked)
                {
                    int currentIndex = -this.id - 2;
                    if (this.id == -1)
                    {
                        currentIndex = -1;
                    }
                    player.BeginTargetLock(currentIndex, 0);
                }
                break;
            case "cannon_target_lock":
                if (!locked)
                {
                    int currentIndex = -this.id - 2;
                    if (this.id == -1)
                    {
                        currentIndex = -1;
                    }
                    player.BeginTargetLock(currentIndex, 2);
                }
                break;
            case "FireMissile":
                if (!locked)
                {
                    GameObject[] indi = GameObject.FindGameObjectsWithTag("UI Circle");
                    int index = -this.id - 1 + radarListStart;
                    GameObject currentTarget;
                    if (TryGetValidRadarTarget(indi, index, out currentTarget))
                    {
                        player.ship.Target(currentTarget, -1, 1);
                    }
                }
                break;
            case "target_launch":
                if (!locked)
                {
                    int currentIndex = -this.id - 2;
                    if (this.id == -1)
                        currentIndex = -1;
                    player.BeginTargetLock(currentIndex, 1);
                }
                break;
            case "intercept_launch":
                if (!locked)
                {
                    int currentIndex = -this.id - 2;
                    if (this.id == -1)
                    {
                        if (player.ship.launchers[0].defend)
                            foreach (MissileLauncher t in player.ship.launchers)
                                t.defend = false;
                        else
                            foreach (MissileLauncher t in player.ship.launchers)
                                t.defend = true;
                    } else
                    {
                        if (player.ship.launchers[currentIndex].defend)
                            player.ship.launchers[currentIndex].defend = false;
                        else
                            player.ship.launchers[currentIndex].defend = true;
                    }
                }
                break;
            case "defend":
                if (!locked)
                {
                    int currentIndex = -this.id - 2;
                    if (this.id == -1)
                    {
                        if (player.ship.turrets[0].defend)
                            foreach (gun t in player.ship.turrets)
                                t.defend = false;
                        else
                            foreach (gun t in player.ship.turrets)
                                t.defend = true;
                    }
                    else
                    {
                        if (player.ship.turrets[currentIndex].defend)
                            player.ship.turrets[currentIndex].defend = false;
                        else
                            player.ship.turrets[currentIndex].defend = true;
                    }
                }
                break;
            case "next":
                if (!locked)
                {
                    if (superiorPanel == null || superiorPanel.subpanels == null || superiorPanel.subpanels.Length == 0)
                        break;
                    if (superiorPanel.subpanels.Length == 1)
                        break;
                    int currentIndex = -this.id - 1;
                    int nextIndex = 0;
                    if (this.id == -1) {
                        currentIndex = 0;
                        nextIndex = 1;
                    } else {
                        nextIndex = (currentIndex + 1) % superiorPanel.subpanels.Length;
                    }
                    superiorPanel.subpanels[nextIndex].gameObject.SetActive(true);
                    superiorPanel.subpanels[currentIndex].gameObject.SetActive(false);
                }
                break;
            case "previous":
                if (!locked)
                {
                    if (superiorPanel == null || superiorPanel.subpanels == null || superiorPanel.subpanels.Length == 0)
                        break;
                    if (superiorPanel.subpanels.Length == 1)
                        break;
                    int currentIndex = -this.id - 1;
                    int nextIndex = 0;
                    if (this.id == -1)
                    {
                        currentIndex = 0;
                        nextIndex = superiorPanel.subpanels.Length - 1;
                    }
                    else
                    {
                        nextIndex = (currentIndex - 1) % superiorPanel.subpanels.Length;
                    }
                    if (nextIndex < 0) nextIndex += superiorPanel.subpanels.Length;
                    superiorPanel.subpanels[nextIndex].gameObject.SetActive(true);
                    superiorPanel.subpanels[currentIndex].gameObject.SetActive(false);
                }
                break;
            case "next2":
                if (!locked)
                {
                    int currentIndex = -this.id - 1;
                    int nextIndex = 0;
                    if (this.id == -1)
                    {
                        currentIndex = 0;
                        nextIndex = 1;
                    }
                    else
                    {
                        nextIndex = (currentIndex + 1) % superiorPanel.subpanels2.Length;
                    }
                    superiorPanel.subpanels2[nextIndex].gameObject.SetActive(true);
                    superiorPanel.subpanels2[currentIndex].gameObject.SetActive(false);
                }
                break;
            case "previous2":
                if (!locked)
                {
                    int currentIndex = -this.id - 1;
                    int nextIndex = 0;
                    if (this.id == -1)
                    {
                        currentIndex = 0;
                        nextIndex = superiorPanel.subpanels.Length - 1;
                    }
                    else
                    {
                        nextIndex = (currentIndex - 1) % superiorPanel.subpanels2.Length;
                    }
                    if (nextIndex < 0) nextIndex += superiorPanel.subpanels2.Length;
                    superiorPanel.subpanels2[nextIndex].gameObject.SetActive(true);
                    superiorPanel.subpanels2[currentIndex].gameObject.SetActive(false);
                }
                break;
            case "next3":
                if (!locked)
                {
                    int currentIndex = -this.id - 1;
                    int nextIndex = 0;
                    if (this.id == -1)
                    {
                        currentIndex = 0;
                        nextIndex = 1;
                    }
                    else
                    {
                        nextIndex = (currentIndex + 1) % superiorPanel.subpanels.Length;
                    }
                    superiorPanel.subpanels3[nextIndex].gameObject.SetActive(true);
                    superiorPanel.subpanels3[currentIndex].gameObject.SetActive(false);
                }
                break;
            case "previous3":
                if (!locked)
                {
                    int currentIndex = -this.id - 1;
                    int nextIndex = 0;
                    if (this.id == -1)
                    {
                        currentIndex = 0;
                        nextIndex = superiorPanel.subpanels.Length - 1;
                    }
                    else
                    {
                        nextIndex = (currentIndex - 1) % superiorPanel.subpanels.Length;
                    }
                    if (nextIndex < 0) nextIndex += superiorPanel.subpanels.Length;
                    superiorPanel.subpanels3[nextIndex].gameObject.SetActive(true);
                    superiorPanel.subpanels3[currentIndex].gameObject.SetActive(false);
                }
                break;
            case "nextHP":
                if (!locked)
                {
                    FancyList fl = GetComponentInChildren<FancyList>();
                    fl.offset += GetComponentInChildren<FancyList>().maxLines;
                    GetComponentInChildren<FancyList>().GetUpdatedValues();
                }
                break;
            case "previousHP":
                if (!locked)
                {
                    FancyList fl = GetComponentInChildren<FancyList>();
                    int maxLines = fl.maxLines;
                    fl.offset -= maxLines;
                    if (fl.offset < 0 && fl.offset > -maxLines)
                        fl.offset = 0;
                    GetComponentInChildren<FancyList>().GetUpdatedValues();
                }
                break;
            case "RadarObjects":
                if (!locked)
                {
                    player.radarObjects = !player.radarObjects;
                }
                break;
            case "TraceRadar":
                if (!locked)
                {
                    player.traceRadar = !player.traceRadar;
                }
                break;
            case "TraceMissile":
                if (!locked)
                {
                    player.traceMissile = !player.traceMissile;
                    //Debug.Log("TraceMissile");
                }
                break;
            case "TraceGun":
                if (!locked)
                {
                    player.traceGun = !player.traceGun;
                    //Debug.Log("TraceGun");
                }
                break;
            case "dock":
                if (!locked)
                {
                    player.ship.DockSubShip();
                }
                break;
            case "eject_as_npc":
                if (!locked && IsConfigSubPanel())
                {
                    int slotIndex = GetConfigSlotIndex();
                    if (slotIndex >= 0)
                        player.ship.EjectSubShip(slotIndex, false);
                }
                break;
            case "eject_as_player":
                if (!locked && IsConfigSubPanel())
                {
                    int slotIndex = GetConfigSlotIndex();
                    if (slotIndex >= 0)
                        player.ship.EjectSubShip(slotIndex, true);
                }
                break;
            case "":
                if (!locked)
                {

                }
                break;
        }
    }

    void UpdateButtons()
    {
        if (player == null || player.ship == null)
            return;
        if (IsConfigSubPanel())
            UpdateConfigSubPanelTitle();
        foreach (Button b in GetComponentsInChildren<Button>())
        {
            TextMeshProUGUI t = b.GetComponentInChildren<TextMeshProUGUI>();
            if (t == null) continue;
            if (t.text.Contains("Recording:"))
            {
                t.text = "Recording: " + (Game.instance.record ? "ON" : "OFF");
            } else if (t.text.Contains("Reactor:"))
            {
                t.text = "Reactor: " + (player.ship.reactor.power ? "ON" : "OFF");
            }
            else if (t.text.Contains("Drive:"))
            {
                t.text = "Drive: " + (player.ship.reactor.drivePower ? "ON" : "OFF");
            }
            else if (t.text.Contains("Weapons:"))
            {
                t.text = "Weapons: " + (player.ship.reactor.weaponPower ? "ON" : "OFF");
            }
            else if (t.text.Contains("Telemetry:"))
            {
                t.text = "Telemetry: " + (player.ship.reactor.telemetryPower ? "ON" : "OFF");
            }
            else if (t.text.Contains("Comms:"))
            {
                t.text = "Comms: " + (player.ship.reactor.commsPower ? "ON" : "OFF");
            }
            else if (t.text.Contains("Overclock:"))
            {
                t.text = "Overclock: " + (player.ship.reactor.highPower ? "ON" : "OFF");
            }
            else if (t.text.Contains("Low Power:"))
            {
                t.text = "Low Power: " + (player.ship.reactor.lowPower ? "ON" : "OFF");
            }
            else if (t.text.Contains("Batteries:"))
            {
                t.text = "Batteries: " + (player.ship.reactor.batteryPower ? "ON" : "OFF");
            }
            else if (t.text.Contains("Fire Turrets:"))
            {
                GameObject[] indi = GameObject.FindGameObjectsWithTag("UI Circle");
                int index = -this.id - 1 + radarListStart;
                GameObject currentTarget;
                if (TryGetValidRadarTarget(indi, index, out currentTarget))
                {
                    if (player.ship.turrets[0].targets.ContainsKey(currentTarget))
                    {
                        t.text = "Fire Turrets: ON";
                    } else
                    {
                        t.text = "Fire Turrets: OFF";
                    }
                }
            }
            else if (t.text.Contains("Point Towards:"))
            {
                GameObject[] indi = GameObject.FindGameObjectsWithTag("UI Circle");
                int index = -this.id - 1 + radarListStart;
                GameObject currentTarget;
                if (TryGetValidRadarTarget(indi, index, out currentTarget))
                {
                    if (player.ship.pointTo == currentTarget.gameObject)
                        t.text = "Point Towards: ON";
                    else
                        t.text = "Point Towards: OFF";
                }
            }
            else if (t.text.Contains("Camera Follows Missile:"))
            {
                t.text = "Camera Follows Missile: " + (player.missileCam ? "ON" : "OFF");
            }
            else if (t.text.Contains("Manual Missile Control:"))
            {
                t.text = "Manual Missile Control: " + (player.missileControl ? "ON" : "OFF");
            }
            else if (t.text.Contains("Radar Objects"))
            {
                t.text = "Radar Objects: " + (player.radarObjects ? "ON" : "OFF");
            }
            else if (t.text.Contains("Shoot Incoming:") && player.ship.turrets.Length > 0)
            {
                if (this.id == -1)
                {
                    t.text = "Shoot Incoming: " + (player.ship.turrets[0].defend ? "ON" : "OFF");
                }
                else
                {
                    int currentIndex = -this.id - 2;
                    if (currentIndex >= 0 && currentIndex <= player.ship.turrets.Length)
                        t.text = "Shoot Incoming: " + (player.ship.turrets[currentIndex].defend ? "ON" : "OFF");
                    else
                        t.text = "Shoot Incoming: ---";
                }
            }
            else if (t.text.Contains("Intercept Incoming:") && player.ship.launchers.Length > 0)
            {
                if (this.id == -1)
                {
                    t.text = "Intercept Incoming: " + (player.ship.launchers[0].defend ? "ON" : "OFF");
                }
                else
                {
                    int currentIndex = -this.id - 2;
                    t.text = "Intercept Incoming: " + (player.ship.launchers[currentIndex].defend ? "ON" : "OFF");
                }
            }
            else if (t.text.Contains("Trace Radar:"))
            {
                t.text = "Trace Radar: " + (player.traceRadar ? "ON" : "OFF");
            }
            else if (t.text.Contains("Trace Missiles"))
            {
                t.text = "Trace Missiles: " + (player.traceMissile ? "ON" : "OFF");
            }
            else if (t.text.Contains("Gun Lasers"))
            {
                t.text = "Gun Lasers: " + (player.traceGun ? "ON" : "OFF");
            }
            else if (t.text.Contains("Stabalizers:"))
            {
                t.text = "Stabalizers: " + (player.ship.stabalizers ? "ON" : "OFF");
            }
            else if (t.text.Contains("Automatic Brakes:"))
            {
                t.text = "Automatic Brakes: " + (player.ship.autoBrakes ? "ON" : "OFF");
            }
            else if (t.text.Contains("Stabalizer Thrust:"))
            {
                t.text = "Stabalizer Thrust: " + (player.ship.useStabalizersForMainThrust ? "ON" : "OFF");
            }
            else if (t.text.Contains("Main Drive Stabalizer:"))
            {
                t.text = "Main Drive Stabalizer: " + (player.ship.useMainDriveForStabalization ? "ON" : "OFF");
            }
            else if (t.text.Contains("Flip and Burn:"))
            {
                t.text = "Flip and Burn: " + (player.ship.flip_n_burn ? "ON" : "OFF");
            }
            else if (t.text.Contains("Flashlights:") && this.id < 0 && player.ship.turrets.Length > 0)
            {
                //Debug.Log("ID = " + this.id);
                if (this.id == -1)
                {
                    t.text = "Flashlights: " + (player.ship.turrets[0].flashLightStatus() ? "ON" : "OFF");
                }
                else
                {
                    int currentIndex = -this.id - 2;
                    t.text = "Flashlights: " + (player.ship.turrets[currentIndex].flashLightStatus() ? "ON" : "OFF");
                }
            }
            else if (t.text.Contains("Turret Fire Mode:") && this.id < 0 && player.ship.turrets.Length > 0)
            {
                int currentIndex = -this.id - 2;
                if (this.id == -1)
                    currentIndex = 0;
                if (player.ship.turrets[currentIndex].fireMode == 0)
                    t.text = "Turret Fire Mode: AUTO";
                else if (player.ship.turrets[currentIndex].fireMode == 1)
                    t.text = "Turret Fire Mode: MANUAL";
                else if (player.ship.turrets[currentIndex].fireMode == 2)
                    t.text = "Turret Fire Mode: OFF";
            }
            else if (t.text.Contains("Cannon Fire Mode:") && this.id < 0 && player.ship.statGuns.Length > 0)
            {
                int currentIndex = -this.id - 2;
                if (this.id == -1)
                    currentIndex = 0;
                if (player.ship.statGuns[currentIndex].fireMode == 0)
                    t.text = "Cannon Fire Mode: AUTO";
                else if (player.ship.statGuns[currentIndex].fireMode == 1)
                    t.text = "Cannon Fire Mode: MANUAL";
                else if (player.ship.statGuns[currentIndex].fireMode == 2)
                    t.text = "Cannon Fire Mode: OFF";
            }
            else if (t.text.Contains("Aim Mode:") && this.id < 0 && player.ship.turrets.Length > 0)
            {
                int currentIndex = -this.id - 2;
                if (this.id == -1)
                    currentIndex = 0;
                if (player.ship.turrets[currentIndex].aimMode == 0)
                    t.text = "Aim Mode: AUTO";
                else if (player.ship.turrets[currentIndex].aimMode == 1)
                    t.text = "Aim Mode: MANUAL";
                else if (player.ship.turrets[currentIndex].aimMode == 2)
                    t.text = "Aim Mode: OFF";
            }
        }
    }

    public void PanelSlider(string id, float x)
    {
        if (player == null || player.ship == null)
            return;
        switch (id)
        {
            case "mainDrivePower":
                if (!locked)
                {
                    player.ship.mainDrivePower = x;
                }
                break;
            case "maneuveringPower":
                if (!locked)
                {
                    player.ship.maneuveringPower = x;
                }
                break;
            case "stabalizers":
                if (!locked)
                {
                    player.ship.stabalizerPower = x;
                }
                break;
            case "stabalizerTolerance":
                if (!locked)
                {
                    player.ship.speedTol = x;
                }
                break;
            case "stabalizerKp":
                if (!locked)
                {
                    player.ship.Kp = x;
                }
                break;
            case "navigationKp":
                if (!locked)
                {
                    player.ship.pointToKp = x;
                }
                break;
            case "navigationKd":
                if (!locked)
                {
                    player.ship.pointToKd = x;
                }
                break;
            case "aim_distance":
                if (!locked)
                {
                    player.ship.aimDistance = x;
                }
                break;
            case "":
                if (!locked)
                {

                }
                break;
        }
    }

    public string[] GetUpdatedListValue(string id)
    {
        if (player == null || player.ship == null)
            return null;
        switch (id)
        {
            case "healthlist":
                List<string> result = new List<string>();
                List<float> hp = new List<float>();
                //int cc = 0;
                foreach (ShipObject o in player.ship.GetComponentsInChildren<ShipObject>())
                {
                        if (hp.Count > 0)
                        {
                            float num = o.HP / o.maxHP;
                            bool inserted = false;
                            for (int i = 0; i < hp.Count; i++)
                            {
                                if (num <= hp[i])
                                {
                                    hp.Insert(i, num);
                                    result.Insert(i, (o.HP / o.maxHP < 0.3f ? "%r" : (o.HP / o.maxHP < 0.6f ? "%y" : "")) + o.name + ": " + Mathf.Round(o.HP) + "/" + o.maxHP);
                                    inserted = true;
                                    break;
                                }
                            }
                            if (!inserted)
                            {
                                hp.Add(o.HP / o.maxHP);
                                result.Add((o.HP / o.maxHP < 0.3f ? "%r" : (o.HP / o.maxHP < 0.6f ? "%y" : "")) + o.name + ": " + Mathf.Round(o.HP) + "/" + o.maxHP);
                            }
                        }
                        else
                        {
                            hp.Add(o.HP / o.maxHP);
                            result.Add((o.HP / o.maxHP < 0.3f ? "%r" : (o.HP / o.maxHP < 0.6f ? "%y" : "")) + o.name + ": " + Mathf.Round(o.HP) + "/" + o.maxHP);
                        }
                }
                return result.ToArray();
        }
        return null;
    }

    public float GetUpdatedSlider(string id)
    {
        if (player == null || player.ship == null)
            return 0;
        switch (id)
        {
            case "mainDrivePower":
                return player.ship.mainDrivePower;
            case "maneuveringPower":
                return player.ship.maneuveringPower;
            case "stabalizers":
                return player.ship.stabalizerPower;
            case "stabalizerTolerance":
                return player.ship.speedTol;
            case "stabalizerKp":
                return player.ship.Kp;
            case "navigationKp":
                return player.ship.pointToKp;
            case "navigationKd":
                return player.ship.pointToKd;
            case "aim_distance":
                return player.ship.aimDistance;
            case "":
                return 0;
        }
        return 0;
    }

    public string GetUpdatedTextScopeValue(string id)
    {
        if (player == null || player.ship == null)
            return "???";
        switch (id)
        {
            case "radar_target_lock":
                GameObject[] indi = GameObject.FindGameObjectsWithTag("UI Circle");
                int index = -this.id - 1 + radarListStart;
                GameObject currentTarget;
                if (TryGetValidRadarTarget(indi, index, out currentTarget))
                {
                    foreach (GameObject o in GameObject.FindGameObjectsWithTag("UI Target"))
                    {
                        if (o.GetComponent<Indicator>().target == currentTarget)
                        {
                            return "targeting";
                        }
                    }
                }
                return "---";
        }
        return "???";
    }

    public float GetUpdatedValue(string id)
    {
        if (player == null || player.ship == null)
            return 0;
        switch (id)
        {
            case "core_size":
                return player.ship.reactor.coreSize;
            case "battery_percentage":
                return 100 * player.ship.reactor.batteryAmount / player.ship.reactor.batteryCapacity;
            case "electric_noise":
                return player.ship.electricNoise;
            case "reactor_stability":
                return player.ship.reactor.reactorStability;
            case "ship_velocity_magnitude":
                if (player.ship.rb == null) return 0;
                return player.ship.rb.velocity.magnitude;
            case "ship_acceleration_magnitude":
                if (player.ship.rb == null) return 0;
                return player.ship.acceleration.magnitude;
            case "launcher_health":
                if (player.ship.launchers.Length == 0) return 0;
                if (this.id == -1)
                {
                    float result = 0;
                    foreach (MissileLauncher l in player.ship.launchers)
                        result += l.GetHP();
                    return result;
                } else
                {
                    int currentIndex = -this.id - 2;
                    return player.ship.launchers[currentIndex].GetHP();
                }
            case "turret_health":
                if (player.ship.turrets.Length == 0) return 0;
                if (this.id == -1)
                {
                    float result = 0;
                    foreach (gun g in player.ship.turrets)
                        result += g.GetHP();
                    return result;
                } else
                {
                    int currentIndex = -this.id - 2;
                    return player.ship.turrets[currentIndex].GetHP();
                }
            case "cannon_health":
                if (player.ship.statGuns.Length == 0) return 0;
                if (this.id == -1)
                {
                    float result = 0;
                    foreach (gun g in player.ship.statGuns)
                        result += g.GetHP();
                    return result;
                }
                else
                {
                    int currentIndex = -this.id - 2;
                    return player.ship.statGuns[currentIndex].GetHP();
                }
            case "launcher_ammo":
                if (player.ship.launchers.Length == 0) return 0;
                if (this.id == -1)
                {
                    float result = 0;
                    foreach (MissileLauncher l in player.ship.launchers)
                        result += l.ammo;
                    return result;
                } else
                {
                    int currentIndex = -this.id - 2;
                    return player.ship.turrets[currentIndex].ammo;
                }
            case "turret_ammo":
                if (player.ship.turrets.Length == 0) return 0;
                if (this.id == -1)
                {
                    float result = 0;
                    foreach (gun g in player.ship.turrets)
                        result += g.ammo;
                    return result;
                }
                else
                {
                    int currentIndex = -this.id - 2;
                    return player.ship.turrets[currentIndex].ammo;
                }
            case "cannon_ammo":
                if (player.ship.statGuns.Length == 0) return 0;
                if (this.id == -1)
                {
                    float result = 0;
                    foreach (gun g in player.ship.statGuns)
                        result += g.ammo;
                    return result;
                }
                else
                {
                    int currentIndex = -this.id - 2;
                    return player.ship.statGuns[currentIndex].ammo;
                }
            case "subship_health":
                {
                    SpaceShip sub = GetConfigSubShip();
                    if (sub == null)
                        return 0;
                    float hp = 0;
                    float maxHp = 0;
                    if (sub.hull != null)
                    {
                        foreach (ShipObject h in sub.hull)
                        {
                            if (h == null) continue;
                            hp += Mathf.Max(0, h.HP);
                            maxHp += Mathf.Max(0.001f, h.maxHP);
                        }
                    }
                    if (sub.reactor != null)
                    {
                        hp += Mathf.Max(0, sub.reactor.HP);
                        maxHp += Mathf.Max(0.001f, sub.reactor.maxHP);
                    }
                    if (maxHp <= 0.001f)
                        return 0;
                    return Mathf.Clamp(100f * hp / maxHp, 0, 100);
                }
            case "subship_ammo":
                {
                    SpaceShip sub = GetConfigSubShip();
                    if (sub == null)
                        return 0;
                    float ammo = 0;
                    float maxAmmo = 0;
                    if (sub.turrets != null)
                    {
                        foreach (gun g in sub.turrets)
                        {
                            if (g == null) continue;
                            ammo += Mathf.Max(0, g.ammo);
                            maxAmmo += Mathf.Max(0, g.maxAmmo);
                        }
                    }
                    if (sub.statGuns != null)
                    {
                        foreach (gun g in sub.statGuns)
                        {
                            if (g == null) continue;
                            ammo += Mathf.Max(0, g.ammo);
                            maxAmmo += Mathf.Max(0, g.maxAmmo);
                        }
                    }
                    if (sub.launchers != null)
                    {
                        foreach (MissileLauncher l in sub.launchers)
                        {
                            if (l == null) continue;
                            ammo += Mathf.Max(0, l.ammo);
                            maxAmmo += Mathf.Max(0, l.maxAmmo);
                        }
                    }
                    if (maxAmmo <= 0.001f)
                        return 0;
                    return Mathf.Clamp(100f * ammo / maxAmmo, 0, 100);
                }
            case "subship_battery":
                {
                    SpaceShip sub = GetConfigSubShip();
                    if (sub == null || sub.reactor == null || sub.reactor.batteryCapacity <= 0)
                        return 0;
                    return Mathf.Clamp(100f * sub.reactor.batteryAmount / sub.reactor.batteryCapacity, 0, 100);
                }
        }
        return 0;
    }
}
