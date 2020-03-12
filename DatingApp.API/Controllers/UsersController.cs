using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DatingApp.API.Controllers
{
	[Authorize]
	[ApiController]
	[Route("api/[controller]")]
	public class UsersController : ControllerBase
	{
		public UsersController(IDatingRepository repo, IMapper mapper)
		{
			_repo = repo;
			_mapper = mapper;
		}

		[HttpGet]
		public async Task<IActionResult> GetUsers()
		{
			var users = await _repo.GetUsers();

			var usersToReturn = _mapper.Map<IEnumerable<UserForListDto>>(users);

			return Ok(usersToReturn);
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> GetUser(int id)
		{
			var user = await _repo.GetUser(id);

			var userToReturn = _mapper.Map<UserForDetailedDto>(user);

			return Ok(userToReturn);
		}

		private readonly IDatingRepository _repo;
		private readonly IMapper _mapper;
	}
}
