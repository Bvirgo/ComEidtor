
using System;
namespace MyFrameWork
{
	public class MsgType 
	{
        #region Test
        public static string Net_MessageTestOne = "Net_MessageTestOne";
        public static string Net_MessageTestTwo = "Net_MessageTestTwo";
        #endregion
        #region Common
        public const string Com_CloseUI = "Com_CloseUI";
        public const string Com_OpenUI = "Com_OpenUI";
        #endregion

        #region WaitingView
        public const string Com_ShowWaiting = "Com_ShowWaiting";
        public const string Com_NewWaiting = "Com_NewWating";
        public const string Com_HideWaiting = "Com_HideWaiting";
        public const string Com_UpdateWaiting = "Com_UpdateWaiting";
        public const string Com_PushWaiting = "Com_PushWaiting";
        public const string Com_PopWaiting = "Com_PopWaiting";
        #endregion

        #region AlertWindow
        public const string Win_Show = "Win_ShowWindow";
        public const string Win_ItemClick = "Win_ItemClick";
        public const string Win_Affirm = "Win_Affirm";
        public const string Win_Refresh = "Win_RefreshWindow";
        public const string Win_Finish = "Win_Finish";
        #endregion

        #region Login
        public const string LoginView_Login = "LoginView_Login";

        #endregion

        #region Component Editor
        public const string MainView_Show = "MainView_Show";
        public const string MainView_ReplaceAll = "MainView_ReplaceAll";
        public const string MainView_RefreshTag = "MainView_RefreshTagList";
        public const string MainView_RefreshCom = "MainView_RefreshComList";
        public const string MainView_TagItemClick = "MainView_TagItemClick";
        public const string MainView_ComItemClick = "MainView_ComItemClick";
        public const string MainView_LoadRes = "MainView_LoadRes";
        public const string MainView_Save = "MainView_Save";
        public const string MainView_NewComp = "MainView_NewComp";
        
        #endregion
    }
}

