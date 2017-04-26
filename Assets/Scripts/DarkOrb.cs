using UnityEngine;
using System.Collections;

public class DarkOrb : MonoBehaviour {
    float timer = 7;
    float amount = Random.Range(10, 40);
    LightMote target;
    
    protected void OnMouseDown()
    {
        if (Rules.GameManagerObject.GameStarted)
        {
            Rules.GameManagerObject.AddScore(amount * 0.05f);
            Destroy(gameObject);
        }
    }
    
    public void SetTarget(LightMote mote)
    {
        target = mote;
    }

    // Update is called once per frame
    void FixedUpdate () {
        if (Rules.GameManagerObject.GameStarted)
        {
            if (timer > 0) timer -= Time.fixedDeltaTime;
            else
            {
                if (target != null) target.ReduceEnergy(amount);
                Destroy(gameObject);
            }
        }
    }
}
