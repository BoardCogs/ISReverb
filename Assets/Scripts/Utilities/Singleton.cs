using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : Component
{

	#region Properties

	/// <summary>
	/// Gets the instance.
	/// </summary>
	/// <value>The instance.</value>
	public static T Instance { get; private set; }

	#endregion

	#region Methods

	/// <summary>
	/// Use this for initialization.
	/// </summary>
	protected virtual void Awake ()
	{
		if ( Instance == null )
		{
			Instance = this as T;
		}
		else
		{
			Debug.LogWarning( "Singleton instance of type " + GetType() + " already exists." );
		}
	}

	protected virtual void OnDestroy()
	{
		if ( Instance == this )
		{
			Instance = null;
		}
	}

	#endregion	
}