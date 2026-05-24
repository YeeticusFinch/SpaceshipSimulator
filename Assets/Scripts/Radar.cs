using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Radar : ShipObject
{
    private static readonly Queue<LineRenderer> linePool = new Queue<LineRenderer>();
    public GameObject point;
    public int range = 100;
    public float electricNoise = 0.4f;

    // Start is called before the first frame update
    void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public bool RadarObject(GameObject obj, bool trace)
    {
        float boost = 1;

        if (ship != null)
        {
            if (ship.reactor != null)
            {
                if (reactor.highPower)
                    boost *= 3;
                else if (reactor.lowPower)
                    boost *= 0.6f;
            }
        }

        SpaceShip oShip = obj.GetComponent<SpaceShip>();
        if (oShip != null)
        {
            if (oShip.stealthFactor >= 1f)
                return false;
            if (oShip.stealthFactor <= -1f)
            {
                if (oShip.playerShip && oShip != ship)
                    oShip.player.addAlert("Incoming radar ping", Color.gray);
                return true;
            }

            if (oShip.electricNoise > 20)
                boost *= 1 + (oShip.electricNoise * Random.Range(0.8f, 1.2f) - 20) * 0.15f;
            else
            {
                boost *= 1 - (20 - oShip.electricNoise) / 19f;
            }
            boost *= (1f - oShip.stealthFactor);
            if (oShip.radarOn)
                boost *= 1.5f;
            if (oShip.mainDrive != null && oShip.mainDrive.Thrusting())
                boost *= 1 + oShip.mainDrive.thrustAmount * Random.Range(2, 4);
        }

        Vector3 rayDirection = obj.transform.position - point.transform.position;
        RaycastHit hit;
        if (Physics.Raycast(point.transform.position, rayDirection, out hit, range * boost, Game.RadarMask))
        {
            if (trace)
                drawLine(point.transform.position, hit.transform.position);
            if (hit.transform == obj.transform || hit.transform.IsChildOf(obj.transform) || obj.transform.IsChildOf(hit.transform))
            {
                SpaceShip o_ship = hit.transform.gameObject.GetComponent<SpaceShip>();
                if (o_ship == null)
                    o_ship = hit.transform.gameObject.GetComponentInParent<SpaceShip>();
                if (o_ship != null)
                {
                    if (o_ship.jamRadars)
                        return false;
                    else
                    {
                        if (o_ship.playerShip && o_ship != ship)
                            o_ship.player.addAlert("Incoming radar ping", Color.gray);
                    }
                }
                
                return true;
            }

        }

        if (Physics.Raycast(point.transform.position + rayDirection.normalized*0.1f, rayDirection, out hit, range * boost, Game.RadarMask))
        {
            if (trace)
                drawLine(point.transform.position, hit.transform.position);
            if (hit.transform == obj.transform || hit.transform.IsChildOf(obj.transform) || obj.transform.IsChildOf(hit.transform))
            {
                SpaceShip o_ship = hit.transform.gameObject.GetComponent<SpaceShip>();
                if (o_ship == null)
                    o_ship = hit.transform.gameObject.GetComponentInParent<SpaceShip>();
                if (o_ship != null)
                {
                    if (o_ship.jamRadars)
                        return false;
                    else if (o_ship.playerShip && o_ship != ship)
                        o_ship.player.addAlert("Incoming radar ping", Color.gray);
                }
                return true;
            }

        }

        return false;
    }

    void drawLine(Vector3 a, Vector3 b)
    {
        LineRenderer lineRenderer;
        if (linePool.Count > 0)
        {
            lineRenderer = linePool.Dequeue();
            if (lineRenderer == null)
                lineRenderer = new GameObject("RadarLine").AddComponent<LineRenderer>();
            else
                lineRenderer.gameObject.SetActive(true);
        }
        else
        {
            lineRenderer = new GameObject("RadarLine").AddComponent<LineRenderer>();
        }
        if (GetComponent<Targeter>() != null)
        {
            lineRenderer.startColor = Color.red;
            lineRenderer.endColor = Color.red;
        } else
        {
            lineRenderer.startColor = Color.green;
            lineRenderer.endColor = Color.green;
        }
        lineRenderer.startWidth = 0.01f;
        lineRenderer.endWidth = 0.01f;
        lineRenderer.positionCount = 2;
        lineRenderer.useWorldSpace = true;

        Material[] mat = new Material[] { point.GetComponent<MeshRenderer>().material };
        lineRenderer.materials = mat;
        lineRenderer.material = point.GetComponent<MeshRenderer>().material;

        //For drawing line in the world space, provide the x,y,z values
        lineRenderer.SetPosition(0, a); //x,y and z position of the starting point of the line
        lineRenderer.SetPosition(1, b); //x,y and z position of the end point of the line
        lineRenderer.receiveShadows = false;
        lineRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        lineRenderer.generateLightingData = false;
        StartCoroutine(FadeLine(lineRenderer));
    }

    IEnumerator FadeLine(LineRenderer l)
    {
        float fadeDuration = 0.5f;
        float fadePeriod = 0.1f;
        float initialWidth = 0.01f;
        for (float i = 0; i < fadeDuration/fadePeriod; i += fadePeriod)
        {
            l.startWidth *= 0.5f;
            l.endWidth *= 0.5f;
            yield return new WaitForSeconds(fadePeriod);
        }
        l.startWidth = initialWidth;
        l.endWidth = initialWidth;
        l.positionCount = 0;
        l.gameObject.SetActive(false);
        linePool.Enqueue(l);

    }
}
