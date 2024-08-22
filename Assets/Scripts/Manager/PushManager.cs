// using UnityEngine;
// using System;
// using System.Collections.Generic;

// //#if UNITY_ANDROID
// using Unity.Notifications.Android;
// //#endif

// #if UNITY_IOS
// using UnityEngine.iOS;
// #endif
// public enum EPushConditionType
// {
//     None = 0,
//     Return,
// }

// public partial class PushManager : MonoObjectSingletonInScene<PushManager>
// {
//     //private EPushOnOff m_pushOnOff = EPushOnOff.None;

//     //private const string PLAYERPREFS_PUSH_ONOFF = "playerprefs_push_onoff";

//     //public EPushOnOff PushOnOff
//     //{
//     //    get
//     //    {
//     //        if( m_pushOnOff == EPushOnOff.None )
//     //        {
//     //            int index = PlayerPrefs.GetInt( PLAYERPREFS_PUSH_ONOFF, 1 );
//     //            m_pushOnOff = ( EPushOnOff )index;
//     //        }

//     //        return m_pushOnOff;
//     //    }

//     //    set
//     //    {
//     //        m_pushOnOff = value;
//     //        PlayerPrefs.SetInt( PLAYERPREFS_PUSH_ONOFF, ( int )m_pushOnOff );
//     //    }
//     //}

//     public class PushInfo
//     {
//         public double remainTime;
//         public string title;
//         public string message;

//         public PushInfo(double _remainTime, string _title, string _message)
//         {
//             remainTime = _remainTime;
//             title = _title;
//             message = _message;
//         }
//     }

//  //   private bool bIsFirst = true;
//     private int m_maxCount = 50;
//     void Start()
//     {
// #if UNITY_ANDROID
//         // 채널 등록
//         var c = new AndroidNotificationChannel()
//         {
//             Id = "com.risingwings.dollygo",
//             Name = "Dollygo Channel",
//             Importance = Importance.Low,
//             Description = "Generic notifications",

//         };

//         AndroidNotificationCenter.RegisterNotificationChannel( c );
// #endif
//         ClearPush();
//         //m_maxCount = DataManager.Instance.GetPushMaxCount();
//     }

//     void OnApplicationQuit()
//     {
//     }

//     void OnApplicationPause(bool isPause)
//     {
//         //#if !UNITY_IPHONE
//         //        if( bIsFirst )
//         //        {
//         //            bIsFirst = false;
//         //            Return();

//         //            return;
//         //        }
//         //#endif
//         if( isPause )
//         {
//             MakePush();
//         }
//         else
//         {
//             Return();
//         }
//     }

//     private void Return()
//     {
//         ClearPush();
//     }

//     private void ClearPush()
//     {
// #if UNITY_EDITOR
//         Debug.Log( "Push - ClearPush" );
// #elif UNITY_IPHONE
//         Debug.Log("Push - ClearPush_IOS"); 
// 		UnityEngine.iOS.NotificationServices.ClearRemoteNotifications();
// 		UnityEngine.iOS.NotificationServices.ClearLocalNotifications();
// 		UnityEngine.iOS.NotificationServices.CancelAllLocalNotifications();
// #elif UNITY_ANDROID
//         Debug.Log("Push - ClearPush_AOS");
//         AndroidNotificationCenter.CancelAllNotifications();
// #endif
//         m_isMakePush = false;
//     }

//     public void MakePush()
//     {
//         MakeCallBackPush();// PushOnOff );
//     }

//     void MakeCallBackPush()// EPushOnOff pushOnOff )
//     {
//         ClearPush();

//         //if( pushOnOff == EPushOnOff.None )
//         //{
//         //    Debug.Log( "Push - 푸시가 꺼져있음" );
//         //    return;
//         //}

//         if( m_isMakePush == false )
//             m_isMakePush = true;

//         List<PushInfo> pushInfos = new List<PushInfo>();
//         //Dictionary<int, CRefPush> dicRefPush = CReferenceManager.Instance.GetRefPushs();
//         //if( dicRefPush == null )
//         //    return;

//         ////int index = 1000;
//         //foreach( CRefPush refPush in dicRefPush.Values )
//         //{
//         //    if( refPush.PushType == 0 ) // 서버 푸시면 패스
//         //        continue;

//         //    PushInfo pushInfo = null;
//         //    switch( (EPushConditionType)refPush.PushConditionType )
//         //    {
//         //    case EPushConditionType.Return:
//         //        {
//         //            //pushInfo = MakeRetrun( refPush );
//         //        }
//         //        break;
//         //    }

//         //    if( pushInfo != null )
//         //    {
//         //        pushInfos.Add( pushInfo );
//         //    }
//         //}       

//         pushInfos.Sort( ComparePushTime );
//         for( int i = 0, iMax = Mathf.Min( pushInfos.Count, m_maxCount ); i < iMax; i++ )
//         {
//             if( pushInfos[ i ] != null )
//             {
//                 DateTime pushPlayTime = DateTime.Now.AddSeconds( (double)pushInfos[ i ].remainTime );

//                 //int pushPlayHour = pushPlayTime.Hour;
//                 //if( PushOnOff == EPushOnOff.PushOnNightOff && ( pushPlayHour >= nightStartHour || pushPlayHour < nightEndHour ) )
//                 //{
//                 //    Debug.Log( "Push -  밤에는 푸시 패스" );
//                 //    continue;
//                 //}

//                 PlayPush( pushInfos[ i ].remainTime, pushInfos[ i ].title, pushInfos[ i ].message );
//             }
//         }
//     }

//     private void PlayPush(double remainTime, string title, string message)
//     {
//         Debug.Log( "Push - PlayPush : " + title + ":" + message + "(" + DateTime.Now.AddSeconds( remainTime ) + ")" );
// #if UNITY_EDITOR
// #elif UNITY_IPHONE
// 		UnityEngine.iOS.LocalNotification localNotification = new UnityEngine.iOS.LocalNotification();
        
// 		localNotification.alertBody = message;
// 		localNotification.soundName = UnityEngine.iOS.LocalNotification.defaultSoundName;
// 		localNotification.fireDate = DateTime.Now.AddSeconds((double)remainTime);
// 		UnityEngine.iOS.NotificationServices.ScheduleLocalNotification(localNotification);
// #elif UNITY_ANDROID
//         // 알림 생성
//         var notification = new AndroidNotification();

//         notification.Title = title;
//         notification.Text = message;
//         notification.FireTime = System.DateTime.Now.AddSeconds( remainTime );

//         notification.SmallIcon = "icon_0";
//         notification.LargeIcon = "icon_1";

//         AndroidNotificationCenter.SendNotification( notification, "com.risingwings.dollygo" );
// #endif
//     }

//     private int ComparePushTime(PushInfo a, PushInfo b)
//     {
//         return a.remainTime.CompareTo( b.remainTime );
//     }

//     //private PushInfo MakeRetrun(CRefPush _info)
//     //{
//     //    if( _info == null )
//     //        return null;
//     //    Debug.Log( "MakeRetrun : " + _info.PushConditionType );

//     //    int addtime = _info.PushConditionValue*60;
//     //    long remainTime = addtime;
//     //    DateTime _time = DateTime.Now.AddSeconds( remainTime );
//     //    Debug.Log( "MakeRetrun PushTime :" + _time );


//     //    string title = _info.PushMsgTitleKey.Localize();
//     //    string message = _info.PushMsgBodyKey.Localize();

//     //    if( remainTime <= 0 )
//     //        return null;

//     //    return new PushInfo( remainTime, title, message);
//     //}

//     private bool m_isMakePush;
// }
