# Bug Fix Report - FMSModManager

This report documents three critical bugs that were identified and fixed in the FMSModManager codebase.

## Bug 1: Null Reference Exception in Service Methods

### **Issue Type**: Logic Error / Runtime Exception
### **Severity**: High
### **Location**: 
- `FMSModManager.Core/Services/ReligionModService.cs` (SaveReligionMod method)
- `FMSModManager.Core/Services/CultureModService.cs` (SaveCultureMod method)

### **Description**
The `SaveReligionMod` and `SaveCultureMod` methods contained a critical null reference vulnerability. The methods accessed `_religionMods[modName]` and `_cultureMods[modName]` directly without checking if the value was null, even when the key existed in the dictionary.

### **Root Cause**
The dictionary could contain keys with null values due to the lazy loading pattern used in the service. When `LoadReligionMods()` and `LoadCultureMods()` were called, they populated the dictionary with null values as placeholders.

### **Impact**
- Application crashes when attempting to save mod data
- Data corruption risk
- Poor user experience with unexpected exceptions

### **Fix Applied**
Added null safety checks before accessing mod data:

```csharp
// Before accessing mod data, verify both key existence and non-null value
if (!_religionMods.ContainsKey(modName) || _religionMods[modName] == null)
{
    return false;
}
```

### **Benefits**
- Prevents null reference exceptions
- Improves application stability
- Provides graceful error handling

---

## Bug 2: Performance Issue - Redundant Service Calls in UI

### **Issue Type**: Performance Issue
### **Severity**: Medium
### **Location**: 
- `FMSModManager.Pages/Pages/LocalModsCulture.razor` (Table RowTemplate)
- `FMSModManager.Pages/Pages/LocalModsReligion.razor` (Table RowTemplate)

### **Description**
The UI components were making multiple identical service calls per table row to retrieve mod data. In `LocalModsCulture.razor`, `CultureModService.GetCultureMod(context)` was called three times per row, and in `LocalModsReligion.razor`, similar redundant calls were made.

### **Root Cause**
Direct service calls in Blazor templates without caching the results, leading to unnecessary repeated operations.

### **Impact**
- Poor performance with multiple mods
- Increased CPU usage
- Slower UI rendering
- Potential blocking of UI thread

### **Fix Applied**
Implemented result caching within the row template:

```razor
<RowTemplate>
    @{
        // Cache the mod data once per row to avoid multiple service calls
        var modData = CultureModService.GetCultureMod(context);
    }
    <MudTd>@(modData?.CityNames.Count ?? 0)</MudTd>
    <MudTd>@(modData?.PoliticalSystems.Count ?? 0)</MudTd>
    <MudTd>@(modData?.StateNames.Count ?? 0)</MudTd>
</RowTemplate>
```

### **Benefits**
- Reduces service calls from 3 to 1 per row
- Improves UI performance significantly
- Better user experience with faster loading
- Reduced system resource usage

---

## Bug 3: Memory Leak - Missing IDisposable Implementation

### **Issue Type**: Memory Leak / Resource Management
### **Severity**: Medium
### **Location**: 
- `FMSModManager.Pages/Pages/LocalModsCulture.razor`
- `FMSModManager.Pages/Pages/LocalModsReligion.razor`  
- `FMSModManager.Pages/Layout/NavMenu.razor`

### **Description**
Multiple Blazor components had `Dispose()` methods for cleaning up event subscriptions, but they didn't implement the `IDisposable` interface. This meant the dispose methods were never called, leading to memory leaks from undisposed event handlers.

### **Root Cause**
- Missing `@implements IDisposable` directive in Blazor components
- Missing event unsubscription in NavMenu component (LanguageChangedEvent)

### **Impact**
- Memory leaks from event handler subscriptions
- Potential performance degradation over time
- Increased memory usage
- Event handlers remaining active after component destruction

### **Fix Applied**

1. **Added IDisposable implementation to all affected components:**
```razor
@implements IDisposable
```

2. **Fixed missing event unsubscription in NavMenu:**
```csharp
public void Dispose()
{
    EventAggregator.GetEvent<ModsChangedEvent>().Unsubscribe(RefreshMods);
    EventAggregator.GetEvent<SteamworkServiceRefuseEvent>().Unsubscribe(RefreshMods);
    EventAggregator.GetEvent<LanguageChangedEvent>().Unsubscribe(LanguageChangedFunc); // Added this line
}
```

### **Benefits**
- Eliminates memory leaks from event subscriptions
- Proper resource cleanup when components are destroyed
- Improved application stability and performance
- Better memory management

---

## Summary

All three bugs have been successfully identified and fixed:

1. **Logic Error**: Added null safety checks to prevent runtime exceptions
2. **Performance Issue**: Optimized UI rendering by caching service call results  
3. **Memory Leak**: Implemented proper disposal pattern for Blazor components

These fixes improve the application's stability, performance, and resource management significantly. The codebase is now more robust and follows better software engineering practices.