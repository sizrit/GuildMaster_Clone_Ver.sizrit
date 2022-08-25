using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.MapEditor
{
    public class GridManager : MonoBehaviour
    {
        #region singleTon

        private static GridManager _instance;

        public static GridManager GetInstance()
        {
            if (_instance == null)
            {
                var obj = FindObjectOfType<GridManager>();
                if (obj == null)
                {
                    Debug.LogError("Error! GridManager is not ready");
                    return null;
                }
                _instance = obj;
            }

            return _instance;
        }

        #endregion

        [SerializeField] private GameObject sizeController;
        [SerializeField] private GameObject verticalLine;
        [SerializeField] private GameObject horizontalLine;
        
        private float size = 1;
        private readonly List<GameObject> verticalLineList = new List<GameObject>();
        private readonly List<GameObject> horizontalLineList = new List<GameObject>();

        [SerializeField] private int lineCount = 20;

        private void SetLine()
        {
            sizeController.GetComponent<Slider>().onValueChanged.AddListener(AdjustGridSize);
            
            for (int i = 0; i < lineCount * 2 + 1; i++)
            {
                verticalLineList.Add(Instantiate(verticalLine, this.transform));
                horizontalLineList.Add(Instantiate(horizontalLine, this.transform));
            }

            for (int i = 0; i < lineCount * 2 + 1; i++)
            {
                verticalLineList[i].transform.position = new Vector3(size * (lineCount - i), 0, 0);
                horizontalLineList[i].transform.position = new Vector3(0, size * (lineCount - i), 0);
            }
        }

        public void AdjustGridSize(float value)
        {
            size = 1 + value;
            AdjustGridLinePosition();
        }

        public void AdjustGridLinePosition()
        {
            Vector3 cameraPosition = Camera.main.transform.position;
            Vector3 zeroPosition = new Vector3(CustomFloor(cameraPosition.x,size), CustomFloor(cameraPosition.y,size), 0);
            for (int i = 0; i < lineCount * 2 + 1; i++)
            {
                verticalLineList[i].transform.position = new Vector3(size * (lineCount - i), 0, 0) + zeroPosition;
                horizontalLineList[i].transform.position = new Vector3(0, size * (lineCount - i), 0) + zeroPosition;
            }
        }

        private float CustomFloor(float dividend, float divisor)
        {
            int quotient = (int)MathF.Floor(dividend/divisor);
            return divisor * quotient;
        }

        private void OnEnable()
        {
            SetLine();
        }
    }
}
