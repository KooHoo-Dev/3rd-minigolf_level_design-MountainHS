using System;
using UnityEngine;

namespace Study3D.SolarSystem
{
    public class OrbitPathDrawer : MonoBehaviour
    {
        // 원 궤도내 점 개수, 중심점, 반지름만 주면 점들 좌표가 좌라락 나와야함
        // 축의 단위는 그대로인데 법선 벡터만 회전하면 평면 방정식이 어떻게 변했지? 법선 벡터 변화정보로는 부족하고 어떤 선이 겹치냐 까지도 줘야했었나?
        // 만약 근데 평면이 달라지면 어떻게 하지?
        [Header("Settings")] public Transform orbitingChild;
        public int segment = 96;
        public float width = 0.05f;

        private void Start()
        {
            Draw();
        }

        [ContextMenu("OrbitPathDraw")]
        private void Draw()
        {
            if (orbitingChild == null) return;
            LineRenderer lr = GetComponent<LineRenderer>();
            
            Vector3 local = orbitingChild.localPosition;
            float radius = new Vector2(local.x, local.z).magnitude;
            int segmentTotalCount = Mathf.Max(4, segment);

            for (int i = 0; i < segmentTotalCount; i++)
            {
                float angle = i * Mathf.PI * 2f / segmentTotalCount;
                Vector3 pos = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)) * radius;
                lr.SetPosition(i, pos);
            }
            
            // 월드 좌표가 아닌 로컬 좌표로 변환
            lr.useWorldSpace = false;
            // 시작과 끝점 잇기
            lr.loop = true;
            // 선의 너비
            lr.widthMultiplier = width;
        }
    }
}