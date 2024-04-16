using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour
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
}
