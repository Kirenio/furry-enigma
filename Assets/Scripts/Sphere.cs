﻿using UnityEngine;
using System.Collections;

public class Sphere : MonoBehaviour
{
    [Header("Base Sphere Properties")]
    public GameObject IndicatorsCanvas;
    protected bool isRevealed = false;

    protected delegate void InternalEventHandler();
    protected event InternalEventHandler MouseEntryEvent;
    protected event InternalEventHandler MouseExitEvent;
    protected event InternalEventHandler IsRevealed;
    protected event InternalEventHandler Clicked;

    // Use this for initialization
    protected virtual void Start ()
    {
        if (isRevealed) SetupSubscriptions();
        else IsRevealed += SetupSubscriptions;
    }

    protected void Reveal()
    {
        if(isRevealed == false)
        {
            isRevealed = true;
            if (IsRevealed != null) IsRevealed();
            if (!Rules.HideHUD) ForceEnableHUD();
        }
    }

    public void checkDistance(Vector3 pos, float radius)
    {
        //Debug.Log(Vector3.Distance(transform.position, pos) + " : " + radius);
        if (Vector3.Distance(transform.position, pos) <= radius) Reveal();
    }

    protected void SetupSubscriptions()
    {
        SubscribeKeyboardHUDEvents();
        SubscribeMouseHUDEvents();

        Rules.GameControls.GamePaused += UnsubscribeMouseHUDEvents;
        Rules.GameControls.GameUnpaused += UnsubscribeMouseHUDEvents;
        Rules.GameControls.ShowHUD += UnsubscribeMouseHUDEvents;
        Rules.GameControls.HideHUD += SubscribeMouseHUDEvents;
        IsRevealed -= SetupSubscriptions;
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
        if (Rules.HideHUD)
        {
            if (MouseEntryEvent != null) MouseEntryEvent(); // Either rename this or find another way to control the HUD
        }
    }

    protected void OnMouseExit()
    {
        if (Rules.HideHUD)
        {
            if (MouseExitEvent != null) MouseExitEvent();
        }
    }

    protected void OnMouseDown()
    {
        if (Clicked != null) Clicked();
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
        MouseEntryEvent -= ShowHUD;
        MouseExitEvent -= HideHUD;
    }

    protected void ForceDisableHud()
    {
        IndicatorsCanvas.SetActive(false);
    }

    protected void ForceEnableHUD()
    {
        IndicatorsCanvas.SetActive(true);
    }
}
