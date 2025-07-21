using System;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public static GameManager instance { get; private set; }
    public event EventHandler<OnClickedOnGridPositionEventArgs>OnClickedOnGridPosition;

    public enum PlayerType
    {
        None,
        Cross,
        Circle,
    }

    private PlayerType LocalPlayerType;

    public class OnClickedOnGridPositionEventArgs : EventArgs
    {
        public int Classx,Classy;
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
    }

    public void ClickOnGridPosition(int x,int y) {
        Debug.Log(x+ ", " + y);
        OnClickedOnGridPosition?.Invoke(this, new OnClickedOnGridPositionEventArgs{
            Classx = x,
            Classy = y,
        });
    }

    public PlayerType GetLocalPlayerType()
    {
        return LocalPlayerType;
    }
}
