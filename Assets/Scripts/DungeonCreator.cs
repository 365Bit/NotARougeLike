using System;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

[RequireComponent(typeof(NavMeshSurface))]
public class DungeonCreator : MonoBehaviour
{
    private NavMeshSurface navMeshSurface;

    public int dungeonWidth, dungeonLength;
    public int roomWidthMin, roomLengthMin;
    public int maxIterations;
    public int corridorWidth;
    public int enemyAmount;
    public float lootProb, shopProb;
    public Material material;
    [Range(0.0f, 0.3f)]
    public float roomBottomCornerModifier;
    [Range(0.7f, 1.0f)]
    public float roomTopCornerMidifier;
    [Range(0, 2)]
    public int roomOffset;

    public GameObject wallPrefab, pillarPrefab, playerPrefab, chestPrefab, enemyPrefab, shopPrefab, navPointPrefab;
    List<Vector3Int> possibleDoorVerticalPosition;
    List<Vector3Int> possibleDoorHorizontalPosition;
    List<Vector3Int> possibleWallHorizontalPosition;
    List<Vector3Int> possibleWallVerticalPosition;

    ItemDefinitions itemDefinitions;

    [Header("User Interfaces")]
    public GameObject canvas;

    // Start is called before the first frame update
    void Start()
    {
        itemDefinitions = GameObject.Find("Definitions").GetComponent<ItemDefinitions>();

        navMeshSurface = GetComponent<NavMeshSurface>();

        CreateDungeon();
    }

    public void CreateDungeon()
    {
        DestroyAllChildren();
        DugeonGenerator generator = new DugeonGenerator(dungeonWidth, dungeonLength);
        var listOfRooms = generator.CalculateDungeon(maxIterations,
            roomWidthMin,
            roomLengthMin,
            roomBottomCornerModifier,
            roomTopCornerMidifier,
            roomOffset,
            corridorWidth);
        GameObject wallParent = new GameObject("WallParent");
        wallParent.transform.parent = transform;
        possibleHorizontalDoorPosition = new List<Vector3Int>();
        possibleVerticalDoorPosition = new List<Vector3Int>();
        possibleHorizontalWallPosition = new List<Vector3Int>();
        possibleVerticalWallPosition = new List<Vector3Int>();
        CreatePlayer(listOfRooms);
        for (int i = 0; i < listOfRooms.Count; i++)
        {
            CreateMesh(listOfRooms[i].BottomLeftAreaCorner, listOfRooms[i].TopRightAreaCorner, listOfRooms[i].Type);
        }
        CreateWalls(wallParent);
        CreatePillars(listOfRooms);
        CreateLoot(listOfRooms);
        CreateEnemy(listOfRooms);
        CreateNavPoint(listOfRooms);
        CreateShop(listOfRooms);

        navMeshSurface.BuildNavMesh();
    }

    private void CreatePlayer(List<Node> listOfRooms)
    {
        Node room = listOfRooms[UnityEngine.Random.Range(0, listOfRooms.Count)];
        int playerPosX = UnityEngine.Random.Range(room.BottomLeftAreaCorner.x + 2, room.BottomRightAreaCorner.x - 1);
        int playerPosY = UnityEngine.Random.Range(room.BottomLeftAreaCorner.y + 2, room.TopLeftAreaCorner.y - 1);
            Vector3 playerPos = new Vector3(
                playerPosX,
                2,
                playerPosY);
        //playerPrefab.transform.SetPositionAndRotation(playerPos, Quaternion.identity);
        //GameObject player = Instantiate(playerPrefab, playerPos, Quaternion.identity);
        Player player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        player.transform.SetPositionAndRotation(playerPos, Quaternion.identity);
        player.name = "Player";
        player.StartGame();

        canvas.SetActive(true);
    }

    private void CreateEnemy(List<Node> listOfRooms)
    {
        bool isEnoughEnemies = false;
        int counter = 0;
        while (!isEnoughEnemies)
        {
            foreach( var room in listOfRooms)
            {
                if (room.Type == "room")
                {
                    if(UnityEngine.Random.Range(0f,1f) > 0.5)
                    {
                        int enemyPosX = UnityEngine.Random.Range(room.BottomLeftAreaCorner.x + 2, room.BottomRightAreaCorner.x - 1);
                        int enemyPosY = UnityEngine.Random.Range(room.BottomLeftAreaCorner.y + 2, room.TopLeftAreaCorner.y - 1);
                        Vector3 enemyPos = new Vector3(
                            enemyPosX,
                            1,
                            enemyPosY);

                        GameObject foe = Instantiate(enemyPrefab, enemyPos, Quaternion.identity);
                        Opponent opponent = foe.GetComponent<Opponent>();
                        opponent.spawnRoom = room;

                        counter++;
                    }
                    if (counter == enemyAmount)
                    {
                        break;
                    }
                }
            }
            isEnoughEnemies = counter >= enemyAmount;
        }
    }

    private void CreateNavPoint(List<Node> listOfRooms)
    {
        foreach( var room in listOfRooms)
        {
            if (room.Type == "room")
            {
                int navPointX = room.BottomLeftAreaCorner.x + 5;
                int navPointY = room.BottomLeftAreaCorner.y + 5;
                Vector3 navPointPos = new Vector3(
                    navPointX,
                    0.5f,
                    navPointY);

                room.navPointList.Add(Instantiate(navPointPrefab, navPointPos, Quaternion.identity).GetComponent<NavPoint>());

                navPointX = room.TopLeftAreaCorner.x + 5;
                navPointY = room.TopLeftAreaCorner.y - 5;
                navPointPos = new Vector3(
                    navPointX,
                    0.5f,
                    navPointY);

                room.navPointList.Add(Instantiate(navPointPrefab, navPointPos, Quaternion.identity).GetComponent<NavPoint>());

                navPointX = room.TopRightAreaCorner.x - 5;
                navPointY = room.TopRightAreaCorner.y - 5;
                navPointPos = new Vector3(
                    navPointX,
                    0.5f,
                    navPointY);

                room.navPointList.Add(Instantiate(navPointPrefab, navPointPos, Quaternion.identity).GetComponent<NavPoint>());

                navPointX = room.BottomRightAreaCorner.x - 5;
                navPointY = room.BottomRightAreaCorner.y + 5;
                navPointPos = new Vector3(
                    navPointX,
                    0.5f,
                    navPointY);

                room.navPointList.Add(Instantiate(navPointPrefab, navPointPos, Quaternion.identity).GetComponent<NavPoint>());
            }
        }
    }


    private void SetRandomShopItems(ItemSlot[] slots) {
        foreach (var slot in slots) {
            slot.storedItem = null;

            // select random definition
            var random = UnityEngine.Random.Range(0f,1f);
            foreach (ItemDefinition def in itemDefinitions.definitions) {
                if (random <= def.shopProbability) {
                    slot.storedItem = def;
                    slot.count = 1;
                    break;
                }
                random -= def.shopProbability;
            }
        }
    }

    private void SetRandomDroppedItem(ItemSlot slot) {
        slot.storedItem = null;

        // select random definition
        var random = UnityEngine.Random.Range(0f,1f);
        foreach (ItemDefinition def in itemDefinitions.definitions) {
            if (random <= def.shopProbability) {
                slot.storedItem = def;
                slot.count = 1;
                break;
            }
            random -= def.shopProbability;
        }
    }

    private void CreateShop(List<Node> listOfRooms)
    {
        foreach( var room in listOfRooms)
        {
            if (room.Type == "room")
            {
                // use center of room
                int shopX = (room.BottomLeftAreaCorner.x + 1 + room.BottomRightAreaCorner.x) / 2;
                int shopY = (room.BottomLeftAreaCorner.y + 1 + room.TopLeftAreaCorner.y) / 2;
                Vector3 shopPos = new Vector3(
                    shopX,
                    0.35f,
                    shopY);
                if(UnityEngine.Random.Range(0f,1f) < shopProb)
                {
                    var shop = Instantiate(shopPrefab, shopPos, Quaternion.identity);
                    // TODO
                    shop.GetComponent<ShopRenderer>().AddRandomItems();
                }
            }
        }
    }

    private void CreateLoot(List<Node> listOfRooms)
    {
        foreach( var room in listOfRooms)
        {
            if (room.Type == "room")
            {
                int chestX = UnityEngine.Random.Range(room.BottomLeftAreaCorner.x + 2, room.BottomRightAreaCorner.x - 1);
                int chestY = UnityEngine.Random.Range(room.BottomLeftAreaCorner.y + 2, room.TopLeftAreaCorner.y - 1);
                Vector3 chestPos = new Vector3(
                    chestX,
                    0.35f,
                    chestY);
                if(UnityEngine.Random.Range(0f,1f) > lootProb)
                {
                    var chest = Instantiate(chestPrefab, chestPos, Quaternion.identity);

                    ItemSlot tmpSlot = new();
                    SetRandomDroppedItem(tmpSlot);
                    chest.GetComponent<DestroyableObject>().SetItem(tmpSlot.storedItem, tmpSlot.count);
                }
            }
        }
    }

    private void CreatePillars(List<Node> listOfRooms)
    {
        foreach (var room in listOfRooms)
        {
            Vector3 bottomLeftAreaCorner = new Vector3(room.BottomLeftAreaCorner.x, 0, room.BottomLeftAreaCorner.y);
            CreatePillar(bottomLeftAreaCorner);
            Vector3 bottomRightAreaCorner = new Vector3(room.BottomRightAreaCorner.x, 0, room.BottomRightAreaCorner.y);
            CreatePillar(bottomRightAreaCorner);
            Vector3 topLeftCorner = new Vector3(room.TopLeftAreaCorner.x, 0, room.TopLeftAreaCorner.y);
            CreatePillar(topLeftCorner);
            Vector3 topRightCorner = new Vector3(room.TopRightAreaCorner.x, 0, room.TopRightAreaCorner.y);
            CreatePillar(topRightCorner);
        }
    }

    private void CreatePillar(Vector3 pillarPos)
    {
        if (pillarPos != Vector3.zero)
        {
            Instantiate(pillarPrefab, pillarPos, Quaternion.identity);
        }
    }

    private void CreateWalls(GameObject wallParent)
    {
        // To Cap the possibleWalls Lists
        possibleHorizontalWallPosition.Add(possibleHorizontalWallPosition[0]);
        possibleVerticalWallPosition.Add(possibleVerticalWallPosition[0]);
        int wallLength;
        float wallDetail;
        float wallPosX = 0;
        int wallPosY = 0;
        float wallPosZ = 0;
        int startX = -1;
        int startZ = -1;
        int temp = 0;
        int wallScaler = 4;
        foreach (var hWallPosition in possibleHorizontalWallPosition)
        {
            // Define starting point
            if (startX == -1)
            {
                startX = hWallPosition.x;
                wallPosZ = hWallPosition.z;
                temp = hWallPosition.x;
            }
            else
            {
                // check if coherent Wall, save end point as temp
                if(hWallPosition.x == temp + 1)
                {
                    temp = hWallPosition.x;
                }
                // (temp - start) = total length of wall
                else
                {
                    wallLength = temp - startX;
                    int segmentCount = (wallLength < wallScaler) ? 1 : wallLength / wallScaler;
                    float xScale = 0;
                    if(segmentCount == 1)
                    {
                        if(wallLength < 4)
                        {
                            int wallOffset = wallLength % wallScaler;
                            xScale = (float)wallOffset / wallScaler;
                        }
                        else
                        {
                            xScale = (float)wallLength / wallScaler;
                        }
                        wallPosX = startX + wallLength / 2;
                        Vector3 wallPos  = new Vector3(
                            wallPosX,
                            wallPosY,
                            wallPosZ
                        );
                        GameObject wall = Instantiate(wallPrefab, wallPos, Quaternion.identity, wallParent.transform);
                        wall.transform.localScale = new Vector3(xScale, 1, 1);
                        wallPosX += wallLength / 2;
                    }
                    else
                    {
                        wallPosX = startX;
                        xScale = (float) wallLength / segmentCount / wallScaler;
                        for (int i = 0; i < segmentCount; i++)
                        {
                            wallPosX += xScale * wallScaler / 2;
                            wallDetail = UnityEngine.Random.Range(-0.1f, 0.1f);
                            Vector3 wallPos  = new Vector3(
                                wallPosX,
                                wallPosY,
                                wallPosZ + wallDetail
                            );
                            GameObject wall = Instantiate(wallPrefab, wallPos, Quaternion.identity, wallParent.transform);
                            wall.transform.localScale = new Vector3(xScale, 1, 1);
                            wallPosX += xScale * wallScaler / 2;
                        }
                    }
                    startX = hWallPosition.x;
                    wallPosZ = hWallPosition.z;
                    temp = hWallPosition.x;
                }
            }
        }
        foreach (var vWallPosition in possibleVerticalWallPosition)
        {
            // Define starting point
            if (startZ == -1)
            {
                startZ = vWallPosition.z;
                wallPosX = vWallPosition.x;
                temp = vWallPosition.z;
            }
            else
            {
                // check if coherent Wall, save end point as temp
                if(vWallPosition.z == temp + 1)
                {
                    temp = vWallPosition.z;
                }
                // (temp - start) = total length of wall
                else
                {
                    wallLength = temp - startZ;
                    int segmentCount = (wallLength < wallScaler) ? 1 : wallLength / wallScaler;
                    float zScale;

                    if(segmentCount == 1)
                    {
                        if(wallLength < 4)
                        {
                            int wallOffset = wallLength % wallScaler;
                            zScale = (float)wallOffset / wallScaler;
                        }
                        else
                        {
                            zScale = (float)wallLength / wallScaler;
                        }
                        wallPosZ = startZ + wallLength / 2;
                        Vector3 wallPos  = new Vector3(
                            wallPosX,
                            wallPosY,
                            wallPosZ
                        );
                        GameObject wall = Instantiate(wallPrefab, wallPos, Quaternion.Euler(0, 90, 0), wallParent.transform);
                        wall.transform.localScale = new Vector3(1, 1, zScale);
                        wallPosX += wallLength / 2;
                    }
                    else
                    {

                        wallPosZ = startZ;
                        zScale = (float) wallLength / segmentCount / wallScaler;
                        for (int i = 0; i < segmentCount; i++)
                        {
                            wallPosZ += zScale * wallScaler / 2;
                            wallDetail = UnityEngine.Random.Range(-0.1f, 0.1f);
                            Vector3 wallPos  = new Vector3(
                                wallPosX + wallDetail,
                                wallPosY,
                                wallPosZ + 2
                            );
                            GameObject wall = Instantiate(wallPrefab, wallPos, Quaternion.Euler(0, 90, 0), wallParent.transform);
                            wall.transform.localScale = new Vector3(1, 1, zScale);
                            wallPosZ += zScale * wallScaler / 2;
                        }
                    }
                    startZ = vWallPosition.z;
                    wallPosX = vWallPosition.x;
                    temp = vWallPosition.z;
                }
            }
        }
    }

    private GameObject CreateWall(GameObject wallParent, Vector3 wallPosition, GameObject wallPrefab, Quaternion rotation)
    {
        return Instantiate(wallPrefab, wallPosition, rotation, wallParent.transform);
    }


    private void CreateMesh(Vector2 bottomLeftCorner, Vector2 topRightCorner, String roomType)
    {
        Vector3 bottomLeftV = new Vector3(bottomLeftCorner.x, 0, bottomLeftCorner.y);
        Vector3 bottomRightV = new Vector3(topRightCorner.x, 0, bottomLeftCorner.y);
        Vector3 topLeftV = new Vector3(bottomLeftCorner.x, 0, topRightCorner.y);
        Vector3 topRightV = new Vector3(topRightCorner.x, 0, topRightCorner.y);

        Vector3[] vertices = new Vector3[]
        {
            topLeftV,
            topRightV,
            bottomLeftV,
            bottomRightV
        };

        Vector2[] uvs = new Vector2[vertices.Length];
        for (int i = 0; i < uvs.Length; i++)
        {
            uvs[i] = new Vector2(vertices[i].x, vertices[i].z);
        }

        int[] triangles = new int[]
        {
            0,
            1,
            2,
            2,
            1,
            3
        };
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;

        GameObject dungeonFloor = new GameObject("Mesh" + bottomLeftCorner, typeof(MeshFilter), typeof(MeshRenderer));

        dungeonFloor.transform.position = Vector3.zero;
        dungeonFloor.transform.localScale = Vector3.one;
        dungeonFloor.GetComponent<MeshFilter>().mesh = mesh;
        dungeonFloor.GetComponent<MeshRenderer>().material = material;
        dungeonFloor.AddComponent<MeshCollider>();
        dungeonFloor.transform.parent = transform;

        for (int row = (int)Math.Ceiling(bottomLeftV.x); row < (int)Math.Ceiling(bottomRightV.x); row++)
        {
            var wallPosition = new Vector3(row, 0, bottomLeftV.z);
            AddWallPositionToList(wallPosition, possibleHorizontalWallPosition, possibleHorizontalDoorPosition);
        }
        for (int row = (int)Math.Ceiling(topLeftV.x); row < (int)Math.Ceiling(topRightCorner.x); row++)
        {
            var wallPosition = new Vector3(row, 0, topRightV.z);
            AddWallPositionToList(wallPosition, possibleHorizontalWallPosition, possibleHorizontalDoorPosition);
        }
        for (int col = (int)Math.Ceiling(bottomLeftV.z); col < (int)Math.Ceiling(topLeftV.z); col++)
        {
            var wallPosition = new Vector3(bottomLeftV.x, 0, col);
            AddWallPositionToList(wallPosition, possibleVerticalWallPosition, possibleVerticalDoorPosition);
        }
        for (int col = (int)Math.Ceiling(bottomRightV.z); col < (int)Math.Ceiling(topRightV.z); col++)
        {
            var wallPosition = new Vector3(bottomRightV.x, 0, col);
            AddWallPositionToList(wallPosition, possibleVerticalWallPosition, possibleVerticalDoorPosition);
        }
    }

    private void AddWallPositionToList(Vector3 wallPosition, List<Vector3Int> wallList, List<Vector3Int> doorList)
    {
        Vector3Int point = Vector3Int.CeilToInt(wallPosition);
        if (wallList.Contains(point)){
            wallList.Remove(point);
        }
        else
        {
            wallList.Add(point);
        }
    }

    private void DestroyAllChildren()
    {
        while(transform.childCount != 0)
        {
            foreach(Transform item in transform)
            {
                DestroyImmediate(item.gameObject);
            }
        }
    }
}
