using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class Test: MonoBehaviour
{
    [ShowInInspector] public Dictionary<int, string> myDictionary = new Dictionary<int, string>();

    private void Start()
   {
       foreach (KeyValuePair<int, string> kvp in myDictionary)
       {
           Debug.Log("Key: " + kvp.Key + ", Value: " + kvp.Value);
       }
   }
}

