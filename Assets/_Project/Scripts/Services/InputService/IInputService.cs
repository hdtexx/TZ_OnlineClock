using UnityEngine;

namespace _Project.Scripts.Services.InputService
{
    public interface IInputService
    {
        public Vector3 GetMousePosition();
        public bool IsLeftMouseButtonPressed();
        public bool IsLeftMouseButtonHeld();
        public bool EscPressed();
    }
}