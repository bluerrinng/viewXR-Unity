using UnityEngine;
using GaussianSplatting.Runtime;
using Unity.Collections.LowLevel.Unsafe;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class GaussianHeatmap : MonoBehaviour
{
    public Transform cam;
    public Material heatmapMaterial;
    public float radius = 0.25f;
    public float destroyTime = 0.5f;
    public float offset = 0.2f;   // ⭐ Inspector 조절 가능

    GaussianSplatRenderer gs;
    RuntimeInputSplatData[] splats;

    string gaussianLogPath;

    void Start()
    {
        gs = FindObjectOfType<GaussianSplatRenderer>();
        if (gs == null)
        {
            Debug.LogError("GaussianSplatRenderer not found!");
            return;
        }

        LoadGaussianData();
        InitGaussianLogPath();
    }

    // ========== 로그 파일 경로 초기화 ==========
    void InitGaussianLogPath()
    {
#if UNITY_EDITOR
        string scriptPath = AssetDatabase.GetAssetPath(MonoScript.FromMonoBehaviour(this));
        string folderPath = Path.GetDirectoryName(scriptPath);
        gaussianLogPath = Path.Combine(folderPath, "gaussian_log.txt");
#else
        gaussianLogPath = Path.Combine(Application.persistentDataPath, "gaussian_log.txt");
#endif

        if (!File.Exists(gaussianLogPath))
            File.WriteAllText(gaussianLogPath, "===== Gaussian Heatmap Log =====\n\n");
    }

    void LoadGaussianData()
    {
        int count = gs.splatCount;
        splats = new RuntimeInputSplatData[count];

        int stride = UnsafeUtility.SizeOf<RuntimeInputSplatData>();
        GraphicsBuffer buffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, count, stride);

        if (!gs.EditExportData(buffer, false))
        {
            Debug.LogError("Failed to extract Gaussian GPU data.");
            return;
        }

        buffer.GetData(splats);
        buffer.Release();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
            ShootRay();
    }

    void ShootRay()
    {
        Ray ray = new Ray(cam.position, cam.forward);
        float minDist = float.MaxValue;

        Vector3 hitPos = Vector3.zero;
        int nearestIndex = -1;

        // ========== 가장 가까운 Gaussian 찾기 ==========
        for (int i = 0; i < splats.Length; i++)
        {
            Vector3 pos = gs.transform.TransformPoint(splats[i].pos);
            float dist = DistancePointToRay(pos, ray);

            if (dist < 0.02f && dist < minDist)
            {
                minDist = dist;
                hitPos = pos;
                nearestIndex = i;
            }
        }

        if (nearestIndex == -1)
            return;

        // ⭐ 로그 저장
        LogGaussianHit(hitPos, nearestIndex, hitPos);

        // ========== 주변 Gaussian 히트맵 시각화 ==========
        for (int i = 0; i < splats.Length; i++)
        {
            Vector3 pos = gs.transform.TransformPoint(splats[i].pos);
            float d = Vector3.Distance(hitPos, pos);
            if (d < radius)
                SpawnHeat(pos, d);
        }
    }

    // ========== 거리 계산 ==========
    float DistancePointToRay(Vector3 p, Ray r)
    {
        return Vector3.Cross(r.direction, p - r.origin).magnitude;
    }

    // ========== 히트맵 시각화 ==========
    void SpawnHeat(Vector3 pos, float dist)
    {
        float t = Mathf.Clamp01(dist / radius);

        float size = Mathf.Lerp(radius * 0.4f, radius * 0.1f, t);
        Color c = Color.Lerp(Color.red, Color.green, t);
        c.a = Mathf.Lerp(0.7f, 0.2f, t);

        GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
        quad.transform.position = pos + Vector3.up * offset;
        quad.transform.localScale = new Vector3(size, size, size);
        quad.transform.rotation = Quaternion.LookRotation(cam.forward);

        Renderer r = quad.GetComponent<Renderer>();
        r.material = heatmapMaterial;
        r.material.color = c;

        Destroy(quad.GetComponent<Collider>());
        Destroy(quad, destroyTime);
    }

    // ========== Gaussian 로그 기록 ==========
    void LogGaussianHit(Vector3 hit, int idx, Vector3 nearestPos)
    {
        string log =
            $"[Time {Time.time:F2}] Hit = ({hit.x:F3}, {hit.y:F3}, {hit.z:F3})\n" +
            $"[Time {Time.time:F2}] Nearest Gaussian Index = {idx}\n" +
            $"[Time {Time.time:F2}] Nearest Gaussian Pos = ({nearestPos.x:F3}, {nearestPos.y:F3}, {nearestPos.z:F3})\n\n";

        File.AppendAllText(gaussianLogPath, log);
    }
}

/* 
Gaussian Splat 기반 모델은 삼각형 메쉬 대신 수많은 3D Gaussian 점(위치, 크기, 회전, 색, 불투명도)을 조합해 물체를 표현하는 방식이다. 
이 Gaussian 점들을 직접 활용하여 시선 히트맵을 생성하는 기능을 추

히트맵 생성 과정은 두 단계로 구성:
1) 카메라 방향으로 레이를 쏘고, 레이와 가장 가까운 Gaussian 점을 찾아 사용자가 실제로 본 위치를 판단한다.
2) 이 점을 중심으로 일정 반경 안의 Gaussian들을 수집하여 색상/크기 변화로 시각화해 히트맵을 만든다.

이 방식은 기존 메쉬 기반 히트맵보다 훨씬 정밀하게 사용자의 시각적 집중 영역을 반영

플러그인의 Aras-p Gaussian Splat Repo의 구조 때문이다.
- Editor 전용 코드(PLY 파싱 등)와 Runtime 코드(렌더링)가 완전히 분리되어 있으며,
  Unity Runtime에서는 Editor 코드를 호출할 수 없다.
- 또한 Editor/Runtime에 각각 InputSplatData가 존재해 구조체 충돌도 발생했다.

이를 해결하기 위해 우리는 Runtime의 GPU Export 기능을 활용해
Gaussian 데이터를 GPU → CPU로 가져온 뒤 CPU에서 히트맵 계산을 수행 
*/
