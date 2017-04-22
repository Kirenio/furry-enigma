using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {
    [SerializeField]
    Material sphereMaterial;
    public GameObject[] Debris;
    public GameObject[] DebrisCores;
    public GameObject InactiveSphere;
    public GameObject[] CrackedSpheres;
    public int debrisQuota;
    public int shatteredSpheresQuota;
    public int debrisPerShatteredSphere;
    public int inactiveSpheresQuota;
    public int crackedSpheresQuota;
    public Material SphereMaterial { get { return sphereMaterial; } }

	// Use this for initialization
	void Awake () {
        Rules.GameManager = this;

        SpawnRandomDerbis(Vector3.zero, 6f, 35f, debrisQuota);

        for(int i = 0; i < shatteredSpheresQuota; i++)
        {
            SpawnShatteredSphere(GetPosition(Vector3.zero, 8f, 40f));
        }

        for(int i = 0; i < inactiveSpheresQuota; i++)
        {
            SpawnInactiveSphere(GetPosition(Vector3.zero, 5f, 7f * (i + 1)));
        }
	}

    void SpawnRandomDerbis(Vector3 pos, float minRange, float maxRange, int amount)
    {
        for (int i = 0; i < debrisQuota; i++)
        {
            Instantiate(Debris[Random.Range(0, Debris.Length)], GetPosition(pos, minRange, maxRange), GetRandomRotation());
        }
    }

    void SpawnInactiveSphere(Vector3 pos)
    {
        Instantiate(InactiveSphere, pos, Quaternion.identity);
    }

    void SpawnShatteredSphere(Vector3 pos)
    {
        Instantiate(DebrisCores[Random.Range(0, DebrisCores.Length)], pos, Quaternion.identity);
        SpawnRandomDerbis(pos, 2f, 5f, debrisPerShatteredSphere + Random.Range(-2, 3));
    }

    Vector3 GetPosition(Vector3 pos, float minRange, float maxRange)
    {
        float distance = Random.Range(minRange, maxRange);

        Vector3 position;
        position.x = pos.x + distance * Mathf.Cos(Random.Range(0f, 2 * Mathf.PI));
        position.y = Random.Range(-2f, 2f);
        position.z = pos.z + distance * Mathf.Sin(Random.Range(0f, 2 * Mathf.PI));

        return position;
    }

    Quaternion GetRandomRotation()
    {
        return new Quaternion(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
    }
}
