using dp.business.Enums;
using System;
using System.Collections.Generic;

namespace dp.business.Models

{
    public class ApiKey : ApiKeyCommon
    {
        public int ApiKeyId { get; set; }
        public string ApiKeyValue { get; set; }
        public string Name { get; set; }

    }
    public class ApiKeyCommon
    {
        public int TeamId { get; set; }
        public KeyActiveState ActiveState { get; set; }
    }
    public class TeamPhoto
    {
        public int TeamId { get; set; }
        public string ImageGuid { get; set; }
        public string Description { get; set; }
    }
    public class TeamPhotoResponse : TeamPhoto
    {
        public DateTime Created { get; set; }
    }

    public class TeamFlat
    {
        public int TeamId { get; set; }
        public string TeamName { get; set; }
    }
    public class Team : TeamFlat
    {

        public DateTime Updated { get; set; }

    }
    public class TeamInfo : Team
    {
        public int AdminUserId { get; set; }
        public DateTime Created { get; set; }
        public string AdminEmail { get; set; }
        public bool IsTrial { get; set; }

    }
    public class TeamInfoGridReponse
    {
        public int Total { get; set; }
        public List<TeamInfo> Data { get; set; }
    }


    public class TeamMember
    {
        public int UserId { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Role { get; set; }
        public bool IsTeamAdmin { get; set; }
        public bool IsPending { get; set; }
    }
    public class TeamMemberGridReponse
    {
        public int Total { get; set; }
        public List<TeamMember> Data { get; set; }
    }

    public class TeamAdminResponse : TeamInfo
    {
        public List<TeamPhotoResponse> TeamPhotos { get; set; }



    }

}