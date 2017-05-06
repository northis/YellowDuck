netsh http add sslcert ipport=0.0.0.0:443 certhash=9c2e81a4a24ca5f0a388b83205caba5fb320063b appid={e79356c6-b412-429c-a96a-b5e2b5bfb550}
netsh http add urlacl url=https://+:443/ user=Все

set CUR_PATH=%~dp0
%systemroot%\Microsoft.NET\Framework64\v4.0.30319\installutil "%CUR_PATH%YellowDuck.LearnChineseBotService.exe" 
sc failure YellowDuck.LearnChineseBotService reset= 0 actions= restart/60000
pause