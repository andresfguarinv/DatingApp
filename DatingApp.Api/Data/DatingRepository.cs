using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.Api.Helpers;
using DatingApp.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.Api.Data
{
    public class DatingRepository: IDatingRepository
    {
        private readonly DataContext context;

        public DatingRepository(DataContext context)
        {
            this.context = context;
        }

        public void Add<T>(T entity) where T : class
        {
            context.Add(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
            context.Remove(entity);
        }

        public async Task<User> GetUser(int id)
        {
            var user = await context.Users.Include(p => p.Photos).FirstOrDefaultAsync(u => u.Id == id);
            return user;
        }

        public async Task<PagedList<User>> GetUsers(ListParams listParams)
        {
            var users = context.Users.Include(p => p.Photos).AsQueryable();
            users = users.Where(u => u.Id != listParams.UserId);
            users = users.Where(u => u.Gender == listParams.Gender);
            
            if (listParams.MinAge != 18 || listParams.MaxAge != 99) {
                var minDob =  DateTime.Today.AddYears(-listParams.MaxAge - 1);
                var maxDob = DateTime.Today.AddYears(-listParams.MinAge);
                users = users.Where(u=> u.DateOfBirth >= minDob && u.DateOfBirth <= maxDob);
            }

            if (!string.IsNullOrEmpty(listParams.OrderBy))
            {
                switch (listParams.OrderBy) {
                    case "created":
                        users = users.OrderByDescending(u => u.Created);
                        break;
                    default:
                        users = users.OrderByDescending(u => u.LastActive);
                        break;

                }
            }

            return await PagedList<User>.CreateAsync(users, listParams.PageNumber, listParams.PageSize);
        }

        public async Task<Photo> GetPhoto(int id)
        {
            var photo = await context.Photos.FirstOrDefaultAsync(p => p.Id == id);
            return photo;
        }

        public async Task<bool> SaveAll()
        {
            return await context.SaveChangesAsync() > 0;
        }

        public async Task<Photo> GetMainPhotoForUser(int userId)
        {
            return await context.Photos.FirstOrDefaultAsync(p => p.UserId == userId && p.IsMain == true);
        }
    }
}