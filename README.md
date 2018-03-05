SlidingStone
============

A C# asynchronous/Parallel class library for logging application activities _(errors, user actions, etc)_, across a variety of dimensions _(ip, action type, username, etc...)_ and generate activity reports.

*Requires:* Azure Table Service.


Examples
============

### Generate Logs:

    CloudStorageAccount storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName={YourAccountName};AccountKey={YourAccountKey}");

    SlidingStone.SlidingStoneDataAccess slidingStoneDataAccess = new SlidingStone.SlidingStoneDataAccess(storageAccount);
    
    LogItem logItem = new LogItem();    
    logItem.LogType = SlidingStone.LogTypes.Account;
    logItem.ActivityType = SlidingStone.ActivityTypes.Asset_Downloaded;
    logItem.IPAddress = "111.1.11";
    logItem.UserName = "JohnSmith";
    logItem.Email = "jsmith@email.com";
    logItem.Company = "Smith Co.";
    logItem.Description = "Example log";

    await slidingStoneDataAccess.LogAsync(logItem);

