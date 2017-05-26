

set H=r:\KSP_1.3.0_dev\GameData

copy bin\Debug\CareerManager.dll ..\GameData\CareerManager\Plugins

cd ..
xcopy /y /E GameData\CareerManager %H%\CareerManager