using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    public int radius;
    public float explodeTime = 2.0f;
    [HideInInspector]
    public int x, y;
    public int host;//放炸弹的人
    public bool isStatic = true;
    private GameManager gameManager;
    public GameManager GameManager { get => gameManager; set => gameManager = value; }
    private BoxCollider2D boxCollider;
    private bool isExploded = false;
    public Vector2Int targetCoord;

    private void Awake()
    {
        gameManager = GameManager.Instance;
        boxCollider = GetComponent<BoxCollider2D>();
        boxCollider.isTrigger = true;
    }

    private void Update()
    {
        if (!isExploded && explodeTime <= 0)
        {
            Explode();
            isExploded = true;
        }

        explodeTime -= Time.deltaTime;

    }

    private void FixedUpdate()
    {
        if (Menu.mode != 2)
            return;
        Vector3 targetPosition = gameManager.CorrectPosition(targetCoord.x, targetCoord.y) - new Vector3(0, 0.3f, 0);
        if (Vector3.Distance(this.transform.position, targetPosition) > 0.1f)
        {
            transform.Translate(3f * Time.fixedDeltaTime * (targetPosition-transform.position).normalized, Space.World);
        }
        else
        {
            if (!isStatic)
            {
                transform.position = targetPosition;
                x = targetCoord.x;
                y = targetCoord.y;
                gameManager.itemsType[x, y] = GameManager.ItemType.BOMB;
                gameManager.itemsObject[x, y] = this.gameObject;
                isStatic = true;
                //更新bombRange和explosionRange
                #region
                gameManager.bombRange[x, y].Add(GetComponent<Bomb>());
                gameManager.explosionRange[x, y]++;
                //向上遍历
                for (int i = 1; i <= radius; i++)
                {
                    if (y - i < 0 || gameManager.itemsType[x, y - i] == GameManager.ItemType.BARRIAR)
                        break;
                    gameManager.bombRange[x, y - i].Add(GetComponent<Bomb>());
                    gameManager.explosionRange[x, y - i]++;
                }
                //向左遍历
                for (int i = 1; i <= radius; i++)
                {
                    if (x - i < 0 || gameManager.itemsType[x - i, y] == GameManager.ItemType.BARRIAR)
                        break;
                    gameManager.bombRange[x - i, y].Add(GetComponent<Bomb>());
                    gameManager.explosionRange[x - i, y]++;
                }
                //向右遍历
                for (int i = 1; i <= radius; i++)
                {
                    if (x + i >= gameManager.xColumn || gameManager.itemsType[x + i, y] == GameManager.ItemType.BARRIAR)
                        break;
                    gameManager.bombRange[x + i, y].Add(GetComponent<Bomb>());
                    gameManager.explosionRange[x + i, y]++;
                }
                //向下遍历
                for (int i = 1; i <= radius; i++)
                {
                    if (y + i >= gameManager.yRow || gameManager.itemsType[x, y + i] == GameManager.ItemType.BARRIAR)
                        break;
                    gameManager.bombRange[x, y + i].Add(GetComponent<Bomb>());
                    gameManager.explosionRange[x, y + i]++;
                }
                #endregion
            }
        }

    }


    private void Explode()
    {
        if (PhotonNetwork.IsConnected && !PhotonNetwork.IsMasterClient)
            return;
        
        if (this.gameObject != null)
        {
            if (PhotonNetwork.IsConnected == false)
            {
                StartCoroutine(gameManager.BombExplode(this.gameObject));
            }
            else if (PhotonNetwork.IsConnected == true && PhotonNetwork.IsMasterClient)
            {
                StartCoroutine(gameManager.BombExplode(this.gameObject));
            }
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Collider2D[] results = new Collider2D[4];
        int total = Physics2D.OverlapCollider(boxCollider, new ContactFilter2D().NoFilter(), results);
        //Debug.Log("total:" + total);
        if(total == 0)
            boxCollider.isTrigger = false;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        //Debug.Log("stay " + collision.GetComponent<Person>().NO);
        boxCollider.isTrigger = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (Menu.mode != 2)
            return;
        Person player = collision.gameObject.GetComponent<Person>();
        if (!player.canPushBomb)
            return;
        if (player.orientation == 0 && collision.transform.position.y > transform.position.y)
        {
            //正在向下走
            if (y+1 <gameManager.yRow && gameManager.itemsType[x, y+1] == GameManager.ItemType.EMPTY)
            {
                targetCoord = new Vector2Int(x, y + 1);
                gameManager.itemsType[x, y] = GameManager.ItemType.EMPTY;
                gameManager.itemsObject[x, y] = null;
                isStatic = false;
                //更新bombRange和explosionRange
                #region
                gameManager.bombRange[x, y].Remove(GetComponent<Bomb>());
                gameManager.explosionRange[x, y]--;
                //向上遍历
                for (int i = 1; i <= radius; i++)
                {
                    if (y - i < 0 || gameManager.itemsType[x, y - i] == GameManager.ItemType.BARRIAR)
                        break;
                    gameManager.bombRange[x, y - i].Remove(GetComponent<Bomb>());
                    gameManager.explosionRange[x, y - i]--;
                }
                //向左遍历
                for (int i = 1; i <= radius; i++)
                {
                    if (x - i < 0 || gameManager.itemsType[x - i, y] == GameManager.ItemType.BARRIAR)
                        break;
                    gameManager.bombRange[x - i, y].Remove(GetComponent<Bomb>());
                    gameManager.explosionRange[x - i, y]--;
                }
                //向右遍历
                for (int i = 1; i <= radius; i++)
                {
                    if (x + i >= gameManager.xColumn || gameManager.itemsType[x + i, y] == GameManager.ItemType.BARRIAR)
                        break;
                    gameManager.bombRange[x + i, y].Remove(GetComponent<Bomb>());
                    gameManager.explosionRange[x + i, y]--;
                }
                //向下遍历
                for (int i = 1; i <= radius; i++)
                {
                    if (y + i >= gameManager.yRow || gameManager.itemsType[x, y + i] == GameManager.ItemType.BARRIAR)
                        break;
                    gameManager.bombRange[x, y + i].Remove(GetComponent<Bomb>());
                    gameManager.explosionRange[x, y + i]--;
                }
                #endregion
            }
        }
        if (player.orientation == 1 && collision.transform.position.x > transform.position.x)
        {
            //正在向左走
            if (x-1>=0 && gameManager.itemsType[x-1, y] == GameManager.ItemType.EMPTY)
            {
                targetCoord = new Vector2Int(x-1, y);
                gameManager.itemsType[x, y] = GameManager.ItemType.EMPTY;
                gameManager.itemsObject[x, y] = null;
                isStatic = false;
                //更新bombRange和explosionRange
                #region
                gameManager.bombRange[x, y].Remove(GetComponent<Bomb>());
                gameManager.explosionRange[x, y]--;
                //向上遍历
                for (int i = 1; i <= radius; i++)
                {
                    if (y - i < 0 || gameManager.itemsType[x, y - i] == GameManager.ItemType.BARRIAR)
                        break;
                    gameManager.bombRange[x, y - i].Remove(GetComponent<Bomb>());
                    gameManager.explosionRange[x, y - i]--;
                }
                //向左遍历
                for (int i = 1; i <= radius; i++)
                {
                    if (x - i < 0 || gameManager.itemsType[x - i, y] == GameManager.ItemType.BARRIAR)
                        break;
                    gameManager.bombRange[x - i, y].Remove(GetComponent<Bomb>());
                    gameManager.explosionRange[x - i, y]--;
                }
                //向右遍历
                for (int i = 1; i <= radius; i++)
                {
                    if (x + i >= gameManager.xColumn || gameManager.itemsType[x + i, y] == GameManager.ItemType.BARRIAR)
                        break;
                    gameManager.bombRange[x + i, y].Remove(GetComponent<Bomb>());
                    gameManager.explosionRange[x + i, y]--;
                }
                //向下遍历
                for (int i = 1; i <= radius; i++)
                {
                    if (y + i >= gameManager.yRow || gameManager.itemsType[x, y + i] == GameManager.ItemType.BARRIAR)
                        break;
                    gameManager.bombRange[x, y + i].Remove(GetComponent<Bomb>());
                    gameManager.explosionRange[x, y + i]--;
                }
                #endregion
            }
        }
        if (player.orientation == 2 && collision.transform.position.x < transform.position.x)
        {
            //正在向右走
            if (x + 1 < gameManager.xColumn && gameManager.itemsType[x + 1, y] == GameManager.ItemType.EMPTY)
            {
                targetCoord = new Vector2Int(x + 1, y);
                gameManager.itemsType[x, y] = GameManager.ItemType.EMPTY;
                gameManager.itemsObject[x, y] = null;
                isStatic = false;
                //更新bombRange和explosionRange
                #region
                gameManager.bombRange[x, y].Remove(GetComponent<Bomb>());
                gameManager.explosionRange[x, y]--;
                //向上遍历
                for (int i = 1; i <= radius; i++)
                {
                    if (y - i < 0 || gameManager.itemsType[x, y - i] == GameManager.ItemType.BARRIAR)
                        break;
                    gameManager.bombRange[x, y - i].Remove(GetComponent<Bomb>());
                    gameManager.explosionRange[x, y - i]--;
                }
                //向左遍历
                for (int i = 1; i <= radius; i++)
                {
                    if (x - i < 0 || gameManager.itemsType[x - i, y] == GameManager.ItemType.BARRIAR)
                        break;
                    gameManager.bombRange[x - i, y].Remove(GetComponent<Bomb>());
                    gameManager.explosionRange[x - i, y]--;
                }
                //向右遍历
                for (int i = 1; i <= radius; i++)
                {
                    if (x + i >= gameManager.xColumn || gameManager.itemsType[x + i, y] == GameManager.ItemType.BARRIAR)
                        break;
                    gameManager.bombRange[x + i, y].Remove(GetComponent<Bomb>());
                    gameManager.explosionRange[x + i, y]--;
                }
                //向下遍历
                for (int i = 1; i <= radius; i++)
                {
                    if (y + i >= gameManager.yRow || gameManager.itemsType[x, y + i] == GameManager.ItemType.BARRIAR)
                        break;
                    gameManager.bombRange[x, y + i].Remove(GetComponent<Bomb>());
                    gameManager.explosionRange[x, y + i]--;
                }
                #endregion
            }
        }
        if (player.orientation == 3 && collision.transform.position.y < transform.position.y)
        {
            //正在向上走
            if (y-1>=0 && gameManager.itemsType[x, y-1] == GameManager.ItemType.EMPTY)
            {
                targetCoord = new Vector2Int(x, y-1);
                gameManager.itemsType[x, y] = GameManager.ItemType.EMPTY;
                gameManager.itemsObject[x, y] = null;
                isStatic = false;
                //更新bombRange和explosionRange
                #region
                gameManager.bombRange[x, y].Remove(GetComponent<Bomb>());
                gameManager.explosionRange[x, y]--;
                //向上遍历
                for (int i = 1; i <= radius; i++)
                {
                    if (y - i < 0 || gameManager.itemsType[x, y - i] == GameManager.ItemType.BARRIAR)
                        break;
                    gameManager.bombRange[x, y - i].Remove(GetComponent<Bomb>());
                    gameManager.explosionRange[x, y - i]--;
                }
                //向左遍历
                for (int i = 1; i <= radius; i++)
                {
                    if (x - i < 0 || gameManager.itemsType[x - i, y] == GameManager.ItemType.BARRIAR)
                        break;
                    gameManager.bombRange[x - i, y].Remove(GetComponent<Bomb>());
                    gameManager.explosionRange[x - i, y]--;
                }
                //向右遍历
                for (int i = 1; i <= radius; i++)
                {
                    if (x + i >= gameManager.xColumn || gameManager.itemsType[x + i, y] == GameManager.ItemType.BARRIAR)
                        break;
                    gameManager.bombRange[x + i, y].Remove(GetComponent<Bomb>());
                    gameManager.explosionRange[x + i, y]--;
                }
                //向下遍历
                for (int i = 1; i <= radius; i++)
                {
                    if (y + i >= gameManager.yRow || gameManager.itemsType[x, y + i] == GameManager.ItemType.BARRIAR)
                        break;
                    gameManager.bombRange[x, y + i].Remove(GetComponent<Bomb>());
                    gameManager.explosionRange[x, y + i]--;
                }
                #endregion
            }
        }
    }

}
