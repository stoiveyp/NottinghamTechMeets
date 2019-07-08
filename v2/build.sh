mkdir lambdaoutput
dotnet lambda package gettalk.zip -pl ./src/NottTechMeet_GetTalk -o ./lambdaoutput/gettalk.zip                           
cdk deploy --profile alexaskills --require-approval never
