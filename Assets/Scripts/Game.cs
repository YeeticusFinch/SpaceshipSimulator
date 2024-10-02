using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Game : MonoBehaviour
{
    public static GameObject MAP;
    public static GameObject[] SHIPS;
    public static List<int> NPCs = new List<int>();
    public static List<int> ENEMIES = new List<int>();
    public static GameObject playerShip;
    public PlayerShip player;

    public static int mapNum = 0;

    public static Game instance;
    public int mapID = 0;

    public float gravity = -0.981f;
    public float maxShipSpeed = 25;
    public float maxMissileSpeed = 40;
    public float maxInterceptMissileSpeed = 48;
    public LayerMask radar_mask;
    public Recorder rec;
    public Material skyboxMaterial;
    public Light sunSource;
    public Texture reflections;
    public bool atmosphere = false;

    [System.NonSerialized]
    public bool timerGoing = false;
    [System.NonSerialized]
    public bool displayTimer = false;
    [System.NonSerialized]
    public float startTime = 0;
    [System.NonSerialized]
    public float stopTime = 0;


    public static LayerMask RadarMask;

    public GameObject DmgIndicator;
    public GameObject TargetDmgIndicator;

    public bool record = false;
    public bool playbackRecording = false;
    public int playbackNumber = 0;
    

    public void StartTimer()
    {
        displayTimer = true;
        timerGoing = true;
        startTime = Time.realtimeSinceStartup;
    }

    public void StopTimer()
    {
        timerGoing = false;
        stopTime = Time.realtimeSinceStartup;
    }

    public float GetTimer()
    {
        if (timerGoing)
            return Time.realtimeSinceStartup - startTime;
        else
            return stopTime - startTime;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (mapNum != mapID)
        {
            //gameObject.SetActive(false);
            if (mapID == -1)
            {
                GameObject m = Instantiate(MAP) as GameObject;
                Game.instance = m.GetComponent<Game>();
            }
            GameObject.Destroy(gameObject);
            return;
        }
        Game.instance = this;
        player = FindObjectOfType<PlayerShip>();
        GameObject temp = Instantiate(playerShip) as GameObject;
        GameObject[] playerSpawnpoints = GameObject.FindGameObjectsWithTag("PlayerSpawn");
        temp.transform.position = playerSpawnpoints[(int)Random.Range(0, playerSpawnpoints.Length)].transform.position;
        player.ship = temp.GetComponent<SpaceShip>();

        try
        {
            GameObject[] enemySpawnpoints = GameObject.FindGameObjectsWithTag("EnemySpawn");
            GameObject[] npcSpawnpoints = GameObject.FindGameObjectsWithTag("NPCSpawn");
            foreach (int e in ENEMIES)
            {
                if (e < 0)
                    continue;
                else
                {
                    GameObject tempShip = Instantiate(SHIPS[e]) as GameObject;
                    tempShip.transform.position = enemySpawnpoints[(int)Random.Range(0, enemySpawnpoints.Length)].transform.position;
                    GameObject g = new GameObject();
                    NpcShip s = g.AddComponent<NpcShip>();
                    s.style = e;
                    
                    s.ship = tempShip.GetComponent<SpaceShip>();

                    g.transform.position = tempShip.transform.position;

                    s.team = 0;
                }
            }
            foreach (int e in NPCs)
            {
                if (e < 0)
                    continue;
                else
                {
                    GameObject tempShip = Instantiate(SHIPS[e]) as GameObject;
                    tempShip.transform.position = npcSpawnpoints[(int)Random.Range(0, npcSpawnpoints.Length)].transform.position;
                    GameObject g = new GameObject();
                    NpcShip s = g.AddComponent<NpcShip>();

                    s.style = e;

                    s.ship = tempShip.GetComponent<SpaceShip>();

                    g.transform.position = tempShip.transform.position;

                    s.team = 1;
                }
            }
        } catch
        {
            
        }

        RenderSettings.skybox = skyboxMaterial;
        RenderSettings.sun = sunSource;
        RenderSettings.customReflectionTexture = reflections;
        Physics.gravity = new Vector3(0, gravity, 0);
        RadarMask = radar_mask;
        if (record == false) stopedRecording = true;
        if (playbackRecording)
            SetupPlayback();
    }

    bool startedRecording = false;
    bool stopedRecording = false;

    // Update is called once per frame
    void Update()
    {
        if (record && !startedRecording)
            StartRecording();
        else if (!record && !stopedRecording)
            StopRecording();
    }

    public void SetupPlayback()
    {

        string json = File.OpenText("Recordings/ShipRec_" + playbackNumber + ".json").ReadToEnd();
        rec.shipLogs = new List<Recorder.ShipLog>(JsonHelper.FromJson<Recorder.ShipLog>(json));

        // Missile Logs
        json = File.OpenText("Recordings/MissileRec_" + playbackNumber + ".json").ReadToEnd();
        rec.missileLogs = new List<Recorder.MissileLog>(JsonHelper.FromJson<Recorder.MissileLog>(json));

        // SpaceObject Logs
        json = File.OpenText("Recordings/ObjectRec_" + playbackNumber + ".json").ReadToEnd();
        rec.objectLogs = new List<Recorder.ObjectLog>(JsonHelper.FromJson<Recorder.ObjectLog>(json));

        // Object List
        json = File.OpenText("Recordings/Objects_" + playbackNumber + ".json").ReadToEnd();
        rec.SetObjects(JsonHelper.FromJson<string>(json));

        // Object Spawn Times List
        json = File.OpenText("Recordings/SpawnTimes_" + playbackNumber + ".json").ReadToEnd();
        rec.SetObjectSpawnTimes(JsonHelper.FromJson<float>(json));
        
        Debug.Log("Recording Loaded!");

        rec.StartCoroutine(rec.Play());
    }

    public void StartRecording()
    {
        record = true;
        startedRecording = true;
        stopedRecording = false;
    }

    public void StopRecording()
    {
        record = false;
        stopedRecording = true;
        startedRecording = false;

        if (!Directory.Exists("Recordings"))
            Directory.CreateDirectory("Recordings");

        // Ship Logs
        string filename = "Recordings/ShipRec_0.json";
        int i = 0;
        while (File.Exists(filename))
        {
            //Debug.Log(filename + " already exists.");
            //return;
            i++;
            filename = "Recordings/ShipRec_" + i + ".json";
        }
        var sr = File.CreateText(filename);
        sr.Write(JsonHelper.ToJson(rec.shipLogs.ToArray()));
        sr.Close();

        // Missile Logs
        filename = "Recordings/MissileRec_" + i + ".json";
        sr = File.CreateText(filename);
        sr.Write(JsonHelper.ToJson(rec.missileLogs.ToArray()));
        sr.Close();


        // SpaceObject Logs
        filename = "Recordings/ObjectRec_" + i + ".json";
        sr = File.CreateText(filename);
        sr.Write(JsonHelper.ToJson(rec.objectLogs.ToArray()));
        sr.Close();

        // Object List
        filename = "Recordings/Objects_" + i + ".json";
        sr = File.CreateText(filename);
        sr.Write(JsonHelper.ToJson(rec.GetObjects()));
        sr.Close();

        // Object Spawn Times List
        filename = "Recordings/SpawnTimes_" + i + ".json";
        sr = File.CreateText(filename);
        sr.Write(JsonHelper.ToJson(rec.GetObjectSpawnTimes()));
        sr.Close();

        Debug.Log("Recording Saved!");

    }

    IEnumerator RecordObjects()
    {
        float period = 1;

        while(record)
        {
            foreach(SpaceObject o in FindObjectsOfType(typeof(SpaceObject)))
            {
                rec.LogObject(o);
                yield return new WaitForSeconds(period);
            }
        }
    }
}
