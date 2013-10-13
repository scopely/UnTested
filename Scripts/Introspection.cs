using UnityEngine;
using System.Collections;

namespace UnTested
{
	public class Introspection {

		static public bool Editor {
			get {
				#if UNITY_EDITOR
				return true;
				#else
				return false;
				#endif
			}
		}

		static public bool Testing {
			get {
				#if TESTS
				return true;
				#else
				return false;
				#endif
			}
		}
	}
}