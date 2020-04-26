using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
 
namespace GridSand
{
    public struct Collision
    {
        public Vector2 Center { get; set; }
        public float Radius { get; set; }

        public Collision(Vector2 center, float radius)
        {
            Center = center;
            Radius = radius;
        }

        public bool Contains(Vector2 point)
        {
            return ((point - Center).Length() <= Radius);
        }

        public bool Intersects(Collision other)
        {
            return ((other.Center - Center).Length() < (other.Radius - Radius));
        }

        public struct CircleLineCollisionResult
        {
            public bool Collision;
            public Vector2 Point;
            public Vector2 Normal;
            public float Distance;
        }
        public static bool LineLineIntersect(Vector2 a, Vector2 b, Vector2 c, Vector2 d, out Vector2 point)
        {
            point = Vector2.Zero;

            double r, s;
            double denominator = (b.X - a.X) * (d.Y - c.Y) - (b.Y - a.Y) * (d.X - c.X);

            // If the denominator in above is zero, AB & CD are colinear
            if (denominator == 0)
            {
                return false;
            }

            double numeratorR = (a.Y - c.Y) * (d.X - c.X) - (a.X - c.X) * (d.Y - c.Y);
            r = numeratorR / denominator;

            double numeratorS = (a.Y - c.Y) * (b.X - a.X) - (a.X - c.X) * (b.Y - a.Y);
            s = numeratorS / denominator;

            // non-intersecting
            if (r < 0 || r > 1 || s < 0 || s > 1)
            {
                return false;
            }

            // find intersection point
            point.X = (float)(a.X + (r * (b.X - a.X)));
            point.Y = (float)(a.Y + (r * (b.Y - a.Y)));

            return true;
        }

        public static bool CircleCircleIntersect(Vector2 center1, float radius1, Vector2 center2, float radius2)
        {
            Vector2 line = center2 - center1;
            // we use LengthSquared to avoid a costly square-root call
            return (line.LengthSquared() <= (radius1 + radius2) * (radius1 + radius2));
        }

        public static bool CircleRectangleCollide(Vector2 center, float radius, Rectangle rectangle, ref CircleLineCollisionResult result)
        {
            float xVal = center.X;
            if (xVal < rectangle.Left) xVal = rectangle.Left;
            if (xVal > rectangle.Right) xVal = rectangle.Right;

            float yVal = center.Y;
            if (yVal < rectangle.Top) yVal = rectangle.Top;
            if (yVal > rectangle.Bottom) yVal = rectangle.Bottom;

            Vector2 direction = new Vector2(center.X - xVal, center.Y - yVal);
            float distance = direction.Length();

            if ((distance > 0) && (distance < radius))
            {
                result.Collision = true;
                result.Distance = radius - distance;
                result.Normal = Vector2.Normalize(direction);
                result.Point = new Vector2(xVal, yVal);
            }
            else
            {
                result.Collision = false;
            }

            return result.Collision;
        }

        public static bool CircleLineCollide(Vector2 center, float radius, Vector2 lineStart, Vector2 lineEnd, ref CircleLineCollisionResult result)
        {
            Vector2 AC = center - lineStart;
            Vector2 AB = lineEnd - lineStart;
            float ab2 = AB.LengthSquared();
            if (ab2 <= 0f)
            {
                return false;
            }
            float acab = Vector2.Dot(AC, AB);
            float t = acab / ab2;

            if (t < 0.0f)
                t = 0.0f;
            else if (t > 1.0f)
                t = 1.0f;

            result.Point = lineStart + t * AB;
            result.Normal = center - result.Point;

            float h2 = result.Normal.LengthSquared();
            float r2 = radius * radius;

            if ((h2 > 0) && (h2 <= r2))
            {
                result.Normal.Normalize();
                result.Distance = (radius - (center - result.Point).Length());
                result.Collision = true;
            }
            else
            {
                result.Collision = false;
            }

            return result.Collision;
        }

        private float HexWidth(float height)
        {
            return (float)(4 * (height / 2 / Math.Sqrt(3)));
        }

        private void PointToHex(float x, float y, float height, out int row, out int col)
        {
            // Find the test rectangle containing the point.
            float width = HexWidth(height);
            col = (int)(x / (width * 0.75f));

            if (col % 2 == 0)
                row = (int)(y / height);
            else
                row = (int)((y - height / 2) / height);

            // Find the test area.
            float testx = col * width * 0.75f;
            float testy = row * height;
            if (col % 2 == 1) testy += height / 2;

            // See if the point is above or
            // below the test hexagon on the left.
            bool is_above = false, is_below = false;
            float dx = x - testx;
            if (dx < width / 4)
            {
                float dy = y - (testy + height / 2);
                if (dx < 0.001)
                {
                    // The point is on the left edge of the test rectangle.
                    if (dy < 0) is_above = true;
                    if (dy > 0) is_below = true;
                }
                else if (dy < 0)
                {
                    // See if the point is above the test hexagon.
                    if (-dy / dx > Math.Sqrt(3)) is_above = true;
                }
                else
                {
                    // See if the point is below the test hexagon.
                    if (dy / dx > Math.Sqrt(3)) is_below = true;
                }
            }

            // Adjust the row and column if necessary.
            if (is_above)
            {
                if (col % 2 == 0) row--;
                col--;
            }
            else if (is_below)
            {
                if (col % 2 == 1) row++;
                col--;
            }
        }

        // Return the width of a triangle.
        private float TriangleWidth(float height)
        {
            return (float)(2 * height / Math.Sqrt(3));
        }

        // Return the row and column of the triangle at this point.
        private void PointToTriangle(float x, float y, float height, out float row, out float col)
        {
            float width = TriangleWidth(height);
            row = (int)(y / height);
            col = (int)(x / width);

            float dy = (row + 1) * height - y;
            float dx = x - col * width;
            if (row % 2 == 1) dy = height - dy;
            if (dy > 1)
            {
                if (dx < width / 2)
                {
                    // Left half of triangle.
                    float ratio = dx / dy;
                    if (ratio < 1f / Math.Sqrt(3)) col -= 0.5f;
                }
                else
                {
                    // Right half of triangle.
                    float ratio = (width - dx) / dy;
                    if (ratio < 1f / Math.Sqrt(3)) col += 0.5f;
                }
            }
        }

    }
}
