using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using LogManager = Assets.Scripts.MapEditor.Log.LogManager;

namespace Assets.Scripts.MapEditor
{
    [Serializable]
    public class SaveData
    {
        public string name;
        public DateTime date;
        public List<NodeJsonData> nodeJsonData;
        public List<LineJsonData> lineJsonData;
        public EtcJsonData etcJsonData;
    }
    
    [Serializable]
    public class NodeJsonData
    {
        public string id;
        public float[] position = new float[2];
        public NodeData nodeData;
    }

    [Serializable]
    public class LineJsonData
    {
        public string id;
        public readonly List<string> connectedNode = new List<string>();
    }

    [Serializable]
    public class EtcJsonData
    {
        public int nodeCount;
        public int lineCount;
    }
    
    public class SaveManager
    {
        #region Singleton

        private static SaveManager _instance;

        public static SaveManager GetInstance()
        {
            if (_instance == null)
            {
                _instance = new SaveManager();
            }

            return _instance;
        }
        #endregion

        private SaveData recentSaveData = null;

        public void SetRecentSaveData(SaveData saveData)
        {
            recentSaveData = saveData;
            UIButtonManager.GetInstance().EnableUIButton(UIButton.QSave);
        }
        
        public void QSave()
        {
            if (recentSaveData == null)
            {
                LogManager.GetInstance().LogError("Can't Quick Save Now");
                return;
            }

            recentSaveData.nodeJsonData = SaveNode();
            recentSaveData.lineJsonData = SaveLine();
            recentSaveData.etcJsonData = SaveEtcData();
            
            string jsonData = JsonConvert.SerializeObject(recentSaveData,Formatting.Indented);
            string path = Application.streamingAssetsPath + "/Save/"+ recentSaveData.name +".json";

            File.WriteAllText(path,jsonData);
            LogManager.GetInstance().Log("Data Saved as " + recentSaveData.name + ".json");
        }
        
        public void Save(string nameValue, Action callBackFunc)
        {
            DirectoryInfo di = new DirectoryInfo(Application.streamingAssetsPath + "/Save/");

            foreach (var file in di.GetFiles())
            {
                if (file.Name == nameValue + ".json")
                {
                    LogManager.GetInstance().LogError(""+file.Name+"   already exist");
                    return;
                }
            }
            
            SaveData saveData = new SaveData
            {
                nodeJsonData = SaveNode(),
                lineJsonData = SaveLine(),
                etcJsonData = SaveEtcData(),
                name = nameValue,
                date = DateTime.Today
            };

            recentSaveData = saveData;
            
            UIButtonManager.GetInstance().EnableUIButton(UIButton.QSave);

            QSave();
            callBackFunc();
            UIButtonManager.GetInstance().EnableUIButton(UIButton.QSave);
        }
        
        private List<NodeJsonData> SaveNode()
        {
            List<NodeJsonData> nodeJsonDataList = new List<NodeJsonData>();
            List<Node> nodeList = NodeManager.GetInstance().GetAllNode();
            foreach (var node in nodeList)
            {
                NodeJsonData newNodeJsonData = new NodeJsonData
                {
                    id = node.id
                };
                var position = node.gameObject.transform.position;
                newNodeJsonData.position[0] = position.x;
                newNodeJsonData.position[1] = position.y;
                newNodeJsonData.nodeData = node.data;
                nodeJsonDataList.Add(newNodeJsonData);
            }

            return nodeJsonDataList;
        }

        private List<LineJsonData> SaveLine()
        {
            List<LineJsonData> lineJsonDataList = new List<LineJsonData>();
            List<Line> lineList = LineManager.GetInstance().GetAllLine();
            foreach (var line in lineList)
            {
                LineJsonData lineJsonData = new LineJsonData();
                lineJsonData.id = line.lineId;
                lineJsonData.connectedNode.Add(line.nodeList[0].id);
                lineJsonData.connectedNode.Add(line.nodeList[1].id);
                lineJsonDataList.Add(lineJsonData);
            }

            return lineJsonDataList;
        }

        private EtcJsonData SaveEtcData()
        {
            EtcJsonData newEtcJsonData = new EtcJsonData
            {
                lineCount = LineManager.GetInstance().lineCount,
                nodeCount = NodeManager.GetInstance().nodeCount
            };

            return newEtcJsonData;
        }
    }
}
