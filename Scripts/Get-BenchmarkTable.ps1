<#
.SYNOPSIS
Outputs Markdown comparison tables from BenchmarkDotNet JSON reports.
.DESCRIPTION
Requires the comparison benchmarks below exported with --exporters json. Accepts either
the artifacts directory or its results subdirectory. Writes Markdown to standard
output without modifying files or running benchmarks.
.EXAMPLE
$filters = @(
    '*.GetNamesBenchmark.*'
    '*.GetValuesBenchmark.*'
    '*.ToStringBenchmark.*'
    '*.IsDefinedBenchmark.*'
    '*.LargeIsDefinedBenchmark.*'
    '*.TryParseBenchmark.*'
    '*.LargeTryParseBenchmark.*'
)
dotnet run -c Release --project Src/FastEnum.Benchmarks -- --filter $filters --exporters json --artifacts BenchmarkDotNet.Artifacts/comparison
./Scripts/Get-BenchmarkTable.ps1 -ResultsPath ./BenchmarkDotNet.Artifacts/comparison/results
.NOTES
The filters select only the seven classes used by these tables. They exclude
the hash-table experiments, flags, display-name parsing, and other unrelated
benchmarks. All parameter combinations in the selected classes still run.
Use a fresh artifacts directory when changing runtime, machine, or job settings.
To preview the selected methods without running benchmarks, add --list flat to
the dotnet command above.
#>
[CmdletBinding()]
param(
    [Parameter(Mandatory)]
    [string]$ResultsPath
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest
$culture = [cultureinfo]::InvariantCulture
$directory = (Resolve-Path -LiteralPath $ResultsPath).Path
if (Test-Path -LiteralPath (Join-Path $directory 'results') -PathType Container) {
    $directory = Join-Path $directory 'results'
}

$files = @(Get-ChildItem -LiteralPath $directory -Filter '*-report*.json' -File)
if ($files.Count -eq 0) { throw "No BenchmarkDotNet JSON reports found in $directory. Export with --exporters json." }

$benchmarks = [System.Collections.Generic.List[object]]::new()
$hostInfo = $null
$hostSignature = $null
foreach ($file in $files) {
    $report = Get-Content -LiteralPath $file.FullName -Raw | ConvertFrom-Json
    $hostData = $report.HostEnvironmentInfo
    $signature = @($hostData.ProcessorName, $hostData.OsVersion, $hostData.RuntimeVersion, $hostData.BenchmarkDotNetVersion) -join '|'
    if ($null -ne $hostSignature -and $signature -cne $hostSignature) {
        throw 'Reports contain different benchmark environments. Supply reports from a single run.'
    }
    $hostSignature = $signature
    $hostInfo = $hostData
    foreach ($benchmark in $report.Benchmarks) { $benchmarks.Add($benchmark) }
}

function Get-Mean([string]$Type, [string]$Method, [string]$Parameters) {
    $results = @($benchmarks | Where-Object {
        $_.Type -ceq $Type -and $_.Method -ceq $Method -and [string]$_.Parameters -ceq $Parameters
    })
    if ($results.Count -ne 1) {
        throw "Expected one result for $Type.$Method [$Parameters], found $($results.Count). Use a complete, single-job run without duplicate exports."
    }
    if ($null -eq $results[0].Statistics -or $null -eq $results[0].Statistics.Mean) {
        throw "Missing measurements for $Type.$Method [$Parameters]."
    }
    $mean = [double]$results[0].Statistics.Mean
    if ([double]::IsNaN($mean) -or [double]::IsInfinity($mean) -or $mean -lt 0) {
        throw "Invalid mean for $Type.$Method [$Parameters]."
    }
    return $mean.ToString('N2', $culture) + ' ns'
}

function New-Row([string]$Label, [string]$Size, [string]$Type, [string]$Fast, [string]$Net, [string]$Enums, [string]$Parameters = '') {
    $a = Get-Mean $Type $Fast $Parameters
    $b = Get-Mean $Type $Net $Parameters
    $c = if ($Enums) { Get-Mean $Type $Enums $Parameters } else { 'Not measured' }
    return "| $Label | $Size | $a | $b | $c |"
}

$common = @(
    '| Operation | Enum size | FastEnum | .NET | Enums.NET |'
    '|---|---|---:|---:|---:|'
    foreach ($operation in @('Get names', 'Get values', 'ToString')) {
        $method = switch ($operation) {
            'Get names' { 'GetNames' }
            'Get values' { 'GetValues' }
            'ToString' { 'ToString' }
        }
        $parameters = if ($operation -eq 'ToString') { 'Value=Second&LargeValue=Value1023' } else { '' }
        foreach ($suffix in @('', 'LargeEnum')) {
            $size = if ($suffix) { 'Large enum' } else { 'Small enum' }
            New-Row $operation $size "${method}Benchmark" "FastEnum$method$suffix" "Enum$method$suffix" "EnumsNet$method$suffix" $parameters
        }
    }
    New-Row 'IsDefined, hit' 'Small enum' 'IsDefinedBenchmark' 'FastEnumIsDefined' 'EnumIsDefined' 'EnumsNetIsDefined' 'Value=Third'
    New-Row 'IsDefined, hit' 'Large enum' 'LargeIsDefinedBenchmark' 'FastEnumIsDefined' 'EnumIsDefined' 'EnumsNetIsDefined' 'Value=Value1023'
    New-Row 'IsDefined, miss' 'Small enum' 'IsDefinedBenchmark' 'FastEnumIsDefined' 'EnumIsDefined' 'EnumsNetIsDefined' 'Value=-1'
    New-Row 'IsDefined, miss' 'Large enum' 'LargeIsDefinedBenchmark' 'FastEnumIsDefined' 'EnumIsDefined' 'EnumsNetIsDefined' 'Value=-1'
)
$parsing = @(
    '| Input / scenario | Enum size | FastEnum | .NET | Enums.NET |'
    '|---|---|---:|---:|---:|'
    foreach ($large in @($false, $true)) {
        $type = if ($large) { 'LargeTryParseBenchmark' } else { 'TryParseBenchmark' }
        $size = if ($large) { 'Large enum' } else { 'Small enum' }
        $hit = if ($large) { 'Value1023' } else { 'Third' }
        foreach ($span in @($false, $true)) {
            $suffix = if ($span) { 'Span' } else { '' }
            $inputType = if ($span) { 'Span' } else { 'String' }
            $enums = if ($span) { '' } else { 'EnumsNetTryParse' }
            New-Row "$inputType, hit" $size $type "FastEnumTryParse$suffix" "EnumTryParse$suffix" $enums "Input=$hit&IgnoreCase=False"
            New-Row "$inputType, miss" $size $type "FastEnumTryParse$suffix" "EnumTryParse$suffix" $enums 'Input=Missing&IgnoreCase=False'
            New-Row "$inputType, ignore case" $size $type "FastEnumTryParse$suffix" "EnumTryParse$suffix" $enums "Input=$($hit.ToLowerInvariant())&IgnoreCase=True"
        }
    }
)

$markdown = @(
    '#### Common operations'
    ''
    $common
    ''
    '#### Parsing'
    ''
    'Exact matches and misses are case-sensitive. Ignore-case rows use a lowercase name. The large-enum hit is the last member (Value1023).'
    ''
    $parsing
)
Write-Output ($markdown -join [Environment]::NewLine)