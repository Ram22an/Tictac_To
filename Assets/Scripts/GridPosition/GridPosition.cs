using UnityEngine;

public class GridPosition : MonoBehaviour
{
    private int x, y;
    private void Start()
    {
        string name = gameObject.name;
        string[] parts = name.Split('_');
        x = int.Parse(parts[0]);
        y = int.Parse(parts[1]);
    }
    public void OnMouseDown()
    {
        GameManager.instance.ClickOnGridPosition(x, y);
    }
}
