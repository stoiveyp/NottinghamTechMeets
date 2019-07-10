mkdir lambdaoutput
dotnet build src
dotnet lambda package gettalk.zip -pl ./src/NottTechMeet_GetTalk -o ./lambdaoutput/gettalk.zip
dotnet lambda package getevents.zip -pl ./src/NottTechMeet_PullMeetupEvents -o ./lambdaoutput/getevents.zip
dotnet lambda package getdata.zip -pl ./src/NottTechMeet_PullMeetupData -o ./lambdaoutput/getdata.zip
cdk deploy --profile alexaskills --require-approval never
