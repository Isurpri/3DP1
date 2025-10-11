using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class ResetGallery : MonoBehaviour
{
    public List<GameObject> m_Listargets = new List<GameObject>();
    private GameObject[] targets;

    private void Start()
    {
        targets = GameObject.FindGameObjectsWithTag("Target");

        m_Listargets.AddRange(targets);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            for (int i = 0; i < m_Listargets.Count; i++)
            {
                m_Listargets[i].SetActive(true);
            }
        }
    }
}
