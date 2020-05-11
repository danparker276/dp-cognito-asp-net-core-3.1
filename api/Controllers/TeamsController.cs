using dp.api.Models;
using dp.business.Enums;
using dp.business.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.Swagger.Annotations;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace dp.api.Controllers
{

    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class TeamsController : BaseController
    {

        public TeamsController()
        {

        }

        /// <summary>
        /// Get teams by various query
        /// </summary>
        /// <param name="search"> Search team by name or email</param>
        /// <param name="expand">Level of details</param>
        /// <param name="limit">The number of team data in return</param>
        /// <param name="sort"></param>
        /// <param name="sortby"> Credits=0 (Default), Updated=1, TeamName=2</param>
        /// <returns></returns>
        [Authorize(Roles = Role.Admin)]
        [HttpGet]
        [SwaggerResponse(200, "TeamInfo", typeof(TeamInfoGridReponse))]
        public async Task<IActionResult> GetAllTeams(
            string search,
            int limit = 10,
            Sort sort = Sort.Asc,
            TeamSort sortby = TeamSort.Credits,
            TeamExpand expand = TeamExpand.Flat
        )
        {
            if (expand == TeamExpand.Flat)
            {
                List<TeamFlat> teams = await AdoNetDao.TeamDao.GetFlatTeams(search, sort, sortby, limit);
                return Ok(teams);
            }
            if (expand == TeamExpand.Info)
            {
                List<TeamInfo> teams = await AdoNetDao.TeamDao.GetAllTeams(search, sort == Sort.Asc, sortby, limit);
                return Ok(teams);

            }

            return NotFound();
        }

        [Authorize(Roles = Role.Admin)]
        [HttpGet("{id}")]
        [SwaggerResponse(200, "Team Information", typeof(TeamInfo))]
        public async Task<IActionResult> GetOneTeam(
            int id,
            string search,
            int limit = 10,
            Sort sort = Sort.Asc,
            TeamSort sortby = TeamSort.Credits,
            TeamExpand expand = TeamExpand.Flat
        )
        {
            TeamAdminResponse team = await AdoNetDao.TeamDao.GetTeamInfo(id);
            if (expand == TeamExpand.Flat) return Ok(team);
            if (expand == TeamExpand.Images)
            {
                team.TeamPhotos = await AdoNetDao.TeamDao.GetTeamImageDocs(id);
                return Ok(team);
            }
            return NotFound();
        }

        /// <summary>
        /// Gets the top 50 results given the search Name or search Email (only use 1 of those)
        /// </summary>
        /// <param name="searchName"> Search Team Name</param>
        /// <param name="searchEmail">Search by Admin Email (only use Team or Email)</param>
        /// <param name="sortAsc">default false</param>
        /// <param name="teamSort"> Credits=0 (Default), Updated=1,TeamName=2</param>
        /// <returns></returns>
        [Authorize(Roles = Role.Admin)]
        [HttpGet("get-teams-list-grid")]
        [SwaggerResponse(200, "TeamInfoGridReponse", typeof(TeamInfoGridReponse))]
        public async Task<IActionResult> GetTeamsGrid(string searchName, string searchEmail, bool sortAsc = false, int teamSort = 0)
        {
            TeamInfoGridReponse tig = new TeamInfoGridReponse();
            //get email and team from token
            List<TeamInfo> lt = await AdoNetDao.TeamDao.GetTeamInfoList(searchName, searchEmail, sortAsc, (TeamSort)teamSort);
            tig.Data = lt;
            tig.Total = lt.Count;
            return Ok(lt);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost("AddTeamImage")]
        public async Task<IActionResult> AddTeamImage(TeamPhoto model)
        {
            if (!User.IsInRole(Role.Admin))
            {
                var currentUserId = int.Parse(User.Identity.Name);
                User user = await AdoNetDao.UserDao.GetUserInfo(currentUserId);
                model.TeamId = (int)user.TeamId;
            }

            await AdoNetDao.TeamDao.AddTeamImage(model.TeamId, model.ImageGuid, model.Description);
            await AdoNetDao.TeamDao.UpdateTeamDate(model.TeamId);

            return Ok();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="teamId"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("GetTeamImages")]
        public async Task<IActionResult> GetTeamImages(int? teamId)
        {

            teamId = await ValidateTeamId(teamId);
            //Get productions and API keys for user plus credits...
            return Ok(await AdoNetDao.TeamDao.GetTeamImageDocs((int)teamId));
        }

        [Authorize(Roles = Role.Admin)]
        [HttpGet("GetTeamInfo")]
        [SwaggerResponse(200, "TeamInfo", typeof(TeamInfo))]
        public async Task<IActionResult> GetTeamInfo(int teamId)
        {
            TeamAdminResponse tar = await AdoNetDao.TeamDao.GetTeamInfo(teamId);
            tar.TeamPhotos = await AdoNetDao.TeamDao.GetTeamImageDocs(teamId);
            return Ok(tar);
        }

    }
}