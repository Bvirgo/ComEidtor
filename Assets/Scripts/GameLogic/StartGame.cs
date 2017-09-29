using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MyFrameWork;
using System;
using Jhqc.EditorCommon;

public class StartGame : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {
        // 加载数据模块
		RegisterAllModules();

        // 初始化网络
        InitNet();

        // 打开指定UI
		UIManager.Instance.OpenUI(UIType.Login,true);
	}

	private void RegisterAllModules()
	{
        LoadModule(typeof(WaitingModule));

        LoadModule(typeof(LoginModule));

        LoadModule(typeof(MainModule));

        LoadModule(typeof(WindowModule));

		//.....add
	}

    /// <summary>
    /// 初始化网络
    /// </summary>
    private void InitNet()
    {
        WWWManager.Instance.Init(Defines.ServerAddress, Jhqc.EditorCommon.LogType.None);// 外网
        WWWManager.Instance.TimeOut = 600f;
    }

    /// <summary>
    /// 创建指定M，初始化
    /// </summary>
    /// <param name="moduleType"></param>
	private void LoadModule(Type moduleType)
	{
		BaseModule bm = System.Activator.CreateInstance(moduleType) as BaseModule;
		bm.Load();
	}
}
