using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Vector2Int gridPosition;
    private GridManager gridManager;
    private PlayerController player;
    private Pathfinding pathfinding;
    private List<Node> path;
    private float moveSpeed = 3f; // Hareket hızı
    private float stoppingDistance = 0.1f; // Düşman hedefe çok yaklaşınca durmamalı
    private int currentNodeIndex = 0; // Şu anda hangi node'da olduğunu takip etmek için
    private Vector2 lastNodePosition; // Son geçilen node'un pozisyonu
    private bool isMovingVertically = false; // Düşmanın dikey hareket edip etmediğini kontrol etmek için
    private Vector3 lastPosition; // Düşmanın önceki pozisyonu

   void Start()
{
    gridManager = FindObjectOfType<GridManager>();
    player = FindObjectOfType<PlayerController>();
    pathfinding = new Pathfinding(gridManager);

    gridPosition = new Vector2Int(Random.Range(0, gridManager.width), Random.Range(0, gridManager.height));
    transform.position = new Vector3(gridPosition.x, gridPosition.y, 0);

    lastPosition = transform.position; // Başlangıç pozisyonu
    StartCoroutine(FollowPlayer());

    moveSpeed = PlayerPrefs.GetFloat("EnemySpeed", 3f);
}

IEnumerator FollowPlayer()
{
    while (true)
    {
        yield return new WaitForSeconds(0.01f); // Hareket hızı için daha kısa aralık

        // Pathfinding ile hedefe giden yolu bul
        path = pathfinding.FindPath(gridPosition, player.GetGridPosition());

        if (path != null && path.Count > 0)
        {
            // Eğer düşman hala hedefe gitmekteyse
            if (currentNodeIndex < path.Count)
            {
                Node currentNode = path[currentNodeIndex];
                Vector3 targetPosition = new Vector3(currentNode.position.x, currentNode.position.y, 0);

                // Yumuşak hareket için MoveTowards kullanımı
                if (Vector3.Distance(transform.position, targetPosition) > stoppingDistance)
                {
                    transform.position = Vector3.MoveTowards(transform.position, targetPosition,
                        moveSpeed * Time.deltaTime);
                }
                else
                {
                    // Hedefe ulaşıldı, bir sonraki node'a geç
                    lastNodePosition = transform.position; // Son pozisyonu güncelle
                    currentNodeIndex++;
                }

                // Anlık hareket yönünü belirleyip rotayı buna göre ayarla
                Vector3 direction = transform.position - lastPosition; // Hareket yönü

                if (Mathf.Abs(direction.x) > 0 || Mathf.Abs(direction.y) < 0.1f)
                {
                    // Yalnızca X ekseninde hareket varsa, Y ekseninde değişiklik yoksa
                    if (direction.x > 0)
                    {
                        
                    }
                    else if (direction.x < 0)
                    {
                        
                    }
                }
                else if (Mathf.Abs(direction.y) > 0)
                {
                    // Y ekseninde hareket varsa
                    if (direction.y > 0)
                    {
                        
                    }
                    else if (direction.y < 0)
                    {
                        
                    }
                }

                lastPosition = transform.position; // Eski pozisyonu güncelle
            }
            else
            {
                // Hedefe ulaştıysa, en son geldiği yöne göre yönü ayarla
                Vector3 directionFromLastNode = transform.position - new Vector3(lastNodePosition.x, lastNodePosition.y, transform.position.z);

                if (directionFromLastNode.x > 0)
                {
                    // Düşman sağdan gelmişse, sağa dönecek
                    transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y,
                        transform.localScale.z);
                }
                else if (directionFromLastNode.x < 0)
                {
                    // Düşman soldan gelmişse, sola dönecek
                    transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y,
                        transform.localScale.z);
                }

                // Hedefe ulaşan düşmanı yok et
                Destroy(gameObject); // Düşmanı yok et
                gridManager.IncrementEnemyKillCount(); // Öldürülen düşman sayısını arttır
                yield break; // Coroutine'i sonlandır
            }
        }
    }
}
}

