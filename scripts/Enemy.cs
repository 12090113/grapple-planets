using Godot;
using System;
using System.Threading;

public partial class Enemy : CharacterBody2D
{
    [Export] 
    public float Speed = 200f;
    private Node2D _player;

    private AnimatedSprite2D outline;
    private AnimatedSprite2D fly;
    public override void _Ready()
    {
        outline = GetNode<AnimatedSprite2D>("Fly/Outline");
        fly = GetNode<AnimatedSprite2D>("Fly");
        _player = GetNode<Node2D>("../Player");
        playanimation();
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

    public void playanimation()
    {
        outline.CallDeferred("play");
        fly.CallDeferred("play");
    }
}
