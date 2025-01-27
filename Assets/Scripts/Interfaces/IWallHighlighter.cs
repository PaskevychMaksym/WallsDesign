using UnityEngine;

public interface IWallHighlighter
{
  void Highlight(Renderer wall);
  void Unhighlight(Renderer wall);
}