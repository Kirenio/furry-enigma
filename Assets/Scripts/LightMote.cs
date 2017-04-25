using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public delegate void MoteEventHandler();
public delegate void MoteResourceEventHandler(float amount);
public class LightMote : Sphere
{
    [Header("Pointers")]
    public Light lightComponent;
    public MeshRenderer meshRendererComponent;
    public Image SliderFill;
    public ParticleSystem Halo;
    public ParticleSystem HaloCollapsing;
    public ParticleSystem Explosion;
    public List<LightMote> Neighbours = new List<LightMote>();
    public GameObject InfuseButton;
    public LightMote Parent;
    public LineRenderer[] LineProgress;

    [Header("Starting values")]
    public float maxLightResource;
    public float startingResourceBurn;
    public float startingLightPower;
    public float passiveLightRaius;

    [Header("Current values")]
    public float currentLightResource;
    public float currentLightPower;
    public float currentBurnRate;
    public float lightFadePower;
    public float lightExtracted;
    public float darknessRessistence;
    public float upstreamRecieved;

    public MoteEventHandler Infused;
    public MoteEventHandler Unstable;
    public MoteEventHandler OnStopUnstable;
    public MoteEventHandler OnCollapse;
    public MoteResourceEventHandler Produced;
    public MoteResourceEventHandler UpstreamProduced;

    //Internal Values
    public bool isActive;
    public bool isInfused;
    public bool isPrimary = false;

    public float basicSpawnCooldown = Random.Range(160f, 200f);
    public float LightOrbChance = 4;
    public float cooldown = 0;
    public int iteration;

    // Use this for initialization
    new void Start () {
        base.Start();

        Unstable += BecomeUnstable;
        currentLightResource = maxLightResource;
        if (!isRevealed) IsRevealed += Activate;
        
        SetUpNeighbours();
    }

	// Update is called once per frame
	void FixedUpdate ()
    {
        darknessRessistence = (currentLightPower / 25 * currentLightPower / 25) * Time.fixedDeltaTime;
        if (isInfused)
        {
            if(Rules.GameManagerObject.GameStarted) SpawnOrb();
            lightFadePower = Random.Range(0f, 1.325f) * Time.fixedDeltaTime;
            lightExtracted = startingResourceBurn * Time.fixedDeltaTime;

            if (currentLightResource <= maxLightResource * 0.25f && currentLightResource > 0)
            {
                currentLightResource -= lightExtracted;
                currentLightResource += upstreamRecieved;
                if (Produced != null) Produced(lightExtracted + upstreamRecieved);
                if (Unstable != null) Unstable();
            }
            else if (currentLightResource - lightExtracted > 0)
            {
                currentLightResource -= lightExtracted;
                currentLightResource += upstreamRecieved;
                if (Produced != null) Produced(lightExtracted + upstreamRecieved);
                if (OnStopUnstable != null) OnStopUnstable();
            }
            else
            {
                lightExtracted = currentLightResource;
                currentBurnRate = lightExtracted;
                if (Produced != null) Produced(lightExtracted);
                currentLightResource = 0;
            }
            float upstreamAmount = 0;
            if (Parent != null)
            {
                upstreamAmount = (lightExtracted + upstreamRecieved) / 2f;
                if (UpstreamProduced != null) UpstreamProduced(upstreamAmount);
            }
            currentLightPower += (lightExtracted + upstreamRecieved) * 0.7f;

            SliderFill.fillAmount = currentLightResource / maxLightResource;

            upstreamRecieved = 0;
        }
        else if (isActive)
        {
            currentLightPower += passiveLightRaius * 0.15f;
            if (currentLightPower > passiveLightRaius) currentLightPower = passiveLightRaius;
        }

        currentLightPower -= (darknessRessistence + lightFadePower);
        if (currentLightPower <= 0)
        {
            currentLightPower = 0;
            if (OnCollapse != null) OnCollapse();
        }

        if (currentLightResource <= 0)
        {
            if (OnCollapse != null) OnCollapse();
        }

        for (int i = 0; i < Neighbours.Count; i++)
        {
            LineProgress[i].SetPosition(1, Vector3.Lerp(transform.position, Neighbours[i].transform.position, currentLightPower / Vector3.Distance(transform.position, Neighbours[i].transform.position)));
        }

        if (currentLightResource > maxLightResource) currentLightResource = maxLightResource;
        lightComponent.range = currentLightPower;
        meshRendererComponent.material.SetColor("_EmissionColor", Color.white * 0.5f * currentLightPower);
        TryActivateOthers();
        upstreamRecieved = 0;
    }

    public void ReduceEnergy(float amount)
    {
        currentLightResource -= amount;
    }

    public void IncreaseEnergy(float amount)
    {
        currentLightResource += amount;
    }

    public bool TryInfuse(float amount)
    {
        if (currentLightResource >= amount)
        {
            currentLightResource -= amount;
            currentLightPower = startingLightPower;
            lightComponent.range = currentLightPower;
            return true;
        }
        else return false;
    }

    protected void SendEnergy(LightMote target, float amount)
    {
        target.currentLightResource += amount;
    }

    protected void RecieveUpstream(float amount)
    {
        upstreamRecieved += amount;
    }

    public void TryActivateOthers()
    {
        for(int i = 0; i < Neighbours.Count; i++)
        {
            Neighbours[i].checkDistance(transform.position, currentLightPower);
        }
    }

    public void AskForInfusion()
    {
        Rules.GameManagerObject.InfuseSphere(this);
    }

    protected void Activate()
    {
        IsRevealed -= Activate;
        Clicked += AskForInfusion;
        isActive = true;
        meshRendererComponent = gameObject.GetComponent<MeshRenderer>();
        meshRendererComponent.material = Rules.GameManagerObject.SpheresMaterial;
        meshRendererComponent.material.EnableKeyword("_EMISSION");
        lightComponent.range = passiveLightRaius;
        meshRendererComponent.material.SetColor("_EmissionColor", Color.white * 0.25f);
        InfuseButton.SetActive(true);
    }

    public void Infuse()
    {
        Rules.GameManagerObject.RegisterProduction(this);
        Clicked -= AskForInfusion;
        isInfused = true;
        isActive = false;
        currentBurnRate = startingResourceBurn;
        ParticleSystem.EmissionModule em = Halo.emission;
        em.enabled = true;
        Halo.Stop();
        Halo.Play();
        Unstable += BecomeUnstable;
        OnCollapse += Collapse;
        InfuseButton.SetActive(false);
    }

    public void PreInfuse()
    {
        Reveal();
        Rules.GameManagerObject.RegisterProduction(this);
        isInfused = true;
        isPrimary = true;
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
        OnCollapse += Collapse;
    }

    void BecomeUnstable()
    {
        Unstable -= BecomeUnstable;
        OnStopUnstable += StopUnstable;
        ParticleSystem.EmissionModule emc = HaloCollapsing.emission;
        emc.enabled = true;
    }

    void StopUnstable()
    {
        Unstable += BecomeUnstable;
        OnStopUnstable -= StopUnstable;
        ParticleSystem.EmissionModule emc = HaloCollapsing.emission;
        emc.enabled = false;
    }

    void Collapse()
    {
        Rules.GameManagerObject.UnregisterInfusedMote(this);
        Rules.GameManagerObject.UnregisterProduction(this);
        meshRendererComponent.material = Rules.GameManagerObject.DarkOrbMaterial;
        isInfused = false;
        isActive = false;
        gameObject.GetComponent<SphereCollider>().enabled = false;
        OnCollapse -= Collapse;
        ParticleSystem.EmissionModule em = HaloCollapsing.emission;
        em.enabled = false;
        ParticleSystem.EmissionModule emc = Explosion.emission;
        emc.enabled = true;
        Explosion.Stop();
        Explosion.Play();
        UnsubscribeKeyboardHUDEvents();
        UnsubscribeMouseHUDEvents();
        ForceDisableHud();
        if (isPrimary) StartCoroutine(Rules.UIManagerObject.ShowGameOver());
    }

    void SetUpNeighbours()
    {
        LineProgress = new LineRenderer[Neighbours.Count];
        for (int i = 0; i < Neighbours.Count; i++)
        {
            Neighbours[i].Parent = this;
            Neighbours[i].UpstreamProduced += RecieveUpstream;

            GameObject lp = Instantiate(Rules.GameManagerObject.LineProgress);
            lp.name = "LP_" + i;
            lp.transform.SetParent(transform);
            LineProgress[i] = lp.GetComponent<LineRenderer>();
            LineProgress[i].SetPosition(0, transform.position);
            LineProgress[i].SetPosition(1, transform.position);
        }
    }

    void SpawnOrb()
    {
        if (cooldown > basicSpawnCooldown)
        {
            LightMote target = this;
            if (Random.Range(0f, 10f) > LightOrbChance)
            {
                GameObject newLightOrb = (GameObject)Instantiate(Rules.GameManagerObject.LightOrb, Rules.GameManagerObject.GetPosition(target.transform.position, 2f, 7f), Quaternion.identity);
                newLightOrb.GetComponent<LightOrb>().SetTarget(target);
            }
            else
            {
                GameObject newDarkOrb = (GameObject)Instantiate(Rules.GameManagerObject.DarkOrb, Rules.GameManagerObject.GetPosition(target.transform.position, 2f, 7f), Quaternion.identity);
                newDarkOrb.GetComponent<DarkOrb>().SetTarget(target);
            }
            iteration++;
            cooldown = 0;
        }
        else
        {
            cooldown += Random.Range(0.8f, 1.2f);
        }

        if (iteration > 3)
        {
            LightOrbChance += 0.1f;
            basicSpawnCooldown -= Random.Range(0.2f, 0.5f);
            iteration = 0;
        }
    }
}
