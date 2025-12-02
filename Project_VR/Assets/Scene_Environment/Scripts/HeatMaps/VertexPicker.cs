using System.Collections.Generic;
using System.IO;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class VertexHeatmap : MonoBehaviour
{
    public Camera cam;
    public MeshFilter meshFilter;
    public MeshCollider meshCollider;

    public float radius = 0.3f;
    public int maxPoints = 150;
    public float destroyTime = 0.5f;
    public Material heatmapMaterial;
    public float interpolationStep = 0.02f;

    string logFilePath;

    void Start()
    {
        if (meshCollider == null)
            meshCollider = meshFilter.GetComponent<MeshCollider>();
        if (meshCollider == null)
            meshCollider = meshFilter.gameObject.AddComponent<MeshCollider>();

        if (cam == null)
            cam = Camera.main;

#if UNITY_EDITOR
        string scriptPath = AssetDatabase.GetAssetPath(MonoScript.FromMonoBehaviour(this));
        string folderPath = Path.GetDirectoryName(scriptPath);
        logFilePath = Path.Combine(folderPath, "monitor_gaze.txt");
#else
        logFilePath = Path.Combine(Application.persistentDataPath, "monitor_gaze.txt");
#endif

        if (!File.Exists(logFilePath))
            File.WriteAllText(logFilePath, "===== Monitor Gaze Log =====\n\n");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

            if (!Physics.Raycast(ray, out RaycastHit hit)) return;
            if (hit.collider != meshCollider) return;

            Vector3 hitPoint = hit.point;

            // ========== 가장 가까운 vertex 찾기 ==========
            Mesh mesh = meshFilter.sharedMesh;
            Vector3[] vertices = mesh.vertices;

            int nearestIndex = -1;
            float nearestDist = float.MaxValue;
            Vector3 nearestVertexWorld = Vector3.zero;

            for (int i = 0; i < vertices.Length; i++)
            {
                Vector3 worldPos = meshFilter.transform.TransformPoint(vertices[i]);
                float dist = Vector3.Distance(hitPoint, worldPos);

                if (dist < nearestDist)
                {
                    nearestDist = dist;
                    nearestIndex = i;
                    nearestVertexWorld = worldPos;
                }
            }

            // ★ 로그 저장 ★
            LogGazePoint(hitPoint, nearestIndex, nearestVertexWorld);

            // ========== 히트맵 시각화 (기존 기능 유지) ==========
            Vector3[] verts = mesh.vertices;
            int[] triangles = mesh.triangles;

            List<(Vector3 pos, float dist)> closePoints = new List<(Vector3, float)>();

            foreach (var v in verts)
            {
                Vector3 wp = meshFilter.transform.TransformPoint(v);
                float d = Vector3.Distance(hitPoint, wp);
                if (d <= radius)
                    closePoints.Add((wp, d));
            }

            var interpolated = CreateInterpolatedPoints(meshFilter.transform, verts, triangles, hitPoint, radius);
            closePoints.AddRange(interpolated);

            closePoints.Sort((a, b) => a.dist.CompareTo(b.dist));

            int count = Mathf.Min(maxPoints, closePoints.Count);

            for (int i = 0; i < count; i++)
            {
                float t = Mathf.Clamp01(closePoints[i].dist / radius);
                float size = Mathf.Lerp(radius * 0.4f, radius * 0.15f, t);
                Color color = Color.Lerp(Color.red, Color.green, t);
                color.a = Mathf.Lerp(0.7f, 0.2f, t);

                CreateHeatmapCircle(closePoints[i].pos, size, color);
            }
        }
    }

    // ========== 로그 기록 ==========
    void LogGazePoint(Vector3 hit, int nearestIndex, Vector3 nearestPos)
    {
        string log =
            $"[Time {Time.time:F2}] Raycast Hit = ({hit.x:F2}, {hit.y:F2}, {hit.z:F2})\n" +
            $"[Time {Time.time:F2}] Nearest Vertex Index = {nearestIndex}, Pos = ({nearestPos.x:F2}, {nearestPos.y:F2}, {nearestPos.z:F2})\n\n";

        File.AppendAllText(logFilePath, log);
    }

    // ========== 보간 ==========
    List<(Vector3 pos, float dist)> CreateInterpolatedPoints(Transform meshTransform, Vector3[] vertices, int[] triangles, Vector3 hitPoint, float maxRadius)
    {
        List<(Vector3 pos, float dist)> points = new();

        for (int i = 0; i < triangles.Length; i += 3)
        {
            Vector3 v0 = meshTransform.TransformPoint(vertices[triangles[i]]);
            Vector3 v1 = meshTransform.TransformPoint(vertices[triangles[i + 1]]);
            Vector3 v2 = meshTransform.TransformPoint(vertices[triangles[i + 2]]);

            AddInterpolated(points, v0, v1, hitPoint, maxRadius);
            AddInterpolated(points, v1, v2, hitPoint, maxRadius);
            AddInterpolated(points, v2, v0, hitPoint, maxRadius);
        }

        return points;
    }

    void AddInterpolated(List<(Vector3 pos, float dist)> points, Vector3 a, Vector3 b, Vector3 hitPoint, float maxRadius)
    {
        float len = Vector3.Distance(a, b);
        if (len < 0.01f) return;

        int steps = Mathf.FloorToInt(len / interpolationStep);

        for (int s = 1; s < steps; s++)
        {
            float t = s / (float)steps;
            Vector3 pos = Vector3.Lerp(a, b, t);
            float dist = Vector3.Distance(pos, hitPoint);

            if (dist <= maxRadius)
                points.Add((pos, dist));
        }
    }

    // ========== 히트맵 시각화 ==========
    void CreateHeatmapCircle(Vector3 position, float size, Color color)
    {
        GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Cube);
        quad.transform.position = position + Vector3.up * 0.01f;
        quad.transform.localScale = new Vector3(size, size, .5f);
        quad.transform.rotation = Quaternion.LookRotation(cam.transform.forward);

        Renderer r = quad.GetComponent<Renderer>();
        r.material = heatmapMaterial;
        r.material.color = color;

        Destroy(quad.GetComponent<Collider>());
        Destroy(quad, destroyTime);
    }
}
