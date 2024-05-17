using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Board _board;
    [SerializeField] private PlayerSymbol _startingPlayer;

    private PlayerSymbol _currentPlayer;
    private PlayerSymbol? _victoryState;

    private Dictionary<Vector2Int, PlayerSymbol> _tileValues = new Dictionary<Vector2Int, PlayerSymbol>();

    private void Awake()
    {
        _board.OnTileClick += TileClick;
        NextTurn();
    }

    private void NextTurn()
    {
        CalculateVictoryState();

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

    private void CalculateVictoryState()
    {
        CalculateVictoryColumn();
        CalculateVictoryLine();

    }

    private void CalculateVictoryLine()
    {
        for (int i = 0; i < _board.GetColumnSize(); i++)
        {
            Vector2Int coordinateFirstElement = new Vector2Int(i, 0);
            Vector2Int coordinateSecondElement = new Vector2Int(i, 1);
            Vector2Int coordinateThirdElement = new Vector2Int(i, 2);

            bool containFirst = _tileValues.ContainsKey(coordinateFirstElement);
            bool containSecond = _tileValues.ContainsKey(coordinateSecondElement);
            bool containThird = _tileValues.ContainsKey(coordinateThirdElement);

            if (!containFirst || !containSecond || !containThird)
                continue;

            PlayerSymbol firstSymbol = _tileValues.GetValueOrDefault(coordinateFirstElement);
            PlayerSymbol secondSymbol = _tileValues.GetValueOrDefault(coordinateSecondElement);
            PlayerSymbol thirdSymbol = _tileValues.GetValueOrDefault(coordinateThirdElement);

            if (firstSymbol == secondSymbol && secondSymbol == thirdSymbol)
            {
                _victoryState = firstSymbol;
                Debug.Log($"O Jogador {_victoryState} ganhou!");
            }
        }
    }

    private void CalculateVictoryColumn()
    {
        for (int i = 0; i < _board.GetColumnSize(); i++)
        {
            Vector2Int coordinateFirstElement = new Vector2Int(0, i);
            Vector2Int coordinateSecondElement = new Vector2Int(1, i);
            Vector2Int coordinateThirdElement = new Vector2Int(2, i);

            bool containFirst = _tileValues.ContainsKey(coordinateFirstElement);
            bool containSecond = _tileValues.ContainsKey(coordinateSecondElement);
            bool containThird = _tileValues.ContainsKey(coordinateThirdElement);

            if (!containFirst || !containSecond || !containThird)
                continue;

            PlayerSymbol firstSymbol = _tileValues.GetValueOrDefault(coordinateFirstElement);
            PlayerSymbol secondSymbol = _tileValues.GetValueOrDefault(coordinateSecondElement);
            PlayerSymbol thirdSymbol = _tileValues.GetValueOrDefault(coordinateThirdElement);

            if (firstSymbol == secondSymbol && secondSymbol == thirdSymbol)
            {
                _victoryState = firstSymbol;
                Debug.Log($"O Jogador {_victoryState} ganhou!");
            }
        }
    }

    private bool IsGameFinished()
    {
        return _victoryState.HasValue;
    }
}
