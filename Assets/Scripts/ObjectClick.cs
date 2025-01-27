using UnityEngine;
using Zenject;

public class ObjectClick : MonoBehaviour
{
  private IWallSelector wallSelector;

  [Inject]
  public void Construct (IWallSelector selector)
  {
    wallSelector = selector;
  }
  public void OnMouseDown()
  {
    Wall wall = GetComponent<Wall>();
    if (wall != null && wallSelector != null)
    {
      wallSelector.SelectWall(wall); 
    }
  }

  public void SetWallSelector(IWallSelector selector)
  {
    wallSelector = selector;
  }
}