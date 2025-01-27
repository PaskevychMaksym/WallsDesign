public interface IWallSelector
{
  public Wall SelectedWall { get;}
  void SelectWall(Wall wall);
  void DeselectWall();
}