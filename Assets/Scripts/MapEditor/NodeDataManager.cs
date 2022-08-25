using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace Assets.Scripts.MapEditor
{
    [Serializable]
    public class NodeData
    {
        public string name = "";
        public string etc = "";
    }

    public class NodeDataManager
    {
        #region Singleton

        private static NodeDataManager _instance;

        public static NodeDataManager GetInstance()
        {
            if (_instance == null)
            {
                _instance = new NodeDataManager();
            }
            return _instance;
        }
    
        #endregion

        private List<NodeData> _nodeDataList = new();

    }
}

