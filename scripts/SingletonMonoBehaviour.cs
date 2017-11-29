#define TmLib_SingletonMonoBehaviour_DontDestroyOnLoad
using UnityEngine;

// usage: public class xx : TmLib.SingletonMonoBehaviour<xx> {}
namespace TmLib{
	public class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour {
		public const string NAME = "_singleton";
		//---------------------------------------------------------
		private static T m_Instance = null;
		public static bool hasInstance{ get { return m_Instance!=null; } }
		public static T Instance{
			get{
				if(m_Instance==null){
					GameObject sysObj = new GameObject(NAME);
					m_Instance = sysObj.AddComponent<T>();
				}
				return m_Instance;
			}
		}

		protected void Awake () {
			if(m_Instance==null){
				m_Instance = FindObjectOfType<T>();
				#if TmLib_SingletonMonoBehaviour_DontDestroyOnLoad
				DontDestroyOnLoad(gameObject);
				#endif
			}else{
				Destroy(this.gameObject);
			}
		}
	}
} //namespace TmLib
