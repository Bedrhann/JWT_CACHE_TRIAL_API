using Hafta._4.Application.Dto;
using Hafta._4.Domain.Entities;
using Hafta._4.Domain.Entities.Identity;
using Hafta._4.Persistence.Context;
using Hafta._4.Persistence.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace Hafta._4.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly TokenCreate _tokenCreate;
        private readonly IMemoryCache _memoryCache;
        private readonly IDistributedCache _distributedCache;

        public HomeController(AppDbContext context, UserManager<AppUser> userManager, IMemoryCache memoryCache, IDistributedCache distributedCache)
        {
            _context = context;
            _userManager = userManager;
            _memoryCache = memoryCache;
            _distributedCache = distributedCache;
        }
        /// <summary>
        /// Kullancının mail ve şifre bilgisine göre eğer kayıtlı bir kullanıcı ise token oluşturup geri dönen endpoint.
        /// </summary>
        /// <param name="Ulogin"></param>
        /// <returns></returns>
        [HttpPost("Login")]
        public async Task<IActionResult> UserLogin([FromBody] UserLoginDto Ulogin)
        {
            List<Claim> claims = new List<Claim>();
            var user = await _userManager.FindByEmailAsync(Ulogin.Email);
            if (user == null) return BadRequest("No Users with this mail");

            var result = await _userManager.CheckPasswordAsync(user, Ulogin.Password);
            if (result)
            {

                var roles = await _userManager.GetRolesAsync(user);
                foreach (var role in roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }

                claims.Add(new Claim(ClaimTypes.Name, user.Email));
                claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));

                var token = _tokenCreate.Create(claims);//Persistence katmanına erişerek token oluşturuyoruz.

                var handler = new JwtSecurityTokenHandler();
                string jwt = handler.WriteToken(token);

                return Ok(new
                {
                    token = jwt,
                    expiration = token.ValidTo
                });
            }

            return Unauthorized();
        }

        [HttpGet("MessageCache")]
        [ResponseCache(Duration = 300, VaryByHeader = "Messages", VaryByQueryKeys = new string[] { "MessageID" })]
        public IEnumerable<Message> MessageCache(int MessageId)//5 Dakika içinde aynı ıd ile Message tablosuna istek gelirse veritabına gitmeden hazır veriler döndürülecek.
        {
            Message[] messages = _context.Messages.Where(x => x.MessageID == MessageId).ToArray();
            if (_memoryCache.TryGetValue("messages", out messages))
            {
                return messages;
            }

            var MessageByts = _distributedCache.Get("messages");
            var MessageJson = Encoding.UTF8.GetString(MessageByts);
            var MessageArr = JsonSerializer.Deserialize<Message[]>(MessageJson);
            MemoryCacheEntryOptions memoryCacheEntryOptions = new MemoryCacheEntryOptions();
            memoryCacheEntryOptions.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(8);
            memoryCacheEntryOptions.SlidingExpiration = TimeSpan.FromHours(3);
            memoryCacheEntryOptions.Priority = CacheItemPriority.Normal;
            _memoryCache.Set("messages", messages, memoryCacheEntryOptions);
            var MesssageArr = JsonSerializer.Serialize(messages);
            _distributedCache.Set("messages", Encoding.UTF8.GetBytes(MesssageArr));
            return messages;
        }

    }
}
