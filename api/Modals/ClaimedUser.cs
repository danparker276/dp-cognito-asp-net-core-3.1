namespace dp.api.Models
{
    public struct ClaimedUser
    {
        public string Email { get; set; }
        public string  UserName { get; set; }
        public bool IsAdmin { get; set; }

        //add as much here to the claims as you want
    }
}