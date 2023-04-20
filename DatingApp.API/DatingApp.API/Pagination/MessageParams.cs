using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DatingApp.API.Pagination
{
    public class MessageParams: PaginationParams
    {
        public string Username { get; set; }
        // This is either the unread or read inbox
        public string Container { get; set; } = "Unread";
    }
}
