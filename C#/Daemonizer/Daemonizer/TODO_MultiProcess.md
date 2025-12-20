# TODO: Extend Daemonizer to Monitor Multiple Processes (Outlook, Firefox, etc.)

## Overview
Refactor the hardcoded Firefox-specific logic to support monitoring multiple configurable processes (Firefox, Outlook, etc.) that need to be closed before running the target executable.

## Tasks

### 1. Update Command-Line Arguments
- [ ] Add new optional parameter for process names to monitor (e.g., comma-separated list)
- [ ] Update argument validation in Main() (lines 55-63)
- [ ] Update help message to document new parameter format
- [ ] Example: `daemonizer.exe <exe> <args> <logdir> <write_always> <processes>`
  - Process list format: "firefox,OUTLOOK,chrome"

### 2. Refactor Single Process Variables to Collections
- [ ] Replace `Process importantProcess` (line 65) with `List<ProcessInfo>` structure
- [ ] Create `ProcessInfo` class/struct to store:
  - `Process` object
  - `string modulePath` (for restart)
  - `string processName` (original name)
- [ ] Replace `string modulePath` (line 66) - move into ProcessInfo
- [ ] Replace single `DialogResult dlgResult` with tracking per-process or overall strategy

### 3. Generalize Process Detection Logic
- [ ] Extract hardcoded "firefox" string (line 89) to iterate over process list
- [ ] Move process detection into a method: `List<ProcessInfo> FindMonitoredProcesses(string[] processNames)`
- [ ] Update parent-finding logic (lines 94-107) to use variable process name instead of "firefox"
- [ ] Handle case where multiple instances of same process exist
- [ ] Add error handling for processes that don't exist

### 4. Refactor Process Closure Dialog
- [ ] Update MessageBox (lines 111-115) to list all found processes
- [ ] Consider dialog options:
  - Yes: Close all and reopen
  - No: Continue without closing
  - Cancel: Abort operation
- [ ] Alternative: Show individual dialogs per process with Yes/No/Cancel for each

### 5. Implement Multiple Process Closure
- [ ] Loop through all found processes to close them (refactor lines 117-137)
- [ ] Track which processes were successfully closed
- [ ] Handle timeout scenarios for each process individually
- [ ] If any process fails to close, decide on error strategy:
  - Abort entire operation?
  - Continue with warning?
  - Skip that process?

### 6. Implement Multiple Process Restart
- [ ] Update finally block (lines 236-239) to restart all closed processes
- [ ] Iterate through list of successfully closed processes
- [ ] Start each process using stored modulePath
- [ ] Add error handling if restart fails (log warning but don't fail overall operation)
- [ ] Consider restart order (reverse order of closure?)

### 7. Add Configuration Options
- [ ] Consider adding config file support for default process list
- [ ] Add option to specify processes should be closed sequentially vs. asking about each
- [ ] Add timeout configuration for process closure waits

### 8. Testing
- [ ] Test with Firefox only (backward compatibility)
- [ ] Test with Outlook only
- [ ] Test with both Firefox and Outlook
- [ ] Test with no processes running
- [ ] Test with processes that refuse to close (timeout scenarios)
- [ ] Test process restart functionality
- [ ] Test with invalid process names

### 9. Documentation
- [ ] Update help message with new parameter
- [ ] Add comments explaining the multi-process logic
- [ ] Create example usage scenarios in README or comments

## Implementation Notes

### Current Hardcoded Locations
- Line 89: `Process.GetProcessesByName("firefox")`
- Line 95: `while (parent.ProcessName == "firefox")`
- Line 100: Parent process check
- Line 112: MessageBox text mentioning process

### Suggested Class Structure
```csharp
class ProcessInfo
{
    public Process Process { get; set; }
    public string ModulePath { get; set; }
    public string ProcessName { get; set; }
    public bool WasClosed { get; set; }
}
```

### Backward Compatibility
- If no process list parameter provided, default to "firefox" to maintain current behavior
- Or make it optional and skip process monitoring entirely if not specified

## Future Enhancements
- [ ] Support for process-specific arguments on restart
- [ ] Support for waiting for process to fully load before continuing
- [ ] Add logging of which processes were closed/restarted
- [ ] Support for checking process tree dependencies
