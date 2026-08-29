using System;
using System.Collections;
using UnityEngine;

public class Cheats : MonoBehaviour
{
    // Activate corner area size by screen width percentage
    public float ActivateAreaSize = 0.1f;

    // How many clicks the player should do before cheats list will be visible
    public int ClicksCount = 5;

    // How many seconds player has to click/touch the screen
    public float WaitTime = 2;
    private float[] _clickTimes;
    private int _clickTimesIndex;
    private bool _active = false;

    public GameController gameController;

    // Start is called before the first frame update
    void Start()
    {
        // Create clicks array and reset it with float.MinValue
        _clickTimes = new float[ClicksCount];
        ResetClicks();
        gameController = GameObject.FindWithTag("GameController").GetComponent<GameController>();
    }

    private void ResetClicks()
    {
        for (int i = 0; i < ClicksCount; i++)
        {
            _clickTimes[i] = float.MinValue;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Check for click or touch and register it
        if (CheckClickOrTouch())
        {
            // Click will be registered at time since level load
            _clickTimes[_clickTimesIndex] = Time.timeSinceLevelLoad;
            // Each next click will be written on next array index or 0 if overflow
            _clickTimesIndex = (_clickTimesIndex + 1) % ClicksCount;
        }

        // Check if cheat list should be activated
        if (ShouldActivate())
        {
            _active = true;
            ResetClicks();
        }
    }

    // Checks if cheat list should be activated
    private bool ShouldActivate()
    {
        // Check if all click/touches were made within WaitTime
        foreach(float clickTime in _clickTimes)
        {
            if (clickTime < Time.timeSinceLevelLoad - WaitTime)
            {
                // Return false if any of click/touch times have been done earlier
                return false;
            }
        }

        // If we are here, cheat should be activated
        return true;
    }

    // Returns true if there's click or touch within the activate area
    private bool CheckClickOrTouch()
    {
        // Convert activation area to pixels
        float sizeInPixels = ActivateAreaSize * Screen.width;

        // Get the click/touch position
        Vector2? position = ClickOrTouchPoint();

        if (position.HasValue) // position.HasValue returns true is there is a click or touch
        {
            // Check if within the range
            if (position.Value.x >= Screen.width - sizeInPixels && Screen.height - position.Value.y <= sizeInPixels)
            {
                return true;
            }
        }

        return false;
    }

    // Checks for click or touch and returns the screen position in pixels
    private Vector2? ClickOrTouchPoint()
    {
        if (Input.GetMouseButtonDown(0)) // left mouse click
        {
            return Input.mousePosition;
        }
        else if (Input.touchCount > 0) // One or more touch
        {
            // Check only the first touch
            Touch touch = Input.touches[0];

            // It should react only when the touch has just begun
            if (touch.phase == TouchPhase.Began)
            {
                return touch.position;
            }
        }

        // Null if there's no click or touch
        return null;
    }

    void OnGUI()
    {
        if (_active)
        {
            DisplayCheat("Close Cheat Menu", () => _active = false);
            DisplayCheat("Move To Next Wave", () => gameController.playedTime += 150);
        }
    }

    private void DisplayCheat(string cheatName, Action clickedCallback)
    {
        if (GUILayout.Button("Cheat: " + cheatName))
        {
            clickedCallback();
        }
    }
}
