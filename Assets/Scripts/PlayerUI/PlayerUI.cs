using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private GameObject CrossArrowGameObject;
    [SerializeField] private GameObject CircleArrowGameObject;
    [SerializeField] private GameObject CrossYouTextGameObject;
    [SerializeField] private GameObject CircleYouTextGameObject;

    private void Awake()
    {
        CrossArrowGameObject.SetActive(false);
        CircleArrowGameObject.SetActive(false);
        CircleYouTextGameObject.SetActive(false);
        CrossYouTextGameObject.SetActive(false);
    }
    private void Start()
    {
        GameManager.instance.OnGameStarted += GameManager_OnGameStarted;
        GameManager.instance.OnCurrentPlayablePlayerTypeChanged += GameManger_OnCurrentPlayablePlayerTypeChanged;
    }
    private void GameManger_OnCurrentPlayablePlayerTypeChanged(object sender,System.EventArgs e)
    {
        UpdateTheCurrentArrow();
    }

    private void GameManager_OnGameStarted(object sender, System.EventArgs e)
    {
        if (GameManager.instance.GetLocalPlayerType() == GameManager.PlayerType.Cross)
        {
            CrossYouTextGameObject.SetActive(true);
        }
        else
        {
            CircleYouTextGameObject.SetActive(true);
        }
        UpdateTheCurrentArrow();
    }
    private void UpdateTheCurrentArrow()
    {
        if (GameManager.instance.GetCurrentPlayerType() == GameManager.PlayerType.Cross) { 
            CrossArrowGameObject.SetActive(true);
            CircleArrowGameObject.SetActive(false);
        }
        else
        {
            CrossArrowGameObject.SetActive(false);
            CircleArrowGameObject.SetActive(true);
        }
    }
}
