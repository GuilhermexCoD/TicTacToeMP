using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    //TODO: Temporary
    [SerializeField] private Button _buttonPlayAgain;

    [SerializeField] private Board _board;
    [SerializeField] private PlayerSymbol _startingPlayer;

    private PlayerSymbol _currentPlayer;
    private PlayerSymbol? _victoryState;

    private Dictionary<Vector2Int, PlayerSymbol> _tileValues = new Dictionary<Vector2Int, PlayerSymbol>();

    private void Awake()
    {
        _board.OnTileClick += TileClick;
        Play();
    }

    private void NextTurn()
    {
        ValidateVictoryState();

        if (IsGameFinished())
            return;

        switch (_currentPlayer)
        {
            case PlayerSymbol.None:
                _currentPlayer = _startingPlayer;
                break;
            case PlayerSymbol.X:
                _currentPlayer = PlayerSymbol.O;
                break;
            case PlayerSymbol.O:
                _currentPlayer = PlayerSymbol.X;
                break;
            default:
                break;
        }
    }

    private void TileClick(object sender, Tile.TileEventArgs args)
    {
        if (IsGameFinished())
            return;

        Vector2Int coordinates = new Vector2Int(args.row, args.column);

        if (_tileValues.TryAdd(coordinates, _currentPlayer))
        {
            _board.DrawOnTile(coordinates, _currentPlayer.ToString());
            NextTurn();
        }
    }

    #region Validate Victory Methods

    private void ValidateVictoryState()
    {
        ValidateVictoryLineColumn();
        ValidateVictoryPrimaryDiagonal();
        ValidateVictorySecondaryDiagonal();

        ValidateDraw();
    }

    private void ValidateVictoryLineColumn()
    {
        for (int i = 0; i < _board.GetColumnSize(); i++)
        {
            //Check victory for line
            Vector2Int lineFirstElement = new Vector2Int(i, 0);
            Vector2Int lineSecondElement = new Vector2Int(i, 1);
            Vector2Int lineThirdElement = new Vector2Int(i, 2);

            ValidateVictoryCondition(lineFirstElement, lineSecondElement, lineThirdElement);

            //Check victory for Column
            Vector2Int columnFirstElement = new Vector2Int(0, i);
            Vector2Int columnSecondElement = new Vector2Int(1, i);
            Vector2Int columnThirdElement = new Vector2Int(2, i);

            ValidateVictoryCondition(columnFirstElement, columnSecondElement, columnThirdElement);
        }
    }

    private void ValidateVictoryPrimaryDiagonal()
    {
        Vector2Int coordinateFirstElement = new Vector2Int(0, 0);
        Vector2Int coordinateSecondElement = new Vector2Int(1, 1);
        Vector2Int coordinateThirdElement = new Vector2Int(2, 2);

        ValidateVictoryCondition(coordinateFirstElement, coordinateSecondElement, coordinateThirdElement);
    }

    private void ValidateVictorySecondaryDiagonal()
    {
        Vector2Int coordinateFirstElement = new Vector2Int(2, 0);
        Vector2Int coordinateSecondElement = new Vector2Int(1, 1);
        Vector2Int coordinateThirdElement = new Vector2Int(0, 2);

        ValidateVictoryCondition(coordinateFirstElement, coordinateSecondElement, coordinateThirdElement);
    }

    private void ValidateVictoryCondition(Vector2Int coordinateFirstElement, Vector2Int coordinateSecondElement, Vector2Int coordinateThirdElement)
    {
        bool containFirst = _tileValues.ContainsKey(coordinateFirstElement);
        bool containSecond = _tileValues.ContainsKey(coordinateSecondElement);
        bool containThird = _tileValues.ContainsKey(coordinateThirdElement);

        if (!containFirst || !containSecond || !containThird)
            return;

        PlayerSymbol firstSymbol = _tileValues.GetValueOrDefault(coordinateFirstElement);
        PlayerSymbol secondSymbol = _tileValues.GetValueOrDefault(coordinateSecondElement);
        PlayerSymbol thirdSymbol = _tileValues.GetValueOrDefault(coordinateThirdElement);

        if (firstSymbol == secondSymbol && secondSymbol == thirdSymbol)
        {
            _victoryState = firstSymbol;
            Debug.Log($"O Jogador {_victoryState} ganhou!");
            _buttonPlayAgain.interactable = true;
        }
    }

    private void ValidateDraw()
    {
        if (_victoryState.HasValue)
            return;

        if (_tileValues.Count == _board.GetSize())
        {
            _victoryState = PlayerSymbol.None;
            Debug.Log($"Deu velha!");
            _buttonPlayAgain.interactable = true;
        }
    }

    #endregion

    private bool IsGameFinished()
    {
        return _victoryState.HasValue;
    }

    public void Play()
    {
        _buttonPlayAgain.interactable = false;

        ClearMatch();
        NextTurn();

        SwitchNextStartingPlayer();
    }

    private void SwitchNextStartingPlayer()
    {
        switch (_startingPlayer)
        {
            case PlayerSymbol.X:
                _startingPlayer = PlayerSymbol.O;
                break;
            case PlayerSymbol.O:
                _startingPlayer = PlayerSymbol.X;
                break;
            default:
                break;
        }
    }

    private void ClearMatch()
    {
        _currentPlayer = PlayerSymbol.None;
        _victoryState = null;
        _tileValues.Clear();
        _board.Clear();
    }
}
