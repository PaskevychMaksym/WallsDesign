using System.Collections.Generic;
using UnityEngine;

public class ChangeTexture : MonoBehaviour
{
  public List<Texture> allTextures;
  private WallSelector wallSelector;

  public void SetWallSelector(WallSelector selector)
  {
    wallSelector = selector;
  }

  public void ChangeTextureForSelectedWall(string textureName)
  {
    /*Wall selectedWall = wallSelector.SelectedWall;
    if (selectedWall == null)
    {
      Debug.Log("No wall selected");
      return;
    }

    Texture currentTexture = allTextures.Find(x => x.name == textureName);
    Renderer renderer = selectedWall.WallRenderer;

    if (currentTexture && renderer)
    {
      renderer.material.color = Color.white;
      renderer.material.mainTexture = currentTexture;
    }
    else
    {
      renderer.material.color = Color.red;
    }*/
  }
}