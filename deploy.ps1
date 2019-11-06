[CmdletBinding()]
param (
    [Parameter(Mandatory=$false, HelpMessage="Enter your config name defined in developers.json: ")][string]$configName
)

$environment = Get-Content 'developers.json' | Out-String | ConvertFrom-Json

$config = $environment.$configName

$stackName = "pecuniary-securitycommand-stack"

$developerPrefix = $config.Prefix

Write-Host "`Parameters from " -NoNewline
Write-Host "developers.json:`n" -ForegroundColor Cyan
Write-Host "`tdeveloperPrefix: `t`t $developerPrefix" -ForegroundColor Yellow

$sourceFile = "samTemplate.yaml"
$localFileName = "$sourceFile.local"
Write-Host "`nCreating/updating $localFileName based on $sourceFile..."

Copy-Item samTemplate.yaml $localFileName

if ($config.Prefix)
{  
    Write-Host "`n`tDeveloper config selected" -ForegroundColor Yellow

    $stackName = $developerPrefix-$stackName

    (Get-Content $localFileName) `
        -replace 'pecuniary-', "$developerPrefix-pecuniary-" `
        -replace 'Name: pecuniary', "Name: $developerPrefix-pecuniary" |
    Out-File $localFileName -Encoding utf8
}

Write-Host "`nDone! $localFileName updated. Please use this file when deploying to our own AWS stack.`n"

if ($config.Prefix) 
{ 
    Write-Host "Press [enter] to continue deploying stack to AWS (Ctrl+C to exit)" -NoNewline -ForegroundColor Green
    Read-Host
}

$samTemplate = 'samTemplate.yaml.local'

Write-Host "`n`nPrebuild:"

dotnet restore Pecuniary.Security.Command/Pecuniary.Security.Command.csproj

Write-Host "`n`nBuild:"

dotnet publish Pecuniary.Security.Command/Pecuniary.Security.Command.csproj
  
Write-Host "`n`nDeploy:"

dotnet-lambda deploy-serverless `
    --stack-name $stackName `
    --template $samTemplate `
    --region us-west-2 `
    --s3-bucket pecuniary-deployment-artifacts

# Get the API Gateway Base URL
$stack = aws cloudformation describe-stacks --stack-name $stackName | ConvertFrom-Json
$outputKey = $stack.Stacks.Outputs.OutputKey.IndexOf("PecuniaryApiGatewayBaseUrl")
$apiGatewayBaseUrl = $stack.Stacks.Outputs[$outputKey].OutputValue

# Add scopes
Write-Host "`n`Adding Scopes to $apiGatewayBaseUrl"
aws lambda invoke `
    --function-name "pecuniary-AddScopes" `
    --payload """{ """"ApiGatewayBaseUrl"""": """"$apiGatewayBaseUrl"""" }""" `
    outfile.json
Remove-Item outfile.json

Write-Host "`n`n YOUR STACK NAME IS:   $stackName   `n`n"
 