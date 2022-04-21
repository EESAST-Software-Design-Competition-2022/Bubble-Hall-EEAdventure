using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviourPun
{
    public enum ItemType
    {
        EMPTY,
        BARRIAR,
        BOX,
        TOOL,
        BOMB,
        DOOR
    }
    public struct Location
    {
        public int x;
        public int y;

        public Location(int x_, int y_)
        {
            x = x_; y = y_;
        }
    }

    //��Ϸģʽ(������)
    public int mode;
    //��ͼ����
    public int xColumn = 16;
    public int yRow = 15;
    public Vector3 reference = new(-5.2f, 7.1f, 0);
    //��Ϸ��Դ
    public GameObject[] item;//初始化地图所需物体 0.障碍 1.箱子 2.边界墙 3.地板 4.炸弹 5.爆炸效果 6.传送门 7.声波
    public GameObject rolePrefab;
    public Sprite[] barriarSprites;
    public AudioClip explosionAudio;
    public AudioClip gameOverAudio;
    public AudioClip gameWinAudio;
    public GameObject[] toolPrefab;
    public Sprite[] toolSprites;

    Player[] allplayers;//获取联机玩家信息
    public bool Online = false;
    public int c = 0;
    public int[] PlayerIndex;
    public int[] PlayerRoleIndex;
    public GameObject rolePrefabOnline;
    public GameObject[] OnlineItem;
    public int InRoomPeople = 0;
    public int OnlinePlayerNum = 0;
    public GameUI ui;
    public int MasterClientIndex;
    public int OnlineLocalIndex;
    public int OnlineLocalPlayerIndex;
    public int[] OnlinePlayerRoleIndex;
    public int[] OnlinePlayerIndex;
    static int NotIntPlayer = 0;
    private static GameManager instance;
    public static GameManager Instance { get => instance; set => instance = value; }

    public GameObject[,] itemsObject;//�����Ὠ�������Ͻ�Ϊԭ��
    public ItemType[,] itemsType;
    public Array<GameObject> roleList = new Array<GameObject>(4);
    public List<Bomb>[,] bombRange;
    public int[,] explosionRange;

    private int[,] map1;
    private List<Location> birthPlaces = new List<Location>();
    private List<int> leftRoleNOList = new List<int> { 0, 1, 2, 3, 4 };//the left roles
    public List<Location> doors = new List<Location>();
    public int[] bombNumbers = { 0, 0, 0, 0 };
    private bool isGameOver = false;
    public bool CollhasIng = false;
    public int OnlineAliveNum;

    private void Awake()
    {
        CollhasIng = false;
        instance = this;
        map1 = (new MapCreation()).RandomMap();
        itemsObject = new GameObject[xColumn, yRow];
        itemsType = new ItemType[xColumn, yRow];
        mode = Menu.mode;
        birthPlaces.Add(new Location(0, 0));
        birthPlaces.Add(new Location(0, yRow - 1));
        birthPlaces.Add(new Location(xColumn - 1, 0));
        birthPlaces.Add(new Location(xColumn - 1, yRow - 1));
        if (PhotonNetwork.IsConnected == true) Online = true;
        bombRange = new List<Bomb>[xColumn, yRow];
        explosionRange = new int[xColumn, yRow];
        for (int i = 0; i < xColumn; i++)
        {
            for (int j = 0; j < yRow; j++)
            {
                bombRange[i, j] = new List<Bomb>();
            }
        }

    }
    private void Start()
    {
        //生成地图
        #region
        for (int i = 0; i < xColumn; i++)
        {
            for (int j = 0; j < yRow; j++)
            {
                itemsType[i, j] = ItemType.EMPTY;
            }
        }
        //生成基本地图
        for (int i = -1; i <= xColumn; i++)
        {
            for (int j = -1; j <= yRow; j++)
            {
                CreateItem(item[3], i, j, Quaternion.identity);//生成地板
                if (i == -1 || i == xColumn || j == -1 || j == yRow)
                    CreateItem(item[2], i, j, Quaternion.identity);//生成边界墙
                else
                {
                    if (mode == 2)
                    {
                        if (map1[j, i] == 1)
                        {
                            CreateItem(item[0], i, j, Quaternion.identity);//生成障碍
                        }
                        if(map1[j,i] == 2)
                        {
                            doors.Add(new Location(i, j));
                            CreateItem(item[6], i, j, Quaternion.identity);
                        }
                    }
                    else
                    {
                        if (map1[j, i] == 1 || map1[j,i] == 2)
                        {
                            CreateItem(item[0], i, j, Quaternion.identity);//生成障碍
                        }
                    }
                    
                }
            }
        }

        //随机生成箱子
        for (int i = 0; i < xColumn; i++)
        {
            for (int j = 0; j < yRow; j++)
            {
                if (i + j > 1 && i + yRow - j > 2 && xColumn - i + j > 2 && xColumn - i + yRow - j > 3)
                {
                    int n = Random.Range(0, 100);
                    bool nextToDoor = ((i-1>=0 && itemsType[i - 1, j] == ItemType.DOOR) || (i+1<xColumn && itemsType[i + 1, j] == ItemType.DOOR) || (j-1>=0 && itemsType[i, j - 1] == ItemType.DOOR) || (j+1<yRow && itemsType[i, j + 1] == ItemType.DOOR));
                    if (itemsType[i, j] == ItemType.EMPTY && n <= 90 && !nextToDoor)
                    {
                        CreateItem(item[1], i, j, Quaternion.identity);
                    }
                }
            }
        }
        #endregion
        //生成角色
        if (mode == 0)
        {
            GameObject selectRole = GameObject.FindGameObjectWithTag("selectRole");
            if (selectRole != null)
            {
                RandomGenerateRole(selectRole.GetComponent<SelectRole>().index, 1, 0);
                Destroy(selectRole);
            }
            Online = false;
            if (!Online)
            {
                while (roleList.Count < 4)
                {
                    int i = Random.Range(0, leftRoleNOList.Count);
                    RandomGenerateRole(leftRoleNOList[i], 0, roleList.Count);
                }

                for (int i = 0; i < roleList.Count; i++)
                {
                    for (int j = i + 1; j < roleList.Count; j++)
                    {
                        Physics2D.IgnoreCollision(roleList.GetT(i).GetComponent<PolygonCollider2D>(), roleList.GetT(j).GetComponent<PolygonCollider2D>());
                    }
                }
            }
        }

        if (mode == 1)
        {
            GameObject selectRole2 = GameObject.FindGameObjectWithTag("selectRole2");
            if (selectRole2 != null)
            {
                RandomGenerateRole(selectRole2.GetComponent<SelectRole2>().index[0], 1, 0);
                RandomGenerateRole(selectRole2.GetComponent<SelectRole2>().index[1], 2, 1);
                Destroy(selectRole2);
            }
            Online = false;
            if (!Online)
            {
                while (roleList.Count < 4)
                {
                    int i = Random.Range(0, leftRoleNOList.Count);
                    RandomGenerateRole(leftRoleNOList[i], 0, roleList.Count);
                }

                for (int i = 0; i < roleList.Count; i++)
                {
                    for (int j = i + 1; j < roleList.Count; j++)
                    {
                        Physics2D.IgnoreCollision(roleList.GetT(i).GetComponent<PolygonCollider2D>(), roleList.GetT(j).GetComponent<PolygonCollider2D>());
                    }
                }
            }
        }

        if (mode == 2)
        {
            GameObject selectRole = GameObject.FindGameObjectWithTag("selectRole");
            if (selectRole != null)
            {
                RandomGenerateRole(selectRole.GetComponent<SelectRole>().index, 1, 0);
                Destroy(selectRole);
            }
            Online = false;
            if (!Online)
            {
                while (roleList.Count < 4)
                {
                    int i = Random.Range(0, leftRoleNOList.Count);
                    RandomGenerateRole(leftRoleNOList[i], 0, roleList.Count);
                }

                for (int i = 0; i < roleList.Count; i++)
                {
                    for (int j = i + 1; j < roleList.Count; j++)
                    {
                        Physics2D.IgnoreCollision(roleList.GetT(i).GetComponent<PolygonCollider2D>(), roleList.GetT(j).GetComponent<PolygonCollider2D>());
                    }
                }
            }
        }
        if (mode == 3 && Online)
        {
            //Online属性
            GetOnlineInfo();
            #region
            //if (PhotonNetwork.IsMasterClient)
            //{
            //    int k = 0;
            //    for (int i = 0; i < InRoomPeople; i++)
            //    {
            //        for (int j = 0; j < InRoomPeople; j++)
            //        {
            //            if (PlayerIndex[j] == k)
            //            {
            //                if (j != MasterClientIndex)
            //                {
            //                    RandomGenerateRole(PlayerRoleIndex[j], PlayerIndex[j] + 1, PlayerIndex[j]);
            //                }
            //                else
            //                {
            //                    RandomGenerateRole(PlayerRoleIndex[j], PlayerIndex[j] + 1, PlayerIndex[j]);
            //                }
            //            }
            //        }
            //        k++;
            //    }
            //}
            //Debug.Log("OnlineLocalPlayerIndex:" + OnlineLocalPlayerIndex);
            #endregion
            GenerateRole(birthPlaces[OnlineLocalPlayerIndex].x, birthPlaces[OnlineLocalPlayerIndex].y, OnlineLocalIndex, 1, OnlineLocalPlayerIndex);
            if (PhotonNetwork.IsMasterClient)
            {
                for (int i = 0; i < InRoomPeople; i++)
                {
                    birthPlaces.RemoveAt(0);
                }
            }
            if (Online && PhotonNetwork.IsMasterClient)
            {
                int j = InRoomPeople;
                while (j < 4)
                {

                    int i = Random.Range(0, leftRoleNOList.Count);
                    RandomGenerateRole(leftRoleNOList[i], 0, j);
                    j++;
                }
            }

        }



        Time.timeScale = 1f;
        //audio
        AudioListener.pause = !Menu.sound;
    }

    private void Update()
    {
        //        Debug.Log("rolelist_num:" + roleList.Count);
        if (isGameOver)
            return;

        //�ж�ʤ����ʧ��
        if (mode == 0 || mode==2)
        {
            if (roleList.GetT(0) != null&&roleList.GetT(1)==null && roleList.GetT(2) == null && roleList.GetT(3) == null)
            {
                isGameOver = true;
                Debug.Log("isGameover");
                Victory();
            }
            else if (roleList.GetT(0) == null)
            {
                isGameOver = true;
                Defeat();
            }
        }

        if (mode == 1)
        {
            if ((roleList.GetT(0) != null || roleList.GetT(1) != null)&& roleList.GetT(2) == null && roleList.GetT(3) == null)
            {
                isGameOver = true;
                Victory();
            }
            if ((roleList.GetT(0) == null && roleList.GetT(1) == null))
            {
                isGameOver = true;
                Defeat();
            }
        }

        if (mode == 3)
        {
            if(PhotonNetwork.IsConnected==false)
            {
                SceneManager.LoadScene(0);
            }
            int j = 0;
            for (int i = 0; i < 4; i++)
            {
                if (roleList.GetT(i) != null)
                    j++;
            }
            if (PhotonNetwork.IsMasterClient)
            {
                if (OnlinePlayerNum == 4 && j == 1)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        if (roleList.GetT(i) != null)
                        {
                            if (PhotonNetwork.IsConnected)
                                photonView.RPC("ShowGameResult", RpcTarget.All, roleList.GetT(i).GetComponent<Person>().index);
                            isGameOver = true;
                            break;
                        }
                    }
                }
                else if (OnlinePlayerNum == 3 && j == 1)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        if (roleList.GetT(i) != null)
                        {
                            if (PhotonNetwork.IsConnected)
                                photonView.RPC("ShowGameResult", RpcTarget.All, roleList.GetT(i).GetComponent<Person>().index);
                            isGameOver = true;
                            break;
                        }
                    }
                }
                else if (OnlinePlayerNum == 2 && j == 2 && roleList.GetT(2) != null && roleList.GetT(3) != null)
                {
                    if (PhotonNetwork.IsConnected)
                        photonView.RPC("ShowGameResult", RpcTarget.All, -1);
                    isGameOver = true;
                }
                else if (OnlinePlayerNum == 2 && j == 1)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        if (roleList.GetT(i) != null)
                        {
                            if (PhotonNetwork.IsConnected)
                                photonView.RPC("ShowGameResult", RpcTarget.All, roleList.GetT(i).GetComponent<Person>().index);
                            isGameOver = true;
                            break;
                        }
                    }
                }
                else if (OnlinePlayerNum == 1 && roleList.GetT(0) != null && j == 1)
                {
                    isGameOver = true;
                    Victory();
                }
                else if (OnlinePlayerNum == 1 && roleList.GetT(0) == null)
                {
                    isGameOver = true;
                    Defeat();
                }
            }


            if (Online && !CollhasIng && roleList.Count == 4)
            {
                for (int i = 0; i < roleList.Count; i++)
                {
                    for (int t = i + 1; t < roleList.Count; t++)
                    {
                        Physics2D.IgnoreCollision(roleList.GetT(i).GetComponent<PolygonCollider2D>(), roleList.GetT(t).GetComponent<PolygonCollider2D>());
                    }
                }
                CollhasIng = true;
            }
        }

        if (!Menu.isFullScreen)
        {
            Menu.screenSize = new int[2] { Screen.width, Screen.height };
        }
    }


    private void CreateItem(GameObject createGameObject, int createX, int createY, Quaternion createRotation)
    {
        Vector3 createPosition = CorrectPosition(createX, createY);
        if (!Online)
        {
            GameObject itemGo = Instantiate(createGameObject, createPosition, createRotation);
            itemGo.transform.SetParent(this.gameObject.transform);
            if (createX >= 0 && createX < xColumn && createY >= 0 && createY < yRow)
            {
                itemsObject[createX, createY] = itemGo;
                itemsType[createX, createY] = ConvertToType(itemGo.tag);
                if (itemGo.CompareTag("door"))
                {
                    itemGo.GetComponent<Door>().x = createX;
                    itemGo.GetComponent<Door>().y = createY;
                    itemGo.GetComponent<Door>().index = doors.Count - 1;
                }
            }
        }

        if (Online && PhotonNetwork.IsMasterClient)
        {
            GameObject itemGo = PhotonNetwork.InstantiateRoomObject(createGameObject.name, createPosition, createRotation);
            itemGo.transform.SetParent(this.gameObject.transform);
            if (createX >= 0 && createX < xColumn && createY >= 0 && createY < yRow)
            {
                itemsObject[createX, createY] = itemGo;
                itemsType[createX, createY] = ConvertToType(itemGo.tag);
            }
        }

    }


    private void RandomGenerateRole(int NO, int PlayerNO, int index)
    {
        if (leftRoleNOList.Contains(NO) && !Online)
        {
            int i = Random.Range(0, birthPlaces.Count);
            GenerateRole(birthPlaces[i].x, birthPlaces[i].y, NO, PlayerNO, index);
            birthPlaces.RemoveAt(i);
        }
        if (Online)
        {
            int i = Random.Range(0, birthPlaces.Count);
            GenerateRole(birthPlaces[i].x, birthPlaces[i].y, NO, PlayerNO, index);
            birthPlaces.RemoveAt(i);
        }

    }

    private void GenerateRole(int createX, int createY, int NO, int PlayerNO, int index)
    {
        if (createX < 0 || createX >= xColumn || createY < 0 || createY >= yRow || roleList.Count >= 4)
            return;
        if (itemsType[createX, createY] == ItemType.EMPTY && !Online)
        {
            Vector3 createPosition = CorrectPosition(createX, createY);
            GameObject itemGo = Instantiate(rolePrefab, createPosition, Quaternion.identity);
            itemGo.GetComponent<Person>().gameManager = this;
            roleList.Add(itemGo);
            itemGo.GetComponent<Person>().NO = NO;
            itemGo.GetComponent<Person>().PlayerNO = PlayerNO;
            itemGo.GetComponent<Person>().index = index;
            leftRoleNOList.Remove(NO);
        }
        if (itemsType[createX, createY] == ItemType.EMPTY && Online)
        {
            Vector3 createPosition = CorrectPosition(createX, createY);
            GameObject itemGo = PhotonNetwork.Instantiate(this.rolePrefabOnline.name, createPosition, Quaternion.identity);
            itemGo.GetComponent<Person>().gameManager = GameManager.Instance;
            itemGo.GetComponent<Person>().NO = NO;
            itemGo.GetComponent<Person>().PlayerNO = PlayerNO;
            itemGo.GetComponent<Person>().index = index;
            roleList.AddByIndex(itemGo, index);
            if (PlayerNO == 0)
                leftRoleNOList.Remove(NO);
        }
    }

    private ItemType ConvertToType(string typeTag)
    {
        if (typeTag.CompareTo("Barriar") == 0)
            return ItemType.BARRIAR;
        if (typeTag.CompareTo("Bomb") == 0)
            return ItemType.BOMB;
        if (typeTag.CompareTo("Box") == 0)
            return ItemType.BOX;
        if (typeTag.CompareTo("Tool") == 0)
            return ItemType.TOOL;
        if (typeTag.CompareTo("door") == 0)
            return ItemType.DOOR;
        return ItemType.EMPTY;
    }

    public Vector3 CorrectPosition(int x, int y)
    {
        return new Vector3(reference.x + x, reference.y - y, 0);
    }

    public Location GetCoord(Vector3 position)
    {
        return new Location((int)Mathf.Floor(position.x - reference.x + 0.5f), (int)Mathf.Floor(reference.y - position.y + 0.5f));
    }

    public GameObject PlaceBomb(Vector3 position, int bombRadius, int host)
    {
        Location coord = GetCoord(position);
        if (itemsType[coord.x, coord.y] == ItemType.EMPTY && bombNumbers[host] < roleList.GetT(host)?.GetComponent<Person>().bombNumber)
        {
            Vector3 createPosition = CorrectPosition(coord.x, coord.y) - new Vector3(0, 0.3f, 0);
            if (!Online)
            {
                GameObject bombObject = Instantiate(item[4], createPosition, Quaternion.identity);
                itemsObject[coord.x, coord.y] = bombObject;
                itemsType[coord.x, coord.y] = ItemType.BOMB;
                Bomb bomb = bombObject.GetComponent<Bomb>();
                bomb.GameManager = this;
                bomb.x = coord.x;
                bomb.y = coord.y;
                bomb.radius = bombRadius;
                bomb.targetCoord = new Vector2Int(coord.x, coord.y);
                bombRange[coord.x, coord.y].Add(bomb);
                explosionRange[coord.x, coord.y]++;
                //向上遍历
                for (int i = 1; i <= bombRadius; i++)
                {
                    if (coord.y - i < 0 || itemsType[coord.x, coord.y - i] == ItemType.BARRIAR)
                        break;
                    bombRange[coord.x, coord.y - i].Add(bomb);
                    explosionRange[coord.x, coord.y - i]++;
                }
                //向左遍历
                for (int i = 1; i <= bombRadius; i++)
                {
                    if (coord.x - i < 0 || itemsType[coord.x - i, coord.y] == ItemType.BARRIAR)
                        break;
                    bombRange[coord.x - i, coord.y].Add(bomb);
                    explosionRange[coord.x - i, coord.y]++;
                }
                //向右遍历
                for (int i = 1; i <= bombRadius; i++)
                {
                    if (coord.x + i >= xColumn || itemsType[coord.x + i, coord.y] == ItemType.BARRIAR)
                        break;
                    bombRange[coord.x + i, coord.y].Add(bomb);
                    explosionRange[coord.x + i, coord.y]++;
                }
                //向下遍历
                for (int i = 1; i <= bombRadius; i++)
                {
                    if (coord.y + i >= yRow || itemsType[coord.x, coord.y + i] == ItemType.BARRIAR)
                        break;
                    bombRange[coord.x, coord.y + i].Add(bomb);
                    explosionRange[coord.x, coord.y + i]++;
                }
                bomb.host = host;
                bombNumbers[host]++;
                return bombObject;
            }
            else
            {
                //Debug.Log("Place Bomb");
                GameObject bombObject = PhotonNetwork.Instantiate(this.item[4].name, createPosition, Quaternion.identity);
                itemsObject[coord.x, coord.y] = bombObject;
                itemsType[coord.x, coord.y] = ItemType.BOMB;
                Bomb bomb = bombObject.GetComponent<Bomb>();
                bomb.GameManager = instance;
                bomb.x = coord.x;
                bomb.y = coord.y;
                bomb.radius = bombRadius;
                bombRange[coord.x, coord.y].Add(bomb);
                explosionRange[coord.x, coord.y]++;
                //向上遍历
                for (int i = 1; i <= bombRadius; i++)
                {
                    if (coord.y - i < 0 || itemsType[coord.x, coord.y - i] == ItemType.BARRIAR)
                        break;
                    bombRange[coord.x, coord.y - i].Add(bomb);
                    explosionRange[coord.x, coord.y - i]++;
                }
                //向左遍历
                for (int i = 1; i <= bombRadius; i++)
                {
                    if (coord.x - i < 0 || itemsType[coord.x - i, coord.y] == ItemType.BARRIAR)
                        break;
                    bombRange[coord.x - i, coord.y].Add(bomb);
                    explosionRange[coord.x - i, coord.y]++;
                }
                //向右遍历
                for (int i = 1; i <= bombRadius; i++)
                {
                    if (coord.x + i >= xColumn || itemsType[coord.x + i, coord.y] == ItemType.BARRIAR)
                        break;
                    bombRange[coord.x + i, coord.y].Add(bomb);
                    explosionRange[coord.x + i, coord.y]++;
                }
                //向下遍历
                for (int i = 1; i <= bombRadius; i++)
                {
                    if (coord.y + i >= yRow || itemsType[coord.x, coord.y + i] == ItemType.BARRIAR)
                        break;
                    bombRange[coord.x, coord.y + i].Add(bomb);
                    explosionRange[coord.x, coord.y + i]++;
                }
                bomb.host = host;
                bombNumbers[host]++;
                return bombObject;
            }
        }
        return null;
    }


    public IEnumerator BombExplode(GameObject bombObject)
    {
        //Debug.Log("BombExplosion");
        Bomb bomb = bombObject.GetComponent<Bomb>();
        int x = bomb.x;
        int y = bomb.y;
        int radius = bomb.radius;

        itemsType[bomb.x, bomb.y] = ItemType.EMPTY;

        //从放炸弹的人的炸弹列表中去除
        bombNumbers[bomb.host]--;

        GetComponent<AudioSource>().PlayOneShot(explosionAudio);
        ExplodeEffect(x, y, -1, -1);
        yield return new WaitForSeconds(0.01f);

        bool[] end = { false, false, false, false }; //0:up 1:left 2:right 3:down
        bool[] stop = { false, false, false, false };//从爆炸范围中移除炸弹
        bombRange[x, y].Remove(bomb);
        for (int i = 1; i <= radius; i++)
        {
            //Debug.Log("explosion");
            //向左遍历
            int isEnd = 0;
            if (x - i < 0 || itemsType[x - i, y] == ItemType.BARRIAR)
            {
                if (!end[1])
                    end[1] = true;
                stop[1] = true;
            }

            if (!stop[1])
            {
                bombRange[x - i, y].Remove(bomb);
                if (end[1])
                {
                    explosionRange[x - i, y]--;
                    //Debug.Log("(" + (x - i) + "," + (y) + ")--");
                }
            }

            if (!end[1] && x - i >= 0)
            {
                if (itemsType[x - i, y] == ItemType.BOX)
                {
                    //Destroy(itemsObject[x - i, y]);
                    //itemsType[x - i, y] = ItemType.EMPTY;
                    //itemsObject[x - i, y] = null;
                    //CreateRandomTool(x - i, y);
                    //isEnd = 1;
                    //end[1] = true;
                    if (Online && PhotonNetwork.IsMasterClient)
                    {
                        PhotonNetwork.Destroy(itemsObject[x - i, y]);
                        itemsType[x - i, y] = ItemType.EMPTY;
                        itemsObject[x - i, y] = null;
                        CreateRandomTool(x - i, y);
                        isEnd = 1;
                        end[1] = true;
                    }
                    else if (!Online)
                    {
                        Destroy(itemsObject[x - i, y]);
                        itemsType[x - i, y] = ItemType.EMPTY;
                        if (Menu.mode==2 && roleList.GetT(bomb.host) != null)
                        {
                            roleList.GetT(bomb.host).GetComponent<Person>().coin++;
                        }
                        itemsObject[x - i, y] = null;
                        CreateRandomTool(x - i, y);
                        isEnd = 1;
                        end[1] = true;
                    }
                }
                if (x <= i || itemsType[x - 1 - i, y] == ItemType.BARRIAR || i == radius)
                {
                    isEnd = 1;
                    end[1] = true;
                }
                ExplodeEffect(x - i, y, isEnd, 1);
            }



            //向右遍历
            isEnd = 0;
            if (x + i >= xColumn || itemsType[x + i, y] == ItemType.BARRIAR)
            {
                if (!end[2])
                    end[2] = true;
                stop[2] = true;
            }

            if (!stop[2])
            {
                bombRange[x + i, y].Remove(bomb);
                if (end[2])
                {
                    explosionRange[x + i, y]--;
                    //Debug.Log("(" + (x + i) + "," + (y) + ")--");
                }
            }

            if (!end[2] && x + i < xColumn)
            {
                if (itemsType[x + i, y] == ItemType.BOX)
                {
                    //Destroy(itemsObject[x + i, bomb.y]);
                    //itemsType[x + i, y] = ItemType.EMPTY;
                    //itemsObject[x + i, y] = null;
                    //CreateRandomTool(x + i, y);
                    //isEnd = 1;
                    //end[2] = true;
                    if (Online && PhotonNetwork.IsMasterClient)
                    {
                        PhotonNetwork.Destroy(itemsObject[x + i, y]);
                        itemsType[x + i, y] = ItemType.EMPTY;
                        itemsObject[x + i, y] = null;
                        CreateRandomTool(x + i, y);
                        isEnd = 1;
                        end[2] = true;
                    }
                    else if (!Online)
                    {
                        Destroy(itemsObject[x + i, y]);
                        if (Menu.mode == 2 && roleList.GetT(bomb.host) != null)
                        {
                            roleList.GetT(bomb.host).GetComponent<Person>().coin++;
                        }
                        itemsType[x + i, y] = ItemType.EMPTY;
                        itemsObject[x + i, y] = null;
                        CreateRandomTool(x + i, y);
                        isEnd = 1;
                        end[2] = true;
                    }
                }
                if (x + i + 1 >= xColumn || itemsType[x + 1 + i, y] == ItemType.BARRIAR || i == radius)
                {
                    isEnd = 1;
                    end[2] = true;
                }
                ExplodeEffect(x + i, y, isEnd, 2);

            }



            //向上遍历
            isEnd = 0;
            if (y < i || itemsType[x, y - i] == ItemType.BARRIAR)
            {
                if (!end[0])
                    end[0] = true;
                stop[0] = true;
            }

            if (!stop[0])
            {
                bombRange[x, y - i].Remove(bomb);
                if (end[0])
                {
                    explosionRange[x, y - i]--;
                    //Debug.Log("(" + (x) + "," + (y - i) + ")--");
                }
            }

            if (!end[0] && y - i >= 0)
            {
                if (itemsType[x, y - i] == ItemType.BOX)
                {
                    //Destroy(itemsObject[x, y - i]);
                    //itemsType[x, y - i] = ItemType.EMPTY;
                    //itemsObject[x, y - i] = null;
                    //CreateRandomTool(x, y - i);
                    //isEnd = 1;
                    //end[0] = true;
                    if (Online && PhotonNetwork.IsMasterClient)
                    {
                        PhotonNetwork.Destroy(itemsObject[x, y - i]);
                        itemsType[x, y - i] = ItemType.EMPTY;
                        itemsObject[x, y - i] = null;
                        CreateRandomTool(x, y - i);
                        isEnd = 1;
                        end[0] = true;
                    }
                    else if (!Online)
                    {
                        Destroy(itemsObject[x, y - i]);
                        if (Menu.mode == 2 && roleList.GetT(bomb.host) != null)
                        {
                            roleList.GetT(bomb.host).GetComponent<Person>().coin++;
                        }
                        itemsType[x, y - i] = ItemType.EMPTY;
                        itemsObject[x, y - i] = null;
                        CreateRandomTool(x, y - i);
                        isEnd = 1;
                        end[0] = true;
                    }
                }
                if (y <= i || itemsType[x, y - 1 - i] == ItemType.BARRIAR || i == bomb.radius)
                {
                    isEnd = 1;
                    end[0] = true;
                }
                ExplodeEffect(x, y - i, isEnd, 0);

            }

            //向下遍历
            isEnd = 0;
            if (bomb.y + i >= yRow || itemsType[bomb.x, bomb.y + i] == ItemType.BARRIAR)
            {
                if (!end[3])
                    end[3] = true;
                stop[3] = true;
            }

            if (!stop[3])
            {
                bombRange[x, y + i].Remove(bomb);
                if (end[3])
                {
                    explosionRange[x, y + i]--;
                    //Debug.Log("(" + (x) + "," + (y + i) + ")--");
                }
            }

            if (!end[3] && bomb.y + i < yRow)
            {
                if (itemsType[bomb.x, bomb.y + i] == ItemType.BOX)
                {
                    //Destroy(itemsObject[bomb.x, bomb.y + i]);
                    //itemsType[bomb.x, bomb.y + i] = ItemType.EMPTY;
                    //itemsObject[bomb.x, bomb.y + i] = null;
                    //CreateRandomTool(x, y + i);
                    //isEnd = 1;
                    //end[3] = true;
                    if (Online && PhotonNetwork.IsMasterClient)
                    {
                        PhotonNetwork.Destroy(itemsObject[bomb.x, bomb.y + i]);
                        itemsType[bomb.x, bomb.y + i] = ItemType.EMPTY;
                        itemsObject[bomb.x, bomb.y + i] = null;
                        CreateRandomTool(bomb.x, bomb.y + i);
                        isEnd = 1;
                        end[3] = true;
                    }
                    else if (!Online)
                    {
                        Destroy(itemsObject[bomb.x, bomb.y + i]);
                        if (Menu.mode == 2 && roleList.GetT(bomb.host) != null)
                        {
                            roleList.GetT(bomb.host).GetComponent<Person>().coin++;
                        }
                        itemsType[bomb.x, bomb.y + i] = ItemType.EMPTY;
                        itemsObject[bomb.x, bomb.y + i] = null;
                        CreateRandomTool(bomb.x, bomb.y + i);
                        isEnd = 1;
                        end[3] = true;
                    }

                }
                if (bomb.y + i + 1 >= yRow || itemsType[bomb.x, bomb.y + 1 + i] == ItemType.BARRIAR || i == bomb.radius)
                {
                    isEnd = 1;
                    end[3] = true;
                }
                ExplodeEffect(bomb.x, bomb.y + i, isEnd, 3);

            }


            yield return new WaitForSeconds(0.02f);



        }

        //Destroy(itemsObject[bomb.x, bomb.y]);
        //itemsObject[bomb.x, bomb.y] = null;
        if (!Online)
        {
            Destroy(itemsObject[bomb.x, bomb.y]);
            itemsObject[bomb.x, bomb.y] = null;
        }
        else if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Destroy(itemsObject[bomb.x, bomb.y]);
            itemsObject[bomb.x, bomb.y] = null;
        }


    }


    //爆炸效果 isEnd:是否是末端(1:末端 0:中间 -1:中心) direction:方向 0:up 1:left 2:right 3:down
    private void ExplodeEffect(int x, int y, int isEnd, int direction)
    {

        Vector3 createPosition = CorrectPosition(x, y);
        GameObject explosionGo = null;
        if (isEnd == -1 && direction == -1)
        {
            if (!Online)
                explosionGo = Instantiate(item[5], createPosition, Quaternion.identity);
            else
                explosionGo = PhotonNetwork.Instantiate(item[5].name, createPosition, Quaternion.identity);
        }
        if (isEnd == 1 || isEnd == 0)
        {
            switch (direction)
            {

                case 0:
                    if (!Online)
                    {
                        explosionGo = Instantiate(item[5], createPosition, Quaternion.Euler(0, 0, 180));
                    }
                    else
                    {
                        explosionGo = PhotonNetwork.Instantiate(item[5].name, createPosition, Quaternion.Euler(0, 0, 180));
                    }
                    break;
                case 1:
                    //explosionGo = Instantiate(item[5], createPosition, Quaternion.Euler(0, 0, 270));
                    if (!Online)
                    {
                        explosionGo = Instantiate(item[5], createPosition, Quaternion.Euler(0, 0, 270));
                    }
                    else
                    {
                        explosionGo = PhotonNetwork.Instantiate(item[5].name, createPosition, Quaternion.Euler(0, 0, 270));
                    }
                    break;
                case 2:
                    //explosionGo = Instantiate(item[5], createPosition, Quaternion.Euler(0, 0, 90));
                    if (!Online)
                    {
                        explosionGo = Instantiate(item[5], createPosition, Quaternion.Euler(0, 0, 90));
                    }
                    else
                    {
                        explosionGo = PhotonNetwork.Instantiate(item[5].name, createPosition, Quaternion.Euler(0, 0, 90));
                    }
                    break;
                case 3:
                    //explosionGo = Instantiate(item[5], createPosition, Quaternion.identity);
                    if (!Online)
                    {
                        explosionGo = Instantiate(item[5], createPosition, Quaternion.identity);
                    }
                    else
                    {
                        explosionGo = PhotonNetwork.Instantiate(item[5].name, createPosition, Quaternion.identity);
                    }
                    break;
            }
        }

        if (itemsType[x, y] == ItemType.BOMB)
        {
            itemsObject[x, y].GetComponent<Bomb>().explodeTime = 0.1f;
        }
        if (explosionGo != null)
        {
            explosionGo.GetComponent<Explosion>().x = x;
            explosionGo.GetComponent<Explosion>().y = y;
            explosionGo.GetComponent<Explosion>().isEnd = isEnd;
        }
    }

    public void RaserAttack(int orientation, Vector3 position_start)
    {
        Location coord = GetCoord(position_start);
        //根据当前人物的朝向进行激光武器的释放
        //0:下 1:左 2:右 3:上
        GetComponent<AudioSource>().PlayOneShot(explosionAudio);
        bool end = false; //0:up 1:left 2:right 3:down
        //向右遍历
        if (orientation == 2)
        {
            int x = coord.x + 1;
            int y = coord.y;

            for (int i = 0; x + i < xColumn; i++)
            {
                int isEnd = 0;
                //当前位置是否是激光顶点
                if (itemsType[x + i, y] == ItemType.BARRIAR)
                {
                    if (!end)
                    {
                        end = true;
                        isEnd = 1;
                    }
                }
                if (!end && x + i < xColumn)
                {
                    if (itemsType[x + i, y] == ItemType.BOX)
                    {
                        Destroy(itemsObject[x + i, y]);
                        if (Menu.mode == 2 && roleList.GetT(0) != null)
                        {
                            roleList.GetT(0).GetComponent<Person>().coin++;
                        }
                        itemsType[x + i, y] = ItemType.EMPTY;
                        itemsObject[x + i, y] = null;
                        CreateRandomTool(x + i, y);
                        end = true;
                        isEnd = 1;
                    }
                    if (x + i + 1 >= xColumn || itemsType[x + 1 + i, y] == ItemType.BARRIAR)
                    {
                        isEnd = 1;
                    }
                    if (i == 0)
                        RaserEffect(x + i, y, -1, orientation);
                    else
                        RaserEffect(x + i, y, isEnd, orientation);
                }

            }
        }
        //向上遍历
        else if (orientation == 3)
        {
            int x = coord.x;
            int y = coord.y - 1;

            for (int i = 0; y - i >= 0; i++)
            {
                int isEnd = 0;
                //当前位置是否是激光顶点
                if (itemsType[x, y - i] == ItemType.BARRIAR)
                {
                    if (!end)
                    {
                        end = true;
                        isEnd = 1;
                    }
                }
                if (!end && y - i >= 0)
                {
                    if (itemsType[x, y - i] == ItemType.BOX)
                    {
                        Destroy(itemsObject[x, y - i]);
                        if (Menu.mode == 2 && roleList.GetT(0) != null)
                        {
                            roleList.GetT(0).GetComponent<Person>().coin++;
                        }
                        itemsType[x, y - i] = ItemType.EMPTY;
                        itemsObject[x, y - i] = null;
                        CreateRandomTool(x, y - i);
                        end = true;
                        isEnd = 1;
                    }
                    if (y - i - 1 < 0 || itemsType[x, y - i - 1] == ItemType.BARRIAR)
                    {
                        isEnd = 1;
                    }
                    //ExplodeEffect(x + i, y, isEnd, 2);
                    RaserEffect(x, y - i, isEnd, orientation);
                }

            }
        }
        else if (orientation == 1)
        {//向左遍历
            int x = coord.x - 1;
            int y = coord.y;

            for (int i = 0; x - i >= 0; i++)
            {
                int isEnd = 0;
                //当前位置是否是激光顶点
                if (itemsType[x - i, y] == ItemType.BARRIAR)
                {
                    if (!end)
                    {
                        end = true;
                        isEnd = 1;
                    }
                }
                if (!end && x - i >= 0)
                {
                    if (itemsType[x - i, y] == ItemType.BOX)
                    {
                        Destroy(itemsObject[x - i, y]);
                        if (Menu.mode == 2 && roleList.GetT(0) != null)
                        {
                            roleList.GetT(0).GetComponent<Person>().coin++;
                        }
                        itemsType[x - i, y] = ItemType.EMPTY;
                        itemsObject[x - i, y] = null;
                        CreateRandomTool(x - i, y);
                        end = true;
                        isEnd = 1;
                    }
                    if (x - i - 1 < 0 || itemsType[x - i - 1, y] == ItemType.BARRIAR)
                    {
                        isEnd = 1;
                    }
                    //ExplodeEffect(x + i, y, isEnd, 2);
                    RaserEffect(x - i, y, isEnd, orientation);
                }

            }
        }
        else if (orientation == 0)
        {//向下遍历
            int x = coord.x;
            int y = coord.y + 1;

            for (int i = 0; y + i < yRow; i++)
            {
                int isEnd = 0;
                //当前位置是否是激光顶点
                if (itemsType[x, y + i] == ItemType.BARRIAR)
                {
                    if (!end)
                    {
                        end = true;
                        isEnd = 1;
                    }
                }
                if (!end && y + i < yRow)
                {
                    if (itemsType[x, y + i] == ItemType.BOX)
                    {
                        Destroy(itemsObject[x, y + i]);
                        if (Menu.mode == 2 && roleList.GetT(0) != null)
                        {
                            roleList.GetT(0).GetComponent<Person>().coin++;
                        }
                        itemsType[x, y + i] = ItemType.EMPTY;
                        itemsObject[x, y + i] = null;
                        CreateRandomTool(x, y + i);
                        end = true;
                        isEnd = 1;
                    }
                    if (y + i + 1 >= yRow || itemsType[x, y + i + 1] == ItemType.BARRIAR)
                    {
                        isEnd = 1;
                    }
                    RaserEffect(x, y + i, isEnd, orientation);
                }

            }
        }
    }

    private void RaserEffect(int x, int y, int isEnd, int orientation)
    {
        Vector3 createPosition = CorrectPosition(x, y);
        GameObject explosionGo = null;
        switch (orientation)
        {
            case 3:
                if (!Online)
                {
                    explosionGo = Instantiate(item[7], createPosition, Quaternion.Euler(0, 0, 180));
                }
                break;
            case 1:
                //explosionGo = Instantiate(item[5], createPosition, Quaternion.Euler(0, 0, 270));
                if (!Online)
                {
                    explosionGo = Instantiate(item[7], createPosition, Quaternion.Euler(0, 0, 270));
                }
                break;
            case 2:
                //explosionGo = Instantiate(item[5], createPosition, Quaternion.Euler(0, 0, 90));
                if (!Online)
                {
                    explosionGo = Instantiate(item[7], createPosition, Quaternion.Euler(0, 0, 90));
                }
                break;
            case 0:
                //explosionGo = Instantiate(item[5], createPosition, Quaternion.identity);
                if (!Online)
                {
                    explosionGo = Instantiate(item[7], createPosition, Quaternion.identity);
                }
                break;
        }
        if (explosionGo != null)
        {
            explosionGo.GetComponent<Wave>().x = x;
            explosionGo.GetComponent<Wave>().y = y;
            explosionGo.GetComponent<Wave>().isEnd = isEnd;
        }
    }


    private void CreateRandomTool(int x, int y)
    {
        if (itemsType[x, y] == ItemType.EMPTY)
        {
            int n = Random.Range(0, 100);
            if (n <= 49 )
                return;
            Vector3 createPosition = CorrectPosition(x, y);
            GameObject toolGo = null;
            if (n >= 50 && n < 65)//����ը��
            {
                if (!Online)
                {
                    toolGo = Instantiate(toolPrefab[0], createPosition, Quaternion.identity);
                }
                else
                {
                    toolGo = PhotonNetwork.Instantiate(toolPrefab[0].name, createPosition, Quaternion.identity);
                }

            }
            else if (n >= 65 && n < 80)//����ҩˮ
            {
                //toolGo = Instantiate(toolPrefab[1], createPosition, Quaternion.identity);
                if (!Online)
                {
                    toolGo = Instantiate(toolPrefab[1], createPosition, Quaternion.identity);
                }
                else
                {
                    toolGo = PhotonNetwork.Instantiate(toolPrefab[1].name, createPosition, Quaternion.identity);
                }

            }
            else if (n >= 80 && n < 90)//����Ь��
            {
                //toolGo = Instantiate(toolPrefab[2], createPosition, Quaternion.identity);
                if (!Online)
                {
                    toolGo = Instantiate(toolPrefab[2], createPosition, Quaternion.identity);
                }
                else
                {
                    toolGo = PhotonNetwork.Instantiate(toolPrefab[2].name, createPosition, Quaternion.identity);
                }
            }
            else if (n >= 90)//������
            {
                //toolGo = Instantiate(toolPrefab[3], createPosition, Quaternion.identity);
                if (!Online)
                {
                    toolGo = Instantiate(toolPrefab[3], createPosition, Quaternion.identity);
                }
                else
                {
                    toolGo = PhotonNetwork.Instantiate(toolPrefab[3].name, createPosition, Quaternion.identity);
                }

            }
            if (toolGo != null)
            {
                toolGo.GetComponent<Tools>().x = x;
                toolGo.GetComponent<Tools>().y = y;
            }

            itemsType[x, y] = ItemType.TOOL;
        }
    }

    //���ʤ��
    private void Victory()
    {
        GameObject.Find("background").GetComponent<AudioSource>().Stop();
        GetComponent<AudioSource>().PlayOneShot(gameWinAudio);
        ui.Victory();
    }

    //ʧ��
    private void Defeat()
    {
        GameObject.Find("background").GetComponent<AudioSource>().Stop();
        GetComponent<AudioSource>().PlayOneShot(gameOverAudio);
        ui.Defeat();
    }

    public void Pause()
    {
        if (PhotonNetwork.IsConnected)
            return;
        Time.timeScale = 0f;
        AudioListener.pause = true;
    }

    public void Continue()
    {
        if (PhotonNetwork.IsConnected)
            return;
        Time.timeScale = 1f;
        AudioListener.pause = !Menu.sound;
    }

    private void GetOnlineInfo()
    {
        allplayers = PhotonNetwork.PlayerList;
        int j = 0;
        object Roleindex;
        object Playerindex;
        OnlinePlayerRoleIndex = new int[4];
        OnlinePlayerIndex = new int[4];
        foreach (var p in allplayers)
        {
            if (p.CustomProperties.TryGetValue("RoleIndex", out Roleindex) && p.CustomProperties.TryGetValue("LocalIndex", out Playerindex))
            {
                if (p == PhotonNetwork.MasterClient)
                    MasterClientIndex = j;
                OnlinePlayerRoleIndex[j] = (int)Roleindex;
                OnlinePlayerIndex[j++] = (int)Playerindex;
                leftRoleNOList.Remove((int)Roleindex);
                if (p == PhotonNetwork.LocalPlayer)
                {
                    OnlineLocalIndex = (int)Roleindex;
                    OnlineLocalPlayerIndex = (int)Playerindex;
                    //birthPlaces.RemoveAt((int)Playerindex);
                }
            }
        }
        InRoomPeople = j;
        OnlinePlayerNum = j;
    }

    [PunRPC]
    void ShowGameResult(int i)
    {
        if (this.OnlineLocalPlayerIndex == i)
        {
            ui.Victory();
        }
        else
        {
            ui.Defeat();
        }
    }
}


public class Array<T>
{
    public T[] list;
    private int capacity;
    public int Capacity { get => capacity; }
    private int count;
    public int Count { get => count; }
    private int cnt = 0;

    public Array(int size)
    {
        capacity = size;
        count = 0;
        list = new T[size + 1];
        for (int i = 0; i < capacity; i++)
            list[i] = default;
    }

    public bool Add(T t)
    {
        //Debug.Log("Add");
        if (count >= capacity)
        {
            return false;
        }
        while (list[cnt] != null) { cnt++; }
        list[cnt++] = t;
        count++;
        return true;
    }

    public bool AddByIndex(T t, int i)
    {
        if (count >= capacity)
        {
            return false;
        }
        list[i] = t;
        count++;
        return true;

    }

    public int IndexOf(T t)
    {
        int index = -1;
        for (int i = 0; i < capacity; i++)
        {
            if (list[i].Equals(t))
                index = i;
        }
        return index;
    }

    public bool Remove(T t)
    {
        //Debug.Log("Remove start");
        for (int i = 0; i < capacity; i++)
        {
            if (list[i] != null && list[i].Equals(t))
            {
                list[i] = default;
                count--;
                return true;
            }

        }
        return false;
    }

    public T GetT(int index)
    {
        if (index >= 0 && index < capacity)
            return list[index];

        return default;
    }

    public int GetCount()
    {
        int j = 0;
        for (int i = 0; i < 4; i++)
        {
            if (list[i] != null)
            {
                j++;
            }
        }
        return j;
    }
}
