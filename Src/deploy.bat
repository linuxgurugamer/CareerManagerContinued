

set H=r:\KSP_1.3.1_dev\GameData

copy bin\Debug\CareerManager.dll ..\GameData\CareerManager\Plugins

cd ..
xcopy /y /E /I GameData\CareerManager %H%\CareerManager
