using System;
using System.Collections.Generic;
using System.Linq;
using LSports.DVS.Framework.DataAccess.Models;
using LSports.DVS.Framework.DataAccess.Repositories;
using LSports.DVS.Framework.DataAccess.Repositories.Interfaces;
using LSports.Framework.Models.CustomClasses;
using LSports.Services.Interfaces;
using LSports.Services.ViewModels;

namespace LSports.Services
{
    public class ArrivalMessageService : IArrivalMessageService
    {
        private readonly IArrivalMessageRepository _arrivalMessageRepository;

        public ArrivalMessageService() : this(new ArrivalMessageRepository())
        {
        }

        public ArrivalMessageService(ArrivalMessageRepository arrivalMessageRepository)
        {
            _arrivalMessageRepository = arrivalMessageRepository;
        }

        public List<ArrivalMessage> GetLastArrivalMessageNotProcessed(int productId)
        {
            return _arrivalMessageRepository.GetLastArrivalMessageDateForProduct(productId);
        }

        public IList<ArrivalMessageViewModel> GetArrivalMessages(int productId, FilterObject filter, int offset, int count)
        {
            var arrivalMessages = _arrivalMessageRepository.GetFilteredItemsByProductId(productId, filter, offset, count);

            var model = arrivalMessages
                .Select(m => new ArrivalMessageViewModel
                {
                    ArrivalMessage = m,
                    EventsCount = m.EventsCount,//
                    SportsCount = m.SportsCount,//(m.Sports ?? "").Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Length,
                    LeaguesCount = m.LeaguesCount,//(m.Leagues ?? "").Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Length,
                    LocationsCount = m.LocationsCount,//(m.Locations ?? "").Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Length,
                    MarketsCount = m.MarketsCount,//(m.Markets ?? "").Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Length,
                    ProvidersCount = m.ProvidersCount,//(m.Providers ?? "").Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Length,
                    BetsCount = m.BetsCount,//(m.Bets ?? "").Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Length,
                    OpenBetsCount = m.BetsCount,//(m.Bets ?? "").Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Length,
                })
                .OrderByDescending(m => m.ArrivalMessage.CreatedOn)
                .ToList();

            return model;
        }


        public int GetCountOfArrivalMessages(int productId, FilterObject filter)
        {
            return _arrivalMessageRepository.GetCountOfFilteredItemsByProductId(productId, filter);
        }


        public ArrivalMessage CreateArrivalMessage(int productId, string pathToFile, string gateway, string sportId)
        {

            var arrivalMessage = new ArrivalMessage
            {
                Events = new List<string>(),
                Leagues = new List<string>(),
                Providers = new List<string>(),
                Locations = new List<string>(),
                Markets = new List<string>(),
                Statuses = new List<string>(),
                Sports = new List<string>(),
                Bets = new List<string>(),

                ProductId = productId,

                CreatedOn = DateTime.UtcNow,
                CreatedBy = "Admin",

                UpdatedBy = "Admin",
                UpdatedOn = DateTime.UtcNow,

                GroupId = sportId,

                Url = gateway,
                // Gateway = gateway,
                PathToXmlFile = pathToFile
            };

            return arrivalMessage;
        }

        public void BulkInsert(List<ArrivalMessage> messages, string groupId)
        {
            _arrivalMessageRepository.BulkInsert(messages, groupId);
        }
    }
}