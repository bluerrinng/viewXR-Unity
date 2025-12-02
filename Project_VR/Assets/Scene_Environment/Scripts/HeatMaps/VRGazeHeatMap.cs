using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class VRGazeHeatMap : MonoBehaviour
{
    public MeshFilter meshFilter;
    public MeshCollider meshCollider;
    public Transform vrHeadTransform;        // 시선 기준 (XR 카메라)
    public Material heatmapMaterial;

    public float radius = 0.3f;
    public int maxPoints = 150;
    public float destroyTime = 0.5f;
    public float interpolationStep = 0.02f;

    private InputDevice rightHand;

    void Start()
    {
        if (meshCollider == null)
            meshCollider = meshFilter.GetComponent<MeshCollider>();
        if (meshCollider == null)
            meshCollider = meshFilter.gameObject.AddComponent<MeshCollider>();

        if (vrHeadTransform == null)
            Debug.LogError("[VRGazeHeatmap] vrHeadTransform (XR 카메라) 를 할당해주세요.");

        InitializeRightHand();
    }

    void Update()
    {
        if (CheckBButtonPressed())
        {
            ShootGazeRaycast();
        }
    }

    void InitializeRightHand()
    {
        var desiredCharacteristics = InputDeviceCharacteristics.Right | InputDeviceCharacteristics.Controller;
        var devices = new List<InputDevice>();
        InputDevices.GetDevicesWithCharacteristics(desiredCharacteristics, devices);
        if (devices.Count > 0)
            rightHand = devices[0];
    }

    bool CheckBButtonPressed()
    {
        if (!rightHand.isValid)
            InitializeRightHand();

        if (rightHand.TryGetFeatureValue(CommonUsages.secondaryButton, out bool pressed) && pressed)
            return true;

        return false;
    }

    void ShootGazeRaycast()
    {
        Ray ray = new Ray(vrHeadTransform.position, vrHeadTransform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider != meshCollider) return;
            CreateHeatmapAtPoint(hit.point);

            Debug.Log($"[Heatmap] Raycast Hit Point: {hit.point}");
        }
    }

    void CreateHeatmapAtPoint(Vector3 hitPoint)
    {
        Mesh mesh = meshFilter.sharedMesh;
        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;

        List<(Vector3 pos, float dist)> closePoints = new();

        foreach (var v in vertices)
        {
            Vector3 worldPos = meshFilter.transform.TransformPoint(v);
            float dist = Vector3.Distance(hitPoint, worldPos);
            if (dist <= radius)
                closePoints.Add((worldPos, dist));
        }

        var interpolated = CreateInterpolatedPoints(meshFilter.transform, vertices, triangles, hitPoint, radius);
        closePoints.AddRange(interpolated);
        closePoints.Sort((a, b) => a.dist.CompareTo(b.dist));

        int count = Mathf.Min(maxPoints, closePoints.Count);
        for (int i = 0; i < count; i++)
        {
            float t = Mathf.Clamp01(closePoints[i].dist / radius);
            float size = Mathf.Lerp(radius * 0.4f, radius * 0.15f, t);
            Color color = Color.Lerp(Color.red, Color.green, t);
            color.a = Mathf.Lerp(0.7f, 0.2f, t);

            CreateHeatmapQuad(closePoints[i].pos, size, color);
        }
    }

    List<(Vector3 pos, float dist)> CreateInterpolatedPoints(Transform meshTransform, Vector3[] vertices, int[] triangles, Vector3 hitPoint, float maxRadius)
    {
        List<(Vector3 pos, float dist)> points = new();

        for (int i = 0; i < triangles.Length; i += 3)
        {
            int i0 = triangles[i];
            int i1 = triangles[i + 1];
            int i2 = triangles[i + 2];

            Vector3 v0 = meshTransform.TransformPoint(vertices[i0]);
            Vector3 v1 = meshTransform.TransformPoint(vertices[i1]);
            Vector3 v2 = meshTransform.TransformPoint(vertices[i2]);

            AddInterpolated(points, v0, v1, hitPoint, maxRadius);
            AddInterpolated(points, v1, v2, hitPoint, maxRadius);
            AddInterpolated(points, v2, v0, hitPoint, maxRadius);
        }

        return points;
    }

    void AddInterpolated(List<(Vector3 pos, float dist)> points, Vector3 a, Vector3 b, Vector3 center, float maxRadius)
    {
        float len = Vector3.Distance(a, b);
        if (len < 0.01f) return;

        int steps = Mathf.FloorToInt(len / interpolationStep);
        for (int s = 1; s < steps; s++)
        {
            float t = s / (float)steps;
            Vector3 p = Vector3.Lerp(a, b, t);
            float dist = Vector3.Distance(p, center);
            if (dist <= maxRadius)
                points.Add((p, dist));
        }
    }

    void CreateHeatmapQuad(Vector3 pos, float size, Color color)
    {
        GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Cube);
        quad.transform.position = pos + Vector3.up * 0.01f;
        quad.transform.localScale = new Vector3(size, size, 0.5f);
        quad.transform.rotation = Quaternion.LookRotation(vrHeadTransform.forward);

        var renderer = quad.GetComponent<Renderer>();
        renderer.material = heatmapMaterial;
        renderer.material.color = color;

        Destroy(quad.GetComponent<Collider>());
        Destroy(quad, destroyTime);
    }
}
