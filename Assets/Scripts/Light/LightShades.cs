using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LightShades : MonoBehaviour
{
    private ColorControl cc;
    private List<GameObject> objectsCollided = new List<GameObject>();
    private List<Tilemap> obstacleTilemaps = new List<Tilemap>();

    private SpriteRenderer sr;

    [SerializeField] private float pulseDuration = 2f;       // Full cycle duration in seconds
    [SerializeField] private float minAlpha = 0.4f;          // Min transparency
    [SerializeField] private float maxAlpha = 1.0f;          // Max opacity

    void Start()
    {
        cc = FindObjectOfType<ColorControl>();
        sr = GetComponent<SpriteRenderer>();

        // Find all Tilemaps with the "Obstacle" tag
        GameObject[] taggedObjects = GameObject.FindGameObjectsWithTag("Obstacle");
        foreach (GameObject obj in taggedObjects)
        {
            if (obj.TryGetComponent<Tilemap>(out var tilemap))
            {
                obstacleTilemaps.Add(tilemap);
            }
        }
    }

    void Update()
    {
        // Breathing effect by modifying alpha only
        float t = (Mathf.Sin(Time.time * (2 * Mathf.PI / pulseDuration)) + 1f) / 2f;
        float alpha = Mathf.Lerp(minAlpha, maxAlpha, t);

        Color baseColor = sr.color;
        sr.color = new Color(baseColor.r, baseColor.g, baseColor.b, alpha);  // Only update alpha

        Color lightColor = new Color(baseColor.r, baseColor.g, baseColor.b, 1f); // Use solid color for logic

        // Step 1: Remove tiles in matching tilemaps
        foreach (var tilemap in obstacleTilemaps)
        {
            ClearTilesInTilemapIfMatches(tilemap, lightColor);
        }

        // Step 2: Handle GameObjects
        for (int i = 0; i < objectsCollided.Count; i++)
        {
            GameObject obj = objectsCollided[i];
            if (obj.TryGetComponent<SpriteRenderer>(out var srObj))
            {
                Color objectColor = srObj.color;

                if (cc.CompareColors(objectColor, lightColor))
                {
                    if (obj.CompareTag("Enemy"))
                    {
                        obj.SetActive(false);

                        int enemyKilledCount = PlayerPrefs.GetInt("EnemyKilledCount", 0);
                        PlayerPrefs.SetInt("EnemyKilledCount", enemyKilledCount + 1);
                        PlayerPrefs.Save();

                        if (CompareTag("LightShades"))
                        {
                            int enemyTrappedCount = PlayerPrefs.GetInt("EnemyTrappedCount", 0);
                            if (enemyTrappedCount > 0)
                            {
                                PlayerPrefs.SetInt("EnemyTrappedCount", enemyTrappedCount - 1);
                                PlayerPrefs.Save();
                            }
                        }
                    }
                    else if (obj.CompareTag("Obstacle"))
                    {
                        obj.SetActive(false);
                    }
                }
            }
        }
    }

    private void ClearTilesInTilemapIfMatches(Tilemap tilemap, Color lightColor)
    {
        if (tilemap == null) return;
        if (!cc.CompareColors(tilemap.color, lightColor)) return;

        Collider2D lightCollider = GetComponent<Collider2D>();
        if (lightCollider == null) return;

        Bounds bounds = lightCollider.bounds;
        Vector3Int min = tilemap.WorldToCell(bounds.min);
        Vector3Int max = tilemap.WorldToCell(bounds.max);

        for (int x = min.x; x <= max.x; x++)
        {
            for (int y = min.y; y <= max.y; y++)
            {
                Vector3Int cellPos = new Vector3Int(x, y, 0);
                if (tilemap.HasTile(cellPos))
                {
                    tilemap.SetTile(cellPos, null);
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!objectsCollided.Contains(collision.gameObject))
        {
            objectsCollided.Add(collision.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (objectsCollided.Contains(collision.gameObject))
        {
            objectsCollided.Remove(collision.gameObject);
        }
    }
}
