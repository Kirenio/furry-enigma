using UnityEngine;
using System.Collections;

public class LightOrb : MonoBehaviour
{
    float timer = 7;
    float amount;
    void Awake()
    {
        amount = Random.Range(0.2f, 5f);
    }

    protected void OnMouseDown()
    {
        Rules.GameManagerObject.AddScore(amount);
        Destroy(gameObject);
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
