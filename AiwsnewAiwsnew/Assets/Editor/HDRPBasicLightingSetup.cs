#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

/// <summary>
/// HDRP 基础实时光照一键配置工具 (Unity 6 / HDRP 17.x)
/// 
/// 适配 Unity 6 新 API：
/// - 通过 Light.type 设置灯光类型（不使用已废弃的 AddHDLight / SetLightTypeAndShape）
/// - AddComponent<Light> 后 HDRP 自动挂载 HDAdditionalLightData
/// - 通过 HDAdditionalLightData 控制 HDRP 专属参数
/// 
/// 生成 [HDRP Lighting Rig] → Sun Light + Sky & Fog Volume
/// 支持 UI 撤回按钮 / Ctrl+Z，生成后也可在 Inspector 里直接调参
/// 
/// 菜单：Tools → Lighting → Setup Basic HDRP Lighting
/// </summary>
public class HDRPBasicLightingSetup : EditorWindow
{
    // ───────── 太阳光参数 ─────────
    private float sunIntensity = 10000f;
    private Color sunColor = new Color(1f, 0.95f, 0.84f);
    private Vector3 sunRotation = new Vector3(50f, -30f, 0f);
    private int shadowResolution = 2048;

    // ───────── Sky 参数 ─────────
    private bool usePhysicalSky = true;
    private float skyExposure = 0f;

    // ───────── 间接光 ─────────
    private float indirectDiffuse = 1f;
    private float indirectSpecular = 1f;

    // ───────── 阴影 ─────────
    private float shadowDistance = 150f;
    private bool contactShadow = true;

    // ───────── 雾 ─────────
    private bool enableFog = true;
    private float fogDistance = 200f;
    private float fogMaxHeight = 50f;

    // ───────── 内部 ─────────
    private const string RIG_NAME = "[HDRP Lighting Rig]";
    private const string SUN_NAME = "Sun Light";
    private const string VOL_NAME = "Sky & Fog Volume";
    private Vector2 scrollPos;
    private string lastAction = "";

    [MenuItem("Tools/Lighting/Setup Basic HDRP Lighting")]
    public static void ShowWindow()
    {
        var win = GetWindow<HDRPBasicLightingSetup>("HDRP Basic Lighting");
        win.minSize = new Vector2(380, 580);
    }

    // =====================================================================
    //  GUI
    // =====================================================================
    private void OnGUI()
    {
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        EditorGUILayout.LabelField("HDRP 基础实时光照配置", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox(
            "一键生成 [HDRP Lighting Rig]，包含太阳光与天空雾效。\n" +
            "修改参数后点「更新配置」即时生效，无需重建。\n" +
            "生成后也可以直接在 Inspector 里手动调整 Sun Light。\n" +
            "所有操作支持「↩ 撤回」按钮或 Ctrl+Z。", MessageType.Info);
        EditorGUILayout.Space(6);

        // ── Sun ──
        EditorGUILayout.LabelField("☀  Directional Light (太阳)", EditorStyles.boldLabel);
        sunIntensity = EditorGUILayout.FloatField("强度 (Lux)", sunIntensity);
        sunColor = EditorGUILayout.ColorField("颜色", sunColor);
        sunRotation = EditorGUILayout.Vector3Field("旋转角度", sunRotation);
        shadowResolution = EditorGUILayout.IntPopup("阴影分辨率", shadowResolution,
            new[] { "512", "1024", "2048", "4096" },
            new[] { 512, 1024, 2048, 4096 });

        EditorGUILayout.Space(10);

        // ── Sky ──
        EditorGUILayout.LabelField("🌤  Sky & Exposure", EditorStyles.boldLabel);
        usePhysicalSky = EditorGUILayout.Toggle("Physically Based Sky", usePhysicalSky);
        skyExposure = EditorGUILayout.Slider("曝光补偿 (EV)", skyExposure, -5f, 5f);

        EditorGUILayout.Space(10);

        // ── Indirect ──
        EditorGUILayout.LabelField("💡  间接光", EditorStyles.boldLabel);
        indirectDiffuse = EditorGUILayout.Slider("漫反射倍率", indirectDiffuse, 0f, 3f);
        indirectSpecular = EditorGUILayout.Slider("镜面反射倍率", indirectSpecular, 0f, 3f);

        EditorGUILayout.Space(10);

        // ── Shadow ──
        EditorGUILayout.LabelField("🔲  阴影", EditorStyles.boldLabel);
        shadowDistance = EditorGUILayout.FloatField("最大阴影距离", shadowDistance);
        contactShadow = EditorGUILayout.Toggle("接触阴影 (Contact Shadows)", contactShadow);

        EditorGUILayout.Space(10);

        // ── Fog ──
        EditorGUILayout.LabelField("🌫  Fog 雾效", EditorStyles.boldLabel);
        enableFog = EditorGUILayout.Toggle("启用", enableFog);
        using (new EditorGUI.DisabledGroupScope(!enableFog))
        {
            fogDistance = EditorGUILayout.FloatField("雾平均距离", fogDistance);
            fogMaxHeight = EditorGUILayout.FloatField("雾最大高度", fogMaxHeight);
        }

        EditorGUILayout.Space(16);

        // ══════════════════════════════════════════════════════
        //  操作按钮
        // ══════════════════════════════════════════════════════
        bool rigExists = GameObject.Find(RIG_NAME) != null;

        using (new EditorGUILayout.HorizontalScope())
        {
            GUI.backgroundColor = new Color(0.3f, 0.9f, 0.4f);
            if (GUILayout.Button(rigExists ? "🔄 更新配置" : "✅ 一键生成", GUILayout.Height(36)))
                Execute();

            GUI.backgroundColor = new Color(1f, 0.45f, 0.4f);
            using (new EditorGUI.DisabledGroupScope(!rigExists))
            {
                if (GUILayout.Button("🗑 删除 Rig", GUILayout.Height(36)))
                    CleanUp();
            }
            GUI.backgroundColor = Color.white;
        }

        EditorGUILayout.Space(4);

        // 撤回按钮
        bool hasUndo = !string.IsNullOrEmpty(lastAction);
        GUI.backgroundColor = new Color(0.85f, 0.85f, 1f);
        using (new EditorGUI.DisabledGroupScope(!hasUndo))
        {
            string undoBtnText = hasUndo
                ? $"↩ 撤回：{lastAction}"
                : "↩ 撤回（无操作可撤回）";

            if (GUILayout.Button(undoBtnText, GUILayout.Height(30)))
            {
                Undo.PerformUndo();
                lastAction = "";
                Repaint();
            }
        }
        GUI.backgroundColor = Color.white;

        // 快捷选中按钮
        if (rigExists)
        {
            EditorGUILayout.Space(4);
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("选中 Sun Light"))
                {
                    var rig = GameObject.Find(RIG_NAME);
                    var sun = rig?.transform.Find(SUN_NAME);
                    if (sun != null) Selection.activeGameObject = sun.gameObject;
                }
                if (GUILayout.Button("选中 Volume"))
                {
                    var rig = GameObject.Find(RIG_NAME);
                    var vol = rig?.transform.Find(VOL_NAME);
                    if (vol != null) Selection.activeGameObject = vol.gameObject;
                }
            }
            EditorGUILayout.HelpBox(
                "提示：生成后可以直接在 Inspector 里选中 Sun Light 手动调参，\n" +
                "或者在此窗口修改后点「更新配置」。两种方式都可以。", MessageType.None);
        }

        EditorGUILayout.EndScrollView();
    }

    // =====================================================================
    //  执行
    // =====================================================================
    private void Execute()
    {
        Undo.IncrementCurrentGroup();
        Undo.SetCurrentGroupName("Setup HDRP Lighting Rig");
        int undoGroup = Undo.GetCurrentGroup();

        // ====== Root ======
        GameObject rig = GameObject.Find(RIG_NAME);
        bool isNew = rig == null;
        if (isNew)
        {
            rig = new GameObject(RIG_NAME);
            Undo.RegisterCreatedObjectUndo(rig, "Create Lighting Rig");
        }
        rig.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);

        // ====== Sun Light ======
        SetupSunLight(rig.transform);

        // ====== Sky & Fog Volume ======
        SetupSkyFogVolume(rig.transform);

        Undo.CollapseUndoOperations(undoGroup);

        Selection.activeGameObject = rig;
        EditorGUIUtility.PingObject(rig);

        lastAction = isNew ? "生成 Lighting Rig" : "更新 Lighting Rig";
        Debug.Log($"[HDRP Lighting] ✅ {RIG_NAME} 已{(isNew ? "生成" : "更新")}");
    }

    // -----------------------------------------------------------------
    //  太阳光 — Unity 6 HDRP 17.x 方式
    //  AddComponent<Light> → HDRP 自动挂 HDAdditionalLightData
    //  通过 Light.type = Directional 设置类型（不用废弃的 AddHDLight）
    // -----------------------------------------------------------------
    private void SetupSunLight(Transform parent)
    {
        Transform existing = parent.Find(SUN_NAME);
        GameObject sunGO;
        Light light;
        HDAdditionalLightData hdLight;

        if (existing != null)
        {
            sunGO = existing.gameObject;
            light = sunGO.GetComponent<Light>();
            hdLight = sunGO.GetComponent<HDAdditionalLightData>();

            if (light == null || hdLight == null)
            {
                // 组件残缺，重建
                Undo.DestroyObjectImmediate(sunGO);
                sunGO = CreateSunGameObject(parent, out light, out hdLight);
            }
            else
            {
                Undo.RecordObject(sunGO.transform, "Update Sun Transform");
                Undo.RecordObject(light, "Update Light");
                Undo.RecordObject(hdLight, "Update HD Light");
            }
        }
        else
        {
            sunGO = CreateSunGameObject(parent, out light, out hdLight);
        }

        // --- Transform ---
        sunGO.transform.localPosition = Vector3.zero;
        sunGO.transform.localRotation = Quaternion.Euler(sunRotation);

        // --- Light 组件（Unity 6 中类型直接在 Light 上设置） ---
        light.type = LightType.Directional;
        light.color = sunColor;
        light.shadows = LightShadows.Soft;

        // --- HDAdditionalLightData（HDRP 专属参数） ---
        hdLight.SetIntensity(sunIntensity, LightUnit.Lux);
        hdLight.SetColor(sunColor);
        hdLight.EnableShadows(true);
        hdLight.SetShadowResolution(shadowResolution);

        // 太阳角直径（在 Inspector 里对应 "Angular Diameter"）
        hdLight.angularDiameter = 0.53f;
        // interactsWithSky 控制是否影响 Physically Based Sky
        hdLight.interactsWithSky = true;

        // 标脏确保即时刷新
        EditorUtility.SetDirty(light);
        EditorUtility.SetDirty(hdLight);
        EditorUtility.SetDirty(sunGO);
        SceneView.RepaintAll();
    }

    private GameObject CreateSunGameObject(Transform parent, out Light light, out HDAdditionalLightData hdLight)
    {
        GameObject sunGO = new GameObject(SUN_NAME);
        Undo.RegisterCreatedObjectUndo(sunGO, "Create Sun Light");
        sunGO.transform.SetParent(parent, false);

        // Unity 6 HDRP: AddComponent<Light> 后 HDRP 自动附加 HDAdditionalLightData
        light = sunGO.AddComponent<Light>();
        light.type = LightType.Directional;

        // HDRP 在 AddComponent<Light> 时自动挂载 HDAdditionalLightData
        // 但为了安全，等一帧后获取，这里先强制获取
        hdLight = sunGO.GetComponent<HDAdditionalLightData>();
        if (hdLight == null)
        {
            // 如果 HDRP 没有自动挂载（极少数情况），手动添加并初始化
            hdLight = sunGO.AddComponent<HDAdditionalLightData>();
            HDAdditionalLightData.InitDefaultHDAdditionalLightData(hdLight);
        }

        return sunGO;
    }

    // -----------------------------------------------------------------
    //  天空 & 雾效 Volume
    // -----------------------------------------------------------------
    private void SetupSkyFogVolume(Transform parent)
    {
        Transform existing = parent.Find(VOL_NAME);
        GameObject volGO;

        if (existing != null)
        {
            volGO = existing.gameObject;
        }
        else
        {
            volGO = new GameObject(VOL_NAME);
            Undo.RegisterCreatedObjectUndo(volGO, "Create Sky Fog Volume");
            volGO.transform.SetParent(parent, false);
        }

        // Volume
        Volume volume = volGO.GetComponent<Volume>();
        if (volume == null) volume = Undo.AddComponent<Volume>(volGO);
        Undo.RecordObject(volume, "Configure Volume");
        volume.isGlobal = true;
        volume.priority = 1;

        // Runtime VolumeProfile
        VolumeProfile profile = ScriptableObject.CreateInstance<VolumeProfile>();
        profile.name = "HDRP_BasicLighting_RuntimeProfile";

        // ▸ Visual Environment
        var visualEnv = profile.Add<VisualEnvironment>(true);
        visualEnv.skyType.Override(usePhysicalSky
            ? (int)SkyType.PhysicallyBased
            : (int)SkyType.HDRI);
        visualEnv.skyAmbientMode.Override(SkyAmbientMode.Dynamic);

        // ▸ Physically Based Sky
        if (usePhysicalSky)
        {
            var pbSky = profile.Add<PhysicallyBasedSky>(true);
            pbSky.active = true;
        }

        // ▸ Exposure
        var exposure = profile.Add<Exposure>(true);
        exposure.mode.Override(ExposureMode.Fixed);
        exposure.fixedExposure.Override(skyExposure);

        // ▸ Indirect Lighting Controller
        var indirect = profile.Add<IndirectLightingController>(true);
        indirect.indirectDiffuseLightingMultiplier.Override(indirectDiffuse);
        indirect.reflectionLightingMultiplier.Override(indirectSpecular);

        // ▸ HD Shadow Settings
        var shadowSettings = profile.Add<HDShadowSettings>(true);
        shadowSettings.maxShadowDistance.Override(shadowDistance);

        // ▸ Contact Shadows
        if (contactShadow)
        {
            var cs = profile.Add<ContactShadows>(true);
            cs.enable.Override(true);
            cs.length.Override(0.15f);
            cs.distanceScaleFactor.Override(0.5f);
            cs.maxDistance.Override(50f);
        }

        // ▸ Fog
        var fog = profile.Add<Fog>(true);
        fog.enabled.Override(enableFog);
        if (enableFog)
        {
            fog.meanFreePath.Override(fogDistance);
            fog.baseHeight.Override(0f);
            fog.maximumHeight.Override(fogMaxHeight);
        }

        volume.sharedProfile = profile;

        EditorUtility.SetDirty(volume);
        EditorUtility.SetDirty(volGO);
    }

    // =====================================================================
    //  清理
    // =====================================================================
    private void CleanUp()
    {
        GameObject rig = GameObject.Find(RIG_NAME);
        if (rig != null)
        {
            Undo.IncrementCurrentGroup();
            Undo.SetCurrentGroupName("Delete HDRP Lighting Rig");
            Undo.DestroyObjectImmediate(rig);
            lastAction = "删除 Lighting Rig";
            Debug.Log($"[HDRP Lighting] 🗑 {RIG_NAME} 已删除");
        }
        else
        {
            Debug.Log("[HDRP Lighting] 场景中没有找到 Lighting Rig");
        }
    }
}
#endif