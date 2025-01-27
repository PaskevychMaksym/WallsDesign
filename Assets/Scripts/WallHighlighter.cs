using DG.Tweening;
using UnityEngine;

public class WallHighlighter : IWallHighlighter
{
  public void Highlight(Renderer wallRenderer)
  {
    if (wallRenderer != null)
    {
      wallRenderer.material.DOColor(Color.yellow, 0.3f);
    }
  }

  public void Unhighlight(Renderer wallRenderer)
  {
    if (wallRenderer != null)
    {
      wallRenderer.material.DOColor(Color.white, 0.3f);
    }
  }
}