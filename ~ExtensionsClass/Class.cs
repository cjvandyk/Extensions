#pragma warning disable CS0162, CS1587, CS1591, CS1998, IDE0059, IDE0028

/// <summary>
/// Author: Cornelius J. van Dyk blog.cjvandyk.com @cjvandyk
/// This code is provided under GNU GPL 3.0 and is a copyrighted work of the
/// author and contributors.  Please see:
/// https://github.com/cjvandyk/Extensions/blob/main/LICENSE
/// </summary>

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using static Extensions.Universal;  //NuGet Extensions.cs
using static System.Logit;          //NuGet Extensions.Logit

namespace $rootnamespace$
{
    [Serializable]
	public static partial class $safeitemrootname$
	{
		try
		{
			//Code here.
		}
		catch (Exception ex)
		{
			Err(ex.ToString());
		}
		finally
		{
		}
	}
}

#pragma warning restore CS0162, CS1587, CS1591, CS1998, IDE0059, IDE0028
