# GCCHigh.Extensions.Graph Version History.

### **4.9.700 (2023-03-29)**<br>
	- Separate `Extensions.Graph` class from Extensions.cs.<br>

### **6.0.800 (2024-02-26)**<br>
	- Rearchitected and Optimized.<br>
	- Rebranded the `Extensions` set of NuGet packages as<br>
	    `GCCHigh.Extensions`.<br>

### **6.1.800 (2024-02-27)**<br>
	- Added the `getFile` option to `.GetDriveItem()` to allow for<br>
	    retrieval of the underlying binary file.<br>

### **6.2.800 (2024-02-27)**<br>
	- Added `.GetSiteOwners()` to Graph.<br>
	- Added `.GetSiteMemers()` to Graph.<br>
	- Added `.GetSiteUsers()` to Graph.<br>

### **6.3.800 (2024-02-28)**<br>
	- Added handler for sites that don't exist to `.GetSiteId()` in Graph.<br>

### **6.4.800 (2024-03-10)**<br>
	- Added `.GetDriveItemVersions()` and `.DownloadVersions()` to<br>
	    `Microsoft.Graph.Models.DriveItem`.<br>
	- Added `consoleFeedback` and `feedbackEvery` parameters to<br>
	    `Graph.GetListItems()`.<br>
	- Added public `Graph.Get()` generic method for handling multiple kinds<br>
	    of Graph object aggregation.<br>
	- Added internal `Graph.AddFilterSelect()` method for applying filter<br>
	    and select parameters to dynamic `QueryParameters`.<br>
	- Added internal `Graph.GetPages()` method for retrieving all pages of<br>
	    a given dynamic `CollectionResponse`.<br>
	- Added `.GetDrives()` extension method to the<br>
	    Microsoft.Graph.Models.Site object.<br>
	- Added multithreading logic to `Graph.GetGroups()` to boost<br>
	    performance.  The number of threads employed is based on the number<br>
		of logical CPU cores reported by the Environment.
	- Fixed a breakout bug in `Graph.GetGroupsPages()`.<br>
	- Removed `using static` references related to the `Graph.Get()` method<br>
	    and used full namespace instead.<br>
	- Name shortening refactor.<br>
