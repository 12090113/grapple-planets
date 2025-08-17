using Godot;
using System;

public partial class TriangleTest : Node2D
{
    [Export]
    Node2D oldA;
    [Export]
    Node2D oldB;
    [Export]
    Node2D oldC;
    [Export]
    Node2D newA;
    [Export]
    Node2D newB;
    [Export]
    Node2D newC;
    [Export]
    Node2D transformedOldC;

    public override void _Process(double delta)
    {
        transformedOldC.Position = FindTransformedThirdPoint(oldA.Position,oldB.Position,oldC.Position,newA.Position,newB.Position);
        QueueRedraw();
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

    public override void _Draw()
    {
        Color color = Colors.Green;

        var intersectionResult = Geometry2D.SegmentIntersectsSegment(transformedOldC.Position, newC.Position, newA.Position, newB.Position);
        if (intersectionResult.VariantType != Variant.Type.Nil) {
            color = Colors.Red;
        }

        DrawLine(newA.Position,newB.Position, color, 5, true);
        DrawLine(newC.Position,transformedOldC.Position, color, 5, true);
    }
}
