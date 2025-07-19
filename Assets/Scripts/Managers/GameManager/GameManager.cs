using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void ClickOnGridPosition(int x,int y) {
        Debug.Log(x+ ", " + y);
    }
}
