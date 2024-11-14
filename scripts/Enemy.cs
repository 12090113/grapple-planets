using Godot;
using System;

public partial class Enemy : CharacterBody2D
{
    [Export] 
    public float Speed = 200f;
    private Node2D _player;

    public override void _Ready()
    {
        _player = GetNode<Node2D>("../Player");
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_player != null)
        {
            Vector2 direction = (_player.GlobalPosition - GlobalPosition).Normalized();

            Velocity = direction * Speed;
            MoveAndSlide();
        }
    }
}
