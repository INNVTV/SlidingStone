using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using SlidingStone.Models;
using SlidingStone.TableEntities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SlidingStone
{
    public class SlidingStoneDataAccess
    {

        CloudStorageAccount _storageAccount;

        public SlidingStoneDataAccess(CloudStorageAccount storageAccount)
        {
            _storageAccount = storageAccount;
        }


        public async Task<SlidingStoneResponseType> LogAsync(LogItem logItem)
        {
            SlidingStoneResponseType response = new SlidingStoneResponseType();

            CloudTableClient cloudTableClient = _storageAccount.CreateCloudTableClient();

            //Create an instance of each entity type and pass in associated CloudTableClient & TableName
            LogTableEntity_IPAddress logTableEntity_IPAddress = new LogTableEntity_IPAddress(cloudTableClient, logItem.LogType.ToString().ToLower() + "" + "byip");
            LogTableEntity_Activity logTableEntity_Activity = new LogTableEntity_Activity(cloudTableClient, logItem.LogType.ToString().ToLower() + "" + "byactivity");
            LogTableEntity_Time logTableEntity_Time = new LogTableEntity_Time(cloudTableClient, logItem.LogType.ToString().ToLower() + "" + "bytime");
            //LogTableEntity_UserName logTableEntity_UserName = new LogTableEntity_UserName(cloudTableClient, logItem.LogType.ToString().ToLower() + "" + "byuser");
            //LogTableEntity_Company logTableEntity_Company = new LogTableEntity_Company(cloudTableClient, logItem.LogType.ToString().ToLower() + "" + "bycompany");

            //Gather up all the entities into a list for our parallel task to execute in a ForEach
            List<Object> entityTypes = new List<object>();
            entityTypes.Add(logTableEntity_IPAddress);
            entityTypes.Add(logTableEntity_Activity);
            entityTypes.Add(logTableEntity_Time);
            //entityTypes.Add(logTableEntity_UserName);
            //entityTypes.Add(logTableEntity_Company);

            try
            {
                
                Parallel.ForEach(entityTypes, obj =>
                {
                    
                    #region Trace Statements

                    //Display the id of the thread for each parallel instance
                    //Trace.TraceInformation("Current thread ID: " + Thread.CurrentThread.ManagedThreadId);

                    #endregion

                    //Transform the LogItem into each corresponding table entity type for insert execution into logs
                    (obj as ILogTableEntity).Activity = logItem.ActivityType.ToString();
                    (obj as ILogTableEntity).Company = logItem.Company;
                    (obj as ILogTableEntity).Description = logItem.Description;
                    (obj as ILogTableEntity).Email = logItem.Email;
                    (obj as ILogTableEntity).IPAddress = logItem.IPAddress;
                    (obj as ILogTableEntity).ObjectID = logItem.ObjectID;
                    (obj as ILogTableEntity).UserName = logItem.UserName;

                    //Create table for entity if not exists
                    (obj as ILogTableEntity).cloudTable.CreateIfNotExists();

                    //create an insert operation for each entity, assign to designated CloudTable, and add to our list of tasks:
                    TableOperation operation = TableOperation.Insert((obj as TableEntity));
                    (obj as ILogTableEntity).cloudTable.Execute(operation);
                });
            }
            catch (Exception e)
            {
                response.isSuccess = false;
                response.errorId = 0;
                response.errorMessage = e.Message;

                return response;
            }


            response.isSuccess = true;
            response.successId = 1;

            return response;
        }
    }
}
