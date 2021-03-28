using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class WaypointManager : MonoBehaviour
{
    public List<GameObject> wayPoints;
    public bool drawWayPoint;
    public Color wayPointColor;

    void Awake()
    {
        wayPoints = new List<GameObject>();
        foreach (Transform node in transform)
        {
            wayPoints.Add(node.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {            
        
        if (wayPoints != null && wayPoints.Count > 1)
        {
            if (drawWayPoint)
            {
                DrawLineBetweenPoints();
            }
        }
    }
    private void DrawLineBetweenPoints()
    {
        //Draw the line around the node loop
        int index = 0;
        Vector3 lastPos = wayPoints[index].transform.position;
        for (int i = 0; i < wayPoints.Count; i++)
        {
            Debug.DrawLine(lastPos, wayPoints[index].transform.position, wayPointColor);
            lastPos = wayPoints[index].transform.position;
            if (index == wayPoints.Count - 1)
            {
                Debug.DrawLine(lastPos, wayPoints[0].transform.position, wayPointColor);
            }
            index++;
        }
    }
}
