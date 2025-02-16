using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class GridManager : MonoBehaviour
{ 
    [Header("Grid Settings")]
    public int width = 20;  
    public int height = 10; 
    public float cellSize = 1f; 

    public GameObject whiteNodePrefab; 
    public GameObject blackNodePrefab; 
    public GameObject enemyPrefab; 
    public GameObject[] obstaclePrefabs;
    public GameObject bloodEffect;

    public int maxObstacles = 30; 
    
    private int enemyKillCount = 0;
    public TMP_Text EnemTextCount;

    private Dictionary<Vector2Int, Node> grid = new Dictionary<Vector2Int, Node>();
    
    Animator playerAnimator;
    
    public AudioSource playerAudio;
    
    private float fillPercentObstacleCount;

    void Start()
    {
        playerAnimator = GameObject.FindGameObjectWithTag("Player").GetComponent<Animator>();
        
        // PlayerPrefs'ten int olarak alınan değeri 1 ile 30 arasında sınırla
        fillPercentObstacleCount = Mathf.Clamp(PlayerPrefs.GetInt("FillPercent", 15), 1, 30);
        Debug.Log("Oyun Sahnesinde Alınan Fill Percent: " + fillPercentObstacleCount);

        maxObstacles = (int)fillPercentObstacleCount;
        
        GenerateGrid();
        GenerateObstacles();
        CenterCamera();
        StartCoroutine(SpawnEnemiesLoop());
    }

    void GenerateGrid()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2Int pos = new Vector2Int(x, y);
                Node newNode = new Node(pos, true);  // Başlangıçta tüm hücreler yürünebilir
                grid[pos] = newNode;

                GameObject prefabToUse = (x + y) % 2 == 0 ? whiteNodePrefab : blackNodePrefab;
                GameObject nodeObj = Instantiate(prefabToUse, new Vector3(x, y, 0), Quaternion.identity);
                nodeObj.transform.parent = transform;
            }
        }
    }

    void GenerateObstacles()
    {
        int placedObstacles = 0;
        List<Vector2Int> availablePositions = new List<Vector2Int>(grid.Keys);

        while (placedObstacles < maxObstacles && availablePositions.Count > 0)
        {
            int index = Random.Range(0, availablePositions.Count);
            Vector2Int position = availablePositions[index];

            if (grid[position].isWalkable)
            {
                grid[position].isWalkable = false;
                SpawnRandomObstacle(position);
                placedObstacles++;
            }

            availablePositions.RemoveAt(index);
        }
    }

    void SpawnRandomObstacle(Vector2Int position)
    {
        if (obstaclePrefabs.Length == 0) return;

        int randomIndex = Random.Range(0, obstaclePrefabs.Length);
        GameObject obstacle = Instantiate(obstaclePrefabs[randomIndex], new Vector3(position.x, position.y, 0), Quaternion.identity);
        obstacle.transform.parent = transform;
    }

    IEnumerator SpawnEnemiesLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(3f);
            SpawnEnemy();
        }
    }

    void SpawnEnemy()
    {
        Vector2Int spawnPos = GetRandomWalkablePosition();
        if (spawnPos != Vector2Int.zero)
        {
            Instantiate(enemyPrefab, new Vector3(spawnPos.x, spawnPos.y, 0), Quaternion.identity);
        }
    }

    Vector2Int GetRandomWalkablePosition()
    {
        List<Vector2Int> walkablePositions = new List<Vector2Int>();

        foreach (var node in grid.Values)
        {
            if (node.isWalkable)
                walkablePositions.Add(node.position);
        }

        if (walkablePositions.Count == 0) return Vector2Int.zero;

        return walkablePositions[Random.Range(0, walkablePositions.Count)];
    }

    public Node GetNode(Vector2Int position)
    {
        return grid.ContainsKey(position) ? grid[position] : null;
    }

    public bool IsWalkable(Vector2Int position)
    {
        return grid.ContainsKey(position) && grid[position].isWalkable;
    }

    public List<Node> GetNeighbors(Node node)
    {
        List<Node> neighbors = new List<Node>();
        Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

        foreach (var dir in directions)
        {
            Vector2Int neighborPos = node.position + dir;
            if (grid.ContainsKey(neighborPos) && grid[neighborPos].isWalkable)
            {
                neighbors.Add(grid[neighborPos]);
            }
        }
        return neighbors;
    }

    void CenterCamera()
    {
        Camera mainCamera = Camera.main;
        if (mainCamera == null) return;

     
        Vector3 centerPosition = new Vector3((width - 1) / 2f, (height - 1) / 2f, -10f);
        mainCamera.transform.position = centerPosition;
        
        float aspectRatio = (float)Screen.width / Screen.height;
        
        float cameraSizeX = width / 2f;
        float cameraSizeY = height / 2f;
        
        if (aspectRatio >= 1f)
        {
            mainCamera.orthographicSize = cameraSizeY;
        }
        else
        {
            mainCamera.orthographicSize = cameraSizeX;
        }
    }
    
    // Düşman öldürme sayısını arttıran fonksiyon
    public void IncrementEnemyKillCount()
    {
        playerAnimator.SetTrigger("Attack");
        bloodEffect.SetActive(true);
        Invoke("EffectFalse",0.4f);
        playerAudio.Play();
        enemyKillCount++;  // Öldürülen düşman sayısını artır
        EnemTextCount.text = enemyKillCount.ToString();
        Invoke("IdleAnimPlayer",0.4f);
        Debug.Log("Öldürülen Düşman Sayısı: " + enemyKillCount);  
    }

    private void EffectFalse()
    {
        bloodEffect.SetActive(false);
    }

}
