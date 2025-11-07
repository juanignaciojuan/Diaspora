using UnityEngine;
using System.Collections;

/// <summary>
/// Controls a drone's behavior, including movement along waypoints and dropping bombs.
/// </summary>
public class DroneController : MonoBehaviour
{
    [Header("Bomb Dropping")]
    [Tooltip("The bomb prefab to be spawned and dropped.")]
    public GameObject bombPrefab;

    [Tooltip("The point from which the bomb is dropped.")]
    public Transform bombSpawnPoint;

    [Tooltip("The time in seconds between each bomb drop.")]
    public float dropInterval = 10f;

    [Header("Movement")]
    [Tooltip("The list of waypoints for the drone to follow.")]
    public Transform[] waypoints;

    [Tooltip("The movement speed of the drone.")]
    public float moveSpeed = 5f;

    private int currentWaypointIndex = 0;
    private bool isDroppingBomb = false;

    private void Start()
    {
        // Start the bomb dropping cycle.
        if (bombPrefab != null && bombSpawnPoint != null)
        {
            InvokeRepeating(nameof(DropBomb), dropInterval, dropInterval);
        }
    }

    private void Update()
    {
        // Handle movement if waypoints are assigned.
        if (waypoints != null && waypoints.Length > 0)
        {
            MoveToNextWaypoint();
        }
    }

    private void MoveToNextWaypoint()
    {
        // If there are no waypoints, do nothing.
        if (waypoints.Length == 0) return;

        // Get the current waypoint.
        Transform targetWaypoint = waypoints[currentWaypointIndex];

        // Move the drone towards the waypoint.
        transform.position = Vector3.MoveTowards(transform.position, targetWaypoint.position, moveSpeed * Time.deltaTime);

        // Look at the waypoint.
        transform.LookAt(targetWaypoint.position);

        // Check if the drone has reached the waypoint.
        if (Vector3.Distance(transform.position, targetWaypoint.position) < 0.1f)
        {
            // Move to the next waypoint in the list.
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        }
    }

    /// <summary>
    /// Spawns and drops a bomb.
    /// </summary>
    private void DropBomb()
    {
        // Instantiate the bomb at the spawn point.
        // The bomb's own Rigidbody and XRBomb script will handle the rest.
        Instantiate(bombPrefab, bombSpawnPoint.position, bombSpawnPoint.rotation);
    }
}
