# Minimalistic SSO/2FA project
Simple sso project used to test different 2FA strategies

The project consists of four directories:
1. api
2. client
3. client2
4. identity

Run identity and client with "dotnet watch run". The /Home/Privacy url requires authorization. 
Go to https://localhost:6001. 
Click on Privacy in the nav-bar. 
Log in with username: scott and password: password

You will now be logged in to client2 (https://localhost:8001/Home/Privacy) as well. 
