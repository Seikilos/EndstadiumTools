
. .\authentication.ps1

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
    $durationTable += "___"
    return # not enough states
  }

  $durationTable += $_.id

}

exit 1

# Request all iterations

#Print (Invoke-WebRequest -Uri $urlIterations -ContentType "application/json"  -Headers @{Authorization=("Basic {0}" -f $base64AuthInfo)}  )


$r = Request "Select [System.Id], [System.Title], [System.State] From WorkItems Where [System.WorkItemType] = 'User Story' AND [State] <> 'Removed' order by [System.CreatedDate] desc"

# Group by iteration, order by iteration
#$r = Request "Select [System.Id], [System.Title], [System.State] From WorkItems Where [System.WorkItemType] = 'User Story' AND [System.TeamProject] = 'Lanos' AND [State] <> 'Closed' AND [State] <> 'Removed' order by [System.IterationPath] asc"



Print $r.Content