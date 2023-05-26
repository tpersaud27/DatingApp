namespace DatingApp.API.SignalR
{
    public class PresenceTracker
    {
        // First param is the username, second is the connection id (could be different devices)
        private static readonly Dictionary<string, List<string>> OnlineUsers = new Dictionary<string, List<string>>();

        // Everytime the user connects they are given a unique id

        public Task UserConnected(string username, string connectionId)
        {
            // Since dictionaries are not thread safe, multiple users accessing it concurrently can cause issues
            // Using the lock will allow us to add one user at a time to the dictionary
            lock(OnlineUsers)
            {
                // If the user is already connected we will just add their new connection id to the list in the dictionary
                if(OnlineUsers.ContainsKey(username))
                {
                    OnlineUsers[username].Add(connectionId);
                }
                // If they are not connected we should be creating a new dictionary item
                else
                {
                    OnlineUsers.Add(username, new List<string>{connectionId});
                }
            }

            return Task.CompletedTask;
        }

        public Task UserDisconnected(string username, string connectionId)
        {
            lock(OnlineUsers)
            {
                // If the user is not in the dictionary then we dont need to do anything
                // This means they are offline
                if(!OnlineUsers.ContainsKey(username)) return Task.CompletedTask;

                // Remove the connection id for the user
                OnlineUsers[username].Remove(connectionId); 

                // If the username is not in the dictionary we remove the user
                if(OnlineUsers[username].Count == 0)
                {
                    OnlineUsers.Remove(username);
                }
            }

            return Task.CompletedTask;
        }

        public Task<string[]> GetOnlineUsers()
        {
            string[] onlineUsers;

            lock(OnlineUsers)
            {
                // Getting alphabetically list of users from the dictionary
                // Here we just get a list of online users that are alphabetically ordered
                onlineUsers = OnlineUsers.OrderBy(k => k.Key).Select(k => k.Key).ToArray();
            }

            // This will return
            return Task.FromResult(onlineUsers);

        }

        // We will get the connection strings so we can send a notification to any device the user is on
        public static Task<List<string>> GetConnectionsForUser(string username)
        {
            List<string> connectionIds;

            lock(OnlineUsers)
            {
                connectionIds = OnlineUsers.GetValueOrDefault(username);
            }

            return Task.FromResult(connectionIds);
        }
        
    }
}
