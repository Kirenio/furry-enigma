using UnityEngine;
using System.Collections;

public class LightOrb : MonoBehaviour
{
    float timer = 7;
    float amount = Random.Range(10f, 20f);
    LightMote target;

    protected void OnMouseDown()
    {
        Rules.GameManagerObject.AddScore(amount * 0.05f);
        if(target != null) target.IncreaseEnergy(amount);
        Destroy(gameObject);
    }

    public void SetTarget(LightMote mote)
    {
        target = mote;
    }

    void FixedUpdate()
    {
        if (timer > 0) timer -= Time.fixedDeltaTime;
        else
        {
            Destroy(gameObject);
        }
    }
}
