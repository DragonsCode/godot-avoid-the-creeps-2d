using Godot;
using System;

public partial class JoystickControl : Control
{
    private Vector2 _dragStartPosition = Vector2.Zero;
    private Vector2 _dragCurrentPosition = Vector2.Zero;
    private bool _isDragging = false;

    // Drag threshold to detect significant movement.
    private const float DragThreshold = 10.0f;

    public Vector2 JoystickDirection { get; private set; } = Vector2.Zero;

    public override void _Input(InputEvent @event)
    {
        // Handle touch input for mobile
        if (@event is InputEventScreenTouch touchEvent)
        {
            if (touchEvent.Pressed)
            {
                _isDragging = true;
                _dragStartPosition = touchEvent.Position;
            }
            else
            {
                _isDragging = false;
                JoystickDirection = Vector2.Zero;
            }
        }
        // Handle drag input for mobile
        else if (@event is InputEventScreenDrag dragEvent && _isDragging)
        {
            _dragCurrentPosition = dragEvent.Position;
            var dragVector = _dragCurrentPosition - _dragStartPosition;

            if (dragVector.Length() > DragThreshold)
            {
                JoystickDirection = dragVector.Normalized();
            }
            else
            {
                JoystickDirection = Vector2.Zero;
            }
        }
        // Handle mouse input for desktop (testing mode)
        else if (OS.HasFeature("desktop"))
        {
            if (@event is InputEventMouseButton mouseEvent)
            {
                if (mouseEvent.Pressed)
                {
                    _isDragging = true;
                    _dragStartPosition = mouseEvent.Position;
                }
                else
                {
                    _isDragging = false;
                    JoystickDirection = Vector2.Zero;
                }
            }
            else if (@event is InputEventMouseMotion mouseMotion && _isDragging)
            {
                _dragCurrentPosition = mouseMotion.Position;
                var dragVector = _dragCurrentPosition - _dragStartPosition;

                if (dragVector.Length() > DragThreshold)
                {
                    JoystickDirection = dragVector.Normalized();
                }
                else
                {
                    JoystickDirection = Vector2.Zero;
                }
            }
        }
    }

    public override void _Process(double delta)
    {
        if (_isDragging)
        {
            GD.Print("Joystick Direction: ", JoystickDirection);
        }
    }
}
