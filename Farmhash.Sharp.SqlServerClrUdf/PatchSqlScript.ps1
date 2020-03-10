
function ConvertFileToHexString{
	param (
		[Parameter(Mandatory=$true)][string]$inputDllFilename
	)

	$inputDllContent = Get-Content $inputDllFilename -Encoding Byte ` -ReadCount 0 

	$dllHexDump = "0x"

	foreach ( $byte in $inputDllContent ) {
		$dllHexDump += "{0:X2}" -f $byte
	}

	return $dllHexDump;
}

$outputSqlFilename = ".\FarmhashInstallation.sql"
$outputSqlContent = Get-Content $outputSqlFilename

$outputSqlContent = $outputSqlContent -replace '_FARMHASH_SHARP_SAFE_DLL_HEX_', (ConvertFileToHexString ".\Farmhash.Sharp.Safe.dll")
$outputSqlContent = $outputSqlContent -replace '_FARMHASH_SHARP_SAFE_PDB_HEX_', (ConvertFileToHexString ".\Farmhash.Sharp.Safe.pdb")
$outputSqlContent = $outputSqlContent -replace '_FARMHASH_SHARP_SQLSERVERCLRUDF_DLL_HEX_', (ConvertFileToHexString ".\Farmhash.Sharp.SqlServerClrUdf.dll")
$outputSqlContent = $outputSqlContent -replace '_FARMHASH_SHARP_SQLSERVERCLRUDF_PDB_HEX_', (ConvertFileToHexString ".\Farmhash.Sharp.SqlServerClrUdf.pdb")

$outputSqlContent | Set-Content -Path $outputSqlFilename

