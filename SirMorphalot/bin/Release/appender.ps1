# Set paths
$exePath = Join-Path -Path (Get-Location) -ChildPath "SirMorphalot.exe"
$csFile = Join-Path -Path (Get-Location) -ChildPath "..\..\Program.cs"

# Overlay data
$labelStr = "firstOL16Bit...."
$keyStr = "firstKey16Bit..."
$iVectorStr = "firstVector16Bit"

# Convert strings to byte arrays
$key = [System.Text.Encoding]::UTF8.GetBytes($keyStr)
$iv = [System.Text.Encoding]::UTF8.GetBytes($iVectorStr)
$label = [System.Text.Encoding]::UTF8.GetBytes($labelStr)

# Read the source code from the file
$sourceCode = Get-Content $csFile -Raw

# Encrypt the source code
function EncryptStringToBytes_Aes {
    param (
        [string]$plainText,
        [byte[]]$Key,
        [byte[]]$IV
    )

    # Set up AES encryption
    $aesAlg = [System.Security.Cryptography.Aes]::Create()
    $aesAlg.Key = $Key
    $aesAlg.IV = $IV
    $encryptor = $aesAlg.CreateEncryptor()

    # Perform encryption
    $msEncrypt = New-Object System.IO.MemoryStream
    $csEncrypt = New-Object System.Security.Cryptography.CryptoStream($msEncrypt, $encryptor, [System.Security.Cryptography.CryptoStreamMode]::Write)
    $swEncrypt = New-Object System.IO.StreamWriter($csEncrypt)

    $swEncrypt.Write($plainText)
    $swEncrypt.Close()
    $csEncrypt.Close()
    $encrypted = $msEncrypt.ToArray()

    # Free up memory
    $msEncrypt.Dispose()
    $csEncrypt.Dispose()

    return $encrypted
}

# Append data to the file using FileStream
function appendOverlay {
    param (
        [string]$exeFilePath,
        [byte[]]$overLayLabel,
        [byte[]]$encSourceCode
    )

    # Open the EXE file in append mode and add the overlay
    $fs = [System.IO.File]::OpenWrite($exeFilePath)
    $fs.Seek(0, [System.IO.SeekOrigin]::End) | Out-Null  # Go to the end of the file and suppress output

    # Write the overlay (label + encrypted code)
    $fs.Write($overLayLabel, 0, $overLayLabel.Length) | Out-Null
    $fs.Write($encSourceCode, 0, $encSourceCode.Length) | Out-Null

    # Close the file
    $fs.Close()
}

# Encrypt the source code
$encSourceCode = EncryptStringToBytes_Aes $sourceCode $key $iv

# Append the overlay (label + encrypted code) to the EXE file
appendOverlay $exePath $label $encSourceCode

Write-Host "Overlay successfully appended."
