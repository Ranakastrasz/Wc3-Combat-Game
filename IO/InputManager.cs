﻿using System.Numerics;

namespace Wc3_Combat_Game.IO
{
    public class InputManager
    {
        private HashSet<Keys> _keysDown = new();
        private HashSet<Keys> _keysPressedThisFrame = new();
        private bool _mouseDown = false;
        private bool _mouseClickedThisFrame = false;
        private Vector2 _mouseClickedPosition;
        private Vector2 _currentMousePosition;

        public void OnKeyDown(Keys key)
        {
            _keysDown.Add(key);
            _keysPressedThisFrame.Add(key);
        }

        public void OnKeyUp(Keys key)
        {
            _keysDown.Remove(key);
        }

        public void OnMouseDown(Vector2 pos)
        {
            _mouseDown = true;
            _mouseClickedThisFrame = true;
            _mouseClickedPosition = pos;
        }

        public void OnMouseUp()
        {
            _mouseDown = false;
        }

        public void OnMouseMove(Vector2 pos)
        {
            _currentMousePosition = pos;
        }

        public bool IsKeyDown(Keys key) => _keysDown.Contains(key);

        public bool WasKeyPressedThisFrame(Keys key) => _keysPressedThisFrame.Contains(key);


        public bool IsMouseDown() => _mouseDown;
        public bool IsMouseClicked() => _mouseClickedThisFrame;

        public Vector2 CurrentMousePosition => _currentMousePosition;
        public Vector2 MouseClickedPosition => _mouseClickedPosition;

        // Call at end of update to clear key press buffer (if not consumed)
        public void EndFrame()
        {
            _keysPressedThisFrame.Clear();
            _mouseClickedThisFrame = false;
        }
    }
}
