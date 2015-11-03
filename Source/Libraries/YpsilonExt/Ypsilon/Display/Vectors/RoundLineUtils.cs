// RoundLine.cs
// By Michael D. Anderson
// Version 4.00, Feb 8 2011
// Microsoft Public License (Ms-PL)

#region Using Statements
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
#endregion

namespace Ypsilon.Display.Vectors
{
    /// <summary>
    /// Various support functions for RoundLines.
    /// You don't need to include this file in your program if
    /// you only want basic RoundLine drawing functionality.
    /// </summary>
    public partial class RoundLine
    {
        /// <summary>
        /// Distance squared from an arbitrary point p to the "virtual" 
        /// version of this line, meaning assuming a thickness of 0.
        /// </summary>
        public float DistanceSquaredPointToVirtualLine(Vector2 p, out Vector2 closestP)
        {
            Vector2 v = P1 - P0; // Vector from line's p0 to p1
            Vector2 w = p - P0; // Vector from line's p0 to p

            // See if p is closer to p0 than to the segment
            float c1 = Vector2.Dot(w, v);
            if (c1 <= 0)
            {
                closestP = P0;
                return Vector2.DistanceSquared(p, P0);
            }

            // See if p is closer to p1 than to the segment
            float c2 = Vector2.Dot(v, v);
            if (c2 <= c1)
            {
                closestP = P1;
                return Vector2.DistanceSquared(p, P1);
            }

            // p is closest to point pB, between p0 and p1
            float b = c1 / c2;
            Vector2 pB = P0 + b * v;
            closestP = pB;
            return Vector2.DistanceSquared(p, pB);
        }


        /// <summary>
        /// A variant of DistanceSquaredPointToVirtualLine that
        /// just returns the distance squared, not the closest point.
        /// </summary>
        public float DistanceSquaredPointToVirtualLine(Vector2 p)
        {
            Vector2 closestP;
            return DistanceSquaredPointToVirtualLine(p, out closestP);
        }


        /// <summary>
        // Given a point moving from point p0 to point p1, past point p2, find the two values of t
        // at which the point is distance dist from p2.  If the point never gets that close, 
        // return MaxFloat for the two t values.
        /// </summary>
        private static void FindRadialT(Vector2 p0, Vector2 p1, Vector2 p2, float dist, out float t1, out float t2)
        {
            // first, adjust things so p2 is at the origin
            p0 -= p2;
            p1 -= p2;

            float a = p0.X;
            float b = p1.X - p0.X;
            float c = p0.Y;
            float d = p1.Y - p0.Y;
            // x = a + bt
            // y = c + dt
            // x * x + y * y = dist * dist // pythagorean theorem
            // Substitute and solve for t using the quadratic formula
            // (a + bt) * (a + bt) + (c + dt) * (c + dt) = dist * dist
            // (a * a + 2 * a * b * t + bt * bt) + (c * c + 2 * c * d * t + dt * dt) = dist * dist
            // (a * a + c * c) + ((2 * a * b + 2 * c * d) * t) + ((b * b + d * d) * t * t) = dist * dist
            // (quadA * t * t) + (quadB * t) + quadC = 0
            float quadA = b * b + d * d;
            float quadB = 2 * a * b + 2 * c * d;
            float quadC = a * a + c * c - dist * dist;
            float discriminant = quadB * quadB - 4 * quadA * quadC; // "b^2-4ac"
            if (discriminant < 0)
            {
                // no real roots exist
                t1 = float.MaxValue;
                t2 = float.MaxValue;
            }
            else
            {
                float root = (float)Math.Sqrt(discriminant);
                t1 = (-quadB + root) / (2 * quadA);
                t2 = (-quadB - root) / (2 * quadA);

                // Ensure that t1 <= t2
                if (t2 < t1)
                {
                    float swap = t1;
                    t1 = t2;
                    t2 = swap;
                }

                // Note: some of the following special cases may no longer
                // be necessary...they only rarely crop up and removing them
                // requires extensive testing.
                if (t1 <= 0 && t2 >= 0)
                {
                    // We are already very close.  Are we moving away or not?
                    // Depends whether t1 or t2 is closer to 0

                    float d1 = Math.Abs(t1);
                    float d2 = Math.Abs(t2);
                    if (t2 < 0.1)
                    {
                        // just grazing it and we're leaving soon anyway
                        t1 = float.MaxValue;
                        t2 = float.MaxValue;
                    }
                    else if (d1 < d2)
                    {
                        // t1 is close to zero, t2 is a big positive number

                        // Refuse to move closer to the line
                        t1 = 0;
                    }
                    else
                    {
                        // t2 is close to zero, t1 is a big negative number

                        // If we leave t2 at near-zero, it will be difficult
                        // or impossible to move away from this line.  So
                        // pretend it's the same as t1
                        t2 = t1;
                    }
                    return;
                }

                if (Math.Abs(t1 - t2) < 0.01)
                {
                    // The path barely grazes the point (perhaps floating point error),
                    // so pretend no collision
                    t1 = float.MaxValue;
                    t2 = float.MaxValue;
                }
            }
        }


        /// <summary>
        // Rotate a Vector2 by theta radians
        /// </summary>
        private static Vector2 Rotate(Vector2 vec, float theta)
        {
            Vector4 unrotatedVec4 = new Vector4(vec.X, vec.Y, 0, 1);
            Matrix matRot = Matrix.CreateRotationZ(theta);
            Vector4 rotatedVec4 = Vector4.Transform(unrotatedVec4, matRot);
            return new Vector2(rotatedVec4.X, rotatedVec4.Y);
        }


        /// <summary>
        // Given a point moving from point p0 to point p1, past this line, find the values of t
        // at which the point is distance dist from the line.  If the point never gets that close, 
        // return MaxFloat for the two t values.
        /// </summary>
        private void FindLinearT(Vector2 p0, Vector2 p1, float dist, out float t1, out float t2)
        {
            // Transform p0 and p1 into a space where the line's p0 is at (0,0) and its p1 is at (1,0)
            // In this space, the y coordinate is the distance to the line.  The x coordinate is the
            // line's valid range (0 to 1 is on the line).
            p0 -= this.P0;
            p1 -= this.P0;
            p0 = Rotate(p0, -this.Theta);
            p1 = Rotate(p1, -this.Theta);
            p0.X *= 1.0f / this.Rho;
            p1.X *= 1.0f / this.Rho;

            // y = a + bt, where a = p0.y and b is p1.y-p0.y
            // find t where y = +- dist
            // dist = a + bt1
            // t1 = (dist - a) / b
            // -dist = a + bt2
            // t2 = (-dist - a) / b
            float a = p0.Y;
            float b = p1.Y - p0.Y;
            t1 = (dist - a) / b;
            t2 = (-dist - a) / b;

            // If we are currently in contact with the wall, pretend
            // there is no contact if we are moving away from it
            if (Math.Abs(t1) < 0.0001)
            {
                if (b > -0.00001)
                    t1 = float.MaxValue;
                else if (t1 < 0)
                    t1 = 0;
            }

            if (Math.Abs(t2) < 0.0001)
            {
                if (b < 0)
                    t2 = float.MaxValue;
                else if (t2 < 0)
                    t2 = 0;
            }

            // Ensure that t1 <= t2
            if (t2 < t1)
            {
                float temp = t2;
                t2 = t1;
                t1 = temp;
            }

            // Now compute x at t1 and t2 to make sure they are in range
            // x = c + dt
            float c = p0.X;
            float d = p1.X - p0.X;
            float x1 = c + d * t1;
            if (x1 < 0 || x1 > 1)
                t1 = float.MaxValue;
            float x2 = c + d * t2;
            if (x2 < 0 || x2 > 1)
                t2 = float.MaxValue;
        }


        /// <summary>
        // Given a point moving from point p0 to point p1, past this line, find the value of t
        // at which the point is distance dist from the line.  If the point never gets that close, 
        // set tMin to MaxFloat.
        /// </summary>
        public void FindFirstIntersection(Vector2 p0, Vector2 p1, float dist, out float tMin)
        {
            float t1;
            float t2;
            tMin = float.MaxValue;

            FindRadialT(p0, p1, this.P0, dist, out t1, out t2);
            if (t1 < tMin && t1 >= 0.0f && t1 < 1.0f)
                tMin = t1;

            FindRadialT(p0, p1, this.P1, dist, out t1, out t2);
            if (t1 < tMin && t1 >= 0.0f && t1 < 1.0f)
                tMin = t1;

            FindLinearT(p0, p1, dist, out t1, out t2);
            if (t1 < tMin && t1 >= 0.0f && t1 < 1.0f)
                tMin = t1;
        }

        /// <summary>
        /// Given a bunch of lines (lineList), find all lines that are within referenceRadius 
        /// of referencePos and add them to nearbyLineList.
        /// </summary>
        public void FindNearbyLines(List<RoundLine> lineList, List<RoundLine> nearbyLineList, float globalLineRadius, Vector2 referencePos, float referenceRadius)
        {
            nearbyLineList.Clear();

            foreach (RoundLine line in lineList)
            {
                float totalDistance;
                totalDistance = referenceRadius + globalLineRadius;
                if (line.DistanceSquaredPointToVirtualLine(referencePos) < (totalDistance * totalDistance))
                    nearbyLineList.Add(line);
            }
        }


        /// <summary>
        /// Given a bunch of lines (lineList), find all lines that are within referenceRadius 
        /// of referencePos, clip them to the (referencePos, referenceRadius) disc, and add 
        /// them to nearbyLineList.
        /// </summary>
        static public void FindNearbyLinesWithClipping(List<RoundLine> lineList, List<RoundLine> nearbyLineList, float globalLineRadius, Vector2 referencePos, float referenceRadius)
        {
            nearbyLineList.Clear();

            Vector2 C = referencePos;
            float r = referenceRadius;
            foreach (RoundLine line in lineList)
            {
                Vector2 A = line.P0;
                Vector2 B = line.P1;

                float a = (B.X - A.X) * (B.X - A.X) + (B.Y - A.Y) * (B.Y - A.Y);
                float b = 2 * ((B.X - A.X) * (A.X - C.X) + (B.Y - A.Y) * (A.Y - C.Y));
                float c = C.X * C.X + C.Y * C.Y + A.X * A.X + A.Y * A.Y - 2 * (C.X * A.X + C.Y * A.Y) - r * r;
                float det = b * b - 4 * a * c;
                if (det <= 0)
                    continue;
                float e = (float)Math.Sqrt(det);
                float u1 = (-b + e) / (2 * a);
                float u2 = (-b - e) / (2 * a);
                if ((u1 < 0 || u1 > 1) && (u2 < 0 || u2 > 1))
                {
                    if ((u1 < 0 && u2 < 0) || (u1 > 1 && u2 > 1))
                        continue; // line does not intersect disc at all
                    else
                        nearbyLineList.Add(line); // line is contained within disc
                }
                else
                {
                    // line intersects disc at one or two points
                    Vector2 newP0 = line.P0;
                    Vector2 newP1 = line.P1;
                    if (0 <= u2 && u2 <= 1)
                        newP0 = Vector2.Lerp(A, B, u2);
                    if (0 <= u1 && u1 <= 1)
                        newP1 = Vector2.Lerp(A, B, u1);
                    nearbyLineList.Add(new RoundLine(newP0, newP1));
                }
            }
        }


        /// <summary>
        /// Given a bunch of lines (lineList), check their distances to the disc described by
        /// currentPos/discRadius, and find the minimum distance.  This value will be negative
        /// if the disc intersects any of the lines.
        /// </summary>
        public float MinDistanceSquaredDeviation(List<RoundLine> lineList, Vector2 currentPos, float lineRadius, float discRadius)
        {
            float minDeviation = float.MaxValue;
            foreach (RoundLine line in lineList)
            {
                float minDist2 = (lineRadius + discRadius) * (lineRadius + discRadius);
                float curDist2 = line.DistanceSquaredPointToVirtualLine(currentPos);
                float deviation = curDist2 - minDist2; // should be positive if no intersection
                if (deviation < minDeviation)
                    minDeviation = deviation;
            }
            return minDeviation;
        }


        /// <summary>
        /// Given a bunch of lines (lineList) and a disc that wants to move from currentPos
        /// to proposedPos, handle intersections and wall sliding and set finalPos to the 
        /// position that the disc should move to.
        /// </summary>
        public void CollideAndSlide(List<RoundLine> lineList, Vector2 currentPos, Vector2 proposedPos, float lineRadius, float discRadius, out Vector2 finalPos)
        {
            Vector2 oldPos = currentPos;
            Vector2 oldTarget = proposedPos;
            bool pastFirstSlide = false;

            // Keep looping until there's no further desire to move somewhere else.
            while (Vector2.DistanceSquared(oldPos, oldTarget) > 0.001f * 0.001f)
            {
                // oldPos should be safe (no intersection with anything in lineList)
                Debug.Assert(MinDistanceSquaredDeviation(lineList, oldPos, lineRadius, discRadius) >= 0);

                // Find minimum "t" at which we collide with a line
                float minT = 1.0f; // Parametric "t" of the closest collision
                RoundLine minTLine = null; // The line which causes closest collision
                foreach (RoundLine line in lineList)
                {
                    float tMinThisLine;
                    float minDist = lineRadius + discRadius;
                    float minDist2 = minDist * minDist;
                    float curDist2 = line.DistanceSquaredPointToVirtualLine(oldPos);
                    Debug.Assert(curDist2 - minDist2 >= 0);

                    line.FindFirstIntersection(oldPos, oldTarget, minDist, out tMinThisLine);
                    if (tMinThisLine >= 0 && tMinThisLine <= 1)
                    {
                        // We can move tMinThisLine toward the line, before intersecting it -- but
                        // we might intersect other lines first, so keep looking for
                        // smaller tMinThisLine values on other lines

                        // But first, refine tMinThisLine (if needed) until it satisfies the distance test
                        Vector2 newPos = new Vector2(MathHelper.Lerp(oldPos.X, oldTarget.X, tMinThisLine),
                                                     MathHelper.Lerp(oldPos.Y, oldTarget.Y, tMinThisLine));
                        if (line.DistanceSquaredPointToVirtualLine(newPos) - minDist2 < 0)
                        {
                            float ta = 0;
                            float tb = tMinThisLine;
                            for (int iterRefine = 0; iterRefine < 10; iterRefine++)
                            {
                                float tc = (ta + tb) / 2;
                                Vector2 newPosC = new Vector2(MathHelper.Lerp(oldPos.X, oldTarget.X, tc),
                                                              MathHelper.Lerp(oldPos.Y, oldTarget.Y, tc));
                                float newDistC = line.DistanceSquaredPointToVirtualLine(newPosC);
                                if (newDistC - minDist2 < 0)
                                    tb = tc;
                                else
                                    ta = tc;
                            }
                            tMinThisLine = ta;
                        }

                        // Remember this "t" and the line that caused it, if it's the closest so far
                        if (tMinThisLine < minT)
                        {
                            minT = tMinThisLine;
                            minTLine = line;
                        }
                    }
                    else
                    {
                        // This line has no issue with the disc moving to oldTarget...or does it?  
                        // Due to floating point variances, we have to double-check and pick a new "t"
                        // if oldTarget is actually too close to this line
                        float newDist = line.DistanceSquaredPointToVirtualLine(oldTarget);
                        if (newDist - minDist2 < 0)
                        {
                            // Find a "t" that is as large as possible while avoiding collision
                            float ta = 0;
                            float tb = 1;
                            for (int i = 0; i < 10; i++)
                            {
                                float tc = (ta + tb) / 2;
                                Vector2 ptC = new Vector2(MathHelper.Lerp(oldPos.X, oldTarget.X, tc),
                                                          MathHelper.Lerp(oldPos.Y, oldTarget.Y, tc));
                                float distC = line.DistanceSquaredPointToVirtualLine(ptC);
                                if (distC - minDist2 < 0)
                                    tb = tc;
                                else
                                    ta = tc;
                            }

                            if (ta < minT)
                            {
                                minT = ta;
                                minTLine = line;
                            }
                        }
                    }
                }

                // At this point, we've looped through all lines and found the minimum "t" value and its line

                if (minTLine == null)
                {
                    // No intersections were found, so move straight to the target
                    Debug.Assert(MinDistanceSquaredDeviation(lineList, oldTarget, lineRadius, discRadius) >= 0);
                    oldPos = oldTarget; // no further motion required
                }
                else
                {
                    // Collide and slide against minTLine
                    Vector2 newPos = new Vector2(MathHelper.Lerp(oldPos.X, oldTarget.X, minT),
                                                 MathHelper.Lerp(oldPos.Y, oldTarget.Y, minT));
                    Vector2 newTarget;

                    float minDist2 = (lineRadius + discRadius) * (lineRadius + discRadius);

                    // Refine minT / newPos til it passes the distance test
                    float minDistDeviation = MinDistanceSquaredDeviation(lineList, newPos, lineRadius, discRadius);
                    if (minDistDeviation < 0)
                    {
                        float ta = 0;
                        float tb = minT;
                        for (int i = 0; i < 10; i++)
                        {
                            float tc = (ta + tb) / 2;
                            Vector2 ptC = new Vector2(MathHelper.Lerp(oldPos.X, oldTarget.X, tc),
                                                      MathHelper.Lerp(oldPos.Y, oldTarget.Y, tc));
                            if (MinDistanceSquaredDeviation(lineList, ptC, lineRadius, discRadius) < 0)
                                tb = tc;
                            else
                                ta = tc;
                        }
                        minT = ta;
                        newPos = new Vector2(MathHelper.Lerp(oldPos.X, oldTarget.X, minT),
                                             MathHelper.Lerp(oldPos.Y, oldTarget.Y, minT));
                    }
                    Debug.Assert(MinDistanceSquaredDeviation(lineList, newPos, lineRadius, discRadius) >= 0);

                    // This is a bit of a hack to avoid "jiggling" when the disc is pressed
                    // against two walls that are at an obtuse angle.  Perhaps the real fix
                    // is to project the new slide vector against the original motion vector?
                    // In practice, we only ever need to slide once -- this fixes the issue.
                    bool doSlide = true;
                    if (pastFirstSlide)
                    {
                        newTarget = newPos;
                        doSlide = false;
                    }
                    else
                    {
                        pastFirstSlide = true;
                    }

                    if (doSlide)
                    {
                        Vector2 closestP;
                        float d2 = minTLine.DistanceSquaredPointToVirtualLine(newPos, out closestP);
                        RoundLine connectionLine = new RoundLine(newPos, closestP);
                        Vector2 lineNormal = (newPos - closestP);
                        lineNormal.Normalize();

                        // create a normal to the above line
                        // (which would thus be a tangent to minTLine)
                        float theta = connectionLine.Theta;
                        theta += MathHelper.PiOver2;
                        Vector2 newPoint = new Vector2(newPos.X + (float)Math.Cos(theta), newPos.Y + (float)Math.Sin(theta));

                        // Project the post-intersection line onto the above line, to provide "wall sliding" effect
                        // v1 dot v2 = |v2| * (projection of v1 onto v2), and |v2| is 1
                        Vector2 v1 = oldTarget - newPos;
                        Vector2 v2 = newPoint - newPos;
                        float dotprod = Vector2.Dot(v1, v2);

                        newTarget = newPos + dotprod * v2;
                        // newTarget should not be too close to minTLine
                        float newTargetDist = minTLine.DistanceSquaredPointToVirtualLine(newTarget);
                        if (newTargetDist - minDist2 < 0)
                        {
                            float shiftAmtA = 0; // not enough
                            float shiftAmtB = -(newTargetDist - minDist2) + 0.0001f; // too much
                            for (int i = 0; i < 10; i++)
                            {
                                float shiftAmtC = (shiftAmtA + shiftAmtB) / 2.0f;
                                Vector2 newTargetTest = newTarget + (shiftAmtC * lineNormal);
                                float newTargetTestDist = minTLine.DistanceSquaredPointToVirtualLine(newTargetTest);
                                if (newTargetTestDist - minDist2 >= 0)
                                    shiftAmtB = shiftAmtC;
                                else
                                    shiftAmtA = shiftAmtC;
                            }
                            newTarget += shiftAmtB * lineNormal;
                        }
                    }
                    else
                    {
                        newTarget = newPos; // No slide
                    }

                    // Get ready to loop around and see if we can move from newPos to newTarget
                    // without colliding with anything
                    oldPos = newPos;
                    oldTarget = newTarget;

                    Debug.Assert(minTLine.DistanceSquaredPointToVirtualLine(newPos) - minDist2 >= 0);
                }
            }

            // oldTarget == oldPos (or is very close), so no further moving/sliding is needed.
            finalPos = oldPos;
            Debug.Assert(MinDistanceSquaredDeviation(lineList, finalPos, lineRadius, discRadius) >= 0);
        }
    }
}