using System;
using UnityEngine;

public interface IGameInput
{
  event Action<Vector3, bool> OnMouseClick;
  event Action<Vector3, bool> OnMouseRelease;
  event Action<Vector3, bool> OnMouseDrag;
  event Action OnUndo;
  event Action OnReset;
}