
. .\variables.ps1

. .\helpers.ps1

$type ="User Story"

Write-Output("Starting retrieval of {0} work item types" -f $type)

# Phase 1 is getting all stories
$r = Request "Select [System.Id], [System.Title], [System.State] From WorkItems Where [System.WorkItemType] = '$type' AND [System.TeamProject] = 'Lanos' AND [State] <> 'Removed' order by [System.CreatedDate] ASC"
$rJson = $r | ConvertFrom-Json

$workItems = $rJson.workItems

Write-Output ("Found {0} work items" -f $workItems.Count)

$witStates = @()

$workItems | ForEach-Object {

  $revisionsUrl = ($_.url +"/revisions")
  Write-Output("Fetching revisions for {0}" -f $_.id)
  
  $revisionJson = Raw-Get $revisionsUrl | ConvertFrom-Json

  Write-Output("Found {0} state changes" -f $revisionJson.count)

  $changeList = Get-State-Changes $revisionJson

  Write-Output("Detected {0} meaningful changes" -f $changeList.Length )

  $witStates += @{id= $_.id; states=$changeList; totalChanges=$revisionJson.count}

}

Write-Output("Creating duration table for {0} stories" -f $witStates.Count)

$durationTable = @()

$witStates | ForEach-Object {

  if ( $_.states.Length -le 1) 
  {
    return # not enough states, skip
  }

  $durationsOfStates = @(Calc-Durations $_)

  $durationTable += @{id=$_.id; totalChanges=$_.totalChanges; durations=$durationsOfStates}

}

Write-Output("Dumping duration table to")

$output = "Id  | Total Changes | States`r`n"

$durationTable | ForEach-Object {
  $states = ""
  $_.durations | ForEach-Object {
    $states += ("{0}: {1}, " -f $_.state, $_.duration)
  }

  $output += ("{0,3} | {1,-13} | {2}`r`n" -f $_.id, $_.totalChanges, $states)
}

$output | Out-File -FilePath $destFile