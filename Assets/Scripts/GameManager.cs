using UnityEngine;

public class GameManager : MonoBehaviour {
    [Header("Prefabs")]
    public GameObject LightMote;
    public GameObject LightOrb;
    public GameObject DarkOrb;
    public GameObject[] Debris;
    public GameObject[] DebrisCores;
    public GameObject InactiveSphere;
    public GameObject[] CrackedSpheres;
    public Material SpheresMaterial;
    //public Material SphereMaterial { get { return sphereMaterial; } }

    [Header("GenerationQuotas")]
    public int debrisRingQuota;
    public int debrisRingDensity;
    public int debrisQuota;
    public int shatteredSpheresQuota;
    public int debrisPerShatteredSphere;
    public int inactiveSpheresQuota;
    public int crackedSpheresQuota;

    public float basicSpawnCooldown = 120f;
    public float LightOrbChance = 4;
    public float cooldown = 0;
    public int iteration;

    bool GameStarted = false;

    public LightMote StartingSphere;
    GameObject[] DirectNeighboursGO;

    public GameObject RandomCrackedMote
    {
        get { return CrackedSpheres[Random.Range(0, CrackedSpheres.Length)]; }
    }
    public GameObject RandomDebris
    {
        get { return Debris[Random.Range(0, Debris.Length)]; }
    }
    public GameObject RandomDebrisCore
    {
        get { return DebrisCores[Random.Range(0, DebrisCores.Length)]; }
    }
    Quaternion GetRandomRotation
    {
        get { return new Quaternion(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f)); }
    }

    // Use this for initialization
    void Awake () {
        //Debug.Log(SphereMaterial.ToString());
        Rules.GameManagerObject = this;
        Rules.GameStarted += CreateLevel;
    }
    
    void FixedUpdate()
    {
        if(GameStarted)
        {
            if (cooldown > basicSpawnCooldown)
            {
                LightMote target = Rules.RandomInfusedMote;
                if (Random.Range(0f, 10f) > LightOrbChance) Instantiate(LightOrb, GetPosition(target.transform.position, 2f, 7f), Quaternion.identity);
                else
                {
                    GameObject newDarkOrb = (GameObject)Instantiate(DarkOrb, GetPosition(target.transform.position, 2f, 7f), Quaternion.identity);
                    newDarkOrb.GetComponent<DarkOrb>().SetTarget(target);
                }
                iteration++;
                cooldown = 0;
            }
            else
            {
                cooldown++;
            }

            if (iteration > 5)
            {
                LightOrbChance += 0.1f;
                basicSpawnCooldown--;
                iteration = 0;
            }
        }
    }

    void SpawnRandomDerbis(Vector3 pos, float minRange, float maxRange, int amount)
    {
        for (int i = 0; i < debrisQuota; i++)
        {
            Instantiate(RandomDebris, GetPosition(pos, minRange, maxRange), GetRandomRotation);
        }
    }

    void SpawnDebrisRing(Vector3 pos, float minRange, float maxRange, int density)
    {
        for (int i = 0; i < debrisQuota; i++)
        {
            Instantiate(RandomDebris, GetPosition(pos, minRange, maxRange), GetRandomRotation);
        }
    }

    void SpawnInactiveSphere(Vector3 pos)
    {
        Instantiate(InactiveSphere, pos, Quaternion.identity);
    }

    void SpawnShatteredSphere(Vector3 pos)
    {
        Instantiate(RandomDebrisCore, pos, Quaternion.identity);
        SpawnRandomDerbis(pos, 2f, 5f, debrisPerShatteredSphere + Random.Range(-2, 3));
    }

    void SpawnShatteredSphereWithRing(Vector3 spherePosition, float minRange, float maxRange)
    {
        SpawnShatteredSphere(spherePosition);
        SpawnDebrisRing(spherePosition, minRange, maxRange, (int)(minRange + maxRange));
    }

    Vector3 GetPosition(Vector3 pos, float minRange, float maxRange)
    {
        float distance = Random.Range(minRange, maxRange);

        Vector3 position;
        float angle = Random.Range(0f, 2 * Mathf.PI);
        position.x = pos.x + distance * Mathf.Cos(angle);
        position.y = Random.Range(-2f, 2f);
        position.z = pos.z + distance * Mathf.Sin(angle);

        return position;
    }

    Vector3 GetPosition(Vector3 pos, float minRange, float maxRange, float minAngle, float maxAngle)
    {
        float distance = Random.Range(minRange, maxRange);

        Vector3 position;
        float angle = Random.Range(minAngle, maxAngle);
        position.x = pos.x + distance * Mathf.Cos(angle);
        position.y = 0;
        position.z = pos.z + distance * Mathf.Sin(angle);

        return position;
    }

    Vector3 GetPositionPush(Vector3 pos, float minRange, float maxRange, float angle, float pushAmount)
    {
        float distance = Random.Range(minRange, maxRange);

        Vector3 position;
        angle += Random.Range(0, pushAmount);
        position.x = pos.x + distance * Mathf.Cos(angle);
        position.y = 0;
        position.z = pos.z + distance * Mathf.Sin(angle);

        return position;
    }

    float GetDistance(float minRange, float maxRange)
    {
        return Random.Range(minRange, maxRange);
    }

    public void InfuseSphere(LightMote target)
    {
        bool result = StartingSphere.ReduceEnergyStore(Rules.InfusionCost);
        if (result) target.Infuse();
    }

    void CreateLevel()
    {
        // Doing the first circle of objects
        GameObject startingSphereGO = (GameObject)Instantiate(LightMote, Vector3.zero, Quaternion.identity);
        startingSphereGO.name = "startingSphere";
        StartingSphere = startingSphereGO.GetComponent<LightMote>();
        StartingSphere.Neighbours = new LightMote[3];
        DirectNeighboursGO = new GameObject[3];
        DirectNeighboursGO[0] = (GameObject)Instantiate(LightMote, GetPosition(StartingSphere.transform.position, 10f, 20f, Mathf.PI / 2.25f, Mathf.PI * 2 / 3), Quaternion.identity);
        DirectNeighboursGO[0].name = "directN_0";
        StartingSphere.Neighbours[0] = DirectNeighboursGO[0].GetComponent<LightMote>();
        DirectNeighboursGO[1] = (GameObject)Instantiate(LightMote, GetPosition(StartingSphere.transform.position, 8f, 16f, Mathf.PI * 11 / 12, Mathf.PI * 13 / 12), Quaternion.identity);
        DirectNeighboursGO[1].name = "directN_1";
        StartingSphere.Neighbours[1] = DirectNeighboursGO[1].GetComponent<LightMote>();
        DirectNeighboursGO[2] = (GameObject)Instantiate(LightMote, GetPosition(StartingSphere.transform.position, 6f, 14f, Mathf.PI * 5 / 3, Mathf.PI * 2), Quaternion.identity);
        DirectNeighboursGO[2].name = "directN_2";
        StartingSphere.Neighbours[2] = DirectNeighboursGO[2].GetComponent<LightMote>();

        // Spawning debri
        SpawnDebrisRing(Vector3.zero, 2f, 20f, debrisRingDensity * 2);
        SpawnDebrisRing(DirectNeighboursGO[0].transform.position, 3f, 5f, debrisRingDensity * 6);
        SpawnRandomDerbis(Vector3.zero, 2f, 15f, debrisQuota);
        SpawnShatteredSphereWithRing(GetPosition(Vector3.zero, 3f, 6f, Mathf.PI / 5f, Mathf.PI / 2.5f), 2f, 6f);

        // Second round os spheres
        StartingSphere.Neighbours[0].Neighbours = new LightMote[1];
        GameObject indirectNeighbour = (GameObject)Instantiate(LightMote, GetPosition(DirectNeighboursGO[0].transform.position, 5f, 10f, Mathf.PI / 2.25f, Mathf.PI * 2 / 3), Quaternion.identity);
        indirectNeighbour.name = StartingSphere.Neighbours[0].name + "_0";
        StartingSphere.Neighbours[0].Neighbours[0] = indirectNeighbour.GetComponent<LightMote>();

        Instantiate(RandomCrackedMote, GetPosition(DirectNeighboursGO[0].transform.position, 3f, 15f, Mathf.PI * 3 / 4, Mathf.PI * 7 / 6), Quaternion.identity);
        if (Random.Range(0, 10) > 5)
            Instantiate(RandomCrackedMote, GetPosition(DirectNeighboursGO[0].transform.position, 5f, 12f, Mathf.PI * 3 / 4, Mathf.PI * 5 / 4), Quaternion.identity);
        else
            SpawnShatteredSphere(GetPosition(DirectNeighboursGO[0].transform.position, 4f, 12f, Mathf.PI * 11 / 6, Mathf.PI * 20 / 19));


        //After everything is done
        StartingSphere.PreInfuse();
        GameStarted = true;
    }
}
