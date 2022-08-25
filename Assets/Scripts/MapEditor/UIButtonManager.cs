using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.MapEditor
{
    public enum UIButton
    {
        Add,
        Delete,
        QSave,
        Save,
        Load,
        Clear
    }
    
    [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
    public class UIButtonManager : MonoBehaviour
    {
        #region singleTon

        private static UIButtonManager _instance;

        public static UIButtonManager GetInstance()
        {
            if (_instance == null)
            {
                var obj = FindObjectOfType<UIButtonManager>();
                if (obj == null)
                {
                    Debug.LogError("Error! ButtonUIManager is not ready");
                    return null;
                }
                _instance = obj;
            }

            return _instance;
        }

        #endregion
        
        [SerializeField] private GameObject add;
        [SerializeField] private GameObject delete;
        [SerializeField] private GameObject qSave;
        [SerializeField] private GameObject save;
        [SerializeField] private GameObject load;
        [SerializeField] private GameObject clear;

        private const float DisableAlpha = 0.2f;
        private const float EnableAlpha = 1f;
        
        public void Click(RaycastHit2D[]hits)
        {
            foreach (var hit in hits)
            {
                if (hit.transform.gameObject == add)
                {
                    UIButtonAdd.GetInstance().Click();
                }

                if (hit.transform.gameObject == delete)
                {
                    UIButtonDelete.GetInstance().Click();
                }
                
                if (hit.transform.gameObject == qSave)
                {
                    SaveManager.GetInstance().QSave();
                }

                if (hit.transform.gameObject == save)
                {
                    SaveLoadUILayer.GetInstance().OpenSaveUI();
                }
                
                if (hit.transform.gameObject == load)
                {
                    SaveLoadUILayer.GetInstance().OpenLoadUI();
                }
                
                if (hit.transform.gameObject == clear)
                {
                    NodeManipulator.GetInstance().ClearSelectedNode();
                    LineManipulator.GetInstance().ClearSelectedLine();
                    NodeManager.GetInstance().ClearAllNode();
                    LineManager.GetInstance().ClearAllLine();
                    ViewInfoModeManager.GetInstance().UpdateInfo();
                }
            }
            
            NodeManipulator.GetInstance().ClearSelectedNode();
            LineManipulator.GetInstance().ClearSelectedLine();
        }
        
        public void DisableAllUIButton()
        {
            DisableUIButton(UIButton.Add);
            DisableUIButton(UIButton.Delete);
            DisableUIButton(UIButton.Save);
            DisableUIButton(UIButton.Load);
        }

        public void EnableAllUIButton()
        {
            EnableUIButton(UIButton.Add);
            UIButtonDelete.GetInstance().SetDeleteButton();
            EnableUIButton(UIButton.Save);
            EnableUIButton(UIButton.Load);
            EnableUIButton(UIButton.Clear);
        }
        
        public void DisableUIButton(UIButton uiButton)
        {
            Color color = Color.white;
            color.a = DisableAlpha;
            switch (uiButton)
            {
                case UIButton.Add:
                    add.GetComponent<Text>().color = color;
                    add.GetComponent<BoxCollider2D>().enabled = false; 
                    break;
                
                case UIButton.Delete:
                    delete.GetComponent<Text>().color = color;
                    delete.GetComponent<BoxCollider2D>().enabled = false; 
                    break;
                
                case UIButton.QSave:
                    qSave.GetComponent<Text>().color = color;
                    qSave.GetComponent<BoxCollider2D>().enabled = false; 
                    break;
                
                case UIButton.Save:
                    save.GetComponent<Text>().color = color;
                    save.GetComponent<BoxCollider2D>().enabled = false; 
                    break;
                
                case UIButton.Load:
                    load.GetComponent<Text>().color = color;
                    load.GetComponent<BoxCollider2D>().enabled = false; 
                    break;
            }
        }

        public void EnableUIButton(UIButton uiButton)
        {
            Color color = Color.white;
            color.a = EnableAlpha;
            switch (uiButton)
            {
                case UIButton.Add:
                    add.GetComponent<Text>().color = color;
                    add.GetComponent<BoxCollider2D>().enabled = true; 
                    break;
                
                case UIButton.Delete:
                    delete.GetComponent<Text>().color = color;
                    delete.GetComponent<BoxCollider2D>().enabled = true; 
                    break;
                
                case UIButton.QSave:
                    qSave.GetComponent<Text>().color = color;
                    qSave.GetComponent<BoxCollider2D>().enabled = true; 
                    break;
                
                case UIButton.Save:
                    save.GetComponent<Text>().color = color;
                    save.GetComponent<BoxCollider2D>().enabled = true; 
                    break;
                
                case UIButton.Load:
                    load.GetComponent<Text>().color = color;
                    load.GetComponent<BoxCollider2D>().enabled = true; 
                    break;
            }
        }

        private void OnEnable()
        {
            DisableUIButton(UIButton.QSave);
        }
    }
}
