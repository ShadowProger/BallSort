using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

namespace Manybits
{
    public enum CameraMode { ConstantWidth, ConstantHeight }

    public class CameraManager : SingletonMonoBehaviour<CameraManager>
    {
        [SerializeField]
        private CameraMode cameraMode;
        [SerializeField]
        private float cameraWidth;
        [SerializeField]
        private float cameraHeight;

        [SerializeField]
        private Canvas canvas;

        public float LeftBorder { get; private set; }
        public float RightBorder { get; private set; }
        public float TopBorder { get; private set; }
        public float BottomBorder { get; private set; }
        public float ScreenHeight { get; private set; }
        public float ScreenWidth { get; private set; }
        public float ScaleCoef { get; private set; }
        public float Ratio { get; private set; }



        void Awake()
        {
            SetCamSize(cameraWidth, cameraHeight, cameraMode);
        }


        
        public void SetCamSize(float newWidth, float newHeight, CameraMode cameraMode)
        {
            this.cameraMode = cameraMode;

            Ratio = (float)Screen.height / (float)Screen.width;

            if (cameraMode == CameraMode.ConstantWidth)
            {
                float newSize = cameraWidth * Ratio * 0.5f;
                Camera.main.orthographicSize = newSize;
            }
            else
            {
                Camera.main.orthographicSize = newHeight * 0.5f;
            }

            RecalcBorders();
            ScreenHeight = TopBorder - BottomBorder;
            ScreenWidth = RightBorder - LeftBorder;
            ScaleCoef = ScreenHeight / (float)Screen.height;
        }



        public void RecalcBorders()
        {
            LeftBorder = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0)).x;
            RightBorder = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, 0)).x;
            TopBorder = Camera.main.ViewportToWorldPoint(new Vector3(0, 1, 0)).y;
            BottomBorder = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0)).y;
        }



        public Rect GetRect()
        {
            Rect rect = new Rect(new Vector2(LeftBorder, BottomBorder), new Vector2(RightBorder - LeftBorder, TopBorder - BottomBorder));
            return rect;
        }



        public float GetCanvasWidth()
        {
            return ((RectTransform)canvas.transform).rect.width;
        }



        public float GetCanvasHeight()
        {
            return ((RectTransform)canvas.transform).rect.height;
        }
    }
}
