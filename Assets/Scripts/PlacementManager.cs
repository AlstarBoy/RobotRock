using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Handles placing celestial objects on the grid.
/// </summary>
public class PlacementManager : MonoBehaviour
{
    public string currentObjectTag = "Tier1"; // Set this based on current object type
    public Camera mainCamera;
    public LayerMask tileLayer;

    [SerializeField]
    private bool isPlacing = true;

    [SerializeField] private PlayerInput _playerInput;
    private InputAction _placeAction;

    private void Awake()
    {
        // Retrieve the 'Point' action from the PlayerInput's action map
        _placeAction = _playerInput.actions["Place"];
    }

    private void Update()
    {
        if (!isPlacing || TurnManager.Instance.remainingMoves <= 0)
            return;

        if (_placeAction.WasPerformedThisFrame())
        {
            TryPlaceObject();
        }
    }

    void TryPlaceObject()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, tileLayer))
        {
            Tile tile = hit.collider.GetComponent<Tile>();
            if (tile != null && !tile.IsOccupied)
            {
                PlaceObjectOnTile(tile);
            }
        }
    }

    void PlaceObjectOnTile(Tile tile)
    {
        // Spawn from pool
        GameObject obj = ObjectPooler.Instance.SpawnFromPool(currentObjectTag, tile.transform.position, Quaternion.identity);
        CelestialObject celestial = obj.GetComponent<CelestialObject>();
        tile.SetObject(celestial);

        // Handle merging
        MergeManager.Instance.CheckForMerge(celestial);

        // Subtract a move
        TurnManager.Instance.MakeMove();

        // Optionally: check if moves = 0 and end turn
        if (TurnManager.Instance.remainingMoves <= 0)
        {
            Debug.Log("No moves left!");
            isPlacing = false;
            // Trigger end turn / failure UI, etc.
        }
    }
}

