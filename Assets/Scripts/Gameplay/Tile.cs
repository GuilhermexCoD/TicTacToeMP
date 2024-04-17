using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Tile : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private int _row;
    [SerializeField] private int _column;
    [SerializeField] private Image _backgroundImage;
    [SerializeField] private TextMeshProUGUI _optionText;

    public void SetOptionText(string option)
    {
        _optionText.text = option;
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
        Debug.Log($"Click {_row} {_column}");
    }
}
