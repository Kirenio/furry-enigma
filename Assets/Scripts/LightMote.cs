using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public delegate void MoteEventHandler();
public delegate void MoteResourceEventHandler(float amount);
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
    public GameObject InfuseButton;
    public LightMote Parent;

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
    public float upstreamRecieved;

    public MoteEventHandler Infused;
    public MoteEventHandler Unstable;
    public MoteEventHandler StopInstability;
    public MoteResourceEventHandler Produced;
    public MoteResourceEventHandler UpstreamProduced;


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

        for(int i = 0; i < Neighbours.Length; i++)
        {
            Neighbours[i].Parent = this;
            Neighbours[i].UpstreamProduced += RecieveEnergy;
        }
    }

	// Update is called once per frame
	void FixedUpdate ()
    {
        if (Input.GetKeyDown(KeyCode.Insert))
        {
            currenLightResource = 20;
        }
        darknessRessistence = (currentLightPower / 25 * currentLightPower / 25) * Time.fixedDeltaTime;
        if (isInfused)
        {
            lightFadePower = Random.Range(0f, 1.325f) * Time.fixedDeltaTime;
            lightExtracted = startingResourceBurn * Time.fixedDeltaTime;

            if (currenLightResource <= maxLightResource * 0.1f && currenLightResource > 0)
            {
                currenLightResource -= lightExtracted;
                currenLightResource += upstreamRecieved;
                if (Produced != null) Produced(lightExtracted + upstreamRecieved);
                if (Unstable != null) Unstable();
            }
            else if (currenLightResource - lightExtracted > 0)
            {
                currenLightResource -= lightExtracted;
                currenLightResource += upstreamRecieved;
                if (Produced != null)Produced(lightExtracted + upstreamRecieved);
            }
            else
            {
                lightExtracted = currenLightResource;
                currentBurnRate = lightExtracted;
                if (Produced != null) Produced(lightExtracted);
                currenLightResource = 0;
            }
            float upstreamAmount = 0;
            if (Parent != null)
            {
                upstreamAmount = (lightExtracted + upstreamRecieved) / 2f;
                if (UpstreamProduced != null) UpstreamProduced(upstreamAmount);
            }
            currentLightPower += lightExtracted + upstreamRecieved;

            SliderFill.fillAmount = currenLightResource / maxLightResource;

            timer.text = ((int)currenLightResource).ToString();
            rate.text = string.Format("{0:0.##}", (lightExtracted - upstreamRecieved));
            upstreamRecieved = 0;
        }
        else if(isActive)
        {
            currentLightPower += passiveLightRaius * 0.15f;
            if (currentLightPower > passiveLightRaius) currentLightPower = passiveLightRaius;
        }

        currentLightPower -= (darknessRessistence + lightFadePower);
        if (currentLightPower <= 0)
        {
            currentLightPower = 0;
            if (StopInstability != null) StopUnstable();
        }
        else if (currentLightPower < 2)
        {
            if (StopInstability != null) StopUnstable();
        }
        lightComponent.range = currentLightPower;
        meshRendererComponent.material.SetColor("_EmissionColor", Color.white * 0.15f * currentLightPower);
        TryActivateOthers();
    }

    public bool ReduceEnergyStore(float amount)
    {
        currenLightResource -= amount;
        if (currenLightResource >= amount) return true;
        else return false;
    }

    protected void SendEnergy(LightMote target, float amount)
    {
        target.currenLightResource += amount;
    }

    protected void RecieveEnergy(float amount)
    {
        upstreamRecieved += amount;
    }

    public void TryActivateOthers()
    {
        for(int i = 0; i < Neighbours.Length; i++)
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
        Rules.RegisterProduction(this);
        Clicked -= AskForInfusion;
        isInfused = true;
        isActive = false;
        currentBurnRate = startingResourceBurn;
        ParticleSystem.EmissionModule em = Halo.emission;
        em.enabled = true;
        Halo.Stop();
        Halo.Play();
        Unstable += BecomeUnstable;
        InfuseButton.SetActive(false);
    }

    public void PreInfuse()
    {
        Reveal();
        Rules.RegisterProduction(this);
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
        Unstable -= BecomeUnstable;
        StopInstability += StopUnstable;
        ParticleSystem.EmissionModule emc = HaloCollapsing.emission;
        emc.enabled = true;
    }

    void StopUnstable()
    {
        Rules.UnregisterProduction(this);
        isInfused = false;
        gameObject.GetComponent<SphereCollider>().enabled = false;
        StopInstability -= StopUnstable;
        ParticleSystem.EmissionModule em = HaloCollapsing.emission;
        em.enabled = false;
        ParticleSystem.EmissionModule emc = Explosion.emission;
        emc.enabled = true;
        Explosion.Stop();
        Explosion.Play();
        UnsubscribeKeyboardHUDEvents();
        UnsubscribeMouseHUDEvents();
        ForceDisableHud();
    }
}
