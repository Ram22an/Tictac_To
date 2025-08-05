using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GamOverUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI ResultTextMesh;
    [SerializeField] private Color winColor;
    [SerializeField] private Color loseColor;
    [SerializeField] private Color TieColor;
    [SerializeField] private Button RematchButton;
    private void Awake()
    {
        RematchButton.onClick.AddListener(() =>
        {
            GameManager.instance.RematchRpc();
        });
    }
    private void Start()
    {
        GameManager.instance.OnGameWin += GameManager_OnGameWin;
        GameManager.instance.OnRematch += GameManager_OnRematch;
        GameManager.instance.OnGameTied += GameManager_OnGameTied;
        Hide();
    }

    private void GameManager_OnGameTied(object sender, System.EventArgs e)
    {
        Show();
        ResultTextMesh.text = "TIE !!!";
        ResultTextMesh.color = TieColor;
    }

    private void GameManager_OnRematch(object sender,System.EventArgs e)
    {
        Hide();
    }
    private void GameManager_OnGameWin(object sender, GameManager.OnGameWinEventArgs e)
    {
        if (e.WinPlayerType == GameManager.instance.GetLocalPlayerType())
        {
            ResultTextMesh.text = "YOU WIN !!!";
            ResultTextMesh.color = winColor;
        }
        else
        {
            ResultTextMesh.text = "YOU LOSE !!!";
            ResultTextMesh.color = loseColor;
        }
        Show();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
    public void Show()
    {
        gameObject.SetActive(true);
    }


}
