@echo off
title Learning Platform - Stop All Services

echo ========================================
echo   Stopping all services
echo ========================================
echo.

:: Закрываем окна по заголовку
taskkill /FI "WINDOWTITLE eq Learning Platform - .NET Server" /F > nul 2>&1
taskkill /FI "WINDOWTITLE eq Learning Platform - Python AI" /F > nul 2>&1
taskkill /FI "WINDOWTITLE eq Learning Platform - Vue Client" /F > nul 2>&1

:: Альтернативный способ - по портам
for /f "tokens=5" %%a in ('netstat -aon ^| find ":5004" ^| find "LISTENING"') do (
    taskkill /F /PID %%a > nul 2>&1
)

for /f "tokens=5" %%a in ('netstat -aon ^| find ":8001" ^| find "LISTENING"') do (
    taskkill /F /PID %%a > nul 2>&1
)

for /f "tokens=5" %%a in ('netstat -aon ^| find ":8080" ^| find "LISTENING"') do (
    taskkill /F /PID %%a > nul 2>&1
)

echo All services stopped
echo.
pause
