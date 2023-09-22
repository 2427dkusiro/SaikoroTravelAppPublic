using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using SaikoroTravelBackend.Models;

using SaikoroTravelCommon.HttpModels;

namespace SaikoroTravelBackend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly UserContext _context;

    public UserController(UserContext context)
    {
        _context = context;
    }

    // GET: api/User
    [HttpGet]
    public async Task<ActionResult<IEnumerable<DbUserInfo>>> GetUserTokenInfos()
    {
        return _context.UserTokenInfos == null ? (ActionResult<IEnumerable<DbUserInfo>>)NotFound() : (ActionResult<IEnumerable<DbUserInfo>>)await _context.UserTokenInfos.ToListAsync();
    }

    // GET: api/User/5
    [HttpGet("{id}")]
    public async Task<ActionResult<DbUserInfo>> GetUserTokenInfo(int id)
    {
        if (_context.UserTokenInfos == null)
        {
            return NotFound();
        }
        DbUserInfo? userTokenInfo = await _context.UserTokenInfos.FindAsync(id);

        return userTokenInfo == null ? (ActionResult<DbUserInfo>)NotFound() : (ActionResult<DbUserInfo>)userTokenInfo;
    }

    // POST: api/User
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<UserRegisterResponse>> PostUserTokenInfo(UserRegisterRequest userTokenInfo)
    {
        if (_context.UserTokenInfos == null)
        {
            return Problem("Entity set 'UserContext.UserTokenInfos'  is null.");
        }

        if (string.IsNullOrEmpty(userTokenInfo.Code))
        {
            DbUserInfo? item = _context.UserTokenInfos.FirstOrDefault(x => x.User == userTokenInfo.User);
            if (item is null)
            {
                throw new KeyNotFoundException();
            }
            item.Token = userTokenInfo.Token;
            _ = await _context.SaveChangesAsync();
            return new UserRegisterResponse()
            {
                IsOK = true,
                Message = null!
            };
        }

        if (UserCheck.Check(userTokenInfo.User, userTokenInfo.Code))
        {
            _ = _context.UserTokenInfos.Add(new DbUserInfo()
            {
                User = userTokenInfo.User,
                Token = userTokenInfo.Token
            });
            _ = await _context.SaveChangesAsync();
            return new UserRegisterResponse()
            {
                IsOK = true,
                Message = null!
            };
        }

        return new UserRegisterResponse()
        {
            IsOK = false,
            Message = "ユーザー確認に失敗しました"
        };
    }
}
