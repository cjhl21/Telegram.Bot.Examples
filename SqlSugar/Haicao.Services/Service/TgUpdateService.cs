using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Haicao.Services.IService;
using SqlSugar;

namespace Haicao.Services.Service
{
    public class TgUpdateService : BaseService, ITgUpdateService
    {
        public TgUpdateService(ISqlSugarClient client) : base(client)
        {
        }
    }
}
