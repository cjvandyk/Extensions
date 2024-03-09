/// <summary>
/// Author: Cornelius J. van Dyk blog.cjvandyk.com @cjvandyk
/// This code is provided under GNU GPL 3.0 and is a copyrighted work of the
/// author and contributors.  Please see:
/// https://github.com/cjvandyk/Extensions/blob/main/LICENSE
/// </summary>

using Microsoft.Graph.Models;
using System;

namespace Extensions
{
	/// <summary>
	/// An extension class to simplify getting data from ListItems.
	/// </summary>
    [Serializable]
    public static partial class ListItemExtensions
    {
		/// <summary>
		/// Method to get a boolean field from ListItem.
		/// </summary>
		/// <param name="item">The item in question.</param>
		/// <param name="fieldName">The field name to get.</param>
		/// <returns></returns>
        public static bool GetJsonBool(
			this ListItem item, 
			string fieldName)
        {
            var result = false;

			try
			{
				result = Convert.ToBoolean(
					item.Fields.AdditionalData[fieldName]);
			}
			catch (Exception ex)
			{
				//Err(ex.ToString());
				result = false;
			}
			finally
			{
			}
			return result;
        }

		/// <summary>
		/// Method to get a string field from ListItem.
		/// </summary>
		/// <param name="item">The item in question.</param>
		/// <param name="fieldName">The field name to get.</param>
		/// <returns></returns>
		public static string GetJsonString(
			this ListItem item, 
			string fieldName)
		{
			var result = "";

			try
			{
				result = Convert.ToString(
					item.Fields.AdditionalData[fieldName]);
			}
			catch (Exception ex)
			{
				//Err(ex.ToString());
				result = "";
			}
			finally
			{
			}
			return result;
		}
    }
}
