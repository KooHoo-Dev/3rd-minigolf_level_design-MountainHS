using UnityEngine;

public class OrbitalSystem : MonoBehaviour
{
    [SerializeField] private Transform[] orbits;
    [SerializeField] private float[] periods =
    {
        0.241f,   // Mercury
        0.615f,   // Venus
        1.000f,   // Earth
        1.881f,   // Mars
        11.862f,  // Jupiter
        29.457f,  // Saturn
        84.017f,  // Uranus
        164.790f, // Neptune
        247.940f  // Pluto
    };
    [SerializeField] private float[] diameters =
    {
        0.383f,   // Mercury
        0.949f,   // Venus
        1.000f,   // Earth
        0.532f,   // Mars
        11.209f,  // Jupiter
        9.450f,   // Saturn
        4.007f,   // Uranus
        3.883f,   // Neptune
        0.186f    // Pluto
    }; 

    [SerializeField] private Transform earthOrbit;
    [SerializeField] private Vector3 earthPlanetBaseScale = Vector3.one; 
    [SerializeField] private float secondsPerEarthYear = 10f;
    [SerializeField] private float sunScale = 109.078f;

    [ContextMenu("Init Planets")]
    private void InitPlanets()
    {
        LineRenderer earthLine = earthOrbit.GetComponent<LineRenderer>();

        Vector3[] earthPositions = new Vector3[earthLine.positionCount];
        earthLine.GetPositions(earthPositions);

        Transform earthAnchor = earthOrbit.GetChild(0);
        Transform sunTransform = transform.GetChild(0);

        // 현재 지구 궤도의 크기를 제거하고 형태와 방향만 추출
        float referenceRadius = earthAnchor.localPosition.magnitude;
        Vector3 earthDirection = earthAnchor.localPosition.normalized;

        // 태양 지름을 기준으로 지구 공전 반지름 설정
        float earthOrbitRadius = sunScale * 107.5f;
        sunTransform.localScale = new Vector3(sunScale, sunScale, sunScale);
        
        for (int i = 0; i < orbits.Length; i++)
        {
            // 공전 주기 기반 궤도 반지름
            float orbitRadius = earthOrbitRadius * Mathf.Pow(periods[i], 2f / 3f);

            // LineRenderer 궤도 크기 변경
            LineRenderer line = orbits[i].GetComponent<LineRenderer>();

            Vector3[] positions = new Vector3[earthPositions.Length];

            for (int j = 0; j < positions.Length; j++)
            {
                positions[j] = earthPositions[j] / referenceRadius * orbitRadius;
            }

            line.positionCount = positions.Length;
            line.SetPositions(positions);

            // 행성 공전 반지름 변경
            Transform anchor = orbits[i].GetChild(0);
            anchor.localPosition = earthDirection * orbitRadius;

            // 행성 자체의 크기 변경
            Transform target = anchor.GetChild(0).GetChild(0);
            target.localScale = earthPlanetBaseScale * diameters[i];
        }
    }
    
    [ContextMenu("Print Orbital Period")]
    private void PrintOrbitalPeriod()
    {
        for (int i = 0; i < orbits.Length; i++)
        {
            float periodInSeconds = periods[i] * secondsPerEarthYear;
            float frequency = 1f / periodInSeconds;
            float degreesPerSecond = frequency * 360f;
            
            Debug.Log($"Orbital Period of {orbits[i].name}: {periodInSeconds} seconds, degrees per second: {degreesPerSecond}, degrees per frame: {degreesPerSecond * Time.deltaTime}");
        }
        
    }

    private void Update()
    {
        for (int i = 0; i < orbits.Length; i++)
        {
            float periodInSeconds = periods[i] * secondsPerEarthYear;
            float frequency = 1f / periodInSeconds;
            float degreesPerSecond = frequency * 360f;

            orbits[i].Rotate(
                Vector3.up,
                degreesPerSecond * Time.deltaTime,
                Space.Self
            );
            // 쿼터니안이여서 하는거 아닌가? 근데 모름
            // 그래서 난 Rotate랑 EulerAngle 누적으로 ㄱㄱ
        }
    }
} 