using UnityEngine;
using System.Collections;

public class DarkOrb : MonoBehaviour {
    float timer = 7;
    float amount = 5;

    protected void OnMouseDown()
    {
        Rules.AddScore(amount);
        Destroy(gameObject);
    }

    // Update is called once per frame
    void FixedUpdate () {
        if (timer > 0) timer -= Time.fixedDeltaTime;
        else
        {
            Rules.GameManagerObject.StartingSphere.ReduceEnergyStore(amount);
            Destroy(gameObject);
        }
	}
}
