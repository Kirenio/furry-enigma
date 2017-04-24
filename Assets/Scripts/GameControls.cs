using UnityEngine;
using System.Collections;

public delegate void UIEventsHandler();
public class GameControls : MonoBehaviour {
    public float panSpeed;
    public float zoomSpeed;

    [Header("Component pointers")]
    public Camera anchoredCamera;
    public GameObject gameMenu;

    [Header("Statuses")]

    // Internal variables
    Vector2 lastPos;
    
    public event UIEventsHandler ShowHUD;
    public event UIEventsHandler HideHUD;
    public event UIEventsHandler GamePaused;
    public event UIEventsHandler GameUnpaused;

    void Awake()
    {
        Rules.GameControls = this;
    }
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButton(1))
        {
            transform.position += transform.right * -Input.GetAxis("Mouse X") * panSpeed;
            transform.position += transform.forward * -Input.GetAxis("Mouse Y") * panSpeed;
        }

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if (!gameMenu.activeSelf)
            {
                Time.timeScale = 0f;
                if (GamePaused != null) GamePaused();
            }
            else
            {
                Time.timeScale = 1f;
                if (GameUnpaused != null) GameUnpaused();
            }
            gameMenu.SetActive(!gameMenu.activeSelf);
        }

        if(Input.GetKeyDown(KeyCode.LeftAlt))
        {
            if (ShowHUD != null) ShowHUD();
        }

        if (Input.GetKeyUp(KeyCode.LeftAlt))
        {
            if (HideHUD != null) HideHUD();
        }
        
        float zoomBy = Input.GetAxis("Mouse ScrollWheel");
        anchoredCamera.transform.position = Vector3.MoveTowards(anchoredCamera.transform.position, transform.position, zoomBy * zoomSpeed);
    }
}
