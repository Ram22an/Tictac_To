using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetWorkManagerUI : MonoBehaviour
{
    [SerializeField] private Button StartHostButton;
    [SerializeField] private Button StartClientButton;

    private void Awake()
    {
        StartHostButton.onClick.AddListener (() =>
        {
            NetworkManager.Singleton.StartHost();
            HideUI();
        });
        StartClientButton.onClick.AddListener (() =>
        { 
            NetworkManager.Singleton.StartClient();
            HideUI();
        });
    }
    private void HideUI()
    {
        gameObject.SetActive (false);
    }
}
