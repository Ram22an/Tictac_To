using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public static GameManager instance { get; private set; }
    public event EventHandler<OnClickedOnGridPositionEventArgs>OnClickedOnGridPosition;
    public event EventHandler OnGameStarted;
    public event EventHandler<OnGameWinEventArgs> OnGameWin;
    public class OnGameWinEventArgs:EventArgs
    {
        public Line Myline;
    }
    public event EventHandler OnCurrentPlayablePlayerTypeChanged;
    public enum PlayerType
    {
        None,
        Cross,
        Circle,
    }
    public enum Orientation
    {
        Horizontal, 
        Vertical,
        DiagonalA,
        DiagonalB,
    }
    public struct Line
    {
        public List<Vector2Int> GridVector2IntList;
        public Vector2Int CenterGridPosition;
        public Orientation orientation;
    }
    private PlayerType LocalPlayerType;
    private NetworkVariable<PlayerType> CurrentPlayablePlayerType=new NetworkVariable<PlayerType>();
    private PlayerType[,] PlayerTypeArray;
    public class OnClickedOnGridPositionEventArgs : EventArgs
    {
        public int Classx,Classy;
        public PlayerType Classplayertype;
    }
    private List<Line> LineList;
    private void Awake()
    {
        if (instance != null) {
            Debug.Log("Error in instance of game manager");
        }
        instance = this;
        PlayerTypeArray=new PlayerType[3,3];
        LineList = new List<Line>
        {
            // Horizontal
            new Line
            {
                GridVector2IntList=new List<Vector2Int>{new Vector2Int(0,0),new Vector2Int(1,0),new Vector2Int(2,0)},
                CenterGridPosition=new Vector2Int(1,0),
                orientation=Orientation.Horizontal,
            },
            new Line
            {
                GridVector2IntList=new List<Vector2Int>{new Vector2Int(0,1),new Vector2Int(1,1),new Vector2Int(2,1)},
                CenterGridPosition=new Vector2Int(1,1),
                orientation=Orientation.Horizontal,
            },
            new Line
            {
                GridVector2IntList=new List<Vector2Int>{new Vector2Int(0,2),new Vector2Int(1,2),new Vector2Int(2,2)},
                CenterGridPosition=new Vector2Int(1,2),
                orientation=Orientation.Horizontal,
            },

            // Vertical
            new Line
            {
                GridVector2IntList=new List<Vector2Int>{new Vector2Int(0,0),new Vector2Int(0,1),new Vector2Int(0,2)},
                CenterGridPosition=new Vector2Int(0,1),
                orientation=Orientation.Vertical,
            },
            new Line
            {
                GridVector2IntList=new List<Vector2Int>{new Vector2Int(1,0),new Vector2Int(1,1),new Vector2Int(1,2)},
                CenterGridPosition=new Vector2Int(1,1),
                orientation=Orientation.Vertical,
            },
            new Line
            {
                GridVector2IntList=new List<Vector2Int>{new Vector2Int(2,0),new Vector2Int(2,1),new Vector2Int(2,2)},
                CenterGridPosition=new Vector2Int(2,1),
                orientation=Orientation.Vertical,
            },

            // Diagonal
            new Line
            {
                GridVector2IntList=new List<Vector2Int>{new Vector2Int(0,0),new Vector2Int(1,1),new Vector2Int(2,2)},
                CenterGridPosition=new Vector2Int(1,1),
                orientation = Orientation.DiagonalA,
            },
            new Line
            {
                GridVector2IntList=new List<Vector2Int>{new Vector2Int(0,2),new Vector2Int(1,1),new Vector2Int(2,0)},
                CenterGridPosition=new Vector2Int(1,1),
                orientation=Orientation.DiagonalB,
            }
        };
    }

    public override void OnNetworkSpawn()
    {
        Debug.Log(NetworkManager.Singleton.LocalClientId);
        if (NetworkManager.Singleton.LocalClientId==0)
        {
            LocalPlayerType = PlayerType.Cross;
        }
        else
        {
            LocalPlayerType = PlayerType.Circle;
        }
        if (IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_OnClientConnectedCallback;
        }
        CurrentPlayablePlayerType.OnValueChanged += (PlayerType oldPlayerType, PlayerType newPlayerType) =>
        {
            OnCurrentPlayablePlayerTypeChanged?.Invoke(this, EventArgs.Empty);
        };

    }

    public void NetworkManager_OnClientConnectedCallback(ulong obj)
    {
        if (NetworkManager.Singleton.ConnectedClientsList.Count == 2)
        {
            CurrentPlayablePlayerType.Value = PlayerType.Cross;
            TriggerOnGameStartedRpc();
        }
    }
    [Rpc(SendTo.ClientsAndHost)]
    public void TriggerOnGameStartedRpc()
    {
        OnGameStarted?.Invoke(this, EventArgs.Empty);
    }

    [Rpc(SendTo.Server)]
    public void ClickOnGridPositionRpc(int x,int y,PlayerType playertype) {
        Debug.Log(x+ ", " + y);
        if (playertype != CurrentPlayablePlayerType.Value)
        {
            return;
        }
        if (PlayerTypeArray[x,y]!=PlayerType.None)
        {
            return;
        }
        PlayerTypeArray[x, y] = playertype;
        OnClickedOnGridPosition?.Invoke(this, new OnClickedOnGridPositionEventArgs{
            Classx = x,
            Classy = y,
            Classplayertype = playertype,
        });
        switch (CurrentPlayablePlayerType.Value) {
            default:
            case PlayerType.Cross:
                CurrentPlayablePlayerType.Value = PlayerType.Circle;
                break;
            case PlayerType.Circle:
                CurrentPlayablePlayerType.Value =PlayerType.Cross;
                break;

        }
        TestWinner();
    }
    private bool TestWinnerLine(Line line)
    {
        return TestWinnerLine(
            PlayerTypeArray[line.GridVector2IntList[0].x, line.GridVector2IntList[0].y],
            PlayerTypeArray[line.GridVector2IntList[1].x, line.GridVector2IntList[1].y],
            PlayerTypeArray[line.GridVector2IntList[2].x, line.GridVector2IntList[2].y]);
    }
    private bool TestWinnerLine(PlayerType PlayerTypeA, PlayerType PlayerTypeB, PlayerType PlayerTypeC)
    {
        return PlayerTypeA!=PlayerType.None && PlayerTypeA==PlayerTypeB && PlayerTypeB==PlayerTypeC;
    }
    private void TestWinner()
    {
        foreach(Line line in LineList)
        {
            if (TestWinnerLine(line))
            {
                OnGameWin?.Invoke(this, new OnGameWinEventArgs
                {
                    Myline = line
                });
                CurrentPlayablePlayerType.Value = PlayerType.None;
                break;
            }

        }
    }
    public PlayerType GetLocalPlayerType()
    {
        return LocalPlayerType;
    }
    public PlayerType GetCurrentPlayerType()
    {
        return CurrentPlayablePlayerType.Value;
    }
}
