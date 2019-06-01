function Append-Version {

    param([string] $url)
  
    if ($url -contains "?") {
        $url += "&api-version=$apiVersion"
    }
    else {
        $url += "?api-version=$apiVersion"
    }

    return $url
}

function Raw-Get {
    param([string] $url) 

    return Invoke-WebRequest -Uri $url -ContentType "application/json" -Method GET -Headers @{Authorization = ("Basic {0}" -f $base64AuthInfo) }  
  
}

function Request {

    param([string] $query)
  
    $req = @{
        query = $query
    } | ConvertTo-Json
    
    $fullUri = Append-Version $url
  
    return Invoke-WebRequest -Uri $fullUri -ContentType "application/json" -Body $req -Method POST -Headers @{Authorization = ("Basic {0}" -f $base64AuthInfo) }  
  
}
function Print {
    param($req)
  
    Write-Output $req | ConvertFrom-Json | ConvertTo-Json
}

function Get-State-Changes {

    param([object] $data)

    $changes = @()

    $lastState = $null

    $data.value | ForEach-Object {

        $currentState = $_.fields."System.State"
        $currentDate = $_.fields."System.ChangedDate"
        if( $lastState -ne $currentState) {

            $lastState = $currentState

            $datetime = [datetime]::Parse($currentDate)
            $changes += @{state=$currentState; date=$datetime}
        }

    }

    return $changes

}

function Calc-Durations {
    param([object] $data)


    $durations = @()

    for( $i = 0; $i -lt $data.states.Count-1; ++$i)
    {
        $currentState = $data.states[$i]
        $nextState = $data.states[$i+1]

        $duration = $nextState.date - $currentState.date

        $durations += @{state=$currentState.state; duration=$duration}
    }

    $durations += @{state=$data.states[$data.states.Length-1].state; duration="still in"}

    return $durations
}