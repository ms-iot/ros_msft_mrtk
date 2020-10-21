

function GenerateUnityMeta 
{

    param 
    (
        $templateName,
        $folder
    )

    $template = Get-Content $templateName -Raw

    $files = Get-ChildItem $folder
    
    foreach ($file in $files)
    {
        $ext = [IO.Path]::GetExtension($file)
        if ($ext -notmatch ".meta")
        {
            $guidRaw = New-Guid
            $guid = $guidRaw.ToString().Replace("-", "")
            $newMetafilename = $file.FullName + ".meta"
    
            Write-Host "Generating $newMetafilename"
    
            $newMetafile = $template.Replace("@@newGuid@@", $guid)
            Set-Content $newMetafilename $newMetafile
        }
    }
}

GenerateUnityMeta -templateName 'wsa_arm64_meta.txt' -folder ".\SampleProject\Assets\Plugins\WSA\arm64"
GenerateUnityMeta -templateName 'wsa_x64_meta.txt' -folder ".\SampleProject\Assets\Plugins\WSA\x64"
GenerateUnityMeta -templateName 'x86_64_meta.txt' -folder ".\SampleProject\Assets\Plugins\x86_64"

