using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.MapEditor.Log
{
    public class LogManager : MonoBehaviour
    {
        #region singleTon

        private static LogManager _instance;

        public static LogManager GetInstance()
        {
            if (_instance == null)
            {
                var obj = FindObjectOfType<LogManager>();
                if (obj == null)
                {
                    Debug.LogError("Error! LogManager is not ready");
                    return null;
                }
                _instance = obj;
            }

            return _instance;
        }

        #endregion
        
        [SerializeField] private GameObject unfold;
        [SerializeField] private GameObject fold;
        [SerializeField] private GameObject currentLogGameObject;
        
        [SerializeField] private bool isFold = true;
        
        private readonly List<string> logList = new List<string>();
        private string _recentLog = "";
        
        private void OnEnable()
        {
            currentLogGameObject = Instantiate(fold, this.transform);
            currentLogGameObject.transform.Find("LogLine").Find("LogText").GetComponent<Text>().text = _recentLog;
        }

        public void Log(string log)
        {
            _recentLog = log;
            logList.Add(log);
            UpdateFold();
        }

        public void LogError(string log)
        {
            string errorLog = "<color=red>Error!  " + log + "</color>";
            _recentLog = errorLog;
            logList.Add(errorLog);
            UpdateFold();
        }

        public void Click(RaycastHit2D[] hits)
        {
            foreach (var hit in hits)
            {
                if (hit.transform.name == "CollapseButton")
                {
                    isFold = !isFold;
                    UpdateFold();
                }
            }
        }

        private void UpdateFold()
        {
            Destroy(currentLogGameObject);
            currentLogGameObject = null;

            string log = "";

            for (int i = 0; i < logList.Count; i++)
            {
                log += i + " : ";
                log += logList[i];
                if (i != logList.Count -1)
                {
                    log += "\n";
                }
            }
            
            if (isFold)
            {
                currentLogGameObject = Instantiate(fold, this.transform);
                currentLogGameObject.transform.Find("LogLine").Find("LogText").GetComponent<Text>().text =
                    logList.Count-1 + " : " + _recentLog;
            }
            else
            {
                currentLogGameObject = Instantiate(unfold, this.transform);
                currentLogGameObject.GetComponent<LogUnFoldManager>().SetLog(log);
            }
        }
        
    }
}
