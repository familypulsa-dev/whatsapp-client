param (
    [Parameter(Mandatory=$true)]
    [string]$Version 
)

Write-Host "Mulai proses build WaDesktop versi $Version..." -ForegroundColor Cyan

# 1. Pastikan VPK terinstall
Write-Host "1. Mengecek Velopack (vpk)..." -ForegroundColor Yellow
if (-not (Get-Command "vpk" -ErrorAction SilentlyContinue)) {
    Write-Host "vpk belum terinstall. Menginstall vpk global..."
    dotnet tool install -g vpk
}

# 2. Build Frontend
Write-Host "2. Build Frontend (React)..." -ForegroundColor Yellow
Set-Location "wa-frontend"
npm ci
npm run build:desktop
Set-Location ".."

# 3. Restore & Build Backend/Desktop (C#)
Write-Host "3. Build Desktop App (C#)..." -ForegroundColor Yellow
Set-Location "wa-desktop"

# --- GENERATE ASSEMBLY INFO ---
Write-Host "   -> Injecting Version $Version ke AssemblyInfo.cs..." -ForegroundColor Cyan
$propsDir = "WaDesktop.Client\Properties"
if (-not (Test-Path $propsDir)) {
    New-Item -ItemType Directory -Force -Path $propsDir | Out-Null
}
@"
using System.Reflection;
using System.Runtime.InteropServices;

[assembly: AssemblyTitle("WaDesktop")]
[assembly: AssemblyDescription("WhatsApp Desktop Client")]
[assembly: AssemblyConfiguration("Release")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("WaDesktop")]
[assembly: AssemblyCopyright("Copyright © $((Get-Date).Year)")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: ComVisible(false)]
[assembly: Guid("c3d4e5f6-a7b8-9012-cdef-123456789012")]
[assembly: AssemblyVersion("$Version.0")]
[assembly: AssemblyFileVersion("$Version.0")]
"@ | Out-File -FilePath "$propsDir\AssemblyInfo.cs" -Encoding utf8
# ------------------------------

nuget restore WaDesktop.sln
msbuild WaDesktop.sln /p:Configuration=Release /p:Platform="Any CPU" /m
Set-Location ".."

# 4. Pack dengan Velopack
Write-Host "4. Packaging dengan Velopack..." -ForegroundColor Yellow
vpk pack --packId "WaDesktop" --packTitle "WaDesktop" --packVersion $Version --packDir "wa-desktop\WaDesktop.Client\bin\Release" --mainExe "WaDesktop.exe"

Write-Host "Selesai." -ForegroundColor Green