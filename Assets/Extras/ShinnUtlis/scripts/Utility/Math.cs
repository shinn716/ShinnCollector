﻿using System.Text;
using UnityEngine;

namespace Shinn
{
    public static class Math
    {
        /// <summary>
        /// 取偶數, 小數點第N位 (小數點最大第二位)
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static float GetEven(float input, int n = 1)
        {
            input = (float)((int)(input * Mathf.Pow(10, n)) / Mathf.Pow(10, n));
            input *= Mathf.Pow(10, n);
            return input % 2 == 0 ? input / Mathf.Pow(10, n) : (input + 1) / Mathf.Pow(10, n);
        }

        /// <summary>
        /// 取偶數
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static int GetEven(int input)
        {
            return input % 2 == 0 ? input : (input + 1);
        }

        /// <summary>
        /// 取 除數 數值
        /// </summary>
        /// <param name="input"></param>
        /// <param name="divisor"></param>
        /// <returns></returns>
        public static int GetCustomValue(int input, int divisor)
        {
            return input % divisor == 0 ? input : input - (input % divisor);
        }

        /// <summary>
        /// 兩四元數 求夾角 
        /// </summary>
        /// <param name="qBase"></param>
        /// <param name="qTarget"></param>
        /// <returns></returns>
        public static float GetQ2EulerAngle(Quaternion qBase, Quaternion qTarget)
        {
            Quaternion qDiff = Quaternion.Inverse(qBase) * qTarget;
            Vector3 vEuler = qDiff.eulerAngles;
            float yaw = vEuler.y;
            if (yaw > 180)
                yaw -= 360;
            return yaw;
        }
        
    }
}
