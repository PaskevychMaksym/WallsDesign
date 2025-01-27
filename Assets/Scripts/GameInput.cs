using System;
using UnityEngine;

public class GameInput : MonoBehaviour, IGameInput
{
  public event Action<Vector3, bool> OnMouseClick = delegate { };
  public event Action<Vector3, bool> OnMouseRelease = delegate { };
  public event Action<Vector3, bool> OnMouseDrag = delegate { };
  public event Action OnUndo = delegate { };
  public event Action OnReset = delegate { };

  private void Update()
  {
    bool isShiftPressed = Input.GetKey(KeyCode.LeftShift);

    if (Input.GetMouseButtonDown(0))
    {
      Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
      if (Physics.Raycast(ray, out RaycastHit hit))
      {
        OnMouseClick?.Invoke(hit.point, isShiftPressed);
      }
    }

    if (Input.GetMouseButton(0))
    {
      Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
      if (Physics.Raycast(ray, out RaycastHit hit))
      {
        OnMouseDrag?.Invoke(hit.point, isShiftPressed);
      }
    }

    if (Input.GetMouseButtonUp(0))
    {
      Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
      if (Physics.Raycast(ray, out RaycastHit hit))
      {
        OnMouseRelease?.Invoke(hit.point, isShiftPressed);
      }
    }

    if (Input.GetKeyDown(KeyCode.Z))
    {
      OnUndo?.Invoke();
    }

    if (Input.GetKeyDown(KeyCode.R))
    {
      OnReset?.Invoke();
    }
  }
}