using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;
using Zenject;

public class WallSplitter : MonoBehaviour
{
    public LineRenderer lineRendererPrefab;
    public GameObject startPointMarkerPrefab;

    private LineRenderer lineRenderer;
    private List<Vector3> points = new List<Vector3>();
    private GameObject startPointMarker;
    private bool isDrawing; 

    private IWallSelector wallSelector;
    private IGameInput gameInput;
    private Wall selectedWall;
    private Wall previousWall;

    [Inject]
    private void Construct(IWallSelector selector, IGameInput input)
    {
        wallSelector = selector;
        gameInput = input;

        gameInput.OnMouseClick += HandleMouseClick;
        gameInput.OnMouseDrag += UpdateLine;
        gameInput.OnMouseRelease += HandleMouseRelease;
        gameInput.OnUndo += UndoLastPoint;
        gameInput.OnReset += ResetDrawing;
    }

    private void Update()
    {
        selectedWall = wallSelector.SelectedWall;

        if (selectedWall != previousWall)
        {
            ResetDrawing();
            previousWall = selectedWall;
        }
    }

    private void HandleMouseClick(Vector3 point,bool isShiftPressed)
    {
        if (isDrawing || selectedWall == null) return;

        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit))
        {
            if (hit.collider.GetComponent<Wall>() == selectedWall)
            {
                StartDrawing(point);
            }
        }
    }

    private void StartDrawing(Vector3 startPoint)
    {
        isDrawing = true;
        points.Clear();

        points.Add(startPoint);

        if (startPointMarker != null)
        {
            Destroy(startPointMarker);
        }

        startPointMarker = Instantiate(startPointMarkerPrefab, startPoint, Quaternion.identity);

        if (lineRenderer == null)
        {
            lineRenderer = Instantiate(lineRendererPrefab);
            lineRenderer.useWorldSpace = true;
        }

        lineRenderer.positionCount = 1;
        lineRenderer.SetPosition(0, startPoint);

        Debug.Log("Почато малювання лінії.");
    }

    private void UpdateLine(Vector3 currentPoint, bool isShiftPressed)
    {
        if (!isDrawing || selectedWall == null) return;

        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit))
        {
            if (hit.collider.GetComponent<Wall>() != selectedWall)
            {
                return;
            }
        }

        if (isShiftPressed && points.Count > 0)
        {
            // Логіка обмеження кута
            Vector3 lastPoint = points[points.Count - 1];
            Vector3 direction = currentPoint - lastPoint;

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            float snappedAngle = Mathf.Round(angle / 15f) * 15f;
            float distance = direction.magnitude;

            float radians = snappedAngle * Mathf.Deg2Rad;
            currentPoint = lastPoint + new Vector3(Mathf.Cos(radians), Mathf.Sin(radians), 0) * distance;
        }

        if (points.Count == 0 || Vector3.Distance(points[points.Count - 1], currentPoint) > 0.1f)
        {
            if (lineRenderer.positionCount == points.Count)
            {
                lineRenderer.positionCount++;
            }

            points.Add(currentPoint);
            lineRenderer.SetPosition(lineRenderer.positionCount - 1, currentPoint);
        }
    }

    private void HandleMouseRelease(Vector3 releasePoint, bool isShiftPressed)
    {
        if (!isDrawing || selectedWall == null) return;

        if (isShiftPressed && points.Count > 0)
        {
            Vector3 lastPoint = points[points.Count - 1];
            Vector3 direction = releasePoint - lastPoint;

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            float snappedAngle = Mathf.Round(angle / 15f) * 15f;
            float distance = direction.magnitude;

            float radians = snappedAngle * Mathf.Deg2Rad;
            releasePoint = lastPoint + new Vector3(Mathf.Cos(radians), Mathf.Sin(radians), 0) * distance;
        }

        if (points.Count == 0 || Vector3.Distance(points[points.Count - 1], releasePoint) > 0.1f)
        {
            points.Add(releasePoint);

            lineRenderer.positionCount = points.Count;
            lineRenderer.SetPosition(points.Count - 1, releasePoint);
        }

        TryCompleteLine();
    }


    private void UndoLastPoint()
    {
        if (points.Count > 0)
        {
            points.RemoveAt(points.Count - 1);

            lineRenderer.positionCount = points.Count;

            for (int i = 0; i < points.Count; i++)
            {
                lineRenderer.SetPosition(i, points[i]);
            }

            Debug.Log("Остання точка видалена.");
        }
    }

    private void ResetDrawing()
    {
        points.Clear();

        if (lineRenderer != null)
        {
            lineRenderer.positionCount = 0;
        }

        if (startPointMarker != null)
        {
            Destroy(startPointMarker);
        }

        isDrawing = false;

        Debug.Log("Малювання скинуто.");
    }

    private void TryCompleteLine()
    {
        if (points.Count < 3)
        {
            Debug.LogWarning("Лінія повинна мати хоча б 3 точки для завершення.");
            return;
        }

        Vector3 firstPoint = points[0];
        Vector3 lastPoint = points[points.Count - 1];

        if (Vector3.Distance(firstPoint, lastPoint) < 0.2f)
        {
            points.Add(firstPoint);
            lineRenderer.positionCount = points.Count;
            lineRenderer.SetPosition(points.Count - 1, firstPoint);

            FinishDrawing();
        }
        else
        {
            Debug.LogWarning("Контур не замкнуто. Клікніть ближче до початкової точки.");
        }
    }

    private void FinishDrawing()
    {
        isDrawing = false;

        if (startPointMarker != null)
        {
            Destroy(startPointMarker);
        }
        
        SplitWall();
        Debug.Log("Лінія завершена.");
    }
    
    private Plane CreateCuttingPlane()
    {
        // Точки лінії в локальному просторі стіни
        List<Vector3> localPoints = points.Select(point => selectedWall.transform.InverseTransformPoint(point)).ToList();

        // Перша та остання точки лінії
        Vector3 startPoint = localPoints[0];
        Vector3 endPoint = localPoints[localPoints.Count - 1];

        // Вектор напряму лінії
        Vector3 lineDirection = (endPoint - startPoint).normalized;

        // Нормаль площини (зробити перпендикуляр до лінії та осі Y стіни)
        Vector3 up = Vector3.up; // Ось Y, якщо це вертикальна стіна
        Vector3 planeNormal = Vector3.Cross(lineDirection, up).normalized;

        // Світова точка початку
        Vector3 worldStartPoint = selectedWall.transform.TransformPoint(startPoint);

        Debug.Log($"Plane Normal: {planeNormal}, Length: {planeNormal.magnitude}");

        return new Plane(planeNormal, worldStartPoint);
    }


    private void SplitWallByPlane(Plane cuttingPlane)
    {
        ProBuilderMesh wallMesh = selectedWall.GetComponent<ProBuilderMesh>();
        if (wallMesh == null)
        {
            Debug.LogError("Об'єкт стіни не має компонента ProBuilderMesh.");
            return;
        }

        // Локальні вершини стіни
        var vertices = wallMesh.positions.ToArray();

        // Списки для розділення
        List<int> verticesAbove = new List<int>();
        List<int> verticesBelow = new List<int>();

        // Перевіряємо розташування кожної вершини
        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 worldVertex = wallMesh.transform.TransformPoint(vertices[i]);
            
            float distance = cuttingPlane.GetDistanceToPoint(worldVertex);
            Debug.Log($"Vertex: {worldVertex}, Distance to Plane: {distance}");

            if (distance > 0.001f) // Трохи більша похибка
            {
                verticesAbove.Add(i);
            }
            else if (distance < -0.001f)
            {
                verticesBelow.Add(i);
            }

        }

        if (verticesAbove.Count == 0 || verticesBelow.Count == 0)
        {
            Debug.LogError("Розділення неможливе: вершини не перетинають площину.");
            return;
        }

        // Створюємо новий об'єкт для розділеної частини
        GameObject newWallObject = Instantiate(selectedWall.gameObject, selectedWall.transform.position, selectedWall.transform.rotation, selectedWall.transform.parent);
        ProBuilderMesh newWallMesh = newWallObject.GetComponent<ProBuilderMesh>();

        // Розділяємо грані між двома Mesh
        SplitFacesByVertices(wallMesh, newWallMesh, verticesAbove);

        // Оновлюємо обидва Mesh
        wallMesh.ToMesh();
        wallMesh.Refresh();
        newWallMesh.ToMesh();
        newWallMesh.Refresh();

        Debug.Log("Стіна успішно розділена.");
    }
    private void SplitFacesByVertices(ProBuilderMesh originalMesh, ProBuilderMesh newMesh, List<int> verticesAbove)
    {
        List<Face> facesToMove = new List<Face>();

        foreach (var face in originalMesh.faces)
        {
            // Перевіряємо, чи грані повністю належать вершинам "вище"
            if (face.indexes.All(index => verticesAbove.Contains(index)))
            {
                facesToMove.Add(face);
            }
        }

        foreach (var face in facesToMove)
        {
            originalMesh.DeleteFace(face);
            newMesh.faces.Add(face);
        }
    }

    private void SplitWall()
    {
        if (selectedWall == null || points.Count < 2)
        {
            Debug.LogError("Не вдалося розділити стіну: виберіть стіну та намалюйте лінію.");
            return;
        }

        // Створюємо площину розрізу
        Plane cuttingPlane = CreateCuttingPlane();

        // Розділяємо стіну за площиною
        SplitWallByPlane(cuttingPlane);
    }
    private void OnDrawGizmos()
    {
        if (selectedWall == null || points.Count < 2) return;

        Plane plane = CreateCuttingPlane();

        // Малюємо нормаль площини
        Gizmos.color = Color.red;
        Gizmos.DrawRay(plane.normal * 5, plane.normal);

        // Малюємо точки лінії
        Gizmos.color = Color.green;
        foreach (var point in points)
        {
            Gizmos.DrawSphere(point, 0.1f);
        }
    }

}
