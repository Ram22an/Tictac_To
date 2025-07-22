using Unity.Netcode;
using UnityEngine;

public class GameVisualManager : NetworkBehaviour
{
    private const float GridSize = 2;
    [SerializeField] Transform CrossPrefab, CirclePrefab;
    [SerializeField] Transform LineCompletePrefab;
    private void Start()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.OnClickedOnGridPosition += GameManager_OnClickedOnGridPosition;
            GameManager.instance.OnGameWin += GameManager_OnGameWin;
        }
        else
        {
            Debug.LogError("GameManager.instance is null in GameVisualManager. Make sure GameManager is in the scene and initializes before this script.");
        }

    }
    private void GameManager_OnGameWin(object sender,GameManager.OnGameWinEventArgs e)
    {
        float eulerZ = 0f;
        switch (e.Myline.orientation)
        {
            default:
            case GameManager.Orientation.Horizontal:
                eulerZ =0f;
                break;
            case GameManager.Orientation.Vertical:
                eulerZ = 90f;
                break;
            case GameManager.Orientation.DiagonalA:
                eulerZ = 45f;
                break;
            case GameManager.Orientation.DiagonalB:
                eulerZ = -45f;
                break;
        }
        Transform LineTransform = Instantiate(LineCompletePrefab, 
            GetGridWorldPosition(e.Myline.CenterGridPosition.x,e.Myline.CenterGridPosition.y), 
            Quaternion.Euler(0,0,eulerZ));
        LineTransform.GetComponent<NetworkObject>().Spawn(true);
    }

    public void GameManager_OnClickedOnGridPosition(object sender,GameManager.OnClickedOnGridPositionEventArgs e)
    {
        Debug.Log("GameManager_OnClickedOnGridPosition");
        SpawnObjectRpc(e.Classx, e.Classy,e.Classplayertype);
    }
    [Rpc(SendTo.Server)]
    public void SpawnObjectRpc(int x,int y,GameManager.PlayerType playertype)
    {
        Debug.Log("Spawn Object");
        Transform Prefab; 
        switch (playertype)
        {
            default:
            case GameManager.PlayerType.Cross:
                Prefab = CrossPrefab;
                break;
            case GameManager.PlayerType.Circle:
                Prefab = CirclePrefab;
                break;
        }
        Transform SpawnedObject = Instantiate(Prefab, GetGridWorldPosition(x,y), Quaternion.identity);
        SpawnedObject.GetComponent<NetworkObject>().Spawn(true);
    }
    public Vector2 GetGridWorldPosition(int x,int y)
    {
        return new Vector2(-GridSize+x*GridSize,-GridSize+y*GridSize);
    }
}
