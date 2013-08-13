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

            //Create tables to store entities by designated values:
            CloudTable table_ip = cloudTableClient.GetTableReference(logItem.LogType.ToString().ToLower() + "" + "byip");
            CloudTable table_activity = cloudTableClient.GetTableReference(logItem.LogType.ToString().ToLower() + "" + "byactivity");
            CloudTable table_time = cloudTableClient.GetTableReference(logItem.LogType.ToString().ToLower() + "" + "bytime");

            //Create an instance of each entity type
            LogTableEntity_IPAddress logTableEntity_IPAddress = new LogTableEntity_IPAddress();
            LogTableEntity_Activity logTableEntity_Activity = new LogTableEntity_Activity();
            LogTableEntity_Time logTableEntity_Time = new LogTableEntity_Time();

            //Assign a corresponding CloudTable to each LogTableEntity for insert operations
            logTableEntity_IPAddress.cloudTable = table_ip;
            logTableEntity_Activity.cloudTable = table_activity;
            logTableEntity_Time.cloudTable = table_time;

            //Add each logtype into
            List<Object> entityTypes = new List<object>();
            entityTypes.Add(logTableEntity_IPAddress);
            entityTypes.Add(logTableEntity_Activity);
            entityTypes.Add(logTableEntity_Time);

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
