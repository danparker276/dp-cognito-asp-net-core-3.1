using dp.business.Enums;
using dp.data.AdoNet.SqlExecution;
using dp.business.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace dp.data.AdoNet.DataAccessObjects
{
    public class TeamDao : BaseDao
    {
        public TeamDao(string dpDbConnectionString) : base(dpDbConnectionString)
        {
        }

        public async Task AddTeamImage(int teamId, string imageGuid, string description)
        {
            SqlQuery proc = new SqlQuery(@"    
                insert into team_images
                    (TeamId, ImageGuid, Description)
                values
                    (@teamId, @imageGuid, @description); 
                ", 30, System.Data.CommandType.Text);

            proc.AddInputParam("teamId", SqlDbType.Int, teamId);
            proc.AddInputParam("imageGuid", SqlDbType.NVarChar, imageGuid);
            proc.AddInputParam("description", SqlDbType.NVarChar, description);
            await _queryExecutor.ExecuteAsync(proc);

        }
        public async Task<TeamAdminResponse> GetTeamInfo(int teamId)
        {

            SqlQuery proc = new SqlQuery(@" 
                select 
                    t.teamId, 
                    t.name, 
                    t.AdminUserId, 
                    t.IsTrial,
                    t.IsHomePay, 
                    t.CreditNotes, 
                    u.Email, 
                    tc.Credits 
                from team t
                inner join users u on t.AdminUserId=u.UserId 
                left join team_credit tc on tc.TeamId=t.teamId
                where t.teamId=@teamId;
                ", 30, System.Data.CommandType.Text);
            proc.AddInputParam("teamId", SqlDbType.Int, teamId);
            return await _queryExecutor.ExecuteAsync(proc, dataReader =>
            {

                while (dataReader.Read())
                {
                    return (new TeamAdminResponse()
                    {
                        TeamId = SqlQueryResultParser.GetValue<Int32>(dataReader, "TeamId"),
                        AdminEmail = SqlQueryResultParser.GetValue<String>(dataReader, "Email"),
                        AdminUserId = SqlQueryResultParser.GetValue<Int32>(dataReader, "AdminUserId"),
                        TeamName = SqlQueryResultParser.GetValue<String>(dataReader, "Name"),
                        IsTrial = SqlQueryResultParser.GetValue<Boolean>(dataReader, "IsTrial")
                    });
                }

                return null;

            });


        }

        public async Task<List<TeamPhotoResponse>> GetTeamImageDocs(int teamId)
        {

            SqlQuery proc = new SqlQuery(@" 
                select * 
                from team_images 
                where teamId=@teamId;
                ", 30, System.Data.CommandType.Text);
            proc.AddInputParam("teamId", SqlDbType.Int, teamId);
            return await _queryExecutor.ExecuteAsync(proc, dataReader =>
            {
                List<TeamPhotoResponse> ltsd = new List<TeamPhotoResponse>();
                while (dataReader.Read())
                {
                    ltsd.Add(new TeamPhotoResponse()
                    {
                        TeamId = SqlQueryResultParser.GetValue<Int32>(dataReader, "TeamId"),
                        ImageGuid = SqlQueryResultParser.GetValue<String>(dataReader, "ImageGuid"),
                        Description = SqlQueryResultParser.GetValue<String>(dataReader, "Description"),
                        Created = SqlQueryResultParser.GetValue<DateTime>(dataReader, "Created")
                    });
                }
                return ltsd;


            });

        }

        public async Task UpdateTeamDocDescription(int teamId, string description, string newDescription)
        {
            SqlQuery proc = new SqlQuery(
                @"update team_images 
                set description = @newDescription 
                where teamId = @teamId and description = @description;",
                30, System.Data.CommandType.Text
                );

            proc.AddInputParam("teamId", SqlDbType.Int, teamId);
            proc.AddInputParam("newDescription", SqlDbType.NVarChar, newDescription);
            proc.AddInputParam("description", SqlDbType.NVarChar, description);
            await _queryExecutor.ExecuteAsync(proc);
        }


        public async Task SetIsTrial(int teamId, bool isTrial)
        {
            SqlQuery proc = new SqlQuery(@" 
          update team set IsTrial=@isTrial, Updated=GETUTCDATE() where teamId=@teamId;
                ", 30, System.Data.CommandType.Text);

            proc.AddInputParam("isTrial", SqlDbType.Bit, isTrial);
            proc.AddInputParam("teamId", SqlDbType.Int, teamId);
            await _queryExecutor.ExecuteAsync(proc);

        }
        public async Task UpdateTeamDate(int teamId)
        {
            SqlQuery proc = new SqlQuery(@" 
          update team set Updated=GETUTCDATE() where teamId=@teamId;
                ", 30, System.Data.CommandType.Text);

            proc.AddInputParam("teamId", SqlDbType.Int, teamId);
            await _queryExecutor.ExecuteAsync(proc);

        }

        public async Task<List<TeamInfo>> GetTeamInfoList(string searchName, string searchEmail, bool sortAsc, TeamSort teamSort)
        {
            string whereName = "";
            if (!string.IsNullOrEmpty(searchName))
            {
                whereName = " where Name like @searchName ";
            }
            string whereEmail = "";
            if (!string.IsNullOrEmpty(searchEmail))
            {
                whereEmail = " where u.email like @searchEmail ";
            }

            string sortSql = " t.Name ";
            if (teamSort == TeamSort.TeamName)
                sortSql = " t.Name ";
            else if (teamSort == TeamSort.Updated)
                sortSql = " t.Updated ";

            string sortAscSql = "";
            if (!sortAsc)
                sortAscSql = " desc ";
            List<TeamInfo> lt = new List<TeamInfo>();
            SqlQuery proc = new SqlQuery($@" 
                select top 50 t.*, tc.credits, u.email, u.Created
                from team t 
                inner join users u on u.userId=t.adminUserId
                left outer join team_credit tc on tc.TeamId=t.TeamId {whereEmail}  {whereName} 
                order by {sortSql} {sortAscSql}", 30, System.Data.CommandType.Text);


            proc.AddInputParam("@searchName", SqlDbType.NVarChar, "%" + searchName + "%", false);
            proc.AddInputParam("@searchEmail", SqlDbType.NVarChar, "%" + searchEmail + "%", false);
            await _queryExecutor.ExecuteAsync(proc, dataReader =>
            {
                while (dataReader.Read())
                {

                    TeamInfo tm = new TeamInfo()
                    {
                        TeamId = SqlQueryResultParser.GetValue<Int32>(dataReader, "TeamId"),
                        TeamName = SqlQueryResultParser.GetValue<String>(dataReader, "Name"),
                        AdminEmail = SqlQueryResultParser.GetValue<String>(dataReader, "Email"),
                        Created = SqlQueryResultParser.GetValue<DateTime>(dataReader, "Created"),
                        Updated = SqlQueryResultParser.GetValue<DateTime>(dataReader, "Updated"),
                        IsTrial = SqlQueryResultParser.GetValue<bool>(dataReader, "IsTrial"),
                        AdminUserId = SqlQueryResultParser.GetValue<Int32>(dataReader, "AdminUserId")

                    };
                    lt.Add(tm);
                }
                return lt;
            });

            return lt;
        }

        public async Task<List<TeamFlat>> GetFlatTeams(string search, Sort sort, TeamSort sortby, int limit)
        {
            string whereSql = "";
            if (!string.IsNullOrEmpty(search)) whereSql = " where Name like @search or u.email like @search";
            string selectSql = @"
                    t.TeamId,
                    t.Name,
                    tc.Credits
                ";

            string sortbySql;
            switch (sortby)
            {
                case TeamSort.Credits:
                    sortbySql = "tc.credits";
                    break;
                case TeamSort.Updated:
                    sortbySql = "t.Updated";

                    break;
                case TeamSort.TeamName:
                    sortbySql = "t.Name";
                    break;
                default:
                    throw new ArgumentOutOfRangeException("sortby argument is invalid");
            }

            string sortSql = "";
            if (sort == Sort.Asc) sortSql = " desc ";
            List<TeamFlat> teams = new List<TeamFlat>();
            SqlQuery proc = new SqlQuery($@" 
                select top {limit} {selectSql}
                from team t 
                left outer join team_credit tc on tc.TeamId=t.TeamId {whereSql} 
                order by {sortbySql} {sortSql}", 30, System.Data.CommandType.Text);

            proc.AddInputParam("@search", SqlDbType.NVarChar, "%" + search + "%", false);
            await _queryExecutor.ExecuteAsync(proc, dataReader =>
            {
                while (dataReader.Read())
                {

                    TeamInfo tm = new TeamInfo()
                    {
                        TeamId = SqlQueryResultParser.GetValue<Int32>(dataReader, "TeamId"),
                        TeamName = SqlQueryResultParser.GetValue<String>(dataReader, "Name")
                    };
                    teams.Add(tm);
                }
                return teams;
            });

            return teams;
        }
        public async Task<List<TeamInfo>> GetAllTeams(string search, bool sortasc, TeamSort sortby, int limit)
        {
            string whereSql = "";
            if (!string.IsNullOrEmpty(search)) whereSql = " where Name like @search or u.email like @search";
            string selectSql = @"
                    t.*,
                    tc.credits, 
                    u.email, 
                    u.Created 
                ";
            string sortbySql;
            switch (sortby)
            {
                case TeamSort.Updated:
                    sortbySql = "t.Updated";

                    break;
                case TeamSort.TeamName:
                    sortbySql = "t.Name";
                    break;
                default:
                    throw new ArgumentOutOfRangeException("sortby argument is invalid");
            }

            string sortAscSql = "";
            if (!sortasc) sortAscSql = " desc ";
            List<TeamInfo> teams = new List<TeamInfo>();
            SqlQuery proc = new SqlQuery($@" 
                select top {limit} {selectSql}
                from team t 
                inner join users u on u.userId=t.adminUserId
                left outer join team_credit tc on tc.TeamId=t.TeamId {whereSql} 
                order by {sortbySql} {sortAscSql}", 30, System.Data.CommandType.Text);

            proc.AddInputParam("@search", SqlDbType.NVarChar, "%" + search + "%", false);
            await _queryExecutor.ExecuteAsync(proc, dataReader =>
            {
                while (dataReader.Read())
                {
                    TeamInfo tm = new TeamInfo()
                    {
                        TeamId = SqlQueryResultParser.GetValue<Int32>(dataReader, "TeamId"),
                        TeamName = SqlQueryResultParser.GetValue<String>(dataReader, "Name"),
                        AdminEmail = SqlQueryResultParser.GetValue<String>(dataReader, "Email"),
                        Created = SqlQueryResultParser.GetValue<DateTime>(dataReader, "Created"),
                        Updated = SqlQueryResultParser.GetValue<DateTime>(dataReader, "Updated"),
                        IsTrial = SqlQueryResultParser.GetValue<bool>(dataReader, "IsTrial"),
                        AdminUserId = SqlQueryResultParser.GetValue<Int32>(dataReader, "AdminUserId")

                    };

                    teams.Add(tm);
                }
                return teams;
            });

            return teams;
        }

    }
}