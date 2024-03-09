# GCCHigh.Extensions.Identity Version History.

### **4.9.700 (2023-03-28)**<br>
	- Separate `System.Identity` class for logging functionality from Extensions.cs.<br>

### **6.0.800 (2024-02-26)**<br>
	- Rearchitected and Optimized.<br>
	- Rebranded the `Extensions` set of NuGet packages as `GCCHigh.Extensions`.<br>

### **6.1.800 (2024-02-26)**<br>
	- Change .LoadConfig() from internal to public.<br>

### **6.2.800 (2024-02-26)**<br>
	- Added `ActiveAuth` validation to the `.GetAuth()` method.<br>
	- Added `ActiveAuth` validation to the `.GetHttpClient()` method.<br>
	- Added `ActiveAuth` validation to the `.GetGraphServiceClient()` method.<br>
	- Added `ActiveAuth` validation to the `.GetGraphBetaServiceClient()` method.<br>

### **6.3.800 (2024-02-26)**<br>
	- Added `ActiveAuth` validation to the `TenantConfig` properties Get methods.<br>

### **6.4.800 (2024-02-27)**<br>
	- Added the `TargetTenantConfig` to AuthMan for initialization optimization.<br>

### **6.5.800 (2024-02-27)**<br>
	- Fixed `Scopes` in Identity.<br>

### **6.6.800 (2024-02-27)**<br>
	- Changed `.GetHttpClient()` from internal to public.<br>

### **6.7.800 (2024-03-02)**<br>
	- Added a `RUNNING_IN_AZURE` switch to `.GetRunFolder()` method.<br>

### **6.8.800 (2024-03-10)**<br>
	- Added `Identity.Cert.NewSelfSigned()` method to cenerate on demand<br>
	  self signed certificates that are RSA/SHA512 based.<br>
	- Name shortening refactor.<br>
