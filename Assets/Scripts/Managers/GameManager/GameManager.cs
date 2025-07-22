using System;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public static GameManager instance { get; private set; }
    public event EventHandler<OnClickedOnGridPositionEventArgs>OnClickedOnGridPosition;
    public event EventHandler OnGameStarted;
    public event EventHandler OnCurrentPlayablePlayerTypeChanged;
    public enum PlayerType
    {
        None,
        Cross,
        Circle,
    }

    private PlayerType LocalPlayerType;
    private NetworkVariable<PlayerType> CurrentPlayablePlayerType=new NetworkVariable<PlayerType>();

    public class OnClickedOnGridPositionEventArgs : EventArgs
    {
        public int Classx,Classy;
        public PlayerType Classplayertype;
    }

    private void Awake()
    {
        if (instance != null) {
            Debug.Log("Error in instance of game manager");
        }
        instance = this;
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
