using Godot;

public partial class Player : Area2D
{
    [Signal]
    public delegate void HitEventHandler();

    [Export]
    public int Speed { get; set; } = 400; // How fast the player will move (pixels/sec).

    private Vector2 _screenSize; // Size of the game window.
    private JoystickControl _joystickControl;

    // Triggered when another body enters the player's collision area.
    private void OnBodyEntered(Node2D body)
    {
        Hide(); // Player disappears after being hit.
        EmitSignal(SignalName.Hit);

        // Must be deferred as we can't change physics properties on a physics callback.
        GetNode<CollisionShape2D>("CollisionShape2D").SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
    }

    // Starts the player's position and state at the beginning of the game.
    public void Start(Vector2 position)
    {
        Position = position;
        Show();
        GetNode<CollisionShape2D>("CollisionShape2D").Disabled = false;
    }

    public override void _Ready()
    {
        _screenSize = GetViewportRect().Size;
        _joystickControl = GetNode<JoystickControl>("../HUD/JoystickControl"); // Adjusted path.
        Hide();
    }

    public override void _Process(double delta)
    {
        var velocity = Vector2.Zero; // The player's movement vector.

        // Check for joystick input first
        if (OS.HasFeature("mobile") && _joystickControl.JoystickDirection.Length() > 0)
        {
            velocity = _joystickControl.JoystickDirection;
        }
        else
        {
            // Keyboard input for desktop
            if (Input.IsActionPressed("move_right"))
            {
                velocity.X += 1;
            }

            if (Input.IsActionPressed("move_left"))
            {
                velocity.X -= 1;
            }

            if (Input.IsActionPressed("move_down"))
            {
                velocity.Y += 1;
            }

            if (Input.IsActionPressed("move_up"))
            {
                velocity.Y -= 1;
            }
        }

        // Update animation
        var animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");

        if (velocity.X != 0)
        {
            animatedSprite2D.Animation = "walk";
            animatedSprite2D.FlipV = false;
            animatedSprite2D.FlipH = velocity.X < 0;
        }
        else if (velocity.Y != 0)
        {
            animatedSprite2D.Animation = "up";
            animatedSprite2D.FlipV = velocity.Y > 0;
        }

        if (velocity.Length() > 0)
        {
            velocity = velocity.Normalized() * Speed;
            animatedSprite2D.Play();
        }
        else
        {
            animatedSprite2D.Stop();
        }

        // Apply movement
        Position += velocity * (float)delta;

        // Clamp the player's position to stay within the screen bounds
        Position = new Vector2(
            x: Mathf.Clamp(Position.X, 0, _screenSize.X),
            y: Mathf.Clamp(Position.Y, 0, _screenSize.Y)
        );
    }
}
