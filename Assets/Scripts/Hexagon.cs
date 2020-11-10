using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Hexagon : MonoBehaviour
{
    [HideInInspector] public Enums.HexegonType hexegonType;
    [SerializeField] private Sprite bomb;
    [SerializeField] private Sprite hexagon;
    [SerializeField] private Sprite hexagonStar;
    [HideInInspector] public int colorIndex;
    [SerializeField] private GameObject destroyEffect;
    [SerializeField] private TextMesh textMesh;

    [HideInInspector]public float distance;

    private Vector3 moveTarget;
    private bool move = false;
    [HideInInspector]public bool destroy;

    private int bombCount = 7; 

    public int BombCount
    {
        get
        {
            return bombCount;
        }
        set
        {
            textMesh.text = value.ToString();
            bombCount = value;
        }
        
    }
    public bool Move
    {
        get
        {
            return move;
        }
    }

    public void _OnDestroy()
    {
        destroy = true;
        GameObject destroyParticle = Instantiate(destroyEffect,transform.position+Vector3.forward*-3,Quaternion.identity);
        destroyParticle.GetComponent<ParticleSystem>().startColor = GameManager.Instance.colors[colorIndex];
        Destroy(gameObject);
    }

    public void Start()
    {
        switch (hexegonType)
        {
            case Enums.HexegonType.Normal:
                GetComponent<SpriteRenderer>().sprite = hexagon;
                break;
            case Enums.HexegonType.Star:
                GetComponent<SpriteRenderer>().sprite = hexagonStar;
                break;
            case Enums.HexegonType.Bomb:
                GetComponent<SpriteRenderer>().sprite = bomb;
                textMesh.gameObject.SetActive(true);
                break;
            default:
                break;
        }
        GetComponent<SpriteRenderer>().color = GameManager.Instance.colors[colorIndex];
    }
    private void Update()
    {
        if(transform.position == moveTarget)
        {
            move = false;
        }
        if (move)
        {
            transform.position = Vector3.MoveTowards(transform.position, moveTarget, 15* Time.deltaTime);
        }
        
        
    }

    public void SetMoveTarget(Vector3 target)
    {
        move = true;
        moveTarget = target;
    }

}
