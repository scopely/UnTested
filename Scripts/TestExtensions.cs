using UnityEngine;
using System.Collections;
using System;

public static class TestExtensions 
{
	public static TestCoroutine<T> StartTestCoroutine<T>(this MonoBehaviour obj, IEnumerator coroutine){
		TestCoroutine<T> coroutineObject = new TestCoroutine<T>();
		coroutineObject.coroutine = obj.StartCoroutine(coroutineObject.InternalRoutine(coroutine));
		return coroutineObject;
	}
}

public class TestCoroutine<T> {
	public T Value {
		get{
			if(e != null){
				throw e;
			}
			return returnVal;
		}
	}

	public Exception Exception {
		get{
			return e;
		}
	}

	private T returnVal;
	private Exception e = null;
	public Coroutine coroutine;

	public IEnumerator InternalRoutine(IEnumerator coroutine){
		while(true){
			try{
				if(!coroutine.MoveNext()){
					yield break;
				}
			}
			catch(Exception e){
				//Debug.LogError (e.Message);
				this.e = e;
				yield break;
			}
			object yielded = coroutine.Current;
			if(yielded != null && yielded.GetType() == typeof(T)){
				returnVal = (T)yielded;
				yield break;
			}
			else{
				yield return coroutine.Current;
			}
		}
	}
}