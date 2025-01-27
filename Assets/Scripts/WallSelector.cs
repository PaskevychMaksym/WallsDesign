using Zenject;
public class WallSelector : IWallSelector
{
  private IWallHighlighter wallHighlighter;
  private Wall selectedWall;
  
  public Wall SelectedWall => selectedWall;

  [Inject]
  public WallSelector(IWallHighlighter highlighter)
  {
    wallHighlighter = highlighter;
  }

  public void SelectWall(Wall wall)
  {
    if (SelectedWall != null)
    {
      DeselectWall();
    }

    selectedWall = wall;
    wallHighlighter.Highlight(wall.WallRenderer);
  }

  public void DeselectWall()
  {
    if (selectedWall == null)
    {
      return;
    }

    wallHighlighter.Unhighlight(selectedWall.WallRenderer);
    selectedWall = null;
  }
}