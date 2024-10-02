using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    public int radius = 20;
    public GameObject yeet;
    public ParticleSystem particles;
    public Light explodeLight;
    public Vector3 vel;
    public Yeet.Dmg dmg = new Yeet.Dmg(10, 1, 1, 0, 1, 5, 5);
    public Particle tendril;
    public GameObject flash;
    public float delay = 0;

    List<Particle> tendrils = new List<Particle>();
    public int density = 5;
    
    IEnumerator DelayedStart()
    {
        tendril.paused = true;
        yield return new WaitForSeconds(delay);
        particles.Play();
        for (int i = 0; i < 360; i += 360 / density)
        {
            for (int j = -90; j < 90; j += 360 / density)
            {
                GameObject newTendril = Instantiate(tendril.gameObject, transform, false) as GameObject;
                Particle newParticle = newTendril.GetComponent<Particle>();
                newParticle.dealsDamage = true;
                newParticle.dmg = dmg;
                newTendril.transform.localEulerAngles = new Vector3(0, i, j);
                newParticle.lifetime = 100;
                newParticle.velocity = Vector3.zero;
                newParticle.dmg.SetOGCombinedDamage(dmg.GetCombinedDamage());
                newParticle.shrinking = false;
                tendrils.Add(newParticle);
                newParticle.paused = false;
            }
        }

        GameObject.Destroy(tendril.gameObject);

        ScaleTendrils(0.1f);

        scaleNum = tendrils[0].transform.localScale.x;

        StartCoroutine(Die(particles.duration));
    }

    // Start is called before the first frame update
    void Start()
    {
        particles.Stop();
        //transform.localScale = Vector3.one * 0.1f;
        particles.startDelay = 0.5f*radius / (growRate / Time.fixedDeltaTime);
        //main.duration += delay + 0.5f * radius / (growRate / Time.fixedDeltaTime);
        explodeLight.intensity = 0;

        //tendrils.Add(tendril);

        StartCoroutine(DelayedStart());
    }

    // Update is called once per frame
    public float growRate = 1.2f;
    int c = 0;
    float scaleNum = 1;
    bool shrinking = false;
    void FixedUpdate()
    {
        transform.position += vel * Time.fixedDeltaTime;
        if (delay > 0)
        {
            delay--;
            explodeLight.intensity = 0;
            return;
        }
        //Debug.Log("scale num = " + scaleNum);
        explodeLight.intensity = scaleNum * 10;
        c++;
        if (scaleNum > radius*2 || shrinking)
        {
            //Debug.Log("Big Enough");
            shrinking = true;
            //transform.localScale *= 0.9f;
            ScaleTendrils(0.8f);
        } else
        {
            //transform.localScale = Vector3.one * growRate * c;
            ScaleTendrils(growRate);
        }
        if (scaleNum < 0.01f)
        {
            yeet.SetActive(false);
            flash.SetActive(false);
            //explodeLight.gameObject.SetActive(false);
        }
    }

    void ScaleTendrils(float k)
    {
        flash.transform.localScale *= k;
        scaleNum *= k;
        foreach (Particle t in tendrils)
        {
            if (t == null) continue;
            t.dmg.ScaleDamageAndOG(1 / k);
            float a = 1 + (k-1)*t.dmg.GetDamagePercent();
            if (a < 1)
                a = k;
            //Debug.Log("DmgPercent = " + t.dmg.GetDamagePercent());
            t.transform.localScale *= a;
        }
    }

    IEnumerator Die(float time)
    {
        //Debug.Log("Waiting " + time);
        yield return new WaitForSeconds(time*2);
        Destroy(gameObject);
    }

    /*
    public void OnTriggerEnter(Collider other)
    {
        GameObject obj = other.gameObject;

        //GameObject.Destroy(this.gameObject);

        ShipObject shipObj = obj.GetComponent<ShipObject>();
        if (shipObj != null)
        {
            shipObj.Damage(dmg, Mathf.Max(0, radius-Vector3.Distance(transform.position, other.transform.position)));
        }
        else
        {
            GameObject.Destroy(this.gameObject);
        }
    }
    */
}
