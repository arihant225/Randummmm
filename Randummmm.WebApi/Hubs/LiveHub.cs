﻿using Microsoft.AspNetCore.SignalR;
using Randummmm.WebApi.HelperFunctions;
using Randummmm.WebApi.HelperFunctions.Interface;
using dto=Randummmm.WebApi.DTOs;
namespace Randummmm.WebApi.Hubs
{
    public class LiveHub:Hub
    {
        private ILiveHelper liveHelper;
        public LiveHub(ILiveHelper liveHelper)
        {
            this.liveHelper = liveHelper;
            
        }



        public async Task EndCall()
        {
            if (LiveHelper.Matches.ContainsKey(Context.ConnectionId))
            {
                string friendConnectionId = LiveHelper.Matches[Context.ConnectionId];
                await Clients.Client(friendConnectionId).SendAsync("Disconnected", $"{LiveHelper.Connections[Context.ConnectionId]} left Chat");

                LiveHelper.Matches.Remove(Context.ConnectionId);
                LiveHelper.Matches.Remove(friendConnectionId);
            }

            if (LiveHelper.Requests.Contains(Context.ConnectionId))
            {
                LiveHelper.Requests.Remove(Context.ConnectionId);
            }
        }
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            // Call the base class method to ensure the default behavior is executed.
            await base.OnDisconnectedAsync(exception);

           await  this.EndCall();
        }
        public async Task GetConnectionId(string Name)
        {
            LiveHelper.Connections.Add(Context.ConnectionId, Name);
            await this.Clients.Caller.SendAsync("NewConnectionConfigured", Context.ConnectionId);
        }
        public async Task SendMessage(string toConnectionId, string message)
        {
            var Dto = new dto.TextMessage()

            {
                connectionId = Context.ConnectionId,
                Message = message,
                Time = DateTime.Now.TimeOfDay,
                name = LiveHelper.Connections[Context.ConnectionId]





            };
           
            await Clients.Client(toConnectionId).SendAsync("RecivedMessage", Dto);

            await Clients.Client(Context.ConnectionId).SendAsync("RecivedMessage", Dto);
        }
        public  void AddRequest()
        {
            if(!LiveHelper.Requests.Contains(Context.ConnectionId))
            LiveHelper.Requests.Add(Context.ConnectionId);
        }
        
        public async Task<string> CreateGroup(string groupName)
        {
            string tempId=Guid.NewGuid().ToString();
            LiveHelper.GroupNames.Add(tempId, groupName);
            await this.Groups.AddToGroupAsync(Context.ConnectionId, tempId);
            return tempId;
        }
    }
}