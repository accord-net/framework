@echo off

cd Scripts
powershell -ExecutionPolicy Bypass -File UpdateVersion.ps1
cd ..