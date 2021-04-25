using System;
using System.Collections.Generic;
using System.Reflection;
using System.ComponentModel;

namespace Common.Library
{
    /// <summary>
    /// 枚举帮助类
    /// </summary>
    public static class EnumHelper
    {
        /// <summary>
        /// 将枚举转为集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<EnumEntity> EnumToList<T>()
        {
            List<EnumEntity> list = new List<EnumEntity>();
            foreach (var e in Enum.GetValues(typeof(T)))
            {
                EnumEntity m = new EnumEntity();
                object[] objArr = e.GetType().GetField(e.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), true);
                if (objArr != null && objArr.Length > 0)
                {
                    DescriptionAttribute da = objArr[0] as DescriptionAttribute;
                    m.Desction = da.Description;
                }
                m.EnumValue = Convert.ToInt32(e);
                m.EnumName = e.ToString();
                list.Add(m);
            }
            return list;
        }

        /// <summary>
        /// 获取枚举所有成员名称
        /// </summary>
        /// <typeparam name="T">枚举名,比如Enum1</typeparam>
        public static string[] GetMemberNames<T>()
        {
            return Enum.GetNames(typeof(T));
        }

        /// <summary>
        /// 获取枚举所有成员值
        /// </summary>
        /// <typeparam name="T">枚举名,比如Enum1</typeparam>
        public static Array GetMemberValues<T>()
        {
            return Enum.GetValues(typeof(T));
        }

        /// <summary>
        /// 返回指定枚举类型的指定值的描述
        /// </summary>
        /// <param name="t">枚举类型</param>
        /// <param name="v">枚举值</param>
        /// <returns></returns>
        public static string GetDescription(System.Type t, object v)
        {
            try
            {
                FieldInfo fi = t.GetField(GetName(t, v));
                DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
                return (attributes.Length > 0) ? attributes[0].Description : GetName(t, v);
            }
            catch
            {
                return "UNKNOWN";
            }
        }

        /// <summary>
        /// 获取枚举描述
        /// </summary>
        /// <param name="enumValue"></param>
        /// <returns></returns>
        public static string GetDescription(this Enum enumValue)
        {
            try
            {
                string str = enumValue.ToString();
                System.Reflection.FieldInfo field = enumValue.GetType().GetField(str);
                object[] objs = field.GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false);
                if (objs == null || objs.Length == 0) return str;
                System.ComponentModel.DescriptionAttribute da = (System.ComponentModel.DescriptionAttribute)objs[0];
                return da.Description;
            }
            catch
            {
                return "UNKNOWN";
            }
        }

        /// <summary>
        /// 返回指定枚举类型的指定值的名称
        /// </summary>
        /// <param name="t">指定枚举类型</param>
        /// <param name="v">指定值</param>
        /// <returns></returns>
        private static string GetName(System.Type t, object v)
        {
            try
            {
                return Enum.GetName(t, v);
            }
            catch
            {
                return "UNKNOWN";
            }
        }
    }
    /// <summary>
    /// 枚举实体
    /// </summary>
    public class EnumEntity
    {
        /// <summary>
        /// 枚举的描述
        /// </summary>
        public string Desction { set; get; }
        /// <summary>
        /// 枚举名称
        /// </summary>
        public string EnumName { set; get; }
        /// <summary>
        /// 枚举对象的值
        /// </summary>
        public int EnumValue { set; get; }
    }
}
