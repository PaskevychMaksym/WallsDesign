using UnityEngine;

public class Wall : MonoBehaviour
{
  public Renderer WallRenderer { get; private set; }

  private void Awake()
  {
    WallRenderer = GetComponent<Renderer>();
  }
}