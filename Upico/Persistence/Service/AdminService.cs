using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Upico.Core.Domain;
using Upico.Core.ServiceResources;
using Upico.Core.Services;

namespace Upico.Persistence.Service
{
    public class AdminService : IAdminService
    {
        private readonly UpicODbContext _context;
        private readonly IUserService _userService;

        public AdminService(UpicODbContext context, IUserService userService)
        {
            this._context = context;
            this._userService = userService;
        }

        public async Task<int> CountMessage()
        {
            return await this._context.Messages.CountAsync();
        }

        public async Task<int> CountLike()
        {
            return await this._context.Posts.SelectMany(p => p.Likes).CountAsync();
        }
        public async Task<int> CountAccess()
        {
            return await this._context.AccessLogs.CountAsync();
        }

        public async Task<IList<AppUser>> GetNewUser(int number)
        {
            var users = await this._context.Users.OrderByDescending(u => u.CreatedAt).Take(number).ToListAsync();
            foreach (var user in users)
            {
                await this._context.Avatars.Where(a => a.IsMain && a.UserID == user.Id).LoadAsync();
            }

            return users;
        }
        public async Task<IList<AppUser>> GetOnlineUser(int number)
        {
            var now = DateTime.Now;
            var before = now.AddSeconds(-10);

            return await this._context.Users.Where(u => u.LastAccessed > before).OrderByDescending(u => u.LastAccessed).Take(number).ToListAsync();
        }

        public async Task<IList<DateAccessCount>> GetDateAccessCount(DateTime date)
        {
            var startOfDate = date.Date;
            var nextDate = date.Date.AddDays(1);


            var dateAccessCounts = new List<DateAccessCount>();

            for (int hour = 0; hour < 24; hour++)
            {
                var startOfHour = startOfDate.AddHours(hour);
                var nextHour = startOfDate.AddHours(hour + 1);

                var accessCount = await this._context.AccessLogs.Where(a => a.LogTime >= startOfHour && a.LogTime < nextHour).CountAsync();

                var dateAccessCount = new DateAccessCount();
                dateAccessCount.Hour = hour;
                dateAccessCount.AccessesCount = accessCount;


                dateAccessCounts.Add(dateAccessCount);
            }

            return dateAccessCounts;
        }

        public async Task<IList<MonthAccessCount>> GetMonthAccessCount(DateTime date)
        {
            var firstDayOfMonth = new DateTime(date.Year, date.Month, 1);
            var days = DateTime.DaysInMonth(date.Year, date.Month);
            var monthAccessCounts = new List<MonthAccessCount>(); 

            for (int day = 0; day < days; day++)
            {
                var startOfDate = firstDayOfMonth.AddDays(day);
                var nextDate = firstDayOfMonth.AddDays(day + 1);

                var accessCount = await this._context.AccessLogs.Where(a => a.LogTime >= startOfDate && a.LogTime < nextDate).CountAsync();
                
                var monthAccessCount = new MonthAccessCount();
                monthAccessCount.Date = startOfDate.Day;
                monthAccessCount.AccessesCount = accessCount;

                monthAccessCounts.Add(monthAccessCount);
            }


            return monthAccessCounts;
        }

        public async Task<IList<YearAccessCount>> GetYearAccessCount(DateTime date)
        {
            var firstDayOfYear = new DateTime(date.Year, 1, 1);
            var yearAccessCounts = new List<YearAccessCount>();

            for (int month = 0; month < 12; month++)
            {
                var startOfMonth = firstDayOfYear.AddMonths(month);
                var nextMonth = firstDayOfYear.AddMonths(month + 1);

                var accessCount = await this._context.AccessLogs.Where(a => a.LogTime >= startOfMonth && a.LogTime < nextMonth).CountAsync();

                var yearAccessCount = new YearAccessCount();
                yearAccessCount.Month = startOfMonth.Month;
                yearAccessCount.AccessesCount = accessCount;

                yearAccessCounts.Add(yearAccessCount);
                    
            }

            return yearAccessCounts;
        }

        public async Task<IList<Counter>> GetDateNewUsers(DateTime date)
        {
            var startOfDate = date.Date;

            var dateCounts = new List<Counter>();

            for (int hour = 0; hour < 24; hour++)
            {
                var startOfHour = startOfDate.AddHours(hour);
                var nextHour = startOfDate.AddHours(hour + 1);

                var count = await this._context.Users.Where(u => u.CreatedAt >= startOfHour && u.CreatedAt < nextHour).CountAsync();

                var counter = new Counter();
                counter.Name = hour.ToString();
                counter.Count = count;


                dateCounts.Add(counter);
            }

            return dateCounts;
        }
        public async Task<IList<Counter>> GetMonthNewUsers(DateTime date)
        {
            var firstDayOfMonth = new DateTime(date.Year, date.Month, 1);
            var days = DateTime.DaysInMonth(date.Year, date.Month);
            var monthCounts = new List<Counter>();

            for (int day = 0; day < days; day++)
            {
                var startOfDate = firstDayOfMonth.AddDays(day);
                var nextDate = firstDayOfMonth.AddDays(day + 1);

                var count = await this._context.Users.Where(u => u.CreatedAt >= startOfDate && u.CreatedAt < nextDate).CountAsync();

                var monthCount = new Counter();
                monthCount.Name = startOfDate.Day.ToString();
                monthCount.Count = count;

                monthCounts.Add(monthCount);
            }


            return monthCounts;
        }
        public async Task<IList<Counter>> GetYearNewUsers(DateTime date)
        {
            var firstDayOfYear = new DateTime(date.Year, 1, 1);
            var yearCounts = new List<Counter>();

            for (int month = 0; month < 12; month++)
            {
                var startOfMonth = firstDayOfYear.AddMonths(month);
                var nextMonth = firstDayOfYear.AddMonths(month + 1);

                var count = await this._context.Users.Where(u => u.CreatedAt >= startOfMonth && u.CreatedAt < nextMonth).CountAsync();

                var yearCount = new Counter();
                yearCount.Name = startOfMonth.Month.ToString();
                yearCount.Count = count;

                yearCounts.Add(yearCount);

            }

            return yearCounts;
        }

        public async Task GenerateAccessData(string username, DateTime date, string mode, int numberOfData)
        {
            var user = await this._context.Users.SingleOrDefaultAsync(u => u.UserName == username);
            if (user == null)
                return;

            if(mode == "year")
            {
                var firstMonth = new DateTime(date.Year, 1, 1);

                for (int i = 0; i < numberOfData; i++)
                {
                    Random rnd = new Random();
                    int randomMonth = rnd.Next(0, 12);
                    var day = DateTime.DaysInMonth(date.Year, randomMonth + 1);
                    int randomDay = rnd.Next(0, day);
                    int randomHour = rnd.Next(0, 24);

                    var randomDate = firstMonth.AddMonths(randomMonth).AddDays(randomDay).AddHours(randomHour);

                    var accessLog = new AccessLog();
                    accessLog.UserId = user.Id;
                    accessLog.LogTime = randomDate;

                    await this._context.AccessLogs.AddAsync(accessLog);

                }
            }
            else if(mode == "month")
            {
                var firstDate = new DateTime(date.Year, date.Month, 1);

                for (int i = 0; i < numberOfData; i++)
                {
                    var days = DateTime.DaysInMonth(date.Year, date.Month);

                    Random rnd = new Random();
                    int randomDay = rnd.Next(0, days);
                    int randomHour = rnd.Next(0, 24);

                    var randomDate = firstDate.AddDays(randomDay).AddHours(randomHour);

                    var accessLog = new AccessLog();
                    accessLog.UserId = user.Id;
                    accessLog.LogTime = randomDate;

                    await this._context.AccessLogs.AddAsync(accessLog);
                }
            }
            else if(mode == "day")
            {
                var firstHour = date.Date;

                for (int i = 0; i < numberOfData; i++)
                {
                    Random rnd = new Random();
                    int randomHour = rnd.Next(0, 24);

                    var randomDate = firstHour.AddHours(randomHour);

                    var accessLog = new AccessLog();
                    accessLog.UserId = user.Id;
                    accessLog.LogTime = randomDate;

                    await this._context.AccessLogs.AddAsync(accessLog);
                }
            }

            await this._context.SaveChangesAsync();
        }

        public async Task GenerateAccountData(DateTime date, string mode, int numberOfData)
        {
            if (mode == "year")
            {
                var firstMonth = new DateTime(date.Year, 1, 1);

                for (int i = 0; i < numberOfData; i++)
                {
                    Random rnd = new Random();
                    int randomMonth = rnd.Next(0, 12);
                    var day = DateTime.DaysInMonth(date.Year, randomMonth + 1);
                    int randomDay = rnd.Next(0, day);
                    int randomHour = rnd.Next(0, 24);

                    var randomDate = firstMonth.AddMonths(randomMonth).AddDays(randomDay).AddHours(randomHour);

                    //Generate user
                    var randomUserName = Guid.NewGuid();
                    var password = "User11";

                    var registerRequest = new RegisterRequest();
                    registerRequest.Username = randomUserName.ToString();
                    registerRequest.Email = randomUserName.ToString() + "@gmail.com";
                    registerRequest.Password = password;

                    await this._userService.Register(registerRequest);

                    var newUser = await this._context.Users.SingleOrDefaultAsync(u => u.UserName == randomUserName.ToString());
                    newUser.CreatedAt = randomDate;
                    await this._context.SaveChangesAsync();

                }
            }
            else if (mode == "month")
            {
                var firstDate = new DateTime(date.Year, date.Month, 1);

                for (int i = 0; i < numberOfData; i++)
                {
                    var days = DateTime.DaysInMonth(date.Year, date.Month);

                    Random rnd = new Random();
                    int randomDay = rnd.Next(0, days);
                    int randomHour = rnd.Next(0, 24);

                    var randomDate = firstDate.AddDays(randomDay).AddHours(randomHour);


                    //Generate user
                    var randomUserName = Guid.NewGuid();
                    var password = "User11";

                    var registerRequest = new RegisterRequest();
                    registerRequest.Username = randomUserName.ToString();
                    registerRequest.Email = randomUserName.ToString() + "@gmail.com";
                    registerRequest.Password = password;

                    await this._userService.Register(registerRequest);

                    var newUser = await this._context.Users.SingleOrDefaultAsync(u => u.UserName == randomUserName.ToString());
                    newUser.CreatedAt = randomDate;
                    await this._context.SaveChangesAsync();
                }
            }
            else if (mode == "day")
            {
                var firstHour = date.Date;

                for (int i = 0; i < numberOfData; i++)
                {
                    Random rnd = new Random();
                    int randomHour = rnd.Next(0, 24);

                    var randomDate = firstHour.AddHours(randomHour);

                    //Generate user
                    var randomUserName = Guid.NewGuid();
                    var password = "User11";

                    var registerRequest = new RegisterRequest();
                    registerRequest.Username = randomUserName.ToString();
                    registerRequest.Email = randomUserName.ToString() + "@gmail.com";
                    registerRequest.Password = password;

                    await this._userService.Register(registerRequest);

                    var newUser = await this._context.Users.SingleOrDefaultAsync(u => u.UserName == randomUserName.ToString());
                    newUser.CreatedAt = randomDate;
                    await this._context.SaveChangesAsync();
                }
            }

        }


    }
}
