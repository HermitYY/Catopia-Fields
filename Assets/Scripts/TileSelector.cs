using System.Text;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileSelector : MonoBehaviour
{
    [Header("Tilemap")]
    public Tilemap groundTilemap;          // 地形 Tilemap（用于判断鼠标指向格）
    public Tilemap highlightTilemap;       // 高亮 Tilemap
    public Tilemap blockTilemap;           // 如果此 Tilemap 上有 tile，则禁止选中
    public Tile highlightTile;             // 高亮瓦片

    [Header("target")]
    public Tilemap targetTilemap;          // 放置瓦片的 Tilemap
    public TileBase placeTile;             // 放置的瓦片（支持 RuleTile）


    [Header("player")]
    public Transform player;               // 玩家物体
    public float range = 10f;              // 可选范围（支持小数）

    [Header("switch")] 
    public bool showHighlight = true;

    private Vector3Int currentTile;

    void Update()
    {
        UpdateHighlight();
        HandleMouseClick();
    }

    void UpdateHighlight()
    {
        highlightTilemap.ClearAllTiles();

        if (!showHighlight || range <= 0f)
            return;

        Vector3 playerPos = player.position;
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0;

        Vector3 dir = mouseWorldPos - playerPos;

        if (dir.magnitude > range)
            dir = dir.normalized * range;

        Vector3 targetPos = playerPos + dir;
        Vector3Int tile = groundTilemap.WorldToCell(targetPos);

        // 如果 ground 上没 tile，直接不处理
        if (!groundTilemap.HasTile(tile))
            return;

        // 鼠标指到的 tile 不在 blockTilemap 中
        if (blockTilemap == null || !blockTilemap.HasTile(tile))
        {
            currentTile = tile;
            highlightTilemap.SetTile(currentTile, highlightTile);
            return;
        }

        // 鼠标指到了 blockTilemap 启用“倒退一格”射线逻辑
        Vector3Int lastValidTile = Vector3Int.zero;
        bool found = false;

        Vector3 normDir = (dir == Vector3.zero) ? Vector3.right : dir.normalized;
        float totalDist = Mathf.Min(dir.magnitude, range);

        int steps = Mathf.CeilToInt(totalDist * 5f);
        float stepSize = totalDist / steps;

        for (int i = 1; i <= steps; i++)
        {
            float dist = stepSize * i;
            Vector3 checkPos = playerPos + normDir * dist;
            Vector3Int checkCell = groundTilemap.WorldToCell(checkPos);

            if (!groundTilemap.HasTile(checkCell))
                continue;

            if (blockTilemap != null && blockTilemap.HasTile(checkCell))
                break;

            lastValidTile = checkCell;
            found = true;
        }

        if (found)
        {
            currentTile = lastValidTile;
            highlightTilemap.SetTile(currentTile, highlightTile);
        }
    }


    void HandleMouseClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // 不能放置：ground 没 tile 或 blockTilemap 有 tile
            if (!groundTilemap.HasTile(currentTile)) return;
            if (blockTilemap != null && blockTilemap.HasTile(currentTile)) return;

            Debug.Log($"选中坐标：{currentTile}");

            if (targetTilemap != null && placeTile != null)
            {
                targetTilemap.SetTile(currentTile, placeTile);
            }
        }
    }
}
