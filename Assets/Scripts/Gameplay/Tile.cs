using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.EventSystems.PointerEventData;

public class Tile : MonoBehaviour, IPointerClickHandler
{
    public event EventHandler<TileEventArgs> OnTileClick;
    public class TileEventArgs : EventArgs
    {
        public int row;
        public int column;
        public string text;
    }

    [SerializeField] private int _row;
    [SerializeField] private int _column;
    [SerializeField] private Image _backgroundImage;
    [SerializeField] private TextMeshProUGUI _optionText;

    public void SetOptionText(string option)
    {
        _optionText.text = option;
    }

    public void ClearOption()
    {
        _optionText.text = "";
    }

    public void SetColumn(int column)
    {
        _column = column;
    }

    public void SetRow(int row)
    {
        _row = row;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnTileClick?.Invoke(this, new TileEventArgs()
        {
            text = _optionText.text,
            row = _row,
            column = _column,
        });
    }
}
