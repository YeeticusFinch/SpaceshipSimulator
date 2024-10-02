using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class Recorder : MonoBehaviour
{

    [NonSerialized]
    public float startTime;
    
    public List<ShipLog> shipLogs = new List<ShipLog>();
    public List<MissileLog> missileLogs = new List<MissileLog>();
    public List<ObjectLog> objectLogs = new List<ObjectLog>();

    Dictionary<int, string> objects = new Dictionary<int, string>();
    Dictionary<int, float> objectSpawnTime = new Dictionary<int, float>();
    Dictionary<int, GameObject> obj = new Dictionary<int, GameObject>();

    public bool paused = false;
    public float elapsedTime = 0;
    public float playbackSpeed = 1f;

    public IEnumerator Play()
    {
        bool playing = true;
        float delay = 0.2f;
        int shipLogIndex = 0;
        int missileLogIndex = 0;
        int objectLogIndex = 0;
        foreach (int i in objects.Keys)
        {
            if (objects[i] != null && objects[i].Length > 0) {
                try
                {
                    obj.Add(i, GameObject.Instantiate(Resources.Load(objects[i], typeof(GameObject))) as GameObject);
                    obj[i].SetActive(false);
                } catch (Exception e)
                {
                    Debug.Log("Can't find prefab " + objects[i]);
                    Debug.Log(e);
                }
            }
        }
        Debug.Log("obj\nKeys=" + Yeet.PrintCol(obj.Keys) + "\nVals=" + Yeet.PrintCol(obj.Values));
        yield return new WaitForSeconds(1);
        //Debug.Log("obj\nKeys=" + Yeet.PrintCol(obj.Keys) + "\nVals=" + Yeet.PrintCol(obj.Values));
        while (playing)
        {
            if (paused)
            {
                yield return new WaitForSeconds(0.5f);
            } else
            {
                if (objectSpawnTime != null && objectSpawnTime.Count > 0)
                {
                    List<int> delete = new List<int>();
                    foreach (int i in objectSpawnTime.Keys)
                    {
                        if (elapsedTime > objectSpawnTime[i])
                        {
                            obj[i].SetActive(true);
                            delete.Add(i);
                        }
                    }
                    foreach (int i in delete)
                    {
                        objectSpawnTime.Remove(i);
                    }
                }
                if (shipLogs.Count > 0)
                {
                    while (objectLogIndex < objectLogs.Count && elapsedTime > objectLogs[objectLogIndex].time)
                    {
                        //Debug.Log("Object Log = " + objectLogIndex);
                        ObjectLog log = objectLogs[objectLogIndex];
                        bool activeState = obj[log.id].activeSelf;
                        obj[log.id].SetActive(true);
                        SpaceObject o = obj[log.id].GetComponent<SpaceObject>();
                        if (o == null)
                        {
                            Debug.Log("Object of ID " + log.id + " isn't a SpaceObject");
                        } else
                        {
                            try
                            {
                                if (log.position != null && log.position.Length > 0)
                                {
                                    o.LerpPos(new Vector3(log.position[0], log.position[1], log.position[2]));
                                }

                                if (log.velocity != null && log.velocity.Length > 0)
                                {
                                    o.rb.velocity = (new Vector3(log.velocity[0], log.velocity[1], log.velocity[2])) * playbackSpeed;
                                }

                                if (log.angularVelocity != null && log.angularVelocity.Length > 0)
                                {
                                    o.rb.angularVelocity = (new Vector3(log.angularVelocity[0], log.angularVelocity[1], log.angularVelocity[2])) * playbackSpeed;
                                }
                                if (log.eulerAngle != null && log.eulerAngle.Length > 0)
                                {
                                    o.transform.eulerAngles = (new Vector3(log.eulerAngle[0], log.eulerAngle[1], log.eulerAngle[2]));
                                }
                            } catch (Exception e)
                            {
                                Debug.Log("Error in SpaceObject ID=" + log.id);
                                Debug.Log(e);
                            }
                        }

                        obj[log.id].SetActive(activeState);
                        objectLogIndex++;
                    }
                    while (shipLogIndex < shipLogs.Count && elapsedTime > shipLogs[shipLogIndex].time)
                    {
                        //Debug.Log("Ship Log = " + shipLogIndex);
                        ShipLog log = shipLogs[shipLogIndex];
                        bool activeState = obj[log.id].activeSelf;
                        obj[log.id].SetActive(true);
                        SpaceShip ship = obj[log.id].GetComponent<SpaceShip>();
                        if (ship != null)
                        {
                            try
                            {
                                if (log.foldTurrets != null && log.foldTurrets.Length > 0)
                                    foreach (int i in log.foldTurrets)
                                        ship.turrets[i].fold = true;

                                if (log.unfoldTurrets != null && log.unfoldTurrets.Length > 0)
                                    foreach (int i in log.unfoldTurrets)
                                        ship.turrets[i].fold = false;

                                if (log.launchMissiles != null && log.launchMissiles.Length > 0)
                                    foreach (int i in log.launchMissiles)
                                        ship.launchers[i].Launch(null);

                                if (log.position != null && log.position.Length > 0)
                                {
                                    ship.LerpPos(new Vector3(log.position[0], log.position[1], log.position[2]));
                                }

                                if (log.velocity != null && log.velocity.Length > 0)
                                {
                                    ship.rb.velocity = (new Vector3(log.velocity[0], log.velocity[1], log.velocity[2])) * playbackSpeed;
                                }

                                if (log.angularVelocity != null && log.angularVelocity.Length > 0)
                                {
                                    ship.rb.angularVelocity = (new Vector3(log.angularVelocity[0], log.angularVelocity[1], log.angularVelocity[2])) * playbackSpeed;
                                }
                                if (log.eulerAngle != null && log.eulerAngle.Length > 0)
                                {
                                    ship.transform.eulerAngles = (new Vector3(log.eulerAngle[0], log.eulerAngle[1], log.eulerAngle[2]));
                                }

                                if (log.startShootingTurrets != null && log.startShootingTurrets.Length > 0)
                                    foreach (int i in log.startShootingTurrets)
                                        ship.turrets[i].Shoot();

                                if (log.stopShootingTurrets != null && log.stopShootingTurrets.Length > 0)
                                    foreach (int i in log.stopShootingTurrets)
                                        ship.turrets[i].StopShooting();

                                if (log.startThrusters != -1)
                                {
                                    Thruster[] thrusters = ship.GetThrustersFromID(log.startThrusters);
                                    if (thrusters == null)
                                        Debug.Log("Couldn't find thrusters of id " + log.startThrusters);
                                    else
                                        ship.FireThrusters(thrusters, log.thrusterPower < 0 ? 1 : log.thrusterPower);
                                }

                                if (log.stopThrusters != -1)
                                {
                                    Thruster[] thrusters = ship.GetThrustersFromID(log.stopThrusters);
                                    if (thrusters == null)
                                        Debug.Log("Couldn't find thrusters of id " + log.stopThrusters);
                                    else
                                        ship.StopThrusters(thrusters);
                                }

                                if (log.destroyChild != null && log.destroyChild.Length > 0)
                                {
                                    Transform curr = ship.transform;
                                    foreach (int i in log.destroyChild)
                                    {
                                        curr = curr.GetChild(i);
                                        if (curr == null)
                                        {
                                            Debug.Log("Can't find child " + i + " from indices " + log.destroyChild);
                                            break;
                                        }
                                    }

                                    if (curr != null)
                                    {
                                        curr.gameObject.SetActive(false);
                                    }
                                }

                                if (log.setTurretRot != null && log.setTurretRot.Length > 0)
                                {
                                    //float[] setTurretRot = new float[] { turretIndex, currentRot.x, currentRot.y, currentRot.z, targetRot.x, targetRot.y, targetRot.z };
                                    int index = (int)log.setTurretRot[0];
                                    ship.turrets[index].turretRot = new Vector3(log.setTurretRot[1], log.setTurretRot[2], log.setTurretRot[3]);
                                    ship.turrets[index].turretRotTarget = new Vector3(log.setTurretRot[4], log.setTurretRot[5], log.setTurretRot[6]);
                                }
                            } catch (Exception e)
                            {
                                Debug.Log("Error with Ship ID=" + log.id);
                                Debug.Log(e);
                            }
                        } else
                            Debug.Log("Object " + log.id + " isn't a SpaceShip");

                        obj[log.id].SetActive(activeState);
                        shipLogIndex++;
                    }
                    while (missileLogIndex < missileLogs.Count && elapsedTime > missileLogs[missileLogIndex].time)
                    {
                        //Debug.Log("Missile Log = " + missileLogIndex);
                        MissileLog log = missileLogs[missileLogIndex];
                        //Debug.Log("obj\nKeys=" + Yeet.PrintCol(obj.Keys) + "\nVals=" + Yeet.PrintCol(obj.Values));
                        bool activeState = obj[log.id].activeSelf;
                        obj[log.id].SetActive(true);
                        Missile m = obj[log.id].GetComponent<Missile>();
                        if (m == null)
                        {
                            Debug.Log("Object of ID " + log.id + " isn't a Missile");
                        }
                        else
                        {
                            try
                            {
                                if (log.position != null && log.position.Length > 0)
                                {
                                    m.LerpPos(new Vector3(log.position[0], log.position[1], log.position[2]));
                                }

                                if (log.velocity != null && log.velocity.Length > 0)
                                {
                                    m.rb.velocity = (new Vector3(log.velocity[0], log.velocity[1], log.velocity[2])) * playbackSpeed;
                                }

                                if (log.angularVelocity != null && log.angularVelocity.Length > 0)
                                {
                                    m.rb.angularVelocity = (new Vector3(log.angularVelocity[0], log.angularVelocity[1], log.angularVelocity[2])) * playbackSpeed;
                                }
                                if (log.eulerAngle != null && log.eulerAngle.Length > 0)
                                {
                                    m.transform.eulerAngles = (new Vector3(log.eulerAngle[0], log.eulerAngle[1], log.eulerAngle[2]));
                                }

                                if (log.die)
                                    m.gameObject.SetActive(false);

                                if (log.explode)
                                    m.Explode();

                                if (log.startThrusters != -1)
                                {
                                    Thruster[] thrusters = m.GetThrustersFromID(log.startThrusters);
                                    if (thrusters == null)
                                        Debug.Log("Couldn't find thrusters of id " + log.startThrusters);
                                    else
                                        m.FireThrusters(thrusters, log.thrusterPower < 0 ? 1 : log.thrusterPower);
                                }

                                if (log.stopThrusters != -1)
                                {
                                    Thruster[] thrusters = m.GetThrustersFromID(log.stopThrusters);
                                    if (thrusters == null)
                                        Debug.Log("Couldn't find thrusters of id " + log.stopThrusters);
                                    else
                                        m.StopThrusters(thrusters);
                                }
                            } catch (Exception e)
                            {
                                Debug.Log("Error with missile ID=" + log.id);
                                Debug.Log(e);
                            }
                        }

                        obj[log.id].SetActive(activeState);
                        missileLogIndex++;
                    }
                }
                yield return new WaitForSeconds(delay*playbackSpeed);
                elapsedTime += delay*playbackSpeed;
            }
        }
    }

    public void SetObjects(string[] s)
    {
        for (int i = 0; i < s.Length; i++)
        {
            objects.Add(i, s[i]);
        }
    }

    public void SetObjectSpawnTimes(float[] s)
    {
        for (int i = 0; i < s.Length; i++)
        {
            objectSpawnTime.Add(i, s[i]);
        }
    }

    public string[] GetObjects()
    {
        string[] result = new string[objects.Count];
        for (int i = 0; i < result.Length; i++)
        {
            result[i] = objects[i];
        }
        return result;
    }

    public float[] GetObjectSpawnTimes()
    {
        float[] result = new float[objectSpawnTime.Count];
        for (int i = 0; i < result.Length; i++)
        {
            result[i] = objectSpawnTime[i];
        }
        return result;
    }

    [System.Serializable]
    public struct ShipLog
    {
        public ShipLog(float time, int id, int[] unfoldTurrets, int[] foldTurrets, int[] startShootingTurrets, int[] stopShootingTurrets, int[] launchMissiles, int startThrusters, int stopThrusters, float[] position, float[] velocity, float[] eulerAngle, float[] angularVelocity, int[] destroyChild, float[] setTurretRot, float thrusterPower)
        {
            this.time = time;
            this.id = id;
            this.unfoldTurrets = unfoldTurrets;
            this.foldTurrets = foldTurrets;
            this.unfoldTurrets = unfoldTurrets;
            this.startShootingTurrets = startShootingTurrets;
            this.stopShootingTurrets = stopShootingTurrets;
            this.startThrusters = startThrusters;
            this.stopThrusters = stopThrusters;
            this.launchMissiles = launchMissiles;
            this.position = position;
            this.velocity = velocity;
            this.eulerAngle = eulerAngle;
            this.angularVelocity = angularVelocity;
            this.destroyChild = destroyChild;
            this.setTurretRot = setTurretRot;
            this.thrusterPower = thrusterPower;
        }

        public float time;
        public int id;
        public int[] unfoldTurrets;
        public int[] foldTurrets;
        public int[] startShootingTurrets;
        public int[] stopShootingTurrets;
        public int[] launchMissiles;
        public int startThrusters;
        public int stopThrusters;
        public float[] position;
        public float[] velocity;
        public float[] eulerAngle;
        public float[] angularVelocity;
        public int[] destroyChild;
        public float[] setTurretRot;
        public float thrusterPower;
    }

    [System.Serializable]
    public struct MissileLog
    {
        public MissileLog(float time, int id, int startThrusters, int stopThrusters, float[] position, float[] velocity, float[] eulerAngle, float[] angularVelocity, bool die, bool explode, float thrusterPower)
        {
            this.time = time;
            this.id = id;;
            this.startThrusters = startThrusters;
            this.stopThrusters = stopThrusters;
            this.position = position;
            this.velocity = velocity;
            this.eulerAngle = eulerAngle;
            this.angularVelocity = angularVelocity;
            this.die = die;
            this.explode = explode;
            this.thrusterPower = thrusterPower;
        }

        public float time;
        public int id;
        public int startThrusters;
        public int stopThrusters;
        public float[] position;
        public float[] velocity;
        public float[] eulerAngle;
        public float[] angularVelocity;
        public bool die;
        public bool explode;
        public float thrusterPower;
    }

    [System.Serializable]
    public struct ObjectLog
    {
        public ObjectLog(float time, int id, float[] position, float[] velocity, float[] eulerAngle, float[] angularVelocity, bool die)
        {
            this.time = time;
            this.id = id;
            this.position = position;
            this.velocity = velocity;
            this.eulerAngle = eulerAngle;
            this.angularVelocity = angularVelocity;
            this.die = die;
        }

        public float time;
        public int id;

        public float[] position;
        public float[] velocity;
        public float[] eulerAngle;
        public float[] angularVelocity;

        public bool die;
    }

    // Start is called before the first frame update
    void Start()
    {
        startTime = GetSeconds();
    }

    float GetSeconds()
    {
        float hour = System.DateTime.Now.Hour * 3600;
        float minute = System.DateTime.Now.Minute * 60;
        float second = System.DateTime.Now.Second;
        float milli = System.DateTime.Now.Millisecond * 0.001f;
        return hour + minute + second + milli;
    }

    float GetTime()
    {
        return GetSeconds() - startTime;
    }

    public void LogMissileLaunch(SpaceShip ship, Missile missile, int launcherIndex)
    {
        ObjLog(ship);
        ObjLog(missile);
        Vector3 pos = ship.rb.position;
        Vector3 vel = ship.rb.velocity;
        Vector3 angle = ship.transform.eulerAngles;
        Vector3 angleVel = ship.rb.angularVelocity;
        float[] position = vectorToArray(pos);
        float[] velocity = vectorToArray(vel);
        float[] eulerAngle = vectorToArray(angle);
        float[] angularVelocity = vectorToArray(angleVel);
        int id = ship.objectID;
        shipLogs.Add(new ShipLog(GetTime(), id, null, null, null, null, new int[] { launcherIndex }, -1, -1, position, velocity, eulerAngle, angularVelocity, null, null, -1));

    }

    public void ObjLog(SpaceObject obj)
    {
        if (!objects.ContainsKey(obj.objectID))
        {
            Debug.Log("Adding new Object " + obj.objectID + ": " + obj.prefabPath);
            objects.Add(obj.objectID, obj.prefabPath);
            objectSpawnTime.Add(obj.objectID, GetTime());
        }
    }

    public void LogTurret(SpaceShip ship, int turretIndex, bool startShooting, bool stopShooting, bool startFold, bool startUnfold)
    {
        ObjLog(ship);
        gun g = ship.turrets[turretIndex];
        int id = ship.objectID;
        Vector3 currentRot = g.turretRot;
        Vector3 targetRot = g.turretRotTarget;

        Vector3 pos = ship.rb.position;
        Vector3 vel = ship.rb.velocity;
        Vector3 angle = ship.transform.eulerAngles;
        Vector3 angleVel = ship.rb.angularVelocity;
        float[] position = vectorToArray(pos);
        float[] velocity = vectorToArray(vel);
        float[] eulerAngle = vectorToArray(angle);
        float[] angularVelocity = vectorToArray(angleVel);

        float[] setTurretRot = new float[] { turretIndex, currentRot.x, currentRot.y, currentRot.z, targetRot.x, targetRot.y, targetRot.z };

        shipLogs.Add(new ShipLog(GetTime(), id, startUnfold ? new int[] { turretIndex } : null, startFold ? new int[] { turretIndex } : null, startShooting ? new int[] { turretIndex } : null, stopShooting ? new int[] { turretIndex } : null, null, -1, -1, position, velocity, eulerAngle, angularVelocity, null, setTurretRot, -1));
    }
    
    public void LogTurretGroup(SpaceShip ship, int[] turretIndices, bool startShooting, bool stopShooting, bool startFold, bool startUnfold)
    {
        ObjLog(ship);
        int id = ship.objectID;
        Vector3 pos = ship.rb.position;
        Vector3 vel = ship.rb.velocity;
        Vector3 angle = ship.transform.eulerAngles;
        Vector3 angleVel = ship.rb.angularVelocity;
        float[] position = vectorToArray(pos);
        float[] velocity = vectorToArray(vel);
        float[] eulerAngle = vectorToArray(angle);
        float[] angularVelocity = vectorToArray(angleVel);

        shipLogs.Add(new ShipLog(GetTime(), id, startUnfold ? turretIndices : null, startFold ? turretIndices : null, startShooting ? turretIndices : null, stopShooting ? turretIndices : null, null, -1, -1, position, velocity, eulerAngle, angularVelocity, null, null, -1));
    }

    public void KillObject(SpaceObject obj)
    {
        ObjLog(obj);
        int id = obj.objectID;
        Vector3 pos = obj.rb.position;
        Vector3 vel = obj.rb.velocity;
        Vector3 angle = obj.transform.eulerAngles;
        Vector3 angleVel = obj.rb.angularVelocity;
        float[] position = vectorToArray(pos);
        float[] velocity = vectorToArray(vel);
        float[] eulerAngle = vectorToArray(angle);
        float[] angularVelocity = vectorToArray(angleVel);

        objectLogs.Add(new ObjectLog(GetTime(), id, position, velocity, eulerAngle, angularVelocity, true));
    }

    public void KillChild(SpaceShip ship, GameObject child)
    {
        ObjLog(ship);
        List<int> indices = new List<int>();
        Transform curr = child.transform;
        while (curr != ship.transform)
        {
            if (indices.Count == 0)
                indices.Add(curr.GetSiblingIndex());
            else
                indices.Insert(0, curr.GetSiblingIndex());
            curr = curr.parent;
        }

        int id = ship.objectID;
        Vector3 pos = ship.rb.position;
        Vector3 vel = ship.rb.velocity;
        Vector3 angle = ship.transform.eulerAngles;
        Vector3 angleVel = ship.rb.angularVelocity;
        float[] position = vectorToArray(pos);
        float[] velocity = vectorToArray(vel);
        float[] eulerAngle = vectorToArray(angle);
        float[] angularVelocity = vectorToArray(angleVel);

        shipLogs.Add(new ShipLog(GetTime(), id, null, null, null, null, null, -1, -1, position, velocity, eulerAngle, angularVelocity, indices.ToArray(), null, -1));

    }

    public void LogThrust(SpaceShip ship, Thruster[] thrusters, bool thrusting, float power)
    {
        //Debug.Log("Thrusters: " + thrusters);
        ObjLog(ship);
        //string thrusterGroupName = GetMemberName(() => thrusters);
        //string thrusterGroupName = thrusters;// nameof(thrusters);
        int thrusterGroupNumber = ship.GetThrusterGroupNumber(thrusters);
        Vector3 pos = ship.rb.position;
        Vector3 vel = ship.rb.velocity;
        Vector3 angle = ship.transform.eulerAngles;
        Vector3 angleVel = ship.rb.angularVelocity;
        float[] position = vectorToArray(pos);
        float[] velocity = vectorToArray(vel);
        float[] eulerAngle = vectorToArray(angle);
        float[] angularVelocity = vectorToArray(angleVel);
        int id = ship.objectID;
        if (thrusting)
            shipLogs.Add(new ShipLog(GetTime(), id, null, null, null, null, null, thrusterGroupNumber, -1, position, velocity, eulerAngle, angularVelocity, null, null, power));
        else
            shipLogs.Add(new ShipLog(GetTime(), id, null, null, null, null, null, -1, thrusterGroupNumber, position, velocity, eulerAngle, angularVelocity, null, null, power));
    }

    public void LogMovement(SpaceShip ship)
    {
        ObjLog(ship);
        Vector3 pos = ship.rb.position;
        Vector3 vel = ship.rb.velocity;
        Vector3 angle = ship.transform.eulerAngles;
        Vector3 angleVel = ship.rb.angularVelocity;
        float[] position = vectorToArray(pos);
        float[] velocity = vectorToArray(vel);
        float[] eulerAngle = vectorToArray(angle);
        float[] angularVelocity = vectorToArray(angleVel);
        int id = ship.objectID;
        shipLogs.Add(new ShipLog(GetTime(), id, null, null, null, null, null, -1, -1, position, velocity, eulerAngle, angularVelocity, null, null, -1));
    }

    public void LogThrust(Missile missile, Thruster[] thrusters, bool thrusting, float power)
    {
        //Debug.Log("Missile Thrusters: " + thrusters);
        ObjLog(missile);
        //string thrusterGroupName = GetMemberName(() => thrusters);
        //string thrusterGroupName = thrusters;// nameof(thrusters);
        int thrusterGroupNumber = missile.GetThrusterGroupNumber(thrusters);
        Vector3 pos = missile.rb.position;
        Vector3 vel = missile.rb.velocity;
        Vector3 angle = missile.transform.eulerAngles;
        Vector3 angleVel = missile.rb.angularVelocity;
        float[] position = vectorToArray(pos);
        float[] velocity = vectorToArray(vel);
        float[] eulerAngle = vectorToArray(angle);
        float[] angularVelocity = vectorToArray(angleVel);
        int id = missile.objectID;
        if (thrusting)
            missileLogs.Add(new MissileLog(GetTime(), id, thrusterGroupNumber, -1, position, velocity, eulerAngle, angularVelocity, false, false, power));
        else
            missileLogs.Add(new MissileLog(GetTime(), id, -1, thrusterGroupNumber, position, velocity, eulerAngle, angularVelocity, false, false, power));
    }

    public void LogMovement(Missile missile)
    {
        ObjLog(missile);
        Vector3 pos = missile.rb.position;
        Vector3 vel = missile.rb.velocity;
        Vector3 angle = missile.transform.eulerAngles;
        Vector3 angleVel = missile.rb.angularVelocity;
        float[] position = vectorToArray(pos);
        float[] velocity = vectorToArray(vel);
        float[] eulerAngle = vectorToArray(angle);
        float[] angularVelocity = vectorToArray(angleVel);
        int id = missile.objectID;
        missileLogs.Add(new MissileLog(GetTime(), id, -1, -1, position, velocity, eulerAngle, angularVelocity, false, false, -1));
    }

    public void LogObject(SpaceObject obj)
    {
        ObjLog(obj);
        Vector3 pos = obj.rb.position;
        Vector3 vel = obj.rb.velocity;
        Vector3 angle = obj.transform.eulerAngles;
        Vector3 angleVel = obj.rb.angularVelocity;
        float[] position = vectorToArray(pos);
        float[] velocity = vectorToArray(vel);
        float[] eulerAngle = vectorToArray(angle);
        float[] angularVelocity = vectorToArray(angleVel);
        int id = obj.objectID;
        missileLogs.Add(new MissileLog(GetTime(), id, -1, -1, position, velocity, eulerAngle, angularVelocity, false, false, -1));
    }

    float[] vectorToArray(Vector3 vec)
    {
        float[] arr = new float[3];
        arr[0] = vec.x;
        arr[1] = vec.y;
        arr[2] = vec.z;
        return arr;
    }

    public static string GetMemberName<T>(Expression<Func<T>> memberExpression)
    {
        MemberExpression expressionBody = (MemberExpression)memberExpression.Body;
        return expressionBody.Member.Name;
    }
}
