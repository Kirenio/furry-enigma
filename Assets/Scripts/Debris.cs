using UnityEngine;
using System.Collections;

public class Debris : MonoBehaviour {
    public Vector3 rotationSpeed;
	// Use this for initialization
	void Start () {
        rotationSpeed = new Vector3(Random.Range(-0.15f, 0.15f), Random.Range(-0.15f, 0.15f), Random.Range(-0.15f, 0.15f));
	}
	
    void FixedUpdate()
    {
        transform.Rotate(rotationSpeed);
    }
}
