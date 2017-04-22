using UnityEngine;
using System.Collections;

public class Debris : MonoBehaviour {
    public Vector3 rotationSpeed;
	// Use this for initialization
	void Start () {
        rotationSpeed = new Vector3(Random.Range(-0.075f, 0.075f), Random.Range(-0.075f, 0.075f), Random.Range(-0.075f, 0.075f));
	}
	
    void FixedUpdate()
    {
        transform.Rotate(rotationSpeed);
    }
}
