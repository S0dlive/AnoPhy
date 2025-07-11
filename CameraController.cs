using Godot;
using System;

public partial class CameraController : Camera3D
{
	[Export] public float MoveSpeed = 5.0f;
	[Export] public float MouseSensitivity = 0.3f;

	private Vector2 rotation = Vector2.Zero;

	public override void _Ready()
	{
		Input.MouseMode = Input.MouseModeEnum.Captured;
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event is InputEventMouseMotion mouseMotion)
		{
			rotation.X -= mouseMotion.Relative.Y * MouseSensitivity;
			rotation.Y -= mouseMotion.Relative.X * MouseSensitivity;

			rotation.X = Mathf.Clamp(rotation.X, -90, 90);

			RotationDegrees = new Vector3(rotation.X, rotation.Y, 0);
		}

		if (@event is InputEventKey keyEvent && keyEvent.Pressed && keyEvent.Keycode == Key.Escape)
		{
			Input.MouseMode = Input.MouseModeEnum.Visible;
		}
	}

	public override void _Process(double delta)
	{
		Vector3 direction = Vector3.Zero;

		if (Input.IsActionPressed("move_forward")) direction -= Transform.Basis.Z;
		if (Input.IsActionPressed("move_backward")) direction += Transform.Basis.Z;
		if (Input.IsActionPressed("move_left")) direction -= Transform.Basis.X;
		if (Input.IsActionPressed("move_right")) direction += Transform.Basis.X;
		if (Input.IsActionPressed("move_up")) direction += Transform.Basis.Y;
		if (Input.IsActionPressed("move_down")) direction -= Transform.Basis.Y;

		Position += direction.Normalized() * MoveSpeed * (float)delta;
		
		GD.Print("position actuelle" + Position);
		GD.Print("rotation actuelle" + RotationDegrees);
	}
}
