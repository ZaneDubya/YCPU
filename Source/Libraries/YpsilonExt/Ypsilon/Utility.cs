using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Ypsilon
{
    class Utility
    {
        public const float Pi = (float)Math.PI;
        public const float PiTwo = (float)(Math.PI * 2);
        public const float DegToRad = (float)(PiTwo / 360f);
        public const float RadToDeg = (float)(360f / PiTwo);
        public const float KPH_Per_TimeUnit = (float)(5f / 3600f);
        public const float KPH_LngLat_Per_TimeUnit = (float)(1f / 111.320f) * (5f / 3600f);
        public const float FuelConsumption_Per_TimeUnit = (float)(.01f * (5f / 600f));
        public const float FuelConsumption_Per_KM = (float)(6f / 100f);
        public const float LngLatToKM = (1f / 111.320f);
        public const float EarthRadius = 6378.1f;

        public static Matrix ProjectionMatrixUI
        {
            get { return Matrix.CreateOrthographicOffCenter(0f, 1280f, 720f, 0f, -1000f, 3000f); }
        }

        public static Matrix ProjectionMatrixScreen
        {
            get { return Matrix.CreateOrthographicOffCenter(0f, 1280f, 720f, 0f, -2000f, 2000f); }
        }

        public static Matrix ProjectionMatrixWorld
        {
            get { return Matrix.CreateOrthographicOffCenter(-640f, 640f, 360f, -360f, -2000f, 2000f); }
        }

        static float _FPS = 0, _Frames = 0, _ElapsedSeconds = 0;
        public static float FPS { get { return (float)Math.Round(_FPS, 2); } }
        // Maintain an accurate count of frames per second.
        public static bool UpdateFPS(GameTime gameTime)
        {
            _Frames++;
            _ElapsedSeconds += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (_ElapsedSeconds >= .5f)
            {
                _FPS = _Frames / _ElapsedSeconds;
                _ElapsedSeconds = 0;
                _Frames = 0;
                return true;
            }
            return false;
        }

        public const float CursorSize = 20f;

        static Random _random;
        public static void RandomSeedSet(int seed)
        {
            _random = null;
            _random = new Random(seed);
        }

        public static int RandomValue(int low, int high)
        {
            if (_random == null)
                _random = new Random();
            return _random.Next(high - low + 1) + low;
        }

        static int _lastSerial = 0;
        public static Serial NextSerial { get { return ++_lastSerial; } }

        /*public static Vector3 Legacy_MakeWorldVector(ref short i, ref short j)
        {
            int j1 = (j < 0) ? j + 2880 : j;
            float R = Data.Legacy.Trigonometry.CosineUnit1[j1];
            return new Vector3(
                Data.Legacy.Trigonometry.CosineUnit1[i] * R,
                Data.Legacy.Trigonometry.SineUnit1[j1],
                -Data.Legacy.Trigonometry.SineUnit1[i] * R);
        }*/

        public static float RadiusAtLatitude(float latitude)
        {
            float j1 = ((latitude < 0) ? latitude + 360f : latitude);
            float r = (float)Math.Abs(Math.Cos(j1 * DegToRad));
            if (r == 0)
                return 1;
            else
                return r;
        }

        public static Vector3 MakeWorldVector(float longitude, float latitude)
        {
            float x = ((longitude < 0) ? longitude + 360f : longitude);
            float y = ((latitude < 0) ? latitude + 360f : latitude);
            float r = RadiusAtLatitude(y);
            return new Vector3(
                (float)Math.Cos(x * DegToRad) * r,
                (float)Math.Sin(y * DegToRad),
                -(float)Math.Sin(x * DegToRad) * r);
        }

        public static float WorldLongitude_From_RealLongitude(int degrees, int minutes, int seconds)
        {
            float d = (degrees > 0) ?
                (degrees) + (minutes + (seconds / 60f)) / 60f :
                (degrees + 360) - (minutes + (seconds / 60f)) / 60f;
            return d;
        }

        // const float _world_Lat_Multiplier = 180f / 1440f;
        public static float LegacyLat_FromRealLat(int degrees, int minutes, int seconds)
        {
            float d = (degrees > 0) ?
                (degrees + 90) + (minutes + (seconds / 60f)) / 60f :
                degrees - (minutes + (seconds / 60f)) / 60f;
            return d;
        }

        public static void GetCoords_FromPointOnRotatedSphere(Vector3 spherePoint, Matrix sphereRotation, out Vector2 longlat, out Vector3 unrotatedPoint)
        {
            // calculate the inverse rotation matrix for this point:
            Matrix mInverseRotation = Matrix.Invert(sphereRotation);

            // Transform our spherePosition to the unrotatedPosition:
            Vector3 position = Vector3.Transform(spherePoint, mInverseRotation);

            // Get the longitude, latitude, and radius:
            float y = (float)(Math.Asin(position.Y));
            float latitude = y * RadToDeg;
            float R = (float)Math.Cos(y);
            float z = -(float)(Math.Asin(position.Z / R));
            float longitude = (z * RadToDeg);
            if (position.X < 0)
            {
                if (longitude < 0)
                    longitude = -(180f + longitude);
                else
                    longitude = 180f - longitude;
            }
            // return our 'out' variables:
            longlat = new Vector2(longitude, latitude);
            unrotatedPoint = position;
        }
    }
}
