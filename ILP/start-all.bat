@echo off
cd /d "e:\Programming\Repositories\Interactive_learning_platform\ILP"

echo ========================================
echo   Learning Platform - Starting Services
echo ========================================
echo.


echo Starting Python AI on port 8001...
start "PY" cmd /k "cd src\py_ai && echo [Python] AI service on http://localhost:8001 && uvicorn main:app --reload --host locahost --port 8001"

timeout /t 3


echo Starting .NET server on port 5004...
start "NET" cmd /k "cd src\server && echo [.NET] Server running on http://localhost:5004 && dotnet watch run"

timeout /t 3


echo Starting Vue client on port 54114...
start "VUE" cmd /k "cd src\client\ilp.client && echo [Vue] Client running on http://localhost:54114 && npm run serve"

timeout /t 8

echo Opening browser...
start http://localhost:54114/hub.html

echo.
echo ========================================
echo   Services started
echo ========================================
echo.
echo   Vue Client:  http://localhost:54114
echo   .NET Swagger: http://localhost:5004/swagger
echo   Python Health: http://localhost:8001/health
echo.
echo   If Python Health shows error - restart manually:
echo   cd src\py_ai && uvicorn main:app --reload --port 8001
echo.
