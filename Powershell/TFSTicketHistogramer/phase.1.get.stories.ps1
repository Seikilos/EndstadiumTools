
. .\authentication.ps1

. .\helpers.ps1

$type ="User Story"

Write-Output("Starting retrieval of {0} work item types" -f $type)

# Phase 1 is getting all stories
$r = Request "Select [System.Id], [System.Title], [System.State] From WorkItems Where [System.WorkItemType] = '$type' AND [System.TeamProject] = 'Lanos' AND [State] <> 'Removed' order by [System.CreatedDate] ASC"
$rJson = $r | ConvertFrom-Json

$workItems = $rJson.workItems

Write-Output ("Found {0} work items" -f $workItems.Count)

$workItems | ForEach-Object {

  $revisionsUrl = ($_.url +"/revisions")
  Write-Output("Fetching revisions for {0}" -f $_.id)
  
  $revisionJson = Raw-Get $revisionsUrl | ConvertFrom-Json

  Write-Output("Found {0} state changes" -f $revisionJson.count)

  $changeList = Get-State-Changes $revisionJson
}



exit 1

# Request all iterations

#Print (Invoke-WebRequest -Uri $urlIterations -ContentType "application/json"  -Headers @{Authorization=("Basic {0}" -f $base64AuthInfo)}  )


$r = Request "Select [System.Id], [System.Title], [System.State] From WorkItems Where [System.WorkItemType] = 'User Story' AND [State] <> 'Removed' order by [System.CreatedDate] desc"

# Group by iteration, order by iteration
#$r = Request "Select [System.Id], [System.Title], [System.State] From WorkItems Where [System.WorkItemType] = 'User Story' AND [System.TeamProject] = 'Lanos' AND [State] <> 'Closed' AND [State] <> 'Removed' order by [System.IterationPath] asc"



Print $r.Content