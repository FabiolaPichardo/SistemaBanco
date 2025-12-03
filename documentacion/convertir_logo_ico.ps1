# Script para convertir logo.png a logo.ico
$sourcePath = "imagenes\logo.png"
$destPath = "imagenes\logo.ico"

Write-Host "Convirtiendo logo.png a logo.ico..." -ForegroundColor Cyan

if (-not (Test-Path $sourcePath)) {
    Write-Host "Error: No se encontro el archivo" -ForegroundColor Red
    exit 1
}

Add-Type -AssemblyName System.Drawing

try {
    $img = [System.Drawing.Image]::FromFile((Resolve-Path $sourcePath))
    $bitmap = New-Object System.Drawing.Bitmap $img
    $icon = [System.Drawing.Icon]::FromHandle($bitmap.GetHicon())
    $fileStream = [System.IO.File]::Create($destPath)
    $icon.Save($fileStream)
    $fileStream.Close()
    $bitmap.Dispose()
    $img.Dispose()
    
    Write-Host "Conversion exitosa: $destPath" -ForegroundColor Green
}
catch {
    Write-Host "Error al convertir: $_" -ForegroundColor Red
    Write-Host "Use un convertidor en linea: https://convertio.co/es/png-ico/" -ForegroundColor Yellow
    exit 1
}
