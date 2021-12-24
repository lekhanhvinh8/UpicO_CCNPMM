using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Upico.Core.Domain;
using Upico.Core.ServiceResources;

namespace Upico.Core.Services
{
    public interface IAdminService
    {
        public Task<int> CountMessage();
        public Task<int> CountLike();
        public Task<int> CountAccess();
        public Task<IList<AppUser>> GetNewUser(int number);
        public Task<IList<AppUser>> GetOnlineUser(int number);
        public Task<IList<DateAccessCount>> GetDateAccessCount(DateTime date);
        public Task<IList<MonthAccessCount>> GetMonthAccessCount(DateTime date);
        public Task<IList<YearAccessCount>> GetYearAccessCount(DateTime date);
        public Task<IList<Counter>> GetDateNewUsers(DateTime date);
        public Task<IList<Counter>> GetMonthNewUsers(DateTime date);
        public Task<IList<Counter>> GetYearNewUsers(DateTime date);
        public Task GenerateAccessData(string username, DateTime date, string mode, int numberOfData);
        public Task GenerateAccountData(DateTime date, string mode, int numberOfData);

    }
}
