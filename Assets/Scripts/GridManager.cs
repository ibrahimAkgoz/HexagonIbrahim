using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public GameObject hexagonPrefab;
    public BoxCollider2D gridCollider; 

    [Header("Grid Setting")]
    public float colomnsSpacingSize;
    public float rowSpacingSize;
    private int colomnsCount;
    private int rowCount;
    private Hexagon[,] hexagons;


    private bool gameBegan;
    private bool lastisPyhsics = true;
    private List<Hexagon> destroyList = new List<Hexagon>();
    private int score;
    private int lastBombScore;

    private int stepCounter;

    public bool GameBegan
    {
        get
        {
            return gameBegan;
        }
    }

    public int StepCounter
    {
        get
        {
            return stepCounter;
        }
        set
        {
            stepCounter = value;
            CheckBombTime();
            UIManager.Instance.MovesText = value;
        }
    }

    public bool IsPhysics
    {
        get
        {
            return IsPhysicsProgress();
        }
    }
    private void OnDisable()
    {
        if(Data.GetInt("HighScore",0) < score)
        {
            Data.SetInt("HighScore", score);
        }
    }
    private void OnApplicationQuit()
    {
        if (Data.GetInt("HighScore", 0) < score)
        {
            Data.SetInt("HighScore", score);
        }
    }

    void Start()
    {
        colomnsCount = GameManager.Instance.colomnsCount;
        rowCount = GameManager.Instance.rowCount;
        gridCollider.size = new Vector2((colomnsCount-1) *colomnsSpacingSize , (rowCount-2)*rowSpacingSize + (rowSpacingSize * 0.5f));
        gridCollider.offset =  new Vector2(gridCollider.size.x * 0.5f, gridCollider.size.y*0.57f);
        hexagons = new Hexagon[colomnsCount,rowCount];
        for (int i = 0; i < colomnsCount; i++)
        {
            for (int ix = 0; ix < rowCount; ix++)
            {
                StartCoroutine(HexagonGenerator(hexagonPrefab,i, ix,(ix+i)*0.2f));
            }

        }
    }


    IEnumerator HexagonGenerator(GameObject gameObject, int colomnIndex, int rowIndex, float Time)
    {
        GameObject tempGameObj = Instantiate(gameObject, new Vector3(GetTargetHexagonPos(colomnIndex, rowIndex).x, GetTargetHexagonPos(colomnIndex, rowIndex).y + 10f, 0), Quaternion.identity);
        tempGameObj.transform.parent = transform;
        Hexagon tempHexagon = tempGameObj.GetComponent<Hexagon>();

        if (IsBomb())
        {
            tempHexagon.hexegonType = Enums.HexegonType.Bomb;
        }
        else
        {
            int random = UnityEngine.Random.Range(0, 10);
            if (random == 0)
            {
                tempHexagon.hexegonType = Enums.HexegonType.Star;
            }
            else
            {
                tempHexagon.hexegonType = Enums.HexegonType.Normal;
            }
        }
        tempHexagon.colorIndex = RandomColor();
        hexagons[colomnIndex, rowIndex] = tempHexagon;

        yield return new WaitForSecondsRealtime(Time);
        tempHexagon.SetMoveTarget(new Vector3(GetTargetHexagonPos(colomnIndex, rowIndex).x, GetTargetHexagonPos(colomnIndex, rowIndex).y, 0));
        if(rowIndex == rowCount-1 && colomnIndex == colomnsCount - 1)
        {
            yield return new WaitForSecondsRealtime(1);
            CheckTripleHexagon();
            gameBegan = true;
        }
    }
    private void CheckBombTime()
    {
        for (int i = 0; i < colomnsCount; i++)
        {
            for (int ix = 0; ix < rowCount; ix++)
            {
                if (hexagons[i,ix].hexegonType == Enums.HexegonType.Bomb)
                {
                    hexagons[i, ix].BombCount--;
                    if (hexagons[i, ix].BombCount == 0)
                    {
                        UIManager.Instance.NewGame();
                    }
                    
                }
            }
        }
    }
    private bool IsBomb()
    {
        if(lastBombScore+1000 <= score)
        {
            lastBombScore = ((int)(score *0.001f))*1000;
            return true;
        }
        else
        {
            return false;
        }
    }
    private Vector3 GetTargetHexagonPos(int colomnIndex, int rowIndex)
    {
        float rowPos = rowIndex * rowSpacingSize + (colomnIndex % 2 * (rowSpacingSize / 2f));
        float colomnPos = colomnIndex * colomnsSpacingSize;
        return new Vector2(colomnPos, rowPos);
    }
    public Vector3 GetTargetHexagonPos(Hexagon hexagon)
    {
        for (int i = 0; i < colomnsCount; i++)
        {
            for (int ix = 0; ix < rowCount; ix++)
            {
                if(hexagon == hexagons[i, ix])
                {
                    return GetTargetHexagonPos(i, ix);
                }
            }
        }
        return Vector2.zero;
    }
    private int RandomColor()
    {
        int random = UnityEngine.Random.Range(0, GameManager.Instance.colors.Length);
        return random;
    }
    private void DestroyHexagon(Hexagon hexagon)
    {
        if (hexagon.destroy)
        {
            return;
        }
        switch (hexagon.hexegonType)
        {
            case Enums.HexegonType.Normal:
                score += 5;
                break;
            case Enums.HexegonType.Star:
                score += 10;
                break;
        }
        UIManager.Instance.ScoreText = score;
        hexagon._OnDestroy();
    }
    private void HexagonPhysicsStep()
    {
        for (int i = 0; i < colomnsCount; i++)
        {
            for (int ix = 0; ix < rowCount; ix++)
            {
                if (ix == rowCount - 1)
                {
                    if (hexagons[i, ix] == null)
                    {
                        StartCoroutine(HexagonGenerator(hexagonPrefab, i, ix, 0));
                    }

                }
                if (hexagons[i, ix] == null)
                {
                    if (hexagons[i, ix + 1] != null)
                    {
                        hexagons[i, ix] = hexagons[i, ix + 1];
                        hexagons[i, ix + 1] = null;
                    }
                }

            }
        }
        for (int i = 0; i < colomnsCount; i++)
        {
            for (int ix = 0; ix < rowCount; ix++)
            {
                if(hexagons[i, ix] != null)
                {
                    if (hexagons[i, ix].transform.position != GetTargetHexagonPos(i, ix))
                        hexagons[i, ix].SetMoveTarget(GetTargetHexagonPos(i, ix));
                }
                
            }
        }
        
    }
    private bool CheckTripleHexagon()
    {
        bool temp = false;
        for (int i = 0; i < colomnsCount-1; i++)
        {
            for (int ix = 0; ix < rowCount-1; ix++)
            {
                Vector2 CheckPos = GetTargetHexagonPos(i, ix+((i+1)%2)) + new Vector3(colomnsSpacingSize * 0.5f, 0, 0);
                Vector2 CheckPos1 = GetTargetHexagonPos(i, ix) + new Vector3(colomnsSpacingSize * 0.5f, rowSpacingSize * 0.5f, 0);
                Hexagon[] tripleHexagons = GetTripleHexagons(CheckPos).ToArray();
                if(tripleHexagons[0].colorIndex == tripleHexagons[1].colorIndex && tripleHexagons[1].colorIndex == tripleHexagons[2].colorIndex)
                {
                    for (int k = 0; k < tripleHexagons.Length; k++)
                    {
                        DestroyHexagon(tripleHexagons[k]);
                        temp = true;
                    }
                }
                tripleHexagons = GetTripleHexagons(CheckPos1).ToArray();
                if (tripleHexagons[0].colorIndex == tripleHexagons[1].colorIndex && tripleHexagons[1].colorIndex == tripleHexagons[2].colorIndex)
                {
                    for (int k = 0; k < tripleHexagons.Length; k++)
                    {
                        DestroyHexagon(tripleHexagons[k]);
                        temp = true;
                    }
                }
            }
        }
        HexagonPhysicsStep();
        return temp;
    }
    public bool RotateTripleHexagon(Hexagon[] hexagons,bool isRight,bool direction)
    {
        for (int i = 0; i < colomnsCount; i++)
        {
            for (int ix = 0; ix < rowCount; ix++)
            {
                if(this.hexagons[i,ix].transform == hexagons[2].transform)
                {
                    if (isRight)
                    {
                        if (direction)
                        {
                            Hexagon temp = this.hexagons[i, ix];
                            this.hexagons[i, ix] = this.hexagons[i + 1, ix + (i % 2)];
                            this.hexagons[i + 1, ix + (i % 2)] = this.hexagons[i , ix+1];
                            this.hexagons[i, ix+1] = temp;
                            return CheckTripleHexagon();
                        }
                        else
                        {
                            Hexagon temp = this.hexagons[i, ix];
                            this.hexagons[i, ix] = this.hexagons[i, ix + 1];
                            this.hexagons[i, ix + 1] = this.hexagons[i + 1, ix + (i % 2)];
                            this.hexagons[i + 1, ix + (i % 2)] = temp;
                            return CheckTripleHexagon();
                        }
                    }
                    else
                    {
                        if (direction)
                        {
                            Hexagon temp = this.hexagons[i, ix];
                            this.hexagons[i, ix] = this.hexagons[i, ix + 1];
                            this.hexagons[i, ix + 1] = this.hexagons[i - 1, ix + (i % 2)];
                            this.hexagons[i - 1, ix + (i % 2)] = temp;
                            return CheckTripleHexagon();
                        }
                        else
                        {
                            Hexagon temp = this.hexagons[i, ix];

                            this.hexagons[i, ix] = this.hexagons[i - 1, ix + (i % 2)];
                            this.hexagons[i - 1, ix + (i % 2)] = this.hexagons[i, ix + 1];
                            this.hexagons[i, ix + 1] = temp;
                            return CheckTripleHexagon();
                        }
                    }
                }
            }
        }
        return false;
    }
    private Hexagon[] ShortHexagon(Hexagon[] hexagons)
    {
        for (int i = 0; i < hexagons.Length; i++)
        {
            for (int ix = 0; ix < hexagons.Length; ix++)
            {
                if (hexagons[i].transform.position.y > hexagons[ix].transform.position.y)
                {
                    Hexagon temp = hexagons[i];
                    hexagons[i] = hexagons[ix];
                    hexagons[ix] = temp;
                }
            }
        }
        return hexagons;
    }

    private bool IsPhysicsProgress()
    {
        for (int i = 0; i < colomnsCount; i++)
        {
            for (int ix = 0; ix < rowCount; ix++)
            {
                if (hexagons[i, ix] == null)
                {
                    return true;
                }
                if (hexagons[i, ix].Move)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public List<Hexagon> GetTripleHexagons(Vector2 point)
    {
        List<Hexagon> Temphexagons = new List<Hexagon>();
        for (int i = 0; i < colomnsCount; i++)
        {
            for (int ix = 0; ix < rowCount; ix++)
            {
                if(hexagons[i, ix] != null)
                {
                    hexagons[i, ix].distance = Vector2.Distance(point, hexagons[i, ix].transform.position);
                    Temphexagons.Add(hexagons[i, ix]);
                }
            }
        }

        for (int i = 0; i < Temphexagons.Count; i++)
        {
            for (int ix = i; ix < Temphexagons.Count; ix++)
            {
                if(Temphexagons[i].distance > Temphexagons[ix].distance)
                {
                    Hexagon temp = Temphexagons[i];
                    Temphexagons[i] = Temphexagons[ix];
                    Temphexagons[ix] = temp;
                }
            }
        }

        List<Hexagon> finalHexagons = new List<Hexagon>();
        for (int i = 0; i < 3; i++)
        {
            finalHexagons.Add(Temphexagons[i]);
        }
        Temphexagons.Clear();
        finalHexagons = ShortHexagon(finalHexagons.ToArray()).ToList();

        return finalHexagons;
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameBegan) return;
        if (IsPhysicsProgress())
        {
            lastisPyhsics = true;
            HexagonPhysicsStep();
            
        }
        else
        {
            
            if (lastisPyhsics)
            {
                CheckTripleHexagon();
                lastisPyhsics = false;
            }
            
        }

        
    }
}
