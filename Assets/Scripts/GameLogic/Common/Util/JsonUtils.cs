using UnityEngine;
using System.IO;
using System;
using LitJson;
using System.Reflection;
using System.Collections.Generic;

public static class JsonUtils
{
    public static JsonData EmptyJsonArray
    {
        get
        {
            return JsonMapper.ToObject("[]");
        }
    }

    public static JsonData EmptyJsonObject
    {
        get
        {
            return JsonMapper.ToObject("{}");
        }
    }

    #region [LitJson]自动序列化

    public static void FieldToJson(object tar, FieldInfo fi, JsonData jd, string paramName)
    {
        if (fi.FieldType == typeof(string))
            jd[paramName] = fi.GetValue(tar) as string;
        else if (fi.FieldType == typeof(int))
            jd[paramName] = (int)fi.GetValue(tar);
        else if (fi.FieldType == typeof(float))
            jd[paramName] = (float)fi.GetValue(tar);
        else if (fi.FieldType == typeof(bool))
            jd[paramName] = (bool)fi.GetValue(tar);
        else if (fi.FieldType == typeof(Vector3))
            jd[paramName] = JsonUtils.vecToStr(((Vector3)fi.GetValue(tar)));
        else if (fi.FieldType == typeof(Bounds))
            jd[paramName] = ((Bounds)fi.GetValue(tar)).ToJsonData();
    }

    public static void JsonToField(object tar, FieldInfo fi, JsonData jd, string paramName)
    {
        if (fi.FieldType == typeof(string))
            fi.SetValue(tar, jd.ReadString(paramName));
        else if (fi.FieldType == typeof(int))
            fi.SetValue(tar, jd.ReadInt(paramName));
        else if (fi.FieldType == typeof(float))
            fi.SetValue(tar, jd.ReadFloat(paramName));
        else if (fi.FieldType == typeof(bool))
            fi.SetValue(tar, jd.ReadBool(paramName));
        else if (fi.FieldType == typeof(Vector3))
            fi.SetValue(tar, jd.ReadVec3(paramName));
        else if (fi.FieldType == typeof(Bounds))
            fi.SetValue(tar, JsonToBounds(jd.ReadJsonData(paramName)));
    }

    #endregion


    /// <summary>读取文件中的json</summary>
    public static JsonData ReadJsonFile(string path)
    {
        if (File.Exists(path))
        {
            //string str = File.ReadAllText(path);
            string str = File.ReadAllText(path, System.Text.Encoding.UTF8);
            try
            {
                return JsonMapper.ToObject(str);
            }
            catch (Exception e)
            {
                Debug.Log("读取Json失败:" + path + " 中的内容不是Json格式!!!");
            }
        }
        else
        {
            Debug.Log("读取Json失败, 找到不文件: " + path);
        }

        return null;
    }

    public static UnityEngine.Color ReadColor(this JsonData jd, string key)
    {
        UnityEngine.Color color = UnityEngine.Color.white;
        if (jd.Keys.Contains(key) && jd[key] != null)
        {
            string colorStr = jd.ReadString(key);
            color = StringUtil.StringToColor4(colorStr);
        }

        return color;
    }

    public static float ReadFloat(this JsonData jd, string key, float defaultValue = 0)
    {
        float res;
        if (jd.Keys.Contains(key) && jd[key] != null && float.TryParse(jd[key].ToString(), out res))
            return res;
        else
            return defaultValue;
    }
    public static int ReadInt(this JsonData jd, string key, int defaultValue = 0)
    {
        int res;
        if (jd.Keys.Contains(key) && jd[key] != null && int.TryParse(jd[key].ToString(), out res))
            return res;
        else
            return defaultValue;
    }

    public static JsonData ReadJsonData(this JsonData jd, string key)
    {
        if (jd.Keys.Contains(key) && jd[key] != null)
            return jd[key];
        else
            return null;
    }

    public static bool ReadBool(this JsonData jd, string key, bool defaultValue = false)
    {
        if (jd.Keys.Contains(key) && jd[key] != null)
            return jd[key].ToString() == "true"
                || jd[key].ToString() == "是"
                || jd[key].ToString() == "True"
                || jd[key].ToString() == "TRUE";
        else
            return defaultValue;
    }

    public static string ReadString(this JsonData jd, string key, string defaultValue = null)
    {
        if (jd.Keys.Contains(key) && jd[key] != null)
            return jd[key].ToString();
        else
            return defaultValue;
    }

    public static Dictionary<T1, T2> ReadDic<T1, T2>(this JsonData jd, string key, Dictionary<T1, T2> defaultValue = null)
    {

        if (jd.Keys.Contains(key) && jd[key] != null)
            return JsonMapper.ToObject<Dictionary<T1, T2>>(jd[key].ToJson());
        else
            return defaultValue;
    }

    #region writeJsonData
    public static JsonData WriteJsonData(this JsonData jd, string key, int value)
    {
        jd[key] = int.Parse(value.ToString());
        return jd;
    }

    public static JsonData WriteJsonData(this JsonData jd, string key, float value)
    {
        jd[key] = float.Parse(value.ToString());
        return jd;
    }

    public static JsonData WriteJsonData(this JsonData jd, string key, string value)
    {
        jd[key] = value.ToString();
        return jd;
    }

    public static JsonData WriteJsonData(this JsonData jd, string key, IJsonData value)
    {
        jd[key] = value.ToJsonData();
        return jd;
    }

    public static JsonData WriteJsonData(this JsonData jd, string key, JsonData value)
    {
        jd[key] = value;
        return jd;
    }

    #endregion

    public static void DicChange(Dictionary<string, float> src, Dictionary<string, float> delta, bool isAdd)
    {
        foreach (var key in delta.Keys)
        {
            if (!src.ContainsKey(key))
                src.Add(key, 0);
            float change = isAdd ? delta[key] : -delta[key];
            src[key] = src[key] + change;
        }
    }

    #region dic.ToJsonData

    public static JsonData ToJsonData(this Dictionary<string, int> dic)
    {
        JsonData jd = new JsonData();
        if (dic.Keys.Count == 0)
            jd = EmptyJsonObject;
        else
        {
            foreach (var key in dic.Keys)
            {
                jd.WriteJsonData(key, dic[key]);
            }
        }
        return jd;
    }
    public static JsonData ToJsonData(this Dictionary<string, float> dic)
    {
        JsonData jd = new JsonData();
        if (dic.Keys.Count == 0)
            jd = EmptyJsonObject;
        else
        {
            foreach (var key in dic.Keys)
            {
                jd.WriteJsonData(key, dic[key]);
            }
        }
        return jd;
    }
    public static JsonData ToJsonData(this Dictionary<string, string> dic)
    {
        JsonData jd = new JsonData();
        if (dic.Keys.Count == 0)
            jd = EmptyJsonObject;
        else
        {
            foreach (var key in dic.Keys)
            {
                jd.WriteJsonData(key, dic[key]);
            }
        }
        return jd;
    }
    public static JsonData ToJsonData<T2>(this Dictionary<string, T2> dic) where T2 : IJsonData
    {
        JsonData jd = new JsonData();
        if (dic.Keys.Count == 0)
            jd = EmptyJsonObject;
        else
        {
            foreach (var key in dic.Keys)
            {
                jd.WriteJsonData(key, dic[key]);
            }
        }
        return jd;
    }
    public static JsonData ToJsonData(this Dictionary<string, JsonData> dic)
    {
        JsonData jd = new JsonData();
        if (dic.Keys.Count == 0)
            jd = EmptyJsonObject;
        else
        {
            foreach (var key in dic.Keys)
            {
                jd.WriteJsonData(key, dic[key]);
            }
        }
        return jd;
    }



    #endregion

    public static Vector3 ReadVec3(this JsonData jd, string key, Vector3 defaultValue = default(Vector3))
    {
        if (jd.Keys.Contains(key) && jd[key] != null)
            return JsonToVec3(jd[key]);
        else
            return defaultValue;
    }

    public static Vector2 ReadVector2(this JsonData jd, string key, Vector2 defaultValue = default(Vector2))
    {
        if (jd.Keys.Contains(key) && jd[key] != null)
            return StringUtil.StringToVector2(jd.ReadString(key));
        else
            return defaultValue;
    }
    public static Vector3 ReadVector3(this JsonData jd, string key, Vector3 defaultValue = default(Vector3))
    {
        if (jd.Keys.Contains(key) && jd[key] != null)
            return StringUtil.StringToVector3(jd.ReadString(key));
        else
            return defaultValue;
    }
    public static Vector4 ReadVector4(this JsonData jd, string key, Vector4 defaultValue = default(Vector4))
    {
        if (jd.Keys.Contains(key) && jd[key] != null)
            return StringUtil.StringToVector4(jd.ReadString(key));
        else
            return defaultValue;
    }


    public static T ReadEnum<T>(this JsonData jd, string key)
    {
        if (jd.Keys.Contains(key) && jd[key] != null)
            return (T)Enum.Parse(typeof(T), jd[key].ToString(), true);
        else
            return default(T);
    }

    public static Vector3 JsonToVec3(JsonData jd)
    {
        return StringToVector3(jd.ToString());
    }

    public static Bounds JsonToBounds(JsonData jd)
    {
        return new Bounds(jd.ReadVec3("center"), jd.ReadVec3("size"));
    }
    public static JsonData ToJsonData(this Bounds bound)
    {
        JsonData jd = new JsonData();
        jd["center"] = vecToStr(bound.center);
        jd["size"] = vecToStr(bound.size);
        return jd;
    }

    public static Vector3 StringToVector3(string str)
    {
        string[] floatArr = str.Split(new Char[] { ',' });
        return new Vector3(float.Parse(floatArr[0]), float.Parse(floatArr[1]), float.Parse(floatArr[2]));
    }

    public static string vecToStr(Vector3 vec)
    {
        return vec.x.ToString("f3") + "," + vec.y.ToString("f3") + "," + vec.z.ToString("f3");
    }


    public static JsonData ToJsonData(this Rect rect)
    {
        JsonData jd = new JsonData();
        jd["x"] = rect.x;
        jd["y"] = rect.y;
        jd["width"] = rect.width;
        jd["height"] = rect.height;
        return jd;
    }

    public static JsonData ToJsonData(object obj)
    {
        return JsonMapper.ToObject(JsonMapper.ToJson(obj));
    }

    public static T ToItemVO<T>(this JsonData jd) where T : new()
    {
        //判断T是否是IJsonData, 若是则用ReadJsonData解析
        T tar = new T();
        if (tar is IJsonData)
        {
            (tar as IJsonData).ReadJsonData(jd);
            return tar;
        }
        else
            return JsonMapper.ToObject<T>(JsonMapper.ToJson(jd));
    }

    public static List<T> ToItemVOList<T>(this JsonData jd) where T : new()
    {
        List<T> list = new List<T>();
        for (int i = 0, length = jd.Count; i < length; i++)
        {
            list.Add(ToItemVO<T>(jd[i]));
        }
        return list;
    }

    public static List<Vector3> ToVec3List(this JsonData jd)
    {
        List<Vector3> list = new List<Vector3>();
        for (int i = 0, length = jd.Count; i < length; i++)
        {
            list.Add(JsonToVec3(jd[i]));
        }
        return list;
    }
    public static List<string> ToStringList(this JsonData jd)
    {
        List<string> list = new List<string>();
        for (int i = 0, length = jd.Count; i < length; i++)
        {
            list.Add(jd[i].ToString());
        }
        return list;
    }

    public static List<Bounds> ToBoundsList(this JsonData jd)
    {
        List<Bounds> list = new List<Bounds>();
        for (int i = 0, length = jd.Count; i < length; i++)
        {
            list.Add(JsonToBounds(jd[i]));
        }
        return list;
    }

    public static JsonData ToJsonDataList<T>(this ICollection<T> list) where T : IJsonData
    {
        JsonData jd = ToJsonData(new List<int>());
        foreach (var item in list)
        {
            jd.Add((item as IJsonData).ToJsonData());
        }
        return jd;
    }

    public static JsonData ToJsonDataDic<T1, T2>(this Dictionary<T1, T2> dic) where T2 : IJsonData
    {
        JsonData jd = new JsonData();
        foreach (var pair in dic)
        {
            string key = pair.Key.ToString();
            JsonData value = pair.Value.ToJsonData();
            jd[key] = value;
        }
        return jd;
    }

    public static bool SetJsonProp<T>(object tar, JsonData jd, string propName, string jsonKey, Func<JsonData, T> dataProcess = null)
    {
        if (jd.Keys.Contains(jsonKey))
        {
            JsonData valueJD = jd[jsonKey];
            PropertyInfo prop = null;
            foreach (PropertyInfo p in tar.GetType().GetProperties())
            {
                if (p.Name == propName)
                {
                    prop = p;
                    break;
                }
            }
            if (prop != null)
            {
                if (dataProcess == null)
                {
                    if (typeof(T) == typeof(string))
                        prop.SetValue(tar, JsonMapper.ToJson(valueJD), null);
                    else if (typeof(T) == typeof(int))
                        prop.SetValue(tar, int.Parse(JsonMapper.ToJson(valueJD)), null);
                    else if (typeof(T) == typeof(float))
                        prop.SetValue(tar, float.Parse(JsonMapper.ToJson(valueJD)), null);
                    else if (typeof(T) == typeof(Vector3))
                        prop.SetValue(tar, JsonToVec3(valueJD), null);
                    else
                    {
                        //Debug.Log("SetJsonProp failed! no fitful default dataProcess(string, int, float, Vector3 ) ");
                        return false;
                    }
                }
                else
                    prop.SetValue(tar, dataProcess(valueJD), null);
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            //Debug.Log("SetJsonProp failed! can not find jsonKey: [" + jsonKey + "] in JsonData : \r\n"+JsonMapper.ToJson(jd));
            return false;
        }

    }
}
public interface IJsonData
{
    JsonData ToJsonData();
    IJsonData ReadJsonData(JsonData jd);
}
