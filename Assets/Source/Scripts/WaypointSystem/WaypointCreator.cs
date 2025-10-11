using Assets.Source.Game.Scripts.Enums;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Game.Scripts.WaypointSystem
{
    public class WaypointCreator : MonoBehaviour
    {
        [SerializeField] private TypeEnemy _typeEnemy;
        [SerializeField] string _waypointName = "Waypoint";

        private List<Waypoint> _waypoints;

        public void CreateWaypoint()
        {
            string nameWaypoint = _waypointName + "_" + _typeEnemy.ToString();
            GameObject waypointObj = new(nameWaypoint);
            Waypoint waypoint = waypointObj.AddComponent<Waypoint>();
            waypointObj.transform.position = transform.position;
            waypointObj.transform.SetParent(transform, worldPositionStays: true);
            waypoint.TypeEnemy = _typeEnemy;
            _waypoints.Add(waypoint);
        }

        public List<Waypoint> GetWaypointsByType(TypeEnemy typeEnemy)
        {
            return _waypoints;
        }
    }
}