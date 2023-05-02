using UnityEngine;
using UnityEngine.InputSystem;

namespace Scripts.InputControls
{
    public class InputController: MonoBehaviour
    {
        private Input _input;

        private void Awake()
        {
            _input = new Input();
        }

        private void OnEnable()
        {
            _input.Enable();
        }

        public InputAction GetMousePress() => _input.Carrom.MousePress;
        
        public Vector2 GetMousePosition() => _input.Carrom.MousePosition.ReadValue<Vector2>();

        private void OnDisable()
        {
            _input.Disable();
        }
    }
}