using DataAccess.Repositories.IRepositories;
using Entities.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.ProjectModel;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MeenA7rf.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class FriendController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        public FriendController(UserManager<ApplicationUser> userManager, IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
        }
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            
            if (user is null)
            {
                return NotFound();
            }

            var friends = await _unitOfWork.FriendshipRepository.GetAsync(e => e.Status == FriendshipStatus.Accepted
                && (e.PlayerId == user.Id || e.FriendId == user.Id)
                , include : new Expression<Func<Friendship, object>>[]
                {
                    e => e.Player,
                    e => e.Friend
                });

            var friendList = friends.Select(f =>
                f.PlayerId == user.Id ? f.Friend : f.Player).ToList();

            return View(friendList);
        }

        [HttpGet]
        public IActionResult AddFriend()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddFriend(string search)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user is null)
                return NotFound();

            if (string.IsNullOrWhiteSpace(search))
                return View(new List<ApplicationUser>());

            var results = await _userManager.Users
            .Where(e => (e.UserName!.Contains(search) || e.Email!.Contains(search)) && e.Id != user.Id)
            .ToListAsync();

            return View(results);
        }

        [HttpPost]
        public async Task<IActionResult> SendFriendRequest(string friendId)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return Json(new { success = false, message = "User not found." });

            var exists = await _unitOfWork.FriendshipRepository
            .GetOneAsync(f =>
                (f.PlayerId == user.Id && f.FriendId == friendId) ||
                (f.PlayerId == friendId && f.FriendId == user.Id));

            if (exists is not null)
                return Json(new { success = false, message = "Request already sent or you're already friends." });

            var friendship = new Friendship
            {
                PlayerId = user.Id,
                FriendId = friendId,
                Status = FriendshipStatus.Pending
            };

            await _unitOfWork.FriendshipRepository.CreateAsync(friendship);
            await _unitOfWork.FriendshipRepository.CommitAsync();

            return Json(new { success = true, message = "Friend request sent successfully!" });
        }

        public async Task<IActionResult> Requests()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user is null)
                return NotFound();

            var pending = await _unitOfWork.FriendshipRepository.GetAsync(
                e => e.FriendId == user.Id && e.Status == FriendshipStatus.Pending,
                include: new Expression<Func<Friendship, object>>[]
                { e => e.Player });

            return View(pending);
        }

        [HttpPost]
        public async Task<IActionResult> AcceptRequest(int requestId)
        {
            var request = await _unitOfWork.FriendshipRepository.GetOneAsync(e => e.Id == requestId);

            if (request == null)
                return NotFound();

            request.Status = FriendshipStatus.Accepted;
            await _unitOfWork.FriendshipRepository.UpdateAsync(request);
            await _unitOfWork.FriendshipRepository.CommitAsync();

            return RedirectToAction("Requests");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteRequest(int requestId)
        {
            var request = await _unitOfWork.FriendshipRepository
                .GetOneAsync(f => f.Id == requestId);

            if (request == null)
                return NotFound();

            await _unitOfWork.FriendshipRepository.DeleteAsync(request);
            await _unitOfWork.FriendshipRepository.CommitAsync();

            return RedirectToAction("Requests");
        }

        [HttpPost]
        public async Task<IActionResult> CancelRequest(string friendId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Json(new { success = false, message = "User not found." });

            var request = await _unitOfWork.FriendshipRepository
                .GetOneAsync(f =>
                    f.PlayerId == user.Id && f.FriendId == friendId && f.Status == FriendshipStatus.Pending);

            if (request == null)
                return Json(new { success = false, message = "No pending request found." });

            await _unitOfWork.FriendshipRepository.DeleteAsync(request);
            await _unitOfWork.FriendshipRepository.CommitAsync();

            return Json(new { success = true, message = "Friend request canceled." });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteFriend(string friendId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return NotFound();

            var friendship = await _unitOfWork.FriendshipRepository.GetOneAsync(f =>
                (f.PlayerId == user.Id && f.FriendId == friendId) ||
                (f.PlayerId == friendId && f.FriendId == user.Id));

            if (friendship == null)
                return NotFound();

            await _unitOfWork.FriendshipRepository.DeleteAsync(friendship);
            await _unitOfWork.FriendshipRepository.CommitAsync();

            TempData["success-notification"] = "Friend removed successfully.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> BlockFriend(string friendId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return NotFound();

            var friendship = await _unitOfWork.FriendshipRepository.GetOneAsync(f =>
                (f.PlayerId == user.Id && f.FriendId == friendId) ||
                (f.PlayerId == friendId && f.FriendId == user.Id));

            if (friendship == null)
            {
                friendship = new Friendship
                {
                    PlayerId = user.Id,
                    FriendId = friendId,
                    Status = FriendshipStatus.Blocked
                };
                await _unitOfWork.FriendshipRepository.CreateAsync(friendship);
            }
            else
            {
                friendship.Status = FriendshipStatus.Blocked;
                await _unitOfWork.FriendshipRepository.UpdateAsync(friendship);
            }

            await _unitOfWork.FriendshipRepository.CommitAsync();

            TempData["success-notification"] = "Friend blocked successfully.";
            return RedirectToAction("Index");
        }


    }
}
