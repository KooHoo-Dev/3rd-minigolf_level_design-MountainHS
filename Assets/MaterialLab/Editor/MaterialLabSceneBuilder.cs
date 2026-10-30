#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;

namespace MaterialLab
{
    public static class MaterialLabSceneBuilder
    {
        private const string Root = "Assets/MaterialLab";
        private const string Generated = Root + "/Generated";
        private const string Materials = Generated + "/Materials";
        private const string Textures = Generated + "/Textures";
        private const string ScenePath = Generated + "/MaterialLab.unity";

        [MenuItem("Tools/Material Lab/Build Material Lab Scene")]
        public static void Build()
        {
            EnsureFolder("Assets", "MaterialLab");
            EnsureFolder(Root, "Generated");
            EnsureFolder(Generated, "Materials");
            EnsureFolder(Generated, "Textures");

            var lit = Shader.Find("Universal Render Pipeline/Lit");
            var unlit = Shader.Find("Universal Render Pipeline/Unlit");
            if (lit == null || unlit == null)
            {
                EditorUtility.DisplayDialog(
                    "Material Lab",
                    "URP Lit/Unlit shader를 찾지 못했습니다.\n현재 프로젝트가 URP인지 확인하세요.",
                    "OK");
                return;
            }

            CreateProceduralTextures();

            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            var root = new GameObject("=== MATERIAL LAB ===");
            BuildEnvironment(root.transform, lit);
            BuildLitVsUnlit(root.transform, lit, unlit);
            BuildMetallicSmoothnessGrid(root.transform, lit);
            BuildAlbedo(root.transform, lit);
            BuildNormalAO(root.transform, lit);
            BuildWetSurface(root.transform, lit);
            BuildMaterialAccess(root.transform, lit);
            BuildBatching(root.transform, lit);

            Selection.activeGameObject = root;
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene, ScenePath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            EditorUtility.DisplayDialog(
                "Material Lab",
                "MaterialLab.unity 생성 완료.\n\n" +
                "Assets/MaterialLab/Generated/MaterialLab.unity\n\n" +
                "Hierarchy의 각 실험 구역과 README를 따라가면 됩니다.",
                "OK");
        }

        private static void BuildEnvironment(Transform parent, Shader lit)
        {
            var section = Section(parent, "00_ENVIRONMENT", Vector3.zero);

            var floorMat = GetOrCreateMaterial("M_Floor", lit);
            floorMat.SetColor("_BaseColor", new Color(0.18f, 0.18f, 0.18f));
            floorMat.SetFloat("_Metallic", 0f);
            floorMat.SetFloat("_Smoothness", 0.15f);

            var floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
            floor.name = "Floor";
            floor.transform.SetParent(section);
            floor.transform.position = new Vector3(15f, 0f, 16f);
            floor.transform.localScale = new Vector3(6f, 1f, 7f);
            floor.GetComponent<Renderer>().sharedMaterial = floorMat;

            var lightGO = new GameObject("Directional Light");
            lightGO.transform.SetParent(section);
            lightGO.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
            var light = lightGO.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1.25f;
            light.shadows = LightShadows.Soft;

            var fillGO = new GameObject("Fill Light");
            fillGO.transform.SetParent(section);
            fillGO.transform.position = new Vector3(14f, 6f, 14f);
            var fill = fillGO.AddComponent<Light>();
            fill.type = LightType.Point;
            fill.range = 18f;
            fill.intensity = 1200f;

            var probeGO = new GameObject("Reflection Probe");
            probeGO.transform.SetParent(section);
            probeGO.transform.position = new Vector3(15f, 4f, 16f);
            var probe = probeGO.AddComponent<ReflectionProbe>();
            probe.mode = ReflectionProbeMode.Realtime;
            probe.refreshMode = ReflectionProbeRefreshMode.OnAwake;
            probe.size = new Vector3(55f, 12f, 60f);

            var camGO = new GameObject("Main Camera");
            camGO.tag = "MainCamera";
            camGO.transform.SetParent(section);
            camGO.transform.position = new Vector3(15f, 18f, -12f);
            camGO.transform.rotation = Quaternion.Euler(24f, 0f, 0f);
            var cam = camGO.AddComponent<Camera>();
            cam.fieldOfView = 60f;
            cam.nearClipPlane = 0.1f;
            cam.farClipPlane = 200f;

            var controls = camGO.AddComponent<MaterialLabFreeCamera>();
            controls.moveSpeed = 12f;
            controls.lookSpeed = 2.5f;

            CreateLabel(section, "Material Lab — WASD 이동 / RMB 드래그 시점", 
                new Vector3(15f, 0.05f, 1f), 0.8f);
        }

        private static void BuildLitVsUnlit(Transform parent, Shader lit, Shader unlit)
        {
            var section = Section(parent, "01_LIT_vs_UNLIT", new Vector3(0f, 0f, 4f));
            CreateHeader(section, "01  Lit vs Unlit", new Vector3(0f, 0.2f, 0f));

            var litMat = GetOrCreateMaterial("M_01_Lit_Blue", lit);
            litMat.SetColor("_BaseColor", new Color(0.1f, 0.45f, 0.9f));
            litMat.SetFloat("_Metallic", 0f);
            litMat.SetFloat("_Smoothness", 0.35f);

            var unlitMat = GetOrCreateMaterial("M_01_Unlit_Blue", unlit);
            unlitMat.SetColor("_BaseColor", new Color(0.1f, 0.45f, 0.9f));

            CreateSphere(section, "Lit", new Vector3(-2.2f, 1.2f, 1.5f), litMat);
            CreateSphere(section, "Unlit", new Vector3(2.2f, 1.2f, 1.5f), unlitMat);
            CreateLabel(section, "조명 영향 O", new Vector3(-2.2f, 0.2f, 1.5f), 0.35f);
            CreateLabel(section, "조명 영향 X", new Vector3(2.2f, 0.2f, 1.5f), 0.35f);
        }

        private static void BuildMetallicSmoothnessGrid(Transform parent, Shader lit)
        {
            var section = Section(parent, "02_METALLIC_x_SMOOTHNESS", new Vector3(0f, 0f, 10f));
            CreateHeader(section, "02  Metallic × Smoothness", new Vector3(5f, 0.2f, 0f));

            float[] values = { 0f, 0.25f, 0.5f, 0.75f, 1f };

            for (int row = 0; row < values.Length; row++)
            {
                for (int col = 0; col < values.Length; col++)
                {
                    float metallic = values[col];
                    float smoothness = values[values.Length - 1 - row];

                    var mat = GetOrCreateMaterial($"M_02_M{metallic:0.00}_S{smoothness:0.00}", lit);
                    mat.SetColor("_BaseColor", new Color(0.72f, 0.22f, 0.08f));
                    mat.SetFloat("_Metallic", metallic);
                    mat.SetFloat("_Smoothness", smoothness);

                    CreateSphere(
                        section,
                        $"Metallic_{metallic:0.00}_Smoothness_{smoothness:0.00}",
                        new Vector3(col * 2.5f, 1.3f, row * 2.5f + 1.7f),
                        mat,
                        0.9f);
                }
            }

            for (int col = 0; col < values.Length; col++)
                CreateLabel(section, $"M {values[col]:0.00}", new Vector3(col * 2.5f, 0.15f, 1.7f), 0.26f);

            for (int row = 0; row < values.Length; row++)
            {
                float s = values[values.Length - 1 - row];
                CreateLabel(section, $"S {s:0.00}", new Vector3(-1.7f, 0.15f, row * 2.5f + 1.7f), 0.26f);
            }
        }

        private static void BuildAlbedo(Transform parent, Shader lit)
        {
            var section = Section(parent, "03_ALBEDO", new Vector3(17f, 0f, 4f));
            CreateHeader(section, "03  Albedo", new Vector3(4f, 0.2f, 0f));

            Color[] colors =
            {
                new Color(0.15f, 0.15f, 0.15f),
                new Color(0.55f, 0.55f, 0.55f),
                new Color(0.85f, 0.15f, 0.1f),
                new Color(0.12f, 0.65f, 0.22f)
            };

            string[] names = { "Dark", "Gray", "Red", "Green" };

            for (int i = 0; i < colors.Length; i++)
            {
                var mat = GetOrCreateMaterial($"M_03_{names[i]}", lit);
                mat.SetColor("_BaseColor", colors[i]);
                mat.SetFloat("_Metallic", 0f);
                mat.SetFloat("_Smoothness", 0.35f);
                CreateSphere(section, names[i], new Vector3(i * 2.5f, 1.2f, 1.7f), mat);
            }
        }

        private static void BuildNormalAO(Transform parent, Shader lit)
        {
            var section = Section(parent, "04_NORMAL_and_AO", new Vector3(17f, 0f, 10f));
            CreateHeader(section, "04  Normal Map / AO", new Vector3(4f, 0.2f, 0f));

            var normalTex = AssetDatabase.LoadAssetAtPath<Texture2D>(Textures + "/T_CheckerNormal.png");
            var aoTex = AssetDatabase.LoadAssetAtPath<Texture2D>(Textures + "/T_CheckerAO.png");

            var baseMat = GetOrCreateMaterial("M_04_Base", lit);
            baseMat.SetColor("_BaseColor", new Color(0.55f, 0.42f, 0.28f));
            baseMat.SetFloat("_Metallic", 0f);
            baseMat.SetFloat("_Smoothness", 0.3f);

            var normalMat = GetOrCreateMaterial("M_04_Normal", lit);
            CopyPbr(baseMat, normalMat);
            normalMat.SetTexture("_BumpMap", normalTex);
            normalMat.SetFloat("_BumpScale", 1f);
            normalMat.EnableKeyword("_NORMALMAP");

            var aoMat = GetOrCreateMaterial("M_04_AO", lit);
            CopyPbr(baseMat, aoMat);
            aoMat.SetTexture("_OcclusionMap", aoTex);
            aoMat.SetFloat("_OcclusionStrength", 1f);
            aoMat.EnableKeyword("_OCCLUSIONMAP");

            var bothMat = GetOrCreateMaterial("M_04_Normal_AO", lit);
            CopyPbr(baseMat, bothMat);
            bothMat.SetTexture("_BumpMap", normalTex);
            bothMat.SetFloat("_BumpScale", 1f);
            bothMat.SetTexture("_OcclusionMap", aoTex);
            bothMat.SetFloat("_OcclusionStrength", 1f);
            bothMat.EnableKeyword("_NORMALMAP");
            bothMat.EnableKeyword("_OCCLUSIONMAP");

            Material[] mats = { baseMat, normalMat, aoMat, bothMat };
            string[] labels = { "Base", "Normal", "AO", "Normal + AO" };

            for (int i = 0; i < mats.Length; i++)
            {
                var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.name = labels[i];
                cube.transform.SetParent(section);
                cube.transform.position = section.position + new Vector3(i * 2.7f, 1.1f, 1.7f);
                cube.transform.localScale = new Vector3(1.8f, 1.8f, 1.8f);
                cube.GetComponent<Renderer>().sharedMaterial = mats[i];
                CreateLabel(section, labels[i], new Vector3(i * 2.7f, 0.15f, 1.7f), 0.28f);
            }
        }

        private static void BuildWetSurface(Transform parent, Shader lit)
        {
            var section = Section(parent, "05_WET_SURFACE", new Vector3(0f, 0f, 25f));
            CreateHeader(section, "05  Wet Surface — 1/2/3 키", new Vector3(4f, 0.2f, 0f));

            var asphalt = GetOrCreateMaterial("M_05_Asphalt", lit);
            asphalt.SetColor("_BaseColor", new Color(0.23f, 0.23f, 0.23f));
            asphalt.SetFloat("_Metallic", 0f);
            asphalt.SetFloat("_Smoothness", 0.2f);

            for (int i = 0; i < 3; i++)
            {
                var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.name = i == 0 ? "Dry" : (i == 1 ? "Runtime_Wetness" : "Wrong_Metallic_Wet");
                cube.transform.SetParent(section);
                cube.transform.position = section.position + new Vector3(i * 4f, 0.6f, 2f);
                cube.transform.localScale = new Vector3(3f, 0.4f, 3f);
                cube.GetComponent<Renderer>().sharedMaterial = asphalt;

                if (i == 1)
                {
                    var wet = cube.AddComponent<WetSurfaceDemo>();
                    wet.target = cube.GetComponent<Renderer>();
                    wet.drySmoothness = 0.2f;
                    wet.wetSmoothness = 0.95f;
                }
                else if (i == 2)
                {
                    var wrong = cube.AddComponent<WrongWetSurfaceDemo>();
                    wrong.target = cube.GetComponent<Renderer>();
                }
            }

            CreateLabel(section, "Dry\nSmoothness 0.2", new Vector3(0f, 0.15f, 4.1f), 0.26f);
            CreateLabel(section, "1/2/3 = Wetness\nSmoothness만 변화", new Vector3(4f, 0.15f, 4.1f), 0.26f);
            CreateLabel(section, "잘못된 예\nMetallic 증가", new Vector3(8f, 0.15f, 4.1f), 0.26f);
        }

        private static void BuildMaterialAccess(Transform parent, Shader lit)
        {
            var section = Section(parent, "06_MATERIAL_ACCESS", new Vector3(17f, 0f, 25f));
            CreateHeader(section, "06  sharedMaterial / material / MPB", new Vector3(5f, 0.2f, 0f));

            var shared = GetOrCreateMaterial("M_06_Shared", lit);
            shared.SetColor("_BaseColor", new Color(0.25f, 0.65f, 0.9f));
            shared.SetFloat("_Metallic", 0f);
            shared.SetFloat("_Smoothness", 0.4f);

            var host = new GameObject("MaterialAccessDemo");
            host.transform.SetParent(section);
            var demo = host.AddComponent<MaterialAccessDemo>();
            demo.sharedMaterial = shared;

            demo.sharedGroup = CreateDemoRow(section, "A_SharedMaterial", new Vector3(0, 0, 1.8f), shared, 5);
            demo.materialGroup = CreateDemoRow(section, "B_renderer.material", new Vector3(0, 0, 4.5f), shared, 5);
            demo.mpbGroup = CreateDemoRow(section, "C_MaterialPropertyBlock", new Vector3(0, 0, 7.2f), shared, 5);

            CreateLabel(section, "A: 모두 원본 공유", new Vector3(-2.2f, 0.15f, 1.8f), 0.24f);
            CreateLabel(section, "B: M 키 → material 접근 후 개별 복제", new Vector3(-2.2f, 0.15f, 4.5f), 0.24f);
            CreateLabel(section, "C: P 키 → MPB로 색만 개별 변경", new Vector3(-2.2f, 0.15f, 7.2f), 0.24f);
        }

        private static void BuildBatching(Transform parent, Shader lit)
        {
            var section = Section(parent, "07_BATCHING_FRAME_DEBUGGER", new Vector3(0f, 0f, 38f));
            CreateHeader(section, "07  Batching / Frame Debugger", new Vector3(8f, 0.2f, 0f));

            var shared = GetOrCreateMaterial("M_07_BatchingShared", lit);
            shared.SetColor("_BaseColor", new Color(0.2f, 0.75f, 0.45f));
            shared.SetFloat("_Metallic", 0f);
            shared.SetFloat("_Smoothness", 0.3f);
            shared.enableInstancing = true;

            var brokenMats = new Material[20];
            for (int i = 0; i < brokenMats.Length; i++)
            {
                brokenMats[i] = GetOrCreateMaterial($"M_07_Unique_{i:00}", lit);
                brokenMats[i].SetColor("_BaseColor", Color.Lerp(
                    new Color(0.8f, 0.25f, 0.2f),
                    new Color(0.9f, 0.75f, 0.15f),
                    i / 19f));
                brokenMats[i].SetFloat("_Metallic", 0f);
                brokenMats[i].SetFloat("_Smoothness", 0.3f);
            }

            var sharedRoot = new GameObject("A_20_Cubes_ONE_Shared_Material");
            sharedRoot.transform.SetParent(section);
            var uniqueRoot = new GameObject("B_20_Cubes_UNIQUE_Materials");
            uniqueRoot.transform.SetParent(section);

            for (int i = 0; i < 20; i++)
            {
                int x = i % 10;
                int z = i / 10;

                var a = GameObject.CreatePrimitive(PrimitiveType.Cube);
                a.name = $"Shared_{i:00}";
                a.transform.SetParent(sharedRoot.transform);
                a.transform.position = section.position + new Vector3(x * 1.5f, 0.6f, 2f + z * 1.5f);
                a.transform.localScale = Vector3.one * 0.9f;
                a.GetComponent<Renderer>().sharedMaterial = shared;

                var b = GameObject.CreatePrimitive(PrimitiveType.Cube);
                b.name = $"Unique_{i:00}";
                b.transform.SetParent(uniqueRoot.transform);
                b.transform.position = section.position + new Vector3(x * 1.5f, 0.6f, 7f + z * 1.5f);
                b.transform.localScale = Vector3.one * 0.9f;
                b.GetComponent<Renderer>().sharedMaterial = brokenMats[i];
            }

            CreateLabel(section, "위: 같은 Mesh + 같은 Material", new Vector3(7f, 0.15f, 0f), 0.26f);
            CreateLabel(section, "아래: Material 20개 → Frame Debugger에서 비교", new Vector3(7f, 0.15f, 5.1f), 0.26f);
        }

        private static Renderer[] CreateDemoRow(Transform parent, string name, Vector3 localBase, Material mat, int count)
        {
            var row = new GameObject(name);
            row.transform.SetParent(parent);

            var result = new Renderer[count];
            for (int i = 0; i < count; i++)
            {
                var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                sphere.name = $"{name}_{i}";
                sphere.transform.SetParent(row.transform);
                sphere.transform.position = parent.position + localBase + new Vector3(i * 2.2f, 1.1f, 0f);
                sphere.transform.localScale = Vector3.one * 0.9f;
                var r = sphere.GetComponent<Renderer>();
                r.sharedMaterial = mat;
                result[i] = r;
            }
            return result;
        }

        private static Transform Section(Transform parent, string name, Vector3 position)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent);
            go.transform.position = position;
            return go.transform;
        }

        private static GameObject CreateSphere(Transform parent, string name, Vector3 localPosition, Material mat, float scale = 1f)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.name = name;
            go.transform.SetParent(parent);
            go.transform.position = parent.position + localPosition;
            go.transform.localScale = Vector3.one * scale;
            go.GetComponent<Renderer>().sharedMaterial = mat;
            return go;
        }

        private static void CreateHeader(Transform parent, string text, Vector3 localPosition)
        {
            CreateLabel(parent, text, localPosition, 0.45f);
        }

        private static void CreateLabel(Transform parent, string text, Vector3 localPosition, float characterSize)
        {
            var go = new GameObject("Label_" + text.Replace("\n", "_"));
            go.transform.SetParent(parent);
            go.transform.position = parent.position + localPosition;
            go.transform.rotation = Quaternion.Euler(90f, 0f, 0f);

            var tm = go.AddComponent<TextMesh>();
            tm.text = text;
            tm.characterSize = characterSize;
            tm.fontSize = 48;
            tm.anchor = TextAnchor.MiddleCenter;
            tm.alignment = TextAlignment.Center;
            tm.color = Color.white;
        }

        private static Material GetOrCreateMaterial(string name, Shader shader)
        {
            string path = $"{Materials}/{name}.mat";
            var mat = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (mat == null)
            {
                mat = new Material(shader) { name = name };
                AssetDatabase.CreateAsset(mat, path);
            }
            else
            {
                mat.shader = shader;
            }

            EditorUtility.SetDirty(mat);
            return mat;
        }

        private static void CopyPbr(Material from, Material to)
        {
            to.SetColor("_BaseColor", from.GetColor("_BaseColor"));
            to.SetFloat("_Metallic", from.GetFloat("_Metallic"));
            to.SetFloat("_Smoothness", from.GetFloat("_Smoothness"));
        }

        private static void CreateProceduralTextures()
        {
            string normalPath = Textures + "/T_CheckerNormal.png";
            string aoPath = Textures + "/T_CheckerAO.png";

            WriteNormalPng(normalPath);
            WriteAOPng(aoPath);
            AssetDatabase.Refresh();

            var normalImporter = AssetImporter.GetAtPath(normalPath) as TextureImporter;
            if (normalImporter != null)
            {
                normalImporter.textureType = TextureImporterType.NormalMap;
                normalImporter.wrapMode = TextureWrapMode.Repeat;
                normalImporter.SaveAndReimport();
            }

            var aoImporter = AssetImporter.GetAtPath(aoPath) as TextureImporter;
            if (aoImporter != null)
            {
                aoImporter.textureType = TextureImporterType.Default;
                aoImporter.sRGBTexture = false;
                aoImporter.wrapMode = TextureWrapMode.Repeat;
                aoImporter.SaveAndReimport();
            }
        }

        private static void WriteNormalPng(string assetPath)
        {
            int size = 128;
            var tex = new Texture2D(size, size, TextureFormat.RGBA32, false, true);
            for (int y = 0; y < size; y++)
            for (int x = 0; x < size; x++)
            {
                float fx = x / 8f;
                float fy = y / 8f;
                float h = (Mathf.Sin(fx) + Mathf.Cos(fy)) * 0.5f;

                Vector3 n = new Vector3(
                    -Mathf.Cos(fx) * 0.6f,
                    Mathf.Sin(fy) * 0.6f,
                    1f).normalized;

                tex.SetPixel(x, y, new Color(
                    n.x * 0.5f + 0.5f,
                    n.y * 0.5f + 0.5f,
                    n.z * 0.5f + 0.5f,
                    1f));
            }
            tex.Apply();
            File.WriteAllBytes(assetPath, tex.EncodeToPNG());
            Object.DestroyImmediate(tex);
        }

        private static void WriteAOPng(string assetPath)
        {
            int size = 128;
            var tex = new Texture2D(size, size, TextureFormat.RGBA32, false, true);
            for (int y = 0; y < size; y++)
            for (int x = 0; x < size; x++)
            {
                int cx = (x / 16) % 2;
                int cy = (y / 16) % 2;
                float v = (cx == cy) ? 0.25f : 1f;
                tex.SetPixel(x, y, new Color(v, v, v, 1f));
            }
            tex.Apply();
            File.WriteAllBytes(assetPath, tex.EncodeToPNG());
            Object.DestroyImmediate(tex);
        }

        private static void EnsureFolder(string parent, string name)
        {
            string path = parent + "/" + name;
            if (!AssetDatabase.IsValidFolder(path))
                AssetDatabase.CreateFolder(parent, name);
        }
    }
}
#endif
