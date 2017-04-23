using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public delegate void MoteEventHandler();
public delegate void MoteProductionEventHandler(float amount);
public class LightMote : Sphere
{
    [Header("Pointers")]
    public Light lightComponent;
    public MeshRenderer meshRendererComponent;
    public Text timer;
    public Text rate;
    public Image SliderFill;
    public ParticleSystem Halo;
    public ParticleSystem HaloCollapsing;
    public ParticleSystem Explosion;
    public LightMote[] Neighbours;

    [Header("Starting values")]
    public float maxLightResource;
    public float startingResourceBurn;
    public float startingLightPower;
    public float passiveLightRaius;

    [Header("Current values")]
    public float currenLightResource;
    public float currentLightPower;
    public float currentBurnRate;
    public float lightFadePower;
    public float lightExtracted;
    public float darknessRessistence;

    public MoteEventHandler Unstable;
    public MoteEventHandler StopInstability;
    public MoteEventHandler Collapse;
    public MoteProductionEventHandler Produced;
    //Internal Values
    public bool isActive;
    public bool isInfused;

    // Use this for initialization
    new void Start () {
        base.Start();
        currenLightResource = maxLightResource;
        timer.text = ((int)currenLightResource).ToString();
        rate.text = currentBurnRate.ToString();
        if (!isRevealed) IsRevealed += Activate;
    }

	// Update is called once per frame
	void FixedUpdate ()
    {
        if (isInfused)
        {
            lightFadePower = Random.Range(0f, 1.325f) * Time.fixedDeltaTime;
            lightExtracted = startingResourceBurn * Time.fixedDeltaTime;
            
            if (currenLightResource <= maxLightResource * 0.1f && currenLightResource > 0)
            {
                currenLightResource -= lightExtracted;
                if (Produced != null) Produced(lightExtracted);
                if (Unstable != null) Unstable();
            }
            else if (currenLightResource - lightExtracted > 0)
            {
                currenLightResource -= lightExtracted;
                if(Produced != null)Produced(lightExtracted);
            }
            else
            {
                lightExtracted = currenLightResource;
                currentBurnRate = lightExtracted;
                if (Produced != null) Produced(lightExtracted);
                currenLightResource = 0;
            }
            darknessRessistence = (currentLightPower / 25 * currentLightPower / 25) * Time.fixedDeltaTime;
            currentLightPower += lightExtracted - darknessRessistence - lightFadePower;

            SliderFill.fillAmount = currenLightResource / maxLightResource;

            timer.text = ((int)currenLightResource).ToString();
            rate.text = currentBurnRate.ToString();
        }
        else if(isActive)
        {
            currentLightPower += passiveLightRaius * 0.1f;
            if (currentLightPower > passiveLightRaius) currentLightPower = passiveLightRaius;
        }

        if (currentLightPower <= 0)
        {
            currentLightPower = 0;
            if (Collapse != null) Collapse();
        }
        else if (currentLightPower < 2)
        {
            if (StopInstability != null) StopUnstable();
        }
        lightComponent.range = currentLightPower;
        meshRendererComponent.material.SetColor("_EmissionColor", Color.white * 0.15f * currentLightPower);
        TryActivateOthers();
    }

    public void TryActivateOthers()
    {
        for(int i = 0; i < Neighbours.Length; i++)
        {
            Neighbours[i].checkDistance(transform.position, currentLightPower);
        }
    }

    protected void Activate()
    {
        IsRevealed -= Activate;
        isActive = true;
        meshRendererComponent = gameObject.GetComponent<MeshRenderer>();
        meshRendererComponent.material = Rules.GameManagerObject.SpheresMaterial;
        meshRendererComponent.material.EnableKeyword("_EMISSION");
        lightComponent.range = passiveLightRaius;
        meshRendererComponent.material.SetColor("_EmissionColor", Color.white * 0.25f);
    }

    public void Infuse()
    {
        isInfused = true;
        isActive = false;
        currentBurnRate = startingResourceBurn;
        ParticleSystem.EmissionModule em = Halo.emission;
        em.enabled = true;
        Unstable += BecomeUnstable;
    }

    public void PreInfuse()
    {
        Reveal();
        isInfused = true;
        meshRendererComponent = gameObject.GetComponent<MeshRenderer>();
        meshRendererComponent.material = Rules.GameManagerObject.SpheresMaterial;
        meshRendererComponent.material.EnableKeyword("_EMISSION");
        currentLightPower = startingLightPower;
        lightComponent.range = currentLightPower;
        meshRendererComponent.material.SetColor("_EmissionColor", Color.white * 0.15f * startingLightPower);
        currentBurnRate = startingResourceBurn;
        ParticleSystem.EmissionModule em = Halo.emission;
        em.enabled = true;
        Halo.Simulate(10f,false, false);
        Halo.Play();
        Unstable += BecomeUnstable;
    }

    void BecomeUnstable()
    {
        Debug.Log("No way back anymore!");
        Unstable -= BecomeUnstable;
        StopInstability += StopUnstable;
        //ParticleSystem.EmissionModule em = Halo.emission;
        //em.enabled = false;
        ParticleSystem.EmissionModule emc = HaloCollapsing.emission;
        emc.enabled = true;
    }

    void StopUnstable()
    {
        Debug.Log("It is over.");
        StopInstability -= StopUnstable;
        Collapse += TotalCollapse;
        ParticleSystem.EmissionModule em = HaloCollapsing.emission;
        em.enabled = false;
        ParticleSystem.EmissionModule emc = Explosion.emission;
        emc.enabled = true;
        Explosion.Stop();
        Explosion.Play();
    }

    void TotalCollapse()
    {
        Debug.Log("Good night...");
        Collapse -= TotalCollapse;
        //ParticleSystem.EmissionModule emc = Halo.emission;
        //emc.enabled = true;
        //Halo.Simulate(10f, false, false);
        //Halo.Play();
    }
}
