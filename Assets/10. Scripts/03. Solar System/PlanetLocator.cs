using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Camera))]
public class PlanetLocator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform[] planets;
    [SerializeField] private GameObject markerPrefab;

    [Header("Marker")]
    [SerializeField] private float markerDepth = 10f;

    [Header("Teleport")]
    [SerializeField] private float distanceMultiplier = 3f;
    [SerializeField] private float minimumDistance = 5f;

    private Camera cam;
    private GameObject[] markers;

    private static readonly Key[] PlanetKeys =
    {
        Key.Digit1, Key.Digit2, Key.Digit3,
        Key.Digit4, Key.Digit5, Key.Digit6,
        Key.Digit7, Key.Digit8, Key.Digit9
    };

    private void Awake()
    {
        cam = GetComponent<Camera>();

        markers = new GameObject[planets.Length];

        for (int i = 0; i < planets.Length; i++)
        {
            if (planets[i] == null || markerPrefab == null)
                continue;

            markers[i] = Instantiate(markerPrefab);
            markers[i].name = $"{planets[i].name}_Marker";
            markers[i].SetActive(false);
        }
    }

    private void Update()
    {
        if (Keyboard.current == null)
            return;

        int count = Mathf.Min(planets.Length, PlanetKeys.Length);

        for (int i = 0; i < count; i++)
        {
            if (Keyboard.current[PlanetKeys[i]].wasPressedThisFrame)
            {
                TeleportToPlanet(i);
                break;
            }
        }
    }

    private void LateUpdate()
    {
        UpdateMarkers();
    }

    private void UpdateMarkers()
    {
        float depth = Mathf.Clamp(
            markerDepth,
            cam.nearClipPlane + 0.01f,
            cam.farClipPlane - 0.01f
        );

        for (int i = 0; i < planets.Length; i++)
        {
            if (planets[i] == null || markers[i] == null)
                continue;

            Vector3 viewport =
                cam.WorldToViewportPoint(planets[i].position);

            bool visible =
                viewport.z > 0f &&
                viewport.x >= 0f && viewport.x <= 1f &&
                viewport.y >= 0f && viewport.y <= 1f;

            markers[i].SetActive(visible);

            if (!visible)
                continue;

            markers[i].transform.position =
                cam.ViewportToWorldPoint(
                    new Vector3(viewport.x, viewport.y, depth)
                );
        }
    }

    private void TeleportToPlanet(int index)
    {
        Transform planet = planets[index];

        if (planet == null)
            return;

        float distance = minimumDistance;

        Renderer renderer = planet.GetComponentInChildren<Renderer>();

        if (renderer != null)
        {
            float radius = renderer.bounds.extents.magnitude;

            distance = Mathf.Max(
                radius * distanceMultiplier,
                minimumDistance
            );
        }

        transform.position =
            planet.position - transform.forward * distance;
    }
}