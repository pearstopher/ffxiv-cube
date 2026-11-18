<#
Reads current plan usage and says whether it is OK to start another issue.

How: a one-turn `claude -p` probe on the cheapest model (haiku) with --output-format stream-json emits a
`rate_limit_event` whose `rate_limit_info.unifiedWindows` has `five_hour` and `seven_day` utilization
(0..1) and reset times. See docs/research-notes.md ("Reading plan usage").

Exit codes: 0 = OK to start (or usage unreadable: that never blocks), 1 = over a threshold.
Prints one line: "OK ...", "UNKNOWN ..." or "STOP ...".

-FromFile reads saved stream-json instead of calling claude (used to test the parsing).
#>
param(
    [double]$SessionLimit = 0.70,
    [double]$WeeklyLimit = 0.85,
    [string]$FromFile = ""
)

function Fmt($v) { if ($null -eq $v) { "?" } else { "{0:P0}" -f $v } }
function At($t) { if ($t) { [DateTimeOffset]::FromUnixTimeSeconds([long]$t).LocalDateTime.ToString("MM-dd HH:mm") } else { "?" } }

try {
    if ($FromFile) {
        $lines = Get-Content $FromFile
    } else {
        $claude = (Get-Command claude -ErrorAction SilentlyContinue).Source
        if (-not $claude) { $claude = Join-Path $env:USERPROFILE ".local\bin\claude.exe" }
        $lines = & $claude -p "reply ok" --model haiku --max-turns 1 --permission-mode dontAsk --output-format stream-json --verbose 2>$null
    }

    $five = $null; $week = $null; $fiveReset = $null; $weekReset = $null
    foreach ($l in $lines) {
        if ($l -notmatch '"rate_limit_event"') { continue }
        $info = ($l | ConvertFrom-Json).rate_limit_info
        $w = $info.unifiedWindows
        if ($w.five_hour) { $five = [double]$w.five_hour.utilization; $fiveReset = $w.five_hour.resetsAt }
        if ($w.seven_day) { $week = [double]$w.seven_day.utilization; $weekReset = $w.seven_day.resetsAt }
        # Other shape: only the window named by rateLimitType is reported.
        if (-not $w) {
            if ($info.rateLimitType -eq "five_hour") { $five = [double]$info.utilization; $fiveReset = $info.resetsAt }
            if ($info.rateLimitType -eq "seven_day") { $week = [double]$info.utilization; $weekReset = $info.resetsAt }
        }
    }

    if ($null -eq $five -and $null -eq $week) { Write-Output "UNKNOWN usage could not be read (no rate_limit_event)"; exit 0 }

    $desc = "5-hour $(Fmt $five) (resets $(At $fiveReset)), weekly $(Fmt $week) (resets $(At $weekReset))"

    if ($null -ne $five -and $five -ge $SessionLimit) { Write-Output "STOP 5-hour limit at $(Fmt $five) (>= $(Fmt $SessionLimit)). $desc"; exit 1 }
    if ($null -ne $week -and $week -ge $WeeklyLimit) { Write-Output "STOP weekly limit at $(Fmt $week) (>= $(Fmt $WeeklyLimit)). $desc"; exit 1 }
    Write-Output "OK $desc"
    exit 0
} catch {
    Write-Output "UNKNOWN usage check failed: $($_.Exception.Message)"
    exit 0
}
