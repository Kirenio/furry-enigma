using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {
    [Header("Prefabs")]
    public GameObject LightMote;
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

    public LightMote StartingSphere;
    GameObject[] DirectNeighboursGO;
    LightMote[] DirectNeighbours;

    GameObject RandomDebris
    {
        get { return Debris[Random.Range(0, Debris.Length)]; }
    }
    GameObject RandomDebrisCore
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
        GameObject startingSphereGO = (GameObject)Instantiate(LightMote, Vector3.zero, Quaternion.identity);
        StartingSphere = startingSphereGO.GetComponent<LightMote>();

        StartingSphere.Neighbours = new LightMote[3];
        DirectNeighboursGO = new GameObject[3];
        DirectNeighboursGO[0] = (GameObject)Instantiate(LightMote, GetPosition(StartingSphere.transform.position, 10f, 20f, Mathf.PI / 3, Mathf.PI / 6), Quaternion.identity);
        StartingSphere.Neighbours[0] = DirectNeighboursGO[0].GetComponent<LightMote>();
        DirectNeighboursGO[1] = (GameObject)Instantiate(LightMote, GetPosition(StartingSphere.transform.position, 8f, 20f, Mathf.PI, Mathf.PI / 2), Quaternion.identity);
        StartingSphere.Neighbours[1] = DirectNeighboursGO[1].GetComponent<LightMote>();
        DirectNeighboursGO[2] = (GameObject)Instantiate(LightMote, GetPosition(StartingSphere.transform.position, 9f, 22f, 5/3*Mathf.PI, 4/3*Mathf.PI), Quaternion.identity);
        StartingSphere.Neighbours[2] = DirectNeighboursGO[2].GetComponent<LightMote>();
    }

    void Start()
    {
        StartingSphere.PreInfuse();
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

    void SpawnShatteredSphereWithRing(Vector3 pos, float minRange, float maxRange)
    {
        SpawnShatteredSphere(pos);
        SpawnDebrisRing(pos, minRange, maxRange, (int)(minRange + maxRange));
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

    float GetDistance(float minRange, float maxRange)
    {
        return Random.Range(minRange, maxRange);
    }
}
