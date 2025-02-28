using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static EmbroideryFramewark.EmbroideryAction;

namespace EmbroideryFramewark
{
    public class m_MouseInput
    {

        public static bool IsMouseMove => !MouseMove.Equals(Vector2.zero);
        public static Vector2 MouseMove
        {
            get => Mouse.current.delta.ReadValue();
        }


        public static bool IsMovePin { 
            get => (!Keyboard.current.leftCtrlKey.isPressed
                && _cameraAction.MovePin.ReadValue<float>() > 0f); }

        /// <summary>
        /// 
        /// </summary>
        public static bool IsConformPinPoint { 
            get => (!Keyboard.current.leftCtrlKey.isPressed
                && _cameraAction.ConfirmPinPoint.ReadValue<float>()>0f); }


        public static EmbroideryAction EmbroideryAction = new();


        /// <summary>
        /// 
        /// </summary>
        private static EmbroideryCamerCtlActions _cameraAction = EmbroideryAction.EmbroideryCamerCtl;

        public static float CameraScale { get => _cameraAction.CameraScale.ReadValue<float>(); }


        //这两个变量结合MouseMove来使用
        public static bool IsCenterRotation { get => _cameraAction.CenterRotation.ReadValue<float>() > 0f; }

        public static bool IsDragCamera { get => _cameraAction.DragCamera.ReadValue<float>() > 0f; }


        public static bool IsSaveOp { get => _cameraAction.IsSaveOp.ReadValue<float>() > 0f; }

        public static bool IsRevokeOp{ get => _cameraAction.IsRevokeOp.ReadValue<float>() > 0f; }

    }
}