using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameController : MonoBehaviour
{
    private InputManager input;
    private GridManager gridManager;
    public GameObject tripleHexagon;

    private Hexagon[] SelectedHexagon = new Hexagon[3];
    private bool isRight;

    private Vector2 lastScreenPos;
    private bool rotate;
    private int rotateCounter = 0;
    private bool rotateDirection;
    private void Start()
    {
        input = InputManager.Instance;
        gridManager = GameManager.Instance.gridManager;
        tripleHexagon.SetActive(false);
    }

    private void Update()
    {
        if (!gridManager.GameBegan) return;
        if (gridManager.IsPhysics) return;
        switch (input.TouchStatus)
        {
            
            case Enums.TouchStatus.Left:
                if (tripleHexagon.activeSelf)
                {
                    rotateDirection = true;
                    rotate = true;
                    rotateCounter = 0;
                }    
                break;
            case Enums.TouchStatus.Right:
                if (tripleHexagon.activeSelf)
                {
                    rotateDirection = true;
                    rotate = true;
                    rotateCounter = 0;
                }
                break;
            case Enums.TouchStatus.Touch:
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity);
                if (hit)
                {
                    Debug.Log(hit.collider.tag);
                    if (hit.collider.tag == "GridCollider")
                    {

                        SelectHexagons(input.ScreenPos);
                        lastScreenPos = input.ScreenPos;
                    }
                }
                

                break;
        }
        if (rotate)
        {
            
            SelectHexagons(lastScreenPos);
            if (gridManager.RotateTripleHexagon(SelectedHexagon, isRight, rotateDirection))
            {
                rotate = false;
                tripleHexagon.SetActive(false);
                gridManager.StepCounter++;
            }
            else
            {
                rotateCounter++;
                if (rotateCounter == 3)
                {
                    rotate = false;
                }
            }
            
        }
        
    }

    public void SelectHexagons(Vector2 screenPos)
    {
        tripleHexagon.SetActive(true);
        for (int i = 0; i < SelectedHexagon.Length; i++)
        {
            if (SelectedHexagon[i] != null)
            {
                SelectedHexagon[i].transform.parent = gridManager.transform;
            }
        }

        SelectedHexagon = gridManager.GetTripleHexagons(screenPos).ToArray();

        Hexagon singleHexagon;
        isRight = false;
        singleHexagon = SelectedHexagon[1];
        if (singleHexagon.transform.position.x > SelectedHexagon[1].transform.position.x)
        {
            isRight = true;
        }
        if (singleHexagon.transform.position.x > SelectedHexagon[2].transform.position.x)
        {
            isRight = true;
        }
        if (singleHexagon.transform.position.x > SelectedHexagon[2].transform.position.x)
        {
            isRight = true;
        }

        if (isRight)
        {
            transform.position = gridManager.GetTargetHexagonPos(singleHexagon) + new Vector3(-gridManager.colomnsSpacingSize * 0.66f, 0, -1);
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        else
        {
            transform.position = gridManager.GetTargetHexagonPos(singleHexagon) + new Vector3(gridManager.colomnsSpacingSize * 0.66f, 0, -1);
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }

        for (int i = 0; i < SelectedHexagon.Length; i++)
        {
            SelectedHexagon[i].transform.parent = transform;
        }
    }
}
