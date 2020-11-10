using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;
    private RuntimePlatform runtimePlatform;
    private Vector2 screenPos;
    private Enums.TouchStatus touchStatus = Enums.TouchStatus.Null;
    private Enums.TouchStatus currentTouchStatus = Enums.TouchStatus.Null;
    private Vector3 lastPos;
    private Vector3 delta;
    private bool Mousestep;
    public Enums.TouchStatus TouchStatus
    {
        get
        {
            return currentTouchStatus;
        }
    } 

    public Vector2 ScreenPos
    {
        get
        {
            return screenPos;
        }
    }
    private void Awake()
    {
        runtimePlatform = Application.platform;
        Instance = this;
    }

    private void Update()
    {
        switch (runtimePlatform)
        {
            case RuntimePlatform.WindowsEditor:

                if (Input.GetMouseButtonDown(0))
                {
                    lastPos = Input.mousePosition;
                    screenPos = Camera.main.ScreenToWorldPoint(lastPos);
                    touchStatus = Enums.TouchStatus.Touch;
                }
                if (Input.GetMouseButton(0))
                {
                    Mousestep = !Mousestep;
                    if (Mousestep)
                    {
                        delta = Input.mousePosition - lastPos;
                    }
                    else
                    {
                        lastPos = Input.mousePosition;
                    }
                    if (delta.magnitude > 2)
                    {
                        if (delta.x > 0 || delta.y > 0)
                        {
                            touchStatus = Enums.TouchStatus.Right;
                        }
                        if (delta.x < 0 || delta.y < 0)
                        {
                            touchStatus = Enums.TouchStatus.Left;
                        }
                    }

                }
                else
                {
                    if (currentTouchStatus == Enums.TouchStatus.Null)
                    {
                        delta = Vector2.zero;
                        currentTouchStatus = touchStatus;
                        touchStatus = Enums.TouchStatus.Null;
                    }
                    else
                    {
                        currentTouchStatus = Enums.TouchStatus.Null;
                    }
                }
                break;
            case RuntimePlatform.IPhonePlayer:
                break;
            case RuntimePlatform.Android:
                if(Input.touchCount > 0)
                {
                    delta = Input.GetTouch(0).deltaPosition;
                    if (delta.magnitude > 1)
                    {
                        if(delta.x > 0 || delta.y > 0)
                        {
                            touchStatus = Enums.TouchStatus.Right;
                        }
                        if(delta.x < 0 || delta.y < 0)
                        {
                            touchStatus = Enums.TouchStatus.Left;
                        }
                    }
                    else 
                    {
                        screenPos = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
                        touchStatus = Enums.TouchStatus.Touch;
                    }
                }
                else
                {
                    if (currentTouchStatus == Enums.TouchStatus.Null)
                    {
                        currentTouchStatus = touchStatus;
                        touchStatus = Enums.TouchStatus.Null;
                    }
                    else
                    {
                        currentTouchStatus = Enums.TouchStatus.Null;
                    }
                    
                }
                break;
            default:
                break;
        }
    }
}
