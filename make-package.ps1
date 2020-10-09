$formsVersion = "3.6.0.220655"

$orgName = "$orgName"
$projectName = "Tabs"

$netstandardProject = ".\$projectName\$projectName\$projectName.csproj"
$droidProject = ".\$projectName\$projectName.Droid\$projectName.Droid.csproj"
$iosProject = ".\$projectName\$projectName.iOS\$projectName.iOS.csproj"
$uwpProject = ".\$projectName\$projectName.UWP\$projectName.UWP.csproj"

rm *.txt

echo "  Setting Xamarin.Forms version to $formsVersion"

$findXFVersion = '(Xamarin.Forms">\s+<Version>)(.+)(</Version>)'
$replaceString = "`$1 $formsVersion `$3"

(Get-Content $netstandardProject -Raw) -replace $findXFVersion, "$replaceString" | Out-File $netstandardProject
(Get-Content $droidProject -Raw) -replace $findXFVersion, "$replaceString" | Out-File $droidProject
(Get-Content $iosProject -Raw) -replace $findXFVersion, "$replaceString" | Out-File $iosProject
(Get-Content $uwpProject -Raw) -replace $findXFVersion, "$replaceString" | Out-File $uwpProject

echo "  deleting android bin-obj folders"
rm -Force -Recurse $droidBin
rm -Force -Recurse $droidObj

echo "  cleaning $orgName.$projectName solution"
msbuild .\$projectName\$projectName.sln /t:Clean

if ($LastExitCode -gt 0)
{
    echo "  Error while cleaning solution"
    return
}

echo "  restoring $orgName.$projectName solution packages"
msbuild .\$projectName\$projectName.sln /t:Restore

if ($LastExitCode -gt 0)
{
    echo "  Error while restoring packages"
    return
}

echo "  building $orgName.$projectName solution"
msbuild .\$projectName\$projectName.sln /t:Build /p:Configuration=Release

if ($LastExitCode -gt 0)
{
    echo "  Error while building solution"
    return
}

$version = (Get-Item $projectName\bin\Release\netstandard2.0\$orgName.$projectName.dll).VersionInfo.FileVersion

echo "  packaging $orgName.$projectName.nuspec (v$version)"
nuget pack .\$orgName.$projectName.nuspec -Version $version