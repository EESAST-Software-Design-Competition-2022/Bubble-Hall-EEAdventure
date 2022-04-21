using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Person : MonoBehaviourPun
{
    //原生变量
    #region
    public int PlayerNO;//PlayerNO = 0:AI;PlayerNO > 0:player
    public float speed = 2.0f;
    public int NO = 5;
    public int index;//the number of birth order
    public int bombNumber = 1;
    public int bombRadius = 1;
    public int life = 1;
    public int coin = 0;
    public bool isDefended = false;
    public bool Online = false;
    public float delayTime;

    
    public Sprite[] sprites;
    public GameObject bombPrefab;
    public Sprite[] deathSprites;

    
    [HideInInspector]
    public GameManager gameManager;
    private SpriteRenderer sr;

    private int leftCount = 0;
    private int rightCount = 0;
    private int upCount = 0;
    private int downCount = 0;
    public int orientation = 0;//0:下 1:左 2:右 3:上
    private bool isFree = true;
    private bool isDead = false;
    private bool setNo = false;
    public bool canPushBomb = false;
    public bool haveGun = false;
    public bool haveUnknownWeapon = false;
    public float canEnterDoor = 0f;//是否可以进入传送门，3s内不能在进入
    #endregion

    //AI相关变量
    #region
    //0:下 1:左 2:右 3:上
    private Vector2Int[] vector2Ints = { Vector2Int.up, Vector2Int.left, Vector2Int.right, Vector2Int.down };
    public int xColumn;
    public int yRow;
    //调控最低更新频率
    public int delta;
    //检查卡在撞墙状态
    public int check;
    //反向运动标志
    public bool reverse;
    //当前运动状态//-1:静止 0:下 1:左 2:右 3:上
    public int moveState;
    //当前点坐标（int）
    public Vector2Int currentPoint;
    //当前实际坐标（float）
    public Vector2 currentLocation;
    //bfs前驱
    public Vector2Int[,] preMap;
    //是否走过
    public bool[,] isUsed;
    //是否放置了炸弹 
    public bool isBomb = false;
    //是否需要放置炸弹
    public bool canFire = false;
    //目标类型(safe:0  BOX:1  TOOL:2)
    public int targetType;
    //是否找到了目标
    public bool isFound = false;
    //是否与箱子相邻
    public bool nearBox = false;
    //是否下一步到达BOX
    public bool nearlyBox = false;
    //是否下一步到达Safe
    public bool nearlySafe = false;
    //是否下一步到达TOOL
    public bool nearlyTool = false;
    //出发点坐标
    public Vector2Int startPoint;
    //目标点坐标
    public Vector2Int targetPoint;
    //随机游走放炸弹
    public int counterStep;
    //冷静期
    public int counterPeace;

    //bfs搜索队列
    public class bfsNode
    {
        public Vector2Int now;
        public Vector2Int pre;
        public bfsNode(Vector2Int now_, Vector2Int pre_)
        {
            now = now_;
            pre = pre_;
        }
    };
    public Queue<bfsNode> queue = new Queue<bfsNode>();
    public Stack<Vector2Int> stack = new Stack<Vector2Int>();
    #endregion

    //基础函数
    #region
    private void Awake()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        if (PhotonNetwork.IsConnected == true) Online = true;
        if (Online)
        {
            if (photonView.IsMine && !PhotonNetwork.IsMasterClient)
            {
                photonView.RPC("setIndex", RpcTarget.All, this.gameManager.OnlineLocalPlayerIndex);
            }
            PhotonNetwork.SendRate = 50;
            if (!photonView.IsMine && !PhotonNetwork.IsMasterClient)
            {
                gameManager.roleList.Add(this.gameObject);
            }

        }
    }

    // Start is called before the first frame update
    void Start()
    {
        xColumn = gameManager.xColumn;
        yRow = gameManager.yRow;
        preMap = new Vector2Int[xColumn, yRow];//记录bfs前驱
        moveState = -1;
        reverse = false;
        counterStep = 0;
        counterPeace = 0;
        sr = this.GetComponent<SpriteRenderer>();
        if (!Online)
        {
            sr.sprite = sprites[NO * 16];
        }
        else
        {
            if (photonView.IsMine)
            {

                photonView.RPC("changeSprite", RpcTarget.All, NO * 16);
            }
        }
        StartCoroutine(Defend());

        if (PhotonNetwork.IsConnected && PhotonNetwork.IsMasterClient)
        {
            if (!photonView.IsMine)
            {
                PlayerNO = 5;
                life = 1;
                gameManager.roleList.AddByIndex(this.gameObject, index);
            }
            delayTime = 5.0f;
        }
        else
        {
            delayTime = 0;
        }
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (delayTime > 0)
        {
            delayTime -= Time.fixedDeltaTime;
            if (delayTime <= 0)
            {
                StartCoroutine(Defend());
            }
            return;
        }
        if (canEnterDoor > 0)
        {
            canEnterDoor -= Time.deltaTime;
        }

        //获取当前实际坐标
        currentLocation = GetFloatLocation(transform.position);
        currentPoint = GetIntLocation(currentLocation);
        //  Debug.Log("life : "+life);
        if (PlayerNO > 0)
        {
            if (Menu.mode == 3 && PlayerNO != 1)
            {
                return;
            }
            MoveByKey();
        }
        else
        {

            if (PhotonNetwork.IsConnected && PhotonNetwork.IsMasterClient)
            {
                MoveByAi();
            }
            else if (!PhotonNetwork.IsConnected)
            {
                MoveByAi();
            }
            //是否需要mode

        }

    }

    private void Update()
    {
        
        if (delayTime > 0)
        {
            delayTime -= Time.deltaTime;
            life = 1;
            return;
        }

        if (!isDead && life <= 0)
        {
            //Debug.Log("die");
            Die();
        }
        if (PlayerNO > 0)
        {
            if (Menu.mode == 3 && PlayerNO != 1)
            {
                return;
            }
            FireByKey();
        }
        else
        {
            FireByAi();
        }

    }
    #endregion

    //原生函数
    #region
    //0:down 1:left 2:right 3:up
    public void Move(int direction)
    {
        if (!isFree)
            return;
        if (direction == 1)
        {
            rightCount = upCount = downCount = 0;
            leftCount = (leftCount + 1) % 20;
            if (!Online)
            {
                sr.sprite = sprites[NO * 16 + leftCount / 5 + 4];
            }
            if (Online && photonView.IsMine)
            {
                photonView.RPC("changeSprite", RpcTarget.All, NO * 16 + leftCount / 5 + 4);
            }
            transform.Translate(speed * Time.fixedDeltaTime * Vector3.left, Space.World);
            orientation = 1;
        }
        if (direction == 2)
        {
            leftCount = downCount = upCount = 0;
            rightCount = (rightCount + 1) % 20;
            if (!Online)
                sr.sprite = sprites[NO * 16 + rightCount / 5 + 8];
            if (Online && photonView.IsMine)
                photonView.RPC("changeSprite", RpcTarget.All, NO * 16 + rightCount / 5 + 8);
            transform.Translate(speed * Time.fixedDeltaTime * Vector3.right, Space.World);
            orientation = 2;
        }
        if (direction == 0)
        {
            rightCount = leftCount = upCount = 0;
            downCount = (downCount + 1) % 20;
            if (!Online)
            {
                sr.sprite = sprites[NO * 16 + downCount / 5];
            }
            if (Online && photonView.IsMine)
            {
                photonView.RPC("changeSprite", RpcTarget.All, NO * 16 + downCount / 5);
            }
            transform.Translate(speed * Time.fixedDeltaTime * Vector3.down, Space.World);
            orientation = 0;
        }
        if (direction == 3)
        {
            rightCount = leftCount = downCount = 0;
            upCount = (upCount + 1) % 20;
            if (!Online)
                sr.sprite = sprites[NO * 16 + upCount / 5 + 12];
            if (Online && photonView.IsMine)
                photonView.RPC("changeSprite", RpcTarget.All, NO * 16 + upCount / 5 + 12);
            transform.Translate(speed * Time.fixedDeltaTime * Vector3.up, Space.World);
            orientation = 3;
        }
        if (direction == -1)
        {
            if (!Online)
                sr.sprite = sprites[NO * 16 + orientation * 4];
            if (Online && photonView.IsMine)
                photonView.RPC("changeSprite", RpcTarget.All, NO * 16 + orientation * 4);
        }




    }

    private void MoveByKey()
    {

        float h = 0, v = 0;

        if (PlayerNO == 1)
        {
            h = Input.GetAxisRaw("Horizontal");
            v = Input.GetAxisRaw("Vertical");
        }

        if (PlayerNO == 2)
        {
            h = Input.GetAxisRaw("Horizontal2");
            v = Input.GetAxisRaw("Vertical2");
        }


        if (h != 0) v = 0;
        if (h > 0)
        {
            orientation = 2;
            Move(2);
            return;
        }
        if (h < 0)
        {
            orientation = 1;
            Move(1);
            return;
        }
        if (v > 0)
        {
            orientation = 3;
            Move(3);
        }
        if (v < 0)
        {
            orientation = 0;
            Move(0);
        }
        if (h == 0 && v == 0)
        {
            Move(-1);
        }

    }

    public void Fire()
    {

        if (isFree && !Online)
        {
            if (haveGun)
            {
                StartCoroutine(Defend());
                gameManager.RaserAttack(orientation, this.transform.position);
                haveGun = false;
            }
            else
            {
                GameObject bombObject = gameManager.PlaceBomb(this.transform.position, bombRadius, index);
            }
            

        }
        else if (isFree && Online && PhotonNetwork.IsMasterClient)
        {
            gameManager.PlaceBomb(this.transform.position, bombRadius, index);

        }
        else if (isFree && Online && !PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("SetBombOnline", RpcTarget.All, index, new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z), bombRadius);
        }
    }

    [PunRPC]
    public void SetBombOnline(int LocalIndex, Vector3 POS, int bombradius)
    {

        if (Online && PhotonNetwork.IsMasterClient)
        {
            GameObject bombObject = GameManager.Instance.PlaceBomb(POS, bombradius, LocalIndex);
        }
    }

    private void FireByKey()
    {
        if (PlayerNO == 1)
        {
            if (Input.GetKeyDown(KeyCode.Space) && Time.timeScale==1f)
            {
                Fire();
            }
        }

        if (PlayerNO == 2)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                Fire();
            }
        }


    }

    private void Die()
    {
        if (delayTime > 0)
        {
            life = 1;
            return;
        }
        isFree = false;
        isDead = true;
        StartCoroutine(DeathEffect());
    }


    public void ReduceLife()
    {
        if (true)
        {
            if (!PhotonNetwork.IsConnected || (PhotonNetwork.IsConnected && PhotonNetwork.IsMasterClient))
            {
                this.life--;
            }
            if (life > 0)
            {
                StartCoroutine(Defend());
            }
        }
    }

    private IEnumerator DeathEffect()
    {
        sr.sprite = deathSprites[NO * 6];
        yield return new WaitForSeconds(1.5f);
        sr.sprite = deathSprites[NO * 6 + 1];
        yield return new WaitForSeconds(0.05f);
        sr.sprite = deathSprites[NO * 6 + 2];
        yield return new WaitForSeconds(0.05f);
        sr.sprite = deathSprites[NO * 6 + 3];
        yield return new WaitForSeconds(0.05f);
        sr.sprite = deathSprites[NO * 6 + 4];
        yield return new WaitForSeconds(0.05f);
        sr.sprite = deathSprites[NO * 6 + 5];
        yield return new WaitForSeconds(0.05f);
        Destroy(gameObject);

    }
    //3s无敌时间
    public IEnumerator Defend()
    {
        isDefended = true;
        for (int i = 0; i < 6; i++)
        {
            sr.color = new Color(1, 1, 1, 0.9f);
            yield return new WaitForSeconds(0.1f);
            sr.color = new Color(1, 1, 1, 0.8f);
            yield return new WaitForSeconds(0.1f);
            sr.color = new Color(1, 1, 1, 0.7f);
            yield return new WaitForSeconds(0.1f);
            sr.color = new Color(1, 1, 1, 0.6f);
            yield return new WaitForSeconds(0.1f);
            sr.color = new Color(1, 1, 1, 0.5f);
            yield return new WaitForSeconds(0.1f);
            sr.color = new Color(1, 1, 1, 0.6f);
            yield return new WaitForSeconds(0.1f);
            sr.color = new Color(1, 1, 1, 0.7f);
            yield return new WaitForSeconds(0.1f);
            sr.color = new Color(1, 1, 1, 0.8f);
            yield return new WaitForSeconds(0.1f);
            sr.color = new Color(1, 1, 1, 0.9f);
            yield return new WaitForSeconds(0.1f);
            sr.color = new Color(1, 1, 1, 1);
            yield return new WaitForSeconds(0.1f);
            if (Menu.mode != 3 && i == 2)
            {
                break;
            }
        }
        sr.color = new Color(1, 1, 1, 1);
        isDefended = false;
    }
    [PunRPC]
    void changeSprite(int i)
    {
        if (this.setNo == false)
        {
            this.NO = i / 16;
            this.setNo = true;
        }
        //Debug.Log("in RPC:changespite:" + i);
        sr = this.GetComponent<SpriteRenderer>();
        sr.sprite = sprites[i];
    }
    #endregion

    [PunRPC]
    void setIndex(int i)
    {
        index = i;
    }

    //AI相关函数
    #region

    private void MoveByAi()
    {
        //如果在当前格点的中心再考虑改变状态
        if (IsCenter(transform.position) && delta >= 5)
        {
            delta = 0;
            check = 0;
            reverse = false;
            StateUpdate();
            //调试
            #region
            /*
            Debug.Log("(" + NO + ") moveState= " + moveState + " " + currentPoint + " -- " + targetPoint + " bomb= " +
                       gameManager.bombRange[currentPoint.x, currentPoint.y].Count + "  target= " +
                       targetType + " nearlyBox= " + nearlyBox + " nearBox= " + nearBox + " nearlySafe= " + nearlySafe + " nearlyTool= " + nearlyTool);
            for (int i = 0; i < 15; i++)
                {
                
                    Debug.Log("第" + (i + 1) + "行:" + "(" + (int)gameManager.bombRange[0, i].Count + ") " + "(" + (int)gameManager.bombRange[1, i].Count + ") " +
                        "(" + (int)gameManager.bombRange[2, i].Count + ") " + "(" + (int)gameManager.bombRange[3, i].Count + ") " +
                        "(" + (int)gameManager.bombRange[4, i].Count + ") " + "(" + (int)gameManager.bombRange[5, i].Count + ") " +
                        "(" + (int)gameManager.bombRange[6, i].Count + ") " + "(" + (int)gameManager.bombRange[7, i].Count + ") " +
                        "(" + (int)gameManager.bombRange[8, i].Count + ") " + "(" + (int)gameManager.bombRange[9, i].Count + ") " +
                        "(" + (int)gameManager.bombRange[10, i].Count + ") " + "(" + (int)gameManager.bombRange[11, i].Count + ") " +
                        "(" + (int)gameManager.bombRange[12, i].Count + ") " + "(" + (int)gameManager.bombRange[13, i].Count + ") " +
                        "(" + (int)gameManager.bombRange[14, i].Count + ") " + "(" + (int)gameManager.bombRange[15, i].Count + ") ");
                }
            Debug.Log("-><color=#A0522D>" + "______________" + "</color>");
           */
            #endregion
        }
        else
        {
            check++;
        }
        //判定卡在撞墙状态
        if (check > 100 && moveState >= 0)
        {
            //取反向
            reverse = true;
            delta = 5;
        }
        if (reverse)
        {
            moveState = 3 - moveState;
            Move(moveState);
        }
        else
        {
            Move(moveState);
        }
        delta++;
    }

    private void FireByAi()
    {
        if (PlayerNO <= 0)
        {
            if (canFire)
            {
                Fire();
                canFire = false;
            }
        }
    }
    //运动状态更新
    public void StateUpdate()
    {

        //到达箱子旁边，可以放置炸弹了
        if ((nearlyBox == true && targetType == 1) || nearBox == true)
        {
            //防止Bug 直接出击
            Fire();
            nearlyBox = false;
            nearBox = false;
        }
        //刚好到达安全区
        if (nearlySafe == true && targetType == 0)
        {
            //停止
            moveState = -1;
            nearlySafe = false;
        }
        if (nearlyTool == true && targetType == 2)
        {
            //停止
            moveState = -1;
            nearlyTool = false;
        }
        else
        {
            //躲避炸弹优先
            if (!SafeUpdate())
            {
                //其次考虑追踪目标
                if (!TargetUpdate())
                {
                    //最后考虑进攻
                    AttackUpdate();
                }
            }
        }
    }
    //状态更新：躲避炸弹(需要躲避则返回true)
    public bool SafeUpdate()
    {
        if (!IsSafe())
        {
            targetType = 0;
            if (Bfs(0))
            {
                //有危险且找到了安全区
                return true;
            }
            else
                //有危险且处于绝境
                return false;
        }
        else
            //当前安全
            return false;
    }
    //判断当前站位是否安全（安全则返回true）
    public bool IsSafe()
    {
        if (gameManager.bombRange[currentPoint.x, currentPoint.y].Count > 0)
            return false;
        else
            return true;
    }

    //状态更新：追踪目标(需要追踪则返回true)
    public bool TargetUpdate()
    {
        //先找TOOL
        if (!Bfs(2))
        {
            //没找到TOOL就找BOX
            /////////////【4】
            if (gameManager.bombNumbers[index] == 0)
            {
                if (!Bfs(1))
                {
                    //都没找到
                    return false;
                }
                else
                    return true;
            }
            else return true;
        }
        else
            return true;
    }
    //状态更新：攻击玩家
    public void AttackUpdate()
    {
        Vector2 floatPos = GetFloatLocation(transform.position);
        Vector2Int intPos = GetIntLocation(floatPos);
        Vector2Int nextPos;
        int i = 0;
        int j = 0;
        if (counterPeace > 30)
        {
            for (i = 0; i < 10; i++)
            {
                j = UnityEngine.Random.Range(0, 4);
                nextPos = intPos + vector2Ints[j];
                if (nextPos.x >= 0 && nextPos.x <= 15 && nextPos.y >= 0 && nextPos.y <= 14)
                {
                    if (((int)gameManager.itemsType[nextPos.x, nextPos.y] == 0))
                    {
                        moveState = j;
                        counterStep++;
                        break;
                    }
                }
            }
            //每10步放炸弹
            if (counterStep >= 10)
            {
                counterStep = 0;
                Fire();
                counterPeace = 0;
            }
        }
        else
        {
            counterPeace++;
        }
    }
    //返回float地图坐标
    public Vector2 GetFloatLocation(Vector3 position)
    {
        return new Vector2((position.x - gameManager.reference.x + 0.5f), (gameManager.reference.y - position.y + 0.5f));
    }
    //返回int地图坐标
    public Vector2Int GetIntLocation(Vector2 location)
    {
        return new Vector2Int((int)Mathf.Floor(location.x), (int)Mathf.Floor(location.y));
    }

    //BFS(safe:0  BOX:1  TOOL:2)
    private bool Bfs(int type)
    {
        //初始化
        targetType = type;
        queue.Clear();
        isUsed = new bool[,] {
            {false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false },
            {false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false },
            {false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false },
            {false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false },
            {false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false },
            {false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false },
            {false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false },
            {false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false },
            {false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false },
            {false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false },
            {false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false },
            {false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false },
            {false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false },
            {false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false },
            {false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false },
            {false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false }};
        isFound = false;
        startPoint = currentPoint;

        //开始时，把开始点加入到队列(head:-1）,标志已走过
        queue.Enqueue(new bfsNode(startPoint, new Vector2Int(-1, -1)));
        preMap[startPoint.x, startPoint.y] = new Vector2Int(-1, -1);
        isUsed[startPoint.x, startPoint.y] = true;

        //搜索，反演，定向
        while (queue.Count > 0)
        {
            //从队列中取一个bfs结点
            Vector2Int currentPoint = queue.Dequeue().now;
            //向周围扩散
            for (int i = 0; i < 4; i++)
            {
                Vector2Int v2Int = vector2Ints[i] + currentPoint;
                if (IsAvailable(v2Int))
                {
                    //目标安全区
                    if (type == 0)
                    {
                        if (gameManager.bombRange[v2Int.x, v2Int.y].Count == 0 && (int)gameManager.itemsType[v2Int.x, v2Int.y] == 0)
                        {
                            //立即结束进程
                            isFound = true;
                            targetPoint = v2Int;
                            //设置目标点的前驱
                            preMap[v2Int.x, v2Int.y] = currentPoint;
                            break;
                        }
                        //是EMPTY？
                        else if ((int)gameManager.itemsType[v2Int.x, v2Int.y] == 0)
                        {
                            //设置该点的前驱
                            preMap[v2Int.x, v2Int.y] = currentPoint;
                            //入队
                            queue.Enqueue(new bfsNode(v2Int, currentPoint));
                            //标记已走过
                            isUsed[v2Int.x, v2Int.y] = true;
                        }
                    }
                    //目标BOX
                    else if (type == 1)
                    {
                        if ((int)gameManager.itemsType[v2Int.x, v2Int.y] == 2)
                        {
                            //立即结束进程
                            isFound = true;
                            targetPoint = v2Int;
                            //设置目标点的前驱
                            preMap[v2Int.x, v2Int.y] = currentPoint;
                            break;
                        }
                        //是EMPTY？
                        else if ((int)gameManager.itemsType[v2Int.x, v2Int.y] == 0)
                        {
                            //设置该点的前驱
                            preMap[v2Int.x, v2Int.y] = currentPoint;
                            //入队
                            queue.Enqueue(new bfsNode(v2Int, currentPoint));
                            //标记已走过
                            isUsed[v2Int.x, v2Int.y] = true;
                        }
                    }
                    //目标TOOL
                    else if (type == 2)
                    {
                        if ((int)gameManager.itemsType[v2Int.x, v2Int.y] == 3)
                        {
                            //立即结束进程
                            isFound = true;
                            targetPoint = v2Int;
                            //设置目标点的前驱
                            preMap[v2Int.x, v2Int.y] = currentPoint;
                            break;
                        }
                        //是EMPTY？
                        else if ((int)gameManager.itemsType[v2Int.x, v2Int.y] == 0)
                        {
                            //设置该点的前驱
                            preMap[v2Int.x, v2Int.y] = currentPoint;
                            //入队
                            queue.Enqueue(new bfsNode(v2Int, currentPoint));
                            //标记已走过
                            isUsed[v2Int.x, v2Int.y] = true;
                        }
                    }
                }
            }
            if (isFound)
            {
                //找到终点后，清空队列
                queue.Clear();

                //形成路径结点栈
                GetPath(type);
                //确定运动状态
                SetMoveState(type);
                break;
            }
        }
        return isFound;
    }
    //未被使用则返回true
    private bool IsAvailable(Vector2Int intloc)
    {
        int x = intloc.x;
        int y = intloc.y;
        //注意从右向左传参
        if (x < 0 || x >= xColumn || y < 0 || y >= yRow)
            return false;
        else if (isUsed[x, y])
            return false;
        else
            return true;
    }
    //反演路径栈
    private void GetPath(int type)
    {
        stack.Clear();
        Vector2Int tp = targetPoint;
        //连同目标位置一起入栈,栈顶与起始点相邻
        while (tp != startPoint)
        {
            stack.Push(tp);
            tp = preMap[tp.x, tp.y];
        }
    }
    //确定运动状态
    private void SetMoveState(int type)
    {
        if (stack.Count > 0)
        {
            Vector2Int nextPoint;
            nextPoint = stack.Peek();
            //逃生路径优先
            if (type == 0)
            {
                if (stack.Count == 1)
                    nearlySafe = true;
                nextPoint = stack.Pop();
                moveState = GetDirection(currentPoint, nextPoint);
            }
            else
            {
                //如果下一点危险
                if (gameManager.bombRange[nextPoint.x, nextPoint.y].Count > 0 || gameManager.explosionRange[nextPoint.x, nextPoint.y] > 0)
                {
                    nearlyBox = false;
                    nearlyTool = false;
                    moveState = -1;
                }
                else
                {
                    //下一点安全
                    //BOX
                    if (type == 1)
                    {
                        if (stack.Count == 1)
                        {
                            //与目标BOX相邻
                            nearBox = true;
                            moveState = -1;
                        }
                        else
                        {
                            if (stack.Count == 2)
                                nearlyBox = true;
                            nextPoint = stack.Pop();
                            moveState = GetDirection(currentPoint, nextPoint);
                        }
                    }
                    //TOOL
                    else if (type == 2)
                    {
                        //与TOOL相邻
                        if (stack.Count == 1)
                            nearlyTool = true;
                        nextPoint = stack.Pop();
                        moveState = GetDirection(currentPoint, nextPoint);
                    }
                }
            }
        }
    }
    //比较前后两点获取运动方向
    private int GetDirection(Vector2Int now, Vector2Int next)
    {
        int dx = next.x - now.x;
        int dy = next.y - now.y;
        if (dy == 1)
            return 0;
        else if (dx == -1)
            return 1;
        else if (dx == 1)
            return 2;
        else
            return 3;
    }
    //在当前点格的中心则返回true
    private bool IsCenter(Vector3 dest)
    {
        GameManager.Location nowLoc = gameManager.GetCoord(dest);
        Vector3 nowPos = gameManager.CorrectPosition(nowLoc.x, nowLoc.y);
        float dx = Mathf.Abs(nowPos.x - dest.x);
        float dy = Mathf.Abs(nowPos.y - dest.y);
        return (dx <= 0.08f && dy <= 0.08f);
    }
    #endregion
}