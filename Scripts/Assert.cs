using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnTested 
{
	public static class Assert {

		#region Null Checks
		public static void IsNotNull (object obj, string message = "") 
		{
			if(obj == null) {
				throw new Exception(string.Format("AssertionException: Expected Object to Not be Null; {0}", message));
			}
		}

		public static void IsNull (object obj, string message = "") 
		{
			if(obj != null) {
				throw new Exception(string.Format("AssertionException: Expected [{0}] to be Null; {1}",
				                              obj.ToString(), message));
			}
		}
		#endregion

		#region Conditions
		public static void IsTrue (bool condition, string message = "")
		{
			if(!condition) {
				throw new Exception (string.Format ("AssertionException: Expected True but was False; {0}", message));
			}
		}

		public static void IsFalse (bool condition, string message = "")
		{
			if(condition) {
				throw new Exception (string.Format ("AssertionException: Expected False but was True; {0}", message));
			}
		}
		#endregion

		#region Equals
		public static void AreEqual (object expected, object actual, string message = "")
		{
			if(expected == null && actual == null) {
				return;
			} else if(expected == null) {
				throw new Exception (string.Format ("AssertionException: Expected [{0}] to be Equal to NULL; {1}",
				                                    actual.ToString (), message));
			} else if(actual == null) {
				throw new Exception (string.Format ("AssertionException: Expected NULL to be Equal to [{0}]; {1}",
				                                    expected.ToString (), message));
			}

			if(!expected.Equals(actual)) {
				throw new Exception (string.Format ("AssertionException: Expected [{0}] to be Equal to [{1}]; {2}",
				                                    actual.ToString (), expected.ToString (), message));
			}
		}

		public static void AreNotEqual (object expected, object actual, string message = "")
		{
			if(expected == null && actual == null) {
				throw new Exception (string.Format ("AssertionException: Expected NULL to Not be Equal to NULL; {0}", message));
			} else if(expected == null) {
				return;
			} else if(actual == null) {
				return;
			}

			if(expected.Equals(actual)) {
				throw new Exception (string.Format ("AssertionException: Expected [{0}] to Not be Equal to [{1}]; {2}", actual.ToString (), expected.ToString (),
				                                    message));
			}
		}
		#endregion

		#region Empty

		#region String
		public static void IsEmpty (string str, string message = "")
		{
			if(str != "") {
				throw new Exception (string.Format ("AssertionException: Expected [{0}] to be Empty; {1}", str, message));
			}
		}

		public static void IsEmptyOrNull (string str, string message = "")
		{
			if(!string.IsNullOrEmpty(str)) {
				throw new Exception (string.Format ("AssertionException: Expected [{0}] to be Empty or NULL; {1}", str, message));
			}
		}

		public static void IsNotEmpty (string str, string message = "")
		{
			if(str == "") {
				throw new Exception (string.Format ("AssertionException: Expected Empty String to not be Empty; {0}", message));
			}
		}

		public static void IsNotEmptyOrNull (string str, string message = "")
		{
			if(string.IsNullOrEmpty(str)) {
				throw new Exception (string.Format ("AssertionException: Expected Empty or NULL String to not be Empty or NULL; {0}", message));
			}
		}
		#endregion

		#region Collection
		public static void IsEmpty<T> (ICollection<T> collection, string message = "")
		{
			if(collection.Count > 0) {
				throw new Exception (string.Format ("AssertionException: Expected [{0}] be Empty but had [{1}] elements; {2}",
				                                    collection.ToString(), collection.Count, message));
			}
		}

		public static void IsEmptyOrNull<T> (ICollection<T> collection, string message = "")
		{
			if (collection == null)
				return;

			if(collection.Count > 0) {
				throw new Exception (string.Format ("AssertionException: Expected [{0}] to be Empty or NULL but had [{1}] elements; {2}",
				                                    collection.ToString(), collection.Count.ToString(), message));
			}
		}

		public static void IsNotEmpty<T> (ICollection<T> collection, string message = "")
		{
			if(collection.Count == 0) {
				throw new Exception (string.Format ("AssertionException: Expected [{0}] to not be Empty; {1}", collection.ToString(),
				                                    message));
			}
		}

		public static void IsNotEmptyOrNull<T> (ICollection<T> collection, string message = "")
		{
			if(collection == null) {
				throw new Exception (string.Format ("AssertionException: Expected NULL to not be Empty or NULL; {0}", message));
			}

			if(collection.Count == 0) {
				throw new Exception (string.Format ("AssertionException: Expected [{0}] to not be Empty or NULL but was Empty; {1}",
				                                    collection, message));
			}
		}
		#endregion

		#endregion
	}
}