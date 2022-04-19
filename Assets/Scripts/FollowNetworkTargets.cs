using System.Collections.Generic;
using UnityEngine;

public class FollowNetworkTargets : MonoBehaviour
{
    private List<Transform> m_Targets;

    public void Awake()
    {
        m_Targets = new List<Transform>();
    }

    public void AddToTargets(Transform target)
    {
        m_Targets.Add(target);
    }

    public void RemoveFromTargets(Transform target)
    {
        m_Targets.Remove(target);
    }

    public Transform[] GetTargetsArray()
    {
        return m_Targets.ToArray();
    }

    public void ClearTargets()
    {
        m_Targets.Clear();
    }
}