using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Lobby : MonoBehaviour
{
    public GameObject shipDisplay;

    public TextMeshProUGUI selectedShipText;
    public TextMeshProUGUI selectedMapText;
    public TextMeshProUGUI selectedEnemyText1;
    public TextMeshProUGUI selectedEnemyText2;
    public TextMeshProUGUI selectedEnemyText3;
    public TextMeshProUGUI selectedEnemyText4;
    public TextMeshProUGUI selectedTeamMateText1;
    public TextMeshProUGUI selectedTeamMateText2;
    public TextMeshProUGUI selectedTeamMateText3;
    public TextMeshProUGUI shipDescriptionText;

    int enemy1 = -1;
    int enemy2 = -1;
    int enemy3 = -1;
    int enemy4 = -1;

    int npc1 = -1;
    int npc2 = -1;
    int npc3 = -1;

    SpaceShip ship;

    public GameObject[] ships;
    public GameObject[] maps;

    [System.NonSerialized]
    public int selectedShip = 0;

    int displayingShipID = -1;

    public string[] scenes;

    [System.NonSerialized]
    public int selectedMap;

    // Start is called before the first frame update
    void Start()
    {
        Game.SHIPS = ships;
        Physics.gravity = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    int c = 0;

    string GetNpcText(int n)
    {
        if (n == -1)
            return "none";
        else return ships[n].GetComponent<SpaceShip>().name;
    }

    private void FixedUpdate()
    {
        c++;
        c %= 10000;
        if (c % 2 == 0)
        {
            DisplayShip();

            selectedEnemyText1.text = GetNpcText(enemy1);
            selectedEnemyText2.text = GetNpcText(enemy2);
            selectedEnemyText3.text = GetNpcText(enemy3);
            selectedEnemyText4.text = GetNpcText(enemy4);

            selectedTeamMateText1.text = GetNpcText(npc1);
            selectedTeamMateText2.text = GetNpcText(npc2);
            selectedTeamMateText3.text = GetNpcText(npc3);

            selectedMapText.text = maps[selectedMap].name;
        }
        if (ship != null)
        {
            //ship.mainDrive.thrustAmount = 1;
            //ship.mainDrive.DisplayThrust(0.5f);
            ship.transform.Rotate(0, 0.5f, 0);
            foreach (Thruster t in ship.space_thrusters)
                t.Thrust(0.8f);
            foreach (Thruster t in ship.q_thrusters)
                t.Thrust(0.4f);
        }
    }

    public void DisplayShip()
    {
        if (displayingShipID != selectedShip)
        {
            if (selectedShip < 0)
                selectedShip += ships.Length;
            displayingShipID = selectedShip;
            if (FindObjectOfType<SpaceShip>() != null)
                GameObject.Destroy(FindObjectOfType<SpaceShip>().gameObject);

            ship = (GameObject.Instantiate(ships[selectedShip]) as GameObject).GetComponent<SpaceShip>();

            //ship.trophy = true;
            ship.GetComponent<Rigidbody>().isKinematic = true;

            ship.transform.parent = shipDisplay.transform;
            ship.transform.localPosition = Vector3.zero;
            selectedShipText.text = ships[selectedShip].GetComponent<SpaceShip>().name;
            shipDescriptionText.text = ships[selectedShip].GetComponent<SpaceShip>().description;
        }
    }

    public void LobbyButton(string id)
    {
        switch (id)
        {
            case "next_map":
                selectedMap++;
                selectedMap %= maps.Length;
                break;
            case "prev_map":
                selectedMap--;
                selectedMap %= maps.Length;
                break;
            case "start":
                Game.playerShip = ships[selectedShip];
                Game.mapNum = selectedMap;
                Game.MAP = maps[selectedMap];
                Game.NPCs.Add(npc1);
                Game.NPCs.Add(npc2);
                Game.NPCs.Add(npc3);
                Game.ENEMIES.Add(enemy1);
                Game.ENEMIES.Add(enemy2);
                Game.ENEMIES.Add(enemy3);
                Game.ENEMIES.Add(enemy4);
                SceneManager.LoadScene(sceneName: "Singleplayer");
                break;
            case "next_ship":
                selectedShip++;
                selectedShip %= ships.Length;
                break;
            case "prev_ship":
                selectedShip--;
                selectedShip %= ships.Length;
                break;
            case "next_enemy1":
                enemy1 = nextNPCShip(enemy1, 1);
                break;
            case "prev_enemy1":
                enemy1 = nextNPCShip(enemy1, -1);
                break;
            case "next_enemy2":
                enemy2 = nextNPCShip(enemy2, 1);
                break;
            case "prev_enemy2":
                enemy2 = nextNPCShip(enemy2, -1);
                break;
            case "next_enemy3":
                enemy3 = nextNPCShip(enemy3, 1);
                break;
            case "prev_enemy3":
                enemy3 = nextNPCShip(enemy3, -1);
                break;
            case "next_enemy4":
                enemy4 = nextNPCShip(enemy4, 1);
                break;
            case "prev_enemy4":
                enemy4 = nextNPCShip(enemy4, -1);
                break;
            case "next_npc1":
                npc1 = nextNPCShip(npc1, 1);
                break;
            case "prev_npc1":
                npc1 = nextNPCShip(npc1, -1);
                break;
            case "next_npc2":
                npc2 = nextNPCShip(npc2, 1);
                break;
            case "prev_npc2":
                npc2 = nextNPCShip(npc2, -1);
                break;
            case "next_npc3":
                npc3 = nextNPCShip(npc3, 1);
                break;
            case "prev_npc3":
                npc3 = nextNPCShip(npc3, -1);
                break;
        }
    }

    public int nextNPCShip(int yeet, int dir)
    {
        yeet += dir;
        if (yeet > ships.Length || yeet == -1)
            return -1;
        else return yeet % ships.Length;
    }
}
