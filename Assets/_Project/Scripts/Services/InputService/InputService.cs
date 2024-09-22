using UnityEngine;

namespace _Project.Scripts.Services.InputService
{
    public class InputService : IInputService
    {
        public Vector3 GetMousePosition() => Input.mousePosition;
        
        public bool IsLeftMouseButtonPressed() => Input.GetMouseButtonDown(0);

        public bool IsLeftMouseButtonHeld() => Input.GetMouseButton(0);

        public bool EscPressed() => Input.GetKeyDown(KeyCode.Escape);
    }
}