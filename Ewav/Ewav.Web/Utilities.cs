/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       Utilities.cs
 *  Namespace:  Ewav.Web    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    27/05/2014    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Reflection;
using System.Collections.Generic;
using System.Text.RegularExpressions;


namespace Ewav.Web
{
    public class Utilities
    {
        public object DeSerializeObject(string deser)
        {
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream memStr = new MemoryStream(Convert.FromBase64String(deser));   //     .GetBytes(objectToSerialize));      //   GetBytes(objectToSerialize));

            try
            {
                object freqList = bf.Deserialize(memStr);
                return freqList;
            }
            finally
            {
                memStr.Close();
            }
        }

        public string SerializeObject<T>(T objectToSerialize)
        {
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream memStr = new MemoryStream();

            try
            {
                bf.Serialize(memStr, objectToSerialize);
                memStr.Position = 0;

                return Convert.ToBase64String(memStr.ToArray());
            }
            finally
            {
                memStr.Close();
            }
        }
        public static F Clone<F>(F original)
        {
            F tempMyClass = (F)Activator.CreateInstance(original.GetType());

            FieldInfo[] fis = original.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (FieldInfo fi in fis)
            {
                object fieldValue = fi.GetValue(original);
                if (fi.FieldType.Namespace != original.GetType().Namespace)
                    fi.SetValue(tempMyClass, fieldValue);
                else
                    fi.SetValue(tempMyClass, Clone(fieldValue));
            }

            return tempMyClass;
        }

        public static void CopyPropertiesTo(object targetObject, object sourceObject)
        {
            FieldInfo[] allSourceFields = sourceObject.GetType().GetFields();
            PropertyInfo targetProperty;

            Dictionary<string, string> dict = new Dictionary<string, string>();



            foreach (FieldInfo fromField in allSourceFields)
            {
                targetProperty = targetObject.GetType().GetProperty(fromField.Name);
                if (targetProperty == null) continue;
                if (!targetProperty.CanWrite) continue;

                dict.Add(fromField.Name, fromField.GetValue(sourceObject).ToString());

                //Type pType = targetProperty.PropertyType;
                var targetProp = targetProperty.GetValue(targetObject, null);
                var sourceProp = fromField.GetValue(sourceObject);

                //    targetProperty.SetValue(targetProperty, sourceProp, null);


            }

            //PropertyInfo[] allProps =  targetObject.GetType().GetProperties()    ;

            //foreach (PropertyInfo toProp   in allProps)
            //{
            //    if (toProp.Name ==  
            //}
        }


        public static DataTable Pivot(DataTable dataValues, string keyColumn, string pivotNameColumn, string pivotValueColumn)
        {
            DataTable tmp = new DataTable();
            DataRow r;
            string LastKey = "//dummy//";
            int i, pValIndex, pNameIndex;
            string s;
            bool FirstRow = true;

            pValIndex = dataValues.Columns[pivotValueColumn].Ordinal;
            pNameIndex = dataValues.Columns[pivotNameColumn].Ordinal;

            for (i = 0; i <= dataValues.Columns.Count - 1; i++)
            {
                if (i != pValIndex && i != pNameIndex)
                    tmp.Columns.Add(dataValues.Columns[i].ColumnName, dataValues.Columns[i].DataType);
            }

            r = tmp.NewRow();

            foreach (DataRow row1 in dataValues.Rows)
            {
                if (row1[keyColumn] != DBNull.Value)
                {
                    if (row1[keyColumn].ToString() != LastKey)
                    {
                        if (!FirstRow)
                            tmp.Rows.Add(r);

                        r = tmp.NewRow();
                        FirstRow = false;

                        //loop thru fields of row1 and populate tmp table
                        for (i = 0; i <= row1.ItemArray.Length - 3; i++)
                            r[i] = row1[tmp.Columns[i].ToString()];

                        LastKey = row1[keyColumn].ToString();
                    }

                    s = row1[pNameIndex].ToString();

                    if (!string.IsNullOrEmpty(s))
                    {
                        if (!tmp.Columns.Contains(s))
                            tmp.Columns.Add(s, typeof(int));// dataValues.Columns[pNameIndex].DataType);
                        r[s] = row1[pValIndex];
                    }
                }
            }

            //add that final row to the datatable:
            tmp.Rows.Add(r);
            return tmp;
        }



        public static string ToCamelCase(string pascalCaseString)
        {
            Regex r = new Regex("(?<=[a-z])(?<x>[A-Z])|(?<=.)(?<x>[A-Z])(?=[a-z])");
            return r.Replace(pascalCaseString, " ${x}");
        }



        public static Object ParseEnum<T>(string s)
        {
            try
            {
                var o = Enum.Parse(typeof(T), s, true);
                return (T)o;
            }
            catch (ArgumentException)
            {
                return null;
            }
        }


        public static void CopyPropertiesTox(object targetObject, object sourceObject)
        {
            PropertyInfo[] allSourceProporties = targetObject.GetType().GetProperties();
            PropertyInfo targetProperty;

            foreach (PropertyInfo fromProp in allSourceProporties)
            {
                targetProperty = targetObject.GetType().GetProperty(fromProp.Name);
                if (targetProperty == null) continue;
                if (!targetProperty.CanWrite) continue;

                //Type pType = targetProperty.PropertyType;
                var targetProp = targetProperty.GetValue(targetObject, null);
                var sourceProp = fromProp.GetValue(sourceObject, null);

                targetProperty.SetValue(targetProperty, sourceProp, null);

            }
        }
    }


}