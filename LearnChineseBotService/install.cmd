set CUR_PATH=%~dp0
%systemroot%\Microsoft.NET\Framework64\v4.0.30319\installutil "%CUR_PATH%YellowDuck.LearnChineseBotService.exe" 
sc failure YellowDuck.LearnChineseBotService reset= 0 actions= restart/60000
pause