using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using static UnityEngine.EventSystems.PointerEventData;

public class Board : MonoBehaviour
{
    [SerializeField] private Tile[] _tiles;

    private Dictionary<Vector2Int, PlayerSymbol> _tileValues = new Dictionary<Vector2Int, PlayerSymbol>();

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

    [ContextMenu("PrintTileDictionary")]
    private void PrintTileDictionary()
    {
        for (int i = 0; i < _tileValues.Count; i++)
        {
            var keyValuePair = _tileValues.ElementAt(i);
            Debug.Log($"KEY: {keyValuePair.Key} | VALUE: {keyValuePair.Value}");
        }
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

            tile.OnTileClick += OnTileClick;
        }
    }

    private void UnSubscribeFromTileClick()
    {
        for (int i = 0; i < _tiles.Length; i++)
        {
            Tile tile = _tiles[i];

            tile.OnTileClick -= OnTileClick;
        }
    }

    private void OnTileClick(object sender, Tile.TileEventArgs args)
    {
        Tile tile = (Tile)sender;

        Vector2Int coordinates = new Vector2Int(args.row, args.column);

        if (args.pressedInput == InputButton.Left)
        {
            if (_tileValues.TryAdd(coordinates, PlayerSymbol.X))
            {
                tile.SetOptionText("X");
            }
            else
            {
                Debug.Log("This Tile is taken");
            }
        }
        else if (args.pressedInput == InputButton.Right)
        {
            if (_tileValues.TryAdd(coordinates, PlayerSymbol.O))
            {
                tile.SetOptionText("O");
            }
            else
            {
                Debug.Log("This Tile is taken");
            }
        }
    }

    private void OnDestroy()
    {
        UnSubscribeFromTileClick();
    }
}
