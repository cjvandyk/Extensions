# GCCHigh.Extensions.Core Version History.

### **4.9.700 (2023-03-28)**<br>
	- Separate `Extensions.Identity` class from Extensions.cs.<br>

### **6.0.800 (2024-02-26)**<br>
	- Rearchitected and Optimized.<br>
	- Rebranded the `Extensions` set of NuGet packages as `GCCHigh.Extensions`.<br>

### **6.1.800 (2024-02-26)**<br>
	- Added `ActiveAuth` validation to the `.GetSetting()` method.<br>

### **6.2.800 (2024-02-26)**<br>
	- Added `TenantString` capture to the `.InitializeTenant()` method.<br>
		
### **6.3.800 (2024-02-27)**<br>
	- Optimized tenant initialization.<br>
		
### **6.4.800 (2024-02-27)**<br>
	- Optimized tenant initialization.<br>
		
### **6.5.800 (2024-02-27)**<br>
	- Optimized tenant initialization.<br>
		
### **6.6.800 (2024-02-27)**<br>
	- Optimized tenant initialization.<br>
		
### **6.7.800 (2024-02-27)**<br>
	- Made `TryAdd()` on `List<>` and `Dictionary<>` in core internal.<br>
		
### **6.8.800 (2024-02-29)**<br>
	- Added the `Core.GetHttpClient()` relay method.<br>
		
### **6.9.800 (2024-03-10)**<br>
	- Added the `RUNNING_IN_AZURE` check to `Core.GetRunFolder()`.<br>
	- Added `Core.ForEach()` method to do parallel foreach processing in<br>
	    batches.  This is especially useful when the Action specified in<br>
		body executes complex operations like making REST calls against big<br>
		data sources e.g. having to call the /_api/web/ensureuser REST<br>
		method in SharePoint when validating 200,000 users will inevitably<br>
		lead to thread timeouts since the CPU just can't handle that many<br>
		parallel threads concurrently.<br>
	- Name shortening refactor.<br>
