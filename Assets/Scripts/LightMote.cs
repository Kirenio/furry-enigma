using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LightMote : Sphere
{
    [Header("Pointers")]
    public Light lightComponent;
    public MeshRenderer meshRendererComponent;
    public Text timer;
    public Text rate;
    public Image SliderFill;

    [Header("Starting values")]
    public float maxLightResource;
    public float resourceBurn;
    public float startingLightPower;

    [Header("Current values")]
    public float currenLightResource;
    public float currentLightPower;
    public float lightFadePower;
    public float lightExtracted;
    public float darknessRessistence;
    
	// Use this for initialization
	new void Start () {
        base.Start();
        meshRendererComponent = gameObject.GetComponent<MeshRenderer>();
        meshRendererComponent.material = GameManager.SphereMaterial;
        meshRendererComponent.material.EnableKeyword("_EMISSION");
        currenLightResource = maxLightResource;
        currentLightPower = startingLightPower;
        timer.text = ((int)currenLightResource).ToString();
        rate.text = resourceBurn.ToString();
    }

	// Update is called once per frame
	void FixedUpdate () {
        lightFadePower = Random.Range(0f, 1.325f) * Time.deltaTime;
        lightExtracted = resourceBurn * Time.deltaTime;
        darknessRessistence = (currentLightPower / 25 * currentLightPower / 25) * Time.deltaTime;

        if (currenLightResource - lightExtracted > 0)
        {
            currenLightResource -= lightExtracted;
        }
        else
        {
            lightExtracted = currenLightResource;
            currenLightResource = 0;
        }

        currentLightPower += lightExtracted - darknessRessistence - lightFadePower;
        if (currentLightPower < 0) currentLightPower = 0;

        meshRendererComponent.material.SetColor("_EmissionColor", Color.white * currentLightPower);
        lightComponent.range = currentLightPower;
        timer.text = ((int)currenLightResource).ToString();
        rate.text = resourceBurn.ToString();
        SliderFill.fillAmount = currenLightResource / maxLightResource;
    }
}
