// File: LinkifyPLL/ViewComponents/UserStatsViewComponent.cs
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using LinkifyDAL.Entities;
using LinkifyBLL.Services.Abstraction;
using LinkifyPLL.Models;

public class UserStatsViewComponent : ViewComponent
{
    private readonly UserManager<User> _userManager;
    private readonly IFriendsService _friendsService;
    private readonly IPostService _postService;
    private readonly IPostReactionsService _postReactionsService;

    public UserStatsViewComponent(
        UserManager<User> userManager,
        IFriendsService friendsService,
        IPostService postService,
        IPostReactionsService postReactionsService)
    {
        _userManager = userManager;
        _friendsService = friendsService;
        _postService = postService;
        _postReactionsService = postReactionsService;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var userId = _userManager.GetUserId(UserClaimsPrincipal);
        var user = userId != null ? await _userManager.FindByIdAsync(userId) : null;

        int connections = 0, posts = 0, reactions = 0;
        if (userId != null)
        {
            connections = await _friendsService.GetFriendCountAsync(userId);
            posts = await _postService.GetUserPostCountAsync(userId);
            reactions = (await _postReactionsService.GetReactionsByUserAsync(userId)).Count();
        }

        var model = new UserStatsVM
        {
            User = user,
            Connections = connections,
            Posts = posts,
            Reactions = reactions
        };

        return View(model);
    }
}

