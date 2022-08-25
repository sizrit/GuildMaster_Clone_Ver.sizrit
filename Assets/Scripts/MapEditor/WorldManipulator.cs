using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace Assets.Scripts.MapEditor
{
    [SuppressMessage("ReSharper", "Unity.InefficientPropertyAccess")]
    [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
    public class WorldManipulator : MonoBehaviour
    {
        #region singleTon

        private static WorldManipulator _instance;

        public static WorldManipulator GetInstance()
        {
            if (_instance == null)
            {
                var obj = FindObjectOfType<WorldManipulator>();
                if (obj == null)
                {
                    Debug.LogError("Error! WorldManipulator is not ready");
                    return null;
                }

                _instance = obj;
            }

            return _instance;
        }

        #endregion
        
        private Action worldManipulatorFunc = delegate { };

        private void ZoomWorld()
        {
            if (Input.mouseScrollDelta.y > 0)
            {
                if (Camera.main.orthographicSize > 4.9)
                {
                    Camera.main.orthographicSize -= 2;
                }
            }
            else if(Input.mouseScrollDelta.y < 0)
            {
                if (Camera.main.orthographicSize < 9.1)
                {
                    Camera.main.orthographicSize += 2;
                }
            }
        }
        
        public void WorldRightClick()
        {
            ClickSystem.GetInstance().DisableClickSystem();
            worldManipulatorFunc = DragWorld;
            prevPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            prevPosition.z = 0;
        }

        Vector3 prevPosition = Vector3.zero;
        
        private void DragWorld()
        {
            if (Input.GetMouseButton(1))
            {
                Vector3 delta = Camera.main.ScreenToWorldPoint(Input.mousePosition) - prevPosition;
                delta.z = 0;
                Camera.main.transform.position -= delta;
                prevPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }

            if (Input.GetMouseButtonUp(1))
            {
                GridManager.GetInstance().AdjustGridLinePosition();
                ClickSystem.GetInstance().EnableClickSystem();
                worldManipulatorFunc = delegate { };
            }
        }

        private bool CheckLogArea()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D[] hits = Physics2D.GetRayIntersectionAll(ray);

            foreach (var hit in hits)
            {
                if (hit.transform.CompareTag("MapEditorLog"))
                {
                    return false;
                }
            }

            return true;
        }

        private void Update()
        {
            if (CheckLogArea())
            {
                ZoomWorld();
                worldManipulatorFunc();
            }
        }
    }
}
