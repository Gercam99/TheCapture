using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace ExtensionsUnity
{
    public static class Extensions
{

    #region GameObjectExtensions

    /// <summary>
    /// Return a component after either finding it on the game object or otherwise attaching it
    /// </summary>
    /// <typeparam name="T">Component To Attatch</typeparam>
    /// <param name="gameObject"></param>
    /// <returns>Instance of the component</returns>
    public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
    {
        if (gameObject.TryGetComponent<T>(out T requestedComponent))
        {
            return requestedComponent;
        }

        return gameObject.AddComponent<T>();
    }

    #endregion

    #region TransformExtensions

    /// <summary>
    /// Destroy all children of this transform
    /// </summary>
    /// <param name="transform"></param>
    public static void DestroyChildren(this Transform transform)
    {
        foreach (Transform child in transform)
        {
            Object.Destroy(child.gameObject);
        }
    }

    #endregion

    #region Vector3Extensions
    
    /// <summary>
    /// Funcion encargada de clampear un vector.
    /// </summary>
    /// <param name="vector"></param>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    public static Vector3 ClampMagnitude(this Vector3 vector, float min, float max)
    {
        double sm = vector.sqrMagnitude;
        if (sm > (double)max * (double)max) return vector.normalized * max;
        else if (sm < (double) min * (double) min) return vector.normalized * min;
        return vector;
    }
    
    /// <summary>
    /// Funcion para saber el centro de las listas
    /// </summary>
    /// <param name="vectors"></param>
    /// <returns></returns>
    public static Vector3 CenterOfVectors(this List<Vector3> vectors)
    {
        Vector3 sum = Vector3.zero;

        if (vectors == null || vectors.Count == 0) return sum;

        foreach (var vector in vectors)
        {
            if (vector != null)
            {
                sum += vector;
            }
        }

        return sum / vectors.Count;
    }
    
    /// <summary>
    /// Return this vector with only its x and y components
    /// </summary>
    /// <param name="v"></param>
    /// <returns></returns>
    public static Vector2 ToVector2(this Vector3 v)
    {
        return new Vector2(v.x, v.y);
    }
    
            /// <summary>
        /// Return a copy of this vector with an altered x component
        /// </summary>
        /// <param name="v"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        public static Vector2 ChangeX(this Vector2 v, float x)
        {
            return new Vector2(x, v.y);
        }

        /// <summary>
        /// Return a copy of this vector with an altered y component
        /// </summary>
        /// <param name="v"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static Vector2 ChangeY(this Vector2 v, float y)
        {
            return new Vector2(v.x, y);
        }

        /// <summary>
        /// Return a copy of this vector with an altered x component
        /// </summary>
        /// <param name="v"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        public static Vector3 ChangeX(this Vector3 v, float x)
        {
            return new Vector3(x, v.y, v.z);
        }

        /// <summary>
        /// Return a copy of this vector with an altered y component
        /// </summary>
        /// <param name="v"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static Vector3 ChangeY(this Vector3 v, float y)
        {
            return new Vector3(v.x, y, v.z);
        }

        /// <summary>
        /// Return a copy of this vector with an altered z component
        /// </summary>
        /// <param name="v"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public static Vector3 ChangeZ(this Vector3 v, float z)
        {
            return new Vector3(v.x, v.y, z);
        }

        /// <summary>
        /// Return a Vector3 with this vector's components as well as the supplied z component
        /// </summary>
        /// <param name="v"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public static Vector3 ChangeZ(this Vector2 v, float z)
        {
            return new Vector3(v.x, v.y, z);
        }

    #endregion

    #region ListExtensions

    /// <summary>
    /// Return a random item from the list
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <returns></returns>
    public static T Random<T>(this IList<T> list)
    {
        // Pick a random item from the list and return it
        return list[UnityEngine.Random.Range(0, list.Count)];
    }

    /// <summary>
    /// Remove a random item from the list and returns it
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <returns></returns>
    public static T RemoveRandom<T>(this IList<T> list)
    {
        int indexToRemove = UnityEngine.Random.Range(0, list.Count);
        var item = list[indexToRemove];
        
        list.RemoveAt(indexToRemove);
        return item;
    }
    
    /// <summary>
    /// Shuffle the list using the Fisher–Yates algorithm
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    public static void Shuffle<T>(this IList<T> list)
    {
        var rng = new System.Random();

        for (int i = list.Count - 1; i > 1; i--)
        {
            int k = rng.Next(i);
            (list[k], list[i]) = (list[i], list[k]);
        }
    }

    #endregion

    #region ColorsExtensions

    private static int HexToDec(string hex)
    {
        int dec = System.Convert.ToInt32(hex, 16);
        return dec;
    }

    private static string DecToHex(int value)
    {
        return value.ToString("X2");
    }

    private static string FloatNormalizedToHex(float value)
    {
        return DecToHex(Mathf.RoundToInt(value * 255f));
    }

    private static float HexToFloatNormalized(string hex)
    {
        return HexToDec(hex) / 255f;
    }

    public static Color GetColorFromString(string hexString)
    {
        float red = HexToFloatNormalized(hexString.Substring(0, 2));
        float green = HexToFloatNormalized(hexString.Substring(2, 2));
        float blue = HexToFloatNormalized(hexString.Substring(4, 2));
        float alpha = 1f;
        if (hexString.Length >= 8)
        {
            alpha = HexToFloatNormalized(hexString.Substring(6, 2));
        }
        return new Color(red, green, blue, alpha);
    }

    public static string GetStringFromColor(Color color, bool useAlpha = false)
    {
        string red = FloatNormalizedToHex(color.r);
        string green = FloatNormalizedToHex(color.g);
        string blue = FloatNormalizedToHex(color.b);
        string alpha = FloatNormalizedToHex(color.a);

        string colorString = useAlpha ? red + green + blue + alpha : red + green + blue;
        return colorString;
    }

    #endregion

    #region MathfExtensions

    
    /// <summary>
    /// Animacion de bob con Mathf.Sin.
    /// </summary>
    /// <param name="bobFrequency">Unidad hasta donde ira</param>
    /// <param name="bobbingAmount">velocidad</param>
    /// <returns></returns>
    public static float BobbingAnimation(float bobFrequency, float bobbingAmount)
    {
        return ((Mathf.Sin(Time.time * bobFrequency) * 0.5f) + 0.5f) * bobbingAmount;
    }

    /// <summary>
    /// Funcion para resolver una regla de tres
    /// </summary>
    /// <param name="b"></param>
    /// <param name="c"></param>
    /// <param name="a"></param>
    /// <returns></returns>
    public static float RuleThree(float b, float c, float a)
    {
        return (b * c) / a;
    }
    
    /// <summary>
    /// Funcion para bloquear angulo de 0 to 180 to -180 to 0
    /// </summary>
    /// <param name="angle"></param>
    /// <returns></returns>
    public static float WrapAngle(float angle)
    {
        angle%=360;
        if(angle >180)
            return angle - 360;
 
        return angle;
    }

    /// <summary>
    /// Funcion para que no haya bloqueo en el angulo
    /// </summary>
    /// <param name="angle"></param>
    /// <returns></returns>
    public static float UnwrapAngle(float angle)
    {
        if(angle >=0)
            return angle;

        angle= - angle%360;
        return 360-angle;
    }

    #endregion

    #region NavMeshAgent

    /// <summary>
    /// Funcion encargada de comprobar si la IA ha llegado a su destion
    /// </summary>
    /// <param name="agent"></param>
    /// <returns></returns>
    public static bool HasArrived(this NavMeshAgent agent) => agent.remainingDistance < agent.stoppingDistance &&
                                                              !agent.pathPending;

    /// <summary>
    /// Funcion encargada de parar la IA
    /// </summary>
    /// <param name="agent"></param>
    public static void StopNavMesh(this NavMeshAgent agent) => agent.isStopped = true;

    /// <summary>
    /// Funcion encargada de que la IA vuelva a un punto
    /// </summary>
    /// <param name="agent"></param>
    public static void PlayNavMesh(this NavMeshAgent agent) => agent.isStopped = false;

    #endregion

    #region DictionaryExtensions

    public static T KeyByValue<T, W>(this Dictionary<T, W> dictionary, int value)
    {
        return dictionary.FirstOrDefault(x => Equals(x.Value, value)).Key;
    }
    #endregion

}
}

