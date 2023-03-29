#pragma warning disable CS0162, CS1587, CS1591, CS1998, IDE0059, IDE0028

/// <summary>
/// Author: Cornelius J. van Dyk blog.cjvandyk.com @cjvandyk
/// This code is provided under GNU GPL 3.0 and is a copyrighted work of the
/// author and contributors.  Please see:
/// https://github.com/cjvandyk/Extensions/blob/main/LICENSE
/// </summary>

using System;
using System.Collections.Generic;
using static System.Logit;
using static Extensions.Core;  //NuGet Extensions.cs

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
        public static bool GetJsonBool(this Microsoft.Graph.ListItem item, 
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
				Err(ex.ToString());
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
		public static string GetJsonString(this Microsoft.Graph.ListItem item, 
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
				Err(ex.ToString());
				result = "";
			}
			finally
			{
			}
			return result;
		}
    }
}

#pragma warning restore CS0162, CS1587, CS1591, CS1998, IDE0059, IDE0028
