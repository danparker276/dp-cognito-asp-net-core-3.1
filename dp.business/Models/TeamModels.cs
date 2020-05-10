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
    public class TeamPhotoDoc
    {
        public int TeamId { get; set; }
        public string ImageGuid { get; set; }
        public string Description { get; set; }
    }
    public class TeamPhotoDocResponse : TeamPhotoDoc
    {
        public DateTime Created { get; set; }
    }


    public class UpdateTeamDocDescription
    {
        public int TeamId { get; set; }
        public string Description { get; set; }
        public string NewDescription { get; set; }
    }
    public class TeamFlat
    {
        public int TeamId { get; set; }
        public string TeamName { get; set; }
        public int? Credits { get; set; }
    }
    public class Team : TeamFlat
    {
        /// <summary>
        /// Team can access production
        /// </summary>
        public bool ProductionAccess { get; set; }
        public DateTime Updated { get; set; }
        public bool IsHomePay { get; set; }
    }
    public class TeamInfo : Team
    {
        public int AdminUserId { get; set; }
        public DateTime Created { get; set; }
        public string AdminEmail { get; set; }
        public string CreditNotes { get; set; }

    }
    public class TeamInfoGridReponse
    {
        public int Total { get; set; }
        public List<TeamInfo> Data { get; set; }
    }

    public class CreditsRequest
    {
        public int ProductId { get; set; }
        public int TeamId { get; set; }
        public int Credits { get; set; }
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
    public class TeamApiPermission
    {
        public int ApiId { get; set; }
        public int TeamId { get; set; }
    }
    public class TeamIsHomePayRequest
    {
        public bool IsHomePay { get; set; }
        public int TeamId { get; set; }
    }

}