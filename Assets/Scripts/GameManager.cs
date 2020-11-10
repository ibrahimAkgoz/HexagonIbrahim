using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GridManager gridManager;
    [Header("Grid Setting")]
    public int colomnsCount;
    public int rowCount;
    public Color[] colors;
    private void Awake()
    {
        Instance = this;
    }

}
