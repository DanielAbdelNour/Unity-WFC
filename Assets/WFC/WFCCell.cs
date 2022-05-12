using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace WFC
{
    public class WFCCell : MonoBehaviour
    {
        public List<WFCModule> candidates;
        public WFCModule collapsedModule;

        public void OnEnable()
        {
            
        }

        public void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireCube(transform.position, new Vector3(2,2,2));
        }

        public bool IsCollapsed()
        {
            return collapsedModule != null;
        }
    }

}