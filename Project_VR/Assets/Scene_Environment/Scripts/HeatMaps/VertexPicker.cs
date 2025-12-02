using System.Collections.Generic;
using UnityEngine;

public class VertexHeatmap : MonoBehaviour
{
    public Camera cam;
    public MeshFilter meshFilter;
    public MeshCollider meshCollider;

    public float radius = 0.3f; // 히트맵 원 최대 반경
    public int maxPoints = 150;  // 최대 점 개수

    public float destroyTime = 0.5f;

    public Material heatmapMaterial; // Inspector에서 투명 셰이더 머티리얼 할당

    public float interpolationStep = 0.02f; // 간격 (보간점 사이 거리)

    void Start()
    {
        if (meshCollider == null)
            meshCollider = meshFilter.GetComponent<MeshCollider>();
        if (meshCollider == null)
            meshCollider = meshFilter.gameObject.AddComponent<MeshCollider>();

        if (cam == null)
            cam = Camera.main;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider != meshCollider) return;


                Mesh mesh = meshFilter.sharedMesh;
                Vector3[] vertices = mesh.vertices;
                int[] triangles = mesh.triangles;
                Vector3 hitPoint = hit.point;
                Debug.Log($"[Heatmap] Raycast Hit Point: {hitPoint}");

                List<(Vector3 pos, float dist)> closePoints = new List<(Vector3, float)>();

                // 1) 버텍스 월드 좌표화 후 반경 내 필터링
                List<Vector3> worldVertices = new List<Vector3>();
                foreach (var v in vertices)
                    worldVertices.Add(meshFilter.transform.TransformPoint(v));

                foreach (var v in worldVertices)
                {
                    float dist = Vector3.Distance(hitPoint, v);
                    if (dist <= radius)
                        closePoints.Add((v, dist));
                }

                // 2) 버텍스들 사이 간격 보간점 추가
                var interpolatedPoints = CreateInterpolatedPoints(meshFilter.transform, vertices, triangles, hitPoint, radius);

                closePoints.AddRange(interpolatedPoints);

                // 거리 기준 정렬
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
    }

    List<(Vector3 pos, float dist)> CreateInterpolatedPoints(Transform meshTransform, Vector3[] vertices, int[] triangles, Vector3 hitPoint, float maxRadius)
    {
        List<(Vector3 pos, float dist)> points = new List<(Vector3, float)>();

        // 각 삼각형마다 3개 에지에 대해 보간점 생성
        for (int i = 0; i < triangles.Length; i += 3)
        {
            int i0 = triangles[i];
            int i1 = triangles[i + 1];
            int i2 = triangles[i + 2];

            Vector3 v0 = meshTransform.TransformPoint(vertices[i0]);
            Vector3 v1 = meshTransform.TransformPoint(vertices[i1]);
            Vector3 v2 = meshTransform.TransformPoint(vertices[i2]);

            // 3개 에지 처리
            AddInterpolatedPointsOnEdge(points, v0, v1, hitPoint, maxRadius);
            AddInterpolatedPointsOnEdge(points, v1, v2, hitPoint, maxRadius);
            AddInterpolatedPointsOnEdge(points, v2, v0, hitPoint, maxRadius);
        }

        return points;
    }

    void AddInterpolatedPointsOnEdge(List<(Vector3 pos, float dist)> points, Vector3 start, Vector3 end, Vector3 hitPoint, float maxRadius)
    {
        float edgeLength = Vector3.Distance(start, end);
        if (edgeLength < 0.01f) return; // 너무 짧으면 스킵

        int steps = Mathf.FloorToInt(edgeLength / interpolationStep);
        for (int s = 1; s < steps; s++) // s=0은 start, s=steps는 end라 중간만
        {
            float t = s / (float)steps;
            Vector3 interpPos = Vector3.Lerp(start, end, t);
            float dist = Vector3.Distance(interpPos, hitPoint);
            if (dist <= maxRadius)
                points.Add((interpPos, dist));
        }
    }

    void CreateHeatmapCircle(Vector3 position, float size, Color color)
    {
        GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Cube);
        quad.transform.position = position + Vector3.up * 0.01f; // 약간 띄워서 Z-fighting 방지
        quad.transform.localScale = new Vector3(size, size, .5f);

        // 카메라 쪽으로 항상 바라보게 설정
        quad.transform.rotation = Quaternion.LookRotation(cam.transform.forward);

        var renderer = quad.GetComponent<Renderer>();
        renderer.material = heatmapMaterial;
        renderer.material.color = color;

        Destroy(quad.GetComponent<Collider>());
        Destroy(quad, destroyTime);
    }
}
