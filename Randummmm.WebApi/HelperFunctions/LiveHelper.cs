using Microsoft.AspNetCore.SignalR;
using Randummmm.WebApi.Hubs;
using Randummmm.WebApi.DTOs;
using Randummmm.WebApi.HelperFunctions.Interface;

namespace Randummmm.WebApi.HelperFunctions
{
    public class LiveHelper : BackgroundService
    {
        readonly IHubContext<LiveHub> HubContext ;
        public LiveHelper(IHubContext<LiveHub> HubContext) { 
            this.HubContext = HubContext;
        } 
        public static SortedList<string, string> Connections = new();
        public static SortedList<string, string> GroupNames = new();
        public static SortedList<string,string> Matches = new();
        public static   List<string>  Requests=new();

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var clients = this.HubContext.Clients;
           return Task.Run(async () =>
            {
                Random rnd = new Random();
                while (true)
                {

                    while (LiveHelper.Requests.Count > 0)
                    {
                        string tempConnectionId = Requests[0];
                        if (Requests.Count > 1)
                        {
                            int index = rnd.Next(1, Requests.Count);
                            if (index < Requests.Count) {
                                string FriendConnectionId = Requests[index];
                                if (!string.IsNullOrEmpty(FriendConnectionId))
                                { 
                                 
                                  Requests.Remove(tempConnectionId);
                                  Requests.Remove(FriendConnectionId);

                                await clients.Client(FriendConnectionId).SendAsync("FoundMatch", 
                                      new Requests() {
                                      ConnectionId=tempConnectionId,
                                      Name=Connections[tempConnectionId]}
                                      );
                                
                                    await  clients.Client(tempConnectionId).SendAsync("FoundMatch",
                                       new Requests()
                                       {
                                           ConnectionId = FriendConnectionId,
                                           Name = Connections[FriendConnectionId]
                                       });
                                   
                               
                                    Matches.Add(tempConnectionId,FriendConnectionId);
                                    Matches.Add(FriendConnectionId, tempConnectionId);
                                }
                            }
                        }
                    }
                    Thread.Sleep(1000);
                }


            });
            

        }

       
    }
}
