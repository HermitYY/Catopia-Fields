using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class UnlimitedGridRecycler_Fixed : MonoBehaviour
{
    [Header("References")]
    public ScrollRect scrollRect;
    public RectTransform viewport;
    public GameObject itemPrefab;
    public GridLayoutGroup grid;

    [Header("Settings")]
    public int totalItemCount = 300;
    public int bufferRows = 2;

    // 外部回调： (itemGameObject, dataIndex)
    public Action<GameObject, int> onUpdateItem;

    RectTransform content;
    List<RectTransform> pool = new List<RectTransform>();

    int columns;
    int totalRows;
    int visibleRows;
    int poolRows;
    int poolCount;

    float cellW, cellH, spaceX, spaceY;
    int firstRow = 0;

    void Awake()
    {
        content = GetComponent<RectTransform>();
        if (grid == null) grid = GetComponent<GridLayoutGroup>();
    }

    void Start()
    {
        Init(totalItemCount);
        if (scrollRect != null)
            scrollRect.onValueChanged.AddListener((v) => OnScroll());
    }

    public void Init(int total)
    {
        totalItemCount = Mathf.Max(0, total);

        if (grid == null)
        {
            Debug.LogError("UnlimitedGridRecycler_Fixed: 必须在 Content 上挂 GridLayoutGroup 或在 Inspector 绑定它。");
            return;
        }

        // 读取 cell/spacing
        cellW = grid.cellSize.x;
        cellH = grid.cellSize.y;
        spaceX = grid.spacing.x;
        spaceY = grid.spacing.y;

        // 必须用 FixedColumnCount（更稳定）
        if (grid.constraint != GridLayoutGroup.Constraint.FixedColumnCount)
        {
            Debug.LogWarning("GridLayoutGroup 推荐使用 FixedColumnCount（脚本当前假定列数固定）。脚本将尝试从宽度估算列数。");
            // 估算列数（当不是 FixedColumnCount 时）
            float contentWidth = content.rect.width - grid.padding.left - grid.padding.right;
            columns = Mathf.Max(1, Mathf.FloorToInt((contentWidth + spaceX) / (cellW + spaceX)));
        }
        else
        {
            columns = Mathf.Max(1, grid.constraintCount);
        }

        // 计算行数（严格）
        totalRows = Mathf.CeilToInt(totalItemCount / (float)columns);

        // 计算 content 高度： padding.top + totalRows*cellH + (totalRows-1)*spaceY + padding.bottom
        float totalSpacingY = Mathf.Max(0, (totalRows - 1)) * spaceY;
        float contentHeight = grid.padding.top + totalRows * cellH + totalSpacingY + grid.padding.bottom;

        // 确保 content 高度至少等于 viewport 高度（避免 ScrollRect 出现奇怪行为）
        float minH = viewport != null ? viewport.rect.height : 0f;
        contentHeight = Mathf.Max(contentHeight, minH);

        // 应用 content 大小（保持 x 不变）
        Vector2 size = content.sizeDelta;
        size.y = contentHeight;
        content.sizeDelta = size;

        // 计算可见行数（至少 1）
        float rowFullHeight = cellH + spaceY;
        visibleRows = Mathf.Max(1, Mathf.CeilToInt((viewport != null ? viewport.rect.height : 0f) / rowFullHeight));

        // poolRows 不应大于 totalRows
        poolRows = Mathf.Min(totalRows, visibleRows + bufferRows);

        // poolCount 最小为 columns（至少一行）
        poolCount = Mathf.Max(columns, poolRows * columns);

        // 清空旧的 pool（保守做法）
        foreach (var r in pool) if (r != null) Destroy(r.gameObject);
        pool.Clear();

        // 生成 pool（固定大小）
        for (int i = 0; i < poolCount; i++)
        {
            var go = Instantiate(itemPrefab, content, false);
            var rt = go.GetComponent<RectTransform>();
            if (rt == null) rt = go.AddComponent<RectTransform>();
            pool.Add(rt);
        }

        // 初始化位置与数据
        firstRow = 0;
        UpdatePool();
    }

    void OnScroll()
    {
        if (totalRows <= visibleRows)
        {
            // 不需要滚动
            if (firstRow != 0)
            {
                firstRow = 0;
                UpdatePool();
            }
            return;
        }

        float rowHeight = cellH + spaceY;

        // 计算 content 在 viewport 中的“滚动顶部”相对于 content 顶部的偏移（以像素计）
        // content.anchoredPosition.y 表示 content 向上移动的像素（top offset）
        float contentTop = content.anchoredPosition.y - 0f; // padding handled below

        // 将 contentTop 与 padding.top 做相对
        float relativeTop = contentTop - grid.padding.top;
        if (relativeTop < 0) relativeTop = 0;

        int newFirstRow = Mathf.FloorToInt(relativeTop / rowHeight);
        newFirstRow = Mathf.Clamp(newFirstRow, 0, Mathf.Max(0, totalRows - visibleRows));

        if (newFirstRow != firstRow)
        {
            firstRow = newFirstRow;
            UpdatePool();
        }
    }

    void UpdatePool()
    {
        float rowHeight = cellH + spaceY;
        float colWidth = cellW + spaceX;

        // pool 按行主序排列：index -> rowInPool = index / columns, col = index % columns
        for (int i = 0; i < pool.Count; i++)
        {
            var rt = pool[i];
            int localRow = i / columns;   // 行相对于 pool 的行索引（0..poolRows-1）
            int col = i % columns;

            int realRow = firstRow + localRow;
            int dataIndex = realRow * columns + col;

            // 如果 realRow 超出总行数或 dataIndex 超出总数量，则隐藏该格（这种情况只会发生在 totalRows < visibleRows 或最后一行不足列）
            if (realRow < 0 || realRow >= totalRows || dataIndex < 0 || dataIndex >= totalItemCount)
            {
                //if (rt.gameObject.activeSelf) rt.gameObject.SetActive(false);
                continue;
            }

            // 显示并放置到正确的位置
            if (!rt.gameObject.activeSelf) rt.gameObject.SetActive(true);

            // 计算 anchoredPosition（以 content 的左上为原点考虑 padding）
            // x = padding.left + col * colWidth + pivot.x * cellW
            // y = -(padding.top + realRow * rowHeight) - (1 - pivot.y) * cellH
            float x = grid.padding.left + col * colWidth + rt.pivot.x * cellW;
            float y = -(grid.padding.top + realRow * rowHeight) - (1 - rt.pivot.y) * cellH;

            rt.anchoredPosition = new Vector2(x, y);

            // 回调填充数据
            try
            {
                onUpdateItem?.Invoke(rt.gameObject, dataIndex);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
    }

    // 外部调用：刷新某一索引（数据改变）
    public void RefreshIndex(int index)
    {
        if (index < 0 || index >= totalItemCount) return;

        int row = index / columns;
        int col = index % columns;
        int localRow = row - firstRow;

        if (localRow < 0 || localRow >= poolRows) return;

        int poolIndex = localRow * columns + col;
        if (poolIndex < 0 || poolIndex >= pool.Count) return;

        onUpdateItem?.Invoke(pool[poolIndex].gameObject, index);
    }

    public void SetTotalCount(int newTotal)
    {
        Init(newTotal);
    }

    void OnDestroy()
    {
        if (scrollRect != null) scrollRect.onValueChanged.RemoveAllListeners();
    }
}
