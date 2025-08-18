using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;

public partial class GrappleCutter : Area2D
{
    [Export]
    private Grapple grapple;
    private Variant cutPoint;
    private Vector2 oldPos;
    private Vector2 oldCutPos;
    private Vector2 oldPos0;
    private Vector2 oldPos1;
    private Vector2 pos0;
    private Vector2 pos1;
    [Export]
    private bool debugLines;
    private List<Vector2> segments = new();
    public override void _Process(double delta)
    {
        Array<Area2D> bodies = GetOverlappingAreas();
        foreach (Area2D area in bodies) {
            if (area is GrappleCutDetector && area != grapple.cutArea) {
                GrappleCutDetector enemy = (GrappleCutDetector)area;

                oldPos0 = enemy.grapple.ToGlobal(enemy.grapple.oldPoints[0]);
                oldPos1 = enemy.grapple.ToGlobal(enemy.grapple.oldPoints[1]);

                pos0 = enemy.grapple.ToGlobal(enemy.grapple.points[0]);
                pos1 = enemy.grapple.ToGlobal(enemy.grapple.points[1]);

                //GD.Print(oldPos1,pos1);
                oldCutPos = FindTransformedThirdPoint(oldPos0, oldPos1, oldPos, pos0, pos1);

                if (debugLines) {
                    segments.Add(oldCutPos);
                    segments.Add(GlobalPosition);
                }

                var intersectionResult = Geometry2D.SegmentIntersectsSegment(oldCutPos, GlobalPosition, pos0, pos1);

                if (intersectionResult.VariantType != Variant.Type.Nil) {
                    cutPoint = intersectionResult;
                    enemy.grapple.Retract(false);
                    enemy.grapple.player.grappleDisabled = grapple.player.grappleCutTime;
                }
            }
        }
        oldPos = GlobalPosition;
        if (debugLines)
            QueueRedraw();
    }

    public override void _Draw()
    {
        if (debugLines) {
            if (cutPoint.VariantType != Variant.Type.Nil) {
                DrawCircle(ToLocal(cutPoint.AsVector2()), 100, Colors.DarkRed);
            }
            
            DrawLine(ToLocal(pos0), ToLocal(pos1), Colors.Blue);
            for (int i = 0; i < segments.Count; i += 2) {
                DrawLine(ToLocal(segments[i]), ToLocal(segments[i+1]), Colors.Green);
            }
        }
    }

    /// <summary>
    /// Finds the third point of a transformed triangle given the original triangle and two points
    /// of the new triangle. This assumes a uniform scale, rotation, and translation (affine transformation).
    /// </summary>
    /// <param name="originalA">The first point of the original triangle.</param>
    /// <param name="originalB">The second point of the original triangle.</param>
    /// <param name="originalC">The third point of the original triangle (the one we want to find in the new space).</param>
    /// <param name="newA">The first point of the transformed triangle.</param>
    /// <param name="newB">The second point of the transformed triangle.</param>
    /// <returns>The calculated position of the third point in the new, transformed space.</returns>
    /// <exception cref="ArgumentException">Thrown if the original points A and B are the same,
    /// which makes the transformation ambiguous.</exception>
    public static Vector2 FindTransformedThirdPoint(
        Vector2 originalA, Vector2 originalB, Vector2 originalC,
        Vector2 newA, Vector2 newB)
    {
        // 1. Define the original vectors relative to point A.
        Vector2 vec_AB = originalB - originalA;
        Vector2 vec_AC = originalC - originalA;

        // Handle the edge case where the original reference points are the same.
        // This would cause a division by zero. The transformation is undefined.
        if (vec_AB.LengthSquared() == 0)
        {
            throw new ArgumentException("Original points A and B cannot be the same.");
        }

        // 2. Define the new vector in the transformed space.
        Vector2 vec_A_prime_B_prime = newB - newA;

        // 3. Find the scale and rotation transformation.
        // The scale is the ratio of the new vector's length to the old one.
        float scale = vec_A_prime_B_prime.Length() / vec_AB.Length();

        // The rotation is the difference in angle between the new and old vectors.
        float rotation = vec_A_prime_B_prime.Angle() - vec_AB.Angle();

        // 4. Apply this transformation to the other original vector (vec_AC).
        // First, apply the scale.
        // Then, apply the rotation.
        Vector2 vec_A_prime_C_prime = (vec_AC * scale).Rotated(rotation);

        // 5. Find the final point C' by adding the new transformed vector to the new point A'.
        Vector2 newC = newA + vec_A_prime_C_prime;

        return newC;
    }
}
