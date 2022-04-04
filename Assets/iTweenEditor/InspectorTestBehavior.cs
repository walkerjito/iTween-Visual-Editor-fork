using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InspectorTestBehavior : MonoBehaviour
{
    public List<int> mIntLists;

    public void Awake()
    {
        mIntLists = new List<int>() { 0,0};
    }
}
