using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using UnityEngine;

namespace MyFrameWork
{
    public class LogicMgr : Singleton<LogicMgr>
    {
        #region Init
        Dictionary<string, Texture2D> imgCrc_img;
        public override void Init()
        {
            base.Init();
            imgCrc_img = new Dictionary<string, Texture2D>();
        }
        #endregion
        #region WaitingPanel
        public void OnShowWaiting(int _nID, string _strTips, bool _bClock = false, int _nMax = 99)
        {
            Message msg = new Message(MsgType.Com_ShowWaiting, this);
            msg["id"] = _nID;
            msg["type"] = _bClock ? Defines.WaitingType_Clock : Defines.WaitingType_Percent;
            msg["tips"] = _strTips;
            msg["t"] = _nMax;
            msg.Send();
        }

        public void OnPopWaiting(int _nID)
        {
            Message msg = new Message(MsgType.Com_PopWaiting, this);
            msg["id"] = _nID;
            msg.Send();
        }

        public void OnPushWaiting(int _nID)
        {
            Message msg = new Message(MsgType.Com_PushWaiting, this);
            msg["id"] = _nID;
            msg.Send();
        }

        public void OnHideWaiting()
        {
            Message msg = new Message(MsgType.Com_HideWaiting, this);
            msg.Send();
        }
        #endregion

        #region Import Files
        private const string FILEFILTER = "Res|*.sb;*.assetbundle;*.fbx;*.png;*.jpg";
        public const string PicFileFilter = "jpg(*.jpg),png(*.png)|*.jpg;*.png;";
        /// <summary>
        /// 导入文件，统一入口
        /// </summary>
        /// <param name="_strFilter">后缀名过滤</param>
        /// <return></returns>
        public List<string> OnImportFiles(string _strFilter = FILEFILTER, string _strDefDir = null)
        {
            List<string> pRes = new List<string>();
            var ofd = new OpenFileDialog();
            ofd.Filter = _strFilter;
            ofd.Multiselect = true;
            ofd.CheckFileExists = true;
            ofd.ShowReadOnly = true;

            // 设置默认目录
            if (!string.IsNullOrEmpty(_strDefDir))
            {
                ofd.InitialDirectory = _strDefDir;
            }

            ofd.Title = "请选择需要导入的文件（可以多选,不能包含中文）";
            var ret = ofd.ShowDialog();

            if (ret == DialogResult.OK)
            {
                pRes = new List<string>(ofd.FileNames);
            }

            for (int i = 0; i < pRes.Count; i++)
            {
                string strFile = pRes[i];
                strFile = Utils.GetFileName(strFile);
                if (Utils.HasChinese(strFile))
                {
                    pRes.Remove(pRes[i]);
                }
            }

            return pRes;
        }

        /// <summary>
        /// 导入一个文件
        /// </summary>
        /// <param name="_strFilter"></param>
        /// <param name="_strDefDir"></param>
        /// <returns></returns>
        public string OnImportOneFile(string _strFilter = FILEFILTER, string _strDefDir = null)
        {
            string fileUrl = "";
            var ofd = new OpenFileDialog();
            ofd.Filter = _strFilter;
            ofd.Multiselect = false;

            // 设置默认目录
            if (!string.IsNullOrEmpty(_strDefDir))
            {
                ofd.InitialDirectory = _strDefDir;
            }

            ofd.Title = "请选择需要导入的文件(不能有中文)";
            var ret = ofd.ShowDialog();

            if (ret == DialogResult.OK)
            {
                fileUrl = ofd.FileName;
            }

            string strFile = Utils.GetFileName(fileUrl);
            if (Utils.HasChinese(strFile))
            {
                return string.Empty;
            }
            return fileUrl;
        }
        #endregion

        #region AlertWindow
        /// <summary>
        /// 提示框
        /// </summary>
        /// <param name="_content">内容</param>
        /// <param name="_strType">类型</param>
        /// <param name="_strTitle">标题</param>
        /// <param name="_cb">确认回调</param>
        public void OnAlert(object _content, string _strTitle = "提示框", Action _cb = null, Action _cancelCb = null,string _strType = Defines.AlertType_Single)
        {
            Message msg = new Message(MsgType.Win_Show, this);
            msg["type"] = _strType;
            msg["data"] = _content;
            msg["title"] = _strTitle;
            msg["cb"] = _cb;
            msg["fcb"] = _cancelCb;
            msg.Send();
        }
        #endregion

        #region Transform
        public void RemoveChildren(Transform _tf)
        {
            if (_tf != null)
            {
                for (int i = _tf.childCount -1; i >=0 ; i--)
                {
                    GameObject obj = _tf.GetChild(i).gameObject;
                    
                    GameObject.Destroy(obj);
                }
            }
        }
        #endregion

        #region Texture
        /// <summary>
        /// 下载图片资源
        /// </summary>
        /// <param name="_strImg"></param>
        /// <param name="_strImgCrc"></param>
        /// <param name="_cb"></param>
        /// <param name="_fCb"></param>
        public void OnGetTexture(string _strImg,string _strImgCrc,Action<Texture2D> _cb,Action<string> _fCb = null)
        {
            string strKey = _strImg + _strImgCrc;
            
            if (imgCrc_img.ContainsKey(strKey))
            {
                _cb(imgCrc_img[strKey]);
                return;
            }

            HttpService.GetDownloadURL(_strImg, _strImgCrc, url =>
            {
                HttpService.GetRemoteTexture(url, tex =>
                {
                    imgCrc_img.AddOrReplace(strKey,tex);
                    _cb(tex);

                }, true,(er)=> {
                    _fCb(er);
                });
            });
        }
        #endregion

        #region Material
        /// <summary>
        /// shader检查
        /// </summary>
        /// <param name="_obj"></param>
        public static void ResetStandardShader(GameObject _obj)
        {
            if (_obj != null)
            {
                MeshRenderer[] pMr = _obj.GetComponentsInChildren<MeshRenderer>();
                for (int i = 0; i < pMr.Length; i++)
                {
                    MeshRenderer mr = pMr[i];
                    Material[] pM = mr.materials;
                    for (int j = 0; j < pM.Length; j++)
                    {
                        Material m = pM[j];
                        string strShaderName = m.shader.name;
                        if (string.IsNullOrEmpty(strShaderName))
                        {
                            var newShader = Shader.Find("Standard");
                            m.shader = newShader;
                            Debug.Log("重置Shader:"+_obj.name);
                        }
                    }
                }
            }
        }
        #endregion
    }
}

