# WebApp
ExpressYourself - Novibet

For the whole project to work we have 2 main dependencies.
1) SQLExpress, where IPdb exists.
   I have the latest DB backup in the folder DatabaseBackUp
2) Redis used for caching.
   Redis should be listening on PORT 6379.
   The App is set to work with a local Redis instance. If you want to change it to
   a Managed Redis Hosted on cloud change the Redis Connection String named RedisConnStr in local.settings.json and 
   appsettings.json.
   
   I used Docker to develop and test using Redis. The command to get a Redis instance going is:
   
   docker run -p 6379:6379 --name ip-redis -d redis redis-server --save 60 1 --loglevel warning
 
 The project TimeTriggerFunc is a Azure Functions Project, for testing purposes so we dont have to wait for
 1 hour for the trigger, i have set << RunOnStartup = true >> which will run the trigger as soon as the project starts.
