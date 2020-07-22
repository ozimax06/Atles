﻿using System;
using System.Threading.Tasks;
using Atlas.Domain;
using Atlas.Server.Services;
using Atlas.Shared.Models.Admin.Forums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Atlas.Domain.Forums.Commands;
using Atlas.Domain.Forums;
using Atlas.Shared.Forums;

namespace Atlas.Server.Controllers.Admin
{
    [Authorize(Policy = "Admin")]
    [Route("api/admin/forum-groups")]
    [ApiController]
    public class ForumsController : ControllerBase
    {
        private readonly IContextService _contextService;
        private readonly IForumService _forumGroupService;
        private readonly IForumRules _forumGroupRules;
        private readonly IForumModelBuilder _modelBuilder;

        public ForumsController(IContextService contextService,
            IForumService forumGroupService,
            IForumRules forumGroupRules,
            IForumModelBuilder modelBuilder)
        {
            _contextService = contextService;
            _forumGroupService = forumGroupService;
            _forumGroupRules = forumGroupRules;
            _modelBuilder = modelBuilder;
        }

        [HttpGet("index/{forumGroupId}")]
        public async Task<IndexModel> Index(Guid forumGroupId)
        {
            var site = await _contextService.CurrentSiteAsync();

            return await _modelBuilder.BuildIndexModelAsync(forumGroupId);
        }

        [HttpGet("create/{forumGroupId}")]
        public async Task<FormModel> Create(Guid forumGroupId)
        {
            var site = await _contextService.CurrentSiteAsync();

            return await _modelBuilder.BuildCreateFormModelAsync(site.Id, forumGroupId);
        }

        [HttpPost("save")]
        public async Task<ActionResult> Save(FormModel.ForumModel model)
        {
            var site = await _contextService.CurrentSiteAsync();
            var member = await _contextService.CurrentMemberAsync();

            var command = new CreateForum
            {
                ForumGroupId = model.ForumGroupId,
                Name = model.Name,
                PermissionSetId = model.PermissionSetId == Guid.Empty ? (Guid?)null : model.PermissionSetId,
                SiteId = site.Id,
                MemberId = member.Id
            };

            await _forumGroupService.CreateAsync(command);

            return Ok();
        }

        [HttpGet("edit/{id}")]
        public async Task<ActionResult<FormModel>> Edit(Guid id)
        {
            var site = await _contextService.CurrentSiteAsync();

            var result = await _modelBuilder.BuildEditFormModelAsync(site.Id, id);

            if (result == null)
            {
                return NotFound();
            }

            return result;
        }

        [HttpPost("update")]
        public async Task<ActionResult> Update(FormModel.ForumModel model)
        {
            var site = await _contextService.CurrentSiteAsync();
            var member = await _contextService.CurrentMemberAsync();

            var command = new UpdateForum
            {
                Id = model.Id,
                ForumGroupId = model.ForumGroupId,
                Name = model.Name,
                PermissionSetId = model.PermissionSetId == Guid.Empty ? (Guid?)null : model.PermissionSetId,
                SiteId = site.Id,
                MemberId = member.Id
            };

            await _forumGroupService.UpdateAsync(command);

            return Ok();
        }

        [HttpPost("move-up")]
        public async Task<ActionResult> MoveUp([FromBody] Guid id)
        {
            var site = await _contextService.CurrentSiteAsync();
            var member = await _contextService.CurrentMemberAsync();

            var command = new MoveForum
            {
                Id = id,
                SiteId = site.Id,
                MemberId = member.Id,
                Direction = Direction.Up
            };

            await _forumGroupService.MoveAsync(command);

            return Ok();
        }

        [HttpPost("move-down")]
        public async Task<ActionResult> MoveDown([FromBody] Guid id)
        {
            var site = await _contextService.CurrentSiteAsync();
            var member = await _contextService.CurrentMemberAsync();

            var command = new MoveForum
            {
                Id = id,
                SiteId = site.Id,
                MemberId = member.Id,
                Direction = Direction.Down
            };

            await _forumGroupService.MoveAsync(command);

            return Ok();
        }

        [HttpDelete("delete/{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            var site = await _contextService.CurrentSiteAsync();
            var member = await _contextService.CurrentMemberAsync();

            var command = new DeleteForum
            {
                Id = id,
                SiteId = site.Id,
                MemberId = member.Id
            };

            await _forumGroupService.DeleteAsync(command);

            return Ok();
        }

        [HttpGet("is-name-unique/{forumGroupId}/{name}")]
        public async Task<IActionResult> IsNameUnique(Guid forumGroupId, string name)
        {
            var site = await _contextService.CurrentSiteAsync();
            var isNameUnique = await _forumGroupRules.IsNameUniqueAsync(forumGroupId, name);
            return Ok(isNameUnique);
        }

        [HttpGet("is-name-unique/{forumGroupId}/{name}/{id}")]
        public async Task<IActionResult> IsNameUnique(Guid forumGroupId, string name, Guid id)
        {
            var site = await _contextService.CurrentSiteAsync();
            var isNameUnique = await _forumGroupRules.IsNameUniqueAsync(forumGroupId, name, id);
            return Ok(isNameUnique);
        }
    }
}
