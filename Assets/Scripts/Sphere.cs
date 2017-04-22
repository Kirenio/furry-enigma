using UnityEngine;
using System.Collections;

public class Sphere : MonoBehaviour
{
    [Header("Base Sphere Properties")]
    public GameManager GameManager;
    public GameObject IndicatorsCanvas;
    public bool isRevealed = false;

    protected delegate void InternalEventHandler();
    protected event InternalEventHandler MouseEntryEvent;
    protected event InternalEventHandler MouseExitEvent;

    // Use this for initialization
    protected virtual void Start ()
    {
        GameManager = Rules.GameManager;
        if(isRevealed)
        {
            SubscribeKeyboardHUDEvents();
            SubscribeMouseHUDEvents();

            Rules.GameControls.GamePaused += UnsubscribeMouseHUDEvents;
            Rules.GameControls.GameUnpaused += UnsubscribeMouseHUDEvents;
            Rules.GameControls.ShowHUD += UnsubscribeMouseHUDEvents;
            Rules.GameControls.HideHUD += SubscribeMouseHUDEvents;
        }
    }
    
    protected void ShowHUD()
    {
        if (isRevealed) IndicatorsCanvas.SetActive(true);
    }

    protected void HideHUD()
    {
        IndicatorsCanvas.SetActive(false);
    }

    protected void OnMouseEnter()
    {
        if (MouseEntryEvent != null) MouseEntryEvent();
    }

    protected void OnMouseExit()
    {
        if (MouseExitEvent != null) MouseExitEvent();
    }

    protected void SubscribeKeyboardHUDEvents()
    {
        Rules.GameControls.ShowHUD += ShowHUD;
        Rules.GameControls.HideHUD += HideHUD;
    }

    protected void UnsubscribeKeyboardHUDEvents()
    {
        Rules.GameControls.ShowHUD -= ShowHUD;
        Rules.GameControls.HideHUD -= HideHUD;
    }

    protected void SubscribeMouseHUDEvents()
    {
        MouseEntryEvent += ShowHUD;
        MouseExitEvent += HideHUD;
    }

    protected void UnsubscribeMouseHUDEvents()
    {
        Debug.Log("Unsubscribing mouse events");
        MouseEntryEvent -= ShowHUD;
        MouseExitEvent -= HideHUD;
    }
}
