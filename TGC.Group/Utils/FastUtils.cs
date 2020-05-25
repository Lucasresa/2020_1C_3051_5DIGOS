using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TGC.Core.Direct3D;
using TGC.Core.Interpolation;
using TGC.Core.Mathematica;

namespace TGC.Group.Utils
{
    static class FastUtils
    {
        public struct Constants
        {
            public static (int width, int height) screen = (width: D3DDevice.Instance.Device.Viewport.Width, height: D3DDevice.Instance.Device.Viewport.Height);
        }

        public static float AngleBetweenVectors(TGCVector3 vectorA, TGCVector3 vectorB)
        {
            var dotProduct = TGCVector3.Dot(vectorA, vectorB) / (vectorA.Length() * vectorB.Length());
            if (dotProduct < 1)
                return FastMath.Acos(dotProduct);
            else
                return 0;
        }

        public static bool IsNumberBetweenInterval(float number, TGCVector2 interval)
        {
            return number > interval.X && number < interval.Y;
        }

        public static TGCVector3 ObtainNormalVector(TGCVector3 vectorA, TGCVector3 vectorB)
        {
            return TGCVector3.Normalize(TGCVector3.Cross(vectorA, vectorB));
        }

        public static bool LessThan(float numberA, float numberB)
        {
            return numberA < numberB;
        }

        public static bool GreaterThan(float numberA, float numberB)
        {
            return numberA > numberB;
        }

        public static bool Contains(string expression, string searchExpression)
        {
            return expression.ToLower().Contains(searchExpression);
        }

        public static bool IsDistanceBetweenVectorsLessThan(float distance, TGCVector3 vectorA, TGCVector3 vectorB)
        {
            return (vectorA - vectorB).Length() < distance;
        }

        public static float DistanceBetweenVectors(TGCVector3 vectorA, TGCVector3 vectorB)
        {
            return (vectorA - vectorB).Length();
        }

        public static float Distance(float numberA, float numberB)
        {
            return FastMath.Abs(numberB - numberA);
        }

        public static int CountOcurrences(List<string> list, string expressionSearch)
        {
            return list.Count(expression => expression.Contains(expressionSearch));
        }
    }
}
