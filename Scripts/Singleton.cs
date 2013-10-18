using UnityEngine;

namespace UnTested
{
	/// <summary>
	/// Mono singleton. From http://wiki.unity3d.com/index.php?title=Singleton#Generic_Based_Singleton_for_MonoBehaviours
	/// </summary>
	public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
	{
	    private static T m_Instance = null;
	    public static T Instance
	    {
	        get
	        {
	        	if(m_Instance) {
	        		return m_Instance;
	        	}
	            // Instance requiered for the first time, we look for it
	            else {
	                m_Instance = GameObject.FindObjectOfType(typeof(T)) as T;
	            }

	            return m_Instance;
	        }
	    }

	    /// <summary>
	    /// Awake function. Override when necessary and call base.Awake() first.
	    /// </summary>
	    protected virtual void Awake()
	    {
	        if( m_Instance == null )
	        {
	            m_Instance = this as T;
	        }
	    }

		protected virtual void OnDisable()
		{
			m_Instance = null;
		}

		protected virtual void OnDestroy()
		{
			m_Instance = null;
		}

	    /// <summary>
	    /// Clear the reference when the application quits. Override when necessary and call base.OnApplicationQuit() last.
	    /// </summary>
	    protected virtual void OnApplicationQuit()
	    {
	        m_Instance = null;
	    }
	}
}