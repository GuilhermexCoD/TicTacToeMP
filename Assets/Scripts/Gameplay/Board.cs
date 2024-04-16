using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Board : MonoBehaviour
{
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
            }

            column++;
        }

        EditorUtility.SetDirty(this);
    }
#endif

}
