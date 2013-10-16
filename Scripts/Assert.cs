using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnTested 
{
	/// <summary>
	/// Assert - Assertion Methods (Extend as Needed)
	/// These Methods throw Exceptions if the Assertions Fail, these exceptions get caught and processed by
	/// TestRunner when running unit tests. Do not use these methods outside of these Attributed Classes and Methods:
	/// 	AssemblyFixture
	/// 		AssemblySetup
	/// 		AssemblyTeardown
	/// 	TestFixture
	/// 		TestSetup
	/// 		Test
	/// 		TestTeardown
	/// </summary>
	public static class Assert {

		#region Null Checks
		/// <summary>
		/// Throw Assertion Exception, with message, if the object is null.
		/// </summary>
		/// <param name="obj">Object.</param>
		/// <param name="message">Message.</param>
		public static void IsNotNull (object obj, string message = "") 
		{
			if(obj == null) {
				throw new Exception(string.Format("AssertionException: Expected Object to Not be Null; {0}", message));
			}
		}

		/// <summary>
		/// Throw Assertion Exception, with message, if the object is not null.
		/// </summary>
		/// <param name="obj">Object.</param>
		/// <param name="message">Message.</param>
		public static void IsNull (object obj, string message = "") 
		{
			if(obj != null) {
				throw new Exception(string.Format("AssertionException: Expected [{0}] to be Null; {1}",
				                              obj.ToString(), message));
			}
		}
		#endregion

		#region Conditions
		/// <summary>
		/// Throw Assertion Exception, with message, if the condition is false.
		/// </summary>
		/// <param name="condition">If set to <c>true</c> condition.</param>
		/// <param name="message">Message.</param>
		public static void IsTrue (bool condition, string message = "")
		{
			if(!condition) {
				throw new Exception (string.Format ("AssertionException: Expected True but was False; {0}", message));
			}
		}

		/// <summary>
		/// Throw Assertion Exception, with message, if the condition is true.
		/// </summary>
		/// <param name="condition">If set to <c>true</c> condition.</param>
		/// <param name="message">Message.</param>
		public static void IsFalse (bool condition, string message = "")
		{
			if(condition) {
				throw new Exception (string.Format ("AssertionException: Expected False but was True; {0}", message));
			}
		}
		#endregion

		#region Equals
		/// <summary>
		/// Throw Assertion Exception, with message, if the actual object is not equal to the expected object.
		/// </summary>
		/// <param name="expected">Expected.</param>
		/// <param name="actual">Actual.</param>
		/// <param name="message">Message.</param>
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

		/// <summary>
		/// Throw Assertion Exception, with message, if the actual object is equal to the expected object.
		/// </summary>
		/// <param name="expected">Expected.</param>
		/// <param name="actual">Actual.</param>
		/// <param name="message">Message.</param>
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
		/// <summary>
		/// Throw Assertion Exception, with message, if the string is not empty.
		/// </summary>
		/// <param name="str">String.</param>
		/// <param name="message">Message.</param>
		public static void IsEmpty (string str, string message = "")
		{
			if(str != "") {
				throw new Exception (string.Format ("AssertionException: Expected [{0}] to be Empty; {1}", str, message));
			}
		}

		/// <summary>
		/// Throw Assertion Exception, with message, if the string is not empty or null.
		/// </summary>
		/// <param name="str">String.</param>
		/// <param name="message">Message.</param>
		public static void IsEmptyOrNull (string str, string message = "")
		{
			if(!string.IsNullOrEmpty(str)) {
				throw new Exception (string.Format ("AssertionException: Expected [{0}] to be Empty or NULL; {1}", str, message));
			}
		}

		/// <summary>
		/// Throw Assertion Exception, with message, if the string is empty.
		/// </summary>
		/// <param name="str">String.</param>
		/// <param name="message">Message.</param>
		public static void IsNotEmpty (string str, string message = "")
		{
			if(str == "") {
				throw new Exception (string.Format ("AssertionException: Expected Empty String to not be Empty; {0}", message));
			}
		}

		/// <summary>
		/// Throw Assertion Exception, with message, if the string is empty or null.
		/// </summary>
		/// <param name="str">String.</param>
		/// <param name="message">Message.</param>
		public static void IsNotEmptyOrNull (string str, string message = "")
		{
			if(string.IsNullOrEmpty(str)) {
				throw new Exception (string.Format ("AssertionException: Expected Empty or NULL String to not be Empty or NULL; {0}", message));
			}
		}
		#endregion

		#region Collection
		/// <summary>
		/// Throw Assertion Exception, with message, if the collection of type T is not empty.
		/// </summary>
		/// <param name="collection">Collection.</param>
		/// <param name="message">Message.</param>
		public static void IsEmpty<T> (ICollection<T> collection, string message = "")
		{
			if(collection.Count > 0) {
				throw new Exception (string.Format ("AssertionException: Expected [{0}] be Empty but had [{1}] elements; {2}",
				                                    collection.ToString(), collection.Count, message));
			}
		}

		/// <summary>
		/// Throw Assertion Exception, with message, if the collection of type T is not empty or null.
		/// </summary>
		/// <param name="collection">Collection.</param>
		/// <param name="message">Message.</param>
		public static void IsEmptyOrNull<T> (ICollection<T> collection, string message = "")
		{
			if (collection == null)
				return;

			if(collection.Count > 0) {
				throw new Exception (string.Format ("AssertionException: Expected [{0}] to be Empty or NULL but had [{1}] elements; {2}",
				                                    collection.ToString(), collection.Count.ToString(), message));
			}
		}

		/// <summary>
		/// Throw Assertion Exception, with message, if the collection of type T is empty.
		/// </summary>
		/// <returns><c>true</c> if is not empty the specified collection message; otherwise, <c>false</c>.</returns>
		/// <param name="collection">Collection.</param>
		/// <param name="message">Message.</param>
		public static void IsNotEmpty<T> (ICollection<T> collection, string message = "")
		{
			if(collection.Count == 0) {
				throw new Exception (string.Format ("AssertionException: Expected [{0}] to not be Empty; {1}", collection.ToString(),
				                                    message));
			}
		}

		/// <summary>
		/// Throw Assertion Exception, with message, if the collection of type T is empty or null.
		/// </summary>
		/// <param name="collection">Collection.</param>
		/// <param name="message">Message.</param>
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