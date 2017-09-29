using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyFrameWork;
using System;
using Jhqc.EditorCommon;

public class JHCQHelper : Singleton<JHCQHelper>
{
    public override void Init()
    {
        base.Init();

    }
    #region SD材质模板
    public MatManager.MatCacheItem m_curMatItem;

    /// <summary>
    /// 记录模型 和 材质
    /// </summary>
    private Dictionary<GameObject, MatManager.MatCacheItem> m_dicObjAndMat = new Dictionary<GameObject, MatManager.MatCacheItem>();
    public MatManager.MatCacheItem GetMatByObj(GameObject _obj)
    {
        if (m_dicObjAndMat.ContainsKey(_obj))
        {
            return m_dicObjAndMat[_obj];
        }

        return null;
    }

    public void AddObjAndMat(GameObject _obj, MatManager.MatCacheItem _mat)
    {
        if (m_dicObjAndMat.ContainsKey(_obj))
        {
            m_dicObjAndMat.Remove(_obj);
        }

        m_dicObjAndMat.Add(_obj, _mat);
    }

    /// <summary>记录每个材质的每一个属性的ProceduralPropertyDescription</summary>
    public Dictionary<string, Dictionary<string, ProceduralPropertyDescription>> Mat2ParamName2DesDic = new Dictionary<string, Dictionary<string, ProceduralPropertyDescription>>();

    public void RecordMatPropName(MatManager.MatCacheItem _mat)
    {
        if (_mat == null)
            return;
        //记录每个subMat的每个属性的描述, 用于面板显示
        string matName = Utils.RemovePostfix_Instance(_mat.Material.name);
        foreach (var ppd in _mat.Material.GetProceduralPropertyDescriptions())
        {
            Mat2ParamName2DesDic.TryAddNoReplace(matName, new Dictionary<string, ProceduralPropertyDescription>());
            Mat2ParamName2DesDic[matName].AddRep(ppd.name, ppd);
        }
    }

    #endregion
    #region 材质实时加载
    private Dictionary<string, MatManager.MatCacheItem> m_dicMat = new Dictionary<string, MatManager.MatCacheItem>();

    public void GetMatByName(string _strName, Action<MatManager.MatCacheItem> _cb, Action _failCb = null)
    {
        if (m_dicMat.ContainsKey(_strName))
        {
            _cb(m_dicMat[_strName]);
        }
        else
        {
            MaterialInfo mif = GetMatInfo(_strName);
            if (null == mif)
            {
                _cb(null);
            }

            MatManager.Instance.LoadMat(_strName, mci =>
            {

                //AddMat(_strName, mci);
                RecordMatPropName(mci);

                _cb(mci);
            });
            MatManager.Instance.OnLoadError = (strError) =>
            {
                if (_failCb != null)
                {
                    _failCb();
                }
            };
        }
    }

    public void AddMat(string _strName, MatManager.MatCacheItem _mat)
    {
        if (!m_dicMat.ContainsKey(_strName))
        {
            m_dicMat.Add(_strName, _mat);
        }
    }

    public MaterialInfo GetMatInfo(string _strName)
    {
        List<MaterialInfo> pMInfo = MatManager.Instance.AllMatInfo;
        if (pMInfo != null)
        {
            for (int i = 0; i < pMInfo.Count; ++i)
            {
                if (pMInfo[i].name.Equals(_strName))
                {
                    return pMInfo[i];
                }
            }
        }

        return null;
    }

    /// <summary>
    /// 获取指定Tag下所有材质信息
    /// </summary>
    /// <param name="_strName"></param>
    /// <returns></returns>
    public List<MaterialInfo> GetAllMatInfoByTag(string _strName)
    {
        List<MaterialInfo> pRes = new List<MaterialInfo>();
        List<MaterialInfo> pMInfo = MatManager.Instance.AllMatInfo;
        for (int i = 0; i < pMInfo.Count; ++i)
        {
            string[] p = pMInfo[i].tags.Split(',');
            List<string> pList = new List<string>(p);
            if (pList.Contains(_strName))
            {
                pRes.Add(pMInfo[i]);
            }
        }
        return pRes;
    }
    #endregion
}
