using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unity_Utils
{
    public static class Extensions
    {
        public static void ResetTransform(this Transform trans)
        {
            trans.localPosition = Vector3.zero;
            trans.localRotation = Quaternion.identity;
            trans.localScale = Vector3.one;
        }
        
        public static void LerpPos(this Transform t, Vector3 targetPos, float maxDistanceDelta)
        {
            t.position = Vector3.Lerp(t.position, targetPos, maxDistanceDelta);
        }
        
        public static void MoveTowards(this Transform t, Vector3 targetPos, float maxDistanceDelta)
        {
            t.position = Vector3.MoveTowards(t.position, targetPos, maxDistanceDelta);
        }

        public static bool IsRange(this float t, float minValue, float maxValue)
        {
            if (t >= minValue && t <= maxValue) return true;
            return false;
        }
        
        public static bool IsRange(this int t, int minValue, int maxValue)
        {
            if (t >= minValue && t <= maxValue) return true;
            return false;
        }
        
        public static bool IsRange(this double t, double minValue, double maxValue)
        {
            if (t >= minValue && t <= maxValue) return true;
            return false;
        }
    }
}
