using UnityEngine;
using System.Collections;

public class DarkOrb : MonoBehaviour {
    float timer = 7;
    float amount = 5;
    LightMote target;

    protected void OnMouseDown()
    {
        Rules.AddScore(amount);
        Destroy(gameObject);
    }

    public void SetTarget(LightMote mote)
    {
        target = mote;
    }

    // Update is called once per frame
    void FixedUpdate () {
        if (timer > 0) timer -= Time.fixedDeltaTime;
        else
        {
            target.ReduceEnergyStore(amount);
            Destroy(gameObject);
        }
	}
}
