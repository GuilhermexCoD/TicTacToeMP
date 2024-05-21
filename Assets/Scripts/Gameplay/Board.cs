using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using static UnityEngine.EventSystems.PointerEventData;

public class Board : MonoBehaviour
{
    private const int MAX_COLUMNS = 3;
    private const int MAX_LINES = 3;

    public event EventHandler<Tile.TileEventArgs> OnTileClick;

    [SerializeField] private Tile[] _tiles;

#if  UNITY_EDITOR
    [ContextMenu("UpdateColumnRowIndex")]
    private void UpdateColumnRowIndex()
    {
        int childCount = transform.childCount;

        int row = 0;
        int column = 0;

        for (int i = 0; i < childCount; i++)
        {
            if (column >= 3)
            {
                column = 0;
                row++;
            }

            GameObject child = transform.GetChild(i).gameObject;

            child.name = $"Tile_{row}_{column}";

            Debug.Log($"Child Name: {child.name}");

            Tile tile = child.GetComponent<Tile>();

            if (tile != null)
            {
                tile.SetColumn(column);
                tile.SetRow(row);
                EditorUtility.SetDirty(tile);
            }

            column++;
        }

        EditorUtility.SetDirty(this);
    }

    [ContextMenu("PopulateTiles")]
    private void PopulateTiles()
    {
        int childCount = transform.childCount;

        _tiles = new Tile[childCount];

        for (int i = 0; i < childCount; i++)
        {
            _tiles[i] = transform.GetChild(i).GetComponent<Tile>();
        }

        EditorUtility.SetDirty(this);
    }

#endif

    private void Awake()
    {
        SubscribeOnTileClick();
    }

    private void SubscribeOnTileClick()
    {
        for (int i = 0; i < _tiles.Length; i++)
        {
            Tile tile = _tiles[i];

            tile.OnTileClick += TileClick;
        }
    }

    private void UnSubscribeFromTileClick()
    {
        for (int i = 0; i < _tiles.Length; i++)
        {
            Tile tile = _tiles[i];

            tile.OnTileClick -= TileClick;
        }
    }

    private void TileClick(object sender, Tile.TileEventArgs args)
    {
        OnTileClick?.Invoke(this, args);
    }

    private int GetTileIndex(int row, int column)
    {
        // (0,0) = 0 = (row * 3) + column
        // (0,1) = 1 = (0 * 3) + 1
        // (0,2) = 2 = (0 * 3) + 2
        // (1,0) = 3 = (1 * 3) + 0
        // (1,1) = 4 = (1 * 3) + 1
        // (1,2) = 5 = (1 * 3) + 2
        // (2,0) = 6
        // (2,1) = 7
        // (2,2) = 8
        return (row * MAX_COLUMNS) + column;
    }

    internal void DrawOnTile(Vector2Int coordinates, string tileText)
    {
        int index = GetTileIndex(coordinates.x, coordinates.y);

        Tile tile = _tiles[index];

        tile.SetOptionText(tileText);
    }

    public int GetLineSize()
    {
        return MAX_LINES;
    }

    public int GetColumnSize()
    {
        return MAX_COLUMNS;
    }

    public int GetSize()
    {
        return MAX_LINES * MAX_COLUMNS;
    }

    public void Clear()
    {
        for (int i = 0; i < _tiles.Length; i++)
        {
            Tile tile = _tiles[i];
            tile.ClearOption();
        }
    }

    private void OnDestroy()
    {
        UnSubscribeFromTileClick();
    }
}
