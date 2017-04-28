netsh http add sslcert ipport=0.0.0.0:8443 certhash=4d7666e29c7c5db6b2e5af9bfdc43f84805c3ce5 appid={e79356c6-b412-429c-a96a-b5e2b5bfb550}
netsh http add urlacl url=https://+:8443/Webhook user=nserver\north

set CUR_PATH=%~dp0
%systemroot%\Microsoft.NET\Framework64\v4.0.30319\installutil "%CUR_PATH%YellowDuck.LearnChineseBotService.exe" 
sc failure YellowDuck.LearnChineseBotService reset= 0 actions= restart/60000
pause