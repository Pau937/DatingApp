﻿using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp.API.Data
{
	public class DatingRepository : IDatingRepository
	{
		public void Add<T>(T entity) where T : class
		{
			_context.Add(entity);
		}

		public void Delete<T>(T entity) where T : class
		{
			_context.Remove(entity);
		}

		public async Task<User> GetUser(int id)
		{
			return await _context.Users
				.Include(x => x.Photos)
				.FirstOrDefaultAsync(x => x.Id == id);
		}

		public async Task<IEnumerable<User>> GetUsers()
		{
			return await _context.Users
				.Include(x => x.Photos)
				.ToListAsync();
		}

		public async Task<bool> SaveAll()
		{
			return await _context.SaveChangesAsync() > 0;
		}

		public DatingRepository(DataContext context)
		{
			_context = context;
		}

		private readonly DataContext _context;
	}
}
