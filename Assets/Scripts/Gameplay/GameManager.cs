using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.PointerEventData;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Board _board;
    [SerializeField] private PlayerSymbol _startingPlayer;

    private PlayerSymbol _currentPlayer;

    private Dictionary<Vector2Int, PlayerSymbol> _tileValues = new Dictionary<Vector2Int, PlayerSymbol>();

    private void Awake()
    {
        _board.OnTileClick += TileClick;
        NextTurn();
    }

    private void NextTurn()
    {
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
        Vector2Int coordinates = new Vector2Int(args.row, args.column);

        if(_tileValues.TryAdd(coordinates, _currentPlayer))
        {
            _board.DrawOnTile(coordinates, _currentPlayer.ToString());
            NextTurn();
        }
    }
}
