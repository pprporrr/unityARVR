using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.AI;

[RequireComponent(typeof(ARRaycastManager))]
public class SurfaceCharacter : MonoBehaviour
{
    // Public variables
    public GameObject PlaceButton;
    public GameObject VFXParticle;
    public GameObject TargetIndicator;
    public Camera xrCamera;
    public GameObject PlayerComponent;
    public GameObject objectPool;
    public GameObject player;
    public GameObject loadCharacter;
    public Material chessboardMaterial;

    // Private variables
    private ARRaycastManager _arRaycastManager;
    private ARPlaneManager _arPlaneManager;
    private Pose placementPose;
    private bool canPlace = false;
    public bool alrPlaced = false;
    private List<ARRaycastHit> hits = new List<ARRaycastHit>();

    private void Awake()
    {
        // Initialize AR components
        _arRaycastManager = GetComponent<ARRaycastManager>();
        _arPlaneManager = GetComponent<ARPlaneManager>();
        TargetIndicator.SetActive(true);
        PlaceButton.SetActive(false);
    }

    // Reset method to reset the script state
    public void ResetState()
    {
        alrPlaced = false;
        canPlace = false;
        PlaceButton.SetActive(true);
        TargetIndicator.SetActive(true);
        _arPlaneManager.enabled = true;
    }

    void Update()
    {
        UpdatePlacementPose();
        UpdateTargetIndicator();
        if (_arRaycastManager.Raycast(placementPose.position, hits, TrackableType.PlaneWithinPolygon) && !alrPlaced)
        {
            PlaceButton.SetActive(true);
        }

        // Handle touch input to place object
        if (canPlace && (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
        {
            if (_arRaycastManager.Raycast(placementPose.position, hits, TrackableType.PlaneWithinPolygon))
            {
                SpawnScene();
            }
        }

        // Handle keyboard input to place object
        if (canPlace && Input.GetKeyDown(KeyCode.Space))
        {
            if (_arRaycastManager.Raycast(placementPose.position, hits, TrackableType.PlaneWithinPolygon))
            {
                SpawnScene();
            }
        }
    }

    // Method to spawn scene objects
    public void SpawnScene()
    {
        TargetIndicator.SetActive(false);
        PlaceButton.SetActive(false);
        if (canPlace && !alrPlaced)
        {
            _arPlaneManager.enabled = false;

            // Set material for detected planes
            foreach (var plane in _arPlaneManager.trackables)
            {
                if (plane.gameObject.activeSelf)
                {
                    Renderer renderer = plane.GetComponent<Renderer>();
                    if (renderer != null)
                    {
                        renderer.material = chessboardMaterial;
                    }
                }
            }

            // Instantiate visual effect
            GameObject vfxParticleInstance = Instantiate(VFXParticle, placementPose.position, Quaternion.Euler(90f, 0f, 0f));
            Destroy(vfxParticleInstance, 2f);

            // Set player position and activate
            if (PlayerComponent != null)
            {
                PlayerComponent.transform.position = placementPose.position + new Vector3(0f, 0.3f, 0f);
                PlayerComponent.SetActive(true);
            }

            // Activate object pool, character loading, and player objects
            TargetIndicator.SetActive(false);
            objectPool.SetActive(true);
            loadCharacter.SetActive(true);
            player.SetActive(true);
            alrPlaced = true;
            GameManager.instance.StartGameplay();
        }
    }

    // Method to update the placement pose based on AR raycasting
    void UpdatePlacementPose()
    {
        Vector3 screenCenter = xrCamera.ViewportToScreenPoint(new Vector3(0.5f, 0.3f, 0.0f));
        List<ARRaycastHit> hits = new List<ARRaycastHit>();
        _arRaycastManager.Raycast(screenCenter, hits, TrackableType.Planes);

        canPlace = hits.Count > 0;

        if (canPlace)
        {
            placementPose = hits[0].pose;
            var cameraForward = xrCamera.transform.forward;
            var cameraBearing = new Vector3(cameraForward.x, 0f, cameraForward.z).normalized;
            placementPose.rotation = Quaternion.LookRotation(cameraBearing) * Quaternion.Euler(0, 180, 0);
        }
    }

    // Method to update the target indicator position and rotation
    void UpdateTargetIndicator()
    {
        TargetIndicator.transform.SetPositionAndRotation(placementPose.position, placementPose.rotation);
    }
}