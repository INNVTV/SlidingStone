using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using SlidingStone.Models;
using SlidingStone.TableEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

            //Add each logtype into list of entities for parallel operation staging
            List<Object> entityTypes = new List<object>();
            entityTypes.Add(logTableEntity_IPAddress);
            entityTypes.Add(logTableEntity_Activity);
            entityTypes.Add(logTableEntity_Time);
            //entityTypes.Add(logTableEntity_UserName);
            //entityTypes.Add(logTableEntity_Company);

            List<Task> tasks = new List<Task>();
            foreach (ILogTableEntity obj in entityTypes)
            {
                //Transform the LogItem into each corresponding table entity type for insert execution into logs
                obj.Activity = logItem.ActivityType.ToString();
                obj.Company = logItem.Company;
                obj.Description = logItem.Description;
                obj.Email = logItem.Email;
                obj.IPAddress = logItem.IPAddress;
                obj.ObjectID = logItem.ObjectID;
                obj.UserName = logItem.UserName;

                //Create table for entity if not exists
                obj.cloudTable.CreateIfNotExists();

                //create an insert operation for each entity, assign to designated CloudTable, and add to our list of tasks:
                TableOperation operation = TableOperation.Insert((obj as TableEntity));           
                tasks.Add(Task.Factory.StartNew(() => obj.cloudTable.Execute(operation)));
            }
            
            try
            {
                //Execute all tasks in parallel
                Task.WaitAll(tasks.ToArray());
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
