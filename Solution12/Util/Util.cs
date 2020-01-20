using System;
using System.Numerics;
using Emotion.Primitives;
using Emotion.Utility;
using Solution12.Util.Models;

namespace Solution12.Util
{
    public static class Util
    {
        /// <summary>
        /// Calculate the cross product of 2D vectors, also known as a 'Wedge product'
        /// </summary>
        public static float Cross(this Vector2 vector1, Vector2 vector2)
        {
            return vector1.X * vector2.Y - vector1.Y * vector2.X;
        }


        public static bool Intersects(this Ray2D ray, Line barrier, out Vector2? intersectionPoint)
        {
            if (ray.Start == barrier.Start || ray.Start == barrier.End)
            {
                intersectionPoint = ray.Start;
                return true;
            }

            intersectionPoint = null;

            Vector2 raySegment = Vector2.Normalize(ray.Direction);
            Vector2 barrierSegment = barrier.End - barrier.Start;

            float barrierIntersectCoef = Cross(ray.Start - barrier.Start, raySegment) / Cross(barrierSegment, raySegment);
            intersectionPoint = barrier.Start + barrierSegment * barrierIntersectCoef;

            if (barrierIntersectCoef < 0f || barrierIntersectCoef > 1f)
                return false;

//            Console.Out.WriteLine($"{Vector2.Normalize(intersectionPoint.Value - ray.Start)} : {raySegment}");
            Vector2 directionValidationSegment = Vector2.Normalize(intersectionPoint.Value - ray.Start);
            // raySegment is the Normalized ray Direction, no need for further modification for the purpose of direction checks
            return Maths.Approximately(directionValidationSegment.X, raySegment.X, 1e06f) && Maths.Approximately(directionValidationSegment.Y, raySegment.Y, 1e06f);
        }
    }
}