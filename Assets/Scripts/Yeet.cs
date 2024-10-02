using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Yeet
{

    public static string[] ShipNames =
        {
            "Fancy Ship",
            "Super Fancy Ship",
            "My Wet Dream",
            "Nauvoo",
            "Donnager",
            "Amun-Ra",
            "Anubis",
            "Corvette",
            "Africanus",
            "Agatha King",
            "Amaterasu",
            "Ambedkar",
            "Amberjack",
            "Andronicus",
            "Arboghast",
            "Artemis",
            "Asander",
            "Askia",
            "Atacama",
            "Augustin Gamarra",
            "Austerlitz",
            "Avalanche",
            "Azure Dragon",
            "Banshee",
            "Wyvern",
            "Barbapiccola",
            "Barkeith",
            "Bashi Bazouk",
            "Battle Angel",
            "Screaming Firehawk",
            "Bellaire",
            "Black Kite",
            "Bradbury",
            "Callisto's Dream",
            "Campsi",
            "Cao Cao",
            "Carson Lei",
            "Cerister",
            "Frontier Horizon",
            "Horizon Frontier",
            "Chet Lam",
            "Chetzemoka",
            "Connaught",
            "Countee Cullen",
            "Cydonia",
            "Dagnar",
            "Dagon",
            "Damascus",
            "Sword of Damocles",
            "Denali",
            "Dewalt",
            "Dorie Miller",
            "Drusilla",
            "Dracula",
            "Edward Israel",
            "Ekandjo",
            "Europa's Bane",
            "Eye of the Typhoon",
            "Wind of Aeons",
            "Ghale of Eidolons",
            "Farragut",
            "Gelidon",
            "Ferdowsi",
            "Franck",
            "Ferdinand",
            "Fernando",
            "Frank Alken",
            "Fuji",
            "Galt",
            "Garcia y Vazquez",
            "Gatamang Feronte",
            "Glambattista",
            "Granicus",
            "Gryphon",
            "Guanshiyin",
            "Guion Bluford",
            "Guy Molinari",
            "Halpenny",
            "Hamill-Stewart",
            "Hammurabi",
            "Hasami",
            "Hiram Grant",
            "Hodson",
            "Horus",
            "Poseidon",
            "Hyperion",
            "Iceni",
            "You're gonna need a bigger gun",
            "Inazami",
            "Intrepid",
            "Jammy Rakshasa",
            "E-Caterina",
            "Jawaharlal",
            "Jefferson Mays",
            "Jimenez",
            "John Cabot",
            "Jotrun",
            "Karakum",
            "Khandahar",
            "Khonsu",
            "Kittur Chennamma",
            "Katoa",
            "Koto",
            "Kunai",
            "Kruschev",
            "Lauber",
            "Lazy Songbird",
            "Leonidas",
            "Lucien",
            "Macedon",
            "Magnetar",
            "Mataclypse",
            "Mammatus",
            "Manitoba",
            "Manituland",
            "Alberta",
            "Columbia",
            "Ontario",
            "Paris",
            "Miami",
            "Santa Cruz",
            "Marasmus",
            "McCabe",
            "Mograph",
            "Montenegro",
            "Copenhagen",
            "Waterloo",
            "Montivideo",
            "Morgaina",
            "Morigan",
            "Mowteng",
            "Mao Zedong",
            "Murphy",
            "Musafir",
            "Mustang Sally",
            "Nang Kwak",
            "Nathan Hale",
            "Nephthys",
            "Nokota Sioux",
            "Panshin",
            "Pella",
            "Pendulum's Arc",
            "Noa's Arc",
            "Phantom",
            "Pizzouza",
            "Pothos",
            "Proserpina",
            "Pulsar",
            "Rabia Balkhi",
            "Raptor",
            "Ravi",
            "Raskolnikov",
            "Rasputin",
            "Saberhagen",
            "Corey",
            "Santa Maria",
            "Sagamatha",
            "Vampire Wyvern",
            "Sagamantha",
            "Donald Trump",
            "Abrahams",
            "Hermano",
            "Prince Herb",
            "Mr. Cube",
            "Lord of the 7 Voids",
            "Seveneves",
            "Snowcrash",
            "Diamond Age",
            "Bridger"
        };

    public static string PrintCol(Dictionary<int, GameObject>.KeyCollection col)
    {
        string result = "[";
        
        foreach (var v in col)
        {
            result += v + ", ";
        }

        result += "]";
        return result;
    }

    public static string PrintCol(Dictionary<int, GameObject>.ValueCollection col)
    {
        string result = "[";

        foreach (var v in col)
        {
            result += v + ", ";
        }

        result += "]";
        return result;
    }

    [System.Serializable]
    public struct Dmg
    {
        

        public Dmg(float fire, float bludgeoning, float piercing, float slashing, float lightning, float force, float radiant)
        {
            this.fire = fire;
            this.bludgeoning = bludgeoning;
            this.piercing = piercing;
            this.slashing = slashing;
            this.lightning = lightning;
            this.force = force;
            this.radiant = radiant;

            ogCombinedDamage = fire + bludgeoning + piercing + slashing + lightning + force + radiant;
        }
        
        public void SetOGCombinedDamage(float x)
        {
            ogCombinedDamage = x;
        }

        public void UpdateDamages()
        {
            if (fire < 0.001f) fire = 0;
            if (bludgeoning < 0.001f) bludgeoning = 0;
            if (piercing < 0.001f) piercing = 0;
            if (slashing < 0.001f) slashing = 0;
            if (lightning < 0.001f) lightning = 0;
            if (force < 0.001f) force = 0;
            if (radiant < 0.001f) radiant = 0;
        }

        public float GetCombinedDamage()
        {
            return fire + bludgeoning + piercing + slashing + lightning + force + radiant;
        }

        public float GetDamagePercent()
        {
            return GetCombinedDamage() / ogCombinedDamage;
        }

        public void ScaleDamage(float k)
        {
            fire *= k;
            bludgeoning *= k;
            piercing *= k;
            slashing *= k;
            lightning *= k;
            force *= k;
            radiant *= k;
        }

        public void ScaleDamageAndOG(float k)
        {
            ScaleDamage(k);
            ogCombinedDamage *= k;
        }

        public float fire;
        public float bludgeoning;
        public float piercing;
        public float slashing;
        public float lightning;
        public float force;
        public float radiant;

        float ogCombinedDamage;
    }

    public static Vector3 Perp(Vector3 vec, Vector3 onto)
    {
        return vec - Vector3.Project(vec, onto.normalized);
    }

    public static Vector3 FixAngle(Vector3 eulers)
    {
        while (eulers.x > 180)
            eulers.x -= 360;
        while (eulers.x < -180)
            eulers.x += 360;
        while (eulers.y > 180)
            eulers.y -= 360;
        while (eulers.y < -180)
            eulers.y += 360;
        while (eulers.z > 180)
            eulers.z -= 360;
        while (eulers.z < -180)
            eulers.z += 360;
        return eulers;
    }

    public static float FixAngle(float eulers)
    {
        while (eulers > 180)
            eulers -= 360;
        while (eulers < -180)
            eulers += 360;

        return eulers;
    }
    
    public static Vector3 AngleDiff(Vector3 a, Vector3 b)
    {
        float x;
        float y;
        float z;

        x = b.x - a.x;
        if (Mathf.Abs(x) > Mathf.Abs(b.x - a.x + 360))
            x = b.x - a.x + 360;
        if (Mathf.Abs(x) > Mathf.Abs(b.x - a.x - 360))
            x = b.x - a.x - 360;

        y = b.y - a.y;
        if (Mathf.Abs(y) > Mathf.Abs(b.y - a.y + 360))
            y = b.y - a.y + 360;
        if (Mathf.Abs(y) > Mathf.Abs(b.y - a.y - 360))
            y = b.y - a.y - 360;

        z = b.z - a.z;
        if (Mathf.Abs(z) > Mathf.Abs(b.z - a.z + 360))
            z = b.z - a.z + 360;
        if (Mathf.Abs(z) > Mathf.Abs(b.z - a.z - 360))
            z = b.z - a.z - 360;

        return new Vector3(x, y, z);
    }

    public static List<Thruster> Concat(List<Thruster> a, List<Thruster> b)
    {
        List<Thruster> result = Copy(a);
        foreach (Thruster t in b)
            result.Add(t);
        return result;
    }
    
    public static Thruster[] Concat(Thruster[] a, Thruster[] b)
    {
        Thruster[] result = new Thruster[a.Length + b.Length];
        for (int i = 0; i < result.Length; i++)
        {
            if (i < a.Length)
                result[i] = a[i];
            else
                result[i] = b[i - a.Length];
        }
        return result;
    }

    public static List<Thruster> Copy(List<Thruster> a)
    {
        List<Thruster> result = new List<Thruster>();
        foreach (Thruster t in a)
        {
            result.Add(t);
        }
        return result;
    }
}
